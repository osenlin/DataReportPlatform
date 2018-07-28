using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using System.Reflection;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using net91com.Core;
using net91com.Stat.Services.Entity;


namespace net91com.Stat.Web.Reports.Services
{

    //[Serializable]
    //public class SoftInfo :ICloneable
    //{
    //    /// <summary>
    //    /// 软件ID
    //    /// </summary>
    //    public int SoftID { get; set; }

    //    /// <summary>
    //    /// UserOptionStatDB 中使用的id
    //    /// </summary>
    //    public int SoftOutID { get; set; }

    //    /// <summary>
    //    /// 软件名称
    //    /// </summary>
    //    public string SoftName { get; set; }

    //    /// <summary>
    //    /// 软件名称 拼音首字母
    //    /// </summary>
    //    public string SoftNamePinyin { get; set; }

    //    /// <summary>
    //    /// 是否默认选中
    //    /// </summary>
    //    public bool Checked { get; set; }

    //    /// <summary>
    //    /// 用于SIMPLE权限验证的KEY
    //    /// </summary>
    //    public string SimpleKey { get; set; }

    //    /// <summary>
    //    /// 软件类型
    //    /// </summary>
    //    public int SoftType { get; set; }

    //    /// <summary>
    //    /// 可支持的平台列表
    //    /// </summary>
    //    public List<PlatformInfo> PlatformList { get; set; }

    //    public object Clone()
    //    {
    //        MemoryStream stream = new MemoryStream();
    //        BinaryFormatter formatter = new BinaryFormatter();
    //        formatter.Serialize(stream, this);
    //        stream.Position = 0;
    //        var obj = formatter.Deserialize(stream) as SoftInfo;
    //        return obj;

    //    }
    //}
    ///// <summary>
    ///// 平台信息实体
    ///// </summary>
    //[Serializable]
    //public class PlatformInfo:ICloneable
    //{
    //    /// <summary>
    //    /// 平台枚举值
    //    /// </summary>
    //    public int PlatformValue { get; set; }

        
    //    /// 平台名称
    //     public string PlatformName { get; set; }

    //    public override  bool Equals(Object other)
    //    {
    //        if (other is PlatformInfo)
    //        {
    //            if (((PlatformInfo)other).PlatformValue == this.PlatformValue)
    //                return true;
    //            else
    //                return false;
    //        }
    //        else
    //            return false;
    //    }
    //    public override int GetHashCode()
    //    {

    //        return this.PlatformValue.GetHashCode() * this.PlatformName.GetHashCode();
            
    //    }
    //    public object Clone()
    //    {
    //        MemoryStream stream = new MemoryStream();
    //        BinaryFormatter formatter = new BinaryFormatter();
    //        formatter.Serialize(stream, this);
    //        stream.Position = 0;
    //        var obj = formatter.Deserialize(stream) as PlatformInfo;
    //        return obj;

    //    }


       
    //}

    ///// <summary>
    ///// 用于distinct 比较
    //public class SoftCompare : IEqualityComparer<SoftInfo>
    //{
    //    public bool Equals(SoftInfo x, SoftInfo y)
    //    {

    //        if (x == null)

    //            return y == null;

    //        return x.SoftID == y.SoftID;

    //    }



    //    public int GetHashCode(SoftInfo obj)
    //    {

    //        if (obj == null)

    //            return 0;

    //        return obj.SoftID.GetHashCode();

    //    }


        
    //}

    
    public static  class Utility
    {
         
        /// <summary>
        /// 获取统计的所有软件列表
        /// </summary>
        //public static List<SoftInfo> SoftList
        //{
        //    get
        //    {
        //        if (softList == null)
        //        {
        //            softList = new List<SoftInfo>();
        //            XmlDocument document = new XmlDocument();
        //            string softsPath = HttpContext.Current.Server.MapPath("Softs.xml");
        //            document.Load(softsPath);
        //            XmlNodeList softNodes = document.DocumentElement.SelectNodes("soft");
        //            foreach (XmlNode sn in softNodes)
        //            {
        //                SoftInfo soft = new SoftInfo();
                         
        //                soft.SoftID = int.Parse(sn.Attributes["id"].Value);
        //                soft.SoftName = sn.Attributes["name"].Value;
        //                soft.Checked = bool.Parse(sn.Attributes["checked"].Value);
        //                if (sn.Attributes["OutID"] != null)
        //                    soft.SoftOutID = int.Parse(sn.Attributes["OutID"].Value);
                       
        //                soft.SimpleKey = sn.Attributes["simpleKey"].Value;

        //                soft.PlatformList = new List<PlatformInfo>();
                       
        //                XmlNodeList platformNodes = sn.SelectNodes("platform");
        //                foreach (XmlNode pn in platformNodes)
        //                {
        //                    soft.PlatformList.Add(
        //                        new PlatformInfo
        //                        {
                                    
        //                            PlatformValue = int.Parse(pn.Attributes["value"].Value),
        //                            PlatformName = pn.InnerText.Trim()
        //                        });
        //                }
        //                softList.Add(soft);
        //            }
        //        }
        //        return softList;
        //    }
        //}
        //public static List<SoftInfo> SoftList
        //{
        //    get {
        //            List<SoftInfo> softList = new List<SoftInfo>();
        //            DataService ds = new DataService(true);
        //            List<SoftInfoAll> softinfoAll = ds.GetSoftAndPlatCache(Core.CacheTimeOption.TenMinutes);
        //            softList = softinfoAll.Select(p => new SoftInfo { SoftID = p.SoftID, SoftName = p.SoftName, SoftNamePinyin = net91com.Core.Util.TextHelper.GetChineseInitials(p.SoftName), SoftOutID = p.SoftOutID, SimpleKey = p.SoftRightKey, SoftType = p.SoftType }).Distinct(new SoftCompare()).ToList();
        //            foreach (var item in softList)
        //            {
        //                List<PlatformInfo> platform = new List<PlatformInfo>();
        //                platform = softinfoAll.Where(p => p.SoftID == item.SoftID)
        //                            .Select(p => new PlatformInfo { PlatformName = p.PlatformName, PlatformValue = p.Pid }).ToList();
        //                item.PlatformList = platform;
        //            }
        //            return softList;
        //       }
        //}

        ///// <summary>
        ///// 获取JSON格式的软件列表
        ///// </summary>
        //public static string GetSoftListJson(List<SoftInfo> softs)
        //{
        //    if (softs != null && softs.Count > 0)
        //    {
        //        string json = "var softs=[";
        //        for (int i = 0; i < softs.Count; i++)
        //        {
        //            json += string.Format("{{\"id\":{0},\"platforms\":[", softs[i].SoftID);
        //            for (int j = 0; j < softs[i].PlatformList.Count; j++)
        //            {
        //                json += string.Format("{0},", (int)softs[i].PlatformList[j]);
        //            }
        //            json = json.TrimEnd(',');
        //            json += "]},";
        //        }
        //        json = json.TrimEnd(',');
        //        json += "];";
        //        return json;
        //    }
        //    return string.Empty;

        //}
         
        ///// <summary>
        ///// 老方法获取数据（比较臃肿，在渠道商管理上有用到）
        ///// </summary>
        ///// <param name="softs"></param>
        ///// <returns></returns>
        //public static string GetSoftListJsonOld(List<SoftInfo> softs)
        //{
        //    if (softs != null && softs.Count > 0)
        //    {
        //        string json = "var softs=[";
        //        for (int i = 0; i < softs.Count; i++)
        //        {
        //            json += string.Format("{{\"id\":{0},\"oid\":{1},\"name\":\"{2}\",\"py\":\"{3}\",\"platforms\":[", softs[i].SoftID, softs[i].SoftOutID, softs[i].SoftName, softs[i].SoftNamePinyin);
        //            for (int j = 0; j < softs[i].PlatformList.Count; j++)
        //            {
        //                json += string.Format("{{\"val\":{0},\"name\":\"{1}\"}},", (int)softs[i].PlatformList[j], softs[i].PlatformList[j].ToString());
        //            }
        //            json = json.TrimEnd(',');
        //            json += "]},";
        //        }
        //        json = json.TrimEnd(',');
        //        json += "];";
        //        return json;
        //    }
        //    return string.Empty;

        //}

        //public static string GetSOftListJsonForOption(List<SoftInfo> softs)
        //{
            //softs = softs.Where(p => p.SoftOutID != 0).ToList();
            //string json = "var softs=[";
            //for (int i = 0; i < softs.Count; i++)
            //{
            //    json += string.Format("{{\"id\":{0},\"name\":\"{1}\",\"platforms\":[", softs[i].SoftID, softs[i].SoftName);
            //    for (int j = 0; j < softs[i].PlatformList.Count; j++)
            //    {
            //        json += string.Format("{{\"val\":{0},\"name\":\"{1}\"}},", (int)softs[i].PlatformList[j], softs[i].PlatformList[j].ToString());
            //    }
            //    json = json.TrimEnd(',');
            //    json += "]},";
            //}
            //json = json.TrimEnd(',');
            //json += "];";
            //return json;
        //}

        //public static SoftInfo GetSoftInfo(int softid)
        //{
        //    return SoftList.Find(delegate(SoftInfo si) { return si.SoftID == softid; });
        //}

        //public static SoftInfo GetSoftInfoByOutID (int Outid)
        //{
        //    return SoftList.Find(delegate(SoftInfo si) { return si.SoftOutID == Outid; });
        //}
        //public static SoftInfo GetSoftInfoByName(string name)
        //{
        //    return SoftList.Find(delegate(SoftInfo si) { return si.SoftName == name; });
        //}

        public static IList<T> Clone<T>(this IList<T> source)
        where T : ICloneable
        {
            IList<T> newList = new List<T>(source.Count);
            foreach (var item in source)
            {
                newList.Add((T)((ICloneable)item.Clone()));
            }
            return newList;
        }

        public static string SetNum(int m)
        {
            return string.Format("{0:N0}", m);
           

        }
        public static string SetNum(long m)
        {
            return string.Format("{0:N0}", m);
        }

        /// <summary>
        /// 得到标准化100%数据
        /// </summary>
        /// <param name="molecules"></param>
        /// <param name="denominator"></param>
        /// <returns></returns>
        public static string GetPercent(int molecules, int denominator)
        {
            if (denominator == 0)
                return "--";
            else if (molecules >= denominator)
                return "100%";
            else
                return (molecules * 1.0 / denominator).ToString("0.00%");
        }
        public static string GetPercent(long molecules, long denominator)
        {
            if (denominator == 0)
                return "--";
            else if (molecules >= denominator)
                return "100%";
            else
                return (molecules * 1.0 / denominator).ToString("0.00%");
        }

        public static string CssVersion = net91com.Core.Util.ConfigHelper.GetSetting("CssVersion", "1");
        public static string JsVersion = net91com.Core.Util.ConfigHelper.GetSetting("JsVersion", "1");
    }
}