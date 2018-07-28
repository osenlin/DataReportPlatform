using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace net91com.Reports.Entities.DataBaseEntity
{
    public class Sjqd_ChannelCustomers
    {
        /// <summary>
        /// 来自数据库的数据构造
        /// </summary>
        public Sjqd_ChannelCustomers(IDataReader reader)
            : this()
        {
            LoadFromDb(reader);
        }

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public Sjqd_ChannelCustomers()
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
                    case "id":
                        ID = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["id"]);
                        break;
                    case "name":
                        Name = reader.IsDBNull(i) ? "" : reader["name"].ToString();
                        break;
                    case "isrealtime":
                        IsRealtime = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["isrealtime"]);
                        break;
                    case "pid":
                        PID = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["pid"]);
                        break;
                    case "cid":
                        CID = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["cid"]);
                        break;
                    case "softid":
                        SoftID = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["SoftID"]);
                        break;
                    case "modulus1":
                        Modulus1 = reader.IsDBNull(i) ? 0 : Convert.ToDecimal(reader["modulus1"]);
                        break;
                    case "modulus2":
                        Modulus2 = reader.IsDBNull(i) ? 0 : Convert.ToDecimal(reader["modulus2"]);
                        break;
                    case "reporttype":
                        ReportType = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["reporttype"]);
                        break;
                    case "modulus_shanzhai":
                        //-1表示继承 0表示关闭 1 表示开启
                        Modulus_Shanzhai = reader.IsDBNull(i) ? -1 : Convert.ToInt32(reader["modulus_shanzhai"]);
                        break;
                    case "minviewtime":
                        MinViewTime = reader.IsDBNull(i) ? DateTime.MinValue : Convert.ToDateTime(reader["minviewtime"]);
                        break;
                    case "showtype":
                        ShowType = reader.IsDBNull(i) ? 0 : Convert.ToInt32(reader["showtype"]);
                        break;
                    case "promotemodename":
                        PromoteModeName = reader.IsDBNull(i) ? "" : reader["promotemodename"].ToString();
                        break;
                    case "channeltypename":
                        ChannelTypeName = reader.IsDBNull(i) ? "" : reader["channeltypename"].ToString();
                        break;
                    case "firstlevelchannelcatename":
                        FirstLevelChannelCateName = reader.IsDBNull(i)
                                                        ? ""
                                                        : reader["firstlevelchannelcatename"].ToString();
                        break;
                    case "secondlevelchannelcatename":
                        SecondLevelChannelCateName = reader.IsDBNull(i)
                                                         ? ""
                                                         : reader["secondlevelchannelcatename"].ToString();
                        break;
                    case "exchangetypename":
                        ExchangeTypeName = reader.IsDBNull(i) ? "" : reader["exchangetypename"].ToString();
                        break;
                    case "cooperationmodename":
                        CooperationModeName = reader.IsDBNull(i) ? "" : reader["cooperationmodename"].ToString();
                        break;
                    case "cooperateidname":
                        CooperateIDName = reader.IsDBNull(i) ? "" : reader["cooperateidname"].ToString();
                        break;
                    case "channeladminidname":
                        ChannelAdminIDName = reader.IsDBNull(i) ? "" : reader["channeladminidname"].ToString();
                        break;
                    case "addtime":
                        AddTime = reader.IsDBNull(i) ? DateTime.MinValue : Convert.ToDateTime(reader["addtime"]);
                        break;
                }
            }
            AddTimeString = AddTime == DateTime.MinValue ? "默认" : AddTime.ToString("yyyy-MM-dd HH:mm:ss");
        }


        public int ID { get; set; }

        /// <summary>
        /// 渠道商名称
        /// </summary>
        public string Name { get; set; }

        public int IsRealtime { get; set; }

        public int PID { get; set; }

        public int CID { get; set; }

        public decimal Modulus1 { get; set; }

        public decimal Modulus2 { get; set; }

        /// <summary>
        /// 0 未设置， 1 一次乘以系数提供用户 
        /// </summary>
        public int ReportType { get; set; }

        /// <summary>
        /// 给内部人看的key
        /// </summary>
        public string Keyfornd { get; set; }

        /// <summary>
        /// 给外部人看的key
        /// </summary>
        public string Keyforout { get; set; }


        public Dictionary<string, string> Keyforout_RetainDic { get; set; }

        /// <summary>
        /// 判断山寨机系数字段 1 代表开启 0 代表关闭 -1 代表继承
        /// </summary>
        public int Modulus_Shanzhai { get; set; }


        public int SoftID { get; set; }

        /// <summary>
        /// 展示类型
        /// </summary>
        public int ShowType { get; set; }

        /// <summary>
        /// 最小可看日期
        /// </summary>
        public DateTime MinViewTime { get; set; }


        public string PromoteModeName { get; set; }

        public string ChannelTypeName { get; set; }

        public string FirstLevelChannelCateName { get; set; }

        public string SecondLevelChannelCateName { get; set; }

        public string ExchangeTypeName { get; set; }

        public string CooperationModeName { get; set; }

        public string CooperateIDName { get; set; }

        public string ChannelAdminIDName { get; set; }

        /// <summary>
        /// 添加时间
        /// </summary>
        public DateTime AddTime { get; set; }

        public string AddTimeString { get; set; }
    }
}