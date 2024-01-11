using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockHaus.ModelClass.AdminPage
{
    public class AssignTasks
    {
        // ekip olarak sadece 3 ekip seçilebilmesine izin vereceğiz
        public string _teamNo { get; set; }
        public string _groupNoL { get; set; }
        public string _perName { get; set; }
        public string _perName2 { get; set; }
        public string _stockStore { get; set; }
        public string _stockPlace { get; set; }
        public string _endStockPlace { get; set; }
    }
}
