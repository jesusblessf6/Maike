using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Management.Controllers.Base;
using Management.Services;

namespace Management.Controllers
{
	public class CommodityController : BaseController
	{
		#region Properties

		public CommodityService CommoditySvc
		{
			get { return _commoditySvc ?? (_commoditySvc = new CommodityService()); }
			set { _commoditySvc = value; }
		}
		private CommodityService _commoditySvc;

		#endregion

		#region Actions

		//
		// GET: /Commodity/
		[HttpGet]
		public override ActionResult Index()
		{
			var comms = CommoditySvc.GetAllCommodities();
			return View(comms);
		}

		//
		// Get: /Commodity/Overview
		[HttpGet]
		public ActionResult Overview()
		{
			return View();
		}

		//
		//Get : /Commodity/GetAllInJson
		[HttpGet]
		public JsonResult GetAllInJson()
		{
			var comms = CommoditySvc.GetAllCommodities();

			var results = comms.Select(o => new Dictionary<string, object>
				                                {
					                                {"id", o.Id},
					                                {"text", o.Name}
				                                }).ToList();
			results.Insert(0, new Dictionary<string, object>
				                  {
					                  {"id", 0},
					                  {"text", "--"}
				                  });
			return Json(results, JsonRequestBehavior.AllowGet);
		}

		#endregion
		
	}
}
