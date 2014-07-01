using System;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using Management.App_Start;
using Management.Controllers;

namespace Management
{
	// 注意: 有关启用 IIS6 或 IIS7 经典模式的说明，
	// 请访问 http://go.microsoft.com/?LinkId=9394801
	public class MvcApplication : HttpApplication
	{
		protected void Application_Start()
		{
			AreaRegistration.RegisterAllAreas();

			WebApiConfig.Register(GlobalConfiguration.Configuration);
			FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
			RouteConfig.RegisterRoutes(RouteTable.Routes);
		}

		protected void Application_Error(object sender, EventArgs e)
		{
			var httpContext = ((MvcApplication)sender).Context;

			var currentRouteData = RouteTable.Routes.GetRouteData(new HttpContextWrapper(httpContext));
			var currentController = " ";
			var currentAction = " ";

			if (currentRouteData != null)
			{
				if (currentRouteData.Values["controller"] != null && !String.IsNullOrEmpty(currentRouteData.Values["controller"].ToString()))
				{
					currentController = currentRouteData.Values["controller"].ToString();
				}

				if (currentRouteData.Values["action"] != null && !String.IsNullOrEmpty(currentRouteData.Values["action"].ToString()))
				{
					currentAction = currentRouteData.Values["action"].ToString();
				}
			}

			var ex = Server.GetLastError();

			var controller = new ErrorController();
			var routeData = new RouteData();
			var action = "Index";

			if (ex is HttpException)
			{
				var httpEx = ex as HttpException;

				switch (httpEx.GetHttpCode())
				{
					case 404:
						action = "NotFound";
						break;

					// others if any
					case 401:
					case 403:
						action = "NoAuthor";
						break;

					default:
						action = "Index";
						break;
				}
			}

			httpContext.ClearError();
			httpContext.Response.Clear();
			httpContext.Response.StatusCode = ex is HttpException ? ((HttpException)ex).GetHttpCode() : 500;
			httpContext.Response.TrySkipIisCustomErrors = true;
			routeData.Values["controller"] = "Error";
			routeData.Values["action"] = action;


			controller.ViewData.Model = new HandleErrorInfo(ex, currentController, currentAction);
			((IController)controller).Execute(new RequestContext(new HttpContextWrapper(httpContext), routeData));
		}
	}
}