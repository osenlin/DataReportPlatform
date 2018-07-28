<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UserManage.aspx.cs" Inherits="net91com.Stat.Web.UserRights.UserManage"
    EnableEventValidation="false" EnableViewState="false" EnableViewStateMac="false" %>

<%@ Register Assembly="AspNetPager" Namespace="Wuqi.Webdiyer" TagPrefix="webdiyer" %>
<%@ Import Namespace="net91com.Reports.UserRights" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>用户管理</title>
    <script src="../Scripts/jquery-1.4.1.min.js" type="text/javascript"></script>
    <link href="../css/Style.css" rel="stylesheet" type="text/css" />
    <link href="../css/version_1.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
        .Grid td a
        { margin-left:6px;
            
            }
    </style>
    <script type="text/javascript" language="javascript">
        function eidt(act, id, status) {
            var msg = status == 0 ? "禁用" : "启用";
            if (confirm("您确定要" + msg + "此用户吗？")) {
                $.getJSON("Service.aspx?uid=" + id + "&st=" + status + "&act=" + act + "&n=" + Math.random(), function (obj) {
                    alert(obj.Message);
                    if (obj.Code == 0)
                        location.href = location.href;
                });
            }
        }
        function dele(id, un) {
            if (confirm("您确定要删除吗？")) {
                $.getJSON("Service.aspx?rid=" + id + "&un=" + un + "&act=deleus&n=" + Math.random(), function (obj) {
                    alert(obj.Message);
                    if (obj.Code == 0)
                        location.href = location.href;
                });
            }
        }
        function eidtright(id) {
            location.href = "RoleRightManage.aspx?rid=" + id;
        }
        function search() {
            var status = $("#selStatus").val();
            var accountType = $("#selAccountTypes").val();
            var keyword = $("#txtKeyword").val();
            var sysId = $("#ddlSystems").val();
            
            location = "UserManage.aspx?sysId=" + sysId + "&status=" + status + "&accountType=" + accountType + "&page=1&pageSize=15&keyword=" + encodeURIComponent(keyword) + "&checkOnlyWhite=" + $("#isOnlyWhiteUser")[0].checked;
        }
        <% if (loginService.LoginUser.AccountType == UserTypeOptions.SuperAdmin)
        { %>
        //导出到Excel
        function exportToExcel() {
            var url = 'UserManage.aspx?action=ExportToExcel&rd=' + encodeURIComponent(new Date());
            window.open(url, "_blank", "height=200,width=300");
        }
        <%} %>
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div class="mcol2">
        <div class="mcol2_cnt">
            <h3 class="t3">
                用户管理</h3>
            <div id="Background">
                <div class="large" id="Content">
                    <div class="RightColumn">
                        <div id="formGarden">
                            <table id="rightTable" class="contextTable" cellspacing="0" border="0" width="100%"
                                style="border-collapse: collapse;">
                                <tr>
                                    <td>
                                        系统：<asp:DropDownList ID="ddlSystems" runat="server"></asp:DropDownList>
                                        状态：
                                        <select id="selStatus" name="selStatus">
                                        <option value="1"<%= Status==StatusOptions.Valid?" selected=\"selected\"": "" %>>启用</option>
                                        <option value="0"<%= Status==StatusOptions.Invalid?" selected=\"selected\"": "" %>>禁用</option>
                                        </select>
                                        只显示白名单用户：<input type="checkbox" id="isOnlyWhiteUser" name="checkOnlyWhite" <%=IsOnlyWhiteUser?"checked='checked'":"" %>/>

                                        用户类型：<%= GetSelectHtml() %>
                                        账号或姓名：<input type="text" id="txtKeyword" name="txtKeyword" maxlength="50" value="<%= Keyword %>" />
                                        <input type="button" id="btnSearch" value="查询" onclick="search();" />
                                        <% if (loginService.LoginUser.AccountType == UserTypeOptions.SuperAdmin)
                                            { %>
                                        <input type="button" id="btnExport" value="导出所有用户" onclick="exportToExcel();" />
                                        <%} %>
                                    </td>
                                    <td align="right">
                                        <a href="AddUser.aspx?ReturnUrl=<%= ReturnUrl %>">新增用户</a>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="left" colspan="2">
                                        <table class="Grid" cellspacing="0" cellpadding="4" border="1" bordercolor="#7C7C94"
                                            style="color: Black; border-color: #CCCCCC; border-collapse: collapse;">
                                            <tr class="ListHeader" style="white-space: nowrap; color: Black">
                                                <td>ID</td>
                                                <td>
                                                    用户名
                                                </td>
                                                <td>
                                                    真实姓名
                                                </td>
                                                <td>
                                                    邮箱
                                                </td>
                                                <td>部门</td>
                                                <td>
                                                    最后登录时间
                                                </td>
                                                <td style="width: 80px;">
                                                    状态
                                                </td>
                                                <td style="width: 30%;">
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
                                                            <%#Eval("Account")%>
                                                        </td>
                                                        <td>
                                                            <%#Eval("TrueName") %>
                                                        </td>
                                                        <td>
                                                            <%#Eval("Email") %>
                                                        </td>
                                                        <td><%#Eval("Department") %></td>
                                                        <td>
                                                            <%# ((DateTime)Eval("LastLoginTime")).ToString("yyyy-MM-dd HH:mm:ss") %>
                                                        </td>
                                                        <td>
                                                            <%# (int)Eval("Status") == 1 ? "启用" : "禁用"%>
                                                        </td>
                                                        <td>
                                                            <%# GetOpHtml((User)Container.DataItem)%>
                                                        </td>
                                                    </tr>
                                                </AlternatingItemTemplate>
                                                <ItemTemplate>
                                                    <tr class="ListAlternatingRow">
                                                        <td>
                                                            <%#Eval("ID")%>
                                                        </td>
                                                        <td>
                                                            <%#Eval("Account")%>
                                                        </td>
                                                        <td>
                                                            <%#Eval("TrueName") %>
                                                        </td>
                                                        <td>
                                                            <%#Eval("Email") %>
                                                        </td>
                                                        <td><%#Eval("Department") %></td>
                                                        <td>
                                                            <%# ((DateTime)Eval("LastLoginTime")).ToString("yyyy-MM-dd HH:mm:ss") %>
                                                        </td>
                                                        <td>
                                                            <%# (int)Eval("Status") == 1 ? "启用" : "禁用"%>
                                                        </td>
                                                        <td>                                                           
                                                            <%# GetOpHtml((User)Container.DataItem)%>
                                                        </td>
                                                    </tr>
                                                </ItemTemplate>
                                            </asp:Repeater>
                                        </table>
                                        <webdiyer:AspNetPager ID="AspNetPager1" runat="server" FirstPageText="首页" PageSize="15"
                                            LastPageText="末页" NextPageText="下一页" EnableUrlRewriting="true" UrlRewritePattern="UserManage.aspx?sysId=%sysId%&status=%status%&&accountType=%accountType%&page={0}&pagesize=%pagesize%&keyword=%keyword%"
                                            PrevPageText="上一页" CustomInfoHTML="共%PageCount%页，当前为第%CurrentPageIndex%页">
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
