using FitCore.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitCore.DAL.Data.Models
{
    public class AuditLog : ISoftDelete
    {
        public int Id { get; set; }
        public required string EntityName { get; set; }
        public required string Action { get; set; }
        public string? OldValue { get; set; }
        public string? NewValue { get; set; }
        public DateTime CreatedAt { get; set; }
        public int EntityPrimaryKey { get; set; }

        public int? UserId { get; set; }
        public virtual User User { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }
    }
}