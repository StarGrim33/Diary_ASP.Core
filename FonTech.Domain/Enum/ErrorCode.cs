﻿namespace FonTech.Domain.Enum
{
    public enum ErrorCode
    {
        // 0 - 10 - Report бизнес-сущность
        // 11 - 20 - User бизнес-сущность
        ReportsNotFound = 0,
        ReportNotFound = 1,
        ReportAlreadyExist = 2,
        InternalServerError = 10,
        UserNotFound = 11,
        UserAlreadyExist = 12,
        UnauthorizedAccess = 13,
        UserAlreadyHasRole = 14,
        PasswordsNotEquals = 21,
        WrongPassword = 22,
        RoleAlreadyExist = 31,
        RoleNotFound = 32,
        DatabaseError = 41,
    }
}
