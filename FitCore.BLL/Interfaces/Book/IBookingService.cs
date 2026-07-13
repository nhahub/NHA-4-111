using FitCore.Shared.DTOs;
using FitCore.Shared.DTOs.Book;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitCore.BLL.Interfaces.Book
{
    public interface IBookingService
    {
        Task<PaginationResponseDto<BookingResponseDto>> GetAllBookingsAsync(BookingParametersDto parameters, int memberId);
    }
}
