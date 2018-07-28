using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Res91com.ResourceDataAccess;
using net91com.Core.Extensions;

namespace net91com.Reports.Entities.ViewEntity.R_DownloadStatistics
{
    public class ResourceRecommendTotalByPosition
    {
           /// <summary>
        /// 来自数据库的数据构造
        /// </summary>
        public ResourceRecommendTotalByPosition(IDataReader reader)
            : this()
        {
            LoadFromDb(reader);
        }

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public ResourceRecommendTotalByPosition()
        {
        }


        #region 属性


        public string PositionName { get; set; }

        public int Position { get; set; }
        /// <summary>
        /// 没有position的时候用tagid
        /// </summary>
        public bool IsTagID { get; set; }

        public int TagID { get; set; }

        public int ProjectSource { get; set; }

        public string Name { get; set; }

        public string PageName { get; set; }

        public string PageType { get; set; }

        public string TagName { get; set; }

        public int Date1Count { get; set; }
        public int Date2Count { get; set; }
        public int Date3Count { get; set; }
        public int Date4Count { get; set; }
        public int Date5Count { get; set; }
        public int Date6Count { get; set; }
        public int Date7Count { get; set; }

        #endregion

        /// <summary>
        /// 来自数据库的数据构造
        /// </summary>
        public void LoadFromDb(IDataReader reader)
        {
            for (int i = 0; i < reader.FieldCount; i++)
            {
                switch (reader.GetName(i).ToLower())
                {  
                    case "name":
                        Name =reader.IsDBNull(i)?"": reader.GetString(i);
                        break;
                    case "pagename":
                        PageName = reader.IsDBNull(i) ? "" : reader.GetString(i);
                        break;
                    case "pagetype":
                        PageType = reader.IsDBNull(i) ? "" : reader.GetString(i);
                        break; 
                    case "date1count":
                        Date1Count = reader.GetInt32(i);
                        break;
                    case "date2count":
                        Date2Count = reader.GetInt32(i);
                        break;
                    case "date3count":
                        Date3Count = reader.GetInt32(i);
                        break;
                    case "date4count":
                        Date4Count = reader.GetInt32(i);
                        break;
                    case "date5count":
                        Date5Count = reader.GetInt32(i);
                        break;
                    case "date6count":
                        Date6Count = reader.GetInt32(i);
                        break;
                    case "date7count":
                        Date7Count = reader.GetInt32(i);
                        break;
                    case "position":
                        Position = reader.IsDBNull(i) ? -1 : Convert.ToInt32(reader["position"]);
                        break;
                    case "tagid":
                        TagID = reader.IsDBNull(i) ? -1 : Convert.ToInt32(reader["tagid"]);
                        break;
                    case "projectsource":
                        ProjectSource = (reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["projectsource"]));
                        break;
                }
            }
            PositionName = GetName();
            if (string.IsNullOrEmpty(PositionName))
            {
                IsTagID = true;
            }
            else
                IsTagID = false;
        }

        private string GetName()
        {
            string name = string.Empty;
            if (!string.IsNullOrEmpty(PageName))
                name = PageName;
            if (!string.IsNullOrEmpty(Name))
            {
                if (!string.IsNullOrEmpty(name))
                    name += "-" + Name;
                else
                    name = Name;
            }
            if (!string.IsNullOrEmpty(PageType))
            {
                name += "(" + PageType + ")";
            }
            name = string.IsNullOrEmpty(name) ? "" : name;
            return  name;
        }
        public object GetIndexValue(int index)
        {
            switch (index)
            {
                case 0:
                    return (int)ProjectSource;
                case 1:
                    return (int)Position;
                case 2:
                    return Date1Count;
                case 3:
                    return Date2Count;
                case 4:
                    return Date3Count;
                case 5:
                    return Date4Count;
                case 6:
                    return Date5Count;
                case 7:
                    return Date6Count;
                case 9:
                    return Date7Count;

                default:
                    return (int)Position;
            }
        }

        public ResourceRecommendTotalByPosition Coyp()
        {
            ResourceRecommendTotalByPosition pos = new ResourceRecommendTotalByPosition();
            pos.IsTagID = this.IsTagID;
            pos.Name = this.Name;
            pos.PageName = this.PageName;
            pos.PageType = this.PageType;
            pos.ProjectSource = this.ProjectSource;
            pos.TagID = this.TagID;
            pos.TagName = this.TagName;
            pos.PositionName = this.PositionName;
            pos.Position = this.Position;
            pos.Date1Count = this.Date1Count;
            pos.Date2Count = this.Date2Count;
            pos.Date3Count = this.Date3Count;
            pos.Date4Count = this.Date4Count;
            pos.Date5Count = this.Date5Count;
            pos.Date6Count = this.Date6Count;
            pos.Date7Count = this.Date7Count;
            return pos;
        }
    }
}
