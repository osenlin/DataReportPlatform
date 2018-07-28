<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="HeadDownSpeedControl.ascx.cs"
    Inherits="net91com.Stat.Web.Reports.Controls.HeadDownSpeedControl" %>
<link href="../../css/headcss/head.css?rd=<%= net91com.Stat.Web.Reports.Services.Utility.CssVersion %>"
    rel="stylesheet" type="text/css" />
<link href="../../css/headcss/jquery-ui.css" rel="stylesheet" type="text/css" />
<link href="../../css/headcss/style.css?rd=<%= net91com.Stat.Web.Reports.Services.Utility.CssVersion %>"
    rel="stylesheet" type="text/css" />
<link href="../../css/headcss/jquery.multiselect.css?rd=<%= net91com.Stat.Web.Reports.Services.Utility.CssVersion %>"
    rel="stylesheet" type="text/css" />
<link href="../../css/headcss/jquery.multiselect.filter.css" rel="stylesheet" type="text/css" />
<script src="../../Scripts/HeadScript/jquery-ui.min.js" type="text/javascript"></script>
<script src="../../Scripts/HeadScript/jquery.multiselect.js?rd=<%= net91com.Stat.Web.Reports.Services.Utility.JsVersion %>"
    type="text/javascript" charset="GBK"></script>
<script src="../../Scripts/HeadScript/jquery.multiselect.filter.js?rd=<%= net91com.Stat.Web.Reports.Services.Utility.JsVersion %>"
    type="text/javascript" charset="GBK"></script>
<script src="../../My97DatePicker/WdatePicker.js" type="text/javascript"></script>
<div class="head">
    <div class="divitem" style="width: 20%">
        <select id="myselectsoft" style="display: none" size="12"  >
            <option value="9" selected="selected">手机助手</option>
            <option value="2">百宝箱</option>
        </select>
    </div>
    <div class="divitem" style="width: 20%">
        <select id="myselectarea" style="display: none" size="12" >
            <option value="全国" selected="selected">全国</option>
            <option value="安徽">安徽</option>
            <option value="澳门">澳门</option>
            <option value="北京">北京</option>
            <option value="福建">福建</option>
            <option value="甘肃">甘肃</option>
            <option value="广东">广东</option>
            <option value="广西">广西</option>
            <option value="贵州">贵州</option>
            <option value="海南">海南</option>
            <option value="河北">河北</option>
            <option value="河南">河南</option>
            <option value="黑龙江">黑龙江</option>
            <option value="湖北">湖北</option>
            <option value="湖南">湖南</option>
            <option value="吉林">吉林</option>
            <option value="江苏">江苏</option>
            <option value="江西">江西</option>
            <option value="辽宁">辽宁</option>
            <option value="内蒙古">内蒙古</option>
            <option value="宁夏">宁夏</option>
            <option value="青海">青海</option>
            <option value="山东">山东</option>
            <option value="山西">山西</option>
            <option value="陕西">陕西</option>
            <option value="上海">上海</option>
            <option value="四川">四川</option>
            <option value="台湾">台湾</option>
            <option value="天津">天津</option>
            <option value="西藏">西藏</option>
            <option value="香港">香港</option>
            <option value="新疆">新疆</option>
            <option value="云南">云南</option>
            <option value="浙江">浙江</option>
            <option value="重庆">重庆</option>
        </select>
    </div>
    <div class="divitem" style="width: 20%;">
        <select id="myselectprovider"  style="display: none"  size="12" multiple="multiple" >
            <option value="所有">所有</option>
            <option value="电信">电信</option>
            <option value="联通">联通</option>
            <option value="移动">移动</option>
        </select>
    </div>
    <div class="divitem" style="width: 20%">
        <button value="选择时间" id="timebtn" class="ui-multiselect ui-widget ui-state-default ui-corner-all"
            type="button" aria-haspopup="true" tabindex="0" style="width: 180px;">
            <span class="ui-icon ui-icon-triangle-2-n-s"></span><span id="timetxt">选择时间</span>
        </button>
    </div>
    <div class="divitem" style="position: absolute; right: 10px; margin-right: 1px; width: 5%;">
        <button id="btnpost" class="ui-multiselect ui-widget ui-state-default ui-corner-all"
            type="button" aria-haspopup="true" tabindex="0">

            <span id="Span1">查询</span>
        </button>
    </div>
  
    <input id="inputsoftselect" class="inputsoftselect" runat="server" value="9" type="hidden" />
    <input id="inputareaselect" class="inputareaselect" runat="server" value="全国" type="hidden" />
    <input id="inputproviderselect" class="inputproviderselect" runat="server" value="所有" type="hidden" />
    <input id="inputbegintime" class="inputbegintime" runat="server" type="hidden" />
    <input id="inputendtime" class="inputendtime" runat="server" type="hidden" />
    <input id="inputisfirstload"   runat="server" type="hidden" value="0" />
</div>
<div id="timeid" class="ui-multiselect-menu ui-widget ui-widget-content ui-corner-all"
    style="none: block; top: 37px;">
    <div class="ui-widget-header ui-corner-all ui-multiselect-header ui-helper-clearfix ui-multiselect-hasfilter">
        <div class="ui-multiselect-filter" style="margin-right: 0px;">
            <span>从</span>
            <input type="text" id="fromtime" class="Wdate fromtime" onclick="WdatePicker()" runat="server" />
        </div>
    </div>
    <div class="ui-widget-header ui-corner-all ui-multiselect-header ui-helper-clearfix ui-multiselect-hasfilter">
        <div class="ui-multiselect-filter" style="margin-right: 0px;">
            <span>至</span>
            <input type="text" id="totime" class="Wdate totime" onclick="WdatePicker()" runat="server" />
        </div>
    </div>
</div>
<script type="text/javascript">
   
    var softbox;
    var area;
    var areaprovider;
    Array.prototype.contains = function(item) {
        for (var i = 0; i < this.length; i++) {
            if (this[i] == item) {
                return true;
            }
        }
        return false;
    };
    $(function () {

        BindData();
       
        ///提交表单检查并提交
        $("#btnpost").bind("click", function () {
            checkCondition();
            if (ischecked)
                checkedforcontrol();
            if (ischecked)
                $("#form1").submit();
        });
        softbox = $("#myselectsoft").multiselect({
            multiple: false,
            noneSelectedText: "选择软件",
            header: false,
            height: 150,
            minWidth: 170,
            selectedList: 100,
            beforeclose: function () {
                if ($("#myselectsoft").val() == null) {
                    alert("至少选择一款软件！");
                    return false;
                }
                $(".inputsoftselect").val($("#myselectsoft").val());

            }

        });
        <%if(!HiddenArea){ %>
        area = $("#myselectarea").multiselect({
            multiple: false,
            noneSelectedText: "选择地区",
            header: '',
            height: 275,
            minWidth: 170,
            selectedList: 100,
            beforeclose: function () {
                if ($("#myselectarea").val() == null) {
                    alert("至少选择一个地区！");
                    return false;
                }
                $(".inputareaselect").val($("#myselectarea").val());

            }

        }).multiselectfilter();

        <%} %>

        <%if(!HiddenPrivider) {%>
        areaprovider = $("#myselectprovider").multiselect({
            multiple: true,
            noneSelectedText: "选择供应商",
            header: false,
            height: 150,
            minWidth: 170,
            selectedList: 100,
            beforeclose: function () {
                if ($("#myselectprovider").val() == null) {
                    alert("至少选择一个供应商！");
                    return false;
                }
                $(".inputproviderselect").val($("#myselectprovider").val());

            }

        });

        <%} %>

        setDivPos();
        ///时间设置
        if ($("#timetxt")[0]) {
            var fromtime = $(".fromtime").val();
            var totime = $(".totime").val();
            if (fromtime != "" && totime != "") {
                $(".inputbegintime").val(fromtime);
                $(".inputendtime").val(totime);
                $("#timetxt").text(fromtime.substring(2, fromtime.length).replace(/-/g, '/') + "至" + totime.substring(2, fromtime.length).replace(/-/g, '/'));
            }
        }
        ///多个时间点击
        $("#timebtn").bind('click', function () {
            var timeid = $("#timeid");
            //alert(123);
            var isopen = timeid.is(":visible");
            if (isopen) {
                var fromtime = $(".fromtime").val();
                var totime = $(".totime").val();
                if (fromtime != "" && totime != "") {
                    $(".inputbegintime").val(fromtime);
                    $(".inputendtime").val(totime);
                    $("#timetxt").text(fromtime.substring(2, fromtime.length).replace(/-/g, '/') + "至" + totime.substring(2, fromtime.length).replace(/-/g, '/'));
                }
                timeid.hide();
            }
            else
                timeid.show();
        });
        $(document).bind('mousedown', function (e) {
            var timeid = $("#timeid");
            if (timeid[0]) {
                var isopen = timeid.is(":visible");
                var timebtn = $("#timebtn");
                if (isopen && !$.contains(timeid[0], e.target) && !$.contains(timebtn[0], e.target) && e.target !== timebtn[0]) {
                    var fromtime = $(".fromtime").val();
                    var totime = $(".totime").val();
                    if (fromtime != "" && totime != "") {
                        var starttime1 = fromtime.replace(/-/g, '/');
                        var endtime1 = totime.replace(/-/g, '/');
                        if (Date.parse(starttime1) > Date.parse(endtime1)) {
                            alert("日期范围有问题！");
                            return;
                        }
                        $(".inputbegintime").val(fromtime);
                        $(".inputendtime").val(totime);
                        $("#timetxt").text(fromtime.substring(2, fromtime.length).replace(/-/g, '/') + "至" + totime.substring(2, fromtime.length).replace(/-/g, '/'));
                    }
                    timeid.hide();
                }
            }

        });

        ///绑定窗体大小改变事件
        $(window).bind("resize", function () {
            if ($("#timebtn")[0]) {
                setDivPos();
            }

            if (softbox) {
                softbox.multiselect('refreshmenupos');
            }
            if (area) {

                area.multiselect('refreshmenupos');
            }
            if (areaprovider) {

                areaprovider.multiselect('refreshmenupos');
            }


        });

    });



    function setDivPos() {

        ///设置时间div 的位置
        var timebtn = $("#timebtn");
        if (timebtn[0]) {
            var pos = timebtn.offset();
            $("#timeid").css({ top: pos.top + $("#timebtn").outerHeight() + 6, left: pos.left })
            ///设置宽度
            var m = $("#timeid");
            width = timebtn.outerWidth() -
				    parseInt(m.css('padding-left'), 10) -
				    parseInt(m.css('padding-right'), 10) -
				    parseInt(m.css('border-right-width'), 10) -
				    parseInt(m.css('border-left-width'), 10);

            m.width(width || timebtn.outerWidth());
        }
        
    }
    ///内部检查
    function checkedforcontrol() {
        if ($("#timebtn")[0]) {
            var starttime1 = $.trim($(".inputbegintime").val()).replace(/-/g, '/');
            var endtime1 = $.trim($(".inputendtime").val()).replace(/-/g, '/');
            if (Date.parse(starttime1) > Date.parse(endtime1)) {
                alert("日期范围有问题！");
                ischecked = false;
                return;
            }
        } 
      
        ischecked = true;

    }
    ///绑定初始化数据
    function BindData() {
        var softid = $(".inputsoftselect").val();
        var areaid = $(".inputareaselect").val();
        var provider = $(".inputproviderselect").val();
        var softcontrol= $("#myselectsoft")[0];
        var areacontrol=$("#myselectarea")[0];
        var providercontrol=$("#myselectprovider")[0];

        for (var i = 0; i < softcontrol.options.length; i++) {
            if (softcontrol.options[i].value == softid) {
                softcontrol.options[i].selected = true;
                break;
            }

        }
        for (var i = 0; i < areacontrol.options.length; i++) {
            if (areacontrol.options[i].value == areaid) {
                areacontrol.options[i].selected = true;
                break;
            }

        }
        var providers = provider.split(',');
         
        for (var  i = 0; i < providercontrol.options.length; i++) {
            if(providers.contains(providercontrol.options[i].value))
            {
                providercontrol.options[i].selected = true;
            }

        }

    }
</script>
