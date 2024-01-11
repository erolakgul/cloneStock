using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace StockHaus.UI.Helpers.Custom
{
    public class AuthAttribute : AuthorizeAttribute//ActionFilterAttribute, IAuthorizationFilter
    {
        public int? AnonymousMinutesTimeout { get; set; }
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            // Let default handling occurs
            base.OnAuthorization(filterContext);

            //if (filterContext.Result as HttpUnauthorizedResult != null)
            //    return;

            //var isAnonAllowed = filterContext.ActionDescriptor.IsDefined(
            //        typeof(AllowAnonymousAttribute), true) ||
            //    filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(
            //        typeof(AllowAnonymousAttribute), true);
            //if (isAnonAllowed && AnonymousMinutesTimeout.HasValue &&
            //    // Not authorized
            //    !AuthorizeCore(filterContext.HttpContext))
            //{
            //    const string visitStartCookieName = "visitStartCookie";
            //    const string visitStartDateFormat = "yyyyMMddhhmmss";
            //    var visitStartCookie = filterContext.HttpContext.Request
            //        .Cookies[visitStartCookieName];
            //    var now = DateTime.UtcNow;
            //    DateTime visitStartDate;
            //    var visitStartCookieValid = visitStartCookie != null &&
            //        DateTime.TryParseExact(visitStartCookie.Value,
            //            visitStartDateFormat, null, DateTimeStyles.AssumeUniversal,
            //            out visitStartDate);
            //    if (!visitStartCookieValid)
            //    {
            //        visitStartDate = now;
            //        filterContext.HttpContext.Response.Cookies.Add(
            //            // Session cookie.
            //            new HttpCookie
            //            {
            //                Name = "visitStartCookie",
            //                Value = now.ToString(visitStartDateFormat)
            //            });
            //    }
            //    if (visitStartDate.AddMinutes(AnonymousMinutesTimeout.Value) < now)
            //    {
            //        // Anonymous visit timed out
            //        HandleUnauthorizedRequest(filterContext);
            //        return;
            //    }
            //}

        }

    }
}