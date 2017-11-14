using Qiniu.IO;
using Qiniu.IO.Model;
using Qiniu.Util;
using System;
using System.IO;

namespace qingjia_MVC.Common
{
    public class UpLoadQiNiu
    {
        /// <summary>
        /// 上传（来自网络回复的）数据流
        /// </summary>
        public static void UploadStream(FileStream _stream)
        {
            string AccessKey = "gDqJ4t4CWoYTsqtcFzV5BuG_NokUq62AL8mP4XCr";
            string SecretKey = "p0J2qiI3d0lHH0jxEF0Cf3h_OiLoLgvrC1-3f7BT";

            // 生成(上传)凭证时需要使用此Mac
            // 这个示例单独使用了一个Settings类，其中包含AccessKey和SecretKey
            // 实际应用中，请自行设置您的AccessKey和SecretKey
            Mac mac = new Mac(AccessKey, SecretKey);
            string bucket = "qingjia";
            string saveKey = "xuejing-001.jpg";

            // 上传策略，参见 
            // https://developer.qiniu.com/kodo/manual/put-policy
            PutPolicy putPolicy = new PutPolicy();


            // 如果需要设置为"覆盖"上传(如果云端已有同名文件则覆盖)，请使用 SCOPE = "BUCKET:KEY"
            putPolicy.Scope = bucket + ":" + saveKey;
            //putPolicy.Scope = bucket;


            // 上传策略有效期(对应于生成的凭证的有效期)          
            putPolicy.SetExpires(3600);


            // 上传到云端多少天后自动删除该文件，如果不设置（即保持默认默认）则不删除
            putPolicy.DeleteAfterDays = 1;


            // 生成上传凭证，参见
            // https://developer.qiniu.com/kodo/manual/upload-token
            string jstr = putPolicy.ToJsonString();
            string token = Auth.CreateUploadToken(mac, jstr);
            try
            {
                string url = "http://img.ivsky.com/img/tupian/pre/201610/09/beifang_shanlin_xuejing-001.jpg";
                var wReq = System.Net.WebRequest.Create(url) as System.Net.HttpWebRequest;
                var resp = wReq.GetResponse() as System.Net.HttpWebResponse;
                //using (var stream = resp.GetResponseStream())
                using (var stream = _stream)
                {
                    // 请不要使用UploadManager的UploadStream方法，因为此流不支持查找(无法获取Stream.Length)
                    // 请使用FormUploader或者ResumableUploader的UploadStream方法
                    FormUploader fu = new FormUploader();
                    var result = fu.UploadStream(stream, "xuejing-001.jpg", token);

                    Console.WriteLine(result);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}