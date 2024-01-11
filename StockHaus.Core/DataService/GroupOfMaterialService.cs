using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StockHaus.Core.BaseService;
using StockHaus.Data.Model;

namespace StockHaus.Core.DataService
{
    public class GroupOfMaterialService : ServiceBase<GroupOfMaterials>
    {

        public override MessageService Insert(GroupOfMaterials dto)
        {
            throw new NotImplementedException();
        }

        public override MessageService Update(GroupOfMaterials dto)
        {
            throw new NotImplementedException();
        }

        public bool IsAgroup(string matCode)
        {
            return context.GroupOfMaterial.Any(c => c.IsActive && c.MatCode == matCode);
        }
    }
}
