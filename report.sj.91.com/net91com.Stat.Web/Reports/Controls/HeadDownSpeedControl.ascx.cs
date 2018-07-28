using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace net91com.Stat.Web.Reports.Controls
{
    public partial class HeadDownSpeedControl : System.Web.UI.UserControl
    {
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


        private bool hiddenArea = false;
        public bool HiddenArea
        {
            get { return hiddenArea; }
            set { hiddenArea = value; }
        }
        private bool hiddenPrivider = false;
        public bool HiddenPrivider
        {
            get { return hiddenPrivider; }
            set { hiddenPrivider = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

            IsFirstLoad = false;
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

        public string SoftID
        {
            get { return inputsoftselect.Value; }
            set { inputsoftselect.Value = value; }
        }

        public string AreaName
        {
            get { return inputareaselect.Value; }
            set { inputareaselect.Value = value; }
        }

        public string ServiceProvider
        {
            get { return inputproviderselect.Value; }
            set { inputproviderselect.Value = value; }
        }
        
    }
}