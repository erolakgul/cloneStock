using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using Microsoft.Office.Interop.Excel;
using StockHaus.Core.BaseService;
using StockHaus.ModelClass.AdminPage;
using StockHaus.UI.Enums;

namespace StockHaus.UI.Areas.Admin.Controllers
{
    public class PanelController : Controller
    {
        ServicePoints point;
        public PanelController()
        {
            point = new ServicePoints();
        }
        //
        // GET: /Admin/Panel/

        public ActionResult Index()
        {
            var d = Session.Timeout;

            // takım ve gruplara kişi oluşturma aşaması hata mesajları
            if (TempData["Kayit"] != null)
            {
                ViewBag.Error = TempData["Kayit"].ToString();
            }
            else if (TempData["Atama"] != null)
            {
                ViewBag.Assign = TempData["Atama"].ToString();
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
                    return View();
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
                    return RedirectToAction("Index", new RouteValueDictionary(
                     new
                     {
                         area = "AreaController",
                         controller = "Panel",
                         action = "Index" //, Id = 2000 
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

        // takımlar ın grupları için personelleri atar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateGroup(CreateGroups model)
        {
            int _teamNo = 0; // team no için
            int _ms = 0; // mission status için

            if (model._teamNo == "Birinci")
            {
                _teamNo = (int)TeamNo.Birinci;
            }
            else if (model._teamNo == "İkinci")
            {
                _teamNo = (int)TeamNo.İkinci;
            }
            else
            {
                _teamNo = (int)TeamNo.Üçüncü;
            }

            if (model._missionStatus == "Yazıcı")
            {
                _ms = (int)MissionStatus.Yazıcı;  // yani 1
            }
            else
            {
                _ms = (int)MissionStatus.Sayıcı;  // yani 2
            }

            //
            int _lastGroupNo = point.InventoryGroupService.LastGroupNo(_teamNo);

            int _intGroupNo;

            int _groupNoIsInteger;

            if (int.TryParse(model._groupNo, out _groupNoIsInteger))
            {
                _intGroupNo = int.Parse(model._groupNo);

                // grup no integer ise buradan devam eder
                // kontrol 1; eklenen kişi daha önce aktif olan bir grupta çalışıyor mu ?  evet ise ekleme
                // kontrol 2; eklenen kişinin mission statusu o gruptaki ilk kişinin statusu ile çakışıyor mu ? evet ise ekleme
                // kontrol 3; eklenen kişinin grubu zaten 2 kişilik mi ? evet ise ekleme
                // kontrol 4; eklenen kişinin seçildiği group no su eğer daha önce pasif edilmiş ve 2 kişilik ise
                // lastID kullanılıp otomatik olarak o değer atılır

                string _upperPerName = (model._perName).ToUpper();

                // eğer isim users tablosunda yoksa ekleme
                int _controlNameID = point.UserService.GetIDFName(_upperPerName);

                if (_controlNameID == 0)
                {
                    TempData["Kayit"] = "İsimYok";
                    //ModelState.AddModelError("Kayit", "İsimYok");
                    return RedirectToAction("Index", "Panel");
                }

                bool _controlOne = point.InventoryGroupService.IsWorking(_upperPerName, _teamNo); // true ise ekleme

                bool _controlTwo = point.InventoryGroupService.IsMissionSame(_intGroupNo, _teamNo, _ms); // true ise ekleme

                bool _controlThree = point.InventoryGroupService.IsGroupFull(_intGroupNo, _teamNo); // true ise ekleme

                bool _controlFour = point.InventoryGroupService.IsNonActiveGroupFull(_intGroupNo, _teamNo);  // true ise lastId ye 1 ekle groupNo yap

                if (_controlThree)  // 2 kişi ise ekleme
                {
                    TempData["Kayit"] = "ikikisi";
                    //ModelState.AddModelError("Kayit", "ikikisi");
                    return RedirectToAction("Index", "Panel");
                }
                else if (_controlOne) // başka bir yerde aktif ise ekleme
                {
                    TempData["Kayit"] = "aktif";
                    //ModelState.AddModelError("Kayit", "aktif");
                    return RedirectToAction("Index", "Panel");
                }
                else if (_controlTwo) // ilk inin statusu ile aynı ise ekleme
                {
                    TempData["Kayit"] = "aynı";
                    //ModelState.AddModelError("Kayit", "aynı");
                    return RedirectToAction("Index", "Panel");
                }
                else if (_controlFour)
                {
                    IGModel igx = new IGModel
                    {
                        PerName = (model._perName).ToUpper(),
                        EndStockPlace = "",
                        GroupNo = _lastGroupNo,
                        MissionStatu = _ms,
                        TeamNo = _teamNo,
                        IpAddress = Helpers.HtmlHelpers.GetIpHelper(),
                        CreatedBy = Session["Email"].ToString(),
                        CreateDate = DateTime.Now,
                        ChangeDate = DateTime.Now,
                        ChangedBy = Session["Email"].ToString(),
                        Company = "01",
                        IsActive = true,
                        StoreID = 1,
                        UserID = _controlNameID
                    };
                    string ms = point.InventoryGroupService.BeforeInsert(igx);

                    if (ms == "True")
                    {
                        TempData["Kayit"] = "true";
                        //ModelState.AddModelError("Kayit", "true");

                        if (igx.MissionStatu == 1) //kişi yazıcı ise user tablosundaki role kolonunu 3000 yapıyoruz
                        {
                            string _userid = point.UserService.UpdateRole(_controlNameID, 3000);

                            if (_userid != "True")
                            {
                                TempData["Kayit"] = "problem";
                                //ModelState.AddModelError("Kayit", "problem");
                            }
                        }
                        else if (igx.MissionStatu == 2) // yazıcı ise rolü 5000 yapılır
                        {
                            string _userid = point.UserService.UpdateRole(_controlNameID, 5000);
                            if (_userid != "True")
                            {
                                TempData["Kayit"] = "problem";
                                //ModelState.AddModelError("Kayit", "problem");
                            }
                        }
                        return RedirectToAction("Index", "Panel");
                    }
                }
                else  // hepsinden de false geldiyse ekleyebiliriz.
                {
                    // ekleme kodu başlangıç
                    if (_lastGroupNo == 0)
                    {
                        _lastGroupNo = _lastGroupNo + 1;
                    }

                    IGModel igx = new IGModel
                    {
                        PerName = (model._perName).ToUpper(),
                        EndStockPlace = "",
                        GroupNo = _lastGroupNo,
                        MissionStatu = _ms,
                        TeamNo = _teamNo,
                        IpAddress = Helpers.HtmlHelpers.GetIpHelper(),
                        CreatedBy = Session["Email"].ToString(),
                        CreateDate = DateTime.Now,
                        ChangeDate = DateTime.Now,
                        ChangedBy = Session["Email"].ToString(),
                        Company = "01",
                        IsActive = true,
                        StoreID = 1,
                        UserID = _controlNameID
                    };
                    string ms = point.InventoryGroupService.BeforeInsert(igx);

                    if (ms == "True")
                    {
                        TempData["Kayit"] = "true";
                        //ModelState.AddModelError("Kayit", "true");

                        if (igx.MissionStatu == 1) //kişi yazıcı ise user tablosundaki role kolonunu 3000 yapıyoruz
                        {
                            string _userid = point.UserService.UpdateRole(_controlNameID, 3000);

                            if (_userid != "True")
                            {
                                TempData["Kayit"] = "problem";
                                //ModelState.AddModelError("Kayit", "problem");
                            }
                        }
                        else if (igx.MissionStatu == 2) // yazıcı ise rolü 5000 yapılır
                        {
                            string _userid = point.UserService.UpdateRole(_controlNameID, 5000);
                            if (_userid != "True")
                            {
                                TempData["Kayit"] = "problem";
                                //ModelState.AddModelError("Kayit", "problem");
                            }
                        }
                        return RedirectToAction("Index", "Panel");
                    }
                    // ekleme kodu son
                }
            }
            else
            {
                TempData["Kayit"] = "false";
                return RedirectToAction("Index", "Panel");
            }
            TempData["Kayit"] = "false";
            return RedirectToAction("Index", "Panel");
        }

        //// Grupları Depo Görevlerine Alır
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AssignTask(AssignTasks model)
        {
            // inventorygroup un storeıd si buradan gidecek
            var _storeId = point.StoreService.GetStoreID(model._stockStore, model._stockPlace);

            if (string.IsNullOrEmpty(model._perName) || string.IsNullOrEmpty(model._perName2))
            {
                TempData["Atama"] = "Boş";
                return RedirectToAction("Index", "Panel");
            }

            // hangi takım seçili
            int _teamNo = 0; // team no için

            if (model._teamNo == "Birinci")
            {
                _teamNo = (int)TeamNo.Birinci;
            }
            else if (model._teamNo == "İkinci")
            {
                _teamNo = (int)TeamNo.İkinci;
            }
            else
            {
                _teamNo = (int)TeamNo.Üçüncü;
            }

            // o takım ve grubun bir sayım kodu var mı? varsa işleme devam etme inventory group a kaydet sadece
            string _te = _teamNo.ToString();
            string _gr = model._groupNoL.ToString();

            bool isDefinedInventCode = point.InventCodeCountService.IsDefined(_te, _gr);


            // son sayım numarası alınır
            int _lastCount = point.InventCodeCountService.GetLastCode();

            if (!isDefinedInventCode)
            {
                if (_lastCount == 0)
                {
                    _lastCount = 1000; // ilk atama burada yapılır

                    InventCodeLogModel _h = new InventCodeLogModel
                    {
                        CountOrder = _lastCount,
                        TeamNo = _teamNo.ToString(),
                        GroupNo = model._groupNoL,
                        IsActive = true,
                        IpAddress = Helpers.HtmlHelpers.GetIpHelper(),
                        ChangeDate = DateTime.Now,
                        ChangedBy = Session["Email"].ToString(),
                        Company = "01",
                        CreateDate = DateTime.Now,
                        CreatedBy = Session["Email"].ToString()
                    };

                    string msg = point.InventCodeCountService.BeforeInsert(_h);

                    if (msg != "True")
                    {
                        TempData["Atama"] = "TekrarKayıt";
                        return RedirectToAction("Index", "Panel");
                    }
                }
                else
                {
                    _lastCount = _lastCount + 1;

                    InventCodeLogModel _h = new InventCodeLogModel
                    {
                        CountOrder = _lastCount,
                        TeamNo = _teamNo.ToString(),
                        GroupNo = model._groupNoL,
                        IsActive = true,
                        IpAddress = Helpers.HtmlHelpers.GetIpHelper(),
                        ChangeDate = DateTime.Now,
                        ChangedBy = Session["Email"].ToString(),
                        Company = "01",
                        CreateDate = DateTime.Now,
                        CreatedBy = Session["Email"].ToString()
                    };

                    string msg = point.InventCodeCountService.BeforeInsert(_h);

                    if (msg != "True")
                    {
                        TempData["Atama"] = "TekrarKayıt";
                        return RedirectToAction("Index", "Panel");
                    }
                }
            };

            string _inventCode = _lastCount + "X" + _teamNo + "X" + model._groupNoL;

            // true ise daha önce o depo ve stok yeri sayılmış demektir,takım numarası değiştirilmesi istenir.
            bool _IsCounted = point.InventApproveService.IsFinishedInvent(_inventCode);

            if (_IsCounted)
            {
                TempData["Atama"] = "Sayılmış";
                return RedirectToAction("Index", "Panel");
            }

            // örneğin grup no su 5 olan ama aktif olan satırların storeid sini değiştir kodu burada olacak
            IGModel ig = new IGModel
            {
                TeamNo = _teamNo,
                GroupNo = int.Parse(model._groupNoL),
                StoreID = _storeId,
                CreatedBy = Session["Email"].ToString(),
                EndStockPlace = model._endStockPlace
            };

            int _ms = point.InventoryGroupService.UpdateGroupStore(ig);

            if (_ms == 1)
            {
                TempData["Atama"] = "Tamamlandı";
                //return RedirectToAction("Index", "Panel");
            }
            else
            {
                TempData["Atama"] = "TekrarKayıt2";
            }
            //TempData["Atama"] = "Yapılamadı";
            return RedirectToAction("Index", "Panel");
        }

        // personel name pre estimated textbox özelliği
        [HttpPost]
        public JsonResult PreEstimatedPerName(string id)
        {
            var _nameList = point.UserService.GetNameFromChar(id);

            var _name = point.InventoryGroupService.ToListIG(id, "0");

            foreach (var item in _name)
            {
                foreach (var items in _nameList.ToList())
                {
                    if (items._name == item.PerName)
                    {
                        _nameList.Remove(items);

                        if (_nameList.Count == 0)
                        {
                            break;
                        }
                    }
                }
            }

            return Json(_nameList);
        }

        [HttpPost]
        public ActionResult GetExcel(WareStock dto)
        {

            string reportPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string reportName = dto._ware + " depo ve " + dto._stock + " stokyeri.xlsb";

            List<InventoriesModel> data = point.InventoryService.GetDataForExcel(dto._ware, dto._stock);

            System.Data.DataTable table = ToDataTable(data); //GetDatatable(); //-your code to create datatable and return it
            table.TableName = "Sonuclar";//dto._ware + " depo ve " + dto._stock + " stok yeri malzeme listesi";

            Microsoft.Office.Interop.Excel.Application excelApp = new Microsoft.Office.Interop.Excel.Application();
            //Create an Excel workbook instance 
            Microsoft.Office.Interop.Excel.Workbook excelWorkBook = excelApp.Workbooks.Add(XlWBATemplate.xlWBATWorksheet);

            Microsoft.Office.Interop.Excel.Worksheet excelWorkSheet = excelWorkBook.Sheets.Add();
            excelWorkSheet.Name = Convert.ToString(table.TableName);
            excelWorkSheet.Columns.AutoFit();

            for (int i = 1; i < table.Columns.Count + 1; i++)
            {
                if (i < 10) // sadece seçtiiğim kolonlar gelsin
                {
                    excelWorkSheet.Cells[1, i] = table.Columns[i - 1].ColumnName;
                }
            }

            for (int j = 0; j < table.Rows.Count; j++)
            {
                for (int k = 0; k < table.Columns.Count; k++)
                {
                    excelWorkSheet.Cells[j + 2, k + 1] = table.Rows[j].ItemArray[k].ToString();
                }
            }

            //-- check file directory is present or not/if note create new
            bool exists = System.IO.Directory.Exists(reportPath);
            if (!exists)
            {
                System.IO.Directory.CreateDirectory(reportPath);
            }

            excelWorkBook.SaveAs(reportPath + reportName,
            XlFileFormat.xlExcel12, Type.Missing, Type.Missing,
            Type.Missing, Type.Missing, XlSaveAsAccessMode.xlExclusive,
            Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
            excelWorkBook.Close();
            excelApp.Quit();

            return File(reportPath + reportName, "application/vnd.ms-excel", reportName);
        }

        public System.Data.DataTable ToDataTable(List<InventoriesModel> data)
        {
            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(InventoriesModel));
            System.Data.DataTable table = new System.Data.DataTable();
            for (int i = 0; i < props.Count; i++)
            {
                PropertyDescriptor prop = props[i];
                table.Columns.Add(prop.Name, prop.PropertyType);
            }
            object[] values = new object[props.Count];
            foreach (InventoriesModel item in data)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    if (i < 9) // sadece seçtiğim kolonların gelmesi için
                    {
                        values[i] = props[i].GetValue(item);
                    }

                }
                table.Rows.Add(values);
            }
            return table;
        }

        [HttpGet]
        public ActionResult GetOnlineUsers()
        {
            List<UserModel> list = point.UserService.GetOnlineUser();

            return PartialView("_PartialPersonelStatus", list);
        }

        [HttpPost]
        public JsonResult LastGroupNoForTeam(int id)
        {
            int _lastNo = point.InventoryGroupService.LastGroupNo(id);

            return Json(_lastNo);
        }

        // SİL
        [HttpPost]
        public ActionResult Delete(int id)
        {
            var _invtGrp = point.InventoryGroupService.ToListIG(id.ToString(), "1");

            foreach (var item in _invtGrp)
            {
                bool _TF = point.InventoryService.CheckActive(item.TeamNo.ToString(), item.GroupNo.ToString());

                // eğer sayım yaptıysa, bu sayım sonuçları aktifse ve de belge imzalanmamışsa bu adam sayım yapıyor demektir
                // sildirme
                if (_TF)
                {
                    return Json("Sayım");
                }
            }

            string _name = point.InventoryGroupService.GetNameFromID(id);

            int _userID = point.UserService.GetIDFName(_name);

            string _userid = point.UserService.UpdateRole(_userID, 5000);

            if (_userid != "True")
            {
                return Json("Hata");
            }

            string _deleted = point.InventoryGroupService.IGDelete(id);

            if (_deleted == "True")
            {
                return Json("Silindi");
            }

            return Json("IGError");
        }

        // partial ı çağırır
        public ActionResult GetCreateTeamPage()
        {
            SetTeamNo();
            SetMissionNo();

            return PartialView("_PartialCreateTeam");
        }
        // partial ı çağırır
        public ActionResult GetAssignTeamPage()
        {
            SetTeamNo();
            SetStockStore();
            SetStockPlace();
            return PartialView("_PartialAssignTask");
        }

        // Grupları ve atandıkları yeri gösterir Method
        [HttpPost]
        public PartialViewResult ShowTeamGroup(int id)
        {
            var _tgList = point.InventoryGroupService.SelectGroup(id);

            return PartialView("_PartialTeamGroup", _tgList);
        }

        // grupno combobox için yazıldı,event change method görevi görür,,textbox ları doldurur
        [HttpPost]
        public ActionResult SelectGroupPersonel(string id)
        {
            string[] _data = id.Split('-');
            int _teamNo = 0;

            // teamNo seçilir
            if (_data[0] == "Birinci")
            {
                _teamNo = 1;
            }
            else if (_data[0] == "İkinci")
            {
                _teamNo = 2;
            }
            else
            {
                _teamNo = 3;
            };

            // groupNo seçilir
            int _groupNo = 1;

            if (_data[1].Length != 0)
            {
                _groupNo = int.Parse(_data[1]);
            }

            var _perNames = point.InventoryGroupService.SelectGroupPerName(_teamNo, _groupNo);

            if (_perNames.Count == 0)
            {
                return Json("", "");
            }

            return Json(_perNames);
        }

        // ekip lerin daha önce görevli oldukları depo ve stok yerleri döndürülür
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

        // depo yeri seçildikten sonra stok yerini doldurur
        [HttpPost]
        public JsonResult SelectStockPlace(string id)
        {
            var _stockPlaceList = point.StoreService.SelectStockPlace(id);

            return Json(_stockPlaceList);
        }

        [HttpPost]
        public JsonResult SetGroupNoJS(string id)
        {
            string[] _data = id.Split('-');
            int _teamNo = 0;

            // teamNo seçilir
            if (_data[0] == "Birinci")
            {
                _teamNo = 1;
            }
            else if (_data[0] == "İkinci")
            {
                _teamNo = 2;
            }
            else
            {
                _teamNo = 3;
            };

            var _groupList = point.InventoryGroupService.SelectGroupNo(Convert.ToInt32(_teamNo));
            return Json(_groupList);
        }
        private void SetTeamNo(object teamNo = null)
        {
            string[] _tt = Enum.GetNames(typeof(TeamNo));

            SetGroupNo(null, 1);

            IList<string> enumList = _tt.ToList();

            var _selectList = new SelectList(enumList, teamNo);
            ViewData.Add("_teamNo", _selectList); // index.cshtml de dropdownlist te => _teamNo  yazdığımız yere bu bilgi gönderilecek
        }

        private void SetStockStore(object storeNo = null)
        {
            var _stockStoreList = point.StoreService.SelectStockStoreNo();
            var _selectList = new SelectList(_stockStoreList, storeNo);

            ViewData.Add("_stockStore", _selectList);
        }
        private void SetStockPlace(object placesNo = null)
        {
            var _stockPlaceList = point.StoreService.SelectStockPlaceNo();
            var _selectList = new SelectList(_stockPlaceList, placesNo);
            ViewData.Add("_stockPlace", _selectList);
            ViewData.Add("_endStockPlace", _selectList);
        }

        private void SetGroupNo(object groupNo = null, object teamNo = null)
        {
            var _groupList = point.InventoryGroupService.SelectGroupNo(Convert.ToInt32(teamNo));
            var _selectList = new SelectList(_groupList, groupNo);
            ViewData.Add("_groupNoL", _selectList);
        }

        private void SetMissionNo(object missionNo = null)
        {
            string[] _tt = Enum.GetNames(typeof(MissionStatus));

            IList<string> enumList = _tt.ToList();

            var _selectList = new SelectList(enumList, missionNo);
            ViewData.Add("_missionStatus", _selectList); // index.cshtml de dropdownlist te => _teamNo  yazdığımız yere bu bilgi gönderilecek
        }

        [HttpPost]
        public ActionResult LogOffOtherUser(string id)
        {
            string _mail = point.UserService.GetMailForName(id);

            int _userId = point.UserService.GetID(_mail);

            string _message = point.UserService.UpdateOnline(_userId, 0);

            if (_message == "True")
            {
                return Json("True");
            }
            return Json("False");
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

        [HttpGet]
        public ActionResult Error404(string str)
        {
            //return RedirectToAction("Index","One",new { id = "EAKGUL"});

            string xquery = Request.Url.ToString(); // search query

            string areaName = RouteData.DataTokens["area"].ToString();

            string degistir = "http://" + Request.Url.Authority.ToString() + Request.Url.AbsolutePath.ToString() + "?aspxerrorpath=/" + areaName;

            xquery = xquery.Replace(degistir, "");

            @TempData["Search"] = xquery;

            if (TempData["Search"] != null)
            {
                ViewBag.Search = xquery;
            }
            return View();
        }
    }
}
