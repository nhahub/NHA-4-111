using FitCore.DAL.Interfaces;
using FitCore.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitCore.DAL.Data.Models
{
    public class ClassBooking : IAuditable
    {
        public int BookingID { get; set; }

        public int ClassScheduleID { get; set; }
        public ClassSchedule ClassSchedule { get; set; } = null!;

        public int MemberUserId { get; set; }
        public MemberProfile MemberProfile { get; set; } = null!;

        // The concrete calendar date of the class occurrence being booked
        public DateTime SessionDate { get; set; }

        public BookingStatus Status { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
