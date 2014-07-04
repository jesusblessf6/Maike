using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Management.Models
{
    public class SalesOrderViewVM
    {
        public int Id { get; set; }
        public string Date { get; set; }
        public string InterCompanyName { get; set; }
        public string CustomerName { get; set; }
        public string CommodityName { get; set; }
        public string CommodityTypeName { get; set; }
        public string BrandName { get; set; }
        public string WarsehouseName { get; set; }
        public decimal Qty { get; set; }
        public decimal Price { get; set; }
        public int Status { get; set; }
        public string StatusMsg { get; set; }
        public string Remark { get; set; }
        public string OptionMsg { get; set; }
    }
}
