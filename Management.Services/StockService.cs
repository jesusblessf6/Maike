using DAL;
using Entities;
using Enums;
using Management.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Util;
using Util.Query;

namespace Management.Services
{
    public class StockService : BaseService
    {
        #region Properties

        private StockDAL _stockDal;
        public StockDAL StockDAL
        {
            get { return _stockDal ?? (_stockDal = new StockDAL()); }
            set { _stockDal = value; }
        }
        #endregion

        public List<StockViewVM> GetStockByRange(int companyId, int warehouseId, int commodityId, int commodityTypeId, int brandId, int from, int to, int userId)
        {
            var list = new List<StockViewVM>();
            var commoditySvc = new CommodityService();
            var companySvc = new CompanyService();
            var listCommodity = commoditySvc.GetCommodityByUser(userId);
            var listCompany = companySvc.GetAllCompanyByUser((int)CustomerType.Internal, userId);
            if (listCompany.Count > 0 && listCommodity.Count > 0)
            {
                List<int> listComm = listCommodity.Select(o => o.Id).ToList();
                List<int> listComp = listCompany.Select(o => o.Id).ToList();
                var sorts = new List<SortCol> { new SortCol { ColName = "Id", IsDescending = false } };
                var func1 = GetQueryExp(companyId, warehouseId, commodityId, commodityTypeId, brandId, listComm, listComp);
                var result = StockDAL.Query(func1, sorts, from, to, new List<string> { "Company", "Warehouse", "Commodity", "CommodityType", "Brand", "SalesOrders" });
                foreach (var r in result)
                {
                    var stockView = new StockViewVM
                                        {
                                            Id = r.Id,
                                            CompanyName = r.Company.Name,
                                            CommotityName = r.Commodity.Name,
                                            CommodityCode = r.Commodity.Code
                                        };
                    if (r.CommodityType != null)
                        stockView.CommodityTypeName = r.CommodityType.Name;
                    if (r.Brand != null)
                        stockView.BrandName = r.Brand.Name;
                    if (r.Warehouse != null)
                        stockView.WarehouseName = r.Warehouse.Name;
                    stockView.AvailableQty = r.AvailableQty;
                    stockView.Premium = r.Premium;
                    stockView.Price = r.Price;
                    stockView.PriceType = r.PricingType;
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
            var commoditySvc = new CommodityService();
            var companySvc = new CompanyService();
            var listCommodity = commoditySvc.GetCommodityByUser(userId);
            var listCompany = companySvc.GetAllCompanyByUser((int)CustomerType.Internal, userId);
            if (listCommodity.Count > 0 && listCompany.Count > 0)
            {
                List<int> listComm = listCommodity.Select(o => o.Id).ToList();
                List<int> listComp = listCompany.Select(o => o.Id).ToList();
                var func1 = GetQueryExp(companyId, warehouseId, commodityId, commodityTypeId, brandId, listComm, listComp);
                return StockDAL.GetCount(func1);
            }

            return 0;
        }

        private Expression<Func<Stock, bool>> GetQueryExp(int companyId, int warehouseId, int commodityId, int commodityTypeId, int brandId, List<int> listCommodity, List<int> listCompany)
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

            if (warehouseId != 0)
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

            if (commodityTypeId != 0)
            {
                clauses.Add(new Clause
                {
                    Operator = Operator.Eq,
                    PropertyName = "CommodityTypeId",
                    Value = commodityTypeId
                });
            }

            if (brandId != 0)
            {
                clauses.Add(new Clause
                {
                    Operator = Operator.Eq,
                    PropertyName = "BrandId",
                    Value = brandId
                });
            }

            clauses.Add(new Clause
            {
                Operator = Operator.Ne,
                PropertyName = "AvailableQty",
                Value = 0M
            });

            var manager = new QueryManager<Stock>();
            return manager.Compile(clauses, o => listCompany.Contains(o.CompanyId) && listCommodity.Contains(o.CommodityId));
        }

        public ErrorCode Create(StockCreateVM vm)
        {
            try
            {
                var stock = new Stock { CompanyId = vm.CompanyId, CommodityId = vm.CommotityId };
                //if ((vm.CommodityTypeId ?? 0) != 0)
                //{
                    stock.CommodityTypeId = vm.CommodityTypeId;
                //}
                //if ((vm.BrandId ?? 0) != 0)
                //{
                    stock.BrandId = vm.BrandId;
                //}
                //if ((vm.WarehouseId ?? 0) != 0)
                //{
                    stock.WarehouseId = vm.WarehouseId;
                //}
                stock.Quantity = vm.Quantity;
                stock.AvailableQty = vm.Quantity;
                stock.PricingType = vm.PricingType;
                if (stock.PricingType == (int)PricingType.Fixed)
                {
                    stock.Price = vm.Price;
                }
                else if (stock.PricingType == (int)PricingType.Premium)
                {
                    stock.Premium = vm.Premium;
                }
                stock.BuyUnit = vm.BuyUnit;
                stock.Date = DateTime.Now;
                StockDAL.Create(stock);
                return ErrorCode.NoError;
            }
            catch (Exception)
            {
                return ErrorCode.ServerError;
            }
        }

        public ErrorCode UpdatePrice(decimal price, int id)
        {
            try
            {
                Stock stock = StockDAL.GetById(id, null);
                if (stock != null)
                {
                    if (stock.PricingType == (int)PricingType.Fixed)
                    {
                        stock.Price = price;
                    }
                    else if (stock.PricingType == (int)PricingType.Premium)
                    {
                        stock.Premium = price;
                    }
                    StockDAL.Update(stock);
                }
                return ErrorCode.NoError;
            }
            catch (Exception)
            {
                return ErrorCode.ServerError;
            }
        }

        public ErrorCode UpdateQty(decimal qty, int id)
        {
            try
            {
                Stock stock = StockDAL.GetById(id, null);
                if (stock != null)
                {
                    if (stock.AvailableQty + qty < 0)
                    {
                        return ErrorCode.StockQuantityNotEnough;
                    }

                    stock.Quantity += qty;
                    stock.AvailableQty += qty;
                    StockDAL.Update(stock);
                }
                return ErrorCode.NoError;
            }
            catch (Exception)
            {
                return ErrorCode.ServerError;
            }
        }

        public ErrorCode ClearQty(int id)
        {
            try
            {
                Stock stock = StockDAL.GetById(id, null);
                if (stock != null)
                {
                    stock.AvailableQty = 0;
                    StockDAL.Update(stock);
                }
                return ErrorCode.NoError;
            }
            catch (Exception)
            {
                return ErrorCode.ServerError;
            }
        }

        public StockCreateVM GetById(int id)
        {
            Stock stock = StockDAL.GetById(id, null);
            return new StockCreateVM { Id = stock.Id, PricingType = stock.PricingType, Price = stock.Price, Premium = stock.Premium, Quantity = stock.Quantity };
        }
    }
}
