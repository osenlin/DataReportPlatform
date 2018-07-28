<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RoleManage.aspx.cs" Inherits="net91com.Stat.Web.UserRights.RoleManage"
    EnableEventValidation="false" EnableViewStateMac="false" %>
<%@ Import Namespace="net91com.Reports.UserRights" %>
<%@ Register Assembly="AspNetPager" Namespace="Wuqi.Webdiyer" TagPrefix="webdiyer" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>用户角色管理</title>
    <script src="../Scripts/jquery-1.4.1.min.js" type="text/javascript"></script>
    <link href="../css/Style.css" rel="stylesheet" type="text/css" />
    <link href="../css/version_1.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
        .Grid td a
        { margin-left:6px;
            
            }
    </style>
    <script type="text/javascript" language="javascript">
        function eidt(id) {
            location.href = "EditRole.aspx?rid=" + id + "&sysId=<%= SelectedSysID %>&sysName=<%= Server.UrlEncode(SelectedSysName) %>&ReturnUrl=<%= ReturnUrl %>";
        }
        function dele(id) {
            if (confirm("您确定要删除吗？")) {
                $.getJSON("Service.aspx?rid=" + id + "&act=drole&n=" + Math.random(), function (obj) {
                    alert(obj.Message);
                    if (obj.Code == 0)
                        location.href = location.href;
                });
            }
        }
        function eidtright(id, name, sysId) {
            location.href = "RoleRightManage.aspx?roleId=" + id + "&name=" + encodeURIComponent(name) + "&sysId=" + sysId + "&ReturnUrl=<%= ReturnUrl %>";
        }
        function eidtsoftright(id, name) {
            location.href = "RoleSoftRightManage.aspx?roleId=" + id + "&name=" + encodeURIComponent(name) + "&ReturnUrl=<%= ReturnUrl %>";
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div class="mcol2">
        <div class="mcol2_cnt">
            <h3 class="t3">
                用户角色管理</h3>
            <div id="Background">
                <div class="large" id="Content">
                    <div class="RightColumn">
                        <div id="formGarden">
                            <table id="rightTable" class="contextTable" cellspacing="0" border="0" width="100%"
                                style="border-collapse: collapse;">
                                <tr>
                                    <td align="left">
                                        系统：<asp:DropDownList ID="ddlSystems" runat="server"></asp:DropDownList>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                        <a href="EditRole.aspx?sysId=<%= SelectedSysID %>&sysName=<%= Server.UrlEncode(SelectedSysName) %>&ReturnUrl=<%= ReturnUrl %>">添加角色</a><br />
                                        <table class="Grid" cellspacing="0" cellpadding="4" border="1" bordercolor="#7C7C94"
                                            style="color: Black; border-color: #CCCCCC; border-collapse: collapse;">
                                            <tr class="ListHeader" style="color:Black">
                                                <td>ID</td>
                                                <td>
                                                    名称
                                                </td>
                                                <td>类型</td>
                                                <td>
                                                    描述
                                                </td>
                                                <td style="width: 80px;">
                                                    状态
                                                </td>
                                                <td style="width: 180px;">
                                                    操作
                                                </td>
                                            </tr>
                                            <asp:Repeater runat="server" ID="repeaData">
                                                <AlternatingItemTemplate>
                                                    <tr class="ListRow">
                                                        <td>
                                                            <%#Eval("ID")%>
                                                        </td>
                                                        <td>
                                                            <%#Eval("Name")%>
                                                        </td>
                                                        <td>
                                                            <%#Eval("RoleType")%>
                                                        </td>
                                                        <td>
                                                            <%#Eval("Description")%>
                                                        </td>
                                                        <td>
                                                            <%# (StatusOptions)Eval("Status") == StatusOptions.Valid ? "启用" : "禁用" %>
                                                        </td>
                                                        <td>
                                                            <%# GetOpHtml((Role)Container.DataItem) %>
                                                        </td>
                                                    </tr>
                                                </AlternatingItemTemplate>
                                                <ItemTemplate>
                                                    <tr class="ListAlternatingRow">
                                                        <td>
                                                            <%#Eval("ID")%>
                                                        </td>
                                                        <td>
                                                            <%#Eval("Name")%>
                                                        </td>
                                                        <td>
                                                            <%#Eval("RoleType")%>
                                                        </td>
                                                        <td>
                                                            <%#Eval("Description")%>
                                                        </td>
                                                        <td>
                                                            <%# (StatusOptions)Eval("Status") == StatusOptions.Valid ? "启用" : "禁用" %>
                                                        </td>
                                                        <td>
                                                            <%# GetOpHtml((Role)Container.DataItem) %>
                                                        </td>
                                                    </tr>
                                                </ItemTemplate>
                                            </asp:Repeater>
                                        </table>
                                        <webdiyer:AspNetPager ID="AspNetPager1" runat="server" AlwaysShow="True" FirstPageText="首页" PageSize="15"
                                            EnableUrlRewriting="true" UrlRewritePattern="RoleManage.aspx?status=%status%&page={0}&pagesize=%pagesize%&keyword=%keyword%"
                                            LastPageText="末页" NextPageText="下一页" PrevPageText="上一页" CustomInfoHTML="共%PageCount%页，当前为第%CurrentPageIndex%页">
                                        </webdiyer:AspNetPager>
                                    </td>
                                </tr>
                            </table>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    </form>
</body>
</html>
