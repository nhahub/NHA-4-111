using System;

namespace FitCore.Shared.DTOs.Classes
{
    public class ClassOccurrenceDto
    {
        public int ClassID { get; set; }
        public string ClassName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string TrainerName { get; set; } = string.Empty;

        public int ClassScheduleID { get; set; }
        public DateTime SessionDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }

        public int Capacity { get; set; }
        public int BookedCount { get; set; }
        public int AvailableSpots => Capacity - BookedCount;
    }
}
