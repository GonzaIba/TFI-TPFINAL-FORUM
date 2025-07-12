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

                //var filteredUsers = (await _repository.Get(
                //    filter: combinedSpecification,
                //    includeProperties: "UsersForum"
                //)).ToList();

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

                return userForum;
            }
            catch (Exception ex)
            {

                throw;
            }
        }
    }
    public static class HOLA
    {
        public static Expression<Func<T, bool>> Combine<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
        {
            var parameter = Expression.Parameter(typeof(T));

            var leftVisitor = new ReplaceExpressionVisitor(first.Parameters[0], parameter);
            var left = leftVisitor.Visit(first.Body);

            var rightVisitor = new ReplaceExpressionVisitor(second.Parameters[0], parameter);
            var right = rightVisitor.Visit(second.Body);

            return Expression.Lambda<Func<T, bool>>(Expression.AndAlso(left, right), parameter);
        }
    }

    class ReplaceExpressionVisitor : ExpressionVisitor
    {
        private readonly Expression _oldValue;
        private readonly Expression _newValue;

        public ReplaceExpressionVisitor(Expression oldValue, Expression newValue)
        {
            _oldValue = oldValue;
            _newValue = newValue;
        }

        public override Expression Visit(Expression node)
        {
            if (node == _oldValue)
                return _newValue;
            return base.Visit(node);
        }
    }
}
