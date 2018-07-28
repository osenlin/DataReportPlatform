<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ChannelCustomerManager_Iframe1.aspx.cs" Inherits="net91com.Stat.Web.Reports_New.R_UserAnalysis.ChannelCustomerManager_Iframe1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
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
    <script src="../js/My97DatePicker/WdatePicker.js" type="text/javascript"></script>
     <style type="text/css">
        .textbox table td { text-align: left; }
        .textbox table td span { cursor: pointer; }
        #linktable td{ text-align: left;}
        .divtable {
            border-collapse: collapse;
            margin: 0 auto;
            width: 100%;
            height: 80%;
            border-left: 1px solid #cccccc;
            border-right: 1px solid #cccccc;
        }
        .divtable td {
            padding: 5px;
            border-style: solid;
            border-color: #cccccc;
            border-width: 1px 0;
            text-align: left;
        }
        .noclasstable td
        {
             border-width: 0px 0;
        }
        
        
        
    </style>
    <script type="text/javascript">
        var serverUrl = '../HttpService.ashx?service=ChannelCustomerManager';
        var currentCustomer;
        var mysoft = "";
        var mychanneltype = "";
        var mychannelcustomerid = "";
        var colorBox1;
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
            $("#checkopen").bind("change", function () {
                openchange();
            });
            colorBox1 = $("#mywindow1").colorbox({ iframe: true, width: "90%", height: "70%", speed: 0 });
            colorBox2 = $("#mywindow2").colorbox({ iframe: true, width: "90%", height: "70%", speed: 0 });
            fillBodyData();
        });
        //是否开放
        function openchange() {
            var openchecked = $("#checkopen")[0];
            var switchvalue = openchecked.checked?1:0;
            var txt = openchecked.checked ? "确定开放外部查看地址?" : "确定关闭外部查看地址?";
            if (confirm(txt)) {
                //setcustomerswitchtype==1
                $.getJSON(serverUrl, { "softs": mysoft, "switchvalue": switchvalue, "switchtype": 1, "id": mychannelcustomerid, "do": "setcustomerswitch", "rd": Math.random() * 1000 }, function(data) {
                    if (data.resultCode == 0) {
                        alert(data.message); 
                    } else {
                        alert(data.message);
                    }
                });
            } else {
                $("#checkopen")[0].checked = !openchecked.checked;
            }
        }
        
    </script>
 
   <script type="text/javascript">
      
       function getSelectHtml(ptype, pid, desid, defaultValue, dataSourceType) {

           var array = getChannelAttributesData(ptype, pid, dataSourceType);
           var selectHtml = "<option value='-1'>请选择</option>";
           for (var i = 0; i < array.length; i++) {
               selectHtml += "<option value='" + array[i].ID + "'";
               if (array[i].ID == defaultValue) {
                   selectHtml += " selected='selected' ";
               }
               selectHtml += ">";
               selectHtml += array[i].Name;
               selectHtml += "</option>";
           }
           $("#" + desid).html(selectHtml);
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

   </script>
</head>
<body>
    
    <div class="maindiv">
        
        <a id="mywindow1"></a> 
        <a id="mywindow2"></a> 
        <div style="margin-top: 4px;" id="titleid" class="title">
            <strong id="Strong1" class="l">渠道商详情</strong> 
            <span class="r">
                    <table id="table_addsoncustomer">
                        <tr>
                            <% if (HasEditRight)
                               { %>
                            <td style="vertical-align: top;">
                                添加同级渠道商：<input type="text" value="" class="txtbox" id="txtcustomername" />
                            </td>
                            <td style="vertical-align: top;">
                                <a class="mybutton hover" style="cursor: pointer; margin-right: 5px;" onclick="addsamelevelcustomer()">
                                    <font>添加</font> </a>
                            </td>
                            <% } %>
                        </tr>
                    </table>                                                                            
                                                              
              </span>
        </div>
        <div class="textbox">
              <div id="div2">
                
                <table id="channeltable" class="divtable">
                     <tr style="height: 40px;">
                         <td style="width:25%">
                            ID :
                        </td>
                        <td>
                            <input type="text" disabled="disabled" class="txtbox" style="width: 150px;" id="editcustomerid" />
                        </td>
                    </tr>
                    <tr style="height: 40px;">
                       
                        <td style="width:25%">
                            名称 :
                        </td>
                        <td>
                            <input type="text" class="txtbox" style="width: 150px;" id="editcustomerName" />
                        </td>
                    </tr>
                    <tr style="height: 40px;">
                        <td>
                            一次系数:
                        </td>
                        <td>
                            <input type="text" class="txtbox" style="width: 150px;" id="editcustomerm1" />
                        </td>
                    </tr>
                     <tr style="height: 50px;">
                        <td>
                            开放给外部：
                        </td>
                        <td>
                             <input type="checkbox" id="opencheckedid" />
                        </td>
                    </tr>
                    <tr style="height: 50px;">
                        <td>
                            对外显示类型：
                        </td>
                        <td>
                            <select id="editshowtype">
                                 <option value="0">新增</option>
                                 <option value="1">新增和活跃</option>
                                 <option value="2">新增和留存</option>
                            </select>
                        </td>
                    </tr>
                    <tr style="height: 50px;">
                        <td>
                            对外开放最小可看日期：
                        </td>
                        <td>
                            <input type="text" id="beginTime" class="Wdate" onclick="WdatePicker()" />
                        </td>
                    </tr>
                     <tr style="height: 50px;">
                        <td>
                           添加时间：
                        </td>
                        <td>
                           <span id="addtimespan"></span>
                        </td>
                    </tr>
                    <tr>
                       
                        <% if (HasEditRight)
                         { %>
                              <td>
                            
                              </td>
                              <td>
                                  <a class="mybutton hover" style="cursor: pointer; margin-right: 5px; float: right;" onclick="deletecustomer()">
                                     <font>删除</font></a>
                                   <a class="mybutton hover" style="cursor: pointer; margin-right: 20px; float: right;" onclick="updatechannelcustomer()">
                                     <font>保存</font></a>
                              </td>
                                 
                       <% } %>
                           
                     
                    </tr>
                </table>
            </div>

           
        </div>
        
        
        <div style="margin-top: 4px;" id="Div1" class="title">
            <strong id="Strong2" class="l">渠道商数据链接</strong> <span class="r"></span>
        </div>
        <div class="textbox">
            <table id="linktable" cellpadding="0" border="0" cellspacing="0" class="display dataTable" id="table2">
                <thead>
                    <tr>
                        <th style="width: 150px">
                            分类
                        </th>
                        <th>
                            相关链接
                        </th>
                        
                    </tr>
                </thead>
                <tbody id="tblinkbody">
                </tbody>
            </table>
        </div>
      

    
    </div>
    <script type="text/javascript">
        function fillBodyData() {
            $.getJSON(serverUrl, { "id": mychannelcustomerid, "softs": mysoft, "do": "getcustomerbyid", "rd": Math.random() * 10000 }, function (mydata) {

                if (mydata.resultCode == 0) {
                    var customer = mydata.data;

                    currentCustomer = customer;
                    $("#editcustomerName").val(currentCustomer.Name);
                    $("#editcustomerm1").val(customer.Modulus1);
                    $("#editshowtype").val(customer.ShowType);
                    $("#editcustomerid").val(customer.ID);
                    $("#beginTime").val(customer.MinViewTime);
                    $("#addtimespan").html(customer.AddTimeString);
                    $("#opencheckedid")[0].checked = customer.ReportType > 0;

                    var url = "http://report.felink.com/reports/sjqd/AnalysisForChannelV2En.aspx?p=" + customer.Keyforout;
                    var html = "";
                    var htmlforcustomtr = "";
                    var htmlforneitr = "";
                    var html_retain = "";
                    var platArray = [1, 4, 7, 9];
                    var url_retain = "http://report.felink.com/Reports_New/R_UserAnalysis/RetainUsersForExternalChannels.aspx?p=";
                    for (var i = 0; i < window.parent.platforms.length; i++) {
                        htmlforcustomtr += "<a href='javascript:window.parent.linkDetail(" + customer.ID + ",2,11," + window.parent.platforms[i] + ")'>";
                        htmlforcustomtr += allplat[window.parent.platforms[i]] + "</a>&nbsp;&nbsp;&nbsp;";
                        htmlforneitr += "<a href='javascript:window.parent.linkDetail(" + customer.ID + ",2,1," + window.parent.platforms[i] + ")'>";
                        htmlforneitr += allplat[window.parent.platforms[i]] + "</a>&nbsp;&nbsp;&nbsp;";
                    }
                    htmlforcustomtr += "<a href='javascript:window.parent.linkDetail(" + customer.ID + ",2,11," + "0" + ")'>";
                    htmlforcustomtr += "不区分平台" + "</a>&nbsp;&nbsp;&nbsp;";
                    htmlforneitr += "<a href='javascript:window.parent.linkDetail(" + customer.ID + ",2,1," + "0" + ")'>";
                    htmlforneitr += "不区分平台" + "</a>&nbsp;&nbsp;&nbsp;";

                    for (var i = 0; i < window.parent.platforms.length; i++) {
                        var detailurl = url + "&plat=" + window.parent.platforms[i];
                        html += "<tr><td>" + allplat[window.parent.platforms[i]] + "</td><td><a>" + detailurl + "</a>" + "</td></tr>";
                    }
                    html += "<tr><td>" + "不区分平台" + "</td><td><a>" + url + "&plat=" + "0" + "</a>" + "</td></tr>";
                    var url2 = "javascript:window.parent.linkfenqudao(" + mychannelcustomerid + ", 2)";
                    var fentongji = "<tr><td>按子渠道商查询</td><td ><a href='" + url2 + "'>查看" + "</a></td></tr>";
                    var fenjie = "<tr><td colspan='2'>提供给外部地址(渠道商)：</td></tr>";
                    var all = "<tr><td>" + "统计地址for渠道商" + "</td><td>" + htmlforcustomtr + "</td></tr>";
                    all += "<tr><td>统计地址for内部</td><td>" + htmlforneitr + "</td></tr>" + fentongji + fenjie + html;

                    html_retain = "<tr><td colspan='2'>提供给外部留存地址：</td></tr>";
                    var detailurlRetain = "";
                    var platID = "";
                    for (var index = 0; index < window.parent.platforms.length; index++) {
                        platID = window.parent.platforms[index];
                        if (platID == 1 || platID == 4 || platID == 7 || platID == 9) {
                            detailurlRetain = url_retain + customer.Keyforout_RetainDic[platID];
                            html_retain += "<tr><td>" + allplat[platID] + "</td><td><a>" + detailurlRetain + "</a>" + "</td></tr>";
                        }

                    }
                    html_retain += "<tr><td>不区分平台</td><td><a>" + url_retain + customer.Keyforout_RetainDic[0] + "</a></td></tr>";
                    all += html_retain;

                    $("#tblinkbody").html(all);
                    setUnable(mysoft);
                } else {
                    alert(data.message);
                }

            });
        }

        //删除本渠道商
        function deletecustomer() {
            if (confirm("确定删除此渠道商?")) {
                $.getJSON(serverUrl, { "softs": mysoft, "id": mychannelcustomerid, "do": "deletechannelcustomer", "rd": Math.random() * 100000 }, function (data) {
                    if (data.resultCode == 0) {
                        alert(data.message);
                        window.parent.myzTree.defaultSelectNodeId = window.parent.myzTree.selectNodes[0].getParentNode().id;
                        window.parent.myzTree.getTree();
                    } else {
                        alert(data.message);
                    }
                });

            }
        }
        //添加同级渠道商
        function addsamelevelcustomer() {
            var name = $("#txtcustomername").val();
            if ($.trim(name) == "") {
                alert("请填写好渠道商名称");
                return;
            }
            
            $.getJSON(serverUrl, { "softs": mysoft, "samelevelcustomerid": mychannelcustomerid, "do": "addsamelevelcustomer", "name": name, "rd": Math.random() * 100000 }, function (data) {

                if (data.resultCode == 0) {
                    alert(data.message);
                    window.parent.myzTree.defaultSelectNodeId ="Customer_"+ data.data;
                    window.parent.myzTree.getTree();
                } else {
                    alert(data.message);
                }
            });
        }

        //更新渠道商
        function updatechannelcustomer() {
            var checkboxvalue = $("#customerrepeatimei").val();
            var reporttype = 0;
            if ($("#opencheckedid")[0].checked)
                reporttype = 1;
            var name = $("#editcustomerName").val();
            if ($.trim(name) == "") {
                alert("请填写好渠道商名称");
                return;
            }
            if ($("#beginTime").val() == "") {
                alert("请填写好最小可查看日期");
                return;
            }
            var value = $("#editcustomerm1").val();
            var reg = /^(-?\d+)(\.\d+)?$/;
            if (!reg.test(value)) {
                alert("系数输入值格式不正确！");
                return;
            }
            //是否需要影响孩子结点属性
            var needEditSonCustomer = false;
            if (confirm("确定保存?")) {
                $.getJSON(serverUrl, { "softs": mysoft, "showtype": $("#editshowtype").val(), "id": mychannelcustomerid,
                    "m1": $("#editcustomerm1").val(), "repeatimei": checkboxvalue, "minviewtime": $("#beginTime").val(),
                    "channeltype": $("#channeltypeselectName").val(),
                    "name": name, "do": "updatecustomer", "reporttype": reporttype,"rd": Math.random() * 1000
                }, function (data) {
                    if (data.resultCode == 0) {
                        alert(data.message);
                        window.parent.myzTree.getTreeFromAdvance();
                        window.parent.setSelectStat(0);

//                        window.parent.myzTree.getTree();
//                        //若需要动用高级搜索
//                        window.parent.myzTree.needadvanceSearch = true;
//                        if (window.parent.myzTree.needadvanceSearch && window.parent.advanceSearchObj != null)
//                            window.parent.advanceSearchTree(true);
//                        //重置关闭高级搜索开关
//                        window.parent.myzTree.needadvanceSearch = false;
//                        window.parent.setSelectStat(0);
                    } else {
                        alert(data.message);
                    }
                }); 
            }
            
            
        }
         
         
    </script>
</body>
</html>
