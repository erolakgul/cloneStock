using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using StockHaus.Core.BaseService;
using StockHaus.ModelClass.AdminPage;

namespace StockHaus.UI.Areas.Admin.Controllers
{
    public class AddController : Controller
    {
        ServicePoints point;
        public AddController()
        {
            point = new ServicePoints();
        }
        //
        // GET: /Admin/Add/

        public ActionResult Index()
        {
            if (TempData["DefPer"] != null)
            {
                ViewBag.DefPer = TempData["DefPer"].ToString();
            }
            else if (TempData["DefStock"] != null)
            {
                ViewBag.DefStock = TempData["DefStock"].ToString();
            }

            return View();
        }

        [HttpPost]
        public ActionResult DefineStore(DefineStores model)
        {
            string _result = "";

            if (model._company == null && model._description == null && model._stockPlace == null && model._wareHouse == null)
            {
                TempData["DefStock"] = "Empty";
                return RedirectToAction("Index", "Add");
            }

            if (model._company == "01" || model._company == "02" || model._company == "03")
            {
                if (model._wareHouse != "" || model._stockPlace != "" || model._description != "")
                {
                    model._createdBy = Session["Email"].ToString();
                    _result = point.StoreService.BeforeInsert(model);
                }
            }

            if (_result != "" || _result != null)
            {
                if (_result == "True")
                {
                    TempData["DefStock"] = "True";
                    return RedirectToAction("Index", "Add");
                }
            }

            TempData["DefStock"] = "False";
            return RedirectToAction("Index", "Add");
        }

        public ActionResult DefinePersonel(DefineUser model)
        {
            string _mail = "";
            string _name = "";
            bool _IsValid;

            if (model._name != null && model._email != null && model._password != null)
            {
                _name = model._name.ToUpper();
                _mail = model._email.ToLower();

                // mail adresi check edilir
                _IsValid = true;//Helpers.HelperClass.CheckMailAddress(_mail);

                if (_IsValid)
                {
                    model._ipAddress = Helpers.HtmlHelpers.GetIpHelper();
                    model._createdBy = Session["Email"].ToString();

                    string mess = point.UserService.BeforeInsert(model);

                    if (mess == "True")
                    {
                        TempData["DefPer"] = "Success";
                        return RedirectToAction("Index", "Add");
                    }
                    TempData["DefPer"] = "False";
                    return RedirectToAction("Index", "Add");
                }
                else
                {
                    TempData["DefPer"] = "MailFalse";
                    return RedirectToAction("Index", "Add");
                }
            }

            TempData["DefPer"] = "Empty";
            return RedirectToAction("Index", "Add");
        }

        [HttpGet]
        public ActionResult DefineStock(int id)
        {
            return PartialView("_PartialDefineStockPlace");
        }

        [HttpGet]
        public ActionResult DefinePrsnl(int id)
        {
            return PartialView("_PartialDefinePersonel");
        }

    }
}
