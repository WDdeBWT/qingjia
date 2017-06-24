using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace qingjia_MVC.Controllers
{
    public class BaseApiController : Controller
    {
        // GET: BaseApi
        public ActionResult Index()
        {
            return View();
        }
    }
}