using Core.Contracts.Repositories;
using Core.Contracts.Services;
using Core.Domain.IdentityModels;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using System.Linq.Expressions;
using Core.Contracts.UoW;
using Core.Domain.Specification;
using CrossCutting.Extensions.Linq;
using Core.Domain.Models;
using Core.Domain.GenericEntityClass;
using Core.Domain.Exceptions.BaseException;
using Core.Domain.Request;

namespace Core.Business.Services
{
    public class UsersService : GenericService<Users>, IUsersService
    {
        private readonly IEmailService _emailService;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUnitOfWorkForum _unitOfWorkForum;
        private readonly IUnitOfWorkGateway _unitOfWorkGateway;

        public UsersService(
            IUnitOfWorkGateway unitOfWorkGateway,
            IUnitOfWorkForum unitOfWorkForum,
            IMapper mapper,
            IEmailService emailService,
            IHttpContextAccessor httpContextAccessor)
            : base(unitOfWorkGateway, unitOfWorkGateway.GetRepository<IUsersRepository>())
        {
            _emailService = emailService;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _unitOfWorkForum = unitOfWorkForum;
            _unitOfWorkGateway = unitOfWorkGateway;
        }      
        
        public async Task<List<Users>> GetUsersAsync()
        {
            return (await _repository.Get(tracking: false)).ToList();            
        }       
            
        public async Task<bool> UpdateUserAsync(Users user)
        {
            try
            {
                //get user
                var userDb = (await _repository.Get(x => x.Id == user.Id)).FirstOrDefault();
                userDb.Active = true;
                userDb.Email = user.Email;
                userDb.Nombre = user.Nombre;
                userDb.Apellido = user.Apellido;
                userDb.PhoneNumber = user.PhoneNumber;
                userDb.UserName = user.UserName;
                await _repository.Update(userDb);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }     
        }

        public async Task<Users> GetUserByNameAsync(string userName)
        {
            return (await _repository.Get(x => x.UserName == userName, tracking: false)).FirstOrDefault();
        }
        
        public async Task<Dictionary<Users,int>> GetTopLastWeekAsync()
        {
            var DictTopUsersLastWeek = new Dictionary<Users, int>();
            try
            {
                var totalUsers = await _repository.Get();
                //Obtenemos el repositorio de recompensas de usuarios
                var usuariosRecompensasRepo = _unitOfWorkForum.GetRepository<IRecompensaUsuarioRepository>();

                var topUsers = await usuariosRecompensasRepo.GetTopThreeUsersLastWeek();

                foreach (var a in topUsers)
                {
                    var user = (await _repository.Get(x => x.Id == a.IDUsuario, includeProperties: "UsersForum", tracking: false)).FirstOrDefault();
                    if (user is not null)
                        DictTopUsersLastWeek.Add(user, a.TotalRecompensa);
                }

                return DictTopUsersLastWeek;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<(PaginatedList<Users>, Dictionary<Users, int>)> GetUsersForumAsync(int pageIndex, int pageCount, string userId)
        {
            try
            {
                var userFiltersRepository = _unitOfWorkGateway.GetRepository<IUserFiltersRepository>();
                var userFilters = (await userFiltersRepository.Get(x => x.UserId == userId, includeProperties: "Filter")).ToList();

                Specification<Users> combinedSpecification = new AdHocSpecification<Users>(user => true);

                foreach (var userFilter in userFilters)
                {
                    var newSpec = new AdHocSpecification<Users>(ExpressionExtensions.CreateContainsExpression<Users>(userFilter.Filter.Description, userFilter.Value));
                    combinedSpecification &= newSpec;
                }

                var paged = await _repository.GetPagedElements(
                    pageIndex,
                    pageCount,
                    orderByExpression: p => p.Nombre, 
                    ascending: false,
                    combinedSpecification,
                    includeProperties: "UsersForum",
                    tracking: false
                );

                Dictionary<Users, int> dicUsers = new Dictionary<Users, int>();

                var users = paged.List.Distinct().ToList();
                users.ForEach(async x =>
                {
                    Specification<RecompensaUsuarioModel> regardUser = new AdHocSpecification<RecompensaUsuarioModel>(reg => reg.IDUsuario == x.Id);
                    var recompensaUsuario = (await _unitOfWorkForum.GetRepository<IRecompensaUsuarioRepository>().Get(regardUser,includeProperties: "Recompensa")).Sum(x => x.Recompensa.Valor);
                    dicUsers.Add(x, recompensaUsuario);
                });

                return (paged,dicUsers);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<Users> GetDetailUserAsync(string userEmail)
        {
            try
            {
                var userForum = (await _repository.Get(x=> x.Email == userEmail, includeProperties: "UsersForum")).FirstOrDefault();

                if(userForum is not null)
                {
                    userForum.RecompensasUsuarios = (await _unitOfWorkForum.GetRepository<IRecompensaUsuarioRepository>().Get(x => x.IDUsuario == userForum.Id)).ToList();
                    userForum.UsuarioMedallas = (await _unitOfWorkForum.GetRepository<IUsuarioMedallaRepository>().Get(x => x.IDUsuario == userForum.Id, includeProperties: "Medalla")).ToList();
                    userForum.Respuestas = (await _unitOfWorkForum.GetRepository<IRespuestaRepository>().Get(x => x.IDUsuario == userForum.Id)).ToList();
                    userForum.Publicaciones = (await _unitOfWorkForum.GetRepository<IPublicacionRepository>().Get(x => x.IDUsuario == userForum.Id)).ToList();
                }

                return userForum; ////////////////////////////////
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<List<NotificacionesModel>> GetNotificationsAsync(string userId)
        {
            try
            {
                var user = (await _repository.Get(x => x.Id == userId, includeProperties: "UsersForum")).FirstOrDefault();

                if (user is null)
                    throw new ApiForumException("El usuario no existe.");
                else
                {
                    return (await _unitOfWorkForum.GetRepository<INotificacionRepository>().Get(x => x.IDUsuario == userId, orderBy: y=> y.OrderByDescending(u=> u.FechaNotificacion)))?.ToList() ?? new List<NotificacionesModel>();
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<bool> MarkNotificationAsReadAsync(MarkNotificationAsReadRequest request)
        {
            try
            {
                var user = (await _repository.Get(x => x.Id == request.UserId, includeProperties: "UsersForum")).FirstOrDefault();

                if (user is null)
                    throw new ApiForumException("El usuario no existe.");
                else
                {
                    var repo = _unitOfWorkForum.GetRepository<INotificacionRepository>();
                    var notification = (await repo.Get(x => x.IDNotificacion == request.CodeNotification && x.IDUsuario == request.UserId)).FirstOrDefault();
                    if (notification is null)
                        return false;
                    else
                    {
                        notification.Leida = true;
                        await repo.Update(notification);
                        return await _unitOfWorkForum.SaveChangesAsync() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
