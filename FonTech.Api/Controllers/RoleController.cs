using FonTech.Domain.Dto.Report;
using FonTech.Domain.Dto.Role;
using FonTech.Domain.Entity;
using FonTech.Domain.Interfaces.Services;
using FonTech.Domain.Result;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace FonTech.Api.Controllers
{
    [Consumes(contentType: MediaTypeNames.Application.Json)]
    [ApiController]
    [Route(template: "api/[controller]")]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        /// <summary>
        /// Создание роли
        /// </summary>
        /// <param name="dto"></param>
        /// <remarks>
        /// Request for create role
        /// 
        ///     POST
        ///     {
        ///         "name": "Report #1",
        ///     }
        ///     
        /// </remarks>
        /// <response code="200">Если роль создалась</response>
        /// <response code="400">Если роль явно не была создана</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<BaseResult<RoleDto>>> Create([FromBody] CreateRoleDto role)
        {
            var response = await _roleService.CreateRoleAsync(role);

            if (response.IsSuccess)
            {
                return Ok(response);
            }

            return BadRequest(response);
        }

        /// <summary>
        /// Удаление роли
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <remarks>
        /// Request for delete role
        /// 
        ///     POST
        ///     {
        ///         "id": 1,
        ///     }
        ///     
        /// </remarks>
        /// <response code="200">Если роль удалилась</response>
        /// <response code="400">Если роль явно не была удалена</response>
        [HttpDelete(template: "{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<BaseResult<RoleDto>>> Delete(long id)
        {
            var response = await _roleService.DeleteRoleAsync(id);

            if (response.IsSuccess)
            {
                return Ok(response);
            }

            return BadRequest(response);
        }

        /// <summary>
        /// Обновление роли с указанием основных свойств
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        /// <remarks>
        /// Request for update role
        /// 
        ///     POST
        ///     {
        ///         "id": 1,
        ///         "name": "Admin #1",
        ///     }
        ///     
        /// </remarks>
        /// <response code="200">Если роль обновилась</response>
        /// <response code="400">Если роль явно не была обновлена</response>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<BaseResult<UpdateReportDto>>> Update([FromBody] RoleDto dto)
        {
            var response = await _roleService.UpdateRoleAsync(dto);

            if (response.IsSuccess)
            {
                return Ok(response);
            }

            return BadRequest(response);
        }

        /// <summary>
        /// Добавление роли пользователю
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        /// <remarks>
        /// Request for add role to user
        /// 
        ///     POST
        ///     {
        ///         "login": "User # 1",
        ///         "RoleName": "Admin #1",
        ///     }
        ///     
        /// </remarks>
        /// <response code="200">Если роль добавлена пользователю</response>
        /// <response code="400">Если роль не была добавлена</response>
        [HttpPost(template: "addRole")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<BaseResult<Role>>> AddRoleForUser([FromBody] UserRoleDto dto)
        {
            var response = await _roleService.AddRoleForUserAsync(dto);

            if (response.IsSuccess)
            {
                return Ok(response);
            }

            return BadRequest(response);
        }
    }
}
