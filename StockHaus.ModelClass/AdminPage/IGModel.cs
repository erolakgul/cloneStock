using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockHaus.ModelClass.AdminPage
{
    public class IGModel : BaseClass
    {
        public string PerName { get; set; }
        public int TeamNo { get; set; }
        public int GroupNo { get; set; }
        public int MissionStatu { get; set; }
        public string EndStockPlace { get; set; }
        public int StoreID { get; set; }
        public int UserID { get; set; }

    }
}
