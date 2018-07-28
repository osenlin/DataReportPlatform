<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="D_ResDownLoadSumByVersionDistributionl.aspx.cs" Inherits="net91com.Stat.Web.Reports_New.D_DownlaodStatistics.D_ResDownLoadSumByVersionDistributionl" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
    <head id="Head1" runat="server">
        <title>下载汇总按渠道</title>
        <link href="/Reports_New/css/site.css?version=1" rel="stylesheet" type="text/css" />
        <link href="/Reports_New/css/jquery-ui.css" rel="stylesheet" type="text/css" />
        <link href="../css/jquery.datatables.table.css?id=1" rel="stylesheet" type="text/css" />
        <link href="/Reports_New/css/jquery.multiselect.css" rel="stylesheet" type="text/css" />
        <link href="/Scripts/zTree/css/zTreeStyle.css" rel="stylesheet" type="text/css" />
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
        <script src="/Scripts/zTree/js/jquery.ztree.core-3.2.min.js" type="text/javascript"></script>
        <script src="/Scripts/zTree/js/jquery.ztree.excheck-3.2.min.js" type="text/javascript"></script>
        <script src="../js/My97DatePicker/WdatePicker.js" type="text/javascript"></script>
        <script src="../js/channelControl.js?id=22" type="text/javascript"></script>
        <script src="../js/selectcontrols.js?id=22" type="text/javascript"></script>
        <script src="../js/highcharts_old.js" type="text/javascript"></script>
        <script src="../js/chartjs.js" type="text/javascript"></script> 
        <script type="text/javascript">
            var softControl;
            var platControl;
            var versionControl;
            var projectControl;
            var showTypeControl;
            var restypeControl;
            var softPlatJson = <%= SoftPlatformJson %>;
            var projectJson = <%= ProjectJson %>;
            var serverUrl = '../HttpService.ashx';
            var SoftAreaJson = <%= SoftAreaJson %>;
            var AreaJson = <%= AreaJson %>;
            var tableHtml = "<tr><th rowspan=\"2\"  >排名</th><th rowspan=\"2\"  >版本名称</th> <th colspan=\"5\" >下载点击</th> <th colspan=\"3\" >游戏下载</th> <th colspan=\"2\" >下载情况</th> <th colspan=\"2\">安装情况</th></tr><tr><th>所有</th><th>去除更新</th><th >静默更新</th>";
            tableHtml += "<th>来自搜索 </th><th>排期下载</th> <th > 所有 </th> <th > 去除更新 </th> <th > 来自搜索 </th><th >下载成功</th><th >下载失败</th>";
            tableHtml += "<th>安装成功</th><th>安装失败</th></tr>";
            var tableHtml_46 = "<tr><th rowspan=\"2\"  >排名</th><th rowspan=\"2\" style=\"width: 120px\">版本名称</th> <th colspan=\"5\" >下载点击</th> <th colspan=\"3\" >下载成功</th> <th colspan=\"3\" >下载失败</th> <th colspan=\"3\">安装成功</th> <th colspan=\"3\">安装失败</th></tr><tr><th>所有</th><th>去除更新</th>";
            tableHtml_46 += "<th>来自更新</th><th>来自搜索</th><th >静默更新</th><th >所有</th><th >更新</th><th >静默更新</th><th >所有</th><th >更新</th><th >静默更新</th>";
            tableHtml_46 += "<th >所有</th><th >更新</th><th >静默更新</th><th >所有</th><th >更新</th><th >静默更新</th></tr>";

            var showType;
            var channelControl;
            var AreaControl;
            var modeType;
            var g_areatype;

            $(function() {
                createTabs($("#parenttags"), 0);
                createTabs($("#tab_type"), 0);
                channelControl = createChannelBoxControl("mychanneltxt", $("#selectsofts").val(), 0, 1);
                channelControl.needrecreate = 1;
                channelControl.clearSelectChannels();
                softControl = createSelectControl("selectsofts", '请选择软件', false, function() {
                    if ($("#selectsofts").val() == null) {
                        alert("请选择一个软件");
                        return false;
                    }
                    reSetPlats($("#selectsofts").val(),4);
                    channelControl.softid = $("#selectsofts").val();
                    channelControl.needrecreate = 1;
                    return true;
                }, function() {
                    return true;
                }, true);
                reSetVersion($("#selectsofts").val());
                reSetPlats($("#selectsofts").val(),4);
                platControl = createSelectControl("selectplats", '请选择平台', false, function() {
                    if ($("#selectplats").val() == null) {
                        alert("请选择一个平台");
                        return false;
                    }
                    reSetVersion($("#selectsofts").val());
                    return true;
                }, function() {
                    return true;
                }, true);

                restypeControl = createSelectControl("selectrestype", '请选择', false, function() {
                    if ($("#selectrestype").val() == null) {
                        $("#selectrestype").val(1);
                        return false;
                    }
                    return true;
                }, function() {
                    return true;
                }, true);

                showTypeControl = createSelectControl("selectshowtype", '请选择', false, function() {
                    if ($("#selectshowtype").val() == null) {
                        alert("请选择一个展示类型");
                        return false;
                    }
                    return true;
                }, function() {
                    return true;
                }, false,false);

                getChart();

            });

            var mytabs;

            function loadData() {
                if (checkDataRight()) {
                    getChart();
                }
            }


            var oTable;
            function getSetting(softid) {
                var defaultSetting={
                    "sDom": '<"top">rt<"bottom"fip><"clear">',
                    "bPaginate": false,
                    "bDestroy":true,
                    "aoColumns": [
                        { "sClass": "tdleft","aTargets" : [ 0 ] },
                        { "sClass": "tdright","aTargets" : [ 1 ],"sWidth":"70px"  },
                        { "sClass": "tdright","aTargets" : [ 2 ] },
                        { "sClass": "tdright","aTargets" : [ 3 ] },
                        { "sClass": "tdright","aTargets" : [ 4 ] },
                        { "sClass": "tdright","aTargets" : [ 5 ] },
                        { "sClass": "tdright","aTargets" : [ 6 ] },
                        { "sClass": "tdright","aTargets" : [ 7 ] },
                        { "sClass": "tdright","aTargets" : [ 8 ] },
                        { "sClass": "tdright","aTargets" : [ 9 ] },
                        { "sClass": "tdright","aTargets" : [ 10 ] },
                        { "sClass": "tdright","aTargets" : [ 11 ] },
                        { "sClass": "tdright","aTargets" : [ 12 ] },
                        { "sClass": "tdright","aTargets" : [ 13 ] },
                    ],
                    "aoColumnDefs": [
                        {
                            "bSortable": false,
                            "sDefaultContent": '',
                            "aTargets": [ '_all' ]
                        }
                    ],
                    "bProcessing": true,
                    "bStateSave" : false,
                    "bServerSide": true,
                    "sServerMethod" : "POST",
                    "iDisplayLength": 30,
                    "oLanguage": { sUrl: "../js/de_DE.txt" },
                    "bFilter": false,
                    "sPaginationType": "full_numbers",
                    "sAjaxSource": serverUrl+"?rand="+Math.random(),
                    "fnServerParams": function(aoData) {
                        aoData.push({ "name": "service", "value": "D_HttpDownStatDownCountSumByVerisionDistributionImpl" });
                        aoData.push({ "name": "do", "value": "get_detailtable" });
                        aoData.push({ "name": "endtime", "value": $("#endTime").val() });
                        aoData.push({ "name": "begintime", "value": $("#beginTime").val() });
                        aoData.push({ "name": "softs", "value": $("#selectsofts").val() });
                        aoData.push({ "name": "plat", "value": $("#selectplats").val() });
                        aoData.push({ "name": "restype", "value": $("#selectrestype").val() });
                       
                        aoData.push({ "name": "channelids", "value": channelControl.getSelectChannels() });
                        aoData.push({ "name": "channelnames", "value": $("#mychanneltxt").val() });
                        aoData.push({ "name": "projectsource", "value": $("#selectprojectsources").val() });
                        aoData.push({ "name": "singlehiddenvalue", "value": $("#hiddenvalue").val() });
                        aoData.push({ "name": "singlehiddenname", "value": $("#hiddenname").val() });
                        aoData.push({ "name": "period", "value": $("#hiddenperiod").val() });
                        aoData.push({ "name": "multype", "value": $("#hiddenmultitype").val() });
                    },
                    "fnServerData": function(sSource, aoData, fnCallback) {
                        $.ajax({
                            "dataType": 'json',
                            "type": "POST",
                            "url": sSource,
                            "data": aoData,
                            "success": function(data) {
                                if (data.resultCode && data.resultCode != 0) {
                                    alert(data.message);
                                } else {
                                    fnCallback(data);
                                }

                            }
                        });
                    },
                    "fnDrawCallback": function() {
                        tableStyleSet("mytable");
                        setDownExcel();
                    }
                    ,
                    "fnInitComplete": function() {
                        this.fnAdjustColumnSizing(true);
                    }
                };
                if (softid == 46|| softid==58 || softid==85) {
                    defaultSetting.aoColumns = [
                        { "sClass": "tdleft","aTargets" : [ 0 ] },
                        { "sClass": "tdright","aTargets" : [ 1 ],"sWidth":"70px"   },
                        { "sClass": "tdright","aTargets" : [ 2 ] },
                        { "sClass": "tdright","aTargets" : [ 3 ] },
                        { "sClass": "tdright","aTargets" : [ 4 ] },
                        { "sClass": "tdright","aTargets" : [ 5 ] },
                        { "sClass": "tdright","aTargets" : [ 6 ] },
                        { "sClass": "tdright","aTargets" : [ 7 ] },
                        { "sClass": "tdright","aTargets" : [ 8 ] },
                        { "sClass": "tdright","aTargets" : [ 9 ] },
                        { "sClass": "tdright","aTargets" : [ 10 ] },
                        { "sClass": "tdright","aTargets" : [ 11 ] },
                        { "sClass": "tdright","aTargets" : [ 12 ] },
                        { "sClass": "tdright","aTargets" : [ 13 ] },
                        { "sClass": "tdright","aTargets" : [ 14 ] },
                        { "sClass": "tdright","aTargets" : [ 15 ] },
                        { "sClass": "tdright","aTargets" : [ 16 ] },
                        { "sClass": "tdright","aTargets" : [ 17 ] },
                         { "sClass": "tdright","aTargets" : [ 18 ] },
                    ];
                    defaultSetting.aoColumnDefs = [
                        {
                            "bSortable": false,
                            "sDefaultContent": '',
                            "aTargets": [ '_all' ],
                            "sClass": "tdright"
                        }
                    ];
                    $("#mytable thead").html(tableHtml_46);
                    return defaultSetting;
                } else {
                    defaultSetting.aoColumns = [
                        { "sClass": "tdleft","aTargets" : [ 0 ]  },
                        { "sClass": "tdright","aTargets" : [ 1 ],"sWidth":"70px"  },
                        { "sClass": "tdright","aTargets" : [ 2 ] },
                        { "sClass": "tdright","aTargets" : [ 3 ] },
                        { "sClass": "tdright","aTargets" : [ 4 ] },
                        { "sClass": "tdright","aTargets" : [ 5 ] },
                        { "sClass": "tdright","aTargets" : [ 6 ] },
                        { "sClass": "tdright","aTargets" : [ 7 ] },
                        { "sClass": "tdright","aTargets" : [ 8 ] },
                        { "sClass": "tdright","aTargets" : [ 9 ] },
                        { "sClass": "tdright","aTargets" : [ 10 ] },
                        { "sClass": "tdright","aTargets" : [ 11 ] },
                        { "sClass": "tdright","aTargets" : [ 12 ] },
                        { "sClass": "tdright","aTargets" : [ 13 ] },
                    ];
                    defaultSetting.aoColumnDefs = [
                        {
                            "bSortable": false
                            ,
                            "sDefaultContent": '',
                            "aTargets": [ '_all' ],
                            "sClass": "tdright" 
                        }
                    ];
                    $("#mytable thead").html(tableHtml);
                    return defaultSetting;
                }
            }

            function gettabs(value, name, multype) {
                $("#hiddenvalue").val(value);
                $("#hiddenname").val(name);
                $("#hiddenmultype").val(multype);
                if (!checkDataRight()) {
                    return;
                }
                var  s = getSetting($("#selectsofts").val());
                oTable = $('#mytable').dataTable(s); 
 
            }

            function getChart() {
                $.ajax({
                    type: "get",
                    url: serverUrl,
                    dataType: "json",
                    data: {
                        'service': 'D_HttpDownStatDownCountSumByVerisionDistributionImpl',
                        'do': 'get_chart',
                        'endtime': $("#endTime").val(),
                        'begintime': $("#beginTime").val(),
                        'softs': $("#selectsofts").val(),
                        'plat': $("#selectplats").val(),
                        'restype': $("#selectrestype").val(),
                        'channelids': channelControl.getSelectChannels(),
                        'channelnames': $("#mychanneltxt").val(),
                        'period': $("#hiddenperiod").val(),
                        'showtype':$("#selectshowtype").val()
                    },
                    success: function(data) {
                        if (data.resultCode == 0) {
                            $("#container").html("");
                            var title = "";
                            var yname = "量";
                            var obj = eval("(" + data.data + ")");
                            createPie("container","下载量版本分布", obj);
                            //createLine("container", 400, obj.title, yname, true, 0, obj.x, obj.y);
                            gettabs();
                        } else {
                            alert(data.message);
                        }
                    }
                });
            }


            ///设置下载
            function setDownExcel() {
                if (!checkDataRight()) { //!isSingleMulti()
                    return false;
                }
                var url = serverUrl + "?do=get_excel&service=D_HttpDownStatDownCountSumByVerisionDistributionImpl&projectsource=" + $("#selectprojectsources").val() + "&softs=" + $("#selectsofts").val()
                    + "&plat=" + $("#selectplats").val() + "&begintime=" + $("#beginTime").val() + "&endtime=" + $("#endTime").val()
                    + "&restype=" + $("#selectrestype").val() 
                    + "&channelids=" + channelControl.getSelectChannels() + "&channelnames=" + $("#mychanneltxt").val()
                    + "&period=" + $("#hiddenperiod").val() + 
                    "&multype=" + $("#hiddenmultitype").val();
                $("#downexcel").attr("href", url);
            }

        </script>

    </head>
    <body>

        <form id="form1" runat="server">
            <div class="maindiv">
                <!--只用于前台的标识-->
                <input type="hidden" value="2" id="hiddenmultitype" />

                <input type="hidden" value="" id="hiddenvalue" />
                <input type="hidden" value="" id="hiddenname" />
                <input type="hidden" value="1" id="hiddenperiod" />
                <input type="hidden" value="1" id="hiddenmultype" />

                <input type="hidden" value="1" id="hiddenareatype" />
                <div style="padding: 10px;">
                    <table id="table1" cellpadding="0" cellspacing="10" border="0" width="100%">
                        <tr>

                            <td style="width: 200px">
                                <div style="position: relative; z-index: 100">
                                    <select id="selectsofts" style="width: 150px;">
                                        <%= SoftHtml %>
                                    </select>
                                </div>
                            </td>

                            <td style="width: 200px">
                                <div style="position: relative; z-index: 100">
                                    <select id="selectplats" style="width: 150px;">
                                        <%= PlatHtml %>
                                    </select>
                                </div>
                            </td>

                            <td style="width: 200px">
                                <div style="position: relative; z-index: 100">
                                    <select id="selectrestype" style="width: 150px;">
                                       <option value="-1" selected="selected">不区分资源类型</option>
                                         <%= RestypeHtml %>
                                    </select>
                                </div>
                            </td>
                             <td>
                                <select id="selectshowtype" style="width: 120px;" >
                                    <option value="1" selected="selected">下载点击(所有)</option>
                                    <option value="2">下载点击(去除更新)</option>
                                    <option value="3">下载点击(搜索带量)</option>
                                    <option value="4">下载点击(更新带量)</option>
                                    <option value="8">下载成功</option>
                                    <option value="9">安装成功</option>
                                </select>
                            </td>
                        </tr>
                        <tr>
                            <td>从：
                                <input  style="width: 135px" class="ui-multiselect ui-widget ui-state-default ui-corner-all Wdate" type="text" id="beginTime"  value="<%= BeginTime.ToString("yyyy-MM-dd") %>" onclick=" WdatePicker() " />
                          
                            </td>  <td>   
                                 到：
                                <input style="width: 135px" class="ui-multiselect ui-widget ui-state-default ui-corner-all Wdate" type="text" id="endTime"  value="<%= EndTime.ToString("yyyy-MM-dd") %>" onclick=" WdatePicker() " />
                            </td>
                            <td  style="width: 140px">
                                渠道：
                                 <input type="text" value=""  id="mychanneltxt" class="txtbox "  style="width: 112px;height: 20px;" />
                            </td>
                            <td>
                                <span style="cursor: pointer; float: right;"><a class="mybutton hover"
                                                                                style="margin-top: 4px; overflow: hidden;" onclick=" loadData(); "><font>查询</font>
                                                                             </a></span>
                            </td>
                        </tr>
                    </table>
                </div>
                <div style="clear: both">
                </div>
                <div class="title" style="margin-top: 4px;">
                    <strong class="l" id="Strong2">
                              下载量版本分布
<%--                        <ul id="parenttags">
                            <li><a onclick=" javascript:getperiod(1) "><font>日</font></a></li>
                            <li><a onclick=" javascript:getperiod(3) "><font>周</font></a></li>
                            <li><a onclick=" javascript:getperiod(12) "><font>月</font></a></li>
                        </ul>--%>
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
                    <table id="mytable" cellpadding="0" cellspacing="0" border="0" width="100%"   class="display cell-border">
                        <thead>
                            
                        </thead>
                    </table>

                </div>

            </div>
        </form>
    </body>
</html>
<script type="text/javascript">


    function reSetPlats(softid, defaultValue) {
        var myplats = softPlatJson[softid];
        var selectHtml = "<option value='-1' selected='selected'>不区分平台</option>"; //"";
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

    function reSetVersion(softid, defaultValue) {
        $.getJSON(
            serverUrl,
            {
                'service': 'utilityservice',
                'do': 'getversionbysoftidnew',
                'softs': softid,
                'platform': $("#selectplats").val()
            },
            function(data2) {
                var mydata2 = data2.data;
                var a = [], j = 0;
                a[j++] = "<option value='-1' selected='selected'>不区分版本</option>";
                for (var i = 0; i < mydata2.length; i++) {
                    a[j++] = "<option value='";
                    a[j++] = mydata2[i].ID;
                    a[j++] = "'";
                    if (mydata2[i].ID == defaultValue) {
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

            });

    }



    function clearSelect() {
        $("#selectversion").val("-1");
        $("#selectprojectsources").val("-1");
        if (projectControl)
            projectControl.refresh();
        if (versionControl)
            versionControl.refresh();
        channelControl.clearSelectChannels();
    }



</script>
