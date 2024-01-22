using FonTech.Application.Resources;
using FonTech.Domain.Entity;
using FonTech.Domain.Enum;
using FonTech.Domain.Interfaces.Validations;
using FonTech.Domain.Result;

namespace FonTech.Application.Validations
{
    public class ReportValidator : IReportValidator
    {
        public BaseResult CreateValidator(Report report, User user)
        {
            if(report != null)
            {
                return new BaseResult()
                {
                    ErrorMessage = ErrorMessage.ReportAlreadyExist,
                    ErrorCode = (int)ErrorCode.ReportAlreadyExist,
                };
            }

            if(user == null)
            {
                return new BaseResult()
                {
                    ErrorMessage = ErrorMessage.UserNotFound,
                    ErrorCode = (int)ErrorCode.UserNotFound,
                };
            }

            return new BaseResult();
        }

        public BaseResult ValidateOnNull(Report model)
        {
            if(model == null)
            {
                _ = new BaseResult()
                {
                    ErrorMessage = ErrorMessage.ReportNotFound,
                    ErrorCode = (int)ErrorCode.ReportNotFound,
                };
            }

            return new BaseResult();
        }
    }
}
