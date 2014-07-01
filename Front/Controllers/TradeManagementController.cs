using Front.Controllers.Base;
using Front.Models;
using Front.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Util.Ajax;

namespace Front.Controllers
{
    public class TradeManagementController : BaseController
    {
        //
        // GET: /TradeManagement/
        #region Properties
        private TradeManagementService _TradeManagementSvc;
        public TradeManagementService TradeManagementSvc
        {
            get { return _TradeManagementSvc ?? (_TradeManagementSvc = new TradeManagementService()); }
            set { _TradeManagementSvc = value; }
        }
        #endregion

        [HttpGet]
        public override ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Create(string id)
        {
            ViewBag.Title = "购买详情";
            int currentUserId = Convert.ToInt32(Session["CurrentUserId"].ToString());
            var companySvc = new CompanyService();
            ViewBag.AllCompanyList = companySvc.GetCompanyByRelUserId(currentUserId);//初始化购买订单页面
            var tradeManagementEditVM = TradeManagementSvc.GetSalesOrderByStockId(Convert.ToInt32(id), currentUserId);
            ViewBag.PricingType = tradeManagementEditVM.PricingType;
            return View("Edit", tradeManagementEditVM);
        }

        [HttpGet]
        public ActionResult CancelAction(string stockId, string companyId, string qty)
        {
            ViewBag.Title = "购买详情";
            int currentUserId = Convert.ToInt32(Session["CurrentUserId"].ToString());
            var companySvc = new CompanyService();
            ViewBag.AllCompanyList = companySvc.GetCompanyByRelUserId(currentUserId);//初始化购买订单页面
            var tradeManagementEditVM = TradeManagementSvc.GetSalesOrderByStockId(Convert.ToInt32(stockId), currentUserId);
            ViewBag.PricingType = tradeManagementEditVM.PricingType;
            tradeManagementEditVM.CompanyId = Convert.ToInt32(companyId);
            tradeManagementEditVM.Quantity = Convert.ToDecimal(qty);
            return View("Edit", tradeManagementEditVM);
        }

        [HttpPost]
        public ActionResult Create(string companyId, string qty, string premium, string price, string stockId, string commodityId, string pricingType)
        {
            if (ModelState.IsValid)
            {
                var result = TradeManagementSvc.Create(companyId, qty, premium, price, stockId, commodityId, pricingType);
                return MyAjaxHelper.RedirectAjax(result, "");
            }

            var error = ModelState.Values.First(o => o.Errors.Count > 0).Errors[0].ErrorMessage;
            return MyAjaxHelper.RedirectAjax(AjaxStatusCode.Error, error, null, "");
        }

        [HttpGet]
        public JsonResult GetBuyerName(string id)
        { 
            var companySvc = new CompanyService();
            var companyVM = companySvc.GetCompanyById(Convert.ToInt32(id));
            var result = new Dictionary<string, object>{{"id", companyVM.Id}, {"name", companyVM.Name}};
            return Json(result,JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetAllCommodities(string q)
        {
            int currentUserId = Convert.ToInt32(Session["CurrentUserId"].ToString());
            var commSvc = new CommodityService();
            List<CommodityVM> allCommos;

            if (string.IsNullOrWhiteSpace(q))
            {
                allCommos = commSvc.GetCommodityByUserId(currentUserId);
            }
            else
            {
                allCommos = commSvc.SearchCommodities(currentUserId,q);
            }

            var result = allCommos.Select(o => new Dictionary<string, object>
				                                   {
					                                   {"id", o.Id},
													   {"text", o.Name}
				                                   }).ToList();
            return Json(result);
        }

        [HttpPost]
        public ActionResult GetCompany(string q, int page_limit, int page, int companyType)
        {
            var CompanySvc = new CompanyService();
            int from = (page - 1) * page_limit;
            int to = from + page_limit - 1;
            var company = CompanySvc.GetCompanyByRange(from, to, q, companyType).Select(o => new Dictionary<string, object> { 
				{"id", o.Id},
				{"text", o.Name}
			}).ToList();
            int count = CompanySvc.GetAllCount(companyType, q);
            var result = new Dictionary<string, object>{
				{"total",count},
				{"company",company}
			};
            return Json(result);
        }

        [HttpPost]
        public ActionResult GetWarehouse(string q, int page_limit, int page)
        {
            var WarehouseSvc = new WarehouseService();
            int from = (page - 1) * page_limit;
            int to = from + page_limit - 1;
            var company = WarehouseSvc.GetWarehouseByRange(from, to, q).Select(o => new Dictionary<string, object> { 
				{"id", o.Id},
				{"text", o.Name}
			}).ToList();
            int count = WarehouseSvc.GetCount(q);
            var result = new Dictionary<string, object>{
				{"total",count},
				{"warehouse",company}
			};
            return Json(result);
        }

        public JsonResult GetCommodityTypeByCommodityId(string commId)
        {
            var commodityTypeSvc = new CommodityTypeService();
            if (string.IsNullOrEmpty(commId))
            {
                return Json(new List<CommodityTypeVM>());
            }
            int id = Convert.ToInt32(commId);
            var result = commodityTypeSvc.GetCommodityTypeByCommodityId(id);

            return Json(result);
        }

        public JsonResult GetBrandByCommodityIdAndCommodityTypeId(int commId, int commTypeId)
        {
            var BrandSvc = new BrandService();
            var result = BrandSvc.GetBrandByCommodity(commId, commTypeId);
            return Json(result);
        }

        [HttpPost]
        public ActionResult GetStocks(int companyId, int warehouseId, int commodityId, int commodityTypeId, int brandId, int start, int length)
        {
            int currentUserId = Convert.ToInt32(Session["CurrentUserId"].ToString());
            var StockSvc = new StockService();
            int from = start;
            int to = from + length - 1;

            var data = StockSvc.GetStockByRange(companyId, warehouseId, commodityId, commodityTypeId, brandId, start, length, currentUserId);
            var allCount = StockSvc.GetStockCount(companyId, warehouseId, commodityId, commodityTypeId, brandId, currentUserId);
            var result = new Dictionary<string, object>
							 {
								 {"aaData", data},
								 {"iTotalRecords", data.Count},
								 {"iTotalDisplayRecords", allCount}
							 };

            return Json(result);
        }

        #region Remote Vlidations

        [HttpPost]
        public JsonResult ValidateQty(string Quantity)
        {
            var result = new Dictionary<string, object>
				             {
					             {"valid", false}
				             };

            result["valid"] = TradeManagementSvc.GetQtyValidate(Quantity);
            return Json(result);
        }

        #endregion
    }
}
