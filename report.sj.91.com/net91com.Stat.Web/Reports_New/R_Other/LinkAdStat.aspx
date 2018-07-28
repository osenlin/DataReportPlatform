<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LinkAdStat.aspx.cs" Inherits="net91com.Stat.Web.Reports_New.R_Other.LinkAdStat" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>广告跳转统计</title>
    <link href="../css/colorbox.css" rel="stylesheet" type="text/css" />
    <link href="../css/site.css?version=1" rel="stylesheet" type="text/css" />
    <link href="../css/defindecontrol.css?version=1" rel="stylesheet" type="text/css" />
    <link href="../css/chosen.css" rel="stylesheet" type="text/css" />
    <link href="../css/jquery.datatables.table.css" rel="stylesheet" type="text/css" />

    <script src="../js/jquery-1.6.4.min.js" type="text/javascript"></script>
    <script src="../js/jquery.colorbox-min.js" type="text/javascript"></script>
    <script src="../js/jquery.dataTables.min.js" type="text/javascript"></script>
    <script src="../js/chosen.jquery.min.js" type="text/javascript"></script>
    <script src="../js/Defindecontrol.js?version=1" type="text/javascript"></script>
    <script src="../js/My97DatePicker/WdatePicker.js" type="text/javascript"></script>
    <script type="text/javascript">
        var serverUrl = '../HttpService.ashx?service=LinkAdStat';
        var oTable;
        $(function () {
            $("#sltSoftID").chosen({ no_results_text: "没有找到匹配" }).change(function () {
                loadVersion();
            });
            $("#sltPlatform").chosen({ no_results_text: "没有找到匹配" });

            $("#windowHours").colorbox({ inline: true, width: "90%", height: "90%", speed: 0 });
            $("#windowArea").colorbox({ inline: true, width: "90%", height: "90%", speed: 0 });
            $("#windowKeyword").colorbox({ inline: true, width: "90%", height: "90%", speed: 0 });
            $("#windowRetained").colorbox({ inline: true, width: "90%", height: "90%", speed: 0 });

            initValue("txtBeginTime", "txtEndTime", -1, -1);

            loadData();
        });

        function loadData() {
            setParm();
            setDownExcel();
            loadTable();
        }

        function loadTable() {
            if (oTable) {
                oTable.fnDraw();
            }
            else {
                oTable = $('#table1').dataTable({
                    "aoColumnDefs": [{
                        "aTargets": [11, 12, 13],
                        "sClass": "center"
                    }, {
                        "bUseRendered": false,
                        "fnRender": function (oObj) {
                            return "<a href='javascript:void(0);' onclick='showHoursDetail(" + oObj.aData[11] + ");'>查看</a>";
                        },
                        "aTargets": [11]
                    }, {
                        "bUseRendered": false,
                        "fnRender": function (oObj) {
                            return "<a href='javascript:void(0);' onclick='showAreaDetail(" + oObj.aData[12] + ");'>查看</a>";
                        },
                        "aTargets": [12]
                    }, {
                        "bUseRendered": false,
                        "fnRender": function (oObj) {
                            return "<a href='javascript:void(0);' onclick='showKeywordDetail(" + oObj.aData[13] + ");'>查看</a>";
                        },
                        "aTargets": [13]
                    }, {
                        "bUseRendered": false,
                        "fnRender": function (oObj) {
                            return "<a href='javascript:void(0);' onclick='showRetainedDetail(" + oObj.aData[14] + ");'>查看</a>";
                        },
                        "aTargets": [14]
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
                        aoData.push({
                            "name": "do", "value": "get_list"
                        }, {
                            "name": "softid", "value": softId
                        }, {
                            "name": "platform", "value": platform
                        }, {
                            "name": "begintime", "value": begintime
                        }, {
                            "name": "endtime", "value": endtime
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

        function setDownExcel() {
            var parm = "&softid=" + softId
                + "&platform=" + platform
                + "&begintime=" + begintime
                + "&endtime=" + endtime;
            $("#downexcel").attr("href", serverUrl + "&do=get_excel" + parm);
        }

        var adId = 0;
        var softId = 0;
        var platform = 0;
        var begintime;
        var endtime;
        function setParm() {
            softId = $("#sltSoftID").val();
            platform = $("#sltPlatform").val();
            begintime = $("#txtBeginTime").val();
            endtime = $("#txtEndTime").val();
        }

        function showHoursDetail(adid) {
            adId = adid;
            var parm = "&softid=" + softId
                + "&platform=" + platform
                + "&begintime=" + begintime
                + "&endtime=" + endtime
                + "&adid=" + adid;
            $("#downexcel_hours").attr("href", serverUrl + "&do=get_excel_hours" + parm);
            $("#windowHours").attr("href", "#divHoursDetail");
            $("#windowHours").click();
            loadHoursDetail();
        }

        var oTableHoursDetail;
        function loadHoursDetail() {
            if (oTableHoursDetail) {
                oTableHoursDetail.fnDraw();
            }
            else {
                oTableHoursDetail = $('#table2').dataTable({
                    "aoColumnDefs": [{
                        "aTargets": [0, 1, 2, 3, 4, 5, 6, 7],
                        "sClass": "center"
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
                        aoData.push({
                            "name": "do", "value": "get_list_hours"
                        }, {
                            "name": "softid", "value": softId
                        }, {
                            "name": "platform", "value": platform
                        }, {
                            "name": "begintime", "value": begintime
                        }, {
                            "name": "endtime", "value": endtime
                        }, {
                            "name": "adid", "value": adId
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

        function showAreaDetail(adid) {
            adId = adid;
            var parm = "&softid=" + softId
                + "&platform=" + platform
                + "&begintime=" + begintime
                + "&endtime=" + endtime
                + "&adid=" + adid;
            $("#downexcel_area").attr("href", serverUrl + "&do=get_excel_area" + parm);
            $("#windowArea").attr("href", "#divAreaDetail");
            $("#windowArea").click();
            loadAreaDetail();
        }

        var oTableAreaDetail;
        function loadAreaDetail() {
            if (oTableAreaDetail) {
                oTableAreaDetail.fnDraw();
            }
            else {
                oTableAreaDetail = $('#table3').dataTable({
                    "aoColumnDefs": [{
                        "aTargets": [1, 2, 3, 4, 5, 6, 7],
                        "sClass": "center"
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
                        aoData.push({
                            "name": "do", "value": "get_list_area"
                        }, {
                            "name": "softid", "value": softId
                        }, {
                            "name": "platform", "value": platform
                        }, {
                            "name": "begintime", "value": begintime
                        }, {
                            "name": "endtime", "value": endtime
                        }, {
                            "name": "adid", "value": adId
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

        function showKeywordDetail(adid) {
            adId = adid;
            var parm = "&softid=" + softId
                + "&platform=" + platform
                + "&begintime=" + begintime
                + "&endtime=" + endtime
                + "&adid=" + adid;
            $("#downexcel_keyword").attr("href", serverUrl + "&do=get_excel_keyword" + parm);
            $("#windowKeyword").attr("href", "#divKeywordDetail");
            $("#windowKeyword").click();
            loadKeywordDetail();
        }

        var oTableKeywordDetail;
        function loadKeywordDetail() {
            if (oTableKeywordDetail) {
                oTableKeywordDetail.fnDraw();
            }
            else {
                oTableKeywordDetail = $('#table4').dataTable({
                    "aoColumnDefs": [{
                        "aTargets": [1, 2, 3, 4, 5, 6, 7],
                        "sClass": "center"
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
                        aoData.push({
                            "name": "do", "value": "get_list_keyword"
                        }, {
                            "name": "softid", "value": softId
                        }, {
                            "name": "platform", "value": platform
                        }, {
                            "name": "begintime", "value": begintime
                        }, {
                            "name": "endtime", "value": endtime
                        }, {
                            "name": "adid", "value": adId
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

        function showRetainedDetail(adid) {
            adId = adid;
            var parm = "&softid=" + softId
                + "&platform=" + platform
                + "&begintime=" + begintime
                + "&endtime=" + endtime
                + "&adid=" + adid;
            $("#downexcel_retained").attr("href", serverUrl + "&do=get_excel_retained" + parm);
            $("#windowRetained").attr("href", "#divRetainedDetail");
            $("#windowRetained").click();
            loadRetainedDetail();
        }

        var oTableRetainedDetail;
        function loadRetainedDetail() {
            if (oTableRetainedDetail) {
                oTableRetainedDetail.fnDraw();
            }
            else {
                oTableRetainedDetail = $('#table5').dataTable({
                    "aoColumnDefs": [{
                        "aTargets": [1, 2, 3, 4, 5, 6, 7, 8],
                        "sClass": "center"
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
                        aoData.push({
                            "name": "do", "value": "get_list_retained"
                        }, {
                            "name": "softid", "value": softId
                        }, {
                            "name": "platform", "value": platform
                        }, {
                            "name": "begintime", "value": begintime
                        }, {
                            "name": "endtime", "value": endtime
                        }, {
                            "name": "adid", "value": adId
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
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <div class="maindiv">
            <table cellpadding="5" cellspacing="0" border="0" width="100%">
                <tr>
                    <td style="width: 800px">产品：
                        <select id="sltSoftID" style="width: 120px">
                            <option value="2" selected="selected">91助手</option>
                        </select>
                        平台：<select id="sltPlatform" style="width: 80px">
                            <option value="0" selected="selected">不区分</option>
                            <option value="1">iPhone</option>
                            <option value="7">iPad</option>
                            <option value="4">Android</option>
                        </select>
                        日期从
                        <input type="text" id="txtBeginTime" class="Wdate" onclick="WdatePicker()" />
                        到
                        <input type="text" id="txtEndTime" class="Wdate" onclick="WdatePicker()" />
                    </td>
                    <td>
                        <span style="cursor: pointer; margin-right: 20px;">
                            <a class="mybutton hover" style="margin-top: 0px; overflow: hidden;" onclick="loadData();"><font>查询</font></a>
                        </span>
                    </td>
                </tr>
            </table>
            <div class="title" style="margin-top: 4px;">
                <strong class="l" id="Strong1">广告跳转统计</strong>
                <span class="r">
                    <a class="mybutton hover" id="downexcel" href="javascript:void(0);"><font>导出Excel</font></a>
                </span>
            </div>
            <div class="textbox">
                <table id="table1" cellpadding="0" cellspacing="0" border="0" class="display">
                    <thead>
                        <tr>
                            <th style="width: 100px">广告计划
                            </th>
                            <th style="width: 150px">广告单元
                            </th>
                            <th style="width: 150px">关键词
                            </th>
                            <th style="width: 100px">匹配形式
                            </th>
                            <th>跳转数
                            </th>
                            <th>活跃数（15分钟）
                            </th>
                            <th>激活数（15分钟）
                            </th>
                            <th>激活转化率
                            </th>
                            <th>活跃数（30分钟）
                            </th>
                            <th>激活数（30分钟）
                            </th>
                            <th>激活转化率
                            </th>
                            <th>时段分布
                            </th>
                            <th>地区分布
                            </th>
                            <th>实际搜索词
                            </th>
                            <th>7天留存
                            </th>
                        </tr>
                    </thead>
                    <tbody>
                    </tbody>
                </table>
            </div>
        </div>
        <div style="display: none; padding: 5px">
            <div id="divHoursDetail">
                <div class="title" style="margin-top: 4px;">
                    <strong class="l" id="Strong2">时段分布</strong>
                    <span class="r">
                        <a class="mybutton hover" id="downexcel_hours" href="javascript:void(0);"><font>导出Excel</font></a>
                    </span>
                </div>
                <div class="textbox">
                    <table id="table2" cellpadding="0" cellspacing="0" border="0" class="display">
                        <thead>
                            <tr>
                                <th style="width: 100px">时段
                                </th>
                                <th>跳转数
                                </th>
                                <th>活跃数（15分钟）
                                </th>
                                <th>激活数（15分钟）
                                </th>
                                <th>激活转化率
                                </th>
                                <th>活跃数（30分钟）
                                </th>
                                <th>激活数（30分钟）
                                </th>
                                <th>激活转化率
                                </th>
                            </tr>
                        </thead>
                        <tbody>
                        </tbody>
                    </table>
                </div>
            </div>
            <div id="divAreaDetail">
                <div class="title" style="margin-top: 4px;">
                    <strong class="l" id="Strong4">地区分布</strong>
                    <span class="r">
                        <a class="mybutton hover" id="downexcel_area" href="javascript:void(0);"><font>导出Excel</font></a>
                    </span>
                </div>
                <div class="textbox">
                    <table id="table3" cellpadding="0" cellspacing="0" border="0" class="display">
                        <thead>
                            <tr>
                                <th style="width: 200px">地区
                                </th>
                                <th>跳转数
                                </th>
                                <th>活跃数（15分钟）
                                </th>
                                <th>激活数（15分钟）
                                </th>
                                <th>激活转化率
                                </th>
                                <th>活跃数（30分钟）
                                </th>
                                <th>激活数（30分钟）
                                </th>
                                <th>激活转化率
                                </th>
                            </tr>
                        </thead>
                        <tbody>
                        </tbody>
                    </table>
                </div>
            </div>
            <div id="divKeywordDetail">
                <div class="title" style="margin-top: 4px;">
                    <strong class="l" id="Strong3">实际搜索词分布</strong>
                    <span class="r">
                        <a class="mybutton hover" id="downexcel_keyword" href="javascript:void(0);"><font>导出Excel</font></a>
                    </span>
                </div>
                <div class="textbox">
                    <table id="table4" cellpadding="0" cellspacing="0" border="0" class="display">
                        <thead>
                            <tr>
                                <th style="width: 200px">搜索词
                                </th>
                                <th>跳转数
                                </th>
                                <th>活跃数（15分钟）
                                </th>
                                <th>激活数（15分钟）
                                </th>
                                <th>激活转化率
                                </th>
                                <th>活跃数（30分钟）
                                </th>
                                <th>激活数（30分钟）
                                </th>
                                <th>激活转化率
                                </th>
                            </tr>
                        </thead>
                        <tbody>
                        </tbody>
                    </table>
                </div>
            </div>
            <div id="divRetainedDetail">
                <div class="title" style="margin-top: 4px;">
                    <strong class="l" id="Strong5">7天留存</strong>
                    <span class="r">
                        <a class="mybutton hover" id="downexcel_retained" href="javascript:void(0);"><font>导出Excel</font></a>
                    </span>
                </div>
                <div class="textbox">
                    <table id="table5" cellpadding="0" cellspacing="0" border="0" class="display">
                        <thead>
                            <tr>
                                <th>日期
                                </th>
                                <th>激活用户
                                </th>
                                <th>第1天
                                </th>
                                <th>第2天
                                </th>
                                <th>第3天
                                </th>
                                <th>第4天
                                </th>
                                <th>第5天
                                </th>
                                <th>第6天
                                </th>
                                <th>第7天
                                </th>
                            </tr>
                        </thead>
                        <tbody>
                        </tbody>
                    </table>
                </div>
            </div>
        </div>
        <a id="windowHours"></a>
        <a id="windowArea"></a>
        <a id="windowKeyword"></a>
        <a id="windowRetained"></a>
    </form>
</body>
</html>
