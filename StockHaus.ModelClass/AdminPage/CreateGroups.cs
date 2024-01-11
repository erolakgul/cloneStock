using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockHaus.ModelClass.AdminPage
{
    public class CreateGroups
    {
        // ekip olarak sadece 3 ekip seçilebilmesine izin vereceğiz
        public string _teamNo { get; set; }

        [Required(ErrorMessage = "Group No Seçilmelidir..")]
        [StringLength(50, ErrorMessage = "1 karakter uzunluğunda olmalıdır..")]
        public string _groupNo { get; set; }

        [Required(ErrorMessage = "Personel ismi soy isimi girilmelidir..")]
        public string _perName { get; set; }

        [Required(ErrorMessage = "Yazıcı mı sayıcı mı olacak ?")]
        public string _missionStatus { get; set; }

        public string _storeID { get; set; }
    }
}
