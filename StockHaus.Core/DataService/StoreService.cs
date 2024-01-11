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
    public class StoreService : ServiceBase<Stores>
    {

        public override MessageService Insert(Stores dto)
        {
            if (context.Store.Any(x => x.WareHouse == dto.WareHouse && x.StockPlace == dto.StockPlace && x.IsActive))
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
                    dbset.Add(dto);
                    context.SaveChanges();
                    result.Message = "True";
                    return result;
                }
                catch (Exception ex)
                {
                    result.Message = ex.Message;
                }
            }
            return result;
        }

        public override MessageService Update(Stores dto)
        {
            throw new NotImplementedException();
        }

        public string BeforeInsert(DefineStores dto)
        {
            Stores d = new Stores
            {
                ChangeDate = DateTime.Now,
                ChangedBy = dto._createdBy,
                Company = dto._company,
                CreateDate = DateTime.Now,
                CreatedBy = dto._createdBy,
                Description = dto._description,
                IpAddress = dto._ipAddress,
                IsActive = true,
                StockPlace = dto._stockPlace,
                WareHouse = dto._wareHouse
            };

            MessageService _mgs = Insert(d);
            return _mgs.Message;
        }

        public ICollection<StoreModel> DetectStockPlace(string _store,string _place)
        {
            var _dd = context.Store.Where(x => x.IsActive && x.WareHouse == _store && x.StockPlace == _place)
                  .Select(d => new StoreModel
                  {
                      ID = d.ID,
                      IsActive = d.IsActive,
                      StockPlace = d.StockPlace,
                      WareHouse = d.WareHouse,
                      Description = d.Description,
                      ChangeDate = d.ChangeDate,
                      ChangedBy = d.ChangedBy,
                      Company = d.Company,
                      CreateDate = d.CreateDate,
                      CreatedBy = d.CreatedBy,
                      IpAddress = d.IpAddress
                  }).ToList();

            return _dd;
        }

        public List<string> GetStoreNoForID(List<int> storeid)
        {
            List<string> _ss = new List<string>();
            string _aa = "";

            for (int i = 0; i < storeid.Count; i++)
            {
                int ab = storeid[i];

                _aa = context.Store.Where(x => x.ID == ab && x.IsActive).Select(c => c.WareHouse).SingleOrDefault();

                _ss.Add(_aa);
            }
            _ss.Sort(); // liste içindeki verileri listeliyoruz.

            return _ss;
        }


        // admin SelectStockPlaceForWriter
        public ICollection<string> SelectStockPlaceForWriters(string id, List<StoreIdStockPlace> model, string userNm)
        {
            //List<string> _gh = new List<string>();
            List<string> _p = new List<string>();
            List<string> _c = new List<string>();

            foreach (var item in model)
            {
                string _depo = context.Store.Where(x => x.ID == item.ID && x.IsActive).Select(c => c.WareHouse).SingleOrDefault();

                int _startStokYeriId = item.ID;
                int _stopStokYeriId = context.Store.Where(x => x.WareHouse == _depo && x.StockPlace == item.StockPlace && x.IsActive).Select(c => c.ID).SingleOrDefault();

                // örneğin 100 geldi, eğer writer da depo olarakta 100 seçili ise username e göre o stok yerlerini listelet
                if (id == _depo)
                {
                    _c = context.Store.Where(x => x.ID >= _startStokYeriId && x.ID <= _stopStokYeriId && x.IsActive).Select(c => c.StockPlace).ToList();
                    _p.AddRange(_c);
                }

            }
            // var _list = context.Store.Where(x => x.IsActive && x.WareHouse == id).Select(y => y.StockPlace).Distinct().ToList();

            return _p;
        }

        public ICollection<StoreModel> AllStoreID(int id)
        {
            var _list = context.Store.Where(x => x.IsActive && x.ID == id).
                                   Select(c => new StoreModel
                                   {
                                       ID = c.ID,
                                       StockPlace = c.StockPlace,
                                       WareHouse = c.WareHouse
                                   }).ToList();
            return _list;
        }

        // depo ve stok yerine göre ıd gönderir
        public int GetStoreID(string depo, string stokyeri)
        {
            int _storeID = context.Store.Where(x => x.WareHouse == depo && x.StockPlace == stokyeri).Select(z => z.ID).SingleOrDefault();
            return _storeID;
        }

        public WareStock SelectWareStock(int storeId)
        {
            var _list = context.Store.Where(x => x.ID == storeId && x.IsActive).Select(c => new WareStock
            {
                _ware = c.WareHouse,
                _stock = c.StockPlace
            }).SingleOrDefault();

            return _list;
        }

        public ICollection<StoreModel> GetStorePlaceNoForID(int storeid)
        {
            var _list = context.Store.Where(x => x.ID == storeid && x.IsActive).
                 Select(c => new StoreModel
                                   {
                                       ID = c.ID,
                                       StockPlace = c.StockPlace,
                                       WareHouse = c.WareHouse
                                   }).ToList();
            return _list;
        }

        //sonraki seçimlerde stok yerini doldurur
        public ICollection<string> SelectStockPlace(string id)
        {
            var _list = context.Store.Where(x => x.IsActive && x.WareHouse == id).Select(y => y.StockPlace).Distinct().ToList();
            return _list;
        }
        public List<string> SelectStockStoreNo()
        {
            var _list = context.Store.Where(x => x.IsActive).Select(y => y.WareHouse).Distinct().ToList();
            return _list;
        }
        // ilk açılışta stok yerini doldurur
        public List<string> SelectStockPlaceNo()
        {
            var _list = context.Store.Where(x => x.WareHouse == "100" && x.IsActive).Select(y => y.StockPlace).Distinct().ToList();
            return _list;
        }
    }
}
