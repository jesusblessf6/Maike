﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace Entities
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class MaikeEntities : DbContext
    {
        public MaikeEntities()
            : base("name=MaikeEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Commodity> Commodities { get; set; }
        public virtual DbSet<Warehouse> Warehouses { get; set; }
        public virtual DbSet<Brand> Brands { get; set; }
        public virtual DbSet<SHFECode> SHFECodes { get; set; }
        public virtual DbSet<CommodityType> CommodityTypes { get; set; }
        public virtual DbSet<Company> Companies { get; set; }
        public virtual DbSet<RelCommoditySHFECode> RelCommoditySHFECodes { get; set; }
        public virtual DbSet<RelUserCommodity> RelUserCommodities { get; set; }
        public virtual DbSet<OpenTime> OpenTimes { get; set; }
        public virtual DbSet<SalesOrder> SalesOrders { get; set; }
        public virtual DbSet<Stock> Stocks { get; set; }
        public virtual DbSet<SysSetting> SysSettings { get; set; }
        public virtual DbSet<RelUserCompany> RelUserCompanies { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<Action> Actions { get; set; }
        public virtual DbSet<Controller> Controllers { get; set; }
        public virtual DbSet<Previlege> Previleges { get; set; }
    }
}
