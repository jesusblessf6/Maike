using Management.Controllers.Base;
using Management.Models;
using Management.Services;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Util.Ajax;

namespace Management.Controllers
{
	public class WarehouseController : BaseController
	{
		//
		// GET: /Warehouse/
		#region Properties
		private WarehouseService _warehouseSvc;
		public WarehouseService WarehouseSvc
		{
			get { return _warehouseSvc ?? (_warehouseSvc = new WarehouseService()); }
			set { _warehouseSvc = value; }
		}
		#endregion

		[HttpGet]
		public override ActionResult Index()
		{
			return View();
		}

		[HttpGet]
		public ActionResult Create()
		{
			ViewBag.Title = "新增仓库";
			return View("Edit", new WarehouseEditVM());
		}

		[HttpPost]
		public ActionResult Create(WarehouseEditVM warehouse)
		{
			if (ModelState.IsValid)
			{
				var result = WarehouseSvc.Create(warehouse);
				return MyAjaxHelper.RedirectAjax(result, "");
			}

			var error = ModelState.Values.First(o => o.Errors.Count > 0).Errors[0].ErrorMessage;
			return MyAjaxHelper.RedirectAjax(AjaxStatusCode.Error, error, null, "");
		}

		[HttpGet]
		public ActionResult Update(int id)
		{
			var model = WarehouseSvc.GetById(id);
			ViewBag.Title = "修改仓库";
			return View("Edit", model);
		}

		[HttpPost]
		public ActionResult Update(WarehouseEditVM warehouse)
		{
			if (ModelState.IsValid)
			{
				var errorCode = WarehouseSvc.Update(warehouse);
				return MyAjaxHelper.RedirectAjax(errorCode, null);
			}

			var error = ModelState.Values.First(o => o.Errors.Count > 0).Errors[0].ErrorMessage;
			return MyAjaxHelper.RedirectAjax(AjaxStatusCode.Error, error, null, "");
		}

		[HttpDelete]
		public ActionResult Delete(int id)
		{
			var errorCode = WarehouseSvc.Delete(id);
			return MyAjaxHelper.RedirectAjax(errorCode, null);
		}

		public JsonResult GetWarehouses(int start, int length, string warehouseName)
		{
			var from = start;
			var to = from + length - 1;
			var data = WarehouseSvc.GetWarehouseByRange(from, to, warehouseName);
			var count = WarehouseSvc.GetCount(warehouseName);
			var result = new Dictionary<string, object> { 
								 {"aaData", data},
								 {"iTotalRecords", data.Count},
								 {"iTotalDisplayRecords", count}
			};
			return Json(result);
		}

		[HttpPost]
		public ActionResult GetWarehouse(string q, int page_limit, int page)
		{
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

		#region Remote Vlidations

		[HttpPost]
		public JsonResult ValidateName(string name, int id)
		{
			var result = new Dictionary<string, object>
							 {
								 {"valid", false}
							 };

			result["valid"] = WarehouseSvc.GetNameExisted(name, id);
			return Json(result);
		}

		public JsonResult ValidateTel(string tel)
		{
			var result = new Dictionary<string, object>
							 {
								 {"valid", false}
							 };

			result["valid"] = WarehouseSvc.ValidateTelFormat(tel);
			return Json(result);
		}

		public JsonResult ValidateFax(string fax)
		{
			var result = new Dictionary<string, object>
							 {
								 {"valid", false}
							 };

			result["valid"] = WarehouseSvc.ValidateFaxFormat(fax);
			return Json(result);
		}
		#endregion

	}
}
