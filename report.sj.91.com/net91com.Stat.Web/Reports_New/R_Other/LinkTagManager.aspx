<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LinkTagManager.aspx.cs" Inherits="net91com.Stat.Web.Reports_New.R_Other.LinkTagManager" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>跳转标签管理</title>
    <link href="/Reports_New/css/colorbox.css" rel="stylesheet" type="text/css" />
    <link href="/Reports_New/css/defindecontrol.css" rel="stylesheet" type="text/css" />
    <link href="/Reports_New/css/jquery.datatables.table.css" rel="stylesheet" type="text/css" />
    <link href="/Reports_New/css/site.css" rel="stylesheet" type="text/css" />
    <link href="/Scripts/zTree/css/zTreeStyle.css" rel="stylesheet" type="text/css" />
    <link href="/Reports_New/css/chosen.css" rel="stylesheet" type="text/css" />

    <script src="/Reports_New/js/Defindecontrol.js" type="text/javascript"></script>
    <script src="/Reports_New/js/jquery-1.6.4.min.js" type="text/javascript"></script>
    <script src="/Reports_New/js/jquery.colorbox-min.js" type="text/javascript"></script>
    <script src="/Reports_New/js/jquery.dataTables.min.js" type="text/javascript"></script>
    <script src="/Scripts/zTree/js/jquery.ztree.core-3.2.min.js" type="text/javascript"></script>
    <script src="/Scripts/zTree/js/jquery.ztree.excheck-3.2.min.js" type="text/javascript"></script>
    <script src="/Scripts/zTree/js/jquery.ztree.exedit-3.2.min.js" type="text/javascript"></script>
    <script src="/Reports_New/js/chosen.jquery.min.js" type="text/javascript"></script>
    <script src="/Scripts/jquery.json-2.3.min.js" type="text/javascript"></script>

    <script type="text/javascript">
        var serverUrl = '../HttpService.ashx?service=LinkTagManager';
        var zTreeSetting = {
            view: {
                selectedMulti: false
            },
            callback: {
                onClick: zTreeOnClick,
                beforeDrag: zTreeOnBeforeDrag,
                beforeDrop: zTreeOnBeforeDrop
            },
            data: {
                simpleData: {
                    enable: true
                }
            },
            edit: {
                enable: true,
                showRemoveBtn: false,
                showRenameBtn: false
            }
        };
        var zTree;
        var oTable;
        var catewindow = {};
        var tagwindow = {};
        $(function () {

            // 加载平台
            loadPlatform();

            // 加载树
            zTree = $.fn.zTree.init($("#tree1"), zTreeSetting);
            loadTree();

            // 软件下拉更改
            $("#sltSofts").chosen({ no_results_text: "没有找到匹配" }).change(function () {
                // 加载平台
                loadPlatform();

                // 重装分类树
                loadTree();
            });

            // 平台下拉更改
            $("#sltPlatform").chosen({ no_results_text: "没有找到匹配" }).change(function () {
                // 重装表格
                loadTable();
            });

            catewindow = $("#cate_window").colorbox({ inline: true, width: "400", height: "200", speed: 0 });
            tagwindow = $("#tag_window").colorbox({ inline: true, width: "600", height: "300", speed: 0 });
        });

        function loadPlatform() {
            var softPlatJsonObj = eval("(" + $("#softPlatJson").val() + ")");
            var platforms = softPlatJsonObj[$("#sltSofts").val()];
            $('#sltPlatform').empty();
            for (var i = 0; i < platforms.length; i++) {
                $('#sltPlatform').append("<option value='" + platforms[i] + "'>" + allplat[platforms[i]] + "</option>");
            }
            $("#sltPlatform").trigger("liszt:updated");
        }
        
        function createurl() {
            var innerUrl = "https://play.google.com/store/apps/details?id=";
            var identifier = $("#txtIdentifier").val();
            var referrer = "utm_campaign=" + $("#txtChannelId").val() + "&utm_source=felink";
            referrer = "&referrer=" + escape(referrer);            
            $("#txtLinkUrl").val(innerUrl + identifier + referrer);
        }

        function loadTree() {
            $.getJSON(serverUrl, { "do": "getlinktagtree", "softid": $("#sltSofts").val() }, function (data) {
                if (data.resultCode == 0) {
                    var obj = eval('(' + data.data + ')');
                    $("#tree1").empty();
                    zTree = $.fn.zTree.init($("#tree1"), zTreeSetting, obj);
                    // 重装表格
                    loadTable();

                    loadCateList();
                } else {
                    alert(data.message);
                }
            });
        }

        function loadTable() {
            if (oTable) {
                oTable.fnDraw();
            }
            else {
                oTable = $('#table1').dataTable({
                    "aoColumnDefs": [{
                        "bUseRendered": false,
                        "fnRender": function (oObj) {
                            return getLinkType(parseInt(oObj.aData[1]));
                        },
                        "aTargets": [1]
                    }, {
                        "bUseRendered": false,
                        "fnRender": function (oObj) {
                            if (oObj.aData[1] == '3') {
                                var s = oObj.aData[2].split('_');
                                return "http://url.felinkapps.com/down?pid=" + $("#sltSofts").val() + "&mt=" + $("#sltPlatform").val() + "&resid=" + s[1] + "&position=" + s[2];
                            }
                            return "http://url.felinkapps.com/" + oObj.aData[2];
                        },
                        "aTargets": [2]
                    }, {
                        "bUseRendered": false,
                        "fnRender": function (oObj) {
                            return "<a href='javascript:void(0);' onclick='edittag(" + oObj.aData[6] + ");'>修改</a>&nbsp;&nbsp;<a href='javascript:void(0);' onclick='deletetag(" + oObj.aData[6] + ");'>删除</a>";
                        },
                        "aTargets": [6]
                    }],
                    "bProcessing": true,
                    "bServerSide": true,
                    "bFilter": false,
                    "bPaginate": false,
                    "bLengthChange": false,
                    "bSort": false,
                    "bInfo": false,
                    "sAjaxSource": serverUrl,
                    "oLanguage": { sUrl: "/Scripts/de_DE.txt" },
                    "fnServerParams": function (aoData) {
                        var cid = 0;
                        var selectNodes = zTree.getSelectedNodes();
                        if (selectNodes.length > 0) {
                            cid = selectNodes[0].id;
                        }
                        aoData.push({
                            "name": "do", "value": "getlinktaglist"
                        }, {
                            "name": "softid", "value": $("#sltSofts").val()
                        }, {
                            "name": "platform", "value": $("#sltPlatform").val()
                        }, {
                            "name": "cid", "value": cid
                        });
                    },
                    "fnServerData": function (sSource, aoData, fnCallback) {
                        $.ajax({
                            "dataType": 'json',
                            "type": "POST",
                            "url": sSource,
                            "data": aoData,
                            "success": function (data) {
                                if (data.resultCode && data.resultCode != 0) {
                                    alert(data.message);
                                } else {
                                    fnCallback(data.data);
                                }

                            }
                        });
                    }
                });
            }
        }

        function getLinkType(value) {
            switch (value) {
                case 0:
                    return 'URL跳转';
                case 1:
                    return 'AppStore渠道跳转';
                case 2:
                    return '标签跳转';
                case 3:
                    return '下载跳转';
                case 4:
                    return '引流推荐跳转';
            }
        }

        function loadCateList() {
            $('#sltCate').empty();
            $('#catepid').empty();
            var nodes = zTree.getNodes();
            if (nodes.length > 0) {
                for (var i = 0, k = nodes.length; i < k; i++) {
                    eachCateList(nodes[i]);
                }
            }
        }
        
        function eachCateList(node) {
            var name = node.name;
            if (node.level > 0) {
                var padder = new Padder(node.level + 1, "　");
                name = padder.pad('∟') + name;
            }
            $('#sltCate').append("<option value='" + node.id + "'>" + name + "</option>");
            $('#catepid').append("<option value='" + node.id + "'>" + name + "</option>");
            if (node.children && node.children.length > 0) {
                for (var i = 0, k = node.children.length; i < k; i++) {
                    eachCateList(node.children[i]);
                }
            }
        }

        function Padder(len, pad) {
            if (len === undefined) {
                len = 1;
            } else if (pad === undefined) {
                pad = '0';
            }
            var pads = '';
            while (pads.length < len) {
                pads += pad;
            }
            this.pad = function (what) {
                var s = what.toString();
                return pads.substring(0, pads.length - s.length) + s;
            };
        }

        function zTreeOnBeforeDrop(treeId, treeNodes, targetNode, moveType) {
            if (!targetNode) {
                return false;
            }
            var pid = 0;
            if (moveType == 'inner') {
                pid = targetNode.id;
            }
            else if (targetNode.pId != null) {
                pid = targetNode.pId;
            }
            if (confirm("请确认移动到该位置？")) {
                $.getJSON(serverUrl, {
                    "do": "changecate",
                    "id": treeNodes[0].id,
                    "pid": pid
                }, function (data) {
                    if (data.resultCode != 0) {
                        loadTree();
                    }
                });
                return true;
            } else {
                return false;
            }
        }

        function zTreeOnBeforeDrag(treeId, treeNodes) {
            for (var i = 0, l = treeNodes.length; i < l; i++) {
                if (treeNodes[i].drag === false) {
                    return false;
                }
            }
            return true;
        }

        function zTreeOnClick(event, treeId, treeNode) {
            loadTable();
        }

        function savecate() {
            if ($.trim($("#catename").val()) == "") {
                alert("分类名称不可为空");
                return;
            }
            $.getJSON(serverUrl, {
                "do": "editcate2",
                "id": $("#cateid").val(),
                "pid": $("#catepid").val(),
                "name": $("#catename").val(),
                "softid": $("#sltSofts").val()
            }, function (data) {
                if (data.resultCode == 0) {
                    catewindow.colorbox.close();
                    loadTree();
                } else {
                    alert(data.message);
                }
            });
        }
        function editcate() {
            var selectNodes = zTree.getSelectedNodes();
            if (selectNodes.length == 0) {
                alert("请选择分类");
                return;
            }
            $('#divcate').hide();
            var selectnode = selectNodes[0];
            $("#cateid").val(selectnode.id);
            $("#catename").val(selectnode.name);
            $("#catepid").val(0);
            $("#cate_window").attr("href", "#divcateedit");
            $("#cate_window").click();
        }
        function addcate() {
            $('#divcate').show();
            $("#cateid").val(0);
            $("#catepid").val(0);
            $("#catename").val('');
            var selectNodes = zTree.getSelectedNodes();
            if (selectNodes.length > 0) {
                $("#catepid").val(selectNodes[0].id);
            }
            $("#cate_window").attr("href", "#divcateedit");
            $("#cate_window").click();
        }
        function deletecate() {
            var selectNodes = zTree.getSelectedNodes();
            if (selectNodes.length == 0) {
                alert("请选择分类");
                return;
            }
            if (!confirm("请确定要删除该分类？")) {
                return;
            }
            $.getJSON(serverUrl, {
                "do": "deletecate",
                "id": selectNodes[0].id
            }, function (data) {
                if (data.resultCode == 0) {
                    loadTree();
                } else {
                    alert(data.message);
                }
            });
        }

        function changeLinkType() {
            switch ($("#sltLinkType").val()) {
                case '0':
                    $('#divlinkurl').show();
                    $('#divlinktag').hide();
                    $('#divappversion').hide();
                    $('#divchannel').hide();
                    $('#divcustomredirect').hide();
                    $('#divdown').hide();
                    $('#divcustomredirectbtn').hide();
                    break;
                case '1':
                    $('#divlinkurl').show();
                    $('#divlinktag').hide();
                    $('#divappversion').hide();
                    $('#divchannel').show();
                    $('#divcustomredirect').hide();
                    $('#divdown').hide();
                    $('#divcustomredirectbtn').hide();
                    break;
                case '2':
                    $('#divlinkurl').hide();
                    $('#divlinktag').show();
                    $('#divappversion').hide();
                    $('#divchannel').hide();
                    $('#divcustomredirect').hide();
                    $('#divdown').hide();
                    $('#divcustomredirectbtn').hide();
                    break;
                case '3':
                    $('#divlinkurl').show();
                    $('#divlinktag').hide();
                    $('#divappversion').hide();
                    $('#divchannel').hide();
                    $('#divcustomredirect').hide();
                    $('#divdown').show();
                    $('#divcustomredirectbtn').hide();
                    break;
                case '4':
                    $('#divlinkurl').show();
                    $('#divlinktag').hide();
                    $('#divappversion').hide();
                    $('#divchannel').hide();
                    $('#divdown').hide();
                    $('#divcustomredirect').show();
                    $('#divcustomredirectbtn').show();
                    
                    break;
            }
        }
        
        function isInt(str) {
            var reg = /^(-|\+)?\d+$/;
            return reg.test(str);
        }

        function savetag() {
            if ($.trim($("#txtLinkName").val()) == "") {
                alert("链接名称不可为空");
                return;
            }
            var type = $('#sltLinkType').val();
            switch (type) {
                case "1":
                    if ($.trim($("#txtLinkUrl").val()) == "") {
                        alert("目标地址不可为空");
                        return;
                    }
                    if ($.trim($("#txtChannel").val()) == "") {
                        alert("渠道标识不可为空");
                        return;
                    }
                    break;
                case "2":
                    if ($.trim($("#txtLinkTag").val()) == "") {
                        alert("标签不可为空");
                        return;
                    }
                    break;
                case "3":
                    if ($.trim($("#txtLinkUrl").val()) == "") {
                        alert("目标地址不可为空");
                        return;
                    }
                    if ($.trim($("#txtResID").val()) == "") {
                        $('#txtResID').focus();
                        alert("资源id不可为空");
                        return;
                    }
                    if (!isInt($('#txtResID').val())) {
                        $('#txtResID').focus();
                        alert('资源id必须为数字');
                        return;
                    }
                    if ($.trim($("#txtPosition").val()) != "" && !isInt($('#txtPosition').val())) {
                        $('#txtPosition').focus();
                        alert('位置编号必须为数字');
                        return;
                    }
                    break;
                default:
                    if ($.trim($("#txtLinkUrl").val()) == "") {
                        alert("目标地址不可为空");
                        return;
                    }
                    break;

            }
            var obj = new Object();
            obj.ID = parseInt($('#tagid').val());
            obj.SoftID = parseInt($('#sltSofts').val());
            obj.Platform = parseInt($('#sltPlatform').val());
            if (type == "3") {
                var resId = parseInt($('#txtResID').val());
                var position = 0;
                if ($('#txtPosition').val() != "") {
                    position = parseInt($('#txtPosition').val());
                }
                obj.LinkTag = "down_" + resId + "_" + position;
            }
            else {
                obj.LinkTag = $('#txtLinkTag').val();
            }
            obj.LinkName = $('#txtLinkName').val();
            obj.LinkUrl = $('#txtLinkUrl').val();
            obj.LinkType = type;
            obj.AppVersion = $('#txtAppVersion').val();
            obj.Channel = $('#txtChannel').val();

            obj.ChannelId = $('#txtChannelId').val();
            obj.CID = parseInt($('#sltCate').val());
            $.getJSON(serverUrl, {
                "do": "edittag",
                "obj": $.toJSON(obj)
            }, function (data) {
                if (data.resultCode == 0) {
                    tagwindow.colorbox.close();
                    loadTable();
                } else {
                    alert(data.message);
                }
            });
        }
        
        function edittag(id) {
            $.getJSON(serverUrl, {
                "do": "gettag",
                "id": id
            }, function (data) {
                if (data.resultCode == 0) {
                    $("#tagid").val(id);
                    $("#sltCate").val(data.data.CID);
                    $("#sltLinkType").val(data.data.LinkType);
                    changeLinkType();
                    $("#txtLinkTag").val(data.data.LinkTag);
                    $("#txtLinkName").val(data.data.LinkName);
                    $("#txtLinkUrl").val(data.data.LinkUrl);
                    $("#txtAppVersion").val(data.data.AppVersion);
                    $("#txtChannel").val(data.data.Channel);
                    
                    $("#txtChannelId").val(data.data.ChannelId);
                    
                    if (data.data.LinkType == 3) {
                        var s = data.data.LinkTag.split('_');
                        $("#txtResID").val(s[1]);
                        $("#txtPosition").val(s[2]);
                    }
                    $("#tag_window").attr("href", "#divtagedit");
                    $("#tag_window").click();
                } else {
                    alert(data.message);
                }
            });
        }
        
        function addtag() {
            var selectNodes = zTree.getSelectedNodes();
            if (selectNodes.length > 0) {
                $("#sltCate").val(selectNodes[0].id);
            }
            $("#tagid").val(0);
            $("#sltLinkType").val(0);
            changeLinkType();
            $("#txtLinkName").val('');
            $("#txtLinkUrl").val('');
            $("#txtAppVersion").val('');
            $("#txtChannel").val('');
            $("#txtLinkTag").val('');
            $("#txtResID").val('');
            $("#txtPosition").val('');

            $("#txtChannelId").val('');
            
            $("#tag_window").attr("href", "#divtagedit");
            $("#tag_window").click();
        }
        function deletetag(id) {
            if (!confirm("请确定要删除该链接？")) {
                return;
            }
            $.getJSON(serverUrl, {
                "do": "deletetag",
                "id": id
            }, function (data) {
                if (data.resultCode == 0) {
                    loadTable();
                } else {
                    alert(data.message);
                }
            });
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <input id="softPlatJson" runat="server" type="hidden" />
        <div class="maindiv">
            <table cellpadding="0" cellspacing="5" border="0" width="100%">
                <tr>
                    <td style="width: 250px; white-space: nowrap" valign="top">
                        <select id="sltSofts" runat="server">
                        </select>
                        <ul id="tree1" class="ztree">
                        </ul>
                    </td>
                    <td style="width: 100%" valign="top">
                        <table cellpadding="0" cellspacing="0" border="0" width="100%">
                            <tr>
                                <td align="left" style="white-space: nowrap">
                                    <select id="sltPlatform" style="width: 120px">
                                    </select>
                                </td>
                                <td align="left" style="width: 100%">
                                    <a class="mybutton hover" style="cursor: pointer; margin-top: 4px; margin: 2px;"
                                        onclick="addtag();"><font>添加链接</font></a>
                                    <a class="mybutton hover" style="cursor: pointer; margin-top: 4px; margin: 2px;"
                                        onclick="addcate();"><font>添加分类</font></a>
                                    <a class="mybutton hover" style="cursor: pointer; margin-top: 4px; margin: 2px;"
                                        onclick="editcate();"><font>编辑分类</font></a>
                                    <a class="mybutton hover" style="cursor: pointer; margin-top: 4px; margin: 2px;"
                                        onclick="deletecate();"><font>删除分类</font></a>
                                </td>
                            </tr>
                        </table>
                        <div style="clear: both"></div>
                        <table id="table1" cellpadding="0" cellspacing="0" border="0" class="display">
                            <thead>
                                <tr>
                                    <th>链接名称
                                    </th>
                                    <th>类型
                                    </th>
                                    <th>跳转地址
                                    </th>
                                    <th>目标地址
                                    </th>
                                    <th>更新时间
                                    </th>
                                    <th>渠道号
                                    </th>
                                    <th style="width: 120px">操作
                                    </th>
                                </tr>
                            </thead>
                            <tbody>
                            </tbody>
                        </table>
                    </td>
                </tr>
            </table>
        </div>
        <div style="display: none; padding: 5px">
            <div id="divcateedit">
                <input style="display: none" id="cateid" value="0" />
                <table cellpadding="5" border="0" cellspacing="5" width="100%">
                    <tr>
                        <td style="width: 20%">分类名称    
                        </td>
                        <td style="width: 80%">
                            <input class="txtbox" id="catename" maxlength="50" style="width: 200px" />
                        </td>
                    </tr>
                    <tr id="divcate" style="display: none;">
                        <td>上级分类</td>
                        <td>
                            <select id="catepid">
                            </select>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <a class="mybutton hover" style="cursor: pointer; margin-top: 4px; margin-right: 5px;"
                                onclick="savecate();"><font>保存</font></a>
                        </td>
                    </tr>
                </table>
            </div>
            <div id="divtagedit">
                <input style="display: none" id="tagid" value="0" />
                <table cellpadding="5" border="0" cellspacing="5" width="100%">
                    <tr>
                        <td style="width: 20%">链接类型</td>
                        <td style="width: 30%">
                            <select id="sltLinkType" onchange="changeLinkType()">
                                <option value="0" selected="selected">URL跳转</option>
                                <option value="1">AppStore渠道跳转</option>
                                <option value="2">标签跳转</option>
                                <option value="3">下载跳转</option>
                                <option value="4">引荐流跳转</option>
                            </select>
                        </td>
                        <td style="width: 20%">所属分类</td>
                        <td style="width: 30%">
                            <select id="sltCate">
                            </select>
                        </td>
                    </tr>
                    <tr>
                        <td>链接名称</td>
                        <td colspan="3">
                            <input class="txtbox" id="txtLinkName" maxlength="50" style="width: 200px" />
                        </td>
                    </tr>
                    <tr id="divcustomredirect" style="display: none;">
                        <td>渠道号</td>
                        <td >
                            <input class="txtbox"  id="txtChannelId" maxlength="50" />
                        </td>
                        <td>包名</td>
                        <td >
                            <input class="txtbox" id="txtIdentifier" />
                        </td>
                    </tr>
                    <tr>
                        <td></td>
                         <td id="divcustomredirectbtn" style="display: none;" colspan="3">
                            <a class="mybutton hover" style="cursor: pointer; margin-top: 4px; margin-right: 5px;"
                                onclick="createurl();"><font>生成目标地址</font></a>
                        </td>
                    </tr>
                    <tr id="divlinkurl">
                        <td>目标地址</td>
                        <td colspan="3">
                            <input class="txtbox" id="txtLinkUrl" maxlength="1000" style="width: 400px" />
                        </td>
                    </tr>


                    <tr id="divdown" style="display: none;">
                        <td>资源id</td>
                        <td>
                            <input class="txtbox" id="txtResID" maxlength="50" style="width: 100px" />
                        </td>
                        <td>位置编号</td>
                        <td>
                            <input class="txtbox" id="txtPosition" maxlength="50" style="width: 100px" />
                        </td>
                    </tr>
                    <tr id="divlinktag" style="display: none;">
                        <td>标签</td>
                        <td colspan="3">
                            <input class="txtbox" id="txtLinkTag" maxlength="50" style="width: 100px" />
                        </td>
                    </tr>
                    <tr id="divappversion" style="display: none;">
                        <td>版本号</td>
                        <td colspan="3">
                            <input class="txtbox" id="txtAppVersion" maxlength="50" style="width: 100px" />
                        </td>
                    </tr>
                    <tr id="divchannel" style="display: none;" >
                        <td>渠道标识</td>
                        <td colspan="3">
                            <input class="txtbox" id="txtChannel" maxlength="50" style="width: 200px" />
                        </td>
                    </tr>
                    <tr>
                        <td >
                            <a class="mybutton hover" style="cursor: pointer; margin-top: 4px; margin-right: 5px;"
                                onclick="savetag();"><font>保存</font></a>
                        </td>                        

                    </tr>
                </table>
            </div>
        </div>
        <a id="cate_window"></a>
        <a id="tag_window"></a>
    </form>
</body>
</html>