using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using net91com.Core.Util;

namespace net91com.Reports.UserRights
{

    #region 扩展方法类(ExtensionMethods)

    /// <summary>
    /// 扩展方法类
    /// </summary>
    internal static class ExtensionMethods
    {
        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="sourceStr"></param>
        /// <returns></returns>
        public static string HashDefaultToMD5Hex(this string sourceStr)
        {
            byte[] Bytes = Encoding.UTF8.GetBytes(sourceStr);
            using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
            {
                byte[] result = md5.ComputeHash(Bytes);

                StringBuilder sBuilder = new StringBuilder();
                for (int i = 0; i < result.Length; i++)
                    sBuilder.Append(result[i].ToString("x2"));

                return sBuilder.ToString();
            }
        }
    }

    #endregion

    #region 通用接口返回JSON对应的实体定义

    /// <summary>
    /// 通用接口返回的用户信息
    /// </summary>
    [DataContract]
    internal class ReturnedUserInfo
    {
        /// <summary>
        /// 用户账号
        /// </summary>
        [DataMember]
        public string UserName { get; set; }

        /// <summary>
        /// 接口返回结果码{1-成功,2-服务所需的参数不完整,3-checksum不正确,4-服务出现异常}
        /// </summary>
        [DataMember]
        public int ResultCode { get; set; }

        /// <summary>
        /// 产品列表
        /// </summary>
        [DataMember]
        public AppInfo[] AppList { get; set; }
    }

    /// <summary>
    /// 外部产品(通用过来的产品信息)
    /// </summary>
    [DataContract]
    internal class AppInfo
    {
        /// <summary>
        /// 产品ID
        /// </summary>
        [DataMember]
        public int AppId { get; set; }

        /// <summary>
        /// 产品名称
        /// </summary>
        [DataMember]
        public string AppName { get; set; }

        /// <summary>
        /// 支持的平台
        /// </summary>
        [DataMember]
        public int[] Platform { get; set; }
    }

    #endregion

    /// <summary>
    /// 通用接口调用
    /// </summary>
    internal class TongyongCalling
    {
        /// <summary>
        /// 根据账号从通用获取关联产品信息
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public static ReturnedUserInfo GetReturnedUserInfo(string account)
        {
            //先暂停掉       2014.8.12   lyq
            //try
            //{
            //    string userName = account.ToLower();
            //    string checkSum = (userName + "CB338483-8985-439D-9A06-652B458263D8").HashDefaultToMD5Hex();
            //    string url = string.Format("http://dev.91.com/UserCenter/CustomerService/Query91UserAppList.ashx?userName={0}&checkSum={1}", userName, checkSum);
            //    HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            //    using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            //    using (Stream stream = response.GetResponseStream())
            //    {
            //        DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(ReturnedUserInfo));
            //        ReturnedUserInfo rUserInfo = (ReturnedUserInfo)ser.ReadObject(stream);
            //        return rUserInfo;
            //    }
            //}
            //catch (Exception ex)
            //{
            //    LogHelper.WriteException("通过通用平台接口获取其它产品权限异常", ex);
            //}
            return new ReturnedUserInfo {AppList = new AppInfo[0], ResultCode = 4, UserName = account};
        }
    }
}