using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StockHaus.UI.Enums
{
    public enum ProductionStatus
    {
        Başlatılmadı = 0,
        İlk_operasyon_sıfır_olarak_onaylı = 1,
        İlk_operasyonda_kısmi = 2,
        Ara_operasyonda = 3,
        İlk_operasyonda_kısmi_Ve_Ara_operasyon = 4,
        İlk_operasyon_onaylı = 5,
        Ara_operasyonda_Ve_tam_onaylı = 6,
        Onaylı = 7
    }
}