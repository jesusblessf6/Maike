using Enums;
using Management.Controllers.Base;
using Management.Models;
using Management.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Util.Ajax;

namespace Management.Controllers
{
    public class SalesOrderController : BaseController
    {
        #region Properties

        private SalesOrderService _salesOrderSvc;
        public SalesOrderService SalesOrderSvc
        {
            get { return _salesOrderSvc ?? (_salesOrderSvc = new SalesOrderService()); }
            set { _salesOrderSvc = value; }
        }

        #endregion
        //
        // GET: /SalesOrder/

        public override ActionResult Index()
        {
            ViewBag.Title = "订单管理";
            Dictionary<string, int> orderStatus = GetAllOrderStatus();
            ViewBag.OrderStatus = orderStatus;
            return View();
        }

        private Dictionary<string, int> GetOrderStatus()
        {
            Dictionary<string, int> orderStatus = new Dictionary<string, int>();

            Type status = typeof(SalesOrderStatus);
            var flag = true;
            int i = 1;
            SalesOrderStatus temp;
            do
            {
                if (Enum.TryParse<SalesOrderStatus>(i.ToString(), out temp))
                {
                    var str = EnumHelper.GetDescription<SalesOrderStatus>(temp);
                    if (str != null)
                    {
                        int len = str.IndexOf(",");
                        if (len != -1)
                        {
                            var msg = str.Split(new char[] { ',' });
                            orderStatus.Add(msg[0], i);
                        }
                        i++;
                    }
                    else
                    {
                        flag = false;
                    }
                }
                else
                {
                    flag = false;
                }
            } while (flag);

            return orderStatus;
        }

        private Dictionary<string, int> GetAllOrderStatus()
        {
            Dictionary<string, int> orderStatus = new Dictionary<string, int>();
            Type status = typeof(SalesOrderStatus);

            foreach (string s in Enum.GetNames(status))
            {
                var value = (int)Enum.Parse(status, s);
                var item = (SalesOrderStatus)Enum.ToObject(status, value);
                string description = EnumHelper.GetDescription<SalesOrderStatus>(item);
                var des = description.Split(new char[] { ',' })[0];
                orderStatus.Add(des, value);
            }
            return orderStatus;
        }

        [HttpPost]
        public ActionResult GetSalesOrder(FormCollection form)
        {
            var currentUser = Session["CurrentUser"] as CurrentUserVM;
            int start = int.Parse(form["start"]);
            int length = int.Parse(form["length"]);
            int companyId = int.Parse(form["companyId"]);
            int customerId = int.Parse(form["customerId"]);
            int commodityId = int.Parse(form["commodityId"]);
            int commodityTypeId = int.Parse(form["commodityTypeId"]);
            int brandId = int.Parse(form["brandId"]);
            int warsehouseId = int.Parse(form["warsehouseId"]);
            int status = 0;
            if (!string.IsNullOrWhiteSpace(form["status"].ToString()))
            {
                status = int.Parse(form["status"]);
            }
            DateTime? startDate = null;
            DateTime? endDate = null;
            if (!string.IsNullOrWhiteSpace(form["startDate"].ToString()))
            {
                startDate = DateTime.Parse(form["startDate"].ToString());
            }
            if (!string.IsNullOrWhiteSpace(form["endDate"].ToString()))
            {
                endDate = DateTime.Parse(form["endDate"].ToString());
            }
            int from = start;
            int to = from + length - 1;
            var data = SalesOrderSvc.GetSalesOrderByRange(from, to, companyId, customerId, commodityId, commodityTypeId, brandId, warsehouseId, status, startDate, endDate, currentUser.Id);
            var allCount = SalesOrderSvc.GetAllCount(companyId, customerId, commodityId, commodityTypeId, brandId, warsehouseId, status, startDate, endDate, currentUser.Id);
            var result = new Dictionary<string, object>
							 {
								 {"aaData", data},
								 {"iTotalRecords", data.Count},
								 {"iTotalDisplayRecords", allCount}
							 };

            return Json(result);
        }

        [HttpPost]
        public ActionResult CancelOrder(int id, string reason)
        {
            int status = 0;

            var result = SalesOrderSvc.CancelOrder(id, reason, ref status);
            string str = EnumHelper.GetDescription<SalesOrderStatus>((SalesOrderStatus)status);
            var msg = str.Split(new char[] { ',' })[0];
            return MyAjaxHelper.RedirectAjax(result, "", "");
        }

        [HttpPost]
        public ActionResult StatusChange(int id)
        {
            int status = 0;
            var result = SalesOrderSvc.StatusChange(id, ref status);
            return MyAjaxHelper.RedirectAjaxWithData(result, "", "", true, new { orderStatus = status });
        }
    }
}
