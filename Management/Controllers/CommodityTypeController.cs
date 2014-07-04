using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Management.Controllers.Base;
using Management.Models;
using Management.Services;
using Util.Ajax;

namespace Management.Controllers
{
	public class CommodityTypeController : BaseController
	{
		#region Properties

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
		// GET: /CommodityType/
		[HttpGet]
		public override ActionResult Index()
		{
			return View();
		}

		//
		//Get: /CommodityType/Create
		[HttpGet]
		public ActionResult Create()
		{
			ViewBag.Title = "金属类型";
			ViewBag.AllCommodities = CommoditySvc.GetAllCommodities();
			return View("Edit", new CommodityTypeEditVM());
		}

		//
		//Post: /CommodityType/Create {CommodityTypeEditVM}
		[HttpPost]
		public ActionResult Create(CommodityTypeEditVM commodityType)
		{
			if (ModelState.IsValid)
			{
				var result = CommodityTypeSvc.Create(commodityType);
				return MyAjaxHelper.RedirectAjax(result, "");
			}
			
			var error = ModelState.Values.First(o => o.Errors.Count > 0).Errors[0].ErrorMessage;
			return MyAjaxHelper.RedirectAjax(AjaxStatusCode.Error, error, null, "");
		}

		//
		//Get: /CommodityType/GetAllCommodityTypes
		[HttpGet]
		public JsonResult GetAllCommodityTypes()
		{
			var data = CommodityTypeSvc.GetAllCommodityTypes();
			var result = new Dictionary<string, object>
							 {
								 {"aaData", data},
								 {"iTotalRecords", data.Count},
								 {"iTotalDisplayRecords", data.Count}
							 };
			return Json(result, JsonRequestBehavior.AllowGet);
		}

		//
		// Post: /CommodityType/GetCommodityTypes :start, :length
		[HttpPost]
		public JsonResult GetCommodityTypes(int start, int length, int? commodityIdFilterValue, string commodityNameFilterValue)
		{
			var from = start;
			var to = from + length - 1;

			var data = CommodityTypeSvc.GetCommodityTypeByRange(from, to, commodityIdFilterValue, commodityNameFilterValue);
			var allCount = CommodityTypeSvc.GetCount(commodityIdFilterValue, commodityNameFilterValue);
			var result = new Dictionary<string, object>
							 {
								 {"aaData", data},
								 {"iTotalRecords", data.Count},
								 {"iTotalDisplayRecords", allCount}
							 };

			return Json(result);
		}

		//
		// Get: /CommodityType/Update/:id
		[HttpGet]
		public ActionResult Update(int id)
		{
			var model = CommodityTypeSvc.GetById(id);
			ViewBag.Title = "金属类型";
			ViewBag.AllCommodities = CommoditySvc.GetAllCommodities();
			return View("Edit", model);
		}

		//
		// Post: /CommodityType/Update {CommodityTypeEditVM}
		[HttpPost]
		public ActionResult Update(CommodityTypeEditVM commodityType)
		{
			if (ModelState.IsValid)
			{
				var errorCode = CommodityTypeSvc.Update(commodityType);
				return MyAjaxHelper.RedirectAjax(errorCode, null);
			}

			var error = ModelState.Values.First(o => o.Errors.Count > 0).Errors[0].ErrorMessage;
			return MyAjaxHelper.RedirectAjax(AjaxStatusCode.Error, error, null, "");
		}

		//
		// Delete: /CommodityType/Delete/:id
		[HttpDelete]
		public ActionResult Delete(int id)
		{
			var errorcode = CommodityTypeSvc.Delete(id);
			return MyAjaxHelper.RedirectAjax(errorcode, null);
		}

		[HttpPost]
		public JsonResult GetAllCommodities(string q)
		{
			List<CommodityViewVM> allCommos = string.IsNullOrWhiteSpace(q) ? CommoditySvc.GetAllCommodities() : CommoditySvc.SearchCommodities(q);

			var result = allCommos.Select(o => new Dictionary<string, object>
												   {
													   {"id", o.Id},
													   {"text", o.Name}
												   }).ToList();
			return Json(result);
		}

		[HttpPost]
		public JsonResult GetAllCommoditiesByUser(string q)
		{
			var user = Session["CurrentUser"] as CurrentUserVM;
			var commSvc = new CommodityService();

			List<CommodityViewVM> allCommos = string.IsNullOrWhiteSpace(q) ? commSvc.GetAllCommoditiesByUser(user.Id) : commSvc.SearchCommoditiesByUser(q,user.Id);

			var result = allCommos.Select(o => new Dictionary<string, object>
												   {
													   {"id", o.Id},
													   {"text", o.Name}
												   }).ToList();
			return Json(result);
		}
		#endregion

		#region Remote Vlidations

		[HttpPost]
		public JsonResult ValidateName(string name, int id, int commodityId)
		{
			var result = new Dictionary<string, object>
							 {
								 {"valid", false}
							 };

			result["valid"] = CommodityTypeSvc.GetNameExisted(name, id, commodityId);
			return Json(result);
		}

		#endregion
	}
}
