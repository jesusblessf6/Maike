using System.Collections.Generic;

namespace Management.Models
{
    public class MarketManagementVM
    {
        public string CommodityName { get; set; }
        public List<ShfeCodeEntity> SHFECodeList = new List<ShfeCodeEntity>();
    }
    public class ShfeCodeEntity
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public bool? IsInUse { get; set; } 
    }

}
