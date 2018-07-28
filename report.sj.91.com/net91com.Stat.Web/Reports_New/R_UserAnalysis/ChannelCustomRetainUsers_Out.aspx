<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ChannelCustomRetainUsers_Out.aspx.cs" Inherits="net91com.Stat.Web.Reports_New.R_UserAnalysis.ChannelCustomRetainUsers_Out" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>外部渠道商用户</title>
    <link href="/Reports_New/css/site.css?version=1" rel="stylesheet" type="text/css" />
    <link href="/Reports_New/css/defindecontrol.css?version=1" rel="stylesheet" type="text/css" />
    <link href="/Reports_New/css/chosen.css" rel="stylesheet" type="text/css" />
    <link href="/Reports_New/css/jquery.datatables.table.css" rel="stylesheet" type="text/css" />
    <link href="/Reports_New/css/colorbox.css" rel="stylesheet" type="text/css" />
    <script src="/Reports_New/js/jquery-1.6.4.min.js" type="text/javascript"></script>
    <script src="/Reports_New/js/jquery.dataTables.min.js" type="text/javascript"></script>
    <script src="/Reports_New/js/Defindecontrol.js?version=1" type="text/javascript"></script>
    <script src="/Reports_New/js/chosen.jquery.min.js" type="text/javascript"></script>
    <script src="/Reports_New/js/jquery.colorbox-min.js" type="text/javascript"></script>
    <script src="/Reports_New/js/highcharts_old.js" type="text/javascript"></script>
    <script src="/Reports_New/js/chartjs.js" type="text/javascript"></script>
    <script src="/Reports_New/js/My97DatePicker/WdatePicker.js" type="text/javascript"></script>
    
    <script type="text/javascript">
        var serverUrl = '../HttpService.ashx';
        var softs=<%=softplatjson %>;
        var channelcustomjson=<%= channelCumstomStr%>;
        var oTable;
        var allplat = { "1": "iPhone", "4": "Android", "7": "IPAD", "255": "PC", "8": "WP7", "9": "AndroidPad", "10": "AndroidTV" };
        var showact=false;
        $(function () {

            initValue("beginTime", "endTime", -30, -1);
            var softcontrol = $("#selectsoft").chosen({ no_results_text: "没有找到匹配", disable_search: true }).change(function () {
                getsoftchange();
            });
            getsoftchange();
            mytabs = createTabs($("#mytags"), 1); 
            oTable = $('#table1').dataTable({
                "sDom": '<"top">rt<"bottom"><"clear">',
                "aoColumnDefs": [
                                  {
                                      "aTargets": [0, 1, 2,3,4,5,6,7,8,9,10,11,12,13],
                                      "sClass": "center"
                                  }
                                  , 
                                  {
                                    "bSortable": false,
                                    "aTargets":  [0, 1, 2,3,4,5,6,7,8,9,10,11,12,13]
                                  } 
                 ], 
                "bProcessing": true,
                "bServerSide": true,
                "oLanguage": { sUrl: "../js/de_DE.txt" },
                "bFilter": false, 
                "sPaginationType": "full_numbers",
                "sAjaxSource": serverUrl,
                "bPaginate": false,
                "fnServerParams": function (aoData) {
                    aoData.push({ "name": "service", "value": "ChannelCustomRetainUsers_Out" });
                    aoData.push({ "name": "do", "value": "get_page" });
                    aoData.push({ "name": "endtime", "value": $("#endTime").val() });
                    aoData.push({ "name": "begintime", "value": $("#beginTime").val() });
                    aoData.push({ "name": "softs", "value": $("#selectsoft").val() });
                    aoData.push({ "name": "platform", "value": $("#selectplat").val() });
                    aoData.push({ "name": "channelid", "value": $("#selectchannelcustom").val() });        
                    aoData.push({ "name": "period", "value": $("#hiddenperiod").val() });     
                },
                "fnServerData": function (sSource, aoData, fnCallback) {
                    $.ajax({
                        "dataType": 'json',
                        "type": "POST",
                        "url": sSource,
                        "data": aoData,
                        "success":  function (data) {
                                //存在输出的resultCode 让他输出message
                                if (data.resultCode && data.resultCode!=0) { 
                                        alert(data.message);
                                } else {
                                    fnCallback(data);
                                }

                            }
                    });
                },
                "fnPreDrawCallback": function (oSettings) {
                    //setDownExcel();
                    return true;
                },

                "fnDrawCallback": function () {
                    setTitle();
                    tableStyleSet("table1");
                     $.ajax({
                        type: "get",
                        url: serverUrl,
                        dataType: "json",
                        data: {
                            'service': 'ChannelCustomRetainUsers_Out',
                            'do': 'get_chart',
                            'endtime': $("#endTime").val(),
                            'begintime': $("#beginTime").val(),
                            'softs': $("#selectsoft").val(), 
                            'platform': $("#selectplat").val(),
                            'channelid': $("#selectchannelcustom").val(),
                            'period':$("#hiddenperiod").val()
                        },
                        success: function (data) {
                            if (data.resultCode == 0) {
                                $("#container").html("");
                                var title = "";
                                var yname = "量";
                                var obj = eval("(" + data.data + ")");
                                createLine("container", 400, obj.title, yname, true, 0, obj.x, obj.y);
                            }
                            else {
                                alert(data.message);
                            }
                        }
                    });
                    
                }
            });

        });

        function setTitle() {
            var zhouqi = "天";
            if ($("#hiddenperiod").val() == 3) {
                zhouqi = "周";
            } else
                zhouqi = "月";
            $("#table1 .titleclass").each(function(i) {
                    var title = $.trim($(this).text());
                    
                    title = title.substr(0, 2)+zhouqi;
                    $(this).text(title);
            });
        }

        ///设置下载
        function setDownExcel() {
            
        }
        function getsoftchange() {
            var plathtml = "";
            var channelhtml = "";
            var softid = $("#selectsoft").val();
            for (var i = 0; i < softs[softid].length; i++) {
                if (allplat[softs[softid][i]]) {
                    plathtml += "<option value='" + softs[softid][i] + "'>" + allplat[softs[softid][i]] + "</option>";
                }
                
            }
            $("#selectplat").html(plathtml);
            $("#selectplat").trigger("liszt:updated").chosen({ no_results_text: "没有找到匹配", disable_search: true });
            for (var j = 0; j < channelcustomjson[softid].length; j++) {
                channelhtml += "<option value='" + channelcustomjson[softid][j].id + "'>" + channelcustomjson[softid][j].name + "</option>";
            }
            $("#selectchannelcustom").html(channelhtml);
            $("#selectchannelcustom").trigger("liszt:updated").chosen({ no_results_text: "没有找到匹配", disable_search: true });
        }
        function loadData() {
            if (checkDataRight()) {
                 oTable.fnDraw();
                return;
            }
        }

        function get(period) {
            $("#hiddenperiod").val(period);
            loadData();
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
            if ($("#selectsoft").val() == "" || $("#selectplat").val() == "" || $("#selectchannelcustom").val() == "") {
                alert("条件请选择完整");
                return false;
            }
            if (t / (3600 * 24 * 1000) > 93) {
                alert("开始日期与结束日期相差不能大于90天");
                return false;
            }
            
            return true;
        }


    </script>
</head>
<body>
    <form id="form1" runat="server">
    <input type="hidden" id="hiddenperiod" value="3"/> 
    <div class="maindiv">
        <div style="padding: 10px;">
            <table cellpadding="0" cellspacing="10" border="0" width="100%">
                <tr>
                    <td>
                        产品:<select id="selectsoft" style="width: 150px;">
                            <%=selectsofthtml%>
                        </select>
                    </td>
                    <td>
                        平台：<select id="selectplat" style="width: 150px;">
                        </select>
                    </td>
                    <td>
                        渠道商：<select id="selectchannelcustom" style="width: 150px;">
                        </select>
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        从
                        <input type="text" id="beginTime" class="Wdate" onclick="WdatePicker()" />
                        到
                        <input type="text" id="endTime" class="Wdate" onclick="WdatePicker()" />
                    </td>
                    <td>
                        <span style="cursor: pointer; float: right; margin-right: 20px;"><a class="mybutton hover"
                            style="margin-top: 4px; overflow: hidden;" onclick="loadData();"><font>查询</font>
                        </a></span>
                    </td>
                </tr>
            </table>
            <ul id="mytags">
                    <li><a onclick="get(5)"><font>月</font></a></li>
                    <li><a onclick="get(3)"><font>周</font></a></li>  
            </ul>
        </div>
        <div class="title" style="margin-top: 4px;">
            <strong class="l" id="Strong2">新增用户留存率</strong> <span class="r"></span>
        </div>
        <div class="textbox">
            <div id="container" style="margin: auto; margin: 2px; height: 400px;">
            </div>
        </div>
        <div class="title" style="margin-top: 4px;">
            <strong class="l" id="Strong1"></strong> 
        </div>
        <div class="textbox">
            <table id="table1" border="1" cellpadding="0" cellspacing="0" border="0" class="display">
                <thead>
                    <tr>
                        <th style="width: 120px" rowspan="2">
                            日期
                        </th>
                        <th  rowspan="2">新增用户
                        </th>
                        <th  class="titleclass" colspan="2"> 第1周
                        </th>
                        <th  class="titleclass"  colspan="2">第2周
                        </th>
                        <th   class="titleclass" colspan="2"> 第3周
                        </th>
                        <th   class="titleclass" colspan="2"> 第4周
                        </th>
                        <th  class="titleclass" colspan="2"> 第5周
                        </th>
                        <th  class="titleclass"  colspan="2"> 第6周
                        </th>
                    </tr>
                    <tr>
                        
                        <th>留存量 </th>
                        <th>留存率 </th>
                        <th>留存量 </th>
                        <th>留存率 </th>
                        <th>留存量 </th>
                        <th>留存率 </th>
                        <th>留存量 </th>
                        <th>留存率 </th>
                        <th>留存量 </th>
                        <th>留存率 </th>
                        <th>留存量 </th>
                        <th>留存率 </th>
                    </tr>
                </thead>
                <tbody>
                </tbody>
            </table>
        </div>
    </div>
    </form>
</body>
</html>
