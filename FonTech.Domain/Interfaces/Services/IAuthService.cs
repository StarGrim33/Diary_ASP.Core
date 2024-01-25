using FonTech.Domain.Dto;
using FonTech.Domain.Dto.User;
using FonTech.Domain.Result;

namespace FonTech.Domain.Interfaces.Services
{
    /// <summary>
    /// Сервис для авторизации и регистрации
    /// </summary>
    public interface IAuthService
    {
        /// <summary>
        /// Регистрация пользователя
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        Task<BaseResult<UserDto>> Register(RegisterUserDto dto);

        /// <summary>
        /// Авторизация пользователя
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        Task<BaseResult<TokenDto>> Login(LoginUserDto dto);
    }
}
