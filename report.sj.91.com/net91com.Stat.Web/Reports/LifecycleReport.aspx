<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LifecycleReport.aspx.cs"
    Inherits="net91com.Stat.Web.Reports.LifecycleReport" EnableViewState="false" EnableViewStateMac="false" %>
<%@ Register Src="/Reports/Controls/HeadAllControl.ascx" TagName="HeadControl" TagPrefix="uc1"  %>
<%@ Import Namespace="net91com.Stat.Web.Reports.Services" %>
<%@ Import Namespace="net91com.Stat.Web.Base" %>
<%@ Import Namespace="net91com.Stat.Services.Entity" %>
<%@ Register Src="Controls/PeriodSelector.ascx" TagName="PeriodSelector" TagPrefix="uc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
     <link href="../css/jquery.loadmask.css" rel="stylesheet" type="text/css" />
    <link href="../css/ReportCss.css" rel="stylesheet" type="text/css" />
    <link href="../css/help.css" rel="Stylesheet" type="text/css" />
    <script src="../Scripts/HeadScript/jquery.js" type="text/javascript"></script>
    <script src="../Scripts/highcharts.js" type="text/javascript"></script>
    <script src="../Scripts/exporting.js" type="text/javascript"></script>
    <script src="../Scripts/common.js" type="text/javascript"></script>
    <script src="../Scripts/jTip.js" type="text/javascript"></script>
    <script src="../Scripts/jquery.loadmask.min.js" type="text/javascript"></script>
    <script type="text/javascript">
        var chart;
        ///提交前检查
        ///自定义控件检查函数实现
        function checkCondition() {
            ischecked = true;           
        }        
    </script>
    
</head>
<body>
    <form id="form1" runat="server">
    <div  class="main">
    <uc1:HeadControl ID="HeadControl1"   runat="server" />
  
    
    <div>
        <%--<uc1:PeriodSelector ID="PeriodSelector1" runat="server" />--%>
        <div style="border: 1px solid #CCCCCC; margin: 3px 0;">
            <div id="containner" style="width: 95%; margin: 0 auto; text-align:center; height:500px;">
            </div>
        </div>
    <script type="text/javascript">
       $(function () {
         $("#containner").html(" <div  > <img height='25px' src='../images/defaultloading.gif'> <br> 正在加载统计数据... </div>");
           setTimeout('getchart()',50);
           
        });
    function getchart()
    {
              $("#containner").html("");
          ///报表制作
            chart = new Highcharts.Chart({
                chart: {
                    renderTo: 'containner',
                    type: 'column',
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
                        text: '留存率(%)'
                    },
                    labels: {
			            formatter: function() {
				        return this.value ;
			            }
		               },
                    minorGridLineWidth: 0,
                    minorTickInterval: 'auto',
                    minorTickColor: '#000000',
                    minorTickWidth: 1 
                },
                tooltip: {
                    formatter: function () {
                        return this.series.name + "<br/>" +"周期："+this.x +"<br/>留存：" +Highcharts.numberFormat(this.point.Denominator*this.point.y/100,0)+"<br/>总量："+this.point.Denominator +"<br/>留存百分比："+this.point.growth;
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
            ///报表制作结束
    }
    </script>
    </div>

    </div>
    </form>
</body>
</html>
