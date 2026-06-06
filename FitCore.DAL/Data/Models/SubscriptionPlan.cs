using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitCore.DAL.Data.Models
{
    public class SubscriptionPlan
    {
        public int PlanID { get; set; }
        public string PlanName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Duration { get; set; }
        public bool IsActive { get; set; } = true;

        public ICollection<Membership> Memberships { get; set; } = new List<Membership>();
    }
}