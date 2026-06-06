using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitCore.DAL.Data.Models
{
    public class Supplier
    {
        public int SupplierID { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string SupplierPhone { get; set; } = string.Empty;
    }
}