<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ISPUsersAnalysis.aspx.cs" Inherits="net91com.Stat.Web.Reports_New.R_UserAnalysis.ISPUsersAnalysis" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>运营商用户统计</title>
    <link href="/Reports_New/css/site.css?version=1" rel="stylesheet" type="text/css" />
    <link href="/Reports_New/css/defindecontrol.css?version=1" rel="stylesheet" type="text/css" />
    <link href="/Reports_New/css/chosen.css" rel="stylesheet" type="text/css" />
    <link href="/Reports_New/css/jquery.datatables.table.css" rel="stylesheet" type="text/css" />
 
    <script src="/Reports_New/js/jquery-1.6.4.min.js" type="text/javascript"></script>
    <script src="/Reports_New/js/jquery.dataTables.min.js" type="text/javascript"></script>
    <script src="/Reports_New/js/Defindecontrol.js?version=1" type="text/javascript"></script>
    <script src="/Reports_New/js/chosen.jquery.min.js" type="text/javascript"></script>
 
    <script src="/Reports_New/js/highcharts_old.js" type="text/javascript"></script>
    <script src="/Reports_New/js/chartjs.js" type="text/javascript"></script>
    <script src="/Reports_New/js/My97DatePicker/WdatePicker.js" type="text/javascript"></script>
    <script type="text/javascript">
         var serverUrl = '../HttpService.ashx';
        var softs=<%=softplatjson %>;
        var oTable;
        var allplat = {"0":"不区分平台", "1": "iPhone", "4": "Android", "7": "IPAD", "255": "PC", "8": "WP7", "9": "AndroidPad", "10": "AndroidTV" };
        var nettypearray =  <%=netModeJson %>;
        var ipsarray=<%=ipsJson %>;
        $(function () {

            initValue("beginTime", "endTime", -30, -1);
            var softcontrol = $("#selectsoft").chosen({ no_results_text: "没有找到匹配", disable_search: true }).change(function () {
                getsoftchange();
            });
            $("#selectplat").chosen({ no_results_text: "没有找到匹配", disable_search: true });
            $("#selectisps").chosen({ no_results_text: "没有找到匹配", disable_search: true });
            $("#selectnettype").chosen({ no_results_text: "没有找到匹配", disable_search: true });
            getsoftchange();
            //var parenttabs = createTabs($("#parenttags"), 2); 
            getchart();


        });
        ///设置下载
        function setDownExcel() {
            var nets = $("#selectnettype").val();
            var ipses=$("#selectisps").val();
            var strs_nets = "";
            for (var i = 0; i < nets.length; i++) {
                strs_nets += nets[i] + ",";
            }
            var strs_ipses="";
            for (var i = 0; i < ipses.length; i++) {
                strs_ipses += ipses[i] + ",";
            }

            var url = serverUrl + "?do=get_excel&softs=" + $("#selectsoft").val()
             + "&platform=" + $("#selectplat").val() + "&begintime=" + $("#beginTime").val() + "&endtime=" + $("#endTime").val()
             + "&period=" + $("#myperiod").val()+"&nettype="+ encodeURI(strs_nets) +"&selectisps="+encodeURI(strs_ipses)
             + "&service=" + "ISPUserAnalysis";
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
            
        }
        function loadData() {
            if (checkDataRight()) {
                getchart();
                return;
            }
        }

        function getTabsControl() {
            var ips = $("#selectisps").val();
            var nettypes = $("#selectnettype").val();
            var html = "";
            for (var i = 0; i < ips.length; i++) {
                for (var j = 0; j < nettypes.length; j++) {
                    html += " <li><a onclick=\"gettabs('" + ips[i]   + "','"+ nettypes[j]  + "')\"><font>" + ipsarray[ips[i]] +"_"+ nettypearray[nettypes[j]] + "</font></a></li>";
                }
            }
            if (ips.length > 1|| nettypes.length > 1) { 
                $("#mytags2").html(html);
                mytabs2 = createTabs($("#mytags2"), 0);
                mytabs2.click(0);
            }
            else {
                $("#mytags2").html("");
                gettabs(ips[0],nettypes[0]);
            }
        }

        function gettabs(isp,nettype) {
            $("#myisps").val(isp);
            $("#mynettype").val(nettype);
            var check = checkDataRight();
            if (oTable && check) {
                oTable.fnDraw();
                return;
            }
            if (!check)
                return;
             oTable = $('#table1').dataTable({
                "sDom": '<"top">rt<"bottom"><"clear">',
                "aoColumnDefs": [
                                  {
                                      "aTargets": [0, 1],
                                      "sClass": "center"
                                  }
                                  ,
                                  {
                                    "bSortable": false,
                                    "aTargets": [0,1]
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
                    aoData.push({ "name": "service", "value": "ISPUserAnalysis" });
                    aoData.push({ "name": "do", "value": "get_page" });
                    aoData.push({ "name": "endtime", "value": $("#endTime").val() });
                    aoData.push({ "name": "begintime", "value": $("#beginTime").val() });
                    aoData.push({ "name": "softs", "value": $("#selectsoft").val() });
                    aoData.push({ "name": "platform", "value": $("#selectplat").val() });
                    aoData.push({ "name": "period", "value":$("#myperiod").val() }); 
                    aoData.push({ "name": "nettype", "value":$("#mynettype").val() }); 
                    aoData.push({ "name": "selectisps", "value": $("#myisps").val() });
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
                    return true;
                },

                "fnDrawCallback": function () {
                    tableStyleSet("table1");
                   
                }
            });
            
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
            if ($("#selectsoft").val() == "" || $("#selectplat").val() == "" || !$("#selectisps").val()|| !$("#selectnettype").val()) {
                alert("条件请选择完整");
                return false;
            }
            return true;
        }
        
        function getperiod(period) {
            $("#myperiod").val(period);
            if (oTable && checkDataRight()) {
                getchart();
            }

        }

        function getchart() {
            var nets = $("#selectnettype").val();
            var ipses=$("#selectisps").val();
            var strs_nets = "";
            for (var i = 0; i < nets.length; i++) {
                strs_nets += nets[i] + ",";
            }
            var strs_ipses="";
            for (var i = 0; i < ipses.length; i++) {
                strs_ipses += ipses[i] + ",";
            }
            setDownExcel();
            //获取表格
            $.ajax({
                        type: "get",
                        url: serverUrl,
                        dataType: "json",
                        data: {
                            'service': 'ISPUserAnalysis',
                            'do': 'get_page',
                            'endtime': $("#endTime").val(),
                            'begintime': $("#beginTime").val(),
                            'softs': $("#selectsoft").val(), 
                            'platform': $("#selectplat").val(),
                            'period': $("#myperiod").val(),
                            'nettype': strs_nets,
                            'selectisps': strs_ipses
                        },
                        success: function (data) {
                            if (data.resultCode == 0) {
                                $("#mytable").html(data.data);
                            }
                            else {
                                alert(data.message);
                            }
                        }
                    });
             //获取图形
             $.ajax({
                        type: "get",
                        url: serverUrl,
                        dataType: "json",
                        data: {
                            'service': 'ISPUserAnalysis',
                            'do': 'get_chart',
                            'endtime': $("#endTime").val(),
                            'begintime': $("#beginTime").val(),
                            'softs': $("#selectsoft").val(), 
                            'platform': $("#selectplat").val(),
                            'period': $("#myperiod").val(),
                            'nettype': strs_nets,
                            'selectisps': strs_ipses
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
        
        

    </script>
</head>
<body>
    
    <input type="hidden" id="myperiod" value="1"/> 
    <input type="hidden" id="mynettype" value="1"/> 
    <input type="hidden" id="myisps" value="1"/> 
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
                        平台：
                        <select id="selectplat" style="width: 150px;" >
                               
                        </select>
                    </td>
                    <td>
                        运营商：<select id="selectisps" style="width: 200px;" multiple="multiple">
                                <%=isphtml%>
                        </select>
                    </td>
                    <td>
                         网络类型：<select id="selectnettype" style="width: 200px;" multiple="multiple">
                             <%=netModeHtml%>
                              </select>
                    </td>
                </tr>
                <tr>
                    <td colspan="3">
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
             <strong class="l" id="Strong2">
               <%--<ul id="parenttags">
                        <li><a onclick="javascript:getperiod(5)"><font>月</font></a></li>
                        <li><a onclick="javascript:getperiod(3)"><font>周</font></a></li>
                        <li><a onclick="javascript:getperiod(1)"><font>日</font></a></li> 
               </ul>--%>
            </strong>  <span class="r"></span>
        </div>
        <div class="textbox">
            <div id="container" style="margin: auto; margin: 2px; height: 400px;">
            </div>
        </div>
        <div class="title" style="margin-top: 4px;">
            <strong class="l" id="Strong1">
                <ul id="mytags2">
                    
                </ul>
            </strong><span class="r"><a class="mybutton hover"
                id="downexcel" href="javascript:void(0);"><font>导出Excel</font> </a></span>
        </div>
        <div class="textbox" id="mytable">
             
        </div>

    </div>
  
</body>
</html>
