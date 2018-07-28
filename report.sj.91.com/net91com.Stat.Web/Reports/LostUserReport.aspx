<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LostUserReport.aspx.cs"
    Inherits="net91com.Stat.Web.Reports.LostUserReport" %>

<%@ Register Src="/Reports/Controls/HeadAllControl.ascx" TagName="HeadControl" TagPrefix="uc1" %>
<%@ Register Src="Controls/PeriodSelector.ascx" TagName="PeriodSelector" TagPrefix="uc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>流失率</title>
    <style type="text/css">
        #mytable table td
        { text-align:center; }
    </style>
    <link href="../css/ReportCss.css" rel="stylesheet" type="text/css" />
    <link href="../css/help.css" rel="Stylesheet" type="text/css" />
    <script src="../Scripts/HeadScript/jquery.js" type="text/javascript"></script>
    <script src="../Scripts/highcharts.js" type="text/javascript"></script>
    <script src="../Scripts/exporting.js" type="text/javascript"></script>
    <script src="../Scripts/common.js" type="text/javascript"></script>
</head>
<body>
    <form id="form1" runat="server">
    <div class="main">
        <uc1:HeadControl ID="HeadControl1" runat="server" />
        <div class="tablehead helpclass_head">
            <span class="helpclass_span1">&nbsp;&nbsp;流失用户 </span><span class="helpclass_span2">
                </span>
        </div>
        <div>
            <uc1:PeriodSelector ID="PeriodSelector1" runat="server" />
            <div style="border: 1px solid #CCCCCC; margin: 3px 0;">
                <div id="containner" style="width: 95%; text-align: center; height: 500px; margin: 0 auto;">
                </div>
            </div>
        </div>
        <%if (ListAll.Count != 0)
          { %>
        <div class="tablehead2">
            <div class="userselected" style="float: left; margin-top: 1px;">
                <%for (int m = 0; m < TabStr.Count; m++)
                  { %>
                <a class="<%if(m==0) {%>userselecttype2 <%}else{ %>userselecttype <%} %>  tabs" onclick="getTabs( this,<%=m%>)"
                    style="">
                    <%=TabStr[m]%></a>
                <% if (m < TabStr.Count - 1)
                   { %>
                &nbsp;&nbsp;|&nbsp;&nbsp;
                <%} %>
                <%} %>
            </div>
            <div style="float: right">
                <a id="download2" class="download2"><span>下载</span>
                    <img alt="下载" class="downloadimg" src="../Images/downloadexcel.gif" />
                </a>
            </div>
        </div>
        <div style="clear: both">
        </div>
        <div id="mytable">
            <%=TableStr %>
        </div>
        <%} %>
    </div>
    </form>
    <script type="text/javascript">
        $(function(){
            $("#containner").html(" <div  > <img height='25px' src='../images/defaultloading.gif'> <br> 正在加载统计数据... </div>");
            setTimeout("getchart()",50);
            geturl();
        });
         ///自定义控件检查函数实现
        function checkCondition() {
            ischecked = true;
        }
        function geturl() {
            var selecttabs = "";
            var tableName = $("#mytable table:visible").attr("name"); 
            var url = "AllHandler.ashx?action=downloadexcellost&" + "paras=" + tableName + "&rds=" + encodeURIComponent(new Date());
            $("#download2").attr("href", url);
        }
        ////table 切换
        function getTabs(b, m) {
            ///样式切换
            $(".tabs").each(function (index) {
                if ($(this).hasClass("userselecttype2")) {
                    $(this).removeClass("userselecttype2");
                    $(this).addClass("userselecttype");
                    ///数据隐藏
                    $("#tab" + index).hide();
                }
            });
            $(b).removeClass("userselecttype");
            $(b).addClass("userselecttype2");
            $("#tab" + m).show();
            geturl();
        }  
        
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
                                //color: '#1E6FAF',
                                color: '#202020',
                                 fontWeight: 'bold'
                            }
                },             
                xAxis:  <%= AxisJsonStr %> ,
                yAxis: {
                    min: 0,
                    allowDecimals: false,
                    title: {
                         text: '流失用户数'
                    },
                    ///将y轴分割成一些小点
                    minorGridLineWidth: 0,
                    minorTickInterval: 'auto',
                    minorTickColor: '#000000',
                    minorTickWidth: 1 
                },
                tooltip: {
                    formatter: function () {
                        return this.series.name + "<br/>" +"日期："+this.x+"<br/>流失：" +Highcharts.numberFormat(this.point.y, 0);
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
</body>
</html>
