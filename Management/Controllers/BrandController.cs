using System.Web.Mvc;
using Management.Controllers.Base;
using Management.Services;
using System.Collections.Generic;
using Management.Models;
using System.Linq;
using System;
using Util.Ajax;

namespace Management.Controllers
{
	public class BrandController : BaseController
	{
		#region Properties
		private BrandService _brandSvc;
		public BrandService BrandSvc
		{
			get { return _brandSvc ?? (_brandSvc = new BrandService()); }
		}

		private CommodityTypeService _commodityTypeSvc;
		public CommodityTypeService CommodityTypeSvc
		{
			get { return _commodityTypeSvc ?? (_commodityTypeSvc = new CommodityTypeService()); }
		}

		private CommodityService _commoditySvc;
		public CommodityService CommoditySvc
		{
			get { return _commoditySvc ?? (_commoditySvc = new CommodityService()); }
		}
		#endregion

		#region Actions

		//
		// GET: /Brand/
		[HttpGet]
		public override ActionResult Index()
		{
			return View();
		}

		[HttpGet]
		public ActionResult Create()
		{
			ViewBag.Title = "新增品牌";
			ViewBag.AllCommodities = CommoditySvc.GetAllCommodities();
			ViewBag.AllCommodityTypes = CommodityTypeSvc.GetAllCommodityTypes();
			return View("Edit", new BrandEditVM());
		}

		[HttpPost]
		public ActionResult Create(BrandEditVM brand)
		{
			if (ModelState.IsValid)
			{
				var result = BrandSvc.Create(brand);
				return MyAjaxHelper.RedirectAjax(result, "");
			}

			var error = ModelState.Values.First(o => o.Errors.Count > 0).Errors[0].ErrorMessage;
			return MyAjaxHelper.RedirectAjax(AjaxStatusCode.Error, error, null, "");
		}

		[HttpGet]
		public ActionResult Update(int id)
		{
			var model = BrandSvc.GetById(id);
			ViewBag.Title = "修改金属品牌";
			ViewBag.AllCommodities = CommoditySvc.GetAllCommodities();
			ViewBag.AllCommodityTypes = CommodityTypeSvc.GetCommodityTypeByCommodityId(model.CommodityId);
			return View("Edit", model);
		}

		[HttpPost]
		public ActionResult Update(BrandEditVM brand)
		{
			if (ModelState.IsValid)
			{
				var errorCode = BrandSvc.Update(brand);
				return MyAjaxHelper.RedirectAjax(errorCode, null);
			}

			var error = ModelState.Values.First(o => o.Errors.Count > 0).Errors[0].ErrorMessage;
			return MyAjaxHelper.RedirectAjax(AjaxStatusCode.Error, error, null, "");
		}

		[HttpDelete]
		public ActionResult Delete(int id)
		{
			var errorcode = BrandSvc.Delete(id);
			return MyAjaxHelper.RedirectAjax(errorcode, null);
		}

		[HttpPost]
		public JsonResult GetCommodityTypeByCommodityId(string commId)
		{
			if (string.IsNullOrEmpty(commId))
			{
				return Json(new List<CommodityTypeViewVM>());
			}
			int id = Convert.ToInt32(commId);
			var result = CommodityTypeSvc.GetCommodityTypeByCommodityId(id);

			return Json(result);
		}

		[HttpPost]
		public JsonResult GetBrands(int start, int length, int? commodityIdFilterValue, int? commodityTypeIdFilterValue, string brandNameFilterValue)
		{
			var from = start;
			var to = from + length - 1;

			var data = BrandSvc.GetBrandByRange(from, to, commodityIdFilterValue, commodityTypeIdFilterValue, brandNameFilterValue);
			var allCount = BrandSvc.GetAllCount(commodityIdFilterValue, commodityTypeIdFilterValue, brandNameFilterValue);
			var result = new Dictionary<string, object>
							 {
								 {"aaData", data},
								 {"iTotalRecords", data.Count},
								 {"iTotalDisplayRecords", allCount}
							 };

			return Json(result);
		}

		[HttpPost]
		public JsonResult GetBrandByCommodityIdAndCommodityTypeId(int commId, int commTypeId)
		{
			var result = BrandSvc.GetBrandByCommodity(commId, commTypeId);
			return Json(result);
		}

		#endregion
		

		#region Remote Vlidations

		[HttpPost]
		public JsonResult ValidateName(string name, int id, int commodityTypeId)
		{
			var result = new Dictionary<string, object>
							 {
								 {"valid", false}
							 };

			result["valid"] = BrandSvc.GetNameExisted(name, id, commodityTypeId);
			return Json(result);
		}

		#endregion

	}
}
