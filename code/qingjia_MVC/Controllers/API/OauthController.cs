using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace qingjia_MVC.Controllers
{
    [RoutePrefix("api/oauth")]
    public class OauthController : ApiController
    {
        [HttpGet]
        public bool Authorize()
        {
            return true;
        }

        [HttpGet, Route("GetValue")]
        public int value()
        {
            return 2;
        }

        [HttpGet, Route("GetValue")]
        public int value(string code)
        {
            if (code != null && code == "2")
            {
                return 1;
            }
            return 2;
        }
    }
}
