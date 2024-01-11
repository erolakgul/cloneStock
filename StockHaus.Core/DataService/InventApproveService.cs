using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StockHaus.Core.BaseService;
using StockHaus.Data.Model;
using StockHaus.ModelClass.UserPage;

namespace StockHaus.Core.DataService
{
    public class InventApproveService : ServiceBase<InventApproves>
    {

        public override MessageService Insert(InventApproves dto)
        {
            if (context.InventApprove.Any(x => x.InventoryCode == dto.InventoryCode))
            {
                result.ResultID = 0;
                if (result.IsSuccess == false)
                {
                    result.Message = "Kayıt başarısız ..";
                    return result;
                }
            }

            bool _isExist = (dto == null) ? false : true;

            if (_isExist)
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

        public override MessageService Update(InventApproves dto)
        {
            if (dbset.Any(x => x.ID == dto.ID))
            {
                InventApproves invt = FindByID(dto.ID);
                //invt.IsApproved = dto.IsApproved;
                //invt.ApprovingDate = dto.ApprovingDate;
                //invt.ApprovingPers = dto.ApprovingPers;

                result.ResultID = invt.ID;
                if (result.IsSuccess == true)
                {
                    result.Message = "Güncelleme başarılı..";
                    context.SaveChanges();
                    return result;
                }
            }
            result.Message = "Güncelleme başarısız..";
            return result;
        }

        public string BeforeInsert(InventApprovesModel d)
        {
            InventApproves dto = new InventApproves
            {
                ChangeDate = d.ChangeDate,
                ChangedBy = d.ChangedBy,
                Company = d.Company,
                CreateDate = d.CreateDate,
                CreatedBy = d.CreatedBy,
                InventoryCode = d.InventoryCode,
                IpAddress = d.IpAddress,
                IsActive = d.IsActive,
                IsCanceled = d.IsCanceled,
                TotalQuantity = d.TotalQuantity
            };
            MessageService msg = Insert(dto);

            return msg.Message;
        }

        public bool IsApproved(string inventoryCode)
        {
            int _invID = context.InventApprove.Where(x => x.InventoryCode == inventoryCode && x.IsActive).Select(y => y.ID).SingleOrDefault();

            if (_invID > 0)
            {
                return true;
            }
            return false;
        }

        public bool IsFinishedInvent(string inventCode)
        {
            string[] _data = inventCode.Split('X');

            // ilk data depo no,ikinci veri stok no,3.veri takım no verisi,son data[3] ise group no verisidir İPTAL

            // ilk data group sayım no bilgisi,2 takım no 3 group no
            // ilk 3 veri eşitse o depo daha önce bir grup tarafından sayılmış demektir.
            // dolayısıyla ekip numarası değiştirmesi istenir

            string _sayımEkibi = _data[0] + "X" + _data[1] + "X" + _data[2];

            string _invCountData = context.InventApprove.Where(x => x.InventoryCode.Contains(_sayımEkibi)).Select(y => y.InventoryCode).SingleOrDefault();

            if (_invCountData != null)
            {
                string[] _invData = _invCountData.Split('X');

                if (_data[0] == _invData[0] && _data[1] == _invData[1] && _data[2] == _invData[2])
                {
                    return true;
                }
                return false;
            }
            return false;
        }
    }
}
