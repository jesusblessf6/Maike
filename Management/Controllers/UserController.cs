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
	public class UserController : BaseController
	{
		#region Properties

		private UserService _userSvc;
		public UserService UserSvc
		{
			get { return _userSvc ?? (_userSvc = new UserService()); }
			set { _userSvc = value; }
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

		#endregion

		#region Actions

		//
		// GET: /Company/
		[HttpGet]
		public override ActionResult Index()
		{
			ViewBag.Title = "客户管理";
			ViewBag.Type = (int)CustomerType.Customer;
			return View();
		}

		public ActionResult Internal()
		{
			ViewBag.Title = "用户管理";
			ViewBag.Type = (int)CustomerType.Internal;
			return View("Index");
		}

		public ActionResult Create(int id)
		{
			ViewBag.Title = id == (int)CustomerType.Customer ? "新增客户" : "新增用户";
			ViewBag.Id = 0;
			var listCommodity = GetAllCommodity();
			var listCompany = GetAllCompany(id);
			ViewBag.Commodity = listCommodity;
			ViewBag.Company = listCompany;
			ViewBag.UserType = id;
			var allRoles = (new RoleService()).GetAllRoles();
			string roles = "";
			foreach (var role in allRoles)
			{
				roles += role.Id + "," + role.Name + "||";
			}
			if (roles.Length > 0)
				roles = roles.Remove(roles.Length - 2);
			ViewBag.AllRoles = roles;
			var user = new UserVM { Type = id, SelectCommodityIds = "", SelectCompanyIds = "" };
			return View("UserInfo", user);
		}

		public ActionResult Edit(int id)
		{
			UserVM res = UserSvc.GetById(id);
			if (res != null)
			{
				ViewBag.Id = id;
				ViewBag.Title = res.Type == (int)CustomerType.Customer ? "编辑客户" : "编辑用户";
				var listCommodity = GetAllCommodity();
				var listCompany = GetAllCompany(res.Type);
				ViewBag.Commodity = listCommodity;
				ViewBag.Company = listCompany;
				ViewBag.UserType = res.Type;
				var allRoles = (new RoleService()).GetAllRoles();
				string roles = "";
				foreach (var role in allRoles)
				{
					roles += role.Id + "," + role.Name + "||";
				}
				if (roles.Length > 0)
					roles = roles.Remove(roles.Length - 2);
				ViewBag.AllRoles = roles;
				return View("UserInfo", res);
			}

			return null;
		}

		[HttpPost]
		public ActionResult Create(UserVM vm)
		{
			if (ModelState.IsValid)
			{
				var result = UserSvc.Create(vm);
				return MyAjaxHelper.RedirectAjax(result, "");
			}
			var error = ModelState.Values.First(o => o.Errors.Count > 0).Errors[0].ErrorMessage;
			return MyAjaxHelper.RedirectAjax(AjaxStatusCode.Error, error, null, "");
		}

		public ActionResult Update(UserVM vm)
		{
			if (ModelState.IsValid)
			{
				var result = UserSvc.Update(vm);
				return MyAjaxHelper.RedirectAjax(result, "");
			}
			var error = ModelState.Values.First(o => o.Errors.Count > 0).Errors[0].ErrorMessage;
			return MyAjaxHelper.RedirectAjax(AjaxStatusCode.Error, error, null, "");
		}

		public ActionResult GetUsers(int start, int length, int type, int companyId, string userName)
		{
			int from = start;
			int to = from + length - 1;

			var data = UserSvc.GetUsersByRange(from, to, userName, companyId, type);
			var allCount = UserSvc.GetUserCount(userName, companyId, type);
			var result = new Dictionary<string, object>
							 {
								 {"aaData", data},
								 {"iTotalRecords", data.Count},
								 {"iTotalDisplayRecords", allCount}
							 };

			return Json(result);
		}


		private List<CommodityViewVM> GetAllCommodity()
		{
			return CommoditySvc.GetAllCommodities();
		}

		private List<CompanyVM> GetAllCompany(int type)
		{
			return CompanySvc.GetAllCompany(type);
		}

		[HttpPost]
		public JsonResult ValidateLoginName(string loginName, int id)
		{
			var result = new Dictionary<string, object>
							 {
								 {"valid", false}
							 };

			result["valid"] = UserSvc.LoginNameExisted(loginName, id);
			return Json(result);
		}

		[HttpPost]
		public JsonResult ValidateCompany(string selectCompanyIds)
		{
			var result = new Dictionary<string, object>
							 {
								 {"valid", false}
							 };
			if (!string.IsNullOrWhiteSpace(selectCompanyIds))
				result["valid"] = true;
			return Json(result);
		}

		[HttpPost]
		public JsonResult ValidateCommodity(string selectCommodityIds)
		{
			var result = new Dictionary<string, object>
							 {
								 {"valid", false}
							 };
			if (!string.IsNullOrWhiteSpace(selectCommodityIds))
				result["valid"] = true;
			return Json(result);
		}

		[HttpPost]
		public ActionResult ResetPWD(int id)
		{
			var result = UserSvc.ResetPwd(id);
			return MyAjaxHelper.RedirectAjax(result, "");
		}

		[HttpDelete]
		public ActionResult Delete(int id)
		{
			var errorcode = UserSvc.Delete(id);
			return MyAjaxHelper.RedirectAjax(errorcode, null);
		}
		#endregion
	}
}
