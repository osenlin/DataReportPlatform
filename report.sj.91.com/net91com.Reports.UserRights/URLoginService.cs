using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Text.RegularExpressions;
using System.Runtime.Serialization.Json;

using net91com.Core;
using net91com.Core.Util;
using net91com.Core.Web;
using System.Net;
using System.IO;
using System.Runtime.Serialization;

namespace net91com.Reports.UserRights
{
    /// <summary>
    /// 登录及权限验证接口
    /// </summary>
    public class URLoginService
    {
        //当前客户端IP
        private string currentClientIP;
        //是否内部用户
        private bool internalRequest;
        //Session使用的KEY
        private const string SessionKeyForUserInfo = "CURRENT_LOGIN_USER_INFO";
        //当前登录用户
        private UserContext curUser = null;

        /// <summary>
        /// 构造函数
        /// </summary>
        public URLoginService()
        {
            currentClientIP = DACommonHelper.GetClientIP();
            internalRequest = GetInternalRequest();
        }

        #region 外网IP白名单(IpWhiteList)

        // 外网IP白名单
        private static HashSet<string> ipWhiteList = null;

        /// <summary>
        /// 外网IP白名单
        /// </summary>
        private static HashSet<string> IpWhiteList
        {
            get
            {
                if (ipWhiteList == null)
                {
                    ipWhiteList = new HashSet<string>();
                    string ipWhiteString = ConfigHelper.GetSetting("IpWhiteList");
                    string[] ipWhites = ipWhiteString.Split(new String[] {","}, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string ip in ipWhites)
                    {
                        string[] ps = ip.Split(new String[] {"."}, StringSplitOptions.RemoveEmptyEntries);
                        if (ps.Length != 4) continue;
                        string whiteIp = string.Empty;
                        int startIndex = 0, endIndex = 0;
                        foreach (string p in ps)
                        {
                            //最多只能有一个~
                            string[] spans = p.Split(new String[] {"~"}, StringSplitOptions.RemoveEmptyEntries);
                            if (spans.Length < 1 || spans.Length > 2) continue;
                            whiteIp += spans.Length == 1 ? "." + spans[0] : ".{0}";
                            if (spans.Length == 2)
                            {
                                startIndex = int.Parse(spans[0]);
                                endIndex = int.Parse(spans[1]);
                            }
                        }
                        whiteIp = whiteIp.Trim('.');
                        if (startIndex > 0 || endIndex > 0)
                        {
                            for (int i = startIndex; i <= endIndex; i++)
                            {
                                string temp = string.Format(whiteIp, i);
                                if (!ipWhiteList.Contains(temp))
                                    ipWhiteList.Add(temp);
                            }
                        }
                        else if (!ipWhiteList.Contains(whiteIp))
                        {
                            ipWhiteList.Add(whiteIp);
                        }
                    }
                }
                return ipWhiteList;
            }
        }

        #endregion

        /// <summary>
        /// 是否是从内部请求
        /// </summary>
        public bool InternalRequest
        {
            get { return internalRequest; }
        }

        /// <summary>
        /// 当前登录用户IP
        /// </summary>
        public string CurrentClientIP
        {
            get { return currentClientIP; }
        }

        /// <summary>
        /// 登录用户信息实例
        /// </summary>
        public User LoginUser
        {
            get { return GetCurrentUser().LoginUser; }
        }

        /// <summary>
        /// 获取所有的资源类型列表
        /// </summary>
        /// <returns></returns>
        public List<ResourceType> GetResourceTypes()
        {
            return DABasicInfoHelper.GetResourceTypes();
        }

        /// <summary>
        /// 拥有的权限列表
        /// </summary>
        public List<Right> AvailableRights
        {
            get { return GetCurrentUser().AvailableRights; }
        }

        /// <summary>
        /// 当前用户可管理的系统列表
        /// </summary>
        public List<SystemInfo> AdminSystems
        {
            get { return GetCurrentUser().AdminSystems; }
        }

        /// <summary>
        /// 根据页面地址查找权限信息实体
        /// </summary>
        /// <param name="pageUrl"></param>
        /// <param name="queryString"></param>
        /// <returns></returns>
        public Right FindRight(string pageUrl)
        {
            return FindRight(pageUrl, null);
        }

        /// <summary>
        /// 根据页面地址查找权限信息实体
        /// </summary>
        /// <param name="pageUrl"></param>
        /// <param name="queryString"></param>
        /// <returns></returns>
        public Right FindRight(string pageUrl, string queryString)
        {
            if (string.IsNullOrEmpty(pageUrl))
                return null;

            string url = pageUrl.TrimStart('/').ToLower();
            int qIndex = url.IndexOf('?');
            if (qIndex > 0)
                url = url.Substring(0, qIndex);
            if (!string.IsNullOrEmpty(queryString))
                url += queryString.ToLower();
            Right right = AvailableRights.FirstOrDefault(a => a.PageUrl == url);
            return right;
        }

        /// <summary>
        /// 有权限的产品列表(如果没有一个权限,会抛出异常)
        /// </summary>
        public List<Soft> AvailableSofts
        {
            get
            {
                List<Soft> softs = GetCurrentUser().AvailableSofts;
                if (softs.Count == 0)
                    throw new NotRightException();

                //softs = avilablesoftsforcxb(softs);

                return softs;
            }
        }

        ///// <summary>
        ///// 为cxbsky这个账号的特别定制
        ///// </summary>
        ///// <param name="softs"></param>
        ///// <returns></returns>
        //private List<Soft> avilablesoftsforcxb(List<Soft> softs)
        //{
        //    string countname = GetCurrentUser().LoginUser.Account;
        //    if (countname == "694982665@qq.com" || countname == "cxbsky" || countname == "tqnd789123")
        //    {
        //        List<Soft> lstcxb = softs.Where(p => p.ID == 2 || p.ID == 6 || p.ID == 100589 || p.ID == 46).ToList();
        //        lstcxb.AddRange(softs.Where(p => p.ID != 2 && p.ID != 6 && p.ID != 100589 && p.ID != 46).ToList());
        //        return lstcxb;
        //    }
        //    return softs;
        //}

        /// <summary>
        /// 获取JSON格式的软件列表
        /// </summary>
        public string GetAvailableSoftsJson()
        {
            return GetAvailableSoftsJson(null);
        }

        /// <summary>
        /// 获取JSON格式的软件列表
        /// </summary>
        /// <param name="selectedSofts"></param>
        /// <returns></returns>
        public string GetAvailableSoftsJson(List<Soft> selectedSofts)
        {
            if (AvailableSofts.Count > 0)
            {
                var softs = selectedSofts == null
                                ? AvailableSofts
                                : (from a in AvailableSofts
                                   join b in selectedSofts on a.ID equals b.ID
                                   select a).ToList();
                string json = "var softs=[";
                for (int i = 0; i < softs.Count; i++)
                {
                    json += string.Format("{{\"id\":{0},\"platforms\":[", softs[i].ID);
                    for (int j = 0; j < softs[i].Platforms.Count; j++)
                    {
                        json += string.Format("{0},", (int) softs[i].Platforms[j]);
                    }
                    json = json.TrimEnd(',');
                    json += "]},";
                }
                json = json.TrimEnd(',');
                json += "];";
                return json;
            }
            return string.Empty;
        }

        /// <summary>
        /// 有权限的项目来源列表(如果没有一个权限,会抛出异常)
        /// </summary>
        public List<ProjectSource> AvailableProjectSources
        {
            get
            {
                List<ProjectSource> projectSources = GetCurrentUser().AvailableProjectSources;
                if (projectSources.Count == 0)
                    throw new NotRightException();
                return projectSources;
            }
        }

        /// <summary>
        /// 有权限的资源列表
        /// </summary>
        public List<int> AvailableResIds
        {
            get { return GetCurrentUser().AvailableResIds; }
        }

        /// <summary>
        /// 获取用户渠道ID权限
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="softId"></param>
        /// <returns></returns>
        public List<int> GetAvailableChannelIds(int softId)
        {
            return GetAvailableChannelIds(softId, 0, new int[0]);
        }

        /// <summary>
        /// 获取用户渠道ID权限
        /// </summary>
        /// <param name="softId"></param>
        /// <param name="channelType"></param>
        /// <param name="channels"></param>
        /// <param name="mustLogin">有些地方不需要登录也可以请求</param>
        /// <returns></returns>
        public List<int> GetAvailableChannelIds(int softId, ChannelTypeOptions channelType, int[] channels, bool mustLogin = true)
        {
            if (mustLogin)
            {
                if (!AvailableSofts.Exists(a => a.ID == softId))
                    return new List<int>();

                if (LoginUser.AccountType == UserTypeOptions.Channel ||
                    LoginUser.AccountType == UserTypeOptions.ChannelPartner)
                {
                    return DAChannelsHelper.GetChannelIds(LoginUser.ID, softId, channelType, channels);
                }
            }
            return DAChannelsHelper.GetChannelIds(softId, channelType, channels);
        }

        /// <summary>
        /// 获取没带子渠道的渠道列表(全部产品)
        /// </summary>
        /// <returns></returns>
        public Dictionary<int, List<Channel>> GetAvailableChannelsWithoutSubChannels()
        {
            string cacheKey = string.Format("net91com.Reports.UserRights.URLoginService.GetAvailableChannelsWithoutSubChannels_{0}", LoginUser.ID);
            if (CacheHelper.Contains(cacheKey))
                return CacheHelper.Get<Dictionary<int, List<Channel>>>(cacheKey);
            Dictionary<int, List<Channel>> channels = DAChannelsHelper.GetChannelsWithoutSubChannels(LoginUser.ID);
            CacheHelper.Set<Dictionary<int, List<Channel>>>(cacheKey, channels, CacheTimeOption.TenMinutes, CacheExpirationOption.AbsoluteExpiration);
            return channels;
        }

        /// <summary>
        /// 获取指定用户指定产品的渠道列表
        /// </summary>
        /// <param name="softId"></param>
        /// <param name="platforms"></param>
        /// <param name="includeChannelIds">是否包含渠道ID</param>
        /// <returns></returns>
        public List<Channel> GetAvailableChannels(int softId, MobileOption[] platforms, bool includeChannelIds)
        {
            if (!AvailableSofts.Exists(a => a.ID == softId))
                return new List<Channel>();

            if (LoginUser.AccountType == UserTypeOptions.Channel || LoginUser.AccountType == UserTypeOptions.ChannelPartner)
            {
                string cacheKey = string.Format("net91com.Reports.UserRights.URLoginService.GetAvailableChannels_{0}_{1}", LoginUser.ID, softId);
                if (CacheHelper.Contains(cacheKey))
                    return CacheHelper.Get<List<Channel>>(cacheKey);

                List<Channel> channels = DAChannelsHelper.GetChannels(LoginUser.ID, softId);
                CacheHelper.Set<List<Channel>>(cacheKey, channels, CacheTimeOption.TenMinutes, CacheExpirationOption.AbsoluteExpiration);
                return channels;
            }
            else
            {
                List<Channel> channels;
                string cacheKey = string.Format("net91com.Reports.UserRights.URLoginService.GetAvailableChannels_{0}_{1}", softId, includeChannelIds);
                if (CacheHelper.Contains(cacheKey))
                {
                    channels = CacheHelper.Get<List<Channel>>(cacheKey);
                }
                else
                {
                    channels = DAChannelsHelper.GetChannels(softId, includeChannelIds);
                    CacheHelper.Set<List<Channel>>(cacheKey, channels, CacheTimeOption.TenMinutes, CacheExpirationOption.AbsoluteExpiration);
                }
                if (!includeChannelIds || platforms == null || platforms.Length == 0 ||
                    platforms.Contains(MobileOption.None))
                    return channels;

                List<Channel> result = new List<Channel>();
                for (int i = 0; i < channels.Count; i++)
                {
                    if (channels[i].Platform == MobileOption.None || platforms.Contains(channels[i].Platform))
                        result.Add(channels[i]);
                }
                return result;
            }
        }

        #region 登录或注销

        /// <summary>
        /// 登录
        /// </summary>
        public void Login()
        {
            ReturnedLoginResult loginResult = CheckLogin();
            UserContext user = new UserContext
            {
                LoginUser = GetUser(loginResult.result.account)
            };
            SetUserRights(user, DACommonHelper.REPORT_SYS_ID);
            HttpContext.Current.Session[SessionKeyForUserInfo] = user;
            //输出登录凭证
            ResponseCredentials(user.LoginUser.Account);
            //更新登录时间
            DABasicInfoHelper.UpdateLastLoginTime(user.LoginUser.ID);

            //记录登录日志
            DABasicInfoHelper.AddAdminLog(
                new AdminLog
                    {
                        Account = user.LoginUser.Account,
                        AccountType = user.LoginUser.AccountType,
                        AddTime = DateTime.Now,
                        IP = currentClientIP,
                        TrueName = user.LoginUser.TrueName,
                        PageUrl = "Login.aspx",
                        SystemID = DACommonHelper.REPORT_SYS_ID,
                        Memo = "登录"
                });

            //跳转至默认页
            HttpContext.Current.Response.Redirect("/index.aspx");
        }

        #region 登录校验

        /// <summary>
        /// 登录校验接口返回的结构信息
        /// </summary>
        [DataContract]
        internal class ReturnedLoginResult
        {
            /// <summary>
            /// 返回代码, 200表示成功
            /// </summary>
            [DataMember]
            public int code { get; set; }

            /// <summary>
            /// 返回消息
            /// </summary>
            [DataMember]
            public string msg { get; set; }

            /// <summary>
            /// 用户信息
            /// </summary>
            [DataMember]
            public ReturnedUserInfo result { get; set; }
        }

        /// <summary>
        /// 返回的账号信息
        /// </summary>
        [DataContract]
        internal class ReturnedUserInfo
        {
            /// <summary>
            /// 用户ID
            /// </summary>
            [DataMember]
            public int userId { get; set; }

            /// <summary>
            /// 用户账号
            /// </summary>
            [DataMember]
            public string account { get; set; }
        }

        /// <summary>
        /// 登录校验地址
        /// </summary>
        private static string UserLoginAuthUrl = ConfigHelper.GetSetting("UserLoginAuthUrl");
        /// <summary>
        /// 登录校验
        /// </summary>
        /// <returns></returns>
        private ReturnedLoginResult CheckLogin()
        {
            string token = HttpContext.Current.Request["token"];
            string ts = DateTime.Now.ToString("yyyy-MM-dd HH:mm:dd");
            string sign = CryptoHelper.MD5_Encrypt(string.Format("sid={0}&ts={1}&key=5cee621329f24e5cbdc43daa995ce9a1", token, ts), "utf-8").ToLower();
            string url = string.Format(UserLoginAuthUrl, token, HttpUtility.UrlEncode(ts), sign);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            {
                DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(ReturnedLoginResult));
                ReturnedLoginResult loginResult = (ReturnedLoginResult)ser.ReadObject(stream);
                if (loginResult.code == 200)
                    return loginResult;
                throw new ToUserException(string.Format("{0}, {1}", loginResult.code, loginResult.msg));
            }

        }

        #endregion

        /// <summary>
        /// 登录注销
        /// </summary>
        public void Logout()
        {
            FormsAuthentication.SignOut();
            HttpCookie cookie = HttpContext.Current.Request.Cookies["report_felink_com"];
            if (cookie != null)
            {
                cookie.Expires = DateTime.Now.AddDays(-1);
                cookie.Value = string.Empty;
                cookie.Domain = ".felink.com";
                HttpContext.Current.Response.Cookies.Add(cookie);
            }
            HttpContext.Current.Session[SessionKeyForUserInfo] = null;
        }

        #endregion

        #region 基本权限验证

        /// <summary>
        /// 验证指定页面权限
        /// </summary>
        /// <param name="pageUrl"></param>
        /// <returns></returns>
        public bool CheckUrlRight(string pageUrl)
        {
            return FindRight(pageUrl, null) != null;
        }

        /// <summary>
        /// 验证指定页面权限
        /// </summary>
        /// <param name="pageUrl"></param>
        /// <param name="queryString"></param>
        /// <returns></returns>
        public bool CheckUrlRight(string pageUrl, string queryString)
        {
            return FindRight(pageUrl, queryString) != null;
        }

        /// <summary>
        /// 验证指定权限
        /// </summary>
        public void HaveUrlRight()
        {
            HaveUrlRight(HttpContext.Current.Request.RawUrl, null);
        }

        /// <summary>
        /// 验证指定权限
        /// </summary>
        /// <param name="pageUrl"></param>
        public void HaveUrlRight(string pageUrl)
        {
            HaveUrlRight(pageUrl, null);
        }

        /// <summary>
        /// 验证指定权限
        /// </summary>
        /// <param name="pageUrl"></param>
        /// <param name="queryString">需要添加的额外参数</param>
        public void HaveUrlRight(string pageUrl, string queryString)
        {
            if (FindRight(pageUrl, queryString) == null)
                throw new NotRightException();
        }

        #endregion

        #region 管理员权限验证证

        /// <summary>
        /// 验证是否有权限(返回BOOL值)
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool CheckAdminRightForUserEdit(User user)
        {
            return CheckAdminRightForUserEdit(user, true);
        }

        /// <summary>
        /// 验证是否有权限(返回BOOL值)
        /// </summary>
        /// <param name="user"></param>
        /// <param name="allowProductAdmin"></param>
        /// <returns></returns>
        public bool CheckAdminRightForUserEdit(User user, bool allowProductAdmin)
        {
            if (user == null)
                return false;

            switch (LoginUser.AccountType)
            {
                case UserTypeOptions.SuperAdmin:
                    if (user.AccountType == UserTypeOptions.SuperAdmin && user.ID > 0 && user.ID != LoginUser.ID)
                        return false;
                    break;
                case UserTypeOptions.Admin:
                    if (user.AccountType == UserTypeOptions.SuperAdmin || user.AccountType == UserTypeOptions.Admin)
                        return false;                    
                    break;
                case UserTypeOptions.ProductAdmin:
                    if (!allowProductAdmin)
                        throw new NotRightException();
                    if (user.AccountType == UserTypeOptions.SuperAdmin || user.AccountType == UserTypeOptions.Admin
                        || user.AccountType == UserTypeOptions.ProductAdmin)
                        return false;
                    //if (user.ID > 0 && user.AdminUserID != LoginUser.ID)
                    //    return false;
                    break;
                default:
                    return false;
            }            
            return true;
        }

        /// <summary>
        /// 验证是否有权限(返回BOOL值)
        /// </summary>
        /// <param name="sysId"></param>
        /// <param name="user"></param>
        /// <param name="allowProductAdmin"></param>
        /// <returns></returns>
        public bool CheckAdminRightForUserGrant(int sysId, User user, bool allowProductAdmin)
        {
            if (user == null || user.ID <= 0)
                return false;

            switch (LoginUser.AccountType)
            {
                case UserTypeOptions.SuperAdmin:
                    if (user.AccountType == UserTypeOptions.SuperAdmin && user.ID != LoginUser.ID)
                        return false;
                    return true;
                case UserTypeOptions.Admin:
                    if (user.AccountType == UserTypeOptions.SuperAdmin || user.AccountType == UserTypeOptions.Admin)
                        return false;
                    break;
                case UserTypeOptions.ProductAdmin:
                    if (!allowProductAdmin)
                        throw new NotRightException();
                    if (user.AccountType == UserTypeOptions.SuperAdmin || user.AccountType == UserTypeOptions.Admin
                        || user.AccountType == UserTypeOptions.ProductAdmin)
                        return false;
                    break;
                default:
                    return false;
            }
            //必须要有该系统的管理权限
            if (sysId > 0 && !AdminSystems.Exists(a => a.ID == sysId))
                return false;
            return true;
        }

        /// <summary>
        /// 验证是否有权限(返回BOOL值)
        /// </summary>
        /// <param name="role"></param>
        public bool CheckAdminRightForRole(Role role)
        {
            if (role == null || role.SystemID <= 0)
                return false;

            switch (LoginUser.AccountType)
            {
                case UserTypeOptions.SuperAdmin:
                    break;
                case UserTypeOptions.Admin:
                    //管理员不能操作系统角色
                    if (role.RoleType != RoleTypeOptions.General)
                        return false;                    
                    //不可以修改自身拥有的角色的权限
                    if (role.ID > 0 && GetMyRoleIds(role.SystemID).Exists(a => a == role.ID))
                        return false;                    
                    break;
                default:
                    return false;
            }
            //不可以修改没有管理权限的系统的角色信息
            if (!AdminSystems.Exists(a => a.ID == role.SystemID))
                return false;
            return true;
        }

        /// <summary>
        /// 验证是否有超级管理员权限(没有权限抛出NotRightException异常)
        /// </summary>
        public void HaveSuperAdminRight()
        {
            switch (LoginUser.AccountType)
            {
                case UserTypeOptions.SuperAdmin:
                    break;
                default:
                    throw new NotRightException();
            }
        }

        /// <summary>
        /// 验证是否有管理员权限(没有权限抛出NotRightException异常)
        /// </summary>
        public void HaveAdminRight()
        {
            HaveAdminRight(0, true);
        }

        /// <summary>
        /// 验证是否有管理员权限(没有权限抛出NotRightException异常)
        /// </summary>
        /// <param name="sysId"></param>
        internal void HaveAdminRight(int sysId)
        {
            HaveAdminRight(sysId, true);
        }

        /// <summary>
        /// 验证是否有管理员权限(没有权限抛出NotRightException异常)
        /// </summary>
        /// <param name="sysId"></param>
        /// <param name="allowProductAdmin"></param>
        internal void HaveAdminRight(int sysId, bool allowProductAdmin)
        {
            switch (LoginUser.AccountType)
            {
                case UserTypeOptions.SuperAdmin:
                case UserTypeOptions.Admin:
                    break;
                case UserTypeOptions.ProductAdmin:
                    if (!allowProductAdmin)
                        throw new NotRightException();
                    break;
                default:
                    throw new NotRightException();
            }
            if (sysId > 0 && !AdminSystems.Exists(a => a.ID == sysId))
                throw new NotRightException();
        }

        /// <summary>
        /// 验证是否有管理员权限(没有权限抛出NotRightException异常)
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="softId"></param>
        /// <returns></returns>
        internal User HaveAdminRightForUserGrantSoft(int userId, int softId)
        {
            User user = HaveAdminRightForUserGrant(userId);
            if (LoginUser.AccountType == UserTypeOptions.ProductAdmin && !AvailableSofts.Exists(a => a.ID == softId))
                throw new NotRightException();
            return user;
        }

        /// <summary>
        /// 验证是否有管理员权限(没有权限抛出NotRightException异常)
        /// </summary>
        /// <param name="userId"></param>
        internal User HaveAdminRightForUserEdit(int userId)
        {
            return HaveAdminRightForUserEdit(userId, true);
        }

        /// <summary>
        /// 验证是否有管理员权限(没有权限抛出NotRightException异常)
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="allowProductAdmin"></param>
        /// <returns></returns>
        internal User HaveAdminRightForUserEdit(int userId, bool allowProductAdmin)
        {
            User user = DABasicInfoHelper.GetUser(userId);
            HaveAdminRightForUserEdit(user, allowProductAdmin);
            return user;
        }

        /// <summary>
        /// 验证是否有管理员权限(没有权限抛出NotRightException异常)
        /// </summary>
        /// <param name="user"></param>
        internal void HaveAdminRightForUserEdit(User user)
        {
            HaveAdminRightForUserEdit(user, true);
        }

        /// <summary>
        /// 验证是否有管理员权限(没有权限抛出NotRightException异常)
        /// </summary>
        /// <param name="user"></param>
        internal void HaveAdminRightForUserEdit(User user, bool allowProductAdmin)
        {
            if (!CheckAdminRightForUserEdit(user, allowProductAdmin))
                throw new NotRightException();           
        }

        /// <summary>
        /// 验证是否有管理员权限(没有权限抛出NotRightException异常)
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        internal User HaveAdminRightForUserGrant(int userId)
        {
            return HaveAdminRightForUserGrant(DACommonHelper.REPORT_SYS_ID, userId, true);
        }

        /// <summary>
        /// 验证是否有管理员权限(没有权限抛出NotRightException异常)
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="allowProductAdmin"></param>
        /// <returns></returns>
        internal User HaveAdminRightForUserGrant(int userId, bool allowProductAdmin)
        {
            return HaveAdminRightForUserGrant(DACommonHelper.REPORT_SYS_ID, userId, allowProductAdmin);
        }

        /// <summary>
        /// 验证是否有管理员权限(没有权限抛出NotRightException异常)
        /// </summary>
        /// <param name="sysId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        internal User HaveAdminRightForUserGrant(int sysId, int userId, bool allowProductAdmin)
        {
            User user = DABasicInfoHelper.GetUser(userId);
            if (!CheckAdminRightForUserGrant(sysId, user, allowProductAdmin))
                throw new NotRightException();

            //必须当前用户有该系统的权限
            if (sysId > 0 && !DARightsHelper.GetUserSystems(user.ID).Exists(a => a.SystemID == sysId))
                throw new NotRightException();

            return user;
        }

        /// <summary>
        /// 验证是否有管理员权限(没有权限抛出NotRightException异常)
        /// </summary>
        /// <param name="role"></param>
        internal void HaveAdminRightForRole(Role role)
        {
            if (!CheckAdminRightForRole(role))
                throw new NotRightException();
        }

        /// <summary>
        /// 验证是否有管理员权限(没有权限抛出NotRightException异常)
        /// </summary>
        /// <param name="roleId"></param>
        internal Role HaveAdminRightForRole(int roleId)
        {
            Role role = DABasicInfoHelper.GetRole(roleId);
            if (!CheckAdminRightForRole(role))
                throw new NotRightException();
            return role;
        }

        #endregion

        #region 添加操作日志

        /// <summary>
        /// 添加操作日志
        /// </summary>
        /// <param name="message"></param>
        public void AddLog(string message)
        {
            AddLog(string.Empty, message);
        }

        /// <summary>
        /// 添加操作日志
        /// </summary>
        /// <param name="actionUrl"></param>
        /// <param name="message"></param>
        public void AddLog(string actionUrl, string message)
        {
            //记录登录日志
            DABasicInfoHelper.AddAdminLog(
                new AdminLog
                    {
                        Account = LoginUser.Account,
                        AccountType = LoginUser.AccountType,
                        AddTime = DateTime.Now,
                        IP = CurrentClientIP,
                        TrueName = LoginUser.TrueName,
                        PageUrl = actionUrl,
                        Memo = message,
                        SystemID = DACommonHelper.REPORT_SYS_ID
                });
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 当前用户信息
        /// </summary>
        private UserContext GetCurrentUser()
        {
            if (curUser == null)
            {
                string account;
                bool validUser = CheckCredentials(out account);
                if (validUser)
                {
                    UserContext user = (UserContext)HttpContext.Current.Session[SessionKeyForUserInfo];
                    if (user == null || account.ToLower() != user.LoginUser.Account.ToLower())
                    {
                        user = new UserContext
                        {
                            LoginUser = GetUser(account)
                        };
                    }
                    //设置权限
                    SetUserRights(user, DACommonHelper.REPORT_SYS_ID);
                    HttpContext.Current.Session[SessionKeyForUserInfo] = user;
                    //更新登录凭证,使有效期加长
                    ResponseCredentials(account);
                    curUser = user;
                }
                if (curUser == null)
                    HttpContext.Current.Response.Redirect("~/Login.aspx", true);            
            }
            return curUser;
        }

        /// <summary>
        /// 是否内部用户
        /// </summary>
        /// <returns></returns>
        private bool GetInternalRequest()
        {
            return true;
            //string ip = DACommonHelper.GetClientIP();
            //return ip == "127.0.0.1" || ip.StartsWith("10.") || ip.StartsWith("192.168.")
            //       || Regex.IsMatch(ip, @"^172\.(1([6-9]{1})|2([0-9]{1})|3([0-1]{1}))(\.[0-9]+){2}$")
            //       || IpWhiteList.Contains(ip);
        }

        /// <summary>
        /// 获取有效的产品列表
        /// </summary>
        /// <returns></returns>
        private List<Soft> GetAvailableSofts()
        {
            string cacheKey = "net91com.Reports.UserRights.URLoginService.GetAvailableSofts";
            if (CacheHelper.Contains(cacheKey))
                return CacheHelper.Get<List<Soft>>(cacheKey);
            List<Soft> softs = DABasicInfoHelper.GetSofts();
            List<ProjectSource> projectSources = GetAvailableProjectSources();
            for (int i = 0; i < softs.Count; i++)
            {
                var temp = projectSources.Where(a => a.SoftID == softs[i].ID);
                if (temp.Count() > 0)
                    softs[i].ProjectSources.AddRange(temp);
            }
            CacheHelper.Set<List<Soft>>(cacheKey, softs, CacheTimeOption.Short, CacheExpirationOption.AbsoluteExpiration);
            return softs;
        }

        /// <summary>
        /// 获取有效的项目来源列表
        /// </summary>
        /// <returns></returns>
        private List<ProjectSource> GetAvailableProjectSources()
        {
            string cacheKey = "net91com.Reports.UserRights.URLoginService.GetAvailableProjectSources";
            if (CacheHelper.Contains(cacheKey))
                return CacheHelper.Get<List<ProjectSource>>(cacheKey);
            List<ProjectSource> projectSources = DABasicInfoHelper.GetProjectSources();
            CacheHelper.Set<List<ProjectSource>>(cacheKey, projectSources, CacheTimeOption.Short, CacheExpirationOption.AbsoluteExpiration);
            return projectSources;
        }

        /// <summary>
        /// 输出登录凭证
        /// </summary>
        /// <param name="account"></param>
        private void ResponseCredentials(string account)
        {
            FormsAuthentication.SetAuthCookie(account, false);
            HttpCookie cookie = HttpContext.Current.Request.Cookies["report_felink_com"];
            if (cookie == null)
            {
                cookie = new HttpCookie("report_felink_com");
                cookie.Values["username"] = HttpUtility.UrlEncode(account);
                cookie.Values["Credential"] = String.Format("{0}{1}", account, "$abc$.//.$123$").HashDefaultToMD5Hex();
            }
            cookie.Domain = ".felink.com";
            cookie.Expires = DateTime.Now.AddHours(4);
            HttpContext.Current.Response.Cookies.Add(cookie);
        }

        /// <summary>
        /// 验证登录凭证
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        private bool CheckCredentials(out string account)
        {
            account = null;
            HttpCookie cookie = HttpContext.Current.Request.Cookies["report_felink_com"];
            if (cookie != null)
            {
                account = HttpUtility.UrlDecode(cookie["username"]);
                string credential = cookie["Credential"];
                if (account != null && credential != null
                    && String.Format("{0}{1}", account, "$abc$.//.$123$").HashDefaultToMD5Hex() == credential)
                {
                    return true;
                }
            }            
            return false;
        }

        /// <summary>
        /// 获取或添加用户信息
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        private User GetUser(string account)
        {
            User user = DABasicInfoHelper.GetUser(account);
            //如果用户不存在,则抛出异常
            if (user == null || user.Status == StatusOptions.Invalid)
            {
                Logout();
                HttpContext.Current.Response.Redirect("~/Login.aspx", true);
            }
            return user;
        }

        /// <summary>
        /// 设置用户的权限
        /// </summary>
        /// <param name="user"></param>
        /// <param name="sysId"></param>
        private void SetUserRights(UserContext user, int sysId)
        {
            UserTypeOptions userType = user.LoginUser.AccountType;
            bool isSuperAdmin = userType == UserTypeOptions.SuperAdmin;
            bool isWhiteUser = user.LoginUser.IsWhiteUser;
            if (user.AvailableRights == null)
            {
                List<Right> availableRights = DABasicInfoHelper.GetRights(sysId, -1, CacheTimeOption.Short);
                List<int> rights = DARightsHelper.GetUserRights(sysId, user.LoginUser.ID, user.LoginUser.AccountType).Select(a => a.RightID).ToList();
                user.AvailableRights = availableRights.Where(a => (isSuperAdmin || internalRequest || !a.OnlyInternal || isWhiteUser) && rights.Contains(a.ID)).ToList();
            }
            //只有report才有这些权限
            if (sysId == DACommonHelper.REPORT_SYS_ID)
            {
                if (user.AvailableSofts == null)
                {
                    //从Report平台获得的产品权限
                    List<Soft> availableSofts = GetAvailableSofts();
                    List<int> rights = DARightsHelper.GetUserSoftRights(user.LoginUser.ID).Select(a => a.RightID).ToList();
                    user.AvailableSofts = availableSofts.Where(a => a.Status == StatusOptions.Valid && (isSuperAdmin || internalRequest || !a.OnlyInternal || isWhiteUser) && rights.Contains(a.ID)).ToList();
                }
                if (user.AvailableProjectSources == null)
                {
                    List<ProjectSource> availableProjectSources = GetAvailableProjectSources();
                    List<int> rights = DARightsHelper.GetUserProjectSourceRights(user.LoginUser.ID).Select(a => a.RightID).ToList();
                    user.AvailableProjectSources = availableProjectSources.Where(a => (isSuperAdmin || internalRequest || !a.OnlyInternal || isWhiteUser) && rights.Contains(a.ProjectSourceID)).ToList();
                }
                if (internalRequest && user.AvailableResIds == null)
                {
                    user.AvailableResIds = DARightsHelper.GetUserResRights(user.LoginUser.ID).Select(a => a.RightID).ToList();
                }
            }
            else
            {
                if (user.AvailableSofts == null)
                    user.AvailableSofts = new List<Soft>();
                if (user.AvailableProjectSources == null)
                    user.AvailableProjectSources = new List<ProjectSource>();
                if (internalRequest && user.AvailableResIds == null)
                    user.AvailableResIds = new List<int>();
            }
            if (user.AdminSystems == null)
            {
                List<SystemInfo> systems = DABasicInfoHelper.GetSystems(CacheTimeOption.Short);
                List<int> sysIds = DARightsHelper.GetAdminSystemIds(user.LoginUser.ID);
                user.AdminSystems = systems.Where(a => sysIds.Contains(a.ID)).ToList();
            }
        }

        /// <summary>
        /// 清掉缓存，重新加载
        /// </summary>
        internal void ReloadAdminSystems()
        {
            List<SystemInfo> systems = DABasicInfoHelper.GetSystems(CacheTimeOption.Short, true);
            List<int> sysIds = DARightsHelper.GetAdminSystemIds(curUser.LoginUser.ID);
            curUser.AdminSystems = systems.Where(a => sysIds.Contains(a.ID)).ToList();
        }

        private List<int> myRoleIds = null;

        /// <summary>
        /// 获取用户的角色列表
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        private List<int> GetMyRoleIds(int sysId)
        {
            if (myRoleIds == null)
            {
                myRoleIds = DARightsHelper.GetUserRoles(sysId, LoginUser.ID);
            }
            return myRoleIds;
        }

        #endregion
    }
}