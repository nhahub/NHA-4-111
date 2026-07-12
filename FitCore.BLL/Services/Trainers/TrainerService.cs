using FitCore.BLL.Exceptions;
using FitCore.BLL.Interfaces.Trainers;
using FitCore.DAL.Data.Contexts;
using FitCore.DAL.Data.Models;
using FitCore.Shared.DTOs;
using FitCore.Shared.DTOs.Trainers;
using FitCore.Shared.Enums;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace FitCore.BLL.Services.Trainers
{
    public class TrainerService(FitCoreDbContext DbContext) : ITrainerService
    {
        public async Task<TrainerDto> CreateStaffAsync(CreateStaffDto dto)
        {
            ArgumentNullException.ThrowIfNull(dto);

            if (dto.Role != UserRoles.Trainer && dto.Role != UserRoles.Receptionist)
                throw new BusinessRuleException("Only Trainer or Receptionist roles are allowed.");

            if (string.IsNullOrWhiteSpace(dto.FullName) || string.IsNullOrWhiteSpace(dto.Email))
                throw new ValidationException("Full name and email are required.");

            if (await DbContext.Users.AnyAsync(u => u.Email == dto.Email))
                throw new BusinessRuleException("Email already exists.");

            var user = new User
            {
                FullName = dto.FullName,
                Email = dto.Email,
                PasswordHash = HashPassword(dto.Password),
                Status = UserStatus.Active,
                JoinDate = DateTime.UtcNow
            };

            user.UserRoles.Add(new UserRole { Role = dto.Role });
            if (dto.Role == UserRoles.Trainer)
            {
                user.Trainer = new Trainer { Specialization = dto.Specialization, Bio = dto.Bio };
            }

            await DbContext.Users.AddAsync(user);
            await DbContext.SaveChangesAsync();

            return dto.Role == UserRoles.Trainer
                ? MapToDto(user.Trainer!, user)
                : new TrainerDto { UserID = user.UserID, FullName = user.FullName, Email = user.Email };
        }

        public async Task<PaginationResponseDto<TrainerDto>> GetAllTrainersAsync(int page, int pageSize)
        {
            if (page <= 0) page = 1;
            const int maxPageSize = 50;
            if (pageSize <= 0 || pageSize > maxPageSize) pageSize = 20;

            var query = DbContext.Set<Trainer>()
                .Include(t => t.User)
                .Include(t => t.WorkingHoursSchedule)
                .OrderBy(t => t.TrainerID);

            var totalCount = await query.CountAsync();

            var trainers = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginationResponseDto<TrainerDto>
            {
                CurrentPage = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                Data = trainers.Select(t => MapToDto(t, t.User)).ToList()
            };
        }

        public async Task<TrainerDto> GetTrainerByIdAsync(int trainerId)
        {
            var trainer = await DbContext.Set<Trainer>()
                .Include(t => t.User)
                .Include(t => t.WorkingHoursSchedule)
                .FirstOrDefaultAsync(t => t.TrainerID == trainerId);

            if (trainer == null)
            {
                throw new KeyNotFoundException("No trainer found with this id.");
            }

            return MapToDto(trainer, trainer.User);
        }

        public async Task<ICollection<TrainerWorkingHourDto>> SetWorkingHoursAsync(int trainerId, SetWorkingHoursDto dto)
        {
            var trainer = await DbContext.Set<Trainer>()
                .Include(t => t.WorkingHoursSchedule)
                .FirstOrDefaultAsync(t => t.TrainerID == trainerId);

            if (trainer == null)
            {
                throw new KeyNotFoundException("No trainer found with this id.");
            }

            foreach (var slot in dto.WorkingHours)
            {
                if (slot.EndTime <= slot.StartTime)
                {
                    throw new ValidationException("Working hour end time must be after start time.");
                }
            }

            DbContext.Set<TrainerWorkingHour>().RemoveRange(trainer.WorkingHoursSchedule);

            var newSlots = dto.WorkingHours.Select(w => new TrainerWorkingHour
            {
                TrainerID = trainerId,
                Day = w.Day,
                StartTime = w.StartTime,
                EndTime = w.EndTime,
            }).ToList();

            await DbContext.Set<TrainerWorkingHour>().AddRangeAsync(newSlots);
            await DbContext.SaveChangesAsync();

            return newSlots.Select(w => new TrainerWorkingHourDto
            {
                Id = w.Id,
                Day = w.Day,
                StartTime = w.StartTime,
                EndTime = w.EndTime,
            }).ToList();
        }

        public async Task<ICollection<TrainerWorkingHourDto>> GetWorkingHoursAsync(int trainerId)
        {
            var exists = await DbContext.Set<Trainer>().AnyAsync(t => t.TrainerID == trainerId);
            if (!exists)
            {
                throw new KeyNotFoundException("No trainer found with this id.");
            }

            return await DbContext.Set<TrainerWorkingHour>()
                .Where(w => w.TrainerID == trainerId)
                .Select(w => new TrainerWorkingHourDto
                {
                    Id = w.Id,
                    Day = w.Day,
                    StartTime = w.StartTime,
                    EndTime = w.EndTime,
                })
                .ToListAsync();
        }

        public async Task<bool> AssignTrainerToClassAsync(int classId, int trainerId)
        {
            var gymClass = await DbContext.Set<Class>().FirstOrDefaultAsync(c => c.ClassID == classId);
            if (gymClass == null)
            {
                throw new KeyNotFoundException("No class found with this id.");
            }

            var trainerExists = await DbContext.Set<Trainer>().AnyAsync(t => t.TrainerID == trainerId);
            if (!trainerExists)
            {
                throw new KeyNotFoundException("No trainer found with this id.");
            }

            gymClass.TrainerID = trainerId;
            DbContext.Set<Class>().Update(gymClass);
            var affected = await DbContext.SaveChangesAsync();

            return affected > 0;
        }

        private static TrainerDto MapToDto(Trainer trainer, User user)
        {
            return new TrainerDto
            {
                TrainerID = trainer.TrainerID,
                UserID = trainer.UserID,
                FullName = user?.FullName ?? string.Empty,
                Email = user?.Email ?? string.Empty,
                PhoneNumber = user?.PhoneNumber ?? string.Empty,
                Specialization = trainer.Specialization,
                Bio = trainer.Bio,
                WorkingHours = trainer.WorkingHoursSchedule?.Select(w => new TrainerWorkingHourDto
                {
                    Id = w.Id,
                    Day = w.Day,
                    StartTime = w.StartTime,
                    EndTime = w.EndTime,
                }).ToList() ?? new List<TrainerWorkingHourDto>()
            };
        }

        private static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }
    }
}
