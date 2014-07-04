using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DAL;
using Enums;

namespace AOP
{
	public class MyActionCheck : ActionFilterAttribute
	{
		public override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			if (filterContext.HttpContext.Session == null ||
			    filterContext.HttpContext.Session["CurrentUserId"] == null)
			{
				filterContext.HttpContext.Response.Redirect("/Sign/Re");
			}
			else
			{
				int userId;
				if (!int.TryParse(filterContext.HttpContext.Session["CurrentUserId"].ToString(), out userId))
				{
					filterContext.HttpContext.Response.Redirect("/Sign/Re");
				}

				var ud = new UserDAL();
				var user = ud.GetById(userId, null);
				if (user.IsSuper)
				{
					return;
				}

				var controllerName = filterContext.RouteData.Values["controller"] as string;
				var actionName = filterContext.RouteData.Values["action"] as string;

				//controller level
				var cd = new ControllerDAL();
				var c = cd.Query(o => o.Name == controllerName).Single();
				if (c == null)
				{
					//throw new HttpException(403, "Forbidden Access. controller: " + controllerName + ", action: " + actionName + ".");
					filterContext.HttpContext.Response.Redirect("/Error/NoAuthor");
				}
				if (c.ForAll)
				{
					return;
				}

				//action level
				var ad = new ActionDAL();
				//var a = ad.GetByName(controllerName, actionName);
				var a = ad.Query(o => o.Controller.Name == controllerName && o.Name == actionName, new List<string>{"Controller"}).Single();

				if (a == null)
				{
					//throw new HttpException(403, "Forbidden Access. controller: " + controllerName + ", action: " + actionName + ".");
					filterContext.HttpContext.Response.Redirect("/Error/NoAuthor");
				}
				if (a.ForAll)
				{
					return;
				}

				//user level
				if (user.RoleId == null)
				{
					//throw new HttpException(403, "Forbidden Access. controller: " + controllerName + ", action: " + actionName + ". 当前用户未分配角色！");
					filterContext.HttpContext.Response.Redirect("/Error/NoAuthor");
				}

				var pl =
					ud.GetById(userId,
					           new List<string> {"Role", "Role.Previleges", "Role.Previleges.Controller", "Role.Previleges.Action"})
					  .Role.Previleges.Where(o => !o.IsDeleted).ToList();

				if (pl.Any(o => o.Controller.Name == controllerName && o.PrevilegeLevel == (int)PrevilegeLevel.ControllerLevel))
				{
					return;
				}

				if (
					pl.Any(
						o =>
						o.Controller.Name == controllerName && o.Action.Name == actionName &&
						o.PrevilegeLevel == (int)PrevilegeLevel.ActionLevel))
				{
					return;
				}

				//throw new HttpException(403, "Forbidden Access. controller: " + controllerName + ", action: " + actionName + ".");
				filterContext.HttpContext.Response.Redirect("/Error/NoAuthor");
			}
		}
	}
}
