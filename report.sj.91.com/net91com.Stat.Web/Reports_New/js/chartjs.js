/// 创建曲线
function createLine(id, height, reportname, yname, showlegend, yminvalue, x, y, linetype) {
    var type = 'line';
    if (linetype)
        type = linetype;
    $("#" + id).html("");
    ////需要设置这样子全局对象,为了兼容IE7下不出错
    var chart_line = new Highcharts.Chart({
        chart: {
            renderTo: id,
            defaultSeriesType: type,
            height: height

        },
        title: {
            text: reportname,
            style: {
                color: '#202020',
                fontWeight: 'bold'
            }
        },
        xAxis: x,
        yAxis: {
            min: yminvalue,
            allowDecimals: false,
            title: {
                text: yname
            },

            ///将y轴分割成一些小点
            minorGridLineWidth: 0,
            minorTickInterval: 'auto',
            minorTickColor: '#000000',
            minorTickWidth: 1



        },
        tooltip: {
            formatter: function () {
                if (this.point.NumType == 2)
                    return this.x + "对应值:" + Highcharts.numberFormat(this.point.y, 2, '.', ',') + "%";
                else if (this.point.NumType == 1)
                    return this.x + "对应值:" + Highcharts.numberFormat(this.point.y, 0, '.', ',');
                else (this.point.NumType == 3)
                    return this.x + "对应值:" + Highcharts.numberFormat(this.point.y, 2, '.', ',');
            }

        },
        series: y,
        ///控制是否显示最下面的多条线
        legend: {
            enabled: showlegend
        },

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
        plotOptions: {

            line: {
                lineWidth: 3,
                dataLabels: {
                    enabled: true
                },
                cursor: 'pointer',
                dataLabels: {
                    formatter: function () {

                        if (this.point.NumType == 1)
                            return Highcharts.numberFormat(this.point.y, 0, '.', ',');
                        else
                            return Highcharts.numberFormat(this.point.y, 2, '.', ',');
                    }
                },
                marker: {
                    radius: 3
                }
            },
            spline: {
                lineWidth: 4,
                marker: {
                    enabled: false
                },
                dataLabels: {
                    formatter: function () {

                        if (this.point.NumType == 1)
                            return Highcharts.numberFormat(this.point.y, 0, '.', ',');
                        else
                            return Highcharts.numberFormat(this.point.y, 2, '.', ',');
                    }
                }
            }
        },
        ///设置导出相关的参数，下面设置他的位置
        exporting: {
            buttons: {
                exportButton: {
                    enabled: false
                },
                printButton: {
                    enabled: false
                }
            }
        },
        ///设置右下角的广告，我这里不启用他
        credits:
                {
                    enabled: false
                }
    });
    return chart_line;

}
///charttype is column or bar
function creatColumnOrBar(charttype, divid, title, ytitle, showlegend, yminvalue, x, y, numType) {
    /// 当值为2时就是百分比，为其他就正常显示
    var showType = 2;
    if (numType)
        showType = numType;
    $("#" + divid).html("");
    ////需要设置这样子全局对象,为了兼容IE7下不出错
   var chart_column = new Highcharts.Chart({
        chart: {
            renderTo: divid,
            type: charttype
        },
        title: {
            text: title
        },
        xAxis: x,
        yAxis: {
            min: yminvalue,
            title: {
                text: ytitle
            }
        },
        ///控制是否显示最下面的多条线
        legend: {
            enabled: showlegend
        },
        tooltip: {
            formatter: function () {
                if (showType == 2)
                    return this.x + "对应值:" + Highcharts.numberFormat(this.point.y, 2, '.', ',') + "%";
                else
                    return this.x + "对应值:" + Highcharts.numberFormat(this.point.y, 0, '.', ',');
            }
        },
        plotOptions: {
            column: {
                pointPadding: 0.2,
                borderWidth: 0,
                dataLabels: {
                    enabled: true,
                    formatter: function () {
                        if (showType == 2)
                            return Highcharts.numberFormat(this.point.y, 2, '.', ',');
                        else
                            return Highcharts.numberFormat(this.point.y, 0, '.', ',');
                    }
                }

            },
            bar: {
                dataLabels: {
                    enabled: true,
                    formatter: function () {
                        if (showType == 2)
                            return Highcharts.numberFormat(this.point.y, 2, '.', ',');
                        else
                            return Highcharts.numberFormat(this.point.y, 0, '.', ',');
                    }
                }


            }
        },
        series: y,

        ///设置导出相关的参数，下面设置他的位置
        exporting: {
            buttons: {
                exportButton: {
                    enabled: false
                },
                printButton: {
                    enabled: false
                }
            }
        },
        ///设置右下角的广告，我这里不启用他
        credits: {
            enabled: false
        }
    });
    return chart_column;
}


function createPie(divid,title, data) {
    $("#" + divid).html("");
   return  chart = new Highcharts.Chart({
        chart: {
            renderTo: divid,
            defaultSeriesType: 'pie',
            plotBackgroundColor: null,
            plotBorderWidth: null,
            plotShadow: false
        },
        title: {
            text: title,
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
        series: [{type:'pie',name:title,data:data}],

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
}
       

 