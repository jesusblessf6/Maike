//------------------------------------------------------------------------------
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
    using System.Collections.Generic;
    
    public partial class Commodity : IEntity
    {
        public Commodity()
        {
            this.SHFECodes = new HashSet<SHFECode>();
            this.Brands = new HashSet<Brand>();
            this.CommodityTypes = new HashSet<CommodityType>();
            this.RelCommoditySHFECodes = new HashSet<RelCommoditySHFECode>();
            this.RelUserCommodities = new HashSet<RelUserCommodity>();
            this.Stocks = new HashSet<Stock>();
        }
    
        public int Id { get; set; }
        public bool IsDeleted { get; set; }
        public byte[] Ver { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Code { get; set; }
        public bool IsOpen { get; set; }
    
        public virtual ICollection<SHFECode> SHFECodes { get; set; }
        public virtual ICollection<Brand> Brands { get; set; }
        public virtual ICollection<CommodityType> CommodityTypes { get; set; }
        public virtual ICollection<RelCommoditySHFECode> RelCommoditySHFECodes { get; set; }
        public virtual ICollection<RelUserCommodity> RelUserCommodities { get; set; }
        public virtual ICollection<Stock> Stocks { get; set; }
    }
}