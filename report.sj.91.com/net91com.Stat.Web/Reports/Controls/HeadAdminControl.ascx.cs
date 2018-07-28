using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using net91com.Stat.Web.Reports.Services;
using System.Text;
using net91com.Core;
using net91com.Stat.Services.Entity;
using net91com.Core.Extensions;

using net91com.Reports.UserRights;

namespace net91com.Stat.Web.Reports.Controls
{
    public partial class HeadFuncControl : System.Web.UI.UserControl
    {
        public List<Soft> MySupportSoft { get; set; }

        private bool hiddenSoft = false;

        private bool hiddenPlat = false;

        private bool showCheckBox = false;

        private bool showAddBtn = true;

        private string addBtnText = "添加";

        private bool showDeleteBtn = true;

        private string deleteBtnText = "删除";

        /// <summary>
        /// 若单选就是single ，多选就是false
        /// </summary>
        private bool isSoftSingle = false;

        private bool isPlatSingle = false;
        /// <summary>
        /// 宽度属性
        /// </summary>

        private string softWidth = "18%";

        private string platWidth = "18%";

       

        private string btnWidth = "10%";

        private string checkboxWidth = "6%";

       
      
        /// <summary>
        /// 是否支持不区分平台，版本，功能类
        /// </summary>
        private bool isHasNoPlat = true;

      
       
        /// <summary>
        /// 设置是否隐藏
        /// </summary>
        public bool HiddenSoft
        {
            set { hiddenSoft = value; }
            get { return hiddenSoft; }
        }

        public bool HiddenPlat
        {
            get { return hiddenPlat; }
            set { hiddenPlat = value; }
        }
        /// <summary>
        /// 展示删除按钮
        /// </summary>
        public bool ShowDeleteBtn
        {
            get { return showDeleteBtn; }
            set { showDeleteBtn = value; }
        }
        /// <summary>
        /// 是否展示添加按钮
        /// </summary>
        public bool ShowAddBtn
        {
            get { return showAddBtn; }
            set { showAddBtn = value; }
        }
 
        ///是否有勾选框
        public bool ShowCheckBox
        {
            get { return showCheckBox; }
            set { showCheckBox = value; }
        }

        ///checkbox 里面的内容是什么
        public string CheckBoxText
        {
            get { return checkbox.Text; }
            set { checkbox.Text = value; }
        }
        /// <summary>
        /// 单选框是否选中
        /// </summary>
        public bool isChecked
        {
            get { return checkbox.Checked; }
            set { checkbox.Checked = value; }
        }

        public string DeleteBtnText
        {
            get { return deleteBtnText; }
            set { deleteBtnText = value; }
        }

        public string AddBtnText
        {
            get { return addBtnText; }
            set { addBtnText = value; }
        }
      

       
        public string SoftID
        {
            set { inputsoftselect.Value = value; }
            get { return inputsoftselect.Value; }
        }

        public string PlatID
        {
            set { inputplatformselect.Value = value; }
            get { return inputplatformselect.Value; }
        }

        /// <summary>
        /// 设置是否单选
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public bool IsSoftSingle
        {
            get { return isSoftSingle; }
            set { isSoftSingle = value; }
        }
        public bool IsPlatSingle
        {
            get { return isPlatSingle; }
            set { isPlatSingle = value; }
        }
       
        /// <summary>
        /// 设置宽度
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public string SoftWidth
        {
            get { return softWidth; }
            set { softWidth = value; }
        }
        public string PlatWidth
        {
            get { return platWidth; }
            set { platWidth = value; }
        }
       
        public string BtnWidth
        {
            get { return btnWidth; }
            set { BtnWidth = value; }
        }
       
        public string CheckboxWidth
        {
            get { return checkboxWidth; }
            set { checkboxWidth = value; }
        }

        /// <summary>
        /// 是否有不区分平台
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param> 

        public bool IsHasNoPlat
        {
            get { return isHasNoPlat; }
            set { isHasNoPlat = value; }
        }
       

        public string DataSource
        {

            get
            {
                if (MySupportSoft == null) return "[]";
                else
                    return new URLoginService().GetAvailableSoftsJson();
            }

        }
        public string AllPlats { get; set; }

        /// <summary>
        /// 是否首次加载
        /// </summary>
        public bool IsFirstLoad
        {
            get
            {

                return inputisfirstload.Value == "0";
            }
            set
            {
                inputisfirstload.Value = value ? "0" : "1";

            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            StringBuilder sb = new StringBuilder("[");
            foreach (MobileOption item in Enum.GetValues(typeof(MobileOption)))
            {
                if (item != MobileOption.None && item != MobileOption.All)
                {
                    sb.Append("{\"id\":");
                    sb.AppendFormat("\"{0}\",", (int)item);
                    sb.Append("\"name\":");
                    sb.AppendFormat("\"{0}\"", (int)item >= 252 && (int)item <= 254 ? "PC" + ((int)item).ToString() : item.ToString());
                    sb.Append("},");
                }
            }
            AllPlats = sb.ToString().TrimEnd(',') + "]";
            IsFirstLoad = false;
        }
    }
}