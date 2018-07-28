using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;
using net91com.Core;
using net91com.Core.Web;
using net91com.Core.Data;

namespace net91com.Reports.UserRights
{
    /// <summary>
    /// 基本信息数据存取(用户/角色/权限等基本信息的增删改查)
    /// </summary>
    internal class DABasicInfoHelper
    {
        #region 用户信息相关

        /// <summary>
        /// 添加用户信息
        /// </summary>
        /// <param name="user"></param>
        public static void AddUser(User user)
        {
            string cmdText = @"insert into R_Users(`Account`,TrueName,AccountType,`Status`,AdminUserID,BeginTime,EndTime,IsSpecialUser,Email,IsWhiteUser,Dept) 
                               select ?Account,?TrueName,?AccountType,?Status,?AdminUserID,?BeginTime,?EndTime,?IsSpecialUser,?Email,?IsWhiteUser,?Dept
                               from dual
                               where not exists(select * from R_Users where `Account`=?Account);";
            MySqlParameter[] parameters = new MySqlParameter[]
            {
                new MySqlParameter("?Account", user.Account),
                new MySqlParameter("?TrueName", user.TrueName),
                new MySqlParameter("?AccountType", (int) user.AccountType),
                new MySqlParameter("?Status", (int) user.Status),
                new MySqlParameter("?AdminUserID", user.AdminUserID),
                new MySqlParameter("?BeginTime", user.BeginTime),
                new MySqlParameter("?EndTime", user.EndTime),
                new MySqlParameter("?IsSpecialUser", user.IsSpecialUser ? 1 : 0),
                new MySqlParameter("?Email", user.Email),
                new MySqlParameter("?IsWhiteUser", user.IsWhiteUser ? 1 : 0),
                new MySqlParameter("?Dept", user.Department)
            };
            int rowCount = MySqlHelper.ExecuteNonQuery(DACommonHelper.ConnectionString, cmdText, parameters);
            if (rowCount == 0)
                throw new ToUserException("用户已经存在!");
        }

        /// <summary>
        /// 删除用户信息
        /// </summary>
        /// <param name="userId"></param>
        public static void DeleteUser(int userId)
        {
            string cmdText = @"delete from R_Users where ID=?ID;
                               delete from R_UserSystems where UserID=?ID;
                               delete from R_UserRoles where UserID=?ID;
                               delete from R_UserRights where UserID=?ID;
                               delete from R_UserSoftRights where UserID=?ID;
                               delete from R_UserProjectSourceRights where UserID=?ID;
                               delete from R_UserResRights where UserID=?ID;
                               delete from R_UserChannelRights where UserID=?ID;";
            MySqlParameter[] parameters = new MySqlParameter[] { new MySqlParameter("?ID", userId) } ;
            MySqlHelper.ExecuteNonQuery(DACommonHelper.ConnectionString, cmdText, parameters);
        }

        /// <summary>
        /// 根据用户ID获取用户信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static User GetUser(int userId)
        {
            string cmdText = "select ID,`Account`,TrueName,AccountType,`Status`,LastLoginTime,AdminUserID,BeginTime,EndTime,IsSpecialUser,Email,IsWhiteUser,Dept from R_Users where ID=?ID";
            MySqlParameter[] parameters = new MySqlParameter[] { new MySqlParameter("?ID", userId) };
            using (MySqlDataReader reader = MySqlHelper.ExecuteReader(DACommonHelper.ConnectionString, cmdText, parameters))
            {
                if (reader.Read())
                {
                    return BindUser(reader);
                }
            }
            return null;
        }

        /// <summary>
        /// 根据账号获取用户信息
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public static User GetUser(string account)
        {
            string cmdText = "select ID,`Account`,TrueName,Email,AccountType,`Status`,LastLoginTime,AdminUserID,IsSpecialUser,IsWhiteUser,Dept,BeginTime,EndTime from R_Users where Account=?Account";
            MySqlParameter[] parameters = new MySqlParameter[] { new MySqlParameter("?Account", account) };
            using (MySqlDataReader reader = MySqlHelper.ExecuteReader(DACommonHelper.ConnectionString, cmdText, parameters))
            {
                if (reader.Read())
                {
                    return BindUser(reader);
                }
            }
            return null;
        }

        /// <summary>
        /// 更新用户信息
        /// </summary>
        /// <param name="user"></param>
        public static void UpdateUser(User user)
        {
            string cmdText = @"update R_Users set TrueName=?TrueName,AccountType=?AccountType,`Status`=?Status,BeginTime=?BeginTime 
                                    ,EndTime=?EndTime,IsSpecialUser=?IsSpecialUser,Email=?Email,IsWhiteUser=?IsWhiteUser,Dept=?Dept 
                               where ID=?ID";
            MySqlParameter[] parameters = new MySqlParameter[]
            {
                new MySqlParameter("?ID", user.ID),
                new MySqlParameter("?TrueName", user.TrueName),
                new MySqlParameter("?AccountType", (int) user.AccountType),
                new MySqlParameter("?Status", (int) user.Status),
                new MySqlParameter("?BeginTime", user.BeginTime),
                new MySqlParameter("?EndTime", user.EndTime),
                new MySqlParameter("?IsSpecialUser", user.IsSpecialUser ? 1 : 0),
                new MySqlParameter("?Email", user.Email),
                new MySqlParameter("?IsWhiteUser", user.IsWhiteUser ? 1 : 0),
                new MySqlParameter("?Dept", user.Department),
            };
            MySqlHelper.ExecuteNonQuery(DACommonHelper.ConnectionString, cmdText, parameters);
        }

        /// <summary>
        /// 更新用户最后一次登录时间
        /// </summary>
        /// <param name="sysId"></param>
        /// <param name="userId"></param>
        public static void UpdateLastLoginTime(int userId)
        {
            string cmdText = @"update R_Users set LastLoginTime=now() where ID=?ID;
                               update R_UserSystems set LastLoginTime=now() where UserID=?ID and SysID=?SysID;";
            MySqlParameter[] parameters = new MySqlParameter[] 
            {
                new MySqlParameter("?ID", userId),
                new MySqlParameter("?SysID", DACommonHelper.REPORT_SYS_ID)
            };
            MySqlHelper.ExecuteNonQuery(DACommonHelper.ConnectionString, cmdText, parameters);
        }

        /// <summary>
        /// 更新指定系统用户最后一次登录时间
        /// </summary>
        /// <param name="sysId"></param>
        /// <param name="userId"></param>
        public static void UpdateSystemLastLoginTime(int sysId, int userId)
        {
            string cmdText = @"update R_UserSystems set LastLoginTime=now() where UserID=?ID and SysID=?SysID;";
            MySqlParameter[] parameters = new MySqlParameter[]
            {
                new MySqlParameter("?ID", userId),
                new MySqlParameter("?SysID", sysId)
            };
            MySqlHelper.ExecuteNonQuery(DACommonHelper.ConnectionString, cmdText, parameters);
        }

        /// <summary>
        /// 获取所有用户
        /// </summary>
        /// <param name="userSofts"></param>
        /// <param name="allSofts"></param>
        /// <returns></returns>
        public static List<User> GetReportUsers(out Dictionary<int, HashSet<int>> userSofts, out List<Soft> allSofts)
        {
            string cmdText = @"select ID,`Account`,TrueName,Email,AccountType,`Status`,A.LastLoginTime,AdminUserID,IsSpecialUser,IsWhiteUser,Dept,BeginTime,EndTime from R_Users A inner join R_UserSystems B on A.ID=B.UserID and B.SysID=" + DACommonHelper.REPORT_SYS_ID.ToString();
            List<User> users = new List<User>();
            using (MySqlDataReader reader = MySqlHelper.ExecuteReader(DACommonHelper.ConnectionString, cmdText))
            {
                while (reader.Read())
                {
                    users.Add(BindUser(reader));
                }
            }            
            userSofts = new Dictionary<int, HashSet<int>>();
            allSofts = new List<Soft>();
            cmdText = @"select UserID,SoftID from R_UserSoftRights
                        union
                        select UserID,SoftID from R_RoleSoftRights A inner join R_UserRoles B on A.RoleID=B.RoleID";
            using (MySqlDataReader reader = MySqlHelper.ExecuteReader(DACommonHelper.ConnectionString, cmdText))
            {
                while (reader.Read())
                {
                    int userId = Convert.ToInt32(reader["UserID"]);
                    if (!userSofts.ContainsKey(userId))
                        userSofts[userId] = new HashSet<int>();
                    int softId = Convert.ToInt32(reader["SoftID"]);
                    userSofts[userId].Add(softId);
                }
            }
            cmdText = @"select ID,OutID,`Name`,SoftType,OnlyInternal,SortIndex,`Status`,Platforms,AddTime,SoftAreaType,StatAloneID from R_Softs
                        where ID in (select SoftID from R_UserSoftRights union select SoftID from R_RoleSoftRights) order by SoftType,SortIndex desc";
            using (MySqlDataReader reader = MySqlHelper.ExecuteReader(DACommonHelper.ConnectionString, cmdText))
            {
                while (reader.Read())
                {
                    allSofts.Add(BindSoft(reader));
                }
            }
            return users;
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
        public static List<User> GetUsers(int sysId, StatusOptions status, UserTypeOptions accountType, 
                                          string keyword, bool onlyWhiteUser, int pageIndex, int pageSize, ref int recordCount)
        {
            string where = "where `Status`=?Status and AccountType=?AccountType";
            where += string.IsNullOrEmpty(keyword) ? "" : string.Format(" and (`Account` like '%{0}%' or TrueName like '%{0}%')", keyword.Replace("'", "\\'"));
            //产品管理员可管理的用户不在限定于自己添加的用户
            //where += adminUserId > 0 ? " and AdminUserID=?AdminUserID" : string.Empty;
            where += onlyWhiteUser ? " and IsWhiteUser=1" : string.Empty;
            where += sysId > 0 ? " and ID in (select UserID from R_UserSystems where SysID=?SysID)" : string.Empty;
            string cmdText = "select count(*) from R_Users " + where;
            MySqlParameter[] parameters = new MySqlParameter[]
            {
                new MySqlParameter("?Status", (int)status),
                new MySqlParameter("?AccountType", (int)accountType),
                //new MySqlParameter("?AdminUserID", adminUserId),
                new MySqlParameter("?SysID", sysId)
            };
            object result = MySqlHelper.ExecuteScalar(DACommonHelper.ConnectionString, cmdText, parameters);
            recordCount = Convert.ToInt32(result);
            cmdText = string.Format(@"select ID,`Account`,TrueName,Email,AccountType,`Status`,LastLoginTime,AdminUserID,IsSpecialUser,IsWhiteUser,Dept,BeginTime,EndTime  
                                      from R_Users " + where + @" order by LastLoginTime desc limit {0},{1}",
                                      (pageIndex - 1)*pageSize, pageSize);
            List<User> users = new List<User>();
            using (MySqlDataReader reader = MySqlHelper.ExecuteReader(DACommonHelper.ConnectionString, cmdText, parameters))
            {
                while (reader.Read())
                {
                    users.Add(BindUser(reader));
                }
            }
            return users;
        }

        /// <summary>
        /// 获取指定产品可给予分配渠道权限的用户列表
        /// </summary>
        /// <param name="softId"></param>
        /// <param name="accountTypes"></param>
        /// <returns></returns>
        public static List<User> GetUsersBySoft(int softId, UserTypeOptions[] accountTypes)
        {
            string accountTypesString = string.Join(",", accountTypes.Select(a => ((int) a).ToString()).ToArray());
            accountTypesString = string.IsNullOrEmpty(accountTypesString) ? string.Empty : " and A.AccountType in (" + accountTypesString + ")";
            string cmdText = @"select A.ID,A.`Account`,A.TrueName,A.Email,A.AccountType,A.`Status`,A.LastLoginTime,A.AdminUserID,A.IsSpecialUser,A.IsWhiteUser,A.Dept,A.BeginTime,A.EndTime from R_Users A inner join R_UserSoftRights B on A.ID=B.`UserID` and B.SoftID=?SoftID" + accountTypesString + @" and A.`Status`=1
                               union 
                               select distinct A.ID,A.`Account`,A.TrueName,A.Email,A.AccountType,A.`Status`,A.LastLoginTime,A.AdminUserID,A.IsSpecialUser,A.IsWhiteUser,A.Dept,A.BeginTime,A.EndTime from R_Users A inner join R_UserRoles B on A.ID=B.`UserID`" + accountTypesString + @" 
                                    and A.`Status`=1 inner join R_RoleSoftRights C on B.RoleID=C.RoleID and C.SoftID=?SoftID";
            MySqlParameter[] parameters = new MySqlParameter[] { new MySqlParameter("?SoftID", softId) };
            List<User> users = new List<User>();
            using (MySqlDataReader reader = MySqlHelper.ExecuteReader(DACommonHelper.ConnectionString, cmdText, parameters))
            {
                while (reader.Read())
                {
                    users.Add(BindUser(reader));
                }
            }
            return users;
        }

        /// <summary>
        /// 绑定用户信息
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private static User BindUser(MySqlDataReader reader)
        {
            User user = new User();
            user.ID = Convert.ToInt32(reader["ID"]);
            user.Account = reader["Account"].ToString();
            user.TrueName = reader["TrueName"] == null || reader["TrueName"] == DBNull.Value ? string.Empty : reader["TrueName"].ToString();
            user.Email = reader["Email"] == null || reader["Email"] == DBNull.Value ? string.Empty : reader["Email"].ToString();
            user.AccountType = (UserTypeOptions) Convert.ToInt32(reader["AccountType"]);
            user.Status = (StatusOptions) Convert.ToInt32(reader["Status"]);
            user.LastLoginTime = reader["LastLoginTime"] == null || reader["LastLoginTime"] == DBNull.Value ? DateTime.Now : Convert.ToDateTime(reader["LastLoginTime"]);
            user.AdminUserID = Convert.ToInt32(reader["AdminUserID"]);
            user.IsSpecialUser = Convert.ToInt32(reader["IsSpecialUser"]) > 0;
            user.IsWhiteUser = Convert.ToInt32(reader["IsWhiteUser"]) > 0;
            user.Department = reader["Dept"] == null || reader["Dept"] == DBNull.Value ? string.Empty : reader["Dept"].ToString();
            user.BeginTime = Convert.ToDateTime(reader["BeginTime"]);
            user.EndTime = Convert.ToDateTime(reader["EndTime"]);
            return user;
        }

        #endregion

        #region 系统信息相关

        /// <summary>
        /// 添加系统信息
        /// </summary>
        /// <param name="system"></param>
        public static void AddSystem(SystemInfo system)
        {
            string cmdText = @"insert into R_Systems(`Name`,`Md5Key`,`AddTime`,`Status`,`Description`,`Url`) 
                               select ?Name,?Md5Key,now(),?Status,?Description,?Url
                               from dual
                               where not exists(select * from R_Systems where `Name`=?Name);";
            MySqlParameter[] parameters = new MySqlParameter[]
            {
                new MySqlParameter("?Name", system.Name),
                new MySqlParameter("?Md5Key", system.Md5Key),
                new MySqlParameter("?Status", (int)system.Status),
                new MySqlParameter("?Description", system.Description),
                new MySqlParameter("?Url", system.Url)
            };
            int rowCount = MySqlHelper.ExecuteNonQuery(DACommonHelper.ConnectionString, cmdText, parameters);
            if (rowCount == 0)
                throw new ToUserException("系统信息已经存在!");

            //自动给超级管理员增加权限
            cmdText = @"insert into R_UserSystems(UserID,SysID,Admin,AddTime,LastLoginTime) 
                        select A.ID,B.ID,1,now(),now() from R_Users A,R_Systems B
                        where A.AccountType=2 and B.`Name`=?Name;";
            MySqlHelper.ExecuteNonQuery(DACommonHelper.ConnectionString, cmdText, parameters);
        }

        /// <summary>
        /// 删除系统信息
        /// </summary>
        /// <param name="sysId"></param>
        public static void DeleteSystem(int sysId)
        {
            string cmdText = @"delete from R_Systems where ID=?ID;
                               delete from R_UserSystems where SysID=?ID;
                               delete from R_UserRoles where RoleID in (select ID from R_Roles where SysID=?ID);
                               delete from R_Roles where SysID=?ID;
                               delete from R_UserRights where RightID in (select ID from R_Rights where SysID=?ID);
                               delete from R_RoleRights where RightID in (select ID from R_Rights where SysID=?ID);
                               delete from R_Rights where SysID=?ID;";
            MySqlParameter[] parameters = new MySqlParameter[] { new MySqlParameter("?ID", sysId) };
            MySqlHelper.ExecuteNonQuery(DACommonHelper.ConnectionString, cmdText, parameters);
        }

        /// <summary>
        /// 更新系统信息
        /// </summary>
        /// <param name="system"></param>
        public static void UpdateSystem(SystemInfo system)
        {
            string cmdText = @"update R_Systems set `Name`=?Name,`Status`=?Status,`Description`=?Description,`Url`=?Url where ID=?ID";
            MySqlParameter[] parameters = new MySqlParameter[]
            {
                new MySqlParameter("?Name", system.Name),
                new MySqlParameter("?Status", (int)system.Status),
                new MySqlParameter("?Description", system.Description),
                new MySqlParameter("?Url", system.Url),
                new MySqlParameter("?ID", system.ID),
            };
            MySqlHelper.ExecuteNonQuery(DACommonHelper.ConnectionString, cmdText, parameters);
        }

        /// <summary>
        /// 获取系统信息
        /// </summary>
        /// <param name="sysId"></param>
        /// <param name="cacheTime"></param>
        /// <returns></returns>
        public static SystemInfo GetSystem(int sysId, CacheTimeOption cacheTime = CacheTimeOption.None)
        {
            string cacheKey = string.Format("net91com.Reports.UserRights.DABasicInfoHelper.GetSystem-{0}", sysId);
            if (cacheTime != CacheTimeOption.None && CacheHelper.Contains(cacheKey))
                return CacheHelper.Get<SystemInfo>(cacheKey);

            SystemInfo result = null;
            string cmdText = "select ID,`Name`,`Md5Key`,`Status`,`AddTime`,`Status`,`Description`,`Url` from R_Systems where ID=?ID";
            MySqlParameter[] parameters = new MySqlParameter[] { new MySqlParameter("?ID", sysId) };
            using (MySqlDataReader reader = MySqlHelper.ExecuteReader(DACommonHelper.ConnectionString, cmdText, parameters))
            {
                if (reader.Read())
                {
                    result = BindSystem(reader);
                }
            }
            if (cacheTime != CacheTimeOption.None)
                CacheHelper.Set(cacheKey, result, cacheTime, CacheExpirationOption.AbsoluteExpiration);
            return result;
        }

        /// <summary>
        /// 获取所有系统信息
        /// </summary>
        /// <param name="cacheTime"></param>
        /// <param name="clearCache">是否强制清除缓存</param>
        /// <returns></returns>
        public static List<SystemInfo> GetSystems(CacheTimeOption cacheTime = CacheTimeOption.None, bool clearCache = false)
        {
            string cacheKey = string.Format("net91com.Reports.UserRights.DABasicInfoHelper.GetSystems");
            if (clearCache)
                CacheHelper.Remove(cacheKey);
            
            if (cacheTime != CacheTimeOption.None && CacheHelper.Contains(cacheKey))
                return CacheHelper.Get<List<SystemInfo>>(cacheKey);

            string cmdText = "select ID,`Name`,`Md5Key`,`Status`,`AddTime`,`Status`,`Description`,`Url` from R_Systems";
            List<SystemInfo> systems = new List<SystemInfo>();
            using (MySqlDataReader reader = MySqlHelper.ExecuteReader(DACommonHelper.ConnectionString, cmdText))
            {
                while (reader.Read())
                {
                    systems.Add(BindSystem(reader));
                }
            }
            if (cacheTime != CacheTimeOption.None)
                CacheHelper.Set(cacheKey, systems, cacheTime, CacheExpirationOption.AbsoluteExpiration);
            return systems;
        }        

        /// <summary>
        /// 绑定系统信息实体
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private static SystemInfo BindSystem(MySqlDataReader reader)
        {
            SystemInfo system = new SystemInfo();
            system.ID = Convert.ToInt32(reader["ID"]);
            system.Name = reader["Name"].ToString();
            system.Md5Key = reader["Md5Key"].ToString();
            system.AddTime = Convert.ToDateTime(reader["AddTime"]);
            system.Status = (StatusOptions)Convert.ToInt32(reader["Status"]);
            system.Description = reader["Description"] == null || reader["Description"] == DBNull.Value ? string.Empty : reader["Description"].ToString();
            system.Url = reader["Url"] == null || reader["Url"] == DBNull.Value ? string.Empty : reader["Url"].ToString();
            return system;
        }

        #endregion

        #region 角色信息相关

        /// <summary>
        /// 添加角色信息
        /// </summary>
        /// <param name="role"></param>
        public static void AddRole(Role role)
        {
            string cmdText = @"insert into R_Roles(`Name`,`Status`,`RoleType`,`Description`,`SysID`) 
                               select ?Name,?Status,?RoleType,?Description,?SysID 
                               from dual
                               where not exists(select * from R_Roles where `SysID`=?SysID and `Name`=?Name);";
            MySqlParameter[] parameters = new MySqlParameter[]
            {
                new MySqlParameter("?SysID", role.SystemID),
                new MySqlParameter("?Name", role.Name),
                new MySqlParameter("?Status", (int) role.Status),
                new MySqlParameter("?RoleType", (int) role.RoleType),
                new MySqlParameter("?Description", role.Description)
            };
            int rowCount = MySqlHelper.ExecuteNonQuery(DACommonHelper.ConnectionString, cmdText, parameters);
            if (rowCount == 0)
                throw new ToUserException("角色已经存在!");
        }

        /// <summary>
        /// 删除角色信息
        /// </summary>
        /// <param name="roleId"></param>
        public static void DeleteRole(int roleId)
        {
            string cmdText = @"delete from R_Roles where ID=?ID;
                               delete from R_RoleRights where RoleID=?ID;
                               delete from R_RoleSoftRights where RoleID=?ID;
                               delete from R_RoleProjectSourceRights where RoleID=?ID;
                               delete from R_UserRoles where RoleID=?ID;";
            MySqlParameter[] parameters = new MySqlParameter[] { new MySqlParameter("?ID", roleId) };
            MySqlHelper.ExecuteNonQuery(DACommonHelper.ConnectionString, cmdText, parameters);
        }

        /// <summary>
        /// 根据角色ID获取角色信息
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public static Role GetRole(int roleId)
        {
            string cmdText = "select ID,`Name`,`Status`,RoleType,`Description`,`SysID` from R_Roles where ID=?ID";
            MySqlParameter[] parameters = new MySqlParameter[] { new MySqlParameter("?ID", roleId) };
            using (MySqlDataReader reader = MySqlHelper.ExecuteReader(DACommonHelper.ConnectionString, cmdText, parameters))
            {
                if (reader.Read())
                {
                    return BindRole(reader);
                }
            }
            return null;
        }

        /// <summary>
        /// 更新角色信息
        /// </summary>
        /// <param name="role"></param>
        public static void UpdateRole(Role role)
        {
            string cmdText = "update R_Roles set `Name`=?Name,`Status`=?Status,RoleType=?RoleType,`Description`=?Description where ID=?ID";
            MySqlParameter[] parameters = new MySqlParameter[]
            {
                new MySqlParameter("?ID", role.ID),
                new MySqlParameter("?Name", role.Name),
                new MySqlParameter("?Status", (int) role.Status),
                new MySqlParameter("?RoleType", (int) role.RoleType),
                new MySqlParameter("?Description", role.Description)
            };
            MySqlHelper.ExecuteNonQuery(DACommonHelper.ConnectionString, cmdText, parameters);
        }

        /// <summary>
        /// 获取指定系统的角色列表
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        public static List<Role> GetRoles(int sysId)
        {
            string cmdText = "select ID,`Name`,`Status`,RoleType,`Description`,`SysID` from R_Roles where SysID=?SysID";
            MySqlParameter[] parameters = new MySqlParameter[] { new MySqlParameter("?SysID", sysId) };
            List<Role> roles = new List<Role>();
            using (MySqlDataReader reader = MySqlHelper.ExecuteReader(DACommonHelper.ConnectionString, cmdText, parameters))
            {
                while (reader.Read())
                {
                    roles.Add(BindRole(reader));
                }
            }
            return roles;
        }

        /// <summary>
        /// 获取指定系统的角色列表
        /// </summary>
        /// <param name="sysId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="recordCount"></param>
        /// <returns></returns>
        public static List<Role> GetRoles(int sysId, int pageIndex, int pageSize, ref int recordCount)
        {
            string cmdText = "select count(*) from R_Roles where SysID=?SysID";
            MySqlParameter[] parameters = new MySqlParameter[] { new MySqlParameter("?SysID", sysId) };
            object result = MySqlHelper.ExecuteScalar(DACommonHelper.ConnectionString, cmdText, parameters);
            recordCount = Convert.ToInt32(result);
            cmdText = string.Format(@"select ID,`Name`,`Status`,RoleType,`Description`,`SysID` from R_Roles where SysID=?SysID order by ID desc limit {0},{1}", (pageIndex - 1) * pageSize, pageSize);
            List<Role> roles = new List<Role>();
            using (MySqlDataReader reader = MySqlHelper.ExecuteReader(DACommonHelper.ConnectionString, cmdText, parameters))
            {
                while (reader.Read())
                {
                    roles.Add(BindRole(reader));
                }
            }
            return roles;
        }

        /// <summary>
        /// 绑定用户信息
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private static Role BindRole(MySqlDataReader reader)
        {
            Role role = new Role();
            role.ID = Convert.ToInt32(reader["ID"]);
            role.SystemID = Convert.ToInt32(reader["SysID"]);
            role.Name = reader["Name"].ToString();
            role.Status = (StatusOptions) Convert.ToInt32(reader["Status"]);
            role.RoleType = (RoleTypeOptions) Convert.ToInt32(reader["RoleType"]);
            role.Description = reader["Description"] == null || reader["Description"] == DBNull.Value ? string.Empty : reader["Description"].ToString();
            return role;
        }

        #endregion

        #region 权限信息相关

        /// <summary>
        /// 添加权限信息
        /// </summary>
        /// <param name="right"></param>
        public static void AddRight(Right right)
        {
            if (right.SystemID <= 0)
                return;

            string cmdText = @"update R_Rights set SortIndex=SortIndex+1 where SysID=?SysID and ParentID=?ParentID and SortIndex>=?SortIndex;

                            insert into R_Rights(`Name`,`Status`,ParentID,RightLevel,RightType,PageUrl,SortIndex,OnlyInternal,AddTime,LastUpdateTime,`Description`,`SysID`) 
                            values(?Name,?Status,?ParentID,?RightLevel,?RightType,?PageUrl,?SortIndex,?OnlyInternal,now(),now(),?Description,?SysID);";
            MySqlParameter[] parameters = new MySqlParameter[]
            {
                new MySqlParameter("?Name", right.Name),
                new MySqlParameter("?Status", (int) right.Status),
                new MySqlParameter("?ParentID", right.ParentID),
                new MySqlParameter("?RightLevel", right.RightLevel),
                new MySqlParameter("?RightType", (int) right.RightType),
                new MySqlParameter("?PageUrl", right.PageUrl),
                new MySqlParameter("?SortIndex", right.SortIndex),
                new MySqlParameter("?OnlyInternal", right.OnlyInternal ? 1 : 0),
                new MySqlParameter("?Description", right.Description),
                new MySqlParameter("?SysID", right.SystemID)
            };
            MySqlHelper.ExecuteNonQuery(DACommonHelper.ConnectionString, cmdText, parameters);
        }

        /// <summary>
        /// 删除权限信息
        /// </summary>
        /// <param name="rightId"></param>
        public static void DeleteRight(int rightId)
        {
            string cmdText = @"update R_Rights A inner join R_Rights B on A.SysID=B.SysID and A.ParentID=B.ParentID and A.SortIndex>B.SortIndex and B.ID=?ID
                               set A.SortIndex=A.SortIndex-1;

                             delete from R_Rights where ID=?ID;
                             delete from R_UserRights where RightID=?ID;
                             delete from R_RoleRights where RightID=?ID;";
            MySqlParameter[] parameters = new MySqlParameter[] { new MySqlParameter("?ID", rightId) };
            MySqlHelper.ExecuteNonQuery(DACommonHelper.ConnectionString, cmdText, parameters);
        }

        /// <summary>
        /// 更新权限信息
        /// </summary>
        /// <param name="right"></param>
        public static void UpdateRight(Right right)
        {
            string cmdText = @"
                         update R_Rights A inner join R_Rights B on A.SysID=B.SysID and A.ParentID=B.ParentID and B.ID=?ID
                            and A.SortIndex between B.SortIndex and ?SortIndex and B.SortIndex<?SortIndex
                         set A.SortIndex=A.SortIndex-1;

                         update R_Rights A inner join R_Rights B on A.SysID=B.SysID and A.ParentID=B.ParentID and B.ID=?ID
                            and A.SortIndex between ?SortIndex and B.SortIndex and B.SortIndex>?SortIndex
                         set A.SortIndex=A.SortIndex+1;

                         update R_Rights set `Name`=?Name,`Status`=?Status,PageUrl=?PageUrl,SortIndex=?SortIndex
                                ,OnlyInternal=?OnlyInternal,LastUpdateTime=now(),`Description`=?Description
                         where ID=?ID;";
            MySqlParameter[] parameters = new MySqlParameter[]
            {
                new MySqlParameter("?Name", right.Name),
                new MySqlParameter("?Status", (int) right.Status),
                new MySqlParameter("?PageUrl", right.PageUrl),
                new MySqlParameter("?SortIndex", right.SortIndex),
                new MySqlParameter("?OnlyInternal", right.OnlyInternal ? 1 : 0),
                new MySqlParameter("?Description", right.Description),
                new MySqlParameter("?ID", right.ID)
            };
            MySqlHelper.ExecuteNonQuery(DACommonHelper.ConnectionString, cmdText, parameters);
        }

        /// <summary>
        /// 获取权限信息
        /// </summary>
        /// <returns></returns>
        public static Right GetRight(int rightId)
        {
            string cmdText = "select ID,`Name`,`Status`,ParentID,RightLevel,RightType,PageUrl,SortIndex,OnlyInternal,AddTime,LastUpdateTime,`Description`,`SysID` from R_Rights where ID=?ID";
            MySqlParameter[] parameters = new MySqlParameter[] { new MySqlParameter("?ID", rightId) };
            using (MySqlDataReader reader = MySqlHelper.ExecuteReader(DACommonHelper.ConnectionString, cmdText, parameters))
            {
                if (reader.Read())
                {
                    return BindRight(reader);
                }
            }
            return null;
        }

        /// <summary>
        /// 获取权限信息列表
        /// </summary>
        /// <param name="sysId"></param>
        /// <param name="parentId">值为0表示该系统的顶级权限，值为负值表示该系统所有权限</param>
        /// <param name="cacheTime"></param>
        /// <returns></returns>
        public static List<Right> GetRights(int sysId, int parentId, CacheTimeOption cacheTime = CacheTimeOption.None)
        {
            if (parentId <= 0 && sysId == 0)
                return new List<Right>();
            string cacheKey = string.Format("net91com.Reports.UserRights.DABasicInfoHelper.GetRights-{0}-{1}", sysId, parentId);
            if (cacheTime != CacheTimeOption.None && CacheHelper.Contains(cacheKey))
                return CacheHelper.Get<List<Right>>(cacheKey);
            List<Right> rights;
            if (parentId == 0)
                rights = GetRights(string.Format("where SysID={0} and ParentID=0 or ParentID is null", sysId));
            else if (parentId < 0)
                rights = GetRights(string.Format("where SysID={0}", sysId));
            else
                rights = GetRights(string.Format("where SysID={0} and ParentID={1}", sysId, parentId));
            if (cacheTime != CacheTimeOption.None)
                CacheHelper.Set(cacheKey, rights, cacheTime, CacheExpirationOption.AbsoluteExpiration);
            return rights;
        }

        /// <summary>
        /// 获取权限信息列表
        /// </summary>
        /// <returns></returns>
        private static List<Right> GetRights(string where)
        {
            string cmdText = "select ID,`Name`,`Status`,ParentID,RightLevel,RightType,PageUrl,SortIndex,OnlyInternal,AddTime,LastUpdateTime,`Description`,`SysID` from R_Rights " + where + " order by SortIndex";
            List<Right> rights = new List<Right>();
            using (MySqlDataReader reader = MySqlHelper.ExecuteReader(DACommonHelper.ConnectionString, cmdText))
            {
                while (reader.Read())
                {
                    rights.Add(BindRight(reader));
                }
            }
            return rights;
        }

        /// <summary>
        /// 绑定权限
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private static Right BindRight(MySqlDataReader reader)
        {
            Right right = new Right();
            right.ID = Convert.ToInt32(reader["ID"]);
            right.Name = reader["Name"].ToString();
            right.Status = (StatusOptions) Convert.ToInt32(reader["Status"]);
            right.ParentID = reader["ParentID"] == null || reader["ParentID"] == DBNull.Value ? 0 : Convert.ToInt32(reader["ParentID"]);
            right.RightLevel = reader["RightLevel"] == null || reader["RightLevel"] == DBNull.Value ? 0 : Convert.ToInt32(reader["RightLevel"]);
            right.RightType = reader["RightType"] == null || reader["RightType"] == DBNull.Value ? 0 : (RightTypeOptions) Convert.ToInt32(reader["RightType"]);
            right.PageUrl = reader["PageUrl"] == null || reader["PageUrl"] == DBNull.Value ? string.Empty : reader["PageUrl"].ToString().TrimStart('/').ToLower();
            right.SortIndex = reader["SortIndex"] == null || reader["SortIndex"] == DBNull.Value ? 0 : Convert.ToInt32(reader["SortIndex"]);
            right.OnlyInternal = Convert.ToInt32(reader["OnlyInternal"]) == 1 ? true : false;
            right.AddTime = Convert.ToDateTime(reader["AddTime"]);
            right.LastUpdateTime = reader["LastUpdateTime"] == null || reader["LastUpdateTime"] == DBNull.Value ? DateTime.Now : Convert.ToDateTime(reader["LastUpdateTime"]);
            right.Description = reader["Description"] == null || reader["Description"] == DBNull.Value ? string.Empty : reader["Description"].ToString();
            right.SystemID = Convert.ToInt32(reader["SysID"]);
            return right;
        }

        #endregion

        #region 产品信息相关

        /// <summary>
        /// 添加产品信息
        /// </summary>
        /// <param name="soft"></param>
        public static void AddSoft(Soft soft)
        {
            if (soft.ID == 0)
                throw new ToUserException("产品ID不能为0!");

            string platforms = string.Empty;
            soft.Platforms.ForEach((i) => { platforms += ((int) i).ToString() + ","; });

            //MySQL
            string cmdText2 = @"insert into R_Softs(ID,OutID,`Name`,SoftType,OnlyInternal,SortIndex,`Status`,AddTime,Platforms,SoftAreaType) 
                                select ?ID,?OutID,?Name,?SoftType,?OnlyInternal,?SortIndex,?Status,now(),?Platforms,?SoftAreaType
                                from dual
                                where not exists(select * from R_Softs where ID=?ID)";
            MySqlParameter[] parameters2 = new MySqlParameter[]
                {
                    new MySqlParameter("?ID", soft.ID),
                    new MySqlParameter("?OutID", soft.OutID),
                    new MySqlParameter("?Name", soft.Name),
                    new MySqlParameter("?SoftType", (int) soft.SoftType),
                    new MySqlParameter("?OnlyInternal", soft.OnlyInternal ? 1 : 0),
                    new MySqlParameter("?SortIndex", soft.SortIndex),
                    new MySqlParameter("?Status", (int) soft.Status),
                    new MySqlParameter("?Platforms", platforms),
                    new MySqlParameter("?SoftAreaType", soft.SoftAreaType)
                };
            int rowCount = MySqlHelper.ExecuteNonQuery(DACommonHelper.ConnectionString, cmdText2, parameters2);
            if (rowCount == 0)
                throw new ToUserException("产品已经存在!");
        }

        /// <summary>
        /// 删除产品信息
        /// </summary>
        /// <param name="softId"></param>
        public static void DeleteSoft(int softId)
        {
            //MySQL
            string cmdText2 = @"delete from R_Softs where ID=?ID;
                               delete from R_UserSoftRights where SoftID=?ID;
                               delete from R_RoleSoftRights where SoftID=?ID;
                               delete from R_UserChannelRights where SoftID=?ID;";
            MySqlParameter[] parameters2 = new MySqlParameter[] { new MySqlParameter("?ID", softId) };
            MySqlHelper.ExecuteNonQuery(DACommonHelper.ConnectionString, cmdText2, parameters2);
        }

        /// <summary>
        /// 更新产品信息
        /// </summary>
        /// <param name="soft"></param>
        public static void UpdateSoft(Soft soft)
        {
            string platforms = string.Empty;
            soft.Platforms.ForEach((i) => { platforms += ((int) i).ToString() + ","; });
            
            //MySQL
            string cmdText2 = "update R_Softs set OutID=?OutID,`Name`=?Name,SoftType=?SoftType,OnlyInternal=?OnlyInternal,SortIndex=?SortIndex,`Status`=?Status,Platforms=?Platforms,SoftAreaType=?SoftAreaType where ID=?ID";
            MySqlParameter[] parameters2 = new MySqlParameter[]
                {
                    new MySqlParameter("?ID", soft.ID),
                    new MySqlParameter("?OutID", soft.OutID),
                    new MySqlParameter("?Name", soft.Name),
                    new MySqlParameter("?SoftType", (int) soft.SoftType),
                    new MySqlParameter("?OnlyInternal", soft.OnlyInternal ? 1 : 0),
                    new MySqlParameter("?SortIndex", soft.SortIndex),
                    new MySqlParameter("?Status", (int) soft.Status),
                    new MySqlParameter("?Platforms", platforms),
                    new MySqlParameter("?SoftAreaType", soft.SoftAreaType)
                };
            MySqlHelper.ExecuteNonQuery(DACommonHelper.ConnectionString, cmdText2, parameters2);
        }

        /// <summary>
        /// 根据ID获取产品信息
        /// </summary>
        /// <param name="softId"></param>
        /// <returns></returns>
        public static Soft GetSoft(int softId)
        {
            string cmdText = "select ID,OutID,`Name`,SoftType,OnlyInternal,SortIndex,`Status`,Platforms,AddTime,SoftAreaType,StatAloneID from R_Softs where ID=?ID";
            MySqlParameter[] parameters = new MySqlParameter[] { new MySqlParameter("?ID", softId) };
            using (MySqlDataReader reader = MySqlHelper.ExecuteReader(DACommonHelper.ConnectionString, cmdText, parameters))
            {
                if (reader.Read())
                {
                    return BindSoft(reader);
                }
            }
            return null;
        }

        /// <summary>
        /// 获取所有产品列表
        /// </summary>
        /// <returns></returns>
        public static List<Soft> GetSofts()
        {
            string cmdText = "select ID,OutID,`Name`,SoftType,OnlyInternal,SortIndex,`Status`,Platforms,AddTime,SoftAreaType,StatAloneID from R_Softs order by SortIndex desc";
            List<Soft> softs = new List<Soft>();
            using (MySqlDataReader reader = MySqlHelper.ExecuteReader(DACommonHelper.ConnectionString, cmdText))
            {
                while (reader.Read())
                {
                    softs.Add(BindSoft(reader));
                }
            }
            return softs;
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
        public static List<Soft> GetSofts(SoftTypeOptions softType, string keyword, int pageIndex, int pageSize,
                                          ref int recordCount)
        {
            string where = string.IsNullOrEmpty(keyword)
                               ? string.Format("where SoftType={0}", (int) softType)
                               : string.Format("where SoftType={0} and `Name` like '%{1}%'", (int) softType,
                                               keyword.Replace("'", "''"));
            string cmdText = "select count(*) from R_Softs " + where;
            object result = MySqlHelper.ExecuteScalar(DACommonHelper.ConnectionString, cmdText);
            recordCount = Convert.ToInt32(result);
            cmdText = string.Format(@"
                        select ID,OutID,`Name`,SoftType,OnlyInternal,SortIndex,`Status`,Platforms,AddTime,SoftAreaType,StatAloneID 
                        from R_Softs {0} order by SortIndex desc limit {1},{2}", where, (pageIndex - 1)*pageSize,
                                    pageSize);
            List<Soft> softs = new List<Soft>();
            using (MySqlDataReader reader = MySqlHelper.ExecuteReader(DACommonHelper.ConnectionString, cmdText))
            {
                while (reader.Read())
                {
                    softs.Add(BindSoft(reader));
                }
            }
            return softs;
        }

        /// <summary>
        /// 绑定产品信息
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private static Soft BindSoft(MySqlDataReader reader)
        {
            Soft soft = new Soft();
            soft.ID = Convert.ToInt32(reader["ID"]);
            soft.OutID = Convert.ToInt32(reader["OutID"]);
            soft.Name = reader["Name"].ToString();
            soft.SoftType = (SoftTypeOptions) Convert.ToInt32(reader["SoftType"]);
            soft.OnlyInternal = Convert.ToInt32(reader["OnlyInternal"]) == 1 ? true : false;
            soft.SortIndex = Convert.ToInt32(reader["SortIndex"]);
            soft.Status = (StatusOptions) Convert.ToInt32(reader["Status"]);
            soft.AddTime = Convert.ToDateTime(reader["AddTime"]);
            soft.Platforms = reader["Platforms"].ToString()
                                   .Split(new string[] {","}, StringSplitOptions.RemoveEmptyEntries)                                   
                                   .Select(a => (MobileOption)int.Parse(a))
                                   .Where(a => a != MobileOption.AndroidPad)
                                   .ToList();
            soft.SoftAreaType = Convert.ToInt32(reader["SoftAreaType"]);
            soft.StatAloneID = Convert.ToInt32(reader["StatAloneID"]);
            soft.ProjectSources = new List<ProjectSource>();
            return soft;
        }

        #endregion

        #region 项目来源信息相关

        /// <summary>
        /// 添加项目来源信息
        /// </summary>
        /// <param name="projectSource"></param>
        public static void AddProjectSource(ProjectSource projectSource)
        {         
            //MySQL
            string cmdText2 = @"insert into R_ProjectSourcesBySoft(ProjectSource,SoftID,SoftID2,`Name`,OnlyInternal,SortIndex,`Status`,AddTime,ProjectSourceType,ClientType)
                                select ?ProjectSource,?SoftID,?SoftID,?Name,?OnlyInternal,?SortIndex,?Status,now(),?ProjectSourceType,1
                                from dual
                                where not exists(select * from R_ProjectSourcesBySoft where ProjectSource=?ProjectSource and SoftID=?SoftID)";
            MySqlParameter[] parameters2 = new MySqlParameter[]
                {
                    new MySqlParameter("?ProjectSource", projectSource.ProjectSourceID),
                    new MySqlParameter("?Name", projectSource.Name),
                    new MySqlParameter("?OnlyInternal", projectSource.OnlyInternal ? 1 : 0),
                    new MySqlParameter("?SortIndex", projectSource.SortIndex),
                    new MySqlParameter("?Status", (int) projectSource.Status),
                    new MySqlParameter("?ProjectSourceType", (int) projectSource.ProjectSourceType),
                    new MySqlParameter("?SoftID", projectSource.SoftID)
                };
            int rowCount = MySqlHelper.ExecuteNonQuery(DACommonHelper.ConnectionString, cmdText2, parameters2);
            if (rowCount == 0)
                throw new ToUserException("项目来源已经存在!");
        }

        /// <summary>
        /// 删除项目来源信息
        /// </summary>
        /// <param name="projectSource"></param>
        /// <param name="softId"></param>
        public static void DeleteProjectSource(int projectSource, int softId)
        {
            //MySQL
            string cmdText2 =
                @"delete from R_ProjectSourcesBySoft where ProjectSource=?ProjectSource and SoftID=?SoftID;";
            MySqlParameter[] parameters2 = new MySqlParameter[]
                {
                    new MySqlParameter("?ProjectSource", projectSource),
                    new MySqlParameter("?SoftID", softId)
                };
            MySqlHelper.ExecuteNonQuery(DACommonHelper.ConnectionString, cmdText2, parameters2);
        }

        /// <summary>
        /// 更新项目来源信息
        /// </summary>
        /// <param name="projectSource"></param>
        public static void UpdateProjectSource(ProjectSource projectSource)
        {
            //MySQL
            string cmdText2 = "update R_ProjectSourcesBySoft set `Name`=?Name,OnlyInternal=?OnlyInternal,SortIndex=?SortIndex,`Status`=?Status,ProjectSourceType=?ProjectSourceType where ProjectSource=?ProjectSource and SoftID=?SoftID";
            MySqlParameter[] parameters2 = new MySqlParameter[]
                {
                    new MySqlParameter("?ProjectSource", projectSource.ProjectSourceID),
                    new MySqlParameter("?Name", projectSource.Name),
                    new MySqlParameter("?OnlyInternal", projectSource.OnlyInternal ? 1 : 0),
                    new MySqlParameter("?SortIndex", projectSource.SortIndex),
                    new MySqlParameter("?Status", (int) projectSource.Status),
                    new MySqlParameter("?ProjectSourceType", (int) projectSource.ProjectSourceType),
                    new MySqlParameter("?SoftID", projectSource.SoftID)
                };
            MySqlHelper.ExecuteNonQuery(DACommonHelper.ConnectionString, cmdText2, parameters2);
        }

        /// <summary>
        /// 根据ID获取项目来源信息
        /// </summary>
        /// <param name="projectSource"></param>
        /// <param name="softId"></param>
        /// <returns></returns>
        public static ProjectSource GetProjectSource(int projectSource, int softId)
        {
            string cmdText = "select ProjectSource,SoftID,`Name`,OnlyInternal,SortIndex,`Status`,AddTime,ProjectSourceType from R_ProjectSourcesBySoft where ProjectSource=?ProjectSource and SoftID=?SoftID";
            MySqlParameter[] parameters = new MySqlParameter[]
                {
                    new MySqlParameter("?ProjectSource", projectSource),
                    new MySqlParameter("?SoftID", softId)
                };
            using (MySqlDataReader reader = MySqlHelper.ExecuteReader(DACommonHelper.ConnectionString, cmdText, parameters))
            {
                if (reader.Read())
                {
                    return BindProjectSource(reader);
                }
            }
            return null;
        }

        /// <summary>
        /// 获取项目来源信息列表
        /// </summary>
        /// <returns></returns>
        public static List<ProjectSource> GetProjectSources()
        {
            string cmdText = "select ProjectSource,SoftID2 softid,`Name`,OnlyInternal,SortIndex,`Status`,AddTime,ProjectSourceType from R_ProjectSourcesBySoft order by SortIndex desc";
            List<ProjectSource> projectSources = new List<ProjectSource>();
            using (MySqlDataReader reader = MySqlHelper.ExecuteReader(DACommonHelper.ConnectionString, cmdText))
            {
                while (reader.Read())
                {
                    projectSources.Add(BindProjectSource(reader));
                }
            }
            return projectSources;
        }

        /// <summary>
        /// 绑定项目来源信息
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private static ProjectSource BindProjectSource(MySqlDataReader reader)
        {
            ProjectSource projectSource = new ProjectSource();
            projectSource.ProjectSourceID = Convert.ToInt32(reader["ProjectSource"]);
            projectSource.SoftID = Convert.ToInt32(reader["SoftID"]);
            projectSource.Name = reader["Name"].ToString();
            projectSource.OnlyInternal = Convert.ToInt32(reader["OnlyInternal"]) == 1 ? true : false;
            projectSource.SortIndex = Convert.ToInt32(reader["SortIndex"]);
            projectSource.Status = (StatusOptions) Convert.ToInt32(reader["Status"]);
            projectSource.AddTime = Convert.ToDateTime(reader["AddTime"]);
            projectSource.ProjectSourceType = (ProjectSourceTypeOptions) Convert.ToInt32(reader["ProjectSourceType"]);
            return projectSource;
        }

        #endregion

        #region 操作日志相关

        /// <summary>
        /// 添加操作日志
        /// </summary>
        /// <param name="log"></param>
        public static void AddAdminLog(AdminLog log)
        {
            string cmdText = "insert into R_AdminLogs(Account,AccountType,TrueName,IP,AddTime,PageUrl,Memo,SysID) values(?Account,?AccountType,?TrueName,?IP,now(),?PageUrl,?Memo,?SysID)";
            MySqlParameter[] parameters = new MySqlParameter[]
            {
                new MySqlParameter("?Account", log.Account),
                new MySqlParameter("?TrueName", log.TrueName),
                new MySqlParameter("?AccountType", (int) log.AccountType),
                new MySqlParameter("?IP", log.IP),
                new MySqlParameter("?PageUrl", log.PageUrl),
                new MySqlParameter("?Memo", log.Memo),
                new MySqlParameter("?SysID", log.SystemID)
            };
            MySqlHelper.ExecuteNonQuery(DACommonHelper.ConnectionString, cmdText, parameters);
        }

        /// <summary>
        /// 获取项目来源信息列表
        /// </summary>
        /// <param name="sysId"></param>
        /// <param name="keyword"></param>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="recordCount"></param>
        /// <returns></returns>
        public static List<AdminLog> GetAdminLogs(int sysId, string keyword, DateTime beginTime, DateTime endTime, int pageIndex,
                                                  int pageSize, ref int recordCount)
        {
            if (sysId <= 0)
                return new List<AdminLog>();

            string where = " where `SysID`=" + sysId.ToString();
            if (beginTime < DateTime.Now && endTime > beginTime)
                where += string.Format(" and `AddTime` between '{0:yyyy-MM-dd}' and '{1:yyyy-MM-dd}' ", beginTime, endTime.AddDays(1));
            if (!string.IsNullOrEmpty(keyword))
                where += string.Format(" and (`Account` like '%{0}%' or `TrueName` like '%{0}%' or `IP` like '%{0}%' or `Memo` like '%{0}%')", keyword.Replace("'", "\\'"));

            string cmdText = "select count(*) from R_AdminLogs" + where;
            object result = MySqlHelper.ExecuteScalar(DACommonHelper.ConnectionString, cmdText);
            recordCount = Convert.ToInt32(result);
            cmdText = string.Format(@"select `ID`,`Account`,`AccountType`,`TrueName`,`IP`,`AddTime`,`PageUrl`,`Memo`,`SysID` from R_AdminLogs " +
                    where + @" order by ID desc limit {0},{1}", (pageIndex - 1)*pageSize, pageSize);
            List<AdminLog> adminLogs = new List<AdminLog>();
            using (MySqlDataReader reader = MySqlHelper.ExecuteReader(DACommonHelper.ConnectionString, cmdText))
            {
                while (reader.Read())
                {
                    adminLogs.Add(BindAdminLog(reader));
                }
            }
            return adminLogs;
        }

        /// <summary>
        /// 绑定项目来源信息
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private static AdminLog BindAdminLog(MySqlDataReader reader)
        {
            AdminLog adminLog = new AdminLog();
            adminLog.ID = Convert.ToInt32(reader["ID"]);
            adminLog.AccountType = (UserTypeOptions) Convert.ToInt32(reader["AccountType"]);
            adminLog.Account = reader["Account"].ToString();
            adminLog.TrueName = reader["TrueName"] == null || reader["TrueName"] == DBNull.Value ? string.Empty : reader["TrueName"].ToString();
            adminLog.IP = reader["IP"] == null || reader["IP"] == DBNull.Value ? string.Empty : reader["IP"].ToString();
            adminLog.PageUrl = reader["PageUrl"] == null || reader["PageUrl"] == DBNull.Value ? string.Empty : reader["PageUrl"].ToString();
            adminLog.Memo = reader["Memo"] == null || reader["Memo"] == DBNull.Value ? string.Empty : reader["Memo"].ToString();
            adminLog.AddTime = Convert.ToDateTime(reader["AddTime"]);
            adminLog.SystemID = Convert.ToInt32(reader["SysID"]);
            return adminLog;
        }

        #endregion

        #region 资源类型相关

        private static List<ResourceType> resTypes = null;

        /// <summary>
        /// 获取所有的资源类型列表
        /// </summary>
        /// <returns></returns>
        public static List<ResourceType> GetResourceTypes()
        {
            if (resTypes == null)
            {
                resTypes = new List<ResourceType>();
                resTypes.Add(new ResourceType {TypeID = 1, TypeName = "软件"});
                resTypes.Add(new ResourceType {TypeID = 19, TypeName = "软件-iTunes免费软件"});
                resTypes.Add(new ResourceType {TypeID = 22, TypeName = "软件-正版体验"});
                resTypes.Add(new ResourceType {TypeID = 2, TypeName = "主题"});
                resTypes.Add(new ResourceType {TypeID = 3, TypeName = "铃声"});
                resTypes.Add(new ResourceType {TypeID = 4, TypeName = "壁纸"});
                resTypes.Add(new ResourceType {TypeID = 13, TypeName = "音乐"});
                resTypes.Add(new ResourceType {TypeID = 21, TypeName = "WinPhone"});
                resTypes.Add(new ResourceType {TypeID = 23, TypeName = "广告资源"});
                resTypes.Add(new ResourceType {TypeID = 24, TypeName = "主题字体"});
                //resTypes.Add(new ResourceType { TypeID = 25, TypeName = "消息" });
                resTypes.Add(new ResourceType {TypeID = 27, TypeName = "通用资源"});
                resTypes.Add(new ResourceType {TypeID = 8, TypeName = "资讯"});
                //resTypes.Add(new ResourceType { TypeID = 6, TypeName = "电子杂志" });
                //resTypes.Add(new ResourceType { TypeID = 14, TypeName = "视频" });
                //resTypes.Add(new ResourceType { TypeID = 15, TypeName = "91商学院资源" });
                resTypes.Add(new ResourceType {TypeID = 28, TypeName = "桌面渠道软件"});
                resTypes.Add(new ResourceType {TypeID = 50, TypeName = "桌面主题模块资源"});
                resTypes.Add(new ResourceType {TypeID = 29, TypeName = "苹果园软件"});
                resTypes.Add(new ResourceType {TypeID = 30, TypeName = "桌面网址导航"});
                resTypes.Add(new ResourceType {TypeID = 31, TypeName = "桌面0屏导航"});
                resTypes.Add(new ResourceType {TypeID = 36, TypeName = "主题系列"});
                resTypes.Add(new ResourceType {TypeID = 37, TypeName = "iOS的Deb资源"});
                resTypes.Add(new ResourceType {TypeID = 38, TypeName = "桌面贴纸"});
                resTypes.Add(new ResourceType {TypeID = 39, TypeName = "桌面美图作品图"});
            }
            return resTypes;
        }

        #endregion
    }
}