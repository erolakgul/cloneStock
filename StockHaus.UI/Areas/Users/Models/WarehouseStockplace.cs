using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace StockHaus.UI.Areas.Users.Models
{
    public class WarehouseStockplace
    {
        public string _stockStore { get; set; }
        public string _stockPlace { get; set; }
        public IEnumerable<SelectListItem> StoreList { get; set; }
    }
}