using Management.Controllers.Base;
using System;
using System.Web.Mvc;
using Management.Services;
using Util.Ajax;

namespace Management.Controllers
{
	public class MarketManagementController : BaseController
	{
		
		#region Properties
		private MarketManagementService _marketManagementSvc;
		public MarketManagementService MarketManageMentSvc
		{
			get { return _marketManagementSvc ?? (_marketManagementSvc = new MarketManagementService()); }
			set { _marketManagementSvc = value; }
		}
		#endregion  

		#region Actions

		//
		// GET: /MarketManagement/
		[HttpGet]
		public override ActionResult Index()
		{
			var mmClassList = MarketManageMentSvc.GetAllSHFECodeGroup();
			return View(mmClassList);
		}

		[HttpPost]
		public ActionResult Update(string[] codeId)
		{
			if (codeId != null)
			{
				int id = Convert.ToInt32(codeId[0]);
				var result = MarketManageMentSvc.UpdateMarketManagement(id);
				return MyAjaxHelper.RedirectAjax(result, "");
			}

			return MyAjaxHelper.RedirectAjax(AjaxStatusCode.Error, "操作失败", null, "");
		}

		#endregion
		
	}
}
