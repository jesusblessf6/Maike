using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Front.Controllers.Base;
using Front.Models;
using Front.Services;
using Util;
using Util.Ajax;

namespace Front.Controllers
{
	public class ResetPasswordController : BaseController
	{
		#region Properties

		private UserService _userSvc;

		public UserService UserSvc
		{
			get { return _userSvc ?? (_userSvc = new UserService()); }
			set { _userSvc = value; }
		}

		#endregion


		#region Remote Vlidations

		[HttpPost]
		public JsonResult ValidatePassword(string OldPsw, int id)
		{
			var result = new Dictionary<string, object>
							 {
								 {"valid", false}
							 };

			//result["valid"] = UserSvc.GetById(userid); // CommodityTypeSvc.GetNameExisted(name, id, commodityId);
			result["valid"] = UserSvc.PasswordIsValid(OldPsw, id); // CommodityTypeSvc.GetNameExisted(name, id, commodityId);

			return Json(result);
		}

		#endregion

		//
		// GET: /ResetPassword/
		public override ActionResult Index()
		{
			var userid = int.Parse(Session["CurrentUserId"].ToString());
			var rpw = new ResetPasswordVM
						  {
							  UserId = userid
						  };
			return View(rpw);
		}

		[HttpPost]
		public ActionResult ResetPassword(ResetPasswordVM psw)
		{
			var errorCode = ErrorCode.NoError;
			ModelState.Remove("UserId");
			psw.UserId = int.Parse(Session["CurrentUserId"].ToString());
			if (ModelState.IsValid)
			{
				errorCode = UserSvc.ResetPassWord(psw);
				if (errorCode == ErrorCode.PasswordNotCorrect)
				{
					//MyAjaxHelper.RedirectAjax(errorCode, null);
					//return Redirect("/ResetPassword/Index");
					//return MyAjaxHelper.RedirectAjax(errorCode, "/ResetPassword/Index", "/ResetPassword/Index");
					return View("index");
				}
				else if (errorCode == ErrorCode.NoError)
				{

					return MyAjaxHelper.RedirectAjax(errorCode, "/Home/Index", "");
				}
				else
					return View();
			}
			else
			{
				var error = ModelState.Values.First(o => o.Errors.Count > 0).Errors[0].ErrorMessage;
				return MyAjaxHelper.RedirectAjax(AjaxStatusCode.Error, error, null, "");
			}
		}

	}
}
