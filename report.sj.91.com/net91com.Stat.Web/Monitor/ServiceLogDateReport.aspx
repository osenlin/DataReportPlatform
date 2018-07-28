<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ServiceLogDateReport.aspx.cs"
    Inherits="net91com.Stat.Web.Monitor.ServiceLogDateReport" %>

<%@ Register Src="/Monitor/Controls/DataLogControl.ascx" TagName="DataLogControl"
    TagPrefix="uc1" %>
<%@ Register Src="/Reports/Controls/PeriodSelector.ascx" TagName="PeriodSelector" TagPrefix="uc1" %>
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

</head>
<body>
    <form id="form1" runat="server">
    <div class="main">
        <uc1:DataLogControl ID="HeadControl1" runat="server" />
        <uc1:PeriodSelector   ID="PeriodSelector1" runat="server" />
        <div style="border: 1px solid #CCCCCC; margin: 3px 0;">

            <div id="containner" style="width: 95%; text-align: center; height: 500px; margin: 0 auto;">
            </div>
        </div>
    </div>
    <asp:HiddenField ID="inputzhouqi" Value=""  runat="server" />
    </form>
</body>
</html>
<script type="text/javascript">
        function checkCondition()
        {
            if($("#inputzhouqi").val()=='小时')
            {
                ///这个在控件里有这个属性，来控制是否需要时间上限值
                needdatecheck=true;
            }
        }
        function getzhouqi( b) {

            var zq=$.trim($(b).text());
             $(".zhouqi").each(function () {
                if ($(this).hasClass("userselecttype2"))
                    $(this).removeClass("userselecttype2");
                $(this).addClass("userselecttype");
            });
            $(b).removeClass("userselecttype");
            $(b).addClass("userselecttype2");
            ///设置表单值
             $("#inputzhouqi").val($.trim($(b).text()));
             ///提交表单
             checkCondition();
             checkedforcontrol();
            if (ischecked == true)
            {
                    chart.showLoading();
                    $("#form1").submit();
            }
        }
        $(function () {
              ///初次进来样式周期切换样式设置
            $(".zhouqi").each(function () {
                if ($.trim($(this).text())== $("#inputzhouqi").val())
                {
                    $(this).removeClass("userselecttype");
                    $(this).addClass("userselecttype2");
                }
            });
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
                         return this.series.name + "<br/>" +"时间："+this.x +"<br/>日志大小(字节)：" +Highcharts.numberFormat(this.point.y, 0);
 
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
