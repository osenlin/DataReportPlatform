<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SoftAreaDetails.aspx.cs" Inherits="net91com.Stat.Web.Reports.TransverseReports.SoftAreaDetails" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>具体城市详情</title>
    <link href="../../css/jquery.loadmask.css" rel="stylesheet" type="text/css" />
    <link href="../../css/ReportCss.css" rel="stylesheet" type="text/css" />
    <link href="../../css/help.css" rel="Stylesheet" type="text/css" />
    <link href="/Reports_New/css/colorbox.css" rel="stylesheet" type="text/css" />
   
    <script src="../../Scripts/HeadScript/jquery.js" type="text/javascript"></script>
     <script src="/Reports_New/js/jquery-1.6.4.min.js" type="text/javascript"></script>
    <script src="../../Scripts/highcharts.js" type="text/javascript"></script>
    <script src="../../Scripts/exporting.js" type="text/javascript"></script>
    <script src="../../Scripts/common.js" type="text/javascript"></script>
    <script src="../../Scripts/jTip.js" type="text/javascript"></script>
    <script src="../../Scripts/jquery.loadmask.min.js" type="text/javascript"></script>
     <script src="/Reports_New/js/jquery.colorbox-min.js" type="text/javascript"></script>
     <style type="text/css">
        .find
        {
            float: left;
            margin-left: 20px;
            cursor: pointer;
        }
        
    </style>
    <script type="text/javascript">
        var channelType = '<%=ChannelCate%>';
        var channelids = '<%=ChannelIDStrs%>';
        var begintime = '<%=BeginTime.ToString("yyyy-MM-dd") %>';
        var endtime = '<%=EndTime.ToString("yyyy-MM-dd") %>';
        var soft = '<%=Softid %>';
        var platform = '<%=Platform %>';
        var province = '<%=AreaName %>';
        function showLine(ix, areaname) {

            if ($("#tr" + ix).is(":hidden")) {

                $(".divtr").hide();
                $(".find").text("查看每天");

                $("#lbl" + ix).text("收缩");

                $("#tr" + ix).show();
                //若没有内容
                if ($.trim($("#div" + ix).html()) == "") {
                    getchart2(ix, areaname);
                }
            } else {

                $("#tr" + ix).hide();
                $("#lbl" + ix).text("查看每天");
            }
        }
        function getchart2(ix, areaname) {
            var url = "ServerHandler.ashx?act=getareacitydetail&soft=" + soft + "&plat=" + platform + "&sdate=" + begintime +
                "&edate=" + endtime + "&channelids=" + channelids + "&channelType=" + channelType + "&province=" + province + "&areaname=" + escape(areaname) + "&n=" + Math.random();
            $.get(url, function (data) {
                var mydata = eval("(" + data + ")");
                ///下拉的线图
                if (mydata.y.length == 0) {
                    $(".divtr").hide();
                    $(".find").text("查看每天");
                    alert("暂无该时段数据");
                    return;
                }
                chartline = new Highcharts.Chart({
                    chart: {
                        renderTo: 'div' + ix,
                        defaultSeriesType: 'line',
                        height: 400
                    },
                    title: {

                        style: {
                            color: '#202020',
                            fontWeight: 'bold',
                            display: 'none'
                        }
                    },
                    xAxis: mydata.x,
                    yAxis: {
                        min: 0,
                        allowDecimals: false,
                        title: {
                            text: '用户数'
                        },
                        minorGridLineWidth: 0,
                        minorTickInterval: 'auto',
                        minorTickColor: '#000000',
                        minorTickWidth: 1
                    },

                    tooltip: {
                        formatter: function () {
                            return this.series.name + "<br/>" + "日期：" + this.x + "<br/>用户数：" + Highcharts.numberFormat(this.point.y, 0);
                        }

                    },
                    plotOptions: {
                        line: {
                            dataLabels: {
                                enabled: true
                            }
                        }
                    },
                    series: mydata.y,
                    ///打印下载模块按钮设置
                    navigation: {
                        buttonOptions: {
                            height: 30,
                            width: 38,
                            symbolSize: 18,
                            symbolX: 20,
                            symbolY: 16,
                            symbolStrokeWidth: 2,
                            backgroundColor: 'white'
                        }
                    },
                    ///设置导出相关的参数，下面设置他的位置
                    exporting: {
                        buttons: {
                            exportButton: {
                                enabled: false
                            },
                            printButton: {
                                x: -10
                            }
                        }
                    },
                    ///设置右下角的广告，我这里不启用他
                    credits:
                    {
                        enabled: false
                    }
                });


            });

        }

    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div class="main">
        <%=TableString %>
    </div>
    </form>
</body>
</html>
