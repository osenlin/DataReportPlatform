<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="D_ResDownLoadSumByChannel.aspx.cs" Inherits="net91com.Stat.Web.Reports_New.D_DownlaodStatistics.D_ResDownLoadSumByChannel" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
    <head runat="server">
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

            function getContronlValueLength(controlid) {
                return $("#" + controlid).find("option:selected").length;
            }
            //渠道1 版本2 showtype:3
            function showtypeMulti(event, ui) {

                if (ui.checked==false|| getContronlValueLength("selectshowtype")<1) {
                    return true;
                }
                
                var threecontrol = $('#mychanneltxt').val().indexOf(',');
                if (threecontrol != -1) {
                    alert('不允许多个维度同时多选，渠道已经多选');
                    $("#hiddenmultitype").val(1);
                    return false;
                }

                if (getContronlValueLength("selectversion") > 1) {
                    alert('不允许多个维度同时多选,版本已经多选');
                    $("#hiddenmultitype").val(2);
                    return false;
                }

                return true;
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
                if (getContronlValueLength("selectshowtype") > 1) {
                    alert('不允许多个维度同时多选,展示类型，获取区域已经多选');
                    $("#hiddenmultitype").val(3);
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
            var softPlatJson = <%= SoftPlatformJson %>;
            var projectJson = <%= ProjectJson %>;
            var serverUrl = '../HttpService.ashx';
            var SoftAreaJson = <%= SoftAreaJson %>;
            var AreaJson = <%= AreaJson %>;
            var tableHtml = "<tr><th rowspan=\"2\"  >日期</th> <th colspan=\"5\" >下载点击</th> <th colspan=\"3\" >游戏下载</th> <th colspan=\"2\" >下载情况</th> <th colspan=\"2\">安装情况</th></tr><tr><th>所有</th><th>去除更新</th><th >静默更新</th>";
            tableHtml += "<th>来自搜索 </th><th>排期下载</th> <th > 所有 </th> <th > 去除更新 </th> <th > 来自搜索 </th><th >下载成功</th><th >下载失败</th>";
            tableHtml += "<th>安装成功</th><th>安装失败</th></tr>";
            var tableHtml_46 = "<tr><th rowspan=\"2\" style=\"width: 120px\">日期</th> <th colspan=\"5\" >下载点击</th> <th colspan=\"3\" >下载成功</th> <th colspan=\"3\" >下载失败</th> <th colspan=\"3\">安装成功</th> <th colspan=\"3\">安装失败</th></tr><tr><th>所有</th><th>去除更新</th>";
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
                channelControl = createChannelBoxControl("mychanneltxt", $("#selectsofts").val(), 0, 5);
                softControl = createSelectControl("selectsofts", '请选择软件', false, function() {
                    if ($("#selectsofts").val() == null) {
                        alert("请选择一个软件");
                        return false;
                    }
                    reSetPlats($("#selectsofts").val(),4);
                    channelControl.softid = $("#selectsofts").val();

                    reSetSelectMode($("#selectmodetype").val());
                    return true;
                }, function() {
                    return true;
                }, true);
                
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


                versionControl = createSimpleSelectControl("selectversion", '请选择', true, function() {
                    if ($("#selectversion").val() == null) {
                        alert("请选择一个版本");
                        return false;
                    }

                    if (getContronlValueLength("selectversion") > 1 ) {
                        $("#hiddenmultitype").val(2);
                        channelzTreeMaxSelected = 1;
                    } else {
                        if (getContronlValueLength("selectshowtype") > 1) {
                            $("#hiddenmultitype").val(3);
                            channelzTreeMaxSelected = 1;
                        } else {
                           // $("#hiddenmultitype").val(1);
                            channelzTreeMaxSelected = 5;
                        }

                    }
                    channelControl.needrecreate = 1;
                    channelControl.clearSelectChannels();
                    
                    return true;
                }, function() {
                    return true;
                }, true, versionMulti);

                restypeControl = createSelectControl("selectrestype", '请选择', false, function() {
                    if ($("#selectrestype").val() == null) {
                        $("#selectrestype").val(1);
                        return false;
                    }
                    return true;
                }, function() {
                    return true;
                }, true);

                showTypeControl = createSimpleSelectControl("selectshowtype", '请选择', true, function() {
                    if ($("#selectshowtype").val() == null) {
                        alert("请选择一个展示类型");
                        return false;
                    }
                    
                    if (getContronlValueLength("selectshowtype") > 1 ) {
                        $("#hiddenmultitype").val(3);
                        channelzTreeMaxSelected = 1;
                    } else {
                        if (getContronlValueLength("selectversion") > 1) {
                            $("#hiddenmultitype").val(3);
                            channelzTreeMaxSelected = 1;
                        } else {
                            //$("#hiddenmultitype").val(1);如果值1 detail表格会有问题
                            channelzTreeMaxSelected = 5;
                        }
                    }
                    channelControl.needrecreate = 1;
                    channelControl.clearSelectChannels();
                    return true;
                }, function() {
                    return true;
                }, false, showtypeMulti);

                modeType = createSelectControl("selectmodetype", '请选择', false, function() {
                    selectChangeMode($("#selectmodetype").val());
                    return true;
                }, function() {
                    return true;
                }, false, false, 100, 100);

                selectChangeMode($("#selectmodetype").val());
                reSetVersion($("#selectsofts").val());//getChart();
                reSetSelectMode($("#selectmodetype").val());

                $(window).bind('resize', function() {
                    oTable.fnAdjustColumnSizing();
                });
            });

            var mytabs;

            function loadData() {
                var threecontrol = $('#mychanneltxt').val().indexOf(',');
                if (threecontrol!=-1) {
                    $("#hiddenmultitype").val(1);
                } else {
                    if (getContronlValueLength("selectversion") > 1) {
                        $("#hiddenmultitype").val(2);
                    } else {
                        if (getContronlValueLength("selectshowtype") > 1) {
                            $("#hiddenmultitype").val(3);
                        } else {
                            $("#hiddenmultitype").val(2);
                        }
                    }
                }
                
                if (checkDataRight()) {
                    getChart();
                }
            }

            //获取展现类型

            function getShowType() {
                var array = $("#selectshowtype").val();
                var showtypes = "";
                for (var i = 0; i < array.length; i++) {
                    showtypes += array[i] + ',';
                }
                showtypes = showtypes.substring(0, showtypes.length - 1);
                return showtypes;
            }

            function getperiod(period) {
                $("#hiddenperiod").val(period);
                loadData();
            }

            var oTable;
            function getSetting(softid) {
                var defaultSetting={
                    "sDom": '<"top">rt<"bottom"fip><"clear">',
                    "bPaginate": false,
                    "bDestroy":true,
                    "aoColumns": [
                        { "sClass": "tdleft","aTargets" : [ 0 ],"sWidth":"70px"  },
                        { "sClass": "tdright","aTargets" : [ 1 ] },
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
                        { "sClass": "tdright","aTargets" : [ 12 ] }
                    ],
                    "aoColumnDefs": [
                        {
                            "bSortable": false,
                            //"aTargets": [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12],
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
                        aoData.push({ "name": "service", "value": "D_HttpResDownLoadSumByChannelImpl" });
                        aoData.push({ "name": "do", "value": "get_detailtable" });
                        aoData.push({ "name": "endtime", "value": $("#endTime").val() });
                        aoData.push({ "name": "begintime", "value": $("#beginTime").val() });
                        aoData.push({ "name": "softs", "value": $("#selectsofts").val() });
                        aoData.push({ "name": "plat", "value": $("#selectplats").val() });
                        aoData.push({ "name": "restype", "value": $("#selectrestype").val() });
                        aoData.push({ "name": "version", "value": $("#selectversion").val() });
                        aoData.push({ "name": "modetype", "value": $("#selectmodetype").val() });
                        aoData.push({ "name": "showtype", "value": getShowType() });
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
                                //存在输出的resultCode 让他输出message
                                if (data.resultCode && data.resultCode != 0) {
                                    alert(data.message);
                                } else {
                                    //alert(data.Title);
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
                if (softid == 46) {
                    defaultSetting.aoColumns = [
                        { "sClass": "tdleft","aTargets" : [ 0 ],"sWidth":"70px"   },
                        { "sClass": "tdright","aTargets" : [ 1 ] },
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
                        { "sClass": "tdright","aTargets" : [ 17 ] }
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
                        { "sClass": "tdleft","aTargets" : [ 0 ],"sWidth":"70px"   },
                        { "sClass": "tdright","aTargets" : [ 1 ] },
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
                        { "sClass": "tdright","aTargets" : [ 12 ] }
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
                var versions = $("#selectversion").val();
                var verstr = "";
                if (versions) {
                    verstr = "";
                    for (var i = 0; i < versions.length; i++) {
                        verstr += versions[i] + ',';
                    }
                    verstr = verstr.substring(0, verstr.length - 1);
                }
                $.ajax({
                    type: "get",
                    url: serverUrl,
                    dataType: "json",
                    data: {
                        'service': 'D_HttpResDownLoadSumByChannelImpl',
                        'do': 'get_chart',
                        'endtime': $("#endTime").val(),
                        'begintime': $("#beginTime").val(),
                        'softs': $("#selectsofts").val(),
                        'plat': $("#selectplats").val(),
                        'restype': $("#selectrestype").val(),
                        'version': verstr,
                        'modetype': $("#selectmodetype").val(),
                        'showtype': getShowType(),
                        'channelids': channelControl.getSelectChannels(),
                        'channelnames': $("#mychanneltxt").val(),
                        'period': $("#hiddenperiod").val(),
                        'multype':$("#hiddenmultitype").val()
                    },
                    success: function(data) {
                        if (data.resultCode == 0) {
                            $("#container").html("");
                            var title = "";
                            var yname = "量";
                            var obj = eval("(" + data.data + ")");
                            //var obj = JSON.parse(data.data);
                            createLine("container", 400, obj.title, yname, true, 0, obj.x, obj.y);
                            getDetailTableTitle();
                        } else {
                            alert(data.message);
                        }
                    }
                });
            }

            function getDetailTableTitle() {
                var arr = new Array();
                var multype = $("#hiddenmultitype").val(); // isSingleMulti();
                if (multype == "1") {
                    var temparray1 = channelControl.getSelectChannels().split(',');
                    var temparray2 = $("#mychanneltxt").val().split(',');
                    if (temparray1.length == temparray2.length) {
                        for (var i = 0; i < temparray1.length; i++) {
                            if (temparray1[i] == "") {
                                continue;
                            }
                            var obj = new Object();
                            obj.id = temparray1[i];
                            obj.value = temparray2[i];
                            arr.push(obj);
                        }
                    }
                } else if (multype == "2") {
                    arr = getConditionArray("selectversion");
                } 
                
                var html = "";
                var detailTable;
                for (var i = 0; i < arr.length; i++) {
                    html += " <li><a onclick=\"gettabs('" + arr[i].id + "','" + arr[i].value + "',2)\"><font>" + arr[i].value + "</font></a></li>";
                }
                $("#mytags2").html(html);
                detailTable = createTabs($("#mytags2"), 0);
                detailTable.click(0);
                if (arr.length>1) {
                    $("#mytags2").show();
                } else {
                    $("#mytags2").hide();
                }
            }

            ///设置下载
            function setDownExcel() {
                if (!checkDataRight()) { //!isSingleMulti()
                    return false;
                }
                var url = serverUrl + "?do=get_excel&service=D_HttpResDownLoadSumByChannelImpl&projectsource=" + $("#selectprojectsources").val() + "&softs=" + $("#selectsofts").val()
                    + "&plat=" + $("#selectplats").val() + "&begintime=" + $("#beginTime").val() + "&endtime=" + $("#endTime").val()
                    + "&restype=" + $("#selectrestype").val() + "&version=" + $("#selectversion").val() + "&modetype=" + $("#selectmodetype").val() + "&showtype=" + getShowType()
                    + "&channelids=" + channelControl.getSelectChannels() + "&channelnames=" + $("#mychanneltxt").val()
                    + "&singlehiddenvalue=" + $("#hiddenvalue").val() + "&singlehiddenname=" + $("#hiddenname").val() + "&period=" + $("#hiddenperiod").val() + 
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
                            <td style="width: 40px;">
                                <div style="position: relative; z-index: 100">
                                  <select id="selectversion" style="width: 150px; display: block" multiple="multiple">
                                        <%--<option value="-1" selected="selected">不区分版本</option>--%>
                                    </select>
                                </div>
                            </td>
                            <td></td>
                        </tr>
                        <tr>
                            <td>从：
                                <input  style="width: 135px" class="ui-multiselect ui-widget ui-state-default ui-corner-all Wdate" type="text" id="beginTime"  value="<%= BeginTime.ToString("yyyy-MM-dd") %>" onclick=" WdatePicker() " />
                          
                            </td>  <td>   
                                 到：
                                <input style="width: 135px" class="ui-multiselect ui-widget ui-state-default ui-corner-all Wdate" type="text" id="endTime"  value="<%= EndTime.ToString("yyyy-MM-dd") %>" onclick=" WdatePicker() " />
                            </td>
                            <td>
                                <select id="selectshowtype" style="width: 120px;" multiple="multiple">
                                    <option value="1" selected="selected">下载点击(所有)</option>
                                    <option value="2">下载点击(去除更新)</option>
                                    <option value="3">下载点击(搜索带量)</option>
                                    <option value="4">下载点击(更新带量)</option>
                                    <option value="5">去除更新和搜索</option>
                                    <option value="6">游戏下载</option>
                                    <option value="7">游戏下载(去除更新)</option>
                                    <option value="8">下载成功</option>
                                    <option value="9">安装成功</option>
                                    <option value="10">排期下载</option>
                                </select>

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
                        <ul id="parenttags">
                            <li><a onclick=" javascript:getperiod(1) "><font>日</font></a></li>
                            <li><a onclick=" javascript:getperiod(3) "><font>周</font></a></li>
                            <li><a onclick=" javascript:getperiod(12) "><font>月</font></a></li>
                        </ul>
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

    function reSetSelectMode(type) {
        channelControl.needrecreate = 1;
        channelControl.clearSelectChannels();
        reSetVersion($("#selectsofts").val());
        reSetProject($("#selectsofts").val());
    }


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
                a[j++] = "";
                for (var i = 0; i < mydata2.length; i++) {
                    a[j++] = "<option value='" ;
                    a[j++] = mydata2[i].Value;
                    a[j++] = "'";
                    if (i==0) {
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
        case "1":
            $("#divchanneltxt").show();
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
        $("#selectversion").val("-1");
        $("#selectprojectsources").val("-1");
        if (projectControl)
            projectControl.refresh();
        if (versionControl)
            versionControl.refresh();
        channelControl.clearSelectChannels();
    }

    function getConditionArray(controlid) {
        var lists = new Array();
        var arr_control = $("#" + controlid).find("option:selected");
        for (var i = 0; i < arr_control.length; i++) {
            var obj = new Object();
            obj.id = arr_control[i].value;
            obj.value = arr_control[i].text;
            lists.push(obj);
        }
        return lists;
    }

</script>
