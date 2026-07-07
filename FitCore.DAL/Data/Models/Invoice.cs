using FitCore.DAL.Interfaces;
using FitCore.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitCore.DAL.Data.Models
{
    public class Invoice : IAuditable, ISoftDelete
    {
        public int InvoiceID { get; set; }
        
        public int UserID { get; set; }
        public User User{ get; set; } = null!;

        public DateTime IssueDate { get; set; }
        public decimal TotalAmount { get; set; }
        public InvoiceStatus InvoiceStatus { get; set; }
        public string Description { get; set; } = string.Empty;
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }

        public ICollection<InvoiceItem> InvoiceItems { get; set; } = new List<InvoiceItem>();
        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }
}
