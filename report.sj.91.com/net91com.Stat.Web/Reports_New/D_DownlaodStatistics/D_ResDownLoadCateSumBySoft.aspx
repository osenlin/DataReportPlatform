<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="D_ResDownLoadCateSumBySoft.aspx.cs" Inherits="net91com.Stat.Web.Reports_New.D_DownlaodStatistics.D_ResDownLoadCateSumBySoft" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
    <head id="Head1" runat="server">
        <title>下载统计（按分类）</title>
        <link href="/Reports_New/css/site.css?version=1" rel="stylesheet" type="text/css" />
        <link href="/Reports_New/css/jquery-ui.css" rel="stylesheet" type="text/css" />
        <link href="../css/jquery.datatables.table.css" rel="stylesheet" type="text/css" />
        <link href="/Reports_New/css/jquery.multiselect.css" rel="stylesheet" type="text/css" />
        <link href="/css/headcss/jquery.multiselect.filter.css" rel="stylesheet" type="text/css" />
        <link href="/Reports_New/css/defindecontrol.css" rel="stylesheet" type="text/css" />
        <script src="/Reports_New/js/jquery-1.6.4.min.js" type="text/javascript"></script>
        <script src="/Scripts/HeadScript/jquery-ui.min.js" type="text/javascript"></script>
        <script src="/Scripts/HeadScript/jquery.multiselect.js" type="text/javascript" charset="GBK"></script>
        <script src="/Scripts/HeadScript/jquery.multiselect.filter.js" type="text/javascript" charset="GBK"></script>
        <script src="../js/Defindecontrol.js" type="text/javascript"></script>
        <script src="../js/jquery.dataTables.min.js" type="text/javascript"></script>
        <script src="/Reports_New/js/selectcontrols.js" type="text/javascript"></script>
        <script src="../js/My97DatePicker/WdatePicker.js" type="text/javascript"></script>

        <script src="../js/highcharts_old.js" type="text/javascript"></script>
        <script src="../js/chartjs.js" type="text/javascript"></script> 
        <script type="text/javascript">
            var tableHtml_46 = "<tr><th rowspan=\"2\" style=\"width: 120px\">日期</th> <th colspan=\"3\" >下载点击</th> <th colspan=\"2\" >下载成功</th> <th colspan=\"2\" >下载失败</th> <th colspan=\"2\">安装成功</th> <th colspan=\"2\">安装失败</th></tr><tr><th>所有</th><th>去除更新</th>";
            tableHtml_46 += "<th>来自搜索</th><th >所有</th><th >去除更新</th><th >所有</th><th >去除更新</th>";
            tableHtml_46 += "<th >所有</th><th >去除更新</th><th >所有</th><th >去除更新</th></tr>";

            var softControl;
            var platControl;
            var restypeControl;
            var softPlatJson = <%= SoftPlatformJson %>;
            var serverUrl = '../HttpService.ashx';
            var SoftAreaJson = <%= SoftAreaJson %>;
            var parentCateControl;
            var subCateControl;
            var downTypeControl;
            $(function() {
                createTabs($("#parenttags"), 0);
                softControl = createSelectControl("selectsofts", '请选择软件', false, function() {
                    if ($("#selectsofts").val() == null) {
                        alert("请选择一个软件");
                        return false;
                    }
                    //reSetPlats($("#selectsofts").val());
                    getbigcate();
                    clearSelect();
                    return true;
                }, function() {
                    return true;
                }, false,false);
                reSetPlats($("#selectsofts").val(),4);
                getbigcate();
                platControl = createSelectControl("selectplats", '请选择平台', true, function() {
                    if ($("#selectplats").val() == null) {
                        alert("请选择一个平台");
                        return false;
                    }
                    return true;
                }, function() {
                    return true;
                }, true);

                restypeControl = createSelectControl("selectrestype", '请选择', false, function() {
                    if ($("#selectrestype").val() == null) {
                        alert("请选择一个资源类型");
                        return false;
                    }
                    getbigcate();
                    return true;
                }, function() {
                    return true;
                }, true);

                parentCateControl = createSelectControl("selectparentcate", '请选择', true, function() {
                    if ($("#selectrestype").val() == null) {
                        alert("请选择一个资源类型");
                        return false;
                    }
                    if ($("#selectparentcate").val() == null) {
                        alert("请选择一个大分类");
                        return false;
                    }
                    getsmallcate();

                    return true;
                }, function() {
                    return true;
                }, true);

                subCateControl = createSelectControl("selectsoncate", '请选择', false, function() {
                    if ($("#selectparentcate").val() == null) {
                        alert("请先选择一个大分类");
                        return false;
                    }
                    if ($("#selectsoncate").val() == null) {
                        alert("请选择一个小分类");
                        return false;
                    }
                    return true;
                }, function() {
                    return true;
                }, true);
                downTypeControl=createSelectControl("selectDownType", '请选择', false, function() {
                    if ($("#selectDownType").val() == null) {
                        alert("请选择一个下载类型");
                        return false;
                    }
                    return true;
                }, function() {
                    return true;
                }, false,false);
                getChart();
                
                $(window).bind('resize', function() {
                    oTable.fnAdjustColumnSizing();
                });
            });


            var mytabs;

            function getperiod(period) {
                $("#hiddenperiod").val(period);
                if (!checkDataRight()) { 
                    return false;
                }
                getChart();
            }
            
            function getVlaueByID(controlid) {
                var cvalue = $("#"+controlid).val();
                var str = "";
                for (var i = 0; i < cvalue.length; i++) {
                    str += cvalue[i] + ',';
                }
                str = str.substring(0, str.length - 1);
                return str;
            }

            var oTable;
            
            function getChart() {
                var str_plat = getVlaueByID("selectplats");
                var str_pcid = getVlaueByID("selectparentcate");
                $.ajax({
                    type: "get",
                    url: serverUrl,
                    dataType: "json",
                    data: {
                        'service': 'D_HttpDownStatDownCountCateSumBySoftImpl',
                        'do': 'get_chart',
                        'endtime': $("#endTime").val(),
                        'begintime': $("#beginTime").val(),
                        'softs': $("#selectsofts").val(),
                        'plat': str_plat,
                        'restype': $("#selectrestype").val(),
                        'pcid': str_pcid,
                        'cid': $("#selectsoncate").val(),
                        'downtype': $("#selectDownType").val(),
                        'period': $("#hiddenperiod").val()
                    },
                    success: function(data) {
                        if (data.resultCode == 0) {
                            $("#container").html("");
                            var yname = "量";
                            var obj = eval("(" + data.data + ")");
                            //var obj = JSON.parse(data.data);
                            createLine("container", 400, obj.title, yname, true, 0, obj.x, obj.y);
                            CreateDetailTableTitle();
                        } else {
                            alert(data.message);
                        }
                    }
                });
            }

            function CreateDetailTableTitle() {
                var arr_plat = getConditionArray("selectplats");
                var arr_pcid = getConditionArray("selectparentcate");
                var html = "";
                for (var i = 0; i < arr_plat.length; i++) {
                    for (var j = 0; j < arr_pcid.length; j++) {
                        html += " <li><a onclick=\"getDetailTableTitle('" + arr_plat[i].id + "','" + arr_pcid[j].id + "')\"><font>" + arr_plat[i].value+"_"+arr_pcid[j].value + "</font></a></li>";
                    }
                }
                $("#mytags2").html(html);
                var detailTable = createTabs($("#mytags2"), 0);
                detailTable.click(0);
                if (arr_plat.length>1||arr_pcid.length>1) {
                    $("#mytags2").show();
                } else {
                    $("#mytags2").hide();
                }
            }
            
            function getDetailTableTitle(var_plat,var_pcid) {
                $('#hiddenplat').val(var_plat);
                $('#hiddenpcid').val(var_pcid);
                if (oTable) {
                    oTable.fnDraw();
                    return;
                }
                $("#mytable thead").html(tableHtml_46);
                oTable = $('#mytable').dataTable({
                    "sDom": '<"top">rt<"bottom"fip><"clear">',
                    "bPaginate": false,
                    "aoColumns": [
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
                        { "sClass": "tdright" },
                        { "sClass": "tdright" }
                    ],
                    "aoColumnDefs": [
                        {
                            "bSortable": false,
                            "aTargets": [0, 1, 2, 3, 4,5,6,7,8,9,10,11]
                        }
                    ],
                    "bProcessing": true,
                    "bServerSide": true,
                    "iDisplayLength": 30,
                    "oLanguage": { sUrl: "../js/de_DE.txt" },
                    "bFilter": false,
                    "sPaginationType": "full_numbers",
                    "sAjaxSource": serverUrl,
                    "fnServerParams": function(aoData) {
                        aoData.push({ "name": "service", "value": "D_HttpDownStatDownCountCateSumBySoftImpl" });
                        aoData.push({ "name": "do", "value": "get_detailtable" });
                        aoData.push({ "name": "begintime", "value": $("#beginTime").val() });
                        aoData.push({ "name": "endtime", "value": $("#endTime").val() });
                        aoData.push({ "name": "softs", "value": $("#selectsofts").val() });
                        aoData.push({ "name": "plat", "value": $('#hiddenplat').val() });
                        aoData.push({ "name": "restype", "value": $("#selectrestype").val() });
                        aoData.push({ "name": "period", "value": $("#hiddenperiod").val() });
                        aoData.push({ "name": "pcid", "value": $('#hiddenpcid').val() });
                        aoData.push({ "name": "cid", "value": $("#selectsoncate").val() });
                        aoData.push({ "name": "downtype", "value": $("#selectDownType").val()  });
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
                });
            }

            function loadData() {
                if (!checkDataRight()) { 
                    return false;
                }
                getChart();
            }            


            function setDownExcel() {
                if (!checkDataRight()) { 
                    return false;
                }
                var url = serverUrl + "?do=get_excel&service=D_HttpDownStatDownCountCateSumBySoftImpl&softs=" + $("#selectsofts").val()
                    + "&plat=" + $("#selectplats").val() + "&begintime=" + $("#beginTime").val() +"&endtime="+ $("#endTime").val()
                    + "&restype=" + $("#selectrestype").val() 
                    + "&period=" + $("#hiddenperiod").val()+"&pcid="+$("#selectparentcate").val() +"&cid="+ $("#selectsoncate").val()+"&downtype="+$("#selectDownType").val();
                $("#downexcel").attr("href", url);
            }

        </script>
   
    </head>
    <body>

        <form id="form1" runat="server">
            <div class="maindiv">
                <input type="hidden" value="" id="hiddenvalue"/>
                <input type="hidden" value="" id="hiddenplat"/>
                <input type="hidden" value="" id="hiddenpcid"/>
                <input type="hidden" value="" id="hiddenname"/>
                <input type="hidden" value="1" id="hiddenperiod"/>
     
                <div style="padding: 10px;">
                    <table id="table1" cellpadding="0" cellspacing="10" border="0" width="100%">
                        <tr>
                    
                            <td  style="width: 200px">
                                <div style="position: relative; z-index: 100">
                                    <select id="selectsofts" style="width: 150px;"  >
                                        <%= SoftHtml %>
                                    </select>
                                </div>
                            </td>
                    
                            <td    style="width: 200px" >
                                <div style="position: relative; z-index: 100">
                                    <select id="selectplats" style="width: 150px;"   multiple="multiple" >
                                        <option value='0' >不区分平台</option>
                                        <%= PlatHtml %>
                                    </select>
                                </div>
                            </td>
                                                <td  style="width: 200px" >
                                <div style="position: relative; z-index: 100">
                                    <select id="selectrestype" style="width: 150px;"  >
                                       
                                        <%= RestypeHtml %>
                                    </select>
                                </div>
                            </td>
                            <td>
                                <div style="position: relative; z-index: 100">
                                    <select id="selectDownType" style="width: 150px;"  >
                                       
                                        <option value='-1' selected='selected'>不区分下载类型</option>
   
                                        <option value='3' >搜索下载</option>
        
                                        <option value='5' >去除更新下载</option>
                                    </select>
                                </div>
                            </td>

                        </tr>
                        <tr>
                            <td>
                                从：<input type="text" style="width:140px" class="ui-multiselect ui-widget ui-state-default ui-corner-all"  id="beginTime" class="Wdate" value="<%= BeginTime.ToString("yyyy-MM-dd") %>" onclick=" WdatePicker() " />
                            </td>
                            <td>
                                到：<input type="text" style="width:140px"  class="ui-multiselect ui-widget ui-state-default ui-corner-all"  id="endTime" class="Wdate" value="<%= EndTime.ToString("yyyy-MM-dd") %>" onclick=" WdatePicker() " />
                            </td>
                            <td>
                                <div style="position: relative; z-index: 100">
                                    <select id="selectparentcate" style="width: 150px;"  multiple="multiple" >
                                        <%=ParentCategoryHtml %>
                                    </select>
                                </div>

                            </td>
                            <td >
                                <div style="position: relative; z-index: 100">
                                    <select id="selectsoncate" style="width: 150px;"  >
                                        <option value='0'  selected='selected' >不区分小分类</option>
                                    </select>
                                </div>
                            </td>
                            <td >
                                <span style="cursor: pointer; float: right; margin-right: 10px;"><a class="mybutton hover"
                                                                                                    style="margin-top: 4px; overflow: hidden;" onclick=" loadData(); "><font>查询</font>
                                                                                                 </a></span>
                            </td>
                        </tr>
                    </table>
                </div>
                <div style="clear: both">
                </div>
                <div class="title" style="margin-top: 4px;">
                    <strong class="l" id="Strong1">
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
                    <strong class="l" id="Strong2">数据明细
                    </strong> 
                    <span class="r">
                        <a class="mybutton hover"
                           id="downexcel" href="javascript:setDownExcel();"><font>导出Excel</font> </a>
                    </span>
                    <ul id="mytags2">
                    </ul>
                </div>
                <div class="textbox">
                    <table id="mytable" cellpadding="0" cellspacing="0" border="0" width="99%"  class="display">
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
        var selectHtml ="<option value='0' >不区分平台</option>";
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
    
    //资源类型获取大分类
    function getbigcate() {
        var softareatype = 1;
        if ($('#selectsofts').val()==-2) {
            softareatype = 1;
        }else if ($('#selectsofts').val()==-46) {
            softareatype = 2;
        }
        $.getJSON(serverUrl,
            {
                'service': 'UtilityService',
                'do': 'getdownloadbigcatenew',
                'type':softareatype,
                'restype': $("#selectrestype").val()
            },
            function (data2) {
                if (data2.resultCode == 0) {
                    var mydata2 = data2.data;
                    var a = [], j = 0;
                    if (mydata2[0].ID!=-1) {
                        a[j++] = "<option value='0'  selected='selected'>不区分大分类</option>";
                    }
                    for (var i = 0; i < mydata2.length; i++) {
                       
                        a[j++] = "<option value='";
                        a[j++] = mydata2[i].ID + "' ";
                        a[j++] = ">";
                        a[j++] = mydata2[i].Value;
                        a[j++] = "</option>";
                    }
                    var html = a.join("");
                    $("#selectparentcate").html(html);
                    if (parentCateControl) {
                        parentCateControl.refresh();
                    }
                    getsmallcate();
                } else {
                    alert(data2.message);
                }
            });
    }
    
    function getsmallcate() {
        
        if ($("#selectparentcate").find("option:selected").length > 1) {
            $("#selectsoncate").val(0);
            $("#selectsoncate").html("<option value='0'  selected='selected' >不区分小分类</option>");
            if (subCateControl) {
                subCateControl.refresh();
            }
            return;
        } 
        var softareatype = 1;
        if ($('#selectsofts').val()==-2) {
            softareatype = 1;
        }else if ($('#selectsofts').val()==-46) {
            softareatype = 2;
        }
        
        var parenttype = getVlaueByID("selectparentcate");
        if (parenttype == "0") {
            $("#selectsoncate").html("<option value='0'  selected='selected' >不区分小分类</option>");
            if (subCateControl) {
                subCateControl.refresh();
            }
        } else {
            $.getJSON(serverUrl,
                {
                    'service': 'UtilityService',
                    'do': 'getdownloadsmallcatenew',
                    'type': softareatype,
                    'restype': $("#selectrestype").val(),
                    'bigcateid': parenttype
                },
                function(data) {
                    if (data.resultCode == 0) {
                        var mydata2 = data.data;
                        var html = "";
                        if (mydata2[0].ID!=-1) {
                            html = "<option value='0'  selected='selected'>不区分小分类</option>";
                        }
                        for (var i = 0; i < mydata2.length; i++) {
                            html+= "<option value='"+ mydata2[i].ID + "'> "+mydata2[i].Value+ "</option>";
                        }         
                        $("#selectsoncate").html(html);
                        if (subCateControl) {
                            subCateControl.refresh();
                        }
                    } else {
                        alert(data.message);
                    }
                });
        }
    }

    function clearSelect() {
        $("#selectrestype").val("1");
        $("#selectparentcate").val("0");
        $("#selectsoncate").val("0");
        if (restypeControl)
            restypeControl.refresh();
        if (parentCateControl)
            parentCateControl.refresh();
        if (subCateControl)
            subCateControl.refresh();
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
