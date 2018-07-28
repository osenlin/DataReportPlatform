using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace net91com.Reports.Entities.ViewEntity.R_DownloadStatistics
{
    public  class Download_BaiduSpecial
    {
         /// <summary>
        /// 来自数据库的数据构造
        /// </summary>
        public Download_BaiduSpecial(IDataReader reader)
            : this()
        {
            LoadFromDb(reader);
        }

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public Download_BaiduSpecial()
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
                    case "statdate":
                        int temp = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["statdate"]);
                        StatDate = new DateTime(temp / 10000, temp % 10000 / 100, temp % 100);
                        break;
                    case "app_pc_downcount_exceptupdated":
                        App_PC_DownCount_ExceptUpdated = reader.IsDBNull(i) ? 0 : Convert.ToInt64(reader["app_pc_downcount_exceptupdated"]);
                        break;
                    case "app_wap_downcount_exceptupdated":
                        App_Wap_DownCount_ExceptUpdated = reader.IsDBNull(i) ? 0 : Convert.ToInt64(reader["app_wap_downcount_exceptupdated"]);
                        break;
                    case "app_api_downcount_exceptupdated":
                        App_Api_DownCount_ExceptUpdated = reader.IsDBNull(i) ? 0 : Convert.ToInt64(reader["app_api_downcount_exceptupdated"]);
                        break;
                    case "app_client_downcount_exceptupdated":
                        App_Client_DownCount_ExceptUpdated = reader.IsDBNull(i) ? 0 : Convert.ToInt64(reader["app_client_downcount_exceptupdated"]);
                        break;
                    case "app_pc_downcount_update":
                        App_PC_DownCount_Update = reader.IsDBNull(i) ? 0 : Convert.ToInt64(reader["app_pc_downcount_update"]);
                        break;
                    case "app_client_downcount_update":
                        App_Client_DownCount_Update = reader.IsDBNull(i) ? 0 : Convert.ToInt64(reader["app_client_downcount_update"]);
                        break;
                    case "game_pc_downcount_exceptupdated":
                        Game_PC_DownCount_ExceptUpdated = reader.IsDBNull(i) ? 0 : Convert.ToInt64(reader["game_pc_downcount_exceptupdated"]);
                        break;
                    case "game_wap_downcount_exceptupdated":
                        Game_Wap_DownCount_ExceptUpdated = reader.IsDBNull(i) ? 0 : Convert.ToInt64(reader["game_wap_downcount_exceptupdated"]);
                        break;
                    case "game_api_downcount_exceptupdated":
                        Game_Api_DownCount_ExceptUpdated = reader.IsDBNull(i) ? 0 : Convert.ToInt64(reader["game_api_downcount_exceptupdated"]);
                        break;
                    case "game_client_downcount_exceptupdated":
                        Game_Client_DownCount_ExceptUpdated = reader.IsDBNull(i) ? 0 : Convert.ToInt64(reader["game_client_downcount_exceptupdated"]);
                        break;

                    case "game_pc_downcount_update":
                        Game_PC_DownCount_Update = reader.IsDBNull(i) ? 0 : Convert.ToInt64(reader["game_pc_downcount_update"]);
                        break;
                    case "game_client_downcount_update":
                        Game_Client_DownCount_Update = reader.IsDBNull(i) ? 0 : Convert.ToInt64(reader["game_client_downcount_update"]);
                        break; 

                }
            }
            DataSet();
        }

        public long App_PC_DownCount_ExceptUpdated;

        public long App_Wap_DownCount_ExceptUpdated;

        public long App_Api_DownCount_ExceptUpdated;

        public long App_Client_DownCount_ExceptUpdated;

        public long App_PC_DownCount_Update;

        public long App_Client_DownCount_Update;

        public long App_Api_DownCount_Update; 

        public long Game_PC_DownCount_ExceptUpdated;

        public long Game_Wap_DownCount_ExceptUpdated;

        public long Game_Api_DownCount_ExceptUpdated;

        public long Game_Client_DownCount_ExceptUpdated;

        public long Game_PC_DownCount_Update;

        public long Game_Client_DownCount_Update;

        public long Game_Api_DownCount_Update;


       




        //各种汇总值
        public long TotalDownCount;

        public long TotalOneCount;

        public long TotalSecondCount;

        public long TotalGameCount;


        public long TotalClientCount;
        public long TotalApiAndWeb;
        public long TotalPCCount;

        public long UnionGameDownCount;
        public long GameAdsDownCount;

        public long GameDownCount;

        public long AppDownCount;

     
        /// <summary>
        /// 数据汇总下
        /// </summary>
        public void DataSet()
        {
            TotalGameCount = Game_PC_DownCount_ExceptUpdated + Game_Wap_DownCount_ExceptUpdated +
                             Game_Api_DownCount_ExceptUpdated + Game_Client_DownCount_ExceptUpdated +
                             Game_PC_DownCount_Update + Game_Client_DownCount_Update +
                             Game_Api_DownCount_Update;

            //TotalDownCount= App_PC_DownCount_ExceptUpdated+
            TotalOneCount = App_PC_DownCount_ExceptUpdated + App_Wap_DownCount_ExceptUpdated +
                            App_Api_DownCount_ExceptUpdated + App_Client_DownCount_ExceptUpdated + 
                            Game_PC_DownCount_ExceptUpdated + Game_Wap_DownCount_ExceptUpdated +
                             Game_Api_DownCount_ExceptUpdated + Game_Client_DownCount_ExceptUpdated;

            TotalSecondCount = App_PC_DownCount_Update + App_Client_DownCount_Update + App_Api_DownCount_Update + 
                                Game_PC_DownCount_Update + Game_Client_DownCount_Update +
                                 Game_Api_DownCount_Update; ;
            
            TotalGameCount = Game_PC_DownCount_ExceptUpdated + Game_Wap_DownCount_ExceptUpdated +
                             Game_Api_DownCount_ExceptUpdated + Game_Client_DownCount_ExceptUpdated +
                             Game_PC_DownCount_Update + Game_Client_DownCount_Update +
                             Game_Api_DownCount_Update;
            TotalDownCount = TotalOneCount + TotalSecondCount   ;

            //
            TotalClientCount = Game_Client_DownCount_ExceptUpdated + App_Client_DownCount_ExceptUpdated 
                                 + App_Client_DownCount_Update + Game_Client_DownCount_Update;
            
            TotalPCCount = App_PC_DownCount_ExceptUpdated + Game_PC_DownCount_ExceptUpdated
                                 + Game_PC_DownCount_Update + App_PC_DownCount_Update;

            TotalApiAndWeb = Game_Api_DownCount_ExceptUpdated + Game_Api_DownCount_Update
                             + App_Wap_DownCount_ExceptUpdated + Game_Wap_DownCount_ExceptUpdated
                             + App_Api_DownCount_ExceptUpdated + App_Api_DownCount_Update;

            AppDownCount = App_PC_DownCount_ExceptUpdated +
                            App_Wap_DownCount_ExceptUpdated + App_PC_DownCount_Update +
                            App_Api_DownCount_ExceptUpdated + App_Client_DownCount_ExceptUpdated
                            + App_Client_DownCount_Update + App_Api_DownCount_Update;
            GameDownCount = Game_PC_DownCount_ExceptUpdated +
                            Game_Wap_DownCount_ExceptUpdated + Game_PC_DownCount_Update +
                            Game_Api_DownCount_ExceptUpdated + Game_Client_DownCount_ExceptUpdated
                            + Game_Client_DownCount_Update + Game_Api_DownCount_Update;

        

        }

        public DateTime StatDate;
    }
}
