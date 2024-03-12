using FonTech.Domain.Dto.Role;
using FonTech.Domain.Dto.UserRole;
using FonTech.Domain.Result;
using UserRoleDto = FonTech.Domain.Dto.UserRole.UserRoleDto;

namespace FonTech.Domain.Interfaces.Services
{
    /// <summary>
    /// Сервис, предназначенный для управления ролями пользователей (CRUD)
    /// </summary>
    public interface IRoleService
    {
        Task<BaseResult<RoleDto>> CreateRoleAsync(CreateRoleDto dto);

        Task<BaseResult<RoleDto>> DeleteRoleAsync(long id);

        /// <summary>
        /// Обновление роли
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        Task<BaseResult<RoleDto>> UpdateRoleAsync(RoleDto dto);
        
        /// <summary>
        /// Обновление роли для пользователя
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        Task<BaseResult<UserRoleDto>> UpdateUserRoleAsync(UpdateUserRoleDto dto);

        /// <summary>
        /// Добавление роли для пользователя
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        Task<BaseResult<UserRoleDto>> AddRoleForUserAsync(UserRoleDto dto);

        /// <summary>
        /// Удаление роли у пользователя
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        Task<BaseResult<UserRoleDto>> DeleteRoleForUserAsync(DeleteUserRoleDto dto);
    }
}
