using AutoMapper;
using FonTech.Application.Resources;
using FonTech.Domain.Dto.Role;
using FonTech.Domain.Dto.UserRole;
using FonTech.Domain.Entity;
using FonTech.Domain.Enum;
using FonTech.Domain.Interfaces.Databases;
using FonTech.Domain.Interfaces.Services;
using FonTech.Domain.Result;
using Microsoft.EntityFrameworkCore;
using UserRoleDto = FonTech.Domain.Dto.UserRole.UserRoleDto;

namespace FonTech.Application.Services
{
    public class RoleService : IRoleService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public RoleService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<BaseResult<UserRoleDto>> AddRoleForUserAsync(
            UserRoleDto dto)
        {
            var user = await _unitOfWork.Users.GetAll()
                .Include(x => x.Roles)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Login == dto.Login);

            if (user == null)
            {
                return new BaseResult<UserRoleDto>
                {
                    ErrorMessage = ErrorMessage.UserNotFound,
                    ErrorCode = (int)ErrorCode.UserNotFound,
                };
            }

            var roles = user.Roles.Select(x => x.Name);

            if (roles.Any(r => r == dto.RoleName))
            {
                return new BaseResult<UserRoleDto>
                {
                    ErrorMessage = ErrorMessage.UserAlreadyHasRole,
                    ErrorCode = (int)ErrorCode.UserAlreadyHasRole,
                };
            }

            var role = await _unitOfWork.Roles
                .GetAll()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Name == dto.RoleName);

            if (role == null)
            {
                return new BaseResult<UserRoleDto>
                {
                    ErrorMessage = ErrorMessage.RoleNotFound,
                    ErrorCode = (int)ErrorCode.RoleNotFound,
                };
            }

            var userNewRole = new UserRole()
            {
                RoleId = role.Id,
                UserId = user.Id,
            };

            await _unitOfWork.UserRoles.CreateAsync(userNewRole);
            await _unitOfWork.SaveChangesAsync();

            return new BaseResult<UserRoleDto>()
            {
                Data = new UserRoleDto(Login: dto.Login, RoleName: dto.RoleName),
            };
        }

        public async Task<BaseResult<RoleDto>> CreateRoleAsync(CreateRoleDto dto)
        {
            var role = await _unitOfWork.Roles
                .GetAll()
                .FirstOrDefaultAsync(x => x.Name == dto.Name);

            if (role != null)
            {
                return new BaseResult<RoleDto>()
                {
                    ErrorMessage = ErrorMessage.RoleAlreadyExist,
                    ErrorCode = (int)ErrorCode.RoleAlreadyExist,
                };
            }

            role = new Role()
            {
                Name = dto.Name
            };

            await _unitOfWork.Roles.CreateAsync(role);
            await _unitOfWork.SaveChangesAsync();

            return new BaseResult<RoleDto>()
            {
                Data = _mapper.Map<RoleDto>(role),
            };
        }

        public async Task<BaseResult<RoleDto>> DeleteRoleAsync(long id)
        {
            var role = await _unitOfWork.Roles
                .GetAll()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (role == null)
            {
                return new BaseResult<RoleDto>()
                {
                    ErrorMessage = ErrorMessage.RoleNotFound,
                    ErrorCode = (int)ErrorCode.RoleNotFound,
                };
            }

            _unitOfWork.Roles.Remove(role);
            await _unitOfWork.SaveChangesAsync();

            return new BaseResult<RoleDto>()
            {
                Data = _mapper.Map<RoleDto>(role),
            };
        }

        public async Task<BaseResult<UserRoleDto>> DeleteRoleForUserAsync(DeleteUserRoleDto dto)
        {
            var user = await _unitOfWork.Users.GetAll()
                .Include(x => x.Roles)
                .FirstOrDefaultAsync(x => x.Login == dto.Login);

            if (user == null)
            {
                return new BaseResult<Domain.Dto.UserRole.UserRoleDto>
                {
                    ErrorMessage = ErrorMessage.UserNotFound,
                    ErrorCode = (int)ErrorCode.UserNotFound,
                };
            }

            var role = user.Roles.FirstOrDefault(x => x.Id == dto.RoleId);

            if (role == null)
            {
                return new BaseResult<Domain.Dto.UserRole.UserRoleDto>()
                {
                    ErrorMessage = ErrorMessage.RoleNotFound,
                    ErrorCode = (int)ErrorCode.RoleNotFound,
                };
            }

            var userRole = await _unitOfWork.UserRoles.GetAll()
                .Where(x => x.RoleId == role.Id)
                .FirstOrDefaultAsync(x => x.UserId == user.Id);

            if (userRole == null)
            {
                return new BaseResult<Domain.Dto.UserRole.UserRoleDto>()
                {
                    ErrorMessage = ErrorMessage.RoleNotFound,
                    ErrorCode = (int)ErrorCode.RoleNotFound,
                };
            }

            _unitOfWork.UserRoles.Remove(userRole);
            await _unitOfWork.SaveChangesAsync();

            return new BaseResult<Domain.Dto.UserRole.UserRoleDto>()
            {
                Data = new Domain.Dto.UserRole.UserRoleDto(Login: user.Login, RoleName: role.Name),
            };
        }

        public async Task<BaseResult<RoleDto>> UpdateRoleAsync(RoleDto dto)
        {
            var role = await _unitOfWork.Roles.GetAll().FirstOrDefaultAsync(x => x.Id == dto.Id);

            if (role == null)
            {
                return new BaseResult<RoleDto>()
                {
                    ErrorMessage = ErrorMessage.RoleNotFound,
                    ErrorCode = (int)ErrorCode.RoleNotFound,
                };
            }

            role.Name = dto.Name;

            var updatedRole = _unitOfWork.Roles.Update(role);

            await _unitOfWork.SaveChangesAsync();

            return new BaseResult<RoleDto>()
            {
                Data = _mapper.Map<RoleDto>(updatedRole),
            };
        }

        public async Task<BaseResult<UserRoleDto>> UpdateUserRoleAsync(UpdateUserRoleDto dto)
        {
            var user = await _unitOfWork.Users.GetAll()
                .Include(x => x.Roles)
                .FirstOrDefaultAsync(x => x.Login == dto.Login);

            if (user == null)
            {
                return new BaseResult<UserRoleDto>
                {
                    ErrorMessage = ErrorMessage.UserNotFound,
                    ErrorCode = (int)ErrorCode.UserNotFound,
                };
            }

            var role = user.Roles.FirstOrDefault(x => x.Id == dto.FromRoleId);

            if (role == null)
            {
                return new BaseResult<UserRoleDto>()
                {
                    ErrorMessage = ErrorMessage.RoleNotFound,
                    ErrorCode = (int)ErrorCode.RoleNotFound,
                };
            }

            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    var userRole = await _unitOfWork.UserRoles.GetAll().Where(x => x.RoleId == role.Id)
                        .FirstOrDefaultAsync(x => x.UserId == user.Id);

                    _unitOfWork.UserRoles.Remove(userRole);
                    
                    var newUserRole = new UserRole()
                    {
                        UserId = user.Id,
                        RoleId = dto.ToRoleId,
                    };

                    await _unitOfWork.UserRoles.CreateAsync(newUserRole);
                    await _unitOfWork.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw new ApplicationException(message: ex.Message);
                }
            }

            return new BaseResult<UserRoleDto>()
            {
                Data = new UserRoleDto(user.Login, role.Name)
            };
        }
    }
}