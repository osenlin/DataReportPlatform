<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ChannelCustomerManager_Iframe2.aspx.cs"
    Inherits="net91com.Stat.Web.Reports_New.R_UserAnalysis.ChannelCustomerManager_Iframe2" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>渠道商管理_iframe2</title>
    <link href="/Reports_New/css/site.css?version=1" rel="stylesheet" type="text/css" />
    <link href="/Reports_New/css/defindecontrol.css?version=7" rel="stylesheet" type="text/css" />
    <link href="/Reports_New/css/chosen.css" rel="stylesheet" type="text/css" />
    <link href="../css/jquery.datatables.table.css" rel="stylesheet" type="text/css" />
    <link href="/Reports_New/css/colorbox.css" rel="stylesheet" type="text/css" />
    <script src="/Reports_New/js/jquery-1.6.4.min.js" type="text/javascript"></script>
    <script src="/Reports_New/js/Defindecontrol.js?version=13" type="text/javascript"></script>
    <script src="../js/jquery.dataTables.min.js" type="text/javascript"></script>
    <script src="../js/chosen.jquery.min.js" type="text/javascript"></script>
    <script src="/Reports_New/js/jquery.colorbox-min.js" type="text/javascript"></script>
    <style>
        .textbox table td
        {
            text-align: center;
        }
        .textbox table td span
        {
            cursor: pointer;
        }
        .divtable
        {
            border-collapse: collapse;
            margin: 0 auto;
            width: 100%;
            height: 80%;
            border-left: 1px solid #cccccc;
            border-right: 1px solid #cccccc;
        }
        .divtable td
        {
            padding: 5px;
            border-style: solid;
            border-color: #cccccc;
            border-width: 1px 0;
        }
    </style>
    <script type="text/javascript">
        var serverUrl = '../HttpService.ashx?service=ChannelCustomerManager';
        var mysoft = "";
        var mychanneltype = "";
        var mychannelcustomerid = "";
        var colorBox2;
        $(function () {
            mysoft = window.parent.document.getElementById("mysoft").value;
            mychanneltype = window.parent.document.getElementById("mychanneltype").value;
            mychannelcustomerid = window.parent.document.getElementById("mychannelid").value;
            //保证进来的都是渠道商
            if (mychanneltype != "2") {
                mychannelcustomerid = 0;
                return;
            }
            resetsize('post_iframe');
            colorBox2 = $("#mywindow").colorbox({ inline: true, width: "60%", height: "40%", speed: 0 });
            fillBodyData();
        });
        //添加渠道商
        function addcustomer() {
            var name = $("#txtcustomername").val();
            if ($.trim(name) == "") {
                alert("请填写好渠道商名称");
                return;
            }
            $.getJSON(serverUrl, { "softs": mysoft, "cid": 0, "parentcustomerid": mychannelcustomerid, "do": "addcustomer", "name": name, "rd": Math.random() * 100000 }, function (data) {
                if (data.resultCode == 0) {
                    alert(data.message);
                    window.parent.myzTree.defaultSelectNodeId = "Customer_" + data.data;
                    window.parent.myzTree.showIndex = 0;
                    window.parent.myzTree.getTree();
                    
                } else {
                    alert(data.message);
                }
            });
        }
        //全选或者全不选

        function checkall() {
            var checkes = $(".mycheckbox");
            var checked = $("#checkallornot")[0].checked;
            for (var i = 0; i < checkes.length; i++) {
                checkes[i].checked = checked;
            }
        }
        //一次性删除所有选中
        function deleteall() {
            var checkes = $(".mycheckbox");
            var checkedids = "";
            for (var i = 0; i < checkes.length; i++) {
                if (checkes[i].checked) {
                    checkedids += checkes[i].value + ",";
                }
            }
            checkedids = checkedids.substring(0,checkedids.length-1);
            if ($.trim(checkedids) == "") {
                alert("请选择要删除的渠道商");
                return;
            }
            if (confirm("确定删除所勾选渠道商?")) {
                $.getJSON(serverUrl, { "softs": mysoft, "id": checkedids, "do": "deletechannelcustomer", "rd": Math.random() * 100000 }, function (data) {
                    if (data.resultCode == 0) {
                        alert(data.message);
                        window.parent.myzTree.getTree();
                    } else {
                        alert(data.message);
                    }
                });

            }
            
        }

    </script>
</head>
<body>
    <div class="maindiv">
        <a id="mywindow"></a>
        <div style="margin-top: 4px;" id="titleid" class="title">
            <strong id="Strong1" class="l"><input type="checkbox" id="checkallornot" onchange="checkall()" /><label for="checkallornot">全选/取消</label> 子渠道商列表</strong> <span class="r">
                 <table id="table_addsoncustomer">
            <tr>
                <% if (HasEditRight)
                   { %>
                <td style="vertical-align: top;">
                    添加其子渠道商：<input type="text" value="" class="txtbox" id="txtcustomername" />
                </td>
                <td style="vertical-align: top;">
                    <a class="mybutton hover" style="cursor: pointer; margin-right: 5px;" onclick="addcustomer()">
                        <font>添加</font> </a>
                </td>
                
                <td style="vertical-align: top;">
                    <a class="mybutton hover" style="cursor: pointer; margin-right: 5px;" onclick="showBatchCustomer()">
                        <font>自定义添加</font> </a>
                </td>
                <% } %>
            </tr>
        </table>                                              
                                                               
          </span>
        </div>
        <div class="textbox">
            <table cellpadding="0" border="0" cellspacing="0" class="display dataTable" id="table1">
                <thead>
                    <tr>
                        <th>
                            ID
                        </th>
                        <th>
                            名称
                        </th>
                        <th>
                            一次系数 
                        </th>
                        <th>
                            操作
                        </th>
                    </tr>
                </thead>
                <tbody id="mybody">
                </tbody>
            </table>
            <% if (HasEditRight)
               { %>
              <a class="mybutton hover" style="cursor: pointer; margin-right: 5px;" onclick="deleteall()">
                        <font>删除如上勾选渠道</font> </a>
                        <% } %>
        </div>
        
        <div style="display: none; padding: 5px">
            <div id="divCustomBatchAdd">
                <input type="text" style="display: none" id="Text1" />
                <table id="Table2" class="divtable">
                    <tr style="height: 40px;">
                        <td colspan="2">
                         <h3>
                                渠道商批量添加</h3>
                        </td>
                    </tr>
                    <tr style="height: 40px;">
                        <td>
                            名称前缀 :
                        </td>
                        <td>
                            <input type="text" class="txtbox" style="width: 150px;" id="txtCustomName_prefix" />
                        </td>
                    </tr>
                    <tr style="height: 40px;">
                        <td>
                            开始编号(数字):
                        </td>
                        <td>
                            <input type="text" class="txtbox" style="width: 150px;" id="txtNum_begin" />
                        </td>
                    </tr>
                     <tr style="height: 40px;">
                        <td>
                             结束编号(数字):
                        </td>
                        <td>
                            <input type="text" class="txtbox" style="width: 150px;" id="txtNum_end" />
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <a class="mybutton hover" style="cursor: pointer; margin-right: 5px; float: right;" onclick="batchAddcustomer()">
                                 <font>保存</font></a>
                        </td>
                    </tr>
                </table>
            </div>
        </div>
    </div>
    <script type="text/javascript">
        
        function fillBodyData() {
            $("#mybody").html("<tr><td colspan='6'><div  style='text-align:center;'><img height='15px' src='../images/common/defaultloading.gif'/><div>加载中..</div></div></td></tr>");
            $.getJSON(serverUrl, { "parentid": mychannelcustomerid, "softs": mysoft, "do": "getcustomersbyparentid", "rd": Math.random() * 10000 }, function (mydata) {
                
                if (mydata.resultCode == 0) {
                    var tablehtml = "";
                    var arrayStr = [], j = 0;
                    for (var i = 0; i < mydata.data.length; i++) {
                        arrayStr[j++] = "<tr><td><input type=\"checkbox\" class=\"mycheckbox\" value=\""+mydata.data[i].ID +"\"/>";
                        arrayStr[j++] = "<label>"+mydata.data[i].ID +"</label></td><td>";
                        arrayStr[j++] = mydata.data[i].Name + "</td><td>";
                        arrayStr[j++] = mydata.data[i].Modulus1 + "</td>";
                        <% if (HasEditRight)
                           { %>
                        arrayStr[j++] = "<td><span  class='caozuo ' onclick='editchannelcustomer(";
                        arrayStr[j++] = mydata.data[i].ID;
                        arrayStr[j++] = ",\"" + mydata.data[i].Name + "\"," + mydata.data[i].Modulus1;
                        arrayStr[j++] = ")'>编辑&nbsp;&nbsp;&nbsp;</span> <span class='caozuo ' onclick='deletechannelcustomer(";
                        arrayStr[j++] = mydata.data[i].ID + ")'>删除&nbsp;</span></td>";
                        <% }
                           else
                           { %>
                             arrayStr[j++] = "<td></td>";
                        
                        <% } %>
                        arrayStr[j++] ="</tr>";

                    } 
                    
                    $("#mybody").html(arrayStr.join(""));
                    setUnable(mysoft);

                } else {
                    alert(data.message);
                }

            });
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
        //编辑渠道商
        function editchannelcustomer(id, name, modulues1, moduluesShanzhai) {
            window.parent.myzTree.defaultSelectNodeId ="Customer_"+id;
            window.parent.myzTree.showIndex = 0;
            window.parent.myzTree.getTree();
            

        }
        //删除渠道商
        function deletechannelcustomer(id) {
            if (confirm("确定删除此渠道商?")) {
                $.getJSON(serverUrl, { "softs": mysoft, "id": id, "do": "deletechannelcustomer", "rd": Math.random() * 100000 }, function (data) {
                    if (data.resultCode == 0) {
                        alert(data.message);
                        window.parent.myzTree.getTree();
                    } else {
                        alert(data.message);
                    }
                });

            }
           
        }
        //更新渠道商
        function updatechannelcustomer() {
            var name = $("#customerName").val();
            if ($.trim(name) == "") {
                alert("请填写好渠道商名称");
                return;
            }
            var value = $("#customerm1").val();
            var reg = /^(-?\d+)(\.\d+)?$/;
            if (!reg.test(value)) {
                alert("系数输入值格式不正确！");
                return;
            }
            $.getJSON(serverUrl, {"softs":mysoft, "id": $("#customerid").val(), "m1": value, "name": name, "do": "updatecustomer", "rd": Math.random() * 1000 }, function (data) {
                if (data.resultCode == 0) {
                    alert(data.message);
                    window.parent.setSelectStat(1);
                } else {
                    alert(data.message);
                }
            });
        }

        function showBatchCustomer() {
            $("#mywindow").attr("href", "#divCustomBatchAdd");
            colorBox2.click();
        }

        //批量添加
        function batchAddcustomer() {
           
            var txtCustomNamePrefix = $("#txtCustomName_prefix").val();
            if ($.trim(txtCustomNamePrefix) == "") {
                alert("请填写好渠道商名称前缀");
                return;
            }
            var beginNum = $("#txtNum_begin").val();
            var endNum = $("#txtNum_end").val();
            var reg = /^(0|[1-9]\d*)$/;
            if (!reg.test(beginNum) || !reg.test(endNum)) {
                alert("开始和结束点输入值格式不正确！");
                return;
            }
            var begin = parseInt(beginNum);
            var end = parseInt(endNum);
            if (end < begin) {
                alert("结束点应该大于开始点");
                return;
            }
            if ((end - begin) > 20) {
                alert("开始点和结束点不能相差20");
                return;
            }
            $.getJSON(serverUrl, { "softs": mysoft, "cid": 0, "parentcustomerid": mychannelcustomerid, "do": "batchaddcustomer", "nameprefix": txtCustomNamePrefix, "beginnum": begin, "endnum": end, "rd": Math.random() * 100000 }, function (data) {
                if (data.resultCode == 0) {
                    alert(data.message);
                    window.parent.myzTree.defaultSelectNodeId = "Customer_" + data.data;
                    window.parent.myzTree.getTree();
                } else {
                    alert(data.message);
                }
            });
        }

    </script>
</body>
</html>
