using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockHaus.ModelClass.AreaController
{
    public class MissionAssign
    {
        public string _ware { get; set; }
        public string _stockPlace { get; set; }
        public List<MatSummary> _matList { get; set; }
    }
}
