using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockHaus.ModelClass.UserPage
{
    public class InventApprovesModel :BaseClass
    {
        public string InventoryCode { get; set; }
        public double TotalQuantity { get; set; }
        public bool IsCanceled { get; set; }
    }
}
