using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitCore.DAL.Data.Models
{
    public class Invoice
    {
        public int InvoiceID { get; set; }
        public int MemberID { get; set; }
        public MemberProfile MemberProfile { get; set; } = null!;

        public DateTime IssueDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string InvoiceStatus { get; set; } = string.Empty; 
        public string Description { get; set; } = string.Empty;

        public ICollection<InvoiceItem> InvoiceItems { get; set; } = new List<InvoiceItem>();
        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }
}
