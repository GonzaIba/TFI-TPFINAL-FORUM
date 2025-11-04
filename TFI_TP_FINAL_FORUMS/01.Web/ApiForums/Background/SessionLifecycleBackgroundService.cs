using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Contracts.Publishers;
using Core.Contracts.Repositories;
using Core.Contracts.UoW;
using Core.Domain.Enum;
using Core.Domain.Models;
using Infrastructure.Data.SQL;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ApiForums.Background;

public sealed class SessionLifecycleBackgroundService : BackgroundService
{
    private static readonly TimeSpan PollInterval = TimeSpan.FromSeconds(60);
    private const int BatchSize = 100;

    private readonly IServiceScopeFactory _scopeFactory;
    private readonly TimeProvider _timeProvider;
    private readonly ILogger<SessionLifecycleBackgroundService> _logger;

    public SessionLifecycleBackgroundService(
        IServiceScopeFactory scopeFactory,
        TimeProvider timeProvider,
        ILogger<SessionLifecycleBackgroundService> logger)
    {
        _scopeFactory = scopeFactory;
        _timeProvider = timeProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("{Service} iniciado", nameof(SessionLifecycleBackgroundService));

        while (!stoppingToken.IsCancellationRequested)
        {
            var iterationStart = _timeProvider.GetUtcNow();

            try
            {
                await ProcessExpiredHelpRequestsAsync(stoppingToken);
                await ProcessFinishedSessionsAsync(stoppingToken);
            }
            catch (Exception ex) when (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogError(ex, "Error al procesar expiración/cierre de solicitudes y sesiones");
            }

            var elapsed = _timeProvider.GetUtcNow() - iterationStart;
            var delay = PollInterval - elapsed;
            if (delay < TimeSpan.FromMilliseconds(100))
            {
                delay = PollInterval;
            }

            try
            {
                await Task.Delay(delay, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
        }

        _logger.LogInformation("{Service} detenido", nameof(SessionLifecycleBackgroundService));
    }

    private async Task ProcessExpiredHelpRequestsAsync(CancellationToken cancellationToken)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var unitOfWorkForum = scope.ServiceProvider.GetRequiredService<IUnitOfWorkForum>();
        var publisherNotification = scope.ServiceProvider.GetRequiredService<IPublisherNotification>();
        var notificationRepository = unitOfWorkForum.GetRepository<INotificacionRepository>();
        var estadoRepository = unitOfWorkForum.GetRepository<ISolicitudAyudaEstadoRepository>();

        var estados = (await estadoRepository
                .Get(x =>
                        x.Estado == RequestHelpStateEnum.Activa.ToString()
                     || x.Estado == RequestHelpStateEnum.Reservada.ToString()
                     || x.Estado == RequestHelpStateEnum.Expirada.ToString(),
                     tracking: true))
            .ToDictionary(s => s.Estado, s => s);

        if (!estados.TryGetValue(RequestHelpStateEnum.Expirada.ToString(), out var estadoExpirada) ||
            !estados.TryGetValue(RequestHelpStateEnum.Activa.ToString(), out var estadoActiva) ||
            !estados.TryGetValue(RequestHelpStateEnum.Reservada.ToString(), out var estadoReservada))
        {
            _logger.LogWarning("No se encontraron todos los estados necesarios para expirar solicitudes");
            return;
        }

        var estadosVigentes = new[] { estadoActiva.IDEstado, estadoReservada.IDEstado };
        var now = _timeProvider.GetUtcNow().UtcDateTime;

        while (true)
        {
            var solicitudes = await dbContext.Set<SolicitudAyudaModel>()
                .Where(r => estadosVigentes.Contains(r.IDEstado) && r.FechaVencimiento <= now)
                .OrderBy(r => r.FechaVencimiento)
                .Take(BatchSize)
                .AsTracking()
                .ToListAsync(cancellationToken);

            if (solicitudes.Count == 0)
            {
                break;
            }

            var notifications = new List<NotificacionesModel>(solicitudes.Count);

            foreach (var solicitud in solicitudes)
            {
                solicitud.IDEstado = estadoExpirada.IDEstado;
                solicitud.UpdateDate = now;

                var message = $"Tu solicitud de ayuda \"{solicitud.Titulo}\" ha expirado.";
                var notification = new NotificacionesModel
                {
                    IDUsuario = solicitud.IDUsuarioSolicitante,
                    Mensaje = message,
                    FechaNotificacion = now,
                    Leida = false
                };

                await notificationRepository.Insert(notification);
                notifications.Add(notification);
            }

            var saved = false;
            try
            {
                await unitOfWorkForum.SaveChangesAsync(cancellationToken);
                saved = true;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogWarning(ex, "Conflicto de concurrencia al expirar solicitudes");
                dbContext.ChangeTracker.Clear();
            }

            if (!saved)
            {
                continue;
            }

            foreach (var notification in notifications)
            {
                await publisherNotification.AddNotification(
                    notification.IDNotificacion,
                    notification.IDUsuario,
                    notification.Mensaje,
                    notification.FechaNotificacion,
                    notification.Leida);
            }

            _logger.LogInformation("Solicitudes expiradas: {Count}", notifications.Count);
        }
    }

    private async Task ProcessFinishedSessionsAsync(CancellationToken cancellationToken)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var unitOfWorkForum = scope.ServiceProvider.GetRequiredService<IUnitOfWorkForum>();
        var publisherNotification = scope.ServiceProvider.GetRequiredService<IPublisherNotification>();
        var notificationRepository = unitOfWorkForum.GetRepository<INotificacionRepository>();
        var estadoRepository = unitOfWorkForum.GetRepository<ISesionAyudaEstadoRepository>();

        var estados = (await estadoRepository
                .Get(x =>
                        x.Estado == SessionStateEnum.Pendiente.ToString()
                     || x.Estado == SessionStateEnum.Iniciada.ToString()
                     || x.Estado == SessionStateEnum.Finalizada.ToString(),
                     tracking: true))
            .ToDictionary(s => s.Estado, s => s);

        if (!estados.TryGetValue(SessionStateEnum.Finalizada.ToString(), out var estadoFinalizada))
        {
            _logger.LogWarning("No se encontró el estado Finalizada para las sesiones");
            return;
        }

        var estadosPendientes = estados
            .Where(kvp => kvp.Key is not null
                       && (kvp.Key == SessionStateEnum.Pendiente.ToString()
                        || kvp.Key == SessionStateEnum.Iniciada.ToString()))
            .Select(kvp => kvp.Value.IDEstado)
            .ToArray();

        if (estadosPendientes.Length == 0)
        {
            _logger.LogDebug("No hay estados de sesión pendientes configurados");
            return;
        }

        var now = _timeProvider.GetUtcNow().UtcDateTime;

        while (true)
        {
            var sesiones = await dbContext.Set<SesionAyudaModel>()
                .Include(s => s.Reserva)
                    .ThenInclude(r => r.Disponibilidad)
                        .ThenInclude(d => d.Solicitud)
                .Include(s => s.TerminosCondiciones)
                .Where(s => estadosPendientes.Contains(s.IDEstado) && s.Fin <= now)
                .OrderBy(s => s.Fin)
                .Take(BatchSize)
                .AsTracking()
                .ToListAsync(cancellationToken);

            if (sesiones.Count == 0)
            {
                break;
            }

            var notifications = new List<NotificacionesModel>();

            foreach (var sesion in sesiones)
            {
                sesion.IDEstado = estadoFinalizada.IDEstado;
                sesion.UpdateDate = now;

                var solicitud = sesion.Reserva?.Disponibilidad?.Solicitud;
                var sessionTitle = string.IsNullOrWhiteSpace(solicitud?.Titulo)
                    ? $"Sesión {sesion.IDSesion:D}"
                    : solicitud.Titulo;

                var participantes = CollectParticipants(sesion);
                if (participantes.Count == 0)
                {
                    continue;
                }

                var asistentes = CollectAttendees(sesion);
                var hadAttendance = participantes.Any(asistentes.Contains);

                var message = hadAttendance
                    ? $"La sesión de ayuda \"{sessionTitle}\" ha finalizado."
                    : $"La sesión de ayuda \"{sessionTitle}\" finalizó sin participantes.";

                foreach (var participante in participantes)
                {
                    var notification = new NotificacionesModel
                    {
                        IDUsuario = participante,
                        Mensaje = message,
                        FechaNotificacion = now,
                        Leida = false
                    };

                    await notificationRepository.Insert(notification);
                    notifications.Add(notification);
                }
            }

            var saved = false;
            try
            {
                await unitOfWorkForum.SaveChangesAsync(cancellationToken);
                saved = true;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogWarning(ex, "Conflicto de concurrencia al finalizar sesiones");
                dbContext.ChangeTracker.Clear();
            }

            if (!saved)
            {
                continue;
            }

            foreach (var notification in notifications)
            {
                await publisherNotification.AddNotification(
                    notification.IDNotificacion,
                    notification.IDUsuario,
                    notification.Mensaje,
                    notification.FechaNotificacion,
                    notification.Leida);
            }

            _logger.LogInformation("Sesiones finalizadas procesadas: {Count}", sesiones.Count);
        }
    }

    private static HashSet<string> CollectParticipants(SesionAyudaModel sesion)
    {
        var participants = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        var solicitante = sesion.Reserva?.Disponibilidad?.Solicitud?.IDUsuarioSolicitante;
        if (!string.IsNullOrWhiteSpace(solicitante))
        {
            participants.Add(solicitante);
        }

        var ayudante = sesion.Reserva?.IDUsuarioAyudante;
        if (!string.IsNullOrWhiteSpace(ayudante))
        {
            participants.Add(ayudante);
        }

        return participants;
    }

    private static HashSet<string> CollectAttendees(SesionAyudaModel sesion)
    {
        if (sesion.TerminosCondiciones is null || sesion.TerminosCondiciones.Count == 0)
        {
            return new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        }

        return sesion.TerminosCondiciones
            .Where(t => t.Aceptado)
            .Select(t => t.UserId)
            .Where(id => !string.IsNullOrWhiteSpace(id))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
    }
}
