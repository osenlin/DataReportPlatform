<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Top20Areas.aspx.cs"
    Inherits="net91com.Stat.Web.Reports.Top20Areas" EnableViewState="false" EnableViewStateMac="false" %>

<%@ Register Src="/Reports/Controls/HeadAllControl.ascx" TagName="HeadControl" TagPrefix="uc1" %>
<%@ Import Namespace="net91com.Stat.Web.Reports.Services" %>
<%@ Import Namespace="net91com.Stat.Web.Base" %>
<%@ Import Namespace="net91com.Stat.Services.Entity" %>
<%@ Register Src="Controls/PeriodSelector.ascx" TagName="PeriodSelector" TagPrefix="uc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>国家或地区排行榜(海外)</title>
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
                <span class="helpclass_span1">&nbsp;&nbsp;国家或地区排行榜(海外)</span>&nbsp;&nbsp;&nbsp;&nbsp;<span style="color: Blue">(时间：<%= string.Format("{0:yyyy-MM-dd}至{1:yyyy-MM-dd}", BeginTime, EndTime) %>)（注：点击表头链接，可按相应的指标排序）</span><span class="helpclass_span2">
                    <a id="A1" href="helphtm/top20Area.htm" class="jTip">
                        <img alt="帮助" src="../Images/help.gif" /></a></span>
            </div>
            <table class="tablesorter" cellspacing="1" id="tbl">
                <thead>
                    <tr style="text-align:center;">
                        <th>排名</th>
                        <th>
                            国家或地区
                        </th>
                        <th>
                            <a style="text-decoration: none;" href="javascript:void(0);" onclick="$('#hOrderBy').val('0');changeOrder();$('#form1').submit();">新增用户<% if (OrderBy == 0)
                                                                                                                         { %>&nbsp;&nbsp;&nbsp;&nbsp;<img style="height:16px;" src="<%= Desc ? "../Images/zd2.png" : "../Images/zd1.png" %>" /><%} %></a>
                        </th>
                        <th>
                            <a style="text-decoration: none;" href="javascript:void(0);" onclick="$('#hOrderBy').val('1');changeOrder();$('#form1').submit();">涨跌量<% if (OrderBy == 1)                                                                                              { %>&nbsp;&nbsp;&nbsp;&nbsp;<img style="height:16px;" src="<%= Desc ? "../Images/zd2.png" : "../Images/zd1.png" %>" /><%} %></a>
                        </th>
                        <th>
                            活跃用户
                        </th>
                        <th>
                            
                            <a style="text-decoration: none;" href="javascript:void(0);" onclick="$('#hOrderBy').val('2');changeOrder();$('#form1').submit();">留存率(上周期)<% if (OrderBy == 2)                                                                                              { %>&nbsp;&nbsp;&nbsp;&nbsp;<img style="height:16px;" src="<%= Desc ? "../Images/zd2.png" : "../Images/zd1.png" %>" /><%} %></a>
                        </th>
                     
                    </tr>
                </thead>
                <tbody>
                    <asp:Repeater ID="Repeater1" runat="server">
                        <ItemTemplate>
                            <tr style="text-align:right;">
                                <td><%# Eval("RankIndex") %></td>
                                <td style="text-align:left;">
                                    <a href="javascript:linkDetail('<%#Eval("ID")%>')">
                                        <%#Eval("Name")%>
                                    </a>
                                </td>
                                <td>
                                    <%#Eval("NewUserCount")%>
                                </td>
                                <td>
                                    <%#Eval("NewUserCountDiff")%>
                                </td>
                                <td>
                                    <%#Eval("ActiveUserCount")%>
                                </td>
                                <td>
                                    <%#Eval("RetainedUserRate")%>
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
        function linkDetail(area) {
            var palt = '<%=Platform %>';
            var father = window.parent;
            ///到时候改就改这个id
            var id = '<%= SjqdUrl%>';
            var url = "Reports/GetMore.aspx?reporttype=29&area=" + area + "&plat=" + palt + "&period=<%= PeriodSelector1.SelectedPeriod %>";
            ///改这个名称
            var title = '<%=SjqdUrlName %>';
            var menupanleid = "mp" + '<%=SjqdParentUrl %>';
            var menuitem = father.Ext.getCmp(id);
            var menupanel = father.Ext.getCmp(menupanleid);
            father.moveTab(id);
            father.addTab(father.TabPanel1, 'idClt' + id, url, title, menuitem, menupanel);

        }
    </script>
</body>
</html>
