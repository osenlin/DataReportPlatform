using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using net91com.Core;

namespace net91com.Reports.UserRights
{

    #region 用户信息实体(User)

    /// <summary>
    /// 用户信息实体类
    /// </summary>
    public class User
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 账号
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// 账号类型
        /// </summary>
        public UserTypeOptions AccountType { get; set; }

        /// <summary>
        /// 用户真实姓名
        /// </summary>
        public string TrueName { get; set; }

        /// <summary>
        /// 用户状态(启用或禁用)
        /// </summary>
        public StatusOptions Status { get; set; }

        /// <summary>
        /// 最后一次登录时间
        /// </summary>
        public DateTime LastLoginTime { get; set; }

        /// <summary>
        /// 添加该用户的管理员ID
        /// </summary>
        public int AdminUserID { get; set; }

        /// <summary>
        /// 所在部门
        /// </summary>
        public string Department { get; set; }

        public DateTime BeginTime { get; set; }

        public DateTime EndTime { get; set; }

        public bool IsSpecialUser { get; set; }

        public string Email { get; set; }

        public bool IsWhiteUser { get; set; }
    }

    #endregion

    #region 系统信息实体(SystemInfo)

    /// <summary>
    /// 系统信息实体
    /// </summary>
    public class SystemInfo
    {
        /// <summary>
        /// 系统ID
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 系统名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// MD5KEY
        /// </summary>
        public string Md5Key { get; set; }

        /// <summary>
        /// 添加时间
        /// </summary>
        public DateTime AddTime { get; set; }

        /// <summary>
        /// 启用或禁用状态 
        /// </summary>
        public StatusOptions Status { get; set; }

        /// <summary>
        /// 系统描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 域名或系统地址
        /// </summary>
        public string Url { get; set; }
    }

    #endregion

    #region 用户系统信息实体(UserSystem)

    /// <summary>
    /// 用户系统信息实体
    /// </summary>
    public class UserSystem
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public int UserID { get; set; }

        /// <summary>
        /// 系统ID
        /// </summary>
        public int SystemID { get; set; }

        /// <summary>
        /// 添加时间
        /// </summary>
        public DateTime AddTime { get; set; }

        /// <summary>
        /// 用户最后一次登录系统时间
        /// </summary>
        public DateTime LastLoginTime { get; set; }

        /// <summary>
        /// 是否有系统管理权限
        /// </summary>
        public bool Admin { get; set; }
    }

    #endregion

    #region 角色信息实体(Role)

    /// <summary>
    /// 角色信息实体
    /// </summary>
    public class Role
    {
        /// <summary>
        /// 角色ID
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 角色名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 角色类型
        /// </summary>
        public RoleTypeOptions RoleType { get; set; }

        /// <summary>
        /// 角色状态(启用或禁用)
        /// </summary>
        public StatusOptions Status { get; set; }

        /// <summary>
        /// 角色具体描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 该角色应用于哪个平台
        /// </summary>
        public int SystemID { get; set; }
    }

    #endregion

    #region 权限信息实体(Right)

    /// <summary>
    /// 权限信息实体
    /// </summary>
    public class Right
    {
        /// <summary>
        /// 权限ID
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 权限名称
        /// </summary>
        public string Name { get; set; }

        ///// <summary>
        ///// 权限KEY
        ///// </summary>
        //public string RightKey { get; set; }

        /// <summary>
        /// 权限状态(启用或禁用)
        /// </summary>
        public StatusOptions Status { get; set; }

        /// <summary>
        /// 父级权限ID
        /// </summary>
        public int ParentID { get; set; }

        /// <summary>
        /// 权限树层次(深度)
        /// </summary>
        public int RightLevel { get; set; }

        /// <summary>
        /// 权限类型
        /// </summary>
        public RightTypeOptions RightType { get; set; }

        /// <summary>
        /// 权限类型为页面时,对应其URL
        /// </summary>
        public string PageUrl { get; set; }

        /// <summary>
        /// 同一组权限下的排序值
        /// </summary>
        public int SortIndex { get; set; }

        /// <summary>
        /// 是否允许外部请求
        /// </summary>
        public bool OnlyInternal { get; set; }

        /// <summary>
        /// 添加时间
        /// </summary>
        public DateTime AddTime { get; set; }

        /// <summary>
        /// 最后一次更新时间
        /// </summary>
        public DateTime LastUpdateTime { get; set; }

        /// <summary>
        /// 权限具体描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 该权限属于哪个平台
        /// </summary>
        public int SystemID { get; set; }
    }

    #endregion

    #region 操作日志实体 AdminLog

    /// <summary>
    /// 操作日志实体
    /// </summary>
    public class AdminLog
    {
        /// <summary>
        /// 日志ID
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 操作者账号
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// 用户类型
        /// </summary>
        public UserTypeOptions AccountType { get; set; }

        /// <summary>
        /// 真实姓名
        /// </summary>
        public string TrueName { get; set; }

        /// <summary>
        /// 操作时IP
        /// </summary>
        public string IP { get; set; }

        /// <summary>
        /// 操作时间
        /// </summary>
        public DateTime AddTime { get; set; }

        /// <summary>
        /// 对应页面地址
        /// </summary>
        public string PageUrl { get; set; }

        /// <summary>
        /// 具体操作描述
        /// </summary>
        public string Memo { get; set; }

        /// <summary>
        /// 操作日志源于哪个平台
        /// </summary>
        public int SystemID { get; set; }
    }

    #endregion

    #region 产品信息实体(Soft)

    /// <summary>
    /// 产品信息实体
    /// </summary>
    public class Soft
    {
        /// <summary>
        /// 产品ID
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 通用ID
        /// </summary>
        public int OutID { get; set; }

        /// <summary>
        /// 产品名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 产品类型
        /// </summary>
        public SoftTypeOptions SoftType { get; set; }

        /// <summary>
        /// 是否允许外部请求
        /// </summary>
        public bool OnlyInternal { get; set; }

        /// <summary>
        /// 排序值
        /// </summary>
        public int SortIndex { get; set; }

        /// <summary>
        /// 权限状态(启用或禁用)
        /// </summary>
        public StatusOptions Status { get; set; }

        /// <summary>
        /// 添加时间
        /// </summary>
        public DateTime AddTime { get; set; }

        /// <summary>
        /// 可支持的平台
        /// </summary>
        public List<MobileOption> Platforms { get; set; }

        //软件所属大类
        public int StatAloneID { get; set; }

        public int SoftAreaType { get; set; }

        /// <summary>
        /// 关联的项目来源
        /// </summary>
        public List<ProjectSource> ProjectSources { get; set; }
    }

    #endregion

    #region 项目来源信息实体(ProjectSource)

    /// <summary>
    /// 项目来源信息实体
    /// </summary>
    public class ProjectSource
    {
        /// <summary>
        /// 项目来源ID
        /// </summary>
        public int ProjectSourceID { get; set; }

        /// <summary>
        /// 对应的软件ID
        /// </summary>
        public int SoftID { get; set; }

        /// <summary>
        /// 项目来源名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 是否允许外部请求
        /// </summary>
        public bool OnlyInternal { get; set; }

        /// <summary>
        /// 排序值
        /// </summary>
        public int SortIndex { get; set; }

        /// <summary>
        /// 权限状态(启用或禁用)
        /// </summary>
        public StatusOptions Status { get; set; }

        /// <summary>
        /// 添加时间
        /// </summary>
        public DateTime AddTime { get; set; }

        /// <summary>
        /// 项目来源类型
        /// </summary>
        public ProjectSourceTypeOptions ProjectSourceType { get; set; }
    }

    #endregion

    #region 资源类型实体(ResourceType)

    /// <summary>
    /// 资源类型实体
    /// </summary>
    public class ResourceType
    {
        /// <summary>
        /// 资源类型定义值
        /// </summary>
        public int TypeID { get; set; }

        /// <summary>
        /// 类型名称
        /// </summary>
        public string TypeName { get; set; }
    }

    #endregion

    #region 用于权限分配的权限项(RightItem)

    /// <summary>
    /// 用于权限分配的权限项
    /// </summary>
    public class RightItem
    {
        /// <summary>
        /// 权限ID
        /// </summary>
        public int RightID { get; set; }

        /// <summary>
        /// 是否从角色中继承过来
        /// </summary>
        public bool FromRole { get; set; }
    }

    #endregion

    #region 渠道权限实体(ChannelRight)

    /// <summary>
    /// 渠道权限实体
    /// </summary>
    public class ChannelRight
    {
        /// <summary>
        /// 渠道类型(分类或渠道商)
        /// </summary>
        public ChannelTypeOptions ChannelType { get; set; }

        /// <summary>
        /// 渠道ID
        /// </summary>
        public int ChannelID { get; set; }
    }

    #endregion

    #region 渠道信息实体(Channel)

    /// <summary>
    /// 渠道信息实体
    /// </summary>
    public class Channel
    {
        /// <summary>
        /// 渠道ID
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 渠道名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 父级渠道ID
        /// </summary>
        public int ParentID { get; set; }

        /// <summary>
        /// 平台
        /// </summary>
        public MobileOption Platform { get; set; }

        /// <summary>
        /// 渠道类型
        /// </summary>
        public ChannelTypeOptions ChannelType { get; set; }

        /// <summary>
        /// 父级渠道类型
        /// </summary>
        public ChannelTypeOptions ParentChannelType { get; set; }        
    }

    #endregion

    #region 登录上下文信息实体(UserContext)

    /// <summary>
    /// 登录上下文信息实体
    /// </summary>
    internal class UserContext
    {
        /// <summary>
        /// 登录用户信息实例
        /// </summary>
        public User LoginUser { get; set; }

        /// <summary>
        /// 拥有的权限列表
        /// </summary>
        public List<Right> AvailableRights { get; set; }

        /// <summary>
        /// 有权限的产品列表
        /// </summary>
        public List<Soft> AvailableSofts { get; set; }

        /// <summary>
        /// 有权限的项目来源列表
        /// </summary>
        public List<ProjectSource> AvailableProjectSources { get; set; }

        /// <summary>
        /// 有权限的资源列表
        /// </summary>
        public List<int> AvailableResIds { get; set; }

        /// <summary>
        /// 有管理权限的系统
        /// </summary>
        public List<SystemInfo> AdminSystems { get; set; }
    }

    #endregion

    #region 没有权限异常类型(NotRightException)

    /// <summary>
    /// 没有权限异常类型
    /// </summary>
    public class NotRightException : ApplicationException
    {
    }

    /// <summary>
    /// 校验数据正确性
    /// </summary>
    public class CheckException : Exception
    {
        public string Mes;

        public CheckException(string ex)
        {
            Mes = ex;
        }
    }

    #endregion
}