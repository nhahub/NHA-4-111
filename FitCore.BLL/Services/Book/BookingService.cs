using FitCore.BLL.Interfaces.Book;
using FitCore.DAL.Data.Contexts;
using FitCore.Shared.DTOs;
using FitCore.Shared.DTOs.Book;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitCore.BLL.Services.Book
{
    public class BookingService(FitCoreDbContext _context) : IBookingService
    {
        public async Task<PaginationResponseDto<BookingResponseDto>> GetAllBookingsAsync(BookingParametersDto parameters, int memberId)
        {
            var query = _context.Bookings
                .Include(b => b.GymService)
                .Include(b => b.Class)
                .Where(b => !b.IsDeleted && b.MemberUserId == memberId)
                .AsQueryable();


            if (!string.IsNullOrWhiteSpace(parameters.SearchTerm))
            {
                var searchTerm = parameters.SearchTerm.ToLower();
                query = query.Where(b =>
                    b.GymService != null && b.GymService.Name.ToLower().Contains(searchTerm) ||
                    b.Class != null && b.Class.ClassName.ToLower().Contains(searchTerm)
                );
            }

            var totalCount = await query.CountAsync();

            var bookings = await query
                .OrderByDescending(b => b.CreatedAt)
                .Skip((parameters.PageNumber - 1) * parameters.PageSize)
                .Take(parameters.PageSize)
                .Select(b => new BookingResponseDto
                {
                    BookingID = b.BookingID,
                    ClassID = b.ClassID,
                    ClassName = b.Class != null ? b.Class.ClassName : null,
                    GymServiceId = b.GymServiceId,
                    GymServiceName = b.GymService != null ? b.GymService.Name : null,
                    MemberUserId = b.MemberUserId,
                    Status = b.Status,
                    CreatedAt = b.CreatedAt
                })
                .ToListAsync();

            return new PaginationResponseDto<BookingResponseDto>
            {
                TotalCount = totalCount,
                CurrentPage = parameters.PageNumber,
                PageSize = parameters.PageSize,
                Data = bookings
            };
        }
    }
}
    