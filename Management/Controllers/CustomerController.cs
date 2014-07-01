using Entities.Enums;
using Management.Controllers.Base;
using Management.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Management.Controllers
{
    public class CustomerController : BaseController
    {
        //
        // GET: /Customer/

        public override ActionResult Index()
        {
            
            return View();
        }
    }
}
