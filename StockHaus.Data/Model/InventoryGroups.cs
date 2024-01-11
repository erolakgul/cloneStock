using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StockHaus.ModelClass;

namespace StockHaus.Data.Model
{
    public class InventoryGroups : BaseClass
    {
        public InventoryGroups()
        {
            //this.InCodLog = new List<InventoryCodeLogs>();
        }

        [Required]
        public int TeamNo { get; set; }
        [Required]
        public int GroupNo { get; set; }
        public string PerName { get; set; }
        // 1 = YAZICI      2 = SAYICI
        public int MissionStatu { get; set; }
        public string EndStockPlace { get; set; }

        ////////////////// NAVIGATION property //////////////////////

        //public Nullable<int> InventoryCodeLogID { get; set; }
        //[ForeignKey("InventoryCodeLogID")] // store un id sini alır  --> grup hangi depo da görevli
        //public virtual InventoryCodeLogs InventoryCodeLog { get; set; }

        //public virtual ICollection<InventoryCodeLogs> InCodLog { get; set; }

        public int StoresID { get; set; }
        [ForeignKey("StoresID")] // store un id sini alır  --> grup hangi depo da görevli
        public virtual Stores Store { get; set; }

        public int UserID { get; set; }
        [ForeignKey("UserID")] // user ın id sini alır. grup personeli user id si
        public virtual Users User { get; set; }
    }
}
