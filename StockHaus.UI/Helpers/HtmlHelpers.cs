using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Runtime.InteropServices;
using System.Web;

namespace StockHaus.UI.Helpers
{
    public static class HtmlHelpers
    {
        public static string GetIpHelper()
        {
            return HttpContext.Current.Request.UserHostAddress;

        }
        public static bool CheckMailAddress(string email)
        {
            try
            {
                MailAddress m = new MailAddress(email);

                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        // net var mı yok mu kontrol et
        [DllImport("wininet.dll")]
        private extern static bool InternetGetConnectedState(out int Description, int ReservedValue);
        public static bool IsConnectedToInternet()
        {
            int Desc;
            return InternetGetConnectedState(out Desc, 0);
        }
    }
}