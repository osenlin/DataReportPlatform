using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace net91com.Reports.Entities.ViewEntity.R_DownloadStatistics
{ 
    public class RescourceAdsOptions
    {
        #region 构造函数

        /// <summary>
        /// 来自数据库的数据构造
        /// </summary>
        public RescourceAdsOptions(IDataReader reader)
            : this()
        {
            LoadFromDb(reader);
        }
        
        /// <summary>
        /// 默认构造函数
        /// </summary>
        public RescourceAdsOptions()
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
                        int temp = Convert.ToInt32(reader["statdate"]);
                        StatDate = new DateTime(temp / 10000, temp % 10000 / 100, temp % 100);
                        break;
                    case "userid":
                        Userid = reader.IsDBNull(i) ? 0 :Convert.ToInt32(reader["userid"]);
                        break;
                    case "f_id":
                        F_id = reader.IsDBNull(i) ? 0 : reader.GetInt32(i);
                        break;
                    case "f_name":
                        F_Name = reader.IsDBNull(i) ? "" : reader["f_name"].ToString();
                        break; 
                    case "sortnumber":
                        Sortnumber = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["sortnumber"]);
                        break;
                    case "starttime":
                        Starttime = reader.GetDateTime(i);
                        break;
                    case "endtime":
                        Endtime = reader.GetDateTime(i);
                        break;
                     
                    case "edittime":
                        Edittime = reader.GetDateTime(i);
                        break;
                    case "memo":
                        Memo = reader.IsDBNull(i) ? "" : reader.GetString(i);
                        break;
                    case "f_type":
                        F_Type = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["f_type"]);
                        break;
                    case "tagid":
                        Tagid = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["tagid"]);
                        break;

                }
            }
            ProjectType = ((Res91com.SoftModel.Enums.SoftADTypeOption)F_Type);


        }

        #endregion

        #region 属性

        public DateTime StatDate { get; set; }

        public long Userid { get; set; }

        public string Account { get; set; }

        public int F_id { get; set; }

        public string F_Name { get; set; }

        public int F_Type { get; set; }

        public int Sortnumber { get; set; }

        public DateTime Starttime { get; set; }

        public DateTime Endtime { get; set; }

        public DateTime Edittime { get; set; }

        public int Tagid { get; set; }
        
        /// <summary>
        /// 标签类别
        /// </summary>
        public int TagType { get; set; }
        /// <summary>
        /// 说明
        /// </summary>
        public string Memo { get; set; }

        public Res91com.SoftModel.Enums.SoftADTypeOption ProjectType { get; set; }
        #endregion
    }
}
