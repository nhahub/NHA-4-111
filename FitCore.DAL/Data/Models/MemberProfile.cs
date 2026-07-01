using FitCore.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitCore.DAL.Data.Models
{
    public class MemberProfile : IAuditable
    {
        public int UserID { get; set; }
        public User User { get; set; } = null!;

        public string QRCodeData { get; set; } = string.Empty;

        public ICollection<Membership> Memberships { get; set; } = new List<Membership>();
        public ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();

    }
}
