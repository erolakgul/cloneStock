using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StockHaus.Core.DataService;

namespace StockHaus.Core.BaseService
{
    public class ServicePoints
    {
        private InventApproveService inventApproveService;
        private InventoryGroupService inventoryGroupService;
        private InventoryService inventoryService;
        private MaterialService materialService;
        private UserService userService;
        private StoreService storeService;
        //private ProdInventoryService prodInventoryService;
        private InventoryCodeLogService inventCodeCountService;
        private GroupOfMaterialService groupOfMaterialService;
        private CancelledInventoryService cancelledInventoryService;

        public InventApproveService InventApproveService { get { return inventApproveService ?? new InventApproveService(); } }
        public InventoryGroupService InventoryGroupService { get { return inventoryGroupService ?? new InventoryGroupService(); } }
        public InventoryService InventoryService { get { return inventoryService ?? new InventoryService(); } }
        public MaterialService MaterialService { get { return materialService ?? new MaterialService(); } }
        public UserService UserService { get { return userService ?? new UserService(); } }
        public StoreService StoreService { get { return storeService ?? new StoreService(); } }
        //public ProdInventoryService ProdInventoryService { get { return prodInventoryService ?? new ProdInventoryService(); } }
        public InventoryCodeLogService InventCodeCountService { get { return inventCodeCountService ?? new InventoryCodeLogService(); } }
        public GroupOfMaterialService GroupOfMaterialService { get { return groupOfMaterialService ?? new GroupOfMaterialService(); } }
        public CancelledInventoryService CancelledInventoryService { get { return cancelledInventoryService ?? new CancelledInventoryService(); } }
    }
}
