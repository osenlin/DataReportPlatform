<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SoftVersionSjqd.aspx.cs"
    Inherits="net91com.Stat.Web.Reports.SoftVersionSjqd" EnableViewState="false"
    EnableViewStateMac="false" %>

<%@ Register Src="/Reports/Controls/HeadAllControlChannel.ascx" TagName="HeadControl"
    TagPrefix="uc1" %>
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
             ///自定义控件检查函数实现
            function checkCondition() {
           
                ischecked = true;
            
            }
             $(function(){
                        
                        <%if (ListAll.Count != 0)
                        { %>
                        geturl();
                        <%} %>
                        ///查询提交表单
                        $("#seachout").click(function () {
                            checkcondition();
                            if (ischecked == true)
                            {
                                 $("#form1").submit();
                                   chart.showLoading();
                            }
                        });
                        ///打印
                           $(".printexcel").click(function(){
                                ///打印
                               printout();
                           });
            });

            function geturl()
            {
                var tableName=$("table:visible").attr("name");
                var paras=tableName.split("_");
                var soft=paras[0];
                var plat=paras[1];
                var zhouqi=paras[2];
                var begintime=paras[3];
                var endtime=paras[4];
                var channelid=paras[5];
                var type=paras[6];
                var isouter=paras[7];
                var url="AllHandler.ashx?action=downsjqd&"+"excelplatform="+plat+"&excelsoft="+escape(soft)+"&inputtimestart="+begintime +"&inputtimeend="+endtime+"&inputzhouqi="+zhouqi+"&channelid="+channelid+"&channeltype="+type+"&isouter="+isouter+"&rds="+ encodeURIComponent(new Date());  
                $(".download2").attr("href",url);
            }
 
            
       
        ///绑定后台数据，初始化一些该有的样式
        $(function(){
                     
                  //tab 样式去除
                 $(".tab-title").each(function () {
                if ($(this).hasClass("tab-title-current"))
                    $(this).removeClass("tab-title-current");

                 });
                ///新增
                if($("#reporttype").val()=="0")
                {
                    $(".tab-title").eq(0).addClass("tab-title-current");
                }
                else
                {
                    $(".tab-title").eq(1).addClass("tab-title-current");
                }
               $("#containner").html(" <div  > <img height='25px' src='../images/defaultloading.gif'> <br> 正在加载统计数据... </div>");
               setTimeout("getchart()",50);

               
                
        });
         ///tabs 切换
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
        
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div class="main">
        <asp:HiddenField ID="inputzhouqi" runat="server" />
        <asp:HiddenField ID="reporttype" runat="server" Value="0" />
        <asp:HiddenField ID="usercate" runat="server" Value="0" />
        <uc1:HeadControl ID="HeadControl1" runat="server" />
        <div>
            <div class="tablehead" style="margin-top: 3px">
                <div>
                    <span class="helpclass_span1">&nbsp;&nbsp;分渠道统计 </span><span class="helpclass_span2">
                        <a id="A1" href="helphtm/newuserexplain.htm?ver=1" class="jTip">
                            <img alt="帮助" src="../Images/help.gif" /></a></span>
                </div>
            </div>
            <uc1:PeriodSelector ID="PeriodSelector1" runat="server" />
        </div>
        <div style="margin-top: 3px">
            <!--标题-->
            <div class="panel-title" style="padding-bottom: 4px;">
                <ul class="column-tab">
                    <li class="tab-title  tab-title-current  " onclick="getsjqd(this)">新增</li>
                    <li class="tab-title  " onclick="getsjqd(this)">活跃</li>
                </ul>
            </div>
            <div class="panel-content">
                <div id="containner" style="width: 95%; margin: 0 auto; height: 500px; text-align: center;">
                </div>
                <%if (ListAll.Count != 0)
                  { %>
                <div class="tablehead2">
                    <div class="userselected" style="float: left; margin-top: 1px;">
                        <%for (int m = 0; m < tabStr.Count; m++)
                          { %>
                        <a class="<%if(m==0) {%>userselecttype2 <%}else{ %>userselecttype <%} %>  tabs" onclick="getTabs( this,<%=m%>)"
                            style="">
                            <%=tabStr[m]%></a>
                        <% if (m < tabStr.Count - 1)
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
                    <%=tableStr %>
                </div>
                <%} %>
            </div>
        </div>
    </div>
    </form>
    <script type="text/javascript">
 
          
          ///点击切换
        function getsjqd(a) {
            $(".tab-title").each(function () {
                if ($(this).hasClass("tab-title-current"))
                    $(this).removeClass("tab-title-current");

            });
            $(a).addClass("tab-title-current");
            
            if ($.trim($(a).text()) == "新增") {
                $("#reporttype").val(0);
                 ///提交表单
                checkCondition();
                checkedforcontrol();
                $("#form1").submit();
                  chart.showLoading();
            }
            else
            {
                
                $("#reporttype").val(1);
                 ///提交表单
                checkCondition();
                checkedforcontrol();
                $("#form1").submit();
                  chart.showLoading();
            }

        }
        function getchart()
        {
              $("#containner").html("");
              chart = new Highcharts.Chart({
                chart: {
                    renderTo: 'containner',
                    defaultSeriesType: 'line',
                    height:500
                },
                title: {
                    text: '<%=reportTitle %>',
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
                      <%if (Period == net91com.Stat.Core.PeriodOptions.Hours || Period == net91com.Stat.Core.PeriodOptions.TimeOfDay){ %>

                            <%if(reporttype.Value == "0"){ %>
                            return this.series.name + "<br/>"+"时间：" +datestr  +"<br/>新增：" +Highcharts.numberFormat(this.point.y, 0);
                            <%}else { %>
                            return this.series.name + "<br/>" +"时间："+ datestr +"<br/>活跃：" +Highcharts.numberFormat(this.point.y, 0);
                            <%} %>

                      <%}else{ %>

                            <%if(reporttype.Value == "0"){ %>
                            return this.series.name + "<br/>" +"时间："+datestr +"<br/>新增：" +Highcharts.numberFormat(this.point.y, 0)+(this.point.growth==""?"":("<br/>说明："+remarks));
                            <%}else { %>
                            return this.series.name + "<br/>" +"日期："+datestr +"<br/>活跃：" +Highcharts.numberFormat(this.point.y, 0)+"<br/>总量："+this.point.Denominator +"<br/>活跃度："+this.point.growth+(remarks==""?"":("<br/>说明："+remarks));
                            <%} %>


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
</body>
</html>
