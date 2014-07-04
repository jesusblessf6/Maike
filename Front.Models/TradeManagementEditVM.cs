using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Front.Models
{
    public class TradeManagementEditVM
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }//购买方
        public string CompanyName { get; set; }
        public decimal? Quantity { get; set; }
        public string Comment { get; set; }
        public int Status { get; set; }
        public int StockId { get; set; }
        public string SalerName { get; set; }//销售方
        public DateTime Date { get; set; }
        public int PricingType { get; set; }
        public string Premium { get; set; }
        public string Price { get; set; }
        public string FinalPrice { get; set; }
        public int CommodityId { get; set; }
        public string CommodityName { get; set; }
        public string CommodityTypeName { get; set; }
        public string BrandName { get; set; }
        public string Warehouse { get; set; }
        public int BuyUnit { get; set; }//最小购买数量
        public int CountDown { get; set; }//系统中设置的下单确定秒数
        public string CommodityCode { get; set; }//金属Code
        public string CommodityUnit { get; set; }//金属单位
    }
}
