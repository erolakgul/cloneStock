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
    public class InventoryGroupService : ServiceBase<InventoryGroups>
    {
        public override MessageService Insert(InventoryGroups dto)
        {
            if (context.InventoryGroup.Any(x => x.GroupNo == dto.GroupNo && x.TeamNo == dto.TeamNo && x.PerName == dto.PerName && x.IsActive == dto.IsActive))
            {
                result.ResultID = 0;
                if (result.IsSuccess == false)
                {
                    result.Message = "False";
                    return result;
                }
            }

            bool isExist = (dto == null) ? false : true;

            if (isExist)
            {
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

        public string BeforeInsert(IGModel dto)
        {
            InventoryGroups _c = new InventoryGroups
            {
                ChangeDate = dto.ChangeDate,
                Company = dto.Company,
                ChangedBy = dto.ChangedBy,
                CreateDate = dto.CreateDate,
                CreatedBy = dto.CreatedBy,
                EndStockPlace = dto.EndStockPlace,
                GroupNo = dto.GroupNo,
                IpAddress = dto.IpAddress,
                IsActive = dto.IsActive,
                MissionStatu = dto.MissionStatu,
                PerName = dto.PerName,
                StoresID = dto.StoreID,
                UserID = dto.UserID,
                TeamNo = dto.TeamNo
            };
            MessageService _x = Insert(_c);
            return _x.Message;
        }

        public override MessageService Update(InventoryGroups dto)
        {
            if (dbset.Any(x => x.ID == dto.ID))
            {
                InventoryGroups user = FindByID(dto.ID);
                user.MissionStatu = dto.MissionStatu;
                user.IpAddress = dto.IpAddress;
                user.IsActive = dto.IsActive;
                user.PerName = dto.PerName;

                result.ResultID = user.ID;
                if (result.IsSuccess == true)
                {
                    try
                    {
                        result.Message = "True";
                        context.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        result.Message = ex.Message;
                    }
                }
            }
            return result;
        }

        // çoklu update edebilmeli olarak bu update yazılacak
        public int UpdateGroupStore(IGModel dto)
        {
            // dto dan kontrol et 

            // aynı takım içindeki bir başka grup o depoya atanamaz,
            //  örneğinn 1.takımdan 2 grup 11 numaralı depoya atandıysa 3.grup 11 numaralı depoya atanamaz
            var _list = dbset.Where(x => x.TeamNo == dto.TeamNo && x.StoresID == dto.StoreID && x.IsActive).ToList();

            if (_list.Count > 0)
            {
                if (_list.Count == 1)
                {
                    goto devam;
                }
                else
                {
                    return 0;
                }
            }

        devam:

            var _list2 = dbset.Where(x => x.GroupNo == dto.GroupNo && x.TeamNo == dto.TeamNo && x.IsActive).ToList();
            var _counter = 0;

            foreach (var item in _list2)
            {
                //InventoryGroups ent = FindByID(item.ID);
                //ent.StoresID = dto.StoresID;

                if (item.StoresID == 1)
                {
                    // eğer grup atanmamış ise henüz satır buraya gelir ve ataması yapılır
                    InventoryGroups ent = FindByID(item.ID);
                    ent.StoresID = dto.StoreID;
                    ent.EndStockPlace = dto.EndStockPlace;

                    try
                    {
                        context.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        return 0;
                    }
                }
                else
                {
                    _counter = _counter + 1;
                    if (_counter < 3)
                    {
                        int _userID = context.User.Where(x => x.IsActive && x.Name == item.PerName).Select(c => c.ID).SingleOrDefault();

                        //eğer atanmış ise o kullanıcı yeni insertler eklemek için bu sekme kullanılır.
                        InventoryGroups _c = new InventoryGroups
                        {
                            IpAddress = item.IpAddress,
                            IsActive = item.IsActive,
                            MissionStatu = item.MissionStatu,
                            PerName = item.PerName,
                            StoresID = dto.StoreID,
                            GroupNo = dto.GroupNo,
                            TeamNo = dto.TeamNo,
                            CreatedBy = dto.CreatedBy,
                            CreateDate = DateTime.Now,
                            EndStockPlace = dto.EndStockPlace,
                            ChangeDate = DateTime.Now,
                            ChangedBy = dto.CreatedBy,
                            Company = "01",
                            UserID = _userID
                        };
                        dbset.Add(_c);
                    }
                }
            }

            try
            {
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                return 0;
            }
            return 1;
        }

        public string IGDelete(int id)
        {
            InventoryGroups _v = FindByID(id);
            _v.IsActive = false;

            MessageService msg = Update(_v);

            return msg.Message;
        }

        public int UpdateIsActive(IGModel dto)
        {
            if (dbset.Any(x => x.IsActive))
            {
                InventoryGroups inv = FindByID(dto.ID);
                inv.IsActive = dto.IsActive;

                try
                {
                    context.SaveChanges();
                    return dto.ID;
                }
                catch (Exception)
                {
                    return 0;
                }
                
            }
            return 0;
        }

        public string GetTeamNoFromName(string name)
        {
            int _teamNo = context.InventoryGroup.Where(x => x.PerName == name && x.IsActive).Select(y => y.TeamNo).Distinct().SingleOrDefault();

            string _teamGroupNo = _teamNo.ToString();

            return _teamGroupNo;
        }

        public List<int> GetStoreID(string name)
        {
            var _storeId = context.InventoryGroup.Where(x => x.PerName == name && x.IsActive).Select(y => y.StoresID).ToList();
            return _storeId;
        }

        public ICollection<IGModel> GetGroupForName(string name)
        {
            int _grupNo = context.InventoryGroup.Where(x => x.PerName == name && x.IsActive).Select(y => y.GroupNo).Distinct().SingleOrDefault();

            int _teamNo = context.InventoryGroup.Where(x => x.PerName == name && x.IsActive).Select(y => y.TeamNo).Distinct().SingleOrDefault();
            var _list = context.InventoryGroup.Where(a => a.GroupNo == _grupNo && a.TeamNo == _teamNo && a.IsActive).Select(c => new IGModel
            {
                ID = c.ID,
                TeamNo = c.TeamNo,
                UserID = c.UserID,
                StoreID = c.StoresID,
                PerName = c.PerName,
                MissionStatu = c.MissionStatu,
                IsActive = c.IsActive,
                GroupNo = c.GroupNo,
                EndStockPlace = c.EndStockPlace

            }).ToList();

            return _list;
        }

        // admin SelectStockPlaceForWriter
        public List<StoreIdStockPlace> GetMissionStockPlace(string name)
        {
            var _storeId = context.InventoryGroup.Where(x => x.PerName == name && x.IsActive).Select(y => new { y.StoresID, y.EndStockPlace }).ToList();
            List<StoreIdStockPlace> _g = new List<StoreIdStockPlace>();

            for (int i = 0; i < _storeId.Count; i++)
            {
                int _f = Convert.ToInt32(_storeId[i].StoresID);

                StoreIdStockPlace _c = new StoreIdStockPlace
                {
                    ID = _f,
                    StockPlace = _storeId[i].EndStockPlace
                };
                _g.Add(_c);
            }

            return _g;
        }

        public List<StoreIdStockPlace> GetStoreIDS(string name)
        {
            List<StoreIdStockPlace> _o = new List<StoreIdStockPlace>();
            string _pername = context.User.Where(c => c.Mail == name && c.IsActive).Select(v => v.Name).SingleOrDefault();

            var _storeId = context.InventoryGroup.Where(x => x.PerName == _pername && x.IsActive).Select(y => new { y.StoresID, y.EndStockPlace }).ToList();

            foreach (var item in _storeId)
            {
                StoreIdStockPlace _p = new StoreIdStockPlace
                {
                    ID = Convert.ToInt32(item.StoresID),
                    StockPlace = item.EndStockPlace
                };
                _o.Add(_p);
            }
            return _o;
        }

        public List<IGModel> TGList(int team,int grp)
        {
            return context.InventoryGroup.Where(x => x.TeamNo == team && x.GroupNo == grp && x.IsActive).Select(c => new IGModel
            {
                ID = c.ID,
                TeamNo = c.TeamNo,
                UserID = c.UserID,
                StoreID = c.StoresID,
                PerName = c.PerName,
                MissionStatu = c.MissionStatu,
                IsActive = c.IsActive,
                GroupNo = c.GroupNo,
                EndStockPlace = c.EndStockPlace
                 
            }).ToList();
        }

        public ICollection<IGModel> SelectGroup(int id)
        {
            var _list = context.InventoryGroup.Where(x => x.IsActive)
                        .Select(c => new IGModel
                           {
                               ID = c.ID,
                               TeamNo = c.TeamNo,
                               UserID = c.UserID,
                               StoreID = c.StoresID,
                               PerName = c.PerName,
                               MissionStatu = c.MissionStatu,
                               IsActive = c.IsActive,
                               GroupNo = c.GroupNo,
                               EndStockPlace = c.EndStockPlace
                           })
                        .ToList();

            return _list;
        }

        public List<WareStock> GetStoreListID(string name)
        {
            List<WareStock> _s = new List<WareStock>();

            var _storeId = context.InventoryGroup.Where(x => x.PerName == name && x.IsActive).Select(y => new { y.StoresID, y.EndStockPlace }).ToList();

            foreach (var item in _storeId)
            {
                WareStock _p = new WareStock
                {
                    _id = Convert.ToInt32(item.StoresID),
                    _endStockPlace = item.EndStockPlace
                };
                _s.Add(_p);
            }
            return _s;
        }

        public ICollection<string> SelectGroupPerName(int a, int b)
        {
            var _list = context.InventoryGroup.Where(x => x.TeamNo == a && x.GroupNo == b && x.IsActive).Select(z => z.PerName).ToList();
            return _list;
        }

        public bool IsWorking(string name, int teamNo)
        {
            // aynı isimde biri herhangi bir şekilde başka bir takımda aktif ise true gönderip,işlem yaptırmayacağız
            return context.InventoryGroup.Any(x => x.PerName == name && x.TeamNo == teamNo && x.IsActive);
        }
        public bool IsMissionSame(int groupNo, int teamNo, int missionStatu)
        {
            // group no lar eşitse ve mission statu eşitse (ki bu o statü de biri var demektir) ve de aktif bir data ise true gönderir
            return context.InventoryGroup.Any(x => x.GroupNo == groupNo && x.TeamNo == teamNo && x.MissionStatu == missionStatu && x.IsActive);
        }
        public bool IsGroupFull(int groupNo, int teamNo)
        {
            int _count = context.InventoryGroup.Where(x => x.GroupNo == groupNo && x.TeamNo == teamNo && x.IsActive).Count();

            if (_count == 2) // zaten 2 kişi varsa grupta buradan true değerini gönderip eklettirmeyeceğiz.
            {
                return true;
            }
            return false;
        }
        public bool IsNonActiveGroupFull(int GroupNo, int TeamNo)
        {
            int _count = context.InventoryGroup.Where(x => x.GroupNo == GroupNo && x.TeamNo == TeamNo && !x.IsActive).Count();

            if (_count == 2) // zaten 2 kişi varsa grupta ve grup pasif ise,groupNo yu son groupno id ye 1 ekleyip atayacağız
            {
                return true;
            }
            return false;
        }

        public ICollection<IGModel> ToListIG(string id,string onay)
        {
            if (onay == "0")
            {
                var _xList = context.InventoryGroup.Where(x => x.IsActive).Select(c => new IGModel
                {
                    PerName = c.PerName
                }).ToList();
                return _xList;
            }
            int _id = int.Parse(id);

            var _xListx = context.InventoryGroup.Where(x => x.IsActive && x.ID == _id).Select(c => new IGModel
            {
                PerName = c.PerName
            }).ToList();

            return _xListx;
            
        }

        public string GetNameFromID(int id)
        {
            string _name = context.InventoryGroup.Where(x => x.ID == id && x.IsActive).Select(y => y.PerName).SingleOrDefault();
            return _name;
        }

        public List<int> SelectGroupNo(int teamNo)
        {
            var _list = context.InventoryGroup.Where(x => x.IsActive && x.TeamNo == teamNo).Select(y => y.GroupNo).Distinct().ToList();
            return _list;
        }

        public int LastGroupNo(int teamNo)
        {
            int _lastNo = 0;
            if (context.InventoryGroup.Any(x => x.TeamNo == teamNo))
            {
                _lastNo = context.InventoryGroup.Where(x => x.TeamNo == teamNo && x.IsActive).Count();

                int _mod = _lastNo % 2;
                if (_mod == 1)
                {
                    //lastNo = context.InventoryGroup.Where(x => x.TeamNo == teamNo && !x.IsActive).OrderByDescending(z => z.GroupNo).Select(y => y.GroupNo).First();
                    _lastNo = context.InventoryGroup.Where(x => x.TeamNo == teamNo && x.IsActive).Select(z => z.GroupNo).Distinct().Count();
                    //lastNo = lastNo - 1;
                }
                else
                {
                    _lastNo = context.InventoryGroup.Where(x => x.TeamNo == teamNo && x.IsActive).Select(z => z.GroupNo).Distinct().Count();
                    _lastNo = _lastNo + 1;
                }
            }
            return _lastNo;
        }
    }
}
