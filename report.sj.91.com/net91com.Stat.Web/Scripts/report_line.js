
       var chart;
       var maxX=16;
       var leb = "align:'left', rotation: -45, tickLength:80,tickPixelInterval:140 ,x:-30,y:45";
       if (xaxis.length > maxX) { var MaxStep = xaxis.length / 17.2; leb+=",step:" + Math.ceil(MaxStep) }
       var lbljson = eval('({'+leb+'})');
       $(function () {
           ///数据表格的点击列排序
           $("table").tablesorter({ widgets: ['zebra'] });
           ///报表制作
           chart = new Highcharts.Chart({
               chart: {
                   renderTo: 'containner',
                   type: 'line',
                   height: 500
               },
               title: {
                   text: reportTitle,
                   x: -20,
                   style: {
                       color: '#202020',
                       fontWeight: 'bold'
                   }
               },
               xAxis: {
                   categories: xaxis,
                   labels: lbljson
               },
               yAxis: {
                   min: 0,
                   title: {
                       text: '用户量'
                   },
                   labels: {
                       formatter: function () {
                           return this.value;
                       }
                   },
                   minorGridLineWidth: 0,
                   minorTickInterval: 'auto',
                   minorTickColor: '#000000',
                   minorTickWidth: 1
               },

               tooltip: {
                   formatter: function () {
                       if (period != 11 && period != 10) {
                           switch (formatType) {
                               case "new":
                                   return this.series.name + "<br/>" + "日期：" + this.x + "<br/>新增：" + Highcharts.numberFormat(this.point.y, 0) + "<br/>前一周期总量：" +  Highcharts.numberFormat(this.point.Denominator,0) + "<br/>增长率：" + this.point.growth;

                               case "active":
                                   return this.series.name + "<br/>" + "日期：" + this.x + "<br/>活跃：" + Highcharts.numberFormat(this.point.y, 0) + "<br/>总量：" +  Highcharts.numberFormat(this.point.Denominator,0) + "<br/>活跃度：" + this.point.growth;

                               case "use": return this.series.name + "<br/>" + "日期：" + this.x + "<br/>量：" + Highcharts.numberFormat(this.point.y, 0) + "<br/>总量：" +  Highcharts.numberFormat(this.point.Denominator,0) + "<br/>百分比：" + this.point.growth;
                               default:
                                   return '<b>' + this.series.name + '</b><br/>日期：' +
					        this.x + ' ' + this.y + '人';
                           }
                       } else {
                           var txt = "新增";
                           switch (formatType) {
                               case "new": txt = "新增"; break;
                               case "active": txt = "活跃"; break;
                               case "use": txt = "使用"; break;
                           }
                           return this.series.name + "<br/>" + "时间：" + this.x + "<br/>" + txt + "：" + Highcharts.numberFormat(this.point.y, 0) + "人";
                       }
                   }
               },
               series: series,

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
///报表制作结束
