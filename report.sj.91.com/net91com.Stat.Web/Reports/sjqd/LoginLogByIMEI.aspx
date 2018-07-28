<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LoginLogByIMEI.aspx.cs"
    Inherits="net91com.Stat.Web.Reports.DownloadReports.LoginLogByIMEI" %>

<%@ Register Assembly="AspNetPager" Namespace="Wuqi.Webdiyer" TagPrefix="webdiyer" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <link href="../../css/jquery.loadmask.css" rel="stylesheet" type="text/css" />
    <link href="../../css/ReportCss.css" rel="stylesheet" type="text/css" />
    <link href="../../css/help.css" rel="Stylesheet" type="text/css" />
    <script src="../../Scripts/HeadScript/jquery.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(function () {
            $("#btnSearch").click(function () {
                var imei = $("#txtIMEI").val();
                if (imei == '') {
                    alert('请输入IMEI');
                    return false;
                }
            });
        });
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div class="main">
        <div class="tablehead2" style="margin-bottom: 3px; margin-top: 3px;">
            <div style="float: left">
                IMEI：<input id="txtIMEI" maxlength="50" runat="server" style="width: 200" />
                <asp:Button ID="btnSearch" runat="server" Text="搜索" OnClick="btnSearch_Click" />    
            </div>
        </div>
        <div>
            <div id="mytable">
                <table class="tablesorter" cellspacing="1" style="margin: 0 auto;">
                    <thead>
                        <tr>
                            <th>
                                访问软件
                            </th>
                            <th>
                                软件版本
                            </th>
                            <th>
                                渠道来源
                            </th>
                            <th>
                                访问时间
                            </th>
                        </tr>
                    </thead>
                    <tbody>
                        <asp:Repeater ID="Repeater1" runat="server">
                            <ItemTemplate>
                                <tr>
                                    <td>
                                        <%# Eval("SoftName")%>
                                    </td>
                                    <td>
                                        <%# Eval("SoftVersion")%>
                                    </td>
                                    <td>
                                        <%# Eval("Fromway")%>
                                    </td>
                                    <td>
                                        <%# Eval("LoginTime")%>
                                    </td>
                                </tr>
                            </ItemTemplate>
                        </asp:Repeater>
                    </tbody>
                </table>
            </div>
        </div>
    </div>
    </form>
</body>
</html>
