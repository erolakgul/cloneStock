using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using StockHaus.Data.Model;

namespace StockHaus.Data.DataContext
{
    public class HausStockContext : DbContext
    {
        public HausStockContext()
        {
            //work connection
            //Database.Connection.ConnectionString = "Server=HAUSCL-116\\BESIKTAS;Database=StockHAUS2018DB;uid=sa;pwd=Istanbul34;";

            //home connection
            Database.Connection.ConnectionString = "Server=EROLAKGUL\\BESIKTAS;Database=StockHAUS2018DB;uid=sa;pwd=istanbul;";

            //server connection
            //Database.Connection.ConnectionString = "Server=SRVHAUSCLOUD\\SQLEXPRESS;Database=StockHAUS2018DB;uid=sa;pwd=Istanbul34*;";
        }
        public DbSet<Users> User { get; set; }
        public DbSet<Materials> Material { get; set; }
        public DbSet<Inventories> Inventory { get; set; }
        public DbSet<InventoryGroups> InventoryGroup { get; set; }
        public DbSet<InventApproves> InventApprove { get; set; }
        public DbSet<Stores> Store { get; set; }
        //public DbSet<ProdInventories> ProdInventory { get; set; }
        public DbSet<InventoryCodeLogs> InventoryCodeLog { get; set; }
        public DbSet<GroupOfMaterials> GroupOfMaterial { get; set; }
        public DbSet<CancelledInventories> CancelledInventory { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //FLUENT APİ
            //modelBuilder.Entity<Page>()
            //       .HasRequired<PageTemplate>(s => s.PageTemplates) // Student entity requires Standard 
            //       .WithMany(s => s.Pages); // Standard entity includes many Students entities

            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            // Introducing FOREIGN KEY constraint 'FK_dbo.Title_dbo.User_UserID' on table 'Title' may cause cycles or 
            //multiple cascade paths. Specify ON DELETE NO ACTION or ON UPDATE NO ACTION, or modify other FOREIGN KEY constraints.

            //Could not create constraint or index. See previous errors. HATASI İÇİN AŞAĞIDAKİ SATIR EKLENDİ
            //modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
        }
    }
}
