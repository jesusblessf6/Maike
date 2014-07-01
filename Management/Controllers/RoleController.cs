using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Enums;
using Management.Controllers.Base;
using Management.Models;
using Management.Services;
using Util.Ajax;

namespace Management.Controllers
{
	public class RoleController : BaseController
	{
		#region Properties

		private RoleService _roleSvc;
		public RoleService RoleSvc
		{
			get { return _roleSvc ?? (_roleSvc = new RoleService()); }
			set { _roleSvc = value; }
		}

		#endregion

		#region Actions

		//
		// GET: /Role/
		[HttpGet]
		public override ActionResult Index()
		{
			return View();
		}

		//
		// Post: /Role/GetRoles
		[HttpPost]
		public JsonResult GetRoles(int start, int length)
		{
			int from = start;
			int to = start + length - 1;

			var data = RoleSvc.GetRolesByRange(from, to);
			var count = RoleSvc.GetAllCount();

			var result = new Dictionary<string, object>
							 {
								 {"aaData", data},
								 {"iTotalRecords", data.Count},
								 {"iTotalDisplayRecords", count}
							 };

			return Json(result);
		}

		[HttpPost]
		public JsonResult GetAllRoles(string q)
		{
			List<RoleVM> allRoles = string.IsNullOrWhiteSpace(q) ? RoleSvc.GetAllRoles() : RoleSvc.SearchRoles(q);

			var result = allRoles.Select(o => new Dictionary<string, object>
												   {
													   {"id", o.Id},
													   {"text", o.Name}
												   }).ToList();
			return Json(result);
		}

		//
		// Get: /Role/Create
		[HttpGet]
		public ActionResult Create()
		{
			ViewBag.Title = "新增角色";
			return View("Edit", new RoleVM());
		}

		[HttpPost]
		public ActionResult Create(RoleVM vm)
		{
			if (ModelState.IsValid)
			{
				var result = RoleSvc.Create(vm);
				return MyAjaxHelper.RedirectAjax(result, "");
			}

			var error = ModelState.Values.First(o => o.Errors.Count > 0).Errors[0].ErrorMessage;
			return MyAjaxHelper.RedirectAjax(AjaxStatusCode.Error, error, null, "");
		}

		//
		[HttpPost]
		public JsonResult NameValidate(string name, int roleId)
		{
			var result = RoleSvc.NameValidate(name, roleId);
			return Json(new Dictionary<string, object>{
														  {
															  "valid", result
														  }});
		}

		[HttpGet]
		public ActionResult Update(int id)
		{
			ViewBag.Title = "修改角色";
			var role = RoleSvc.GetById(id);
			return View("Edit", role);
		}

		[HttpPost]
		public ActionResult Update(RoleVM vm)
		{
			if (ModelState.IsValid)
			{
				var result = RoleSvc.Update(vm);
				return MyAjaxHelper.RedirectAjax(result, "");
			}

			var error = ModelState.Values.First(o => o.Errors.Count > 0).Errors[0].ErrorMessage;
			return MyAjaxHelper.RedirectAjax(AjaxStatusCode.Error, error, null, "");
		}

		[HttpDelete]
		public ActionResult Delete(int id)
		{
			var result = RoleSvc.Delete(id);
			return MyAjaxHelper.RedirectAjax(result, "");
		}

		[HttpGet]
		public ActionResult AssignPerm(int id)
		{
			ViewBag.Title = "编辑权限";
			ViewBag.RoleId = id;
			return View();
		}

		[HttpGet]
		public ActionResult AssignPermCore(int id)
		{
			var role = RoleSvc.GetById(id);
			ViewBag.Title = "编辑角色 "+role.Name+" 的权限";
			return View(role);
		}

		[HttpGet]
		public ActionResult RolePermList(int id)
		{
			var controllers = RoleSvc.GetControllersWithPermInfo(id);
			var result = new Dictionary<string, object> {{"rows", controllers}, {"total", controllers.Count}};
			return Json(result, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		public ActionResult AddPerm(int targetId, int roleId, string type)
		{
			var perm = new PrevilegeVM { RoleId = roleId };
			if (type == "Controller")
			{
				perm.ControllerId = targetId;
				perm.ActionId = null;
				perm.PrevilegeLevel = PrevilegeLevel.ControllerLevel;
			}
			else if (type == "Action")
			{
				var conSvc = new ControllerActionService();
				var action = conSvc.GetActionById(targetId);
				perm.ControllerId = action.ControllerId;
				perm.ActionId = action.Id;
				perm.PrevilegeLevel = PrevilegeLevel.ActionLevel;
			}

			var result = RoleSvc.AddPerm(perm);
			return AjaxRedirect(result, "", "/Role/AssignPerm/" + roleId);
		}

		[HttpPost]
		public ActionResult RemovePerm(int targetId, int roleId, string type)
		{
			var perm = new PrevilegeVM { RoleId = roleId };
			if (type == "Controller")
			{
				perm.ControllerId = targetId;
				perm.ActionId = null;
				perm.PrevilegeLevel = PrevilegeLevel.ControllerLevel;
			}
			else if (type == "Action")
			{
				var conSvc = new ControllerActionService();
				var action = conSvc.GetActionById(targetId);
				perm.ControllerId = action.ControllerId;
				perm.ActionId = action.Id;
				perm.PrevilegeLevel = PrevilegeLevel.ActionLevel;
			}

			var result = RoleSvc.RemovePerm(perm);
			return AjaxRedirect(result, "", "/Role/AssignPerm/" + roleId);
		}

		#endregion
		
	}
}
