using System;
using System.Collections.Generic;
using System.Data;
using net91com.Core;
using net91com.Core.Data;
using net91com.Core.Util;
using net91com.Core.Web;
using net91com.Reports.DataAccess.OtherDataAccess;
using net91com.Reports.DataAccess.ToolDataStat;


namespace net91com.Reports.Services.CommonServices.Tool
{
    public class ToolService : BaseService
    {
        private static ToolService instance;
        private static readonly object obj = new object();
        private List<Dictionary<string, string>> lststring;

        public static ToolService Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (obj)
                    {
                        if (instance == null)
                        {
                            instance = new ToolService();
                            instance._cachePreviousKey = "ToolService";
                        }
                    }
                }
                return instance;
            }
        }

        /// <summary>
        ///     获取对应状态值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public DateTime GetEltStates(string key, string conString = null)
        {
            string cachekey = BuildCacheKey("ToolServic", "GetEltStates", key, conString);
            if (CacheHelper.Contains(cachekey))
            {
                return CacheHelper.Get<DateTime>(cachekey);
            }
            else
            {
                DateTime result = Tool_DataAccess.Instance.GetEtlStates(key, conString);
                CacheHelper.Set(cachekey, result, CacheTimeOption.TenMinutes,
                                CacheExpirationOption.AbsoluteExpiration);
                return result;
            }
        }

        /// <summary>
        ///     获取国家名称列表
        /// </summary>
        /// <returns></returns>
        public List<string> GetCountryNameList()
        {
            return CacheHelper.Get<List<string>>
                (BuildCacheKey("GetCountryNameList"), CacheTimeOption.TenMinutes,
                 () => ToolDataAccess.Instance.GetCountryNameList());
        }

        public List<Dictionary<string, string>> GetClusterStatLog(string startdate, string enddate, int modulutype)
        {
            return Tool_DataAccess.Instance.GetClusterStatLog(startdate, enddate, modulutype);
        }

        public string GetVersionInfo()
        {
            string statdbConn = ConfigHelper.GetConnectionString("StatDB_MySQL_ConnString");
            string sql = "select id,version,softid from B_Versions where softid = 46 and platform = 4;";
            DataSet ds = SqlHelper.ExecuteDataset(statdbConn, CommandType.Text, sql);
            if (ds != null && ds.Tables.Count > 0)
            {
                return JsonHelper.GetJSONString(ds.Tables[0]);
            }
            return string.Empty;
        }

    }
}