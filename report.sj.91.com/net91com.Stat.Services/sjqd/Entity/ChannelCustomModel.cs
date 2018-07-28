using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using net91com.Core;

namespace net91com.Stat.Services.sjqd.Entity
{
    public class ChannelCategories
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public int SoftID { get; set; }

        public DateTime InDate { get; set; }

        /// <summary>
        /// 构树的时候用到，孩子渠道商
        /// </summary>
        public List<ChannelCustomers> SonCustomers = new List<ChannelCustomers>();
    }

    public class ChannelCustomers
    {
        public ChannelCustomers()
        {
            this.Modulus1 = 0m;
            this.Modulus2 = 0m;
        }

        public int ID { get; set; }

        /// <summary>
        /// 渠道商名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int IsRealtime { get; set; }

        public int PID { get; set; }

        public int CID { get; set; }

        public decimal Modulus1 { get; set; }

        public decimal Modulus2 { get; set; }

        /// <summary>
        /// 0 未设置， 1 一次乘以系数提供用户 2 二次乘以系数提供用户
        /// </summary>
        public int ReportType { get; set; }

        /// <summary>
        /// 给内部人看的key
        /// </summary>
        public string Keyfornd { get; set; }

        /// <summary>
        /// 给外部人看的key
        /// </summary>
        public string Keyforout { get; set; }

        public int Modulus_Shanzhai { get; set; }

        public int SoftID { get; set; }

        /// <summary>
        /// 展示类型
        /// </summary>
        public int ShowType { get; set; }

        public DateTime MinViewTime { get; set; }

        /// <summary>
        /// 构树的时候用到
        /// </summary>
        public List<ChannelCustomers> SonCustomers { get; set; }
    }

    /// <summary>
    /// 渠道 实体
    /// </summary>
    public class Channels
    {
        /// <summary>
        /// 自增id(用于编辑修改的)
        /// </summary>
        public int AutoID { get; set; }

        /// <summary>
        /// 真正的渠道ID(关联其他表的)
        /// </summary>
        public int ChannelID { get; set; }

        public int SoftID { get; set; }

        public MobileOption Platform { get; set; }

        public string Name { get; set; }

        public string E_Name { get; set; }

        /// <summary>
        /// 上级渠道商
        /// </summary>
        public int CCID { get; set; }

        public decimal Modulus1 { get; set; }

        public int Modulus_Shanzhai { get; set; }
    }

    /// <summary>
    /// 页面上树形节点实例对象
    /// </summary>
    public class Node
    {
        /// <summary>
        /// 显示名称
        /// </summary>
        public string title { get; set; }

        /// <summary>
        /// 显示的key
        /// </summary>
        public string key { get; set; }

        /// <summary>
        /// 是否为文件夹
        /// </summary>
        public bool expand = false;

        public bool isFolder { get; set; }
        public string addClass { get; set; }

        public bool activate = false;
        public List<Node> children { get; set; }
    }

    /// <summary>
    /// 客户端上传过来的channel
    /// </summary>
    public class ChannelFormClient
    {
        public int AuID { get; set; }

        public int SoftID { get; set; }

        public MobileOption Platform { get; set; }

        public string Name { get; set; }

        /// <summary>
        /// 是否绑定
        /// </summary>
        public bool Bound { get; set; }
    }

    /// <summary>
    /// 头部控件中异步实体类
    /// </summary>
    public class ChannelInfo
    {
        /// <summary>
        /// 渠道ID
        /// </summary>
        public int ChannelID { get; set; }

        /// <summary>
        /// 渠道名称
        /// </summary>
        public string ChannelName { get; set; }

        /// <summary>
        /// 渠道真实名称
        /// </summary>
        public string ChannelRealName { get; set; }

        /// <summary>
        /// 软件ID
        /// </summary>
        public int SoftID { get; set; }

        /// <summary>
        /// 软件平台
        /// </summary>
        public MobileOption Platform { get; set; }
    }
}