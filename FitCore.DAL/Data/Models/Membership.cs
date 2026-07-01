using FitCore.DAL.Interfaces;
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
        public int MemberID { get; set; }
        public MemberProfile MemberProfile { get; set; } = null!;

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime? FreezeStartDate { get; set; }
        public DateTime? FreezeEndDate { get; set; }
        public bool IsAutoRenew { get; set; }
    }
}
