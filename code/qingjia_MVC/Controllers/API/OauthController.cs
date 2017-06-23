using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using qingjia_MVC.Models;
using qingjia_MVC.Common;

namespace qingjia_MVC.Controllers
{
    public class User_Login
    {
        public string UserID { get; set; }
        public string UserPsd { get; set; }
        public string YiBanID { get; set; }
    }

    public class ApiError
    {

    }

    [RoutePrefix("api/oauth")]
    public class OauthController : ApiController
    {
        //实例化数据库
        private imaw_qingjiaEntities db = new imaw_qingjiaEntities();

        /// <summary>
        /// 获取授权 Post请求 用户ID 用户Psd 易班用户ID
        /// </summary>
        /// <param name="user_login_info"></param>
        /// <returns></returns>
        [HttpPost]
        public bool Authorize([FromBody]User_Login user_login_info)
        {
            string UserID = user_login_info.UserID;
            string UserPSd = user_login_info.UserPsd;
            string YiBanID = user_login_info.YiBanID;

            var accountList = from T_Account in db.T_Account where (T_Account.YiBanID == YiBanID) select T_Account;
            if (accountList.Any())
            {
                //此账号已绑定
            }
            else
            {
                //此账号尚未绑定
                T_Account account = db.T_Account.Find(UserID);
                if (account != null)
                {
                    if (account.Psd == UserPSd)
                    {
                        //验证通过
                        string GuidString = Guid.NewGuid().ToString();
                        string access_token = UserID + "_" + GuidString;
                        account.YiBanID = YiBanID;
                        account.YB_AccessToken = GuidString;
                        db.SaveChanges();
                    }
                    else
                    {
                        //用户密码错误
                    }
                }
                else
                {
                    //此用户ID不存在
                }
            }
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
