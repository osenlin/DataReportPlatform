<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ChannelCustomerManager.aspx.cs"
    Inherits="net91com.Stat.Web.Reports_New.R_UserAnalysis.ChannelCustomerManager" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>渠道商管理</title>
    <link href="/Reports_New/css/site.css?version=1" rel="stylesheet" type="text/css" />
    <link href="/Scripts/zTree/css/zTreeStyle.css" rel="stylesheet" type="text/css" />
    <link href="/Reports_New/css/defindecontrol.css" rel="stylesheet" type="text/css" />
    <link href="/Reports_New/css/chosen.css" rel="stylesheet" type="text/css" />
    <link href="/Reports_New/css/colorbox.css" rel="stylesheet" type="text/css" />
    <link href="../css/jquery.datatables.table.css" rel="stylesheet" type="text/css" />
    <script src="/Reports_New/js/jquery-1.6.4.min.js" type="text/javascript"></script>
    <script src="/Reports_New/js/Defindecontrol.js?version=13" type="text/javascript"></script>
    <script src="../js/chosen.jquery.min.js" type="text/javascript"></script>
    <script src="/Reports_New/js/jquery.colorbox-min.js" type="text/javascript"></script>
    <script src="/Scripts/zTree/js/jquery.ztree.core-3.2.min.js" type="text/javascript"></script>
    <script src="/Scripts/zTree/js/jquery.ztree.excheck-3.2.min.js" type="text/javascript"></script>
    <script src="/Scripts/zTree/js/jquery.ztree.exedit-3.2.min.js" type="text/javascript"></script>
    <style type="text/css">
        .catetable
        {
            border-collapse: collapse;
            margin: 0 auto;
            width: 100%;
            height: 80%;
            border-left: 1px solid #cccccc;
            border-right: 1px solid #cccccc;
        }
        .catetable td
        {
            padding: 5px;
            border-style: solid;
            border-color: #cccccc;
            border-width: 1px 0;
        }
    </style>
    <script type="text/javascript">
        var serverUrl = '../HttpService.ashx?service=ChannelCustomerManager';
        var softPlatJson = <%=SoftPlatformJson %>;
        var platforms = [];
        var colorBox = {};
        var colorBox2;
        var myzTree;
        var zNodeData;
        var tabs;
        var haseditright = <%=HasEditRight ? "true" : "false" %>;
        var setting = {
            view: {
                selectedMulti: false,
                fontCss: setFontCss
            },
            callback: {
                onClick: zTreeOnClick,
                beforeDrag: beforeDrag,
                beforeDrop: beforeDrop
            },
            data: {
                simpleData: {
                    enable: true
                }
            },
            edit: {
                enable: haseditright,
                showRemoveBtn: false,
                showRenameBtn: false
            }
        }; 
        function setFontCss(treeId, treeNode) {
	        return treeNode.needFlag&&treeNode.type==2 ? {color:"red"} : {};
        };
 
        //节点着陆
        function beforeDrop(treeId, treeNodes, targetNode, moveType) {
            if (moveType != "inner" || !targetNode) {
                alert("拖拽的目标不规范");
                return false;
            }  
            else if (confirm("你确认将该渠道商放入到目标位置中？(该渠道商下所有子渠道也将受影响)")) {
                 changeChannelPosion(targetNode.val, targetNode.type, treeNodes[0].val, $("#mysoft").val(),treeNodes[0].id);
                return true;
            } else
                return false;
        }
        //位置迁移
        function changeChannelPosion(targetid, targettype, sourceid, softid,sourceNodeID) {
            //alert("目标id:"+targetid+",目标类型:"+targettype+",来源:"+sourceid);
          
            
            $.getJSON(serverUrl, { "targetid": targetid, "targettype": targettype, "sourceid": sourceid, "softs":softid, "do": "changecustomerposition", "rd": Math.random() * 100000 }, function (data) {
                if (data.resultCode == 0) { 
                    
//                    myzTree.defaultSelectNodeId =(targettype==1?"Category_":"Customer_")+ targetid;
//                    myzTree.getTree();
                    //出现名称相同的删去老元素
                    var node = myzTree.controlTree.getNodeByParam("id", sourceNodeID, null);
                    if ($.trim(data.data)  == "") {
                        myzTree.controlTree.removeNode(node);
                        alert(data.message);
                    } else {
                        node.pId = data.data;
                        myzTree.controlTree.updateNode(node);
                        alert(data.message);
                    } 
                   

                } else {
                    alert(data.message);
                }
            });
        }
        //放手的时候触发
        function beforeDrag(treeId, treeNodes) {
            for (var i = 0, l = treeNodes.length; i < l; i++) {
                if (treeNodes[i].drag === false) {
                    return false;
                }
            } 
            return true;
        }
        //ztree 节点点击触发
        function zTreeOnClick(event, treeId, treeNode) {
            setSelectStat();
        };

        function setSelectStat(indexoftab) {
            myzTree.selectNodes = myzTree.controlTree.getSelectedNodes();
            if (myzTree.selectNodes.length != 0) {
                  $("#mychanneltype").val( myzTree.selectNodes[0].type);
                  $("#mychannelid").val( myzTree.selectNodes[0].val); 
                  myzTree.defaultSelectNodeId = myzTree.selectNodes[0].id;
            }
            drowRightDiv(indexoftab);
        }
        //获取包含重复IMEI 设置的内容
        function getrepeatimei(repreat) {
            if (repreat == -1)
                return "默认上级设置";
            else if (repreat == 1)
                return "开启";
            else if (repreat == 0)
                return "关闭";
            else
                return "";
        }
        function setUnable(softid) {
            
            if (isNotPc(softid)) {
                $(".forbiddenclass").each(function () {
                    $(this).hide();
                }); 
            } else {
                 $(".forbiddenclass").each(function () {
                     $(this).show();
                 });
            }
        }
        //初始化入口
        $(function () {
            $("#selectsoft").chosen({ no_results_text: "没有找到匹配" }).change(function () {
                var soft = $("#selectsoft").val();
                platforms = softPlatJson[soft];
                $("#mysoft").val(soft);
             
                setUnable(soft);
                myzTree.soft = $("#mysoft").val();
                myzTree.getTree();
               

            });
            setUnable($("#selectsoft").val());
            colorBox=$("#mywindow").colorbox({ inline: true, width: "30%", height: "30%", speed: 0 });
            colorBox2=$("#mywindow2").colorbox({ inline: true, width: "60%", height: "60%", speed: 10 });
            $("#mysoft").val($("#selectsoft").val());
            platforms = softPlatJson[$("#mysoft").val()];
            $("#selectsearchtype").chosen({ no_results_text: "没有找到匹配", disable_search: true }).change(function () {
                myzTree.rebuildTree( myzTree.zNodeData); 
            });
            myzTree = createChannelTree("treeDemo", $("#mysoft").val(), 0, "", setting);
            myzTree.getTree();
            myzTree.callBack = drowTreeCallBack;
        });
       
        

    </script>
   
</head>
<body>
    <form id="form1" style="padding: 10px" runat="server">
    <div class="maindiv">
        <input type="hidden" id="mysoft" value="" />
        <input type="hidden" id="mychanneltype" value="" />
        <input type="hidden" id="mychannelid" value="" />
        <input type="hidden" id="mycateid" value="" />
        <div class="left">
            <div>
                <% if (HasEditRight)
                   { %>
                <div style="float: left; text-align: left; margin-bottom: 10px;">
                    <a class="mybutton hover " style="cursor: pointer; margin-top: 4px; margin: 2px;"
                        onclick="editcate();"><font>编辑分类>></font></a> <a class="mybutton hover " style="cursor: pointer;
                            margin-top: 4px; margin: 2px;" onclick="addcate();"><font>添加分类>></font></a>
                    <a class="mybutton hover" style="cursor: pointer; margin-top: 4px; margin: 2px;"
                        onclick="deletecate();"><font>删除分类>></font></a>
                </div>
                <% } %>
                <div style="clear: both">
                </div>
                <div>
                    <table>
                        <tr>
                            <td>
                                <select id="selectsoft" style="width: 125px;">
                                    <%=SoftHtml%>
                                </select>
                            </td>
                            <td>
                                <select id="selectsearchtype" style="width: 125px;">
                                    <option value="0">按渠道商查找</option>
                                    <option value="1">按渠道ID查找</option>
                                </select>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <input type="text" value="" class="txtbox" style="width: 130px;" id="usersearch" />
                            </td>
                            <td>
                                <table>
                                    <tr>
                                        <td><a class="mybutton hover" style="cursor: pointer; margin-top: 4px;" onclick="searchTree();">
                                                    <font>查找</font></a> </td>
                                         <td>
                                             
                                             <a class="mybutton hover" style="cursor: pointer; margin-top: 4px;display: none" onclick="advanceSearchTree();">
                                                    <font>高级查找</font></a>
                                         </td>
                                    </tr>

                                </table>
                                
                                
                            </td>
                        </tr>
                    </table>
                    <span style="font-size: 12.5px; margin-left: 2px">特别说明：下面各个渠道商节点支持拖拽进行迁移</span>
                    <div><span style="font-size: 12.5px; margin-left: 2px">红色字体表示渠道属性未设置完全</span></div>
                    
                </div>
            </div>
            <ul id="treeDemo" class="ztree">
            </ul>
        </div>
        <div class="right">
            <div id="div1">
                <div style="margin-top: 4px;" id="titleid" class="title">
                    <strong id="Strong1" class="l">渠道分类操作</strong> <span class="r"></span>
                </div>
                <div class="textbox">
                    <table cellpadding="0" border="0" cellspacing="0" class="display dataTable" id="table1">
                        <thead>
                            <tr role="row" style="">
                                <th>
                                    分类
                                </th>
                                <th>
                                    相关链接
                                </th>
                            </tr>
                        </thead>
                        <tbody id="mybody">
                        </tbody>
                    </table>
                </div>
                <table>
                    <tr>
                        <%if (HasEditRight)
                           { %>
                        <td>
                            <span style="font-size: 12px">添加该分类下渠道商：</span><input class="txtbox" type="text"
                                id="catecustom" />
                        </td>
                        <td>
                            <a class="mybutton hover " style="cursor: pointer; margin-top: 4px;" onclick="addSubCateCustom();">
                                <font>添加</font></a>
                        </td>
                        <% } %>
                    </tr>
                </table>
            </div>
            <div id="div2" style="display: none">
                <div>
                    <ul id="parenttags">
                        <li><a onclick="postUrl('ChannelCustomerManager_Iframe1.aspx')"><font>综合</font></a></li>
                        <li><a onclick="postUrl('ChannelCustomerManager_Iframe2.aspx')"><font>子渠道商</font></a></li>
                        <li><a onclick="postUrl('ChannelCustomerManager_Iframe3.aspx')"><font>渠道ID</font></a></li>
                    </ul>
                </div>
                <iframe id="post_iframe" frameborder="no" border="0" marginwidth="0" marginheight="0"
                    width="100%" height="0" allowtransparency="true"></iframe>
            </div>
        </div>
    </div>
    </form>
      <div style="display: none; padding: 5px">
        <div id="divcateedit">
            <input type="text" style="display: none" id="catesoftid" />
            <input type="text" style="display: none" id="cateid" value="0" />
            <table id="catetable" class="catetable">
                <tr style="height: 40px;">
                    <td>
                        所属软件:
                    </td>
                    <td>
                        <input type="text" class="txtbox" disabled="disabled" id="catesoftname" />
                    </td>
                </tr>
                <tr style="height: 40px;">
                    <td>
                        分类名称:
                    </td>
                    <td>
                        <input type="text" class="txtbox" id="catename" />
                    </td>
                </tr>
                <tr style="height: 50px;">
                    <td>
                    </td>
                    <td>
                        <a class="mybutton hover" style="cursor: pointer; margin-top: 4px; margin-right: 5px;
                            float: right" onclick="save();"><font>保存</font></a>
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <a id="mywindow"></a>
    
    <div style="display: none; padding: 5px">
            <div id="advanceSearchDiv">
                <input type="text" style="display: none" id="customerid" />
                <table id="channeltable" class="catetable">
                    <tr style="height: 40px;">
                        <td colspan="8" style="text-align: center">
                            <h3>高级搜索</h3>
                        </td>
                    </tr>
                    <tr style="height: 50px;">
                        <td colspan="2">
                           渠道商名称:
                        </td>
                         <td colspan="6"  style="vert-align: bottom">
                           <input type="text" class="txtbox" value="" id="searchCustomName" />
                        </td>
                    </tr>
                   <tr>
                       <td colspan="8">
                            <a class="mybutton hover" style="cursor: pointer; margin-right: 5px; float: right;" onclick="advanceSearch(0)">
                                 <font>查找</font></a> 
                        </td>
                   </tr>
                        
                    
                </table>
            </div>
        </div> 
    <a id="mywindow2"></a>

    <script type="text/javascript">
        Array.prototype.contains = function (item) {
            for (var i = 0; i < this.length; i++) {
                if (this[i] == item) {
                    return true;
                }
            }
            return false;
        };
        //实现编辑和添加方法
        function save() {
            if ($.trim($("#catename").val()) == "") {
                alert("请填写好分类名称");
                return;
            }
            $.getJSON(serverUrl, { "do": "editaddcate", "cateid": $("#cateid").val(), "catename": $("#catename").val(), "softs": $("#catesoftid").val() }, function (data) {
                if (data.resultCode == 0) {
                    colorBox.colorbox.close();
                    //colorBox.close();
                    alert(data.message);
                    myzTree.getTree();

                } else {
                    alert(data.message);
                }
            });
        }
        function editcate() {
            if (myzTree.selectNodes.length == 0 || myzTree.selectNodes[0].type != "1") {
                alert("请选择一个分类");
                return;
            }
            var selectnode = myzTree.selectNodes[0];
            var cateid = selectnode.val;
            $("#catesoftname").val($("#selectsoft").find("option:selected").text());
            $("#catesoftid").val($("#selectsoft").val());
            $("#cateid").val(cateid);
            $("#catename").val(selectnode.name);
            $("#mywindow").attr("href", "#divcateedit");
            $("#mywindow").click();

        }
        
        //添加分类
        function addcate() {
            $("#catesoftname").val($("#selectsoft").find("option:selected").text());
            $("#catesoftid").val($("#selectsoft").val());
            $("#cateid").val(0);
            $("#catename").val("");
            $("#mywindow").attr("href", "#divcateedit");
            $("#mywindow").click();
        }

        //删除分类
        function deletecate() {
            if (!confirm("是否确定删除该分类?"))
                return;
            if (myzTree.selectNodes.length == 0 || myzTree.selectNodes[0].type != "1") {
                alert("请选择一个分类");
                return;
            }
            var cateid = myzTree.selectNodes[0].val;
            $.getJSON(serverUrl, { "do": "deletecate", "cateid": cateid, "softs": $("#selectsoft").val() }, function (data) {
                if (data.resultCode == 0) {
                    alert(data.message);
                    myzTree.getTree();
                } else {
                    alert(data.message);
                }
            });
        }
        //高级搜索弹出
        function advanceSearchTree() {
            $("#checkuneditid")[0].checked = false;
            $("#searchCustomName").val("");
            $("#mywindow2").attr("href", "#advanceSearchDiv");
            $("#mywindow2").click();
        }

        function advanceSearch() {
            var obj = new Object();
            if ($("#checkuneditid")[0].checked) {
                obj.needFlag = true;
            } else {
                obj.needFlag = null;
            }
            obj.searchkey = $("#searchCustomName").val();


            if (obj.needFlag == null  && $.trim(obj.searchkey) == "") {
                searchTree();
            } else {
                myzTree.advanceSearchObj = obj;
                myzTree.advanceSearchTree(true);
                setSelectStat();
               
            }
            colorBox2.colorbox.close();
        }


        //添加分类下渠道商
        function addSubCateCustom() {
            var cateid = myzTree.selectNodes[0].val;
            var soft = myzTree.selectNodes[0].softid;
            var name = $.trim($("#catecustom").val());
            if ($.trim(name) == "") {
                alert("请填写好渠道商名称");
                return;
            }
            $.getJSON(serverUrl, { "id": cateid, "do": "addcatecustom", "name": name, "cateid": cateid, "softs": soft, "rd": Math.random() * 10000 }, function (data) {
                if (data.resultCode == 0) {
                    alert(data.message); 
                    myzTree.defaultSelectNodeId = "Customer_" + data.data;
                    myzTree.getTree();
                    //myzTree.needExpandDefaultNode = true;
                    $("#catecustom").val("");
                } else {
                    alert(data.message);
                }
            });
        }

        //绘制tree 后回调方法
        function drowTreeCallBack() {

           
            
            var mynodes = myzTree.controlTree.getNodes(); 
            var node = myzTree.controlTree.getNodeByParam("id", myzTree.defaultSelectNodeId, null); 
            if (mynodes.length > 0) {
                ///默认加载tree 的第一个节点
                if (myzTree.defaultSelectNodeId == "" || !node) {
                    myzTree.controlTree.selectNode(mynodes[0], true);
                    $("#mychanneltype").val(mynodes[0].type);
                    $("#mychannelid").val(mynodes[0].val);
                    node = mynodes[0];
                } else {
                    myzTree.controlTree.selectNode(node, true);
                    $("#mychanneltype").val(node.type);
                    $("#mychannelid").val(node.val);
                }
                myzTree.defaultSelectNodeId = node.id;
                myzTree.selectNodes = myzTree.controlTree.getSelectedNodes();
                //设置是展开默认节点
                if (myzTree.needExpandDefaultNode) {
                    myzTree.controlTree.expandNode(node, true, false, false, false);
                    myzTree.needExpandDefaultNode = false;

                }
                setSelectStat(myzTree.showIndex);
                myzTree.showIndex = null;
            }
            else {
                $("#div1").hide();
                $("#div2").hide();
                myzTree.defaultSelectNodeId = "";
                myzTree.selectNodes = [];
            }
        }

        //重新绘制右边div 展现
        function drowRightDiv(indexoftab) {
            if (myzTree.controlTree.getSelectedNodes().length != 0) {
                var type = $("#mychanneltype").val();
                //按分类
                if (type == "1") {
                    $("#div1").show();
                    $("#div2").hide();
                    fillTableData();
                } else {
                    $("#div2").show();
                    $("#div1").hide();
                    if (!tabs) { //若tabs 不存在
                        tabs = createTabs($("#parenttags"), 0);
                        tabs.click(indexoftab != null ? indexoftab : 0);
                    } else {
                        tabs.click(indexoftab != null ? indexoftab : tabs.getindex());
                    }
                }
            } else {
                $("#div2").hide();
                $("#div1").hide();
            }

        }
        //填充table 中的数据
        function fillTableData() {
            var node = myzTree.selectNodes[0];
            var htmlforcustomtr = "";
            var htmlforneitr = "";
            for (var i = 0; i < platforms.length; i++) {
                htmlforcustomtr += "<a href='javascript:linkDetail(" + node.val + ",1,11," + platforms[i] + ")'>" + allplat[platforms[i]] + "</a>&nbsp;&nbsp;&nbsp;";
                htmlforneitr += "<a href='javascript:linkDetail(" + node.val + ",1,1," + platforms[i] + ")'>" + allplat[platforms[i]] + "</a>&nbsp;&nbsp;&nbsp;";
            }
            ///不区分平台
            htmlforcustomtr += "<a href='javascript:linkDetail(" + node.val + ",1,11," + "0" + ")'>" + "不区分平台" + "</a>&nbsp;&nbsp;&nbsp;";
            htmlforneitr += "<a href='javascript:linkDetail(" + node.val + ",1,1," + "0" + ")'>" + "不区分平台" + "</a>&nbsp;&nbsp;&nbsp;";
            var url = "javascript:linkfenqudao(" + node.val + ", 1)";
            var fentongji = "<tr><td>按子渠道商查询：</td><td  ><a href='" + url + "'>查看" + "</a></td></tr>";
            var all = "<tr><td>" + "统计地址for渠道商" + "</td><td>" + htmlforcustomtr + "</td></tr>" + "<tr><td>统计地址for内部</td><td>" + htmlforneitr + "</td></tr>" + fentongji;
            $("#mybody").html(all);
        }

        //查找树
        function searchTree() {
            var searchKey = $.trim($("#usersearch").val());
            if (searchKey != "" && $("#selectsearchtype").val() == "1") {
                var firstnode;
                $.getJSON(serverUrl, { "softs": $("#mysoft").val(), "do": "searchcustomersbychannelid", "searchkey": searchKey, "rd": Math.random() * 100000 }, function (data) {
                    if (data.resultCode != 0) {
                        alert(data.message);
                        return;
                    }
                    if (data.data.length > 0) {
                        var nodes = myzTree.controlTree.transformToArray(myzTree.controlTree.getNodes());
                        for (var i = 0; i < nodes.length; i++) {
                            if (data.data.contains(nodes[i].val)) {
                                if (!nodes[i].isParent)
                                    myzTree.controlTree.expandNode(nodes[i].getParentNode(), true, false, false, false);
                                if (!firstnode) {
                                    firstnode = nodes[i];
                                    break;
                                }
                            }
                        }
                        if (!firstnode)
                            alert("搜索不到对应渠道ID");
                        else {
                            //控制右边展现
                            myzTree.controlTree.selectNode(firstnode);
                            setSelectStat(2);

                        }
                    } else {
                        alert("搜索不到对应渠道ID");
                    }


                });
            }
            else if (searchKey != "" && $("#selectsearchtype").val() == "0") { //查找渠道商
                myzTree.searchkey = $("#usersearch").val();
                myzTree.searchTree(true);
                setSelectStat();
            } 
            else { 
                myzTree.searchkey = $("#usersearch").val();
                myzTree.searchTree(true,true);
                setSelectStat();
            }


        }

      


        //iframe 链接
        function postUrl(url) {
            $("#post_iframe").attr("src", url);
        }

        //跳到其他页面的方法
        function linkDetail(channelid, channeltype, cate, platid) {
            var father = window.parent;
            ///到时候改就改这个id
            var id = '<%=SjqdUrl %>';
            var url = "Reports/GetMore.aspx?reporttype=13" + "&channelid=" + channelid + "&channeltype=" + channeltype + "&cate=" + cate + "&plat=" + platid;
            ///改这个名称
            var title = '<%=SjqdUrlName %>';
            var menupanleid = "mp" + '<%=SjqdParentUrl %>';
            var menuitem = father.Ext.getCmp(id);
            var menupanel = father.Ext.getCmp(menupanleid);
            father.moveTab(id);
            father.addTab(father.TabPanel1, 'idClt' + id, url, title, menuitem, menupanel);
        }
        ///这个有点特殊左边是没有对应菜单的
        function linkfenqudao(channelid, channeltype) {
            var father = window.parent;
            ///这个id 可以随便取只要不和其他的冲突就可以，应该没有设置到左边菜单
            var id = '111111111111111111111111';
            var url = "Reports/GetMore.aspx?reporttype=14" + "&channelid=" + channelid + "&channeltype=" + channeltype + "&rd=" + Math.random();
            ///改这个名称
            var title = '总渠道数据';
            var menupanleid = "mp" + '<%=SjqdParentUrl %>';
            var menuitem = father.Ext.getCmp(id);
            var menupanel = father.Ext.getCmp(menupanleid);
            father.moveTab(id);
            father.addOtherTab(father.TabPanel1, 'idClt' + id, url, title, menuitem, menupanel);
        }

    </script>
</body>
</html>
