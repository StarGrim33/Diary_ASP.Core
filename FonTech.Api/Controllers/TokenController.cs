using FonTech.Application.Services;
using FonTech.Domain.Dto;
using FonTech.Domain.Interfaces.Services;
using FonTech.Domain.Result;
using Microsoft.AspNetCore.Mvc;

namespace FonTech.Api.Controllers
{
    /// <summary>
    /// Токен контроллер для авторизации и аутентификации в Web API
    /// </summary>
    public class TokenController : Controller
    {
        private readonly ITokenService _tokenService;

        public TokenController(ITokenService tokenService)
        {
            _tokenService = tokenService;
        }

        /// <summary>
        /// Обновить токен
        /// </summary>
        /// <param name="tokenDto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<BaseResult<TokenDto>>> RefreshToken([FromBody] TokenDto tokenDto)
        {
            var response = await _tokenService.RefreshToken(tokenDto);

            if (response.IsSuccess)
            {
                return Ok(response);
            }

            return BadRequest(response);
        }
    }
}
