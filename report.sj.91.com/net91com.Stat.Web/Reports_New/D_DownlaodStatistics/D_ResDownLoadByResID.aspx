<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="D_ResDownLoadByResID.aspx.cs"
    Inherits="net91com.Stat.Web.Reports_New.D_DownlaodStatistics.D_ResDownLoadByResID" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>新按资源ID查询</title>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <link href="../css/site.css?version=1" rel="stylesheet" type="text/css" />
    <link href="../css/defindecontrol.css?version=1" rel="stylesheet" type="text/css" />
    <link href="../css/intelligencesearch.css" rel="stylesheet" type="text/css" />
    <link href="../css/jquery.datatables.table.css" rel="stylesheet" type="text/css" />
    <link href="../css/colorbox.css" rel="stylesheet" type="text/css" />
    <link href="../css/jquery-ui.css" rel="stylesheet" type="text/css" />
    <link href="../css/jquery.multiselect.css" rel="stylesheet" type="text/css" />
    <link href="/css/headcss/jquery.multiselect.filter.css" rel="stylesheet" type="text/css" />

    <script src="../js/jquery-1.6.4.min.js" type="text/javascript"></script>
    <script src="../js/jquery.dataTables.min.js" type="text/javascript"></script>
    <script src="../js/Defindecontrol.js?version=1" type="text/javascript"></script>
    <script src="../js/intelligencesearch.js" type="text/javascript"></script>
    <script src="../js/jquery.colorbox-min.js" type="text/javascript"></script>
    <script src="../js/highcharts_old.js" type="text/javascript"></script>
    <script src="../js/chartjs.js" type="text/javascript"></script>
    <script src="../js/My97DatePicker/WdatePicker.js" type="text/javascript"></script>
    <script src="/Scripts/HeadScript/jquery-ui.min.js" type="text/javascript"></script>
    <script src="/Scripts/HeadScript/jquery.multiselect.js" type="text/javascript" charset="GBK"></script>
    <script src="/Scripts/HeadScript/jquery.multiselect.filter.js" type="text/javascript" charset="GBK"></script>
    <script src="../js/selectcontrols.js" type="text/javascript"></script> 

    <script type="text/javascript">
        var serverUrl = '../HttpService.ashx';
        var stattypes = { "1": "下载点击", "4": "下载成功", "5": "安装成功", "6": "安装失败", "8": "下载失败" };
        var softPlatJson = <%=SoftPlatformJson %>;
        var projectJson=<%=ProjectJson%>;
        var oTable ;
        var resContronl;
        var platControl;
        var projectControl;
        var downstateControl;
        var softControl;
        var versionControl;
        var modetypeControl;
        var typeControl;
        var SoftAreaJson = <%= SoftAreaJson %>;
        var AreaJson = <%= AreaJson %>;
        var g_areatype;
        var hiddenDialog;
        var AreaControl;
        $(function () {
            initValue("beginTime", "endTime", -7, 0);
            $("#mywindow").colorbox({ iframe: true, width: "85%", height: "80%" });
            hiddenDialog = $("#mywindow").colorbox({ iframe: true, width: "100%", height: "80%", speed: 0 });
            
            softControl = createSelectControl("selectsofts", '请选择', false, function () {
                if ($("#selectsofts").val() == null) {
                    alert("请选择一个软件类型");
                    return false;
                }
                funchiddenarea();
                getAreaTypeByType($("#selectsofts").val());
                reSetPlats($("#selectsofts").val(),4);
                setpopplatform();
                setrestype();
                reSetSelectMode();
                clearSelect();
                reSetArea($("#selectsofts").val());
                return true;
            }, function () {
                return true;
            }, true);
            funchiddenarea();
            reSetArea($("#selectsofts").val() );
            resContronl = createSelectControl("selectrestype", '请选择', false, function () {
                if ($("#selectrestype").val() == null) {
                    alert("请选择一个资源类型");
                    return false;
                }
                setrestype();
                return true;
            }, function () {
                return true;
            }, true);
            
            modetypeControl = createSelectControl("selectmodetype", '请选择', false, function() {
                selectChangeMode($("#selectmodetype").val());
                return true;
            }, function () {
                return true;
            },false,false,110,110);
            
            platControl = createSelectControl("selectplatform", '请选择', false, function () {
                if ($("#selectplatform").val() == null) {
                    alert("请选择一个平台");
                    return false;
                }
                setpopplatform();
                return true;
            }, function () {
                return true;
            }, true);
            
            projectControl = createSelectControl("selectprojectsource", '请选择', false, function () {
                if ($("#selectprojectsource").val() == null) {
                    alert("请选择一个平台");
                    return false;
                }
                return true;
            }, function () {
                return true;
            }, true);
            
            downstateControl = createSelectControl("selectdownstat", '请选择', true, function () {
                if ($("#selectdownstat").val() == null) {
                    alert("请选择一个下载类型");
                    return false;
                }
                return true;
            }, function () {
                return true;
            }, true); 
            
            versionControl= createSelectControl("selectversion", '请选择', true, function () {
                if ($("#selectversion").val() == null) {
                    alert("请选择一个版本");
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
            
            typeControl= createSelectControl("selecttype", '请选择', false, function () {
                clearsoft();
                return true;
            }, function () {
                return true;
            }, false,false,110,110);
            
            $('#input_f_showname').focus().keyup(
                function (a) {
                    ///若为资源名称
                    var restype = SoftAreaJson[$("#selectsofts").val()];
                    pop.restype = restype;
                    if (restype==2) {
                        pop.restype = 101;
                    } else {
                        pop.restype = 0;
                    }
                    if ($("#selecttype").val() == "2"||$("#selecttype").val() == "3")
                        return;
                    if ($(this).val().replace(/ /g, '').length == 0) {
                        pop.target.hide();
                        return;
                    }
                    $("#residspan").html("");
                    switch (a.keyCode) {
                        case 37: { pop.pre(); } break;
                        case 38:
                            {
                                pop.fromSelect(-1);
                                return false;
                            } break;
                        case 39: { pop.next(); } break;
                        case 40:
                            {
                                pop.fromSelect(1);
                                return false;
                            } break;
                        case 13: { pop.toSelect(); } break;
                        default:
                            {
                                pop.key = $(this).val();
                                pop.page = 1;
                                pop.postAjax();
                            } break;
                    }
                });

            $(document).bind('mousedown', function (event) {
                var e = event;
                var target = e.srcElement || e.target;
                if (!$.contains($("#popDiv")[0], target)) {
                    pop.target.hide();
                }
            });
            reSetPlats($("#selectsofts").val());
            getAreaTypeByType($("#selectsofts").val());
            
            pop.init();
            pop.toSelect = selectcallback;
            setpopplatform();
            setrestype();
            //获取表格
    
            selectChangeMode($("#selectmodetype").val());
            reSetSelectMode();
            getchart();
        });

        //获取地区类型
        function getAreaTypeByType(softid) {
            g_areatype=SoftAreaJson[softid];
            if (softid == 113938 || softid == 97410 || softid == 113939) {
                //为了后台能取到台湾的资源信息
                g_areatype = 3;
            }
        }

        function setpopplatform() {
            var plat = $("#selectplatform").val();
            if (g_areatype == 2) {
                if (plat == 4 || plat == 9)
                    pop.platform = 4;
                else if (plat == 1 || plat == 7)
                    pop.platform = 4;
            } else {
                if (plat == 4 || plat == 9)
                    pop.platform = 4;
                else if (plat == 1 || plat == 7)
                    pop.platform = 4;
                else
                    pop.platform = plat;
            }

        
        }
        function setrestype() {
            var restype = $("#selectrestype").val();
            if (g_areatype==2) {
                if (restype == "22" || restype=="19")
                    pop.restype = 1; //底层接口特别处理结果
                else
                    pop.restype = 101;
            } else {
                if (restype == "22" || restype=="19")
                    pop.restype = 1; //底层接口特别处理结果
                else
                    pop.restype = 0;
            }

        }
        
        function selectcallback() {
            var o = pop.target.find('table[index=' + pop.index + ']');
            if (o.length > 0) {
                $("#hiddensoftid").val(o.attr("fid"));
                if ($("#selecttype").val() == "1")//若选择名称
                    $("#residspan").html("ID:" + o.attr("fid"));
                $("#input_f_showname").val(o.attr("fname"));
                pop.target.hide();
            }
        }
        
        function clearsoft() {
            $("#hiddensoftid").val("-1");
            $("#input_f_showname").val("").focus();
            $("#residspan").html("");
        }
        
        function loadData() {
            if($("#selecttype").val()=="2")
            {
                if ($("#input_f_showname").val()<=0) {
                    alert('资源ID要大于0');
                    return;
                }
                $("#hiddensoftid").val($("#input_f_showname").val());
            }
            if($("#selecttype").val()=="3")
            {  
                $("#hiddensoftid").val(0);
            }
            if (checkDataRight()) {
                getchart();
            }
        }
        
        function getDetailTable() {
            var arr = getConditionArray("selectversion");
            var strs = "";
            var html = "";
            for (var i = 0; i < arr.length; i++) {
                html += " <li><a onclick=\"gettabs('" + arr[i].id   + "')\"><font>" + arr[i].value + "</font></a></li>";
            }
            if (arr.length > 1 ) {
                $("#mytags2").html(html);
                mytabs2 = createTabs($("#mytags2"), 0);
                mytabs2.click(0);
            }
            else {
                gettabs(arr[0].id);
                $("#mytags2").html("");
            }
        }
    </script>
</head>
<body> 
    <form id="form1" runat="server"> 
     <asp:HiddenField runat="server" ID="myhidden" />
    <div class="maindiv">
        <div style="padding: 10px;">
            <table cellpadding="0" cellspacing="10" border="0" width="90%">
                <tr>
                    <td  style="width: 200px">
                        <div style="position: relative; z-index: 100">
                            <select id="selectsofts" style="width: 150px;"  >
                                <%=SoftHtml%>
                            </select>
                        </div>
                    </td>
                   <td>
                        <div style="position: relative; z-index: 100">
                            <select id="selectplatform"  style="width: 120px;">
                            </select>
                        </div>
                    </td>

                    <td>
                        <div style="position: relative; z-index: 100">
                            <select id="selectrestype" style="width: 150px;">
                                <%=restypehtml %>
                            </select>
                        </div>
                    </td>

                    <td style="width: 40px">
                        <select id="selectmodetype" style="width: 80px;">
                               <option  value="2">版本:</option>
                               <option  value="3">项目来源:</option>
                        </select>
                    </td>
                    <td style="width: 200px;">
                        <div id="divversion" class="modetypeclass"><select id="selectversion" style="width: 150px; display: none" multiple="multiple"    >
                                <option  value="-1" selected="selected">不区分版本</option>
                        </select></div>
                        
                        <div id="divprojectsources"  class="modetypeclass"><select id="selectprojectsource" style="width: 150px; display: none"    >
                             <option  value="-1"  selected="selected">不区分来源</option>
                        </select></div>
                    </td>
                </tr>
                <tr>
                     <td >
                        <select id="selectdownstat" style="width: 300px;" multiple="multiple">
                            <option value="1" selected="selected">下载点击</option>
                            <option value="2" >展示</option>
                            <option value="4" selected="selected">下载成功</option>
                             <option value="8">下载失败</option>
                            <option value="5">安装成功</option>
                            <option value="6">安装失败</option>
                        </select>
                    </td>
                    <td>
                        从：
                        <input type="text" id="beginTime"  style="width: 135px" class="ui-multiselect ui-widget ui-state-default ui-corner-all Wdate"  onclick="WdatePicker()" /></td>
                    <td>
                        到：
                        <input type="text" id="endTime"  style="width: 135px" class="ui-multiselect ui-widget ui-state-default ui-corner-all Wdate"  onclick="WdatePicker()" />
                    </td>
                    <td>
                        <select id="selecttype" onchange="clearsoft()">
                            <option selected="selected" value="1">资源名称:</option>
                            <option value="2">资源ID:</option>
                            <option value="3">资源标识符:</option>
                        </select>
                        
                    </td>
                    <td>
                        <input type="text" class="txtbox ui-multiselect ui-widget ui-state-default ui-corner-all" style="width: 165px"  value="91助手" id="input_f_showname" />
                    </td>
                    <td></td>
                      <td></td>
                </tr>
                <tr>

                    <td colspan="4">
                        位置：<input type="text" onkeyup="this.value=this.value.replace(/[^,0123456789]/g,'')" style="width: 60%" class="txtbox" value="" id="txtPosition" />
                       <span id="Span1">逗号分开,eg:1,3</span> <span id="residspan"></span>
                    </td>
                    <td style="width: 200px;">
                        <div  style="width: 100%">
                       <div class="selectdisplay" style="width: 100%">
                            <div id="divareatxt">
                                    <select id="selectArea" style="width: 150px;"  >
                     
                                    </select>
                            </div>
                        </div>
                        </div>
                    </td>
                    <td style="width: 250px;">
                        <span style="cursor: pointer; float: right; margin-right: 10px;"><a class="mybutton hover"
                            style="margin-top: 4px; overflow: hidden;" onclick="loadData();"><font>查询</font>
                        </a></span> 
                    </td>
                </tr>
            </table>
        </div>
        <input type="hidden" id="hiddensoftid" value="41293197" />
        <input type="hidden" id="myversion"  value="-1" />
        <div style="clear: both">
        </div>
        <div class="title" style="margin-top: 4px;">
            <strong class="l" id="Strong2">下载数据曲线图</strong> <span class="r"></span>
        </div>
        <div class="textbox">
            <div id="container" style="margin: auto; margin: 2px; height: 400px; width: 100%">
            </div>
        </div>
        <div class="title" style="margin-top: 4px;">
            <strong class="l" id="Strong1">下载数据明细 </strong><span class="r">
            <a class="mybutton hover"
                id="downexcel" href="javascript:void(0);"><font>导出Excel</font> </a>
            </span>
            <ul id="mytags2">
            </ul>
        </div>

        <div class="textbox">
            <table id="table1" width="100%" cellpadding="0" cellspacing="0" border="0" class="display">
                <thead>
                    <tr>
                        <th rowspan="2" style="width: 120px">
                            时间
                        </th>
                        <th colspan="4">
                            下载点击
                        </th>
                        <th  colspan="3">
                            下载情况
                        </th>
                        <th  colspan="3">
                            安装情况
                        </th>
                        <th  rowspan="2">
                            下载浏览
                        </th>
                        <th  rowspan="2">
                            明细位置
                        </th>
                    </tr>
                    <tr>
                         <th>
                             所有
                        </th>
                        <th>
                            更新
                        </th>
                         <th>
                            去除更新
                        </th>
                        <th>
                            来自搜索
                        </th>
                         <th>
                            下载成功
                        </th>
                         <th>
                            下载成功去除更新
                        </th>
                        <th>
                            下载失败
                        </th>
                         <th>
                            安装成功
                        </th>
                         <th>
                            安装成功去除更新
                        </th>
                        <th>
                            安装失败
                        </th>
                    </tr>
                </thead>
                <tbody>
                </tbody>
            </table>
        </div>
         <a id="mywindow" style="display: none"></a>
    </div>
    <div id='popDiv' style="display: none" />
    
    </form>
</body>
</html>
<script type="text/javascript">
    /*检查数据的正确性*/
    function checkDataRight() {
        if ($("#beginTime").val() == "") {
            alert("请选择开始日期");
            return false;
        }
        if ($("#endTime").val() == "") {
            alert("请选择结束日期");
            return false;
        }
        var s = $("#beginTime").val().split('-');
        var sdate = new Date(s[0], s[1] - 1, s[2]);
        var e = $("#endTime").val().split('-');
        var edate = new Date(e[0], e[1] - 1, e[2]);
        // 开始日期与结束日期的时间差
        var t = edate.getTime() - sdate.getTime();
        if (t < 0) {
            alert("开始日期不能大于结束日期");
            return false;
        }
        if (e[1] - s[1] > 1) {
            alert("跨度不能超过两个月");
            return false;
        }
        var softid=$("#hiddensoftid").val();
        if (softid=="-1") {
            alert("请先填写一款软件");
            return false;
        }
        var reg = /^\d+$/;
        if (!reg.test(softid)) {
            alert("输入软件格式不正确");
            return false;
        }
        if (!$("#selectprojectsource").val()) {

            alert("请选择项目来源");
            return false;
        }
        if (!$("#selectdownstat").val()) {

            alert("请选择状态类型");
            return false;
        }

        return true;
    }
    
    function openwindow(param) {
        $("#mywindow").attr("href","D_ResDownLoadByResIDDetail.aspx?params="+param);
        $("#mywindow").click();
    }
    
    function gettabs(versionid) {
        $("#myversion").val(versionid); 
        if (oTable && checkDataRight()) {
            oTable.fnDraw();
            return;
        }
        oTable = $('#table1').dataTable({
            "sDom": '<"top">rt<"bottom"><"clear">',
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
                { "sClass": "tdright" },
                { "sClass": "center" }
            ],
            "aoColumnDefs": [
                {
                    "aTargets": [0, 1, 2, 3, 4, 5, 6, 7, 8, 9,10,11,12],
                },
                {
                    "bUseRendered": false,
                    "fnRender": function (oObj) {
                        var colindex = oObj.iDataColumn;
                        if (colindex == 12) {
                            var params = oObj.aData[12];
                            if (params != "")
                                return "<span   onclick=\"openwindow('" + params + "')\" style='text-decoration:underline;color:blue;display:block;cursor:pointer;'>明细位置</span>";
                            else
                                return "";
                        } else {
                            return oObj.aData[colindex];
                        }


                    },
                    "aTargets": [12]
                }
            ],
            "aaSorting": [[0, 'asc']],
            "bProcessing": true,
            "bServerSide": true,
            "oLanguage": { sUrl: "../js/de_DE.txt" },
            "bFilter": false,
            "bSortable": true,
            "sPaginationType": "full_numbers",
            "sAjaxSource": serverUrl,
            "bPaginate": false,
            "fnServerParams": function (aoData) {
                aoData.push({ "name": "service", "value": "D_HttpDownStatDownCountSumByResIDImpl" });
                aoData.push({ "name": "do", "value": "get_detailtable" });
                aoData.push({ "name": "endtime", "value": $("#endTime").val() });
                aoData.push({ "name": "begintime", "value": $("#beginTime").val() });
                aoData.push({ "name": "softid", "value": $("#selectsofts").val() });
                aoData.push({ "name": "projectsource", "value": $("#selectprojectsource").val() });
                aoData.push({ "name": "platform", "value": $("#selectplatform").val() });
                aoData.push({ "name": "restype", "value": $("#selectrestype").val() });
                aoData.push({ "name": "resid", "value": $("#hiddensoftid").val() });
                aoData.push({ "name": "resname", "value": $("#input_f_showname").val() }); 
                aoData.push({ "name": "version", "value": $("#myversion").val() }); 
                aoData.push({ "name": "position", "value": $.trim($("#txtPosition").val()) });
                aoData.push({ "name": "areatype", "value": g_areatype });
                aoData.push({ "name": "resselecttype", "value":$("#selecttype").val() });
                aoData.push({ "name": "areaid", "value":$("#selectArea").val() });
                
                
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
            "fnPreDrawCallback": function (oSettings) {
                setDownExcel();
                return true;
            },

            "fnDrawCallback": function () {
                tableStyleSet();
            }
        });
    }
    function link(para) {
        $("#mywindow").attr("href", "ResDownLoadByResID_Position.aspx?para=" + para);
        $("#mywindow").click();
    }

    function tableStyleSet() {
        $("#table1 tr").mouseover(function () {
            $(this).attr("style", "background:none repeat scroll 0 0 #73A3CC;cursor:pointer");

        });
        $("#table1 tr").mouseout(function () {
            $(this).attr("style", "");
        });
    }
    ///设置下载
    function setDownExcel() {
        var url = serverUrl + "?do=get_excel&projectsource=" + $("#selectprojectsource").val()+"&softid="+$('#selectsofts').val()+"&version="+$("#myversion").val()
             + "&platform=" + $("#selectplatform").val() + "&begintime=" + $("#beginTime").val() + "&endtime=" + $("#endTime").val()
             + "&resid=" + $("#hiddensoftid").val() + "&restype=" + $("#selectrestype").val() + "&service=" + "D_HttpDownStatDownCountSumByResIDImpl" +
            "&resname=" + $("#input_f_showname").val() + "&position=" + $.trim($("#txtPosition").val())+"&areatype="
            +g_areatype+"&resselecttype="+$("#selecttype").val()+"&areaid="+$("#selectArea").val();; 
        $("#downexcel").attr("href", url);
    }

    function parsArrayToString(controlid) {
        var arr = $("#"+controlid).val(); 
        var strs = "";
        for (var i = 0; i < arr.length; i++) {
            strs += arr[i] + ",";
        }
        return strs.substring(0, strs.length - 1);
    }
    function getchart() {
        
        var strsVers = parsArrayToString("selectversion");
        var strsStattaype=parsArrayToString("selectdownstat");
        
        $.ajax({
            type: "get",
            url: serverUrl,
            dataType: "json",
            data: {
                'service': 'D_HttpDownStatDownCountSumByResIDImpl',
                'do': 'get_chart',
                'endtime': $("#endTime").val(),
                'begintime': $("#beginTime").val(),
                'softid':$("#selectsofts").val(),
                'projectsource': $("#selectprojectsource").val(),
                'version': strsVers,
                'stattype': strsStattaype,
                'modetype':$("#selectmodetype").val(),
                'platform': $("#selectplatform").val(),
                'restype': $("#selectrestype").val(),
                'resid': $("#hiddensoftid").val(),
                'resname': $("#input_f_showname").val(),
                'position': $.trim($("#txtPosition").val()),
                'areatype': g_areatype,
                'resselecttype':$("#selecttype").val(),
                'areaid':$("#selectArea").val()
            },
            success: function (data) {
                if (data.resultCode == 0) {
                    $("#container").html("");
                    var title = "";
                    var yname = "资源下载";
                    var obj = eval("(" + data.data + ")");
                    var sdata=createLine("container", 400, obj.title, yname, true, 0, obj.x, obj.y);
                    getDetailTable();
                }
                else {
                    alert(data.message);
                }
            }
        });
    }
</script>

<script type="text/javascript">
    function reSetSelectMode() {
        reSetVersion($("#selectsofts").val());
        reSetProject($("#selectsofts").val());
    }
    
    function reSetArea(softid, defaultValue) {
       
        //台湾地区也属中国
        var v_areatype = 1;
        if (softid == 113938 || softid == 97410 || softid == 113939) {
            v_areatype = 1;
            //为了后台能取到台湾的资源信息
            g_areatype = 3;
        } else {
            g_areatype = SoftAreaJson[softid];
            v_areatype = g_areatype==2?2:1;
        }
        var areahtml = AreaJson[v_areatype];
        var selectHtml = "<option value='-1' selected='selected'>不区分地区</option>";
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
        $("#selectplatform").html(selectHtml);
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
        $("#selectprojectsource").html(selectHtml);
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
                            'platform': $("#selectplatform").val()
                        },
                        function (data2) {
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
    ///模式切换
    function selectChangeMode(type) {
        clearSelect();
        $(".modetypeclass").hide();
        switch (type) {
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
        $("#selectprojectsource").val("-1");
        if (projectControl)
            projectControl.refresh();
        if (versionControl)
            versionControl.refresh();
    }
    
    function funchiddenarea() {
        if (SoftAreaJson[$("#selectsofts").val()]!=2) {
            $(".selectdisplay").hide();
        } else {
            $(".selectdisplay").show();
        }
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
