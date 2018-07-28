using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using net91com.Core.Data;
using net91com.Core.Util;
using net91com.Reports.DataAccess.DataAccesssUtil;
using net91com.Reports.Entities.B_Other;
using net91com.Reports.Entities.D_DownLoadStatisticsEntities;
using net91com.Reports.Entities.DataBaseEntity;

namespace net91com.Reports.DataAccess.B_Basic
{
    public class B_BaseTool_DataAccess:BaseDataAccess
    {
        protected static string ComputingDB_En_ConnString = ConfigHelper.GetConnectionString("ComputingDB_En_ConnString");
        protected static string mysql_statdb_ConnString = ConfigHelper.GetConnectionString("StatDB_MySQL_ConnString");

        protected static string ComputingDB_CN_ConnString = ConfigHelper.GetConnectionString("ComputingDB_CN_ConnString");
        public List<B_AreaEntity> GetArea(int flag = 0)
        {
            return GetArea(0,flag);
        }

        /// <summary>
        /// 获取所有国家信息
        /// </summary>
        /// <returns></returns>
        public List<B_AreaEntity> GetCountries(int flag = 0)
        {
            return GetArea(1,flag);
        }

        /// <summary>
        /// 获取所有省信息
        /// </summary>
        /// <returns></returns>
        public List<B_AreaEntity> GetProvinces(int flag = 0)
        {
            return GetArea(2,flag);
        }

        /// <summary>
        /// 获取所有主要城市
        /// </summary>
        /// <returns></returns>
        public List<B_AreaEntity> GetCities(int flag = 0)
        {
            return GetArea(3,flag);
        }

        /// <summary>
        /// 根据地区类型获取对应的地区信息
        /// </summary>
        /// <param name="areaType"></param>
        /// <returns></returns>
        private List<B_AreaEntity> GetArea(int areaType,int flag=0)
        {
            string where;
           
            switch (areaType)
            {
                case 1:
                    where = " where ParentID=0";
                    break;
                case 2:
                    where = " where ParentID=253";
                    break;
                case 3:
                    where = " where ParentID>253";
                    break;
                default:
                    where = string.Empty;
                    break;
            }

            if (flag != 0 && areaType>0 && areaType<=3)
            {
                where += " and flag&2>0";
            }
            else if (flag != 0 && (areaType <= 0 || areaType>3))
            {
                where = " where flag&2>0";
            }

            string sql = "select ID,Name,ParentID,EnShortName from Cfg_Areas " + where + " order by OrderIndex desc";
            List<B_AreaEntity> arealist = null;
            using (var reader = MySqlHelper.ExecuteReader(Mysql_Statdb_Connstring, sql))
            {
                arealist = new List<B_AreaEntity>();
                while (reader.Read())
                {
                    arealist.Add(new B_AreaEntity(reader));
                }
            }
            return arealist;
        }

        public B_AreaEntity GetAreabyid(int areaid)
        {

            if (areaid==-1)
            {
                return new B_AreaEntity(){EnShortName = "-1"};
            }
            string sql = "select ID,Name,ParentID,EnShortName from Cfg_Areas where id="+areaid;
           B_AreaEntity entity = null;
            using (var reader = MySqlHelper.ExecuteReader(Mysql_Statdb_Connstring, sql))
            {
                
                if (reader.Read())
                {
                    entity=new B_AreaEntity(reader);
                }
            }
            return entity;
        }

        public List<B_AreaEntity> GetAreabyid(List<int> areaidlst)
        {

            List<B_AreaEntity> entity = new List<B_AreaEntity>();
            if (areaidlst.Count == 0)
            {
                return entity;
            }
            string sql = string.Format("select ID,Name,ParentID,EnShortName from Cfg_Areas where id in ({0})" ,string.Join(",",areaidlst.ConvertAll(p=>p.ToString()).ToArray()));
           
            using (var reader = MySqlHelper.ExecuteReader(Mysql_Statdb_Connstring, sql))
            {
                while (reader.Read())
                {
                    entity.Add(new B_AreaEntity(reader));
                }
            }
            return entity;
        }

        /// <summary>
        /// 1 国内库 2 海外库
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public List<B_ResInfo> GetResInfo(string residentifer, int restype, int areatype)
        {
            string sql = @"select ResId,ResName  from Cfg_ResInfo
                          where residentifier = ?identify and restype=?restype and areatype=?areatype";
            var param = new []
                {
                    new MySqlParameter("?identify",residentifer),
                    new MySqlParameter("?restype",  restype),
                    new MySqlParameter("?areatype", areatype)
                };
            List<B_ResInfo> reslist = null;
            using (var reader = MySqlHelper.ExecuteReader(mysql_statdb_ConnString,sql, param))
            {
                reslist = new List<B_ResInfo>();
                while (reader.Read())
                {
                    reslist.Add(new B_ResInfo(reader));
                }
            }
            return reslist;
        }

        //为下载排行包名统计准备的特殊方式
        public List<B_ResInfo> GetResInfo(int restype, int areatype,List<string> residentifierlst,bool isresid=false)
        {
            string sql = string.Format(@"
                                            SELECT
                                                AreaType ,
                                                ResType,
                                                max(ResName) ResName,
                                                {4}
                                            FROM
                                                Cfg_ResInfo
                                            where  areatype={0} and restype={1} and {3} in({2})
                                            group by  
                                                AreaType ,
                                                ResType,
                                                {3}
                                        ", areatype, restype, string.Format("'{0}'",string.Join("','",residentifierlst.ToArray()))
                                         , isresid == true ? "ResId " : "ResIdentifier"
                                         , isresid == true ? "ResId ResIdentifier" : "ResIdentifier");

            List<B_ResInfo> reslist = null;
            using (var reader = MySqlHelper.ExecuteReader(mysql_statdb_ConnString,sql))
            {
                reslist = new List<B_ResInfo>();
                while (reader.Read())
                {
                    reslist.Add(new B_ResInfo(reader));
                }
            }
            return reslist;
        }

        //为下载排行包名统计准备的特殊方式
        public List<B_ResInfo> GetResInfo2(int restype, int areatype, List<string> residentifierlst, bool isresid = false)
        {
            string sql = string.Format(@"
                                            SELECT
                                                AreaType ,
                                                ResType,
                                                max(ResName) ResName,
                                                {4}
                                            FROM
                                                Cfg_ResInfo
                                            where  areatype={0} and restype={1} and {3} in({2})
                                            group by  
                                                AreaType ,
                                                ResType,
                                                {3}
                                        ", areatype, restype, string.Format("'{0}'", string.Join("','", residentifierlst.ToArray()))
                                         , isresid == true ? "ResId" : "ResIdentifier"
                                         , isresid == true ? "ResId,max(ResIdentifier) ResIdentifier" : "ResIdentifier");

            List<B_ResInfo> reslist = null;
            using (var reader = MySqlHelper.ExecuteReader(mysql_statdb_ConnString, sql))
            {
                reslist = new List<B_ResInfo>();
                while (reader.Read())
                {
                    reslist.Add(new B_ResInfo(reader));
                }
            }
            return reslist;
        }



        /// <summary>
        /// 1 国内库 2 海外库 3台湾
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public List<B_ResCateEntity> GetResCate(int type)
        {
            string sql = string.Format(@"select ResType,ID CID,PID PCID,CName from Cfg_ResCate where AreaType={0}",type);

            List<B_ResCateEntity> reslist = null;
            using (var reader = MySqlHelper.ExecuteReader(mysql_statdb_ConnString, sql))
            {
                reslist = new List<B_ResCateEntity>();
                while (reader.Read())
                {
                    reslist.Add(new B_ResCateEntity(reader));
                }
            }
            return reslist;
        }

        public List<B_VersionEntity> GetVersions()
        {
            string sql = "select ID,SoftID,Platform,Version,Alias from Cfg_Versions order by Version desc";

            List<B_VersionEntity> versionlist = null;
            using (MySqlDataReader reader = MySqlHelper.ExecuteReader(mysql_statdb_ConnString, sql))
            {
                versionlist = new List<B_VersionEntity>();
                while (reader.Read())
                {
                    versionlist.Add(new B_VersionEntity(reader));
                }
            }
            return versionlist;
        }

        public B_VersionEntity GetVersionById(int versionid)
        {
            string sql = "select ID,SoftID,Platform,Version,Alias from Cfg_Versions where ID="+versionid;

            B_VersionEntity entity = null;
            using (MySqlDataReader reader = MySqlHelper.ExecuteReader(mysql_statdb_ConnString, sql))
            {
                
                if (reader.Read())
                {
                    entity= new B_VersionEntity(reader);
                }
            }
            return entity;
        }


        public List<B_VersionEntity> GetVersions(List<int> lstver )
        {

            if (lstver.Count==0)
            {
                return new List<B_VersionEntity>();
            }
            string sql = string.Format(@"select ID,SoftID,Platform,Version,Alias from Cfg_Versions 
                            where  id in  ({0}) 
                          ",string.Join(",",lstver.Select(p=>p.ToString()).ToArray()));

            List<B_VersionEntity> versionlist = null;
            using (MySqlDataReader reader = MySqlHelper.ExecuteReader(mysql_statdb_ConnString, sql))
            {
                versionlist = new List<B_VersionEntity>();
                while (reader.Read())
                {
                    versionlist.Add(new B_VersionEntity(reader));
                }
            }
            return versionlist;
        }

        public List<B_AuthorEntity> GetAuthorEntity()
        {
            string sql = @"select f_authorid AuthorID,f_author AuthorName  from authors";
            var lstauthor = new List<B_AuthorEntity>();
            using (SqlDataReader reader = SqlHelper.ExecuteReader(NewResourceDB_ConnString, CommandType.Text, sql))
            {
                while (reader.Read())
                {
                    lstauthor.Add(new B_AuthorEntity(reader));
                }

            }
            return lstauthor;
        }

        public List<Dictionary<string,string>> getExtendResAttr(int restype)
        {
            string sql = string.Format(@"SELECT  [Type],[Name]
                                         FROM [StatDB].[dbo].[B_ResAttribute]
                                         where restype={0}
                                        order by Type desc ", restype);

            using (IDataReader reader=SqlHelper.ExecuteReader(StatConn, CommandType.Text, sql))
            {
               return RelationDBDataSetUtil.ParseDataSet(reader);
            }
        }

        public int getSoftid2byProjectsource(int projectsource, int projectsourcetype)
        {
            string sql = string.Format(@"SELECT  Softid2
                                         FROM R_ProjectSourcesBySoft
                                         where projectsource={0} and ProjectSourceType={1}
                                        ", projectsource,projectsourcetype);

            var reader = SqlHelper.ExecuteScalar(StatConn, CommandType.Text, sql);
            if (reader != null)
            {
                return Convert.ToInt32(reader);
            }
            else
            {
                if (projectsourcetype==1)
                {
                    return -10000000;
                }
                else
                {
                    return -20000000;
                }
            }
        }

        public int GetProjectSourceTypeBySoftId2(int softid2)
        {
            //对台湾产品做了特殊处理，日后配置表更改了，这个判断可以删除
            if (softid2 == 113938 || softid2 == 97410 || softid2 == 113939)
            {
                return 3;
            }

            string sql = string.Format(@"SELECT  ProjectSourceType
                                         FROM R_ProjectSourcesBySoft
                                         where SoftID2={0} ", softid2);


            var reader = MySqlHelper.ExecuteScalar(mysql_statdb_ConnString, sql);
            if (reader != null)
            {
                return Convert.ToInt32(reader);
            }
            else
            {
                if (softid2 == -10000000 || softid2==-2)
                {
                    return 1;
                }
                else
                {
                    return 2;
                }
            }
        }
    }
}
