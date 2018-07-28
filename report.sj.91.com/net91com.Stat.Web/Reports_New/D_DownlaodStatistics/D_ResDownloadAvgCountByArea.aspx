<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="D_ResDownloadAvgCountByArea.aspx.cs" Inherits="net91com.Stat.Web.Reports_New.D_DownlaodStatistics.D_ResDownloadAvgCountByArea" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
 

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
 <title>下载平均值</title>
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
    function getContronlValueLength(controlid) {
        return $("#" + controlid).find("option:selected").length;
    }
    function versionMulti(event, ui) {
        if (ui.checked==false || getContronlValueLength("selectversion")<1) {
            return true;
        }  
                
        var threecontrol = $('#mychanneltxt').val().indexOf(',');
        if (threecontrol != -1) {
            alert('不允许多个维度同时多选，渠道已经多选');
            $("#hiddenmultitype").val(1);
            return false;
        }
        return true;
    }
</script>
<script type="text/javascript">
    var softControl;
    var platControl;
    var versionControl;
    var projectControl;
    var showTypeControl;
    var restypeControl;
    var softPlatJson = <%=SoftPlatformJson %>;
    var AreaJson = <%= AreaJson %>;
    var SoftAreaJson = <%= SoftAreaJson %>;
    var projectJson = <%=ProjectJson %>;
    var serverUrl = '../HttpService.ashx';
    var showType;
    var channelControl;
    var AreaControl;
    var g_areatype;
    
    $(function () {
        channelControl = createChannelBoxControl("mychanneltxt",$("#selectsofts").val(),$("#selectplats").val(),5);
        channelControl.needrecreate = 1;
        softControl = createSelectControl("selectsofts", '请选择', false, function () {
            if ($("#selectsofts").val()==null) {
                alert("请选择一个软件");
                return false;
            }
            reSetPlats($("#selectsofts").val());
            reSetArea($("#selectsofts").val());
            channelControl.softid = $("#selectsofts").val();
            reSetSelectMode($("#selectmodetype").val());
            clearSelect();
            return true;
        }, function () {
            return true;
        }, true);
        reSetPlats($("#selectsofts").val());
        reSetArea($("#selectsofts").val());
        
        platControl= createSelectControl("selectplats", '请选择', false, function () {
            if ($("#selectplats").val()==null) {
                alert("请选择一个平台");
                return false;
            } 
            reSetVersion($("#selectsofts").val());
            return true;
        }, function () {
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

        restypeControl=createSelectControl("selectrestype", '请选择', false, function () {
            if ($("#selectrestype").val()==null) {
                alert("请选择一个资源类型");
                return false;
            } 
            return true;
        }, function () {
            return true;
        }, true);
        
        AreaControl = createSelectControl("selectArea", '请选择', false, function() {
            if ($("#selectArea").val() == null) {
                alert("请选择一个区域");
                return false;
            }
            return true;
        }, function() {
            return true;
        }, true);
        
        selectChangeMode($("#selectmodetype").val());
        reSetVersion($("#selectsofts").val());
        reSetSelectMode($("#selectmodetype").val());
    });
    

    var mytabs;
    function loadData() {
        if ($('#mychanneltxt').val().indexOf(',') != -1) {
            $("#hiddenmultitype").val(1);
        } else {
                $("#hiddenmultitype").val(2);
        }
        
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
                'service': 'D_HttpDownStatDownAvgCountByAreaImpl',
                'do': 'get_chart', 
                'endtime': $("#endTime").val(),
                'begintime':$("#beginTime").val(),
                'softs': $("#selectsofts").val(),
                'plat': $("#selectplats").val(),
                'period':$("#hiddenperiod").val(),
                'restype': $("#selectrestype").val(),
                'showtype': 3,
                'channelids': channelControl.getSelectChannels(),
                'channelnames': $("#mychanneltxt").val(),
                'areaid':$("#selectArea").val(),
                'areatype':g_areatype,
                'stattype': 1,
                'modetype':4
            },
            success: function (data) {
                if (data.resultCode == 0) {
                    $("#container").html("");
                    var title = "";
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


       
        var mytabs2;
        var html = "";
        
        var arrchannel = new Array();
        var temparray1 = channelControl.getSelectChannels().split(',');
        var temparray2 = $("#mychanneltxt").val().split(',');
        if (temparray1.length == temparray2.length) {
            for (var i = 0; i < temparray1.length; i++) {
                var obj = new Object();
                obj.id = temparray1[i];
                obj.value = temparray2[i];
                arrchannel.push(obj);
            }
        }
     
 
        for (var i = 0; i < arrchannel.length; i++) {
            html += " <li><a onclick=\"gettabs('" + arrchannel[i].id + "','"+ arrchannel[i].value +"')\"><font>" + arrchannel[i].value + "</font></a></li>";
        }
      

        if (arrchannel.length>1) {
            $("#mytags2").html(html);
            mytabs2 = createTabs($("#mytags2"), 0);
            mytabs2.click(0);
        }
        else {
            gettabs("","");
            $("#mytags2").html("");
        }
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
                    "aTargets": [0, 1, 2, 3,4,5,6,7,8]
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
                aoData.push({ "name": "service", "value": "D_HttpDownStatDownAvgCountByAreaImpl" });
                aoData.push({ "name": "do", "value": "get_detailtable" });
                aoData.push({ "name": "endtime", "value": $("#endTime").val() });
                aoData.push({ "name": "begintime", "value": $("#beginTime").val() });
                aoData.push({ "name": "softs", "value": $("#selectsofts").val() });
                aoData.push({ "name": "plat", "value": $("#selectplats").val() });
                aoData.push({ "name": "period", "value": $("#hiddenperiod").val() });
                aoData.push({ "name": "restype", "value": $("#selectrestype").val() });
                aoData.push({ "name": "channelids", "value": channelControl.getSelectChannels()});
                aoData.push({ "name": "channelnames", "value": $("#mychanneltxt").val() });
                aoData.push({ "name": "singlehiddenvalue", "value": $("#hiddenvalue").val() });
                aoData.push({ "name": "singlehiddenname", "value": $("#hiddenname").val() });
                aoData.push({ "name": "areaid", "value": $("#selectArea").val()  });
                aoData.push({ "name": "areatype", "value": g_areatype});
                aoData.push({ "name": "stattype", "value": 1 });
                aoData.push({ "name": "modetype", "value": 4 });
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
        var url = serverUrl + "?do=get_excel&stattype=1&service=D_HttpDownStatDownAvgCountByAreaImpl&projectsource=-1&softs=" + $("#selectsofts").val()
            + "&plat=" + $("#selectplats").val() + "&begintime=" + $("#beginTime").val() + "&endtime=" + $("#endTime").val()
            + "&restype=" + $("#selectrestype").val()  + "&channelids=" + channelControl.getSelectChannels() 
            + "&channelnames=" + $("#mychanneltxt").val()+"&areaid="+$("#selectArea").val() +"&areatype="+ g_areatype  
            + "&singlehiddenvalue=" + $("#hiddenvalue").val() + "&singlehiddenname=" + $("#hiddenname").val()
            +"&period="+$("#hiddenperiod").val()+"&modetype=4";
        $("#downexcel").attr("href", url);
    }
</script>
   
</head>
<body>
    <form id="avgbyversion" runat="server">
    <div class="maindiv">
        <input type="hidden" value="1" id="hiddenperiod" />
        <input type="hidden" value="" id="hiddenvalue"/>
        <input type="hidden" value="" id="hiddenname"/>
        <input type="hidden" value="0" id="hiddenmultitype"/>
        
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
                    <td style="width: 200px;">
                        <div  style="width: 100%">
                            <div id="divareatxt">
                                    <select id="selectArea" style="width: 150px;"  >
                                    </select>
                            </div>
                        </div>
                    </td>
                    <td></td>
          
                </tr>
                <tr>
                    <td>
                        从：
                        <input type="text" style="width: 135px" class="ui-multiselect ui-widget ui-state-default ui-corner-all"  id="beginTime" class="Wdate" value="<%=BeginTime.ToString("yyyy-MM-dd") %>" onclick="WdatePicker()" />
                     </td>
                    <td >
                           到：
                        <input type="text"  style="width:135px" class="ui-multiselect ui-widget ui-state-default ui-corner-all"  id="endTime" class="Wdate" value="<%=EndTime.ToString("yyyy-MM-dd") %>"  onclick="WdatePicker()" />
                    </td>   
                    <td>渠道： <input type="text" style="width:125px" value="" id="mychanneltxt" class="txtbox " style="height: 20px;" /></td>        
                    <td></td>
                    <td>
                        <span style="cursor: pointer; float: right; margin-right: 10px;">
                            <a class="mybutton hover" style="margin-top: 4px; overflow: hidden;" onclick="loadData();">
                                <font>查询</font>
                            </a>
                        </span>
                    </td>
                </tr>
            </table>
        </div>
         <div style="clear: both">
        </div>
        <div class="title" style="margin-top: 4px;">
            <strong class="l" id="Strong2"> 平均下载量
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
                        <th  style="width:10%" rowspan="2">
                            日期
                        </th>
                        <th   style="width:35%"  colspan="3">
                            所有人均分发
                        </th>
                         <th   style="width:35%"  colspan="3">
                            一次人均分发
                        </th>
                        <th   style="width:20%"  colspan="2">
                            二次人均分发
                        </th>
                    </tr>
                    <tr>
                        <th>
                            新用户
                        </th>
                        <th>
                            老用户
                        </th>                                                   
                        <th>
                            所有
                        </th>

                       <th>
                            新用户
                        </th>
                        <th>
                            老用户
                        </th>
                         <th>
                            所有
                        </th>
                        <th>
                            静默
                        </th>
                        <th>
                            所有
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
        reSetVersion($("#selectsofts").val());
        reSetProject($("#selectsofts").val());
    }
    function reSetPlats(softid) {
        var myplats = softPlatJson[softid];
        var selectHtml = "";
        for (var i = 0; i < myplats.length; i++) {
            selectHtml += "<option value='" + myplats[i] + "'";
            if (i == 0) {
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
        var selectHtml = "";
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
    function reSetVersion(softid) {
        $.getJSON(
            serverUrl,
            {
                'service': 'utilityservice',
                'do': 'getversionbysoftidnew',
                'softs': softid,
                'platform': $("#selectplats").val()
            },
            function (data2) {
                var mydata2 = data2.data;
                var a = [], j = 0;
                a[j++] = " ";
                for (var i = 0; i < mydata2.length; i++) {
                    a[j++] = "<option value='";
                    a[j++] = mydata2[i].ID;
                    a[j++] = "'";
                    if (i == 0) {
                        a[j++] = " selected='selected' ";
                    }
                    a[j++] = " >";
                    a[j++] = mydata2[i].Value;
                    a[j++] = "</option>";
                }
                var html = a.join("");
                $("#selectversion").html(html);
                if (versionControl)
                    versionControl.refresh();
                getChart();

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
    
    function reSetArea(softid, defaultValue) {
        g_areatype = SoftAreaJson[softid];
        //g_areatype = SoftAreaJson[softid];
        var areahtml = AreaJson[g_areatype];
        var selectHtml = "";
        for (var i = 0; i < areahtml.length; i++) {
            selectHtml += "<option value='" + areahtml[i].key + "'";
            if (areahtml[i].key == defaultValue) {
                selectHtml += " selected='selected' ";
            }
            selectHtml += ">";
            selectHtml += areahtml[i].value;
            selectHtml += "</option>";
        }
        $("#selectArea").html(selectHtml);
        if (AreaControl) {
            AreaControl.refresh();
        }
    }
    
    function clearSelect() {
        $("#selectversion").val("-1");
        $("#selectprojectsources").val("-1");
        if (projectControl)
            projectControl.refresh();
        if (versionControl)
            versionControl.refresh();
    }
    
    //function funchiddenarea() {
    //    if (SoftAreaJson[$("#selectsofts").val()]==1) {
    //        $(".selectdisplay").hide();
    //    } else {
    //        $(".selectdisplay").show();
    //    }
    //}
</script>