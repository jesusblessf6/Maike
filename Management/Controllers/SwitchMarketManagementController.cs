using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Management.Services;
using Management.Models;
using Management.Controllers.Base;
using Util.Ajax;

namespace Management.Controllers
{
	public class SwitchMarketManagementController : BaseController
	{
		

		#region Properties
		private SwitchMarketManagementService _switchMarketManagementSvc;
		public SwitchMarketManagementService SwitchMarketManagementSvc
		{
			get { return _switchMarketManagementSvc ?? (_switchMarketManagementSvc = new SwitchMarketManagementService());}
			set { _switchMarketManagementSvc = value; }
		}
		#endregion

		//
		// GET: /SwitchMarketManagement/
		[HttpGet]
		public override ActionResult Index()
		{
			var commSvc = new CommodityService();
			ViewBag.AllCommodities = commSvc.GetAllCommodities();
			return View();
		}

		[HttpGet]
		public ActionResult Create()
		{
			ViewBag.Title = "开市时间";
			return View("Edit", new SwitchMarketManagementEditVM());
		}

		[HttpPost]
		public ActionResult Create(SwitchMarketManagementEditVM openTime)
		{
			if (ModelState.IsValid)
			{
				var result = SwitchMarketManagementSvc.Create(openTime);
				return MyAjaxHelper.RedirectAjax(result, "");
			}

			var error = ModelState.Values.First(o => o.Errors.Count > 0).Errors[0].ErrorMessage;
			return MyAjaxHelper.RedirectAjax(AjaxStatusCode.Error, error, null, "");
		}

		[HttpPost]
		public ActionResult UpdateCommodityIsOpen(int comId, bool isOpen)
		{
			var commSvc = new CommodityService();
			var errorCode = commSvc.UpdateCommodityIsOpen(comId, isOpen);
			return MyAjaxHelper.RedirectAjax(errorCode, null);
		}

		[HttpDelete]
		public ActionResult Delete(int id)
		{
			var errorcode = SwitchMarketManagementSvc.Delete(id);
			return MyAjaxHelper.RedirectAjax(errorcode, null);
		}

		[HttpGet]
		public JsonResult GetAllOpenTimes()
		{
			var data = SwitchMarketManagementSvc.GetAllOpenTimes();
			var result = new Dictionary<string, object> { 
				{"aaData", data},
				{"iTotalRecords", data.Count},
				{"iTotalDisplayRecords", data.Count}
			};
			return Json(result, JsonRequestBehavior.AllowGet);
		}

		public JsonResult GetOpenTimes(int start, int length)
		{
			var from = start;
			var to = from + length - 1;

			var data = SwitchMarketManagementSvc.GetOpenTimeByRange(from ,to);
			var allCount = SwitchMarketManagementSvc.GetAllCount();
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
		public JsonResult ValidateOpenTimes(string startTime, string endTime)
		{
			var result = new Dictionary<string, object>
							 {
								 {"valid", false}
							 };

			result["valid"] = SwitchMarketManagementSvc.GetOpenTimeExisted(startTime, endTime);
			return Json(result);
		}

		#endregion

	}
}
