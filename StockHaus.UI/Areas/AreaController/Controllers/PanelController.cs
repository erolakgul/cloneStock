using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using StockHaus.Core.BaseService;
using StockHaus.ModelClass.AdminPage;
using StockHaus.ModelClass.AreaController;
using PagedList;
using PagedList.Mvc;
using System.Web.Routing;
using System.Web.Security;
using StockHaus.UI.Helpers.Custom;

namespace StockHaus.UI.Areas.AreaController.Controllers
{
    public class PanelController : Controller
    {
        ServicePoints point;
        public PanelController()
        {
            point = new ServicePoints();
        }
        //
        // GET: /AreaController/Panel/

        //[Auth]
        //[Auth(AnonymousMinutesTimeout = 10)]
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
                            action = "Index" //, Id = 4000 
                        }));
                }
                else if (_role == 4000)
                {
                    // eğer
                    bool _isonline = point.UserService.isOnline(Session["Email"].ToString());

                    if (!_isonline)
                    {
                        SignOut();
                        return RedirectToAction("Index", "Home", new { area = "" });
                    }
                    return View();
                }

            }

            return RedirectToAction("Index", new RouteValueDictionary(
                             new
                             {
                                 area = "",
                                 controller = "Home",
                                 action = "Index" //, Id = 3000
                             }));
        }

        [HttpPost]
        public ActionResult GetPersonel(string id)
        {
            List<UserModel> _list = point.UserService.GetPersonel();

            return PartialView("_PartialSignAuth", _list);
        }

        [HttpPost]
        public ActionResult AuthSign(string id)
        {
            // id == isim soyisim 
            string _mail = point.UserService.GetMailForName(id);

            bool _auth = point.UserService.UpdateAuth(_mail);

            return Json(_auth);
        }

        [HttpPost]
        public JsonResult CheckedMissionPlace(string id)
        {
            var _listStoreId = point.InventoryGroupService.GetStoreListID(id);

            // dönen store id leri store da aratıp depo ve stokyerlerini alıyoruz
            List<WareStock> _listStore = new List<WareStock>();

            foreach (var item in _listStoreId)
            {
                var _depoStok = point.StoreService.GetStorePlaceNoForID(item._id);

                foreach (var items in _depoStok)
                {
                    WareStock _d = new WareStock
                    {
                        _ware = items.WareHouse,
                        _stock = items.StockPlace,  // start
                        _endStockPlace = item._endStockPlace // end
                    };
                    _listStore.Add(_d);
                }
            }

            return Json(_listStore);
        }

        [HttpPost]
        public ActionResult CheckActive(string mail)
        {
            bool _isonline = point.UserService.isOnline(mail);

            return Json(_isonline);
        }

        [HttpPost]
        public ActionResult CancelApprovedMaterial(MissionAssign dto)
        {
            // onaylanmışı iptal eder
            List<InventoriesModel> CancelledModels = point.InventoryService.Cancel(dto);

            // onayları iptal edilenlerin material tablosundaki iscounted kolonu 1 den 0 a alınır
            string _canMessage = point.MaterialService.CancelIsCounted(CancelledModels);

            // iptal edilenleri loglar
            string msg = point.CancelledInventoryService.BeforeInsert(CancelledModels);

            if (msg == "True")
            {
                return Json("success");
            }
            return Json("false");
        }

        [HttpGet]
        public ActionResult GetListApproveMaterial(WareStock model)
        {
            var _matList = point.InventoryService.GetMaterialList(model._ware, model._stock, model._endStockPlace); //.GetMatList(model._ware, model._stock);

            // birinci sayfa açılsın,12 tane göstersin anlamında
            var _list2 = _matList.OrderByDescending(x => x.MatSection).ThenByDescending(c => c.MatSectionPlace).ThenByDescending(v => v.MatCode).ThenByDescending(b => b.MatIndex).ToList();//.ToPagedList(1, 30);

            return PartialView("_PartialListApproveMaterial", _list2);
        }

        [HttpGet]
        public ActionResult OpenAppMaterialPage(string id)
        {
            SetStockStoreForTrace();
            SetStockPlaceForTrace();
            return PartialView("_PartialChooseWarehouseForAppMat");
        }

        [HttpPost]
        public ActionResult GetListAppMaterial(WareStock model)
        {
            TempData["DepoStockCancel"] = model._ware + "X" + model._stock + "X" + model._endStockPlace;
            ViewBag.DepoStockCancel = TempData["DepoStockCancel"].ToString();

            var _matList = point.InventoryService.GetApprovedMaterialList(model._ware, model._stock, model._endStockPlace); //.GetMatList(model._ware, model._stock);
            var _order = _matList.OrderByDescending(x => x.MatSection).ThenByDescending(c => c.MatSectionPlace).ThenByDescending(v => v.MatCode).ThenByDescending(b => b.MatIndex).ToList();

            if (_matList.Count < 100)
            {
                if (_matList.Count == 0)
                {
                    return PartialView("_PartialListApproveMaterial", null);
                }
                var list = _order.ToPagedList(1, _matList.Count);
                return PartialView("_PartialListApproveMaterial", list);
            }
            var list2 = _order.ToPagedList(1, 100);
            return PartialView("_PartialListApproveMaterial", list2);
        }

        [HttpGet]
        public ActionResult GetListAppMaterialOrder(string id)
        {
            string[] array = id.Split('X');
            string _depo = array[0];
            string _stSy = array[1];
            string _eDSy = array[2];
            int _id = int.Parse(array[3]);

            TempData["DepoStockCancel"] = _depo + "X" + _stSy + "X" + _eDSy;
            ViewBag.DepoStockCancel = TempData["DepoStockCancel"].ToString();

            var _matList = point.InventoryService.GetApprovedMaterialList(_depo, _stSy, _eDSy); //.GetMatList(model._ware, model._stock);
            var _order = _matList.OrderByDescending(x => x.MatSection).ThenByDescending(c => c.MatSectionPlace).ThenByDescending(v => v.MatCode).ThenByDescending(b => b.MatIndex).ToList();
            var list2 = _order.ToPagedList(_id, 100);
            return PartialView("_PartialListApproveMaterial", list2);
        }

        [HttpPost]
        public ActionResult GetListMaterial(WareStock model)
        {
            TempData["DepoStock"] = model._ware + "X" + model._stock + "X" + model._endStockPlace;
            ViewBag.DepoStock = TempData["DepoStock"].ToString();

            //.GetMaterialList(model._ware, model._stock); //
            var _matList = point.InventoryService.GetMaterialList(model._ware, model._stock, model._endStockPlace); //.GetMatList(model._ware, model._stock);
            var _order = _matList.OrderByDescending(x => x.MatSection).ThenByDescending(c => c.MatSectionPlace).ThenByDescending(v => v.MatCode).ThenByDescending(b => b.MatIndex).ToList();

            if (_matList.Count < 50)
            {
                if (_matList.Count == 0)
                {
                    return PartialView("_PartialListMaterial", null);
                }
                var list = _order.ToPagedList(1, _matList.Count);
                return PartialView("_PartialListMaterial", list);
            }

            //var _list2 = _matList.OrderByDescending(x => x.MatSection).ThenByDescending(c => c.MatSectionPlace).ThenByDescending(v => v.MatCode).ThenByDescending(b => b.MatIndex).ToList();
            var list2 = _order.ToPagedList(1, 50);
            return PartialView("_PartialListMaterial", list2);
        }

        [HttpGet]
        public ActionResult GetListMaterialOrder(string id)
        {
            string[] array = id.Split('X');
            string _depo = array[0];
            string _stSy = array[1];
            string _eDSy = array[2];
            int _id = int.Parse(array[3]);

            TempData["DepoStock"] = _depo + "X" + _stSy + "X" + _eDSy;
            ViewBag.DepoStock = TempData["DepoStock"].ToString();

            var _matList = point.InventoryService.GetMaterialList(_depo, _stSy, _eDSy); //.GetMatList(model._ware, model._stock);
            var _order = _matList.OrderByDescending(x => x.MatSection).ThenByDescending(c => c.MatSectionPlace).ThenByDescending(v => v.MatCode).ThenByDescending(b => b.MatIndex).ToList();
            var list2 = _order.ToPagedList(_id, 50);
            return PartialView("_PartialListMaterial", list2);
        }

        // depo yeri seçildikten sonra stok yerini doldurur
        [HttpPost]
        public JsonResult SelectStockPlace(string id)
        {
            var _stockPlaceList = point.StoreService.SelectStockPlace(id);

            return Json(_stockPlaceList);
        }

        [HttpGet]
        public ActionResult OpenAppPage(int id)
        {
            SetStockStoreForTrace();
            SetStockPlaceForTrace();
            return PartialView("_PartialChooseWarehouse");
        }

        private void SetStockStoreForTrace(object storeNo = null)
        {
            var _stockStoreList = point.StoreService.SelectStockStoreNo();
            var _selectList = new SelectList(_stockStoreList, storeNo);

            ViewData.Add("_ware", _selectList);
        }

        private void SetStockPlaceForTrace(object placesNo = null)
        {
            var _stockPlaceList = point.StoreService.SelectStockPlaceNo();
            var _selectList = new SelectList(_stockPlaceList, placesNo);
            ViewData.Add("_stock", _selectList);
            ViewData.Add("_endStockPlace", _selectList);
        }

        public ActionResult SignOut()
        {
            if (Session["Email"] != null)
            {
                string _name = Session["Email"].ToString();

                int _userId = point.UserService.GetID(_name);

                string _message = point.UserService.UpdateOnline(_userId, 0);

                if (_message != "True")
                {
                    ModelState.AddModelError("error_msg", _message);
                    return RedirectToAction("Index", "Panel", new { area = "Admin" });
                }
            }
            // session sonlandırma
            FormsAuthentication.SignOut();
            Session.Abandon();
            //çıkış yaptırdıktan sonra direk olarak login sayfasına geri gönderiyoruz
            return RedirectToAction("Index", "Home", new { area = "" });
        }
    }
}
