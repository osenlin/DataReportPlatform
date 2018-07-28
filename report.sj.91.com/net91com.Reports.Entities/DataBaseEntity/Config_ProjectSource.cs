using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace net91com.Reports.Entities.DataBaseEntity
{
    public class Config_ProjectSource
    {
        /// <summary>
        /// 来自数据库的数据构造
        /// </summary>
        public Config_ProjectSource(IDataReader reader)
            : this()
        {
            LoadFromDB(reader);
        }

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public Config_ProjectSource()
        {
        }

        public void LoadFromDB(IDataReader reader)
        {
            for (int i = 0; i < reader.FieldCount; i++)
            {
                switch (reader.GetName(i).ToLower())
                {
                    case "projectsource":
                        ProjectSourceID = reader.IsDBNull(i) ? 0 : reader.GetInt32(i);
                        break;
                    case "name":
                        ProjectSourceName = reader.IsDBNull(i) ? "" : reader.GetString(i);
                        break;
                    case "softid":
                        SoftID = reader.IsDBNull(i) ? 0 : reader.GetInt32(i);
                        break;
                }
            }
        }

        //项目ID
        public int ProjectSourceID { get; set; }
        //软件ID
        public int SoftID { get; set; }
        //项目名称
        public string ProjectSourceName { get; set; }
    }
}