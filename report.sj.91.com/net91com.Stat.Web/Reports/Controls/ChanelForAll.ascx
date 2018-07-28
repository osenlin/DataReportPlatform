<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ChanelForAll.ascx.cs" Inherits="net91com.Stat.Web.Reports.Controls.ChanelForAll" %>
<link rel="stylesheet" href="/Scripts/zTree/css/zTreeStyle.css" type="text/css" />
<link rel="stylesheet" href="/css/ControlsStyle.css?rd=<%= net91com.Stat.Web.Reports.Services.Utility.CssVersion %>" type="text/css" />
<script src="../../Scripts/zTree/js/jquery.ztree.core-3.2.min.js" type="text/javascript"></script>
<script src="../../Scripts/zTree/js/jquery.ztree.excheck-3.2.min.js" type="text/javascript"></script>
 

 
  <div class="divitem" style="width: <%=Width%>; <%=Visible1%>" id="<%=ClientID %>">
    
        <button   id="channelcontrola" style="width:130px; overflow:hidden; height:20px;"   class="ui-multiselect ui-widget ui-state-default ui-corner-all" type="button" aria-haspopup="true" tabindex="0">
        <span class="ui-icon ui-icon-triangle-2-n-s"></span>
        <span id="channelcontrolspan" ><%=title%></span>
        </button>

   </div>
 

<div id="mask" class="lean_overlay"  style="opacity:0.4;filter: alpha(opacity=40);"></div>
<div id="Channel_treeDivBox" class="pop-box" style="width: 500px; height: 450px; overflow:auto">
    <div class="OpenWindow-header " style="color:black">
            <h4>选择渠道</h4>
        </div>
    <div class="pop-box-body">
        
        <input type="text" id="treesearchtxt" style="border:1px solid #B6B6B6; height:20px"  />
        <input type="button" id="btnonsearch" class="btnclass" onclick="javascript:search();"
            value="搜索" />
        &nbsp;
        <input type="button" id="btnoncheck" class="btnclass" value="确定(关闭)" style="width: 80px" />
         <input type="button" id="btncancelcheck" class="btnclass" value="取消所选" style="width: 80px" />
        <div id="loadimg" style="display:none; margin-left:80px;"><img height='25px' src='/Images/defaultloading.gif'><span style="color:Black;"> 正在加载数据...</span></div>
        <div class="zTreeDemoBackground left" style="width: 470px;">
            <ul id="treedemo" class="ztree">
            </ul>
        </div>
        <div id="channeltree" style="width: 97%;">
        </div>
    </div>
</div>
<asp:HiddenField runat="server" ID="hidcheckedValue" />
<asp:HiddenField runat="server" ID="hidcheckedText" />
<asp:HiddenField runat="server" ID="hidcheckedpid" />
<asp:HiddenField runat="server" ID="hidchanneltype" />
<asp:HiddenField runat="server" ID="hidsoftId" />
<asp:HiddenField runat="server" ID="hidplatform" />
<input type="hidden" id="hidloaddata_Channel" name="hidloaddata_Channel" value="<%=HasLoadData %>" />
<input type="hidden" id="hidloadtree_Channel" name="hidloadtree_Channel" value="<%=HasLoadTree %>" />
<input type="hidden" id="hidperiod_Channel" name="hidperiod_Channel" value="<%=HasPeriod %>" />
<script type="text/javascript" language="javascript">
    var maxNumber=<%=MaxCheckNumber %>;   
    var selCount=0;
    var softids_Channel="<%=SoftId %>";
    var platforms_Channel="<%=Platform %>";
    var hasloadData_Channel=true;//<%=HasLoadData %>; 
    var loadTree_Channel=true;//<%=HasLoadTree %>;
    var period_channel=true;//<%=HasPeriod %>;
   function getFont(treeId, node) {
        return node.font ? node.font : {};
    }
    function Expand()
    {
        var zTree = $.fn.zTree.getZTreeObj("treedemo");
        var nodes = zTree.getCheckedNodes(true);
        if (nodes.length > 0) {
	       for(var i=0,j=nodes.length;i<j;i++)
           {
                var node = nodes[i].getParentNode();
                zTree.expandNode(node,true,false,true,false);
           }
        }
        var checkval=$("#<%=ClientID %>_hidcheckedValue").val();
        if(checkval.length>0)
        {
            var vals = checkval.split(',');
            for(var i=0,j=vals.length;i<j;i++)
            {
               for(var z=0,k=zNodeData.length;z<k;z++)
               {
                   if(zNodeData[z].val==vals[i]){
                        //node.checked=true;
                        zTree.selectNode(zNodeData[z]);
                   }
               }
            }
           checkvalue();
        }
       
    }

</script>
<script type="text/javascript" language="javascript">
    var setting = <%=_treeSetting %>;
    var zNodeData = [];
    function clear()
    {
        $("#<%=ClientID %>_hidcheckedValue").val("");
        $("#<%=ClientID %>_hidcheckedpid").val("");
        $("#<%=ClientID %>_hidcheckedText").val("");
        $("#channelcontrolspan").text("选择渠道");
        $("#<%=ClientID %>_hidchanneltype").val("");       
    }
    function TreeBeforeCheck(treeId, treeNode)
    {
        var zTree = $.fn.zTree.getZTreeObj("treedemo");
        var nodes = zTree.getCheckedNodes(true);
        if(!treeNode.checked){
            if(nodes.length>=maxNumber)
            {
                alert("渠道最多只能选择"+maxNumber+"项");
                return false;
            }
            var pid = $("#<%=ClientID %>_hidcheckedpid").val();
            if(pid.length>0&&pid!=(treeNode.pId==null?0:treeNode.pId))
            {
                alert('渠道来源只能选择在同一分类下');
                return false;
            }
        }
        return true;
    }
    function checkvalue()
    {
        var zTree = $.fn.zTree.getZTreeObj("treedemo");
        var nodes = zTree.getCheckedNodes(true);
        ///设置平台
        var ids="";
        var txt="";
        var types="";
        for(var i = 0;i<nodes.length;i++)
        {
            ids+=nodes[i].val+",";
            txt+=nodes[i].name+",";
            types+=nodes[i].type+",";
        }
        txt= txt.substring(0,txt.length-1);
        ids= ids.substring(0,ids.length-1);
        types= types.substring(0,types.length-1); 
        if(ids.length>0)
        {
            $("#<%=ClientID %>_hidcheckedValue").val(ids);
            $("#<%=ClientID %>_hidchanneltype").val(types);
        }
        else
        {
            $("#<%=ClientID %>_hidcheckedValue").val("");
            $("#<%=ClientID %>_hidcheckedpid").val("");
            $("#<%=ClientID %>_hidchanneltype").val("");
        }
        if(txt.length>0)
        {
            $("#<%=ClientID %>_hidcheckedText").val(txt);
            txt=txt.length>8?txt.substring(0,7)+"...":txt;
            $("#channelcontrolspan").text(txt);
        }
        else
        {
            $("#<%=ClientID %>_hidcheckedText").val("");
            $("#channelcontrolspan").text("选择渠道");
        }
    }
    function TreeCheckNodeValue(e, treeId, treeNode)
    {              
        var zTree = $.fn.zTree.getZTreeObj("treedemo");
        var nodes = zTree.getCheckedNodes(true);
        if(treeNode.checked)
        {
            if(treeNode.val==0)
                treeNode.checked=false;
             if(nodes.length>maxNumber)
             {
                treeNode.checked=false;
                 alert("渠道最多只能选择"+maxNumber+"项");
                 return false;
             }
          var pid = $("#<%=ClientID %>_hidcheckedpid").val();
           if(nodes.length==0){
                pid =treeNode.pId==null?0:treeNode.pId;
               $("#<%=ClientID %>_hidcheckedpid").val(pid);
             }
            if(pid.length==0){
                $("#<%=ClientID %>_hidcheckedpid").val(treeNode.pId==null?0:treeNode.pId);
             }
            else
            {
                if(pid.length>0&&pid!=(treeNode.pId==null?0:treeNode.pId)){
                    treeNode.checked=false;
                    alert('渠道来源只能选择在同一分类下');
                    return false;
                }
            } 
        } 
        checkvalue();
    }
    $(function(){
        $("#channelcontrola").click(function(){
             
            <%if(PeriodCheck){ %>         
            if(!period_channel)
            {
                alert("该周期下不能选择渠道");
                return;
            }
            <%} %>
            var reg=/^-?\d+$/;
            if(!reg.test(softids_Channel))
            {
                alert("请选择唯一的软件！");
                return;
            }
            <%if(!IsHasNoPlat){ %>
                if(platforms_Channel=="0")
                {
                     alert("请选择某一个平台！");
                     return;
                }
            <%} %>
            var windowWidth =$(document).outerWidth();
            var windowHeight =$(document).height();
            var popupHeight =  $("#Channel_treeDivBox").height(); 
            var popupWidth =  $("#Channel_treeDivBox").width(); 

            $("body").addClass("hidscroll");
            $("#Channel_treeDivBox").css({left:(windowWidth/1.2-popupWidth)/2,top:100}).show();
            $("#mask").show();
            if(hasloadData_Channel){
                resetTree(softids_Channel,platforms_Channel);
                loadTree_Channel=true;
            }
            if(loadTree_Channel)
            {
                loadTree(zNodeData);
                loadTree_Channel=false;
                hasloadData_Channel=false;
                
            }
        });
        $("#btnoncheck").click(function(){
            // $("#Channel_treeDivBox").animate({left:0,top:0, opacity: "hide" }, "slow");
             $("#Channel_treeDivBox").hide();
             //$("#mask").animate({left:-175,top:-15, opacity: "hide" }, "slow");
            $("#mask").hide();
             $("body").removeClass("hidscroll");
             checkvalue();
        });

        $("#btncancelcheck").click(function(){
           clear();
        });
        function clear()
        {
            var zTree = $.fn.zTree.getZTreeObj("treedemo");
            $("#<%=ClientID %>_hidcheckedValue").val("");
            $("#<%=ClientID %>_hidcheckedpid").val("");
            $("#<%=ClientID %>_hidchanneltype").val("");
            zTree.checkAllNodes(false);
            
        }

    });
</script>
<script type="text/javascript" language="javascript">
    function loadTree(data) {
        hasloadData_Channel = false;
        $.fn.zTree.init($("#treedemo"), setting, data);
        Expand();
    }

    ///按照名称查找节点
    function search() {
        var keystr = $.trim($("#treesearchtxt").val());
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

            clear();
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

    Array.prototype.Contains = function(obj) {
        var b = false;
        for (var i = 0, j = this.length; i < j; i++) {
            if (obj.id == this[i].id) {
                b = true;
                break;
            }
        }
        return b;
    };

    function resetTree(softids, platforms) {
        var ids = $("#<%=ClientID %>_hidcheckedValue").val();
        var types = $("#<%=ClientID %>_hidchanneltype").val();
        var pid = $("#<%=ClientID %>_hidcheckedpid").val();
        $("#loadimg").show();
        $.get("/Reports/HanderForOption.ashx",
        { id: softids_Channel, plat: 0, key: "", types: types, ids: ids, action: "channeltree", "n": Math.random() },
        function (data, status) {
            $("#loadimg").hide();
            if (status == "success") {
                data = eval('(' + data + ')');
                loadTree(data);
                hasloadData_Channel = false;
                zNodeData = data;
                $("#<%=ClientID %>_hidcheckedpid").val(pid);
            }
        });
    }
</script>
