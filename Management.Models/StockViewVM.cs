namespace Management.Models
{
    public class StockViewVM
    {
        public int Id { get; set; }
        public string CompanyName { get; set; }
        public string CommotityName { get; set; }
        public string CommodityTypeName { get; set; }
        public string BrandName { get; set; }
        public string WarehouseName { get; set; }
        public decimal AvailableQty { get; set; }
        public decimal SaleQty { get; set; }
        public decimal? Premium { get; set; }
        public decimal? Price { get; set; }
        public int PriceType { get; set; }
        public string CommodityCode { get; set; }
    }
}
