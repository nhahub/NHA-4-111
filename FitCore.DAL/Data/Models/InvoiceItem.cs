using FitCore.DAL.Interfaces;
using FitCore.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitCore.DAL.Data.Models
{
    public class InvoiceItem : IAuditable
    {
        public int InvoiceItemID { get; set; }
        public int InvoiceID { get; set; }
        public Invoice Invoice { get; set; } = null!;
        public InvoiceItemType ItemType { get; set; }
        public int ProductID { get; set; }
        public Product Product { get; set; } = null!;
        public string ItemName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal LineTotal { get; set; }
        public decimal UnitPrice { get; set; }
    }
}