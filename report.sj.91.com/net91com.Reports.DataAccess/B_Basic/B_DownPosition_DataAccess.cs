using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using net91com.Core.Data;
using net91com.Core.Util;
using net91com.Reports.Entities.B_Other;
using net91com.Reports.UserRights;

namespace net91com.Reports.DataAccess.B_Basic
{
    public class B_DownPosition_DataAccess : BaseDataAccess
    {
        protected static string statdbConn = ConfigHelper.GetConnectionString("StatDBReport_ConnString");

        
        protected static string mysql_statdb_connstring = ConfigHelper.GetConnectionString("StatDB_MySQL_ConnString");
            
        public List<B_DownPositionEntity> GetB_DownPositionList(ProjectSourceTypeOptions projectSourceType, int downtype, int istag,
                                                                            int restype, int project, string position,
                                                                            string name, string pagename, string pagetype, int beginsize,
                                                                            int pagesize,int softid2, out int count)
        {
            count = 0;
            string datasql = string.Format(@" select SoftId,ProjectSource,ProjectSourceType,ResType,Position,Name,PageName,PageType,ByTag bytag4mysql,DownType
                                                from Cfg_DownPositions 
                                              where projectsource=?projectsource and ProjectSourceType=?projectsourcetype 
                                                 and softid=?softid
                                                and restype=?restype  and bytag=?bytag {0} {1} {2} {3} {4} 
                                              ",
                                              position == "" ? "" : " and  position like '%" + position + "%'",
                                              name == "" ? "" : " and  name like '%" + name + "%'",
                                              pagename == "" ? "" : " and  pagename like '%" + pagename + "%'",
                                              downtype == -1 ? "" : " and downtype=?downtype ",
                                              pagetype == "不区分页面类型" ? "" : " and pagetype=?pagetype"
                                              );
            string resultsql1 = string.Format(@"select SoftId,ProjectSource,ProjectSourceType,ResType,Position,Name,PageName,PageType,bytag4mysql,DownType
                                               from({0}) as result
                                              order by position
                                              limit {1},{2};", datasql, beginsize , pagesize);
            string resultsql2 = string.Format(@"select count(1)
                                                from({0}) as result", datasql);
            string lastsql = resultsql1 + " " + resultsql2;
            var param = new [] {
                new MySqlParameter("?softid",softid2),
                new MySqlParameter("?projectsource",project),
                new MySqlParameter("?projectsourcetype",(int)projectSourceType),
                new MySqlParameter("?restype",restype),
                new MySqlParameter("?downtype",downtype),
                new MySqlParameter("?bytag",istag),
                new MySqlParameter("?pagetype",pagetype)
            };
            var list = new List<B_DownPositionEntity>();
            using (IDataReader read = MySqlHelper.ExecuteReader(mysql_statdb_connstring, lastsql, param))
            {
                while (read.Read())
                {
                    list.Add(new B_DownPositionEntity(read));
                }
                if (read.NextResult())
                {
                    if (read.Read())
                    {
                        count = Convert.ToInt32(read[0]);
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// 获取具体位置
        /// </summary>
        /// <param name="projectsourcetype"></param>
        /// <param name="restype"></param>
        /// <param name="project"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public B_DownPositionEntity GetB_DownPosition(ProjectSourceTypeOptions projectsourcetype, int restype, int project, string position)
        {

            string datasql = string.Format(@" select 
                                                    ProjectSource,
                                                    ProjectSourceType,
                                                    ResType,
                                                    Position,
                                                    `Name`,
                                                    PageName,
                                                    PageType,
                                                    ByTag bytag4mysql,
                                                    DownType,
                                                    AddTime,
                                                    SoftID,
                                                    TagID
                                              from Cfg_DownPositions
                                              where  projectsource=?projectsource 
                                                     and projectsourcetype=?projectsourcetype 
                                                     and restype=?restype 
                                                     and position=?position ");

            MySqlParameter[] param = new MySqlParameter[] {
                new MySqlParameter("?projectsource",project),
                new MySqlParameter("?restype",restype),
                new MySqlParameter("?projectsourcetype",(int)projectsourcetype),
                new MySqlParameter("?position",position)
               
            };
            B_DownPositionEntity us = null;
            using (IDataReader read = MySqlHelper.ExecuteReader(mysql_statdb_connstring, datasql, param))
            {
                if (read.Read())
                {
                    us = new B_DownPositionEntity(read);
                }
            }
            return us;
        }

        public int UpdatePosition2MySql(B_DownPositionEntity position)
        {
            string sql = string.Format(@"
                            update Cfg_DownPositions 
                            set Name=?name, PageName=?pagename, ByTag=?bytag,downtype=?downtype,PageType=?pagetype
                            where Position=?position 
                                 and ResType=?restype 
                                 and projectsource=?projectsource 
                                 and projectsourcetype=?projectsourcetype");
            var param = new [] {
                new MySqlParameter("?name",position.Name),
                new MySqlParameter("?pagename",position.PageName),
                new MySqlParameter("?pagetype",position.PageType),
                new MySqlParameter("?bytag",position.ByTag4MySql),
                new MySqlParameter("?downtype",position.DownType),
                new MySqlParameter("?position",position.Position),
                new MySqlParameter("?restype",position.ResType),
                new MySqlParameter("?projectsource",position.ProjectSource),
                new MySqlParameter("?projectsourcetype",position.ProjectSourceType)
            };
            return MySqlHelper.ExecuteNonQuery(mysql_statdb_connstring, sql, param);
        }

        public int BatchEditPositionName(B_DownPositionEntity position, string strlist)
        {

            using (var conn = new SqlConnection(statdbConn))
            {
                conn.Open();
                var list = strlist.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                string sql = @"create table #temp(name varchar(100),position int);";
                SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql);

                var dt = new DataTable();

                var dc = new DataColumn("name", typeof(string)) { MaxLength = 100 };
                dt.Columns.Add(dc);

                dc = new DataColumn("position", typeof(int));
                dt.Columns.Add(dc);

                foreach (var item in list)
                {
                    var data = item.Split(",，\t".ToCharArray());
                    if (data.Length < 2)
                        continue;

                    var row = dt.NewRow();
                    row["position"] = data[0];
                    row["name"] = data[1];

                    dt.Rows.Add(row);
                }

                if (dt.Rows.Count > 0)
                {
                    using (var sbc = new SqlBulkCopy(conn))
                    {
                        sbc.BulkCopyTimeout = 600;
                        sbc.BatchSize = dt.Rows.Count;
                        sbc.DestinationTableName = "#temp";
                        sbc.WriteToServer(dt);
                    }
                    sql = @"
update a
set a.Name = b.name
from B_DownPositions a left join #temp b
on a.Position = b.position
where a.ResType=@restype and a.projectsource=@projectsource and a.projectsourcetype=@projectsourcetype and b.position is not null ";
                    var param = new[] {
                        SqlParamHelper.MakeInParam("@restype", SqlDbType.SmallInt,2,position.ResType),
                        SqlParamHelper.MakeInParam("@projectsource", SqlDbType.SmallInt,2,position.ProjectSource),
                        SqlParamHelper.MakeInParam("@projectsourcetype", SqlDbType.TinyInt,1,position.ProjectSourceType)
                    };
                    return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, param);
                }
                return 0;

            }
        }

        public int AddPosition2MySql(B_DownPositionEntity position)
        {
            string sql = string.Format(@"
                           insert into Cfg_DownPositions(ProjectSource,ProjectSourceType, ResType, Position, Name, PageName, PageType, ByTag,DownType,softid,addTime)
                           select ?projectsource,?projectsourcetype,?restype,?position,?name,?pagename,?pagetype,?bytag,?downtype,?softid,sysdate()
                           from dual
                           where not exists(
                                select 1 from Cfg_DownPositions 
                                where Position=?position 
                                      and projectsource=?projectsource 
                                      and ResType=?restype 
                                      and projectsourcetype=?projectsourcetype 
                                      and softid=?softid) 
                    ");
            
            var param = new [] {
                new MySqlParameter("?name", position.Name),
                new MySqlParameter("?pagename",position.PageName),
                new MySqlParameter("?pagetype",position.PageType),
                new MySqlParameter("?bytag", position.ByTag4MySql),
                new MySqlParameter("?downtype",position.DownType),
                new MySqlParameter("?position",position.Position),
                new MySqlParameter("?softid", position.SoftId),
                new MySqlParameter("?restype",position.ResType),
                new MySqlParameter("?projectsource",position.ProjectSource),
                new MySqlParameter("?projectsourcetype",position.ProjectSourceType)

            };
            return MySqlHelper.ExecuteNonQuery(mysql_statdb_connstring,sql, param);
        }

        /// <summary>
        /// 根据项目来源，项目来源类型，资源类型
        /// </summary>
        /// <param name="projectSourceType"></param>
        /// <param name="restype"></param>
        /// <param name="project"></param>
        /// <returns></returns>
        public List<B_DownPositionEntity> GetB_DownPositionListByCache(ProjectSourceTypeOptions projectSourceType, int restype, int project,int softid=-1)
        {
            string datasql = string.Format(@" select 
                                                ProjectSource,
                                                    ProjectSourceType,
                                                    ResType,
                                                    Position,
                                                    `Name`,
                                                    PageName,
                                                    PageType,
                                                    ByTag bytag4mysql,
                                                    DownType,
                                                    AddTime,
                                                    SoftID,
                                                    TagID

                                              from Cfg_DownPositions
                                              where  projectsourcetype=?projectsourcetype 
                                                    {0} {1} {2}", restype==-1?"":" and restype=?restype"
                                                            , softid == -1 ? "" : " and softid=?softid"
                                                            , project == -1 ? "" : " and projectsource=?projectsource");

            MySqlParameter[] param = new MySqlParameter[] {

                new MySqlParameter("?softid", softid),
                new MySqlParameter("?projectsource", project),
                new MySqlParameter("?restype", restype),
                new MySqlParameter("?projectsourcetype",(int)projectSourceType),
            };

            var us = new List<B_DownPositionEntity>();
            using (IDataReader read = MySqlHelper.ExecuteReader(mysql_statdb_connstring, datasql, param))
            {
                while (read.Read())
                {
                    us.Add(new B_DownPositionEntity(read));
                }
            }
            return us;
        }

        public List<B_DownPositionEntity> GetAdPosition(int softid, int platform,int restype,int areatype,int pagetype)
        {
            string datasql = string.Format(@" select distinct position,softid,restype 
                                              from Schedule_AdTagIdMapPosition 
                                              where  {0} {1} {2}", restype == -1 ? "" : "restype=?restype"
                                                , softid == -1 ? "" : " and softid=?softid"
                                                , pagetype==-1?"":" and pagetype=?pagetype"
                                               );

            var param = new [] {
                new MySqlParameter("?pagetype",pagetype),
                new MySqlParameter("?softid",softid),
                new MySqlParameter("?restype",restype),
            };
            var us = new List<B_DownPositionEntity>();
            using (IDataReader read = MySqlHelper.ExecuteReader(mysql_statdb_connstring,datasql, param))
            {
                while (read.Read())
                {
                    us.Add(new B_DownPositionEntity(read));
                }
            }
            return us;
        }
    }
}
