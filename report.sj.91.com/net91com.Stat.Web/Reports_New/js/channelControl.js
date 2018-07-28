
var setting = {
    view: {
        showIcon: false
    },
    callback: {
        beforeCheck: zTreeOnCheck
    },
    check: {
        enable: true,
        chkboxType: { 'Y': '', 'N': '' }
    }, data: {
        simpleData: {
            enable: true
        }
    }
};

var channelzTree;
var channelcolorBox;
var channelzTreeMaxSelected = 5;
function searchTree() {
    channelzTree.searchkey = $("#usersearch").val();
    channelzTree.searchTree(true, false);
}

function clearTree  () {
    channelzTree.controlTree.checkAllNodes(false);
}
function zTreeOnCheck(treeId, treeNode) {
    if (!treeNode.checked) {
        var nodes = channelzTree.controlTree.getCheckedNodes(true);
        if (nodes.length >= channelzTreeMaxSelected) {
            alert("仅仅能选"+channelzTreeMaxSelected+"个以下渠道");
            return false;
        }
    }
    return true;
}

function closeTree() {

    var nodes = channelzTree.controlTree.getCheckedNodes(true);
    ///设置平台
    var ids = "";
    var txt = "";
    for (var i = 0; i < nodes.length; i++) {
        ids += nodes[i].id + ",";
        txt += nodes[i].name + ",";

    }
    txt = txt.substring(0, txt.length - 1);
    ids = ids.substring(0, ids.length - 1);
    if (ids.length > 0) {
        $("#myselectchannelid").val(ids);
        $("#" + desId).val(txt);
    }
    else {
        $("#myselectchannelid").val("");
        $("#" + desId).val("");
    }

    $.colorbox.close();
}

var desId;
//maxSelected 最多选几个多选 maxSelected>0
function createChannelBoxControl(des,softid,platform,maxSelected) {
    $("body").prepend(createHiddenHtml());
    if (maxSelected) {
        channelzTreeMaxSelected = maxSelected;
    }
    channelcolorBox = $("#mychannelwindow").colorbox({ inline: true, width: "50%", height: "50%", speed: 0, overlayClose: true, onClosed: closeTree });
    desId = des;

    var channelControl = {
        softid: softid,
        platform: platform,
        needrecreate: 0,
        createNewTree: function () {
            $("#loaddiv").show();
            channelzTree = createChannelTree("treeDemo", channelControl.softid, channelControl.platform, "", setting);
            channelzTree.callBack = function () {
                $("#loaddiv").hide();
            };
            channelzTree.showChannelId = 1;
            channelzTree.getTree();
            $("#myselectchannelid").val("");
            $("#" + desId).val("");

        },
        open: function () {

            $("#" + desId).val("");
            if (channelControl.needrecreate > 0) {
                channelControl.createNewTree();
            }
            channelControl.needrecreate = 0;
            $("#mychannelwindow").attr("href", "#dicchanneldiv");
            $("#mychannelwindow").click();

        },

        getSelectChannels: function () {
            return $("#myselectchannelid").val();
        },
        clearSelectChannels: function () {
            $("#myselectchannelid").val("");
            $("#" + desId).val("");
            if (channelzTree && channelzTree.controlTree && channelzTree.controlTree.checkAllNodes)
                channelzTree.controlTree.checkAllNodes(false);
        }
    };
    $("#" + des).bind("click", channelControl.open);
    return channelControl;
}

function createHiddenHtml() {
    var arrayStr = [], j = 0;        
    arrayStr[j++] = '  <a id="mychannelwindow"></a> <div style="display: none; padding: 5px"><div id="dicchanneldiv">';
    arrayStr[j++] = ' <div id="loaddiv" style="display:none;border: 1px solid #F9FAFC; text-align:left; top:50px; position:absolute; z-index:10; "><img height="25px" src="/Images/defaultloading.gif"/><div>  正在加载数据...</div></div>';
    arrayStr[j++] = '<input type="hidden" value="" id="myselectchannelid"/> ';
    arrayStr[j++] = ' <table> <tr> <td>';
    arrayStr[j++] = ' <input type="text" value="" class="txtbox" style="width: 130px;" id="usersearch" /></td>';
    arrayStr[j++] = '<td><a class="mybutton hover" style="cursor: pointer; margin-top: 4px;" onclick="searchTree();">';
    arrayStr[j++] = '<font>查找</font></a> ';
    arrayStr[j++] = '</td>';
    arrayStr[j++] = '<td>';
    arrayStr[j++] = '<a class="mybutton hover" style="cursor: pointer; margin-top: 4px;" onclick="clearTree();">';
    arrayStr[j++] = '<font>清除所选</font></a>';
    arrayStr[j++] = ' </td>';
    arrayStr[j++] = '<td>';
    arrayStr[j++] = '<a class="mybutton hover" style="cursor: pointer; margin-top: 4px;" onclick="closeTree();">';
    arrayStr[j++] = '<font>确定</font></a>';
    arrayStr[j++] = '</td>';
    arrayStr[j++] = '</tr>';
    arrayStr[j++] = '<tr>';
    arrayStr[j++] = ' <td colspan="3">';
    arrayStr[j++] = '<ul id="treeDemo" class="ztree">';
    arrayStr[j++] = '</ul>  ';
    arrayStr[j++] = '</td>';
    arrayStr[j++] = ' </tr>';
    arrayStr[j++] = ' </table>';
    arrayStr[j++] = '</div> ';
    arrayStr[j++] = '</div> ';
    return arrayStr.join("");
}