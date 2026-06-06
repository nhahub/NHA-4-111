using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitCore.DAL.Data.Models
{
    public class Wallet
    {
        public int WalletID { get; set; }
        public int MemberID { get; set; }
        public MemberProfile MemberProfile { get; set; } = null!;

        public decimal Balance { get; set; }
        public DateTime LastUpdated { get; set; }

        public ICollection<WalletTransaction> WalletTransactions { get; set; } = new List<WalletTransaction>();
    }
}
