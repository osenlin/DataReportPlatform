using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using net91com.Core;
using Newtonsoft.Json;
using System.Text;
using net91com.Stat.Services.Entity;
using net91com.Stat.Web.Reports.Services;
using net91com.Stat.Services;

using net91com.Reports.UserRights;

namespace net91com.Stat.Web.Reports.Controls
{
    public partial class HeadAllControlChannel : System.Web.UI.UserControl
    {
        public List<Soft> MySupportSoft { get; set; }
        /// <summary>
        /// 自定义类型的来源
        /// </summary>
        public Dictionary<string, string> CustomTypeSource { get; set; }

        public Dictionary<int, string> CustomPeriodDic { get; set; }

        private bool hiddenSoft = false;

        private bool hiddenPlat = false;

        private bool hiddenVersion = true;

        private bool hiddenFunction = true;

        private bool hiddenTime = false;

        private bool hiddenCustomType = true;

        private bool hiddenPeriod = true;

        /// <summary>
        /// 报表版本类型
        /// </summary>
        private int versionType = 0;
        /// <summary>
        /// 用于筛选版本的，用户这边版本类型为0，下载那边版本类型为1
        /// </summary>
        public int VersionType
        {
            get { return versionType; }
            set { versionType = value; }
        }

        public Dictionary<int, string> DateMonthList { get; set; }
         
        /// <summary>
        /// 若单选就是single ，多选就是false
        /// </summary>
        private bool isSoftSingle = false;

        private bool isPlatSingle = false;

        private bool isVersionSingle = false;

        private bool isFunctionSingle = false;

        private bool showCheckBox = false;

        private bool isCustomTypeSingle = true;
        /// <summary>
        /// 宽度属性
        /// </summary>

        private string softWidth = "18%";

        private string platWidth = "18%";

        private string versionWidth = "18%";

        private string functionWidth = "18%";

        private string timeWidth = "18%";

        private string btnWidth = "4%";

        private string checkboxWidth = "6%";

        private string customTypeWidth = "18%";

        private string compareTypeWidth = "18%";

        private string periodWidth = "18%";
        /// <summary>
        /// 是否支持不区分平台，版本，功能类
        /// </summary>
        private bool isHasNoPlat = true;

        private bool isHasNoVersion = true;

        private bool isHasNoFunction = true;

        private bool hiddenCompareTime = true;

        

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
        public bool HiddenCustomType
        {
            get { return hiddenCustomType; }
            set { hiddenCustomType = value; }
        }
        public bool HiddenTime
        {
            get { return hiddenTime; }
            set { hiddenTime = value; }
        }

        public bool HiddenCompareTime
        {
            get { return hiddenCompareTime; }
            set { hiddenCompareTime = value; }
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

        public string CheckboxWidth
        {
            get { return checkboxWidth; }
            set { checkboxWidth = value; }
        }


        /// <summary>
        /// 当软件和平台都是单选的时候才能展现
        /// </summary>
        public bool HiddenChannel
        {
            set { if (value == true) Channel1.SoftId = ""; else if (isPlatSingle && isSoftSingle) { Channel1.SoftId = "0"; Channel1.Platform = "0"; } else  Channel1.SoftId = ""; }
        }
        /// <summary>
        /// 设置值
        /// </summary>
        public DateTime BeginTime
        {
            set
            {
                inputbegintime.Value = value.ToString("yyyy-MM-dd");
                fromtime.Value = value.ToString("yyyy-MM-dd");
            }
            get
            {
                if (string.IsNullOrEmpty(inputbegintime.Value))
                {
                    inputbegintime.Value = DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd");
                    fromtime.Value = inputbegintime.Value;
                }
                return Convert.ToDateTime(inputbegintime.Value);
            }
        }

        public DateTime EndTime
        {

            set
            {
                inputendtime.Value = value.ToString("yyyy-MM-dd");
                totime.Value = value.ToString("yyyy-MM-dd");
            }
            get
            {
                if (string.IsNullOrEmpty(inputendtime.Value))
                {
                    inputendtime.Value = DateTime.Now.ToString("yyyy-MM-dd");
                    totime.Value = inputendtime.Value;
                }
                return Convert.ToDateTime(inputendtime.Value);
            }
        }

        public DateTime CompareBeginTime
        {
            get
            {
                if (string.IsNullOrEmpty(txtCompareTime.Value))
                {
                    return DateTime.MinValue;
                }
                else
                    return Convert.ToDateTime(txtCompareTime.Value);
            }
         
        }

        public int CompareTimeType
        {
            get { return Convert.ToInt16(inputcomparetimetype.Value); }
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
        /// 是否单选
        /// </summary>
        public bool IsCustomTypeSingle
        {
            get { return isCustomTypeSingle; }

            set { isCustomTypeSingle = value; }
        }
        public bool HiddenPeriod
        {
            get { return hiddenPeriod; }
            set { hiddenPeriod = value; }
        }

        public string PeriodWidth
        {
            get { return periodWidth; }
            set { periodWidth = value; }
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
        public string VersionWidth
        {
            get { return versionWidth; }
            set { VersionWidth = value; }
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
        public string CustomTypeWidth
        {
            get { return customTypeWidth; }
            set { customTypeWidth = value; }
        }

        public string CompareTypeWidth
        {
            get { return compareTypeWidth; }
            set { compareTypeWidth = value; }
        }

        /// <summary>
        /// 是否有不区分平台
        /// </summary>
        public bool IsHasNoPlat
        {
            get { return isHasNoPlat; }
            set { isHasNoPlat = value; }
        }
        /// <summary>
        /// 是否有不区分版本
        /// </summary>
        public bool IsHasNoVersion
        {
            get { return isHasNoVersion; }
            set { isHasNoVersion = value; }
        }
        /// <summary>
        /// 是否有不区分功能
        /// </summary>
        public bool IsHasNoFunction
        {
            get { return isHasNoFunction; }
            set { isHasNoFunction = value; }
        }

        /// <summary>
        /// 是否使用新版版本信息
        /// </summary>
        public bool NewVersionInfo = false;

        public string DataSource
        {

            get
            {
                if (MySupportSoft == null) return "[]";
                else
                    return new URLoginService().GetAvailableSoftsJson();
            }

        }
        /// <summary>
        /// 返回控件的channel1
        /// </summary>
        public ChanelForAll Channel1
        {
            get { return Channels; }
        }
        /// <summary>
        /// 所有平台
        /// </summary>
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
        /// <summary>
        /// 自定义类型值
        /// </summary>
        public string CustomType
        {
            get { return inputcustomtypeselect.Value; }
            set { inputcustomtypeselect.Value = value; }
        }

        public string SinglePeriod
        {
            get { return inputperiod.Value; }
            set { inputperiod.Value = value; }
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            IsFirstLoad = false;
            if (CustomPeriodDic == null)
            {
                CustomPeriodDic = new Dictionary<int, string>();
                CustomPeriodDic.Add(0, "全部");
                CustomPeriodDic.Add(6, "最近一周");
                CustomPeriodDic.Add(7, "最近两周");
                CustomPeriodDic.Add(8, "最近一个月");
                CustomPeriodDic.Add(9, "最近三个月"); 
            }
            StringBuilder sb = new StringBuilder("[");
            List<MobileOption> mobilelist = UtilityService.GetMobileList(true);

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
                var m = nowTemp - i;
                if (m <= 0)
                {
                    m = m + 12;
                }
                DateMonthList.Add(m - 1, m + "月");
            }
            

        }
    }
}