﻿using AutoMapper;
using FonTech.Application.Resources;
using FonTech.Domain.Dto.Report;
using FonTech.Domain.Entity;
using FonTech.Domain.Enum;
using FonTech.Domain.Interfaces.Repositories;
using FonTech.Domain.Interfaces.Services;
using FonTech.Domain.Interfaces.Validations;
using FonTech.Domain.Result;
using Microsoft.EntityFrameworkCore;
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

        public ReportService(IBaseRepository<Report> reportRepository, IReportValidator reportValidator, IBaseRepository<User> userRepository, IMapper mapper)
        {    
            _reportRepository = reportRepository;
            _userRepository = userRepository;
            _reportValidator = reportValidator;
            _mapper = mapper;
        }

        /// <inheritdoc />
        public async Task<CollectionResult<ReportDto>> GetReportsAsync(long userId)
        {
            ReportDto[] reports;

            try
            {
                reports = await _reportRepository.GetAll().Where(x => x.UserId == userId). // Фильтрация по userId
                    Select(x => new ReportDto(x.Id, x.Name, x.Description, x.CreatedAt.ToLongDateString())).ToArrayAsync(); // Формирую ReportDto
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
        public async Task<BaseResult<ReportDto>> GetReportByIdAsync(long id)
        {
            ReportDto? report;

            try
            {
                report = await _reportRepository.GetAll().Select(x => new ReportDto(
                    x.Id,
                    x.Name,
                    x.Description,
                    x.CreatedAt.ToLongDateString())).FirstOrDefaultAsync(x => x.Id == id);
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

            if(report == null)
            {
                _logger.Warning(ErrorMessage.ReportNotFound);

                return new BaseResult<ReportDto>
                {
                    ErrorMessage = ErrorMessage.ReportNotFound,
                    ErrorCode = (int)ErrorCode.ReportNotFound,
                };
            }

            return new BaseResult<ReportDto>()
            {
                Data = report,
            };
        }

        /// <inheritdoc />
        public async Task<BaseResult> CreateReportAsync(CreateReportDto dto)
        {
            try
            {
                var user = await _userRepository.GetAll().FirstOrDefaultAsync(x => x.Id == dto.UserId);
                var report = await _reportRepository.GetAll().FirstOrDefaultAsync(x => x.Name == dto.Name);
                var result = _reportValidator.CreateValidator(report: report, user: user);

                if(!result.IsSuccess)
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