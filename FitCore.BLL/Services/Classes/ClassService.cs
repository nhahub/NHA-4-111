using FitCore.BLL.Exceptions;
using FitCore.BLL.Interfaces.Classes;
using FitCore.DAL.Data.Contexts;
using FitCore.DAL.Data.Models;
using FitCore.Shared.DTOs;
using FitCore.Shared.DTOs.Classes;
using FitCore.Shared.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FitCore.BLL.Services.Classes
{
    public class ClassService(FitCoreDbContext DbContext) : IClassService
    {
        private const int MaxBrowseRangeDays = 90;

        public async Task<ClassDto> CreateClassAsync(CreateClassDto dto)
        {
            ArgumentNullException.ThrowIfNull(dto);

            if (string.IsNullOrWhiteSpace(dto.ClassName)) throw new ValidationException("Class name is required.");
            if (dto.Capacity <= 0) throw new ValidationException("Capacity must be greater than zero.");
            if (dto.NumberOfSessions <= 0) throw new ValidationException("Number of sessions must be greater than zero.");
            if (dto.Schedules == null || !dto.Schedules.Any()) throw new ValidationException("At least one schedule is required.");
            foreach (var slot in dto.Schedules)
            {
                if (slot.EndTime <= slot.StartTime) throw new ValidationException("End time must be after start time.");
            }

            var trainer = await DbContext.Set<Trainer>().FirstOrDefaultAsync(t => t.TrainerID == dto.TrainerID);
            if (trainer == null) throw new KeyNotFoundException("No trainer found with this id.");

            var gymClass = new Class
            {
                ClassName = dto.ClassName,
                Description = dto.Description,
                Capacity = dto.Capacity,
                NumberOfSessions = dto.NumberOfSessions,
                TrainerID = dto.TrainerID,
                Status = ClassStatus.Active,
                Schedules = dto.Schedules.Select(s => new ClassSchedule { Day = s.Day, StartTime = s.StartTime, EndTime = s.EndTime }).ToList()
            };

            await DbContext.Set<Class>().AddAsync(gymClass);
            await DbContext.SaveChangesAsync();

            return MapToDto(gymClass, trainer);
        }

        public async Task<ClassDto> UpdateClassAsync(int classId, UpdateClassDto dto)
        {
            var gymClass = await DbContext.Set<Class>()
                .Include(c => c.Trainer)
                .Include(c => c.Schedules)
                .FirstOrDefaultAsync(c => c.ClassID == classId);

            if (gymClass == null)
            {
                throw new KeyNotFoundException("No class found with this id.");
            }

            if (dto.Capacity <= 0)
            {
                throw new ValidationException("Capacity must be greater than zero.");
            }

            if (dto.NumberOfSessions <= 0)
            {
                throw new ValidationException("Number of sessions must be greater than zero.");
            }

            gymClass.ClassName = dto.ClassName;
            gymClass.Description = dto.Description;
            gymClass.Capacity = dto.Capacity;
            gymClass.NumberOfSessions = dto.NumberOfSessions;
            gymClass.Status = dto.Status;

            DbContext.Set<Class>().Update(gymClass);
            await DbContext.SaveChangesAsync();

            return MapToDto(gymClass, gymClass.Trainer);
        }

        public async Task<PaginationResponseDto<ClassDto>> GetAllClassesAsync(int page, int pageSize)
        {
            if (page <= 0) page = 1;
            const int maxPageSize = 50;
            if (pageSize <= 0 || pageSize > maxPageSize) pageSize = 20;

            var query = DbContext.Set<Class>()
                .Include(c => c.Trainer).ThenInclude(t => t.User)
                .Include(c => c.Schedules)
                .OrderBy(c => c.ClassID);

            var totalCount = await query.CountAsync();

            var classes = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginationResponseDto<ClassDto>
            {
                CurrentPage = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                Data = classes.Select(c => MapToDto(c, c.Trainer)).ToList()
            };
        }

        public async Task<ClassDto> GetClassByIdAsync(int classId)
        {
            var gymClass = await DbContext.Set<Class>()
                .Include(c => c.Trainer).ThenInclude(t => t.User)
                .Include(c => c.Schedules)
                .FirstOrDefaultAsync(c => c.ClassID == classId);

            if (gymClass == null) throw new KeyNotFoundException("No class found with this id.");
            return MapToDto(gymClass, gymClass.Trainer);
        }

        public async Task<ClassScheduleDto> AddScheduleAsync(int classId, ClassScheduleDto dto)
        {
            var gymClass = await DbContext.Set<Class>().FirstOrDefaultAsync(c => c.ClassID == classId);
            if (gymClass == null)
            {
                throw new KeyNotFoundException("No class found with this id.");
            }

            if (dto.EndTime <= dto.StartTime)
            {
                throw new ValidationException("Schedule end time must be after start time.");
            }

            var schedule = new ClassSchedule
            {
                ClassID = classId,
                Day = dto.Day,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
            };

            await DbContext.Set<ClassSchedule>().AddAsync(schedule);
            await DbContext.SaveChangesAsync();

            return new ClassScheduleDto
            {
                Id = schedule.Id,
                Day = schedule.Day,
                StartTime = schedule.StartTime,
                EndTime = schedule.EndTime,
            };
        }

        public async Task<PaginationResponseDto<ClassOccurrenceDto>> BrowseClassesAsync(DateTime fromDate, DateTime toDate, int page, int pageSize)
        {
            if (page <= 0) page = 1;
            const int maxPageSize = 50;
            if (pageSize <= 0 || pageSize > maxPageSize) pageSize = 20;

            fromDate = fromDate.Date;
            toDate = toDate.Date;

            if (toDate < fromDate)
            {
                throw new ValidationException("The end date must be on or after the start date.");
            }

            if ((toDate - fromDate).TotalDays > MaxBrowseRangeDays)
            {
                throw new ValidationException($"The date range cannot exceed {MaxBrowseRangeDays} days.");
            }

            var classes = await DbContext.Set<Class>()
                .Where(c => c.Status == ClassStatus.Active)
                .Include(c => c.Trainer).ThenInclude(t => t.User)
                .Include(c => c.Schedules)
                .ToListAsync();

            var occurrences = new List<ClassOccurrenceDto>();

            for (var date = fromDate; date <= toDate; date = date.AddDays(1))
            {
                foreach (var gymClass in classes)
                {
                    foreach (var schedule in gymClass.Schedules.Where(s => s.Day == date.DayOfWeek))
                    {
                        occurrences.Add(new ClassOccurrenceDto
                        {
                            ClassID = gymClass.ClassID,
                            ClassName = gymClass.ClassName,
                            Description = gymClass.Description,
                            TrainerName = gymClass.Trainer?.User?.FullName ?? string.Empty,
                            ClassScheduleID = schedule.Id,
                            SessionDate = date,
                            StartTime = schedule.StartTime,
                            EndTime = schedule.EndTime,
                            Capacity = gymClass.Capacity,
                        });
                    }
                }
            }

            if (occurrences.Any())
            {
                var scheduleIds = occurrences.Select(o => o.ClassScheduleID).Distinct().ToList();

                var bookedCounts = await DbContext.Set<ClassBooking>()
                    .Where(b => scheduleIds.Contains(b.ClassScheduleID)
                        && b.SessionDate >= fromDate && b.SessionDate <= toDate
                        && b.Status == BookingStatus.Booked)
                    .GroupBy(b => new { b.ClassScheduleID, b.SessionDate.Date })
                    .Select(g => new { g.Key.ClassScheduleID, g.Key.Date, Count = g.Count() })
                    .ToListAsync();

                foreach (var occurrence in occurrences)
                {
                    var match = bookedCounts.FirstOrDefault(b => b.ClassScheduleID == occurrence.ClassScheduleID && b.Date == occurrence.SessionDate.Date);
                    occurrence.BookedCount = match?.Count ?? 0;
                }
            }

            var ordered = occurrences.OrderBy(o => o.SessionDate).ThenBy(o => o.StartTime).ToList();

            var totalCount = ordered.Count;
            var paged = ordered.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            return new PaginationResponseDto<ClassOccurrenceDto>
            {
                CurrentPage = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                Data = paged
            };
        }

        public async Task<ClassBookingDto> BookClassAsync(int memberUserId, BookClassDto dto)
        {
            var member = await DbContext.Set<MemberProfile>().FirstOrDefaultAsync(m => m.UserID == memberUserId);
            if (member == null)
            {
                throw new BusinessRuleException($"No member profile found for user id {memberUserId}.");
            }

            var schedule = await DbContext.Set<ClassSchedule>()
                .Include(s => s.Class)
                .FirstOrDefaultAsync(s => s.Id == dto.ClassScheduleID);

            if (schedule == null)
            {
                throw new KeyNotFoundException("No class schedule found with this id.");
            }

            if (schedule.Class.Status != ClassStatus.Active)
            {
                throw new BusinessRuleException("This class is not currently active.");
            }

            var sessionDate = dto.SessionDate.Date;

            if (sessionDate.DayOfWeek != schedule.Day)
            {
                throw new BusinessRuleException($"This class only runs on {schedule.Day}.");
            }

            if (sessionDate < DateTime.UtcNow.Date)
            {
                throw new BusinessRuleException("Cannot book a class session in the past.");
            }

            var alreadyBooked = await DbContext.Set<ClassBooking>().AnyAsync(b =>
                b.ClassScheduleID == schedule.Id &&
                b.MemberUserId == memberUserId &&
                b.SessionDate.Date == sessionDate &&
                b.Status == BookingStatus.Booked);

            if (alreadyBooked)
            {
                throw new BusinessRuleException("You already have a booking for this class session.");
            }

            var bookedCount = await DbContext.Set<ClassBooking>().CountAsync(b =>
                b.ClassScheduleID == schedule.Id &&
                b.SessionDate.Date == sessionDate &&
                b.Status == BookingStatus.Booked);

            if (bookedCount >= schedule.Class.Capacity)
            {
                throw new BusinessRuleException("This class session is fully booked.");
            }

            var booking = new ClassBooking
            {
                ClassScheduleID = schedule.Id,
                MemberUserId = memberUserId,
                SessionDate = sessionDate,
                Status = BookingStatus.Booked,
                CreatedAt = DateTime.UtcNow,
            };

            await DbContext.Set<ClassBooking>().AddAsync(booking);
            await DbContext.SaveChangesAsync();

            return new ClassBookingDto
            {
                BookingID = booking.BookingID,
                ClassID = schedule.ClassID,
                ClassName = schedule.Class.ClassName,
                SessionDate = booking.SessionDate,
                StartTime = schedule.StartTime,
                EndTime = schedule.EndTime,
                Status = booking.Status,
            };
        }

        public async Task<bool> CancelBookingAsync(int memberUserId, int bookingId)
        {
            var booking = await DbContext.Set<ClassBooking>().FirstOrDefaultAsync(b => b.BookingID == bookingId);

            if (booking == null || booking.MemberUserId != memberUserId)
            {
                throw new KeyNotFoundException("No booking found with this id for this member.");
            }

            if (booking.Status != BookingStatus.Booked)
            {
                throw new BusinessRuleException("This booking cannot be cancelled.");
            }

            booking.Status = BookingStatus.Cancelled;
            DbContext.Set<ClassBooking>().Update(booking);
            var affected = await DbContext.SaveChangesAsync();

            return affected > 0;
        }

        public async Task<ICollection<ClassBookingDto>> GetMemberBookingsAsync(int memberUserId)
        {
            var bookings = await DbContext.Set<ClassBooking>()
                .Include(b => b.ClassSchedule).ThenInclude(s => s.Class)
                .Where(b => b.MemberUserId == memberUserId)
                .OrderByDescending(b => b.SessionDate)
                .ToListAsync();

            return bookings.Select(b => new ClassBookingDto
            {
                BookingID = b.BookingID,
                ClassID = b.ClassSchedule.ClassID,
                ClassName = b.ClassSchedule.Class.ClassName,
                SessionDate = b.SessionDate,
                StartTime = b.ClassSchedule.StartTime,
                EndTime = b.ClassSchedule.EndTime,
                Status = b.Status,
            }).ToList();
        }

        private static ClassDto MapToDto(Class gymClass, Trainer? trainer)
        {
            return new ClassDto
            {
                ClassID = gymClass.ClassID,
                ClassName = gymClass.ClassName,
                Description = gymClass.Description,
                Capacity = gymClass.Capacity,
                NumberOfSessions = gymClass.NumberOfSessions,
                Status = gymClass.Status,
                TrainerID = gymClass.TrainerID,
                TrainerName = trainer?.User?.FullName ?? string.Empty,
                Schedules = gymClass.Schedules?.Select(s => new ClassScheduleDto
                {
                    Id = s.Id,
                    Day = s.Day,
                    StartTime = s.StartTime,
                    EndTime = s.EndTime,
                }).ToList() ?? new List<ClassScheduleDto>()
            };
        }
    }
}
