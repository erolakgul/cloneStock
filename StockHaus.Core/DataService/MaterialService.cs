using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StockHaus.Core.BaseService;
using StockHaus.Data.Model;
using StockHaus.ModelClass.AdminPage;
using StockHaus.ModelClass.UserPage;

namespace StockHaus.Core.DataService
{
    public class MaterialService : ServiceBase<Materials>
    {
        public override MessageService Insert(Materials dto)
        {
            if (context.Material.Any(x => x.MatCode == dto.MatCode && x.MatIndex == dto.MatIndex &&
                                     x.MatSerialNumber == dto.MatSerialNumber && x.MatSection == dto.MatSection
                                     && x.MatSectionPlace == dto.MatSectionPlace))
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

        public override MessageService Update(Materials dto)
        {
            throw new NotImplementedException();
        }

        public string BeforeInsert(MaterialsModel d)
        {
            Materials dto = new Materials
            {
                MatCode = d.MatCode,
                MatName = d.MatName,
                MatSection = d.MatSection,
                MatSectionPlace = d.MatSectionPlace,
                MatQuantity = d.MatQuantity,
                MatIndex = d.MatIndex,
                MatSpecialStock = d.MatSpecialStock,
                MatSerialNumber = d.MatSerialNumber,
                IpAddress = d.IpAddress,
                IsActive = d.IsActive,
                MatSerNoType = d.MatSerNoType,
                MatType = d.MatType,
                IsCounted = d.IsCounted,
                ChangeDate = d.ChangeDate,
                ChangedBy = d.ChangedBy,
                Company = d.Company,
                CreateDate = d.CreateDate,
                CreatedBy = d.CreatedBy,
            };
            MessageService msg = Insert(dto);
            return msg.Message;
        }

        public string UpdateIsCount(MaterialsModel dto)
        {
            int _matId = context.Material.Where(x => x.MatCode == dto.MatCode && x.MatIndex == dto.MatIndex && x.MatSection == dto.MatSection && x.MatSectionPlace == dto.MatSectionPlace && x.MatSerialNumber == dto.MatSerialNumber && x.IsActive).Select(c => c.ID).SingleOrDefault();

            Materials _dto = FindByID(_matId);
            _dto.IsCounted = "1";
            try
            {
                context.SaveChanges();
                return "true";
            }
            catch (Exception)
            {
                return "false";
            }
     
        }

        // Sayım iptali butonu ile material tablosundaki iscounted ı 1 olan malzemelerin iscounted ını 0 a çeker
        public string CancelIsCounted(List<InventoriesModel> dtos)
        {
            int count = 0;
            string err = "";

            foreach (var item in dtos)
            {
                Materials d = context.Material.Where(x => x.IsActive && x.MatCode == item.MatCode && x.MatIndex == item.MatIndex && x.MatSpecialStock == item.MatSpecialStock && x.MatSerialNumber == item.MatSerialNumber && x.MatSection == item.MatSection && x.MatSectionPlace == item.MatSectionPlace).SingleOrDefault();
                d.IsCounted = "0";

                try
                {
                    context.SaveChanges();
                    count = count + 1;
                }
                catch (Exception ex)
                {
                    err = err + ex.Message;
                }
            }
            if (count != dtos.Count)
            {
                return err;
            }
            return "True";
        }

        public string IsCountedStatus(string _mCode, string _mInd, string _mSer, string _mSpe, string _mStor, string _mPlac)
        {
            string _g = context.Material.Where(x => x.MatCode == _mCode && x.MatIndex == _mInd && x.MatSerialNumber == _mSer && x.MatSpecialStock == _mSpe && x.MatSection == _mStor && x.MatSectionPlace == _mPlac).Select(c => c.IsCounted).SingleOrDefault();

            return _g;
        }

        public int FindMaterialFromMatId(Materials dto, string _teamGroupNo)
        {
            string _xs = "X" + _teamGroupNo;

            int _mat = context.Inventory.Where(x => x.IsActive && x.InventoryCode.Contains(_xs) && x.MatCode == dto.MatCode && x.MatIndex == dto.MatIndex && x.MatSerialNumber == dto.MatSerialNumber && x.MatSection == dto.MatSection && x.MatSectionPlace == dto.MatSectionPlace).Select(c => c.ID).SingleOrDefault();

            return _mat;

        }

        public string FindMaterialName(string matcode)
        {
            return context.Material.Where(x => x.IsActive && x.MatCode == matcode).Select(c=> c.MatName).FirstOrDefault();
        }

        public MaterialsModel FindModelByID(int id)
        {
            return context.Material.Where(x => x.IsActive && x.ID == id)
                .Select(itemz =>
                 new MaterialsModel
                 {
                     ID = itemz.ID,
                     IpAddress = itemz.IpAddress,
                     IsActive = itemz.IsActive,
                     MatCode = itemz.MatCode,
                     MatIndex = itemz.MatIndex,
                     MatName = itemz.MatName,
                     MatQuantity = itemz.MatQuantity,
                     MatSection = itemz.MatSection,
                     MatSectionPlace = itemz.MatSectionPlace,
                     MatSerialNumber = itemz.MatSerialNumber,
                     MatSerNoType = itemz.MatSerNoType,
                     MatSpecialStock = itemz.MatSpecialStock,
                     MatType = itemz.MatType,
                     ChangeDate = itemz.ChangeDate,
                     ChangedBy = itemz.ChangedBy,
                     Company = itemz.Company,
                     CreateDate = itemz.CreateDate,
                     CreatedBy = itemz.CreatedBy,
                     IsCounted = itemz.IsCounted
                 }).SingleOrDefault();
        }

        public bool CheckMaterial(string _mC, string _mI, string _mSp, string _mS, string _mDe, string _mSy)
        {
            return context.Material.Any(x => x.MatCode == _mC && x.MatIndex == _mI && x.MatSpecialStock == _mSp && x.MatSerialNumber == _mS && x.MatSection == _mDe && x.MatSectionPlace == _mSy);
        }

        public ICollection<MaterialsModel> GetSearchMaterialFiltered(string stockPlace, string matCode, List<StoreIdStockPlace> storeIDList)
        {
            List<MaterialsModel> _g = new List<MaterialsModel>();

            foreach (var item in storeIDList)
            {
                var _depo = context.Store.Where(x => x.IsActive && x.ID == item.ID).Select(c => c.WareHouse).SingleOrDefault();
                var _stokyeri = context.Store.Where(x => x.IsActive && x.ID == item.ID).Select(c => c.StockPlace).SingleOrDefault();

                // atandğı stok yeri başlangıç ve bitiş store id leri
                int _startValue = context.Store.Where(x => x.WareHouse == _depo && x.StockPlace == _stokyeri).Select(c => c.ID).SingleOrDefault();
                int _finalValue = context.Store.Where(x => x.WareHouse == _depo && x.StockPlace == item.StockPlace).Select(c => c.ID).SingleOrDefault();


                for (int i = _startValue; i <= _finalValue; i++)
                {
                    string _depo2 = context.Store.Where(x => x.IsActive && x.ID == i).Select(c => c.WareHouse).SingleOrDefault();
                    var _stokyeri2 = context.Store.Where(x => x.IsActive && x.ID == i).Select(c => c.StockPlace).SingleOrDefault();

                    var _x = context.Material.Where(x => x.MatSection == _depo2 && x.MatSectionPlace == _stokyeri2
                                                && (x.MatSectionPlace.Contains(stockPlace))
                                                && (x.MatCode.Contains(matCode) || x.MatName.Contains(matCode) || x.MatSerialNumber.Contains(matCode))
                                                && x.IsActive
                                            ).ToList();

                    foreach (var itemz in _x)
                    {
                        MaterialsModel _h = new MaterialsModel
                        {
                            ID = itemz.ID,
                            IpAddress = itemz.IpAddress,
                            IsActive = itemz.IsActive,
                            MatCode = itemz.MatCode,
                            MatIndex = itemz.MatIndex,
                            MatName = itemz.MatName,
                            MatQuantity = itemz.MatQuantity,
                            MatSection = itemz.MatSection,
                            MatSectionPlace = itemz.MatSectionPlace,
                            MatSerialNumber = itemz.MatSerialNumber,
                            MatSerNoType = itemz.MatSerNoType,
                            MatSpecialStock = itemz.MatSpecialStock,
                            MatType = itemz.MatType,
                            ChangeDate = itemz.ChangeDate,
                            ChangedBy = itemz.ChangedBy,
                            Company = itemz.Company,
                            CreateDate = itemz.CreateDate,
                            CreatedBy = itemz.CreatedBy,
                            IsCounted = itemz.IsCounted
                        };
                        _g.Add(_h);
                    };


                };//for
            }//foreach

            _g = _g.OrderBy(c => c.MatSection).ThenBy(v => v.MatSectionPlace).ThenBy(x => x.MatCode).ToList();

            return _g;
        }

        public ICollection<MaterialsModel> GetFilteredMaterial(string stockPlace, List<StoreIdStockPlace> storeIDList, string _teamNo)
        {
            string[] _c = stockPlace.Split('ç');

            string _dep = _c[0];       // 100
            string _stokYer = _c[1];  // 102-A03

            List<MaterialsModel> _g = new List<MaterialsModel>();

            foreach (var item in storeIDList)
            {
                var _depo = context.Store.Where(x => x.IsActive && x.ID == item.ID).Select(c => c.WareHouse).SingleOrDefault();
                var _stokyeri = context.Store.Where(x => x.IsActive && x.ID == item.ID).Select(c => c.StockPlace).SingleOrDefault();

                // atandğı stok yeri başlangıç ve bitiş store id leri
                int _startValue = context.Store.Where(x => x.WareHouse == _depo && x.StockPlace == _stokyeri).Select(c => c.ID).SingleOrDefault();
                int _finalValue = context.Store.Where(x => x.WareHouse == _depo && x.StockPlace == item.StockPlace).Select(c => c.ID).SingleOrDefault();

                int _searchingValue = context.Store.Where(x => x.WareHouse == _dep && x.StockPlace == _stokYer).Select(c => c.ID).SingleOrDefault();

                //if (_dep == _depo && _stokYer == _stokyeri)
                //{
                for (int i = _startValue; i <= _finalValue; i++)
                {
                    if (_searchingValue == i)
                    {
                        string _depo2 = context.Store.Where(x => x.IsActive && x.ID == i).Select(c => c.WareHouse).SingleOrDefault();
                        var _stokyeri2 = context.Store.Where(x => x.IsActive && x.ID == i).Select(c => c.StockPlace).SingleOrDefault();

                        var _x = context.Material.Where(x => x.MatSection == _depo2 && x.MatSectionPlace == _stokyeri2
                                                    && (x.MatSectionPlace.Contains(_stokYer)
                                                    && (x.MatSection.Contains(_dep))
                                                    )
                                                    && x.IsActive
                                                ).ToList();
                        if (_teamNo == "3")
                        {
                            var _x2 = context.Material.Where(x => x.MatSection == _depo2 && x.MatSectionPlace == _stokyeri2
                                                   && (x.MatSectionPlace.Contains(_stokYer)
                                                   && (x.MatSection.Contains(_dep))
                                                   )
                                                   && x.IsActive && x.IsCounted == "1"
                                                   ).ToList();

                            foreach (var itemz in _x2)
                            {
                                MaterialsModel _h = new MaterialsModel
                                {
                                    ID = itemz.ID,
                                    IpAddress = itemz.IpAddress,
                                    IsActive = itemz.IsActive,
                                    MatCode = itemz.MatCode,
                                    MatIndex = itemz.MatIndex,
                                    MatName = itemz.MatName,
                                    MatQuantity = itemz.MatQuantity,
                                    MatSection = itemz.MatSection,
                                    MatSectionPlace = itemz.MatSectionPlace,
                                    MatSerialNumber = itemz.MatSerialNumber,
                                    MatSerNoType = itemz.MatSerNoType,
                                    MatSpecialStock = itemz.MatSpecialStock,
                                    MatType = itemz.MatType
                                };
                                _g.Add(_h);
                            };

                        }
                        else
                        {
                            foreach (var itemz in _x)
                            {
                                MaterialsModel _h = new MaterialsModel
                                {
                                    ID = itemz.ID,
                                    IpAddress = itemz.IpAddress,
                                    IsActive = itemz.IsActive,
                                    MatCode = itemz.MatCode,
                                    MatIndex = itemz.MatIndex,
                                    MatName = itemz.MatName,
                                    MatQuantity = itemz.MatQuantity,
                                    MatSection = itemz.MatSection,
                                    MatSectionPlace = itemz.MatSectionPlace,
                                    MatSerialNumber = itemz.MatSerialNumber,
                                    MatSerNoType = itemz.MatSerNoType,
                                    MatSpecialStock = itemz.MatSpecialStock,
                                    MatType = itemz.MatType
                                };
                                _g.Add(_h);
                            };
                        }

                    };
                };//for
                //};
            }//foreach

            _g = _g.OrderBy(c => c.MatSection).ThenBy(v => v.MatSectionPlace).ThenBy(x => x.MatCode).ToList();

            return _g;
        }

        public ICollection<MaterialsModel> GetSearchMaterialS(string matCode, List<StoreIdStockPlace> storeIDList, string _depoStok, string _name)
        {
            string _pername = context.User.Where(c => c.Mail == _name && c.IsActive).Select(v => v.Name).SingleOrDefault();
            var _tS = context.InventoryGroup.Where(x => x.IsActive && x.PerName == _pername).Select(c => new { c.TeamNo, c.GroupNo }).ToList();

            string _team = "";
            string _grp = "";

            foreach (var item in _tS)
            {
                _team = item.TeamNo.ToString();
                _grp = item.GroupNo.ToString();
            }

            string[] _depSo = null;
            string _ware = "";
            string _stockPlace = "";

            if (_depoStok != "")
            {
                _depSo = _depoStok.Split('ç');
                _ware = _depSo[0];
                _stockPlace = _depSo[1];
            }

            List<MaterialsModel> _g = new List<MaterialsModel>();

            if (_ware == "")
            {
                foreach (var item in storeIDList)
                {
                    var _depo = context.Store.Where(x => x.IsActive && x.ID == item.ID).Select(c => c.WareHouse).SingleOrDefault();
                    var _stokyeri = context.Store.Where(x => x.IsActive && x.ID == item.ID).Select(c => c.StockPlace).SingleOrDefault();

                    // atandğı stok yeri başlangıç ve bitiş store id leri
                    int _startValue = context.Store.Where(x => x.WareHouse == _depo && x.StockPlace == _stokyeri).Select(c => c.ID).SingleOrDefault();
                    int _finalValue = context.Store.Where(x => x.WareHouse == _depo && x.StockPlace == item.StockPlace).Select(c => c.ID).SingleOrDefault();


                    for (int i = _startValue; i <= _finalValue; i++)
                    {
                        string _depo2 = context.Store.Where(x => x.IsActive && x.ID == i).Select(c => c.WareHouse).SingleOrDefault();
                        var _stokyeri2 = context.Store.Where(x => x.IsActive && x.ID == i).Select(c => c.StockPlace).SingleOrDefault();

                        List<MaterialsModel> _x = null;

                        if (_team == "3")
                        {
                            _x = context.Material.Where(x => x.MatSection == _depo2 && x.MatSectionPlace == _stokyeri2
                                              && (x.MatCode.Contains(matCode) || x.MatName.Contains(matCode) || x.MatSerialNumber.Contains(matCode))
                                              && x.IsActive && x.IsCounted == "1"
                                          )
                                          .Select(itemz => new MaterialsModel
                                          {
                                              ID = itemz.ID,
                                              IpAddress = itemz.IpAddress,
                                              IsActive = itemz.IsActive,
                                              MatCode = itemz.MatCode,
                                              MatIndex = itemz.MatIndex,
                                              MatName = itemz.MatName,
                                              MatQuantity = itemz.MatQuantity,
                                              MatSection = itemz.MatSection,
                                              MatSectionPlace = itemz.MatSectionPlace,
                                              MatSerialNumber = itemz.MatSerialNumber,
                                              MatSerNoType = itemz.MatSerNoType,
                                              MatSpecialStock = itemz.MatSpecialStock,
                                              MatType = itemz.MatType,
                                              IsCounted = itemz.IsCounted,
                                              ChangeDate = itemz.ChangeDate,
                                              ChangedBy = itemz.ChangedBy,
                                              Company = itemz.Company,
                                              CreatedBy = itemz.CreatedBy,
                                              CreateDate = itemz.CreateDate

                                          })
                                          .ToList();
                        }
                        else
                        {
                            _x = context.Material.Where(x => x.MatSection == _depo2 && x.MatSectionPlace == _stokyeri2
                                              && (x.MatCode.Contains(matCode) || x.MatName.Contains(matCode) || x.MatSerialNumber.Contains(matCode))
                                              && x.IsActive
                                          )
                                          .Select(itemz => new MaterialsModel
                                          {
                                              ID = itemz.ID,
                                              IpAddress = itemz.IpAddress,
                                              IsActive = itemz.IsActive,
                                              MatCode = itemz.MatCode,
                                              MatIndex = itemz.MatIndex,
                                              MatName = itemz.MatName,
                                              MatQuantity = itemz.MatQuantity,
                                              MatSection = itemz.MatSection,
                                              MatSectionPlace = itemz.MatSectionPlace,
                                              MatSerialNumber = itemz.MatSerialNumber,
                                              MatSerNoType = itemz.MatSerNoType,
                                              MatSpecialStock = itemz.MatSpecialStock,
                                              MatType = itemz.MatType,
                                              IsCounted = itemz.IsCounted,
                                              ChangeDate = itemz.ChangeDate,
                                              ChangedBy = itemz.ChangedBy,
                                              Company = itemz.Company,
                                              CreatedBy = itemz.CreatedBy,
                                              CreateDate = itemz.CreateDate
                                          }).ToList();
                        }


                        foreach (var itemz in _x)
                        {
                            MaterialsModel _h = new MaterialsModel
                            {
                                ID = itemz.ID,
                                IpAddress = itemz.IpAddress,
                                IsActive = itemz.IsActive,
                                MatCode = itemz.MatCode,
                                MatIndex = itemz.MatIndex,
                                MatName = itemz.MatName,
                                MatQuantity = itemz.MatQuantity,
                                MatSection = itemz.MatSection,
                                MatSectionPlace = itemz.MatSectionPlace,
                                MatSerialNumber = itemz.MatSerialNumber,
                                MatSerNoType = itemz.MatSerNoType,
                                MatSpecialStock = itemz.MatSpecialStock,
                                MatType = itemz.MatType,
                                IsCounted = itemz.IsCounted,
                                ChangeDate = itemz.ChangeDate,
                                ChangedBy = itemz.ChangedBy,
                                Company = itemz.Company,
                                CreatedBy = itemz.CreatedBy,
                                CreateDate = itemz.CreateDate
                            };
                            _g.Add(_h);
                        };


                    };//for
                }//foreach
            }
            else
            {
                var _y = context.Material.Where(x => x.MatSection == _ware && x.MatSectionPlace == _stockPlace
                                && (x.MatCode.Contains(matCode) || x.MatName.Contains(matCode) || x.MatSerialNumber.Contains(matCode))
                                && x.IsActive
                            ).ToList();

                foreach (var itemz in _y)
                {
                    MaterialsModel _h = new MaterialsModel
                    {
                        ID = itemz.ID,
                        IpAddress = itemz.IpAddress,
                        IsActive = itemz.IsActive,
                        MatCode = itemz.MatCode,
                        MatIndex = itemz.MatIndex,
                        MatName = itemz.MatName,
                        MatQuantity = itemz.MatQuantity,
                        MatSection = itemz.MatSection,
                        MatSectionPlace = itemz.MatSectionPlace,
                        MatSerialNumber = itemz.MatSerialNumber,
                        MatSerNoType = itemz.MatSerNoType,
                        MatSpecialStock = itemz.MatSpecialStock,
                        MatType = itemz.MatType
                    };
                    _g.Add(_h);
                };

            }
            _g = _g.OrderBy(c => c.MatSection).ThenBy(v => v.MatSectionPlace).ThenBy(x => x.MatCode).ToList();

            return _g;
        }

        public ICollection<MaterialsModel> GetMaterialTwo(List<StoreIdStockPlace> storeIDList, string depostok)
        {
            string[] _ab = null;
            string _dep = "";
            string _stokyer = "";
            string _keyWord = "";

            if (depostok != "")
            {
                _ab = depostok.Split('X');

                if (_ab.Length == 3)
                {
                    _dep = _ab[0];
                    _stokyer = _ab[1];
                    _keyWord = _ab[2];
                }
                else
                {
                    _dep = _ab[0];
                    _stokyer = _ab[1];
                }


            }

            List<MaterialsModel> _g = new List<MaterialsModel>();

            foreach (var item in storeIDList)
            {
                var _depo = context.Store.Where(x => x.IsActive && x.ID == item.ID).Select(c => c.WareHouse).SingleOrDefault();
                var _stokyeri = context.Store.Where(x => x.IsActive && x.ID == item.ID).Select(c => c.StockPlace).SingleOrDefault();

                // atandğı stok yeri başlangıç ve bitiş store id leri
                int _startValue = context.Store.Where(x => x.WareHouse == _depo && x.StockPlace == _stokyeri).Select(c => c.ID).SingleOrDefault();
                int _finalValue = context.Store.Where(x => x.WareHouse == _depo && x.StockPlace == item.StockPlace).Select(c => c.ID).SingleOrDefault();


                for (int i = _startValue; i <= _finalValue; i++)
                {
                    string _depo2 = context.Store.Where(x => x.IsActive && x.ID == i).Select(c => c.WareHouse).SingleOrDefault();
                    var _stokyeri2 = context.Store.Where(x => x.IsActive && x.ID == i).Select(c => c.StockPlace).SingleOrDefault();

                    List<MaterialsModel> _x = new List<MaterialsModel>();

                    if (_keyWord != "")
                    {
                        _x = context.Material.Where(x => x.MatSection == _depo2 && x.MatSectionPlace == _stokyeri2
                                                && x.MatName.Contains(_keyWord)
                                                && x.IsActive && x.MatSection.Contains(_dep) && x.MatSectionPlace.Contains(_stokyer)
                                            ).Select(itemz => new MaterialsModel
                                            {
                                                ID = itemz.ID,
                                                IpAddress = itemz.IpAddress,
                                                IsActive = itemz.IsActive,
                                                MatCode = itemz.MatCode,
                                                MatIndex = itemz.MatIndex,
                                                MatName = itemz.MatName,
                                                MatQuantity = itemz.MatQuantity,
                                                MatSection = itemz.MatSection,
                                                MatSectionPlace = itemz.MatSectionPlace,
                                                MatSerialNumber = itemz.MatSerialNumber,
                                                MatSerNoType = itemz.MatSerNoType,
                                                MatSpecialStock = itemz.MatSpecialStock,
                                                MatType = itemz.MatType,
                                                IsCounted = itemz.IsCounted,
                                                ChangeDate = itemz.ChangeDate,
                                                ChangedBy = itemz.ChangedBy,
                                                Company = itemz.Company,
                                                CreatedBy = itemz.CreatedBy,
                                                CreateDate = itemz.CreateDate
                                            }).ToList();
                    }
                    else
                    {
                        _x = context.Material.Where(x => x.MatSection == _depo2 && x.MatSectionPlace == _stokyeri2
                            //&& (x.MatCode.Contains(matCode) || x.MatName.Contains(matCode) || x.MatSerialNumber.Contains(matCode))
                                               && x.IsActive && x.MatSection.Contains(_dep) && x.MatSectionPlace.Contains(_stokyer)
                                           ).Select(itemz => new MaterialsModel
                                           {
                                               ID = itemz.ID,
                                               IpAddress = itemz.IpAddress,
                                               IsActive = itemz.IsActive,
                                               MatCode = itemz.MatCode,
                                               MatIndex = itemz.MatIndex,
                                               MatName = itemz.MatName,
                                               MatQuantity = itemz.MatQuantity,
                                               MatSection = itemz.MatSection,
                                               MatSectionPlace = itemz.MatSectionPlace,
                                               MatSerialNumber = itemz.MatSerialNumber,
                                               MatSerNoType = itemz.MatSerNoType,
                                               MatSpecialStock = itemz.MatSpecialStock,
                                               MatType = itemz.MatType,
                                               IsCounted = itemz.IsCounted,
                                               ChangeDate = itemz.ChangeDate,
                                               ChangedBy = itemz.ChangedBy,
                                               Company = itemz.Company,
                                               CreatedBy = itemz.CreatedBy,
                                               CreateDate = itemz.CreateDate
                                           }).ToList();
                    }

                    foreach (var itemz in _x)
                    {
                        MaterialsModel _h = new MaterialsModel
                        {
                            ID = itemz.ID,
                            IpAddress = itemz.IpAddress,
                            IsActive = itemz.IsActive,
                            MatCode = itemz.MatCode,
                            MatIndex = itemz.MatIndex,
                            MatName = itemz.MatName,
                            MatQuantity = itemz.MatQuantity,
                            MatSection = itemz.MatSection,
                            MatSectionPlace = itemz.MatSectionPlace,
                            MatSerialNumber = itemz.MatSerialNumber,
                            MatSerNoType = itemz.MatSerNoType,
                            MatSpecialStock = itemz.MatSpecialStock,
                            MatType = itemz.MatType,
                            ChangeDate = itemz.ChangeDate,
                            ChangedBy = itemz.ChangedBy,
                            Company = itemz.Company,
                            CreateDate = itemz.CreateDate,
                            CreatedBy = itemz.CreatedBy,
                            IsCounted = itemz.IsCounted
                        };
                        _g.Add(_h);
                    };


                };//for
            }//foreach

            _g = _g.OrderBy(c => c.MatSection).ThenBy(v => v.MatSectionPlace).ThenBy(x => x.MatCode).ToList();
            return _g;
        }

        public bool IsHasMaterial(string matCode, string matIndex)
        {
            bool _IsHas = context.Material.Any(x => x.MatCode == matCode && x.MatIndex == matIndex);

            if (_IsHas)
            {
                return true;
            }
            return false;
        }

        public string GetNameEnterMaterial(string mcode, string stockstore, string stockplace)
        {
            string _name = "";
            var _nameList = context.Material.Where(x => x.MatCode == mcode && x.MatSection == stockstore && x.MatSectionPlace == stockplace).Select(y => new { y.MatName }).ToList();

            foreach (var item in _nameList)
            {
                if (item.MatName != null && item.MatName != "")
                {
                    _name = item.MatName;
                }

            }
            return _name;
        }

        public string GetName(string mcode)
        {
            string _name = context.Material.Where(x => x.MatCode == mcode && x.IsActive).Select(y => y.MatName).FirstOrDefault();
            return _name;
        }

        public string CheckMatSerialNumber(string index, string mCode, string ware, string stockPlace, string matser)
        {
            string _sr = "";

            if (matser != null)
            {
                _sr = matser;
            }
            //
            var _isIn = context.Material.Where(x => x.MatIndex == index && x.MatCode == mCode && x.MatSection == ware && x.MatSectionPlace == stockPlace && x.MatSerialNumber == _sr).Select(c => c.MatSerialNumber).SingleOrDefault();

            if (_isIn != null)
            {
                _isIn = _isIn.ToString();
            }
            else
            {
                _isIn = "";
            }
            return _isIn;
        }

        public bool CheckMatSerialNumberAll(string index, string mCode, string ware, string stockPlace, string matser)
        {
            string _sr = "";
            //
            if (matser != null)
            {
                _sr = matser;
            }

            if (index == "X9X9")
            {
                var _isInXY = context.Material.Where(x => x.MatCode == mCode && x.MatSection == ware && x.MatSectionPlace == stockPlace).Select(c => new { c.MatSerialNumber }).ToList();

                int _c2 = 0;

                foreach (var item in _isInXY)
                {
                    if (item.MatSerialNumber.ToString() == _sr)
                    {
                        _c2 = _c2 + 1;
                    }
                }

                if (_c2 > 0)
                {
                    return true;
                }
                return false;
            }

            var _isInX = context.Material.Where(x => x.MatIndex == index && x.MatCode == mCode && x.MatSection == ware && x.MatSectionPlace == stockPlace).Select(c => new { c.MatSerialNumber }).ToList();

            int _c = 0;

            foreach (var item in _isInX)
            {
                if (item.MatSerialNumber.ToString() == _sr)
                {
                    _c = _c + 1;
                }
            }

            if (_c > 0)
            {
                return true;
            }
            return false;
        }
        // o depo ve stok yerindeki toplam kalem adedi
        public int GetTotalAmount(string wh, string sp)
        {
            return context.Material.Where(x => x.MatSection == wh && x.MatSectionPlace == sp && x.IsActive).Count();
        }

        public string IsMission(string matcode, string matIndex, string matSpec, string matSer, string matWare, string matStock)
        {
            return context.Material.Where(x => x.IsActive && x.MatCode == matcode && x.MatIndex == matIndex && x.MatSpecialStock == matSpec && x.MatSerialNumber == matSer && x.MatSection == matWare && x.MatSectionPlace == matStock).Select(c => c.IsCounted).SingleOrDefault();
        }

        public double WhatIsCount(string matcode, string matIndex, string matSpec, string matSer, string matWare, string matStock)
        {
            return context.Material.Where(x => x.IsActive && x.MatCode == matcode && x.MatIndex == matIndex && x.MatSpecialStock == matSpec && x.MatSerialNumber == matSer && x.MatSection == matWare && x.MatSectionPlace == matStock).Select(c => c.MatQuantity).SingleOrDefault();
        }

    }
}
