using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StockHaus.ModelClass;

namespace StockHaus.Data.Model
{
    public class InventApproves : BaseClass
    {
        public string InventoryCode { get; set; }
        public double TotalQuantity { get; set; }
        public bool IsCanceled { get; set; }
    }
}
