using System.Linq;
using System.Web.Mvc;
using Front.Models;
using Front.Services;
using Util;
using Util.Ajax;

namespace Front.Controllers
{
	public class SignController : Controller
	{
		#region Properties

		private SignService _signSvc;
		public SignService SignSvc {
			get { return (_signSvc ?? new SignService()); }
			set { _signSvc = value; }
		}

		#endregion

		// Open Sign-in Page
		// GET: /Sign/
		[HttpGet]
		public ActionResult Index()
		{
			return View();
		}

		// Open Sign-In page, same as Index
		// GET: /Sign/In
		[HttpGet]
		public ActionResult In()
		{
			return View("Index");
		}

		// Post to Sign in
		// POST: /Sign/In/{SignInVM}
		[HttpPost]
		public ActionResult In(SignInVM signInVM)
		{
			if (ModelState.IsValid)
			{
				int userId;
				var errorCode = SignSvc.Login(signInVM.LoginName, signInVM.Password, out userId);

				if (errorCode == ErrorCode.NoError)
				{
					Session["CurrentUserId"] = userId;
					return MyAjaxHelper.RedirectAjax(errorCode, "/Home/Index", "", false);
				}
				else
				{
					return MyAjaxHelper.RedirectAjax(errorCode, "/Home/Index", "");
				}
			}
			else
			{
				var error = ModelState.Values.First(o => o.Errors.Count > 0).Errors[0].ErrorMessage;
				return MyAjaxHelper.RedirectAjax(AjaxStatusCode.Error, error, null, "");
			}
		}

		//Sign out
		// Get: /Sign/Out
		[HttpGet]
		public ActionResult Out()
		{
			Session.Clear();
			return Redirect("/Sign/In");
		}

		//Resign in
		//Get: /Sign/Re
		[HttpGet]
		public ActionResult Re()
		{
			Session.Clear();
			return View();
		}
	}
}
