using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace net91com.Reports.Entities.B_Other
{
    public class B_AuthorEntity
    {
        public int AuthorID { get; set; }
        public string AuthorName { get; set; }

        public B_AuthorEntity(){}

        public B_AuthorEntity(IDataReader reader)
        {
            LoadFromDb(reader);
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
                    case "authorid":
                        AuthorID = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["authorid"]);
                        break;
                    case "authorname":
                        AuthorName = reader.IsDBNull(i) ? "" : reader["authorname"].ToString();
                        break;
                }
            }
        }
    }
}
