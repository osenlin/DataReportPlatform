<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ReportByDevice.aspx.cs"
    Inherits="net91com.Stat.Web.ReportByDevice" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>按设备查询</title>
    <script src="/Scripts/HeadScript/jquery.js" type="text/javascript"></script>
    <script src="/Scripts/jquery.query-2.1.7.js" type="text/javascript"></script>
    <script src="/Scripts/highcharts.js" type="text/javascript"></script>
    <script type="text/javascript">
        var serverUrl = '/Services/HttpReportService.ashx';
        var type = $.query.get("type");
        var projectSource = $.query.get("projectsource");
        var platform = $.query.get("platform");
        var resType = $.query.get("restype");
        var resId = $.query.get("resid");
        var resVersion = $.query.get("resversion");
        var beginTime = $.query.get("begintime");
        var endTime = $.query.get("endtime");
        $(function () {
            var options = {
                chart: {
                    renderTo: 'container',
                    defaultSeriesType: 'pie',
                    plotBackgroundColor: null,
                    plotBorderWidth: null,
                    plotShadow: false
                },
                title: {
                    text: '举报分布',
                    style: { fontWeight: 'bold' }
                },
                tooltip: {
                    formatter: function () {
                        return "<b>" + this.point.name + ': </b>' + this.point.y + '次';
                    }
                },
                plotOptions: {
                    pie: {
                        allowPointSelect: true,
                        cursor: 'pointer',
                        dataLabels: {
                            enabled: true,
                            color: '#000000',
                            connectorColor: '#000000',
                            formatter: function () {
                                return '<b>' + this.point.name + '</b>: ' + Highcharts.numberFormat(this.percentage, 2) + ' %';
                            }
                        },
                        showInLegend: true
                    }
                },
                series: [{
                    type: 'pie',
                    name: '举报占比',
                    data: []
                }],
                credits: { enabled: false }
            };

            $.ajax({
                "dataType": 'json',
                "type": "POST",
                "url": serverUrl,
                "data": {
                    'act': 'get_by_device_to_chart',
                    'type': type,
                    'projectsource': projectSource,
                    'platform': platform,
                    'restype': resType,
                    'resid': resId,
                    'resversion': resVersion,
                    'begintime': beginTime,
                    'endtime': endTime
                },
                "success": function (obj) {
                    if (obj != null) {
                        if (type == 0) {
                            options.title.text = "手机系统分布";
                        }
                        else {
                            options.title.text = "机型分布";
                        }
                        options.series[0].data = obj;
                        var chart = new Highcharts.Chart(options);
                    }
                }
            });
        });
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div id="container" style="width: 95%; margin: 0 auto; text-align: center; height: 500px;">
    </div>
    </form>
</body>
</html>
