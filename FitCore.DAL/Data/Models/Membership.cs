using FitCore.DAL.Interfaces;
using FitCore.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitCore.DAL.Data.Models
{
    public class Membership : IAuditable
    {
        public int MembershipID { get; set; }
        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public MemberShipStatus Status { get; set; }
        public DateTime? FreezeStartDate { get; set; }
        public DateTime? FreezeEndDate { get; set; }
        public bool IsAutoRenew { get; set; }

        public int GymServiceId { get; set; }
        public GymService GymService { get; set; }
    }
}
