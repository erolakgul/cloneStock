using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockHaus.ModelClass.UserPage
{
    public class MaterialsModel : BaseClass
    {
        // malzeme kodu / ismi / olduğu ana bölüm / ana bölüme ait stok yeri bilgileri / hali hazırda olan adet bilgisi
        public string MatCode { get; set; }
        public string MatIndex { get; set; }
        public string MatSpecialStock { get; set; }
        public string MatSerialNumber { get; set; }
        public string MatName { get; set; }
        public string MatSection { get; set; }
        public string MatSectionPlace { get; set; }
        public string MatType { get; set; }
        public double MatQuantity { get; set; } // adet yada kg bilgisi içeriyor olabileceğinden double kullanıldı
        public string MatSerNoType { get; set; }
        public string IsCounted { get; set; } // Eğer o malzeme alan sorumlusu tarafından 3.takıma atanırsa aşağıdaki kolon 1 olur.
    }
}
