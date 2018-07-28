<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ChannelRightManager.aspx.cs"
    Inherits="net91com.Stat.Web.UserRights.ChannelRightManager" %>

<%@ Import Namespace="net91com.Reports.UserRights" %>
<%@ Import Namespace="System.Collections.Generic" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <link href="../../Scripts/zTree/css/zTreeStyle.css" rel="stylesheet" type="text/css" />
    <link href="../../css/manage.css" rel="stylesheet" type="text/css" />
    <script src="../../Scripts/jquery-1.5.2.min.js" type="text/javascript"></script>
    <script src="../../Scripts/zTree/js/jquery.ztree.core-3.2.min.js" type="text/javascript"></script>
    <title>渠道商管理</title>
    <script type="text/javascript">
     var zNodes=<%= ChannelTreeJson %>;
     var setting = {
                            view: {
                                showIcon: false
                            },
                           callback:{    
                              onClick:selectChannel                               
                            } ,
                            check: {
                                enable: true, 
                                chkboxType: { 'Y' : '', 'N' : '' }
                            }, data: {
                                simpleData: {
                                    enable: true
                                }
                            }};
       //初始化设置
       var zTree;
       $(function(){          
          zTree=  $.fn.zTree.init($("#treedemo"), setting, zNodes);
       });
       function search() 
    {
        var keystr = $.trim($("#usersearch").val());
        if (keystr == "") {
            zTree=  $.fn.zTree.init($("#treedemo"), setting, zNodes);
        }
        else {

            var rex = new RegExp(".*" + keystr.toLowerCase() + ".*");
            var tempData = [];
            for (var i = 0, j = zNodes.length; i < j; i++) {
                var node = zNodes[i];
                if (node.name.toLowerCase().search(rex) != -1) {
                    if (!tempData.Contains(node)) {
                         tempData.push(node);
                    }
                    GetPanrenNode(node, tempData);
                    GetChanildNode(node, tempData);

                }
            }
            tempData = tempData.sort(function (obj) { if (obj.pId == 0) { return 0; } else { return -1; } });
            
            zTree=  $.fn.zTree.init($("#treedemo"), setting, tempData);
            zTree = $.fn.zTree.getZTreeObj("treedemo");
            zTree.expandAll(true);
        }
        
    }
         function GetPanrenNode(node, data) {
        var keystr = $.trim($("#usersearch").val());
        for (var i = 0, j = zNodes.length; i < j; i++) {
            if (zNodes[i].id == node.pId) {
                if (!data.Contains(zNodes[i])) {
                    data.push(zNodes[i]);
                    GetPanrenNode(zNodes[i], data);
                }
            }
        }
    }
    function GetChanildNode(node, data) {
        var keystr = $.trim($("#usersearch").val());
        var rex = new RegExp(".*" + keystr.toLowerCase() + ".*");
        for (var i = 0, j = zNodes.length; i < j; i++) {
            if (zNodes[i].pId == node.id && zNodes[i].name.toLowerCase().search(rex) != -1) {
                if (!data.Contains(zNodes[i])) {
                    data.push(zNodes[i]);
                    GetChanildNode(zNodes[i], data);
                }
            }
        }
    }

    Array.prototype.Contains = function (obj) {
        var b = false;
        for (var i = 0, j = this.length; i < j; i++) {
            if (obj.id == this[i].id) {
                b = true;
                break;
            }
        }
        return b;
    }
    function selectChannel(event, treeId, treeNode) {
        curTreeNode = treeNode;
        $.getJSON("ChannelRightManager.aspx?act=GetChannelUserIds&softId=<%= CurrentSoft.ID %>&channelType=" + treeNode.type + "&channelId=" + treeNode.val + "&rnd=" + encodeURIComponent(new Date()),
            function(result) {
                for (var i = 0; i < <%= Users.Count %>; i++) {
                    $("#chk_" + i).attr("checked", false);
                    var userId = $("#chk_" + i).attr("userId");
                    for (var j = 0; j < result.length; j++) {
                        if (result[j]==userId) {
                            $("#chk_" + i).attr("checked", true);
                            break;
                        }
                    }
                }
            });
    }
    var curTreeNode;
    function changeUserChannelRights(chk) {
        var act = $(chk).attr("checked") ? "AddUserChannelRight" : "DeleteUserChannelRight";
        $.getJSON("ChannelRightManager.aspx?act=" + act + "&userId=" + $(chk).attr("userId") + "&softId=<%= CurrentSoft.ID %>&channelType=" + curTreeNode.type + "&channelId=" + curTreeNode.val + "&rnd=" + encodeURIComponent(new Date()),
            function(result) {
                if (result.state!=1) {
                    $(chk).attr("checked", !$(chk).attr("checked"));
                }
            });
    }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div class="main">
        <div class="left">
            <div>
                可选软件：
                <asp:DropDownList ID="ddlSoft" Width="120" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlSoft_SelectedIndexChanged">
                </asp:DropDownList>
                <input type="text" value="" style="width: 130px;" id="usersearch" />
                <input type="button" class="btnclass" onclick="search()" value="搜索" />
            </div>
            <ul id="treedemo" class="ztree">
            </ul>
        </div>
        <div class="right">
            <table width="100%">
                <%
                    int xCount = Users.Count % 4 == 0 ? Users.Count / 4 : Users.Count / 4 + 1;
                   for (int i = 0; i < xCount; i++)
                   {%>
                <tr>
                    <%
                        for (int j = 0; j < 4 && i * 4 + j < Users.Count; j++)
                        {
                            User user = Users[i * 4 + j];  %>
                    <td>
                        <input type="checkbox" id="chk_<%= i * 4 + j %>" userid="<%= user.ID %>" onclick="changeUserChannelRights(this);" /><%= string.IsNullOrEmpty(user.TrueName) ? user.Account : user.TrueName %>
                    </td>
                    <%}%>
                </tr>
                <% } %>
            </table>
        </div>
    </div>
    </form>
</body>
</html>
