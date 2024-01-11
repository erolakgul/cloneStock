using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using StockHaus.Core.BaseService;
using PagedList;
using PagedList.Mvc;
using StockHaus.UI.Areas.Users.Models;
using StockHaus.ModelClass.AdminPage;
using StockHaus.ModelClass.UserPage;
using System.Web.Routing;

namespace StockHaus.UI.Areas.Users.Controllers
{
    public class PanelController : Controller
    {
        ServicePoints point;
        public PanelController()
        {
            point = new ServicePoints();
        }
        //
        // GET: /Users/Panel/

        public ActionResult Index()
        {
            if (TempData["Material"] != null)
            {
                ViewBag.Material = TempData["Material"].ToString();
            }

            if (TempData["DefMat"] != null)
            {
                ViewBag.DefMat = TempData["DefMat"].ToString();
            }

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
                    // eğer
                    bool _isonline = point.UserService.isOnline(Session["Email"].ToString());

                    if (!_isonline)
                    {
                        SignOut();
                        return RedirectToAction("Index", "Home", new { area = "" });
                    }

                    return View();
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

            return RedirectToAction("Index", new RouteValueDictionary(
                             new
                             {
                                 area = "",
                                 controller = "Home",
                                 action = "Index" //, Id = 3000
                             }));

        }

        [HttpPost]
        public ActionResult CheckActive(string mail)
        {
            bool _isonline = point.UserService.isOnline(mail);

            return Json(_isonline);
        }

        [HttpPost]
        public ActionResult IsSign(string id)
        {
            bool _isSign = point.UserService.IsCanSignPersonel(id);

            return Json(_isSign);
        }

        [HttpPost]
        public ActionResult EnterMaterial(AddInvent model)
        {
            //  sayım kodları   DEPO NO + STOK YERİ NO + "AA" + takım numarası + "AA" + group numarası şeklinde verilen rakamlardan oluşur
            // ÖRNEĞİN 300 101 deposunu takım 1 group 1 den birileri sayacaksa numara 300A101A1A1 şeklinde bir kod oluşturur

            int _last = point.InventCodeCountService.GetTeamInventCode(model._teamNo.ToString(), model._groupNo.ToString());

            string _inventCode = _last + "X" + model._teamNo + "X" + model._groupNo;

            // alanlar boş ise geri gönderiyoruz
            if (model._matIndex == null || model._mCode == null || model._matSpecial == null) //|| model._quantity == 0
            {
                TempData["Material"] = "Boş";
                //return RedirectToAction("Index", "Panel");
                return Json(new { msg = "Boş" }, JsonRequestBehavior.AllowGet);
            }

            string _matSerial = "";

            // o malzemenin seri numarası varsa eski sistemde ve giriş yapılırkende girilmediyse geri gönderiyoruz

            string _IsIn = point.MaterialService.CheckMatSerialNumber(model._matIndex, model._mCode, model._stockStore, model._stockPlace, model._matSeriNum);

            bool _serNoVarMi = false; //point.MaterialService.CheckMatSerialNumberAll(model._matIndex, model._mCode, model._stockStore, model._stockPlace, model._matSeriNum);

            if (model._warningNum == "1")
            {
                _serNoVarMi = point.MaterialService.CheckMatSerialNumberAll("X9X9", model._mCode, model._stockStore, model._stockPlace, model._matSeriNum);
                bool _sss = point.MaterialService.CheckMatSerialNumberAll(model._matIndex, model._mCode, model._stockStore, model._stockPlace, model._matSeriNum);

                if (_sss)
                {
                    TempData["Material"] = "Have";
                    //return RedirectToAction("Index", "Panel");
                    return Json(new { msg = "Have" }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                _serNoVarMi = point.MaterialService.CheckMatSerialNumberAll(model._matIndex, model._mCode, model._stockStore, model._stockPlace, model._matSeriNum);
            }

            if (!_serNoVarMi)
            {
                TempData["Material"] = "UnAccepted";
                // return RedirectToAction("Index", "Panel");
                return Json(new { msg = "UnAccepted" }, JsonRequestBehavior.AllowGet);
            }

            if (model._matSeriNum != null)
            {
                model._quantity = 1;
                _matSerial = model._matSeriNum.Trim().ToUpper();
            }
            else // seri numarası boş gönderildiyse
            {    // ve de o malzemenin seri numarası varsa
                _matSerial = "";
                if (_IsIn != "")
                {
                    TempData["Material"] = "SerNoBoş";
                    //return RedirectToAction("Index", "Panel");
                    return Json(new { msg = "SerNoBoş" }, JsonRequestBehavior.AllowGet);
                }
            }

            if (_IsIn != _matSerial)
            {
                TempData["Material"] = "UnAccepted";
                //return RedirectToAction("Index", "Panel");
                return Json(new { msg = "UnAccepted" }, JsonRequestBehavior.AllowGet);
            }

            string _matIndex = model._matIndex.Trim().ToUpper();


            string _materialName = point.MaterialService.GetNameEnterMaterial(model._mCode.ToString(), model._stockStore, model._stockPlace);

            bool _IsCountedMaterial = point.InventoryService.IsCounted(_inventCode, model._mCode, _matIndex, _matSerial, model._stockStore, model._stockPlace);

            // true ise bu malzeme daha önce o takım ve grup tarafından sayılmış demektir
            if (_IsCountedMaterial)
            {
                TempData["Material"] = "Sayılmış";
                //return RedirectToAction("Index", "Panel");
                return Json(new { msg = "Sayılmış" }, JsonRequestBehavior.AllowGet);
            }

            // ilk olarak o malzemenin materials tablosunda kaydı var mı ona bakılır.
            // eğer o malzeme o depo ve stok yerinde yok ise kaydettirilmez erol akgül e bilgi vermesi istenir.
            // true ise kaydettir,false ise uyarı ver
            // bool _IsHasMaterialInStockPlace = point.MaterialService.IsHasMaterial(model._mCode, _matIndex);

            // 20 KASIM 2017 SONRASI kapatıldı , farklı indis girilebilir oldu
            //if (!_IsHasMaterialInStockPlace)
            //{
            //    TempData["Material"] = "Yok";
            //    return RedirectToAction("Index", "Writer");
            //}

            // kişinin malzemeyi kaydetmesine izin vermeden önce inventoryapproves ta yani onaylanmış tabloda
            // invetory code var mı yok mu diye kontrol ediyoruz. eğer varsa ve onaylanmışsa malzemeyi kaydettirmiyoruz
            // true ise onaylanmış bir sayım kodu var demektir kaydettirmiyoruz.
            bool _IsApprovedInventory = point.InventApproveService.IsApproved(_inventCode);

            if (_IsApprovedInventory)
            {
                TempData["Material"] = "Onaylanmış";
                //return RedirectToAction("Index", "Panel");
                return Json(new { msg = "Onaylanmış" }, JsonRequestBehavior.AllowGet);
            }

            string _serNo = "";

            if (model._matSeriNum != null)
            {
                _serNo = model._matSeriNum;
            }

            // 3.takımın saydığı malzeme eğer daha önce sayıldıysa
            // 3.takımın saydığı geçerli olacağı için daha önce sayılmış olan o malzemenin isapprove u 1 ise 0 a çekilir
            if (int.Parse(model._teamNo) == 3)
            {
                //string strAppPersName = Session["Email"].ToString();
                string _sessionEmail = Session["Email"].ToString();

                //bool _varMi = point.InventoryService.IsCounted(_inventCode, model._mCode, _matIndex, _matSerial);

                //int _onaySil = point.InventoryService.RemoveApproveX(_sessionEmail, _inventCode, model._mCode, model._matIndex, model._matSeriNum, model._stockStore, model._stockPlace);

                if (model._warningNum == "1")
                {
                    MaterialsModel mt = new MaterialsModel
                    {
                        MatCode = model._mCode,
                        MatName = _materialName,
                        MatSection = model._stockStore,
                        MatSectionPlace = model._stockPlace,
                        MatQuantity = model._quantity,
                        MatIndex = _matIndex,
                        MatSpecialStock = model._matSpecial,
                        MatSerialNumber = _serNo,
                        IpAddress = Helpers.HtmlHelpers.GetIpHelper(),
                        IsActive = true,
                        MatSerNoType = "",
                        MatType = model._matType,
                        IsCounted = "0",
                        ChangeDate = DateTime.Now,
                        ChangedBy = Session["Email"].ToString(),
                        Company = "01",
                        CreateDate = DateTime.Now,
                        CreatedBy = Session["Email"].ToString()
                    };

                    string ms = point.MaterialService.BeforeInsert(mt);

                    if (ms != "True")
                    {
                        TempData["Material"] = "Başarısız";
                        // return RedirectToAction("Index", "Panel");
                        return Json(new { msg = "Başarısız" }, JsonRequestBehavior.AllowGet);
                    };
                }

                InventoriesModel inv = new InventoriesModel
                {
                    InventoryCode = _inventCode,
                    MatCode = model._mCode,
                    MatName = _materialName,
                    MatSection = model._stockStore,
                    MatSectionPlace = model._stockPlace,
                    MatQuantity = model._quantity,
                    MatIndex = _matIndex,
                    MatSpecialStock = model._matSpecial,
                    MatSerialNumber = _serNo,
                    IpAddress = Helpers.HtmlHelpers.GetIpHelper(),
                    CreateDate = DateTime.Now,
                    IsSigned = false,
                    SignedTime = DateTime.Parse("1900.01.01"),
                    ApprovedPers = _sessionEmail,
                    ApprovedDate = DateTime.Now,
                    IsApproved = true,
                    IsActive = true,
                    ChangeDate = DateTime.Now,
                    ChangedBy = Session["Email"].ToString(),
                    Company = "01",
                    CreatedBy = Session["Email"].ToString(),
                    SignedBy = ""
                };

                string _passive = point.InventoryService.BeforePassive(inv);

                string mst = point.InventoryService.BeforeInsert(inv);

                if (mst != "True") // daha önce aynı depo/stok yerinde sayımı yapılmış ve aktifliği 1 olan malzeme varsa false döner
                {                          // inventory tablosunda o malzeme kaydına ilişkin aktif liği false yaparsan sorun düzelir
                    TempData["Material"] = "Sayılmış";
                    //return RedirectToAction("Index", "Panel");
                    return Json(new { msg = "Sayılmış" }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                if (model._warningNum == "1")
                {
                    MaterialsModel mt = new MaterialsModel
                    {
                        MatCode = model._mCode,
                        MatName = _materialName,
                        MatSection = model._stockStore,
                        MatSectionPlace = model._stockPlace,
                        MatQuantity = model._quantity,
                        MatIndex = _matIndex,
                        MatSpecialStock = model._matSpecial,
                        MatSerialNumber = _serNo,
                        IpAddress = Helpers.HtmlHelpers.GetIpHelper(),
                        IsActive = true,
                        MatSerNoType = "",
                        MatType = model._matType,
                        IsCounted = "0",
                        ChangeDate = DateTime.Now,
                        ChangedBy = Session["Email"].ToString(),
                        Company = "01",
                        CreateDate = DateTime.Now,
                        CreatedBy = Session["Email"].ToString()
                    };

                    string ms = point.MaterialService.BeforeInsert(mt);

                    if (ms != "True")
                    {
                        TempData["Material"] = "Başarısız";
                        //return RedirectToAction("Index", "Panel");
                        return Json(new { msg = "Başarısız" }, JsonRequestBehavior.AllowGet);
                    };

                }

                // 1 ve 2.takım saydıysa
                InventoriesModel invt = new InventoriesModel
                {
                    InventoryCode = _inventCode,
                    MatCode = model._mCode,
                    MatName = _materialName,
                    MatSection = model._stockStore,
                    MatSectionPlace = model._stockPlace,
                    MatQuantity = model._quantity,
                    MatIndex = _matIndex,
                    MatSpecialStock = model._matSpecial,
                    MatSerialNumber = _serNo,
                    IpAddress = Helpers.HtmlHelpers.GetIpHelper(),
                    CreateDate = DateTime.Now,
                    IsSigned = false,
                    SignedTime = DateTime.Parse("1900.01.01"),
                    ApprovedPers = "",
                    ApprovedDate = DateTime.Parse("1900.01.01"),
                    IsApproved = false,
                    ChangeDate = DateTime.Now,
                    ChangedBy = Session["Email"].ToString(),
                    Company = "01",
                    CreatedBy = Session["Email"].ToString(),
                    IsActive = true,
                    SignedBy = ""
                };

                string mst = point.InventoryService.BeforeInsert(invt);

                if (mst != "True") // daha önce aynı depo/stok yerinde sayımı yapılmış ve aktifliği 1 olan malzeme varsa false döner
                {                          // inventory tablosunda o malzeme kaydına ilişkin aktif liği false yaparsan sorun düzelir
                    TempData["Material"] = "Sayılmış";
                    //return RedirectToAction("Index", "Panel");
                    return Json(new { msg = "Sayılmış" }, JsonRequestBehavior.AllowGet);
                }
            }
            TempData["Material"] = "Başarılı";
            return Json(new { msg = "Başarılı" }, JsonRequestBehavior.AllowGet);//RedirectToAction("Index", "Panel");
        }

        [HttpPost]
        public ActionResult UpdateMaterial(AddInvent model)
        {
            string _matIndex = model._matIndex.ToUpper();

            var _isApproved = point.InventoryService.MatIsApproved(model._id);//İSACTİVE TRUE

            foreach (var item in _isApproved)
            {
                if (item.IsApproved)
                {   // MAVİ
                    //1002X3X1
                    string _cs = item.InventoryCode.Substring((item.InventoryCode.Length - 4), 2);
                    string _xs = _cs.Replace("X", "");

                    if (_xs != "3")
                    {
                        TempData["Material"] = "TEKLİONAY";
                        return Json(new { msg = "SingularApprove" }, JsonRequestBehavior.AllowGet);//RedirectToAction("Index", "Writer");
                    }
                }
                else if (!item.IsApproved && item.IsActive && item.ApprovedPers == "")
                {   // SARI
                    // burada material tablosu kontrol edilir
                    // iscounted 1 ise gene işleme devam ettirmeyiz
                    string _isCounted = point.MaterialService.IsCountedStatus(item.MatCode, item.MatIndex, item.MatSerialNumber, item.MatSpecialStock, item.MatSection, item.MatSectionPlace);

                    if (_isCounted == "1")
                    {
                        TempData["Material"] = "PASSIVE";
                        return Json(new { msg = "PASSIVE" }, JsonRequestBehavior.AllowGet);
                    }
                }
            }

            if (_isApproved.Count == 0)
            {   // ISAPPROVED FALSE,APPROVING PERSON DOLU
                //BORDO
                TempData["Material"] = "REDD";
                return Json(new { msg = "REDD" }, JsonRequestBehavior.AllowGet);
            }

            string _serN = "";
            if (model._matSeriNum == null)
            {
                _serN = "";
            }
            else
            {
                _serN = model._matSeriNum.Trim().ToUpper();
            }

            InventoriesModel inv = new InventoriesModel
            {
                ID = model._id,
                MatQuantity = model._quantity,
                MatIndex = _matIndex,
                MatSpecialStock = model._matSpecial,
                MatSerialNumber = _serN
            };

            int ms = point.InventoryService.UpdateInventory(inv);

            if (ms == 1)
            {
                TempData["Material"] = "Başarılı";
                return Json(new { msg = "Success" }, JsonRequestBehavior.AllowGet);
            }
            TempData["Material"] = "Başarısız";
            return Json(new { msg = "Unsuccess" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult DelMaterial(int id)
        {

            var _willDeleteMat = point.MaterialService.FindModelByID(id);

            string _sessionEmail = "";

            if (Session["Email"] == null)
            {
                return RedirectToAction("Index", "Home", new { area = "" });
            }

            _sessionEmail = Session["Email"].ToString();

            string _name = point.UserService.GetNameFromMail(_sessionEmail);
            TeamGroup tgModel = point.UserService.GetTeamAndGroup(_sessionEmail);

            int _IsInInventoryTableMatId = point.InventoryService.FindMaterialFromMatId(_willDeleteMat, tgModel);

            int sil = point.InventoryService.DeleteInventRecord(_IsInInventoryTableMatId, _sessionEmail);

            var _InventResultList = point.InventoryService.GetInventResultFromId(id);

            if (_InventResultList.Count < 31)
            {
                if (_InventResultList.Count == 0)
                {
                    return PartialView("_PartialInventResult");
                }
                var _list = _InventResultList.OrderByDescending(x => x.CreateDate).ToPagedList(1, _InventResultList.Count);
                return PartialView("_PartialInventResult", _list);

            }

            var _list2 = _InventResultList.OrderByDescending(x => x.CreateDate).ToPagedList(1, 30);
            return PartialView("_PartialInventResult", _list2);
        }

        [HttpPost]
        public ActionResult DefineMaterial(EnterMaterial model)
        {
            string _matTypeUpper = "";

            _matTypeUpper = model._matType.ToUpper();

            if (_matTypeUpper != "AD" && _matTypeUpper != "KG" && model._matType.ToUpper() != "MT")
            {
                TempData["DefMat"] = "TipFalse";
                return RedirectToAction("Index", "Panel");
            }

            var _StockPlaceIsInStore = point.StoreService.DetectStockPlace(model._stockStore, model._stockPlace);

            int _count = 0;

            foreach (var item in _StockPlaceIsInStore)
            {
                if (item.StockPlace == model._stockPlace)
                {
                    _count = _count + 1;
                }
            }

            if (_StockPlaceIsInStore.Count == 0)
            {
                TempData["DefMat"] = "WareHouse";
                return RedirectToAction("Index", "Panel");
            }
            if (_count == 0)
            {
                TempData["DefMat"] = "StockPlace";
                return RedirectToAction("Index", "Panel");
            }


            string _mtp = "";
            double _quan = 0;
            string _sn = "";

            if (!String.IsNullOrEmpty(model._matSeriNum))
            {
                _mtp = "2";
                _quan = 1;
                _sn = model._matSeriNum.Trim().ToUpper();
            }
            else
            {
                _mtp = "0";
                _quan = model._quantity;
            }


            // EKLENECEK OLAN MALZEME material tablosunda zaten var mı ?

            string _mcode = model._mCode.Trim();
            string _ind = model._matIndex.Trim().ToUpper();
            string _str = model._stockStore.Trim();
            string _plc = model._stockPlace.Trim();

            bool _isInclude = point.MaterialService.CheckMaterial(_mcode, _ind, model._matSpecial, _sn, _str, _plc);

            if (_isInclude)
            {
                TempData["DefMat"] = "FULL";
                return RedirectToAction("Index", "Panel");
            }

            MaterialsModel dto = new MaterialsModel
            {
                IpAddress = Helpers.HtmlHelpers.GetIpHelper(),
                MatCode = _mcode,
                MatIndex = _ind,
                MatName = model._matName,
                MatQuantity = _quan,
                MatSection = _str,
                MatSectionPlace = _plc,
                MatSerialNumber = _sn,
                MatSpecialStock = model._matSpecial,
                MatType = _matTypeUpper,
                IsActive = true,
                MatSerNoType = _mtp,
                IsCounted = "0",
                ChangeDate = DateTime.Now,
                ChangedBy = Session["Email"].ToString(),
                Company = "01",
                CreateDate = DateTime.Now,
                CreatedBy = Session["Email"].ToString()
            };

            string _ms = point.MaterialService.BeforeInsert(dto);

            string _email = Session["Email"].ToString();
            string _name = point.UserService.GetNameFromMail(_email);
            TeamGroup _teamNo = point.UserService.GetTeamAndGroup(_email);
            string _tg = _teamNo._team + "X" + _teamNo._group;
            string _inventCode = point.InventoryService.GetInventResultFromName(_name, _tg);

            if (_ms == "True")
            {
                //material tablosuna kaydedilebildiyse sayım tablosuna da kaydediyoruz
                InventoriesModel dtox = new InventoriesModel
                {
                    InventoryCode = _inventCode,
                    IpAddress = Helpers.HtmlHelpers.GetIpHelper(),
                    IsActive = true,
                    IsSigned = false,
                    IsApproved = false,
                    CreateDate = DateTime.Now,
                    ApprovedDate = DateTime.Parse("1900.01.01"),
                    ApprovedPers = "",
                    MatCode = _mcode,
                    MatIndex = _ind,
                    MatName = model._matName,
                    MatQuantity = _quan,
                    MatSection = _str,
                    MatSectionPlace = _plc,
                    MatSerialNumber = _sn,
                    MatSpecialStock = model._matSpecial,
                    SignedTime = DateTime.Parse("1900.01.01"),
                    ChangeDate = DateTime.Now,
                    ChangedBy = Session["Email"].ToString(),
                    Company = "01",
                    CreatedBy = Session["Email"].ToString(),
                    SignedBy = ""
                };

                string msg = point.InventoryService.BeforeInsert(dtox);

                if (msg == "True")
                {
                    TempData["DefMat"] = "Success";
                    return RedirectToAction("Index", "Panel");
                }

            }
            TempData["DefMat"] = "Warning";
            return RedirectToAction("Index", "Panel");
        }

        [HttpPost]
        public ActionResult GetMaterialName(string matcode)
        {
            string _name = point.MaterialService.FindMaterialName(matcode);

            if (String.IsNullOrEmpty(_name))
            {
                return Json("False");
            }
            return Json(_name);
        }

        [HttpGet]
        public ActionResult AccountedList(string id)
        {
            string[] _gonderilenVeri = id.Split('-'); // gelen parametreyi 2 ye ayırıp diziye atıyoruz

            string _belgeID = _gonderilenVeri[0]; //belgeid döner
            int _pageID = int.Parse(_gonderilenVeri[1]);  // pageid döner

            string _email = _belgeID; //.Replace('ş', '.');

            var _name = point.UserService.GetNameFromMail(_email);

            var _datalist = point.InventoryGroupService.GetGroupForName(_name);

            string[] _depo = new string[2];
            string[] _takGrp = new string[2];

            foreach (var item in _datalist)
            {
                if (item.PerName != _name)
                {
                    int _storeID = item.StoreID;
                    var _storeIDList = point.StoreService.GetStorePlaceNoForID(_storeID);

                    foreach (var items in _storeIDList)
                    {
                        _depo[0] = items.WareHouse;
                        _depo[1] = items.StockPlace;
                    }
                }
                _takGrp[0] = item.TeamNo.ToString();
                _takGrp[1] = item.GroupNo.ToString();
            }

            // aktif olarak sayım kodunu burada oluştur,o var mı yok mu diye kontrol ettir ve listelet
            // ar0 depo ar1 stok yeri bilgisi -- TAKIM VE GROUP BİLGİSİ LAZIM 
            string _inventCode = _depo[0] + "X" + _depo[1] + "X" + _takGrp[0] + "X" + _takGrp[1];

            // belirtilen depo ve stok yerinde o grubun saydığı malzemeler listelenir.
            var _InventResultList = point.InventoryService.GetItems(_inventCode);

            var _list2 = _InventResultList.OrderByDescending(c => c.CreateDate).ToPagedList(_pageID, 7);

            return PartialView("_PartialInventResult", _list2);
        }

        [HttpPost]
        public ActionResult ShowMaterials(string id)
        {
            // store id ve son raf bilgisi var
            var _storeIDList = point.InventoryGroupService.GetStoreIDS(id);

            string _matCode = "";

            var _matlist = point.MaterialService.GetSearchMaterialS(_matCode, _storeIDList, "", id);

            if (_matlist.Count < 30)
            {
                if (_matlist.Count == 0)
                {
                    return PartialView("_PartialShowMaterials");
                }
                // 12 den az ise belge sayısı olduğu kadar ı gösterilecek 9 sa 9 u gibi
                var _list = _matlist.ToPagedList(1, _matlist.Count);//.OrderBy(x => x.ID)
                return PartialView("_PartialShowMaterials", _list);
            }
            // birinci sayfa açılsın,12 tane göstersin anlamında
            var _list2 = _matlist.ToPagedList(1, 30); //.OrderBy(x => x.ID)

            return PartialView("_PartialShowMaterials", _list2);
        }

        // sayfalama 
        [HttpGet]
        public ActionResult MatList(string id)
        {
            string _sessionEmail = "";

            if (Session["Email"] == null)
            {
                return RedirectToAction("Index", "Home", new { area = "" });
            }

            _sessionEmail = Session["Email"].ToString();
            string _name = point.UserService.GetNameFromMail(_sessionEmail);

            // store id ve son raf bilgisi var
            var _storeIDList = point.InventoryGroupService.GetStoreIDS(_sessionEmail);


            string[] _data = id.Split('Ş'); // gelen parametreyi 2 ye ayırıp diziye atıyoruz

            string _belgeID = _data[0]; //belgeid döner
            int _pageID = int.Parse(_data[1]);  // pageid döner

            ICollection<MaterialsModel> _docs = null;

            if (_belgeID.Length > 0)
            {
                _docs = point.MaterialService.GetMaterialTwo(_storeIDList, _belgeID); //point.HausFilesService.GetDocs(belgeID);
            }
            else
            {
                _docs = point.MaterialService.GetSearchMaterialS("", _storeIDList, "", _sessionEmail);
            }

            var _list = _docs.ToPagedList(_pageID, 30);
            return PartialView("_PartialShowMaterials", _list);
        }

        // writer combobox lar dolar
        [HttpPost]
        public ActionResult SetStockStore()
        {
            string _sessionEmail = "";

            if (Session["Email"] == null)
            {
                return RedirectToAction("Index", "Home", new { area = "" });
            }

            _sessionEmail = Session["Email"].ToString();

            //int _idMail = point.UserService.GetID(_sessionEmail);

            //Data.Model.Users _user = point.UserService.FindByID(_idMail);
            string _name = point.UserService.GetNameFromMail(_sessionEmail);

            var _storeIDList = point.InventoryGroupService.GetStoreID(_name);

            List<WarehouseStockplace> x = new List<WarehouseStockplace>();

            WarehouseStockplace _c = new WarehouseStockplace
            {
                StoreList = GetStores(_storeIDList)
            };
            x.Add(_c);

            return PartialView("_PartialDropDownList", _c);
        }

        [HttpPost]
        public ActionResult SearchMaterial(string id)
        {
            id = id.Replace('ç', '.');
            string[] _search = id.Split('ş');
            string _searchChar = _search[0];
            string _inventCode = _search[1];

            // belirtilen depo ve stok yerinde o grubun saydığı malzemeler listelenir.
            var _InventResultList = point.InventoryService.SeachGetItems(_inventCode, _searchChar);

            if (_InventResultList.Count < 7)
            {
                if (_InventResultList.Count == 0)
                {
                    return PartialView("_PartialInventResult");
                }
                var _list = _InventResultList.OrderByDescending(x => x.CreateDate).ToPagedList(1, _InventResultList.Count);
                return PartialView("_PartialInventResult", _list);

            }

            var _list2 = _InventResultList.OrderByDescending(x => x.CreateDate).ToPagedList(1, 7);
            return PartialView("_PartialInventResult", _list2);
        }

        [HttpPost]
        public ActionResult FilteredSearchedShowMaterials(StoreIdStockPlace model)
        {

            string id = model.ID.ToString();

            string[] _matCodeEmail = id.Split('ğ');

            string _matCode = _matCodeEmail[0];
            string _mail = _matCodeEmail[1];

            string _name = point.UserService.GetNameFromMail(_mail);

            // store id ve son raf bilgisi var
            var _storeIDList = point.InventoryGroupService.GetStoreIDS(_name);

            var _matlist = point.MaterialService.GetSearchMaterialFiltered(model.StockPlace, _matCode, _storeIDList);

            if (_matlist.Count < 30)
            {
                if (_matlist.Count == 0)
                {
                    return PartialView("_PartialSerachedMaterials");
                }
                // 12 den az ise belge sayısı olduğu kadar ı gösterilecek 9 sa 9 u gibi
                var _list = _matlist.ToPagedList(1, _matlist.Count);
                return PartialView("_PartialSerachedMaterials", _list);
            }
            // birinci sayfa açılsın,12 tane göstersin anlamında
            var _list2 = _matlist.ToPagedList(1, 30);

            return PartialView("_PartialSerachedMaterials", _list2);
        }


        [HttpPost]
        public ActionResult SearchedShowMaterialsUser(SearchData dto)
        {
            string _matCode = dto._matCode;
            string _mail = dto._mail;
            string _depo = dto._ware;
            string _stock = dto._stock;
            string _depoStok = _depo + "ç" + _stock;

            string _name = point.UserService.GetNameFromMail(_mail);

            // store id ve son raf bilgisi var
            var _storeIDList = point.InventoryGroupService.GetStoreIDS(_mail);

            ViewBag.StorageItem = _matCode;

            var _matlist = point.MaterialService.GetSearchMaterialS(_matCode, _storeIDList, _depoStok, _mail);

            if (_matlist.Count < 30)
            {
                if (_matlist.Count == 0)
                {
                    return PartialView("_PartialSerachedMaterials");
                }
                // 12 den az ise belge sayısı olduğu kadar ı gösterilecek 9 sa 9 u gibi
                var _list = _matlist.ToPagedList(1, _matlist.Count);
                return PartialView("_PartialSerachedMaterials", _list);
            }
            // birinci sayfa açılsın,12 tane göstersin anlamında
            var _list2 = _matlist.ToPagedList(1, 30);

            return PartialView("_PartialSerachedMaterials", _list2);
        }

        [HttpPost]
        public ActionResult FilteredStockPlace(string id)
        {
            string _sessionEmail = "";

            if (Session["Email"] == null)
            {
                return RedirectToAction("Index", "Home", new { area = "" });
            }

            _sessionEmail = Session["Email"].ToString();

            string _name = point.UserService.GetNameFromMail(_sessionEmail);

            // store id ve son raf bilgisi var
            var _storeIDList = point.InventoryGroupService.GetStoreIDS(_sessionEmail);

            var _teamNo = point.InventoryGroupService.GetTeamNoFromName(_name);

            var _matlist = point.MaterialService.GetFilteredMaterial(id, _storeIDList, _teamNo);

            if (_matlist.Count < 30)
            {
                if (_matlist.Count == 0)
                {
                    return PartialView("_PartialSerachedMaterials");
                }
                // 12 den az ise belge sayısı olduğu kadar ı gösterilecek 9 sa 9 u gibi
                var _list = _matlist.ToPagedList(1, _matlist.Count);
                return PartialView("_PartialSerachedMaterials", _list);
            }
            // birinci sayfa açılsın,12 tane göstersin anlamında
            var _list2 = _matlist.ToPagedList(1, 30); //.OrderBy(x => x.ID)

            return PartialView("_PartialSerachedMaterials", _list2);
        }

        // partialdropdownlist FillStockPlace2()
        [HttpPost]
        public JsonResult SelectStockPlaceForWriter(string id)
        {
            string _sessionEmail = "";

            if (Session["Email"] == null)
            {
                return Json("false");
            }

            _sessionEmail = Session["Email"].ToString();

            string _name = point.UserService.GetNameFromMail(_sessionEmail);

            var _storeIDList = point.InventoryGroupService.GetMissionStockPlace(_name);

            var _stockPlaceList = point.StoreService.SelectStockPlaceForWriters(id, _storeIDList, _name);

            return Json(_stockPlaceList);
        }

        private IEnumerable<SelectListItem> GetStores(List<int> list)
        {
            List<string> _missionInStoreList = point.StoreService.GetStoreNoForID(list);

            List<SelectListItem> c3 = new List<SelectListItem>();

            //List<string> c = new List<string>();
            //string _so = "";
            string depo = "";

            for (int i = 0; i < _missionInStoreList.Count; i++)
            {
                if (depo != _missionInStoreList[i])
                {
                    depo = _missionInStoreList[i];

                    SelectListItem v = new SelectListItem
                    {
                        Text = _missionInStoreList[i],
                        Value = _missionInStoreList[i]
                    };
                    c3.Add(v);
                };
            }

            return new SelectList(c3, "Value", "Text");
        }


        // İMZALAMA AŞAMASI 1
        [HttpPost]
        public JsonResult CheckInventMaterial(string id)
        {
            // id sayım nosunu verir,bu sayım nosundan [Inventories] den veriler alınır [InventoriesApproves] tablosuna veri kaydedidilir
            string _email = id; //.Replace("ğ", ".");

            string _name = point.UserService.GetNameFromMail(_email);

            TeamGroup _teamNo = point.UserService.GetTeamAndGroup(_email);
            string _TGNO = _teamNo._team + "X" + _teamNo._group;

            string _inventCode = point.InventoryService.GetInventResultFromName(_name, _TGNO);

            var _doc = point.InventoryService.GetList(_inventCode);

            // bu ekibin görevli olduğu yerlerdeki malzeme listelerini topla
            var _storeIDList = point.InventoryGroupService.GetStoreIDS(_email);
            string _matCode = "";

            var _matList = point.MaterialService.GetSearchMaterialS(_matCode, _storeIDList, "", _name);

            List<InventoriesModel> _mList = new List<InventoriesModel>();
            List<InventoriesModel> _dList = new List<InventoriesModel>();

            foreach (var x in _doc)
            {
                InventoriesModel _c = new InventoriesModel
                {
                    IsActive = x.IsActive,
                    MatCode = x.MatCode,
                    MatIndex = x.MatIndex,
                    MatSection = x.MatSection,
                    MatSectionPlace = x.MatSectionPlace,
                    MatSerialNumber = x.MatSerialNumber,
                    MatSpecialStock = x.MatSpecialStock
                };
                _dList.Add(_c);
            }
            foreach (var x in _matList)
            {
                InventoriesModel _c = new InventoriesModel
                {
                    IsActive = x.IsActive,
                    MatCode = x.MatCode,
                    MatIndex = x.MatIndex,
                    MatSection = x.MatSection,
                    MatSectionPlace = x.MatSectionPlace,
                    MatSerialNumber = x.MatSerialNumber,
                    MatSpecialStock = x.MatSpecialStock
                };
                _mList.Add(_c);
            }

            //var _MLISTS = _mList.ToDictionary(x => new { x.MatCode, x.MatIndex, x.MatSection, x.MatSectionPlace, x.MatSerialNumber });            
            //var _DLISTS = _dList.ToDictionary(x => new { x.MatCode, x.MatIndex, x.MatSection, x.MatSectionPlace, x.MatSerialNumber });
            //List<InventModel> _gf = new List<InventModel>();
            List<InventoriesModel> _empt = new List<InventoriesModel>();

            foreach (var item in _mList)
            {
                var _gf = _dList.Where(x => x.MatCode.Contains(item.MatCode) && x.MatIndex.Contains(item.MatIndex)
                                                                            && x.MatSection.Contains(item.MatSection)
                                                                            && x.MatSectionPlace.Contains(item.MatSectionPlace)
                                                                            && x.MatSerialNumber == item.MatSerialNumber
                                                                            ).ToList();
                if (_gf.Count == 0)
                {
                    InventoriesModel _g = new InventoriesModel
                    {
                        IsActive = item.IsActive,
                        MatCode = item.MatCode,
                        MatIndex = item.MatIndex,
                        MatSection = item.MatSection,
                        MatSectionPlace = item.MatSectionPlace,
                        MatSerialNumber = item.MatSerialNumber,
                        MatSpecialStock = item.MatSpecialStock
                    };
                    _empt.Add(_g);
                }

            }

            return Json(_empt);
        }

        // İMZALAMA AŞAMASI 2
        [HttpPost]
        public ActionResult DocSigns(string id)
        {
            var _model = GetUnCounted(id);

            // id sayım nosunu verir,bu sayım nosundan [Inventories] den veriler alınır [InventoriesApproves] tablosuna veri kaydedidilir
            string _email = id;//.Replace("ğ", ".");

            string _name = point.UserService.GetNameFromMail(_email);

            TeamGroup _teamNo = point.UserService.GetTeamAndGroup(_email);
            string _TGNO = _teamNo._team + "X" + _teamNo._group;
            string _inventCode = point.InventoryService.GetInventResultFromName(_name, _TGNO);

            // sıfır olarak inventory tablosuna kaydedilecekler
            foreach (var item in _model)
            {
                string _sr = "";

                if (item.MatSerialNumber != null)
                {
                    _sr = item.MatSerialNumber;
                }

                if (item.MatCode != null)
                {
                    string _matname = point.MaterialService.GetName(item.MatCode);

                    var _penIsApproved = point.InventoryService.IsApproved(item.MatCode, item.MatIndex, item.MatSpecialStock, item.MatSerialNumber, item.MatSection, item.MatSectionPlace);

                    if (!_penIsApproved)
                    {
                        // malzeme henüz onaylanmadıysa
                        var _isZero = point.InventoryService.IsZero(item.MatCode, item.MatIndex, item.MatSpecialStock, item.MatSerialNumber, item.MatSection, item.MatSectionPlace);

                        if (!_isZero)
                        {
                            // malzeme daha önce başkası tarafından 0 olarak kaydedilmediyse

                            InventoriesModel _cf = new InventoriesModel
                            {
                                MatCode = item.MatCode,
                                MatIndex = item.MatIndex,
                                MatSection = item.MatSection,
                                MatSectionPlace = item.MatSectionPlace,
                                MatSerialNumber = _sr,
                                MatSpecialStock = item.MatSpecialStock,
                                IpAddress = Helpers.HtmlHelpers.GetIpHelper(),
                                IsActive = true,
                                IsSigned = true,
                                IsApproved = false,
                                MatName = _matname,
                                MatQuantity = 0,
                                ApprovedDate = DateTime.Parse("1900.01.01"),
                                ApprovedPers = "",
                                SignedTime = DateTime.Now,
                                InventoryCode = _inventCode,
                                SignedBy = _email,
                                ChangeDate = DateTime.Now,
                                ChangedBy = _email,
                                Company = "01",
                                CreateDate = DateTime.Now,
                                CreatedBy = _email
                            };
                            string msg = point.InventoryService.BeforeInsert(_cf);

                            if (msg != "True")
                            {
                                return Json(false);
                            }

                        };
                    };
                }
            }
            // sıfır olarak inventory tablosuna kaydedilecekler

            // belge imzalanma

            var _doc = point.InventoryService.GetList(_inventCode);

            double _totalMaterialQuantity = 0;

            foreach (var item in _doc)
            {
                _totalMaterialQuantity = _totalMaterialQuantity + item.MatQuantity;
            }

            InventApprovesModel ia = new InventApprovesModel
            {
                InventoryCode = _inventCode,
                TotalQuantity = _totalMaterialQuantity,
                IsActive = true,
                IpAddress = Helpers.HtmlHelpers.GetIpHelper(),
                IsCanceled = false,
                ChangeDate = DateTime.Now,
                ChangedBy = Session["Email"].ToString(),
                Company = "01",
                CreateDate = DateTime.Now,
                CreatedBy = Session["Email"].ToString()
            };
            // onaylanması için tabloya veri gönderiliyor
            string ms = point.InventApproveService.BeforeInsert(ia);

            if (ms == "True")
            {
                // başarı ile onaylandı ise öncelikle yazıcının user tablosunda rol ünü 5000 yap
                // sonrada inventories tablosunda malzemelerin imzalandı mı kolonunu 1 yap
                // son olarakta inventorygroup tablosunda sayımı yapan ekibin aktifliğini pasife çek

                // ROLE 5000 e çek
                int _userid = point.UserService.GetIDFName(_name);
                string _userMsg = point.UserService.UpdateRole(_userid, 5000);

                if (_userMsg != "True")
                {
                    TempData["Kayit"] = "problem";
                    //ModelState.AddModelError("Kayit", "problem");
                }

                #region EDIT : 29OCAK2018 ; DESC : INSERT işleminde aşağıdaki işlem yaptırıldı

                // şimdi belgeleri imzlandı olarak işaretle
                //var _IsSignedList = point.InventoryService.FindIDListInventoryCode(_inventCode);

                //foreach (var item in _IsSignedList)
                //{
                //    InventoriesModel invs = new InventoriesModel
                //    {
                //        ID = item.ID,
                //        InventoryCode = _inventCode,
                //        IsSigned = true,
                //        SignedTime = DateTime.Now,
                //        SignedBy = _email
                //    };
                //    string msg = point.InventoryService.UpdateSign(invs);

                //    if (msg != "True")
                //    {
                //        return Json(false);
                //    }
                //}

                #endregion

                // şimdi de inventry grup tablosunda ekibi pasif yap

                var _listActiveName = point.InventoryGroupService.GetGroupForName(_name);

                foreach (var item in _listActiveName)
                {
                    IGModel invtGrp = new IGModel
                    {
                        ID = item.ID,
                        PerName = item.PerName,
                        IsActive = false
                    };

                    int _ms2 = point.InventoryGroupService.UpdateIsActive(invtGrp);

                    if (_ms2 == 0)
                    {
                        return Json(false);
                    }
                }

                //return RedirectToAction("Index", "Writer");
                return Json(true);
            }
            //return RedirectToAction("Index", "Writer");
            return Json(false);
        }

        public List<InventoriesModel> GetUnCounted(string id)
        {
            // id sayım nosunu verir,bu sayım nosundan [Inventories] den veriler alınır [InventoriesApproves] tablosuna veri kaydedidilir
            string _email = id;//.Replace("ğ", ".");

            string _name = point.UserService.GetNameFromMail(_email);

            TeamGroup _teamNo = point.UserService.GetTeamAndGroup(_email);
            string _TGNO = _teamNo._team + "X" + _teamNo._group;
            string _inventCode = point.InventoryService.GetInventResultFromName(_name, _TGNO);

            var _doc = point.InventoryService.GetList(_inventCode);

            // bu ekibin görevli olduğu yerlerdeki malzeme listelerini topla
            var _storeIDList = point.InventoryGroupService.GetStoreIDS(_email);
            string _matCode = "";

            var _matList = point.MaterialService.GetSearchMaterialS(_matCode, _storeIDList, "", _email);

            List<InventoriesModel> _mList = new List<InventoriesModel>();
            List<InventoriesModel> _dList = new List<InventoriesModel>();

            foreach (var x in _doc)
            {
                InventoriesModel _c = new InventoriesModel
                {
                    IsActive = x.IsActive,
                    MatCode = x.MatCode,
                    MatIndex = x.MatIndex,
                    MatSection = x.MatSection,
                    MatSectionPlace = x.MatSectionPlace,
                    MatSerialNumber = x.MatSerialNumber,
                    MatSpecialStock = x.MatSpecialStock
                };
                _dList.Add(_c);
            }
            foreach (var x in _matList)
            {
                InventoriesModel _c = new InventoriesModel
                {
                    IsActive = x.IsActive,
                    MatCode = x.MatCode,
                    MatIndex = x.MatIndex,
                    MatSection = x.MatSection,
                    MatSectionPlace = x.MatSectionPlace,
                    MatSerialNumber = x.MatSerialNumber,
                    MatSpecialStock = x.MatSpecialStock
                };
                _mList.Add(_c);
            }

            //var _MLISTS = _mList.ToDictionary(x => new { x.MatCode, x.MatIndex, x.MatSection, x.MatSectionPlace, x.MatSerialNumber });            
            //var _DLISTS = _dList.ToDictionary(x => new { x.MatCode, x.MatIndex, x.MatSection, x.MatSectionPlace, x.MatSerialNumber });
            //List<InventModel> _gf = new List<InventModel>();
            List<InventoriesModel> _empt = new List<InventoriesModel>();

            foreach (var item in _mList)
            {
                var _gf = _dList.Where(x => x.MatCode.Contains(item.MatCode) && x.MatIndex.Contains(item.MatIndex)
                                                                            && x.MatSection.Contains(item.MatSection)
                                                                            && x.MatSectionPlace.Contains(item.MatSectionPlace)
                                                                            && x.MatSerialNumber == item.MatSerialNumber
                                                                            ).ToList();
                if (_gf.Count == 0)
                {
                    InventoriesModel _g = new InventoriesModel
                    {
                        IsActive = item.IsActive,
                        MatCode = item.MatCode,
                        MatIndex = item.MatIndex,
                        MatSection = item.MatSection,
                        MatSectionPlace = item.MatSectionPlace,
                        MatSerialNumber = item.MatSerialNumber,
                        MatSpecialStock = item.MatSpecialStock
                    };
                    _empt.Add(_g);
                }

            }

            return _empt;
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
