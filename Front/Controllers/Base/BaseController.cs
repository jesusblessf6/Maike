﻿using AOP;
using Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Util;
using Util.Ajax;

namespace Front.Controllers.Base
{
    [MyActionCheck]
    public abstract class BaseController : Controller
    {
        public virtual ActionResult Index()
        {
            return View();
        }

        public virtual ActionResult AjaxRedirect(ErrorCode result, string successUrl, string failUrl = "")
        {
            AjaxStatusCode status = result == ErrorCode.NoError ? AjaxStatusCode.Success : AjaxStatusCode.Error;
            string targetAction = result == ErrorCode.NoError ? successUrl : failUrl;
            string message = EnumHelper.GetDescription(result);
            return MyAjaxHelper.RedirectAjax(status, message, null, targetAction);
        }
    }
}