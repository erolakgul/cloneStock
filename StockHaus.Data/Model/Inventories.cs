using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StockHaus.ModelClass;

namespace StockHaus.Data.Model
{
    public class Inventories : BaseClass
    {
        public Inventories()
        {
            //  this.InventApprove = new List<InventApproves>();
        }
        public string InventoryCode { get; set; }
        public string MatCode { get; set; }
        public string MatIndex { get; set; }
        public string MatSpecialStock { get; set; }
        public string MatSerialNumber { get; set; }
        public double MatQuantity { get; set; }
        public string MatName { get; set; }
        public string MatSection { get; set; }
        public string MatSectionPlace { get; set; }
        // IMZALANDI MI?
        public string SignedBy { get; set; }
        public DateTime SignedTime { get; set; }
        public bool IsSigned { get; set; }
        // ONAYLANDI MI ?
        public string ApprovedPers { get; set; }
        public DateTime ApprovedDate { get; set; }
        public bool IsApproved { get; set; }
        public bool IsDeleted { get; set; }
    }
}
