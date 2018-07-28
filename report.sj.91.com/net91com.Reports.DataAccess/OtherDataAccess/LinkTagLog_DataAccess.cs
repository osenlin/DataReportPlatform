using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

using MySql.Data.MySqlClient;

using net91com.Core.Data;
using net91com.Reports.Entities.DataBaseEntity;
using net91com.Core.Util;

namespace net91com.Reports.DataAccess.OtherDataAccess
{
    public class LinkTagLog_DataAccess : BaseDataAccess
    {
        private static LinkTagLog_DataAccess instance = null;
        private static readonly object obj = new object();
        public static LinkTagLog_DataAccess Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (obj)
                    {
                        if (instance == null)
                        {
                            instance = new LinkTagLog_DataAccess();
                            instance._cachePreviousKey = "LinkTagLog_DataAccess";
                        }
                    }
                }
                return instance;
            }
        }


        public List<LinkTagLog> GetTags(int softId, bool includeTagIds)
        {
            string cmdText = string.Format("call sp_GetLinkCategories({0},{1});", softId, includeTagIds);
            List<LinkTagLog> list = new List<LinkTagLog>();
            using (IDataReader reader = MySqlHelper.ExecuteReader(MySQLConnectionString, cmdText))
            {
                while (reader.Read())
                {
                    list.Add(new LinkTagLog(reader));
                }
            }
            return list;
        }

        public List<LinkTagCount> GetTagCountList(DateTime begin, DateTime end, int period, int softid, int platform, int version, int tagId, string tagText, bool isCategory)
        {
            string cmdText = string.Empty;
            if (isCategory)
            {
                //tagid就是categoryID
                cmdText = @"
CALL sp_GetLinkTreeNodeCategories(?softid,TRUE," + (tagId > 0 ? "?tagid" : "0") + @");
drop table IF EXISTS cte;
create temporary table cte 
select ID from link_cte;";

                if (version == 0)
                {
                    cmdText += @"
 select c.StatDate,c.SoftID,c.Platform,Sum(c.StatCount) StatCount,'" + tagText + @"' TagName
 from Cfg_LinkTags a  inner join cte b on a.CID=b.ID #and a.TagID>0
  inner join Link_StatCount c  on a.Tag=c.Tag
  and c.StatDate between ?begin and ?end and Period=?period and c.Softid=?softid and c.Platform=?platform
 group by c.StatDate,c.SoftID,c.Platform";
                }
                else
                {
                    cmdText += @"
 select c.StatDate,c.SoftID,c.Platform,VersionID,Sum(StatCount) StatCount,'" + tagText + @"' TagName
 from Cfg_LinkTags a inner join cte b on a.CID=b.ID #and a.TagID>0
  inner join Link_StatCountByVersion c  on a.Tag=c.Tag
  and c.StatDate between ?begin and ?end and Period=?period and c.Softid=?softid and c.Platform=?platform and c.VersionID=?version
 group by c.StatDate,c.SoftID,c.Platform,c.VersionID";
                }
            }
            else
            {
                if (version == 0)
                {
                    cmdText = string.Format(@"
 select StatDate,SoftID,Platform,Sum(StatCount) StatCount,'{0}' TagName
 from Link_StatCount 
 where StatDate between ?begin and ?end and Period=?period and Softid=?softid and Platform=?platform {1}
 group by StatDate,SoftID,Platform", tagText, tagText == "" ? "and Tag='-1'" : " and Tag=?tag");
                }
                else
                {
                    cmdText = @"
 select StatDate,SoftID,Platform,VersionID,Sum(StatCount) StatCount,'" + tagText + @"' TagName
 from Link_StatCountByVersion 
 where StatDate between ?begin and ?end and Period=?period and Softid=?softid and Platform=?platform and VersionID=?version  " + tagText == "" ? "and Tag='-1'" : "and Tag=?tag" + @"
 group by StatDate,SoftID,Platform,VersionID";
                }
            }

            String tagName=ToolDataAccess.Instance.getTagByTagId(tagId);
            var param = new MySqlParameter[] 
            { 
                new MySqlParameter("?softid",  softid),
                new MySqlParameter("?platform",platform),
                new MySqlParameter("?begin",   Convert.ToInt32(begin.ToString("yyyyMMdd"))),
                new MySqlParameter("?end",     Convert.ToInt32(end.ToString("yyyyMMdd"))),
                new MySqlParameter("?period",  period),
                new MySqlParameter("?version", version),
                //todo tagId要转化成tag
                new MySqlParameter("?tagid",   tagId),
                new MySqlParameter("?tag",   tagName)
            };
            List<LinkTagCount> list = new List<LinkTagCount>();
            using (var reader = MySqlHelper.ExecuteReader(StatConn,cmdText, param))
            {
                while (reader.Read())
                {
                    list.Add(new LinkTagCount(reader));
                }
            }
            return list;
        }

        public DataTable GetTagCountTable(DateTime statDate, int softid, int platform)
        {
            var sql = string.Format(@"
 select {3} StatDate,a.Tag,a.linkCount,b.linkCount linkCountYesterday 
 from (
  select Tag,sum(LinkCount) linkCount
  from Link_StatLinkCount
  where SoftID={0} {1} and StatDate={3} and Period=1
  group by Tag
 ) a left join (
  select Tag,sum(LinkCount) linkCount
  from Link_StatLinkCount
  where SoftID={0} {1} and StatDate={2} and Period=1
  group by Tag
 ) b on a.Tag=b.Tag;
", softid, (platform == 0 ? " and Platform in (1,4)" : " and Platform=" + platform),
 statDate.AddDays(-1).ToString("yyyyMMdd"), statDate.ToString("yyyyMMdd"));
            var ds = MySqlHelper.ExecuteDataset(MySQLConnectionString, sql);
            if (ds != null && ds.Tables.Count > 0)
            {
                return ds.Tables[0];
            }
            return null;
        }

        public DataTable GetTagCountMonthTable(int softid, int platform, string tag)
        {
            var sql = string.Format(@"
 select StatDate as t,sum(LinkCount) linkCount
 from Link_StatLinkCount
 where period=1 {0} and SoftID={1} and Tag='{2}' and statdate between {3} and {4}
 group by StatDate
 order by StatDate;", (platform == 0 ? " and Platform in (1,4)" : " and Platform=" + platform),
                    softid, tag, DateTime.Now.AddDays(-30).ToString("yyyyMMdd"), DateTime.Now.ToString("yyyyMMdd"));
            var ds = MySqlHelper.ExecuteDataset(MySQLConnectionString, sql);
            if (ds != null && ds.Tables.Count > 0)
            {
                return ds.Tables[0];
            }
            return null;
        }

        public DataTable GetTagCountDailyTable(DateTime statDate, int softid, int platform, string tag)
        {
            var sql = string.Format(@"
 select StatDate,Hours as t,sum(LinkCount) linkCount
 from Link_StatLinkCount
 where period=1 {0} and SoftID={1} and Tag='{2}' and statdate between {3} and {4}
 group by StatDate,Hours
 order by StatDate,Hours", (platform == 0 ? " and Platform in (1,4)" : " and Platform=" + platform),
                         softid, tag, statDate.AddDays(-2).ToString("yyyyMMdd"), statDate.ToString("yyyyMMdd"));
            var ds = MySqlHelper.ExecuteDataset(MySQLConnectionString, sql);
            if (ds != null && ds.Tables.Count > 0)
            {
                return ds.Tables[0];
            }
            return null;
        }

        public Dictionary<string, Dictionary<string, string>> GetLinkTagInfoDic(int softid, int platform)
        {
            string sql = string.Format("call sp_GetLinkName({0},{1});", softid, platform);
            var ds = MySqlHelper.ExecuteDataset(MySQLConnectionString, sql);
            var dics = new Dictionary<string, Dictionary<string, string>>();
            if (ds != null && ds.Tables.Count > 0)
            {
                var dt = ds.Tables[0];
                foreach (DataRow row in dt.Rows)
                {
                    var dic = new Dictionary<string, string>();
                    foreach (DataColumn column in dt.Columns)
                    {
                        dic.Add(column.ColumnName.ToLower(), row[column.ColumnName].ToString());
                    }
                    dics.Add(row["tag"].ToString(), dic);
                }
            }
            return dics;
        }

        public List<LinkTagInfo> GetTagInfoList(int softId, int platform, int cid)
        {
            string cmdText = @"
 select id,tag,linkname,linktype,url,Updatetime,ChannelId
 from Cfg_LinkTags
 where SoftID=?softid and Platform=?platform and CID=?cid";
            MySqlParameter[] param = new MySqlParameter[] {
                new MySqlParameter("?softid", softId),
                new MySqlParameter("?platform", platform),
                new MySqlParameter("?cid", cid)
            };
            List<LinkTagInfo> list = new List<LinkTagInfo>();
            using (IDataReader dataReader = MySqlHelper.ExecuteReader(MySQLConnectionString, cmdText, param))
            {
                while (dataReader.Read())
                {
                    LinkTagInfo obj = new LinkTagInfo(dataReader);
                    obj.AppStoreUrlDecode();
                    list.Add(obj);
                }
            }
            return list;
        }

        public bool ChangeCate(int id, int pid)
        {

            String cmdText = @"update Cfg_LinkCategories set PID=?pid where ID=?id";
            MySqlParameter[] param2 = new MySqlParameter[] {
                new MySqlParameter("@id", id),
                new MySqlParameter("@pid", pid)
            };
            return MySqlHelper.ExecuteNonQuery(MySQLConnectionString, cmdText, param2) > 0;
        }


        public bool AddCate(int softid, int pid, string name)
        {
            string cmdText = @"
 insert into Cfg_LinkCategories (SoftID,PID,Name,Updatetime) values(?softid,?pid,?name,now());
 select last_insert_id();";
            MySqlParameter[] parameters2 = new MySqlParameter[]
            {
                new MySqlParameter("?softid", softid),
                new MySqlParameter("?pid", pid),
                new MySqlParameter("?name", name)
            };
            int id = Convert.ToInt32(MySqlHelper.ExecuteNonQuery(MySQLConnectionString, cmdText, parameters2));

            return id > 0;
        }

//        public bool AddCate(int softid, int pid, string name)
//        {
//            string cmdText = @"
// insert into Cfg_LinkCategories (SoftID,PID,Name,Updatetime) values(?softid,?pid,?name,now());
// select last_insert_id();";
//            MySqlParameter[] parameters2 = new MySqlParameter[]
//            {
//                new MySqlParameter("?softid", softid),
//                new MySqlParameter("?pid", pid),
//                new MySqlParameter("?name", name)
//            };
//            int id = Convert.ToInt32(MySqlHelper.ExecuteScalar(MySQLConnectionString, cmdText, parameters2));
       
//            return id > 0;
//        }

        public bool UpdateCate(int id, string name)
        {
            string cmdText = @"update Cfg_LinkCategories set name=?name where id=?id;";
            MySqlParameter[] parameters2 = new MySqlParameter[]
            {
                new MySqlParameter("?name", name),
                new MySqlParameter("?id", id)
            };
            int cc = Convert.ToInt32(MySqlHelper.ExecuteNonQuery(MySQLConnectionString, cmdText, parameters2));

            return cc > 0;
        }

        public bool DeleteCate(int id)
        {
            string cmdText = @"
 CREATE TEMPORARY TABLE tmp_Cfg_LinkCategories(id int);

 insert into tmp_Cfg_LinkCategories(id)
 select id from Cfg_LinkCategories where pid=?id;

 delete from Cfg_LinkCategories
 where id=?id
  and not EXISTS(select 1 from tmp_Cfg_LinkCategories)
  and not EXISTS(select 1 from Cfg_LinkTags where cid=?id);";
            MySqlParameter[] parameters2 = new MySqlParameter[]
            {
                new MySqlParameter("?id", id)
            };
            int cc = Convert.ToInt32(MySqlHelper.ExecuteNonQuery(MySQLConnectionString, cmdText, parameters2));

            return cc > 0;
        }

        public LinkTagInfo GetTag(int id)
        {
            LinkTagInfo obj = null;
            string cmdText = @"
                     select SoftID,Platform,Tag,LinkName,Url,CID,Updatetime,LinkType,channelId
                     from Cfg_LinkTags
                     where id=" + id;
            using (IDataReader dataReader = MySqlHelper.ExecuteReader(MySQLConnectionString, cmdText))
            {
                if (dataReader.Read())
                {
                    obj = new LinkTagInfo(dataReader);
                    obj.AppStoreUrlDecode();
                }
            }
            return obj;
        }

        public bool AddTag(LinkTagInfo obj)
        {
            string cmdText = @"
 insert into Cfg_LinkTags
  (SoftID,Platform,Tag,TagID,LinkName,Url,CID,Updatetime,LinkType,ChannelId)
 select ?softid,?platform,?tag,0,?name,?url,?cid,now(),?type,?ChannelId
 from dual
 where not exists(select 1 from Cfg_LinkTags where Tag=?tag);
 select last_insert_id();";
            MySqlParameter[] para = new MySqlParameter[]
            {
                new MySqlParameter("?softid", obj.SoftID),
                new MySqlParameter("?platform", obj.Platform),
                new MySqlParameter("?tag", obj.LinkTag),
                new MySqlParameter("?name", obj.LinkName),
                new MySqlParameter("?url", obj.LinkUrl),
                new MySqlParameter("?cid", obj.CID),
                new MySqlParameter("?type", obj.LinkType),
                 new MySqlParameter("?ChannelId", obj.ChannelId)
            };
            int id = Convert.ToInt32(MySqlHelper.ExecuteScalar(MySQLConnectionString, cmdText, para));

   
            return id > 0;
        }

        public bool UpdateTag(LinkTagInfo obj)
        {
            string cmdText = @"
                             update Cfg_LinkTags set
                              LinkName=?name,
                              Url=?url,
                              CID=?cid,
                              LinkType=?type,
                              Updatetime=now(),
                              ChannelId=?channelid
                             where id=?id";
            var para = new MySqlParameter[]
            {
                new MySqlParameter("?id", obj.ID),
                new MySqlParameter("?name", obj.LinkName),
                new MySqlParameter("?url", obj.LinkUrl),
                new MySqlParameter("?cid", obj.CID),
                new MySqlParameter("?type", obj.LinkType),
                new MySqlParameter("?channelid", obj.ChannelId)
            };
            int cc = Convert.ToInt32(MySqlHelper.ExecuteNonQuery(MySQLConnectionString, cmdText, para));

            return cc > 0;
        }

        public bool DeleteTag(int id)
        {
            string sql = @"delete from Cfg_LinkTags where id=" + id;
            int cc = Convert.ToInt32(MySqlHelper.ExecuteNonQuery(MySQLConnectionString, sql));
         
            return cc > 0;
        }
    }
}