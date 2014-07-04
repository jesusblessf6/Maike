using System.Web.Mvc;
using Enums;
using Management.Controllers.Base;
using Management.Models;
using Management.Services;
using System.Collections.Generic;
using System.Linq;
using Util.Ajax;

namespace Management.Controllers
{
	public class CompanyController : BaseController
	{

		#region Properties

		private CompanyService _companySvc;
		public CompanyService CompanySvc
		{
			get { return _companySvc ?? (_companySvc = new CompanyService()); }
			set { _companySvc = value; }
		}

		#endregion

		#region Actions

		//
		// GET: /Company/
		[HttpGet]
		public override ActionResult Index()
		{
			ViewBag.Title = "客户公司";
			ViewBag.Type = (int)CustomerType.Customer;
			return View();
		}

		public ActionResult Internal()
		{
			ViewBag.Title = "内部公司";
			ViewBag.Type = (int)CustomerType.Internal;
			return View("Index");
		}

		public ActionResult Create(int id)
		{
			ViewBag.Title = id == (int)CustomerType.Customer ? "客户公司" : "内部公司";
			ViewBag.Id = 0;
			var company = new CompanyVM { Type = id };
			return View("CompanyInfo", company);
		}

		[HttpPost]
		public ActionResult Create(CompanyVM company)
		{
			if (ModelState.IsValid)
			{
				var result = CompanySvc.Create(company);
				return MyAjaxHelper.RedirectAjax(result, "");
			}

			var error = ModelState.Values.First(o => o.Errors.Count > 0).Errors[0].ErrorMessage;
			return MyAjaxHelper.RedirectAjax(AjaxStatusCode.Error, error, null, "");
		}

		public ActionResult Update(int id)
		{
			CompanyVM res = CompanySvc.GetById(id);
			if (res != null)
			{
				ViewBag.Id = id;
				ViewBag.Title = res.Type == (int)CustomerType.Customer ? "客户公司" : "内部公司";
				return View("CompanyInfo", res);
			}

			return null;
		}

		[HttpPost]
		public ActionResult Update(CompanyVM company)
		{
			if (ModelState.IsValid)
			{
				var result = CompanySvc.Update(company);
				return MyAjaxHelper.RedirectAjax(result, null);
			}

			var error = ModelState.Values.First(o => o.Errors.Count > 0).Errors[0].ErrorMessage;
			return MyAjaxHelper.RedirectAjax(AjaxStatusCode.Error, error, null, "");
		}

		[HttpDelete]
		public ActionResult Delete(int id)
		{
			
			var errorcode = CompanySvc.Delete(id);
			return MyAjaxHelper.RedirectAjax(errorcode, null);
		}

		[HttpPost]
		public ActionResult GetCompanys(FormCollection form)
		{
			string key = form["key"];//查询关键字
			int type = int.Parse(form["customerType"]);//公司类型
			int start = int.Parse(form["start"]);//起始条数
			int length = int.Parse(form["length"]);//获取条数

			int from = start;
			int to = from + length - 1;

			var data = CompanySvc.GetCompanyByRange(from, to, key.Trim(), type);
			var allCount = CompanySvc.GetAllCount(type, key);
			var result = new Dictionary<string, object>
							 {
								 {"aaData", data},
								 {"iTotalRecords", data.Count},
								 {"iTotalDisplayRecords", allCount}
							 };

			return Json(result);
		}

		[HttpPost]
		public ActionResult GetCompany(string q, int page_limit, int page, int companyType)
		{
			int from = (page - 1) * page_limit;
			int to = from + page_limit - 1;
			var company = CompanySvc.GetCompanyByRange(from, to, q, companyType).Select(o => new Dictionary<string, object> { 
				{"id", o.Id},
				{"text", o.Name}
			}).ToList();
			int count = CompanySvc.GetAllCount(companyType, q);
			var result = new Dictionary<string, object>{
				{"total",count},
				{"company",company}
			};
			return Json(result);
		}

		[HttpPost]
		public ActionResult GetCompanyByUser(string q, int page_limit, int page, int companyType)
		{
			var user = Session["CurrentUser"] as CurrentUserVM;
			int from = (page - 1) * page_limit;
			int to = from + page_limit - 1;
			var company = CompanySvc.GetCompanyByRangeByUser(from, to, q, companyType, user.Id).Select(o => new Dictionary<string, object> { 
				{"id", o.Id},
				{"text", o.Name}
			}).ToList();
			int count = CompanySvc.GetAllCountByUser(companyType, q, user.Id);
			var result = new Dictionary<string, object>{
				{"total",count},
				{"company",company}
			};
			return Json(result);
		}

		#endregion

		#region Remote Vlidations

		[HttpPost]
		public JsonResult ValidateName(string name, int type, int id)
		{
			var result = new Dictionary<string, object>
							 {
								 {"valid", false}
							 };

			result["valid"] = CompanySvc.NameExisted(name, type, id);
			return Json(result);
		}

		[HttpPost]
		public JsonResult ValidateFullName(string fullName, int type, int id)
		{
			var result = new Dictionary<string, object>
							 {
								 {"valid", false}
							 };

			result["valid"] = CompanySvc.FullNameExisted(fullName, type, id);
			return Json(result);
		}
		#endregion
	}
}
