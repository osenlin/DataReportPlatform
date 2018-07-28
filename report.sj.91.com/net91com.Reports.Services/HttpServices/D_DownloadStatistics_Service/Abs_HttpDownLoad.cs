using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using net91com.Reports.Entities.D_DownLoadStatisticsEntities;
using net91com.Reports.Entities.JsEntity;
using net91com.Reports.Services.ServicesBaseEntity;

namespace net91com.Reports.Services.HttpServices.D_DownloadStatistics_Service
{
    public abstract  class Abs_HttpDownLoad<T>:HttpServiceBase
    {

        public virtual Result GetChart<T>(HttpContext context)
        {
            throw new NotImplementedException();
        }

        public override Result Process(HttpContext context)
        {
            string action = context.Request["do"];
            Result myresult = Result.GetFailedResult("");
            if (action != null)
            {
                switch (action.Trim().ToLower())
                {
                    //获取Detail表格的接口
                    case "get_detailtable":
                        myresult = GetDetailTable<T>(context);
                        return myresult;
                    //获取图形报表的接口
                    case "get_chart":
                        myresult = GetChart<T>(context);
                        return myresult;
                    //获取Excel文档的接口
                    case "get_excel":
                        myresult = GetExcel<T>(context);
                        return myresult;
                }
            }
            return myresult;
        }
        //下载和展现的detail表格的列数是否一致，不一致为false
        protected bool supdetaileqchart = true;
        /// <summary>
        /// 获取填充报表的数据
        /// </summary>
        /// <typeparam name="T">domain model</typeparam>
        /// <param name="context">HttpContext</param>
        /// <returns></returns>
        protected virtual Result GetDetailTable<T>(HttpContext context)
        {
            List<T> list;
            var lists = GetData<T>(context, true);
            list = lists.Count != 0 ? lists[0] : new List<T>();
            DataTablesRequest param = new DataTablesRequest(context.Request);
            JQueryDataTableData dt = new JQueryDataTableData();
            dt.sEcho = param.sEcho;
            dt.iTotalRecords = dt.iTotalDisplayRecords = list.Count;
            dt.aaData = ObjectParseListFillDetailTable(list);
            return Result.GetSuccessedResult(dt, false);
        }

        protected virtual Result GetExcel<T>(HttpContext context)
        {
            var lists = GetData<T>(context, true);
            List<T> list;
            list = lists.Count != 0 ? lists[0] : new List<T>();

            List<List<string>> result = ObjectParseListFillDetailTable(list, supdetaileqchart);
            string html = string.Empty;
            SetDownHead(context.Response, DownloadExcelName,true);
            html = GetTableHtml(DownloadExcelDataTableTitleName, result);
            return Result.GetSuccessedResult(html, false, true);
        }

        /// <summary>
        /// 下载的Excel数据表的表头名
        /// </summary>
        protected abstract string[] DownloadExcelDataTableTitleName { get; } 

        /// <summary>
        /// 填充表格 将对象转为集合
        /// </summary>
        /// <typeparam name="T">对象</typeparam>
        /// <param name="list">对象集合</param>
        /// <returns>转换后的集合</returns>
        protected abstract List<List<string>> ObjectParseListFillDetailTable<T>(List<T> list,bool DetailEqChart=true);

        /// <summary>
        /// 获取数据对象集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <param name="flag"></param>
        /// <returns></returns>
        protected abstract List<List<T>> GetData<T>(HttpContext context, bool flag = false);

        /// <summary>
        /// 下载的Excel的文件名
        /// </summary>
        protected abstract string DownloadExcelName { get;  }


    }
}
