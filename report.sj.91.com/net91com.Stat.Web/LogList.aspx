<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LogList.aspx.cs" Inherits="net91com.Stat.Web.LogList" EnableViewState="false" EnableViewStateMac="false" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>服务器日志列表</title>
</head>
<body>
    <div>
        <div><h2>服务器日志列表</h2></div>
        <ul>
            <li><a href="http://community.sj.91.com/sjspacems/stat/LogViewer.aspx?account=<%= Request["account"] %>&sign=<%= Request["sign"] %>" target="10.1.20.181">[10.1.20.181日志]</a></li>
            <li><a href="http://sjstatic.sj.91.com/LogViewer.aspx?account=<%= Request["account"] %>&sign=<%= Request["sign"] %>&ip=10.1.242.100" target="10.1.242.100">[10.1.242.100日志]</a>&nbsp;&nbsp;
                <a href="http://sjstatic.sj.91.com/LogViewer.aspx?account=<%= Request["account"] %>&sign=<%= Request["sign"] %>&ip=10.1.242.150" target="10.1.242.150">[10.1.242.150日志]</a>&nbsp;&nbsp;
                <a href="http://sjstatic.sj.91.com/LogViewer.aspx?account=<%= Request["account"] %>&sign=<%= Request["sign"] %>&ip=10.1.242.176" target="10.1.242.176">[10.1.242.176日志]</a>&nbsp;&nbsp;
                <a href="http://sjstatic.sj.91.com/LogViewer.aspx?account=<%= Request["account"] %>&sign=<%= Request["sign"] %>&ip=10.1.242.189" target="10.1.242.189">[10.1.242.189日志]</a>&nbsp;&nbsp;
                <a href="http://sjstatic.sj.91.com/LogViewer.aspx?account=<%= Request["account"] %>&sign=<%= Request["sign"] %>&ip=10.1.242.247" target="10.1.242.247">[10.1.242.247日志]</a></li>
            <li><a href="http://sjstatic.sj.91.com/LogViewer.aspx?account=<%= Request["account"] %>&sign=<%= Request["sign"] %>&ip=10.2.104.146" target="10.2.104.146">[10.2.104.146日志]&nbsp;&nbsp;
                <a href="http://sjstatic.sj.91.com/LogViewer.aspx?account=<%= Request["account"] %>&sign=<%= Request["sign"] %>&ip=10.2.100.211" target="10.2.100.211">[10.2.100.211日志]</a>
            </li>

            <li><a href="http://sjstatic.sj.91.com/LogViewer.aspx?account=<%= Request["account"] %>&sign=<%= Request["sign"] %>&ip=10.79.132.31" target="10.79.132.31">[10.79.132.31日志]</a>&nbsp;&nbsp;
                <a href="http://sjstatic.sj.91.com/LogViewer.aspx?account=<%= Request["account"] %>&sign=<%= Request["sign"] %>&ip=10.79.132.32" target="10.79.132.32">[10.79.132.32日志]</a>&nbsp;&nbsp;
                <a href="http://sjstatic.sj.91.com/LogViewer.aspx?account=<%= Request["account"] %>&sign=<%= Request["sign"] %>&ip=10.79.132.33" target="10.79.132.33">[10.79.132.33日志]</a>&nbsp;&nbsp;
                <a href="http://sjstatic.sj.91.com/LogViewer.aspx?account=<%= Request["account"] %>&sign=<%= Request["sign"] %>&ip=10.79.132.34" target="10.79.132.34">[10.79.132.34日志]</a>&nbsp;&nbsp;
                <a href="http://sjstatic.sj.91.com/LogViewer.aspx?account=<%= Request["account"] %>&sign=<%= Request["sign"] %>&ip=10.79.132.35" target="10.79.132.35">[10.79.132.35日志]</a></li>
            <li><a href="http://sjstatic.sj.91.com/LogViewer.aspx?account=<%= Request["account"] %>&sign=<%= Request["sign"] %>&ip=10.79.227.31" target="10.79.227.31">[10.79.227.31日志]&nbsp;&nbsp;
                <a href="http://sjstatic.sj.91.com/LogViewer.aspx?account=<%= Request["account"] %>&sign=<%= Request["sign"] %>&ip=10.79.228.53" target="10.79.228.53">[10.79.228.53日志]</a>
            </li>
        </ul>
    </div>
</body>
</html>
