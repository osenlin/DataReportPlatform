<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LinkReport.aspx.cs" Inherits="net91com.Stat.Web.Reports.LinkReport" EnableViewState="false" EnableViewStateMac="false" %>
<%@ Import Namespace="net91com.Stat.Web.Reports.Services" %>
<%@ Import Namespace="net91com.Stat.Web.Base" %>
<%@ Import Namespace="net91com.Stat.Services.Entity" %>
<%@ Register Src="/Reports/Controls/HeadLinkTagControl.ascx" TagName="HeadControl" TagPrefix="uc1" %>

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
        function checkCondition() {
            ischecked = true;
        }

        $(function () {
            <%if (ListAll.Count != 0)
              { %>
            geturl();
            <%} %>

            // 查询提交表单
            $("#seachout").click(function () {
                checkcondition();
                if (ischecked == true) {
                    $("#form1").submit();
                    chart.showLoading();
                }
            });
            // 打印
            $(".printexcel").click(function () {
                // 打印
                printout();
            });
            $("#containner").html(" <div><img height='25px' src='../images/defaultloading.gif'> <br> 正在加载统计数据... </div>");
            setTimeout("getchart()", 50);
        });

        function geturl() {
            var tableName = $("table:visible").attr("name");
            var paras = tableName.split("_");
            var soft = paras[0];
            var plat = paras[1];
            var begintime = paras[2];
            var endtime = paras[3];
            var tagid = paras[4];
            var type = paras[5];
            var url = "../Reports/AllHandler.ashx?action=downlink&excelplatform=" + plat + "&excelsoft=" + escape(soft) + "&inputtimestart=" + begintime + "&inputtimeend=" + endtime + "&tagid=" + tagid + "&tagtype=" + type + "&rds=" + encodeURIComponent(new Date());
            $(".download2").attr("href", url);
        }

        function getTabs(b, m) {
            $(".tabs").each(function (index) {
                if ($(this).hasClass("userselecttype2")) {
                    $(this).removeClass("userselecttype2");
                    $(this).addClass("userselecttype");
                    // 数据隐藏
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
                    height: 500
                },
                title: {
                    text: '跳转统计',
                    style: {
                        //color: '#1E6FAF',
                        color: '#202020',
                        fontWeight: 'bold'
                    }
                },
                xAxis: <% = AxisJsonStr %>,
                yAxis: {
                    min: 0,
                    allowDecimals: false,
                    title: {
                        text: '次数'
                    },
                    minorGridLineWidth: 0,
                    minorTickInterval: 'auto',
                    minorTickColor: '#000000',
                    minorTickWidth: 1
                },
                tooltip: {
                    formatter: function () {
                        return this.series.name + "<br/>" + "时间：" + this.x + "<br/>次数：" + Highcharts.numberFormat(this.point.y, 0);
                    }
                },
                series: <% = SeriesJsonStr %>,
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
                credits: {
                    enabled: false
                }
            });
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <div class="main">
            <uc1:HeadControl ID="HeadControl1" runat="server" />
            <div style="margin-top: 3px; ">
                <div class="panel-content">
                    <div id="containner" style="width: 95%; margin: 0 auto; height: 500px; text-align: center;">
                    </div>
                    <%if (ListAll.Count != 0)
                      { %>
                    <div class="tablehead2">
                        <div class="userselected" style="float: left; margin-top: 1px;">
                            <%for (int m = 0; m < tabStr.Count; m++)
                              { %>
                            <a class="<%if (m == 0)
                                        {%>userselecttype2 <%}
                                        else
                                        { %>userselecttype <%} %>  tabs"
                                onclick="getTabs(this, <%=m%>)"
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
                            </a>
                            <a class="download2"><span>下载</span>
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
</body>
</html>
