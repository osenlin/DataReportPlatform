﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EditSystem.aspx.cs" Inherits="net91com.Stat.Web.UserRights.EditSystem"
    EnableViewStateMac="false" EnableEventValidation="false" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>添加/修改系统</title>
    <link href="../css/version_1.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript">
        var focusBgColor = "#FFFFFF"; //文本框获得焦点时的背景色
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div class="mcol2">
        <div class="mcol2_cnt">
            <h3 class="t3">
                系统管理</h3>
            <div id="Background">
                <div class="large" id="Content">
                    <div class="RightColumn">
                        <div id="formGarden">
                            <table cellspacing="0" cellpadding="0" border="0">
                                <% if (SystemID > 0)
                                    { %>
                                <tr>
                                    <td height="30" align="right" class="td2">
                                        ID：
                                    </td>
                                    <td class="tdline">
                                        <asp:TextBox ID="txtId" runat="server" Width="280px" Font-Bold="True" CssClass="required" Enabled="false"></asp:TextBox>
                                    </td>
                                </tr>
                                <%} %>
                                <tr>
                                    <td height="30" align="right" class="td2">
                                        名称：
                                    </td>
                                    <td class="tdline">
                                        <asp:TextBox ID="txtName" runat="server" Width="280px" Font-Bold="True" CssClass="required"></asp:TextBox>
                                    </td>
                                </tr>
                                <% if (SystemID > 0)
                                    { %>
                                <tr>
                                    <td height="30" align="right" class="td2">
                                        MD5Key：
                                    </td>
                                    <td class="tdline">
                                        <asp:TextBox ID="txtMd5Key" runat="server" Width="280px" Font-Bold="True" CssClass="required" Enabled="false"></asp:TextBox>
                                    </td>
                                </tr>
                                <%} %>
                                <tr>
                                    <td height="30" align="right" class="td2">
                                        地址：
                                    </td>
                                    <td class="tdline">
                                        <asp:TextBox ID="txtUrl" runat="server" Width="280px" Font-Bold="True" CssClass="required"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td height="30" align="right" class="td2">
                                        描述：
                                    </td>
                                    <td class="tdline">
                                        <asp:TextBox ID="txtDescription" runat="server" Width="280px" TextMode="MultiLine"
                                            Font-Bold="True" CssClass="required" Rows="5"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td height="30" align="right" class="td2">
                                        状态：
                                    </td>
                                    <td class="tdline">
                                        <asp:RadioButtonList ID="rblstatus" runat="server" Width="136px" RepeatDirection="Horizontal"
                                            RepeatLayout="Flow">
                                            <asp:ListItem Value="1" Selected="True">启用</asp:ListItem>
                                            <asp:ListItem Value="0">禁用</asp:ListItem>
                                        </asp:RadioButtonList>
                                    </td>
                                </tr>
                                <tr>
                                    <td height="30" colspan="2" align="center">
                                        <br />
                                        <asp:Button ID="btnSave" runat="server" CssClass="primaryAction" Text="保   存" OnClick="btnSave_Click">
                                        </asp:Button>&nbsp;&nbsp;&nbsp;
                                        <input class="primaryAction" type="button" value="返    回" onclick="javascript:location.href='<%= ReturnUrl %>'" />
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
