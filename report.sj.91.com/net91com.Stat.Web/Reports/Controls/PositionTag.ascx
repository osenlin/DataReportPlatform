<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PositionTag.ascx.cs" Inherits="net91com.Stat.Web.Reports.Controls.PositionTag" %>


<link rel="stylesheet" href="/Scripts/zTree/css/zTreeStyle.css" type="text/css" />
<link rel="stylesheet" href="/css/ControlsStyle.css?rd=<%= net91com.Stat.Web.Reports.Services.Utility.CssVersion %>" type="text/css" />
<link href="/Reports_New/css/colorbox.css" rel="stylesheet" type="text/css" />
<script src="../../Scripts/zTree/js/jquery.ztree.core-3.2.min.js" type="text/javascript"></script>
<script src="../../Scripts/zTree/js/jquery.ztree.excheck-3.2.min.js" type="text/javascript"></script>
<script src="/Reports_New/js/jquery.colorbox.js" type="text/javascript"></script>

<script type="text/javascript">
	<!--
    var setting = {
        check: {
            enable: true
        },
        data: {
            simpleData: {
                enable: true
            }
        },
        callback: {
            onCheck: onCheck
        }
    };

    var zNodes = [
        <%=data %>
    ];

    function onCheck() {
        var zTree = $.fn.zTree.getZTreeObj("positionTagTree"),
        nodes = zTree.getChangeCheckedNodes();
        var txt = [];
        var ids = [];
        for (var i = 0, l = nodes.length; i < l; i++) {
            if (nodes[i].isParent) continue;
            ids.push(nodes[i].id);
            txt.push(nodes[i].name);
        }

        $("#selectedPostionTagText").text(txt.join(','));
        $("#PositionTag_selectPostionTag").val(ids.join(','));

        if (txt.length == 0) {
            $("#selectedPostionTagText").text("不区分栏目位置");
        }
    }

    $(document).ready(function () {
        $.fn.zTree.init($("#positionTagTree"), setting, zNodes);
        $("#PositionTag_selectPostionTag").val("");
        $(".positionTagTreeLink").colorbox({ inline: true, width: "50%", height:"75%" });
    });
    //-->
</script>

<div style="display: none;">
    <ul id="positionTagTree" class="ztree"></ul>
</div>
<input type="hidden" name="selectedPostionTag" id="selectPostionTag" runat="server" />
<div style="position: relative; z-index: 100">
    <a href="#positionTagTree" class="ui-multiselect ui-widget ui-state-default ui-corner-all positionTagTreeLink" aria-haspopup="true" tabindex="0" style="width: 170px; display:block; text-decoration:none;white-space:nowrap; overflow:hidden;">
        <span class="ui-icon ui-icon-triangle-2-n-s"></span><span id="selectedPostionTagText">不区分栏目位置</span>
    </a>
</div>
