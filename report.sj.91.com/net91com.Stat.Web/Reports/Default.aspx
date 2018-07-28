<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="net91com.Stat.Web.Reports.Default"
    EnableViewState="false" %>
<%@ Register Src="/Reports/Controls/HeadAllControl.ascx" TagName="HeadControl" TagPrefix="uc1"  %>
<%@ Import Namespace="net91com.Stat.Web.Reports.Services" %>
<%@ Import Namespace="net91com.Stat.Web.Base" %>
<%@ Import Namespace="net91com.Stat.Services.Entity" %>
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


        var softid = '<%=SoftID %>';
        var platid = '<%=PlatID %>';
        var chartTimeSpanTodayNew;
        var chartTimeSpanTodayAct;
        var chartTimeTodayNew;
        var chartTimeTodayAct;
        var chart;
        ///自定义控件检查函数实现
        function checkCondition() {
             
            ischecked = true;
       
            $("#tablegaikuang").mask("Waiting...");
           
        }
       
        //--------------其他脚本设置--------------------------------------
        ///获取选中的单选框的值
        function getRadioValue() {
            ///返回的是newuser 或者
            return $(".spanneworactiviyt:checked").val();
        }
        ////-----新增和活跃用户对比-------------------------------
        function getchartfor30(a) {
            $(".chartfor30").each(function () {
                if ($(this).hasClass("tab-title-current"))
                    $(this).removeClass("tab-title-current");

            });
            $(a).addClass("tab-title-current");
            getchartfor30data($(a).text());
        }

        function getchartfor30data(a) {
            a = $.trim(a);
            if(a == "活跃")
            {
                if ($.trim($("#activityUser").html()) == "") {
                    $("#activityUser").html(" <div  > <img height='25px' src='../images/defaultloading.gif'> <br> 正在加载统计数据... </div>");
                    $.ajax({
                        type: "get",
                        url: "AllHandler.ashx?action=GetActivityfor30&" + "soft=" + softid + "&platform=" + platid + "&type=" + "2",
                        success: function (data) {
                            $("#activityUser").html("");
                            var mydata = eval("(" + data + ")");
                            buildYesDate(chartTimeTodayAct, mydata, 'activityUser', '活跃用户', 2,false);
                            $("#newUser").hide();
                            $("#activityUser").show();

                        }
                    });
                }
                $("#newUser").hide();
                $("#activityUser").show();
            }
            if ( a== "新增") {
                if ($.trim($("#newUser").html()) == "") {
                    $("#newUser").html(" <div  > <img height='25px' src='../images/defaultloading.gif'> <br> 正在加载统计数据... </div>");
                    $.ajax({
                        type: "get",
                        url: "AllHandler.ashx?action=GetActivityfor30&" + "soft=" + softid + "&platform=" + platid + "&type=" + "1",
                        success: function (data) {
                      
                            $("#newUser").html("");
                            var mydata = eval("(" + data + ")");
                  
                            buildYesDate(chartTimeTodayNew, mydata, 'newUser', '新增用户', 1, false);
                  
                            $("#activityUser").hide();
                            $("#newUser").show();
                        }
                    });
                }
                $("#activityUser").hide();
                $("#newUser").show();
            }
        }
        
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <uc1:HeadControl ID="HeadControl1"   runat="server" />
    <div class="divfordefault" style="padding: 10px">

     <div class="tablehead helpclass_head">
            <span class="helpclass_span1">&nbsp;&nbsp;基础信息 </span><span class="helpclass_span2">
            <a id="A3" href="helphtm/default1Explain.htm" class="jTip">
                    <img alt="帮助" src="../Images/help.gif" /></a></span>
        </div>

        <div>
            <%=TabStrGaiKuang %>
        </div>
        <!--基础信息-->
        <div class="tablehead helpclass_head">
            <span class="helpclass_span1">&nbsp;&nbsp;统计概况 </span><span class="helpclass_span2">
            <a id="A1" href="helphtm/default2Explain.htm" class="jTip">
                    <img alt="帮助" src="../Images/help.gif" /></a></span>
        </div>


        <div id="tablegaikuang">
            <%=TabStrJcXx %>
        </div>
        <!--最近30日数据趋势-->
        <div class="tablehead helpclass_head">
            <span class="helpclass_span1">&nbsp;&nbsp;&nbsp;&nbsp;最近30日趋势(<%=DateTimeFor30Begin%>至<%=DateTimeFor30 %>) </span>
            <span class="helpclass_span2" style=" display:block;cursor:pointer" id="spangetmore2"> 
            更多>></span>
        </div>

        
        <div id="divfor30days" style="margin-top:2px">
            <!--标题-->
            <div class="panel-title" style="padding-bottom: 4px;">
                <ul class="column-tab">
                    <li class="tab-title  tab-title-current chartfor30" onclick="getchartfor30(this)">新增</li>
                    <li class="tab-title chartfor30" onclick="getchartfor30(this)">活跃</li>
                </ul>
            </div>
            <!--tab 切换-->
            <div class="panel-content">
                <div id="newUser" style="height:400px; width:98%; text-align:center;">
                    
                </div>
                <div id="activityUser" style="display: none; height: 400px; width:98%; text-align:center;">
               
                </div>
            </div>
        </div>
    </div>
    <script type="text/javascript">
        $(function () {

               setTimeout("getchart()", 50);
             
        });

         function getchart() {
              
              getchartfor30data('新增'); 
              setTimeout("gettabSpanData('newuser')",700);
            
        }
       
       
 
    </script>
    <script type="text/javascript">
        function buildYesDate(mychart, mydate, div, title, type, needcut) {
           
            mychart = new Highcharts.Chart({
                chart: {
                    renderTo: div,
                    defaultSeriesType: 'line',
                    plotBorderColor: '#C0C0C0',
                    borderWidth: 0.3,
                    spacingLeft: 20,
                    spacingRight: 20



                },
                title: {
                    text: '',
                    style: {
                        color: '#202020',
                        fontWeight: 'bold'

                    }
                },

                xAxis: mydate.x,
                yAxis: {
                    min: 0,
                    allowDecimals: false,
                    title: {
                        text: '用户量'
                    },

                    minorGridLineWidth: 0,
                    minorTickInterval: 'auto',
                    minorTickColor: '#000000',
                    minorTickWidth: 1



                },
                tooltip: {
                    formatter: function () {
                        if (type == 2)
                            return this.series.name + "<br/>" + "时段：" + this.x + "<br/>活跃：" + Highcharts.numberFormat(this.point.y, 0);
                        else
                            return this.series.name + "<br/>" + "时段：" + this.x + "<br/>新增：" + Highcharts.numberFormat(this.point.y, 0);
                    }

                },

                series: mydate.y,

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
            ///是否需要对数据进行处理

        } 
    </script>
    <script type="text/javascript">
        $("#spangetmore1").click(function () {
            var soft = '<%=SoftID %>';
            var platform =  '<%=PlatID %>';
            var begintime = '<%=DateTimeSpan %>';
            var endtime = '<%=DateTimeSpan %>';

            if ($("#containertoday").is(":hidden")) {
                ///1 为活跃 ，smalltype 1 为周期为span
                var father = window.parent;
                var id = '<%=Activityurl %>';
                var url = "Reports/GetMore.aspx?soft=" + soft + "&plat=" + platform + "&endtime=" + endtime + "&reporttype=" + 1 + "&smalltype=" + 1+"&begintime="+begintime;
                var title = '活跃用户';
                var menupanleid = "mp" + '<%=Activityparenturl %>';
                var menuitem = father.Ext.getCmp(id);
                var menupanel = father.Ext.getCmp(menupanleid);
                father.moveTab(id);
                father.addTab(father.TabPanel1, 'idClt' + id, url, title, menuitem, menupanel);

            }
            else {
                ///2 为新增 ，smalltype 1 为周期为span
                var father = window.parent;
                var id = '<%=Newuserurl %>';
                var url = "Reports/GetMore.aspx?soft=" + soft + "&plat=" + platform + "&endtime=" + endtime + "&reporttype=" + 2 + "&smalltype=" + 1 + "&begintime=" + begintime;
                var title = '新增用户';
                var menupanleid = "mp" + '<%=Newuserparenturl %>';
                var menuitem = father.Ext.getCmp(id);
                var menupanel = father.Ext.getCmp(menupanleid);
                father.moveTab(id);
                father.addTab(father.TabPanel1, 'idClt' + id, url, title, menuitem, menupanel);

            }

        });
        $("#spangetmore2").click(function () {


            var soft = '<%=SoftID %>';
            var platform = '<%=PlatID %>';
            var endtime = '<%=DateTimeFor30 %>';
            var begintime = '<%=DateTimeFor30Begin %>';
            if ($("#newUser").is(":hidden")) {
                ///1 为活跃 ，smalltype 2 为周期为天
                var father = window.parent;
                var id = '<%=Activityurl %>';
                var url = "Reports/GetMore.aspx?soft=" + soft + "&plat=" + platform + "&endtime=" + endtime + "&reporttype=" + 1 + "&smalltype=" + 2+"&begintime="+begintime;
                var title = '活跃用户';
                var menupanleid = "mp" + '<%=Activityparenturl %>';
                var menuitem = father.Ext.getCmp(id);
                var menupanel = father.Ext.getCmp(menupanleid);
                father.moveTab(id);
                father.addTab(father.TabPanel1, 'idClt' + id, url, title, menuitem, menupanel);
                ///添加一个tab ；

            }
            else {
                ///2 为新增 ，smalltype 2 为周期为天
                var father = window.parent;
                var id = '<%=Newuserurl %>';
                var url = "Reports/GetMore.aspx?soft=" + soft + "&plat=" + platform + "&endtime=" + endtime + "&reporttype=" + 2 + "&smalltype=" + 2 + "&begintime=" + begintime;
                var title = '新增用户';
                var menupanleid = "mp" + '<%=Newuserparenturl %>';
                var menuitem = father.Ext.getCmp(id);
                var menupanel = father.Ext.getCmp(menupanleid);
                father.moveTab(id);
                father.addTab(father.TabPanel1, 'idClt' + id, url, title, menuitem, menupanel);

            }
        });
    </script>
    </form>
</body>
</html>
