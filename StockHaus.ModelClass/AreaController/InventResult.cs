using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockHaus.ModelClass.AreaController
{
    public class InventResult
    {
        public string InventCode { get; set; }
        public string MatCode { get; set; }
        public string MatIndex { get; set; }
        public string _matName { get; set; }
        public string _matSerNo { get; set; }
        public string _matSpecStock { get; set; }
        public string MatSection { get; set; }
        public string MatSectionPlace { get; set; }
        public double _S1 { get; set; }
        public int _S1ID { get; set; }
        public double _S2 { get; set; }
        public int _S2ID { get; set; }
        public double _S3 { get; set; }
        public int _S3ID { get; set; }
    }
}
