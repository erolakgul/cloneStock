using System.Web.Mvc;

namespace StockHaus.UI.Areas.AreaController
{
    public class AreaControllerAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "AreaController";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "AreaController_default",
                "AreaController/{controller}/{action}/{id}",
                new { area = "AreaController", controller = "Panel", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
