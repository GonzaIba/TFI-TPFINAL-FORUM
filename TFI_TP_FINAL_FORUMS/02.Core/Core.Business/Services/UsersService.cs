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
using CrossCutting.Helpers.JWT;
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

namespace Core.Business.Services
{
    public class UsersService : GenericService<Users>, IUsersService
    {
        private readonly UserManager<Users> _userManager;
        private readonly SignInManager<Users> _signInManager;

        private readonly IJwtBearerTokenHelper _jwtBearerTokenHelper;
        private readonly ITokenGenerator _refreshTokenFactory;
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly IEmailService _emailService;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public UsersService(
            IUnitOfWork unitOfWork,
            UserManager<Users> userManager,
            IJwtBearerTokenHelper jwtBearerTokenHelper,
            ITokenGenerator tokenGenerator,
            IRefreshTokenService refreshTokenService,
            IMapper mapper,
            IEmailService emailService,
            SignInManager<Users> signInManager,
            IHttpContextAccessor httpContextAccessor)
            : base(unitOfWork, unitOfWork.GetRepository<IUsersRepository>())
        {
            _userManager = userManager;
            _jwtBearerTokenHelper = jwtBearerTokenHelper;
            _refreshTokenFactory = tokenGenerator;
            _refreshTokenService = refreshTokenService;
            _emailService = emailService;
            _mapper = mapper;
            _signInManager = signInManager;
            _httpContextAccessor = httpContextAccessor;
        }
        
        public async Task<IGenericResult<RegisterDto>> CreateUserAsync(Users user, string password)
        {
            RegisterDto registerDto = new RegisterDto();
            GenericResult<RegisterDto> usuarioDto = new GenericResult<RegisterDto>();
            try
            {
                var result = await _userManager.CreateAsync(user, password);
                usuarioDto = _mapper.Map<GenericResult<RegisterDto>>(result);
                if (!result.Succeeded)
                {
                    //string Errores = string.Join("\n", result.Errors.Select(p => p.Description));
                    return usuarioDto;
                }
                var role = PrivilegeEnum.User.ToString();
                result = await _userManager.AddToRoleAsync(user, role.ToUpper());
                if (!result.Succeeded)
                {
                    await _userManager.DeleteAsync(await _userManager.FindByNameAsync(user.UserName));
                    usuarioDto.Data = registerDto;
                    return usuarioDto;
                }
                
                await _emailService.RegistrationEmailAsync(user);
                registerDto.IsRegistred = true;
                usuarioDto.Data = registerDto;
                return usuarioDto;
            }
            catch (InvalidOperationException ex) when (ex is Exception) //Si cae a esta exepción es porque no existe el rol user en la base...
            {
                await _userManager.DeleteAsync(user);
                usuarioDto.Data.IsRegistred = false;
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> DeleteUserAsync(string id)
        {
            try
            {
                var user = (await _repository.Get(x => x.Id == id)).FirstOrDefault();
                if(user== null)
                {
                    return false;
                }
                user.Active = false;
                await _repository.Update(user);
                var userPrivilegesRepository = _unitOfWork.GetRepository<IUsersRolesRepository>();
                var relations = (await userPrivilegesRepository.Get(x => x.UserId == user.Id)).ToList();
                relations.ForEach(x =>
                {
                    userPrivilegesRepository.Delete(x);
                });
                await _unitOfWork.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<Users>> GetUsersAsync()
        {
            return (await _repository.Get(tracking: false)).ToList();            
        }       
        

        public async Task<IGenericResult<LoginTokenDto>> LoginUserAsync(string email, string password)
        {
            var identityUser = await _userManager.FindByEmailAsync(email);
            
            if (identityUser is null) return new GenericResult<LoginTokenDto>("UserNotExist");
            
            if (identityUser.EmailConfirmed /*&& identityUser.Active //Mas adelante agregar active*/)
            {
                var result = await _userManager.CheckPasswordAsync(identityUser, password);
                return result ? await GenerateLoginToken(identityUser) : new GenericResult<LoginTokenDto>("InvalidCredentials");
            }
            else
                return new GenericResult<LoginTokenDto>("UserNotConfirmed");    
        }

        public async Task<IGenericResult<LoginTokenDto>> ExternalLoginUserAsync(ExternalLoginInfo info)
        {
            // If the user already has a login (i.e if there is a record in AspNetUserLogins
            // table) then sign-in the user with this external login provider
            var signInResult = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider,
                info.ProviderKey, isPersistent: false, bypassTwoFactor: true);

            if (signInResult.Succeeded)
            {

                #region test youtube
                //// Crea la instancia del servicio de YouTube
                //YouTubeService youtubeService = new YouTubeService(new BaseClientService.Initializer()
                //{
                //    ApiKey = "AIzaSyD_Lo6OQcB8JfJiStl2xPKJTfSnvjmRvFQ",
                //    ApplicationName = "TFI-TPFINAL-WEB"
                //});

                ////Obtener información del canal del usuario autenticado
                //var channelsListRequest = youtubeService.Channels.List("snippet");
                //channelsListRequest.Mine = true;
                //var channelsListResponse = channelsListRequest.Execute();

                //// Obtener la información del canal
                //var channel = channelsListResponse.Items.FirstOrDefault();
                #endregion


                var userSucceeded = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);
                if(userSucceeded == null)
                    return new GenericResult<LoginTokenDto>("UserNotExists");

                return await GenerateLoginToken(userSucceeded);
            }
            // If there is no record in AspNetUserLogins table, the user may not have
            // a local account
            else
            {
                // Get the email claim value
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);

                if (email != null)
                {
                    // Create a new user without password if we do not have a user already
                    var user = await _userManager.FindByEmailAsync(email);

                    if (user == null)
                    {
                        user = new Users
                        {
                            UserName = info.Principal.FindFirstValue(ClaimTypes.Email),
                            Email = info.Principal.FindFirstValue(ClaimTypes.Email),
                            FirstName = info.Principal.FindFirstValue(ClaimTypes.Name) ?? "",
                            LastName = info.Principal.FindFirstValue(ClaimTypes.Surname) ?? "",
                            EmailConfirmed = true,
                            Active = true
                        };

                        await _userManager.CreateAsync(user);
                        var role = PrivilegeEnum.User.ToString();
                        await _userManager.AddToRoleAsync(user, role.ToUpper());
                    }

                    // Add a login (i.e insert a row for the user in AspNetUserLogins table)
                    await _userManager.AddLoginAsync(user, info);
                    await _signInManager.SignInAsync(user, isPersistent: false);

                    return await GenerateLoginToken(user);
                }

                // If we cannot find the user email we cannot continue
                //ViewBag.ErrorTitle = $"Email claim not received from: {info.LoginProvider}";
                //ViewBag.ErrorMessage = "Please contact support on Pragim@PragimTech.com";

                return new GenericResult<LoginTokenDto>("Error");
            }
        }

        public async Task<IdentityResult> ConfirmUserEmailAsync(string userId, string token)
        {
            var identityUser = await _userManager.FindByIdAsync(userId);
            return await _userManager.ConfirmEmailAsync(identityUser ?? throw new Exception("Ocurrió un error al confirmar el usuario"), token);
        }

        public async Task ForgotPasswordGenerateToken(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null || !user.EmailConfirmed /*|| !user.Active*/)
            {
                throw new Exception("El usuario no existe o su email no está confirmado");
            }

            await _emailService.ForgotPasswordSendEmail(user);
        }

        public async Task ChangePasswordGenerateToken(ChangePasswordDto changePasswordDto)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(changePasswordDto.UserId);
                if (user == null)
                {
                    throw new Exception("UserNotExist");
                }

                var result = await _userManager.ResetPasswordAsync(user, changePasswordDto.Token, changePasswordDto.Password);
                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.ToString("\n"));
                }

                //await _emailService.SendPasswordChangeConfirmationEmailAsync(user);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public async Task<LoginTokenDto> GenerateRefreshToken(Users user, string token)
        {
            LoginTokenDto loginTokenDto = new LoginTokenDto();

            try
            {
                // Busco al usuario en base a los claims del bearer token
                
                if (user == null)
                {
                    throw new Exception($"Invalid bearer token claims.");
                }

                var role = _userManager.GetRolesAsync(user).Result.First();

                var newBearerToken = _jwtBearerTokenHelper.CreateJwtToken(user.Id, user.UserName, new List<string>{ role});
                var refreshToken = _refreshTokenFactory.GenerateToken();

                var replaceResult = await _refreshTokenService.ReplaceAsync(user.Id, token, refreshToken);
                if (!replaceResult.Success)
                {
                    throw new Exception(replaceResult.Errors.ToString("\n"));
                }

                return new LoginTokenDto
                {
                    Token = newBearerToken,
                    RefreshToken = refreshToken,
                    ValidFrom = _jwtBearerTokenHelper.GetValidFromDate(newBearerToken),
                    ExpirationDate = _jwtBearerTokenHelper.GetExpirationDate(newBearerToken)
                };

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        public async Task<bool> UpdateUserAsync(Users user)
        {
            try
            {
                //get user
                var userDb = (await _repository.Get(x => x.Id == user.Id)).FirstOrDefault();
                userDb.Active = true;
                userDb.Email = user.Email;
                userDb.FirstName = user.FirstName;
                userDb.LastName = user.LastName;
                userDb.PhoneNumber = user.PhoneNumber;
                userDb.UserName = user.UserName;
                var result = await _userManager.UpdateAsync(userDb);

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
            return await _userManager.FindByNameAsync(userName);
        }

        #region Helpers
        private async Task<IGenericResult<LoginTokenDto>> GenerateLoginToken(Users user)
        {
            var result = new GenericResult<LoginTokenDto>();

            var role = await _userManager.GetRolesAsync(user);
            var roleBase = role.ToList();
            bool IsAdmin = false;
            roleBase.ForEach(x =>
            {
                if (!IsAdmin)
                    IsAdmin = x.ToLower() == "admin" ? true : false;
            }
            );


            var bearerToken = _jwtBearerTokenHelper.CreateJwtToken(user.Id, user.UserName, role.ToList());
            if (bearerToken is null)
            {
                result.AddError("TokenError");
                return result;
            }

            var refreshToken = _refreshTokenFactory.GenerateToken();
            var creationResult = await _refreshTokenService.CreateAsync(user.Id, refreshToken);

            if (!creationResult.Success)
            {
                result.Issues = creationResult.Errors;
            }
            //transform role tolowwer all items


            var response = new LoginTokenDto
            {
                Token = bearerToken,
                RefreshToken = creationResult.Success ? refreshToken : null,
                ValidFrom = _jwtBearerTokenHelper.GetValidFromDate(bearerToken),
                ExpirationDate = _jwtBearerTokenHelper.GetExpirationDate(bearerToken),
                UserName = user.UserName,
                Email = user.Email ?? "",
                RoleName = IsAdmin ? "Admin" : role?.FirstOrDefault() ?? "User"
            };

            result.Data = response;
            return result;
        }
        #endregion
    }
}
