using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace StockHaus.UI.Helpers.Custom
{
    public class ConnAttribute : ActionFilterAttribute, IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var dd = filterContext.Controller.TempData;
            // OnActionExecuting(filterContext);

        }
    }
}