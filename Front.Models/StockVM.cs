using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Front.Models
{
    public class StockVM
    {
        public int Id { get; set; }
        public string CompanyName { get; set; }
        public string CommotityName { get; set; }
        public string CommodityTypeName { get; set; }
        public string BrandName { get; set; }
        public string WarehouseName { get; set; }
        public decimal AvailableQty { get; set; }
        public decimal SaleQty { get; set; }
        public string Premium { get; set; }
        public string Price { get; set; }
        public int BuyUnit { get; set; }//最小购买单位
    }
}
