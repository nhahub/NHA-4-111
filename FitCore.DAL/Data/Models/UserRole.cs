using FitCore.Shared.Enums;
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
        public UserRoles Role { get; set; }

        public int UserID { get; set; }
        public User User { get; set; } = null!;
    }
}
