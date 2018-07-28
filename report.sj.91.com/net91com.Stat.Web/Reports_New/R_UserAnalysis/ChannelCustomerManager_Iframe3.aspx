<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ChannelCustomerManager_Iframe3.aspx.cs" Inherits="net91com.Stat.Web.Reports_New.R_UserAnalysis.ChannelCustomerManager_Iframe3" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>渠道商管理_iframe3</title>
    <link href="/Reports_New/css/site.css?version=1" rel="stylesheet" type="text/css" /> 
    <link href="/Reports_New/css/defindecontrol.css?version=7" rel="stylesheet" type="text/css" />
    <link href="/Reports_New/css/chosen.css" rel="stylesheet" type="text/css" /> 
    <link href="../css/jquery.datatables.table.css" rel="stylesheet" type="text/css" />
    <link href="/Reports_New/css/colorbox.css" rel="stylesheet" type="text/css" />
    <script src="/Reports_New/js/jquery-1.6.4.min.js" type="text/javascript"></script> 
    <script src="/Reports_New/js/Defindecontrol.js?version=13" type="text/javascript"></script> 
    <script src="../js/jquery.dataTables.min.js" type="text/javascript"></script>
    <script src="../js/chosen.jquery.min.js" type="text/javascript"></script>
    <script src="/Reports_New/js/jquery.colorbox-min.js" type="text/javascript"></script> 
    <style type="text/css">
        .textbox table td{ text-align: center;}
        .textbox table td span{ cursor: pointer;}
        .divtable { border-collapse: collapse; margin: 0 auto; width: 100%; height: 80%; border-left: 1px solid #cccccc; border-right: 1px solid #cccccc; }
        .divtable td { padding: 5px; border-style: solid; border-color: #cccccc;  border-width: 1px 0; }
    </style>
    <script type="text/javascript">
        var mysoft = "";
        var mychanneltype = "";
        var mychannelcustomerid = "";
        var serverUrl = '../HttpService.ashx?service=ChannelCustomerManager';
        var oTable;
        var changePlat = {};
        var colorBox3;
        var channelsUnBindData;
        $(function () {
            mysoft = window.parent.document.getElementById("mysoft").value;
            if (mysoft == '109192') {
                $("#divSetAppStoreLink").show();
            }
            else {
                $("#divSetAppStoreLink").hide();
            }
            mychanneltype = window.parent.document.getElementById("mychanneltype").value;
            mychannelcustomerid = window.parent.document.getElementById("mychannelid").value;
            //保证进来的都是渠道商
            if (mychanneltype != "2") {
                mychannelcustomerid = 0;
                return;
            }
            resetsize('post_iframe');
            colorBox3 = $("#mywindow").colorbox({ inline: true, width: "60%", height: "40%", speed: 0 });
            fillBodyData();
            var platHtml = "";
            var checkboxHtml = "";
            for (var i = 0; i < window.parent.platforms.length; i++) {
                var platvalue = window.parent.platforms[i];
                var plattext = allplat[window.parent.platforms[i]];
                var inputid = "checkplats_" + platvalue;
                platHtml += "<option value='" + platvalue + "'>" + plattext + "</option>";
                checkboxHtml += "<input type=\"checkbox\" id=\"" + inputid + "\" class=\"checkplats\" value=\"" + platvalue + "\"/><label >" + plattext + "</label>";
            }

            $("#drpPlatform").html(platHtml);
            $("#drpPlatform").chosen({ no_results_text: "没有找到匹配", disable_search: true }).change(function () {
                $("#searchText").val("");
                getUnBindChannel();
                $("#channeladdselect").val($("#drpPlatform").val());
                changePlat.trigger("liszt:updated");
            });
            checkboxHtml += "<input type=\"checkbox\" id=\"checkplats_0\" class=\"checkplats\" value=\"" + 0 + "\"/><label >" + "(IOS&Andriod)" + "</label>";
            $("#channeladdplats").html(checkboxHtml);
            //changePlat = $("#channeladdselect").chosen({ no_results_text: "没有找到匹配", disable_search: true });
            getUnBindChannel();
        });

        function fillBodyData() {
            $("#mybody").html("<tr><td colspan='6'><div  style='text-align:center;'><img height='15px' src='../images/common/defaultloading.gif'/><div>加载中..</div></div></td></tr>");
            $.getJSON(serverUrl, { "customerid": mychannelcustomerid, "softs": mysoft, "do": "getchannelsbycustomer", "rd": Math.random() * 10000 }, function (data) {
               
                if (data.resultCode == 0) {
                    var mydata = eval("(" + data.data + ")");
                    var tablehtml = "";
                    var arrayStr = [], j = 0;
                    for (var i = 0; i < mydata.data.length; i++) {
                        arrayStr[j++] = "<tr><td  class='channelid'><input class='channelcheck'  type='checkbox' value='";
                        arrayStr[j++] = mydata.data[i].ID;
                        arrayStr[j++] = "'/>" + mydata.data[i].ID + "</td><td>";
                        arrayStr[j++] = mydata.data[i].Name + "</td><td>";
                        arrayStr[j++] = allplat[mydata.data[i].Platform] + "</td><td>" + mydata.data[i].Modulus1 + "</td>";
                        <% if (HasEditRight)
                           { %>
                        arrayStr[j++] = "<td><span  class='caozuo' onclick='editchannelid(";
                        arrayStr[j++] = mydata.data[i].ID;
                        arrayStr[j++] = ",\"" + mydata.data[i].Name + "\"," + mydata.data[i].Modulus1;
                        arrayStr[j++] = ")'>编辑&nbsp;&nbsp;&nbsp;</span> <span class='caozuo' onclick='deletesonchannel(";
                        arrayStr[j++] = mydata.data[i].ID + ")'>解绑&nbsp;</span></td></tr>";
                        <% }
                           else
                           { %>
                        arrayStr[j++] = "<td></td></tr>";
                        <% } %>
                    }
                    $("#mybody").html(arrayStr.join(""));
                    $("#firstmodulus1").text(+mydata.m1);
                    setUnable(mysoft);

                } else {
                    alert(data.message);
                }

            });
        }
        //删除渠道ID
        function deletesonchannel(id) {
            if (confirm("确定要取消绑定选择的渠道吗？")) {
                $.getJSON(serverUrl, { "id": id, "do": "deletechannelid", "softs": mysoft, "rd": Math.random() * 100000 }, function (data) {
                    if (data.resultCode == 0) {
                        alert(data.message);
                        window.parent.setSelectStat(2);
                    } else {
                        alert(data.message);
                    }


                });
            }

        }
         function setUnable(softid) {
             if (isNotPc(softid)) {
                $(".forbiddenclass").each(function () {
                    $(this).hide();
                }); 
            } else {
                 $(".forbiddenclass").each(function () {
                     $(this).show();
                 });
            }
             
        }
        //删除渠道IDs
        function deletesonchannels() {
            var checkes = $(".channelcheck");
            var checkedids = "";
            for (var i = 0; i < checkes.length; i++) {
                if (checkes[i].checked) {
                    checkedids += checkes[i].value + ",";
                }
            }
            checkedids = checkedids.substring(0, checkedids.length - 1);
            if ($.trim(checkedids) == "") {
                alert("请选择要取消绑定的渠道");
                return;
            }
            deletesonchannel(checkedids);
             
        }



        //编辑channelid
        function editchannelid(id, name, m1, mshanzhai) {
            $("#mywindow").attr("href", "#divChannelidEdit");
            colorBox3.click();
            $("#channelName").val(name);
            $("#sonrepeatimei").val(mshanzhai);
            //id==0 表示新增
            if(id==0)
                $("#channelName").attr("disabled", "disabled");
            $("#txtchannelm1").val(m1);
            $("#channelid").val(id);


        }
        function addchannelid( ) {
            $("#mywindow").attr("href", "#divChannelidAdd");
             colorBox3.click();
            $("#channelName").val(""); 
            $("#txtchannelm1").val(0); 

        }

        function setAppStoreLink() {
            window.open('/tools/appstorelink.aspx');
        }

        //全选或者全不选
        function checkall() {
            var checkes = $(".channelcheck");
            var checked = $("#checkallornot")[0].checked;
            for (var i = 0; i < checkes.length; i++) {
                checkes[i].checked = checked;
            }
        }
        function checkall2() {
            var checkes = $(".newchannelcheck");
            var checked = $("#checkunbind2")[0].checked;
            for (var i = 0; i < checkes.length; i++) {
                checkes[i].checked = checked;
            }
        }
    </script>
</head>
<body> 
       
          
    <div class="maindiv" >
                <a id="mywindow"></a>
                <div style="margin-top: 4px;" id="titleid" class="title">
                    <strong id="Strong1" class="l">
                      <input type="checkbox" id="checkallornot" onchange="checkall()" /><label for="checkallornot">全选/取消</label>
                         绑定的渠道ID列表
                    </strong> <span class="r"></span>
                </div>
                <div class="textbox">
                     <table cellpadding="0" border="0" cellspacing="0" class="display dataTable" id="table1">
                        <thead>
                            <tr  >
                                <th>
                                    ID
                                </th>
                                <th>
                                    名称
                                </th>
                                <th>
                                    平台
                                </th>
                                <th>
                                    一次系数(<span id="firstmodulus1"></span>)
                                </th>
                                <th>
                                    操作
                                </th>
                            </tr>
                        </thead>
                        <tbody id="mybody">
                            
                        </tbody>
                    </table>
                </div>
                <div>
                    <table style="width: 100%" >
                        <tr>
                            <td></td>
                            <td>
                             <% if (HasEditRight)
                                { %> 
                            <a class="mybutton hover" style="cursor: pointer; margin-top: 4px; margin-right: 5px;
                              float: right" onclick="deletesonchannels();"><font>取消绑定所选渠道</font></a>
                              <% } %>
                             </td>
                            <td> </td>

                        </tr>
                        <tr>
                            <% if (HasEditRight)
                               { %>
                            <td>对如上勾选的渠道设置一次系数：</td>
                            <td><input type="text" id="channelm1" class="txtbox"/>
                            <a class="mybutton hover" style="cursor: pointer; margin-top: 4px; margin-right: 5px;
                              float: right" onclick="setallM1();"><font>设置</font></a>
                             </td>
                            <td><div id="divSetAppStoreLink"><a class="mybutton hover" style="cursor: pointer; margin-top: 4px; margin-right: 5px;
                              float: right" onclick="setAppStoreLink();"><font>设置AppStore推广链接</font></a></div></td>
                            <% } %>
                        </tr>
                         
                        <tr>
                            <td colspan="3">
                                <div id="unbinddiv">
                                   <div style="margin-top: 4px;" id="Div1" class="title">
                                       <strong id="Strong2" class="l"> <input type="checkbox" id="checkunbind2" onchange="checkall2()" /><label for="checkallornot">全选/取消</label>未绑定的渠道ID</strong>
                                       <span class="r">
                                        <table>
                                            <tr>
                                                 
                                                <td style="vertical-align:top;padding-top: 6px;">
                                                     <select id="drpPlatform" style="width: 100px;" > </select>
                                                </td>
                                                <td style="vertical-align: top;">
                                                      渠道ID：<input type="text" class="txtbox" id="searchText" style="width: 50px" /> 

                                                </td>
                                                <td style="vertical-align: top;">
                                                    <a class="mybutton hover" style="cursor: pointer; margin-right: 5px;
                                                        float: right" onclick="searchChannel()"><font>搜&nbsp;索</font></a> 
                                                </td>
                                                 <td style="vertical-align: top;">
                                                     <% if (HasEditRight)
                                                        { %> 
                                                        <a class="mybutton hover " style="cursor: pointer;  margin-right: 5px;
                                                        float: left" onclick="addchannelid()"><font>添加渠道ID</font> </a> 
                                                        <% } %>
                                                </td>
                            
                                               
                                            </tr>
                                        </table>
                                       </span>
                                    </div>
                                   <div class="textbox">
                                     <table cellpadding="0" border="0" cellspacing="0" class="display dataTable" id="table2">
                                        <thead>
                                            <tr  >
                                             <th>
                                                ID
                                            </th>
                                            <th>
                                                名称
                                            </th>
                                            <th>
                                                操作
                                            </th>
                                                
                                            </tr>
                                        </thead>
                                        <tbody id="unbindtbody">
                                            
                                        </tbody>
                                        </table>
                                    </div>
                                    <% if (HasEditRight)
                                       { %>
                                    <a class="mybutton hover" style="cursor: pointer; margin-top: 4px; margin-right: 15px;
                                       float: right" onclick="bindSelectChannels();"><font>一次性设置到本渠道商下</font></a>
                                 <% } %>
                                 <%-- <input type="button" class="btnclass" value="一次性设置如上勾选的渠道到本渠道商下" onclick="bindSelectChannels()" />--%>
                                </div>
                                   
                            </td>
                        </tr>
                    </table>
                </div>
        
    </div>
      <!--隐藏div 弹出框用-->      
         <div style="display: none; padding: 5px">
        <div id="divChannelidEdit">
            <input type="text" style="display: none" id="channelid" /> 
            <table id="channeltable" class="divtable">
                 <tr style="height: 40px;">
                    <td colspan="2">
                       <h3>编辑渠道ID</h3>
                    </td>
                    
                </tr>
                <tr style="height: 40px;">
                    <td>
                        名称(不可更改):
                    </td>
                    <td>
                        <input type="text" class="txtbox" disabled="disabled" style="width: 150px;"  id="channelName" />
                    </td>
                </tr>
                <tr style="height: 40px;">
                    <td>
                        一次系数:
                    </td>
                    <td>
                        <input type="text" class="txtbox"  style="width: 150px;" id="txtchannelm1" />
                    </td>
                </tr>
                <tr>
                    <td colspan="2"><a class="mybutton hover" style="cursor: pointer; margin-top: 4px; margin-right: 15px;
                            float: right" onclick="setChanelUpdate();"><font>保存</font></a></td>

                </tr>
            </table>
        </div>
        
        <div id="divChannelidAdd"> 
            <table id="channeladdtable"  class="divtable">
                <tr style="height: 40px;">
                    <td colspan="2">
                       <h3>添加渠道ID</h3>
                    </td>
                    
                </tr>
                <tr style="height: 40px;">
                    <td>
                        名称:
                    </td>
                    <td>
                        <input type="text" class="txtbox" style="width: 150px;"  id="addChannelName" />
                    </td>
                </tr>
                <tr style="height: 40px;">
                    <td>
                        平台:
                    </td>
                    <td id="channeladdplats">
                       <%-- <select id="channeladdselect" style="width: 100px" > </select> --%>
                    </td>
                </tr>
                <tr style="height: 40px;">
                    <td>
                        一次系数:
                    </td>
                    <td>
                        <input type="text" class="txtbox" value="0"  style="width: 150px;" id="addChannelModule1" />
                    </td>
                </tr>
                
                <tr>
                    <td colspan="2"><a class="mybutton hover" style="cursor: pointer; margin-top: 4px; margin-right: 15px;
                            float: right" onclick="addDefindChannel();"><font>保存</font></a></td>

                </tr>
            </table>
        </div>
    </div>
         <!--隐藏div--> 
       
</body>
</html>
<script type="text/javascript">
    function getUnBindChannel() {
        var tablehtml = "";
        $("#unbindtbody").html("<tr><td colspan='3'><div  style='text-align:center;'><img height='15px' src='../images/common/defaultloading.gif'/><div>加载中..</div></div></td></tr>");
        $.getJSON(serverUrl, { "customerid": mychannelcustomerid, "softs": mysoft, "do": "getunbindlist", "rd": Math.random() * 10000 }, function (data) {
            channelsUnBindData = data.data;
            searchChannel(); 
        });
    }
    

    ///渠道搜索
    function searchChannel() {
        if (typeof (channelsUnBindData) == "undefined") {
                return;
        }
        var plat = $("#drpPlatform").val();
        var reg = new RegExp(".*" + $("#searchText").val().toLowerCase() + ".*"); 
        var arrayStr = [], m = 0;
        for (var i = 0, j = channelsUnBindData.length; i < j; i++) {
            if ( plat == channelsUnBindData[i].platform   && channelsUnBindData[i].Name.toLowerCase().search(reg) != -1)
            {
                arrayStr[m++] = "<tr><td  class='addchannelclass'><input class='newchannelcheck'  type='checkbox' value='" + channelsUnBindData[i].ID + "' />";
                arrayStr[m++] = channelsUnBindData[i].ID;
                arrayStr[m++] = "</td><td>";
                arrayStr[m++] = channelsUnBindData[i].Name + "</td>";
                <% if (HasEditRight)
                   { %>
                arrayStr[m++] = "<td><span  class='caozuo' onclick='bindSelectChannels(" + channelsUnBindData[i].ID + ")'>设置到本渠道商下&nbsp;</span></td>";
                <% } else  
                   { %>
                 arrayStr[m++] = "<td></td>";
                <% } %>
                arrayStr[m++] ="</tr>";
            }
        }
        $("#unbindtbody").html(arrayStr.join(""));
    }

    //设置1次系数
    function setallM1() {
        var value = $("#channelm1").val();
        var reg = /^(-?\d+)(\.\d+)?$/;
        if (!reg.test(value)) {
            alert("系数输入值格式不正确！");
            return;
        }
        var ids = "";
        $("#mybody .channelcheck").each(function (index, a) {
            if (a.checked) {
                ids += $(a).val() + ",";
            }
        });
        ids = ids.substring(0,ids.length-1); 
        if (ids == "") {
            alert("请选择渠道ID！");
            return;
        }
        $.getJSON(serverUrl, { "channelids": ids, "softs": mysoft, "do": "setselectchannelm1", "m": value, "rd": Math.random() * 10000 }, function (data) {
            if (data.resultCode == 0) {
                alert(data.message);
                //右边再次刷新
                window.parent.setSelectStat(2);
            } else {
                alert(data.message);
            }

        });
    }
    //更新channelid
    function setChanelUpdate(type) {
        var name = $("#channelName").val(); ;
        var m1 = $("#txtchannelm1").val();
        var repeatimei = $("#sonrepeatimei").val(); 
        var id = $("#channelid").val();
        var reg = /^(-?\d+)(\.\d+)?$/;
        if (!reg.test(m1)) {
            alert("系数格式不正确");
            return;
        }
        if (m1 < -99 || m1 > 99) {
            alert("系数范围出错");
            return;
        }
        //type 0:update;type 1:add
        $.getJSON(serverUrl, {"softs":mysoft ,"id": id, "m1": m1, "name": name, "do": "updatechannelid", "rd": Math.random() * 1000 }, function (data) {
            if (data.resultCode == 0) {
                colorBox3.colorbox.close();
                alert(data.message);
                //右边再次刷新
                window.parent.setSelectStat(2);
                
                
            } else {
                alert(data.message);
            }

        });


    }
    //添加渠道ID
    function addDefindChannel() {
        var name = $("#addChannelName").val();
        var m1 = $("#addChannelModule1").val();

        var plats = $("#channeladdplats .checkplats");
        var platvalues = "";
        for (var i = 0; i < plats.length; i++) {
            if (plats[i].checked == true) {
                platvalues += plats[i].value+",";
            }
        }
        platvalues = platvalues.substring(0, platvalues.length - 1);
        if ($.trim(name) == "" || $.trim(platvalues) == "") {
            alert("请填写完整!");
            return;
        }
        var reg = /^(-?\d+)(\.\d+)?$/;
        if (!reg.test(m1)) {
            alert("系数格式不正确");
            return;
        }
        if (m1 < -99 || m1 > 99) {
            alert("系数范围出错");
            return;
        }
        $.getJSON(serverUrl, { "customerid": mychannelcustomerid, "softs": mysoft, "platform": platvalues, "m1": m1, "name": name, "do": "adddefinedchannelid", "rd": Math.random() * 1000 }, function (data) {
            if (data.resultCode == 0) {
                colorBox3.colorbox.close();
                alert(data.message);
                //右边再次刷新
                window.parent.setSelectStat(2);
                
            } else {
                alert(data.message);
            }   
        });
    }

     
    //绑定渠道到对应的渠道商
    function bindSelectChannels(id) {
        if (confirm("您确定要设置新渠道到本渠道商下吗?")) {
            var ids = "";
            if (id) {
                ids = id;
            } else {
                $(".newchannelcheck").each(function (index, a) {
                    if (a.checked) {
                        ids += $(a).val() + ",";
                    }
                });
                ids = ids.substring(0, ids.length - 1); 

            }
            if (ids == "") {
                alert("请选择勾选的新渠道！");
                return;
            }
            $.getJSON(serverUrl, { "autoids": ids, "customerid": mychannelcustomerid, "softs": mysoft, "do": "addnewchannelfromclient", "rd": Math.random() * 1000 }, function (data) {
                if (data.resultCode == 0) {
                    
                    colorBox3.colorbox.close();
                    alert(data.message);
                    //右边再次刷新
                    window.parent.setSelectStat(2);
                    
                } else {
                    alert(data.message);
                } 
                 
            });
            
        }
       
    }

</script>
