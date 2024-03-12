using AutoMapper;
using FonTech.Application.Resources;
using FonTech.Domain.Dto;
using FonTech.Domain.Dto.User;
using FonTech.Domain.Entity;
using FonTech.Domain.Enum;
using FonTech.Domain.Interfaces.Repositories;
using FonTech.Domain.Interfaces.Services;
using FonTech.Domain.Result;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore.Storage;
using FonTech.Domain.Interfaces.Databases;


namespace FonTech.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBaseRepository<User> _userRepository;
        private readonly IBaseRepository<UserToken> _userTokenRepository;
        private readonly IBaseRepository<Role> _roleRepository;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;

        public AuthService(IBaseRepository<User> userRepository, IBaseRepository<UserToken> userTokenRepository, IMapper mapper,
            ITokenService tokenService, IUnitOfWork unitOfWork, IBaseRepository<Role> roleRepository)
        {
            _userRepository = userRepository;
            _userTokenRepository = userTokenRepository;
            _mapper = mapper;
            _tokenService = tokenService;
            _unitOfWork = unitOfWork;
            _roleRepository = roleRepository;
        }

        public async Task<BaseResult<TokenDto>> Login(LoginUserDto dto)
        {
            var user = await _userRepository.GetAll().AsNoTracking()
                .Include(x => x.Roles)
                .FirstOrDefaultAsync(x => x.Login == dto.Login);

            if (user == null)
            {
                return new BaseResult<TokenDto>()
                {
                    ErrorMessage = ErrorMessage.UserNotFound,
                    ErrorCode = (int)ErrorCode.UserNotFound,
                };
            }

            if (!IsVerifiedPassword(user.Password, dto.Password))
            {
                return new BaseResult<TokenDto>()
                {
                    ErrorMessage = ErrorMessage.WrongPassword,
                    ErrorCode = (int)ErrorCode.WrongPassword,
                };
            }

            var userToken = await _userTokenRepository.GetAll().AsNoTracking().FirstOrDefaultAsync(x => x.Id == user.Id);
            var userRoles = user.Roles;
            var claims = userRoles.ConvertAll(x => new Claim(ClaimTypes.Role, x.Name));
            claims.Add(new Claim(ClaimTypes.Name, user.Login));

            var accessToken = _tokenService.GenerateAccessToken(claims);
            var refreshToken = _tokenService.GenerateRefreshToken();

            if (userToken == null)
            {
                userToken = new UserToken()
                {
                    UserId = user.Id,
                    RefreshToken = refreshToken,
                    RefreshTokenExpireTime = DateTime.UtcNow.AddDays(7),
                };

                await _userTokenRepository.CreateAsync(userToken);
            }
            else
            {
                userToken.RefreshToken = refreshToken;
                userToken.RefreshTokenExpireTime = DateTime.UtcNow.AddDays(7);
                _userTokenRepository.Update(userToken);
            }

            return new BaseResult<TokenDto>()
            {
                Data = new TokenDto()
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                }
            };
        }

        public async Task<BaseResult<UserDto>> Register(RegisterUserDto dto)
        {
            if (dto.Password != dto.PasswordConfirm)
            {
                return new BaseResult<UserDto>()
                {
                    ErrorMessage = ErrorMessage.PasswordsNotEquals,
                    ErrorCode = (int)ErrorCode.PasswordsNotEquals,
                };
            }


            var user = await _userRepository.GetAll().FirstOrDefaultAsync(x => x.Login == dto.Login);

            if (user != null)
            {
                return new BaseResult<UserDto>()
                {
                    ErrorMessage = ErrorMessage.UserAlreadyExist,
                    ErrorCode = (int)ErrorCode.UserAlreadyExist,
                };
            }

            var hashUserPassword = HashPassword(dto.Password);
            
            await using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    user = new User()
                    {
                        Login = dto.Login,
                        Password = hashUserPassword,
                    };

                    await _unitOfWork.Users.CreateAsync(user);
                    await _unitOfWork.SaveChangesAsync();

                    var role = await _roleRepository.GetAll().AsNoTracking().FirstOrDefaultAsync(x => x.Name == nameof(UserRoles.User));

                    if (role == null)
                    {
                        return new BaseResult<UserDto>
                        {
                            ErrorMessage = ErrorMessage.RoleNotFound,
                            ErrorCode = (int)ErrorCode.RoleNotFound,
                        };
                    }

                    var userRole = new UserRole()
                    {
                        UserId = user.Id,
                        RoleId = role.Id,
                    };

                    await _unitOfWork.UserRoles.CreateAsync(userRole);

                    await transaction.CommitAsync();
                }
                catch (Exception)
                {

                    await transaction.RollbackAsync();
                }
            }

            return new BaseResult<UserDto>()
            {
                Data = _mapper.Map<UserDto>(user),
            };
        }

        private string HashPassword(string password)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }

        private bool IsVerifiedPassword(string userPasswordHash, string userPassword)
        {
            var hash = HashPassword(userPassword);
            return hash.Equals(userPasswordHash);
        }
    }
}
