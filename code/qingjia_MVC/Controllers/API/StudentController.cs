using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using qingjia_MVC.Models;
using qingjia_MVC.Common;

namespace qingjia_MVC.Controllers.API
{
    #region 数据模型

    public class NightInfoModel
    {
        public string TeacherID { get; set; }
        public string TeacherName { get; set; }
        public string BatchTime { get; set; }
        public string DeadLine { get; set; }
    }

    public class HolidayModel
    {
        public string TeacherID { get; set; }
        public string DeadLine { get; set; }
    }

    #endregion

    [RoutePrefix("api/student")]
    public class StudentController : ApiController
    {
        private imaw_qingjiaEntities db = new imaw_qingjiaEntities();

        /// <summary>
        /// GET
        /// 
        /// 获取学生基本信息
        /// </summary>
        /// <param name="access_token"></param>
        /// <returns></returns>
        [HttpGet, Route("me")]
        public ApiBaseResult Me(string access_token)
        {
            ApiBaseResult result = Check(access_token);
            if (result == null)
            {
                result = new ApiBaseResult();

                #region 获取用户信息

                string UserID = access_token.Substring(0, access_token.IndexOf("_"));

                var studentInfo = from vw_Student in db.vw_Student where (vw_Student.ST_Num == UserID) select vw_Student;
                if (studentInfo.Any())
                {
                    vw_Student student = studentInfo.ToList().First();
                    result.result = "success";
                    result.data = student;
                }
                else
                {
                    result.result = "error";
                    result.messages = "出现未知错误、请联系管理员";
                }

                #endregion

                return result;
            }
            else
            {
                return result;
            }
        }

        /// <summary>
        /// GET
        /// 
        /// 获取学生晚点名信息
        /// </summary>
        /// <param name="access_token"></param>
        /// <returns></returns>
        [HttpGet, Route("night")]
        public ApiBaseResult Nigth(string access_token)
        {
            ApiBaseResult result = Check(access_token);
            if (result == null)
            {
                result = new ApiBaseResult();

                #region 获取数据

                string UserID = access_token.Substring(0, access_token.IndexOf("_"));

                NightInfoModel nightInfo = new NightInfoModel();

                var studentInfo = from vw_Student in db.vw_Student where (vw_Student.ST_Num == UserID) select vw_Student;
                if (studentInfo.Any())
                {
                    string className = studentInfo.ToList().First().ST_Class;
                    string TeacherID = studentInfo.ToList().First().ST_TeacherID;
                    nightInfo.TeacherID = TeacherID;
                    nightInfo.TeacherName = studentInfo.ToList().First().ST_Teacher;

                    var batchList = from vw_ClassBatch in db.vw_ClassBatch where (vw_ClassBatch.ClassName == className) select vw_ClassBatch;
                    if (batchList.Any())
                    {
                        nightInfo.BatchTime = batchList.ToList().First().Datetime.ToString();
                    }
                    else
                    {
                        result.result = "error";
                        result.messages = "未设置晚点名时间！";
                    }

                    T_Deadline deadLine = new T_Deadline();
                    try
                    {
                        deadLine = (from T_Deadline in db.T_Deadline where ((T_Deadline.TeacherID == TeacherID) && (T_Deadline.TypeID == 2)) select T_Deadline).ToList().First();
                        nightInfo.DeadLine = deadLine.Time.ToString();
                        result.result = "success";
                        result.data = nightInfo;
                    }
                    catch
                    {
                        result.result = "error";
                        result.messages = "未设置晚点名请假截止时间";
                    }
                }
                else
                {
                    result.result = "error";
                    result.messages = "出现未知错误、请联系管理员";
                }

                #endregion

                return result;
            }
            else
            {
                return result;
            }
        }

        /// <summary>
        /// GET
        /// 
        /// 获取学生节假日信息
        /// </summary>
        /// <param name="access_token"></param>
        /// <returns></returns>
        [HttpGet, Route("holiday")]
        public ApiBaseResult Holiaday(string access_token)
        {
            ApiBaseResult result = Check(access_token);
            if (result == null)
            {
                result = new ApiBaseResult();

                #region 获取数据

                HolidayModel holiday = new HolidayModel();

                string UserID = access_token.Substring(0, access_token.IndexOf("_"));
                var studentInfo = from vw_Student in db.vw_Student where (vw_Student.ST_Num == UserID) select vw_Student;
                if (studentInfo.Any())
                {
                    string TeacherID = studentInfo.ToList().First().ST_TeacherID;
                    holiday.TeacherID = TeacherID;

                    var deadLineList = from T_Deadline in db.T_Deadline where ((T_Deadline.TeacherID == TeacherID) && (T_Deadline.TypeID == 1)) select T_Deadline;
                    if (deadLineList.Any())
                    {
                        holiday.DeadLine = deadLineList.ToList().First().Time.ToString();
                        result.result = "success";
                        result.data = holiday;
                    }
                    else
                    {
                        result.result = "error";
                        result.messages = "未设置节假日请假截止时间，请联系辅导员";
                    }
                }
                else
                {
                    result.result = "error";
                    result.messages = "出现未知错误、请联系管理员";
                }

                #endregion

                return result;
            }
            else
            {
                return result;
            }
        }

        /// <summary>
        /// POST
        /// 
        /// 学生修改密码
        /// </summary>
        /// <param name="old_psd"></param>
        /// <param name="new_psd"></param>
        /// <param name="access_token"></param>
        /// <returns></returns>
        [HttpPost, Route("password")]
        public ApiBaseResult PassWord(string old_psd, string new_psd, string access_token)
        {
            ApiBaseResult result = Check(access_token);
            if (result == null)
            {
                result = new ApiBaseResult();

                string StudentID = access_token.Substring(0, access_token.IndexOf("_"));
                T_Account account = db.T_Account.Find(StudentID);
                if (account.Psd == old_psd)
                {
                    account.Psd = new_psd;
                    db.SaveChanges();

                    result.result = "success";
                }
                else
                {
                    result.result = "error";
                    result.messages = "原密码错误，修改密码失败";
                }
                return result;
            }
            else
            {
                return result;
            }
        }

        /// <summary>
        /// 检查Access_Token
        /// </summary>
        /// <param name="access_token"></param>
        /// <returns></returns>
        public ApiBaseResult Check(string access_token)
        {
            ApiBaseResult result = new ApiBaseResult();

            if (access_token != null)
            {
                string[] sArray = access_token.Split('_');
                string UserID = sArray[0];
                string GuidString = sArray[1];

                T_Account account = db.T_Account.Find(UserID);
                if (account != null)
                {
                    if (account.YB_AccessToken == GuidString)
                    {
                        return null;
                    }
                    else
                    {
                        result.result = "error";
                        result.messages = "Access_Token错误";
                        return result;
                    }
                }
                else
                {
                    result.result = "error";
                    result.messages = "Access_Token错误";
                    return result;
                }
            }
            else
            {
                result.result = "error";
                result.messages = "Access_Token为空，传值错误";
                return result;
            }
        }

    }
}
