using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitCore.DAL.Data.Models
{
    public class AuditLog
    {
        public int LogID { get; set; }
        public int UserID { get; set; }
        public User User { get; set; } = null!;

        public string Action { get; set; } = string.Empty; 
        public string TableName { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
    }
}