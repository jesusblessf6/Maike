using System.Web.Mvc;
using Enums;

namespace Util.Ajax
{
	public class MyAjaxHelper
	{
		public static ActionResult RedirectAjax(AjaxStatusCode status, string msg, object data, string backurl)
		{
			var ajax = new AjaxMsgModel
			{
				Status = status,
				Msg = msg,
				Data = data,
				BackUrl = backurl
			};
			var res = new JsonResult { Data = ajax, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
			return res;
		}

		public static ActionResult RedirectAjax(ErrorCode result, string successUrl, string failUrl = "", bool showMsg = true)
		{
			AjaxStatusCode status = result == ErrorCode.NoError ? AjaxStatusCode.Success : AjaxStatusCode.Error;
			string targetAction = result == ErrorCode.NoError ? successUrl : failUrl;
			string message = string.Empty;
			if (showMsg)
				message = EnumHelper.GetDescription(result);
			return RedirectAjax(status, message, null, targetAction);
		}
	}
}
