<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MsmqMonitor.aspx.cs" Inherits="net91com.Stat.Web.Monitor.MsmqMonitor" %>

<%@ Register Src="/Monitor/Controls/DataLogControl.ascx" TagName="DataLogControl"
    TagPrefix="uc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
   
    <link href="/css/ReportCss.css" rel="stylesheet" type="text/css" />
    <script src="/Scripts/HeadScript/jquery.js" type="text/javascript"></script>
    <script src="/Scripts/highcharts.js" type="text/javascript"></script>
    <script src="/Scripts/exporting.js" type="text/javascript"></script>
    <script src="/Scripts/common.js" type="text/javascript"></script>
    <script src="/Scripts/jTip.js" type="text/javascript"></script>
    <script src="/Scripts/jquery.loadmask.min.js" type="text/javascript"></script>

</head>
<body>
    <form id="form1" runat="server">
    <div class="main">
        <uc1:DataLogControl ID="HeadControl1" runat="server" />
        <div style="border: 1px solid #CCCCCC; margin: 3px 0;">

            <div id="containner" style="width: 95%; text-align: center; height: 500px; margin: 0 auto;">
            </div>
        </div>
    </div>
    </form>
</body>
</html>
<script type="text/javascript">
        function checkCondition(){
        }
        $(function () {
            $("#containner").html(" <div  > <img height='25px' src='/images/defaultloading.gif'> <br> 正在加载统计数据... </div>");
            setTimeout("getchart()",50);
            
       });
        function getchart() { 
         $("#containner").html("");
         chart = new Highcharts.Chart({
                chart: {
                    renderTo: 'containner',
                    defaultSeriesType: 'line',
                    height:500
                   
                },
                title: {
                    text: '<%=ReportTitle %>',
                     style: {
                                 
                                color: '#202020',
                                fontWeight: 'bold'
 
                              
                            }
                },
               
                xAxis:  <%= AxisJsonStr %> ,
                yAxis: {
                    min: 0,
                    allowDecimals: false,
                    title: {
                         text: '量'
                    },
                   
	                ///将y轴分割成一些小点
                    minorGridLineWidth: 0,
                    minorTickInterval: 'auto',
                    minorTickColor: '#000000',
                    minorTickWidth: 1 
                   


                },
                tooltip: {
                    formatter: function () {
                         return this.series.name + "<br/>" +"时间："+this.x +"<br/>消息数：" +Highcharts.numberFormat(this.point.y, 0);
 
                     }
 
                },
               
              

                series: <%=SeriesJsonStr %>,

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
                            enabled:false 
                               },
                     printButton: {
                           x: -10
                      }
                   }
                },
                ///设置右下角的广告，我这里不启用他
                credits:
                {
                    enabled:false
                }

                

            });
       
       }

</script>
