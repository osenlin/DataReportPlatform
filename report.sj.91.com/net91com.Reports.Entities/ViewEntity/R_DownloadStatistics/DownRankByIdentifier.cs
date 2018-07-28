using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace net91com.Reports.Entities.ViewEntity.R_DownloadStatistics
{
    public class DownRankByIdentifier
    {

        /// <summary>
        /// 来自数据库的数据构造
        /// </summary>
        public DownRankByIdentifier(IDataReader reader)
            : this()
        {
            LoadFromDb(reader);
        }

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public DownRankByIdentifier()
        {
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
                    case "f_identifier":
                        F_Identifier = reader.IsDBNull(i) ? "" : reader.GetString(i);
                        break;
                    case "name":
                        Name = reader.IsDBNull(i) ? "" : reader["name"].ToString();
                        break;
                    case "totaldowncount":
                        TotalDownCount = reader.IsDBNull(i) ? 0 : Convert.ToInt64(reader["totaldowncount"]);
                        break;
                    case "downcount_one":
                        DownCount_One = reader.IsDBNull(i) ? 0 : Convert.ToInt64(reader["downcount_one"]);
                        break;
                    case "downcount_two":
                        DownCount_Two = reader.IsDBNull(i) ? 0 : Convert.ToInt64(reader["downcount_two"]);
                        break;
                    case "downcount_search":
                        DownCount_Search = reader.IsDBNull(i) ? 0 : Convert.ToInt64(reader["downcount_search"]);
                        break;
                    case "ranknum":
                        RankNum = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["ranknum"]);
                        break;
                    case "resid":
                        ResId = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["resid"]);
                        break;
                    case "f_cateid":
                        int smallCateid = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["f_cateid"]);
                        CateName = GetCateNameString(smallCateid);
                        break;
                    case "totaldownsuccess":
                        TotalDownSuccess = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["totaldownsuccess"]);
                        break;
                    case "totaldownfail":
                        TotalDownFail = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["totaldownfail"]);
                        break;
                    case "setupsuccess":
                        SetupSuccess = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["setupsuccess"]);
                        break;
                    case "setupfail":
                        SetupFail = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["setupfail"]);
                        break;

                }
            }
        }

        public string F_Identifier;

        public long TotalDownCount;

        public long DownCount_One;

        public long DownCount_Two;

        public string Name;

        public int RankNum;

        public int LastRankNum;

        public long DownCount_Search;

        public int ResId;

        public string CateName;

        public int TotalDownSuccess;

        public int TotalDownFail;

        public int SetupSuccess;

        public int SetupFail;

        public string GetCateNameString(int cateId)
        {
            switch (cateId)
            {
               case 33:
                    return "角色扮演";
               case 34:
                    return "休闲娱乐";
               case 35:
                    return "射击游戏";
               case 36:
                    return "益智游戏";
               case 37:
                    return "棋牌天地";
               case 38:
                    return "情景游戏";
               case 39:
                    return "冒险游戏";
               case 40:
                    return "策略游戏";
               case 41:
                    return "模拟经营";
               case 42:
                    return "动作游戏";
               case 43:
                    return "体育竞技";
               case 44:
                    return "竞速游戏";
               case 45:
                    return "格斗游戏";
               case 53:
                    return "网络游戏";
                default:
                    return cateId.ToString();

            }
        }
    }

   

}
