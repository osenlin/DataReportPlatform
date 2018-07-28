using System;
using System.Data;

namespace net91com.Reports.Entities.B_Other
{
    public class B_ResUninstallEntity
    {
        #region 属性

        public int Rank { get; set; }
        public int ResID { get; set; }
        public string ResName { get; set; }
        public string ResIdentifier { get; set; }
        public int UninstallCount { get; set; }
        public int UserCount { get; set; }
        public double Ratio { get; set; }
        public string UninstallInfo { get; set; }


        #endregion

        /// <summary>
        /// 来自数据库的数据构造
        /// </summary>
        public B_ResUninstallEntity(IDataReader reader)
            : this()
        {
            LoadFromDb(reader);
        }

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public B_ResUninstallEntity()
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
                    case "resid":
                        ResID = Convert.ToInt32((reader["resid"]));
                        break;
                    case "resname":
                        ResName = reader["resname"].ToString();
                        break;
                    case "uninstallinfo":
                        UninstallInfo = reader["uninstallinfo"].ToString();
                        break;
                    case "residentifier":
                        ResIdentifier = reader["residentifier"].ToString();
                        break;
                    case "uninstallcount":
                        UninstallCount = Convert.ToInt32((reader["uninstallcount"]));
                        break;
                    case "usercount":
                        UserCount = Convert.ToInt32((reader["usercount"]));
                        break;

                }

                Ratio = UserCount * 1.0 / UninstallCount;
            }
        }
    }
}
