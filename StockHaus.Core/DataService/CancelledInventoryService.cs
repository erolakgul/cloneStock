using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StockHaus.Core.BaseService;
using StockHaus.Data.Model;
using StockHaus.ModelClass.AdminPage;

namespace StockHaus.Core.DataService
{
    public class CancelledInventoryService : ServiceBase<CancelledInventories>
    {
        public override MessageService Insert(CancelledInventories dto)
        {
            if (context.CancelledInventory.Any(x =>  x.InventoryCode == dto.InventoryCode && x.MatCode == dto.MatCode && x.MatIndex == dto.MatIndex && x.MatSerialNumber == dto.MatSerialNumber && x.MatSection == dto.MatSection && x.MatSectionPlace == dto.MatSectionPlace))
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

        public override MessageService Update(CancelledInventories dto)
        {
            throw new NotImplementedException();
        }

        public string BeforeInsert(List<InventoriesModel> dtos)
        {
            foreach (var dto in dtos)
            {
                CancelledInventories mod = new CancelledInventories
                {
                    ChangeDate = DateTime.Now,
                    ChangedBy = dto.ChangedBy,
                    Company = "01",
                    CreateDate = DateTime.Now,
                    CreatedBy = dto.CreatedBy,
                    InventoryCode = dto.InventoryCode,
                    IsActive = true,
                    MatCode = dto.MatCode,
                    MatIndex = dto.MatIndex,
                    MatName = dto.MatName,
                    MatQuantity = dto.MatQuantity,
                    MatSection = dto.MatSection,
                    MatSectionPlace = dto.MatSectionPlace,
                    MatSerialNumber = dto.MatSerialNumber,
                    MatSpecialStock = dto.MatSpecialStock
                };

                MessageService msg = Insert(mod);
            }

            return "True";
        }
    }
}
