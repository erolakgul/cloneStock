using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockHaus.ModelClass.AdminPage
{
    public class UserModel
    {
        public int _id { get; set; }
        public string _name { get; set; }
        public string _mail { get; set; }
        public int _status { get; set; }
        public bool _isSignable { get; set; }
    }
}
