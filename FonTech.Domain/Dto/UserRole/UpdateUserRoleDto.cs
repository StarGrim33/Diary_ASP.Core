namespace FonTech.Domain.Dto.UserRole;

public record UpdateUserRoleDto(string Login, long FromRoleId, long ToRoleId);