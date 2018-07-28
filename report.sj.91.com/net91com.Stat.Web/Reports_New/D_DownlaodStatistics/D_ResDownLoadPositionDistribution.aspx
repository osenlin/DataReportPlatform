<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="D_ResDownLoadPositionDistribution.aspx.cs" Inherits="net91com.Stat.Web.Reports_New.D_DownlaodStatistics.D_ResDownLoadPositionDistribution" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
    <head id="Head1" runat="server">
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
            var softControl;
            var platControl;
            var versionControl;
            var projectControl;
            var showTypeControl;
            var restypeControl;
            var softPlatJson = <%= SoftPlatformJson %>;
            var projectJson = <%= ProjectJson %>;
            var SoftAreaJson = <%=SoftAreaJson %>;
            var serverUrl = '../HttpService.ashx';
            var showType;
            var channelControl;
            var IsUpdateControl;
            var modeType;
            var g_areatype;
            var hiddenDialog;
            var nowopen;
            var diffpagetypeControl;
            var stattypeControl;
            var AreaControl;
            
            $(function() {
                createTabs($("#parenttags"), 0);
                softControl = createSelectControl("selectsofts", '请选择软件', false, function() {
                    if ($("#selectsofts").val() == null) {
                        alert("请选择一个软件");
                        return false;
                    }

                    funchiddenarea();
                    
                    reSetPlats($("#selectsofts").val());
                    reSetVersion($("#selectsofts").val());
                    reSetSelectMode($("#selectmodetype").val());
                    return true;
                }, function() {
                    return true;
                }, true);
                funchiddenarea();
                reSetPlats($("#selectsofts").val());
                reSetVersion($("#selectsofts").val());
                
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

                versionControl = createSelectControl("selectversion", '请选择', true, function() {
                    if ($("#selectversion").val() == null) {
                        alert("请选择一个版本");
                        return false;
                    }
                    return true;
                }, function() {
                    return true;
                }, true);
                projectControl = createSelectControl("selectprojectsources", '请选择', false, function() {
                    if ($("#selectprojectsources").val() == null) {
                        alert("请选择一个来源");
                        return false;
                    }
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

                IsUpdateControl=createSelectControl("selectIsUpdate", '请选择', false, function() {
                    if ($("#selectIsUpdate").val() == null) {
                        $("#selectIsUpdate").val(0);
                        return false;
                    }
                    return true;
                }, function() {
                    return true;
                }, false,false);
                
                diffpagetypeControl = createSelectControl("selectdiffpagetype", '请选择', false, function() {
                    if ($("#selectdiffpagetype").val() == null) {
                        $("#selectdiffpagetype").val(-1);
                        return false;
                    }
                    return true;
                }, function() {
                    return true;
                }, false,false);

                stattypeControl = createSelectControl("selectstattype", '请选择', false, function() {
                    if ($("#selectstattype").val() == null) {
                        $("#selectstattype").val(1);
                        return false;
                    }
                    return true;
                }, function() {
                    return true;
                }, false,false);
                
                AreaControl=createSelectControl("selectareaid", '请选择', false, function() {
                    return true;
                }, function() {
                    return true;
                }, false,false);
                
                
                reSetSelectMode();
                getChart();
               
                hiddenDialog = $("#mywindow").colorbox({ iframe: true, width: "100%", height: "100%", speed: 0 });
                
            });

            var mytabs;

            function loadData() {
                if (checkDataRight()) { //&&isSingleMulti()
                    getChart();
                }
            }

           
            function getperiod(period) {
                $("#hiddenperiod").val(period);
                loadData();
            }
            
            function getVersionList() {
                var versions = $("#selectversion").val();
                var verstr = "";
                if (versions) {
                    verstr = "";
                    for (var i = 0; i < versions.length; i++) {
                        verstr += versions[i] + ',';
                    }
                    verstr = verstr.substring(0, verstr.length - 1);
                }
                return verstr;
            }
            
            var oTable;
            function getChart() {
               
                $.ajax({
                    type: "get",
                    url: serverUrl,
                    dataType: "json",
                    data: {
                        'service': 'D_HttpDownStatDownPositionDistributionImpl',
                        'do': 'get_chart',
                        'endtime': $("#endTime").val(),
                        'begintime': $("#beginTime").val(),
                        'softs': $("#selectsofts").val(),
                        'plat': $("#selectplats").val(),
                        'restype': $("#selectrestype").val(),
                        'version': getVersionList(),
                        'projectsource': $("#selectprojectsources").val(),
                        'period': $("#hiddenperiod").val(),
                        'downtype':$("#selectIsUpdate").val(),
                        'isdiffpagetype':$("#selectdiffpagetype").val(),
                        'stattype':$('#selectstattype').val(),
                        'areaid':$('#selectareaid').val()
                    },
                    success: function(data) {
                        if (data.resultCode == 0) {
                            $("#container").html("");
                            var obj = eval("(" + data.data + ")");
                            createPie("container","下载位置分布", obj);
                            getDetailTableTitle();
                        } else {
                            alert(data.message);
                        }
                    }
                });
            }
            
            //日期详细
            function openwindow(positionid,isTagClassDetail,pagename) {
                var array = pagename.split("_");
                pagename = array[0];
                positionid = positionid.indexOf("_") > -1 ? "-1" : positionid;
                //isTagClassDetail 0 false 1true
                var url = "projectsource=" + $("#selectprojectsources").val() + "&softs=" + $("#selectsofts").val()+"&positionid="+positionid+"&pagename="+escape(pagename)
                    + "&plat=" + $("#selectplats").val() + "&begintime=" + $("#beginTime").val() + "&endtime=" + $("#endTime").val()
                    + "&restype=" + $("#selectrestype").val() + "&version=" + getVersionList() +"&downtype="+$("#selectIsUpdate").val()
                    + "&period=" + $("#hiddenperiod").val()+"&isTagClassDetail="+ isTagClassDetail+"&isdiffpagetype="+$("#selectdiffpagetype").val()
                    +"&stattype="+$('#selectstattype').val()+"&areaid="+$('#selectareaid').val();
                
                $("#mywindow").attr("href","D_ResDownLoadPositionDistributionDetail.aspx?"+url);
                $("#mywindow").click();
            }

            function gettabs() {
                if (!checkDataRight()) {
                    return;
                }
                oTable = $('#mytable').dataTable({
                    "sDom": '<"top">rt<"bottom"fip><"clear">',
                    "bPaginate": false,
                    "bDestroy":true,
                    "aoColumns": [
                        { "sClass": "tdleft","sWidth":"25%"},
                        { "sClass": "tdright","sWidth":"20%" },
                        { "sClass": "tdright","sWidth":"10%" },
                        { "sClass": "tdright","sWidth":"10%" },
                        { "sClass": "tdright","sWidth":"10%" },
                        { "sClass": "center" ,"sWidth":"10%"},
                        { "sClass": "center" }
                    ],
                    "aoColumnDefs": [
                        {
                            "bUseRendered": false,
                            "fnRender": function(oObj) {
                                var colindex = oObj.iDataColumn;
                                if (colindex == 5) {
                                    var isTagClassDetail = 1 == oObj.aData[6] ? 1 : 0;
                                    return "<span style=\"text-decoration:underline;color:blue;display:block;\"  onclick=\"openwindow('"+oObj.aData[5]+"',"+isTagClassDetail+",'"+oObj.aData[0]+"')\" >" + "日期明细" + "</a>";
                                } else if (colindex == 6) {
                                    if (1==oObj.aData[6]) {
                                        //return "<span style=\"text-decoration:underline;color:blue;display:block;\"  onclick=\"openwindowbytag('"+oObj.aData[0]+"')\" >" + "专辑明细" + "</a>";
                                        return "<span style=\"text-decoration:underline;color:blue;display:block;\"  onclick=\"expand(this,'"+oObj.aData[0]+"','"+oObj.aData[5]+"')\" >" + "专辑明细展开" + "</a>";
                                    }else{
                                        return "";
                                    }
                                } else
                                    return oObj.aData[colindex];
                            },
                            "aTargets": [5, 6]
                        },
                        {
                            "bSortable": false,
                            "aTargets": [0, 1, 2, 3, 4, 5, 6]
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
                        aoData.push({ "name": "service", "value": "D_HttpDownStatDownPositionDistributionImpl" });
                        aoData.push({ "name": "do", "value": "get_detailtable" });
                        aoData.push({ "name": "endtime", "value": $("#endTime").val() });
                        aoData.push({ "name": "begintime", "value": $("#beginTime").val() });
                        aoData.push({ "name": "softs", "value": $("#selectsofts").val() });
                        aoData.push({ "name": "plat", "value": $("#selectplats").val() });
                        aoData.push({ "name": "restype", "value": $("#selectrestype").val() });
                        aoData.push({ "name": "version", "value": getVersionList() });
                        aoData.push({ "name": "projectsource", "value": $("#selectprojectsources").val() });
                        aoData.push({ "name": "period", "value": $("#hiddenperiod").val() });
                        aoData.push({ "name": "downtype", "value": $("#selectIsUpdate").val() });
                        aoData.push({ "name": "isdiffpagetype", "value": $("#selectdiffpagetype").val() });
                        aoData.push({ "name": "stattype", "value":  $('#selectstattype').val() });
                        aoData.push({ "name": "areaid", "value":  $('#selectareaid').val() });
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
                }); 
            }

            function getDetailTableTitle() {
                $("#mytags2").html("");
                createTabs($("#mytags2"), 0);
                gettabs();
            }
            ///设置下载
            function setDownExcel() {
                if (!checkDataRight()) { 
                    return false;
                }
                var url = serverUrl + "?do=get_excel&service=D_HttpDownStatDownPositionDistributionImpl&projectsource=" + $("#selectprojectsources").val() + "&softs=" + $("#selectsofts").val()
                    + "&plat=" + $("#selectplats").val() + "&begintime=" + $("#beginTime").val() + "&endtime=" + $("#endTime").val()
                    + "&restype=" + $("#selectrestype").val() + "&version=" + getVersionList() +"&downtype="+$("#selectIsUpdate").val()
                     + "&period=" + $("#hiddenperiod").val()+"&isdiffpagetype="+$("#selectdiffpagetype").val()+"&stattype="+ $('#selectstattype').val()+"&areaid="+ $('#selectareaid').val();
                $("#downexcel").attr("href", url);
            }

            String.prototype.trim = function() {
                return this.replace(/[(^\s*)|(\s*$)]/g, "");
            };
            
            ///----------------------------------------------第七列展开图处理JS Begin---------------------------------
            var nowOpen;
            function expand(e,pagename,pagetype) {
                var array = pagename.split("_");
                pagename = array[0];
                var varpagetype = pagetype.split("_")[0];
                var src = e; //.srcElement || e.target; // 获取触发事件的源对象
                var nTr = $(src).parent().parents('tr')[0];
                if (oTable.fnIsOpen(nTr)) {
                    /* This row is already open - close it */
                    $(src).html('专辑明细展开');
                    oTable.fnClose(nTr);
                }
                else {
                    $(src).html('专辑明细收缩');
                    oTable.fnOpen(nTr, fnFormatDetails(oTable, nTr), 'details');
                    getSubTagChart(nowOpen, pagename,varpagetype);
                    //$("#subtagstatdate").html("统计日期:"+$("#beginTime").val()+" ~ "+ $("#endTime").val());
                }
            }
           
            function fnFormatDetails(oTable, nTr) {
                var aData = oTable.fnGetData(nTr);
                nowOpen = aData[5];
                var a = [], j = 0;
                a[j++] = "<div class=\"title\" style=\"margin-top: 4px;\"><strong class=\"l\" id=\"Strongsubchart\">专辑饼图</strong>";
                a[j++] = "<span class=\"r\"> <font id=\"subtagstatdate\" style=\"margin-left: 35%;font-size:14px\" ></font> </span></div>";
                a[j++]="<div class=\"textbox\"><div id=\"container_"+nowOpen+"_Chart\" style=\"margin: auto; margin: 2px; height: 300px;\"></div></div>"
                a[j++] = "<div style=\"margin:4px;\"><div class=\"title\" style=\"margin-top:2px;\"><strong class=\"l\" id=\"topname_" + nowOpen + "\">专辑明细</strong>";
                a[j++] = "<span class=\"r\"></span></div><div class=\"textbox\">";
                a[j++] = "<div id=\"container_";
                a[j++] = nowOpen + "\">";
                a[j++] = "<div class=\"textbox\">";
                a[j++] = "<table  cellpadding=\"0\" cellspacing=\"0\" border=\"0\" class=\"display\">";
                a[j++] = "<thead><tr><th style=\"width: 120px\">位置</th><th>位置编号</th><th>统计量</th><th>所占比例</th><th>日均</th><th>明细</th>";
                a[j++] = "</tr></thead>";
                a[j++] = "<tbody> </tbody></table></div> ";
                a[j++] = "</div></div>";
                a[j++] = "</div>";
                var html = a.join("");
                return html;
            }
            
     
            function getSubTagChart(nowOpen,pagename,pagetype) {   
                $.ajax({
                    type: "get",
                    url: serverUrl,
                    dataType: "json",
                    data: {
                        'service': 'D_HttpDownStatDownPositionDistributionByTagImpl',
                        'do': 'get_chart',
                        'endtime': $("#endTime").val(),
                        'begintime': $("#beginTime").val(),
                        'softs': $("#selectsofts").val(),
                        'plat': $("#selectplats").val(),
                        'restype': $("#selectrestype").val(),
                        'version':getVersionList(),
                        'projectsource': $("#selectprojectsources").val(),
                        'period': $("#hiddenperiod").val(),
                        'downtype':$("#selectIsUpdate").val(),
                        'pagename': escape(pagename),
                        'pagetype': escape(pagetype),
                        'isdiffpagetype':$("#selectdiffpagetype").val(),
                        'stattype': $('#selectstattype').val(),
                        'areaid':$('#selectareaid').val()
                    },
                    success: function(data) {
                        if (data.resultCode == 0) {
                            $("#container_"+nowOpen+"_Chart").html("");
                            var obj = eval("(" + data.data + ")");
                            createPie("container_"+nowOpen+"_Chart", "下载位置分布按专辑", obj);
                            getSubTagDetailTable(nowOpen,pagename,pagetype);
                        } else {
                            alert(data.message);
                        }
                    }
                });
            }
            
            function openwindowbytagdetail(positionid) {
                var url = "projectsource=" + $("#selectprojectsources").val() + "&softs=" + $("#selectsofts").val() + "&positionid=" + positionid
                    + "&plat=" + $("#selectplats").val() + "&begintime=" + $("#beginTime").val() + "&endtime=" + $("#endTime").val()
                    + "&restype=" + $("#selectrestype").val() + "&version=" + getVersionList() + "&downtype=" + $("#selectIsUpdate").val()
                   + "&period=" + $("#hiddenperiod").val()+"&isdiffpagetype="+$("#selectdiffpagetype").val()
                    +"&stattype="+ $('#selectstattype').val()+"&areaid="+$("#selectareaid").val();
                $("#mywindow").attr("href", "D_ResDownLoadPositionDistributionByTagDetail.aspx?" + url);
                $("#mywindow").click();
            }

            function getSubTagDetailTable(nowOpen, pagename,pagetype) {
                var temp = $("#container_" + nowOpen+ " table").dataTable({
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
                        aoData.push({ "name": "softs", "value": $("#selectsofts").val() });
                        aoData.push({ "name": "plat", "value": $("#selectplats").val() });
                        aoData.push({ "name": "restype", "value": $("#selectrestype").val() });
                        aoData.push({ "name": "version", "value": getVersionList() });
                        aoData.push({ "name": "projectsource", "value": $("#selectprojectsources").val() });
                        aoData.push({ "name": "period", "value": $("#hiddenperiod").val() });
                        aoData.push({ "name": "downtype", "value": $("#selectIsUpdate").val() });
                        aoData.push({ "name": "pagename", "value": escape(pagename) });
                        aoData.push({ "name": "pagetype", "value": escape(pagetype) });
                        aoData.push({ "name": "isdiffpagetype", "value": $("#selectdiffpagetype").val() });
                        aoData.push({ "name": "stattype", "value": $('#selectstattype').val() });
                        aoData.push({ "name": "areaid", "value": $('#selectareaid').val() });
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
                });

                ///----------------------------------------------第七列展开图处理JS End---------------------------------
            }
        </script>

    </head>
    <body>

        <form id="form1" runat="server">
            <div class="maindiv">
                <!--只用于前台的标识-->
                <input type="hidden" value="" id="hiddenvalue" />
                <input type="hidden" value="" id="hiddenname" />
                <input type="hidden" value="1" id="hiddenperiod" />

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


                            <td  style="width: 120px">
                                    <select id="selectprojectsources" style="width: 150px;">
                                    </select>
                            </td>

                                                      <td style="width: 200px">
                                <div style="position: relative; z-index: 100">
                                    <select id="selectrestype" style="width: 150px;">
                                        <%= RestypeHtml %>
                                    </select>
                                </div>
                            </td>
                        </tr>
                        <tr>
                             <td  style="width: 120px">
                                <div style="position: relative; z-index: 100">
                                    <select id="selectIsUpdate" style="width: 150px;" >
                                            <option value="0" selected="selected">排除更新位置</option>
                                            <option value="1">所有位置</option>
                                    </select>
                                </div>
                            </td>
                            <td style="width: 120px;">
                                    <select id="selectdiffpagetype" style="width: 150px; ">
                                        <option value="0" selected="selected">不分页面类型</option>
                                        <option value="1" >区分页面类型</option>
                                    </select>
                            </td>
                             <td style="width: 120px;">
                                    <select id="selectstattype" style="width: 150px; ">
                                        <option value="1" selected="selected">点击下载</option>
                                        <option value="5" >安装成功</option>
                                        <option value="6" >安装失败</option>
                                        <option value="4" >下载成功</option>
                                        <option value="8" >下载失败</option>
                                    </select>
                            </td>
                             <td style="width: 120px">
                                    <select id="selectversion" style="width: 150px; " multiple="multiple">
                                        <option value="-1" selected="selected" >不区分版本</option>
                                    </select>
                            </td>
                        </tr>
                        <tr>
                                                        <td >从：
                                <input style="width: 135px" class="ui-multiselect ui-widget ui-state-default ui-corner-all" type="text" id="beginTime" class="Wdate" value="<%= BeginTime.ToString("yyyy-MM-dd") %>" onclick=" WdatePicker() " />
                            </td>
                            <td >到：
                             <input style="width: 135px"  class="ui-multiselect ui-widget ui-state-default ui-corner-all" type="text" id="endTime" class="Wdate" value="<%= EndTime.ToString("yyyy-MM-dd") %>" onclick=" WdatePicker() " />
                            </td>
                            <td  style="width: 120px">
                                <div class="selectdisplay" style="width: 100%">
                                     <select id="selectareaid" style="width: 150px; ">
                                        <option value="-1" selected="selected">不区分地区</option>
                                        <option value="EG" >埃及</option>
                                        <option value="SA" >沙特阿拉伯</option>
                                        <option value="TH" >泰国</option>
                                        <option value="IN" >印度</option>
                                        <option value="VN" >越南</option>
                                     </select>
                                </div>
                            </td>
                            <td ></td>
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
                    <strong class="l" id="Strong2">下载位置分布
                        <ul id="parenttags">
                        </ul>
                    </strong><span class="r"> </span>
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
                    <table id="mytable" cellpadding="0" width="100%" cellspacing="0" border="0"  class="display cell-border">
                        <thead>
                            <tr>
                                <th>
                                    位置
                                </th>
                                <th >
                                    位置编号
                                </th>
                                <th >
                                    统计量
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
                                <th >
                                    专辑明细
                                </th>
                            </tr>               
                        </thead>
                        <tbody>
                        </tbody>
                    </table>

                </div>

            </div>
        </form>
        
        <a id="mywindow" style="display: none"></a>
    </body>
</html>
<script type="text/javascript">
    function reSetSelectMode() {
        reSetVersion($("#selectsofts").val());
        reSetProject($("#selectsofts").val());
    }


    function reSetPlats(softid, defaultValue) {
        var myplats = softPlatJson[softid];
        var selectHtml = ""; //"<option value='0' selected='selected'>不区分平台</option>";
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
                    a[j++] = mydata2[i].Value;
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
    
    function funchiddenarea() {
        if (SoftAreaJson[$("#selectsofts").val()]==1) {
            $(".selectdisplay").hide();
        } else {
            $(".selectdisplay").show();
        }
    }

</script>

