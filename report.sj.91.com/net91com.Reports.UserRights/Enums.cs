using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace net91com.Reports.UserRights
{

    #region 用户类型(UserTypeOptions)

    /// <summary>
    /// 用户类型
    /// </summary>
    public enum UserTypeOptions
    {
        /// <summary>
        /// 普通用户
        /// </summary>
        General = 0,

        /// <summary>
        /// 管理员
        /// </summary>
        Admin = 1,

        /// <summary>
        /// 超级管理员
        /// </summary>
        SuperAdmin = 2,

        /// <summary>
        /// 产品管理员
        /// </summary>
        ProductAdmin = 3,

        /// <summary>
        /// 渠道内部用户
        /// </summary>
        Channel = 4,

        /// <summary>
        /// 渠道合作方用户
        /// </summary>
        ChannelPartner = 5
    }

    #endregion

    #region 角色类型(RoleTypeOptions)

    /// <summary>
    /// 角色类型
    /// </summary>
    public enum RoleTypeOptions
    {
        /// <summary>
        /// 普通角色
        /// </summary>
        General = 0,

        /// <summary>
        /// 渠道内部用户
        /// </summary>
        Channel = 1,

        /// <summary>
        /// 渠道合作方
        /// </summary>
        ChannelPartner = 2,
    }

    #endregion

    #region 状态类型(StatusOptions)

    /// <summary>
    /// 状态类型
    /// </summary>
    public enum StatusOptions
    {
        /// <summary>
        /// 无效或禁用状态
        /// </summary>
        Invalid = 0,

        /// <summary>
        /// 有效或启用状态
        /// </summary>
        Valid = 1
    }

    #endregion

    #region 权限类型(RightTypeOptions)

    /// <summary>
    /// 权限类型
    /// </summary>
    public enum RightTypeOptions
    {
        /// <summary>
        /// 分类
        /// </summary>
        Category = 0,

        /// <summary>
        /// 页面
        /// </summary>
        Page = 1,

        /// <summary>
        /// 按钮
        /// </summary>
        Button = 2
    }

    #endregion

    #region 产品类型(SoftTypeOptions)

    /// <summary>
    /// 产品类型
    /// </summary>
    public enum SoftTypeOptions
    {
        /// <summary>
        /// 内部产品
        /// </summary>
        InternalSoft = 1,

        /// <summary>
        /// 渠道内部用户产品
        /// </summary>
        OuterSoft = 2,

        /// <summary>
        /// 多产品
        /// </summary>
        MultiSofts = 3
    }

    #endregion

    #region 项目来源类型(ProjectSourceTypeOptions)

    /// <summary>
    /// 项目来源类型
    /// </summary>
    public enum ProjectSourceTypeOptions
    {
        /// <summary>
        /// 不限定
        /// </summary>
        None = 0,

        /// <summary>
        /// 国内
        /// </summary>
        Domestic = 1,

        /// <summary>
        /// 海外
        /// </summary>
        Oversea = 2,

        /// <summary>
        /// 台湾
        /// </summary>
        Traditional = 3
    }

    #endregion

    #region 渠道信息类型(ChannelTypeOptions)

    /// <summary>
    /// 渠道信息类型
    /// </summary>
    public enum ChannelTypeOptions
    {
        /// <summary>
        /// 渠道分类
        /// </summary>
        Category = 1,

        /// <summary>
        /// 渠道商
        /// </summary>
        Customer = 2,

        /// <summary>
        /// 渠道ID
        /// </summary>
        ChannelID = 3
    }

    #endregion
}