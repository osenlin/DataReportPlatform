using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace net91com.Reports.Entities.ViewEntity.R_DownloadStatistics
{
     public  class ResourceDownLoad
    {
            #region 构造函数

            /// <summary>
            /// 来自数据库的数据构造
            /// </summary>
            public ResourceDownLoad(IDataReader reader)
                : this()
            {
                LoadFromDb(reader);
            }

            /// <summary>
            /// 默认构造函数
            /// </summary>
            public ResourceDownLoad()
            {
            }

            /// <summary>
            /// 来自数据库的数据构造
            /// </summary>
            public   void LoadFromDb(IDataReader reader)
            {
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    switch (reader.GetName(i).ToLower())
                    {
                        case "statdate":
                            int m = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["statdate"]);
                            StatTime = new DateTime(m/10000,m%10000/100,m%100);
                            break;
                        case "platform":
                            Platform = reader.IsDBNull(i) ? -1 : Convert.ToInt32(reader["platform"]);
                            break;
                        case "projectsource":
                            ProjectSource = reader.IsDBNull(i) ? -1 : Convert.ToInt32(reader["projectsource"]);
                            break;
                        case "restype":
                            ResType = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["restype"]);
                            break;
                        case "resid":
                            ResID = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["resid"]);
                            break;
                        case "downcount":
                            Downcount = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["downcount"]);
                            break;
                        case "downsuccesscount":
                            Downsuccesscount = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["downsuccesscount"]);
                            break;
                        case "downfailedcount":
                            Downfailedcount = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["downfailedcount"]);
                            break;
                        case "setupsuccesscount":
                            Setupsuccesscount = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["setupsuccesscount"]);
                            break;
                        case "setupfailedcount":
                            Setupfailedcount = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["setupfailedcount"]);
                            break;
                        case "position":
                            Position = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["position"]);
                            break;
                        case "downupdatecount":
                            DownUpdateCount = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["downupdatecount"]);
                            break;
                        case "browsecount":
                            BrowseCount = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["browsecount"]);
                            break;
                            //来自更新
                        case "downfromsearch":
                            DownFromSearch = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["downfromsearch"]);
                            break;

                        
                    }
                }
                int totaldown = Downsuccesscount + Downfailedcount;
                int totalsetup = Setupfailedcount +Setupsuccesscount;
                Downseccesspercent = totaldown == 0 ? -1 : (decimal)(Downsuccesscount * 1.0 / totaldown);
                Downfailedpercent = totaldown == 0 ? -1 : (decimal)(Downfailedcount * 1.0 / totaldown);
                SetUpSeccessPercent = totalsetup == 0 ? -1 : (decimal)(Setupsuccesscount * 1.0 / totalsetup);
                SetUpFailedPercent = totalsetup == 0 ? -1 : (decimal)(Setupfailedcount * 1.0 / totalsetup);
            }

            public int GetStatDownByStatType(int i)
            {
                switch ( i)
                {
                    case 1:
                        return Downcount;
                    case 2:
                        return BrowseCount;
                    case 4:
                        return Downsuccesscount;
                    case 8:
                        return Downfailedcount;
                    case 5:
                        return Setupsuccesscount;
                    case 6:
                        return Setupfailedcount;
                    default:
                        return Downcount;
                         
                }
            }
            /// <summary>
            /// 获取每一列数据
            /// </summary>
            /// <param name="i"></param>
            /// <returns></returns>
            public object GetIndexValue(int i)
            {
                switch (i)
                {
                    case 0:
                        return this.StatTime;
                    case 1:
                        return this.Downcount;
                    case 2:
                        return this.Downsuccesscount;
                    case 3:
                        return this.Downfailedcount;
                    case 4:
                        return this.Setupsuccesscount;
                    case 5:
                        return this.Setupfailedcount;
                    case 6:
                        return this.BrowseCount;
                    case 7:
                        return this.Downseccesspercent;
                    case 8:
                        return this.Downfailedpercent;
                    case 9:
                        return this.SetUpSeccessPercent;
                    case 10:
                        return this.SetUpFailedPercent;
                    
                    default:
                        return this.StatTime;
                } 
            }

            #endregion

            #region 属性


            public DateTime StatTime { get; set; }
            
             /// <summary>
             /// 位置编号
             /// </summary>
             //来自搜索
            public int DownFromSearch { get; set; }

            public int Position { get; set; }

            public int ProjectSource { get; set; }
            
            public int ResType { get; set; }

            public int ResID { get; set; } 

            public int Downsuccesscount { get; set; }

            public int BrowseCount { get; set; }

            public int Downcount { get; set; }

            public int Downfailedcount { get; set; }

            public int Setupsuccesscount { get; set; }

            public int Setupfailedcount { get; set; }

            public int DownUpdateCount { get; set; }

            public decimal Downseccesspercent { get; set; }

            public decimal Downfailedpercent { get; set; }

            public decimal SetUpSeccessPercent { get; set; }

            public decimal SetUpFailedPercent { get; set; }

            public int AddDate { get; set; }

            public int Platform { get; set; }

            public string PositionName { get; set; }

            public string ResName { get; set; }


         #endregion
    }
}
