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
    #region 数据模型

    public class User_Login
    {
        public string UserID { get; set; }
        public string UserPsd { get; set; }
        public string YiBanID { get; set; }
    }

    public class AuthorizeModel
    {
        public string access_token { get; set; }
    }

    #endregion

    [RoutePrefix("api/oauth")]
    public class OauthController : ApiController
    {
        //实例化数据库
        private imaw_qingjiaEntities db = new imaw_qingjiaEntities();

        /// <summary>
        /// POST
        /// 
        /// 获取授权 用户ID 用户Psd 易班用户ID  返回给用户一个GUID当做AccessToken
        /// </summary>
        /// <param name="user_login_info"></param>
        /// <returns></returns>
        [HttpPost, Route("authorize")]
        public ApiBaseResult Authorize([FromBody]User_Login user_login_info)
        {
            ApiBaseResult result = new ApiBaseResult();

            string UserID = "";
            string UserPSd = "";
            string YiBanID = "";

            #region 检查参数是否正确

            if (user_login_info == null)
            {
                //参数错误
                result.result = "error";
                result.messages = "未接收到合法参数！";
                return result;
            }
            else
            {
                //存在合法参数正确
                try
                {
                    UserID = user_login_info.UserID;
                    UserPSd = user_login_info.UserPsd;
                    YiBanID = user_login_info.YiBanID;
                    if (UserID == null || UserPSd == null || YiBanID == null)
                    {
                        result.result = "error";
                        result.messages = "参数格式错误或缺少参数！";
                        return result;
                    }
                }
                catch
                {
                    result.result = "error";
                    result.messages = "参数格式错误或缺少参数！";
                    return result;
                }
            }

            #endregion

            #region 账号绑定

            var accountList = from T_Account in db.T_Account where (T_Account.YiBanID == YiBanID) select T_Account;
            if (accountList.Any())
            {
                //此账号已绑定
                result.result = "error";
                result.messages = "此账号已绑定！";
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

                        AuthorizeModel authorizeModel = new AuthorizeModel();
                        authorizeModel.access_token = access_token;
                        result.result = "success";
                        result.data = authorizeModel;
                    }
                    else
                    {
                        //用户密码错误
                        result.result = "error";
                        result.messages = "账号密码错误！";
                    }
                }
                else
                {
                    //此用户ID不存在
                    result.result = "error";
                    result.messages = "此用户ID不存在！";
                }
            }

            #endregion

            return result;
        }

        /// <summary>
        /// GET
        /// 
        /// 获取AccessToken
        /// </summary>
        /// <param name="YiBanID"></param>
        /// <returns></returns>
        [HttpGet, Route("access_token")]
        public ApiBaseResult Access_Token(string YiBanID)
        {
            ApiBaseResult result = new ApiBaseResult();

            var userList = from T_Account in db.T_Account where (T_Account.YiBanID == YiBanID) select T_Account;
            if (userList.Any())
            {
                //验证通过
                string GuidString = Guid.NewGuid().ToString();
                string access_token = "";
                string UserID = userList.ToList().First().ID;
                T_Account account = db.T_Account.Find(UserID);
                access_token = account.ID + "_" + GuidString;
                account.YiBanID = YiBanID;
                account.YB_AccessToken = GuidString;
                db.SaveChanges();

                AuthorizeModel authorizeModel = new AuthorizeModel();
                authorizeModel.access_token = access_token;
                result.result = "success";
                result.data = authorizeModel;
            }
            else
            {
                //尚未绑定YiBanID
                result.result = "error";
                result.messages = "尚未绑定账号的易班ID，通过Authorize接口实现易班账号绑定。";
            }
            return result;
        }

    }
}
