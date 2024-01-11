using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using StockHaus.Core.BaseService;
using StockHaus.ModelClass.LoginService.ViewModel;
using StockHaus.UI.Helpers;
using StockHaus.UI.Helpers.Custom;

namespace StockHaus.UI.Controllers
{
    public class HomeController : Controller
    {
        ServicePoints point;
        public HomeController()
        {
            point = new ServicePoints();
        }
        //
        // GET: /Home/

        public ActionResult Index()
        {
            if (Session["Email"] != null)
            {
                string _email = Session["Email"].ToString();

                int _role = point.UserService.CheckRoleFromMail(_email);

                if (_role == 1000)
                {
                    return RedirectToAction("Index", new RouteValueDictionary(
                              new
                              {
                                  area = "Users",
                                  controller = "Panel",
                                  action = "Index" //, Id = 1000 
                              }));
                }
                else if (_role == 2000)
                {
                    return RedirectToAction("Index", new RouteValueDictionary(
                              new
                              {
                                  area = "Admin",
                                  controller = "Panel",
                                  action = "Index" //, Id = 2000 
                              }));
                }
                else if (_role == 3000)
                {
                    return RedirectToAction("Index", new RouteValueDictionary(
                             new
                             {
                                 area = "Users",
                                 controller = "Panel",
                                 action = "Index" //, Id = 3000
                             }));
                }
                else if (_role == 4000)
                {
                    return RedirectToAction("Index", new RouteValueDictionary(
                             new
                             {
                                 area = "AreaController",
                                 controller = "Panel",
                                 action = "Index" //, Id = 4000 
                             }));
                }

            }
            return View();
        }

        //[Conn]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(LoginModel model)
        {
            if (HtmlHelpers.IsConnectedToInternet())
            {
                bool _isIt = point.UserService.AccountIsActive(model.Email, model.Password);

                // giriş yapmaya çalışan kişi kullanıcı ise
                if (_isIt)
                {
                    // is online arttırılır
                    bool isIncrease = point.UserService.UpdateBeforeOnline(model);

                    if (isIncrease)
                    {
                        int _role = point.UserService.CheckRoleFromMail(model.Email);
                        // kullanıcı var
                        model.SessionGUID = Guid.NewGuid();
                        Session["SessionGUID"] = model.SessionGUID;
                        Session["Email"] = model.Email;
                        Session["Id"] = Session.SessionID;

                        // daha sonra kullanıcının rolün e göre bir yönlendirme yaparız
                        if (_role == 1000)
                        {
                            return RedirectToAction("Index", new RouteValueDictionary(
                                                          new
                                                          {
                                                              area = "Users",
                                                              controller = "UserHome",
                                                              action = "Index" //, Id = 1000 
                                                          }));
                        }
                        else if (_role == 2000)
                        {
                            // sayfanın get methoduna gider
                            //return RedirectToAction("Index", "Panel", new { area = "Admin" });
                            return RedirectToAction("Index", new RouteValueDictionary(
                                                          new
                                                          {
                                                              area = "Admin",
                                                              controller = "Panel",
                                                              action = "Index" //, Id = 2000 
                                                          }));
                        }
                        else if (_role == 3000)
                        {
                            //return RedirectToAction("Index", "UserHome", new { area = "Users" });
                            return RedirectToAction("Index", new RouteValueDictionary(
                                      new
                                      {
                                          area = "Users",
                                          controller = "Panel",
                                          action = "Index" //, Id = 3000 
                                      }));
                        }
                        else if (_role == 4000)
                        {
                            //return RedirectToAction("Index", "UserHome", new { area = "Users" });
                            return RedirectToAction("Index", new RouteValueDictionary(
                                      new
                                      {
                                          area = "AreaController",
                                          controller = "Panel",
                                          action = "Index" //, Id = 4000
                                      }));
                        }
                        else
                        {
                            return RedirectToAction("Index", new RouteValueDictionary(
                                   new
                                   {
                                       area = "",
                                       controller = "Home",
                                       action = "Index" //, Id = 5000
                                   }));
                        }
                    }
                }
            }
            // değilse ;
            //return Json(new { authen = false , message = "Böyle birşey yok ama olabilirde.." });
            @TempData["Login"] = "False";
            return RedirectToAction("Index");
        }
    }
}
