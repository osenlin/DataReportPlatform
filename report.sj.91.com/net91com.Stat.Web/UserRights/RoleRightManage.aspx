<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RoleRightManage.aspx.cs"
    Inherits="net91com.Stat.Web.UserRights.RoleRightManage" EnableEventValidation="false"
    EnableViewStateMac="false" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>设置操作权限</title>
    <link href="../css/version_1.css" rel="stylesheet" type="text/css" />
    <link rel="stylesheet" href="/Scripts/zTree/css/zTreeStyle.css" type="text/css" />
    <script src="../Scripts/jquery-1.4.1.min.js" type="text/javascript"></script>
    <script src="../Scripts/zTree/js/jquery.ztree.core-3.2.min.js" type="text/javascript"></script>
    <script src="../Scripts/zTree/js/jquery.ztree.excheck-3.2.min.js" type="text/javascript"></script>
    <script type="text/javascript"> 
    var setting={   
                            view: {
                                showIcon: false
                            }
                           , check: {
                                enable: true,
                                chkboxType :  { "Y" : "ps", "N" : "s" }
                            }, data: {
                               
                                simpleData: {
                                    enable: true
                                }
                            }};

    var zNodeData = <%= GetRightTree() %>;
    $(document).ready(function () {
        $.fn.zTree.init($("#RightTree"), setting, zNodeData);
    });
    function TreeCheckNodeValue() {
        var zTree = $.fn.zTree.getZTreeObj("RightTree");
        var str="";
        var nodes = zTree.getCheckedNodes(true);
        for(var i = 0;i<nodes.length;i++)
        {
            str+=nodes[i].id+",";
        }        
       $("#SelectedRightIds").val(str.substring(0,str.length-1));
    }
		 
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div class="mcol2">
        <div class="mcol2_cnt">
            <h3 class="t3">
                操作权限设置 &nbsp;&nbsp;&nbsp;<span style="font-weight: normal"><%=PageTitle%></span></h3>
            <div id="Background">
                <div class="large" id="Content">
                    <div class="RightColumn">
                        <div id="formGarden">
                            <div class="content_wrap">
                                系统：<asp:DropDownList ID="ddlSystems" runat="server"></asp:DropDownList>
                                <div class="zTreeDemoBackground left">
                                    <ul id="RightTree" class="ztree">
                                    </ul>
                                </div>
                                <input type="hidden" id="SelectedRightIds" name="SelectedRightIds" />
                            </div>
                            <div style="position: fixed; top: 400px; right: 200px;">
                                <asp:Button runat="server" ID="btnSave" Text="保   存" OnClientClick="TreeCheckNodeValue();"
                                    OnClick="btnSave_Click" class="primaryAction" />
                                &nbsp;&nbsp;&nbsp;<input type="button" value="返   回" onclick="location='<%= ReturnUrl %>';"
                                    class="primaryAction" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    </form>
</body>
</html>
