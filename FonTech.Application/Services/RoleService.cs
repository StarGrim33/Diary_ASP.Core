using AutoMapper;
using FonTech.Application.Resources;
using FonTech.Domain.Dto.Role;
using FonTech.Domain.Entity;
using FonTech.Domain.Enum;
using FonTech.Domain.Interfaces.Repositories;
using FonTech.Domain.Interfaces.Services;
using FonTech.Domain.Result;
using Microsoft.EntityFrameworkCore;

namespace FonTech.Application.Services
{
    public class RoleService : IRoleService
    {
        private readonly IBaseRepository<User> _userRepository;
        private readonly IBaseRepository<Role> _roleRepository;
        private readonly IBaseRepository<UserRole> _userRoleRepository;
        private readonly IMapper _mapper;

        public RoleService(IBaseRepository<User> userRepository, IBaseRepository<Role> roleRepository, IMapper mapper, IBaseRepository<UserRole> userRoleRepository)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _mapper = mapper;
            _userRoleRepository = userRoleRepository;
        }

        public async Task<BaseResult<UserRoleDto>> AddRoleForUserAsync(UserRoleDto dto)
        {
            var user = await _userRepository.GetAll()
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

            var role = await _roleRepository.GetAll().AsNoTracking().FirstOrDefaultAsync(x => x.Name == dto.RoleName);

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

            await _userRoleRepository.CreateAsync(userNewRole);

            return new BaseResult<UserRoleDto>()
            {
                Data = new UserRoleDto(Login: dto.Login, RoleName: dto.RoleName),
            };
        }

        public async Task<BaseResult<RoleDto>> CreateRoleAsync(CreateRoleDto dto)
        {
            var role = await _roleRepository.GetAll().FirstOrDefaultAsync(x => x.Name == dto.Name);

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

            await _roleRepository.CreateAsync(role);

            return new BaseResult<RoleDto>()
            {
                Data = _mapper.Map<RoleDto>(role),
            };
        }

        public async Task<BaseResult<RoleDto>> DeleteRoleAsync(long id)
        {
            var role = await _roleRepository
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

            _roleRepository.Remove(role);
            await _roleRepository.SaveChangesAsync();

            return new BaseResult<RoleDto>()
            {
                Data = _mapper.Map<RoleDto>(role),
            };
        }

        public async Task<BaseResult<UserRoleDto>> DeleteRoleForUserAsync(UserRoleDto dto)
        {
            var user = await _userRepository.GetAll()
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

            var role = user.Roles.FirstOrDefault(x => x.Name == dto.RoleName);

            if (role == null)
            {
                return new BaseResult<UserRoleDto>()
                {
                    ErrorMessage = ErrorMessage.RoleNotFound,
                    ErrorCode = (int)ErrorCode.RoleNotFound,
                };
            }

            var userRole = await _userRoleRepository.GetAll()
               .Where(x => x.RoleId == role.Id)
               .FirstOrDefaultAsync(x => x.UserId == user.Id);
            
            _userRoleRepository.Remove(userRole);
            await _userRoleRepository.SaveChangesAsync();

            return new BaseResult<UserRoleDto>()
            {
                Data = new UserRoleDto(Login: dto.Login, RoleName: dto.RoleName),
            };
        }

        public async Task<BaseResult<RoleDto>> UpdateRoleAsync(RoleDto dto)
        {
            var role = await _roleRepository.GetAll().FirstOrDefaultAsync(x => x.Id == dto.Id);

            if (role == null)
            {
                return new BaseResult<RoleDto>()
                {
                    ErrorMessage = ErrorMessage.RoleNotFound,
                    ErrorCode = (int)ErrorCode.RoleNotFound,
                };
            }

            role.Name = dto.Name;

            var updatedRole = _roleRepository.Update(role);

            await _roleRepository.SaveChangesAsync();

            return new BaseResult<RoleDto>()
            {
                Data = _mapper.Map<RoleDto>(updatedRole),
            };
        }
    }
}
