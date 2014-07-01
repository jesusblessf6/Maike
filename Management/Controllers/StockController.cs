using Enums;
using Management.Controllers.Base;
using Management.Models;
using Management.Services;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Util.Ajax;

namespace Management.Controllers
{
	public class StockController : BaseController
	{
		#region Properties

		private StockService _stockSvc;
		public StockService StockSvc
		{
			get { return _stockSvc ?? (_stockSvc = new StockService()); }
			set { _stockSvc = value; }
		}
		private CompanyService _companySvc;
		public CompanyService CompanySvc
		{
			get { return _companySvc ?? (_companySvc = new CompanyService()); }
			set { _companySvc = value; }
		}

		private CommodityService _commoditySvc;
		public CommodityService CommoditySvc
		{
			get { return _commoditySvc ?? (_commoditySvc = new CommodityService()); }
			set { _commoditySvc = value; }
		}

		private CommodityTypeService _commodityTypeSvc;
		public CommodityTypeService CommodityTypeSvc
		{
			get { return _commodityTypeSvc ?? (_commodityTypeSvc = new CommodityTypeService()); }
			set { _commodityTypeSvc = value; }
		}

		public SysSettingService SysSettingSvc
		{
			get { return _sysSettingSvc ?? (_sysSettingSvc = new SysSettingService()); }
			set { _sysSettingSvc = value; }
		}
		private SysSettingService _sysSettingSvc;
		#endregion
		//
		// GET: /Stock/

		public override ActionResult Index()
		{
			ViewBag.Title = "库存管理";
			return View();
		}

		[HttpPost]
		public ActionResult GetStocks(int companyId, int warehouseId, int commodityId, int commodityTypeId, int brandId, int start, int length)
		{
			var currentUser = Session["CurrentUser"] as CurrentUserVM;
			int from = start;
			int to = from + length - 1;

			var data = StockSvc.GetStockByRange(companyId, warehouseId, commodityId, commodityTypeId, brandId, from, to, currentUser.Id);
			var allCount = StockSvc.GetStockCount(companyId, warehouseId, commodityId, commodityTypeId, brandId, currentUser.Id);
			var result = new Dictionary<string, object>
							 {
								 {"aaData", data},
								 {"iTotalRecords", data.Count},
								 {"iTotalDisplayRecords", allCount}
							 };

			return Json(result);
		}

		public ActionResult Create()
		{
			var currentUser = Session["CurrentUser"] as CurrentUserVM;
			var commodity = CommoditySvc.GetAllCommoditiesByUser(currentUser.Id);
			var allCompany = CompanySvc.GetAllCompanyVMByUser((int)CustomerType.Internal, currentUser.Id);

			var list = new List<CommodityTypeViewVM>();
			if (commodity.Count > 0)
			{
				var comm = commodity.FirstOrDefault();
				list = CommodityTypeSvc.GetCommodityTypeByCommodityId(comm.Id);
			}
			ViewBag.Title = "新增库存";
			ViewBag.AllCommodities = commodity;
			string str = "";
			for (int i = 0; i < list.Count; i++)
			{
				str += list[i].Id + "," + list[i].Name + "||";
			}
			if (str.Length > 0)
			{
				str = str.Remove(str.Length - 2);
			}
			ViewBag.AllCommodityTypes = str;
			ViewBag.AllCompany = allCompany;
			var sysSetting = SysSettingSvc.GetSysSetting();
			ViewBag.BuyUnit = sysSetting.BuyUnit;
			return View();
		}

		[HttpPost]
		public ActionResult Create(StockCreateVM vm)
		{
			if (ModelState.IsValid)
			{
				var result = StockSvc.Create(vm);
				return MyAjaxHelper.RedirectAjax(result, "");
			}
			var error = ModelState.Values.First(o => o.Errors.Count > 0).Errors[0].ErrorMessage;
			return MyAjaxHelper.RedirectAjax(AjaxStatusCode.Error, error, null, "");

		}

		public ActionResult UpdatePrice(int id)
		{
			StockCreateVM vm = StockSvc.GetById(id);
			if (vm.PricingType == (int)PricingType.Fixed)
			{
				var stock = new StockUpdatePriceVM { Id = vm.Id, Price = vm.Price ?? 0 };
				return View(stock);
			}
			else
			{
				var stock = new StockUpdatePremiumVM { Id = vm.Id, Premium = vm.Premium ?? 0 };
				return View("UpdatePremium", stock);
			}
		}

		[HttpPost]
		public ActionResult UpdatePrice(StockUpdatePriceVM vm)
		{
			if (ModelState.IsValid)
			{
				var result = StockSvc.UpdatePrice(vm.Price, vm.Id);
				return MyAjaxHelper.RedirectAjax(result, "");
			}
			var error = ModelState.Values.First(o => o.Errors.Count > 0).Errors[0].ErrorMessage;
			return MyAjaxHelper.RedirectAjax(AjaxStatusCode.Error, error, null, "");
		}


		[HttpPost]
		public ActionResult UpdatePremium(StockUpdatePremiumVM vm)
		{
			if (ModelState.IsValid)
			{
				var result = StockSvc.UpdatePrice(vm.Premium, vm.Id);
				return MyAjaxHelper.RedirectAjax(result, "");
			}
			var error = ModelState.Values.First(o => o.Errors.Count > 0).Errors[0].ErrorMessage;
			return MyAjaxHelper.RedirectAjax(AjaxStatusCode.Error, error, null, "");

		}

		public ActionResult UpdateQty(int id)
		{
			StockCreateVM vm = StockSvc.GetById(id);
			var stock = new StockUpdateQtyVM { Id = vm.Id, Qty = vm.Quantity };
			return View(stock);
		}

		[HttpPost]
		public ActionResult UpdateQty(StockUpdateQtyVM vm)
		{
			if (ModelState.IsValid)
			{
				var result = StockSvc.UpdateQty(vm.Qty, vm.Id);
				return MyAjaxHelper.RedirectAjax(result, "");
			}
			var error = ModelState.Values.First(o => o.Errors.Count > 0).Errors[0].ErrorMessage;
			return MyAjaxHelper.RedirectAjax(AjaxStatusCode.Error, error, null, "");

		}

		[HttpPost]
		public ActionResult ClearQty(int id)
		{
			var result = StockSvc.ClearQty(id);
			return MyAjaxHelper.RedirectAjax(result, "");
		}
	}
}
