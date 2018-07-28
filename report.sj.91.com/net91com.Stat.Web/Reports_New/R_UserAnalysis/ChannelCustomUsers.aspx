<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ChannelCustomUsers.aspx.cs"
    Inherits="net91com.Stat.Web.Reports_New.R_UserAnalysis.ChannelCustomUsers" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
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
            oTable = $('#table1').dataTable({
                "sDom": '<"top">rt<"bottom"><"clear">',
                "aoColumnDefs": [
                                  {
                                      "aTargets": [0, 1,2],
                                      "sClass": "center"
                                  }
                                  , {
                                    "bSortable": false,
                                    "aTargets": [0, 1,2]
                                    },
                                 {
                                        "bUseRendered": false,
                                        "fnRender": function (oObj) {
                                             var act = oObj.aData[2];
                                             if(act!="")
                                                showact=true;
                                             else
                                                showact=false;
                                             return act;
                                        },
                                        "aTargets": [2]
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
                    aoData.push({ "name": "service", "value": "ChannelCustomUsers" });
                    aoData.push({ "name": "do", "value": "get_page" });
                    aoData.push({ "name": "endtime", "value": $("#endTime").val() });
                    aoData.push({ "name": "begintime", "value": $("#beginTime").val() });
                    aoData.push({ "name": "softs", "value": $("#selectsoft").val() });
                    aoData.push({ "name": "platform", "value": $("#selectplat").val() });
                    aoData.push({ "name": "channelid", "value": $("#selectchannelcustom").val() }); 
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
                    setDownExcel();
                    if(showact)
                    {
                        $("#tablehead").html("活跃量");
                    }
                    else
                    {
                         $("#tablehead").html("");
                    }

                    return true;
                },

                "fnDrawCallback": function () {
                    tableStyleSet("table1");
                    $.ajax({
                        type: "get",
                        url: serverUrl,
                        dataType: "json",
                        data: {
                            'service': 'ChannelCustomUsers',
                            'do': 'get_chart',
                            'endtime': $("#endTime").val(),
                            'begintime': $("#beginTime").val(),
                            'softs': $("#selectsoft").val(), 
                            'platform': $("#selectplat").val(),
                            'channelid': $("#selectchannelcustom").val() 
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
        ///设置下载
        function setDownExcel() {
            var url = serverUrl + "?do=get_excel&softs=" + $("#selectsoft").val()
             + "&platform=" + $("#selectplat").val() + "&begintime=" + $("#beginTime").val() + "&endtime=" + $("#endTime").val()
             + "&channelid=" + $("#selectchannelcustom").val()  + "&service=" + "ChannelCustomUsers";
            $("#downexcel").attr("href", url);
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
            return true;
        }


    </script>
</head>
<body>
    <form id="form1" runat="server">
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
        </div>
        <div class="title" style="margin-top: 4px;">
            <strong class="l" id="Strong2">每日曲线图</strong> <span class="r"></span>
        </div>
        <div class="textbox">
            <div id="container" style="margin: auto; margin: 2px; height: 400px;">
            </div>
        </div>
        <div class="title" style="margin-top: 4px;">
            <strong class="l" id="Strong1">每日明细 </strong><span class="r"><a class="mybutton hover"
                id="downexcel" href="javascript:void(0);"><font>导出Excel</font> </a></span>
        </div>
        <div class="textbox">
            <table id="table1" cellpadding="0" cellspacing="0" border="0" class="display">
                <thead>
                    <tr>
                        <th style="width: 120px">
                            时间
                        </th>
                        <th>新增量
                        </th>
                        <th id="tablehead"> 
                        </th>
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
