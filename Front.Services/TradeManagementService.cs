using DAL;
using Entities;
using Front.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Util;

namespace Front.Services
{
     public class TradeManagementService
    {
         #region Properties

        private TradeManagementDAL _tradeManagementDal;
        public TradeManagementDAL TradeManagementDal
        {
            get { return _tradeManagementDal ?? (_tradeManagementDal = new TradeManagementDAL()); }
            set { _tradeManagementDal = value; }
        }

        #endregion

        #region Method
         //根据库存ID生成新增的SaleOrder
        public TradeManagementEditVM GetSalesOrderByStockId(int stockId, int currentUserId)
        {
            var stockDal = new StockDAL();
            var stock = stockDal.GetById(stockId, new List<string> { "Commodity","CommodityType","Brand","Warehouse","Company"});
            var tradeManagementEditVM = new TradeManagementEditVM { 
              Premium = string.Format("{0:#,##0}", stock.Premium),
              Price = string.Format("{0:#,##0.00}", stock.Price),
              CommodityName = stock.Commodity == null ? "" : stock.Commodity.Name,
              CommodityTypeName = stock.CommodityType == null ? "" : stock.CommodityType.Name,
              BrandName = stock.Brand == null ? "" : stock.Brand.Name,
              Warehouse = stock.Warehouse == null ? "" : stock.Warehouse.Name,
              BuyUnit = stock.BuyUnit,
              PricingType = stock.PricingType,
              SalerName = stock.Company.Name,
              CommodityId = stock.CommodityId,
              StockId = stock.Id
            };
            var companySvc = new CompanyService();
            List<CompanyVM> companyList = companySvc.GetCompanyByRelUserId(currentUserId);
            if(companyList != null && companyList.Count > 0)
            {
                tradeManagementEditVM.CompanyId = companyList.FirstOrDefault().Id;
            }
            var sysSettingSvc = new SysSettingService();
            var sysSettingVM = sysSettingSvc.GetSysSetting();
            tradeManagementEditVM.CountDown = sysSettingVM.CountDown;

            return tradeManagementEditVM;
        }

        public ErrorCode Create(string companyId, string qty, string premium, string price, string stockId,string commodityId, string pricingType)
        {
            try
            {
            decimal? quantity = 0;
            decimal? premiumResult = 0;
            decimal? priceResult = 0;
            if (!string.IsNullOrEmpty(qty))
            {
                quantity = Convert.ToDecimal(qty);
            }
            if (!string.IsNullOrEmpty(premium))
            {
                premiumResult = Convert.ToDecimal(premium);
            }
            if (!string.IsNullOrEmpty(price))
            {
                priceResult = Convert.ToDecimal(price);
            }

            var tradeManagement = new SalesOrder { CompanyId = Convert.ToInt32(companyId), Quantity = quantity.Value, StockId = Convert.ToInt32(stockId), Date = DateTime.Now, Premium = premiumResult, Price = priceResult, PricingType = Convert.ToInt32(pricingType) };

            var stockDal = new StockDAL();
            var stock = stockDal.GetById(Convert.ToInt32(stockId), null);
            var commDal = new CommodityDAL();
            var commodity = commDal.GetById(Convert.ToInt32(commodityId),null);
            if(quantity > stock.AvailableQty)
            {
                return ErrorCode.QtyNotEnough;
            }
            if(quantity % stock.BuyUnit != 0)
            {
                return ErrorCode.NotInteger;
            }
            if(!commodity.IsOpen)
            {
                return ErrorCode.CommodityNotOpen;
            }

            TradeManagementDal.Create(tradeManagement);
            stock.AvailableQty = stock.AvailableQty - tradeManagement.Quantity;
            stockDal.Update(stock);
            return ErrorCode.NoError;
            }
            catch (Exception)
            {
                return ErrorCode.ServerError;
            }
        }


        public bool GetQtyValidate(string quantity)
        {
            decimal qty;
            if(!Decimal.TryParse(quantity,out qty))
            {
                return false;
            }
            if (qty < 0 || qty.ToString().Contains("."))
            {
                return false;
            }
            return true;
        }
        #endregion
    }
}
