using FitCore.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace FitCore.DAL.Data.Models
{
    public class MemberProfile : IAuditable, ISoftDelete
    {
        public int UserID { get; set; }
        public User User { get; set; } = null!;
        public int MemberProfileId { get; set; }
        public string QRCodeData { get; set; } = string.Empty;
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }

        public ICollection<Membership> Memberships { get; set; } = new List<Membership>();

        public ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
        public ICollection<ClassBooking> ClassBookings { get; set; } = new List<ClassBooking>();
        public ICollection<PrivateSession> PrivateSessions { get; set; } = new List<PrivateSession>();
    }
}
