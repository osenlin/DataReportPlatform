<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="NoRight.aspx.cs" Inherits="net91com.Stat.Web.Reports.NoRight"
    EnableViewState="false" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <h1>
            对不起，你没有权限！</h1>
        <h3>
            请按以下步骤定位问题：</h3>
        <h3>
            1、先确认是否有当前报表的权限，并且至少拥有一个查看该报表的产品权限。</h3>
        <h3>
            2、使用REPORT系统有IP限制，请先确认你当前出口IP是多少，在百度搜索框中输入"IP"就可以查看的到。</h3>
        <h3>
            3、如果当前的IP在REPORT限定IP列表内，请检查是否使用代理或浏览器插件。</h3>
    </div>
    </form>
</body>
</html>
