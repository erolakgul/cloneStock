using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockHaus.ModelClass.UserPage
{
    public class EnterMaterial
    {
        public string _teamNo { get; set; }

        public string _groupNo { get; set; }

        [Required(ErrorMessage = "Depo Yeri Zorunlu")]
        public string _stockStore { get; set; }
        [Required(ErrorMessage = "Stok Yeri Zorunlu")]
        public string _stockPlace { get; set; }
        // 2 kolon malzeme eklemesi için kullanıldı
        [Required(ErrorMessage = "Malzeme Kodu Zorunlu")]
        public string _mCode { get; set; }
        [Required(ErrorMessage = "Miktar Bilgisi Zorunlu")]
        public double _quantity { get; set; }
        [Required(ErrorMessage = "Indis Zorunlu")]
        public string _matIndex { get; set; }
        [Required(ErrorMessage = "Özel Stok Bilgisi Zorunlu")]
        public string _matSpecial { get; set; }
        public string _matSeriNum { get; set; }
        //
        [Required(ErrorMessage = "Malzeme Stok Tipi Zorunlu")]
        public string _matType { get; set; }
        public string _matName { get; set; }
    }
}
