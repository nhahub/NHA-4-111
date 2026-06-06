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

        public int MemberID { get; set; }
        public MemberProfile MemberProfile { get; set; } = null!;

        public decimal AmountPaid { get; set; }
        public DateTime PaymentDate { get; set; }
        public string PaymentMethod { get; set; } = string.Empty; 
        public string TransactionReference { get; set; } = string.Empty;
    }
}