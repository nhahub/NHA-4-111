using FitCore.Shared.Enums;
using System;

namespace FitCore.Shared.DTOs.Classes
{
    public class ClassBookingDto
    {
        public int BookingID { get; set; }
        public int ClassID { get; set; }
        public string ClassName { get; set; } = string.Empty;
        public DateTime SessionDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public BookingStatus Status { get; set; }
    }
}
