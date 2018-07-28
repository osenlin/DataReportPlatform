<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EditEtlState.aspx.cs" Inherits="net91com.Stat.Web.Tools.EditEtlState"
    EnableViewStateMac="false" EnableEventValidation="false" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>添加/修改统计状态</title>
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
                统计状态管理</h3>
            <div id="Background">
                <div class="large" id="Content">
                    <div class="RightColumn">
                        <div id="formGarden">
                            <table cellspacing="0" cellpadding="0" border="0">
                                <tr>
                                    <td height="30" align="right" class="td2">
                                        Key：
                                    </td>
                                    <td class="tdline">
                                        <asp:TextBox ID="txtKey" runat="server" Width="280px" Font-Bold="True" CssClass="required"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td height="30" align="right" class="td2">
                                        Value：
                                    </td>
                                    <td class="tdline">
                                        <asp:TextBox ID="txtValue" runat="server" Width="280px" Font-Bold="True" CssClass="required"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td height="30" align="right" class="td2">
                                        分类：
                                    </td>
                                    <td class="tdline">
                                        <asp:DropDownList ID="ddlTypes" runat="server">                                            
                                        </asp:DropDownList>
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
