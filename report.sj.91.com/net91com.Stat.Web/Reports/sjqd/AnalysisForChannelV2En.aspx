<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AnalysisForChannelV2En.aspx.cs" Inherits="net91com.Stat.Web.Reports.sjqd.AnalysisForChannelV2En" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <link href="../../css/ReportCss.css" rel="stylesheet" type="text/css" />
    <link href="../../css/headcss/jquery-ui.css" rel="stylesheet" type="text/css" />
    <script src="../../Scripts/jquery-1.5.2.min.js" type="text/javascript"></script>
    <link href="../../css/headcss/jquery.multiselect.css?rd=<%= net91com.Stat.Web.Reports.Services.Utility.CssVersion %>" rel="stylesheet" type="text/css" />
    <link href="../../css/headcss/jquery.multiselect.filter.css" rel="stylesheet" type="text/css" />
    <script src="../../Scripts/HeadScript/jquery-ui.min.js" type="text/javascript"></script>
    <script src="../../Scripts/HeadScript/jquery.multiselect.js?rd=<%= net91com.Stat.Web.Reports.Services.Utility.JsVersion %>" type="text/javascript"  charset="GBK"></script>
    <script src="../../Scripts/HeadScript/jquery.multiselect.filter.js?rd=<%= net91com.Stat.Web.Reports.Services.Utility.JsVersion %>" type="text/javascript" charset="GBK"></script>
    <script src="../../Scripts/highcharts.js" type="text/javascript"></script>
    <script src="../../My97DatePicker/WdatePicker.js" type="text/javascript"></script>
    <script type="text/javascript">
        var ischecked = false;
        function checkcondition() {
            var starttime1 = $.trim($("#fromtime").val()).replace(/-/g, '/'); ;
            var endtime1 = $.trim($("#totime").val()).replace(/-/g, '/');
            if (starttime1.length == 0) {
                alert("请选择时间");
                return false;
            }

            if (endtime1.length == 0) {
                alert("请选择结束时间");
                return false;
            }

            if (Date.parse(starttime1) > Date.parse(endtime1)) {
                alert("日期范围有问题！");
                return false;
            }
            ischecked = true;

        }
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
        $(function () {
            countrys = $("#myselectcountry").multiselect({
            multiple:false,
            noneSelectedText: "选择平台",
            header: true,
            height:300,
            minWidth: 170,
            selectedList: 100,
            beforeclose: function () {
                 if ($("#myselectcountry").val() == null) {
                    alert("至少选择一个选项！");
                    return false;
                 }
                $("#hiddencountry").val($("#myselectcountry").val());

               } 
            }).multiselectfilter({ label: "搜索", width: 80 });
            $("#hiddencountry").val($("#myselectcountry").val());
            <%if( listAll.Count != 0){%>
                geturl();
            <%} %>
            $(".printexcel").click(function () {
                ///打印
                printout();
            });
        });
        
        

        function geturl() {
            var tableName = $(".tablesorter:visible").attr("name");
            var paras = tableName.split("_");
            var soft = paras[0];
            var plat = paras[1];
            var begintime = paras[2];
            var endtime = paras[3];
            var p = paras[4];

            var url = "../AllHandler.ashx?action=downloadforoutcustomen&" + "excelplatform=" + plat + "&excelsoft=" + escape(soft) + "&inputtimestart=" + begintime + "&inputtimeend=" + endtime + "&p=" + p +"&mycountry="+ $("#hiddencountry").val() + "&rds=" + encodeURIComponent(new Date());
            $(".download2").attr("href",url);
        }


       
        function printout() {
            var BrowserAgent = navigator.userAgent;
            ///打印完回退
            // var css="<style>body{font-size:14px;color:#ccc;margin:0 0 0 0;}";
            var css = '<link rel="stylesheet" type="text/css" href="../../css/printScreen.css" />';
            var headstr = "<html><head>" + css + "</head><body>";
            //var headstr = "<html><head></head><body>";
            var footstr = "\<script\>window.print()\</script\></body></html>";
            var newstr = document.getElementById("mytable").innerHTML;
            var str = headstr + newstr + footstr;
            var printWindow = window.open("", "_blank");
            printWindow.document.write(str);
            printWindow.document.close();
            if (BrowserAgent.indexOf("Chrome") == -1) {
                ///取消就关闭
                printWindow.close();
            }
            return false;
        }
       
       

        $(function () {
            $("#seachout").click(function () {
                checkcondition();
                if (ischecked == true) {
                    $("#form1").submit();
                    chart.showLoading();
                }
            });
        
        });
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <input type="hidden" id="hiddencountry" name="mycountry" />
    <div class="main">
     <div class="caidan" style="position:static">
     <table  style="font-size:11px; width:100%">
     <tr>
     <td style="width:12%; padding-left:10px">
      <span id="softName" runat="server">熊猫看书</span>
     </td>
    <td style="width:10%; padding-left:10px">
      <span id="paltName" runat="server">熊猫看书</span>
     </td>
    
     <td  style="width:40%; padding-bottom:1px;">
      <div  >
         <span>时间从：</span>  <input id="fromtime" type="text" class="Wdate" onclick="WdatePicker()" runat="server" />
         <span>到：</span> <input id="totime" type="text" class="Wdate" onclick="WdatePicker()" runat="server" />
        </div>
     </td>
      <td style="width:13%; padding-left:10px">
         <select id="myselectcountry">
             <%=countryHtml%>
        </select>
     </td>
      <td style="width:15%; padding-left:10px">
      <span id="channelCustomName" runat="server">熊猫看书</span>
     </td>
     <td style="width:10%; text-align:center;">
      <a  id="seachout" style="display:block; cursor:pointer"><span>查询</span></a>
     </td>
     </tr>
     </table>
        </div>
    <div style="border: 1px solid #CCCCCC; margin: 3px 0;">
        <div id="containner" style="width: 95%; margin: 0 auto;">

        </div>

    </div>
    <div>
           <%if (listAll.Count != 0)
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
                            <img alt="打印" class="downloadimg" src="../../Images/printout.gif" />
                        </a>
                        <a class="download2"><span>下载</span>
                            <img alt="下载" class="downloadimg" src="../../Images/downloadexcel.gif" />
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
    </form>
</body>
</html>
<script type="text/javascript">
  $(function () {
            ///报表制作
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
                       
                        return this.series.name + "<br/>" +"时间："+this.x +"<br/>新增：" +Highcharts.numberFormat(this.point.y, 0);
                        
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
         });
</script>
