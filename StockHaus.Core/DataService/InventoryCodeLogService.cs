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
    public class InventoryCodeLogService : ServiceBase<InventoryCodeLogs>
    {
        public override MessageService Insert(InventoryCodeLogs dto)
        {
            // takım no grup no ve aktifliği varsa tabloda kaydetme
            if (context.InventoryCodeLog.Any(x => x.IsActive && x.GroupNo == dto.GroupNo && x.TeamNo == dto.TeamNo //&& x.InventoryGroupID == dto.InventoryGroupID
                ))
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
                    result.Message = "True";
                    context.SaveChanges();
                }
                catch (Exception ex)
                {
                    result.Message = ex.Message;
                }
            }
            return result;
        }

        public override MessageService Update(InventoryCodeLogs dto)
        {
            if (dbset.Any(x => x.ID == dto.ID && !x.IsActive))
            {
                InventoryCodeLogs d = FindByID(dto.ID);
                d.IsActive = d.IsActive;

                result.ResultID = d.ID;
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

        public string BeforeInsert(InventCodeLogModel dto)
        {
            InventoryCodeLogs v = new InventoryCodeLogs
            {
                ChangedBy = dto.ChangedBy,
                ChangeDate = dto.ChangeDate,
                Company = dto.Company,
                CountOrder = dto.CountOrder,
                CreateDate = dto.CreateDate,
                CreatedBy = dto.CreatedBy,
                GroupNo = dto.GroupNo,
                IpAddress = dto.IpAddress,
                IsActive = dto.IsActive,
                TeamNo = dto.TeamNo
            };

            MessageService msg = Insert(v);
            return msg.Message;
        }

        public int GetTeamInventCode(string _team, string _group)
        {
            int _last = context.InventoryCodeLog.Where(c => c.TeamNo == _team && c.GroupNo == _group && c.IsActive).Select(v => v.CountOrder).SingleOrDefault();
            return _last;
        }

        public bool IsDefined(string team,string group)
        {
            return context.InventoryCodeLog.Any(x => x.IsActive && x.TeamNo == team && x.GroupNo == group);
        }

        public int GetLastCode()
        {
            //int _val = context.InventCodeCount.Max(c=> c.CountOrder);
            int _lC = context.InventoryCodeLog.Where(x => x.IsActive).Select(c => c.CountOrder).OrderByDescending(e => e).FirstOrDefault();
            return _lC;
        }

        public int GetCountOrder(int team,int group)
        {
            return context.InventoryCodeLog.Where(x => x.TeamNo == team.ToString() && x.GroupNo == group.ToString() && x.IsActive).Select(c => c.CountOrder).SingleOrDefault();
        }

    }
}
