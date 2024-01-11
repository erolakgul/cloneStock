using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StockHaus.ModelClass;

namespace StockHaus.Data.Model
{
    public class Stores : BaseClass
    {
        public Stores()
        {
            this.InGroups = new List<InventoryGroups>();
        }

        public string WareHouse { get; set; }
        public string StockPlace { get; set; }
        public string Description { get; set; }

        // InventoryGroups tablosuna Stores_ID kolonu FK olarak bulunacak
        public virtual ICollection<InventoryGroups> InGroups { get; set; }
    }
}
