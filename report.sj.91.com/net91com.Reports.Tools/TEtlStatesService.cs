using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using net91com.Core;

namespace net91com.Reports.Tools
{
    /// <summary>
    /// 状态信息数据存取
    /// </summary>
    public class TEtlStatesService
    {
        /// <summary>
        /// 添加状态信息
        /// </summary>
        /// <param name="state"></param>
        public void AddEtlState(EtlState state)
        {
            DAEtlStatesHelper.AddEtlState(state);
        }

        /// <summary>
        /// 删除状态信息
        /// </summary>
        /// <param name="id"></param>
        public void DeleteEtlState(int id)
        {
            DAEtlStatesHelper.DeleteEtlState(id);
        }

        /// <summary>
        /// 更新状态信息
        /// </summary>
        /// <param name="state"></param>
        public void UpdateEtlState(EtlState state)
        {
            DAEtlStatesHelper.UpdateEtlState(state);
        }

        /// <summary>
        /// 获取状态信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public EtlState GetEtlState(int id)
        {
            return DAEtlStatesHelper.GetEtlState(id);
        }

        /// <summary>
        /// 获取指定分类的状态信息
        /// </summary>
        /// <param name="type"></param>
        /// <param name="keyword"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="recordCount"></param>
        /// <returns></returns>
        public List<EtlState> GetEtlStates(EtlStateTypeOptions type, string keyword, int pageIndex, int pageSize, ref int recordCount)
        {
            return DAEtlStatesHelper.GetEtlStates(type, keyword, pageIndex, pageSize, ref recordCount);
        }

        /// <summary>
        /// 获取所有的状态分类信息
        /// </summary>
        /// <returns></returns>
        public Dictionary<int, string> GetEtlStateTypes()
        {
            Dictionary<int, string> typeDict = new Dictionary<int, string>();
            typeDict.Add(0, "未指定");
            typeDict.Add(1, "应用启动日志统计");
            typeDict.Add(2, "使用日志统计");
            typeDict.Add(3, "广告日志统计");
            return typeDict;
        }
    }
}