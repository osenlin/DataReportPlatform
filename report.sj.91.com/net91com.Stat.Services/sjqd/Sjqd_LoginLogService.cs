using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using net91com.Core.Data;
using net91com.Core.Extensions;
using net91com.Core.Util;
using net91com.Stat.Services.sjqd.Entity;

namespace net91com.Stat.Services.sjqd
{
    public static class Sjqd_LoginLogService
    {
        private static string SJStaticDBConn = ConfigHelper.GetConnectionString("SJStaticDB_ConnString");

        public static List<Sjqd_LoginLog> GetList(string imei)
        {
            if (string.IsNullOrEmpty(imei))
            {
                return null;
            }
            int hashCode = imei.ToLower().GetHashCode32(false);
            int part = Math.Abs(hashCode%128);

            string cmdText = string.Format(@"
select softid,softversion,fromway,intimes 
from SoftLoginLog_{0:yyyyMMdd} with(nolock) 
where Part=@Part and HashCode=@HashCode and IMEI=@IMEI
union all 
select softid,softversion,fromway,intimes 
from PCSoftLoginLog_{0:yyyyMMdd} with(nolock) 
where Part=@Part and HashCode=@HashCode and IMEI=@IMEI", DateTime.Now);
            SqlParameter[] param = new SqlParameter[]
                {
                    SqlParamHelper.MakeInParam("@Part", SqlDbType.Int, 4, part),
                    SqlParamHelper.MakeInParam("@HashCode", SqlDbType.Int, 4, hashCode),
                    SqlParamHelper.MakeInParam("@IMEI", SqlDbType.VarChar, 100, imei)
                };

            List<Sjqd_LoginLog> list = new List<Sjqd_LoginLog>();
            using (IDataReader read = SqlHelper.ExecuteReader(SJStaticDBConn, CommandType.Text, cmdText, param))
            {
                while (read.Read())
                {
                    list.Add(new Sjqd_LoginLog(read));
                }
            }
            return list;
        }
    }
}