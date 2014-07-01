using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Front.Models;
using Entities;
using System.Linq.Expressions;
using Util.Query;

namespace Front.Services
{
    public class StockService
    {
        #region Properties

        private StockDAL _stockDal;
        public StockDAL StockDAL
        {
            get { return _stockDal ?? (_stockDal = new StockDAL()); }
            set { _stockDal = value; }
        }
        #endregion
        #region Method
        public List<StockVM> GetStockByRange(int companyId, int warehouseId, int commodityId, int? commodityTypeId, int brandId, int from, int to,int userId)
        {
            List<StockVM> list = new List<StockVM>();
            List<SortCol> sorts = new List<SortCol>();
            CommodityService commoditySvc = new CommodityService();
            List<CommodityVM> listCommodity = commoditySvc.GetCommodityByUserId(userId);
            if (listCommodity != null && listCommodity.Count > 0)
            {
                List<int> listCommId = listCommodity.Select(o => o.Id).ToList();
                sorts.Add(new SortCol { ColName = "Id", IsDescending = false });

                var func1 = GetQueryExp(companyId, warehouseId, commodityId, commodityTypeId, brandId, listCommId);
                var result = StockDAL.Query(func1, sorts, from, to, new List<string> { "Company", "Warehouse", "Commodity", "CommodityType", "Brand", "SalesOrders" });
                foreach (var r in result)
                {
                    StockVM stockView = new StockVM();
                    stockView.Id = r.Id;
                    stockView.CompanyName = r.Company.Name;
                    stockView.CommotityName = r.Commodity.Name;
                    stockView.CommodityTypeName = r.CommodityType == null ? "" : r.CommodityType.Name;
                    stockView.BrandName = r.Brand == null ? "" : r.Brand.Name;
                    stockView.WarehouseName = r.Warehouse == null ? "" : r.Warehouse.Name;
                    stockView.AvailableQty = r.AvailableQty;
                    stockView.Premium = string.Format("{0:#,##0}", r.Premium);
                    stockView.Price = string.Format( "{0:#,##0.00}",r.Price);
                    stockView.BuyUnit = r.BuyUnit;
                    decimal sumSaleQty = 0;
                    foreach (var order in r.SalesOrders)
                    {
                        if (!order.IsDeleted)
                        {
                            sumSaleQty += order.Quantity;
                        }
                    }
                    stockView.SaleQty = sumSaleQty;
                    list.Add(stockView);
                }
            }
            return list;
        }

        public int GetStockCount(int companyId, int warehouseId, int commodityId, int commodityTypeId, int brandId, int userId)
        {
            //if (companyId == 0 && warehouseId == 0 && commodityId == 0 && commodityTypeId == 0 && brandId == 0)
            //{
            //    return StockDAL.GetAllCount();
            //}
            CommodityService commoditySvc = new CommodityService();
            List<CommodityVM> listCommodity = commoditySvc.GetCommodityByUserId(userId);
            if (listCommodity != null && listCommodity.Count > 0)
            {
                List<int> commodityIdList = listCommodity.Select(c => c.Id).ToList();
                var func1 = GetQueryExp(companyId, warehouseId, commodityId, commodityTypeId, brandId, commodityIdList);
                if (func1 == null)
                {
                    return StockDAL.GetAllCount();
                }
                return StockDAL.GetCount(func1);
            }
            else
            {
                return 0;
            }
        }

        private Expression<Func<Stock, bool>> GetQueryExp(int companyId, int? warehouseId, int commodityId, int? commodityTypeId, int? brandId, List<int> commodityIdList)
        {
            var clauses = new List<Clause>();
            if (companyId != 0)
            {
                clauses.Add(new Clause
                {
                    PropertyName = "CompanyId",
                    Operator = Operator.Eq,
                    Value = companyId
                });
            }

            if (warehouseId.HasValue && warehouseId != 0)
            {
                clauses.Add(new Clause
                {
                    Operator = Operator.Eq,
                    PropertyName = "WarehouseId",
                    Value = warehouseId
                });
            }

            if (commodityId != 0)
            {
                clauses.Add(new Clause
                {
                    Operator = Operator.Eq,
                    PropertyName = "CommodityId",
                    Value = commodityId
                });
            }

            if (commodityTypeId.HasValue && commodityTypeId.Value != 0)
            {
                clauses.Add(new Clause
                {
                    Operator = Operator.Eq,
                    PropertyName = "CommodityTypeId",
                    Value = commodityTypeId
                });
            }

            if (brandId.HasValue && brandId != 0)
            {
                clauses.Add(new Clause
                {
                    Operator = Operator.Eq,
                    PropertyName = "BrandId",
                    Value = brandId
                });
            }

            clauses.Add(new Clause { Operator = Operator.Gt, PropertyName = "AvailableQty", Value = 0M });
            
            var manager = new QueryManager<Stock>();
            return manager.Compile(clauses, c => commodityIdList.Contains(c.CommodityId));
        }
        #endregion
    }
}
