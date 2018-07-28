using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using net91com.Common;
using net91com.Core;
using net91com.Reports.DataAccess.SjqdUserStat;
using net91com.Reports.Entities.DataBaseEntity;
using net91com.Reports.Entities.JsEntity;
using net91com.Reports.Services.CommonServices.SjqdUserStat;
using net91com.Reports.Services.ServicesBaseEntity;
using net91com.Reports.UserRights;

namespace net91com.Reports.Services.HttpServices.R_UserAnalysis_Service
{
    public class HttpChannelCustomerManager : HttpServiceBase
    {
        public override string ServiceCategory
        {
            get { return "ChannelCustomerManager"; }
        }

        public override string ServiceName
        {
            get { return "渠道商管理页面"; }
        }

        public override RightEnum RightType
        {
            get { return RightEnum.DefinedByYourselfAndUrlAndSoftRight; }
        }

        public override string RightUrl
        {
            get { return "Reports_New/R_UserAnalysis/ChannelCustomerManager.aspx"; }
        }

        public override bool Check(object obj)
        {
            return true;
        }

        public override Result Process(HttpContext context)
        {
            string action = context.Request["do"];
            Result myresult = Result.GetFailedResult("");
            if (action != null)
            {
                switch (action.Trim().ToLower())
                {
                        #region 分类增删改

                    case "editaddcate":
                        myresult = EditAddCate(context);
                        return myresult;
                    case "getcate":
                        myresult = GetCate(context);
                        return myresult;
                    case "deletecate":
                        myresult = DeleteCate(context);
                        return myresult;
                    case "addcatecustom":
                        myresult = AddCateCustom(context);
                        return myresult;

                        #endregion

                        #region channelid页面相关设置

                    case "searchcustomersbychannelid":
                        myresult = SearchcustomersByChannelid(context);
                        return myresult;
                    case "getchannelsbycustomer":
                        myresult = GetChannelsByCustomer(context);
                        return myresult;
                    case "deletechannelid":
                        myresult = DeleteChannelId(context);
                        return myresult;
                    case "updatechannelid":
                        myresult = UpdateChannelId(context);
                        return myresult;
                    case "setselectchannelm1":
                        myresult = SetSelectChannelM1(context);
                        return myresult;
                    case "adddefinedchannelid":
                        myresult = AddDefinedChannelid(context);
                        return myresult;
                    case "getunbindlist":
                        myresult = GetUnBindList(context);
                        return myresult;
                    case "addnewchannelfromclient":
                        myresult = BindNewChannelFromClient(context);
                        return myresult;

                        #endregion

                        #region 对渠道商管理

                    case "deletechannelcustomer":
                        myresult = DeleteChannelCustomer(context);
                        return myresult;
                    case "addcustomer":
                        myresult = AddCustomer(context);
                        return myresult;
                    case "updatecustomer":
                        myresult = UpdateCustomer(context);
                        return myresult;
                    case "getcustomersbyparentid":
                        myresult = GetCustomersByParentId(context);
                        return myresult;
                    case "getcustomerbyid":
                        myresult = GetCustomerById(context);
                        return myresult;
                    case "setcustomerswitch":
                        myresult = SetCustomerSwitch(context);
                        return myresult;
                    case "changecustomerposition":
                        myresult = ChangeCustomerPositon(context);
                        return myresult;
                        //添加同级渠道商
                    case "addsamelevelcustomer":
                        myresult = AddSameLevelCustomer(context);
                        return myresult;
                    case "batchaddcustomer":
                        myresult = AddBatchCustomer(context);
                        return myresult;

                        #endregion 对渠道商管理                       
                }
            }
            return myresult;
        }

        //添加同级渠道商
        private Result AddSameLevelCustomer(HttpContext context)
        {
            var loginService = new URLoginService();
            string name = HttpUtility.HtmlEncode(context.Request["name"]);
            int softid = Convert.ToInt32(context.Request["softs"]);
            int samelevelcustomer = Convert.ToInt32(context.Request["samelevelcustomerid"]);
            //获取来源目标的类型
            Sjqd_ChannelCustomers parentcustomer =
                SjqdChannelCustomers_DataAccess.Instance.GetChannelCustomer(samelevelcustomer);
            if (parentcustomer.ID == 0)
            {
                return Result.GetFailedResult("添加失败!");
            }
            var custom = new Sjqd_ChannelCustomers();
            custom.IsRealtime = 0;
            custom.Name = name;
            //默认值
            custom.ReportType = 0;
            custom.PID = parentcustomer.PID;
            custom.CID = parentcustomer.CID;
            custom.SoftID = parentcustomer.SoftID;
            custom.AddTime = DateTime.Now;
            //若不是顶级渠道商,需要将父级渠道商属性赋予
            if (custom.PID != 0)
            {
                Sjqd_ChannelCustomers obj = SjqdChannelCustomers_DataAccess.Instance.GetChannelCustomer(custom.PID);
            }
            int resultid = SjqdChannelCustomers_DataAccess.Instance.AddChannelCustomer(custom);
            if (resultid > 0)
            {
                loginService.AddLog("AddChannelCustomer",
                                    string.Format("添加渠道商(ID={0},SoftID={1},PID={2},CID={3},Name={4})", resultid, softid,
                                                  custom.PID, custom.CID, custom.Name));
                return Result.GetSuccessedResult(resultid, "添加成功!", true);
            }
            else
            {
                return Result.GetFailedResult("添加失败!");
            }
        }

        /// <summary>
        ///     设置渠道商系数参数
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private Result SetCustomerSwitch(HttpContext context)
        {
            var loginService = new URLoginService();
            int soft = Convert.ToInt32(context.Request["softs"]);
            int id = Convert.ToInt32(context.Request["id"]);
            int switchtype = Convert.ToInt32(context.Request["switchtype"]);
            int switchvalue = Convert.ToInt32(context.Request["switchvalue"]);
            //这里不要使用GetChannelCustomeById 方法 因为获取到的cid 就是标准的cid
            Sjqd_ChannelCustomers obj = SjqdChannelCustomers_DataAccess.Instance.GetChannelCustomer(id);
            //对外链接地址开关设置
            if (switchtype == 1)
            {
                obj.ReportType = switchvalue;
            }
            else if (switchtype == 2)
            {
                obj.ShowType = switchvalue;
            }
            if (SjqdChannelCustomers_DataAccess.Instance.UpdateChannelCustomer(obj) >= 0)
            {
                loginService.AddLog(string.Format("设置渠道商开关,softid:{3},customerid:{0},switchtype:{1},switchvalue:{2}", id,
                                                  switchtype, switchvalue, soft));

                return Result.GetSuccessedResult("", "设置成功!", true);
            }
            else
            {
                return Result.GetFailedResult("设置失败!");
            }
        }

        /// <summary>
        ///     修改渠道商位置
        /// </summary>
        public Result ChangeCustomerPositon(HttpContext context)
        {
            var loginService = new URLoginService();
            int softid = Convert.ToInt32(context.Request["softs"]);
            int targetID = Convert.ToInt32(context.Request["targetid"]);
            int sourceID = Convert.ToInt32(context.Request["sourceid"]);
            int type = Convert.ToInt32(context.Request["targettype"]);
            //获取来源目标的类型
            int result = SjqdChannelCustomers_DataAccess.Instance.ChangeChannelCustomerPosition(targetID, type, sourceID);
            if (result > 0)
            {
                loginService.AddLog(string.Format("修改渠道商位置,softid:{0},customerid:{1},targetid:{2},targettype:{3}",
                                                  softid, sourceID, targetID, type));
                //合并了渠道商
                if (result == 2)
                {
                    return Result.GetSuccessedResult("", "合并成功", true);
                }
                else
                {
                    return Result.GetSuccessedResult((ChannelTypeOptions) type + "_" + targetID, "迁移成功", true);
                }
            }
            else
            {
                return Result.GetFailedResult("迁移失败!");
            }
        }

        /// <summary>
        ///     获取渠道商信息根据渠道商ID
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private Result GetCustomerById(HttpContext context)
        {
            int id = Convert.ToInt32(context.Request["id"]);
            Sjqd_ChannelCustomers obj = SjqdChannelCustomers_DataAccess.Instance.GetChannelCustomer(id);
            obj.Keyfornd = CryptoHelper.DES_Encrypt(id.ToString());
            obj.Keyforout = CryptoHelper.DES_Encrypt(id.ToString(), "ndwebweb");
            var list = new List<int> {0, 1, 4, 7, 9};
            obj.Keyforout_RetainDic = new Dictionary<string, string>();
            for (int i = 0; i < list.Count; i++)
            {
                obj.Keyforout_RetainDic.Add(list[i].ToString(),
                                            CryptoHelper.DES_Encrypt(id + "&" + list[i], "$retain^"));
            }

            //这里做了个特殊处理创建一个临时result 序列化后再页面控制输出
            Result result = Result.GetSuccessedResult();
            result.data = obj;
            var timeConverter = new IsoDateTimeConverter();
            timeConverter.DateTimeFormat = "yyyy-MM-dd";
            string json = JsonConvert.SerializeObject(result, timeConverter);
            return Result.GetSuccessedResult(json, true, true);
        }

        /// <summary>
        ///     根据父级渠道商获取孩子渠道商列表
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private Result GetCustomersByParentId(HttpContext context)
        {
            int id = Convert.ToInt32(context.Request["parentid"]);
            List<Sjqd_ChannelCustomers> customers =
                SjqdChannelCustomers_DataAccess.Instance.GetSjqdCustomersByParentId(id);
            return Result.GetSuccessedResult(customers, true);
        }

        /// <summary>
        ///     更新渠道商系数和名称
        /// </summary>
        /// <returns></returns>
        private Result UpdateCustomer(HttpContext context)
        {
            //pc 产品列表
            var softList = new[] {9, 105550, 72, 57, 68, 69};

            var loginService = new URLoginService();
            int soft = Convert.ToInt32(context.Request["softs"]);
            int id = Convert.ToInt32(context.Request["id"]);
            decimal m1 = Convert.ToDecimal(context.Request["m1"]);
            string name = HttpUtility.HtmlEncode(context.Request["name"]);
            int showtype = Convert.ToInt32(context.Request["showtype"]);
            DateTime minViewTime = Convert.ToDateTime(context.Request["minviewtime"]);
            bool needEditSonCustomer = Convert.ToBoolean(context.Request["needEditSonCustomer"]);
            //开放给外部
            int reporttype = Convert.ToInt32(context.Request["reporttype"]);
            

            //这里不要使用GetChannelCustomeById 方法 因为获取到的cid 就是标准的cid
            Sjqd_ChannelCustomers customer = SjqdChannelCustomers_DataAccess.Instance.GetChannelCustomer(id);
            //minviewtime 若没传 用其老的minviewtime
            if (context.Request["minviewtime"] == null)
            {
                minViewTime = customer.MinViewTime;
            }
            string oldName = customer.Name;
            int oldreporttype = customer.ReportType;
            int oldmodulus_Shanzhai = customer.Modulus_Shanzhai;
            decimal oldm1 = customer.Modulus1;
            int oldShowType = customer.ShowType;
            DateTime oldmintime = customer.MinViewTime;
            customer.MinViewTime = minViewTime;
            customer.Modulus1 = m1;
            customer.Name = name;
            customer.ReportType = reporttype;
            bool canUpdateName = true;
            //有传入就修改
            if (context.Request["showtype"] != null)
            {
                customer.ShowType = Convert.ToInt32(context.Request["showtype"]);
            }
            if (name != oldName &&
                SjqdChannelCustomers_DataAccess.Instance.GetCustomerCount(customer.CID, customer.PID, customer.Name) > 1)
            {
                return Result.GetFailedResult("修改失败,已经存在对应名称渠道商!");
            }
            if (SjqdChannelCustomers_DataAccess.Instance.UpdateChannelCustomer(customer, canUpdateName) >= 0)
            {
                loginService.AddLog(
                    "UpdateChannelCustomer"
                    ,
                    string.Format(
                        "修改渠道商(ID={0},SoftID={1},Name={2}(Old={3}),Modulus1={4}(Old={5}),MinViewTime={6}(Old={7}),ReportType={8}(Old={9}),ShowType={10}(Old={11})",
                        id, soft, name, oldName, m1, oldm1, oldmintime, minViewTime, oldmintime, customer.ReportType,
                        oldreporttype, customer.ShowType, oldShowType));
                return Result.GetSuccessedResult("", "修改成功!", true);
            }
            else
            {
                return Result.GetFailedResult("修改失败!");
            }
        }

        /// <summary>
        ///     添加渠道商
        /// </summary>
        /// <returns></returns>
        private Result AddCustomer(HttpContext context)
        {
            var loginService = new URLoginService();
            string name = HttpUtility.HtmlEncode(context.Request["name"]);
            int id = Convert.ToInt32(context.Request["parentcustomerid"]);
            int cid = Convert.ToInt32(context.Request["cid"]);
            int soft = Convert.ToInt32(context.Request["softs"]);
            var custom = new Sjqd_ChannelCustomers();
            custom.IsRealtime = 0;
            custom.Name = name;
            //默认值
            custom.ReportType = 0;
            custom.PID = id;
            custom.CID = cid;
            custom.SoftID = soft;
            custom.AddTime = DateTime.Now;
            Sjqd_ChannelCustomers obj = SjqdChannelCustomers_DataAccess.Instance.GetChannelCustomer(id);
            if (obj != null)
            {
                custom.CID = obj.CID;
                custom.SoftID = obj.SoftID;
            }
            int resultid = SjqdChannelCustomers_DataAccess.Instance.AddChannelCustomer(custom);
            if (resultid > 0)
            {
                loginService.AddLog("AddChannelCustomer",
                                    string.Format("添加渠道商(ID={0},SoftID={1},PID={2},CID={3},Name={4})", resultid, soft,
                                                  custom.PID, custom.CID, custom.Name));
                return Result.GetSuccessedResult(resultid, "添加成功!", true);
            }
            else
            {
                return Result.GetFailedResult("添加失败!");
            }
        }

        /// <summary>
        ///     添加渠道商
        /// </summary>
        /// <returns></returns>
        private Result AddBatchCustomer(HttpContext context)
        {
            var loginService = new URLoginService();
            string nameprefix = HttpUtility.HtmlEncode(context.Request["nameprefix"]);
            int begin = Convert.ToInt32(context.Request["beginnum"]);
            int end = Convert.ToInt32(context.Request["endnum"]);
            if ((end - begin) >= 20)
                end = begin + 19;
            int id = Convert.ToInt32(context.Request["parentcustomerid"]);
            int cid = Convert.ToInt32(context.Request["cid"]);
            int soft = Convert.ToInt32(context.Request["softs"]);
            int resultid = 0;

            Sjqd_ChannelCustomers obj = SjqdChannelCustomers_DataAccess.Instance.GetChannelCustomer(id);

            for (int i = begin; i <= end; i++)
            {
                string name = nameprefix + i;
                var custom = new Sjqd_ChannelCustomers();
                custom.IsRealtime = 0;
                custom.Name = name;
                //默认值
                custom.ReportType = 0;
                custom.PID = id;
                custom.CID = cid;
                custom.SoftID = soft;
                if (obj != null)
                {
                    custom.CID = obj.CID;
                    custom.SoftID = obj.SoftID;
                }

                resultid = SjqdChannelCustomers_DataAccess.Instance.AddChannelCustomer(custom);
                if (resultid > 0)
                {
                    loginService.AddLog("AddChannelCustomer",
                                        string.Format("添加渠道商(ID={0},SoftID={1},PID={2},CID={3},Name={4})", resultid,
                                                      soft, custom.PID, custom.CID, custom.Name));
                }
            }
            return Result.GetSuccessedResult(resultid, "添加成功!", true);
        }

        /// <summary>
        ///     删除渠道商
        /// </summary>
        /// <returns></returns>
        private Result DeleteChannelCustomer(HttpContext context)
        {
            var loginService = new URLoginService();
            //int id = Convert.ToInt32(context.Request["id"]);
            List<int> ids =
                context.Request["id"].Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries)
                                     .Select(p => Convert.ToInt32(p))
                                     .ToList();
            int softid = Convert.ToInt32(context.Request["softs"]);
            for (int i = 0; i < ids.Count; i++)
            {
                if (SjqdChannelCustomers_DataAccess.Instance.DeleteChannelCustomer(ids[i]) > 0)
                {
                    loginService.AddLog("DeleteChannelCustomer",
                                        string.Format("删除渠道商(ID={0},SoftID={1})", ids[i], softid));
                    if (ids.Count == 1)
                        return Result.GetSuccessedResult("", "删除成功!", true);
                }
                else if (ids.Count == 1)
                {
                    return Result.GetFailedResult("该渠道商下含有渠道ID或渠道商,删除失败!");
                }
            }
            return Result.GetSuccessedResult("", "删除成功(若仍出现请确保该渠道商上没有挂渠道)!", true);
        }

        /// <summary>
        ///     绑定到渠道表中去
        /// </summary>
        /// <returns></returns>
        private Result BindNewChannelFromClient(HttpContext context)
        {
            var loginService = new URLoginService();
            List<int> channelids =
                context.Request["autoids"].Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries)
                                          .Select(p => Convert.ToInt32(p)).ToList();
            int customerid = Convert.ToInt32(context.Request["customerid"]);
            int softid = Convert.ToInt32(context.Request["softs"]);

            string channelidstr = string.Join(",", channelids.Select(p => p.ToString()).ToArray());
            ;
            if (SjqdChannelsFromClient_DataAccess.Instance.AddNewChannelFromClient(channelids, customerid) >= 0)
            {
                loginService.AddLog(string.Format("设置channelid到渠道商下,softid:{0},channelid:{1},customerid:{2}",
                                                  softid, context.Request["autoids"], customerid));
                return Result.GetSuccessedResult("", "添加成功!", true);
            }
            else
            {
                return Result.GetFailedResult("添加失败!");
            }
        }

        /// <summary>
        ///     获取未绑定的channelid
        /// </summary>
        /// <returns></returns>
        private Result GetUnBindList(HttpContext context)
        {
            var loginService = new URLoginService();
            int id = Convert.ToInt32(context.Request["customerid"]);
            int soft = Convert.ToInt32(context.Request["softs"]);
            int plat = Convert.ToInt32(context.Request["plat"]);
            List<Sjqd_ChannelFromClient> channelsList =
                SjqdChannelsFromClient_DataAccess.Instance.GetNotBoundChannelFromClientList(soft);
            var list = new List<object>();
            foreach (Sjqd_ChannelFromClient item in channelsList)
            {
                list.Add(new {ID = item.AuID, Name = item.Name.Trim(), platform = item.Platform});
            }
            return Result.GetSuccessedResult(list, true);
        }

        /// <summary>
        ///     添加自定义channelid
        /// </summary>
        /// <returns></returns>
        private Result AddDefinedChannelid(HttpContext context)
        {
            var loginService = new URLoginService();
            int customerid = Convert.ToInt32(context.Request["customerid"]);
            decimal m1 = Convert.ToDecimal(context.Request["m1"]);
            string[] plats = context.Request["platform"].Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);

            HashSet<int> selectPlats = GetPlats(plats);
            int softid = Convert.ToInt32(context.Request["softs"]);
            string name = HttpUtility.HtmlEncode(context.Request["name"]);

            foreach (int plat in selectPlats)
            {
                if (SjqdChannels_DataAccess.Instance.DefineIsExsit(plat, softid, name) == 0)
                {
                    if (SjqdChannels_DataAccess.Instance.AddDefineChannel(customerid, m1, 0m, plat, softid, name) >= 0)
                    {
                        loginService.AddLog(string.Format("添加自定义渠道商,softid:{0},platform:{2},name:{1}", softid, name,
                                                          plat));
                    }
                }
            }
            return Result.GetSuccessedResult("", "添加成功!", true);
        }

        private HashSet<int> GetPlats(string[] plats)
        {
            var hashSet = new HashSet<int>();
            for (int i = 0; i < plats.Length; i++)
            {
                int tempPlat = Convert.ToInt32(plats[i]);
                if (tempPlat != 0)
                {
                    hashSet.Add(tempPlat);
                }
                else
                {
                    hashSet.Add((int) MobileOption.Android);
                    hashSet.Add((int) MobileOption.iPhone);
                    hashSet.Add((int) MobileOption.AndroidPad);
                    hashSet.Add((int) MobileOption.IPAD);
                }
            }
            return hashSet;
        }

        /// <summary>
        ///     设置所选channelid的m1系数
        /// </summary>
        /// <returns></returns>
        private Result SetSelectChannelM1(HttpContext context)
        {
            var loginService = new URLoginService();
            string[] ids = context.Request["channelids"].TrimEnd(',').Split(new[]
                {
                    ','
                }
                                                                            , StringSplitOptions.RemoveEmptyEntries);
            int softid = Convert.ToInt32(context.Request["softs"]);
            decimal m1 = Convert.ToDecimal(context.Request["m"]);
            if (SjqdChannels_DataAccess.Instance.UpdateM1ByChannels(ids.Select(p => Convert.ToInt32(p)).ToList(), m1) >=
                0)
            {
                loginService.AddLog(string.Format("修改所选渠道id系数值,softid:{0},channelids:{1},m:{2}", softid,
                                                  context.Request["channelids"], m1));
                return Result.GetSuccessedResult("", "设置成功!", true);
            }
            else
            {
                return Result.GetFailedResult("设置失败!");
            }
        }

        /// <summary>
        ///     删除渠道id
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private Result DeleteChannelId(HttpContext context)
        {
            var loginService = new URLoginService();
            List<int> channels =
                context.Request["id"].TrimEnd(',').Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries)
                                     .Select(p => Convert.ToInt32(p)).ToList();
            int softid = Convert.ToInt32(context.Request["softs"]);
            for (int i = 0; i < channels.Count; i++)
            {
                if (SjqdChannels_DataAccess.Instance.DeleteChannelBind(channels[i]) >= 0)
                {
                    loginService.AddLog(string.Format("取消渠道绑定,softid:{0},channelid:{1}", softid, channels[i]));
                    if (channels.Count == 1)
                        return Result.GetSuccessedResult("", "取消渠道绑定成功!", true);
                }
                else
                {
                    if (channels.Count == 1)
                        return Result.GetFailedResult("取消渠道绑定失败!");
                }
            }
            return Result.GetSuccessedResult("", "取消渠道绑定成功!", true);
        }

        /// <summary>
        ///     更新渠道ID
        /// </summary>
        /// <returns></returns>
        private Result UpdateChannelId(HttpContext context)
        {
            var loginService = new URLoginService();
            int id = Convert.ToInt32(context.Request["id"]);
            decimal m1 = Convert.ToDecimal(context.Request["m1"]);
            string name = HttpUtility.HtmlEncode(context.Request["name"]);
            int softid = Convert.ToInt32(context.Request["softs"]);

            if (SjqdChannels_DataAccess.Instance.UpdateChannelModulus(id, name, m1) >= 0)
            {
                loginService.AddLog(string.Format("更新渠道ID,softid:{0},name:{1},channelid:{2},m1:{3}",
                                                  softid, name, id, m1));
                return Result.GetSuccessedResult("", "更新成功!", true);
            }
            else
            {
                return Result.GetFailedResult("更新失败!");
            }
        }

        /// <summary>
        /// 根据渠道商获取渠道ID
        /// </summary>
        /// <returns></returns>
        private Result GetChannelsByCustomer(HttpContext context)
        {
            int id = Convert.ToInt32(context.Request["customerid"]);
            Sjqd_ChannelCustomers m = SjqdChannelCustomers_DataAccess.Instance.GetCustomerModulus(id);
            //父级系数
            decimal m1 = m.Modulus1;
            int softid = Convert.ToInt32(context.Request["softs"]);
            var list = new List<object>();
            List<Sjqd_Channels> channelsList = SjqdChannels_DataAccess.Instance.GetChannelsByCustomID(id, softid);
            foreach (Sjqd_Channels item in channelsList)
            {
                list.Add(
                    new
                        {
                            ID = item.AutoID,
                            item.Name,
                            Platform = item.Platform.ToString(),
                            item.Modulus1
                        });
            }
            m1 = (m1 == 0m ? 1m : m1);
            var sb = new StringBuilder("{");
            string res = string.Format("\"m1\":\"{0}\",", m1.ToString());
            sb.Append(res);
            sb.Append("\"data\":");
            sb.Append(JsonConvert.SerializeObject(list));
            sb.Append("}");
            return Result.GetSuccessedResult(sb.ToString(), true);
        }

        private Result AddCateCustom(HttpContext context)
        {
            var loginService = new URLoginService();
            string name = HttpUtility.HtmlEncode(context.Request["name"]);
            int cateid = Convert.ToInt32(context.Request["cateid"]);
            int softid = Convert.ToInt32(context.Request["softs"]);
            var custom = new Sjqd_ChannelCustomers();
            //默认值
            custom.IsRealtime = 0;
            custom.Name = name;
            custom.ReportType = 0;
            custom.PID = 0;
            custom.CID = cateid;
            custom.SoftID = softid;
            custom.AddTime = DateTime.Now;
            int result = SjqdChannelCustomers_DataAccess.Instance.AddChannelCustomer(custom);
            if (result > 0)
            {
                loginService.AddLog("AddChannelCustomer",
                                    string.Format("添加渠道商(ID={0},SoftID={1},PID={2},CID={3},Name={4})", result, softid,
                                                  custom.PID, custom.CID, custom.Name));
                return Result.GetSuccessedResult(result, "保存成功！", true);
            }
            else
            {
                return Result.GetFailedResult("该名称渠道商已经存在,保存失败！");
            }
        }

        /// <summary>
        ///     根据渠道id 找出对应渠道商列表
        /// </summary>
        /// <returns></returns>
        private Result SearchcustomersByChannelid(HttpContext context)
        {
            string searchkey = HttpUtility.HtmlEncode(context.Request["searchkey"]);
            int softID = Convert.ToInt32(context.Request["softs"]);
            List<int> tempList = SjqdChannelCustomers_DataAccess.Instance.GetCustomIDsByChannelId(searchkey, softID);
            return Result.GetSuccessedResult(tempList, "获取成功！", true);
        }

        #region 关于分类的增删改

        /// <summary>
        ///     编辑添加分类
        /// </summary>
        /// <returns></returns>
        private Result EditAddCate(HttpContext context)
        {
            var loginService = new URLoginService();
            int softid = Convert.ToInt32(context.Request["softs"]);
            string catename = context.Request["catename"];
            int cateid = Convert.ToInt32(context.Request["cateid"]);
            var cate = new Sjqd_ChannelCategories();
            cate.SoftID = softid;
            string oldName = cate.Name;
            cate.Name = catename;
            cate.ID = cateid;
            cate.InDate = DateTime.Now;
            int result = cate.ID == 0
                             ? SjqdChannelCategories_DataAccess.Instance.AddChannelCategory(cate)
                             : SjqdChannelCategories_DataAccess.Instance.UpdateChannelCategory(cate);
            if (result > 0)
            {
                if (cate.ID == 0)
                    loginService.AddLog("AddChannelCategory",
                                        string.Format("添加渠道分类(SoftID={0},Name={1})", softid, cate.Name));
                else
                    loginService.AddLog("UpdateChannelCategory",
                                        string.Format("修改渠道分类(ID={0},SoftID={1},Name={2})", cate.ID, softid, cate.Name));
                return Result.GetSuccessedResult("", "保存成功！", true);
            }
            else
            {
                return Result.GetFailedResult("保存失败！");
            }
        }

        /// <summary>
        ///     获取分类
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private Result GetCate(HttpContext context)
        {
            int cateid = Convert.ToInt32(context.Request["cateid"]);

            Sjqd_ChannelCategories result = SjqdChannelCategories_DataAccess.Instance.GetChannelCategory(cateid);
            if (result.ID != 0)
            {
                return Result.GetSuccessedResult(result, "保存成功！", true);
            }
            else
            {
                return Result.GetFailedResult("保存失败！");
            }
        }

        /// <summary>
        ///     删除分类
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private Result DeleteCate(HttpContext context)
        {
            var loginService = new URLoginService();
            int cateid = Convert.ToInt32(context.Request["cateid"]);
            int softid = Convert.ToInt32(context.Request["softs"]);
            int result = SjqdChannelCategories_DataAccess.Instance.DeleteChannelCategory(cateid);
            if (result > 0)
            {
                loginService.AddLog("DeleteChannelCategory", string.Format("删除渠道分类(ID={0},SoftID={1})", cateid, softid));
                return Result.GetSuccessedResult("", "删除成功！", true);
            }
            else
            {
                return Result.GetFailedResult("删除失败!");
            }
        }

        #endregion

        
    }
}