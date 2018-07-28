<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AllCustomStat.aspx.cs"
    Inherits="net91com.Stat.Web.Reports.sjqd.AllCustomStat" %>
<%@ Import Namespace="net91com.Core.Extensions" %>
<%@ Register Src="/Reports/Controls/PeriodSelector.ascx" TagName="PeriodSelector" TagPrefix="uc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="/css/ReportCss.css" rel="stylesheet" type="text/css" />
    <script src="/Scripts/HeadScript/jquery.js" type="text/javascript"></script>
    <script src="/Scripts/highcharts.js" type="text/javascript"></script>
    <script src="/My97DatePicker/WdatePicker.js" type="text/javascript"></script>
    <link href="/css/headcss/jquery-ui.css" rel="stylesheet" type="text/css" />
    <link href="/css/headcss/jquery.multiselect.css?rd=<%= net91com.Stat.Web.Reports.Services.Utility.CssVersion %>" rel="stylesheet" type="text/css" />
    <link href="/css/headcss/jquery.multiselect.filter.css" rel="stylesheet" type="text/css" />
    <script src="/Scripts/HeadScript/jquery-ui.min.js" type="text/javascript"></script>
    <script src="/Scripts/HeadScript/jquery.multiselect.js?rd=<%= net91com.Stat.Web.Reports.Services.Utility.JsVersion %>" type="text/javascript"
        charset="GBK"></script>
    <script type="text/javascript">
        var period=<%=(int)Period %>;
        var ischecked = false;
        function checkcondition() {
            var starttime1 = $.trim($("#fromtime").val()).replace(/-/g, '/'); ;
            var endtime1 = $.trim($("#totime").val()).replace(/-/g, '/');
            if (starttime1.length == 0) {
                alert("请选择时间");
                return false;
            }

            if (endtime1.length == 0) {
                alert("请选择结束时间");
                return false;
            }

            if (Date.parse(starttime1) > Date.parse(endtime1)) {
                alert("日期范围有问题！");
                return false;
            }
            ischecked = true;

        }

        function geturl() {
            var begintime = $(".begintime").val();
            var endtime = $(".endtime").val();
            var url = "ChannelCustom.ashx?code=allcustomstatexcel&" + "Channelid=" + $("#ChannelID").val() + "&Channeltype=" + $("#ChannelType").val() + "&begintime=" + begintime + "&endtime=" + endtime + "&plat=" + $(".inputplatformselect").val() + "&channelname=" + $("#Name").text() + "&rds=" + encodeURIComponent(new Date())+"&period="+period;
            $("#excelbtn").attr("href", url);
        }

        $(function () {

            geturl();
            $("#seachout").click(function () {
                checkcondition();
                if (ischecked == true) {
                    $("#form1").submit();

                }

            });
             
            ///初始化设计
            var platselect = $("#myselectplat")[0];
            platselect.add(new Option('不区分平台', '0'));
            var plats = $(".inputplatformselect").val().split(',');
            for (var i = 0; i < platselect.options.length; i++) {
                for (var j = 0; j < plats.length; j++) {
                    if (plats[j] == platselect.options[i].value) {
                        platselect.options[i].selected = true;
                        break;
                    }
                }
            }

            ///初始化platbox
            platbox = $("#myselectplat").multiselect({
                multiple: false,
                noneSelectedText: "选择平台",
                header: false,
                height: 250,
                minWidth: 170,
                selectedList: 100,
                beforeclose: function () {
                    if ($("#myselectplat").val() == null) {
                        alert("至少选择一个选项！");
                        return false;
                    }
                    $(".inputplatformselect").val($("#myselectplat").val() == null ? "-1" : $("#myselectplat").val());

                },
                beforeopen: function () {
                    ischangedplat = $(".inputplatformselect").val();
                }

            });
            ///绑定窗体大小改变事件
            $(window).bind("resize", function () {
                if (platbox) {
                    platbox.multiselect('refreshmenupos');
                }
            });

        });

        
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <asp:HiddenField ID="ChannelID" runat="server" />
    <asp:HiddenField ID="ChannelType" runat="server" /> 
    <input id="inputplatformselect" class="inputplatformselect" runat="server" type="hidden" value="0" />
    <div class="main">
        <div class="caidan" style="position: static">
            <table style="font-size: 11px; width: 100%">
                <tr>
                    <td style="width: 15%; padding-left: 5px">
                        <span id="Name" runat="server">熊猫看书</span>
                    </td>
                    <td style=" width:15%;">
                     
                        <select id="myselectplat" size="12" multiple="multiple">
                        <%for (int i = 0; i < MySupportPlat.Count; i++)
                          {%>
                            <option value="<%=(int)MySupportPlat[i]%>"><%= MySupportPlat[i].GetDescription()%></option>  
                          <%} %>
                        </select>
                      
                    </td>
                    <td style="width: 45%; padding-bottom: 1px;">
                        <div>
                            <span>时间从：</span>
                            <input id="fromtime" type="text" class="Wdate begintime" onclick="WdatePicker()" runat="server" />
                            <span>到：</span>
                            <input id="totime" type="text" class="Wdate endtime" onclick="WdatePicker()" runat="server" />
                        </div>
                    </td>
                    <td style="width: 10%; text-align: center;">
                        <a id="excelbtn" style="display: block; cursor: pointer"><span>下载到Excel</span> </a>
                    </td>
                    <td style="width: 10%; text-align: center;">
                        <a id="seachout" style="display: block; cursor: pointer"><span>查询</span> </a>
                    </td>
                </tr>
            </table>
        </div>
          <uc1:PeriodSelector ID="PeriodSelector1" runat="server" />
        <div>
        <%=tableStr %>
        
        </div>
    </div>
    </form>
</body>
</html>
