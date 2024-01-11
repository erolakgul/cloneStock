using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StockHaus.ModelClass;

namespace StockHaus.Data.Model
{
    public class InventoryCodeLogs : BaseClass
    {
        //public InventoryCodeLogs()
        //{
            //this.InGroups = new List<InventoryGroups>();
        //}
        public int CountOrder { get; set; }
        // InventoryGroups tablosuna Stores_ID kolonu FK olarak bulunacak
        public string TeamNo { get; set; }
        public string GroupNo { get; set; }

        ////////////////// NAVIGATION property //////////////////////
        //public virtual ICollection<InventoryGroups> InGroups { get; set; }
        //public int InventoryGroupID { get; set; }
        //[ForeignKey("InventoryGroupID")] // store un id sini alır  --> grup hangi depo da görevli
        //public virtual InventoryGroups InventoryGroup { get; set; }
    }
}
