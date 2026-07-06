using FitCore.BLL.Interfaces.Profile;
using FitCore.DAL.Data.Contexts;
using FitCore.Shared.DTOs.User;
using Microsoft.EntityFrameworkCore;

namespace FitCore.BLL.Services.Profile
{
    public class ProfileService(FitCoreDbContext dbContext) : IProfileService
    {
        public async Task<UserDto> GetProfile()
        {
            int userId = 3;
            //int userId = _currentService.UserId ?? throw new UnauthorizedAccessException("No user id assigned");

            var user = await dbContext.Users.Include(x => x.UserRoles).
                FirstOrDefaultAsync(x => x.UserID == userId);

            if (user == null)
            {
                throw new KeyNotFoundException("This User not found");
            }

            var roles = user.UserRoles.Select(
                x => new UserRoleDto{
                   Role = x.Role,
                }).ToList();

            UserDto userDto = new UserDto()
            {
                FullName = user.FullName,
                Email = user.Email,
                JoinDate = user.JoinDate,
                PhoneNumber = user.PhoneNumber,
                Status = user.Status,
                UserRoles = roles,
            };

            if (user.Trainer != null)
            {
                TrainerDto trainerDto = new TrainerDto()
                {
                    Bio = user.Trainer.Bio,
                    Specialization = user.Trainer.Specialization,
                    WorkingHours = user.Trainer.WorkingHours,
                };
                userDto.TrainerDto = trainerDto;
            }

            if (user.MemberProfile != null)
            {
                MemberDto memberDto = new MemberDto()
                {
                    QRCodeData = user.MemberProfile.QRCodeData,
                };
                userDto.MemberDto = memberDto;
            }

            return userDto;

        }
    }
}
