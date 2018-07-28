<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ReportStat.aspx.cs" Inherits="net91com.Stat.Web.ReportStat" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>举报统计</title>
    <link href="/css/headcss/head.css" rel="Stylesheet" type="text/css" />
    <link href="/css/colorbox.css" rel="stylesheet" type="text/css" />
    <link href="/css/jquery.dataTables.css" rel="stylesheet" type="text/css" />
    <script src="/Scripts/HeadScript/jquery.js" type="text/javascript"></script>
    <script src="/Scripts/jquery.dataTables.min.js" type="text/javascript"></script>
    <script src="/Scripts/jquery.jselect.js" type="text/javascript"></script>
    <script src="/Scripts/jquery.colorbox-min.js" type="text/javascript"></script>
    <script src="/My97DatePicker/WdatePicker.js" type="text/javascript"></script>
    <script type="text/javascript">
        var serverUrl = '/Services/HttpReportService.ashx';
        var projectSource = 0;
        var platform = 0;
        var resType = 0;
        var beginTime = '';
        var endTime = '';
        var option = 0;
        var state = 0;

        var oTable;
        $(function () {
            init();

            loadData();
        });

        // 初始化
        function init() {
            if ($("#txtBeginTime").val() == "") {
                var date = new Date();
                var y = date.getFullYear();
                var M = "0" + (date.getMonth() + 1);
                M = M.substring(M.length - 2);
                var d = "0" + date.getDate();
                d = d.substring(d.length - 2);
                $("#txtEndTime").val(y + "-" + M + "-" + d);

                date.setMonth(date.getMonth() - 1);
                y = date.getFullYear();
                M = "0" + (date.getMonth() + 1);
                M = M.substring(M.length - 2);
                d = "0" + date.getDate();
                d = d.substring(d.length - 2);
                $("#txtBeginTime").val(y + "-" + M + "-" + d);
            }

            $('#ddlReportOptions').jselect({
                replaceAll: false,
                onChange: function (value, text) {
                    changeOption();
                }
            });
        }

        function check() {
            if ($("#ddlResType").val() == "") {
                alert("请选择资源类型！");
                return false;
            }
            if ($("#ddlProjectSource").val() == "") {
                alert("请选择来源！");
                return false;
            }
            if ($("#ddlPlatform").val() == "") {
                alert("请选择平台！");
                return false;
            }
            if ($("#txtBeginTime").val() == "") {
                alert("请选择开始日期");
                return false;
            }
            if ($("#txtEndTime").val() == "") {
                alert("请选择结束日期");
                return false;
            }
            var s = $("#txtBeginTime").val().split('-');
            var sdate = new Date(s[0], s[1] - 1, s[2]);
            var e = $("#txtEndTime").val().split('-');
            var edate = new Date(e[0], e[1] - 1, e[2]);
            var t = edate.getTime() - sdate.getTime();
            if (t < 0) {
                alert("开始日期不能大于结束日期");
                return false;
            }
            var days = new Date(s[0], s[1], 0).getDate() + 1;
            if (t >= days * 24 * 60 * 60 * 1000) {
                alert("日期跨度不能超过一个月");
                return false;
            }
            return true;
        }

        function changeOption() {
            option = $("#ddlReportOptions").val();
            loadTable();
        }

        function loadData() {
            if (!check()) {
                return;
            }
            // 设置变量
            option = 0;
            projectSource = $("#ddlProjectSource").val();
            platform = $("#ddlPlatform").val();
            resType = $("#ddlResType").val();
            beginTime = $("#txtBeginTime").val();
            endTime = $("#txtEndTime").val();
            state = $("#ddlState").val();

            loadOption();
        }

        // 加载举报类型
        function loadOption() {
            $.ajax({
                "dataType": 'json',
                "type": "POST",
                "url": serverUrl,
                "data": {
                    "act": "get_total",
                    "projectsource": projectSource,
                    "platform": platform,
                    "restype": resType,
                    "begintime": beginTime,
                    "endtime": endTime,
                    "state": state
                },
                "success": function (obj) {
                    var total = 0;
                    $('#ddlReportOptions').empty();
                    for (var opt in obj) {
                        var num = parseInt(obj[opt]);
                        total += num;
                        $('#ddlReportOptions').append("<option value='" + opt + "'>" + getOptionName(parseInt(opt)) + "（" + num + "）</option>");
                    }
                    $('#ddlReportOptions').prepend("<option value='0'>全部（" + total + "）</option>");
                    $('#ddlReportOptions').val(option);

                    loadTable();
                }
            });
        }

        // 加载表格
        function loadTable() {
            if (oTable) {
                oTable.fnDraw();
            }
            else {
                oTable = $('#table1').dataTable({
                    "aoColumnDefs": [{
                        "sClass": "center", "aTargets": [0, 3, 5, 7, 8]
                    }, {
                        "bSortable": false, "aTargets": [2, 4, 5, 6, 7, 8]
                    }, {
                        "bUseRendered": false,
                        "fnRender": function (oObj) {
                            return "<a href='javascript:void(0);' onclick='showDetail(" +
                            projectSource + "," +
                            platform + "," +
                            resType + "," +
                            oObj.aData[0] + ",\"" +
                            oObj.aData[1] + "\",\"" +
                            oObj.aData[2] + "\",\"" +
                            beginTime + "\",\"" +
                            endTime + "\"," +
                            option + ");'>" + oObj.aData[1] + "</a>";
                            return info;
                        },
                        "aTargets": [1]
                    }, {
                        "bUseRendered": false,
                        "fnRender": function (oObj) {
                            if (option == 0 && state == 0) {
                                var obj = eval('(' + oObj.aData[4] + ')');
                                var info = '';
                                for (var opt in obj) {
                                    info += (getOptionName(parseInt(opt)) +
                                    "（<a href='javascript:void(0);' onclick='showDetail(" +
                                    projectSource + "," +
                                    platform + "," +
                                    resType + "," +
                                    oObj.aData[0] + ",\"" +
                                    oObj.aData[1] + "\",\"" +
                                    oObj.aData[2] + "\",\"" +
                                    beginTime + "\",\"" +
                                    endTime + "\"," +
                                    opt + ");'>" + obj[opt] + "</a>） ");
                                }
                                return info;
                            }
                            else {
                                return getOptionName(parseInt(oObj.aData[4]));
                            }
                        },
                        "aTargets": [4]
                    }, {
                        "bUseRendered": false,
                        "fnRender": function (oObj) {
                            var option = parseInt(oObj.aData[4]);
                            if (option == (1 << 30)) {
                                return '';
                            }
                            else {
                                return getProcessStateName(parseInt(oObj.aData[5]));
                            }
                        },
                        "aTargets": [5]
                    }, {
                        "bUseRendered": false,
                        "fnRender": function (oObj) {
                            var resId = parseInt(oObj.aData[0]);
                            var resVersion = parseInt(oObj.aData[2]);
                            var option = parseInt(oObj.aData[4]);
                            var state = parseInt(oObj.aData[5]);
                            if (option == (1 << 30) || state == 2) {
                                return '';
                            }
                            else {
                                return "<a href='javascript:void(0);' onclick='process(" + projectSource + "," + platform + "," + resType + "," + resId + "," + resVersion + "," + option + "," + state + ");'>" + getOperateName(state) + "</a>";
                            }
                        },
                        "aTargets": [8]
                    }],
                    "iDisplayLength": 50,
                    "aaSorting": [[3, "desc"]],
                    "sDom": '<"top">rt<"bottom"ip><"clear">',
                    "bProcessing": true,
                    "bServerSide": true,
                    "oLanguage": { sUrl: "/Scripts/de_DE.txt" },
                    "bFilter": false,
                    "sPaginationType": "full_numbers",
                    "sAjaxSource": serverUrl,
                    "fnServerParams": function (aoData) {
                        aoData.push({
                            "name": "act", "value": "get_page"
                        }, {
                            "name": "projectsource", "value": projectSource
                        }, {
                            "name": "platform", "value": platform
                        }, {
                            "name": "restype", "value": resType
                        }, {
                            "name": "begintime", "value": beginTime
                        }, {
                            "name": "endtime", "value": endTime
                        }, {
                            "name": "state", "value": state
                        }, {
                            "name": "option", "value": option
                        });
                    },
                    "fnServerData": function (sSource, aoData, fnCallback) {
                        $.ajax({
                            "dataType": 'json',
                            "type": "POST",
                            "url": sSource,
                            "data": aoData,
                            "success": function (data) {
                                //存在输出的resultCode 让他输出message
                                if (data.resultCode && data.resultCode != 0) {
                                    alert(data.message);
                                } else {
                                    fnCallback(data);
                                }

                            }
                        });
                    }
                });
            }

            // 设置隐藏列
            oTable.fnSetColumnVis(5, true, false);
            oTable.fnSetColumnVis(6, true, false);
            oTable.fnSetColumnVis(7, true, false);
            oTable.fnSetColumnVis(8, true, false);
            if (state == 0 && option == 0) {
                oTable.fnSetColumnVis(5, false, false);
                oTable.fnSetColumnVis(6, false, false);
                oTable.fnSetColumnVis(7, false, false);
                oTable.fnSetColumnVis(8, false, false);
            }
            else if (state == 0 && option > 0) {
                // 待处理：隐藏“反馈”、“处理时间”列
                oTable.fnSetColumnVis(6, false, false);
                oTable.fnSetColumnVis(7, false, false);
            }
            else if (state == 2) {
                // 已处理：隐藏“操作”列
                oTable.fnSetColumnVis(8, false, false);
            }
        }

        function process(projectSource, platform, resType, resId, resVersion, option, state) {
            $.colorbox({
                href: 'ReportProcess.aspx?' +
                '&projectsource=' + projectSource +
                '&platform=' + platform +
                '&restype=' + resType +
                '&resid=' + resId +
                '&resversion=' + resVersion +
                '&option=' + option +
                '&state=' + state,
                transition: 'none',
                title: '反馈进度',
                iframe: true,
                innerWidth: '420',
                innerHeight: '260',
                overlayClose: false,
                onClosed: function () {
                    // 重新加载
                    loadOption();
                }
            });
        }

        function showDetail(projectSource, platform, resType, resId, resName, resVersion, beginTime, endTime, option) {
            var father = window.parent;
            var url = "/Reports/ReportDetail.aspx?" +
            "projectsource=" + projectSource +
            "&platform=" + platform +
            "&restype=" + resType +
            "&resid=" + resId +
            "&resname=" + encodeURIComponent(resName) +
            "&resversion=" + resVersion +
            "&begintime=" + beginTime +
            "&endtime=" + endTime +
            "&option=" + option;
            var title = '举报详情';
            var id = 'ReportDetail_id';
            var menuitem = father.Ext.getCmp(id);
            var menupanel = father.Ext.getCmp("MenuPanel" + $('#menuPannelKey').val());
            father.moveTab(id);
            father.addOtherTab(father.TabPanel1, 'idClt' + id, url, title, menuitem, menupanel);
        }

        function getOptionName(option) {
            switch (option) {
                case 1:
                    return '无法下载';
                case 1 << 1:
                    return '无法安装';
                case 1 << 2:
                    return '无法启动';
                case 1 << 3:
                    return '无法更新';
                case 1 << 11:
                    return '含暴力色情';
                case 1 << 12:
                    return '广告过多';
                case 1 << 13:
                    return '与描述不符';
                case 1 << 14:
                    return '恶意扣费';
                case 1 << 15:
                    return '偷流量';
                case 1 << 16:
                    return '携带病毒';
                case 1 << 17:
                    return '侵犯版权';
                case 1 << 18:
                    return '当前版本较旧';
                case 1 << 30:
                    return '其他';
                default:
                    return '未知';
            }
        }

        function getProcessStateName(state) {
            switch (state) {
                case 1:
                    return '<font color=blue>处理中</font>';
                case 2:
                    return '<font color=green>已处理</font>';
                case 3:
                    return '<font color=gray>忽略中</font>';
                default:
                    return '<font color=red>待处理</font>';
            }
        }

        function getOperateName(state) {
            switch (state) {
                case 1:
                    return '反馈进度';
                case 2:
                    return '';
                case 3:
                    return '取消忽略';
                default:
                    return '处理';
            }
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <input type="hidden" id="menuPannelKey" runat="server" />
    <div>
        <div class="head">
            <div class="divitem">
                来源：<select id="ddlProjectSource" runat="server">
                </select>
            </div>
            <div class="divitem">
                平台：
                <select id="ddlPlatform" runat="server">
                </select>
            </div>
            <div class="divitem">
                资源类型：
                <select id="ddlResType" runat="server">
                    <option value="1">应用</option>
                </select>
            </div>
            <div class="divitem">
                处理状态：
                <select id="ddlState">
                    <option value="0" selected="selected">待处理</option>
                    <option value="1">处理中</option>
                    <option value="2">已处理</option>
                    <option value="3">忽略中</option>
                </select>
            </div>
            <div class="divitem">
                日期从：
                <input type="text" id="txtBeginTime" onclick="WdatePicker()" runat="server" style="width: 80px" />
                到：
                <input type="text" id="txtEndTime" onclick="WdatePicker()" runat="server" style="width: 80px" />
            </div>
            <div class="divitem">
                <span style="cursor: pointer;"><a class="mybutton hover" style="margin-top: 4px"
                    onclick="loadData();"><font>查询</font> </a></span>
            </div>
        </div>
        <div class="title">
            <strong class="l">举报类型</strong>
            <select id="ddlReportOptions" style="margin-top: 8px">
                <option value="0">全部</option>
            </select>
            <span class="r"></span>
        </div>
        <div class="textbox">
            <table id="table1" cellpadding="0" cellspacing="0" border="0" class="display">
                <thead>
                    <tr>
                        <th style="width: 80px; white-space: nowrap">
                            应用ID
                        </th>
                        <th style="width: 200px; white-space: nowrap">
                            应用名称
                        </th>
                        <th style="width: 100px; white-space: nowrap">
                            应用版本号
                        </th>
                        <th style="width: 80px; white-space: nowrap">
                            举报次数
                        </th>
                        <th style="width: 100px; white-space: nowrap">
                            举报内容
                        </th>
                        <th style="width: 100px; white-space: nowrap">
                            状态
                        </th>
                        <th style="width: 100px; white-space: nowrap">
                            进度反馈
                        </th>
                        <th style="width: 180px; white-space: nowrap">
                            处理时间
                        </th>
                        <th style="width: 120px; white-space: nowrap">
                            操作
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
