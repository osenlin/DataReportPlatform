using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using net91com.Stat.Services.sjqd.Entity;
using System.Text;
using net91com.Stat.Services.sjqd;
using net91com.Stat.Services.Entity;
using net91com.Stat.Services;
using net91com.Core.Extensions;
using net91com.Core;
using System.Text.RegularExpressions;

using net91com.Reports.UserRights;

namespace net91com.Stat.Web.Reports.Controls
{
    public partial class ChanelForAll : System.Web.UI.UserControl
    {
        #region 属性
        protected string title = string.Empty;
        private string _selectedValue = string.Empty;

        private List<string> listSelectValue = new List<string>();

        private List<ChannelTypeOptions> listSelectType = new List<ChannelTypeOptions>();

        private List<string> listTypes = new List<string>();

        private List<SelectChannelValue> channelValues;

        private URLoginService loginService = new URLoginService();

        /// <summary>
        /// 获取选中项目list列表
        /// </summary>
        public List<SelectChannelValue> ChannelValues
        {
            get {
               
                if (channelValues == null)
                {
                    channelValues = new List<SelectChannelValue>();
                    string[] strvalues = SelectedValue.Split(',');
                    string[] strcates = SelectedCate.Split(',');
                    string[] strtxt = SelectedText.Split(',');
                    if (SelectedValue == "" || strcates.Length != strtxt.Length || strvalues.Length != strcates.Length)
                    {
                        channelValues= new List<SelectChannelValue>();
                    }
                    else
                    {
                        channelValues = new List<SelectChannelValue>();
                        for (int i = 0; i < strvalues.Length; i++)
                        {
                            var tempChannel=strcates[i].ToEnum<ChannelTypeOptions>(ChannelTypeOptions.Category);
                           ///当选择的不是渠道 
                            string[] plats = Platform.Split(',');
                            for (int j = 0; j < plats.Length; j++)
                            {
                                SelectChannelValue chanel = new SelectChannelValue(Convert.ToInt32(strvalues[i]), tempChannel,Regex.Replace(strtxt[i],"\\([\\S\\s]+\\)",""),
                                    Convert.ToInt32(plats[j]));
                                channelValues.Add(chanel);
                            }
                          
                        } 

                    }
                    
                }
                return channelValues;
               

            
            }
        }

        ///是否支持不区分平台
        private bool isHasNoPlat = false;
        public bool IsHasNoPlat
        {
            set { isHasNoPlat = value; }
            get { return isHasNoPlat; }
        }

        /// <summary>
        /// 选择值
        /// </summary>
        public string SelectedValue
        {
            get { if (string.IsNullOrEmpty(_selectedValue)) _selectedValue = hidcheckedValue.Value; return _selectedValue; }
            set { hidcheckedValue.Value = _selectedValue = value; }
        }
        private string _selectedText = string.Empty;

        private string _selectedCate = string.Empty;
        /// <summary>
        /// 选择的文本值
        /// </summary>
        public string SelectedText
        {
            get { if (string.IsNullOrEmpty(_selectedText)) _selectedText = hidcheckedText.Value; return _selectedText; }
            set { hidcheckedText.Value = _selectedText = value; }
        }

        public string SelectedCate
        {
            get { if (string.IsNullOrEmpty(_selectedCate)) _selectedCate = hidchanneltype.Value; return _selectedCate; }
            set { hidchanneltype.Value = _selectedCate = value; }
        }

        private string _visible = "display:none";

        protected string Visible1
        {
            get { return _visible; }
            set { _visible = value; }
        }

        /// <summary>
        /// 周期要检查
        /// </summary>
        private bool _PeriodCheck = true;
        public bool PeriodCheck
        {
            get { return _PeriodCheck; }
            set { _PeriodCheck = value; }
        }

        //private ChannelTypeOptions _selectchanneltype = ChannelTypeOptions.Category;

        ///// <summary>
        ///// 选中值的渠道类型
        ///// </summary>
        //public ChannelTypeOptions SelectChannelType
        //{
        //    get { _selectchanneltype = hidchanneltype.Value.ToEnum<ChannelTypeOptions>(ChannelTypeOptions.Category); return _selectchanneltype; }
        //    set { _selectchanneltype = value; hidchanneltype.Value = value.ToString(); }
        //}
        private string _width = "15%";

        public string Width
        {
            get { return _width; }
            set { _width = value; }
        }


        private int _maxCheckNumber = 5;
        /// <summary>
        /// 默认最大选择值
        /// </summary>
        public int MaxCheckNumber
        {
            get { return _maxCheckNumber; }
            set { _maxCheckNumber = value; }
        }
        private string _parentId = string.Empty;
        /// <summary>
        /// 父级ID
        /// </summary>
        public string ParentId
        {
            get { if (string.IsNullOrEmpty(_parentId)) _parentId = hidcheckedpid.Value; return _parentId; }
            set { hidcheckedpid.Value = _parentId = value; }
        }
        

        private string _softId = "";
        /// <summary>
        /// 资源ID
        /// </summary>
        public string SoftId
        {
            get { if (string.IsNullOrEmpty(_softId))_softId = hidsoftId.Value; return _softId; }
            set { hidsoftId.Value = _softId = value; }
        }
        private string _platform = string.Empty;
        /// <summary>
        /// 平台
        /// </summary>
        public string Platform
        {
            get { if (string.IsNullOrEmpty(_platform))_platform = hidplatform.Value; return _platform; }
            set { hidplatform.Value = _platform = value; }
        }

        protected string _treeSetting = @"{
                            view: {
                                showIcon: false,
                                fontCss: getFont
                            },
                            callback:{
                                    beforeCheck:TreeBeforeCheck,
                                    onCheck:TreeCheckNodeValue
                                   
                            } ,
                            check: {
                                enable: true,
                                chkboxType: { 'Y' : '', 'N' : '' }
                            }, data: {
                                simpleData: {
                                    enable: true
                                }
                            }};";

        private bool _HasLoadData = true;
        /// <summary>
        /// 是否重新加载Tree数据
        /// </summary>
        public bool HasLoadData
        {
            get { return _HasLoadData; }
            set { _HasLoadData = value; }
        }
        private bool _HasLoadTree = true;
        /// <summary>
        /// 是否重新加载Tree控件
        /// </summary>
        public bool HasLoadTree
        {
            get { return _HasLoadTree; }
            set { _HasLoadTree = value; }
        }
        private bool _HasPeriod = true;
        /// <summary>
        /// 当前周期是否可用
        /// </summary>
        public bool HasPeriod
        {
            get { return _HasPeriod; }
            set { _HasPeriod = value; }
        }

        #endregion
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(SoftId))
            {
                Visible1 = "";
                List<Soft> list = loginService.AvailableSofts;
                string softids = string.Empty;
                if (list != null)
                {
                    list.ForEach(soft =>
                    {
                        softids += soft.ID + ",";
                    });
                    softids = softids.TrimEnd(',');
                }
                //ResultDic = new ChannelCustomerService(string.IsNullOrEmpty(SoftId) ? softids : SoftId).GetChannelTreeData(Platform);
            }
            title = string.IsNullOrEmpty(SelectedText) ? "选择渠道" : SelectedText.Length > 9 ? SelectedText.Substring(0, 9) + "..." : SelectedText;
            if (!IsPostBack)
            {
                hidcheckedpid.Value = ParentId;
                hidcheckedText.Value = SelectedText;
                hidcheckedValue.Value = SelectedValue;
            } 
        }
    }
}