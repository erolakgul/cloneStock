using System.Web;
using System.Web.Mvc;
using StockHaus.UI.Helpers.Custom;

namespace StockHaus.UI
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());

            //filters.Add(
            //         new AuthAttribute
            //         {
            //             // By example, 10 minutes
            //             AnonymousMinutesTimeout = 10
            //         });
        }
    }
}