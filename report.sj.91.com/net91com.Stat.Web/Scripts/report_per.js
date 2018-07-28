var chart;
///其他的设置
$(function () {
    ///数据表格的点击列排序
    $("table").tablesorter({ widgets: ['zebra'] });
    ///报表制作
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
                return "<b>" + this.point.name + ': </b>' + this.point.y + '人';
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
});