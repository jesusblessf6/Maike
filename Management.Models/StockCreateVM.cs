using System.ComponentModel.DataAnnotations;

namespace Management.Models
{
    public class StockCreateVM
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public int CommotityId { get; set; }
        public int? CommodityTypeId { get; set; }
        public int? BrandId { get; set; }
        public int? WarehouseId { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "数量不能为空！")]
        public decimal Quantity { get; set; }
        public int PricingType { get; set; }
        public decimal? Premium { get; set; }
        public decimal? Price { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "购买单位不能为空！")]
        public int BuyUnit { get; set; }
    }
}
