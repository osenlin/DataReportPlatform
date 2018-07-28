<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Log.aspx.cs" Inherits="net91com.Stat.Web.UserRights.Log" EnableEventValidation="false" EnableViewStateMac="false" %>

<%@ Register Assembly="AspNetPager" Namespace="Wuqi.Webdiyer" TagPrefix="webdiyer" %>
<%@ Import Namespace="net91com.Reports.UserRights" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>日志</title>
    <script src="../Scripts/jquery-1.4.1.min.js" type="text/javascript"></script>
    <link href="../css/Style.css" rel="stylesheet" type="text/css" />
    <link href="../css/version_1.css" rel="stylesheet" type="text/css" />
    <script src="../My97DatePicker/WdatePicker.js" type="text/javascript"></script>
</head>
<body>
    <form id="form1" runat="server">
        <div class="mcol2">
            <div class="mcol2_cnt">
                <h3 class="t3">用户管理</h3>
                <div id="Background">
                    <div class="large" id="Content">
                        <div class="RightColumn">
                            <div id="formGarden">

                                <table id="rightTable" class="contextTable" cellspacing="0" border="0" width="100%"
                                    style="border-collapse: collapse;">
                                    <tr>
                                        <td>系统：<asp:DropDownList ID="ddlSystems" runat="server"></asp:DropDownList>
                                            搜索关键字：<asp:TextBox runat="server" ID="txtKeyword" MaxLength="50"></asp:TextBox>
                                            操作时间：<input type="text" runat="server" id="txtStartTime" onclick="WdatePicker()" onkeypress="return false" oncopy="return false" />
                                            至<input type="text" runat="server" id="txtEndTime" onclick="WdatePicker()" onkeypress="return false" oncopy="return false" />
                                            <asp:Button runat="server" ID="btnSreach" Text="查询" OnClick="btnSreach_Click" /></td>
                                    </tr>
                                    <tr>
                                        <td align="left" colspan="2">

                                            <table cellspacing="0" cellpadding="4" border="1" bordercolor="#7C7C94" style="color: Black; border-color: #CCCCCC; border-collapse: collapse;">
                                                <tr class="ListHeader" style="white-space:nowrap;color:black;">
                                                    <td>账号
                                                    </td>
                                                    <td>用户类型</td>
                                                    <td>真实姓名
                                                    </td>
                                                    <td>操作页面
                                                    </td>
                                                    <td style="width: 80px;">IP地址
                                                    </td>
                                                    <td>操作时间
                                                    </td>
                                                    <td>操作描述
                                                    </td>
                                                </tr>
                                                <asp:Repeater runat="server" ID="repeaData">
                                                    <AlternatingItemTemplate>
                                                        <tr class="ListRow" style="white-space:nowrap;">
                                                            <td>
                                                                <%#Eval("Account")%>
                                                            </td>
                                                            <td>
                                                                <%#Eval("AccountType")%>
                                                            </td>
                                                            <td><%#Eval("TrueName") %></td>
                                                            <td>
                                                                <%#Eval("PageUrl")%>
                                                            </td>
                                                            <td>
                                                                <%#Eval("IP")%>
                                                            </td>
                                                            <td>
                                                                <%# ((DateTime)Eval("AddTime")).ToString("yyyy-MM-dd HH:mm:ss") %>
                                                            </td>
                                                            <td>
                                                                <%#Eval("Memo")%>
                                                            </td>

                                                        </tr>
                                                    </AlternatingItemTemplate>
                                                    <ItemTemplate>
                                                        <tr class="ListAlternatingRow" style="white-space:nowrap;">
                                                            <td>
                                                                <%#Eval("Account")%>
                                                            </td>
                                                            <td>
                                                                <%#Eval("AccountType")%>
                                                            </td>
                                                            <td><%#Eval("TrueName") %></td>
                                                            <td>
                                                                <%#Eval("PageUrl")%>
                                                            </td>
                                                            <td>
                                                                <%#Eval("IP")%>
                                                            </td>
                                                            <td>
                                                                <%# ((DateTime)Eval("AddTime")).ToString("yyyy-MM-dd HH:mm:ss") %>
                                                            </td>
                                                            <td>
                                                                <%#Eval("Memo")%>
                                                            </td>

                                                        </tr>
                                                    </ItemTemplate>
                                                </asp:Repeater>
                                            </table>
                                            <webdiyer:AspNetPager ID="AspNetPager1" runat="server" AlwaysShow="True"
                                                FirstPageText="首页" ShowCustomInfoSection="Right" PageSize="15"
                                                LastPageText="尾页" NextPageText="下一页" OnPageChanged="AspNetPager1_PageChanged"
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
