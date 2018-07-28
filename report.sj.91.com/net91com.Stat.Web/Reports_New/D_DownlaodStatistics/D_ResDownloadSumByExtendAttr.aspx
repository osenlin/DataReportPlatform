<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="D_ResDownloadSumByExtendAttr.aspx.cs" Inherits="net91com.Stat.Web.Reports_New.D_DownlaodStatistics.D_ResDownloadSumByExtendAttr" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
 

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
 <title>下载量统计(扩展属性)</title>
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
    var versionControl;
    var projectControl;
    var showTypeControl;
    var restypeControl;
    var softPlatJson = <%=SoftPlatformJson %>;
    var projectJson = <%=ProjectJson %>;
    var serverUrl = '../HttpService.ashx';
    var showType;
    var channelControl;
    var AreaControl;
    var g_areatype;
    var ExtendAttributeLstControl;
    var selectstatypeControl;
    $(function () {
        channelControl = createChannelBoxControl("mychanneltxt",$("#selectsofts").val(),$("#selectplats").val(),5);
        channelControl.needrecreate = 1;
        softControl = createSelectControl("selectsofts", '请选择', false, function () {
            if ($("#selectsofts").val()==null) {
                alert("请选择一个软件");
                return false;
            }
            reSetPlats($("#selectsofts").val());
            channelControl.softid = $("#selectsofts").val();
            
            reSetSelectMode($("#selectmodetype").val());
            clearSelect();
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
        
        ExtendAttributeLstControl = createSelectControl("selectExtendAttrLst", '请选择', false, function() {
            if ($("#selectExtendAttrLst").val() == null) {
                alert("请选择一个扩展资源类型");
                return false;
            }
            
            return true;
        }, function() {
            return true;
        }, true);
        
        projectControl=createSelectControl("selectprojectsources", '请选择', false, function () {
            if ($("#selectprojectsources").val()==null) {
                alert("请选择一个来源");
                return false;
            } 
            return true;
        }, function () {
            return true;
        }, true);
        selectstatypeControl=createSelectControl("selectstatype", '请选择', false, function () {
            if ($("#selectstatype").val()==null) {
                alert("请选择一个下载类型");
                return false;
            } 
            return true;
        }, function () {
            return true;
        }, true);
        

        restypeControl=createSelectControl("selectrestype", '请选择', false, function () {
            if ($("#selectrestype").val()==null) {
                alert("请选择一个资源类型");
                return false;
            } 
            getExtendAttr();
            return true;
        }, function () {
            return true;
        }, true);
        getExtendAttr();
        selectChangeMode($("#selectmodetype").val());
        getChart();
        reSetSelectMode($("#selectmodetype").val());
    });
    var mytabs;
    function loadData() {
        if (checkDataRight()) {
            getChart();
        }
    }
    var oTable;
    
    function getperiod(period) {
        $("#hiddenperiod").val(period);
        loadData();
    }
    
    function getChart() {
        $.ajax({
            type: "get",
            url: serverUrl,
            dataType: "json",
            data: {
                'service': 'D_HttpDownStatDownCountSumByExtendAttrLstImpl',
                'do': 'get_chart', 
                'endtime': $("#endTime").val(),
                'begintime':$("#beginTime").val(),
                'softs': $("#selectsofts").val(),
                'plat': $("#selectplats").val(),
                'period':$("#hiddenperiod").val(),
                'restype': $("#selectrestype").val(),
                'ExtendAttrLst': $("#selectExtendAttrLst").val(),
                'stattype':$('#selectstatype').val()
            },
            success: function (data) {
                if (data.resultCode == 0) {
                    $("#container").html("");
                    
                    var yname = "量";
                    var obj = eval("(" + data.data + ")");
                    createLine("container", 400, obj.title, yname, true, 0, obj.x, obj.y);
                    getTabsControl();
                }
                else {
                    alert(data.message);
                }
            }
        });
    }
    
    function getTabsControl() {
        gettabs("", "");
    }
    
    function gettabs(value,name) {
        $("#hiddenvalue").val(value);
        $("#hiddenname").val(name);

        if (oTable && checkDataRight()) {
            oTable.fnDraw();
            return;
        }
       
        oTable = $('#mytable').dataTable({
            "sDom": '<"top">rt<"bottom"fip><"clear">',
            "bPaginate": false,
            "aoColumns": [
                { "sClass": "center" },
                { "sClass": "tdright" }
            ],
            "aoColumnDefs": [
                {
                    "bSortable": false,
                    "aTargets": [0, 1]
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
                aoData.push({ "name": "service", "value": "D_HttpDownStatDownCountSumByExtendAttrLstImpl" });
                aoData.push({ "name": "do", "value": "get_detailtable" });
                aoData.push({ "name": "begintime", "value": $("#beginTime").val() });
                aoData.push({ "name": "endtime", "value": $("#endTime").val() });
                aoData.push({ "name": "softs", "value": $("#selectsofts").val() });
                aoData.push({ "name": "plat", "value": $("#selectplats").val() });
                aoData.push({ "name": "restype", "value":$("#selectrestype").val()});
                aoData.push({ "name": "period", "value": $("#hiddenperiod").val() });
                aoData.push({ "name": "ExtendAttrLst", "value": $("#selectExtendAttrLst").val()  });
                aoData.push({ "name": "stattype", "value": $("#selectstatype").val()  });                
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
        var url = serverUrl + "?do=get_excel&service=D_HttpDownStatDownCountSumByExtendAttrLstImpl" + "&softs=" + $("#selectsofts").val()
            + "&plat=" + $("#selectplats").val() + "&begintime=" + $("#beginTime").val() + "&endtime=" + $("#endTime").val()
            + "&restype=" + $("#selectrestype").val() + "&version=-1&stattype=" + $("#selectstatype").val()+"&ExtendAttrLst="+$("#selectExtendAttrLst").val() ;
        $("#downexcel").attr("href", url);
    }
</script>
</head>
<body>
    <form id="formattest" runat="server">
    <div class="maindiv">
        <input type="hidden" value="1" id="hiddenperiod" />
        <input type="hidden" value="<%=restypebyrequest %>" id="hiddenrestype"/>
        <input type="hidden" value="" id="hiddenname"/>
        <input type="hidden" value="0" id="hiddenmultitype"/>
        <div style="padding: 10px;">
            <table id="table1" cellpadding="0" cellspacing="10" border="0" width="850px">
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
                    <td style="width: 200px" >
                        <div style="position: relative; z-index: 100">
                           <select id="selectrestype" style="width: 150px;"  >
                                <%=RestypeHtml%>
                            </select>
                        </div>
                    </td>
                     <td   style="width: 200px" >
                           <div style="position: relative; z-index: 100">
                            <select id="selectExtendAttrLst" style="width: 150px;"  >
                                     <%=ExtendAttrHtml%>
                            </select>
                        </div>

                    </td>
                   <td></td>
                </tr>
                <tr>
                    <td>
                        从：
                        <input type="text" style="width: 135px" class="ui-multiselect ui-widget ui-state-default ui-corner-all Wdate"  id="beginTime" value="<%=BeginTime.ToString("yyyy-MM-dd") %>" onclick="WdatePicker()" />
                     </td>
                    <td >
                           到：
                        <input type="text"  style="width:135px" class="ui-multiselect ui-widget ui-state-default ui-corner-all Wdate"  id="endTime" value="<%=EndTime.ToString("yyyy-MM-dd") %>"  onclick="WdatePicker()" />
                    </td>     
                    <td   style="width: 200px" >
                           <div style="position: relative; z-index: 100">
                            <select id="selectstatype" style="width: 150px;"  >
                                    <option value="1" selected="selected">下载点击(所有)</option>
                                    <option value="2">展示</option>
                                    <option value="4">下载成功</option>    
                                    <option value="5">安装成功</option>
                                    <option value="6">安装失败</option>
                                    <option value="8">下载失败</option>
                            </select>
                        </div>

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
            <strong class="l" id="Strong2"> 扩展属性下载量统计
            </strong> <span class="r"></span>
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
            <table id="mytable" cellpadding="0" width="100%" cellspacing="0" border="0" class="display">
                <thead>
                    <tr>
                        <th>日期</th>
                        <th>
                             数据量
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
    function reSetSelectMode(type) {
        reSetProject($("#selectsofts").val());
    }
    function reSetPlats(softid, defaultValue) {
        var myplats = softPlatJson[softid];
        var selectHtml = "";
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
    
    function reSetProject(softid, defaultValue) {
        var myprojects = projectJson[softid];
        var selectHtml = "<option value='-1' selected='selected'>不区分来源</option>";
        if (myprojects != undefined) {
            for (var i = 0; i < myprojects.length; i++) {
                selectHtml += "<option value='" + myprojects[i].Key + "'";
                if (myprojects[i].Key == defaultValue) {
                    selectHtml += " selected='selected' ";
                }
                selectHtml += ">";
                selectHtml += myprojects[i].Value;
                selectHtml += "</option>";
            }

        }
        $("#selectprojectsources").html(selectHtml);
        if (projectControl) {
            projectControl.refresh();
        }
    }
    
    function getExtendAttr() {
        $.getJSON(serverUrl,
            {
                'service': 'UtilityService',
                'do': 'getextendattrbyrestype',
                'restype': $("#selectrestype").val()
            },
            function (data2) {
                if (data2.resultCode == 0) {
                    var mydata2 = data2.data;
                    var a = [], j = 0;
                    for (var i = 0; i < mydata2.length; i++) {
                        a[j++] = "<option value='";
                        a[j++] = mydata2[i].ID + "' ";
                        a[j++] = ">";
                        a[j++] = mydata2[i].Value;
                        a[j++] = "</option>";
                    }
                    var html = a.join("");
                    $("#selectExtendAttrLst").html(html);
                    if (ExtendAttributeLstControl) {
                        ExtendAttributeLstControl.refresh();
                    }
                } else {
                    alert(data2.message);
                }
            });
    }
    
    ///模式切换
    function selectChangeMode(type) {
        clearSelect();
        $(".modetypeclass").hide();
        switch (type) {
        case "4":
            $("#divareatxt").show();
            break;
        case "2":
            $("#divversion").show();
            break;
        case "3":
            $("#divprojectsources").show();
            break;
        default:
            break;
        }
    }
    
    function clearSelect() {
        $("#selectprojectsources").val("-1");
        if (projectControl)
            projectControl.refresh();
    }
</script>