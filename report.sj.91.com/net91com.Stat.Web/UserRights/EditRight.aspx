<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EditRight.aspx.cs" Inherits="net91com.Stat.Web.UserRights.EditRight" EnableViewStateMac="false" EnableEventValidation="false" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>添加/修改权限</title>
    <link href="../css/version_1.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript" language="javascript">
        var focusBgColor = "#FFFFFF"; //文本框获得焦点时的背景色
    </script>
</head>
<body>
    <form id="Form1" method="post" runat="server">
    <div class="mcol2">
        <div class="mcol2_cnt">
            <h3 class="t3">
                权限模块管理</h3>
            <div id="Div1">
                <div class="large" id="Div2">
                    <div class="RightColumn">
                        <div id="Div3">
                            <table cellspacing="0" cellpadding="0" border="0">
                                <tr>
                                    <td align="right" height="30" class="td2">
                                        权限名称：
                                    </td>
                                    <td class="tdline">
                                        <asp:TextBox ID="txtName" runat="server" CssClass="required" Font-Bold="True" Width="232px"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="right" height="30" class="td2">
                                        系统：
                                    </td>
                                    <td class="tdline">
                                        <input type="text" id="txtSysName" name="txtSysName" value="<%= Request.QueryString["sysName"] %>" disabled="disabled" />
                                    </td>
                                </tr>
                                <tr>
                                    <td align="right" height="30" class="td2">
                                        权限类型：
                                    </td>
                                    <td class="tdline">
                                        <font face="宋体">
                                            <asp:RadioButtonList ID="rblType" runat="server" Width="180px" RepeatDirection="Horizontal"
                                                RepeatLayout="Flow" Height="29px" CssClass="required">
                                                <asp:ListItem Value="0">分类</asp:ListItem>
                                                <asp:ListItem Value="1">页面</asp:ListItem>
                                                <asp:ListItem Value="2">按钮</asp:ListItem>
                                            </asp:RadioButtonList>
                                        </font>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="right" height="30" class="td2">
                                        地址/参数：
                                    </td>
                                    <td class="tdline">
                                        <asp:TextBox ID="txtURL" runat="server" CssClass="required"
                                            Width="232px"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="right" height="30" class="td2">
                                        权限排序：
                                    </td>
                                    <td class="tdline">
                                        <table>
                                            <tr>
                                                <td>
                                                    在
                                                </td>
                                                <td>
                                                    <asp:DropDownList ID="ddlSortIndex" runat="server">
                                                    </asp:DropDownList>
                                                </td>
                                                <td>
                                                    <asp:RadioButtonList ID="rblPosition" runat="server" Width="180px" RepeatDirection="Horizontal"
                                                        RepeatLayout="Flow" Height="29px">
                                                        <asp:ListItem Value="0">之前</asp:ListItem>
                                                        <asp:ListItem Value="1" Selected="True">之后</asp:ListItem>
                                                    </asp:RadioButtonList>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="right" height="30" class="td2">
                                        权限状态：
                                    </td>
                                    <td class="tdline">
                                        <asp:RadioButtonList ID="rblStatus" runat="server" Width="120px" RepeatDirection="Horizontal"
                                            RepeatLayout="Flow" Height="29px">
                                            <asp:ListItem Value="1" Selected="True">启用</asp:ListItem>
                                            <asp:ListItem Value="0">禁用</asp:ListItem>
                                        </asp:RadioButtonList>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="right" height="30" class="td2">
                                        是否仅对内：
                                    </td>
                                    <td class="tdline">
                                        <asp:RadioButtonList ID="rblOnlyInternal" runat="server" Width="120px" RepeatDirection="Horizontal"
                                            RepeatLayout="Flow" Height="29px">
                                            <asp:ListItem Value="1" Selected="True">是</asp:ListItem>
                                            <asp:ListItem Value="0">否</asp:ListItem>
                                        </asp:RadioButtonList>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="right" height="30" class="td2">
                                        权限描述：
                                    </td>
                                    <td class="tdline">
                                        <asp:TextBox ID="txtDescription" runat="server" CssClass="required" Width="275px" Rows="5" TextMode="MultiLine"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td height="30" colspan="2" align="center">
                                        <br>
                                        <asp:Button ID="btnSave" runat="server" CssClass="primaryAction" Text="保   存" OnClick="btnSave_Click">
                                        </asp:Button>&nbsp;&nbsp;&nbsp;
                                        <input class="primaryAction" type="button" value="返  回" onclick="javascript:location.href='<%= ReturnUrl %>'" />
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
