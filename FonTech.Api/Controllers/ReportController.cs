using Asp.Versioning;
using FonTech.Domain.Dto.Report;
using FonTech.Domain.Interfaces.Services;
using FonTech.Domain.Result;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FonTech.Api.Controllers
{
    // [Authorize(Roles = "Admin")]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class ReportController : Controller
    {
        private readonly IReportService _reportService;

        public ReportController(IReportService reportService)
        {
            _reportService = reportService;
        }

        /// <summary>
        /// Получить один отчет пользователя по id отчета
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <remarks>
        /// Request for create report
        /// 
        ///     POST
        ///     {
        ///         "id": 1,
        ///     }
        ///     
        /// </remarks>
        /// <response code="200">Если отчет получен</response>
        /// <response code="400">Если отчет не был получен</response>
        [HttpGet(template: "{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<BaseResult<ReportDto>>> GetReport(long id)
        {
            var response = await _reportService.GetReportByIdAsync(id);

            if (response.IsSuccess)
            {
                return Ok(response);
            }

            return BadRequest(response);
        }

        /// <summary>
        /// Получение всех отчетов пользователя по его id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        /// <remarks>
        /// Request for create report
        /// 
        ///     POST
        ///     {
        ///         "userId": 1,
        ///     }
        ///     
        /// </remarks>
        /// <response code="200">Если отчеты получены</response>
        /// <response code="400">Если отчет не были получены</response>
        [HttpGet(template: "Reports/{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<BaseResult<ReportDto>>> GetUserReports(long userId)
        {
            var response = await _reportService.GetReportsAsync(userId);

            if (response.IsSuccess)
            {
                return Ok(response);
            }

            return BadRequest(response);
        }

        [HttpGet(template: "Reports/{date}/{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<BaseResult<ReportDto>>> GetUserReportsByDate(DateTime date, long userId)
        {
            var response = await _reportService.GetUserReportsByDateAsync(date, userId);

            if (response.IsSuccess)
            {
                return Ok(response);
            }

            return BadRequest(response);
        }

        /// <summary>
        /// Создание отчета
        /// </summary>
        /// <param name="dto"></param>
        /// <remarks>
        /// Request for create report
        /// 
        ///     POST
        ///     {
        ///         "name": "Report #1",
        ///         "description": Test report #1",
        ///         "userId": 1,
        ///     }
        ///     
        /// </remarks>
        /// <response code="200">Если отчет создался</response>
        /// <response code="400">Если отчет явно не был создан</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<BaseResult<ReportDto>>> Create([FromBody] CreateReportDto dto)
        {
            var response = await _reportService.CreateReportAsync(dto);

            if (response.IsSuccess)
            {
                return Ok(response);
            }

            return BadRequest(response);
        }

        /// <summary>
        /// Удаление отчета
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <remarks>
        /// Request for delete report
        /// 
        ///     POST
        ///     {
        ///         "id": 1,
        ///     }
        ///     
        /// </remarks>
        /// <response code="200">Если отчет удалился</response>
        /// <response code="400">Если отчет явно не был удален</response>
        [HttpDelete(template: "{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<BaseResult<ReportDto>>> Delete(long id)
        {
            var response = await _reportService.DeleteReportAsync(id);

            if (response.IsSuccess)
            {
                return Ok(response);
            }

            return BadRequest(response);
        }

        /// <summary>
        /// Обновление отчета
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        /// <remarks>
        /// Request for create report
        /// 
        ///     POST
        ///     {
        ///         "id": 1,
        ///         "name": "Report #1",
        ///         "description": Test report #1",
        ///     }
        ///     
        /// </remarks>
        /// <response code="200">Если отчет обновился</response>
        /// <response code="400">Если отчет явно не был обновлен</response>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<BaseResult<UpdateReportDto>>> Update([FromBody] UpdateReportDto dto)
        {
            var response = await _reportService.UpdateReportAsync(dto);

            if (response.IsSuccess)
            {
                return Ok(response);
            }

            return BadRequest(response);
        }
    }
}
