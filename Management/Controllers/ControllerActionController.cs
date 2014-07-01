using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Management.Controllers.Base;
using Management.Models;
using Management.Services;
using Util.Ajax;

namespace Management.Controllers
{
	public class ControllerActionController : BaseController
	{
		#region Properties

		private ControllerActionService _controllerActionSvc;
		public ControllerActionService ControllerActionSvc
		{
			get { return _controllerActionSvc ?? (_controllerActionSvc = new ControllerActionService()); }
			set { _controllerActionSvc = value; }
		}

		#endregion

		#region Actions

		//
		// GET: /ControllerAction/
		[HttpGet]
		public override ActionResult Index()
		{
			return View();
		}

		//
		// Get: /ControllerAction/
		[HttpGet]
		public ActionResult ListController()
		{
			return View();
		}

		[HttpPost]
		public JsonResult ListControllerCore(int page, int rows)
		{
			int from = (page - 1) * rows;
			int to = page * rows - 1;

			var controllers = ControllerActionSvc.GetControllersByRange(from, to);
			var result = new Dictionary<string, object> { { "rows", controllers }, { "total", ControllerActionSvc.GetControllerCount() } };

			return Json(result);
		}

		[HttpGet]
		public ActionResult CreateController()
		{
			ViewBag.Title = "新增控制器";
			ViewBag.FormAction = "CreateController";
			return View("ControllerEdit", new ControllerEditVM());
		}

		[HttpPost]
		public ActionResult CreateController(ControllerEditVM vm)
		{
			if (ModelState.IsValid)
			{
				var result = ControllerActionSvc.CreateController(vm);
				return MyAjaxHelper.RedirectAjax(result, "/ControllerAction/ListController");
			}

			var error = ModelState.Values.First(o => o.Errors.Count > 0).Errors[0].ErrorMessage;
			return MyAjaxHelper.RedirectAjax(AjaxStatusCode.Error, error, null, "");
		}

		[HttpGet]
		public ActionResult UpdateController(int id)
		{
			var controller = ControllerActionSvc.GetControllerById(id);
			ViewBag.Title = "修改控制器";
			ViewBag.FormAction = "UpdateController";
			return View("ControllerEdit", controller);
		}

		[HttpPost]
		public ActionResult UpdateController(ControllerEditVM vm)
		{
			if (ModelState.IsValid)
			{
				var result = ControllerActionSvc.UpdateController(vm);
				return MyAjaxHelper.RedirectAjax(result, "/ControllerAction/ListController");
			}

			var error = ModelState.Values.First(o => o.Errors.Count > 0).Errors[0].ErrorMessage;
			return MyAjaxHelper.RedirectAjax(AjaxStatusCode.Error, error, null, "");
		}

		[HttpDelete]
		public ActionResult DeleteController(int id)
		{
			var result = ControllerActionSvc.DeleteController(id);
			return MyAjaxHelper.RedirectAjax(result, "/ControllerAction/ListController");
		}

		[HttpGet]
		public ActionResult CreateAction()
		{
			ViewBag.Title = "新增操作";
			ViewBag.FormAction = "CreateAction";

			ViewBag.ControllerList = ControllerActionSvc.GetAllControllerAsSelectList();

			return View("ActionEdit", new ActionEditVM());
		}

		[HttpPost]
		public ActionResult CreateAction(ActionEditVM vm)
		{
			if (ModelState.IsValid)
			{
				var result = ControllerActionSvc.CreateAction(vm);
				return MyAjaxHelper.RedirectAjax(result, "/ControllerAction/ListController");
			}

			var error = ModelState.Values.First(o => o.Errors.Count > 0).Errors[0].ErrorMessage;
			return MyAjaxHelper.RedirectAjax(AjaxStatusCode.Error, error, null, "");
		}

		[HttpGet]
		public ActionResult UpdateAction(int id)
		{
			var action = ControllerActionSvc.GetActionById(id);
			ViewBag.ControllerList = ControllerActionSvc.GetAllControllerAsSelectList();
			ViewBag.Title = "修改操作";
			ViewBag.FormAction = "UpdateAction";
			return View("ActionEdit", action);

		}

		[HttpPost]
		public ActionResult UpdateAction(ActionEditVM vm)
		{
			if (ModelState.IsValid)
			{
				var result = ControllerActionSvc.UpdateAction(vm);
				return MyAjaxHelper.RedirectAjax(result, "/ControllerAction/ListController");
			}

			var error = ModelState.Values.First(o => o.Errors.Count > 0).Errors[0].ErrorMessage;
			return MyAjaxHelper.RedirectAjax(AjaxStatusCode.Error, error, null, "");
		}

		[HttpDelete]
		public ActionResult DeleteAction(int id)
		{
			var result = ControllerActionSvc.DeleteAction(id);
			return MyAjaxHelper.RedirectAjax(result, "/ControllerAction/ListController");
		}

		#endregion
		
	}
}
