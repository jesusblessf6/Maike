using DAL;
using Entities;
using Enums;
using Management.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Util;
using Util.Query;

namespace Management.Services
{
    public class SalesOrderService : BaseService
    {
        #region Properties
        private SalesOrderDAL _salesOrderDal;
        public SalesOrderDAL SalesOrderDAL
        {
            get { return _salesOrderDal ?? (_salesOrderDal = new SalesOrderDAL()); }
            set { _salesOrderDal = value; }
        }

        private CompanyDAL _companyDal;
        public CompanyDAL CompanyDal
        {
            get { return _companyDal ?? (_companyDal = new CompanyDAL()); }
            set { _companyDal = value; }
        }

        private UserDAL _userDal;
        public UserDAL UserDAL
        {
            get { return _userDal ?? (_userDal = new UserDAL()); }
            set { _userDal = value; }
        }

        #endregion

        #region Method

        private Expression<Func<SalesOrder, bool>> GetQueryExp(int companyId, int customerId, int commodityId, int commodityTypeId, int brandId, int warsehouseId, int status, DateTime? startDate, DateTime? endDate, List<int> listCommodity, List<int> listCompany)
        {
            var clauses = new List<Clause>();
            if (customerId != 0)
            {
                clauses.Add(new Clause
                {
                    PropertyName = "CompanyId",
                    Operator = Operator.Eq,
                    Value = customerId
                });
            }

            if (companyId != 0)
            {
                clauses.Add(new Clause
                {
                    Operator = Operator.Eq,
                    PropertyName = "Stock.CompanyId",
                    Value = companyId
                });
            }

            if (commodityId != 0)
            {
                clauses.Add(new Clause
                {
                    Operator = Operator.Eq,
                    PropertyName = "Stock.CommodityId",
                    Value = commodityId
                });
            }

            if (commodityTypeId != 0)
            {
                clauses.Add(new Clause
                {
                    Operator = Operator.Eq,
                    PropertyName = "Stock.CommodityTypeId",
                    Value = commodityTypeId
                });
            }

            if (brandId != 0)
            {
                clauses.Add(new Clause
                {
                    Operator = Operator.Eq,
                    PropertyName = "Stock.BrandId",
                    Value = brandId
                });
            }

            if (warsehouseId != 0)
            {
                clauses.Add(new Clause
                {
                    Operator = Operator.Eq,
                    PropertyName = "Stock.WarehouseId",
                    Value = warsehouseId
                });
            }

            if (status != 0)
            {
                clauses.Add(new Clause
                {
                    Operator = Operator.Eq,
                    PropertyName = "Status",
                    Value = status
                });
            }

            if (startDate.HasValue)
            {
                clauses.Add(new Clause
                {
                    Operator = Operator.Ge,
                    PropertyName = "Date",
                    PropertyType = typeof(DateTime),
                    Value = startDate.Value
                });
            }

            if (endDate.HasValue)
            {
                clauses.Add(new Clause
                {
                    Operator = Operator.Le,
                    PropertyName = "Date",
                    PropertyType = typeof(DateTime),
                    Value = endDate.Value
                });
            }

            var manager = new QueryManager<SalesOrder>();
            return manager.Compile(clauses, o => listCompany.Contains(o.Stock.CompanyId) && listCommodity.Contains(o.Stock.CommodityId));
        }

        public List<SalesOrderViewVM> GetSalesOrderByRange(int from, int to, int companyId, int customerId, int commodityId, int commodityTypeId, int brandId, int warsehouseId, int status, DateTime? startDate, DateTime? endDate, int userId)
        {
            List<SalesOrderViewVM> list = new List<SalesOrderViewVM>();
            var commoditySvc = new CommodityService();
            var companySvc = new CompanyService();
            var listCommodity = commoditySvc.GetCommodityByUser(userId);
            var listCompany = companySvc.GetAllCompanyByUser((int)CustomerType.Internal, userId);
            if (listCompany.Count > 0 && listCommodity.Count > 0)
            {
                List<int> listComm = listCommodity.Select(o => o.Id).ToList();
                List<int> listComp = listCompany.Select(o => o.Id).ToList();
                var sorts = new List<SortCol> { new SortCol { ColName = "Id", IsDescending = false } };
                var func1 = GetQueryExp(companyId, customerId, commodityId, commodityTypeId, brandId, warsehouseId, status, startDate, endDate, listComm, listComp);
                var result = SalesOrderDAL.Query(func1, sorts, from, to, new List<string> { "Company", "Stock.Company", "Stock.Warehouse", "Stock.Commodity", "Stock.CommodityType", "Stock.Brand" });
                foreach (var r in result)
                {
                    SalesOrderViewVM order = new SalesOrderViewVM()
                    {
                        Id = r.Id,
                        Date = r.Date.ToString("yyyy-MM-dd"),
                        Qty = r.Quantity,
                        Price = r.Price ?? 0,
                        Remark = r.Comment,
                        Status = r.Status,
                        CustomerName = r.Company.Name,
                        InterCompanyName = r.Stock.Company.Name,
                        CommodityName = r.Stock.Commodity.Name,
                        CommodityTypeName = r.Stock.CommodityType.Name,
                        BrandName = r.Stock.Brand.Name,
                        WarsehouseName = r.Stock.Warehouse.Name
                    };
                    Type orderStatus = typeof(SalesOrderStatus);
                    var item = (SalesOrderStatus)Enum.ToObject(orderStatus, r.Status);
                    string description = EnumHelper.GetDescription<SalesOrderStatus>(item);
                    var des = description.Split(new char[] { ',' });
                    order.StatusMsg = des[0];
                    if (des.Length > 1)
                    {
                        order.OptionMsg = des[1];
                    }
                    list.Add(order);
                }
            }
            return list;
        }

        public int GetAllCount(int companyId, int customerId, int commodityId, int commodityTypeId, int brandId, int warsehouseId, int status, DateTime? startDate, DateTime? endDate, int userId)
        {
            var commoditySvc = new CommodityService();
            var companySvc = new CompanyService();
            var listCommodity = commoditySvc.GetCommodityByUser(userId);
            var listCompany = companySvc.GetAllCompanyByUser((int)CustomerType.Internal, userId);
            if (listCompany.Count > 0 && listCommodity.Count > 0)
            {
                List<int> listComm = listCommodity.Select(o => o.Id).ToList();
                List<int> listComp = listCompany.Select(o => o.Id).ToList();
                var sorts = new List<SortCol> { new SortCol { ColName = "Id", IsDescending = false } };
                var func1 = GetQueryExp(companyId, customerId, commodityId, commodityTypeId, brandId, warsehouseId, status, startDate, endDate, listComm, listComp);
                return SalesOrderDAL.GetCount(func1);
            }
            return 0;
        }

        public ErrorCode CancelOrder(int id, string reason, ref int status)
        {
            try
            {
                SalesOrder order = SalesOrderDAL.GetById(id, null);
                if (order != null)
                {
                    order.Status = (int)SalesOrderStatus.OrderCancelled;
                    SalesOrderDAL.Update(order);
                    status = order.Status;
                }
                return ErrorCode.NoError;
            }
            catch (Exception)
            {
                return ErrorCode.ServerError;
            }
        }

        public ErrorCode StatusChange(int id, ref int status)
        {
            try
            {
                SalesOrder order = SalesOrderDAL.GetById(id, null);
                if (order != null)
                {
                    var orderStatus = order.Status + 1;
                    var str = EnumHelper.GetDescription<SalesOrderStatus>((SalesOrderStatus)orderStatus);
                    if (str == null)
                    {
                        return ErrorCode.ServerError;
                    }
                    else
                    {
                        order.Status = orderStatus;
                        SalesOrderDAL.Update(order);
                        status = order.Status;
                    }
                }
                return ErrorCode.NoError;
            }
            catch (Exception)
            {
                return ErrorCode.ServerError;
            }
        }

        #endregion
    }
}
