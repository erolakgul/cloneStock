using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StockHaus.ModelClass;

namespace StockHaus.Data.Model
{
    public class CancelledInventories : BaseClass
    {
        public string InventoryCode { get; set; }
        public string MatCode { get; set; }
        public string MatIndex { get; set; }
        public string MatSpecialStock { get; set; }
        public string MatSerialNumber { get; set; }
        public double MatQuantity { get; set; }
        public string MatName { get; set; }
        public string MatSection { get; set; }
        public string MatSectionPlace { get; set; }
    }
}
