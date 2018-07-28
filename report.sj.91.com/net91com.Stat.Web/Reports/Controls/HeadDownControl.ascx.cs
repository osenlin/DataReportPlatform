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
using Res91com.ResourceDataAccess;
using net91com.Stat.Services;

using net91com.Reports.UserRights;

namespace net91com.Stat.Web.Reports.Controls
{
    public partial class HeadDownControl : System.Web.UI.UserControl
    {
        public string AllPlats { get; set; }

        public Dictionary<object, string> CustomTypeSource { get; set; }
        public Dictionary<int, string> DateMonthList { get; set; }
         

        private bool hiddenPlat = false;

        private bool hiddenResSource = false;

        private bool hiddenCustomType = true;

        private bool hiddenTime=false;

        private bool hiddenResType = false;

        private bool hiddenResGrant = true;

        private bool isPlatSingle = true;

        private bool isResSourceSingle = true;

        private bool isCustomTypeSingle = true;

        private bool hiddenQuickTime = true;
        private bool hiddenCompareTime = true;
        /// <summary>
        /// 是否为海外版本
        /// </summary>
        private int versionType = 0;
        private bool isHasCheckedBox = false;

        /// <summary>
        /// 是否单选
        /// </summary>
        public bool IsPlatSingle
        {
            get { return isPlatSingle; }
            set { isPlatSingle = value; }
        }
        /// <summary>
        /// 是否单选
        /// </summary>
        public bool IsResSourceSingle
        {
            get { return isResSourceSingle; }
            set { isResSourceSingle = value; }
        }
        /// <summary>
        /// 是否单选
        /// </summary>
        public bool IsCustomTypeSingle
        {
            get { return isCustomTypeSingle; }

            set { isCustomTypeSingle = value; }
        }
        /// <summary>
        /// 1 海外，2 印度，0 是国内
        /// </summary>
        public int VersionType
        {
            get { return versionType; }
            set { versionType = value; }
        }

        public bool IsHasCheckedBox
        {
            get { return isHasCheckedBox; }
            set { isHasCheckedBox = value; }
        }
        /// <summary>
        /// 是否隐藏平台
        /// </summary>
        public bool HiddenPlat
        {
            get { return hiddenPlat; }
            set { hiddenPlat = value; }
        }
        /// <summary>
        /// 是否隐藏资源来源
        /// </summary>
        public bool HiddenResSource
        {
            get { return hiddenResSource; }
            set { hiddenResSource = value; }
        }
        /// <summary>
        /// 是否隐藏自定义类型
        /// </summary>
        public bool HiddenCustomType
        {
            get{return hiddenCustomType;}
            set{hiddenCustomType=value;}
        }

        /// <summary>
        /// 是否隐藏时间 
        /// </summary>
        public bool HiddenTime
        {
            get { return hiddenTime; }
            set { hiddenTime = value; }
        }

        public bool HiddenResType
        {
            get { return hiddenResType; }
            set { hiddenResType = value; }

        }
        public bool HiddenCompareTime
        {
            get { return hiddenCompareTime; }
            set { hiddenCompareTime = value; }
        }
        /// <summary>
        /// 是否隐藏快捷时间
        /// </summary>
        public bool HiddenQuickTime
        {
            get { return hiddenQuickTime; }
            set { hiddenQuickTime = value; ; }
        }

        public bool HiddenResGrant
        {
            get { return hiddenResGrant; }
            set { hiddenResGrant = value; }
        }

        public string Granted
        {
            get { return inputgrantselect.Value; }
            set { inputgrantselect.Value = value; }
        }

        public bool IsChecked
        {
            get { return checkbox.Checked; }
            set { checkbox.Checked = value; }
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


        private string resTypeWidth = "12%";

        private string platWidth = "14%";

        private string resSourceWidth = "14%";

        private string customTypeWidth = "15%";

        private string resGrantWidth = "15%";

        private string checkboxWidth="15%";

        private string timeWidth = "18%";

        private string btnWidth = "4%";

        private string compareTypeWidth = "18%";

        private List<ProjectSource> projectlist;

        /// <summary>
        /// 宽度 类似"18%"
        /// </summary>
        public string ResTypeWidth
        {
            get { return resTypeWidth; }
            set { resTypeWidth = value; }
        }
        /// <summary>
        /// 宽度 类似"18%"
        /// </summary>
        public string PlatWidth
        {
            get { return platWidth; }
            set { platWidth = value; }
        }
        /// <summary>
        /// 宽度 类似"18%"
        /// </summary>
        public string ResSourceWidth
        {
            get { return resSourceWidth; }
            set { resSourceWidth = value; }
        }
        /// <summary>
        /// 宽度 类似"18%"
        /// </summary>
        public string CustomTypeWidth
        {
            get { return customTypeWidth; }
            set { customTypeWidth = value; }
        }
        /// <summary>
        /// 宽度 类似"18%"
        /// </summary>
        public string BtnWidth
        {
            get { return btnWidth; }
            set { BtnWidth = value; }
        }
        /// <summary>
        /// 宽度 类似"18%"
        /// </summary>
        public string TimeWidth
        {
            get { return timeWidth; }
            set { timeWidth = value; }
        }

        public string ResGrantWidth
        {
            get { return resGrantWidth; }
            set { resGrantWidth = value; }
        }

        public string CheckBoxWidth
        {
            get { return checkboxWidth; }
            set { checkboxWidth = value; }
        }
        public string CompareTypeWidth
        {
            get { return compareTypeWidth; }
            set { compareTypeWidth = value; }
        }

        public List<ProjectSource> ProjectList
        {
            get { return projectlist; }
            set { projectlist = value; }
        }

        private bool isHasNoPlat = false;
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
        /// 是否首次加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public bool IsFirstLoad
        {
            get {
             return inputisfirstload.Value == "0"; }
            set {
                inputisfirstload.Value = value ? "0" : "1";
            }
        
        }

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

        public string ResType
        {
            set { inputrestypeselect.Value= value; }
            get { return inputrestypeselect.Value; }
        }

        public string PlatID
        {
            set { inputplatformselect.Value = value; }
            get { return inputplatformselect.Value; }
        }

        public string ResSource
        {
            set { inputressourceselect.Value= value; }
            get { return inputressourceselect.Value; }
        }

        public string CustomType
        {
            get { return inputcustomtypeselect.Value; }
            set { inputcustomtypeselect.Value = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
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
            IsFirstLoad = false;
        }
    }
}