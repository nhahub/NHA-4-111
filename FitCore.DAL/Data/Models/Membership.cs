using FitCore.DAL.Interfaces;
using FitCore.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace FitCore.DAL.Data.Models
{
    public class Membership : IAuditable, ISoftDelete
    {
        public int MembershipID { get; set; }

        public int MemberProfileId { get; set; }
        public MemberProfile MemberProfile { get; set; } = null!;

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public MemberShipStatus Status { get; set; }
        public DateTime? FreezeStartDate { get; set; }
        public DateTime? FreezeEndDate { get; set; }
        public bool IsAutoRenew { get; set; }

        public int? GymServiceId { get; set; } ///////////
        public GymService? GymService { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }

        public int? ClassID { get; set; }
        public Class? Class { get; set; }

        // ✅ التعديل التاني: رصيد الحصص المتبقية
        public int? RemainingSessions { get; set; }
    }
}
