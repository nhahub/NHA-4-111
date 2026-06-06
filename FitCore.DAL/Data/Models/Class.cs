using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitCore.DAL.Data.Models
{
    public class Class
    {
        public int ClassID { get; set; }
        public int TrainerID { get; set; }
        public Trainer Trainer { get; set; } = null!;

        public string ClassName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; } = string.Empty;

        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
        public ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
    }
}
