using Front.Controllers.Base;
using Front.Services;
using System.Web.Mvc;

namespace Front.Controllers
{
    public class HomeController : BaseController
    {
        public override ActionResult Index()
        {
            var homeSvc = new HomeService();
            var userId = int.Parse(Session["CurrentUserId"].ToString());
            var currentUser = homeSvc.GetCurrentUser(userId);

            Session["CurrentUser"] = currentUser;
            ViewBag.CurrentUser = currentUser;
            return View();
        }
    }
}
