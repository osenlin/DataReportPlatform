<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ReportDetail.aspx.cs" Inherits="net91com.Stat.Web.ReportDetail" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>举报详情</title>
    <link href="/css/headcss/head.css" rel="Stylesheet" type="text/css" />
    <link href="/css/colorbox.css" rel="stylesheet" type="text/css" />
    <link href="/css/jquery.dataTables.css" rel="stylesheet" type="text/css" />
    <script src="/Scripts/HeadScript/jquery.js" type="text/javascript"></script>
    <script src="/Scripts/jquery.query-2.1.7.js" type="text/javascript"></script>
    <script src="/Scripts/jquery.dataTables.min.js" type="text/javascript"></script>
    <script src="/Scripts/jquery.jselect.js" type="text/javascript"></script>
    <script src="/Scripts/jquery.colorbox-min.js" type="text/javascript"></script>
    <script src="/My97DatePicker/WdatePicker.js" type="text/javascript"></script>
    <script type="text/javascript">
        var serverUrl = '/Services/HttpReportService.ashx';
        var projectSource = $.query.get("projectsource");
        var platform = $.query.get("platform");
        var resType = $.query.get("restype");
        var resId = $.query.get("resid");
        var resName = $.query.get("resname");
        var resVersion = $.query.get("resversion");
        var beginTime = $.query.get("begintime");
        var endTime = $.query.get("endtime");
        var option = $.query.get("option");

        var totalNum = 0;
        var oTableTotal;
        var oTableDetail;
        var hasLoadDetail = true;
        $(function () {
            init();

            $('#ddlReportOptions').jselect({
                replaceAll: false,
                onChange: function (value, text) {
                    loadDetailList();
                }
            });

            $('#btnShowByFw').colorbox({
                href: 'ReportByDevice.aspx?type=0' +
                '&projectsource=' + projectSource +
                '&platform=' + platform +
                '&restype=' + resType +
                '&resid=' + resId +
                '&resversion=' + resVersion +
                '&begintime=' + beginTime +
                '&endtime=' + endTime +
                '&option=' + $('#ddlReportOptions').val(),
                transition: 'none',
                iframe: true,
                title: '手机系统统计',
                innerWidth: '720',
                innerHeight: '540',
                overlayClose: false
            });

            $('#btnShowByModel').colorbox({
                href: 'ReportByDevice.aspx?type=1' +
                '&projectsource=' + projectSource +
                '&platform=' + platform +
                '&restype=' + resType +
                '&resid=' + resId +
                '&resversion=' + resVersion +
                '&begintime=' + beginTime +
                '&endtime=' + endTime +
                '&option=' + $('#ddlReportOptions').val(),
                transition: 'none',
                iframe: true,
                title: '机型统计',
                innerWidth: '720',
                innerHeight: '540',
                overlayClose: false
            });

            oTableTotal = $('#tableTotal').dataTable({
                "aoColumnDefs": [{
                    "bUseRendered": false,
                    "fnRender": function (oObj) {
                        return getOptionName(parseInt(oObj.aData[0]));
                    },
                    "aTargets": [0]
                }, {
                    "bUseRendered": false,
                    "fnRender": function (oObj) {
                        var option = parseInt(oObj.aData[0]);
                        if (option == (1 << 30)) {
                            return '';
                        }
                        else {
                            return getProcessStateName(parseInt(oObj.aData[2]));
                        }
                    },
                    "aTargets": [2]
                }, {
                    "bUseRendered": false,
                    "fnRender": function (oObj) {
                        var option = parseInt(oObj.aData[0]);
                        var state = parseInt(oObj.aData[2]);
                        if (option == (1 << 30) || state == 2) {
                            return '';
                        }
                        else {
                            return "<a href='javascript:void(0);' onclick='process(" + projectSource + "," + platform + "," + resType + "," + resId + "," + resVersion + "," + option + "," + state + ");'>" + getOperateName(state) + "</a>";
                        }
                    },
                    "aTargets": [5]
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
                        "name": "act", "value": "get_total_by_id"
                    }, {
                        "name": "projectsource", "value": projectSource
                    }, {
                        "name": "platform", "value": platform
                    }, {
                        "name": "restype", "value": resType
                    }, {
                        "name": "resid", "value": resId
                    }, {
                        "name": "resversion", "value": resVersion
                    }, {
                        "name": "begintime", "value": beginTime
                    }, {
                        "name": "endtime", "value": endTime
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
                },
                "fnDrawCallback": function (oSettings) {
                    if (hasLoadDetail) {
                        // 填充总次数
                        $('#txtTotalNum').html(totalNum);
                        totalNum = 0;

                        // 填充举报类型下拉
                        $('#ddlReportOptions').empty();
                        $('#ddlReportOptions').append("<option value='0'>全部</option>");
                        for (var i in oSettings.aoData) {
                            var obj = oSettings.aoData[i]._aData;
                            $('#ddlReportOptions').append("<option value='" + obj[0] + "'>" + getOptionName(parseInt(obj[0])) + "（" + obj[1] + "）</option>");
                        }
                        // 设置默认举报类型
                        $('#ddlReportOptions').val(option);

                        // 加载详情
                        loadDetailList();
                    }
                    hasLoadDetail = true;
                },
                "fnRowCallback": function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {
                    totalNum += parseInt(aData[1]);
                    return nRow;
                }
            });

        });

        // 初始化
        function init() {
            $('#txtResName').html("<font size='5' color='greed'>" + resName + "</font>");
            $('#txtResId').html(resId);
            $('#txtResVersion').html(resVersion);
        }

        function loadTotalList() {
            oTableTotal.fnDraw();
        }

        function loadDetailList() {
            if (oTableDetail) {
                oTableDetail.fnDraw();
            }
            else {
                oTableDetail = $('#tableDetail').dataTable({
                    "aoColumnDefs": [{
                        "bSortable": false, "aTargets": [0, 1, 2, 3, 4]
                    }, {
                        "fnRender": function (oObj) {
                            return getOptionsName(parseInt(oObj.aData[0]));
                        },
                        "aTargets": [0]
                    }],
                    "iDisplayLength": 50,
                    "aaSorting": [[5, "desc"]],
                    "sDom": '<"top">rt<"bottom"ip><"clear">',
                    "bProcessing": true,
                    "bServerSide": true,
                    "oLanguage": { sUrl: "/Scripts/de_DE.txt" },
                    "bFilter": false,
                    "sPaginationType": "full_numbers",
                    "sAjaxSource": serverUrl,
                    "fnServerParams": function (aoData) {
                        aoData.push({
                            "name": "act", "value": "get_page_by_id"
                        }, {
                            "name": "projectsource", "value": projectSource
                        }, {
                            "name": "platform", "value": platform
                        }, {
                            "name": "restype", "value": resType
                        }, {
                            "name": "resid", "value": resId
                        }, {
                            "name": "resversion", "value": resVersion
                        }, {
                            "name": "begintime", "value": beginTime
                        }, {
                            "name": "endtime", "value": endTime
                        }, {
                            "name": "option", "value": $('#ddlReportOptions').val()
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
                    hasLoadDetail = false;
                    loadTotalList();
                }
            });
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

        var optionList = [1, 1 << 1, 1 << 2, 1 << 3, 1 << 11, 1 << 12, 1 << 13, 1 << 14, 1 << 15, 1 << 16, 1 << 17, 1 << 18, 1 << 30];
        function getOptionsName(options) {
            var result = '';
            for (var i in optionList) {
                var option = optionList[i];
                if (options < option) break;
                if ((options & option) > 0) {
                    result += (getOptionName(option) + "、");
                }
            }
            if (result.length > 0) {
                result = result.substring(0, result.length - 1);
            }
            return result;
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
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <div>
            <p id="txtResName">
            </p>
            应用ID：&nbsp;<span id="txtResId"></span>&nbsp;&nbsp; 版本号：&nbsp;<span id="txtResVersion"></span>&nbsp;&nbsp;
            总举报次数：&nbsp;<span id="txtTotalNum">加载中..</span>
        </div>
        <br />
        <div class="title" style="margin-top: 4px;">
            <strong class="l">分类合计</strong> <span class="r"></span>
        </div>
        <div class="textbox">
            <table id="tableTotal" cellpadding="0" cellspacing="0" border="0" class="display">
                <thead>
                    <tr>
                        <th style="width: 120px">
                            举报类型
                        </th>
                        <th style="width: 80px">
                            举报次数
                        </th>
                        <th style="width: 60px">
                            状态
                        </th>
                        <th>
                            进度反馈
                        </th>
                        <th style="width: 180px">
                            处理时间
                        </th>
                        <th style="width: 120px">
                            操作
                        </th>
                    </tr>
                </thead>
                <tbody>
                </tbody>
            </table>
        </div>
        <br />
        <div class="title">
            <strong class="l">举报明细
                <select id="ddlReportOptions">
                </select>
                <input type="button" id="btnShowByFw" value="查看手机系统统计" />&nbsp;&nbsp;<input type="button"
                    id="btnShowByModel" value="查看机型统计" /></strong> <span class="r"></span>
        </div>
        <div class="textbox">
            <table id="tableDetail" cellpadding="0" cellspacing="0" border="0" class="display">
                <thead>
                    <tr>
                        <th>
                            举报类型
                        </th>
                        <th style="width: 80px">
                            举报者ID
                        </th>
                        <th style="width: 120px">
                            举报者IP
                        </th>
                        <th>
                            举报者机型
                        </th>
                        <th>
                            举报者手机系统
                        </th>
                        <th style="width: 180px">
                            举报时间
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
