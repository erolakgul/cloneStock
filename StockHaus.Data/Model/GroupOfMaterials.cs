using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StockHaus.ModelClass;

namespace StockHaus.Data.Model
{
    public class GroupOfMaterials : BaseClass
    {
        public string MatCode { get; set; }
        public string MatName { get; set; }
        public string MatType { get; set; }
        public string Group { get; set; }
    }
}
