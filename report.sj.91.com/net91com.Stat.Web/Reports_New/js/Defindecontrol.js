var allplat = { "1": "iPhone", "2": "WM", "3": "S60", "4": "Android", "5": "Oms", "7": "IPAD", "255": "PC", "8": "WP7", "9": "AndroidPad", "10": "AndroidTV","12":"Win8" };
function createTabs(myul, defaultindex) {
    var mytabs = new Object();
    mytabs.index = defaultindex;
    myul.addClass("bottunDiv");
    myul.children().eq(defaultindex).addClass("hover");
    mytabs.click = function (index) { 
        myul.children().eq(index).children().eq(0).click();
    
    };
    mytabs.getindex = function() {
        var a = 0;
        myul.children().each(function(index) {
            if ($(this).hasClass("hover")) {
                a = index;
            }
        });
        return a;
    };
    var childlength = myul.children().length;
    myul.children().each(function (index, element) {
        if (index == 0) {
            $(element).children().eq(0).addClass("bottunfirst");
        }
        else if (index == childlength - 1) {
            $(element).children().eq(0).addClass("bottunlast");
        }
        else
            $(element).children().eq(0).addClass("bottun");
        $(element).bind({
            click: function () {

                $(element).parent().children().removeClass("hover");
                $(this).addClass("hover");
            }
        });
    });


    return mytabs;

}
//创建渠道树
function createChannelTree(id,soft,platform,serverurl,setting) {
    var tree = {
        soft: soft,
        platform: platform,
        //绑定控件的tree的id
        desid: id,
        //请求数据接口地址
        serverurl: serverurl,
        //是否展示channelid 1表示true0表示false
        showChannelId: 0,
        setting: setting,
        zNodeData: {},
        searchkey: "",
        controlTree: {},
        callBack: {},
        selectNodes: [],
        defaultSelectNodeId: "",
        needExpandDefaultNode: false,
        advanceSearchObj: null,
        needadvanceSearch: false,
        showIndex: null,
        getTree: function () {
            var url = "";
            if (tree.serverurl && tree.serverurl != "")
                url = tree.serverurl;
            else
                url = "/Reports_New/HttpService.ashx?service=UtilityService&do=getchanneltree&needAttributes=1";
            $.getJSON(url,
                {
                    "softs": tree.soft, "n": Math.random(), "showChannelId": tree.showChannelId, "platform": tree.platform
                },
                function (data) {
                    if (data.resultCode == 0) {
                        tree.zNodeData = eval('(' + data.data + ')');
                        {
                            tree.controlTree = $.fn.zTree.init($("#" + tree.desid), tree.setting, tree.zNodeData);
                            tree.advanceSearchObj = null;
                            //回调
                            tree.callBack();
                        }
                    } else {
                        alert(data.message);
                    }

                });
        },
        rebuildTree: function (json) {
            tree.controlTree = $.fn.zTree.init($("#" + tree.desid), tree.setting, json);
            tree.callBack();
        },
        searchTree: function (expandAll, refreshTree) {
            tree.advanceSearchObj = null;
            if (refreshTree != null && refreshTree) {
                tree.getTree();
            }
            var zNodeData = tree.zNodeData;
            if (tree.searchkey == "") {
                tree.rebuildTree(zNodeData);
            } else {
                var rex = new RegExp(".*" + tree.searchkey.toLowerCase() + ".*");
                var tempData = [];
                for (var i = 0, j = zNodeData.length; i < j; i++) {
                    var node = zNodeData[i];
                    if (node.name.toLowerCase().search(rex) != -1) {
                        if (!tempData.treeContains(node)) {
                            tempData.push(node);
                        }
                        tree.GetParentNode(node, tempData);
                        tree.GetChildNode(node, tempData);

                    }
                }
                tempData = tempData.sort(function (obj) {
                    if (obj.pId == 0) {
                        return 0;
                    } else {
                        return -1;
                    }
                });
                tree.rebuildTree(tempData);
                tree.controlTree.expandAll(expandAll);
            }
        },
        advanceSearchTree: function (expandAll) {
            var objdemo = tree.advanceSearchObj;
            var zNodeData = tree.zNodeData;
            var tempData = [];
            for (var i = 0, j = zNodeData.length; i < j; i++) {
                var node = zNodeData[i];
                if (getEqual(node, objdemo)) {
                    if (!tempData.treeContains(node)) {
                        tempData.push(node);
                    }
                    tree.GetParentNode(node, tempData);
                }
            }
            tempData = tempData.sort(function (obj) {
                if (obj.pId == 0) {
                    return 0;
                } else {
                    return -1;
                }
            });
            tree.rebuildTree(tempData);
            tree.controlTree.expandAll(expandAll);

        },
        getTreeFromAdvance: function () {
            var url = "";
            if (tree.serverurl && tree.serverurl != "")
                url = tree.serverurl;
            else
                url = "/Reports_New/HttpService.ashx?service=UtilityService&do=getchanneltree&needAttributes=1";
            $.getJSON(url,
                {
                    "softs": tree.soft, "n": Math.random(), "showChannelId": tree.showChannelId, "platform": tree.platform
                },
                function (data) {
                    if (data.resultCode == 0) {
                        tree.zNodeData = eval('(' + data.data + ')');
                        {
                            tree.controlTree = $.fn.zTree.init($("#" + tree.desid), tree.setting, tree.zNodeData);

                            if (tree.advanceSearchObj != null)
                            //重新高级搜索tree
                                tree.advanceSearchTree(true);
                            //回调
                            tree.callBack();
                        }
                    } else {
                        alert(data.message);
                    }

                });
        },
        GetParentNode: function (node, data) {
            var zNodeData = tree.zNodeData;
            var keystr = tree.searchkey;
            for (var i = 0, j = zNodeData.length; i < j; i++) {
                if (zNodeData[i].id == node.pId) {
                    if (!data.treeContains(zNodeData[i])) {
                        data.push(zNodeData[i]);
                        tree.GetParentNode(zNodeData[i], data);
                    }
                }
            }
        },
        GetChildNode: function (node, data) {
            var zNodeData = tree.zNodeData;
            var keystr = tree.searchkey;
            var rex = new RegExp(".*" + keystr.toLowerCase() + ".*");
            for (var i = 0, j = zNodeData.length; i < j; i++) {
                if (zNodeData[i].pId == node.id && zNodeData[i].name.toLowerCase().search(rex) != -1) {
                    if (!data.treeContains(zNodeData[i])) {
                        data.push(zNodeData[i]);
                        tree.GetChildNode(zNodeData[i], data);
                    }
                }
            }
        }
    };
    return tree;
}

function getEqual(node, objdemo) {
    //若过滤条件没有设置则返回true
    if (!objdemo)
        return true;
    var result0 = true;
    var result1 = true;
    var result2 = true;
    var result3 = true;
    var result4 = true;
    var result5 = true;
    var result6 = true;
    var result7 = true;
    var result8 = true;
    var result9 = true;
    //找出未标记属性的节点
    if (objdemo.needFlag!=null) {
        result0= node.needFlag == true;
    }
    if (objdemo.channelAdminIDName != "") {
        result1 = node.channelAdminIDName == objdemo.channelAdminIDName;
    }
    if (objdemo.ChannelTypeName != "") {
        result2 = node.ChannelTypeName == objdemo.ChannelTypeName;
    }
    if (objdemo.cooperationModeName != "") {
        result3 = node.cooperationModeName == objdemo.cooperationModeName;
    }
    if (objdemo.exchangeTypeName != "") {
        result4 = node.exchangeTypeName == objdemo.exchangeTypeName;
    }
    if (objdemo.firstLevelChannelCateName != "") {
        result5 = node.firstLevelChannelCateName == objdemo.firstLevelChannelCateName;
    }
    if (objdemo.secondLevelChannelCateName != "") {
        result6 = node.secondLevelChannelCateName == objdemo.secondLevelChannelCateName;
    }
    if (objdemo.promoteModeName != "") {
        result7 = node.promoteModeName == objdemo.promoteModeName;
    }
    if (objdemo.cooperateIDName != "") {
        result8 = node.cooperateIDName == objdemo.cooperateIDName;
    }
    //匹配名称
    var rex = new RegExp(".*" + objdemo.searchkey.toLowerCase() + ".*");
    if (node.name.toLowerCase().search(rex) == -1) {
        result9 = false;
    }
    //所有条件符合则返回true
    if (result1 && result2 && result3 && result4 && result5 && result6 && result7 && result8 && result9 && result0) {
        return true;
    }
}

Array.prototype.treeContains = function(obj) {
    var b = false;
    for (var i = 0, j = this.length; i < j; i++) {
        if (obj.id == this[i].id) {
            b = true;
            break;
        }
    }
    return b;
};

/*初始化时间*/
function initValue(begintimeid,endtimeid,begintimespan,endtimespan) {
    // 设置默认时间段
    var date = new Date();
    date.setDate(date.getDate() + endtimespan);
    var y = date.getFullYear();
    var M = "0" + (date.getMonth() + 1);
    M = M.substring(M.length - 2);
    var d = "0" + date.getDate();
    d = d.substring(d.length - 2);
    $("#" + endtimeid).val(y + "-" + M + "-" + d);
    
    var date2 = new Date();
    date2.setDate(date2.getDate() + begintimespan);
    y = date2.getFullYear();
    M = "0" + (date2.getMonth() + 1);
    M = M.substring(M.length - 2);
    d = "0" + date2.getDate();
    d = d.substring(d.length - 2);
    $("#" + begintimeid).val(y + "-" + M + "-" + d);
}
/*检查数据的正确性*/
function checkDataRight() {
    
    if ($("#beginTime").val() == "") {
        alert("请选择开始日期");
        return false;
    }
    if ($("#endTime").val() == "") {
        alert("请选择结束日期");
        return false;
    }
    var s = $("#beginTime").val().split('-');
    var sdate = new Date(s[0], s[1] - 1, s[2]);
    var e = $("#endTime").val().split('-');
    var edate = new Date(e[0], e[1] - 1, e[2]);
    // 开始日期与结束日期的时间差
    var t = edate.getTime() - sdate.getTime();
    if (t < 0) {
        alert("开始日期不能大于结束日期");
        return false;
    }
    if (t / (3600 * 24 * 1000) > 93) {
        alert("开始日期与结束日期相差不能大于90天");
        return false;
    }
    return true;
}

/*检查数据的正确性*/
function checkDataRight2(maxdays, beginid, endid) {

    if ($("#" + beginid).val() == "") {
        alert("请选择开始日期");
        return false;
    }
    if ($("#" + endid).val() == "") {
        alert("请选择结束日期");
        return false;
    }
    var s = $("#" + beginid).val().split('-');
    var sdate = new Date(s[0], s[1] - 1, s[2]);
    var e = $("#" + endid).val().split('-');
    var edate = new Date(e[0], e[1] - 1, e[2]);
    // 开始日期与结束日期的时间差
    var t = edate.getTime() - sdate.getTime();
    if (t < 0) {
        alert("开始日期不能大于结束日期");
        return false;
    }
    if (t / (3600 * 24 * 1000) >= maxdays) {
        alert("开始日期与结束日期相差不能大于" + maxdays + "天");
        return false;
    }
    return true;
}

/*设置浏览器高度*/
function resetsize(frameid) {
    var dvalue = 0;
//    var height = 0;
//    var iframe = parent.document.getElementById(frameid);
//    alert(navigator.userAgent);
//    if (navigator.userAgent.indexOf("Chrome")!=-1) {
//        height = iframe.contentDocument.body.scrollHeight;
//    }
//    if (navigator.userAgent.indexOf("Firefox") != -1) {
//        height = iframe.contentDocument.body.scrollHeight;
//    }
//    if (navigator.userAgent.indexOf("Safari") != -1) {
//        height = iframe.contentDocument.body.scrollHeight;
//    }
//    else {
//        height = iframe.contentWindow.document.body.scrollHeight;
    //    }
    var height = document.body.clientHeight;
    
    if (height < 700)
        height = 700;
    parent.document.getElementById(frameid).height = height + 100;
}

function strTrim(a) {
    return a.replace(/(\s+)|(\++)|(\-+)|(\\+)|(\/+)/g, "");
}

function tableStyleSet(id) {

    $("#" + id + "  tr").mouseover(function () {
        $(this).attr("style", "background:none repeat scroll 0 0 #73A3CC;cursor:pointer");

    });
    $("#" + id + "  tr").mouseout(function () {
        $(this).attr("style", "");
    });
}
function getTimeString(date) {
    var y = date.getFullYear();
    var M = "0" + (date.getMonth() + 1);
    M = M.substring(M.length - 2);
    var d = "0" + date.getDate();
    d = d.substring(d.length - 2);
    return (y + "-" + M + "-" + d);
}

function getTimeByString(timeString) {
    var s = timeString.split('-');
    return new Date(s[0], s[1] - 1, s[2]);
}


function isNotPc(softid) {
    if (softid != 9 && softid != 105550 && softid != 72 && softid != 57 && softid != 68 && softid != 69) {
        return true;
    }
    else {
        return false;
    }
}
