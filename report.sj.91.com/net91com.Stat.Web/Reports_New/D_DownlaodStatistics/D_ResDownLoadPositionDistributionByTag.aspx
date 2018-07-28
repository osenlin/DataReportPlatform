<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="D_ResDownLoadPositionDistributionByTag.aspx.cs" Inherits="net91com.Stat.Web.Reports_New.D_DownlaodStatistics.D_ResDownLoadPositionDistributionByTag" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
    <head id="HeadPositionDistributionbyTag" runat="server">
        <!--这个页面占时没用-->
        <title>下载位置分布按专辑</title>
        <link href="/Reports_New/css/site.css?version=1" rel="stylesheet" type="text/css" />
        <link href="/Reports_New/css/jquery-ui.css" rel="stylesheet" type="text/css" />
        <link href="../css/jquery.datatables.table.css?id=1" rel="stylesheet" type="text/css" />
        <link href="/Reports_New/css/jquery.multiselect.css" rel="stylesheet" type="text/css" />
        <link href="/Reports_New/css/colorbox.css" rel="stylesheet" type="text/css" />
        <link href="/css/headcss/jquery.multiselect.filter.css" rel="stylesheet" type="text/css" />
        <link href="/Reports_New/css/defindecontrol.css" rel="stylesheet" type="text/css" />
        <script src="/Reports_New/js/jquery-1.6.4.min.js" type="text/javascript"></script>
        <script src="/Scripts/HeadScript/jquery-ui.min.js" type="text/javascript"></script>
        <script src="/Scripts/HeadScript/jquery.multiselect.js?id=0.2" type="text/javascript" charset="GBK"></script>
        <script src="/Scripts/HeadScript/jquery.multiselect.filter.js" type="text/javascript" charset="GBK"></script>
        <script src="/Reports_New/js/jquery.colorbox-min.js" type="text/javascript"></script>
    
        <script src="../js/Defindecontrol.js" type="text/javascript"></script>
        <script src="../js/jquery.dataTables.min.js" type="text/javascript"></script>
        <script src="../js/My97DatePicker/WdatePicker.js" type="text/javascript"></script>
        <script src="../js/selectcontrols.js?id=22" type="text/javascript"></script>
        <script src="../js/highcharts_old.js" type="text/javascript"></script>
        <script src="../js/chartjs.js?ver=1" type="text/javascript"></script> 

        <script type="text/javascript">
            var serverUrl = '../HttpService.ashx';
            var hiddenDialogByTag;
            
            $(function() {
                initSelect();
                getChart();
                hiddenDialogByTag = $("#mywindow2").colorbox({ iframe: true, width: "100%", height: "100%", speed: 0 });
            });
            
            //初始化页面选择
            function initSelect() {
                var array = $("#myhidden").val().split("_"); 
                $("#beginTime").val(array[0]);
                $("#endTime").val(array[1]);
                $("#hiddenprojectsource").val(array[9]);    
                $("#hiddensoftid").val(array[4]);
                $("#hiddenversion").val(array[2]);
                $("#hiddenrestype").val(array[3]);
                $("#hiddenperiod").val(array[7]);
                $("#hiddenplatform").val(array[5]);
                $("#hiddenIsUpdate").val(array[8]);
                $("#hiddenpositionid").val(array[10]);
                $("#hiddenpagename").val(array[11]);
            }
            
            var mytabs;
            var oTable;
            function getChart() {
                $.ajax({
                    type: "get",
                    url: serverUrl,
                    dataType: "json",
                    data: {
                        'service': 'D_HttpDownStatDownPositionDistributionByTagImpl',
                        'do': 'get_chart',
                        'endtime': $("#endTime").val(),
                        'begintime': $("#beginTime").val(),
                        'softs': $("#hiddensoftid").val(),
                        'plat': $("#hiddenplatform").val(),
                        'restype': $("#hiddenrestype").val(),
                        'version': $("#hiddenversion").val(),
                        'projectsource': $("#hiddenprojectsource").val(),
                        'singlehiddenvalue': $("#hiddenvalue").val(),
                        'period': $("#hiddenperiod").val(),
                        'downtype': $("#hiddenIsUpdate").val(),
                        'positionid': $("#hiddenpositionid").val(),
                        'pagename': $("#hiddenpagename").val()
                    },
                    success: function(data) {
                        if (data.resultCode == 0) {
                            $("#container").html("");
                            var obj = eval("(" + data.data + ")");
                            createPie("container", "下载量位置分布按专辑", obj);
                            getDetailTableTitle();
                        } else {
                            alert(data.message);
                        }
                    }
                });
            }
            

            
            function gettabs(value, name) {
                $("#hiddenvalue").val(value);
                $("#hiddenname").val(name);
                if (!checkDataRight()) {
                    return;
                }
                var  s = getSetting();
                oTable = $('#mytable').dataTable(s); 
            }

            function getDetailTableTitle() {
                var detailTable;
                $("#mytags2").html("");
                detailTable = createTabs($("#mytags2"), 0);
                gettabs();
            }
            
            ///设置下载
            function setDownExcel() {
                if (!checkDataRight()) { 
                    return false;
                }
                var url = serverUrl + "?do=get_excel&service=D_HttpDownStatDownPositionDistributionByTagImpl&projectsource=" + $("#hiddenprojectsource").val() + "&softs=" + $("#hiddensoftid").val()
                    + "&plat=" + $("#hiddenplatform").val() + "&begintime=" + $("#beginTime").val() + "&endtime=" + $("#endTime").val()
                    + "&restype=" + $("#hiddenrestype").val() + "&version=" + $("#hiddenversion").val() +"&downtype="+$("#hiddenIsUpdate").val()
                    + "&period=" + $("#hiddenperiod").val() + "&positionid=" + $("#hiddenpositionid").val() + "&pagename=" + $("#hiddenpagename").val();
                $("#downexcel").attr("href", url);
            }
            
            function loadData() {
                if (checkDataRight()) { //&&isSingleMulti()
                    getChart();
                }
            }

            function getperiod(period) {
                $("#hiddenperiod").val(period);
                loadData();
            }
            
            function openwindowbytagdetail(positionid) {
                var url = "projectsource=" + $("#hiddenprojectsource").val() + "&softs=" + $("#hiddensoftid").val() + "&positionid=" + positionid
                    + "&plat=" + $("#hiddenplatform").val() + "&begintime=" + $("#beginTime").val() + "&endtime=" + $("#endTime").val()
                    + "&restype=" + $("#hiddenrestype").val() + "&version=" + $("#hiddenversion").val() + "&downtype=" + $("#hiddenIsUpdate").val()
                   + "&period=" + $("#hiddenperiod").val();
                $("#mywindow2").attr("href", "D_ResDownLoadPositionDistributionByTagDetail.aspx?" + url);
                $("#mywindow2").click();
            }
            
            function getSetting() {
                var defaultSetting = {
                    "sDom": '<"top">rt<"bottom"fip><"clear">',
                    "bPaginate": false,
                    "bDestroy": true,
                    "aoColumns": [
                        { "sClass": "tdleft", "sWidth": "25%" },
                        { "sClass": "tdright", "sWidth": "25%" },
                        { "sClass": "tdright", "sWidth": "10%" },
                        { "sClass": "tdright", "sWidth": "10%" },
                        { "sClass": "tdright", "sWidth": "10%" },
                        { "sClass": "center", "sWidth": "20%" }
                    ],
                    "aoColumnDefs": [
                        {
                            "bUseRendered": false,
                            "fnRender": function (oObj) {
                                var colindex = oObj.iDataColumn;
                                if (colindex == 5) {
                                    return "<span style=\"text-decoration:underline;color:blue;display:block;\"  onclick=\"openwindowbytagdetail('" + oObj.aData[5] + "')\" >" + "日期明细" + "</a>";
                                } else
                                    return oObj.aData[colindex];
                            },
                            "aTargets": [5]
                        },
                        {
                            "bSortable": false,
                            "aTargets": [0, 1, 2, 3, 4, 5]
                        }
                    ],
                    "bProcessing": true,
                    "bStateSave": false,
                    "bServerSide": true,
                    "sServerMethod": "POST",
                    "iDisplayLength": 30,
                    "oLanguage": { sUrl: "../js/de_DE.txt" },
                    "bFilter": false,
                    "sPaginationType": "full_numbers",
                    "sAjaxSource": serverUrl + "?rand=" + Math.random(),
                    "fnServerParams": function (aoData) {
                        aoData.push({ "name": "service", "value": "D_HttpDownStatDownPositionDistributionByTagImpl" });
                        aoData.push({ "name": "do", "value": "get_detailtable" });
                        aoData.push({ "name": "endtime", "value": $("#endTime").val() });
                        aoData.push({ "name": "begintime", "value": $("#beginTime").val() });
                        aoData.push({ "name": "softs", "value": $("#hiddensoftid").val() });
                        aoData.push({ "name": "plat", "value": $("#hiddenplatform").val() });
                        aoData.push({ "name": "restype", "value": $("#hiddenrestype").val() });
                        aoData.push({ "name": "version", "value": $("#hiddenversion").val() });
                        aoData.push({ "name": "projectsource", "value": $("#hiddenprojectsource").val() });
                        aoData.push({ "name": "period", "value": $("#hiddenperiod").val() });
                        aoData.push({ "name": "downtype", "value": $("#hiddenIsUpdate").val() });
                        aoData.push({ "name": "positionid", "value": $("#hiddenpositionid").val() });
                        aoData.push({ "name": "pagename", "value": $("#hiddenpagename").val() });
                    },
                    "fnServerData": function (sSource, aoData, fnCallback) {
                        $.ajax({
                            "dataType": 'json',
                            "type": "POST",
                            "url": sSource,
                            "data": aoData,
                            "success": function (data) {
                                //存在输出的resultCode 让他输出message
                                if (data.resultCode && data.resultCode != 0) {
                                    alert(data.message);
                                } else {
                                    fnCallback(data);
                                }

                            }
                        });
                    },
                    "fnDrawCallback": function () {
                        tableStyleSet("mytable");
                        setDownExcel();
                    }
                    ,
                    "fnInitComplete": function () {
                        this.fnAdjustColumnSizing(true);
                    }
                };
                return defaultSetting;
            }
        </script>

    </head>
    <body>

        <form id="form1" runat="server">
             <asp:HiddenField runat="server" ID="myhidden" />
            <div class="maindiv">
                <!--只用于前台的标识-->
                <input type="hidden" value="<%=BeginTime.ToString("yyyy-MM-dd") %>" id="beginTime" />
                <input type="hidden" value="<%=EndTime.ToString("yyyy-MM-dd") %>" id="endTime" />
                <input type="hidden" value="" id="hiddenpositionid" />
                <input type="hidden" value="" id="hiddenprojectsource" />
                <input type="hidden" value="" id="hiddenmodetype" />
                <input type="hidden" value="" id="hiddenrestype" />
                <input type="hidden" value="" id="hiddensoftid" />
                <input type="hidden" value="" id="hiddenplatform" />
                <input type="hidden" value="" id="hiddenIsUpdate" />
                <input type="hidden" value="" id="hiddenversion" />
                 <input type="hidden" value="" id="hiddenpagename" />
                
                <input type="hidden" value="" id="hiddenvalue" />
                <input type="hidden" value="" id="hiddenname" />
                <input type="hidden" value="1" id="hiddenperiod" />

                <div class="title" style="margin-top: 4px;">
                    <strong class="l" id="Strong2">
                    </strong><span class="r"></span>
                </div>
                <div class="textbox">
                    <div id="container" style="margin: auto; margin: 2px; height: 400px;">
                    </div>
                </div>
                <div class="title" style="margin-top: 4px;">
                    <strong class="l" id="Strong1">数据明细 
                    </strong>
                    <span class="r">
                        <a class="mybutton hover"
                           id="downexcel" href="javascript:void(0);"><font>导出Excel</font> </a>
                    </span>
                    <ul id="mytags2">
                    </ul>
                </div>
                <div class="textbox">
                    <table id="mytable" cellpadding="0" cellspacing="0" border="0" style="width: 99%"  class="display cell-border">
                        <thead>
                            <tr>
                                <th>
                                    位置
                                </th>
                                <th >
                                    位置编号
                                </th>
                                <th >
                                    下载量
                                </th>
                                 <th >
                                    所占比例
                                </th>
                                <th >
                                    日均
                                </th>
                                <th >
                                    日期明细
                                </th>
                            </tr>               
                        </thead>
                        <tbody>
                        </tbody>
                    </table>
                </div>
            </div>
        </form>
        
        <a id="mywindow2" style="display: none"></a>
    </body>
</html>

