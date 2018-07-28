<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="D_ResDownLoadRankByAuthorID.aspx.cs" Inherits="net91com.Stat.Web.Reports_New.D_DownlaodStatistics.D_ResDownLoadRankByAuthorID" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
    <head id="Head1" runat="server">
        <title>下载排行按区域</title>
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
        <script type="text/javascript">
            var softControl;
            var platControl;
            var versionControl;
            var projectControl;
            var restypeControl;
            var softPlatJson = <%= SoftPlatformJson %>;
            var serverUrl = '../HttpService.ashx';

            var ExtendAttributeLstControl;
            var g_areatype;
            var downTypeControl;
            $(function() {
                createTabs($("#parenttags"), 0);
                softControl = createSelectControl("selectsofts", '请选择软件', false, function() {
                    if ($("#selectsofts").val() == null) {
                        alert("请选择一个软件");
                        return false;
                    }
                    reSetPlats($("#selectsofts").val());
                    return true;
                }, function() {
                    return true;
                }, true);
                reSetPlats($("#selectsofts").val());
                platControl = createSelectControl("selectplats", '请选择平台', false, function() {
                    if ($("#selectplats").val() == null) {
                        alert("请选择一个平台");
                        return false;
                    }
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

          

                ExtendAttributeLstControl = createSelectControl("selectExtendAttrLst", '请选择', false, function() {
                    if ($("#selectExtendAttrLst").val() == null) {
                        alert("请选择一个扩展资源类型");
                        return false;
                    }
                    return true;
                }, function() {
                    return true;
                }, true);
             
                gettabs();
            });
            var mytabs;

            function getperiod(period) {
                $("#hiddenperiod").val(period);
                gettabs();
            }

            var oTable;
            
            function gettabs() {
                if (oTable) {
                    oTable.fnDraw();
                    return;
                }
                $("#loaddiv").html("<div  style='text-align:center;'><img height='15px' src='/Reports_New/images/common/defaultloading.gif' /><div>加载中..</div></div>");
                oTable = $('#mytable').dataTable({
                    "sDom": '<"top">rt<"bottom"fip><"clear">',
                    "bPaginate": false,
                    "aoColumns": [
                        { "sClass": "tdright"},
                        { "sClass": "tdright"},
                        { "sClass": "tdleft" },
                        { "sClass": "tdleft" },
                        { "sClass": "tdright"},
                        { "sClass": "tdright"}
                    ],
                    "aoColumnDefs": [
                        {
                            "bSortable": false,
                            "aTargets": [0, 1, 2, 3, 4,5]
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
                        aoData.push({ "name": "service", "value": "D_HttpDownStatDownRankByAuthorIDImpl" });
                        aoData.push({ "name": "do", "value": "get_detailtable" });
                        aoData.push({ "name": "begintime", "value": $("#beginTime").val() });
                        aoData.push({ "name": "softs", "value": $("#selectsofts").val() });
                        aoData.push({ "name": "plat", "value": $("#selectplats").val() });
                        aoData.push({ "name": "restype", "value": 2 });
                        aoData.push({ "name": "period", "value": $("#hiddenperiod").val() });
                    },
                    "fnServerData": function(sSource, aoData, fnCallback) {
                        $.ajax({
                            "dataType": 'json',
                            "type": "POST",
                            "url": sSource,
                            "data": aoData,
                            "success": function(data) {
                                var str = "统计时间:" + data.begintime + " ~ " + data.endtime;
                                $('#statdate').html(str);
                                //存在输出的resultCode 让他输出message
                                if (data.resultCode && data.resultCode != 0) {
                                    alert(data.message);
                                } else {
                                    fnCallback(data);
                                }
                                $("#loaddiv").html("");
                            }
                        });
                    },
                    "fnDrawCallback": function() {
                        tableStyleSet("mytable");
                        setDownExcel();
                    }
                });
            }

            function getareas() {
                var areas = $("#selectArea").val();
                var str = "";
                for (var i = 0; i < areas.length; i++) {
                    str += areas[i] + ',';
                }
                str = str.substring(0, str.length - 1);
                return str;
            }
            
            function loadData() {
                gettabs();
            }            

            function setDownExcel() {
                var url = serverUrl + "?do=get_excel&service=D_HttpDownStatDownRankByAuthorIDImpl&softs=" + $("#selectsofts").val()
                    + "&plat=" + $("#selectplats").val() + "&begintime=" + $("#beginTime").val()
                    + "&restype=2&period=" + $("#hiddenperiod").val();
                $("#downexcel").attr("href", url);
            }
        </script>
   
    </head>
    <body>

        <form id="form1" runat="server">
            <div class="maindiv">
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
                                    <select id="selectplats" style="width: 150px;"  >
                                        <%= PlatHtml %>
                                    </select>
                                </div>
                            </td>
                            <td>
                                <input type="text"  class="ui-multiselect ui-widget ui-state-default ui-corner-all"  id="beginTime" class="Wdate" value="<%= BeginTime.ToString("yyyy-MM-dd") %>" onclick=" WdatePicker() " />
                            </td>
                                                        <td>
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
                    <strong class="l" id="Strong2">
                        <ul id="parenttags">
                            <li><a onclick=" javascript:getperiod(1) "><font>日</font></a></li> 
                            <li><a onclick=" javascript:getperiod(3) " title="统计上周数据"><font>周</font></a></li>
                            <li><a onclick=" javascript:getperiod(12) " title="统计上月数据"><font>月</font></a></li>
                        </ul>
                        
                    </strong> 
                   <span id="statdate" style="font-size:14px; padding-top: 10px;float: left;color: #63666D;" > </span>
                    
                    <span class="r">
                        <a class="mybutton hover"
                           id="downexcel" href="javascript:setDownExcel();"><font>导出Excel</font> </a>
                    </span>
                    <ul id="mytags2">
                    </ul>
                </div>
                <div class="textbox">
                    <table id="mytable" cellpadding="0" cellspacing="0" border="0" class="display">
                        <thead>
                            <tr>
                                <th>
                                    排名
                                </th>
                                <th >
                                    作者ID
                                </th>
                                <th >
                                    作者名称
                                </th>
                                 <th >
                                    资源数
                                </th>
                                <th >
                                    下载量
                                </th>
                                <th >
                                    上次排行
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
        var selectHtml = "";// "<option value='0' selected='selected'>不区分平台</option>";
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
