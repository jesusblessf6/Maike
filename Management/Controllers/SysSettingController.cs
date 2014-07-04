using Management.Services;
using System.Linq;
using System.Web.Mvc;
using Management.Controllers.Base;
using Management.Models;
using Util.Ajax;

namespace Management.Controllers
{
	public class SysSettingController : BaseController
	{

		#region Properties

		public SysSettingService SysSettingSvc
		{
			get { return _sysSettingSvc ?? (_sysSettingSvc = new SysSettingService()); }
			set { _sysSettingSvc = value; }
		}
		private SysSettingService _sysSettingSvc;

		#endregion

		#region Actions

		public override ActionResult Index()
		{
			var sysSetting = SysSettingSvc.GetSysSetting();
			return View(sysSetting);
		}

		[HttpPost]
		public ActionResult Update(SysSettingVM sysSetting)
		{
			if (ModelState.IsValid)
			{
				bool result = SysSettingSvc.SaveSysSetting(sysSetting);
				if (result)
				{
					return MyAjaxHelper.RedirectAjax(AjaxStatusCode.Success, "保存成功", null, "");
				}
				
				return MyAjaxHelper.RedirectAjax(AjaxStatusCode.Error, "保存失败",null,string.Empty);
			}
			
			var error = ModelState.Values.First(o => o.Errors.Count > 0).Errors[0].ErrorMessage;
			return MyAjaxHelper.RedirectAjax(AjaxStatusCode.Error, error, null, "");
		}
		#endregion
	}
}
