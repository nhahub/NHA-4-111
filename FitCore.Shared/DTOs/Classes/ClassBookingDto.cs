using FitCore.Shared.Enums;
using System;

namespace FitCore.Shared.DTOs.Classes
{
    public class ClassBookingDto
    {
        public int BookingID { get; set; }
        public int ClassID { get; set; }
        public string ClassName { get; set; } = string.Empty;
        public BookingStatus Status { get; set; }
        public List<string> ScheduleDetails { get; set; } = new List<string>();
    }

}
