using System.Web.Mvc;

namespace Management.Controllers
{
	public class ErrorController : Controller
	{
		//
		// GET: /Error/
		[HttpGet]
		public ActionResult Index()
		{
			return View();
		}

		[HttpGet]
		public ActionResult NotFound()
		{
			return View();
		}

		[HttpGet]
		public ActionResult NoAuthor()
		{
			return View();
		}

	}
}
