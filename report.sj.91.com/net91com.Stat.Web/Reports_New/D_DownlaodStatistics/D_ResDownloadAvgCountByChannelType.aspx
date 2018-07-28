<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="D_ResDownloadAvgCountByChannelType.aspx.cs" Inherits="net91com.Stat.Web.Reports_New.D_DownloadStatistics.D_ResDownloadAvgCountByChannelType" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
 

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
 <title>下载按渠道类型</title>
<link href="/Reports_New/css/site.css?version=1" rel="stylesheet" type="text/css" />
<link href="/Reports_New/css/jquery-ui.css" rel="stylesheet" type="text/css" />
    <link href="../css/jquery.datatables.table.css" rel="stylesheet" type="text/css" />
<link href="/Reports_New/css/jquery.multiselect.css" rel="stylesheet" type="text/css" />
<link href="/Scripts/zTree/css/zTreeStyle.css" rel="stylesheet" type="text/css" />
 <link href="/Reports_New/css/colorbox.css" rel="stylesheet" type="text/css" />
<link href="/css/headcss/jquery.multiselect.filter.css" rel="stylesheet" type="text/css" />
<link href="/Reports_New/css/defindecontrol.css" rel="stylesheet" type="text/css" />
 <script src="/Reports_New/js/jquery-1.6.4.min.js" type="text/javascript"></script>
<script src="/Scripts/HeadScript/jquery-ui.min.js" type="text/javascript"></script>
<script src="/Scripts/HeadScript/jquery.multiselect.js" type="text/javascript" charset="GBK"></script>
<script src="/Scripts/HeadScript/jquery.multiselect.filter.js" type="text/javascript" charset="GBK"></script>
<script src="/Reports_New/js/jquery.colorbox-min.js" type="text/javascript"></script>
<script src="../js/Defindecontrol.js" type="text/javascript"></script>
<script src="../js/jquery.dataTables.min.js" type="text/javascript"></script>
<script src="/Reports_New/js/selectcontrols.js" type="text/javascript"></script>
<script src="/Scripts/zTree/js/jquery.ztree.core-3.2.min.js" type="text/javascript"></script>
<script src="/Scripts/zTree/js/jquery.ztree.excheck-3.2.min.js" type="text/javascript"></script>
<script src="../js/My97DatePicker/WdatePicker.js" type="text/javascript"></script>
<script src="../js/channelControl.js?id=22" type="text/javascript"></script>
<script src="../js/highcharts_old.js" type="text/javascript"></script>
<script src="../js/chartjs.js" type="text/javascript"></script>
<script type="text/javascript">
    var softControl;
    var platControl;
    var showTypeControl;
    var restypeControl;
    var softPlatJson = <%=SoftPlatformJson %>;
    var serverUrl = '../HttpService.ashx';
    var showType;
    $(function () {
        softControl = createSelectControl("selectsofts", '请选择', false, function () {
            if ($("#selectsofts").val()==null) {
                alert("请选择一个软件");
                return false;
            }
            reSetPlats($("#selectsofts").val());
            
            return true;
        }, function () {
            return true;
        }, true);
        reSetPlats($("#selectsofts").val());
        platControl= createSelectControl("selectplats", '请选择', false, function () {
            if ($("#selectplats").val()==null) {
                alert("请选择一个平台");
                return false;
            } 
            return true;
        }, function () {
            return true;
        }, true);
        
        showTypeControl=createSelectControl("selectshowtype", '请选择', false, function () {
            if ($("#selectshowtype").val()==null) {
                alert("请选择一个展示类型");
                return false;
            } 
            return true;
        }, function () {
            return true;
        }, false,false,100);
        restypeControl=createSelectControl("selectrestype", '请选择', false, function () {
            if ($("#selectrestype").val()==null) {
                alert("请选择一个资源类型");
                return false;
            } 
            return true;
        }, function () {
            return true;
        }, true);
        
        getChart();
    });
    var mytabs;
    function loadData() {
        if (checkDataRight()) {
             getChart();
        }
        
    }
    var oTable;
    function getChart() {
            $.ajax({
                type: "get",
                url: serverUrl,
                dataType: "json",
                data: {
                    'service': 'D_HttpDownStatDownAvgCountByChannelTypeImpl',
                    'do': 'get_chart', 
                    'endtime': $("#endTime").val(),
                    'begintime':$("#beginTime").val(),
                    'softs': $("#selectsofts").val(),
                    'plat': $("#selectplats").val(),
                    'restype': $("#selectrestype").val(),
                    'showtype': $("#selectshowtype").val(),
                    'channeltype':$('#hiddenchanneltype').val()
                },
                success: function (data) {
                    if (data.resultCode == 0) {
                        
                        $("#container").html("");
                        var title = "";
                        var yname = "量";
                        var obj = eval("(" + data.data + ")");
                        createLine("container", 400, obj.title, yname, true, 0, obj.x, obj.y);
                 
                        gettabs();
                    }
                    else {
                        alert(data.message);
                    }
                }
            });
        }
    
    
    function gettabs() {
        
            if (oTable && checkDataRight()) {
                oTable.fnDraw();
                return;
            }
            oTable = $('#mytable').dataTable({
                "sDom": '<"top">rt<"bottom"fip><"clear">',
                "bPaginate": false,
                "aoColumns": [
                    { "sClass": "center" },
                    { "sClass": "tdright" },
                    { "sClass": "tdright" },
                    { "sClass": "tdright" },
                    { "sClass": "tdright" },
                    { "sClass": "tdright" },
                    { "sClass": "tdright" },
                    { "sClass": "tdright" },
                    { "sClass": "tdright" },
                    { "sClass": "tdright" },
                    { "sClass": "tdright" },
                    { "sClass": "tdright" }
                ],
                "aoColumnDefs": [
                    {
                        "bSortable": false,
                        "aTargets": [0, 1, 2, 3, 4, 5, 6, 7, 8, 9,10,11]
                    }
                ],
                "bProcessing": true,
                "bServerSide": true,
                "iDisplayLength": 30,
                "oLanguage": { sUrl: "../js/de_DE.txt" },
                "bFilter": false,
                "sPaginationType": "full_numbers",
                "sAjaxSource": serverUrl,
                "fnServerParams": function (aoData) {
                    aoData.push({ "name": "service", "value": "D_HttpDownStatDownAvgCountByChannelTypeImpl" });
                    aoData.push({ "name": "do", "value": "get_detailtable" });
                    aoData.push({ "name": "endtime", "value": $("#endTime").val() });
                    aoData.push({ "name": "begintime", "value": $("#beginTime").val() });
                    aoData.push({ "name": "softs", "value": $("#selectsofts").val() });
                    aoData.push({ "name": "plat", "value": $("#selectplats").val() });
                    aoData.push({ "name": "restype", "value": $("#selectrestype").val() });
                    aoData.push({ "name": "showtype", "value": $("#selectshowtype").val() });
                    aoData.push({ "name": "channeltype", "value": $('#hiddenchanneltype').val() });
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
            });
        }
    
     ///设置下载
        function setDownExcel() {
            var url = serverUrl + "?do=get_excel&service=D_HttpDownStatDownAvgCountByChannelTypeImpl&projectsource=" + $("#selectprojectsources").val() + "&softs=" + $("#selectsofts").val()
                + "&plat=" + $("#selectplats").val() + "&begintime=" + $("#beginTime").val() + "&endtime=" + $("#endTime").val()
                + "&restype=" + $("#selectrestype").val()  + "&showtype=" + $("#selectshowtype").val()+"&channeltype="+$('#hiddenchanneltype').val();
            $("#downexcel").attr("href", url);
        }
</script>
   
</head>
<body>
    <form id="form1" runat="server">
        <input name="hiddenchanneltype" id="hiddenchanneltype" type="hidden" value="<%=ChannelType %>"/>
    <div class="maindiv">
        <div style="padding: 10px;">
            <table id="table1" cellpadding="0" cellspacing="10" border="0" width="100%">
                <tr>
                    
                    <td  style="width: 200px">
                        <div style="position: relative; z-index: 100">
                            <select id="selectsofts" style="width: 150px;"  >
                                <%=SoftHtml%>
                            </select>
                        </div>
                    </td>
                    
                    <td    style="width: 200px" >
                        <div style="position: relative; z-index: 100">
                           <select id="selectplats" style="width: 150px;"  >
                               <%=PlatHtml%>
                            </select>
                        </div>
                    </td>
                    
                     <td   style="width: 200px" >
                        <div style="position: relative; z-index: 100">
                           <select id="selectrestype" style="width: 150px;"  >
                                <option selected="selected" value="-1">不区分资源类型</option>
                                <%=RestypeHtml%>
                            </select>
                        </div>
                    </td>
                </tr>
                <tr>
                    <td >
                        从：
                        <input type="text" id="beginTime" style="width: 135px" class="ui-multiselect ui-widget ui-state-default ui-corner-all Wdate"  value="<%=BeginTime.ToString("yyyy-MM-dd") %>" onclick="WdatePicker()" />
                        </td>
                    <td > 到：
                        <input type="text" id="endTime" style="width: 135px" class="ui-multiselect ui-widget ui-state-default ui-corner-all Wdate" value="<%=EndTime.ToString("yyyy-MM-dd") %>"  onclick="WdatePicker()" />
                    </td>
                    <td  colspan="2">
                         <select id="selectshowtype" style="width: 120px;"    >
                               <option  value="1" selected="selected">展示总下载数</option>
                               <option  value="2">展示下载用户数</option>
                               <option  value="3">展示人均下载数</option>
                        </select>

                    </td>
                     
                    <td>
                        <span style="cursor: pointer; float: right; margin-right: 10px;"><a class="mybutton hover"
                            style="margin-top: 4px; overflow: hidden;" onclick="loadData();"><font>查询</font>
                        </a></span>
                    </td>
                </tr>
            </table>
        </div>
         <div style="clear: both">
        </div>
        <div class="title" style="margin-top: 4px;">
            <strong class="l" id="Strong2">曲线图</strong> <span class="r"></span>
        </div>
        <div class="textbox">
            <div id="container" style="margin: auto; margin: 2px; height: 400px;">
            </div>
        </div>
        <div class="title" style="margin-top: 4px;">
            <strong class="l" id="Strong1">数据明细 </strong><span class="r">
            <a class="mybutton hover"
                id="downexcel" href="javascript:void(0);"><font>导出Excel</font> </a>
            </span>
            <ul id="mytags2">
            </ul>
        </div>
 
        <div class="textbox">
            <table id="mytable" cellpadding="0" cellspacing="0" border="0" class="display">
                <thead>  
                   <tr>
                        <th  style="width:10%" rowspan="2">
                            日期
                        </th>
                        <th   style="width:25%"  colspan="3">
                            所有下载
                        </th>
                         <th   style="width:25%"  colspan="3">
                            新用户
                        </th>
                        <th   style="width:25%"  colspan="3">
                           老用户
                        </th>
                        <th  style="width:10%" rowspan="2">
                            人均一次
                        </th>
                        <th  style="width:10%" rowspan="2">
                            人均二次
                        </th>
                    </tr>
                    <tr>
                        <th  style="width:7%"  >
                            下载数
                        </th>
                        <th  style="width:7%"  >
                            用户量
                        </th>
                        <th   style="width:7%" >
                            人均
                        </th>
                        <th   style="width:7%" >
                            下载量
                        </th>
                        <th   style="width:7%" >
                            用户量
                        </th>
                        <th   style="width:7%" >
                            人均
                        </th>
                        <th   style="width:7%" >
                            下载量
                        </th>
                        <th   style="width:7%" >
                            用户量
                        </th>
                        <th   style="width:7%" >
                           人均
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
<script type="text/javascript">

    function reSetPlats(softid, defaultValue) {
        var myplats = softPlatJson[softid];
        var selectHtml = "<option value='-1' selected='selected'>不区分平台</option>";
        for (var i = 0; i < myplats.length; i++) {
            selectHtml += "<option value='" + myplats[i] + "'";
            if (myplats[i] == defaultValue) {
                selectHtml += " selected='selected' ";
            }
            selectHtml += ">";
            selectHtml += allplat[myplats[i]];
            selectHtml += "</option>";
        }
        $("#selectplats").html(selectHtml);
        if (platControl) {
            platControl.refresh();
        }
    }


</script>
