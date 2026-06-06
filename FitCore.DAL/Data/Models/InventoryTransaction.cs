using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitCore.DAL.Data.Models
{
    public class InventoryTransaction
    {
        public int TransactionID { get; set; }
        public int ProductID { get; set; }
        public Product Product { get; set; } = null!;

        public string TransactionType { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public DateTime TransactionDate { get; set; }
    }
}