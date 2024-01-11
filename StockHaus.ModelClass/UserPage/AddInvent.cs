using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockHaus.ModelClass.UserPage
{
    public class AddInvent
    {
        public int _id { get; set; } // material table id
        public string _matIndex { get; set; }
        public string _matSeriNum { get; set; }
        public string _matSpecial { get; set; }
        public double _quantity { get; set; }
        public string _teamNo { get; set; }
        public string _groupNo { get; set; }
        public string _mCode { get; set; }
        public string _stockStore { get; set; }
        public string _stockPlace { get; set; }
        public string _warningNum { get; set; }
        public string _matType { get; set; }
    }
}
