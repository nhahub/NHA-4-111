using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitCore.DAL.Data.Models
{
    public class WalletTransaction
    {
        public int TransactionID { get; set; }
        public int WalletID { get; set; }
        public Wallet Wallet { get; set; } = null!;

        public string TransactionType { get; set; } = string.Empty; 
        public decimal Amount { get; set; }
        public bool IsActive { get; set; }
        public DateTime TransactionDate { get; set; }
    }
}
