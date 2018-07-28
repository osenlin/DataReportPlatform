using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using net91com.Core;

namespace net91com.Reports.UserRights
{
    /// <summary>
    /// 基本信息数据存取(用户/角色/权限等基本信息的增删改查)
    /// </summary>
    public class URBasicInfoService
    {
        private URLoginService loginService = new URLoginService();

        /// <summary>
        /// 获取REPORT系统ID
        /// </summary>
        public static int REPORT_SYS_ID
        {
            get { return DACommonHelper.REPORT_SYS_ID; }
        }

        #region 用户信息相关

        /// <summary>
        /// 添加用户信息
        /// </summary>
        /// <param name="user"></param>
        public void AddUser(User user)
        {
            user.ID = 0;
            //权限判断
            loginService.HaveAdminRightForUserEdit(user);

            user.AdminUserID = loginService.LoginUser.ID;
            DABasicInfoHelper.AddUser(user);

            //记录登录日志
            loginService.AddLog(
                "AddUser",
                string.Format("添加用户(Account={0},AccountType={1},TrueName={2})"
                                , user.Account, user.AccountType, user.TrueName));            
        }

        /// <summary>
        /// 删除用户信息
        /// </summary>
        /// <param name="userId"></param>
        public void DeleteUser(int userId)
        {
            //权限判断
            User user = loginService.HaveAdminRightForUserEdit(userId, false);

            //如果用户允许访问的系统不在管理员可管理的系统列表里，则就不允许编辑该用户
            List<int> userSysIds = DARightsHelper.GetUserSystems(userId).Select(a => a.SystemID).ToList();
            if (userSysIds.Except(loginService.AdminSystems.Select(a => a.ID)).Count() > 0)
                throw new NotRightException();

            DABasicInfoHelper.DeleteUser(userId);

            string userSysIdsString = string.Join(",", userSysIds.ToArray());
            //记录登录日志
            loginService.AddLog(
                "DeleteUser",
                string.Format("删除用户(ID={0},Account={1},AccountType={2},TrueName={3},SysIds={4})"
                                , userId, user.Account, user.AccountType, user.TrueName, userSysIdsString));
        }

        /// <summary>
        /// 根据用户ID获取用户信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public User GetUser(int userId)
        {
            //权限判断
            User user = loginService.HaveAdminRightForUserEdit(userId);

            return user;
        }

        /// <summary>
        /// 更新用户信息
        /// </summary>
        /// <param name="user"></param>
        public void UpdateUser(User user)
        {
            //权限判断
            User oldUser = loginService.HaveAdminRightForUserEdit(user.ID);
            loginService.HaveAdminRightForUserEdit(user);

            DABasicInfoHelper.UpdateUser(user);

            //记录登录日志
            loginService.AddLog(
                "UpdateUser",
                string.Format(
                    "更新用户(ID={0},Account={1},AccountType={2}(Old={3}),TrueName={4}(Old={5}),EndTime={6}(Old={7}),IsSpecialUser={8}(Old={9}))",
                    user.ID, user.Account,
                    user.AccountType, oldUser.AccountType, user.TrueName, oldUser.TrueName, user.EndTime,
                    oldUser.EndTime, user.IsSpecialUser, oldUser.IsSpecialUser));
        }

        /// <summary>
        /// 更新用户状态
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="status"></param>
        public void UpdateUserStatus(int userId, StatusOptions status)
        {
            User user = DABasicInfoHelper.GetUser(userId);
            user.Status = status;
            UpdateUser(user);
        }

        /// <summary>
        /// 分页获取用户信息列表
        /// </summary>
        /// <param name="sysId"></param>
        /// <param name="status"></param>
        /// <param name="accountType"></param>
        /// <param name="onlyWhiteUser"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="recordCount"></param>
        /// <returns></returns>
        public List<User> GetUsers(int sysId, StatusOptions status, UserTypeOptions accountType, bool onlyWhiteUser, int pageIndex, int pageSize, ref int recordCount)
        {
            return GetUsers(sysId, status, accountType, string.Empty, onlyWhiteUser, pageIndex, pageSize, ref recordCount);
        }

        /// <summary>
        /// 根据账号或姓名模糊查找,并分页返回用户信息列表
        /// </summary>
        /// <param name="sysId"></param>
        /// <param name="status"></param>
        /// <param name="accountType"></param>
        /// <param name="keyword"></param>
        /// <param name="onlyWhiteUser"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="recordCount"></param>
        /// <returns></returns>
        public List<User> GetUsers(int sysId, StatusOptions status, UserTypeOptions accountType, string keyword, bool onlyWhiteUser, int pageIndex, int pageSize, ref int recordCount)
        {
            //权限判断
            loginService.HaveAdminRight(sysId, true);

            //if (loginService.LoginUser.AccountType == UserTypeOptions.ProductAdmin)
            //{
            //    return DABasicInfoHelper.GetUsers(sysId, status, accountType, keyword, onlyWhiteUser, pageIndex, pageSize, ref recordCount);
            //}
            return DABasicInfoHelper.GetUsers(sysId, status, accountType, keyword, onlyWhiteUser, pageIndex, pageSize, ref recordCount);
        }

        /// <summary>
        /// 获取REPORT所有用户
        /// </summary>
        /// <param name="userSofts"></param>
        /// <param name="allSofts"></param>
        /// <returns></returns>
        public List<User> GetReportUsers(out Dictionary<int, HashSet<int>> userSofts, out List<Soft> allSofts)
        {
            //权限判断
            loginService.HaveSuperAdminRight();

            return DABasicInfoHelper.GetReportUsers(out userSofts, out allSofts);
        }

        /// <summary>
        /// 获取账号类型说明
        /// </summary>
        /// <param name="userType"></param>
        /// <returns></returns>
        public static string GetUserTypeDescipt(UserTypeOptions userType)
        {
            string accType;
            switch (userType)
            {
                case UserTypeOptions.Admin:
                    accType = "管理员";
                    break;
                case UserTypeOptions.Channel:
                    accType = "渠道内部用户";
                    break;
                case UserTypeOptions.ChannelPartner:
                    accType = "渠道合作方";
                    break;
                case UserTypeOptions.General:
                    accType = "普通用户";
                    break;
                case UserTypeOptions.ProductAdmin:
                    accType = "产品管理员";
                    break;
                case UserTypeOptions.SuperAdmin:
                    accType = "超级管理员";
                    break;
                default:
                    accType = string.Empty;
                    break;
            }
            return accType;
        }

        /// <summary>
        /// 获取指定产品的用户列表
        /// </summary>
        /// <param name="softId"></param>
        /// <param name="accountTypes"></param>
        /// <returns></returns>
        public List<User> GetUsersBySoft(int softId, UserTypeOptions[] accountTypes)
        {
            //权限判断
            loginService.HaveAdminRight(DACommonHelper.REPORT_SYS_ID);

            if (loginService.LoginUser.AccountType == UserTypeOptions.ProductAdmin
                && !loginService.AvailableSofts.Exists(a => a.ID == softId))
                throw new NotRightException();

            return DABasicInfoHelper.GetUsersBySoft(softId, accountTypes);
        }

        #endregion

        #region 系统信息相关

        /// <summary>
        /// 添加系统信息
        /// </summary>
        /// <param name="system"></param>
        public void AddSystem(SystemInfo system)
        {
            //权限判断
            loginService.HaveSuperAdminRight();

            system.Md5Key = Guid.NewGuid().ToString();
            DABasicInfoHelper.AddSystem(system);

            loginService.ReloadAdminSystems();

            //记录日志
            loginService.AddLog(
                "AddSystem",
                string.Format("添加系统(Name={0},Md5Key={1})", system.Name, system.Md5Key));
            
        }

        /// <summary>
        /// 删除系统信息
        /// </summary>
        /// <param name="sysId"></param>
        public void DeleteSystem(int sysId)
        {
            //权限判断
            loginService.HaveSuperAdminRight();

            SystemInfo system = DABasicInfoHelper.GetSystem(sysId);
            if (system == null)
                throw new NotRightException();

            DABasicInfoHelper.DeleteSystem(sysId);

            loginService.ReloadAdminSystems();

            //记录日志
            loginService.AddLog(
                "DeleteSystem",
                string.Format("删除系统(ID={0},Name={1},Md5Key={2})", system.ID, system.Name, system.Md5Key));
        }

        /// <summary>
        /// 更新系统信息
        /// </summary>
        /// <param name="system"></param>
        public void UpdateSystem(SystemInfo system)
        {
            //权限判断
            loginService.HaveSuperAdminRight();

            SystemInfo oldSystem = DABasicInfoHelper.GetSystem(system.ID);
            if (oldSystem == null)
                throw new NotRightException();

            DABasicInfoHelper.UpdateSystem(system);

            loginService.ReloadAdminSystems();

            //记录日志
            loginService.AddLog(
                "UpdateSystem",
                string.Format("更新系统(ID={0},Name={1}(Old={2}),Md5Key={3})", system.ID, system.Name, oldSystem.Name, system.Md5Key));
        }

        /// <summary>
        /// 获取系统信息
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        public SystemInfo GetSystem(int sysId)
        {
            //权限判断
            loginService.HaveSuperAdminRight();

            SystemInfo system = DABasicInfoHelper.GetSystem(sysId);
            if (system == null)
                throw new NotRightException();

            return system;
        }

        /// <summary>
        /// 获取系统信息列表
        /// </summary>
        /// <returns></returns>
        public List<SystemInfo> GetSystems()
        {
            //权限判断
            loginService.HaveSuperAdminRight();

            return DABasicInfoHelper.GetSystems(); ;
        }

        #endregion

        #region 角色信息相关

        /// <summary>
        /// 添加角色信息
        /// </summary>
        /// <param name="role"></param>
        public void AddRole(Role role)
        {
            role.ID = 0;
            //权限判断
            loginService.HaveAdminRightForRole(role);

            //只有超级管理员才能添加系统角色
            if (role.RoleType != RoleTypeOptions.General &&
                loginService.LoginUser.AccountType != UserTypeOptions.SuperAdmin)
                throw new NotRightException();

            DABasicInfoHelper.AddRole(role);

            //记录登录日志
            loginService.AddLog(
                "AddRole",
                string.Format("添加角色(Name={0})", role.Name));

        }

        /// <summary>
        /// 删除角色信息
        /// </summary>
        /// <param name="roleId"></param>
        public void DeleteRole(int roleId)
        {
            //权限判断
            Role role = loginService.HaveAdminRightForRole(roleId);

            DABasicInfoHelper.DeleteRole(roleId);

            //记录登录日志
            loginService.AddLog(
                "DeleteRole",
                string.Format("删除角色(ID={0},Name={1})", roleId, role.Name));
        }

        /// <summary>
        /// 根据角色ID获取角色信息
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public Role GetRole(int roleId)
        {
            //权限判断
            Role role = loginService.HaveAdminRightForRole(roleId);

            return role;
        }

        /// <summary>
        /// 更新角色信息
        /// </summary>
        /// <param name="role"></param>
        public void UpdateRole(Role role)
        {
            //权限判断
            Role oldRole = loginService.HaveAdminRightForRole(role.ID);

            //只有超级管理员才能添加系统角色
            if (role.RoleType != RoleTypeOptions.General &&
                loginService.LoginUser.AccountType != UserTypeOptions.SuperAdmin)
                throw new NotRightException();

            DABasicInfoHelper.UpdateRole(role);

            //记录登录日志
            loginService.AddLog(
                "UpdateRole",
                string.Format("更新角色(ID={0},Name={1}(Old={2}))", role.ID, role.Name, oldRole.Name));
        }

        /// <summary>
        /// 获取角色列表
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        public List<Role> GetRoles(int sysId)
        {
            //权限判断
            loginService.HaveAdminRight(sysId);

            List<Role> roles = DABasicInfoHelper.GetRoles(sysId);
            if (loginService.LoginUser.AccountType == UserTypeOptions.ProductAdmin)
            {
                List<int> rangeRoles = DARightsHelper.GetUserRoles(sysId, loginService.LoginUser.ID);
                var availableRoles = from r in roles
                                     join rr in rangeRoles
                                         on r.ID equals rr
                                     select r;
                return availableRoles.ToList();
            }
            return roles;
        }

        /// <summary>
        /// 获取角色列表
        /// </summary>
        /// <param name="sysId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="recordCount"></param>
        /// <returns></returns>
        public List<Role> GetRoles(int sysId, int pageIndex, int pageSize, ref int recordCount)
        {
            //权限判断
            loginService.HaveAdminRight(sysId);

            return DABasicInfoHelper.GetRoles(sysId, pageIndex, pageSize, ref recordCount);
        }

        #endregion

        #region 权限信息相关

        /// <summary>
        /// 添加权限信息
        /// </summary>
        /// <param name="right"></param>
        public void AddRight(Right right)
        {
            //权限判断
            loginService.HaveAdminRight(right.SystemID, false);

            DABasicInfoHelper.AddRight(right);

            //记录登录日志
            loginService.AddLog(
                "AddRight",
                string.Format("添加权限(Name={0},PageUrl={1})", right.Name, right.PageUrl));
            
        }

        /// <summary>
        /// 删除权限信息
        /// </summary>
        /// <param name="rightId"></param>
        public void DeleteRight(int rightId)
        {   
            Right right = DABasicInfoHelper.GetRight(rightId);
            if (right == null)
                throw new NotRightException();

            //权限判断
            loginService.HaveAdminRight(right.SystemID, false);

            DABasicInfoHelper.DeleteRight(rightId);

            //记录登录日志
            loginService.AddLog(
                "DeleteRight",
                string.Format("删除权限(ID={0},Name={1},SysID={2},PageUrl={3})", right.ID, right.Name, right.SystemID, right.PageUrl));
        }

        /// <summary>
        /// 更新权限信息
        /// </summary>
        /// <param name="right"></param>
        public void UpdateRight(Right right)
        {
            //权限判断
            loginService.HaveAdminRight(right.SystemID, false);

            Right oldRight = DABasicInfoHelper.GetRight(right.ID);
            if (oldRight == null)
                throw new NotRightException();

            //权限判断
            loginService.HaveAdminRight(oldRight.SystemID, false);

            DABasicInfoHelper.UpdateRight(right);

            //记录登录日志
            loginService.AddLog(
                "UpdateRight",
                string.Format("更新权限(ID={0},Name={1}(Old={2}),SysID={3},PageUrl={4})", right.ID, right.Name, oldRight.SystemID, oldRight.Name,
                              right.PageUrl));
        }

        /// <summary>
        /// 获取权限信息
        /// </summary>
        /// <returns></returns>
        public Right GetRight(int rightId)
        {
            Right right = DABasicInfoHelper.GetRight(rightId);
            if (right == null)
                throw new NotRightException();

            //权限判断
            loginService.HaveAdminRight(right.SystemID, false);

            return right;
        }

        /// <summary>
        /// 获取权限信息列表
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        public List<Right> GetRights(int sysId)
        {
            //权限判断
            loginService.HaveAdminRight(sysId);

            List<Right> rights = DABasicInfoHelper.GetRights(sysId, -1);
            if (loginService.LoginUser.AccountType == UserTypeOptions.ProductAdmin)
            {
                List<RightItem> rangeRights = DARightsHelper.GetUserRights(sysId, loginService.LoginUser.ID, loginService.LoginUser.AccountType);
                var availableRights = from r in rights
                                      join rr in rangeRights
                                          on r.ID equals rr.RightID
                                      select r;
                return availableRights.ToList();
            }
            return rights;
        }

        /// <summary>
        /// 获取权限信息列表
        /// </summary>
        /// <param name="sysId"></param>
        /// <param name="parentId"></param>
        /// <returns></returns>
        public List<Right> GetRights(int sysId, int parentId)
        {
            //权限判断
            loginService.HaveAdminRight(sysId);

            return DABasicInfoHelper.GetRights(sysId, parentId);
        }

        #endregion

        #region 产品信息相关

        /// <summary>
        /// 添加产品信息
        /// </summary>
        /// <param name="soft"></param>
        public void AddSoft(Soft soft)
        {
            //权限判断
            loginService.HaveSuperAdminRight();

            DABasicInfoHelper.AddSoft(soft);

            //记录登录日志
            loginService.AddLog(
                "AddSoft",
                string.Format("添加产品(ID={0},OutID={1},Name={2},SoftType={3})", soft.ID, soft.OutID, soft.Name,
                              soft.SoftType));
        }

        /// <summary>
        /// 删除产品信息
        /// </summary>
        /// <param name="softId"></param>
        public void DeleteSoft(int softId)
        {
            //权限判断
            loginService.HaveSuperAdminRight();

            Soft soft = DABasicInfoHelper.GetSoft(softId);
            if (soft == null)
                throw new NotRightException();

            DABasicInfoHelper.DeleteSoft(softId);

            //记录登录日志
            loginService.AddLog(
                "DeleteSoft",
                string.Format("删除产品(ID={0},OutID={1},Name={2},SoftType={3})", soft.ID, soft.OutID, soft.Name,
                              soft.SoftType));
        }

        /// <summary>
        /// 更新产品信息
        /// </summary>
        /// <param name="soft"></param>
        public void UpdateSoft(Soft soft)
        {
            //权限判断
            loginService.HaveSuperAdminRight();

            Soft oldSoft = DABasicInfoHelper.GetSoft(soft.ID);
            if (oldSoft == null)
                throw new NotRightException();

            DABasicInfoHelper.UpdateSoft(soft);

            //记录登录日志
            loginService.AddLog(
                "UpdateSoft",
                string.Format("更新产品(ID={0},OutID={1}(Old={2}),Name={3}(Old={4}),SoftType={5}(Old={6}))", soft.ID,
                              soft.OutID, oldSoft.OutID, soft.Name, oldSoft.Name, soft.SoftType, oldSoft.SoftType));
        }

        /// <summary>
        /// 根据ID获取产品信息
        /// </summary>
        /// <param name="softId"></param>
        /// <returns></returns>
        public Soft GetSoft(int softId)
        {
            ////权限判断
            //loginService.HaveSuperAdminRight();

            Soft soft = DABasicInfoHelper.GetSoft(softId);
            //if (soft == null)
            //    throw new NotRightException();

            return soft;
        }

        /// <summary>
        /// 获取软件信息列表
        /// </summary>
        /// <param name="softType"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="recordCount"></param>
        /// <returns></returns>
        public List<Soft> GetSofts()
        {
            //权限判断
            loginService.HaveAdminRight(DACommonHelper.REPORT_SYS_ID);

            List<Soft> softs = DABasicInfoHelper.GetSofts();
            if (loginService.LoginUser.AccountType == UserTypeOptions.ProductAdmin)
            {
                List<RightItem> rangeRights = DARightsHelper.GetUserSoftRights(loginService.LoginUser.ID);
                var availableRights = from r in softs
                                      join rr in rangeRights
                                          on r.ID equals rr.RightID
                                      select r;
                return availableRights.ToList();
            }
            return softs;
        }

        /// <summary>
        /// 获取软件信息列表
        /// </summary>
        /// <param name="softType"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="recordCount"></param>
        /// <returns></returns>
        public List<Soft> GetSofts(SoftTypeOptions softType, int pageIndex, int pageSize, ref int recordCount)
        {
            return GetSofts(softType, string.Empty, pageIndex, pageSize, ref recordCount);
        }

        /// <summary>
        /// 获取软件信息列表
        /// </summary>
        /// <param name="softType"></param>
        /// <param name="keyword"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="recordCount"></param>
        /// <returns></returns>
        public List<Soft> GetSofts(SoftTypeOptions softType, string keyword, int pageIndex, int pageSize,
                                   ref int recordCount)
        {
            //权限判断
            loginService.HaveSuperAdminRight();

            return DABasicInfoHelper.GetSofts(softType, keyword, pageIndex, pageSize, ref recordCount);
        }

        #endregion

        #region 项目来源信息相关

        /// <summary>
        /// 添加项目来源信息
        /// </summary>
        /// <param name="projectSource"></param>
        public void AddPrjectSource(ProjectSource projectSource)
        {
            //权限判断
            loginService.HaveSuperAdminRight();

            DABasicInfoHelper.AddProjectSource(projectSource);

            //记录登录日志
            loginService.AddLog(
                "AddPrjectSource",
                string.Format("添加项目来源(PrjectSource={0},Name={1})", projectSource.ProjectSourceID, projectSource.Name));
        }

        /// <summary>
        /// 删除项目来源信息
        /// </summary>
        /// <param name="projectSourceId"></param>
        /// <param name="softId"></param>
        public void DeleteProjectSource(int projectSourceId, int softId)
        {
            //权限判断
            loginService.HaveSuperAdminRight();

            ProjectSource projectSource = DABasicInfoHelper.GetProjectSource(projectSourceId, softId);
            if (projectSource == null)
                throw new NotRightException();

            DABasicInfoHelper.DeleteProjectSource(projectSourceId, softId);

            //记录登录日志
            loginService.AddLog(
                "DeleteProjectSource",
                string.Format("删除项目来源(PrjectSource={0},Name={1},SoftID={2})", projectSource.ProjectSourceID,
                              projectSource.Name, softId));
        }

        /// <summary>
        /// 更新项目来源信息
        /// </summary>
        /// <param name="projectSource"></param>
        public void UpdateProjectSource(ProjectSource projectSource)
        {
            //权限判断
            loginService.HaveSuperAdminRight();

            ProjectSource oldProjectSource = DABasicInfoHelper.GetProjectSource(projectSource.ProjectSourceID, projectSource.SoftID);
            if (oldProjectSource == null)
                throw new NotRightException();

            DABasicInfoHelper.UpdateProjectSource(projectSource);

            //记录登录日志
            loginService.AddLog(
                "UpdateProjectSource",
                string.Format("更新项目来源(PrjectSource={0},Name={1},SoftID={2}(Old={3}))", projectSource.ProjectSourceID, projectSource.Name, projectSource.SoftID, oldProjectSource.Name));
        }

        /// <summary>
        /// 根据ID获取项目来源信息
        /// </summary>
        /// <param name="projectSourceId"></param>
        /// <param name="softId"></param>
        /// <returns></returns>
        public ProjectSource GetProjectSource(int projectSourceId, int softId)
        {
            ////权限判断
            //loginService.HaveSuperAdminRight();

            ProjectSource projectSource = DABasicInfoHelper.GetProjectSource(projectSourceId, softId);
            //if (projectSource == null)
            //    throw new NotRightException();

            return projectSource;
        }

        /// <summary>
        /// 获取项目来源信息列表
        /// </summary>
        /// <param name="mustAdmin"></param>
        /// <returns></returns>
        public List<ProjectSource> GetProjectSources(bool mustAdmin = true)
        {
            if (mustAdmin)
            {
                //权限判断
                loginService.HaveAdminRight(DACommonHelper.REPORT_SYS_ID);

                List<ProjectSource> projectSources = DABasicInfoHelper.GetProjectSources();
                if (loginService.LoginUser.AccountType == UserTypeOptions.ProductAdmin)
                {
                    List<RightItem> rangeRights = DARightsHelper.GetUserProjectSourceRights(loginService.LoginUser.ID);
                    var availableRights = from r in projectSources
                                          join rr in rangeRights
                                              on r.ProjectSourceID equals rr.RightID
                                          select r;
                    return availableRights.ToList();
                }
                return projectSources;
            }
            return DABasicInfoHelper.GetProjectSources();
        }

        #endregion

        #region 操作日志相关

        /// <summary>
        /// 获取操作日志列表
        /// </summary>
        /// <param name="sysId"></param>
        /// <param name="keyword"></param>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="recordCount"></param>
        /// <returns></returns>
        public List<AdminLog> GetAdminLogs(int sysId, string keyword, DateTime beginTime, DateTime endTime, int pageIndex, int pageSize, ref int recordCount)
        {
            //权限判断
            loginService.HaveAdminRight(sysId, false);

            return DABasicInfoHelper.GetAdminLogs(sysId, keyword, beginTime, endTime, pageIndex, pageSize, ref recordCount);
        }

        #endregion
    }
}