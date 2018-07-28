using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using net91com.Core;
using net91com.Core.Util;
using System.Text.RegularExpressions;

namespace net91com.Reports.UserRights
{
    /// <summary>
    /// 权限分配
    /// </summary>
    public class URRightsService
    {
        private URLoginService loginService = new URLoginService();       

        #region 给用户分配权限

        /// <summary>
        /// 授予用户操作权限
        /// </summary>
        /// <param name="sysId"></param>
        /// <param name="userId"></param>
        /// <param name="selectedRightIds">当前选择授予的权限列表</param>
        public void AddUserRights(int sysId, int userId, List<int> selectedRightIds)
        {
            if (sysId <= 0)
                throw new NotRightException();

            //权限判断
            User user = loginService.HaveAdminRightForUserGrant(sysId, userId, true);

            //不能直接给渠道方面的用户分配操作权限,只能给他们分配角色
            if (user.AccountType == UserTypeOptions.Channel || user.AccountType == UserTypeOptions.ChannelPartner)
                throw new NotRightException();

            List<int> rangeRightIds = loginService.LoginUser.AccountType == UserTypeOptions.ProductAdmin
                                          ? DARightsHelper.GetUserRights(sysId, loginService.LoginUser.ID, loginService.LoginUser.AccountType).Select(a => a.RightID).ToList()
                                          : null;
            DARightsHelper.AddUserRights(sysId, userId, selectedRightIds, rangeRightIds);

            //记录登录日志
            string rights = string.Empty;
            selectedRightIds.ForEach((i) => { rights += i.ToString() + ","; });
            rights = rights.TrimEnd(',');
            loginService.AddLog(
                "AddUserRights",
                string.Format("添加用户权限(UserID={0},SysID={1},Rights={2})", userId, sysId, rights));
        }

        /// <summary>
        /// 授予用户产品权限
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="roleId"></param>
        /// <param name="selectedRightKeys">当前选择授予的权限列表</param>
        public void AddUserSoftRights(int userId, List<int> selectedSoftIds)
        {
            //权限判断
            loginService.HaveAdminRightForUserGrant(userId);

            List<int> rangeSoftIds = loginService.LoginUser.AccountType == UserTypeOptions.ProductAdmin
                                         ? loginService.AvailableSofts.Select(a => a.ID).ToList()
                                         : null;
            DARightsHelper.AddUserSoftRights(userId, selectedSoftIds, rangeSoftIds);

            //记录登录日志
            string rights = string.Empty;
            selectedSoftIds.ForEach((i) => { rights += i.ToString() + ","; });
            rights = rights.TrimEnd(',');
            loginService.AddLog(
                "AddUserSoftRights",
                string.Format("添加用户产品权限(UserID={0},SoftRights={1})", userId, rights));
        }

        /// <summary>
        /// 授予用户项目源权限
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="roleId"></param>
        /// <param name="selectedRightKeys">当前选择授予的权限列表</param>
        public void AddUserProjectSourceRights(int userId, List<int> selectedProjectSources)
        {
            //权限判断
            loginService.HaveAdminRightForUserGrant(userId);

            List<int> rangeProjectSources = loginService.LoginUser.AccountType == UserTypeOptions.ProductAdmin
                                                ? loginService.AvailableProjectSources.Select(a => a.ProjectSourceID).ToList()
                                                : null;
            DARightsHelper.AddUserProjectSourceRights(userId, selectedProjectSources, rangeProjectSources);

            //记录登录日志
            string rights = string.Empty;
            selectedProjectSources.ForEach((i) => { rights += i.ToString() + ","; });
            rights = rights.TrimEnd(',');
            loginService.AddLog(
                "AddUserProjectSourceRights",
                string.Format("添加用户项目来源权限(UserID={0},ProjectSourceRights={1})", userId, rights));
        }

        /// <summary>
        /// 授予用户特定资源权限
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="resIds"></param>
        public void AddUserResRights(int userId, List<int> resIds)
        {
            //权限判断
            loginService.HaveAdminRightForUserGrant(userId, false);

            DARightsHelper.AddUserResRights(userId, resIds);

            //记录登录日志
            string rights = string.Empty;
            resIds.ForEach((i) => { rights += i.ToString() + ","; });
            rights = rights.TrimEnd(',');
            loginService.AddLog(
                "AddUserResRights",
                string.Format("添加用户资源权限(UserID={0},ResIds={1})", userId, rights));
        }

        /// <summary>
        /// 获取用户权限(仅用于权限分配)
        /// </summary>
        /// <param name="sysId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<RightItem> GetUserRights(int sysId, int userId)
        {
            //权限判断
            User user = loginService.HaveAdminRightForUserGrant(sysId, userId, true);

            return DARightsHelper.GetUserRights(sysId, userId, user.AccountType);
        }

        //第三方系统请求IP配置信息
        private static string GetUserRightsJson_ClientIP = ConfigHelper.GetSetting("GetUserRightsJson_ClientIP");
        /// <summary>
        /// 获取指定用户在指定系统所有拥有的权限信息(专门用于第三方系统调用)
        /// </summary>
        /// <param name="sysId"></param>
        /// <param name="account"></param>
        /// <param name="sign"></param>
        /// <returns></returns>
        public string GetUserRightsJson(int sysId, string account, string sign)
        {
            try
            {
                //验证请求参数
                if (sysId <= 0 || string.IsNullOrEmpty(account) || string.IsNullOrEmpty(sign))
                    return "{\"State\":1,\"Message\":\"请求参数无效。\"}";

                //限定配置过的IP才能请求
                string clientIp = DACommonHelper.GetClientIP();
                //if (clientIp != "127.0.0.1"
                //    && !clientIp.StartsWith("10.")
                //    && !clientIp.StartsWith("192.168.")
                //    && !Regex.IsMatch(clientIp, @"^172\.(1([6-9]{1})|2([0-9]{1})|3([0-1]{1}))(\.[0-9]+){2}$")
                //    && !GetUserRightsJson_ClientIP.Contains(clientIp))
                //{
                //    return "{\"State\":2,\"Message\":\"当前请求IP无效。\"}";
                //}

                //指定的系统必须存在
                SystemInfo system = DABasicInfoHelper.GetSystem(sysId, CacheTimeOption.Short);
                if (system == null)
                    return "{\"State\":101,\"Message\":\"当前系统不存在。\"}";
                if (system.Status == StatusOptions.Invalid)
                    return "{\"State\":102,\"Message\":\"当前系统已被禁用。\"}";

                //请求有做MD5校验
                string md5 = CryptoHelper.MD5_Encrypt(string.Format("{0}{1}{2}", sysId, system.Md5Key, account));
                if (md5.ToLower() != sign.ToLower())
                    return "{\"State\":3,\"Message\":\"无效的请求。\"}";

                //验证用户有效性
                User user = DABasicInfoHelper.GetUser(account);
                if (user == null)
                    return "{\"State\":103,\"Message\":\"用户不存在。\"}";
                if (user.Status == StatusOptions.Invalid)
                    return "{\"State\":104,\"Message\":\"用户已被禁用。\"}";
                if (user.AccountType != UserTypeOptions.SuperAdmin 
                    && (DateTime.Now > user.EndTime || DateTime.Now < user.BeginTime))
                    return "{\"State\":105,\"Message\":\"用户权限已过期。\"}";
                List<UserSystem> userSystems = DARightsHelper.GetUserSystems(user.ID);
                UserSystem userSystem = userSystems.FirstOrDefault(a => a.SystemID == sysId);
                if (userSystem == null)
                    return "{\"State\":106,\"Message\":\"用户没有当前系统的访问权限。\"}";

                //提取用户权限
                List<Right> allRights = DABasicInfoHelper.GetRights(sysId, -1, CacheTimeOption.Short);
                List<RightItem> myRights = DARightsHelper.GetUserRights(sysId, user.ID, user.AccountType);
                var rights = from a in allRights
                             join b in myRights on a.ID equals b.RightID
                             where a.Status == StatusOptions.Valid
                             select a;
                if (rights.Count() == 0)
                    return "{\"State\":107,\"Message\":\"用户没有当前系统的操作权限。\"}";

                //生成正常返回JSON
                StringBuilder result = new StringBuilder("{\"State\":0,\"Message\":\"OK\",");
                result.AppendFormat("\"System\":{{\"ID\":{0},\"Name\":\"{1}\",\"Url\":\"{2}\"}},", system.ID, system.Name, system.Url);
                result.AppendFormat("\"User\":{{\"ID\":{0},\"Account\":\"{1}\",\"TrueName\":\"{2}\",\"UserType\":{3},\"Email\":\"{4}\",\"Department\":\"{5}\",\"LastLoginTime\":\"{6}\"}},"
                    , user.ID, user.Account, user.TrueName, userSystem.Admin ? (int)user.AccountType : 0, user.Email, user.Department, userSystem.LastLoginTime.ToString("yyyy-MM-dd HH:mm:ss"));
                result.Append("\"Rights\":[");
                foreach (var right in rights)
                {
                    result.AppendFormat("{{\"ID\":{0},\"PID\":{1},\"Name\":\"{2}\",\"Level\":{3},\"Type\":{4},\"SortIndex\":{5},\"URL\":\"{6}\"}},"
                        , right.ID, right.ParentID, right.Name, right.RightLevel, (int)right.RightType, right.SortIndex, right.PageUrl);
                }

                //更新最后一次访问时间
                DABasicInfoHelper.UpdateSystemLastLoginTime(sysId, user.ID);

                //记录日志
                DABasicInfoHelper.AddAdminLog(
                    new AdminLog
                    {
                        Account = user.Account,
                        TrueName = user.TrueName,
                        AccountType = user.AccountType,
                        AddTime = DateTime.Now,
                        IP = clientIp,
                        PageUrl = "GetUserRightsJson",
                        SystemID = sysId,
                        Memo = string.Format("{0}系统获取用户{1}权限", system.Name, user.Account)
                    });

                return result.ToString(0, result.Length - 1) + "]}";
            }
            catch (Exception ex)
            {
                LogHelper.WriteException("GetUserRightsJson异常", ex);
                return "{\"State\":4,\"Message\":\"系统异常。\"}";
            }
        }        

        /// <summary>
        /// 获取用户产品权限(仅用于权限分配)
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<RightItem> GetUserSoftRights(int userId)
        {
            //权限判断
            loginService.HaveAdminRightForUserGrant(userId);

            return DARightsHelper.GetUserSoftRights(userId);
        }

        /// <summary>
        /// 获取用户项目来源权限(仅用于权限分配)
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<RightItem> GetUserProjectSourceRights(int userId)
        {
            //权限判断
            loginService.HaveAdminRightForUserGrant(userId);

            return DARightsHelper.GetUserProjectSourceRights(userId);
        }

        /// <summary>
        /// 获取用户资源权限
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<RightItem> GetUserResRights(int userId)
        {
            //权限判断
            User user = loginService.HaveAdminRightForUserGrant(userId, false);

            return DARightsHelper.GetUserResRights(userId);
        }

        #endregion

        #region 给用户分配系统

        /// <summary>
        /// 授予用户操作权限
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="userSystems">当前选择授予的系统列表</param>
        public void AddUserSystems(int userId, List<UserSystem> userSystems)
        {
            //权限判断
            User user = loginService.HaveAdminRightForUserGrant(0, userId, true);
            //非管理员不允许有管理系统权限
            if (user.AccountType == UserTypeOptions.General 
                || user.AccountType == UserTypeOptions.Channel
                || user.AccountType == UserTypeOptions.ChannelPartner)
            {
                for (int i = 0; i < userSystems.Count; i++)
                    userSystems[i].Admin = false;
            }
            List<int> rangeSysIds = loginService.AdminSystems.Select(a => a.ID).ToList();
            DARightsHelper.AddUserSystems(userId, userSystems, rangeSysIds);

            //记录登录日志
            string systems = string.Empty;
            userSystems.ForEach((i) => { systems += "(" + i.SystemID.ToString() + "," + i.Admin.ToString() + "),"; });
            systems = systems.TrimEnd(',');
            loginService.AddLog(
                "AddUserSystems",
                string.Format("添加用户系统(UserID={0},Systems={{{1}}})", userId, systems));
        }

        /// <summary>
        /// 获得有权限的系统
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<UserSystem> GetUserSystems(int userId)
        {
            //权限判断
            loginService.HaveAdminRightForUserGrant(0, userId, true);

            return DARightsHelper.GetUserSystems(userId);
        }

        #endregion

        #region 给用户分配角色

        /// <summary>
        /// 批量添加用户角色
        /// </summary>
        /// <param name="sysId"></param>
        /// <param name="userId"></param>
        /// <param name="selectedRoleIds">已选择的角色</param>
        public void AddUserRoles(int sysId, int userId, List<int> selectedRoleIds)
        {
            //权限判断
            loginService.HaveAdminRightForUserGrant(sysId, userId, true);

            List<int> rangeRoleIds = loginService.LoginUser.AccountType == UserTypeOptions.ProductAdmin
                                         ? DARightsHelper.GetUserRoles(sysId, loginService.LoginUser.ID)
                                         : null;
            DARightsHelper.AddUserRoles(sysId, userId, selectedRoleIds, rangeRoleIds);

            //记录登录日志
            string roles = string.Empty;
            selectedRoleIds.ForEach((i) => { roles += i.ToString() + ","; });
            roles = roles.TrimEnd(',');
            loginService.AddLog(
                "AddUserRoles",
                string.Format("添加用户角色(UserID={0},SysID={1},Roles={2})", userId, sysId, roles));
        }

        /// <summary>
        /// 获取指定用户的角色
        /// </summary>
        /// <param name="sysId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<int> GetUserRoles(int sysId, int userId)
        {
            //权限判断
            User user = loginService.HaveAdminRightForUserGrant(sysId, userId, true);

            return DARightsHelper.GetUserRoles(sysId, userId);
        }

        #endregion

        #region 给角色分配权限

        /// <summary>
        /// 授予角色操作权限
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="rightIds"></param>
        public void AddRoleRights(int roleId, List<int> rightIds)
        {
            //权限判断
            Role role = loginService.HaveAdminRightForRole(roleId);

            List<int> rangeRightIds = loginService.LoginUser.AccountType == UserTypeOptions.ProductAdmin
                                          ? DARightsHelper.GetUserRights(role.SystemID, loginService.LoginUser.ID, loginService.LoginUser.AccountType).Select(a => a.RightID).ToList()
                                          : null;
            DARightsHelper.AddRoleRights(roleId, rightIds, rangeRightIds);

            //记录登录日志
            string rights = string.Empty;
            rightIds.ForEach((i) => { rights += i.ToString() + ","; });
            rights = rights.TrimEnd(',');
            loginService.AddLog(
                "AddRoleRights",
                string.Format("添加角色权限(RoleID={0},Rights={1})", roleId, rights));
        }

        /// <summary>
        /// 授予角色产品权限
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="softIds"></param>
        public void AddRoleSoftRights(int roleId, List<int> softIds)
        {
            //权限判断
            loginService.HaveAdminRightForRole(roleId);

            List<int> rangeRightIds = loginService.LoginUser.AccountType == UserTypeOptions.ProductAdmin
                                          ? loginService.AvailableSofts.Select(a => a.ID).ToList()
                                          : null;
            DARightsHelper.AddRoleSoftRights(roleId, softIds, rangeRightIds);

            //记录登录日志
            string rights = string.Empty;
            softIds.ForEach((i) => { rights += i.ToString() + ","; });
            rights = rights.TrimEnd(',');
            loginService.AddLog(
                "AddRoleSoftRights",
                string.Format("添加角色产品权限(RoleID={0},Rights={1})", roleId, rights));
        }

        /// <summary>
        /// 授予角色项目源权限
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="projectSource"></param>
        public void AddRoleProjectSourceRights(int roleId, List<int> projectSources)
        {
            //权限判断
            loginService.HaveAdminRightForRole(roleId);

            List<int> rangeRightIds = loginService.LoginUser.AccountType == UserTypeOptions.ProductAdmin
                                          ? loginService.AvailableProjectSources.Select(a => a.ProjectSourceID).ToList()
                                          : null;
            DARightsHelper.AddRoleProjectSourceRights(roleId, projectSources, rangeRightIds);

            //记录登录日志
            string rights = string.Empty;
            projectSources.ForEach((i) => { rights += i.ToString() + ","; });
            rights = rights.TrimEnd(',');
            loginService.AddLog(
                "AddRoleProjectSourceRights",
                string.Format("添加角色项目来源权限(RoleID={0},Rights={1})", roleId, rights));
        }

        /// <summary>
        /// 获取角色权限(仅用于权限分配)
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public List<RightItem> GetRoleRights(int roleId)
        {
            //权限判断
            loginService.HaveAdminRightForRole(roleId);

            return DARightsHelper.GetRoleRights(roleId);
        }

        /// <summary>
        /// 获取角色产品权限
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public List<RightItem> GetRoleSoftRights(int roleId)
        {
            //权限判断
            loginService.HaveAdminRightForRole(roleId);

            return DARightsHelper.GetRoleSoftRights(roleId);
        }

        /// <summary>
        /// 获取角色项目来源权限(仅用于权限分配)
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public List<RightItem> GetRoleProjectSourceRights(int roleId)
        {
            //权限判断
            loginService.HaveAdminRightForRole(roleId);

            return DARightsHelper.GetRoleProjectSourceRights(roleId);
        }

        #endregion
    }
}