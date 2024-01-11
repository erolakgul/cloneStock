using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StockHaus.Core.BaseService;
using StockHaus.Data.Model;
using StockHaus.ModelClass.AdminPage;
using StockHaus.ModelClass.AreaController;
using StockHaus.ModelClass.UserPage;

namespace StockHaus.Core.DataService
{
    public class InventoryService : ServiceBase<Inventories>
    {
        public override MessageService Insert(Inventories dto)
        {
            if (context.Inventory.Any(x => !x.IsDeleted && x.InventoryCode == dto.InventoryCode && x.MatCode == dto.MatCode && x.MatIndex == dto.MatIndex && x.MatSerialNumber == dto.MatSerialNumber && x.MatSection == dto.MatSection && x.MatSectionPlace == dto.MatSectionPlace && x.IsActive))
            {
                result.ResultID = 0;
                if (result.IsSuccess == false)
                {
                    result.Message = "Kayıt başarısız ..";
                    return result;
                }
            }

            bool isExist = (dto == null) ? false : true;

            if (isExist)
            {
                dto.IsActive = true;
                try
                {
                    result.Message = "True";
                    dbset.Add(dto);
                    context.SaveChanges();
                }
                catch (Exception ex)
                {
                    result.Message = ex.Message;
                }
            }
            return result;
        }

        public override MessageService Update(Inventories dto)
        {
            if (dbset.Any(x => x.InventoryCode == dto.InventoryCode && !x.IsSigned && !x.IsDeleted))
            {
                Inventories invt = FindByID(dto.ID);
                invt.IsSigned = true;
                invt.SignedTime = DateTime.Now;

                result.ResultID = invt.ID;
                if (result.IsSuccess == true)
                {
                    result.Message = "Güncelleme başarılı..";
                    context.SaveChanges();
                    return result;
                }
            }
            else if (dbset.Any(z => z.ID == dto.ID)) /*malzeme adet güncellemesi için yazııldı*/
            {
                Inventories invtx = FindByID(dto.ID);
                invtx.MatQuantity = dto.MatQuantity;
                invtx.MatIndex = dto.MatIndex;
                invtx.MatSerialNumber = dto.MatSerialNumber;

                context.SaveChanges();
                result.Message = invtx.InventoryCode;
                return result;
            }
            result.Message = "Güncelleme başarısız..";
            return result;
        }

        public string UpdateSign(InventoriesModel d)
        {
            Inventories dto = FindByID(d.ID);
            dto.IsSigned = d.IsSigned;
            dto.SignedTime = d.SignedTime;
            dto.SignedBy = d.SignedBy;

            try
            {
                context.SaveChanges();
                return "True";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public int UpdateIsActiveStatus(InventoriesModel dto)
        {
            if (dbset.Any(x => x.ID == dto.ID))
            {
                Inventories invt = FindByID(dto.ID);
                invt.IsActive = false;
                invt.ApprovedPers = dto.ApprovedPers;
                invt.ApprovedDate = DateTime.Now;

                try
                {
                    context.SaveChanges();
                    return 1;
                }
                catch (Exception)
                {
                    return 0;
                }

            }
            return 0;
        }

        public int UpdateMaterial(InventoriesModel dto)
        {
            if (dbset.Any(x => x.ID == dto.ID))
            {
                Inventories invt = FindByID(dto.ID);
                invt.IsApproved = true;
                invt.ApprovedPers = dto.ApprovedPers;
                invt.ApprovedDate = DateTime.Now;

                try
                {
                    context.SaveChanges();
                    return 1;
                }
                catch (Exception)
                {
                    return 0;
                }
            }
            return 0;
        }

        public List<InventoriesModel> GetDataForExcel(string depo,string stokyeri)
        {
            List<InventoriesModel> first = new List<InventoriesModel>();
            List<InventoriesModel> second = new List<InventoriesModel>();

            // alan sorumlusu yada 3.takım tarafından onaylanmış malzemelerin sonuç belgesi listesi
            first = context.Inventory.Where(x => !x.IsDeleted && x.IsApproved && x.IsActive && x.MatSection == depo && x.MatSectionPlace.Contains(stokyeri))
                    .Select(d => new InventoriesModel
                    {
                        //ApprovedDate = d.ApprovedDate,
                        //ApprovedPers = d.ApprovedPers,
                        //ChangeDate = d.ChangeDate,
                        //ChangedBy = d.ChangedBy,
                        //Company = "01",
                        //CreateDate = d.CreateDate,
                        //CreatedBy = d.CreatedBy,
                        InventoryCode = d.InventoryCode,
                        //IpAddress = d.IpAddress,
                        //IsActive = d.IsActive,
                        //IsApproved = d.IsApproved,
                        //IsSigned = d.IsSigned,
                        MatCode = d.MatCode,
                        MatIndex = d.MatIndex,
                        MatName = d.MatName,
                        MatQuantity = d.MatQuantity,
                        MatSection = d.MatSection,
                        MatSectionPlace = d.MatSectionPlace,
                        MatSerialNumber = d.MatSerialNumber,
                        MatSpecialStock = d.MatSpecialStock,
                        //SignedBy = d.SignedBy,
                        //SignedTime = d.SignedTime
                    }).ToList();

            // sıfır olarak imzalanmış sonuç belgesi listesi
            second = context.Inventory.Where(x => !x.IsDeleted && x.IsSigned && x.IsActive && x.MatSection == depo && x.MatSectionPlace.Contains(stokyeri))
                    .Select(d => new InventoriesModel
                    {
                        //ApprovedDate = d.ApprovedDate,
                        //ApprovedPers = d.ApprovedPers,
                        //ChangeDate = d.ChangeDate,
                        //ChangedBy = d.ChangedBy,
                        //Company = "01",
                        //CreateDate = d.CreateDate,
                        //CreatedBy = d.CreatedBy,
                        InventoryCode = d.InventoryCode,
                        //IpAddress = d.IpAddress,
                        //IsActive = d.IsActive,
                        //IsApproved = d.IsApproved,
                        //IsSigned = d.IsSigned,
                        MatCode = d.MatCode,
                        MatIndex = d.MatIndex,
                        MatName = d.MatName,
                        MatQuantity = d.MatQuantity,
                        MatSection = d.MatSection,
                        MatSectionPlace = d.MatSectionPlace,
                        MatSerialNumber = d.MatSerialNumber,
                        MatSpecialStock = d.MatSpecialStock,
                        //SignedBy = d.SignedBy,
                        //SignedTime = d.SignedTime
                    }).ToList();

            first.AddRange(second);

            return first;
        }

        public string BeforePassive(InventoriesModel d)
        {
            // o malzemeden sayılmış başka sayım var mı diye kontrol edilir
            var _mod = context.Inventory.Where(x => x.MatCode == d.MatCode && x.MatIndex == d.MatIndex && x.MatSerialNumber == d.MatSerialNumber && x.MatSection == d.MatSection && x.MatSectionPlace == d.MatSectionPlace && !x.IsDeleted).ToList();

            foreach (var item in _mod)
            {
                Inventories dto = context.Inventory.Where(x => x.ID == item.ID).SingleOrDefault();
                dto.IsActive = false;
                dto.IsApproved = false;
                dto.ApprovedPers = d.ApprovedPers;
                dto.ApprovedDate = d.ApprovedDate;

                try
                {
                    context.SaveChanges();
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
            return "True";
        }

        public string BeforeInsert(InventoriesModel d)
        {

            Inventories dto = new Inventories
            {
                ApprovedDate = d.ApprovedDate,
                ApprovedPers = d.ApprovedPers,
                ChangeDate = d.ChangeDate,
                ChangedBy = d.ChangedBy,
                Company = "01",
                CreateDate = d.CreateDate,
                CreatedBy = d.CreatedBy,
                InventoryCode = d.InventoryCode,
                IpAddress = d.IpAddress,
                IsActive = d.IsActive,
                IsApproved = d.IsApproved,
                IsSigned = d.IsSigned,
                MatCode = d.MatCode,
                MatIndex = d.MatIndex,
                MatName = d.MatName,
                MatQuantity = d.MatQuantity,
                MatSection = d.MatSection,
                MatSectionPlace = d.MatSectionPlace,
                MatSerialNumber = d.MatSerialNumber,
                MatSpecialStock = d.MatSpecialStock,
                SignedBy = d.SignedBy,
                SignedTime = d.SignedTime
            };
            MessageService ms = Insert(dto);
            return ms.Message;
        }

        public int UpdateInventory(InventoriesModel dto)
        {
            if (dbset.Any(x => x.ID == dto.ID))
            {
                Inventories invt = FindByID(dto.ID);
                invt.MatIndex = dto.MatIndex;
                invt.MatQuantity = dto.MatQuantity;
                invt.MatSerialNumber = dto.MatSerialNumber;
                invt.MatSpecialStock = dto.MatSpecialStock;

                try
                {
                    context.SaveChanges();
                    return 1;
                }
                catch (Exception)
                {
                    return 0;
                }

            }
            return 0;
        }

        public int DeleteInventRecord(int id, string _mail)
        {
            Inventories entity = FindByID(id);
            entity.IsDeleted = true;
            entity.ChangeDate = DateTime.Now;
            entity.ChangedBy = _mail;

            if (entity == null)
            {
                return 0;
            }
            // silmeden hallediyruz.
            try
            {
                //context.Inventory.Remove(entity);
                context.SaveChanges();
                return 1;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public List<InventoriesModel> Cancel(MissionAssign dto)
        {
            string _depo = dto._ware;

            List<InventoriesModel> _OR = new List<InventoriesModel>();

            foreach (var item in dto._matList)
            {
                int id = int.Parse(item._id);

                Inventories _d = FindByID(id);
                _d.IsApproved = false;
                _d.ApprovedDate = DateTime.Parse("29.10.1923");
                _d.ApprovedPers = "";

                try
                {
                    // onaylanmışlar iptal edildi
                    context.SaveChanges();
                }
                catch (Exception)
                {
                    return null;
                }

                if (String.IsNullOrEmpty(item._serNo))
                {
                    item._serNo = "";
                }

                // isactiv i false olmuşları tekrar true yap
                Inventories _md = context.Inventory.Where(x => !x.IsActive && !x.IsDeleted && !x.IsApproved && x.MatCode == item._matCode && x.MatIndex == item._matIndis && x.MatSerialNumber == item._serNo && x.MatSection == _depo && x.MatSectionPlace == item._stockPlace).SingleOrDefault();
                _md.IsActive = true;
                _md.ApprovedPers = "";
                _md.ApprovedDate = DateTime.Parse("29.10.1923");

                try
                {
                    // isactive i false yapılmış olanlar geri alındı
                    context.SaveChanges();
                }
                catch (Exception)
                {
                    return null;
                }

                // onayı iptal edilmiş modelin verisi panel e geri gönderilir,oradan loglanır
                InventoriesModel mod = new InventoriesModel
                {
                    ChangeDate = DateTime.Now,
                    ChangedBy = item._user,
                    Company = "01",
                    CreateDate = DateTime.Now,
                    CreatedBy = item._user,
                    InventoryCode = _d.InventoryCode,
                    IsActive = true,
                    MatCode = _d.MatCode,
                    MatIndex = _d.MatIndex,
                    MatName = _d.MatName,
                    MatQuantity = _d.MatQuantity,
                    MatSection = _d.MatSection,
                    MatSectionPlace = _d.MatSectionPlace,
                    MatSerialNumber = _d.MatSerialNumber,
                    MatSpecialStock = _d.MatSpecialStock,
                    ApprovedDate = _d.ApprovedDate,
                    ApprovedPers = _d.ApprovedPers,
                    IpAddress = _d.IpAddress,
                    IsApproved = _d.IsApproved,
                    IsSigned = _d.IsSigned,
                    SignedBy = _d.SignedBy,
                    SignedTime = _d.SignedTime
                };
                _OR.Add(mod);

            }

            return _OR;
        }


        // trace onaylanmış bir malzeme varsa aynı depo stokyerinde diğer malzeme onaylanamasın
        public bool IsApproved(string matcode, string matindex, string matSpec, string matSerNo, string warehouse, string stockplace)
        {
            string _invcode = warehouse + "X" + stockplace;

            var _listOfMaterialInvent = context.Inventory.Where(x => x.IsApproved && x.IsActive && !x.IsDeleted && x.MatCode == matcode && x.MatIndex == matindex && x.MatSpecialStock == matSpec && x.MatSerialNumber == matSerNo && x.MatSection == warehouse && x.MatSectionPlace == stockplace).ToList();

            foreach (var item in _listOfMaterialInvent)
            {
                if (item.IsApproved)
                {
                    return true;
                }
            }
            return false;
        }
        public bool IsZero(string matcode, string matindex, string matSpec, string matSerNo, string warehouse, string stockplace)
        {
            string _invcode = warehouse + "X" + stockplace;

            var _listOfMaterialInvent = context.Inventory.Where(x => x.IsActive && !x.IsDeleted && x.MatCode == matcode && x.MatIndex == matindex && x.MatSpecialStock == matSpec && x.MatSerialNumber == matSerNo && x.MatSection == warehouse && x.MatSectionPlace == stockplace).ToList();

            foreach (var item in _listOfMaterialInvent)
            {
                if (item.MatQuantity == 0)
                {
                    return true;
                }
            }
            return false;
        }

        public ICollection<InventoriesModel> FindIDListInventoryCode(string code)
        {
            var _inventoryNotApprovedList = context.Inventory.Where(x => x.InventoryCode == code && x.IsActive && !x.IsSigned && !x.IsDeleted).Select(d => new InventoriesModel
            {
                ApprovedDate = d.ApprovedDate,
                ApprovedPers = d.ApprovedPers,
                ChangeDate = d.ChangeDate,
                ChangedBy = d.ChangedBy,
                Company = "01",
                CreateDate = d.CreateDate,
                CreatedBy = d.CreatedBy,
                InventoryCode = d.InventoryCode,
                IpAddress = d.IpAddress,
                IsActive = d.IsActive,
                IsApproved = d.IsApproved,
                IsSigned = d.IsSigned,
                MatCode = d.MatCode,
                MatIndex = d.MatIndex,
                MatName = d.MatName,
                MatQuantity = d.MatQuantity,
                MatSection = d.MatSection,
                MatSectionPlace = d.MatSectionPlace,
                MatSerialNumber = d.MatSerialNumber,
                MatSpecialStock = d.MatSpecialStock,
                SignedBy = d.SignedBy,
                SignedTime = d.SignedTime

            }).ToList();
            return _inventoryNotApprovedList;
        }

        public string GetInventResultFromName(string name, string _teamNo)
        {
            string[] _deps = _teamNo.Split('X'); // 1X1
            string _team = _deps[0];
            string _gr = _deps[1];

            //_teamNo = "X" + _teamNo; // X1X1 şeklinde olur böylece

            int _inventCode = context.InventoryCodeLog.Where(x => x.IsActive && x.TeamNo == _team && x.GroupNo == _gr).Select(c => c.CountOrder).SingleOrDefault();//context.Inventory.Where(x => x.IsActive && !x.IsSigned && x.InventoryCode.Contains(_teamNo)).Select(c => c.InventoryCode).Distinct().SingleOrDefault();

            string _invCode = _inventCode + "X" + _team + "X" + _gr;

            return _invCode;
        }

        public ICollection<InventoriesModel> GetList(string docNum)
        {
            var _list = context.Inventory.Where(x => x.InventoryCode == docNum && !x.IsDeleted).Select(d => new InventoriesModel
            {
                ApprovedDate = d.ApprovedDate,
                ApprovedPers = d.ApprovedPers,
                ChangeDate = d.ChangeDate,
                ChangedBy = d.ChangedBy,
                Company = "01",
                CreateDate = d.CreateDate,
                CreatedBy = d.CreatedBy,
                InventoryCode = d.InventoryCode,
                IpAddress = d.IpAddress,
                IsActive = d.IsActive,
                IsApproved = d.IsApproved,
                IsSigned = d.IsSigned,
                MatCode = d.MatCode,
                MatIndex = d.MatIndex,
                MatName = d.MatName,
                MatQuantity = d.MatQuantity,
                MatSection = d.MatSection,
                MatSectionPlace = d.MatSectionPlace,
                MatSerialNumber = d.MatSerialNumber,
                MatSpecialStock = d.MatSpecialStock,
                SignedBy = d.SignedBy,
                SignedTime = d.SignedTime

            }).ToList();
            return _list;
        }

        public ICollection<InventoriesModel> GetItems(string inventCode)
        {
            var _list = context.Inventory.Where(x => x.InventoryCode == inventCode && x.IsActive && !x.IsSigned && !x.IsDeleted).Select(d => new InventoriesModel
            {
                ApprovedDate = d.ApprovedDate,
                ApprovedPers = d.ApprovedPers,
                ChangeDate = d.ChangeDate,
                ChangedBy = d.ChangedBy,
                Company = "01",
                CreateDate = d.CreateDate,
                CreatedBy = d.CreatedBy,
                InventoryCode = d.InventoryCode,
                IpAddress = d.IpAddress,
                IsActive = d.IsActive,
                IsApproved = d.IsApproved,
                IsSigned = d.IsSigned,
                MatCode = d.MatCode,
                MatIndex = d.MatIndex,
                MatName = d.MatName,
                MatQuantity = d.MatQuantity,
                MatSection = d.MatSection,
                MatSectionPlace = d.MatSectionPlace,
                MatSerialNumber = d.MatSerialNumber,
                MatSpecialStock = d.MatSpecialStock,
                SignedBy = d.SignedBy,
                SignedTime = d.SignedTime

            }).ToList();
            return _list;
        }

        public ICollection<InventoriesModel> SeachGetItems(string inventCode, string searchChar)
        {
            var _list = context.Inventory.Where(x => x.InventoryCode == inventCode && x.MatCode.Contains(searchChar) && x.IsActive && !x.IsSigned && !x.IsDeleted)
                    .Select(d => new InventoriesModel
                    {
                        ApprovedDate = d.ApprovedDate,
                        ApprovedPers = d.ApprovedPers,
                        ChangeDate = d.ChangeDate,
                        ChangedBy = d.ChangedBy,
                        Company = "01",
                        CreateDate = d.CreateDate,
                        CreatedBy = d.CreatedBy,
                        InventoryCode = d.InventoryCode,
                        IpAddress = d.IpAddress,
                        IsActive = d.IsActive,
                        IsApproved = d.IsApproved,
                        IsSigned = d.IsSigned,
                        MatCode = d.MatCode,
                        MatIndex = d.MatIndex,
                        MatName = d.MatName,
                        MatQuantity = d.MatQuantity,
                        MatSection = d.MatSection,
                        MatSectionPlace = d.MatSectionPlace,
                        MatSerialNumber = d.MatSerialNumber,
                        MatSpecialStock = d.MatSpecialStock,
                        SignedBy = d.SignedBy,
                        SignedTime = d.SignedTime

                    }).ToList();

            return _list;
        }

        public ICollection<InventoriesModel> GetInventResultFromId(int id)
        {
            string _inventCode = context.Inventory.Where(x => x.ID == id).Select(z => z.InventoryCode).SingleOrDefault();

            return context.Inventory.Where(z => z.InventoryCode == _inventCode && z.IsActive && !z.IsSigned && !z.IsDeleted).
                 Select(d => new InventoriesModel
                 {
                     ApprovedDate = d.ApprovedDate,
                     ApprovedPers = d.ApprovedPers,
                     ChangeDate = d.ChangeDate,
                     ChangedBy = d.ChangedBy,
                     Company = "01",
                     CreateDate = d.CreateDate,
                     CreatedBy = d.CreatedBy,
                     InventoryCode = d.InventoryCode,
                     IpAddress = d.IpAddress,
                     IsActive = d.IsActive,
                     IsApproved = d.IsApproved,
                     IsSigned = d.IsSigned,
                     MatCode = d.MatCode,
                     MatIndex = d.MatIndex,
                     MatName = d.MatName,
                     MatQuantity = d.MatQuantity,
                     MatSection = d.MatSection,
                     MatSectionPlace = d.MatSectionPlace,
                     MatSerialNumber = d.MatSerialNumber,
                     MatSpecialStock = d.MatSpecialStock,
                     SignedBy = d.SignedBy,
                     SignedTime = d.SignedTime
                 }).ToList();
        }

        public ICollection<InventoriesModel> MatIsApproved(int id)
        {
            var _f = context.Inventory.Where(x => x.IsActive && x.ID == id && !x.IsDeleted)
                 .Select(d => new InventoriesModel
                 {
                     ApprovedDate = d.ApprovedDate,
                     ApprovedPers = d.ApprovedPers,
                     ChangeDate = d.ChangeDate,
                     ChangedBy = d.ChangedBy,
                     Company = "01",
                     CreateDate = d.CreateDate,
                     CreatedBy = d.CreatedBy,
                     InventoryCode = d.InventoryCode,
                     IpAddress = d.IpAddress,
                     IsActive = d.IsActive,
                     IsApproved = d.IsApproved,
                     IsSigned = d.IsSigned,
                     MatCode = d.MatCode,
                     MatIndex = d.MatIndex,
                     MatName = d.MatName,
                     MatQuantity = d.MatQuantity,
                     MatSection = d.MatSection,
                     MatSectionPlace = d.MatSectionPlace,
                     MatSerialNumber = d.MatSerialNumber,
                     MatSpecialStock = d.MatSpecialStock,
                     SignedBy = d.SignedBy,
                     SignedTime = d.SignedTime
                 }).ToList();//.Select(c => c.IsApproved).SingleOrDefault();

            return _f;
        }

        public int RemoveApproveX(string _email, string invtCode, string matCode, string matIndex, string matSerNo, string matSec, string matSecPlace)
        {
            string[] _invCodeArray = invtCode.Split('X');// SAYIM KODU - TEAM NO - GROUP NO
            string _inventoryCode = _invCodeArray[0];
            string _sN = "";
            if (matSerNo != null)
            {
                _sN = matSerNo;
            }

            var _invList = context.Inventory.Where(x => x.MatCode == matCode && x.MatIndex == matIndex && x.IsActive && !x.IsDeleted && x.MatSerialNumber == _sN && x.MatSection == matSec && x.MatSectionPlace == matSecPlace).ToList();

            foreach (var item in _invList)
            {
                // diğer sayımlara ait veriler böylece false edilmiş olur
                int _invId = item.ID;//context.Inventory.Where(x => x.MatCode == item.MatCode && x.MatIndex == item.MatIndex  && x.IsActive && x.MatSerialNumber == item.MatSerialNumber && x.MatSection == item.MatSection && x.MatSectionPlace == item.MatSectionPlace).Select(y => y.ID).SingleOrDefault();
                Inventories invtc = FindByID(_invId);
                invtc.IsApproved = false;
                invtc.IsActive = false;
                invtc.ApprovedPers = _email;

                try
                {
                    context.SaveChanges();
                    return 1;
                }
                catch (Exception)
                {
                    return 0;
                }
            }
            return 1;
        }

        public ICollection<InventoriesModel> GetListMaterial(string _email, string _mC, string _mS, string _mSP, string _isInc)
        {
            ICollection<Inventories> _list = null;
            List<InventoriesModel> _other = new List<InventoriesModel>(); // rolü 4000 olmayanların saydıklarını da getirmek için

            if (_mC != null)
            {
                if (_isInc == "1")
                {
                    //_list = context.Inventory.Where(x => x.MatCode.Contains(_mC) && x.MatSection == _mS && x.MatSectionPlace == _mSP && x.IsActive && x.IsApproved && x.ApprovingPers == _email).ToList();
                    _list = context.Inventory.Where(x => x.MatCode.Contains(_mC) && x.MatSection == _mS && x.MatSectionPlace == _mSP && x.IsActive && x.IsApproved && !x.IsDeleted).ToList();
                }
                else if (_isInc == "0")
                {
                    //_list = context.Inventory.Where(x => x.MatCode.Contains(_mC) && x.IsActive && x.IsApproved && x.ApprovingPers == _email).ToList();
                    _list = context.Inventory.Where(x => x.MatCode.Contains(_mC) && x.IsActive && x.IsApproved && !x.IsDeleted).ToList();
                }
            }
            else
            {
                if (_isInc == "1")
                {
                    //_list = context.Inventory.Where(x => x.MatSection == _mS && x.MatSectionPlace == _mSP && x.IsActive && x.IsApproved && x.ApprovingPers == _email).ToList();
                    _list = context.Inventory.Where(x => x.MatSection == _mS && x.MatSectionPlace == _mSP && x.IsActive && x.IsApproved && !x.IsDeleted).ToList();
                }
                else if (_isInc == "0" || _isInc == null)
                {
                    //_list = context.Inventory.Where(x => x.IsActive && x.IsApproved && x.ApprovingPers == _email).ToList();
                    _list = context.Inventory.Where(x => x.IsActive && x.IsApproved && !x.IsDeleted).ToList();
                }
            }

            foreach (var c in _list)
            {
                InventoriesModel v = new InventoriesModel
                {
                    ApprovedDate = c.ApprovedDate,
                    ApprovedPers = c.ApprovedPers,
                    ChangeDate = c.ChangeDate,
                    ChangedBy = c.ChangedBy,
                    Company = c.Company,
                    CreateDate = c.CreateDate,
                    CreatedBy = c.CreatedBy,
                    ID = c.ID,
                    InventoryCode = c.InventoryCode,
                    IpAddress = c.IpAddress,
                    IsActive = c.IsActive,
                    IsApproved = c.IsApproved,
                    IsSigned = c.IsSigned,
                    MatCode = c.MatCode,
                    MatIndex = c.MatIndex,
                    MatName = c.MatName,
                    MatQuantity = c.MatQuantity,
                    MatSection = c.MatSection,
                    MatSectionPlace = c.MatSectionPlace,
                    MatSerialNumber = c.MatSerialNumber,
                    MatSpecialStock = c.MatSpecialStock,
                    SignedBy = c.SignedBy,
                    SignedTime = c.SignedTime
                };
                _other.Add(v);
            }

            return _other;
        }

        public int calcPenCount()
        {
            int _f = context.Inventory.Where(x => x.IsActive && x.IsApproved && !x.IsDeleted).Count();
            return _f;
        }

        public double calcTotalMatCount()
        {
            double _f = 0.0;

            if (context.Inventory.Any(x => x.IsActive && x.IsApproved && !x.IsDeleted))
            {
                _f = context.Inventory.Where(x => x.IsActive && x.IsApproved && !x.IsDeleted).Sum(c => c.MatQuantity);
            }

            return _f;
        }

        public bool SearchTeamGroup(string tg)
        {
            return context.Inventory.Any(x => x.InventoryCode.Contains(tg) && x.IsActive && !x.IsDeleted);
        }

        public List<WareStock> CountedWareStockPlace()
        {
            List<WareStock> _c = new List<WareStock>();

            var _ff = context.Inventory.Where(x => x.IsActive && !x.IsDeleted).Select(c => new { c.MatSection, c.MatSectionPlace }).Distinct().ToList();

            foreach (var item in _ff)
            {
                //string _fg = item.MatSection + "#" + item.MatSectionPlace;
                WareStock _v = new WareStock
                {
                    _ware = item.MatSection,
                    _stock = item.MatSectionPlace
                };
                _c.Add(_v);
            }

            return _c;
        }

        // o depo stok yerinde sayılmış malzeme adedi
        public int CountedMaterial(string wh, string sp)
        {
            //önce o depodaki sayımları al, daha sonra da 1.2. veya 3.sayımların olmuş olma ihtimaline karşı code u distinctle say
            var _countList = context.Inventory.Where(x => x.IsActive && x.MatSection == wh && x.MatSectionPlace == sp && !x.IsDeleted).ToList();  // !IsSigned kaldırıldı
            int _count = _countList.Select(z => new { z.MatCode, z.MatIndex, z.MatSerialNumber }).Distinct().Count();
            return _count;
        }

        // onaylanmış kalemlern adedi
        public int CountedApproved(string wh, string sp)
        {
            //önce o depodaki sayımları al, daha sonra da 1.2. veya 3.sayımların olmuş olma ihtimaline karşı code u distinctle say
            var _countList = context.Inventory.Where(x => x.IsActive && x.IsApproved && !x.IsDeleted && x.MatSection == wh && x.MatSectionPlace == sp).ToList();
            int _count = _countList.Select(z => new { z.MatCode, z.MatIndex, z.MatSerialNumber }).Distinct().Count();
            return _count;
        }
        public ICollection<InventoriesModel> ApprovedMaterials()
        {
            return context.Inventory.Where(x => x.IsActive && x.IsApproved && !x.IsDeleted).Select(c => new InventoriesModel
            {
                ApprovedDate = c.ApprovedDate,
                ApprovedPers = c.ApprovedPers,
                ChangeDate = c.ChangeDate,
                ChangedBy = c.ChangedBy,
                Company = c.Company,
                CreateDate = c.CreateDate,
                CreatedBy = c.CreatedBy,
                ID = c.ID,
                InventoryCode = c.InventoryCode,
                IpAddress = c.IpAddress,
                IsActive = c.IsActive,
                IsApproved = c.IsApproved,
                IsSigned = c.IsSigned,
                MatCode = c.MatCode,
                MatIndex = c.MatIndex,
                MatName = c.MatName,
                MatQuantity = c.MatQuantity,
                MatSection = c.MatSection,
                MatSectionPlace = c.MatSectionPlace,
                MatSerialNumber = c.MatSerialNumber,
                MatSpecialStock = c.MatSpecialStock,
                SignedBy = c.SignedBy,
                SignedTime = c.SignedTime
            }).ToList();
        }

        public ICollection<InventResult> GetApprovedMaterialList(string depo, string stokyeri, string SonStokyeri)
        {
            int _startId = context.Store.Where(x => x.IsActive && x.WareHouse == depo && x.StockPlace == stokyeri).Select(c => c.ID).SingleOrDefault();
            int _endId = 0;

            if (String.IsNullOrEmpty(SonStokyeri))
            {
                _endId = context.Store.Where(x => x.IsActive && x.WareHouse == depo && x.StockPlace == stokyeri).Select(c => c.ID).SingleOrDefault();
            }
            else
            {
                _endId = context.Store.Where(x => x.IsActive && x.WareHouse == depo && x.StockPlace == SonStokyeri).Select(c => c.ID).SingleOrDefault();
            }


            List<Inventories> _listX = new List<Inventories>();

            for (int i = _startId; i <= _endId; i++)
            {
                List<Inventories> _listXY = new List<Inventories>();

                var _stokyeri = context.Store.Where(c => c.IsActive && c.ID == i).Select(v => v.StockPlace).SingleOrDefault();
                var _depo = context.Store.Where(c => c.IsActive && c.ID == i).Select(v => v.WareHouse).SingleOrDefault();

                if (_listX.Count == 0)
                {
                    _listX = context.Inventory.Where(x => x.MatSection == _depo && x.MatSectionPlace == _stokyeri && x.IsActive && !x.IsDeleted && x.IsApproved)
                         .OrderBy(v => new { v.MatCode, v.MatIndex, v.MatSpecialStock, v.MatSerialNumber }).ToList();
                }
                else
                {
                    _listXY = context.Inventory.Where(x => x.MatSection == _depo && x.MatSectionPlace == _stokyeri && x.IsActive && !x.IsDeleted && x.IsApproved)
                         .OrderBy(v => new { v.MatCode, v.MatIndex, v.MatSpecialStock, v.MatSerialNumber }).ToList();

                    _listX.AddRange(_listXY);
                };
            };

            List<InventResult> _IR = new List<InventResult>();

            foreach (var item in _listX)
            {
                InventResult RR = new InventResult();
                RR.InventCode = item.InventoryCode;
                RR._matName = item.MatName;
                RR._matSerNo = item.MatSerialNumber;
                RR._matSpecStock = item.MatSpecialStock;
                RR.MatCode = item.MatCode;
                RR.MatIndex = item.MatIndex;
                RR.MatSection = item.MatSection;
                RR.MatSectionPlace = item.MatSectionPlace;
                RR._S1 = item.MatQuantity;
                RR._S1ID = item.ID;

                _IR.Add(RR);
            }

            return _IR;
        }

        public ICollection<InventResult> GetMaterialList(string depo, string stokyeri, string SonStokyeri)
        {
            int _startId = context.Store.Where(x => x.IsActive && x.WareHouse == depo && x.StockPlace == stokyeri).Select(c => c.ID).SingleOrDefault();
            int _endId = 0;

            if (String.IsNullOrEmpty(SonStokyeri))
            {
                _endId = context.Store.Where(x => x.IsActive && x.WareHouse == depo && x.StockPlace == stokyeri).Select(c => c.ID).SingleOrDefault();
            }
            else
            {
                _endId = context.Store.Where(x => x.IsActive && x.WareHouse == depo && x.StockPlace == SonStokyeri).Select(c => c.ID).SingleOrDefault();
            }


            List<Inventories> _listX = new List<Inventories>();
            List<Inventories> _listXYZ = new List<Inventories>();

            for (int i = _startId; i <= _endId; i++)
            {
                List<Inventories> _listXY = new List<Inventories>();

                var _stokyeri = context.Store.Where(c => c.IsActive && c.ID == i).Select(v => v.StockPlace).SingleOrDefault();
                var _depo = context.Store.Where(c => c.IsActive && c.ID == i).Select(v => v.WareHouse).SingleOrDefault();

                if (_listX.Count == 0)
                {
                    _listX = context.Inventory.Where(x => x.MatSection == _depo && x.MatSectionPlace == _stokyeri && x.IsActive && !x.IsDeleted && !x.IsApproved)
                         .OrderBy(v => new { v.MatCode, v.MatIndex, v.MatSpecialStock, v.MatSerialNumber }).ToList();
                    _listXYZ = context.Inventory.Where(x => x.MatSection == _depo && x.MatSectionPlace == _stokyeri && x.IsActive && !x.IsDeleted && !x.IsApproved)
                        .OrderBy(v => new { v.MatCode, v.MatIndex, v.MatSpecialStock, v.MatSerialNumber }).ToList();
                }
                else
                {
                    _listXY = context.Inventory.Where(x => x.MatSection == _depo && x.MatSectionPlace == _stokyeri && x.IsActive && !x.IsDeleted && !x.IsApproved)
                         .OrderBy(v => new { v.MatCode, v.MatIndex, v.MatSpecialStock, v.MatSerialNumber }).ToList();

                    _listX.AddRange(_listXY);
                    _listXYZ.AddRange(_listXY);
                };
            };

            List<InventResult> _IR = new List<InventResult>();

            //var _teamNo = new String(item.InventoryCode.Split('X').Select(x => x[0]).ToArray());
            int count = 0;

            foreach (var item in _listX)
            {
                InventResult RR = new InventResult();
                RR.InventCode = item.InventoryCode;
                RR._matName = item.MatName;
                RR._matSerNo = item.MatSerialNumber;
                RR._matSpecStock = item.MatSpecialStock;
                RR.MatCode = item.MatCode;
                RR.MatIndex = item.MatIndex;
                RR.MatSection = item.MatSection;
                RR.MatSectionPlace = item.MatSectionPlace;

                int goonCount = 0;

                if (_IR.Count != 0)
                {
                    foreach (var xyx in _IR)
                    {
                        if (item.MatCode == xyx.MatCode && item.MatIndex == xyx.MatIndex && item.MatSpecialStock == xyx._matSpecStock && item.MatSerialNumber == xyx._matSerNo)
                        {
                            goonCount = goonCount + 1;
                        }
                    }
                }

                if (goonCount == 0)
                {
                    var _tab2 = _listXYZ.Where(x => x.MatCode == item.MatCode && x.MatIndex == item.MatIndex && x.MatSpecialStock == item.MatSpecialStock && x.MatSerialNumber == item.MatSerialNumber).ToList();

                    foreach (var itemx in _tab2)
                    {
                        count = count + 1;

                        string[] _teamNoArray2 = itemx.InventoryCode.Split('X');

                        if (_teamNoArray2[1].ToString() == "1")
                        {
                            RR._S1 = itemx.MatQuantity;
                            RR._S1ID = itemx.ID;
                        }
                        else if (_teamNoArray2[1].ToString() == "2")
                        {
                            RR._S2 = itemx.MatQuantity;
                            RR._S2ID = itemx.ID;
                        }
                        else if (_teamNoArray2[1].ToString() == "3")
                        {
                            RR._S3 = itemx.MatQuantity;
                            RR._S3ID = itemx.ID;
                        };

                        if (_tab2.Count == count)
                        {
                            _IR.Add(RR);
                            count = 0;
                        }

                    } // foreach 2   
                }

            } // foreach 1

            return _IR;
        }

        public ICollection<InventoriesModel> GetMaterial(int id)
        {
            var _list = context.Inventory.Where(x => x.ID == id && x.IsActive && !x.IsDeleted).Select(c => new InventoriesModel
            {
                ApprovedDate = c.ApprovedDate,
                ApprovedPers = c.ApprovedPers,
                ChangeDate = c.ChangeDate,
                ChangedBy = c.ChangedBy,
                Company = c.Company,
                CreateDate = c.CreateDate,
                CreatedBy = c.CreatedBy,
                ID = c.ID,
                InventoryCode = c.InventoryCode,
                IpAddress = c.IpAddress,
                IsActive = c.IsActive,
                IsApproved = c.IsApproved,
                IsSigned = c.IsSigned,
                MatCode = c.MatCode,
                MatIndex = c.MatIndex,
                MatName = c.MatName,
                MatQuantity = c.MatQuantity,
                MatSection = c.MatSection,
                MatSectionPlace = c.MatSectionPlace,
                MatSerialNumber = c.MatSerialNumber,
                MatSpecialStock = c.MatSpecialStock,
                SignedBy = c.SignedBy,
                SignedTime = c.SignedTime
            }).ToList();
            return _list;
        }

        public List<string> FindInventCode(string matcode, string matindex, string matSpec, string matSerNo, string warehouse, string stockplace)
        {
            List<string> _c = new List<string>();
            string _f = "";

            var _listOfMaterialInvent = context.Inventory.Where(x => x.IsActive && !x.IsDeleted && x.MatCode == matcode && x.MatIndex == matindex && x.MatSpecialStock == matSpec && x.MatSerialNumber == matSerNo && x.MatSection == warehouse && x.MatSectionPlace == stockplace).ToList();

            foreach (var item in _listOfMaterialInvent)
            {
                _f = item.ID + "#" + item.InventoryCode;

                _c.Add(_f);
            }
            return _c;
        }

        // trace
        public ICollection<InventoriesModel> GetMatList(string depo, string stokyeri)
        {
            var _list = context.Inventory.Where(x => x.MatSection == depo && x.MatSectionPlace == stokyeri && x.IsActive && !x.IsDeleted && !x.IsApproved)
                       .Select(c => new InventoriesModel
                       {
                           ApprovedDate = c.ApprovedDate,
                           ApprovedPers = c.ApprovedPers,
                           ChangeDate = c.ChangeDate,
                           ChangedBy = c.ChangedBy,
                           Company = c.Company,
                           CreateDate = c.CreateDate,
                           CreatedBy = c.CreatedBy,
                           ID = c.ID,
                           InventoryCode = c.InventoryCode,
                           IpAddress = c.IpAddress,
                           IsActive = c.IsActive,
                           IsApproved = c.IsApproved,
                           IsSigned = c.IsSigned,
                           MatCode = c.MatCode,
                           MatIndex = c.MatIndex,
                           MatName = c.MatName,
                           MatQuantity = c.MatQuantity,
                           MatSection = c.MatSection,
                           MatSectionPlace = c.MatSectionPlace,
                           MatSerialNumber = c.MatSerialNumber,
                           MatSpecialStock = c.MatSpecialStock,
                           SignedBy = c.SignedBy,
                           SignedTime = c.SignedTime
                       }).ToList();

            return _list;
        }

        public ICollection<InventoriesModel> InvToList()
        {
            return context.Inventory.Where(x => x.IsActive && !x.IsDeleted).Select(c => new InventoriesModel
                {
                    ApprovedDate = c.ApprovedDate,
                    ApprovedPers = c.ApprovedPers,
                    ChangeDate = c.ChangeDate,
                    ChangedBy = c.ChangedBy,
                    Company = c.Company,
                    CreateDate = c.CreateDate,
                    CreatedBy = c.CreatedBy,
                    ID = c.ID,
                    InventoryCode = c.InventoryCode,
                    IpAddress = c.IpAddress,
                    IsActive = c.IsActive,
                    IsApproved = c.IsApproved,
                    IsSigned = c.IsSigned,
                    MatCode = c.MatCode,
                    MatIndex = c.MatIndex,
                    MatName = c.MatName,
                    MatQuantity = c.MatQuantity,
                    MatSection = c.MatSection,
                    MatSectionPlace = c.MatSectionPlace,
                    MatSerialNumber = c.MatSerialNumber,
                    MatSpecialStock = c.MatSpecialStock,
                    SignedBy = c.SignedBy,
                    SignedTime = c.SignedTime
                }).ToList();
        }

        public bool AllInvent(string inventoryCode)
        {
            return context.Inventory.Any(x => x.IsActive && x.InventoryCode == inventoryCode && !x.IsDeleted);
        }

        public bool CheckActive(string teamNo, string groupNo)
        {
            // daha önce bu takım ve group nosu eğer bi yerde kayıtlı ise o kayda ilişkin belgesi imzalanmamış veri var mı diye bakarız
            // eğer true ise  kayıt var demektir uyarı olarak ekip sayım yapıyor diye uyarı veriririz.

            string _rigthInventoryCode = "X" + teamNo + "X" + groupNo;

            var _listInvent = context.Inventory.Where(x => x.InventoryCode.Contains(_rigthInventoryCode) && x.IsActive && !x.IsSigned && !x.IsDeleted).ToList();

            if (_listInvent.Count != 0)
            {
                return true;
            }
            return false;
        }

        public int FindMaterialFromMatId(MaterialsModel dto, TeamGroup _teamGroupNo)
        {
            string _xs = "X" + _teamGroupNo._team + "X" + _teamGroupNo._group;

            int _mat = context.Inventory.Where(x => x.IsActive && !x.IsDeleted && x.InventoryCode.Contains(_xs) && x.MatCode == dto.MatCode && x.MatIndex == dto.MatIndex && x.MatSerialNumber == dto.MatSerialNumber && x.MatSection == dto.MatSection && x.MatSectionPlace == dto.MatSectionPlace).Select(c => c.ID).SingleOrDefault();

            return _mat;

        }

        public InventoriesModel GetActiveInventList(string matcode, string matindex, string inventcode, string matser, string matsec, string matsecplace)
        {
            return context.Inventory.Where(x => x.MatCode == matcode && x.MatIndex == matindex && x.InventoryCode == inventcode && !x.IsDeleted  //&& x.IsActive 3.TAKIM tarafından da sayılmış olabilme ihtimalinden dolayı kaldırıldı
                                                             && x.MatSerialNumber == matser && x.MatSection == matsec && x.MatSectionPlace == matsecplace
                                                        ).
                                                        Select(c => new InventoriesModel
                                                             {
                                                                 ID = c.ID,
                                                                 MatQuantity = c.MatQuantity,
                                                                 MatIndex = c.MatIndex,
                                                                 MatSerialNumber = c.MatSerialNumber,
                                                                 IsApproved = c.IsApproved,
                                                                 MatSection = c.MatSection,
                                                                 MatSectionPlace = c.MatSectionPlace,
                                                                 IsActive = c.IsActive,
                                                                 ApprovedPers = c.ApprovedPers
                                                             }).SingleOrDefault();
        }

        public InventoriesModel GetApprovedInventList(string matcode, string matindex, string inventcode, string matser, string matsec, string matsecplace)
        {
            return context.Inventory.Where(x => x.MatCode == matcode && x.MatIndex == matindex && x.InventoryCode == inventcode && !x.IsActive && x.ApprovedPers != ""
                                                             && x.MatSerialNumber == matser && x.MatSection == matsec && x.MatSectionPlace == matsecplace && !x.IsDeleted
                                                        ).
                                                        Select(c => new InventoriesModel
                                                        {
                                                            ID = c.ID,
                                                            MatQuantity = c.MatQuantity,
                                                            MatIndex = c.MatIndex,
                                                            MatSerialNumber = c.MatSerialNumber,
                                                            IsApproved = c.IsApproved,
                                                            MatSection = c.MatSection,
                                                            MatSectionPlace = c.MatSectionPlace
                                                        }).SingleOrDefault();
        }

        public bool IsCounted(string inventCode, string matCode, string matIndex, string matSer, string _depo, string _stokyeri)
        {
            // sayım kodu aynı olan ve aynı malzeme kodu olandan aktif olan malzeme varsa o daha önce sayılmış demektir.
            return context.Inventory.Any(x => x.InventoryCode == inventCode && x.MatCode == matCode && x.MatIndex == matIndex && x.MatSerialNumber == matSer && x.MatSection == _depo && x.MatSectionPlace == _stokyeri && x.IsActive && !x.IsDeleted);
        }

    }
}
