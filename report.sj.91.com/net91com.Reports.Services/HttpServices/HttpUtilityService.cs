using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using net91com.Reports.Entities.B_Other;
using net91com.Reports.Entities.ViewEntity;
using net91com.Reports.Services.CommonServices.B_BaseTool;
using net91com.Reports.Services.CommonServices.SjqdUserStat;
using net91com.Reports.Services.ServicesBaseEntity;
using net91com.Reports.UserRights;

namespace net91com.Reports.Services.HttpServices
{
    public class HttpUtilityService : HttpServiceBase
    {
        public override string ServiceCategory
        {
            get { return "UtilityService"; }
        }

        public override string ServiceName
        {
            get { return "获取页面通用数据"; }
        }

        public override RightEnum RightType
        {
            get { return RightEnum.NoCheck; }
        }

        public override string RightUrl
        {
            get { return ""; }
        }


        public override Result Process(HttpContext context)
        {
            string action = context.Request["do"];
            Result myresult = Result.GetFailedResult("");
            if (action != null)
            {
                switch (action.Trim().ToLower())
                {
               
                    case "getchanneltree":
                        myresult = GetTree(context);
                        return myresult;
                    case "getversionbysoftid":
                        myresult = GetVersionBySoft(context);
                        return myresult;
                    case "getversionbysoftidnew":
                        myresult = GetVersionBySoftnew(context);
                        return myresult;
                    case "getextendattrbyrestype":
                        myresult = GetExtendAttrByRestType(context);
                        return myresult;
                }
            }
            return myresult;
        }

        private Result GetExtendAttrByRestType(HttpContext context)
        {
            int restype = Convert.ToInt32(context.Request["restype"]);

            List<Dictionary<string, string>> map = B_BaseToolService.Instance.getExtendResAttr(restype);

            var lst = new List<KeyValueModel>();

            foreach (var dicitem in map)
            {
                lst.Add(new KeyValueModel {ID = dicitem["type"], Value = dicitem["name"]});
            }

            return Result.GetSuccessedResult(lst, true);
        }

        private Result GetVersionBySoftnew(HttpContext context)
        {
            int softid = Convert.ToInt32(context.Request["softs"]);
            int platform = Convert.ToInt32(context.Request["platform"]);
            List<KeyValueModel> keys = null;
            if (platform == 0)
            {
                keys = new List<KeyValueModel>();
            }
            else
            {
                keys =
                    B_BaseToolService.Instance.GetVersionCache(softid, platform)
                                     .Select(
                                         p => new KeyValueModel {ID = p.Version, Value = p.Version })
                                     .ToList();
            }
            return Result.GetSuccessedResult(keys, true);
        }



        /// <summary>
        ///     渠道商管理的渠道树获取
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public Result GetTree(HttpContext context)
        {
            int softId = Convert.ToInt32(context.Request["softs"]);
            bool showChannelId = Convert.ToInt32(context.Request["showChannelId"]) > 0 ? true : false;

            List<Channel> channels = new URChannelsService().GetChannels(softId, showChannelId);
            var jsonBuilder = new StringBuilder("[");
            foreach (Channel chl in channels)
            {
                jsonBuilder.AppendFormat(
                    "{{\"id\":\"{0}_{1}\",\"pId\":\"{2}_{3}\",\"name\":\"{4}\",\"open\":false,\"val\":{5}," +
                    "\"type\":{6},\"checked\":false,\"softid\":\"{7}\",\"drag\":{8}}},"
                    , chl.ChannelType, chl.ID, chl.ParentChannelType, chl.ParentID,
                    (chl.Platform == 0 ? chl.Name : chl.Name + "(" + chl.Platform + ")").Replace("\"", ""),
                    chl.ID, (int) chl.ChannelType, softId,
                    (!(chl.ChannelType == ChannelTypeOptions.Category)).ToString().ToLower());
            }
            string nodes = jsonBuilder.ToString().TrimEnd(',') + "]";
            return Result.GetSuccessedResult(nodes, true);
        }

        private Result GetVersionBySoft(HttpContext context)
        {
            int softid = Convert.ToInt32(context.Request["softs"]);
            int platform = Convert.ToInt32(context.Request["platform"]);
            List<KeyValueModel> keys = null;
            if (platform == 0)
            {
                keys = new List<KeyValueModel>();
            }
            else
            {
                keys =
                    Sjqd_SoftVersionsService.Instance.GetVersionByVersionType(softid, platform, 0)
                                            .Select(
                                                p =>
                                                new KeyValueModel {ID = p.Version, Value = p.Version })
                                            .ToList();
            }
            return Result.GetSuccessedResult(keys, true);
        }
    }
}