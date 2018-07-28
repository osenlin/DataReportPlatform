using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using net91com.Core.Util;
using net91com.Stat.Services.sjqd.Entity;
using net91com.Stat.Services.Entity;
using System.Data;
using System.Data.SqlClient;
using net91com.Core;
using net91com.Core.Data;
using net91com.Core.Web;
using net91com.Stat.Services.Monitor.Entity;

namespace net91com.Stat.Services.Monitor
{
    public class MsmqMonitorService
    {
        private static string _connectionString = ConfigHelper.GetConnectionString("StatDB_MySQL_ConnString");

        /// <summary>
        /// 构造函数
        /// </summary>
        public MsmqMonitorService()
        {
            SqlHelper.CommandTimeout = 120;
        }

        private static Dictionary<string, string> msmqNameDict = null;

        /// <summary>
        /// 获取消息队列名称列表
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, string> GetMsmqNameDict()
        {
            if (msmqNameDict == null)
            {
                msmqNameDict = new Dictionary<string, string>();
                msmqNameDict.Add(@".\private$\sjchanneluserdata", "渠道");
                msmqNameDict.Add(@".\private$\channel_stat.sj.91.com", "CPA");
                msmqNameDict.Add(@".\private$\stat.sj.91.com", "助手");
                msmqNameDict.Add(@".\private$\appuse.sj.91.com_mobile_softloginlog", "外部渠道");
                msmqNameDict.Add(@".\private$\logstatic.sj.91.com_mobile_uselonglog", "时长");
                msmqNameDict.Add(@".\private$\dataservice.sj.91.com_keyword", "搜索");
                msmqNameDict.Add(@".\private$\dataservice.sj.91.com_resource", "下载");
                msmqNameDict.Add(@".\private$\funcstatic.sj.91.com_mobile_function", "功能");
                msmqNameDict.Add(@".\private$\funcstatic.sj.91.com_pczs_function", "助手功能");
            }
            return msmqNameDict;
        }

        /// <summary>
        /// 获取日志名称列表
        /// </summary>
        /// <returns></returns>
        public static string[] GetServerIPList()
        {
            return ConfigHelper.GetSetting("MONITOR_SERVER_IP_LIST")
                               .Split(new string[] {","}, StringSplitOptions.RemoveEmptyEntries);
        }

        /// <summary>
        /// 获取指定日期指定消息队列所有服务器的消息数量列表
        /// </summary>
        /// <param name="msmqName"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public Dictionary<string, List<int>> GetMsmqMsgCountList(string msmqName, DateTime date,
                                                                 out Dictionary<string, List<DateTime>> xData)
        {
            string cmdText =
                "select ServerIP,AddTime,MsgCount from Monitor_Msmq with(nolock) where MsmqName=@MsmqName and AddTime between @BeginTime and @EndTime order by AddTime";
            DateTime beginTime = date.Date == DateTime.Now.Date ? DateTime.Now.AddDays(-1) : date.Date;
            DateTime endTime = beginTime.AddDays(1);
            SqlParameter[] parameters = new SqlParameter[]
                {
                    SqlParamHelper.MakeInParam("@MsmqName", SqlDbType.VarChar, 100, msmqName),
                    SqlParamHelper.MakeInParam("@BeginTime", SqlDbType.DateTime, 8, beginTime),
                    SqlParamHelper.MakeInParam("@EndTime", SqlDbType.DateTime, 8, endTime)
                };
            Dictionary<string, List<int>> result = new Dictionary<string, List<int>>();
            xData = new Dictionary<string, List<DateTime>>();
            foreach (string svrip in GetServerIPList())
            {
                result.Add(svrip, new List<int>());
                xData.Add(svrip, new List<DateTime>());
            }
            using (
                SqlDataReader reader = SqlHelper.ExecuteReader(_connectionString, CommandType.Text, cmdText, parameters)
                )
            {
                while (reader.Read())
                {
                    string serverIp = reader["ServerIP"].ToString();
                    DateTime addTime = Convert.ToDateTime(reader["AddTime"]);
                    int msgCount = Convert.ToInt32(reader["MsgCount"]);
                    if (result.ContainsKey(serverIp))
                    {
                        xData[serverIp].Add(addTime);
                        result[serverIp].Add(msgCount);
                    }
                }
            }
            return result;
        }
    }
}