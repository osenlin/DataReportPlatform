using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Text.RegularExpressions;

namespace net91com.Reports.Entities.DataBaseEntity
{
    public class LinkTagInfo
    {
        /// <summary>
        /// 默认构造函数
        /// </summary>
        public LinkTagInfo()
        {
        }

        /// <summary>
        /// 来自数据库的数据构造
        /// </summary>
        public LinkTagInfo(IDataReader reader)
            : this()
        {
            LoadFromDb(reader);
        }

        /// <summary>
        /// 来自数据库的数据构造
        /// </summary>
        public void LoadFromDb(IDataReader reader)
        {
            for (int i = 0; i < reader.FieldCount; i++)
            {
                switch (reader.GetName(i).ToLower())
                {
                    case "id":
                        ID = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["ID"]);
                        break;
                    case "softid":
                        SoftID = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["SoftID"]);
                        break;
                    case "platform":
                        Platform = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["Platform"]);
                        break;
                    case "tag":
                        LinkTag = reader.IsDBNull(i) ? "" : reader["Tag"].ToString();
                        break;
                    case "linkname":
                        LinkName = reader.IsDBNull(i) ? "" : reader["LinkName"].ToString();
                        break;
                    case "url":
                        LinkUrl = reader.IsDBNull(i) ? "" : reader["Url"].ToString();
                        break;
                    case "cid":
                        CID = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["CID"]);
                        break;
                    case "linktype":
                        LinkType = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["LinkType"]);
                        break;
                    case "updatetime":
                        UpdateTime = reader.IsDBNull(i) ? DateTime.MinValue : Convert.ToDateTime(reader["UpdateTime"]);
                        break;
                    case "channelid":
                        ChannelId =reader.IsDBNull(i) ? "" : reader["channelid"].ToString();
                        break;
                }
            }
        }

        public int ID { get; set; }
        public int SoftID { get; set; }
        public int Platform { get; set; }
        public string LinkTag { get; set; }
        public string LinkName { get; set; }
        public string LinkUrl { get; set; }
        public string AppVersion { get; set; }
        public string Channel { get; set; }
        public int CID { get; set; }
        public int LinkType { get; set; }
        public DateTime UpdateTime { get; set; }

        public string ChannelId { get; set; }

        /// <summary>
        /// AppStore跳转地址编码
        /// </summary>
        public void AppStoreUrlEncode()
        {
            if (LinkType == 1)
            {
                string appid = net91com.Core.Util.CryptoHelper.DES_Encrypt(SoftID.ToString(), "91appuse");
                string appStoreUrl = System.Web.HttpUtility.UrlEncode(LinkUrl.Trim());
                string channel = net91com.Stat.Core.UtilHelper.EncryptDES(Channel, "5a@4a$0e", "e2Eb9A82");
                LinkUrl = string.Format(@"http://funcstatic.sj.91.com/link.ashx?id={0}&mt={1}&v={2}&chl={3}&url={4}",
                    appid, Platform, AppVersion.Trim(), channel, appStoreUrl);
            }
            if (ID == 0 && string.IsNullOrEmpty(LinkTag))
            {
                LinkTag = net91com.Stat.Core.UtilHelper.ShortUrl(SoftID + "_" + Platform + "_" + DateTime.Now.Ticks)[0];
            }
        }

        private static readonly Regex regexUrl = new Regex(@"&v=([^&]*)&chl=([^&]*)&url=([^&]*)", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        /// <summary>
        /// AppStore跳转地址解码
        /// </summary>
        public void AppStoreUrlDecode()
        {
            if (LinkType == 1)
            {
                string url = LinkUrl;
                Match mat = regexUrl.Match(url);
                if (mat.Success)
                {
                    AppVersion = mat.Groups[1].Value;
                    Channel = net91com.Stat.Core.UtilHelper.DecryptChannelName(mat.Groups[2].Value);
                    LinkUrl = System.Web.HttpUtility.UrlDecode(mat.Groups[3].Value);
                }
                else
                {
                    AppVersion = string.Empty;
                    Channel = string.Empty;
                    LinkUrl = string.Empty;
                }
                mat = null;
            }
        }
    }
}
