<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="D_ResDownLoadByResIDDetail.aspx.cs" Inherits="net91com.Stat.Web.Reports_New.D_DownlaodStatistics.D_ResDownLoadByResIDDetail" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
    <head id="HeadD_ResDownLoadByResIDDetail" runat="server">
        <title>下载位置分布</title>
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
            //var hiddenDialog;
            $(function() {
                initSelect();
                getDetailTableTitle();
            });
            
    
            //初始化页面选择
            function initSelect() {
                var array = $("#myhidden").val().split("_"); 
                $("#hiddenbeginTime").val(array[0]);
                $("#beginTime").val(array[0]);
                $("#endTime").val(array[0]);
                
                $("#hiddensoftid").val(array[1]);
                $("#hiddenplatform").val(array[2]);
                $("#hiddenrestype").val(array[3]);
                $("#hiddenversion").val(array[4]);
                $("#hiddenprojectsource").val(array[5]);
                $("#hiddenareatype").val(array[6]);    
                $("#hiddenresselecttype").val(array[7]);
                $("#hiddenrescontext").val(array[8]);
                $("#hiddenareaid").val(array[9]);
                
            }

            var mytabs;
            function loadData() {
                    getDetailTableTitle();
            }
            function getperiod(period) {
                $("#hiddenperiod").val(period);
                loadData();
            }
            var oTable;
            function getDetailTableTitle() {
                var defaultSetting = {
                    "sDom": '<"top">rt<"bottom"fip><"clear">',
                    "bPaginate": false,
                    "bDestroy": true,
                    "aoColumns": [
                        { "sClass": "tdright", "sWidth": "25%" },
                        { "sClass": "tdleft" },
                        { "sClass": "tdright" }
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
                        aoData.push({ "name": "service", "value": "D_HttpDownStatDownCountSumByResIDDetailImpl" });
                        aoData.push({ "name": "do", "value": "get_detailtable" });
                        aoData.push({ "name": "begintime", "value": $("#beginTime").val() });
                        aoData.push({ "name": "endtime", "value": $("#endTime").val() });
                        aoData.push({ "name": "softs", "value": $("#hiddensoftid").val() });
                        aoData.push({ "name": "plat", "value": $("#hiddenplatform").val() });
                        aoData.push({ "name": "restype", "value": $("#hiddenrestype").val() });
                        aoData.push({ "name": "version", "value": $("#hiddenversion").val() });
                        aoData.push({ "name": "projectsource", "value": $("#hiddenprojectsource").val() });
                        aoData.push({ "name": "areatype", "value": $("#hiddenareatype").val() });
                        aoData.push({ "name": "resselecttype", "value": $("#hiddenresselecttype").val() });
                        aoData.push({ "name": "rescontext", "value": $("#hiddenrescontext").val() });
                        aoData.push({ "name": "areaid", "value": $("#hiddenareaid").val() });
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
                oTable = $('#mytable').dataTable(defaultSetting);
            }
            
            ///设置下载
            function setDownExcel() {
                var url = serverUrl + "?do=get_excel&service=D_HttpDownStatDownCountSumByResIDDetailImpl&projectsource=" + $("#hiddenprojectsource").val() + "&softs=" + $("#hiddensoftid").val()
                    + "&plat=" + $("#hiddenplatform").val()
                    + "&begintime=" + $("#beginTime").val() + "&endtime=" + $("#endTime").val()
                    + "&restype=" + $("#hiddenrestype").val() + "&version=" + $("#hiddenversion").val()
                    + "&areatype=" + $("#hiddenareatype").val() + "&resselecttype=" + $("#hiddenresselecttype").val()
                    + "&rescontext=" + $("#hiddenrescontext").val() + "&areaid=" + $("#hiddenareaid").val();
                $("#downexcel").attr("href", url);
            }
        </script>

    </head>
    <body>
        
        <form id="form1" runat="server">
             <asp:HiddenField runat="server" ID="myhidden" />
            <div class="maindiv">
                <!--只用于前台的标识-->
                <input type="hidden" value="" id="hiddenbeginTime" />
                <input type="hidden" value="" id="hiddenrescontext" />
                <input type="hidden" value="" id="hiddenprojectsource" />
                <input type="hidden" value="" id="hiddenresselecttype" />
                <input type="hidden" value="" id="hiddenrestype" />
                <input type="hidden" value="" id="hiddensoftid" />
                <input type="hidden" value="" id="hiddenplatform" />
                <input type="hidden" value="" id="hiddenareatype" />
                <input type="hidden" value="" id="hiddenversion" />
                 <input type="hidden" value="" id="hiddenareaid" />

                
                
                <input type="hidden" value="" id="hiddenvalue" />
                <input type="hidden" value="" id="hiddenname" />

                 <div style="padding: 10px;">
                        <table cellpadding="0" cellspacing="10" border="0" width="450px">
                            <tr>
                                <td>
                                    从：
                                    <input type="text" id="beginTime"  style="width: 135px" class="ui-multiselect ui-widget ui-state-default ui-corner-all Wdate"  onclick="WdatePicker()" /></td>
                                <td>
                                    到：
                                    <input type="text" id="endTime"  style="width: 135px" class="ui-multiselect ui-widget ui-state-default ui-corner-all Wdate"  onclick="WdatePicker()" />
                                </td>
                                <td>
                                    <span style="cursor: pointer; float: right; margin-right: 10px;"><a class="mybutton hover"
                                        style="margin-top: 4px; overflow: hidden;" onclick="loadData();"><font>查询</font>
                                    </a></span>
                                </td>
                            </tr>
                        </table>
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
                    <table id="mytable" cellpadding="0" cellspacing="0" border="0" width="100%"   class="display cell-border">
                        <thead>
                            <tr>
                                <th>
                                    位置ID
                                </th>
                                <th >
                                    位置名称
                                </th>
                                <th >
                                    下载量
                                </th>
                            </tr>               
                        </thead>
                        <tbody>
                        </tbody>
                    </table>

                </div>

            </div>
        </form>
        
    </body>
</html>

