<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="HeadDownControl.ascx.cs"
    Inherits="net91com.Stat.Web.Reports.Controls.HeadDownControl" %>
<%@ Import Namespace="net91com.Core.Extensions" %>
<%@ Import Namespace="net91com.Core" %>
<%@ Import Namespace="net91com.Reports.UserRights" %>
<link href="../../css/headcss/head.css?rd=<%= net91com.Stat.Web.Reports.Services.Utility.CssVersion %>"
    rel="stylesheet" type="text/css" />
<link href="../../css/headcss/jquery-ui.css" rel="stylesheet" type="text/css" />
<link href="../../css/headcss/style.css?rd=<%= net91com.Stat.Web.Reports.Services.Utility.CssVersion %>"
    rel="stylesheet" type="text/css" />
<link href="../../css/headcss/jquery.multiselect.css?rd=<%= net91com.Stat.Web.Reports.Services.Utility.CssVersion %>"
    rel="stylesheet" type="text/css" />
<link href="../../css/headcss/jquery.multiselect.filter.css" rel="stylesheet" type="text/css" />
<script src="../../Scripts/HeadScript/jquery-ui.min.js" type="text/javascript"></script>
<script src="../../Scripts/HeadScript/jquery.multiselect.js?rd=<%= net91com.Stat.Web.Reports.Services.Utility.JsVersion %>"
    type="text/javascript" charset="GBK"></script>
<script src="../../Scripts/HeadScript/jquery.multiselect.filter.js?rd=<%= net91com.Stat.Web.Reports.Services.Utility.JsVersion %>"
    type="text/javascript" charset="GBK"></script>
<script src="../../My97DatePicker/WdatePicker.js" type="text/javascript"></script>
<script type="text/javascript">
    var ischecked=false;
    var allplats = <%=AllPlats %>;
    var ischangedrestype = "";
    var ischangedplat = "";
    var ischangedressouce = "";
    var ischangedcustomtype = "";
    var restypebox;
    var platbox;
    var ressourcebox;
    var grantbox;
    var customtypebox;
    Array.prototype.contains = function(item) {
        for (var i = 0; i < this.length; i++) {
            if (this[i] == item) {
                return true;
            }
        }
        return false;
    };
    $(function () { 
       
        ///平台加载进去
        <%if(!HiddenPlat){ %>
            var platselect=$("#myselectplat")[0];
              for (var  i = 0; i < allplats.length; i++) {
            platselect.add(new Option(allplats[i].name, allplats[i].id));
        }
        <%} %>
 

        <%if(IsHasNoPlat&&!HiddenPlat) {%>
             platselect.add(new Option('不区分平台', '0'),0);    
            
       
        <%} %> 
    
        <%if(!HiddenResType)
          { %>
             ///获取资源类型节点
            var restypeselect=$("#myselectrestype")[0];
            var selectedrestype=$(".inputrestypeselect").val().split(',');

            if(selectedrestype.length!=0)
            {
            
                for (var i = 0; i < restypeselect.options.length; i++) {
                    if(selectedrestype.contains(restypeselect.options[i].value))
                        restypeselect.options[i].selected=true;
    
                }
            }
        <% } %>
        <%if(!HiddenResSource) {%>
            var ressoures = $(".inputressourceselect").val().split(',');
            var ressourceselect = $("#myselectressource")[0];
            for (var  i = 0; i < ressourceselect.options.length; i++) {
                 if(ressoures.contains( ressourceselect.options[i].value))
                 {
                    ressourceselect.options[i].selected=true;
                 }

            }


        <%} %>
        <%if(!HiddenResGrant) {%>
              ///是否授权
            var isgrant=$(".inputgrantselect").val();
            var grantselect=$("#myselectresgrant")[0];
            for (var i = 0; i < grantselect.options.length; i++) {
                if(isgrant==grantselect.options[i].value)
                {
                    grantselect.options[i].selected=true;
                }
            }
        <%} %>
         <%if(!HiddenPlat){ %>
            var plats = $(".inputplatformselect").val().split(',');
            var platselect = $("#myselectplat")[0];
            for (var  i = 0; i < platselect.options.length; i++) {
                for (var j = 0; j < plats.length; j++) {
                    if (plats[j] == platselect.options[i].value&&!platselect.options[i].disabled) {
                        platselect.options[i].selected = true;
                        break;
                    }
                }

            }
       <%} %>
        
       <%if(!HiddenCustomType){ %>
            var customtype = $(".inputcustomtypeselect").val().split(',');
            var customtypeselect = $("#myselectcustomtype")[0];
             for (var  i = 0; i < customtypeselect.options.length; i++) {
                if(customtype.contains(customtypeselect.options[i].value))
                 {
                    customtypeselect.options[i].selected = true;
                 }

            }

       <%} %>

       
       <%if(!HiddenResType)
         {%>
            ///初始化资源类型选项
            restypebox= $("#myselectrestype").multiselect({
            multiple:false,
            noneSelectedText: "选择资源类型",
            header: false,
            height:275,
            minWidth: 170,
            selectedList: 100,
            beforeclose: function () {
                if ($("#myselectrestype").val() == null) {
                    alert("至少选择一个选项！");
                    return false;
                }
                $(".inputrestypeselect").val($("#myselectrestype").val() == null ? "-1" : $("#myselectrestype").val());
               

            } 
        });
        <% } %>

 


       <%if(!HiddenPlat){ %>
        ///初始化platbox
        platbox = $("#myselectplat").multiselect({
            multiple:!<%=IsPlatSingle.ToString().ToLower()%>,
            noneSelectedText: "选择平台",
            header: false,
            height:300,
            minWidth: 140,
            selectedList: 100,
            beforeclose: function () {
                
                if ($("#myselectplat").val() == null) {
                    alert("至少选择一个选项！");
                    return false;
                }
               
                $(".inputplatformselect").val($("#myselectplat").val() == null ? "-1" : $("#myselectplat").val());
               

            } 

        });

       <%} %>

       <%if(!HiddenResSource){ %>
            ressourcebox = $("#myselectressource").multiselect({
                multiple:!<%=IsResSourceSingle.ToString().ToLower()%>,
                noneSelectedText: "选择来源",
                height:275,
                minWidth: 170,
                selectedList: 100,
                uncheckAllText: '清空',
                showCheckAll: true,
                checkAllText:'',
                beforeclose: function () {
                
                    if ($("#myselectressource").val() == null) {
                        alert("至少选择一个选项！");
                        return false;
                    }
               
                    $(".inputressourceselect").val($("#myselectressource").val() == null ? "-2" : $("#myselectressource").val());
               

                } 
            }).multiselectfilter({ label: "搜索", width: 50 });

       <%} %>

       <%if(!HiddenCustomType){ %>
             customtypebox = $("#myselectcustomtype").multiselect({
                    multiple:!<%=IsCustomTypeSingle.ToString().ToLower()%>,
                    noneSelectedText: "选择类型",
                    header: true,
                    height:225,
                    minWidth: 210,
                    selectedList: 100,
                    beforeclose: function () {
                
                        if ($("#myselectcustomtype").val() == null) {
                            alert("至少选择一个选项！");
                            return false;
                        }
               
                        $(".inputcustomtypeselect").val($("#myselectcustomtype").val() == null ? "-1" : $("#myselectcustomtype").val());
               

                    } 
                }).multiselectfilter({ label: "搜索", width: 80 });

        <%} %>
        <%if(!HiddenResGrant){ %>
        grantbox=$("#myselectresgrant").multiselect({
                    multiple:false,
                    noneSelectedText: "选择授权",
                    header:false,
                    height:75,
                    minWidth: 150,
                    selectedList: 100,
                    beforeclose: function () {
                        if ($("#myselectresgrant").val() == null) {
                            alert("至少选择一个选项！");
                            return false;
                        }
                        $(".inputgrantselect").val($("#myselectresgrant").val() == null ? "-2" : $("#myselectresgrant").val());
                   } 
                });


        <%} %>
        
        ///初始位置设置
         setDivPos();
        if($("#btnComparetime")[0])
        {
              
            showCompareTxt();
              
        }
        ///其他设置
        if($("#timetxt")[0])
        {
            var fromtime = $(".fromtime").val();
            var totime = $(".totime").val();
            if (fromtime != "" && totime != "") {
                $(".inputbegintime").val(fromtime);
                $(".inputendtime").val(totime);
                $("#timetxt").text(fromtime.substring(2, fromtime.length).replace(/-/g, '/') + "至" + totime.substring(2, fromtime.length).replace(/-/g, '/'));
            }
        }

          $(document).bind('mousedown', function (e) {
            var timeid = $("#timeid");
            var singletimeid=$("#singletimeid");
            if(timeid[0])
            {
                var isopen = timeid.is(":visible");
                var timebtn=$("#timebtn");
                if (isopen && !$.contains(timeid[0], e.target) && !$.contains(timebtn[0], e.target) && e.target !== timebtn[0]) {
                    var fromtime = $(".fromtime").val();
                    var totime = $(".totime").val();
                    if (fromtime != "" && totime != "") {
                        var starttime1 = fromtime.replace(/-/g, '/');
                        var endtime1 = totime.replace(/-/g, '/');
                        if (Date.parse(starttime1) > Date.parse(endtime1)) {
                            alert("日期范围有问题！"); 
                            return;
                        }
                        $(".inputbegintime").val(fromtime);
                        $(".inputendtime").val(totime);
                        $("#timetxt").text(fromtime.substring(2, fromtime.length).replace(/-/g, '/') + "至" + totime.substring(2, fromtime.length).replace(/-/g, '/'));
                    }
                    timeid.hide();
                    setStart();
                    clearCompare();
                }
            }
            var compareTimeDiv = $("#compareTimeDiv");
            var isopen = compareTimeDiv.is(":visible");
            var btnComparetime=$("#btnComparetime");
            if (isopen && !$.contains(compareTimeDiv[0], e.target) && !$.contains(btnComparetime[0], e.target) && e.target !== btnComparetime[0]) {
                var comparetime = $(".txtCompareTime").val();
                if($.trim(comparetime)!="")
                {
                     sureCompareTime();
                }
                compareTimeDiv.hide();
            }
            
          });
           $(window).bind("resize",function(){
              if($("#timebtn")[0])
              {
                 setDivPos();
              }
             
              if(restypebox)
              { 
                  restypebox.multiselect('refreshmenupos');
			  }
                
              
              if(platbox)
              {
                
                platbox.multiselect('refreshmenupos');
              }
               if(ressourcebox)
              {
                
                ressourcebox.multiselect('refreshmenupos');
              }
              if(customtypebox)
              {
               
                customtypebox.multiselect('refreshmenupos'); 
              }
              if(grantbox)
              {
                  grantbox.multiselect('refreshmenupos'); 
              }
              
            
          });
         $("#btnComparetime").bind('click', function () {
            
            var compareTimeDiv = $("#compareTimeDiv");
            var isopen = compareTimeDiv.is(":visible");
            compareTimeDiv.show();

         });
           ///提交表单检查并提交
        $("#btnpost").bind("click", function () {
            checkCondition();
            if(ischecked)
                 checkedforcontrol();
            if(ischecked)
                $("#form1").submit();
        });
        ///多个时间点击
        $("#timebtn").bind('click', function () {
            var timeid = $("#timeid");
            var isopen = timeid.is(":visible");
            if (isopen) {
                var fromtime = $(".fromtime").val();
                var totime = $(".totime").val();
                if (fromtime != "" && totime != "") {
                    $(".inputbegintime").val(fromtime);
                    $(".inputendtime").val(totime);
                    $("#timetxt").text(fromtime.substring(2, fromtime.length).replace(/-/g, '/') + "至" + totime.substring(2, fromtime.length).replace(/-/g, '/'));
                }
                timeid.hide();
            }
            else
                timeid.show();
        });


     });


    function setDivPos() {

        ///设置时间div 的位置
        var timebtn = $("#timebtn");
        if (timebtn[0]) {
            var pos = timebtn.offset();
            $("#timeid").css({ top: pos.top + $("#timebtn").outerHeight() + 6, left: pos.left })
            ///设置宽度
            var m = $("#timeid");
            width = timebtn.outerWidth() -
				    parseInt(m.css('padding-left'), 10) -
				    parseInt(m.css('padding-right'), 10) -
				    parseInt(m.css('border-right-width'), 10) -
				    parseInt(m.css('border-left-width'), 10);

            m.width(width || timebtn.outerWidth());
        }
        ///若存在对比时间
        var comparebtn=$("#btnComparetime");
        if(comparebtn[0])
        {
            var pos = comparebtn.offset();
            $("#compareTimeDiv").css({ top: pos.top + comparebtn.outerHeight()+6, left: pos.left });
            var m=$("#compareTimeDiv");
            width = comparebtn.outerWidth() -
				    parseInt(m.css('padding-left'), 10) -
				    parseInt(m.css('padding-right'), 10) -
				    parseInt(m.css('border-right-width'), 10) -
				    parseInt(m.css('border-left-width'), 10);
            m.width(width || comparebtn.outerWidth());
        }
        
    }
    ///内部检查
    function checkedforcontrol()
    {
         if($("#timebtn")[0])
            {
                var starttime1=$.trim($(".inputbegintime").val()).replace(/-/g,'/');
                var endtime1=$.trim($(".inputendtime").val()).replace(/-/g,'/');
                if (Date.parse(starttime1) > Date.parse(endtime1)) {
                    alert("日期范围有问题！");
                    ischecked=false;
                    return;
                }
            }
            
            if($("#myselectrestype")[0])
            {
                if( $(".inputrestypeselect").val()=="-1")
                {
                    alert("请选择资源类型！");
                    ischecked=false;
                    return;
                }
            }
            if($("#myselectplat")[0])
            {
                if( $(".inputplatformselect").val()=="-1")
                {
                    alert("请选择平台！");
                    ischecked=false;
                    return;
                }
             
            }
            if($("#myselectressource")[0])
            {
                if( $(".inputressourceselect").val()=="-2")
                {
                    alert("请选择来源！");
                    ischecked=false;
                    return;
                }
             
            }
            if($("#myselectresgrant")[0])
            {
                if( $(".inputgrantselect").val()=="-2")
                {
                    alert("请选择是否授权！");
                    ischecked=false;
                    return;
                }
            }
//            if($("#myselectcustomtype")[0])
//            {
//                if( $(".inputcustomtypeselect").val()=="-1")
//                {
//                    alert("请选择类型！");
//                    ischecked=false;
//                    return;
//                }
//               
//            }
            ischecked=true;
        
    }
    
</script>
<div class="head">
    <%if (!HiddenResType)
      {%>
    <div class="divitem" style="width: <%= ResTypeWidth %>;">
        <select id="myselectrestype" size="12" style="display: none" multiple="multiple">

               <% URLoginService loginService = new URLoginService();
                  foreach (ResourceType resType in loginService.GetResourceTypes())
                  { %>
                  <option value="<%= resType.TypeID %>"><%= resType.TypeName %></option>
          <%} %>

        </select>
    </div>
    <% } %>
    <%if (!HiddenResSource)
      { %>
    <div class="divitem" style="width: <%=ResSourceWidth%>;">
        <select id="myselectressource" style="display: none" size="12" multiple="multiple">
            <% foreach (ProjectSource item in ProjectList)
               {
                   if (item.ProjectSourceType == ProjectSourceTypeOptions.None
                       || (VersionType == 1 && item.ProjectSourceType == ProjectSourceTypeOptions.Oversea)
                       || (VersionType == 2 && item.ProjectSourceType == ProjectSourceTypeOptions.Traditional)
                       || (VersionType == 0 && item.ProjectSourceType == ProjectSourceTypeOptions.Domestic))
                   {                               
            %>
            <option value="<%= item.ProjectSourceID %>">
                <%= item.Name %>
            </option>
            <%
                }
               } 
            %>
        </select>
    </div>
    <%} %>
    <%if (!HiddenPlat)
      { %>
    <div class="divitem" style="width: <%=PlatWidth%>;">
        <select id="myselectplat" style="display: none" size="12" multiple="multiple">
        </select>
    </div>
    <%} %>
    <%if (!HiddenCustomType)
      { %>
    <div class="divitem" style="width: <%=CustomTypeWidth%>;">
        <select id="myselectcustomtype" style="display: none" size="12" multiple="multiple">
            <%   foreach (var item in CustomTypeSource.Keys)
                 {%>
            <option value="<%=item %>">
                <%=CustomTypeSource[item] %></option>
            <%} %>
        </select>
    </div>
    <%} %>
    <%if (!HiddenResGrant)
      { %>
    <div class="divitem" style="width: <%=ResGrantWidth%>;">
        <select id="myselectresgrant" style="display: none" size="12" multiple="multiple">
            <option value="-1">全部</option>
            <option value="1">已授权</option>
            <option value="0">未授权</option>
        </select>
    </div>
    <%} %>
    <%if (IsHasCheckedBox)
      { %>
    <div class="divitem" style="width: <%= CheckBoxWidth%>;">
        <asp:CheckBox ID="checkbox" Text="包含更新" runat="server" />
    </div>
    <%} %>
    <%if (!HiddenTime)
      { %>
    <div class="divitem" style="width: <%=TimeWidth%>">
        <button value="选择时间" id="timebtn" class="ui-multiselect ui-widget ui-state-default ui-corner-all"
            type="button" aria-haspopup="true" tabindex="0" style="width: 180px;">
            <span class="ui-icon ui-icon-triangle-2-n-s"></span><span id="timetxt">选择时间</span>
        </button>
    </div>
    <%} %>
    <%if (!HiddenCompareTime)
      { %>
    <div class="divitem" style="width: <%=CompareTypeWidth%>">
        <button value="选择对比时间" id="btnComparetime" class="ui-multiselect ui-widget ui-state-default ui-corner-all"
            type="button" aria-haspopup="true" tabindex="0" style="width: 155px;">
            <span class="ui-icon ui-icon-triangle-2-n-s"></span><span id="txtCompareTimeSpan">选择对比时间</span>
        </button>
    </div>
    <%} %>
    <div class="divitem" style="position: absolute; right: 10px; margin-right: 1px; width: <%=BtnWidth%>;">
        <button id="btnpost" class="ui-multiselect ui-widget ui-state-default ui-corner-all"
            type="button" aria-haspopup="true" tabindex="0">
            <span id="Span1">查询</span>
        </button>
    </div>
</div>
<input id="inputrestypeselect" class="inputrestypeselect" runat="server" value="-1"
    type="hidden" />
<input id="inputplatformselect" class="inputplatformselect" runat="server" type="hidden"
    value="-1" />
<input id="inputressourceselect" class="inputressourceselect" runat="server" type="hidden"
    value="-2" />
<input id="inputcustomtypeselect" class="inputcustomtypeselect" runat="server" type="hidden"
    value="-1" />
<input id="inputbegintime" class="inputbegintime" runat="server" type="hidden" />
<input id="inputendtime" class="inputendtime" runat="server" type="hidden" />
<input id="inputisfirstload" runat="server" type="hidden" value="0" />
<input id="inputgrantselect" class="inputgrantselect" runat="server" type="hidden"
    value="-1" />
<input id="inputcomparetimetype" class="inputcomparetimetype" runat="server" type="hidden"
    value="-1" />
<div id="timeid" class="ui-multiselect-menu ui-widget ui-widget-content ui-corner-all"
    style="none: block; top: 37px;">
    <%if (!HiddenQuickTime)
      {%>
    <ul class="ui-helper-reset">
        <li style="margin-bottom: 2px;">
            <label class="ui-corner-all ui-state-active" style="display: block; height: 30px;
                padding-top: 3px; padding-bottom: 3px" title="">
                <span style="margin: 3px; color: Black; cursor: pointer;" onclick="setQuickTime(1)">
                    近7天</span> <span style="color: Black; cursor: pointer; margin: 3px;" onclick="setQuickTime(2)">
                        近30天</span>
                <select id="myQuickMonth" onchange="changeSelectMonth()">
                    <%   foreach (var item in DateMonthList.Keys)
                         {%>
                    <option value="<%=(int)item %>">
                        <%=DateMonthList[item]%></option>
                    <%} %>
                </select>
            </label>
        </li>
    </ul>
    <%} %>
    <div class="ui-widget-header ui-corner-all ui-multiselect-header ui-helper-clearfix ui-multiselect-hasfilter">
        <div class="ui-multiselect-filter" style="margin-right: 0px;">
            <span>从</span>
            <input type="text" id="fromtime" class="Wdate fromtime" onclick="WdatePicker()" runat="server" />
        </div>
    </div>
    <div class="ui-widget-header ui-corner-all ui-multiselect-header ui-helper-clearfix ui-multiselect-hasfilter">
        <div class="ui-multiselect-filter" style="margin-right: 0px;">
            <span>至</span>
            <input type="text" id="totime" class="Wdate totime" onclick="WdatePicker()" runat="server" />
        </div>
    </div>
</div>
<div id="compareTimeDiv" class="ui-multiselect-menu ui-widget ui-widget-content ui-corner-all ui-multiselect-single"
    style="display: none; top: 37px;">
    <ul class="ui-helper-reset">
        <li style="margin-bottom: 2px;">
            <label class="ui-corner-all ui-state-active" style="display: block; height: 20px;
                padding-top: 5px" title="">
                <span style="margin: 5px; color: Black; cursor: pointer;" onclick="setCompareType(1)">
                    上周</span> <span style="color: Black; cursor: pointer; margin: 5px;" onclick="setCompareType(2)">
                        上月</span> <span style="color: Black; cursor: pointer; margin: 5px;" onclick="setCompareType(0)">
                            不对比</span>
            </label>
        </li>
    </ul>
    <div class="ui-widget-header ui-corner-all ui-helper-clearfix ui-multiselect-hasfilter">
        <div class="ui-multiselect-filter" style="margin-right: 0px; padding-top: 10px;">
            <span>自定义对比开始时间:</span>
            <input type="text" id="txtCompareTime" style="width: 100px;" class="txtCompareTime"
                onclick="WdatePicker()" runat="server" />
            <span onclick="sureCompareTime()" style="cursor: pointer;">确定</span>
        </div>
    </div>
</div>
<script type="text/javascript">

    //对比时间控件
    function sureCompareTime() {
        $(".inputcomparetimetype").val(3);
        showCompareTxt();
    }
    ///选择对比周期
    function setCompareType(a) {
        $(".inputcomparetimetype").val(a);
        showCompareTxt();

    }
    function showCompareTxt() {
        var a = $(".inputcomparetimetype").val();
        var begintime = new Date(Date.parse($(".fromtime").val().replace(/-/g, '/')));
        var endtime = new Date(Date.parse($(".totime").val().replace(/-/g, '/')));
        var spanday = endtime.getTime() - begintime.getTime();
        if (spanday < 0) {
            alert("基准时间范围有问题");
            return;
        }
        if (a == 1) {
            var beginCompare = new Date(begintime.getTime());

            beginCompare.setDate(beginCompare.getDate() - 7);
            var endCompare = new Date(beginCompare.getTime() + spanday);
            $("#txtCompareTimeSpan").text(beginCompare.format("yy/MM/dd") + "至" + endCompare.format("yy/MM/dd"));
        }
        else if (a == 2) {
            var beginCompare = new Date(begintime.getTime());
            beginCompare.setMonth(beginCompare.getMonth() - 1);
            var endCompare = new Date(beginCompare.getTime() + spanday);
            $("#txtCompareTimeSpan").text(beginCompare.format("yy/MM/dd") + "至" + endCompare.format("yy/MM/dd"));
        }
        else if (a == 0) {
            $("#txtCompareTimeSpan").text("选择对比时间");
        }
        else if (a == 3) {
            var compareTimeStr = $(".txtCompareTime").val().replace(/-/g, '/');
            var starttimestr = $.trim($(".inputbegintime").val()).replace(/-/g, '/');
            var endtimestr = $.trim($(".inputendtime").val()).replace(/-/g, '/');
            if (compareTimeStr != "" && starttimestr != "" && endtimestr != "") {
                var compareDate = Date.parse(compareTimeStr);
                var startTime = Date.parse(starttimestr);
                var endTime = Date.parse(endtimestr);
                var spanTime = endTime - startTime;
                var compareendtimeint = compareDate + spanTime;
                var compareendtime = new Date(compareendtimeint);
                $("#txtCompareTimeSpan").text(new Date(compareDate).format("yy/MM/dd") + "至" + compareendtime.format("yy/MM/dd"));
            }
            else {
                $("#txtCompareTimeSpan").text("选择对比时间");
                $(".inputcomparetimetype").val(0);
            }
        }
        $("#compareTimeDiv").hide();

    }

    ///快捷选择日期
    function changeSelectMonth() {
        var month = $("#myQuickMonth").val();
        if (month >= 0) {
            var nowTime = new Date();
            var nextQuickTime = new Date(nowTime.getFullYear(), month, 20);
            var nowQuickTime = new Date(nextQuickTime.getTime());
            nowQuickTime.setMonth(nowQuickTime.getMonth() - 1);
            nowQuickTime.setDate(nowQuickTime.getDate() + 1);
            if (nowQuickTime.getTime() > nowTime.getTime()) {
                nowQuickTime.setFullYear(nowQuickTime.getFullYear() - 1);
                nextQuickTime.setFullYear(nextQuickTime.getFullYear() - 1);
            }
            $(".inputbegintime").val(nowQuickTime.format("yyyy-MM-dd"));
            $(".inputendtime").val(nextQuickTime.format("yyyy-MM-dd"));
            $(".fromtime").val(nowQuickTime.format("yyyy-MM-dd"));
            $(".totime").val(nextQuickTime.format("yyyy-MM-dd"));
            $("#timetxt").text(nowQuickTime.format("yy/MM/dd") + "至" + nextQuickTime.format("yy/MM/dd"));
            $("#timeid").hide();
            clearCompare();
        }
    }
    ///快捷选择上周
    function setQuickTime(type) {
        var nowQuickTime = new Date();
        var nextQuickTime = new Date();
        nextQuickTime.setDate(nextQuickTime.getDate() - 1);
        //上周
        if (type == 1) {
            nowQuickTime.setDate(nowQuickTime.getDate() - 7);

            //上月
        } else {
            nowQuickTime.setDate(nowQuickTime.getDate() - 31);
        }

        $(".inputbegintime").val(nowQuickTime.format("yyyy-MM-dd"));
        $(".inputendtime").val(nextQuickTime.format("yyyy-MM-dd"));
        $(".fromtime").val(nowQuickTime.format("yyyy-MM-dd"));
        $(".totime").val(nextQuickTime.format("yyyy-MM-dd"));
        $("#timetxt").text(nowQuickTime.format("yy/MM/dd") + "至" + nextQuickTime.format("yy/MM/dd"));
        $("#timeid").hide();
        setStart();
        clearCompare();

    }

    ///让其归到月份选项
    function setStart() {
        if ($("#myQuickMonth")[0])
            $("#myQuickMonth").val(-1);
    }
    ///清除比较
    function clearCompare() {
        $(".inputcomparetimetype").val("0");
        $("#txtCompareTimeSpan").text("选择对比时间");
    }
    ///时间转化方法
    Date.prototype.format = function (format) // 
    {
        var o = {
            "M+": this.getMonth() + 1, //month
            "d+": this.getDate(), //day
            "h+": this.getHours(), //hour
            "m+": this.getMinutes(), //minute
            "s+": this.getSeconds(), //second
            "q+": Math.floor((this.getMonth() + 3) / 3), //quarter
            "S": this.getMilliseconds() //millisecond
        }
        if (/(y+)/.test(format)) format = format.replace(RegExp.$1,
            (this.getFullYear() + "").substr(4 - RegExp.$1.length));
        for (var k in o)
            if (new RegExp("(" + k + ")").test(format))
                format = format.replace(RegExp.$1,
              RegExp.$1.length == 1 ? o[k] :
                ("00" + o[k]).substr(("" + o[k]).length));
        return format;
    }

</script>
