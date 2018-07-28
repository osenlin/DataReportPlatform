<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RankOfChannels.aspx.cs"
    Inherits="net91com.Stat.Web.Reports.RankOfChannels" EnableViewState="false" EnableViewStateMac="false" %>

<%@ Register Src="/Reports/Controls/HeadAllControl.ascx" TagName="HeadControl" TagPrefix="uc1" %>
<%@ Import Namespace="net91com.Stat.Web.Reports.Services" %>
<%@ Import Namespace="net91com.Stat.Web.Base" %>
<%@ Import Namespace="net91com.Stat.Services.Entity" %>
<%@ Register Src="Controls/PeriodSelector.ascx" TagName="PeriodSelector" TagPrefix="uc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>渠道排行榜</title>
    <link href="../css/jquery.loadmask.css" rel="stylesheet" type="text/css" />
    <link href="../css/ReportCss.css" rel="stylesheet" type="text/css" />
    <link href="../css/help.css" rel="Stylesheet" type="text/css" />
    <script src="../Scripts/HeadScript/jquery.js" type="text/javascript"></script>
    <script src="../Scripts/highcharts.js" type="text/javascript"></script>
    <script src="../Scripts/exporting.js" type="text/javascript"></script>
    <script src="../Scripts/common.js" type="text/javascript"></script>
    <script src="../Scripts/jTip.js" type="text/javascript"></script>
    <script src="../Scripts/jquery.loadmask.min.js" type="text/javascript"></script>
    <script type="text/javascript">


        var chart;
        ///自定义控件检查函数实现(这个在控件内部调用了)
        function checkCondition() {
            ischecked = true;

        }

        function changeOrder() {
            if ($("#hDesc").val() == "false") {
                $("#hDesc").val("true");
            } else {
                $("#hDesc").val("false");
            }
        }        
        
    </script>
    <script src="../Scripts/common.js" type="text/javascript"></script>
</head>
<body>
    <form id="form1" runat="server">
    <input type="hidden" id="hOrderBy" name="hOrderBy" value="<%= OrderBy %>" />
    <input type="hidden" id="hDesc" name="hDesc" value="<%= (Desc).ToString().ToLower() %>" />
    <div class="main">
        <uc1:HeadControl ID="HeadControl1" runat="server" />
        <div>
            <table style="width: 100%">
                <tr>
                    <td>
                        <uc1:PeriodSelector ID="PeriodSelector1" runat="server" />
                    </td>
                </tr>
            </table>
        </div>
        <div style="margin-top: 3px">
            <div class="tablehead helpclass_head">
                <span class="helpclass_span1">&nbsp;&nbsp;渠道排行榜</span>&nbsp;&nbsp;&nbsp;&nbsp;<span style="color: Blue">(时间：<%= string.Format("{0:yyyy-MM-dd}至{1:yyyy-MM-dd}", BeginTime, EndTime) %>)（注：点击表头链接，可按相应的指标排序）</span><span class="helpclass_span2">
                    <a id="A1" href="helphtm/top20sjqd.htm?v=20141120" class="jTip">
                        <img alt="帮助" src="../Images/help.gif" /></a></span>
            </div>
            <table class="tablesorter" cellspacing="1" id="tbl">
                <thead>
                    <tr style="text-align:center;">
                        <th>排名</th>
                        <th>
                            渠道
                        </th>
                        <th>
                            <a style="text-decoration: none;" href="javascript:void(0);" onclick="$('#hOrderBy').val('0');changeOrder();$('#form1').submit();">新增用户<% if (OrderBy == 0)
                                                                                                                         { %>&nbsp;&nbsp;&nbsp;&nbsp;<img style="height:16px;" src="<%= Desc ? "../Images/zd2.png" : "../Images/zd1.png" %>" /><%} %></a>
                        </th>
                        <th>
                            <a style="text-decoration: none;" href="javascript:void(0);" onclick="$('#hOrderBy').val('1');changeOrder();$('#form1').submit();">涨跌量<% if (OrderBy == 1)
                                                                                                                         { %>&nbsp;&nbsp;&nbsp;&nbsp;<img style="height:16px;" src="<%= Desc ? "../Images/zd2.png" : "../Images/zd1.png" %>" /><%} %></a>
                        </th>
                        <th>
                            累计用户
                        </th>
                        <th>
                            活跃用户
                        </th>
                        <th>
                            活跃度
                        </th>
                        <th>
                            留存率(上周期)
                        </th>
                        <th>
                            操作
                        </th>
                    </tr>
                </thead>
                <tbody>
                    <asp:Repeater ID="Repeater1" runat="server">
                        <ItemTemplate>
                            <tr style="text-align:right;">
                                <td><%# Eval("RankIndex") %></td>
                                <td style="text-align:left;">
                                    <a href='javascript:linkDetail(<%#Eval("ChannelID")%>,2)'>
                                        <%#Eval("ChannelName")%>
                                    </a>
                                </td>
                                <td>
                                    <%#Eval("NewUserCount")%>
                                </td>
                                <td>
                                    <%#Eval("NewUserCountDiff")%>
                                </td>
                                <td>
                                    <%#Eval("TotalUserCount")%>
                                </td>
                                <td>
                                    <%#Eval("ActiveUserCount")%>
                                </td>
                                <td>
                                    <%#Eval("ActiveUserRate")%>
                                </td>
                                <td>
                                    <%#Eval("RetainedUserRate")%>
                                </td>
                                <td style="text-align:left;">
                                    <span class="caozuo" onclick="getRankOfSubChannels(this,<%#Eval("ChannelID")%>)">子渠道</span>
                                    <span class="caozuo" onclick="getRankOfVersions(this,<%#Eval("ChannelID")%>)">版本</span>
                                    <span class="caozuo" onclick="getRankOfCountries(this,<%#Eval("ChannelID")%>)">国家</span>
                                </td>
                            </tr>
                            <tr style="display: none" id="tr_<%#Eval("ChannelID")%>">
                                <td colspan="10">
                                    <div id="div_<%#Eval("ChannelID")%>">
                                    </div>
                                </td>
                            </tr>
                            
                        </ItemTemplate>
                    </asp:Repeater>
                </tbody>
            </table>
        </div>
    </div>
    </form>
    <script type="text/javascript">

        //获取子渠道排行
        function getRankOfSubChannels(a, channelId) {
            var url = "<%= GetRankOfSubChannelsUrl %>&ChannelIds=" + channelId;
            getMore(a, channelId, url, "子渠道");
        }

        //获取版本排行
        function getRankOfVersions(a, channelId) {
            var url = "<%= GetRankOfVersionsUrl %>&ChannelIds=" + channelId;
            getMore(a, channelId, url, "版本");
        }

        //获取国家排行
        function getRankOfCountries(a, channelId) {
            var url = "<%= GetRankOfCountriesUrl %>&ChannelIds=" + channelId;
            getMore(a, channelId, url, "国家");
        }

        //获取省排行
        function getRankOfProvinces(a, channelId) {
            var url = "<%= GetRankOfProvincesUrl %>&ChannelIds=" + channelId;
            getMore(a, channelId, url, "省");
        }

        //获取市排行
        function getRankOfCities(a, channelId) {
            var url = "<%= GetRankOfCitiesUrl %>&ChannelIds=" + channelId;
            getMore(a, channelId, url, "市");
        }

        var preA = null;
        var preText = null;
        //异步请求接口
        function getMore(a, channelId, url, text) {
            var tr = $("#tr_" + channelId);
            var div = $("#div_" + channelId);
            if ($(tr).is(":hidden")) {
                $.getJSON(url,
                    function (table) {
                        if (table.code > 0) {
                            preA = $(a);
                            preText = $(a).text();
                            $(div).html(table.data);
                            $(tr).show();
                            $(a).text("收缩");
                        }
                        else {
                            alert(table.data);
                        }
                    });
            } else {
                $(tr).hide();
                $(div).html("");
                if ($(preA).text() == $(a).text()) {
                    preA = null;
                    preText = null;
                    $(a).text(text);
                } else {
                    $(preA).text(preText);
                    preA = null;
                    preText = null;
                    getMore(a, channelId, url, text);
                }
            }
        }

        function linkDetail(channelid, channeltype) {
            var palt = '<%=Platform %>';
            var father = window.parent;
            ///到时候改就改这个id
            var id = '<%= SjqdUrl%>';
            var url = "Reports/GetMore.aspx?reporttype=13&channelid=" + channelid + "&channeltype=" + channeltype + "&plat=" + palt + "&period=<%= PeriodSelector1.SelectedPeriod %>";
            ///改这个名称
            var title = '<%=SjqdUrlName %>';
            var menupanleid = "mp<%=SjqdParentUrl %>";
            var menuitem = father.Ext.getCmp(id);
            var menupanel = father.Ext.getCmp(menupanleid);
            father.moveTab(id);
            father.addTab(father.TabPanel1, 'idClt' + id, url, title, menuitem, menupanel);

        }
    </script>
</body>
</html>
