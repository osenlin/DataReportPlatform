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
using net91com.Stat.Services;

using net91com.Reports.UserRights;

namespace net91com.Stat.Web.Reports.Controls
{
    public partial class HeadAllControl : System.Web.UI.UserControl
    {
        protected URLoginService loginService = new URLoginService();

        public List<Soft> MySupportSoft { get; set; }

        public Dictionary<string, string> CustomTypeSource { get; set; }

        public Dictionary<int, string> DateMonthList { get; set; }

        private bool hiddenSoft=false;

        private bool hiddenPlat = false;

        private bool hiddenVersion = true;

        private bool hiddenFunction = true;

        private bool hiddenTime = false;

        private bool hiddenSingleTime = true;

        private bool showCheckBox = false;

        private bool hiddenPeriod = true;

        private bool hiddenCustomType = true;
        /// <summary>
        /// 报表版本类型
        /// </summary>
        private int versionType = 0;

        /// <summary>
        /// 若单选就是single ，多选就是false
        /// </summary>
        private bool isSoftSingle = false;

        private bool isPlatSingle = false;

        private bool isVersionSingle = false;

        private bool isFunctionSingle = false;

        private bool isCustomTypeSingle = true;
        /// <summary>
        /// 宽度属性
        /// </summary>

        private string softWidth="18%";

        private string platWidth = "18%";

        private string versionWidth = "18%";

        private string functionWidth = "18%";

        private string timeWidth = "18%";

        private string btnWidth = "4%";

        private string checkboxWidth = "6%";

        private string customTypeWidth = "18%";

        private string periodWidth = "18%";
        /// <summary>
        /// 用于筛选版本的，用户这边版本类型为0，下载那边版本类型为1
        /// </summary>
        public int VersionType
        {
            get { return versionType; }
            set { versionType = value; }
        }
        /// <summary>
        /// 是否支持不区分平台，版本，功能类
        /// </summary>
        private bool isHasNoPlat = true;

        private bool isHasNoVersion = true;

        private bool isHasNoFunction = true;

       

        ///不区分版本是否和其他版本选项互斥
        private bool isBanbenHuChi = false;

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

        public bool HiddenVersion
        {
            get { return hiddenVersion; }
            set { hiddenVersion = value; } 
        }

        public bool HiddenFunction
        {
            get { return hiddenFunction; }
            set { hiddenFunction = value; }
        }

        public bool HiddenTime
        {
            get { return hiddenTime; }
            set { hiddenTime = value; }
        }
        public bool HiddenSingleTime
        {
            get { return hiddenSingleTime; }
            set { hiddenSingleTime = value; }
        }

        public bool HiddenPeriod
        {
            get { return hiddenPeriod; }
            set { hiddenPeriod = value; }
        }

        public bool HiddenCustomType
        {
            get { return hiddenCustomType; }
            set { hiddenCustomType = value; }
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
        /// <summary>
        /// 自定义类型值
        /// </summary>
        public string CustomType
        {
            get { return inputcustomtypeselect.Value; }
            set { inputcustomtypeselect.Value = value; }
        }
        /// <summary>
        /// 是否版本互斥
        /// </summary>
        public bool IsBanbenHuChi
        {
            get { return isBanbenHuChi; }
            set { isBanbenHuChi = value; }
            
        }

        /// <summary>
        /// 设置值
        /// </summary>
        public DateTime BeginTime
        {
            set { inputbegintime.Value = value.ToString("yyyy-MM-dd");
                    fromtime.Value = value.ToString("yyyy-MM-dd");
            }
            get {
                if (string.IsNullOrEmpty(inputbegintime.Value))
                {
                    inputbegintime.Value = DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd");
                    fromtime.Value = inputbegintime.Value;
                }
                return Convert.ToDateTime( inputbegintime.Value);
            }
        }

        public DateTime EndTime
        {

            set {
                 inputendtime.Value = value.ToString("yyyy-MM-dd");
                 totime.Value = value.ToString("yyyy-MM-dd");    
            }
            get
            {
                if (string.IsNullOrEmpty(inputendtime.Value))
                {
                    inputendtime.Value = DateTime.Now.ToString("yyyy-MM-dd");
                    totime.Value= inputendtime.Value;
                }
                return Convert.ToDateTime(inputendtime.Value);
            }
        }

        public DateTime SingleTime
        {
            set
            {
                inputsingletime.Value = value.ToString("yyyy-MM-dd");
                singlefromtime.Value = value.ToString("yyyy-MM-dd");
            }
            get
            {
                if (string.IsNullOrEmpty(inputsingletime.Value))
                {
                    inputsingletime.Value = DateTime.Now.ToString("yyyy-MM-dd");
                    singlefromtime.Value = inputsingletime.Value;
                }
                return Convert.ToDateTime(inputsingletime.Value);
            }
        }
        /// <summary>
        /// 是否单选
        /// </summary>
        public bool IsCustomTypeSingle
        {
            get { return isCustomTypeSingle; }

            set { isCustomTypeSingle = value; }
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

        public string VersionID
        {
            set { inputversionselect.Value = value; }
            get { return inputversionselect.Value; }
        }

        public string FunctionID
        {
            get { return inputfunctionselect.Value; }
            set { inputfunctionselect.Value = value; }
        }

        public string SinglePeriod
        {
            get { return inputperiod.Value; }
            set { inputperiod.Value = value; }
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
        public bool IsVersionSingle
        {
            get { return isVersionSingle; }
            set { isVersionSingle = value; }
        }
        public bool IsFunctionSingle
        {
            get { return isFunctionSingle; }
            set { isFunctionSingle = value; }
        }
        /// <summary>
        /// 设置宽度
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public string SoftWidth {
            get { return softWidth; }
            set { softWidth = value; }
        }
        public string PlatWidth
        {
            get { return platWidth; }
            set { platWidth = value; }
        }
        public string VersionWidth
        {
            get { return versionWidth; }
            set { VersionWidth = value; }
        }
        public string CustomTypeWidth
        {
            get { return customTypeWidth; }
            set { customTypeWidth = value; }
        }

        public string FunctionWidth
        {
            get { return functionWidth; }
            set { functionWidth = value; }
        }
        public string BtnWidth
        {
            get { return btnWidth; }
            set { BtnWidth = value; }
        }
        public string TimeWidth
        {
            get { return timeWidth; }
            set { timeWidth = value; }
        }
        public string CheckboxWidth
        {
            get { return checkboxWidth; }
            set { checkboxWidth = value; }
        }

        public string PeriodWidth
        {
            get { return periodWidth; }
            set { periodWidth = value; }
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
        /// <summary>
        /// 是否有不区分版本
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param> 
        public bool IsHasNoVersion
        {
            get { return isHasNoVersion; }
            set { isHasNoVersion = value; }
        }
        /// <summary>
        /// 是否有不区分功能
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param> 
        public bool IsHasNoFunction
        {
            get { return isHasNoFunction; }
            set { isHasNoFunction = value; }
        }

        public string AllPlats { get; set; }

        /// <summary>
        /// 是否首次加载
        /// </summary>
        public bool IsFirstLoad {
            get {
                
                return inputisfirstload.Value == "0"; }
            set
            {
                inputisfirstload.Value = value ? "0" : "1";
                
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            StringBuilder sb = new StringBuilder("[");
            var array=Enum.GetValues(typeof(MobileOption));
            List<MobileOption> mobilelist =UtilityService.GetMobileList(true);
             
            foreach (MobileOption item in mobilelist)
            {
                sb.Append("{\"id\":");
                sb.AppendFormat("\"{0}\",", (int)item);
                sb.Append("\"name\":");
                sb.AppendFormat("\"{0}\"", (int)item >= 252 && (int)item <= 254 ? "PC" + ((int)item).ToString() : item.ToString());
                sb.Append("},");
            }
            
            AllPlats = sb.ToString().TrimEnd(',') + "]";
            DateMonthList = new Dictionary<int, string>();
            int nowTemp = DateTime.Now.Month;
            DateMonthList.Add(-1, "月份");
            for (int i = 0; i < 12; i++)
            {
                var mm = nowTemp - i;
                if (mm <= 0)
                {
                    mm = mm + 12;
                }
                DateMonthList.Add(mm - 1, mm + "月");
            }
            IsFirstLoad = false;
        }
    }
}