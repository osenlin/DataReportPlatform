using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using net91com.Reports.DataAccess.B_Basic;
using net91com.Reports.Entities.B_Other;
using net91com.Reports.Entities.JsEntity;
using net91com.Reports.Services.CommonServices.B_BaseTool;
using net91com.Reports.Services.ServicesBaseEntity;
using net91com.Reports.UserRights;
using net91com.Stat.Services.sjqd;
using net91com.Stat.Services.sjqd.Entity;

namespace net91com.Reports.Services.HttpServices.R_Manage
{
    public class B_HttpDownPositionManage : HttpServiceBase
    {
        public override string ServiceCategory
        {
            get { return "B_HttpDownPositionManage"; }
        }
        public override string ServiceName
        {
            get { return "下载位置管理"; }
        }
        public override RightEnum RightType
        {
            get { return RightEnum.UrlAndProjectSourceRight; }
        }
        public override string RightUrl
        {
            get { return "/Reports_New/R_Manage/B_DownPositionManage.aspx"; }
        }

        public override Result Process(HttpContext context)
        {

            string ss=context.Request["iDisplayLength"];
            string action = context.Request["do"];
            Result myresult = Result.GetFailedResult("");
            if (action != null)
            {
                switch (action.Trim().ToLower())
                {
                    case "get_page":
                        myresult = get_page(context);
                        return myresult;
                    case "getpositionbypsp":
                        myresult = getPositionByPSP(context);
                        return myresult;
                    case "addposition":
                        myresult = addPosition(context);
                        return myresult;
                    case "saveposition":
                        myresult = savePosition(context);
                        return myresult;
                    case "batcheditname":
                        myresult = BatchEditPositionName(context);
                        return myresult;
                }
            }
            return myresult;
        }

        private Result BatchEditPositionName(HttpContext context)
        {
            int region = Convert.ToInt32(context.Request["region"]);
            int restype = Convert.ToInt32(context.Request["restype"]);
            int projectsource = Convert.ToInt32(context.Request["projectsource"]);
            string pagetype = context.Request["pagetype"];
            string idnamelist = context.Request["idnamelist"];
            int flag = B_BaseDownPositionService.BatchEditPositionName(
                                                          new B_DownPositionEntity
                                                          {
                                                              ProjectSourceType = region,
                                                              PageType = pagetype,
                                                              ProjectSource = projectsource,
                                                              ResType = restype,
                                                          }, idnamelist);
            if (flag <= 0)
            {
                return Result.GetFailedResult("保存失败");
            }
            return Result.GetSuccessedResult("", "保存成功", true);
        }

        private Result savePosition(HttpContext context)
        {
            int region = Convert.ToInt32(context.Request["region"]);
            int restype = Convert.ToInt32(context.Request["restype"]);
            int projectsource = Convert.ToInt32(context.Request["projectsource"]);
            int positionid = Convert.ToInt32(context.Request["position"]);
            string positionname = context.Request["positionname"];
            string tag = context.Request["checkistag"];
            int downtype = Convert.ToInt32(context.Request["downtype"]);
            string pagename = context.Request["pagename"];
            string pagetype = context.Server.UrlDecode(context.Request["pagetype"]);

            int flag2 = B_BaseDownPositionService.UpdatePosition2MySql(
                                              new B_DownPositionEntity
                                              {
                                                  ByTag4MySql = tag == "1" ? 1 : 0,
                                                  Name = positionname,
                                                  ProjectSourceType = region,
                                                  PageName = pagename,
                                                  PageType = pagetype,
                                                  Position = positionid,
                                                  ProjectSource = projectsource,
                                                  ResType = restype,
                                                  DownType = downtype,
                                              });

            if (flag2 <= 0)
            {
                return Result.GetFailedResult(string.Format("保存{0}失败","新数据库"));
            }
            return Result.GetSuccessedResult("", "保存成功", true);
        }

        private Result addPosition(HttpContext context)
        {
            int region = Convert.ToInt32(context.Request["region"]);
            int restype = Convert.ToInt32(context.Request["restype"]);
            int projectsource = Convert.ToInt32(context.Request["projectsource"]);
            int positionid = Convert.ToInt32(context.Request["position"]);
            string positionname = context.Request["positionname"];
            string tag = context.Request["checkistag"];
            int downtype = Convert.ToInt32(context.Request["downtype"]);
            string pagename = context.Request["pagename"];
            string pagetype = context.Server.UrlDecode(context.Request["pagetype"]);

            B_BaseTool_DataAccess basedata=new B_BaseTool_DataAccess();
            int softid2=basedata.getSoftid2byProjectsource(projectsource, region);
            int flag2 = B_BaseDownPositionService.AddPosition2MySql(
                                                          new B_DownPositionEntity
                                                          {
                                                              ByTag4MySql = tag == "1" ? 1 : 0,
                                                              Name = positionname,
                                                              PageName = pagename,
                                                              PageType = pagetype,
                                                              Position = positionid,
                                                              ProjectSource = projectsource,
                                                              ProjectSourceType = region,
                                                              ResType = restype,
                                                              DownType = downtype,
                                                              SoftId = softid2

                                                          });

            if ((flag2)<=0)
            {
                return Result.GetFailedResult(string.Format("添加到{0}失败","新数据库"));
            }
            return Result.GetSuccessedResult("", "添加成功", true);
        }

        private Result get_page(HttpContext context)
        {
            DataTablesRequest param = new DataTablesRequest(context.Request);
            int region = Convert.ToInt32(context.Request["region"]);
            int restype = Convert.ToInt32(context.Request["restype"]);
            int projectsource = Convert.ToInt32(context.Request["projectsource"]);
            string positionid = context.Request["position"];
            string positionname = context.Request["positionname"];
            string pagename = context.Request["pagename"];
            string pagetype = context.Server.UrlDecode(context.Request["pagetype"]);
            int downtype = Convert.ToInt32(context.Request["downtype"]);
            int checkistag = Convert.ToInt32(context.Request["checkistag"]);
            int pagesize = param.iDisplayLength;
            int pageIndex = (param.iDisplayStart + 1) / param.iDisplayLength + 1;
            int recordcount = 0;

            B_BaseTool_DataAccess basetool = new B_BaseTool_DataAccess();
            int softid2=basetool.getSoftid2byProjectsource(projectsource, region);
            
            var list = B_BaseDownPositionService.Instance.GetB_DownPositionListByCache((ProjectSourceTypeOptions)region,downtype,checkistag,
                                                                                restype, projectsource, positionid,
                                                                                positionname, pagename,pagetype, (pageIndex - 1) * pagesize,
                                                                                pagesize,softid2, out recordcount);        
           

            JQueryDataTableData dt = new JQueryDataTableData();
            dt.sEcho = param.sEcho;
            dt.iTotalRecords =dt.iTotalDisplayRecords = recordcount;
            dt.aaData = GetDataStrList(list);
            return Result.GetSuccessedResult(dt, false);
        }

        private List<List<string>> GetDataStrList(List<B_DownPositionEntity> list)
        {
            var tempList = new List<List<string>>();
            foreach (B_DownPositionEntity obj in list)
            {
                List<string> values = new List<string>();
                values.Add(obj.Position.ToString());
                values.Add(obj.Name);
                values.Add(obj.PageName);
                values.Add(obj.PageType);
                values.Add(obj.ByTag4MySql!=1?"否":"是");
                values.Add(GetDownTypeName(obj.DownType));
                values.Add(obj.ResType + "_" + obj.ProjectSource);
                tempList.Add(values);
            }
            return tempList;
        }

        private string GetDownTypeName(int downtype)
        {
            switch (downtype)
            {
                case 1:
                    return "普通下载位置";
                case 2:
                    return "更新下载位置";
                case 3:
                    return "搜索下载位置";
                case 4:
                    return "静默更新位置";
            }
            return "";
        }

        private Result getPositionByPSP(HttpContext context)
        {
            int region = Convert.ToInt32(context.Request["region"]);
            int restype = Convert.ToInt32(context.Request["restype"]);
            int projectsource = Convert.ToInt32(context.Request["projectsource"]);
            string positionid = context.Request["position"];

            var us =
                B_BaseDownPositionService.Instance.GetB_DownPosition(
                    (ProjectSourceTypeOptions)region, restype,
                   projectsource, positionid);
            return  Result.GetSuccessedResult(Newtonsoft.Json.JsonConvert.SerializeObject(us),false);

        }
    }
}
