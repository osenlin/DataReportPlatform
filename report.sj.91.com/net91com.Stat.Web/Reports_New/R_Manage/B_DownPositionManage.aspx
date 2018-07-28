<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="B_DownPositionManage.aspx.cs" Inherits="net91com.Stat.Web.Admin.B_DownPositionManage" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
    <head id="Head1" runat="server">
        <title>下载位置配置</title>
        <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
        <link href="../css/site.css?version=1" rel="stylesheet" type="text/css" />
        <link href="../css/defindecontrol.css?version=1" rel="stylesheet" type="text/css" />
        <link href="../css/intelligencesearch.css" rel="stylesheet" type="text/css" />
        <link href="../css/jquery.datatables.table.css" rel="stylesheet" type="text/css" />
        <link href="../css/colorbox.css" rel="stylesheet" type="text/css" />
        <link href="../css/jquery-ui.css" rel="stylesheet" type="text/css" />
        <link href="../css/jquery.multiselect.css" rel="stylesheet" type="text/css" />
        <link href="/css/headcss/jquery.multiselect.filter.css" rel="stylesheet" type="text/css" />
        <style type="text/css">
            .catetable
            {
                border-collapse: collapse;
                margin: 0 auto;
                width: 100%;
                height: 80%;
                border-left: 1px solid #cccccc;
                border-right: 1px solid #cccccc;
            }
            .catetable td
            {
                padding: 5px;
                border-style: solid;
                border-color: #cccccc;
                border-width: 1px 0;
            }
        </style>

        <script src="../js/jquery-1.6.4.min.js" type="text/javascript"> </script>
        <script src="../js/jquery.dataTables.min.js" type="text/javascript"> </script>
        <script src="../js/Defindecontrol.js?version=1" type="text/javascript"> </script>
        <script src="../js/intelligencesearch.js" type="text/javascript"> </script>
        <script src="../js/jquery.colorbox-min.js" type="text/javascript"> </script>
        <script src="../js/highcharts_old.js" type="text/javascript"> </script>
        <script src="../js/chartjs.js" type="text/javascript"> </script>
        <script src="../js/My97DatePicker/WdatePicker.js" type="text/javascript"> </script>
        <script src="/Scripts/HeadScript/jquery-ui.min.js" type="text/javascript"> </script>
        <script src="/Scripts/HeadScript/jquery.multiselect.js" type="text/javascript" charset="GBK"> </script>
        <script src="/Scripts/HeadScript/jquery.multiselect.filter.js" type="text/javascript" charset="GBK"> </script>
        <script src="../js/selectcontrols.js" type="text/javascript"> </script> 
        <script type="text/javascript">
            var serverUrl = '../HttpService.ashx';
            var oTable;
            var resContronl;
            var sourceControl;
            var pagetypeControl;
            var regionControl;
            //隐藏的弹出框
            var hiddenDialog;
            var downtypeControl;
            var projectjson = <%=projectdichtml%>;
            var projectjson_en = <%=projectdichtml_en%>;
            $(function() {
                
                regionControl = createSelectControl("selectregion", '请选择', false, function () {
                  
                    
                    if ($("#selectregion").val() == null) {
                        $('#selectdowntype').val(-1);
                        return false;
                    }
                    reSetProject($("#selectregion").val());
                    gettabs();
                    return true;
                }, function() {
                    return true;
                }, false,false,262,160);

                resContronl = createSelectControl("selectrestype", '请选择', false, function() {
                    if ($("#selectrestype").val() == null) {
                        alert("请选择一个资源类型");
                        return false;
                    }
                    return true;
                }, function() {
                    return true;
                }, true, true, 262, 160);

                sourceControl = createSelectControl("selectprojectsource", '请选择', false, function() {
                    if ($("#selectprojectsource").val() == null) {
                        alert("请选择一个平台");
                        return false;
                    }
                    return true;
                }, function() {
                    return true;
                }, true, true, 262, 160);
                pagetypeControl = createSelectControl("selectpagetype", 'false', false, function() {
                    if ($("#selectpagetype").val() == null) {
                        alert("请选择一个平台");
                        return false;
                    }
                    return true;
                }, function() {
                    return true;
                }, false, false, 262, 160);
                
                downtypeControl = createSelectControl("selectdowntype", 'false', false, function () {
                    return true;
                }, function () {
                    return true;
                }, false, false, 262, 160);
                gettabs();
                hiddenDialog = $("#mywindow").colorbox({ inline: true, width: "80%", height: "50%", speed: 0 });
                $("#batch_modify").colorbox({ inline: true, width: "80%", height: "80%", speed: 0 });
                
            });


            function loadData() {
                gettabs();
            }
                                
            function edit(positionid, restype_projectsource) {
                var arr = restype_projectsource.split('_');
                $.getJSON(serverUrl, { "service": "B_HttpDownPositionManage",
                    "do": "getpositionbypsp",
                    "projectsource": arr[1],
                    "position": positionid,
                    "region": $('#selectregion').val(),
                    "restype": arr[0]
                },
                function (data) {

                    if (data != null) {
                        //eval
                        var editdt = JSON.parse(data); //eval("("+data+")");
                        $("#hiddendiv_region").val(editdt.ProjectSourceType);
                        console.info($("#hiddendiv_region").val());
                        $("#hiddendiv_project").val(editdt.ProjectSource);
                        $("#hiddendiv_restype").val(editdt.ResType);
                        $("#hiddendiv_txtpositionid").val(editdt.Position);
                        $("#hiddendiv_txtpositionname").val(editdt.Name);
                        $("#hiddendiv_txtpagename").val(editdt.PageName);
                        $("#hiddendiv_txtpagetype").val(editdt.PageType);
                        $("#hiddendiv_tag")[0].checked = editdt.ByTag4MySql;

                        $("#hiddendiv_downtype").val(editdt.DownType);
                        $("#mywindow").attr("href", "#hiddendiv");
                        hiddenDialog.click();
                    }
                });
            }

            function save() {
                if ($("#hiddendiv_txtpositionname").val() == "") {
                    alert("请填写好位置名称");
                    return false;
                }
                $.getJSON(serverUrl, {
                    "service": "B_HttpDownPositionManage",
                    "do": "saveposition",
                    "region": $("#hiddendiv_region").val(),
                    "position": $("#hiddendiv_txtpositionid").val(),
                    "positionname": $("#hiddendiv_txtpositionname").val(),
                    "restype": $("#hiddendiv_restype").val(),
                    "projectsource": $("#hiddendiv_project").val(),
                    "pagename": $("#hiddendiv_txtpagename").val(),
                    "pagetype": escape($("#hiddendiv_txtpagetype").val()),
                    "downtype": $("#hiddendiv_downtype").val(),
                    "checkistag": $("#hiddendiv_tag")[0].checked ? 1 : 0
                }, function (data) {
                    if (data.resultCode == 0) {
                        alert(data.message);
                        $("#mywindow").colorbox.close();
                        gettabs();
                    } else {
                        alert(data.message);
                    }
                });
            }

            function batchEditName() {
                if ($("#id_name_list").val() == "") {
                    alert("请填写好位置名称");
                    return false;
                }
                $.getJSON(serverUrl, {
                    "service": "B_HttpDownPositionManage",
                    "do": "batcheditname",
                    "region": $("#selectregion").val(),
                    "restype": $("#selectrestype").val(),
                    "projectsource": $("#selectprojectsource").val(),
                    "pagetype": $("#selectpagetype").val(),
                    "idnamelist": $("#id_name_list").val()
                }, function (data) {
                    if (data.resultCode == 0) {
                        alert(data.message);
                        $("#batch_modify_dialog").colorbox.close();
                        gettabs();
                    } else {
                        alert(data.message);
                    }
                });
            }
            function add() {
                    if ($("#txtPositionID").val() == "") {
                        alert("请填写好位置ID");
                        return false;
                    }
                    if ($("#txtPositionName").val() == "") {
                        alert("请填写好位置名称");
                        return false;
                    }
                    if ($("#selectpagetype").val() == "不区分页面类型") {
                        alert("页面类型不能为：不区分页面类型，请重新选择");
                        return false;
                    }
                    if ($("#selectdowntype").val() == -1) {
                        alert("下载位置类型不能为：不区分下载位置，请重新选择");
                        return false;
                    }
                   $.getJSON(serverUrl, {
                        "service": "B_HttpDownPositionManage",
                        "do": "addposition",
                        "region":$("#selectregion").val(),
                        "position": $("#txtPositionID").val(),
                        "positionname": $("#txtPositionName").val(),
                        "restype": $("#selectrestype").val(),
                        "projectsource":  $("#selectprojectsource").val(),
                        "pagename": $("#txtPageName").val(),
                        "pagetype": escape($("#selectpagetype").val()),
                        "downtype": $("#selectdowntype").val(),
                        "checkistag": $("#checkIsTag")[0].checked ? 1 : 0 
   
                    }, function(data) {                      
                        if (data.resultCode == 0) {
                            alert(data.message);
                            gettabs($("#selectregion").val());
                        } else {
                            alert(data.message);
                        }
                    });
            }

            function gettabs() {
                if (oTable) {
                    oTable.fnDraw();
                }
                oTable = $('#table1').dataTable({
                    "sDom": '<"top">rt<"bottom"ip><"clear">',
                    "bPaginate": true,
                    "iDisplayLength": 50,
                    "aoColumns": [
                        { "sClass": "tdright" },
                        { "sClass": "tdright" },
                        { "sClass": "tdleft" },
                        { "sClass": "tdright" },
                        { "sClass": "center" },
                        { "sClass": "center" },
                        { "sClass": "center" }
                    ],
                    "aoColumnDefs": [
                        {
                            "aTargets": [0, 1, 2, 3, 4, 5, 6]
                        },
                        {
                            "bUseRendered": false,
                            "fnRender": function(oObj) {
                                var colindex = oObj.iDataColumn;
                                if (colindex == 6) {
                                    return "<span style=\"text-decoration:underline;color:blue;display:block;\"  onclick=\"" + "edit('" + oObj.aData[0] + "','" + oObj.aData[6] + "')\" >" + "编辑" + "</a>";
                                } else
                                    return oObj.aData[colindex];
                            },
                            "aTargets": [6]
                        },
                        {
                            "bSortable": false,
                            "aTargets": [0, 1, 2, 3, 4, 5, 6]
                        }
                    ],
                   // "bLengthChange":true,
                    "aaSorting": [[0, 'asc']],
                    "bProcessing": true,
                    "bServerSide": true,
                    "oLanguage": { sUrl: "../js/de_DE.txt" },
                    "bFilter": false,
                    "bSortable": true,
                    "sPaginationType": 'full_numbers',
                    "sAjaxSource": serverUrl+"?rand="+Math.random(),
                    "bDestroy": false,
                    "bRetrieve": true,
                    "fnServerParams": function(aoData) {
                        aoData.push({ "name": "service", "value": "B_HttpDownPositionManage" });
                        aoData.push({ "name": "do", "value": "get_page" });
                        aoData.push({ "name": "region", "value": $("#selectregion").val() });
                        aoData.push({ "name": "restype", "value": $("#selectrestype").val() });
                        aoData.push({ "name": "projectsource", "value": $("#selectprojectsource").val() });
                        aoData.push({ "name": "position", "value": $("#txtPositionID").val() });
                        aoData.push({ "name": "positionname", "value": $("#txtPositionName").val() });
                        aoData.push({ "name": "pagetype", "value": escape($("#selectpagetype").val()) });
                        aoData.push({ "name": "pagename", "value": $("#txtPageName").val() });
                        aoData.push({ "name": "downtype", "value": $("#selectdowntype").val() });
                        aoData.push({ "name": "checkistag", "value": $("#checkIsTag")[0].checked ? 1 : 0 });

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
                        tableStyleSet();
                    }
                });
            }

            function tableStyleSet() {
                $("#table1 tr").mouseover(function() {
                    $(this).attr("style", "background:none repeat scroll 0 0 #73A3CC;cursor:pointer");
                });
                $("#table1 tr").mouseout(function() {
                    $(this).attr("style", "");
                });
            }
            
            function reSetProject(areaid) {
                var myprojects = "";
                if (areaid == 1) {
                    myprojects = projectjson;
                } else {
                    myprojects = projectjson_en;
                }
                var selectHtml = "";
                if (myprojects != undefined) {
                    for (var i in myprojects) {
                        selectHtml += "<option value='" + i + "'";
                        selectHtml += ">";
                        selectHtml += myprojects[i];
                        selectHtml += "</option>";
                    }

                }
                $("#selectprojectsource").html(selectHtml);
                if (sourceControl) {
                    sourceControl.refresh();
                }
            }

        </script>
    </head>
    <body> 
        <form id="form1" runat="server"> 
            <div class="maindiv">
                <div style="padding: 10px;">
                    <table cellpadding="0" cellspacing="10" border="0" width="100%">
                        <tr>
                            <td>
                                <div style="position: relative; z-index: 100">
                                    <select id="selectregion" onchange=" " style="width: 150px;">
                                        <option value="1" selected="selected">国内</option>
                                        <option value="2" >海外</option>
                                    </select>
                                </div>
                            </td>
                            <td>
                                <div style="position: relative; z-index: 100">
                                    <select id="selectrestype" onchange=" " style="width: 150px;">
                                        <%= restypehtml %>
                                    </select>
                                </div>
                            </td>
                            <td>
                                <div style="position: relative; z-index: 100">
                                    <select id="selectprojectsource" style="width: 300px;">
                                        <%= projecthtml %>
                                    </select>
                                </div>
                            </td>
      
                            <td>
                                <select id="selectpagetype" >
                                    <option value="不区分页面类型" selected="selected" >不区分页面类型</option>
                                    <option value="列表下载">列表下载</option>
                                    <option  value="详细页下载">详细页下载</option>
                                </select></td>
                        </tr>
                      
                        <tr>
                            <td>
                                <input class="ui-widget ui-state-default ui-corner-all" placeholder="位置" type="text" onkeyup=" this.value = this.value.replace(/[^,0123456789]/g, '') "  value="" id="txtPositionID" />
                               </td>
                            <td >
                                <input class="ui-widget ui-state-default ui-corner-all" placeholder="位置名称" type="text"  value="" id="txtPositionName" /></td>

                            <td>
                                <input class="ui-widget ui-state-default ui-corner-all" placeholder="页面名称" type="text"  value="" id="txtPageName" /></td>
                            <td></td>
                        </tr>
                        <tr>
                            <td>
                                <select id="selectdowntype" >
                                    <option selected="selected" value="-1">不区分下载位置</option>
                                    <option  value="1">普通下载位置</option>
                                    <option  value="2">更新下载位置</option>
                                    <option  value="3">搜索下载位置</option>
                                    <option  value="4">静默更新位置</option>
                                </select>
                            </td>
                            <td colspan="2">
                                <input style="color: #1c94c4"type="checkbox"  id="checkIsTag"/><b style="color:#1c94c4">是否专辑</b></td>
                                                        <td colspan="2"></td>
                            <td >
                                <span style="cursor: pointer; float: right; margin-right: 20px;"><a class="mybutton hover"
                                                                                                    style="margin-top: 4px; overflow: hidden;" onclick=" loadData(); "><font>查询</font>
                                                                                                 </a></span>
                                <span style="cursor: pointer; float: right; margin-right: 20px;"><a class="mybutton hover"
                                                                                                    style="margin-top: 4px; overflow: hidden;" onclick="add()"><font>添加</font>
                                                                                                 </a></span>
                                <span style="cursor: pointer; float: right; margin-right: 20px;">
                                    <a class="mybutton hover" style="margin-top: 4px; overflow: hidden;" id="batch_modify" href="#batch_modify_dialog">
                                        <font>批量修改名称</font>
                                    </a>
                                </span>
                            </td>

                        </tr>
                    </table>
                </div>

                <div class="title" style="margin-top: 4px;">
                    <strong class="l" id="Strong1">位置管理</strong>
                </div>
                <div class="textbox">
                    <table id="table1" cellpadding="0" cellspacing="0" border="0" class="display">
                        <thead>
                            <tr>
                                <th style="width: 10%">
                                    位置ID
                                </th>
                                <th style="width: 20%">
                                    位置名称
                                </th>
                                <th style="width: 20%">
                                    页面名称
                                </th>
                                <th style="width: 10%" >
                                    页面类型
                                </th>
                                <th style="width: 15%">
                                    是否为专辑
                                </th>
                                <th style="width: 15%">
                                    位置类型
                                </th>
                                <th style="width: 10%">
                                    编辑
                                </th>
                            </tr>
                        </thead>
                        <tbody>
                        </tbody>
                    </table>
             
                </div>
    
            </div>
            <div id='popDiv' style="display: none" />
    
    
            <a id="mywindow" style="display: none"></a>
            <div style="display: none; padding: 5px">
                <div id="hiddendiv">
                         <input type="hidden"  id="hiddendiv_region" value="" />
                         <input type="hidden"  id="hiddendiv_project" value="" />
                         <input type="hidden"  id="hiddendiv_restype" value="" />
                        <table id="titletable"  style="width: 100%;border: 10;" class="catetable" >
                            <tr>
                                <td style="width: 30%;text-align: right">
                                    位置ID:
                                </td>
                                <td>
                                    <input type="txtbox"  disabled="true" id="hiddendiv_txtpositionid" value="" />
                                </td>
                            </tr>
                            <tr>
                                <td style="width: 30%;text-align: right">
                                    位置名称:
                                </td>
                                <td>
                                    <input type="txtbox" id="hiddendiv_txtpositionname" value="" />
                                </td>
                            </tr>
                            <tr>
                                <td style="width: 30%;text-align: right">
                                    页面名称:
                                </td>
                                <td>
                                    <input type="txtbox" id="hiddendiv_txtpagename" value="" />
                                </td> 
                            </tr>
                            <tr>
                                <td style="width: 30%;text-align: right">
                                    页面类型:
                                </td>
                                <td>
                                    <select id="hiddendiv_txtpagetype" >

                                        <option value="详细页下载">详细页下载</option>
                                        <option value="列表下载"  >列表下载</option>
                                    </select>
                                </td> 
                            </tr>
                            <tr>
                                <td style="width: 30%;text-align: right">
                                    是否专辑
                                </td>
                                <td>
                                    <input type="checkbox" checked="" id="hiddendiv_tag"/>
                                </td> 
                            </tr>
                            <tr>
                                <td style="width: 30%;text-align: right">
                                    位置类型
                                </td>
                                <td>
                                     <select id="hiddendiv_downtype" >
                                        <option value="1" selected="selected" >普通下载位置</option>
                                        <option value="2">更新下载位置</option>
                                        <option value="3">搜索下载位置</option>
                                        <option value="4">静默更新位置</option>
                                    </select>
                                </td> 
                            </tr>
                            <tr>
                                <td colspan="2" >
                                    <a class="mybutton hover" style="cursor: pointer; margin-right: 5px; float: right;" onclick="save() ">
                                        <font>保存</font></a> 
                                </td>
                            </tr>
                        </table>
                    </div>
                     <div id="batch_modify_dialog" style="height:100%;">
                        <table style="width:100%; height: 100%; border:1px;">
                            <tr style="height:80%;">
                                <td>
                                    <textarea name="id_name_list" id="id_name_list" style="width:98%; height:100%;"></textarea>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <b>格式说明：位置ID,位置名称,使用逗号（,）进行分隔，每行一条记录。 </b>
                                    <a class="mybutton hover" style="cursor: pointer; margin-right: 5px; float: right;" onclick="batchEditName() ">
                                        <font>保存</font></a> 
                                </td>
                            </tr>
                        </table>
                    </div>
                </div> 
        </form>
    </body>
</html>