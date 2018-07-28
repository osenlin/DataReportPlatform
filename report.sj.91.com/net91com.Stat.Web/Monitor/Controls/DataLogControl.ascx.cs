using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace net91com.Stat.Web.Monitor.Controls
{
    public partial class DataLogControl : System.Web.UI.UserControl
    {
        public Dictionary<string, string> LogNameSource { get; set; }
        private bool hiddenDoubleTime = false;
        private bool hiddenSingleTime = true; 
        private bool hiddenServerIp = false;
        private bool isHasNoServerIp = false;

        private string logNameWidth = "18%";

        private string serverIpWidth = "18%";

        private string timeWidth = "18%";

        private string btnWidth = "4%";

        private bool serverIpSingle = false;
        /// <summary>
        /// 日志名称的宽度
        /// </summary>
        public string LogNameWidth
        {
            get { return logNameWidth; }
            set { logNameWidth = value; }
        }
        /// <summary>
        /// 服务器Ip宽度
        /// </summary>
        public string ServerIpWidth
        {
            get { return serverIpWidth; }
            set { serverIpWidth = value; }
        }

       
        /// <summary>
        /// 时间空间宽度
        /// </summary>
        public string TimeWidth
        {
            get { return timeWidth; }
            set { timeWidth = value; }
        }
        /// <summary>
        /// 查询按钮宽度
        /// </summary>
        public string BtnWidth
        {
            get { return btnWidth; }
            set { btnWidth = value; }
        }
        /// <summary>
        /// 是否隐藏
        /// </summary>
        public bool HiddenDoubleTime
        {
            get { return hiddenDoubleTime; }
            set { hiddenDoubleTime = value; }
        }
        public bool HiddenSingleTime
        {
            get { return hiddenSingleTime; }
            set { hiddenSingleTime = value; }
        }
         
        public bool HiddenServerIp
        {
            get { return hiddenServerIp; }
            set { hiddenServerIp = value; }
        }

        public bool ServerIpSingle
        {
            get { return serverIpSingle; }
            set { serverIpSingle = value; }
        }

        public bool IsHasNoServerIp
        {
            get { return isHasNoServerIp; }
            set { isHasNoServerIp = value; }
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
                    inputbegintime.Value = DateTime.Now.AddDays(-14).ToString("yyyy-MM-dd");
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

        public string LogName
        {
            set { inputlogname.Value = value; }
            get { return inputlogname.Value; }
        }

        public string ServerIp
        {
            set { inputserveripselect.Value = value; }
            get
            {
                return inputserveripselect.Value;
            }
        }

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
            IsFirstLoad = false;
        }
    }
}