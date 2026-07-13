using FitCore.Shared.Enums;

namespace FitCore.Shared.DTOs.Book
{
    public class BookingParametersDto
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        // Search
        public string? SearchTerm { get; set; }

    }
}