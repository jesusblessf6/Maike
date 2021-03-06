//------------------------------------------------------------------------------
// <auto-generated>
//    此代码是根据模板生成的。
//
//    手动更改此文件可能会导致应用程序中发生异常行为。
//    如果重新生成代码，则将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace Entities
{
    using System;
    using System.Collections.Generic;
    
    public partial class Stock : IEntity
    {
        public Stock()
        {
            this.SalesOrders = new HashSet<SalesOrder>();
        }
    
        public int Id { get; set; }
        public byte[] Ver { get; set; }
        public bool IsDeleted { get; set; }
        public int CompanyId { get; set; }
        public int CommodityId { get; set; }
        public int CommodityTypeId { get; set; }
        public int BrandId { get; set; }
        public int WarehouseId { get; set; }
        public decimal Quantity { get; set; }
        public decimal AvailableQty { get; set; }
        public int PricingType { get; set; }
        public Nullable<decimal> Premium { get; set; }
        public Nullable<decimal> Price { get; set; }
        public string Comment { get; set; }
        public int BuyUnit { get; set; }
        public System.DateTime Date { get; set; }
    
        public virtual Brand Brand { get; set; }
        public virtual Commodity Commodity { get; set; }
        public virtual CommodityType CommodityType { get; set; }
        public virtual Company Company { get; set; }
        public virtual ICollection<SalesOrder> SalesOrders { get; set; }
        public virtual Warehouse Warehouse { get; set; }
    }
}
