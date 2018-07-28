using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using net91com.Core;
using net91com.Core.Util;
using net91com.Core.Extensions;
using net91com.Reports.UserRights;
using Ext.Net;
using Ext.Net.Utilities; 

namespace net91com.Stat.Web.UserRights
{
    public partial class ProjectSourceManage : URBasePage 
    {
        protected URBasicInfoService biService = new URBasicInfoService();

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void MyData_Refresh(object sender, StoreRefreshDataEventArgs e)
        {
            Store4.DataSource = biService.GetProjectSources().Select(p => new SimpleProjectource()
                {
                    ProjectSourceID = p.ProjectSourceID,
                    AddTime = p.AddTime,
                    Name = p.Name,
                    OnlyInternal = p.OnlyInternal,
                    ProjectSourceType = p.ProjectSourceType.ToString(),
                    SortIndex = p.SortIndex,
                    Status = ((int)p.Status)>0?true:false,
                    SoftID = p.SoftID 
                }).ToList();
            Store4.DataBind();
        }

        protected void OnSave(object sender, DirectEventArgs e)
        {
            try
            {
                ProjectSource obj = new ProjectSource();
                obj.ProjectSourceID = Convert.ToInt32(txtSourceID.Text);
                obj.Name = txtSourceName.Text;
                obj.SortIndex = Convert.ToInt32(txtNumSort.Text);
                obj.Status = StatusOptions.Valid;
                obj.OnlyInternal = Convert.ToInt32(onlyinternalselect.SelectedItem.Value) > 0 ? true : false;
                obj.ProjectSourceType = (ProjectSourceTypeOptions)Convert.ToInt32(sourcetypeselect.SelectedItem.Value);
                obj.SoftID = Convert.ToInt32(txtSoftID.Text.Trim());
                biService.AddPrjectSource(obj);
                e.ExtraParamsResponse.Add(new Ext.Net.Parameter("success", "1", false));
                X.Msg.Alert("消息", "添加成功").Show();
            }
            catch (ToUserException ex)
            {
                e.ExtraParamsResponse.Add(new Ext.Net.Parameter("success", "0", false));
                X.Msg.Alert("消息", ex.Message).Show();
            }
        }

        protected void HandleChanges(object sender, BeforeStoreChangedEventArgs e)
        {
            ChangeRecords<SimpleProjectource> list = e.DataHandler.ObjectData<SimpleProjectource>();
            foreach (SimpleProjectource updated1 in list.Updated)
            {
                var updated = ConvertToProject(updated1);
                biService.UpdateProjectSource(updated);
                if (Store4.UseIdConfirmation)
                {
                    e.ConfirmationList.ConfirmRecord(updated.ProjectSourceID.ToString());
                }
            }
            X.Msg.Alert("消息", "保存修改成功").Show();
            e.Cancel = true;
        }

        protected void OnCommand(object sender, DirectEventArgs e)
        {
            if (e.ExtraParams["Command"].ToString() == "delete")
            {
                int id = Convert.ToInt32(e.ExtraParams["SourceID"]);
                int softId = Convert.ToInt32(e.ExtraParams["SoftID"]);
                biService.DeleteProjectSource(id, softId);
                e.ExtraParamsResponse.Add(new Ext.Net.Parameter("success", "1", false));
                X.Msg.Alert("消息", "删除成功").Show();
            }
        }
        /// <summary>
        /// 类转换
        /// </summary>
        /// <param name="project"></param>
        /// <returns></returns>
        public ProjectSource ConvertToProject(SimpleProjectource project)
        {
            ProjectSource ps=new ProjectSource();
            ps.Name = project.Name;
            ps.OnlyInternal = project.OnlyInternal;
            ps.ProjectSourceID = project.ProjectSourceID;
            ps.ProjectSourceType = project.ProjectSourceType.ToEnum<ProjectSourceTypeOptions>(ProjectSourceTypeOptions.Domestic);
            ps.SortIndex = project.SortIndex;
            ps.Status = project.Status ? StatusOptions.Valid : StatusOptions.Invalid;
            ps.SoftID = project.SoftID;
            return ps;
        }
    }

    public class SimpleProjectource
    {
        /// <summary>
        /// 项目来源ID
        /// </summary>
        public int ProjectSourceID { get; set; }

        /// <summary>
        /// 项目来源名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 软件ID
        /// </summary>
        public int SoftID { get; set; }

        /// <summary>
        /// 是否允许外部请求
        /// </summary>
        public bool OnlyInternal { get; set; }

        /// <summary>
        /// 排序值
        /// </summary>
        public int SortIndex { get; set; }

        /// <summary>
        /// 权限状态(启用或禁用)
        /// </summary>
        public bool Status { get; set; }

        /// <summary>
        /// 添加时间
        /// </summary>
        public DateTime AddTime { get; set; }

        /// <summary>
        /// 项目来源类型
        /// </summary>
        public string ProjectSourceType { get; set; }
    }
}