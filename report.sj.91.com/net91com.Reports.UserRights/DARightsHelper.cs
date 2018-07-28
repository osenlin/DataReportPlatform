using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;
using net91com.Core.Data;

namespace net91com.Reports.UserRights
{
    /// <summary>
    /// 权限分配及验证相关的数据存取
    /// </summary>
    internal class DARightsHelper
    {
        #region 给用户分配权限

        /// <summary>
        /// 授予用户操作权限
        /// </summary>
        /// <param name="sysId"></param>
        /// <param name="userId"></param>
        /// <param name="selectedRightIds">当前选择授予的权限列表</param>
        /// <param name="rangeRightIds">授权人可以分配的权限范围</param>
        public static void AddUserRights(int sysId, int userId, List<int> selectedRightIds, List<int> rangeRightIds)
        {
            string cmdText = string.Empty;
            try
            {
                //当前选择的权限
                cmdText = "CREATE TEMPORARY TABLE T_SelectedRights(RightID int);";
                if (selectedRightIds.Count > 0)
                {
                    string selectedValues = string.Empty;
                    foreach (int rid in selectedRightIds)
                        selectedValues += string.Format("({0}),", rid);
                    selectedValues = selectedValues.TrimEnd(',');
                    cmdText += "insert into T_SelectedRights values" + selectedValues + ";";
                }
                //当前用户拥有的角色包含的权限（不包括分类权限）
                cmdText += @"CREATE TEMPORARY TABLE T_RoleRights(RightID int); 
                         insert into T_RoleRights select C.RightID 
                         from R_UserRoles A inner join R_Roles B on A.RoleID=B.ID and B.SysID=?SysID and A.`UserID`=?UserID
                         inner join R_RoleRights C on A.RoleID=C.RoleID;";
                if (rangeRightIds != null) //有限定范围
                {
                    //权限限定范围
                    cmdText += "CREATE TEMPORARY TABLE T_RangeRights(RightID int);";
                    if (rangeRightIds.Count > 0)
                    {
                        string rangeValues = string.Empty;
                        foreach (int rid in rangeRightIds)
                            rangeValues += string.Format("({0}),", rid);
                        rangeValues = rangeValues.TrimEnd(',');
                        cmdText += "insert into T_RangeRights values" + rangeValues + ";";
                    }
                    //不在限定范围内的原权限和在限定范围内当前选择的权限
                    cmdText += @"CREATE TEMPORARY TABLE T_ValidRights(RightID int); 
                             insert into T_ValidRights select A.RightID 
                             from R_UserRights A inner join R_Rights B on A.RightID=B.ID and B.SysID=?SysID and A.`UserID`=?UserID 
                             left join T_RangeRights C on A.RightID=C.RightID where C.RightID is null;
                             insert into T_ValidRights select A.RightID from T_SelectedRights A inner join T_RangeRights B on A.RightID=B.RightID;";
                    //添加权限时要排除角色已包含的权限
                    cmdText += @"delete A from R_UserRights A inner join R_Rights B on A.RightID=B.ID and B.SysID=?SysID and A.`UserID`=?UserID;
                             insert into R_UserRights(`UserID`,RightID)
                             select distinct ?UserID,A.RightID from T_ValidRights A left join T_RoleRights B on A.RightID=B.RightID where B.RightID is null;";
                    cmdText += @"drop table T_SelectedRights;
                             drop table T_RoleRights;
                             drop table T_RangeRights;
                             drop table T_ValidRights;";
                }
                else //没限定范围
                {
                    //添加权限时要排除角色已包含的权限
                    cmdText += @"delete A from R_UserRights A inner join R_Rights B on A.RightID=B.ID and B.SysID=?SysID and A.`UserID`=?UserID;
                             insert into R_UserRights(`UserID`,RightID)
                             select distinct ?UserID,A.RightID 
                             from T_SelectedRights A inner join R_Rights B on A.RightID=B.ID and B.SysID=?SysID 
                             left join T_RoleRights C on A.RightID=C.RightID where C.RightID is null;
                             drop table T_SelectedRights;
                             drop table T_RoleRights;";
                }
                MySqlParameter[] parms = new MySqlParameter[]
                {
                new MySqlParameter("?SysID", sysId),
                new MySqlParameter("?UserID", userId)
                };
                MySqlHelper.ExecuteNonQuery(DACommonHelper.ConnectionString, cmdText, parms);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(cmdText, ex);
            }
        }

        /// <summary>
        /// 授予用户产品权限
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="roleId"></param>
        /// <param name="selectedRightKeys">当前选择授予的权限列表</param>
        /// <param name="rangeRightKeys">授权人可以分配的权限范围</param>
        public static void AddUserSoftRights(int userId, List<int> selectedSoftIds, List<int> rangeSoftIds)
        {
            AddUserRightsInternal(userId, selectedSoftIds, rangeSoftIds, "R_UserSoftRights", "R_RoleSoftRights", "SoftID");
        }

        /// <summary>
        /// 授予用户项目源权限
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="roleId"></param>
        /// <param name="selectedRightKeys">当前选择授予的权限列表</param>
        /// <param name="rangeRightKeys">授权人可以分配的权限范围</param>
        public static void AddUserProjectSourceRights(int userId, List<int> selectedProjectSources,
                                                      List<int> rangeProjectSources)
        {
            AddUserRightsInternal(userId, selectedProjectSources, rangeProjectSources, "R_UserProjectSourceRights", "R_RoleProjectSourceRights", "ProjectSource");
        }

        /// <summary>
        /// 授予用户特定资源权限
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="resIds"></param>
        public static void AddUserResRights(int userId, List<int> resIds)
        {
            string cmdText = string.Format("delete from R_UserResRights where `UserID`={0};", userId);
            if (resIds.Count > 0)
            {
                string values = string.Empty;
                foreach (int rid in resIds)
                    values += string.Format("({0},{1}),", userId, rid);
                values = values.TrimEnd(',');
                cmdText += "insert into R_UserResRights(`UserID`,ResID) values" + values;
            }
            MySqlHelper.ExecuteNonQuery(DACommonHelper.ConnectionString, cmdText);
        }

        /// <summary>
        /// 授予用户权限
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="selectedRightIds">当前选择授予的权限列表</param>
        /// <param name="rangeRightIds">授权人可以分配的权限范围</param>
        /// <param name="rightTableName">权限表名称</param>
        /// <param name="roleRightTableName">角色权限表名称</param>
        /// <param name="rightFieldName">权限表字段名称</param>        
        private static void AddUserRightsInternal(int userId, List<int> selectedRightIds, List<int> rangeRightIds,
                                                  string rightTableName, string roleRightTableName, string rightFieldName)
        {
            string cmdText = string.Empty;
            try
            {
                //当前选择的权限
                cmdText = "CREATE TEMPORARY TABLE T_SelectedRights(" + rightFieldName + " int);";
                if (selectedRightIds.Count > 0)
                {
                    string selectedValues = string.Empty;
                    foreach (int rid in selectedRightIds)
                        selectedValues += string.Format("({0}),", rid);
                    selectedValues = selectedValues.TrimEnd(',');
                    cmdText += "insert into T_SelectedRights values" + selectedValues + ";";
                }
                //当前用户拥有的角色包含的权限（不包括分类权限）
                cmdText += string.Format(@"CREATE TEMPORARY TABLE T_RoleRights({0} int); 
                         insert into T_RoleRights select B.{0} 
                         from R_UserRoles A inner join {1} B 
                         on A.RoleID=B.RoleID and A.`UserID`=?UserID;", rightFieldName, roleRightTableName);
                if (rangeRightIds != null) //有限定范围
                {
                    //权限限定范围
                    cmdText += "CREATE TEMPORARY TABLE T_RangeRights(" + rightFieldName + " int);";
                    if (rangeRightIds.Count > 0)
                    {
                        string rangeValues = string.Empty;
                        foreach (int rid in rangeRightIds)
                            rangeValues += string.Format("({0}),", rid);
                        rangeValues = rangeValues.TrimEnd(',');
                        cmdText += "insert into T_RangeRights values" + rangeValues + ";";
                    }
                    //不在限定范围内的原权限和在限定范围内当前选择的权限
                    cmdText += string.Format(@"CREATE TEMPORARY TABLE T_ValidRights({0} int); 
                             insert into T_ValidRights select A.{0} from {1} A left join T_RangeRights B on A.{0}=B.{0} where A.`UserID`=?UserID and B.{0} is null;
                             insert into T_ValidRights select A.{0} from T_SelectedRights A inner join T_RangeRights B on A.{0}=B.{0};",
                                             rightFieldName, rightTableName);
                    //添加权限时要排除角色已包含的权限
                    cmdText += string.Format(@"delete from {1} where `UserID`=?UserID;
                             insert into {1}(`UserID`,{0})
                             select distinct ?UserID,A.{0} from T_ValidRights A left join T_RoleRights B on A.{0}=B.{0} where B.{0} is null;",
                                             rightFieldName, rightTableName);
                    cmdText += @"drop table T_SelectedRights;
                             drop table T_RoleRights;
                             drop table T_RangeRights;
                             drop table T_ValidRights;";
                }
                else //没限定范围
                {
                    //添加权限时要排除角色已包含的权限
                    cmdText += string.Format(@"delete from {1} where `UserID`=?UserID;
                             insert into {1}(`UserID`,{0})
                             select distinct ?UserID,A.{0} from T_SelectedRights A left join T_RoleRights B on A.{0}=B.{0} where B.{0} is null;
                             drop table T_SelectedRights;
                             drop table T_RoleRights;", rightFieldName, rightTableName);
                }
                MySqlParameter[] parms = new MySqlParameter[] { new MySqlParameter("?UserID", userId) };
                MySqlHelper.ExecuteNonQuery(DACommonHelper.ConnectionString, cmdText, parms);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(cmdText, ex);
            }
        }

        /// <summary>
        /// 获取用户权限(仅用于权限分配)
        /// </summary>
        /// <param name="sysId"></param>
        /// <param name="userId"></param>
        /// <param name="accountType"></param>
        /// <returns></returns>
        public static List<RightItem> GetUserRights(int sysId, int userId, UserTypeOptions accountType)
        {
            string cmdText;
            //渠道相关的用户只能限于角色(系统定义的)关联的权限
            if (accountType == UserTypeOptions.Channel || accountType == UserTypeOptions.ChannelPartner)
            {
                cmdText = string.Format(@"select distinct C.RightID,1 FromRole 
                                        from R_UserRoles A 
                                            inner join R_Roles B on A.RoleID=B.ID and B.RoleType={0} and A.`UserID`=?UserID and B.SysID=?SysID 
                                            inner join R_RoleRights C on B.ID=C.RoleID",
                                        accountType == UserTypeOptions.Channel
                                            ? (int)RoleTypeOptions.Channel
                                            : (int)RoleTypeOptions.ChannelPartner);
            }
            else
            {
                cmdText = @"select RightID,if(sum(FromRole)>0,1,0) FromRole  
                            from (
                                select RightID,0 FromRole from R_UserRights A inner join R_Rights B on A.RightID=B.ID and B.SysID=?SysID and A.`UserID`=?UserID
                                union all
                                select RightID,1 FromRole from R_UserRoles A inner join R_Roles B on A.RoleID=B.ID and B.SysID=?SysID and A.`UserID`=?UserID inner join R_RoleRights C on A.RoleID=C.RoleID) A
                            group by RightID";
            }
            MySqlParameter[] parms = new MySqlParameter[] 
            {
                new MySqlParameter("?SysID", sysId),
                new MySqlParameter("?UserID", userId)
            };
            List<RightItem> items = new List<RightItem>();
            using (MySqlDataReader reader = MySqlHelper.ExecuteReader(DACommonHelper.ConnectionString, cmdText, parms))
            {
                while (reader.Read())
                {
                    RightItem r = new RightItem
                    {
                        RightID = Convert.ToInt32(reader["RightID"]),
                        FromRole = Convert.ToInt32(reader["FromRole"]) == 1
                    };
                    items.Add(r);
                }
            }
            return items;
        }

        /// <summary>
        /// 获取用户产品权限(仅用于权限分配)
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static List<RightItem> GetUserSoftRights(int userId)
        {
            string cmdText = @"select SoftID,if(sum(FromRole)>0,1,0) FromRole  
                               from (
                                   select SoftID,0 FromRole from R_UserSoftRights where `UserID`=?UserID
                                   union all
                                   select SoftID,1 FromRole from R_UserRoles A inner join R_RoleSoftRights B on A.RoleID=B.RoleID and A.`UserID`=?UserID) A
                               group by SoftID";
            MySqlParameter[] parameters = new MySqlParameter[] { new MySqlParameter("?UserID", userId) };
            List<RightItem> items = new List<RightItem>();
            using (MySqlDataReader reader = MySqlHelper.ExecuteReader(DACommonHelper.ConnectionString, cmdText, parameters))
            {
                while (reader.Read())
                {
                    RightItem r = new RightItem
                        {
                            RightID = Convert.ToInt32(reader["SoftID"]),
                            FromRole = Convert.ToInt32(reader["FromRole"]) == 1
                        };
                    items.Add(r);
                }
            }
            return items;
        }

        /// <summary>
        /// 获取用户项目来源权限(仅用于权限分配)
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static List<RightItem> GetUserProjectSourceRights(int userId)
        {
            string cmdText = @"select ProjectSource,if(sum(FromRole)>0,1,0) FromRole  
                               from (
                                   select ProjectSource,0 FromRole from R_UserProjectSourceRights where `UserID`=?UserID
                                   union all
                                   select ProjectSource,1 FromRole from R_UserRoles A inner join R_RoleProjectSourceRights B on A.RoleID=B.RoleID and A.`UserID`=?UserID) A
                               group by ProjectSource";
            MySqlParameter[] parameters = new MySqlParameter[] { new MySqlParameter("?UserID", userId) };
            List<RightItem> items = new List<RightItem>();
            using (MySqlDataReader reader = MySqlHelper.ExecuteReader(DACommonHelper.ConnectionString, cmdText, parameters))
            {
                while (reader.Read())
                {
                    RightItem r = new RightItem
                        {
                            RightID = Convert.ToInt32(reader["ProjectSource"]),
                            FromRole = Convert.ToInt32(reader["FromRole"]) == 1
                        };
                    items.Add(r);
                }
            }
            return items;
        }

        /// <summary>
        /// 获取用户资源权限
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static List<RightItem> GetUserResRights(int userId)
        {
            string cmdText = "SELECT ResID,0 FromRole FROM R_UserResRights WHERE `UserID`=?UserID";
            MySqlParameter[] parameters = new MySqlParameter[] { new MySqlParameter("?UserID", userId) };
            List<RightItem> items = new List<RightItem>();
            using (MySqlDataReader reader = MySqlHelper.ExecuteReader(DACommonHelper.ConnectionString, cmdText, parameters))
            {
                while (reader.Read())
                {
                    RightItem r = new RightItem
                        {
                            RightID = Convert.ToInt32(reader["ResID"]),
                            FromRole = Convert.ToInt32(reader["FromRole"]) == 1
                        };
                    items.Add(r);
                }
            }
            return items;
        }

        #endregion

        #region 给用户分配系统

        /// <summary>
        /// 给用户分配系统
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="userSystems"></param>
        /// <param name="rangeSysIds"></param>
        public static void AddUserSystems(int userId, List<UserSystem> userSystems, List<int> rangeSysIds)
        {
            //当前选择的权限
            string cmdText = @"DROP TABLE IF EXISTS T_SelectedSystems;
                               CREATE TEMPORARY TABLE T_SelectedSystems(SysID int,Admin tinyint(4));";
            if (userSystems.Count > 0)
            {
                string selectedValues = string.Empty;
                foreach (UserSystem us in userSystems)
                    selectedValues += string.Format("({0},{1}),", us.SystemID, us.Admin ? 1 : 0);
                selectedValues = selectedValues.TrimEnd(',');
                cmdText += "insert into T_SelectedSystems values" + selectedValues + ";";
            }
            //权限限定范围
            cmdText += @"DROP TABLE IF EXISTS T_RangeSystems;
                         CREATE TEMPORARY TABLE T_RangeSystems(SysID int);";
            if (rangeSysIds.Count > 0)
            {
                string rangeValues = string.Empty;
                foreach (int sid in rangeSysIds)
                    rangeValues += string.Format("({0}),", sid);
                rangeValues = rangeValues.TrimEnd(',');
                cmdText += "insert into T_RangeSystems values" + rangeValues + ";";
            }
            //不在限定范围内的原权限和在限定范围内当前选择的权限
            cmdText += @"DROP TABLE IF EXISTS T_ValidSystems;
                         CREATE TEMPORARY TABLE T_ValidSystems(SysID int,Admin tinyint(4)); 
                             insert into T_ValidSystems select A.SysID,A.Admin 
                             from R_UserSystems A left join T_RangeSystems B on A.SysID=B.SysID where A.`UserID`=?UserID and B.SysID is null;
                             insert into T_ValidSystems select A.SysID,A.Admin from T_SelectedSystems A inner join T_RangeSystems B on A.SysID=B.SysID;";
            cmdText += @"delete A from R_UserSystems A left join T_ValidSystems B on A.SysID=B.SysID where A.`UserID`=?UserID and B.SysID is null;
                         update R_UserSystems A inner join T_ValidSystems B on A.SysID=B.SysID and A.`UserID`=?UserID and A.Admin<>B.Admin set A.Admin=B.Admin;
                         insert into R_UserSystems(UserID,SysID,AddTime,LastLoginTime,Admin) 
                         select ?UserID,A.SysID,now(),now(),A.Admin
                         from T_ValidSystems A left join R_UserSystems B 
                         on A.SysID=B.SysID and B.`UserID`=?UserID
                         where B.SysID is null";
            MySqlParameter[] parms = new MySqlParameter[] { new MySqlParameter("?UserID", userId) };
            MySqlHelper.ExecuteNonQuery(DACommonHelper.ConnectionString, cmdText, parms);
        }

        /// <summary>
        /// 获得有权限的系统
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static List<UserSystem> GetUserSystems(int userId)
        {
            string cmdText = "select UserID,SysID,Admin,AddTime,LastLoginTime from R_UserSystems where UserID=" + userId.ToString();
            List<UserSystem> userSystems = new List<UserSystem>();
            using (MySqlDataReader reader = MySqlHelper.ExecuteReader(DACommonHelper.ConnectionString, cmdText))
            {
                while (reader.Read())
                {
                    UserSystem uSys = new UserSystem
                    {
                        UserID = Convert.ToInt32(reader["UserID"]),
                        SystemID = Convert.ToInt32(reader["SysID"]),
                        Admin = Convert.ToInt32(reader["Admin"]) == 1,
                        AddTime = Convert.ToDateTime(reader["AddTime"]),
                        LastLoginTime = Convert.ToDateTime(reader["LastLoginTime"])
                    };
                    userSystems.Add(uSys);
                }
            }
            return userSystems;
        }

        /// <summary>
        /// 获得有管理权限的系统
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static List<int> GetAdminSystemIds(int userId)
        {
            string cmdText = "select SysID from R_UserSystems where Admin=1 and UserID=" + userId.ToString();
            List<int> sysIds = new List<int>();
            using (MySqlDataReader reader = MySqlHelper.ExecuteReader(DACommonHelper.ConnectionString, cmdText))
            {
                while (reader.Read())
                {
                    sysIds.Add(Convert.ToInt32(reader["SysID"]));
                }
            }
            return sysIds;
        }

        #endregion

        #region 给用户分配角色

        /// <summary>
        /// 批量添加用户角色
        /// </summary>
        /// <param name="sysId"></param>
        /// <param name="userId"></param>
        /// <param name="selectedRoleIds">已选择的角色</param>
        /// <param name="rangeRoleIds">授权人可以分配的角色范围</param>
        public static void AddUserRoles(int sysId, int userId, List<int> selectedRoleIds, List<int> rangeRoleIds)
        {
            string cmdText = string.Empty;
            try
            {
                cmdText = "CREATE TEMPORARY TABLE T_SelectedRoles(RoleID int);";
                if (selectedRoleIds.Count > 0)
                {
                    string selectedValues = string.Empty;
                    foreach (int rid in selectedRoleIds)
                        selectedValues += string.Format("({0}),", rid);
                    selectedValues = selectedValues.TrimEnd(',');
                    cmdText += "insert into T_SelectedRoles values" + selectedValues + ";";
                }
                //有限定范围
                if (rangeRoleIds != null)
                {
                    cmdText += "CREATE TEMPORARY TABLE T_RangeRoles(RoleID int);";
                    if (rangeRoleIds.Count > 0)
                    {
                        string rangeValues = string.Empty;
                        foreach (int rid in rangeRoleIds)
                            rangeValues += string.Format("({0}),", rid);
                        rangeValues = rangeValues.TrimEnd(',');
                        cmdText += "insert into T_RangeRoles values" + rangeValues + ";";
                    }
                    cmdText += @"CREATE TEMPORARY TABLE T_ValidRoles(RoleID int);
                             insert into T_ValidRoles select A.RoleID 
                             from R_UserRoles A inner join R_Roles B on A.RoleID=B.ID and B.SysID=?SysID and A.`UserID`=?UserID 
                             left join T_RangeRoles C on A.RoleID=C.RoleID where C.RoleID is null;
 
                             insert into T_ValidRoles select A.RoleID from T_SelectedRoles A inner join T_RangeRoles B on A.RoleID=B.RoleID;
                             delete A from R_UserRoles A inner join R_Roles B on A.RoleID=B.ID and B.SysID=?SysID and A.`UserID`=?UserID;
                             insert into R_UserRoles(`UserID`,RoleID) select distinct ?UserID,RoleID from T_ValidRoles;
                             drop table T_RangeRoles;
                             drop table T_SelectedRoles;
                             drop table T_ValidRoles;";
                }
                else //没限定范围
                {
                    cmdText += @"delete A from R_UserRoles A inner join R_Roles B on A.RoleID=B.ID and B.SysID=?SysID and A.`UserID`=?UserID;
                            insert into R_UserRoles(`UserID`,RoleID)
                            select ?UserID,A.RoleID from T_SelectedRoles A inner join R_Roles B on A.RoleID=B.ID and B.SysID=?SysID;
                            drop table T_SelectedRoles;";
                }
                MySqlParameter[] parameters = new MySqlParameter[]
                {
                    new MySqlParameter("?SysID", sysId),
                    new MySqlParameter("?UserID", userId)
                };
                MySqlHelper.ExecuteNonQuery(DACommonHelper.ConnectionString, cmdText, parameters);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(cmdText, ex);
            }
        }

        /// <summary>
        /// 获取指定用户的角色
        /// </summary>
        /// <param name="sysId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static List<int> GetUserRoles(int sysId, int userId)
        {
            string cmdText = "select A.RoleID from R_UserRoles A inner join R_Roles B on A.RoleID=B.ID and B.SysID=?SysID and A.`UserID`=?UserID";
            MySqlParameter[] parameters = new MySqlParameter[]
            {
                new MySqlParameter("?SysID", sysId),
                new MySqlParameter("?UserID", userId)
            };
            List<int> roleIds = new List<int>();
            using (MySqlDataReader reader = MySqlHelper.ExecuteReader(DACommonHelper.ConnectionString, cmdText, parameters))
            {
                while (reader.Read())
                {
                    roleIds.Add(Convert.ToInt32(reader["RoleID"]));
                }
            }
            return roleIds;
        }

        #endregion

        #region 给角色分配权限

        /// <summary>
        /// 授予角色操作权限
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="rightIds"></param>
        /// <param name="rangeRightIds"></param>
        public static void AddRoleRights(int roleId, List<int> rightIds, List<int> rangeRightIds)
        {
            AddRoleRightsInternal(roleId, rightIds, rangeRightIds, "R_RoleRights", "RightID");
        }

        /// <summary>
        /// 授予角色产品权限
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="softIds"></param>
        /// <param name="rangeRightIds"></param>
        public static void AddRoleSoftRights(int roleId, List<int> softIds, List<int> rangeRightIds)
        {
            AddRoleRightsInternal(roleId, softIds, rangeRightIds, "R_RoleSoftRights", "SoftID");
        }

        /// <summary>
        /// 授予角色项目源权限
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="projectSources"></param>
        /// <param name="rangeRightIds"></param>
        public static void AddRoleProjectSourceRights(int roleId, List<int> projectSources, List<int> rangeRightIds)
        {
            AddRoleRightsInternal(roleId, projectSources, rangeRightIds, "R_RoleProjectSourceRights", "ProjectSource");
        }

        /// <summary>
        /// 授予角色权限
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="selectedRightIds"></param>
        /// <param name="rangeRightIds"></param>
        /// <param name="rightTableName"></param>
        /// <param name="rightFieldName"></param>
        private static void AddRoleRightsInternal(int roleId, List<int> selectedRightIds, List<int> rangeRightIds,
                                                  string rightTableName, string rightFieldName)
        {
            //当前选择的权限
            string cmdText = "CREATE TEMPORARY TABLE T_SelectedRights(" + rightFieldName + " int);";
            if (selectedRightIds.Count > 0)
            {
                string selectedValues = string.Empty;
                foreach (int rid in selectedRightIds)
                    selectedValues += string.Format("({0}),", rid);
                selectedValues = selectedValues.TrimEnd(',');
                cmdText += "insert into T_SelectedRights values" + selectedValues + ";";
            }
            if (rangeRightIds != null) //有限定范围
            {
                //权限限定范围
                cmdText += "CREATE TEMPORARY TABLE T_RangeRights(" + rightFieldName + " int);";
                if (rangeRightIds.Count > 0)
                {
                    string rangeValues = string.Empty;
                    foreach (int rid in rangeRightIds)
                        rangeValues += string.Format("({0}),", rid);
                    rangeValues = rangeValues.TrimEnd(',');
                    cmdText += "insert into T_RangeRights values" + rangeValues + ";";
                }
                //不在限定范围内的原权限和在限定范围内当前选择的权限
                cmdText += string.Format(@"CREATE TEMPORARY TABLE T_ValidRights({0} int); 
                             insert into T_ValidRights select A.{0} from {1} A left join T_RangeRights B on A.{0}=B.{0} where A.RoleID=?RoleID and B.{0} is null;
                             insert into T_ValidRights select A.{0} from T_SelectedRights A inner join T_RangeRights B on A.{0}=B.{0};",
                                         rightFieldName, rightTableName);
                //添加权限时要排除角色已包含的权限
                cmdText += string.Format(@"delete from {1} where RoleID=?RoleID;
                             insert into {1}(RoleID,{0}) select distinct ?RoleID,{0} from T_ValidRights;",
                                         rightFieldName, rightTableName);
                cmdText += @"drop table T_SelectedRights;
                             drop table T_RangeRights;
                             drop table T_ValidRights;";
            }
            else //没限定范围
            {
                //添加权限时要排除角色已包含的权限
                cmdText += string.Format(@"delete from {1} where RoleID=?RoleID;
                             insert into {1}(RoleID,{0}) select distinct ?RoleID,{0} from T_SelectedRights;
                             drop table T_SelectedRights;", rightFieldName, rightTableName);
            }
            MySqlParameter[] parameters = new MySqlParameter[] { new MySqlParameter("?RoleID", roleId) };
            MySqlHelper.ExecuteNonQuery(DACommonHelper.ConnectionString, cmdText, parameters);
        }

        /// <summary>
        /// 获取角色权限(仅用于权限分配)
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public static List<RightItem> GetRoleRights(int roleId)
        {
            string cmdText = @"select RightID,0 FromRole from R_RoleRights where RoleID=?RoleID";
            MySqlParameter[] parameters = new MySqlParameter[] { new MySqlParameter("?RoleID", roleId) };
            List<RightItem> items = new List<RightItem>();
            using (MySqlDataReader reader = MySqlHelper.ExecuteReader(DACommonHelper.ConnectionString, cmdText, parameters))
            {
                while (reader.Read())
                {
                    RightItem r = new RightItem
                        {
                            RightID = Convert.ToInt32(reader["RightID"]),
                            FromRole = Convert.ToInt32(reader["FromRole"]) == 1
                        };
                    items.Add(r);
                }
            }
            return items;
        }

        /// <summary>
        /// 获取角色产品权限
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public static List<RightItem> GetRoleSoftRights(int roleId)
        {
            string cmdText = @"select SoftID,0 FromRole from R_RoleSoftRights where RoleID=?RoleID";
            MySqlParameter[] parameters = new MySqlParameter[] { new MySqlParameter("?RoleID", roleId) };
            List<RightItem> items = new List<RightItem>();
            using (MySqlDataReader reader = MySqlHelper.ExecuteReader(DACommonHelper.ConnectionString, cmdText, parameters))
            {
                while (reader.Read())
                {
                    RightItem r = new RightItem
                        {
                            RightID = Convert.ToInt32(reader["SoftID"]),
                            FromRole = Convert.ToInt32(reader["FromRole"]) == 1
                        };
                    items.Add(r);
                }
            }
            return items;
        }

        /// <summary>
        /// 获取角色项目来源权限(仅用于权限分配)
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public static List<RightItem> GetRoleProjectSourceRights(int roleId)
        {
            string cmdText = @"select ProjectSource,0 FromRole from R_RoleProjectSourceRights where RoleID=?RoleID";
            MySqlParameter[] parameters = new MySqlParameter[] { new MySqlParameter("?RoleID", roleId) };
            List<RightItem> items = new List<RightItem>();
            using (MySqlDataReader reader = MySqlHelper.ExecuteReader(DACommonHelper.ConnectionString, cmdText, parameters))
            {
                while (reader.Read())
                {
                    RightItem r = new RightItem
                        {
                            RightID = Convert.ToInt32(reader["ProjectSource"]),
                            FromRole = Convert.ToInt32(reader["FromRole"]) == 1
                        };
                    items.Add(r);
                }
            }
            return items;
        }

        #endregion
    }
}