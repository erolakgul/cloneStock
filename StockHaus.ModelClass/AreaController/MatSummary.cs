using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockHaus.ModelClass.AreaController
{
    public class MatSummary
    {
        public string _matCode { get; set; }
        public string _matIndis { get; set; }
        public string _serNo { get; set; }
        public int _inventId { get; set; } // inventoory tabloundaki ID kolonunu refer eder,aşağıdaki _id de aynı görevdedir
        public string _stockPlace { get; set; }
        public string _id { get; set; }
        public string _inventoryCode { get; set; }
        public string _user { get; set; }
    }
}
