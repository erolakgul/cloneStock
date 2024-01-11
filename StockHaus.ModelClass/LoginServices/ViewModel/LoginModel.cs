using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockHaus.ModelClass.LoginService.ViewModel
{
    public class LoginModel
    {
        //[DataType(DataType.EmailAddress)]
        // loginden gönderilecek olan kayıtların var olup olmadığı usermanagement tan kontrol edilcek
        [Required]
        public string Email { get; set; }
        [DataType(DataType.Password)]
        [RegularExpression(@"[A-Za-z0-9]{6,8}")]
        public string Password { get; set; }
        public Guid SessionGUID { get; set; }
    }
}
