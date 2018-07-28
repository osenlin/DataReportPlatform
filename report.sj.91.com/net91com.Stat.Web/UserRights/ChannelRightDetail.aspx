<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ChannelRightDetail.aspx.cs" Inherits="net91com.Stat.Web.UserRights.ChannelRightDetail" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<%@ Import Namespace="net91com.Reports.UserRights" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
     <link href="../css/version_1.css" rel="stylesheet" type="text/css" />
    <link rel="stylesheet" href="/Scripts/zTree/css/zTreeStyle.css" type="text/css" />
    <script src="/Scripts/jquery-1.4.1.min.js" type="text/javascript"></script>
    <script src="/Scripts/zTree/js/jquery.ztree.core-3.2.min.js" type="text/javascript"></script>
    <script src="/Scripts/zTree/js/jquery.ztree.excheck-3.2.min.js" type="text/javascript"></script>
    <script type="text/javascript">
    var setting = {
                            view: {
                                showIcon: false
                            },
                           callback:{                                   
                            } ,
                            check: {
                                enable: true, 
                                chkboxType: { 'Y' : '', 'N' : '' }
                            }, data: {
                                simpleData: {
                                    enable: true
                                }
                            }};
   
    var zNodeData = <%=nodeStr %>;
    $(function(){         
         loadTree(zNodeData);
    });
    
    function GetSelect(selectSoftId) {
        var selectedCategories = [];
        var selectedCustomers = [];
        var zTree = $.fn.zTree.getZTreeObj("treedemo");
        var nodes = zTree.transformToArray(zTree.getNodes());
        var softId = selectSoftId == undefined ? $("#selSofts").val() : selectSoftId;
        var categoriesString = $("#hiddencateids_" + softId).val() + ",";
        var customersString = $("#hiddenchannelids_" + softId).val() + ",";
        for (var i = 0;i < nodes.length; i++) {
            if (nodes[i].type == 1) {
                if (categoriesString.indexOf(nodes[i].val + ",") != -1) {
                    categoriesString = categoriesString.replace(nodes[i].val + ",", "");
                }
                if (nodes[i].checked) {
                    selectedCategories.push(nodes[i].val);
                } 
            } else if (nodes[i].type == 2) {
                if (customersString.indexOf(nodes[i].val + ",") != -1) {
                    customersString = customersString.replace(nodes[i].val + ",", "");
                }
                if (nodes[i].checked) {
                    selectedCustomers.push(nodes[i].val);
                } 
            }
        }
        if (categoriesString == "-1,") {
            categoriesString = "";
            customersString = "";
        }
        categoriesString += selectedCategories.join(",");
        customersString += selectedCustomers.join(",");
        $("#hiddencateids_" + softId).val(categoriesString);
        $("#hiddenchannelids_" + softId).val(customersString);
    }

    function OpenParent(node,zTree)
    {
         var  parent=node.getParentNode();
         if(parent)
         { 
            zTree.expandNode(parent,true,false,false); 
         }
    }  
    function loadTree(data) {
           var zTree=  $.fn.zTree.init($("#treedemo"), setting, data);
           var nodes = zTree.getCheckedNodes(true);
           if(nodes)
           {
                for (var i = 0; i < nodes.length; i++) {
                    OpenParent(nodes[i],zTree);
                }
           }
    }
    function search() 
    {
        //搜索前先保存选择的产品
        GetSelect(curSoftId);
        var keystr = $.trim($("#searchinput").val());
        if (keystr == "") {
            loadTree(zNodeData);
        }
        else {

            var rex = new RegExp(".*" + keystr.toLowerCase() + ".*");
            var tempData = [];
            for (var i = 0, j = zNodeData.length; i < j; i++) {
                var node = zNodeData[i];
                if (node.name.toLowerCase().search(rex) != -1) {
                    if (!tempData.Contains(node)) {
                         tempData.push(node);
                    }
                    GetPanrenNode(node, tempData);
                    GetChanildNode(node, tempData);

                }
            }
            tempData = tempData.sort(function (obj) { if (obj.pId == 0) { return 0; } else { return -1; } });
            loadTree(tempData);

            var zTree = $.fn.zTree.getZTreeObj("treedemo");
            zTree.expandAll(true);
        }
        
    }

     function GetPanrenNode(node, data) {
        var keystr = $.trim($("#treesearchtxt").val());
        for (var i = 0, j = zNodeData.length; i < j; i++) {
            if (zNodeData[i].id == node.pId) {
                if (!data.Contains(zNodeData[i])) {
                    data.push(zNodeData[i]);
                    GetPanrenNode(zNodeData[i], data);
                }
            }
        }
    }
    function GetChanildNode(node, data) {
        var keystr = $.trim($("#treesearchtxt").val());
        var rex = new RegExp(".*" + keystr.toLowerCase() + ".*");
        for (var i = 0, j = zNodeData.length; i < j; i++) {
            if (zNodeData[i].pId == node.id && zNodeData[i].name.toLowerCase().search(rex) != -1) {
                if (!data.Contains(zNodeData[i])) {
                    data.push(zNodeData[i]);
                    GetChanildNode(zNodeData[i], data);
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

    Array.prototype.Exists = function (obj) {
        var b = false;
        for (var i = 0, j = this.length; i < j; i++) {
            if (obj == this[i]) {
                b = true;
                break;
            }
        }
        return b;
    }

    var curSoftId = <%= UserSofts[0].ID %>;
    function selectSofts() {
        //在改变软件时，暂存选择的渠道
        GetSelect(curSoftId);
        var softId = $("#selSofts").val();
        $.getJSON("ChannelRightDetail.aspx?act=getchannels&userId=<%= Request["userId"] %>&softId=" + softId + "&rnd=" + encodeURIComponent(new Date()),
            function(result) {
                if (result != undefined) {
                    zNodeData = result;   
                    if ($("#hiddencateids_" + softId).val() != "-1") {
                        var selectedCateIds = $("#hiddencateids_" + softId).val().split(',');
                        var selectedChannelIds = $("#hiddenchannelids_" + softId).val().split(',');
                        for (var i = 0; i < zNodeData.length; i++) {  
                            if ((zNodeData[i].type==1 && selectedCateIds.Exists(zNodeData[i].val)) 
                                || (zNodeData[i].type==2 && selectedChannelIds.Exists(zNodeData[i].val)))
                                zNodeData[i].checked = true;
                            else
                                zNodeData[i].checked = false;
                        }
                    }                                         
                    loadTree(zNodeData);
                }
            });
        curSoftId = softId;
    }

   
  
    </script>
 
</head>
<body>
    <form id="form1" runat="server">
    <div class="mcol2">
          <% foreach (Soft soft in UserSofts)
           { %>  
        <input type="hidden" id="hiddencateids_<%= soft.ID %>" name="hiddencateids_<%= soft.ID %>" value="-1" />
        <input type="hidden" id="hiddenchannelids_<%= soft.ID %>" name="hiddenchannelids_<%= soft.ID %>" />  
        <%} %>      
        可选软件：
        <select id="selSofts" name="selSofts" onchange="selectSofts();">
        <% foreach (Soft soft in UserSofts)
           { %>
            <option value="<%= soft.ID %>"><%= soft.Name %></option>
        <%} %>
        </select>
        <input type="text" id="searchinput" /> <input type="button" value="搜索" style=" margin:10px;"  class="primaryAction" onclick="search()" />
         <ul id="treedemo" class="ztree"> 
         </ul>
         <div>
            <table>
                <tr>
                    <td>
                     <asp:Button ID="Ok"   CssClass="primaryAction"  runat="server" Text="确定"   OnClick="Ok_Click" OnClientClick="GetSelect()" />
                    </td>
                    <td>
                    <input type="button" value="返回" style=" margin:10px;"  class="primaryAction" onclick="location='<%= ReturnUrl %>';" />
                    </td>
                </tr>
            </table>
            
             
              
         </div>
    </div>
    </form>
</body>
</html>
