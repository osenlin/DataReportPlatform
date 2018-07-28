<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ActivityUserReport.aspx.cs" Inherits="net91com.Stat.Web.Reports.ActivityUserReport" EnableViewState="false" EnableViewStateMac="false" %>

<%@ Import Namespace="net91com.Stat.Web.Reports.Services" %>
<%@ Import Namespace="net91com.Stat.Web.Base" %>
<%@ Import Namespace="net91com.Stat.Services.Entity" %>
<%@ Register Src="/Reports/Controls/HeadAllControlChannel.ascx" TagName="HeadControl" TagPrefix="uc1"  %>
<%@ Register Src="Controls/PeriodSelector.ascx" TagName="PeriodSelector" TagPrefix="uc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
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
        ///自定义控件检查函数实现
        function checkCondition() {
           
            ischecked = true;
            ///让其第一次计算的线不显示
            $("#checkForNotUpdated").attr("checked",false);
        }
        function sendPost()
        {
            $("form:first").submit();
        } 
 
        ////table 切换
        function getTabs(b,m)
        {
            ///样式切换
            $(".tabs").each(function (index) {
                if ($(this).hasClass("userselecttype2"))
                {
                    $(this).removeClass("userselecttype2");
                    $(this).addClass("userselecttype");
                    ///数据隐藏
                    $("#tab"+index).hide();

                }
                
            });
            $(b).removeClass("userselecttype");
            $(b).addClass("userselecttype2");
            $("#tab"+m).show();
            geturl();
        }       
      
         ////初始化时候初始一下后台传来的数据
         $(function(){
           <%if (ListAll.Count != 0)
             { %>
            geturl();
            <%} %>
         });
         function geturl()
         {
               var selecttabs="";
               var tableName=$("#mytable table:visible").attr("name");
               var paras=tableName.split("_");
               
               var soft=paras[0];
               var plat=paras[1];
               var begintime=paras[3];
               var endtime=paras[4];
               var zhouqi=paras[2];
            
               var channelcate=paras[5];
               var channeltype=paras[6];
               var isold=paras[7];
               var url="AllHandler.ashx?action=downloadexcelactivity&"+"excelplatform="+plat+"&excelsoft="+escape(soft)+"&inputtimestart="+begintime +"&inputtimeend="+endtime+"&inputzhouqi="+zhouqi+"&channelcate="+channelcate+"&channeltype="+channeltype+"&isold="+isold+"&rds="+ encodeURIComponent(new Date());
               $(".download2").attr("href",url);
         }

        
    </script>
    

</head>
<body>
    <form id="form1" runat="server">
     <div class="main">
      <uc1:HeadControl ID="HeadControl1"   runat="server" />
    <div class="tablehead helpclass_head">
            <span  class="helpclass_span1">&nbsp;&nbsp;活跃用户
            </span>
            <span class="helpclass_span2" ><a id="A1" href="helphtm/activity.htm?ver=1"
                class="jTip">
             <img  alt="帮助" src="../Images/help.gif" /></a></span>
        </div>
   
    <div>
        <table style="width:100%">
            <tr>
                 
                <td><uc1:PeriodSelector   ID="PeriodSelector1" runat="server" /></td>
                <td style="text-align:right; padding-right:10px"><asp:CheckBox  ID="checkForNotUpdated" Text="显示未修正数据" runat="server" onclick="sendPost()" /></td>
            </tr>
        </table>
        <div style="border: 1px solid #CCCCCC; margin: 3px 0;">
            <div id="containner" style="width: 95%; text-align:center; height:500px; margin: 0 auto;">
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
        <%=TableStr %>
    </div>
    <%} %>
     </div>
     <script type="text/javascript">
      $(function () {
            
           $(".printexcel").click(function(){
                ///打印
               printout();
            });
            $("#containner").html(" <div  > <img height='25px' src='../images/defaultloading.gif'> <br> 正在加载统计数据... </div>");
            setTimeout("getchart()",50);
           
            ///onready 结束
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
                                //color: '#1E6FAF',
                                color: '#202020',
                                 fontWeight: 'bold'
 
                              
                            }
                },
//                
                xAxis:  <%= AxisJsonStr %> ,
                yAxis: {
                    min: 0,
                    allowDecimals: false,
                    title: {
                         text: '用户量'
                    },
                   
	                ///将y轴分割成一些小点
                    minorGridLineWidth: 0,
                    minorTickInterval: 'auto',
                    minorTickColor: '#000000',
                    minorTickWidth: 1 
                   


                },
                tooltip: {
                    formatter: function () {
                        var datestr=this.x;
                        if(this.point.Datestr!="")
                        {
                            datestr=this.point.Datestr;
                        }
                        var remarks="";
                        if(this.point.marker!=null)
                        {
                            remarks=this.point.marker.marks;
                        }
                       <%if(Period==net91com.Stat.Core.PeriodOptions.TimeOfDay||Period==net91com.Stat.Core.PeriodOptions.Hours){ %>
                        return this.series.name + "<br/>" +"时间："+datestr +"<br/>活跃：" +Highcharts.numberFormat(this.point.y, 0);
                        <%}else{ %>
                        if(this.point.Denominator==0)
                             return this.series.name + "<br/>" +"日期："+datestr+"<br/>流失：" +Highcharts.numberFormat(this.point.y, 0);
                        else
                            return this.series.name + "<br/>" +"日期："+datestr+"<br/>活跃：" +Highcharts.numberFormat(this.point.y, 0)+"<br/>总量："+this.point.Denominator +"<br/>活跃度："+this.point.growth+(remarks==""?"":("<br/>说明："+remarks));
                            
                        <%} %>
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
    </form>
</body>
</html>
