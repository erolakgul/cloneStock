using System.Web.Mvc;

namespace StockHaus.UI.Areas.Users
{
    public class UsersAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Users";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Users_default",
                "Users/{controller}/{action}/{id}",
                  new { area = "Users", controller = "Panel", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
