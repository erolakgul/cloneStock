using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StockHaus.ModelClass;

namespace StockHaus.Data.Model
{
    public class Users : BaseClass
    {
        public Users()
        {
            this.InGroups = new List<InventoryGroups>();
        }
        // Sisteme kayıt girebilecek kişilerin tutulduğu tablo
        public string Name { get; set; }

        [Required(ErrorMessage = "Mail gereklidir.")]
        //[RegularExpression(@"^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$", ErrorMessage = "Please enter correct email address")]
        public string Mail { get; set; }

        [Required(ErrorMessage = "Şifre gereklidir.")]
        [DataType(DataType.Password)]
        [StringLength(10, MinimumLength = 3, ErrorMessage = "Şifre 3-10 karakter arası olmalıdır.")]
        public string Password { get; set; }

        // 1000 = Koordinatör   2000 = Alan Sorumlusu    3000 = Grupların Ekiplerinde sisteme giriş yetkisi verilecek kişi
        public int Role { get; set; }
        public int IsOnline { get; set; }
        public bool IsSignableDocument { get; set; } // sayım belgesini imzalayabilme yetkisi

        // InventoryGroups tablosuna User_ID kolonu FK olarak bulunacak
        public virtual ICollection<InventoryGroups> InGroups { get; set; }
    }
}
