using FitCore.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitCore.DAL.Data.Models
{
    public class Payment
    {
        public int PaymentID { get; set; }
        public int InvoiceID { get; set; }
        public Invoice Invoice { get; set; } = null!;

        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public decimal AmountPaid { get; set; }
        public DateTime PaymentDate { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public string TransactionReference { get; set; } = string.Empty;
    }
}