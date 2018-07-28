<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="StatRetainedUsers.aspx.cs"
    Inherits="net91com.Stat.Web.Reports.StatRetainedUsers" EnableViewState="false"
    EnableViewStateMac="false" %>

<%@ Import Namespace="net91com.Stat.Web.Reports.Services" %>
<%@ Import Namespace="net91com.Stat.Web.Base" %>
<%@ Import Namespace="net91com.Stat.Services.Entity" %>
<%@ Register Src="/Reports/Controls/HeadAllControlChannel.ascx" TagName="HeadControl"
    TagPrefix="uc1" %>
<%@ Register Src="Controls/PeriodSelector.ascx" TagName="PeriodSelector" TagPrefix="uc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>新增用户留存率</title>
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
        ///自定义控件检查函数实现
        function checkCondition() {

            ischecked = true;

        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div class="main">
        <asp:HiddenField ID="inputzhouqi" runat="server" />
        <uc1:HeadControl ID="HeadControl1" runat="server" />
        <div class="tablehead helpclass_head">
            <span class="helpclass_span1">&nbsp;&nbsp;新增用户留存率</span><span class="helpclass_span2">
                <a id="A1" href="helphtm/retaineduserexplain.htm?ver=1" class="jTip">
                    <img alt="帮助" src="../Images/help.gif" /></a></span>
        </div>
        <div>
            <uc1:PeriodSelector ID="PeriodSelector1" runat="server" />
            <div style="border: 1px solid #CCCCCC; margin: 3px 0;">
                <div id="containner" style="width: 95%; margin: 0 auto; text-align: center; height: 500px;">
                </div>
            </div>
        </div>
        <div class="tablehead2">
            <div style="float: right">
                <a class="printexcel"><span>打印</span>
                    <img alt="打印" class="downloadimg" src="../Images/printout.gif" />
                </a><a class="download2"><span>下载</span>
                    <img alt="下载" class="downloadimg" src="../Images/downloadexcel.gif" />
                </a>
            </div>
        </div>
        <div style="clear: both">
        </div>
        <div id="mytable">
            <%=TablesHtml%>
        </div>
    </div>
    <script type="text/javascript">
      function geturl()
      {
                var url = "<%= ExcelDownUrl %>";
                $(".download2").attr("href",url);
        
      }
      $(function () {
                geturl();
            $(".printexcel").click(function(){
                //打印
               printout();
            });
              $("#containner").html(" <div  > <img height='25px' src='../images/defaultloading.gif'> <br> 正在加载统计数据... </div>");
              setTimeout("getchart()",50);
            //onready 结束
       });

       function getchart() { 
              $("#containner").html("");
             //报表制作
            chart = new Highcharts.Chart({
                chart: {
                    renderTo: 'containner',
                    defaultSeriesType: 'line',
                    marginRight: 50,
                    height:500
                   
                },
                title: {
                    text: '<%=ReportTitle %>',
                     style: {
                                 color: '#202020',
                                 fontWeight: 'bold'
                                
                            }
                },
                xAxis: <%= AxisJsonStr %>,
                yAxis: {
                    min: 0,
                    title: {
                        text: '留存率(%)'
                    },
                    allowDecimals: false,
                    labels: {
			            formatter: function() {
				        return this.value ;
			            }
		               },
                    //将y轴分割成一些小点
                    minorGridLineWidth: 0,
                    minorTickInterval: 'auto',
                    minorTickColor: '#000000',
                    minorTickWidth: 1
                },
                tooltip: {
                    formatter: function () {
                        <%if(Period!=net91com.Stat.Core.PeriodOptions.Daily){ %>
                        return this.series.name+"<br/>"+ this.point.Other + "<br/>留存：" +Highcharts.numberFormat(this.point.Denominator*this.point.y/100,0)+"<br/>新增："+Highcharts.numberFormat(this.point.Denominator,0)+"<br/>留存率："+this.point.growth;
                        <%}else{ %>
                        return this.series.name+"<br/>"+ this.point.Other + "<br/>回访：" +Highcharts.numberFormat(this.point.Denominator*this.point.y/100,0)+"<br/>新增："+Highcharts.numberFormat(this.point.Denominator,0)+"<br/>回访率："+this.point.growth;
                        <%} %>
                     }
 
                },
                series: <%=SeriesJsonStr %>,
                //打印下载模块按钮设置
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
                 //设置导出相关的参数，下面设置他的位置
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
                //设置右下角的广告，我这里不启用他
                credits:
                {
                    enabled:false
                }
            });
            //报表制作结束       
       }
    </script>
    </form>
</body>
</html>
