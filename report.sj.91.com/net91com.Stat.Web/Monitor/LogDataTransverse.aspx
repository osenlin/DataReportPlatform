<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LogDataTransverse.aspx.cs" Inherits="net91com.Stat.Web.Monitor.LogDataTransverse" %>
<%@ Register Src="/Monitor/Controls/DataLogControl.ascx" TagName="DataLogControl"
    TagPrefix="uc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    
    <link href="/css/ReportCss.css" rel="stylesheet" type="text/css" />
    <link href="/css/help.css" rel="Stylesheet" type="text/css" />
    <script src="/Scripts/HeadScript/jquery.js" type="text/javascript"></script>
    <script src="/Scripts/highcharts.js" type="text/javascript"></script>
    <script src="/Scripts/exporting.js" type="text/javascript"></script>
    <script src="/Scripts/common.js" type="text/javascript"></script>
    <script src="/Scripts/jTip.js" type="text/javascript"></script>
    <script src="/Scripts/jquery.loadmask.min.js" type="text/javascript"></script>
    <script type="text/javascript">
        var chartjson =  <%=SeriesJsonStr %>;
        var reportTitle = '<%=reportTitle %>';
        function checkCondition() {
            ischecked = true;

        }
        $(function () {
            $("#containner").html(" <div  > <img height='25px' src='/images/defaultloading.gif'> <br> 正在加载统计数据... </div>");
            setTimeout("getchart()", 50);
            ///onready 结束
        });
        function getchart() {
            $("#containner").html("");
            chart = new Highcharts.Chart({
                chart: {
                    renderTo: 'containner',
                    defaultSeriesType: 'pie',
                    plotBackgroundColor: null,
                    plotBorderWidth: null,
                    plotShadow: false

                },
                title: {
                    text: reportTitle,
                    style: { fontWeight: 'bold' }
                },
                tooltip: {
                    formatter: function () {
                        return "<b>" + this.point.name + ': </b>' + this.point.y +" 字节";
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
                                return '<b>' + this.point.name + ': </b>' + Highcharts.numberFormat(this.percentage, 2) + ' %';
                            }
                        },
                        showInLegend: true
                    }

                },
                series: chartjson,

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
                        },
                //设置下面标题
                legend: {
                    enabled: true
                }
            });
            ///报表制作结束

        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div class="main">
         <uc1:DataLogControl ID="HeadControl1" runat="server" />
           
            <div style="border: 1px solid #CCCCCC; margin: 3px 0;">
                <div id="containner" style="width: 95%;text-align: center; margin: 0 auto;">
                </div>
        
        </div>  
    </div>
    </form>
</body>
</html>
