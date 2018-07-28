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
using net91com.Stat.Services;
namespace net91com.Stat.Web.Reports.Controls
{
    public partial class HeadDownTop100 : System.Web.UI.UserControl
    {
        public string AllPlats { get; set; }

        public Dictionary<int, string> CustomTypeSource { get; set; }

        private bool isHasNoBigCate =false;

        private bool isHasNoSmallCate = false;

        private bool isHasNoPlat = false;

        private bool isPlatSingle = true;

        private bool isHiddenGrant = false;

        private bool hiddenCustomType = true;

        private bool isCustomTypeSingle = true;
         
   
        private string resTypeWidth = "18%";
       
        private string timeWidth = "18%";
        private string btnWidth = "4%";
        private string customTypeWidth="14%";
        private string parentCateWidth = "13%";
        private string childCateWidth = "13%";
        private string resGrantWidth = "13%";
        private string platWidth = "13%";

        private int versionType = 0;
       
        /// <summary>
        /// 宽度 类似"18%"
        /// </summary>
        public string ResTypeWidth
        {
            get { return resTypeWidth; }
            set { resTypeWidth = value; }
        }
        /// <summary>
        /// 不区分大分类
        /// </summary>
        public bool IsHasNoBigCate
        {
            get { return  isHasNoBigCate; }
            set { isHasNoBigCate = value; }
        }
        /// <summary>
        /// 不区分小分类
        /// </summary>
        public bool IsHasNoSmallCate
        {
            get { return isHasNoSmallCate; }
            set { isHasNoSmallCate = value; }
        }

        public bool IsHasNoPlat
        {
            set { isHasNoPlat = value; }
            get { return isHasNoPlat; }
        }

        public bool HiddenCustomType
        {
            get { return hiddenCustomType; }
            set { hiddenCustomType = value; }
        }

        /// <summary>
        /// 平台单选
        /// </summary>
        public bool IsPlatSingle
        {
            get { return isPlatSingle; }
            set { isPlatSingle = value; }
        }

        public bool IsHiddenGrant
        {
            get { return isHiddenGrant; }
            set { isHiddenGrant = value; }
        }

        public int VersionType
        {
            get { return versionType; }
            set { versionType = value; }
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

       
        public string CateParentWidth
        {
            get { return parentCateWidth; }
            set { parentCateWidth = value; }
        }

        public string CateChildWidth
        {
            get { return childCateWidth; }
            set { childCateWidth = value; }
        }

        public string ResGrantWidth
        {
            get { return resGrantWidth; }
            set { resGrantWidth = value; }
        }

        public string PlatWidth
        {
            get { return platWidth; }
            set { platWidth = value; }
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
        /// 是否首次加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
        /// 是否单选
        /// </summary>
        public bool IsCustomTypeSingle
        {
            get { return isCustomTypeSingle; }

            set { isCustomTypeSingle = value; }
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
            set { inputrestypeselect.Value = value; }
            get { return inputrestypeselect.Value; }
        }

       
     
      
        public string CateParent
        {
            set { inputcateparentselect.Value = value; }
            get { return inputcateparentselect.Value; }
        }

        public string CateChild
        {
            get { return inputcatechildselect.Value; }
            set { inputcatechildselect.Value = value; }
        }

        public string Granted
        {
            get { return inputgrantselect.Value ;  }
            set { inputgrantselect.Value = value; }
        }

        public string Platform
        {
            get { return inputplatformselect.Value; }
            set { inputplatformselect.Value = value; }
        }
        /// <summary>
        /// 自定义类型值
        /// </summary>
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
            IsFirstLoad = false;


        }
    }
}