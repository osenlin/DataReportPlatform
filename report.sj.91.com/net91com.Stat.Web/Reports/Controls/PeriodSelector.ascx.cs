using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using net91com.Stat.Services.Entity;

namespace net91com.Stat.Web.Reports.Controls
{
    public partial class PeriodSelector : System.Web.UI.UserControl
    {
        private net91com.Stat.Core.PeriodOptions defaultPeriod;
        
        /// <summary>
        /// 可支持的周期列表
        /// </summary>
        protected List<net91com.Stat.Core.PeriodOptions> PeriodList = new List<net91com.Stat.Core.PeriodOptions>();

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// 添加可支持的周期
        /// </summary>
        public void AddPeriods(net91com.Stat.Core.PeriodOptions[] periods, net91com.Stat.Core.PeriodOptions defaultPeriod)
        {
            PeriodList.AddRange(periods);
            this.defaultPeriod = defaultPeriod;
        }

        /// <summary>
        /// 获取或设置当前选择的周期
        /// </summary>
        public net91com.Stat.Core.PeriodOptions SelectedPeriod
        {
            get
            {
                return !string.IsNullOrEmpty(Request["hPeriod"])
                    ? (net91com.Stat.Core.PeriodOptions)int.Parse(Request["hPeriod"])
                    : defaultPeriod;
            }
        }
    }
}