using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace qingjia_MVC.Controllers
{
    [RoutePrefix("api/qingjia")]
    public class qingjiaController : ApiController
    {
        [Authorize]
        public bool login()
        {
            return true;
        }

        [HttpGet, Route("GetValue")]
        public int value()
        {
            return 2;
        }
    }
}
