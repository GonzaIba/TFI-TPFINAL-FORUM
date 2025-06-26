using Microsoft.AspNetCore.SignalR;
using CrossCutting.Extensions;
using CrossCutting.Helpers;
using Infrastructure.ML.Repositories;
using Core.Contracts.Services;

namespace ApiForums.Background
{
    public class TasksResolver : BackgroundService
    {
        private readonly ILogger<TasksResolver> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IBackgroundTasksQueue _backgroundTasksQueue;

        /// <summary>
        /// Marca de tiempo utilizada como condicion en la ejecucion de varios métodos
        /// </summary>
        private DateTime _lastExecution;

        public TasksResolver(ILogger<TasksResolver> logger, IServiceProvider serviceProvider, IBackgroundTasksQueue backgroundTasksQueue)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _backgroundTasksQueue = backgroundTasksQueue;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("BackgroundService Running");

            while (!stoppingToken.IsCancellationRequested)
            {
                var task = _backgroundTasksQueue.GetNextTask();

                if (task != null)
                {
                    HandleTask(task);
                }

                // Se ejecuta cada 5 minutos
                if (DateTime.Now.Minute % 5 == 0 && _lastExecution.Minute != DateTime.Now.Minute)
                {
                    _lastExecution = DateTime.Now;
                    var service = _serviceProvider.GetService<IEtiquetasPrediccionModeloService>();
                    await service?.TrainAndSaveLabelsAsync();
                }


                await Task.Delay(50);
            }
        }

        private void HandleTask(BackgroundTask task)
        {
            using var scope = _logger.BeginNamedScope(
                LoggerScope.BackgroundService.ToString(),
                ("BackgroundTask", task.ToJson())
            );

            //if (task is TimerTask timerTask)
            //{
            //    _logger.LogInformation($"Running a {nameof(TimerTask)}");
            //    RunTimerTask(timerTask);
            //}

            //if (task is DeviceTask deviceTask)
            //{
            //    _logger.LogInformation($"Running a {nameof(DeviceTask)}");
            //    RunDeviceTask(deviceTask);
            //}
        }       


        private void DeleteTemporalUsers()
        {
            _logger.LogInformation($"Executing {nameof(DeleteTemporalUsers)} at {DateTime.Now}");

            _ = Task.Run(async () =>
            {
                try
                {
                    //using var scope = _serviceProvider.CreateScope();

                    //var userService = scope.ServiceProvider.GetRequiredService<IUserService>();

                    //await userService.DeleteTemporalUsersAsync();

                    _logger.LogInformation($"{nameof(DeleteTemporalUsers)} ended at {DateTime.Now}");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An exception has occurred");
                    _logger.LogError(ex, $"{nameof(DeleteTemporalUsers)} failed at {DateTime.Now}");
                }
            });
        }

        private void FinalizeMeetings()
        {
            _logger.LogInformation($"Executing {nameof(FinalizeMeetings)} at {DateTime.Now}");

            _ = Task.Run(async () =>
            {
                try
                {
                    //using var scope = _serviceProvider.CreateScope();

                    //var meetingService = scope.ServiceProvider.GetRequiredService<IMeetingService>();

                    //await meetingService.FinalizeMeetings();

                    _logger.LogInformation($"{nameof(FinalizeMeetings)} ended at {DateTime.Now}");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An exception has occurred");
                    _logger.LogError(ex, $"{nameof(FinalizeMeetings)} failed at {DateTime.Now}");
                }
            });
        }
    }
}
