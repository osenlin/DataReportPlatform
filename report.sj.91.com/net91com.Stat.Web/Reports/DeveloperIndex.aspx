<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DeveloperIndex.aspx.cs"
    Inherits="net91com.Stat.Web.Reports.DeveloperIndex" %>

<%@ Import Namespace="net91com.Stat.Web.Reports.Services" %>
<%@ Import Namespace="net91com.Core" %>
<%@ Import Namespace="net91com.Core.Extensions" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<%@ Register Assembly="AspNetPager" Namespace="Wuqi.Webdiyer" TagPrefix="webdiyer" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="../css/help.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
        .tablehead
        {
            background-image: url("../Images/bg_tablehead.JPG");
            background-repeat: repeat;
            border: 1px solid #CCCCCC;
            color: #1E6FAF;
            height: 20px;
            line-height: 20px;
            padding: 5px;
        }
        .tablesorter td
        {
            text-align: center;
            vertical-align: middle;
        }
        .tablesorter th
        {
            text-align: center;
            vertical-align: middle;
            background-position: right center;
            background-repeat: no-repeat;
        }
        .headimg
        {
            background-image: url(../Images/bg.gif);
            cursor: pointer;
        }
        .headimgup
        {
            background-image: url(../Images/desc.gif);
            cursor: pointer;
        }
        .headimgdown
        {
            background-image: url(../Images/asc.gif);
            cursor: pointer;
        }
    </style>
    <link href="../css/ReportCss.css" rel="stylesheet" type="text/css" />
    <script src="../Scripts/jquery-1.5.2.min.js" type="text/javascript"></script>
    <script src="../Scripts/jtip.js" type="text/javascript"></script>
   
</head>
<body>
    <form id="form1" runat="server">
    <asp:HiddenField ID="HiddenSorterType" Value="1" runat="server" />
    <asp:HiddenField ID="HiddenSorterField" Value="0" runat="server" />
    <asp:HiddenField ID="HiddenSorterChange" Value="0" runat="server" />
    <div class="main">
        <div class="tablehead">
            <span class="helpclass_span1">&nbsp;&nbsp;昨日汇总数据 </span><span class="helpclass_span2">
                <a id="one" href="helphtm/explain.htm" class="jTip">
                    <img alt="帮助" src="../Images/help.gif" /></a></span>
        </div>
        <!--中间区域的div-->
        <div id="middle" style="margin-top: 3px;">
            <table class="tablesorter" cellspacing="1" style="margin: 0 auto;">
                <thead>
                    <tr>
                        <th align="center">
                            平台
                        </th>
                        <th align="center">
                            应用数
                        </th>
                        <th align="center">
                            累计用户
                        </th>
                        <th align="center">
                            日新增用户
                        </th>
                        <th align="center">
                            日活跃用户
                        </th>
                       
                    </tr>
                </thead>
                <tbody>
                    <asp:Repeater ID="TableRepeat5"  runat="server">
                        <ItemTemplate>
                            <tr>
                                <td>
                                    <%# ((net91com.Core.MobileOption)Eval("Platform")).ToString()%>
                                </td>
                                <td>
                                    <%#Utility.SetNum(Convert.ToInt32( Eval("Count")))%>
                                </td>
                                <td>
                                    <%#Utility.SetNum(Convert.ToInt32( Eval("AllUserNum"))) %>
                                </td>
                                <td>
                                    <%#Utility.SetNum(Convert.ToInt32( Eval("DayNewNum"))) %>
                                </td>
                                <td>
                                    <%#Utility.SetNum(Convert.ToInt32( Eval("DayActive")))%>
                                </td>
                               
                            </tr>
                        </ItemTemplate>
                        <FooterTemplate>
                         <tr>
                                <td>
                                  总计
                                </td>
                                <td>
                                   <%=TotalNumList[0]%>
                                </td>
                                <td>
                                     <%=TotalNumList[1]%>
                                </td>
                                <td>
                                     <%=TotalNumList[2]%>
                                </td>
                                <td>
                                     <%=TotalNumList[3]%>
                                </td>
                               
                            </tr>
                        </FooterTemplate>
                    </asp:Repeater>
                </tbody>
            </table>
        </div>
        <!--下面区域的div-->
     
                <div id="low" style="margin-top: 3px;">
                    <table class="tablesorter" cellspacing="1" style="margin: 0 auto;">
                        <thead>
                            <tr>
                                <th id="headsort_1" class="headimg " style="width: 10%;">
                                    平台
                                </th>
                                <th id="headsort_2" class="headimg" style="width: 10%;">
                                    应用名称
                                </th>
                                <th id="headsort_3" class="headimg" style="width: 10%;">
                                    累计用户
                                </th>
                                <th id="headsort_5" class="headimg " style="width: 14%;">
                                    日新增用户[昨日]
                                </th>
                                <th id="headsort_6" class="headimg" style="width: 14%;">
                                    日活跃用户[昨日]
                                </th>
                                <th style="width: 10%;">
                                    查看报表
                                </th>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:Repeater ID="Repeater1" runat="server">
                                <ItemTemplate>
                                    <tr>
                                        <td style="vertical-align: middle">
                                           <img title="<%#  ((net91com.Core.MobileOption)Eval("Platform")).ToString()  %>" src="<%# GetSrc( (int)Eval("Platform")) %>" />
                                        </td>
                                        <td style="vertical-align: middle">
                                            <%#Eval("SoftName") %>
                                        </td>
                                        <td style="vertical-align: middle">
                                            <%#Utility.SetNum(Convert.ToInt32( Eval("AllUserNum")))  %>
                                        </td>
                                        <td style="vertical-align: middle">
                                            <%#Utility.SetNum(Convert.ToInt32( Eval("DayNewNum"))) %>
                                        </td>
                                        <td style="vertical-align: middle">
                                            <%#Utility.SetNum(Convert.ToInt32( Eval("DayActiveNum"))) %>
                                        </td>
                                        <td style="vertical-align: middle">
                                            <a style="text-decoration: none" href="javascript:linkDetail('<%#Eval("SoftId") %>','<%#(int)Eval("Platform") %>')">
                                                查看</a>
                                        </td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                        </tbody>
                    </table>
                     <div>
                    <webdiyer:AspNetPager ID="AspNetPager1" runat="server" CssClass="paginator" AlwaysShow="false"
                        CurrentPageButtonClass="cpb" FirstPageText="首页" ShowCustomInfoSection="right"
                        Width="100%" NavigationButtonType="Image" LastPageText="尾页" NextPageText="下一页"
                        OnPageChanged="AspNetPager1_PageChanged" PrevPageText="上一页" CustomInfoHTML=""
                        LayoutType="Table">
                    </webdiyer:AspNetPager>
                </div>
                </div>
               
                <script type="text/javascript">
                    $(function () {
                        for (var i = 1; i <= 5; i++) {
                            $("#headsort_" + i).click(function () {
                                headsort($(this));
                                $("#HiddenSorterChange").val("1");
                                $("#form1").submit();

                            });

                        }
                         var m = $("#HiddenSorterField").val();
                         var n = $("#HiddenSorterType").val();
                         setheadclass(m, n);
                     });
                     //m 为列序号，n 为升还是降
                     function setheadclass(m, n) {
                           $("#headsort_"+m).attr("class", "");
                            if (n == 1) {
                                $("#headsort_"+m).addClass("headimgdown");
                            }
                            else {
                                $("#headsort_"+m).addClass("headimgup");
                            }
                         
                     }
                     ///排序
                     function headsort(a) {
                         var id = $(a).attr("id");
                         var field = id.substr(id.length - 1)
                         if (a.hasClass("headimg")) {
                             $("#HiddenSorterType").val("1");
                             $("#HiddenSorterField").val(field);
                         }
                         if (a.hasClass("headimgdown")) {
                             $("#HiddenSorterType").val("2");
                             $("#HiddenSorterField").val(field);
                         }
                         if (a.hasClass("headimgup")) {
                             $("#HiddenSorterType").val("1");
                             $("#HiddenSorterField").val(field);
                         }

                     }
        
       
                </script>
         
    </div>
    </form>
    <form id="formlink" method="post" action="Default.aspx" style="display: none">
    <input type="text" value="" id="inputversionselect" />
    <input type="text" value="" id="inputplatformselect" />
    </form>
    <script type="text/javascript">
        function linkDetail(softname, platform) {
            var father = window.parent;
            var id = '<%=DefaultUrl %>';
            var url = "Reports/GetMore.aspx?soft=" + softname + "&plat=" + platform + "&reporttype=15" + "&smalltype=" + 2;
            var title = '<%=DefaltName %>';
            var menupanleid = "mp" + '<%=DefaultParentUrl %>';

            var menuitem = father.Ext.getCmp(id);
            var menupanel = father.Ext.getCmp(menupanleid);
            father.moveTab(id);
            father.addTab(father.TabPanel1, 'idClt' + id, url, title, menuitem, menupanel);
        }
    </script>
</body>
</html>
