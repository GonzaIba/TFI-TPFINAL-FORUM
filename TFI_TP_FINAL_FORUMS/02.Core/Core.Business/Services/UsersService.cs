using Core.Contracts.Repositories;
using Core.Contracts.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CrossCutting.Helpers.ResultClasses;
using Core.Domain.Enum;
using Core.Domain.IdentityModels;
using Core.Domain.DTOs;
using AutoMapper;
using CrossCutting.Extensions;
using System.Security.Claims;
using System.Collections;
using Google.Apis.Auth.OAuth2;
using Google.Apis.YouTube.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Core.Domain.Models;
using System.Linq.Expressions;
using Core.Contracts.UoW;

namespace Core.Business.Services
{
    public class UsersService : GenericService<Users>, IUsersService
    {
        private readonly IEmailService _emailService;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUnitOfWorkForum _unitOfWorkForum;

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
                throw ex;
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

                //Agarramos la fecha de hace 1 semana
                var oneWeekAgo = DateTime.Now.AddDays(-7);

                //Obtenemos los usuarios con mas recompensas de la ultima semana
                //var groupedUsers = await usuariosRecompensasRepo.GetWithGroupBy(
                //    x => x.FechaObtencion >= oneWeekAgo,
                //    null,
                //    query => query
                //        .GroupBy(x => x.IDUsuario)
                //        .Select(g => new
                //        {
                //            User = g.FirstOrDefault().Usuario,
                //            TotalRecompensa = g.Sum(x => x.CantidadRecompensa)
                //        }),
                //    "Usuario",
                //    tracking: false
                //);

                ////Ordenamos Descendentemente y tomamos tambien su recompensa
                //var topUsersDict = groupedUsers.OrderByDescending(x => x.TotalRecompensa)
                //                               .ToDictionary(k => k.User, v => v.TotalRecompensa);

                //return topUsersDict;

                var groupedUsers = await usuariosRecompensasRepo.GetWithGroupBy(
                    x => x.FechaObtencion >= oneWeekAgo,
                    null,
                    query => query
                        .GroupBy(x => x.IDUsuario)
                        .Select(g => new
                        {
                            IdUser = g.FirstOrDefault().IDUsuario,
                            TotalRecompensa = g.Sum(x => x.CantidadRecompensa)
                        }),
                    tracking: false
                );

                foreach (var a in groupedUsers.OrderByDescending(x => x.TotalRecompensa))
                {
                    var user = (await _repository.Get(x => x.Id == a.IdUser, includeProperties: "UsersForum", tracking: false)).FirstOrDefault();
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

        public async Task<IEnumerable<Users>> GetUsersForumAsync()
        {
            try
            {
                var usersForum = (await _repository.Get(includeProperties: "UsersForum")).ToList();

                foreach (var user in usersForum)
                {
                    user.RecompensasUsuarios = (await _unitOfWorkForum.GetRepository<IRecompensaUsuarioRepository>().Get(x => x.IDUsuario == user.Id)).ToList();
                }

                return usersForum;
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
}
