<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EtlStateManage.aspx.cs" Inherits="net91com.Stat.Web.Tools.EtlStateManage"
    EnableEventValidation="false" EnableViewStateMac="false" %>
<%@ Import Namespace="net91com.Reports.Tools" %>
<%@ Register Assembly="AspNetPager" Namespace="Wuqi.Webdiyer" TagPrefix="webdiyer" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>统计状态管理</title>
    <script src="../Scripts/jquery-1.4.1.min.js" type="text/javascript"></script>
    <link href="../css/Style.css" rel="stylesheet" type="text/css" />
    <link href="../css/version_1.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
        .Grid td a
        { margin-left:6px;
            
            }
    </style>
    <script type="text/javascript" language="javascript">
        function add() {
            var type = $("#ddlTypes").val();
            location.href = "EditEtlState.aspx?type=" + type + "&ReturnUrl=<%= ReturnUrl %>";
        }
        function eidt(id, type) {
            location.href = "EditEtlState.aspx?id=" + id + "&type=" + type + "&ReturnUrl=<%= ReturnUrl %>";
        }
        function dele(id) {
            if (confirm("您确定要删除吗？")) {
                $.getJSON("Service.aspx?id=" + id + "&act=detlstate&n=" + Math.random(), function (obj) {
                    alert(obj.Message);
                    if (obj.Code == 0)
                        location.href = location.href;
                });
            } 
        }
        function search() {
            var type = $("#ddlTypes").val();
            var keyword = $("#txtKeyword").val();
            location = "EtlStateManage.aspx?type=" + type + "&page=1&pageSize=15&keyword=" + encodeURIComponent(keyword);
        }
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
                            <table id="rightTable" class="contextTable" cellspacing="0" border="0" width="100%"
                                style="border-collapse: collapse;">
                                <tr>
                                    <td align="left">
                                        分类：<asp:DropDownList ID="ddlTypes" runat="server"></asp:DropDownList>
                                        KEY：<input type="text" id="txtKeyword" name="txtKeyword" maxlength="50" value="<%= Keyword %>" />
                                        <input type="button" id="btnSearch" value="查询" onclick="search();" />
                                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                        <a href="javascript:add();">添加统计状态</a><br />
                                        <table class="Grid" cellspacing="0" cellpadding="4" border="1" bordercolor="#7C7C94"
                                            style="color: Black; border-color: #CCCCCC; border-collapse: collapse;">
                                            <tr class="ListHeader" style="color: Black">
                                                <td>Key</td>
                                                <td>
                                                    Value
                                                </td>
                                                <td>
                                                    分类
                                                </td>
                                                <td>添加时间</td>
                                                <td>
                                                    描述
                                                </td>                                                
                                                <td style="width: 180px;">
                                                    操作
                                                </td>
                                            </tr>
                                            <asp:Repeater runat="server" ID="repeaData">
                                                <AlternatingItemTemplate>
                                                    <tr class="ListRow" style="text-align:left;">
                                                        <td>
                                                            <%#Eval("Key")%>
                                                        </td>
                                                        <td>
                                                            <%#Eval("Value")%>
                                                        </td>
                                                        <td>
                                                            <%# typeDict[(int)Eval("Type")] %>
                                                        </td>
                                                        <td>
                                                            <%# ((DateTime)Eval("AddTime")).ToString("yyyy-MM-dd HH:mm:ss") %>
                                                        </td>                                                        
                                                        <td>
                                                            <%#Eval("Description")%>
                                                        </td>                                                        
                                                        <td>
                                                            <%# GetOpHtml((EtlState)Container.DataItem) %>
                                                        </td>
                                                    </tr>
                                                </AlternatingItemTemplate>
                                                <ItemTemplate>
                                                    <tr class="ListAlternatingRow" style="text-align:left;">
                                                        <td>
                                                            <%#Eval("Key")%>
                                                        </td>
                                                        <td>
                                                            <%#Eval("Value")%>
                                                        </td>
                                                        <td>
                                                            <%# typeDict[(int)Eval("Type")] %>
                                                        </td>
                                                        <td>
                                                            <%# ((DateTime)Eval("AddTime")).ToString("yyyy-MM-dd HH:mm:ss") %>
                                                        </td>                                                        
                                                        <td>
                                                            <%#Eval("Description")%>
                                                        </td>                                                        
                                                        <td>
                                                            <%# GetOpHtml((EtlState)Container.DataItem) %>
                                                        </td>
                                                    </tr>
                                                </ItemTemplate>
                                            </asp:Repeater>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                            <webdiyer:AspNetPager ID="AspNetPager1" runat="server" FirstPageText="首页" PageSize="15"
                                            LastPageText="末页" NextPageText="下一页" EnableUrlRewriting="true" UrlRewritePattern="EtlStateManage.aspx?type=%type%&page={0}&pagesize=%pagesize%&keyword=%keyword%"
                                            PrevPageText="上一页" CustomInfoHTML="共%PageCount%页，当前为第%CurrentPageIndex%页">
                                        </webdiyer:AspNetPager>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    </form>
</body>
</html>
