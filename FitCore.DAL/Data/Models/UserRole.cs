using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitCore.DAL.Data.Models
{
    public class UserRole
    {
        public int RoleID { get; set; }
        public Role Role { get; set; } = null!;

        public int UserID { get; set; }
        public User User { get; set; } = null!;
    }
}
