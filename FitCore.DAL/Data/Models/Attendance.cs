using FitCore.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitCore.DAL.Data.Models
{
    public class Attendance
    {
        public int AttendanceID { get; set; }
        public int UserId { get; set; }
        public MemberProfile MemberProfile { get; set; } = null!;

        public int? ClassID { get; set; }
        public Class? Class { get; set; }

        public AttendenceType Type { get; set; }
        public DateTime CheckInTime { get; set; }
    }
}
