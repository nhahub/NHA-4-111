using FitCore.Shared.Enums;
using System;

namespace FitCore.Shared.DTOs.Book
{
    public class BookingResponseDto
    {
        public int BookingID { get; set; }

        public int? ClassID { get; set; }
        public string? ClassName { get; set; }

        public int? GymServiceId { get; set; }
        public string? GymServiceName { get; set; }

        public int MemberUserId { get; set; }
        public BookingStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}