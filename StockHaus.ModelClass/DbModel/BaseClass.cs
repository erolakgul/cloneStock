using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockHaus.ModelClass
{
    public class BaseClass
    {
        [Key]
        public int ID { get; set; }
        public string Company { get; set; }
        public bool IsActive { get; set; }
        public string IpAddress { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime ChangeDate { get; set; }
        public string ChangedBy { get; set; }
    }
}
