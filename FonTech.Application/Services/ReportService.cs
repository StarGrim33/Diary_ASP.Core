using AutoMapper;
using FonTech.Application.Resources;
using FonTech.Domain.Dto.Report;
using FonTech.Domain.Entity;
using FonTech.Domain.Enum;
using FonTech.Domain.Interfaces.Repositories;
using FonTech.Domain.Interfaces.Services;
using FonTech.Domain.Interfaces.Validations;
using FonTech.Domain.Result;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Serilog;

namespace FonTech.Application.Services
{
    public class ReportService : IReportService
    {
        private readonly IBaseRepository<Report> _reportRepository;
        private readonly IBaseRepository<User> _userRepository;
        private readonly IReportValidator _reportValidator;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _memoryCache;

        public ReportService(IBaseRepository<Report> reportRepository, IReportValidator reportValidator,
            IBaseRepository<User> userRepository, IMapper mapper, ILogger logger, IMemoryCache memoryCache)
        {
            _reportRepository = reportRepository;
            _userRepository = userRepository;
            _reportValidator = reportValidator;
            _mapper = mapper;
            _logger = logger;
            _memoryCache = memoryCache;
        }

        /// <inheritdoc />
        public async Task<CollectionResult<ReportDto>> GetReportsAsync(long userId)
        {
            ReportDto[] reports;

            try
            {
                if (_memoryCache.TryGetValue(userId, out ReportDto[]? result))
                {
                    reports = result;
                }
                else
                {
                    reports = await _reportRepository.GetAll().Where(x => x.UserId == userId). // Фильтрую по userId
                        Select(x => new ReportDto(x.Id, x.Name, x.Description, x.CreatedAt.ToLongDateString())).ToArrayAsync(); // Формирую ReportDto
                    _memoryCache.Set(userId, reports, TimeSpan.FromMinutes(10));
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);

                return new CollectionResult<ReportDto>()
                {
                    ErrorMessage = ErrorMessage.InternalServerError,
                    ErrorCode = (int)ErrorCode.InternalServerError,
                };
            }

            if (reports.Length == 0)
            {
                _logger.Warning(ErrorMessage.ReportsNotFound, reports.Length);

                return new CollectionResult<ReportDto>()
                {
                    ErrorMessage = ErrorMessage.ReportsNotFound,
                    ErrorCode = (int)ErrorCode.ReportsNotFound,
                };
            }

            return new CollectionResult<ReportDto>()
            {
                Data = reports,
                Count = reports.Length
            };
        }

        /// <inheritdoc />
        public Task<BaseResult<ReportDto>> GetReportByIdAsync(long id)
        {
            ReportDto? report;

            try
            {
                if (_memoryCache.TryGetValue(id, out ReportDto result))
                {
                    report = result;
                }
                else
                {
                    report = _reportRepository.GetAll().AsEnumerable().Select(x => 
                    new ReportDto(x.Id, x.Name, x.Description, x.CreatedAt.ToLongDateString())).FirstOrDefault(x => x.Id == id);
                    _memoryCache.Set(id, report, TimeSpan.FromMinutes(10));
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);

                return Task.FromResult(new BaseResult<ReportDto>()
                {
                    ErrorMessage = ErrorMessage.InternalServerError,
                    ErrorCode = (int)ErrorCode.InternalServerError,
                });
            }

            if (report == null)
            {
                _logger.Warning(ErrorMessage.ReportNotFound);

                return Task.FromResult(new BaseResult<ReportDto>()
                {
                    ErrorMessage = ErrorMessage.ReportNotFound,
                    ErrorCode = (int)ErrorCode.ReportNotFound,
                });
            }

            return Task.FromResult(new BaseResult<ReportDto>()
            {
                Data = report,
            });
        }

        /// <inheritdoc />
        public async Task<BaseResult> CreateReportAsync(CreateReportDto dto)
        {
            try
            {
                var user = await _userRepository.GetAll().FirstOrDefaultAsync(x => x.Id == dto.UserId);
                var report = await _reportRepository.GetAll().FirstOrDefaultAsync(x => x.Name == dto.Name);
                var result = _reportValidator.CreateValidator(report: report, user: user);

                if (!result.IsSuccess)
                {
                    return new BaseResult<ReportDto>()
                    {
                        ErrorMessage = result.ErrorMessage,
                        ErrorCode = result.ErrorCode,
                    };
                }

                report = new Report()
                {
                    Name = dto.Name,
                    Description = dto.Description,
                    UserId = user.Id,
                };

                await _reportRepository.CreateAsync(report);

                return new BaseResult<ReportDto>()
                {
                    Data = _mapper.Map<ReportDto>(report),
                };
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);

                return new BaseResult<ReportDto>
                {
                    ErrorMessage = ErrorMessage.InternalServerError,
                    ErrorCode = (int)ErrorCode.InternalServerError,
                };
            }
        }

        /// <inheritdoc />
        public async Task<BaseResult<ReportDto>> DeleteReportAsync(long id)
        {
            try
            {
                var report = await _reportRepository.GetAll().FirstOrDefaultAsync(x => x.Id == id);
                var result = _reportValidator.ValidateOnNull(report);

                if (!result.IsSuccess)
                {
                    return new BaseResult<ReportDto>()
                    {
                        ErrorMessage = result.ErrorMessage,
                        ErrorCode = result.ErrorCode,
                    };
                }

                await _reportRepository.RemoveAsync(report);

                return new BaseResult<ReportDto>()
                {
                    Data = _mapper.Map<ReportDto>(report)
                };
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);

                return new BaseResult<ReportDto>
                {
                    ErrorMessage = ErrorMessage.InternalServerError,
                    ErrorCode = (int)ErrorCode.InternalServerError,
                };
            }
        }

        /// <inheritdoc />
        public async Task<BaseResult<ReportDto>> UpdateReportAsync(UpdateReportDto dto)
        {
            try
            {
                var report = _reportRepository.GetAll().FirstOrDefault(x => x.Id == dto.Id);
                var result = _reportValidator.ValidateOnNull(report);

                if (!result.IsSuccess)
                {
                    return new BaseResult<ReportDto>()
                    {
                        ErrorMessage = result.ErrorMessage,
                        ErrorCode = result.ErrorCode,
                    };
                }

                report.Name = dto.Name;
                report.Description = dto.Description;

                await _reportRepository.UpdateAsync(report);

                return new BaseResult<ReportDto>()
                {
                    Data = _mapper.Map<ReportDto>(report),
                };
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);

                return new BaseResult<ReportDto>
                {
                    ErrorMessage = ErrorMessage.InternalServerError,
                    ErrorCode = (int)ErrorCode.InternalServerError,
                };
            }
        }
    }
}
