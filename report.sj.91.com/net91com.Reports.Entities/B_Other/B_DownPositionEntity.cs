using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace net91com.Reports.Entities.B_Other
{
    public class B_DownPositionEntity
    {
        /// <summary>
        /// 数据来源
        /// </summary>
        public int ProjectSource { get; set; }

        public int SoftId { get; set; }
        /// <summary>
        /// 数据来源类型
        /// </summary>
        public int ProjectSourceType { get; set; }
        /// <summary>
        /// 资源类型
        /// </summary>
        public int ResType { get; set; }
        /// <summary>
        /// 位置id
        /// </summary>
        public int Position { get; set; }
        /// <summary>
        /// 位置名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 页面名称
        /// </summary>
        public string PageName { get; set; }
        /// <summary>
        /// 页面类型
        /// </summary>
        public string PageType { get; set; }
        /// <summary>
        /// 是否为专辑
        /// </summary>
        public bool ByTag { get; set; }

        /// <summary>
        /// 是否为专辑
        /// </summary>
        public int ByTag4MySql { get; set; }
        /// <summary>
        /// 下载类型1：一般下载 2：更新下载 3：搜索下载 4：静默更新下载
        /// </summary>
        public int DownType { get; set; }


        public B_DownPositionEntity()
        {
        }

        public B_DownPositionEntity(IDataReader read):this()
        {
            for (int i = 0; i < read.FieldCount; i++)
            {
                switch (read.GetName(i).ToLower())
                {
                    case "softid":
                        SoftId = (read["softid"] == DBNull.Value || read["softid"] == null) ? 0 : Convert.ToInt32(read["softid"]);
                        break;
                    case "projectsource":
                        ProjectSource = Convert.ToInt32(read["projectsource"]);
                        break;
                    case "projectsourcetype":
                        ProjectSourceType = Convert.ToInt32(read["projectsourcetype"]);
                        break;
                    case "restype":
                        ResType = Convert.ToInt32(read["restype"]);
                        break;
                    case "position":
                        Position = Convert.ToInt32(read["position"]);
                        break;
                    case "name":
                        Name = (read["name"] == DBNull.Value || read["name"] == null) ? "" : read["name"].ToString();
                        break;
                    case "pagename":
                        PageName = (read["pagename"] == DBNull.Value || read["pagename"] == null) ? "" : read["pagename"].ToString();
                        break;
                    case "pagetype":
                        PageType = (read["pagetype"] == DBNull.Value || read["pagetype"] == null) ? "" : read["pagetype"].ToString();
                        break;
                    case "bytag":
                        ByTag = (read["bytag"] == DBNull.Value || read["bytag"] == null) ? false : (Boolean)read["bytag"];
                        break;
                    case "bytag4mysql":
                        ByTag4MySql = Convert.ToInt32(read["bytag4mysql"]);
                        break;
                    case "downtype":
                        DownType = Convert.ToInt32(read["downtype"]);;
                        break;
                }
            }                       
        }
    }
}
