using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitCore.DAL.Data.Models
{
    public class Booking
    {
        public int BookingID { get; set; }
        public int MemberID { get; set; }
        public MemberProfile MemberProfile { get; set; } = null!;

        public int ClassID { get; set; }
        public Class Class { get; set; } = null!;

        public DateTime BookingDate { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
