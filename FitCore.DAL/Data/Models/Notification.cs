using FitCore.DAL.Interfaces;
using FitCore.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitCore.DAL.Data.Models
{
    public class Notification : ISoftDelete
    {
        public int NotificationID { get; set; }
        public int UserID { get; set; }
        public User User { get; set; } = null!;

        public string Content { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public NotificationTypeEnum Type { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }

    }
}