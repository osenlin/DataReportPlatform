﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SoftAreaTransverse.aspx.cs"
    Inherits="net91com.Stat.Web.Reports.TransverseReports.SoftAreaTransverse" %>

<%@ Register Src="/Reports/Controls/HeadAllControlChannel.ascx" TagName="HeadControl"
    TagPrefix="uc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>软件语言分布</title>
    <link href="../../css/jquery.loadmask.css" rel="stylesheet" type="text/css" />
    <link href="../../css/ReportCss.css" rel="stylesheet" type="text/css" />
    <link href="../../css/help.css" rel="Stylesheet" type="text/css" />
    <link href="/Reports_New/css/colorbox.css" rel="stylesheet" type="text/css" />
    <script src="../../Scripts/HeadScript/jquery.js" type="text/javascript"></script>
    <script src="../../Scripts/highcharts.js" type="text/javascript"></script>
    <script src="../../Scripts/exporting.js" type="text/javascript"></script>
    <script src="../../Scripts/common.js" type="text/javascript"></script>
    <script src="../../Scripts/jTip.js" type="text/javascript"></script>
    <script src="../../Scripts/jquery.loadmask.min.js" type="text/javascript"></script>

    <style type="text/css">
        .find
        {
            float: left;
            margin-left: 20px;
            cursor: pointer;
        }
        .find2
        {
            float: left;
            margin-left: 20px;
            cursor: pointer;
        }
    </style>
    <script type="text/javascript">
        
        
         var chartjson =  <%=SeriesJsonStr %>;
         var reportTitle = '<%=reportTitle %>';
         var platform='<%=platformsid %>';
         var soft='<%=softsid %>';
         var zhouqi='<%=(int)Period %>';
         var channelType='<%=ChannelCate%>';
         var channelids='<%=ChannelValues%>';
         var type="<%=CustomType %>"; 
         var begintime='<%=startTime.ToString("yyyy-MM-dd") %>';
         var endtime='<%=maxTime.ToString("yyyy-MM-dd") %>';
         var act="getareadetail";
         var chart;
         function checkCondition() {
            ischecked = true;
  
         }
         function showLine(ix,areaname) {
           
            if ($("#tr" + ix).is(":hidden")) {

                $(".divtr").hide();
                $(".find").text("查看每天");

                $("#lbl" + ix).text("收缩");

                $("#tr" + ix).show();
                //若没有内容
                if($.trim($("#div" + ix).html())=="")
                {
                    getchart2(ix,areaname);
                }
            } else 
            {
                
                $("#tr" + ix).hide();
                $("#lbl" + ix).text("查看每天");
            }
          }

           function getchart2( ix,areaname) {
            var url = "ServerHandler.ashx?act=getareadetail&soft=" + soft + "&plat=" + platform + "&sdate=" + begintime + 
            "&edate=" + endtime+"&type="+type+"&channelids="+channelids+"&channelType="+channelType + "&areaname="+ escape(areaname)  + "&n=" + Math.random();
            $.get(url, function (data) {
                var mydata = eval("(" + data + ")"); 
                ///下拉的线图
                if(mydata.y.length==0)
                {
                    $(".divtr").hide();
                    $(".find").text("查看每天");
                    alert("暂无该时段数据");
                    return ;
                }
                chartline = new Highcharts.Chart({
                    chart: {
                        renderTo: 'div' + ix,
                        defaultSeriesType: 'line',
                        height: 400
                    },
                    title: {

                        style: {
                            color: '#202020',
                            fontWeight: 'bold',
                            display: 'none'
                        }
                    },
                    xAxis: mydata.x,
                    yAxis: {
                        min: 0,
                        allowDecimals: false,
                        title: {
                            text: '用户数'
                        },
                        minorGridLineWidth: 0,
                        minorTickInterval: 'auto',
                        minorTickColor: '#000000',
                        minorTickWidth: 1
                    },

                    tooltip: {
                        formatter: function () {
                             return this.series.name + "<br/>" + "日期：" + this.x + "<br/>用户数：" + Highcharts.numberFormat(this.point.y, 0);
                        }

                    },
                    plotOptions: {
                        line: {
                            dataLabels: {
                                enabled: true
                            }
                        }
                    },
                    series: mydata.y,
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

     } 
            

         
        
        $(function () {
      		    var url = "ServerHandler.ashx?act=area" +"&platform=" +platform + "&soft=" + soft + "&zhouqi=" + zhouqi+ "&type=" +"<%=CustomType %>"+"&nds="+encodeURIComponent(new Date())+"&channelids="+channelids+"&channelType="+channelType;
                $(".download2").attr("href",url);
               
                $(".printexcel").click(function () {
                    ///打印
                    printout();
                });
                period_channel=true;
                $("#mywindow").colorbox({ iframe: true, width: "85%", height: "60%" });
			    $("#containner").html(" <div  > <img height='25px' src='../../images/defaultloading.gif'> <br> 正在加载统计数据... </div>");
                   setTimeout("getchart()",50);
                
                });
         function getchart()
         {
                   $("#containner").html("");
                   chart = new Highcharts.Chart({
                        chart: {
                            renderTo: 'containner',
                            defaultSeriesType: 'pie',
                            plotBackgroundColor: null,
                            plotBorderWidth: null,
                            plotShadow: false

                        },
                        title: {
                            text: reportTitle,
                            style: { fontWeight: 'bold' }
                        },
                        tooltip: {
                            formatter: function () {
                                return "<b>" + this.point.name + ': </b>' + this.point.y + '人';
                            }

                        },
                        plotOptions: {
                            pie: {
                                allowPointSelect: true,
                                cursor: 'pointer',
                                dataLabels: {
                                    enabled: true,
                                    color: '#000000',
                                    connectorColor: '#000000',
                                    formatter: function () {
                                        return '<b>' + this.point.name + ': </b>' + Highcharts.numberFormat(this.percentage, 2) + ' %';
                                    }
                                },
                                showInLegend: true
                            }

                        },
                        series: chartjson,

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
                                },
                        //设置下面标题
                        legend: {
                            enabled: true
                        }
                    });
         }

         function showDetail(area) {
               var positionUrlParas   = "begintime=" + begintime + "&endtime=" + endtime + "&platform=" + platform
                + "&softid=" + soft + "&channelcate=" + channelType + "&channelids=" + channelids
                + "&area=" + area +"&period="+zhouqi;
               $("#mywindow").attr("href","SoftAreaDetails.aspx?" + positionUrlParas);
               $("#mywindow").click(); 
         }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div class="main">
        <uc1:HeadControl ID="HeadControl1" runat="server" />
        <script src="/Reports_New/js/jquery.colorbox-min.js" type="text/javascript"></script>
        <div>
            <div style="border: 1px solid #CCCCCC; margin: 3px 0;">
                <div id="containner" style="width: 95%; margin: 0 auto; text-align: center; height: 500px;">
                </div>
            </div>
        </div>
        <%if (allSoftArea.Count != 0)
          { %>
        <div class="tablehead2">
            <div style="float: right">
                <a class="printexcel"><span>打印</span>
                    <img alt="打印" class="downloadimg" src="../../Images/printout.gif" />
                </a><a class="download2"><span>下载</span>
                    <img alt="下载" class="downloadimg" src="../../Images/downloadexcel.gif" />
                </a>
            </div>
        </div>
        <div style="clear: both">
        </div>
        <div id="mytable">
            <%=tableStr%>
        </div>
        <%} %>
        <a id="mywindow" style="display: none"></a>
    </div>
    </form>
</body>
</html>
