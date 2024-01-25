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
using System.Security.Cryptography;
using System.Text;

namespace FonTech.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IBaseRepository<User> _userRepository;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;

        public AuthService(IBaseRepository<User> userRepository, ILogger logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        public Task<BaseResult<TokenDto>> Login(LoginUserDto dto)
        {

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

            try
            {
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

                user = new User()
                {
                    Login = dto.Login,
                    Password = HashPassword(dto.Password),
                };
                
                await _userRepository.CreateAsync(user);

                return new BaseResult<UserDto>()
                {
                    Data = _mapper.Map<UserDto>(user),
                };
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);

                return new BaseResult<UserDto>()
                {
                    ErrorMessage = ErrorMessage.InternalServerError,
                    ErrorCode = (int)ErrorCode.InternalServerError,
                };
            }
        }

        private string HashPassword(string password)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes).ToLower();
        }
    }
}
