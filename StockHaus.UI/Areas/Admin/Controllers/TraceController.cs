using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using StockHaus.Core.BaseService;
using PagedList;
using PagedList.Mvc;
using StockHaus.ModelClass.AdminPage;
using StockHaus.ModelClass.AreaController;
using StockHaus.ModelClass.UserPage;

namespace StockHaus.UI.Areas.Admin.Controllers
{
    public class TraceController : Controller
    {
        ServicePoints point;
        public TraceController()
        {
            point = new ServicePoints();
        }
        //
        // GET: /Admin/Trace/

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public PartialViewResult InventoryLog(string id)
        {
            int idx = int.Parse(id);

            var _matlist = point.InventoryService.InvToList();

            if (_matlist.Count < 51)
            {
                if (_matlist.Count == 0)
                {
                    return null;
                }
                // 21 den az ise belge sayısı olduğu kadar ı gösterilecek 9 sa 9 u gibi
                var _list = _matlist.OrderByDescending(x => x.ID).ToPagedList(idx, _matlist.Count);
                return PartialView("_PartialLogMaterial", _list);
            }
            // birinci sayfa açılsın,20 tane göstersin anlamında
            var _list2 = _matlist.OrderByDescending(x => x.ID).ToPagedList(idx, 50);

            return PartialView("_PartialLogMaterial", _list2);
        }

        [HttpPost]
        public PartialViewResult TraceStockCount()
        {
            // her bir depo ve stok yeri içinde sayılmış malzemelerin toplam malzemelere olan oranı belirlenir.
            IList<ShowPercent> sper = new List<ShowPercent>();

            var _cList = point.InventoryService.CountedWareStockPlace();

            // _cList depo ve stok yerlerinden oluşmalı
            foreach (var item in _cList)
            {

                int _countedMaterialX = point.InventoryService.CountedMaterial(item._ware, item._stock);
                int _totalMaterialX = point.MaterialService.GetTotalAmount(item._ware, item._stock);
                int _countApprovedX = point.InventoryService.CountedApproved(item._ware, item._stock);


                double _counted = Convert.ToDouble(_countedMaterialX) / Convert.ToDouble(_totalMaterialX);
                double _appreoved = Convert.ToDouble(_countApprovedX) / Convert.ToDouble(_totalMaterialX);

                ShowPercent spp = new ShowPercent
                {
                    WareHouse = item._ware,
                    StockPlace = item._stock,
                    InventPercent = _counted,
                    ApprovedPercent = _appreoved
                };
                sper.Add(spp);
            }

            return PartialView("_PartialCompletePercent", sper);
        }

        [HttpGet]
        public PartialViewResult InventApprovedMat(int id)
        {
            string _sessionEmail = Session["Email"].ToString();

            var _list = point.InventoryService.ApprovedMaterials();//.GetListMaterial(_sessionEmail, model._matCode, model._matSection, model._matSectionPlace, model._IsInclude);

            if (_list.Count < 51)
            {
                if (_list.Count == 0)
                {
                    return null;
                }
                // 21 den az ise belge sayısı olduğu kadar ı gösterilecek 9 sa 9 u gibi
                var _listX = _list.OrderByDescending(x => x.ID).ToPagedList(id, _list.Count);
                return PartialView("_PartialAppRsltShow", _listX);
            }
            // birinci sayfa açılsın,50 tane göstersin anlamında
            var _list2 = _list.OrderByDescending(x => x.ID).ToPagedList(id, 50);

            return PartialView("_PartialAppRsltShow", _list2);
        }

        [HttpPost]
        public JsonResult ApproveInventMaterial(MissionAssign model)
        {
            // malzeme kodu indisi seri no su ve sayım tablosundaki ID bilgisi alınmalı
            // alınan ID bilgisi İLE takım no suna ulaşılır,buna göre onaylananın ISAPPROVED u 1 olacak,
            // onaylanmayan malzemelerin isactive durumu ise 0 olacak

            foreach (var itemx in model._matList)
            {

                // onaylanacak olan malzemenin invent id si
                var _matContent = point.InventoryService.GetMaterial(itemx._inventId);
                int _ms = 0;

                foreach (var item in _matContent)
                {
                    List<string> _invCodeList = point.InventoryService.FindInventCode(item.MatCode, item.MatIndex, item.MatSpecialStock, item.MatSerialNumber, item.MatSection, item.MatSectionPlace);

                    foreach (var c in _invCodeList)
                    {
                        string[] _data = c.Split('#');
                        int _DataID = int.Parse(_data[0]);
                        string _InventCode = _data[1];

                        InventoriesModel invt = new InventoriesModel
                        {
                            ID = _DataID,
                            ApprovedPers = Session["Email"].ToString(),
                            ApprovedDate = DateTime.Now,
                            IsApproved = true
                        };

                        if (item.ID == _DataID)
                        {
                            // onaylanacak olan malzemenin id si sayım listesinden gelen malzemenin idsine eşitse burada isapprove 1 yapılır
                            _ms = point.InventoryService.UpdateMaterial(invt);
                            if (_ms != 1)
                            {
                                return Json("false");
                            }
                        }
                        else
                        {
                            // onaylanacak olan malzemenin id si sayım listesinden gelen malzemenin idsine eşitse burada isactive 0 yapılır
                            _ms = point.InventoryService.UpdateIsActiveStatus(invt);
                            if (_ms != 1)
                            {
                                return Json("false");
                            }
                        }

                    }

                }
            }

            return Json("true");
        }


        // 3.takımı görevlendir ve material tablosuunda bu verilerin IsCount UNU 1 yap
        [HttpPost]
        public JsonResult AssignAndPassiveMaterial(MissionAssign model)
        {
            string _depo = model._ware;
            string _stokyeri = model._stockPlace;

            //int storeId = point.StoreService.GetStoreID(_depo, _stokyeri);

            // o store id de görevli olan personellerin bilgisi
            //var _biggerStoreId = point.InventoryGroupService.


            // material tablosunda iscount ların 1 yapılması
            foreach (var item in model._matList)
            {
                string _sr = "";
                if (item._serNo == null)
                {
                    _sr = "";
                }
                else
                {
                    _sr = item._serNo;
                }
                MaterialsModel _m = new MaterialsModel
                {
                    MatCode = item._matCode,
                    MatIndex = item._matIndis,
                    MatSerialNumber = _sr,
                    MatSection = model._ware,
                    MatSectionPlace = model._stockPlace
                };
                string _msg = point.MaterialService.UpdateIsCount(_m);
                if (_msg != "true")
                {
                    return Json("false");
                }
            }

            return Json("success");
        }

        [HttpPost]
        public ActionResult GetListMaterial(WareStock model)
        {
            var _matList = point.InventoryService.GetMaterialList(model._ware, model._stock,model._endStockPlace); //.GetMatList(model._ware, model._stock);

            // birinci sayfa açılsın,12 tane göstersin anlamında
            var _list2 = _matList.OrderByDescending(x => x.MatSection).ThenByDescending(c => c.MatSectionPlace).ThenByDescending(v => v.MatCode).ThenByDescending(b => b.MatIndex).ToList();//.ToPagedList(1, 30);

            return PartialView("_PartialListMaterial", _list2);
        }

     

        [HttpPost]
        public JsonResult calcInventMaterial()
        {
            string result = "";

            int _penCount = point.InventoryService.calcPenCount();
            double _totalMatCount = point.InventoryService.calcTotalMatCount();

            result = _penCount + "#" + _totalMatCount;

            return Json(result);
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
    }
}
