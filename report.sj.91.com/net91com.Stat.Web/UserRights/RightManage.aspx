<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RightManage.aspx.cs" Inherits="net91com.Stat.Web.UserRights.RightManage"
    EnableViewStateMac="false" EnableEventValidation="false" %>

<%@ Import Namespace="net91com.Reports.UserRights" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>权限列表</title>
    <script src="../Scripts/jquery-1.4.1.min.js" type="text/javascript"></script>
    <link href="../css/general.css" rel="stylesheet" type="text/css" />
    <link href="../css/TreeView/treeStyle.css" rel="stylesheet" type="text/css" />
    <link href="../css/Style.css" rel="stylesheet" type="text/css" />
    <link rel="stylesheet" href="/Scripts/zTree/css/zTreeStyle.css" type="text/css" />
    <script src="../Scripts/zTree/js/jquery.ztree.core-3.2.min.js" type="text/javascript"></script>
    <script src="../Scripts/zTree/js/jquery.ztree.excheck-3.2.min.js" type="text/javascript"></script>
    <script type="text/javascript"> 
    var setting={ 
                    view: {
                        showIcon: false
                    }
                    ,callback:{
                        onClick:checkNode} ,data: {
                            key: {
                            checked:'checked'
                        },
                        simpleData: {
                            enable: true
                        }
                    }};

    var zNodeData=<%= GetRightTree() %>;
    $(document).ready(function () {
        $.fn.zTree.init($("#rightTree"), setting, zNodeData);
    });	
    function checkNode(event, id, node) {
            $("#tdkey").text(node.id);
            $("#tdName").text(node.name);
            $("#tdType").text(node.type == 0 ? "分类" : node.type == 1 ? "页面" : "按钮");
            $("#tdURL").text(node._url);
            $("#tdStatus").text(node.status == 1 ? "启用" : "禁用");
            $("#tdDescription").text(node.descript);
            $("#tdOnlyInternal").text(node.onlyinternal);
        }	 
	</script>
</head>
<body>
    <form id="Form1" method="post" runat="server">
    <table id="rightTable" class="contextTable" cellspacing="0" border="0" width="100%">
        <tr>
            <td height="30" colspan="2" align="left">
                <h1 style="background: url(../Images/version_1/t2.gif) no-repeat 0 0; width: 661px;
                    height: 34px; line-height: 34px; font-size: 14px; padding-left: 45px; margin-bottom: 10px;">
                    权限模块管理
                </h1>
            </td>
        </tr>
        <tr>
            <td style="text-align:left;">系统：<asp:DropDownList ID="ddlSystems" runat="server"></asp:DropDownList></td>
        </tr>
        <tr>
            <td>                
                <table cellpadding="0" cellspacing="0" border="1" width="100%" style="border-collapse: collapse;">
                    <tr>
                        <td>
                            <div class="content_wrap">
                                <div class="zTreeDemoBackground left">
                                    <ul id="rightTree" class="ztree">
                                    </ul>
                                </div>
                                <input type="hidden" id="hiddenNodeValue" runat="server" />
                            </div>
                        </td>
                        <td style="width: 70%" valign="top">
                            <table class="contextTable" cellspacing="0" border="0" width="100%" style="border-collapse: collapse">
                                <tr>
                                    <td>
                                        <table cellspacing="0" cellpadding="0" border="0" width="100%" style="font-size: 12px">
                                            <tr>
                                                <td width="80" style="font-weight: bold; font-size: 12px; height: 26px">
                                                    权限ID：
                                                </td>
                                                <td id="tdkey" style="font-size: 12px; height: 26px">
                                                </td>
                                            </tr>
                                            <tr>
                                                <td width="80" style="font-weight: bold; font-size: 12px; height: 26px">
                                                    权限名称：
                                                </td>
                                                <td id="tdName" style="font-size: 12px; height: 26px">
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="font-weight: bold; font-size: 12px; height: 26px">
                                                    权限类型：
                                                </td>
                                                <td id="tdType" style="font-size: 12px; height: 26px">
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="font-weight: bold; font-size: 12px; height: 26px">
                                                    地址/参数：
                                                </td>
                                                <td id="tdURL" style="font-size: 12px; height: 26px">
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="font-weight: bold; font-size: 12px; height: 26px">
                                                    权限状态：
                                                </td>
                                                <td id="tdStatus" style="font-size: 12px; height: 26px">
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="font-weight: bold; font-size: 12px; height: 26px">
                                                    是否仅对内：
                                                </td>
                                                <td id="tdOnlyInternal" style="font-size: 12px; height: 26px">
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="font-weight: bold; font-size: 12px; height: 26px">
                                                    权限描述：
                                                </td>
                                                <td id="tdDescription" style="font-size: 12px; height: 26px">
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="font-weight: bold; font-size: 12px; height: 26px">
                                                    选择操作：
                                                </td>
                                                <td style="font-size: 12px; height: 26px">
                                                    <button type="button" class="defaultButton" id="btnaddp" runat="server">
                                                        添加分类</button>&nbsp;
                                                    <button type="button" class="defaultButton" id="btnadd" runat="server">
                                                        添加权限</button>&nbsp;
                                                    <button type="button" class="defaultButton" id="btnupd" runat="server">
                                                        修改权限</button>&nbsp;
                                                    <button type="button" class="defaultButton" id="btndel" runat="server">
                                                        删除权限</button>&nbsp;
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <iframe id="RoleRightIframe" width="100%" height="100" src="about:blank" frameborder="0"
                                            marginheight="0" marginwidth="0"></iframe>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <iframe id="UserRightIframe" width="100%" height="100" src="about:blank" frameborder="0"
                                            marginheight="0" marginwidth="0"></iframe>
                                        <br />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <font face="宋体"></font>
    </form>
    <script type="text/javascript">
        $("#btnaddp").click(function () {
            location.href = "EditRight.aspx?level=-1&sysId=<%= SelectedSysID %>&sysName=<%= Server.UrlEncode(SelectedSysName) %>&ReturnUrl=<%= ReturnUrl %>";
        });
        $("#btnadd").click(function () {
            var key = $.trim($("#tdkey").text());
            if (key != '') {
                location.href = "EditRight.aspx?level=0&sysId=<%= SelectedSysID %>&sysName=<%= Server.UrlEncode(SelectedSysName) %>&ReturnUrl=<%= ReturnUrl %>&parentKey=" + key;
            }
            else {
                alert("请选择父级权限");
            }
        });
        $("#btnupd").click(function () {
            var key = $.trim($("#tdkey").text());
            if (key != '') {
                location.href = "EditRight.aspx?level=0&sysId=<%= SelectedSysID %>&sysName=<%= Server.UrlEncode(SelectedSysName) %>&ReturnUrl=<%= ReturnUrl %>&rkey=" + key;
            }
            else {
                alert("请选择需修改的权限");
            }
        });
        $("#btndel").click(function () {
            var key = $.trim($("#tdkey").text());
            if (key != '') {
                if (confirm("您确定要删除此权限节点吗？")) {
                    $.getJSON("Service.aspx?rky=" + key + "&act=dright&n=" + Math.random(), function (obj) {
                        alert(obj.Message);
                        if (obj.Code == 0)
                            location.href = location.href;
                    });
                }
            }
            else {
                alert("请选要删除的权限节点");
            }
        });
    </script>
</body>
</html>
