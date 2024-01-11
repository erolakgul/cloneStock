using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockHaus.Core.BaseService
{
    public class MessageService
    {
        public int ResultID { get; set; }
        public string Message { get; set; }

        public bool IsSuccess
        {
            get
            {
                if (ResultID > 0)
                {
                    return true;
                }
                return false;
            }
        } // control KD
    }
}
