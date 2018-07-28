using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using net91com.Core;

namespace net91com.Stat.Services.Sjzj.Entity
{
    /// <summary>
    /// 手机助手的固件版本数据
    /// </summary>
    public class FwVersions
    {
        public int ID { get; set; }

        /// <summary>
        /// 平台
        /// </summary>
        public MobileOption Platform { get; set; }

        private string _fwVersion = string.Empty;

        /// <summary>
        /// 上传固件
        /// </summary>
        public string FwVersion
        {
            get { return _fwVersion; }
            set { _fwVersion = value; }
        }

        private string _e_fwVersion = string.Empty;

        /// <summary>
        /// 编辑固件
        /// </summary>
        public string E_FwVersion
        {
            get { return _e_fwVersion; }
            set { _e_fwVersion = value; }
        }

        public string GJBB
        {
            get { return _fwVersion; }
        }

        public string E_GJBB
        {
            get { return _e_fwVersion; }
        }
    }

    /// <summary>
    /// 手机助手的机型 Sjzs__Jixing
    /// </summary>
    public class Jixings
    {
        public int ID { get; set; }

        public MobileOption Platform { get; set; }

        private string _jixing = string.Empty;

        public string Jixing
        {
            get { return _jixing; }
            set { _jixing = value; }
        }

        private string _e_jixing = string.Empty;

        public string E_Jixing
        {
            get { return _e_jixing; }
            set { _e_jixing = value; }
        }

        public string Sbxh
        {
            get { return _jixing; }
        }

        public string E_Sbxh
        {
            get { return _e_jixing; }
        }
    }

    #region 软件版本

    /// <summary>
    /// 软件版本
    /// </summary>
    public class SjzsVersions
    {
        public int ID { get; set; }

        public long SoftId { get; set; }

        public MobileOption Platform { get; set; }

        public string Version { get; set; }

        public string E_Version { get; set; }
    }

    #endregion

    #region  手机助手操作系统

    /// <summary>
    /// 手机助手操作系统
    /// </summary>
    public class SjzsOsVersions
    {
        public int ID { get; set; }

        public string OsVersion { get; set; }

        public string E_OsVersion { get; set; }
    }

    #endregion

    #region 手机助手地区

    /// <summary>
    /// 手机助手地区
    /// </summary>
    public class SjzsArea
    {
        public int ID { get; set; }

        /// <summary>
        /// 国家
        /// </summary>
        public string Country { get; set; }

        /// <summary>
        /// 省份
        /// </summary>
        public string Province { get; set; }

        public string City { get; set; }
    }

    #endregion
}