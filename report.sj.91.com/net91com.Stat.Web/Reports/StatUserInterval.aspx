<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="StatUserInterval.aspx.cs" Inherits="net91com.Stat.Web.Reports.StatUserInterval" %>
<%@ Register Src="/Reports/Controls/HeadAllControl.ascx" TagName="HeadControl" TagPrefix="uc1"  %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
     <link href="../css/ReportCss.css" rel="stylesheet" type="text/css" />
    <link href="../css/help.css" rel="Stylesheet" type="text/css" />
    <script src="../Scripts/HeadScript/jquery.js" type="text/javascript"></script>
    <script src="../Scripts/highcharts.js" type="text/javascript"></script>
    <script src="../Scripts/exporting.js" type="text/javascript"></script>
    <script src="../Scripts/common.js" type="text/javascript"></script>
    <script src="../Scripts/jTip.js" type="text/javascript"></script>
    <title></title>
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
     <uc1:HeadControl ID="HeadControl1"   runat="server" />
      <div style="border: 1px solid #CCCCCC; margin: 8px 0;">
            <div id="containner" style="width: 95%; margin: 0 auto; text-align: center; height: 500px;">
            </div>
        </div>
    </div>
    </form>
        <script type="text/javascript">

      $(function () {
            $("#containner").html(" <div  > <img height='25px' src='../images/defaultloading.gif'> <br> 正在加载统计数据... </div>");
            setTimeout("getchart()",50);
       });
        function getchart() {
            $("#containner").html("");
               ///报表制作
            chart = new Highcharts.Chart({
                chart: {
                    renderTo: 'containner',
                    type: 'bar' 
                  
                   
                },
                title: {
                    text: '<%=reportTitle %>',
                     style: {
                                 color: '#202020',
                                 fontWeight: 'bold'
                              
                            }
                },
 
                xAxis: <%=AxisJsonStr %> ,
                yAxis: {
                    min: 0,
                   max:100,
                    allowDecimals: false,
                     title: {
                        text: '人数比例'
                    } 
                    
  


                },
                
                 plotOptions:{
                     bar: {
				            dataLabels: {
					            enabled: true,
                                 formatter: function() {
				                    return this.point.y +"%";
			                        }
				            }

		            	    }
                   
                
                },

                 tooltip: {
			        formatter: function() {
				        return   this.series.name +': '+ this.y +'%';
			        }
		        } ,

               
              

                series: <%=SeriesJsonStr %>  ,

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
