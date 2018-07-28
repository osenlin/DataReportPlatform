<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="HeadLinkTagControl.ascx.cs"
    Inherits="net91com.Stat.Web.Reports.Controls.HeadLinkTagControl" %>
<%@ Register Src="LinkTagForAll.ascx" TagName="LinkTag" TagPrefix="uc3" %>
<%@ Import Namespace="net91com.Core" %>
<link href="../../css/headcss/head.css?rd=<%= net91com.Stat.Web.Reports.Services.Utility.CssVersion %>"
    rel="stylesheet" type="text/css" />
<link href="../../css/headcss/jquery-ui.css" rel="stylesheet" type="text/css" />
<link href="../../css/headcss/style.css?rd=<%= net91com.Stat.Web.Reports.Services.Utility.CssVersion %>"
    rel="stylesheet" type="text/css" />
<link href="../../css/headcss/jquery.multiselect.css?rd=<%= net91com.Stat.Web.Reports.Services.Utility.CssVersion %>"
    rel="stylesheet" type="text/css" />
<link href="../../css/headcss/jquery.multiselect.filter.css" rel="stylesheet" type="text/css" />
<script src="../../Scripts/HeadScript/jquery.js" type="text/javascript"></script>
<script src="../../Scripts/HeadScript/jquery-ui.min.js" type="text/javascript"></script>
<script src="../../Scripts/HeadScript/jquery.multiselect.js?rd=<%= net91com.Stat.Web.Reports.Services.Utility.JsVersion %>"
    type="text/javascript" charset="GBK"></script>
<script src="../../Scripts/HeadScript/jquery.multiselect.filter.js?rd=<%= net91com.Stat.Web.Reports.Services.Utility.JsVersion %>"
    type="text/javascript" charset="GBK"></script>
<script src="../../My97DatePicker/WdatePicker.js" type="text/javascript"></script>
<script type="text/javascript">
    <%=DataSource %>
    var allplats = <%=AllPlats %>;
    var ischecked=false;
    var canviewplats = [];
    var canviewversion = [];
    var canviewfunction = [];
    var ischangedsoft = "";
    var ischangedplat = "";
    var ischangedversion = "";
    var ischangedfunction = "";
    var softbox;
    var platbox;
    var versionbox;
    var functionbox;
    var periodbox;
    var customtypebox;
    var periodbox; 
    Array.prototype.contains = function(item) {
        for (var i = 0; i < this.length; i++) {
            if (this[i] == item) {
                return true;
            }
        }
        return false;
    };
     ///只是用于平台
    Array.prototype.delRepeat = function() {
        var newArray = [];
        for (var i = 0; i < this.length; i++) {
            if (newArray.contains(this[i]))
                continue;
            else {
                newArray.push(this[i]);
            }
        }
        return newArray;
    };
    ///获取平台数组，并重绘dom
    function drawplatsdoms(mysofts, first) {
        var platselect = $("#myselectplat")[0];
        var myplatforms = new Array();
        for (var i = 0; i < softs.length; i++) {
            for (var j = 0; j < mysofts.length; j++) {
                if (mysofts[j] == softs[i].id) {
                    myplatforms = myplatforms.concat(softs[i].platforms);
                  
                }

            }
        }
        canviewplats=myplatforms.delRepeat();
        var plat=$("#myselectplat")[0];
      
         ///不支持的平台disable掉
        for (var i = 0; i < plat.options.length; i++) {
             plat.options[i].disabled=true;
             plat.options[i].selected=false;
             for (var j = 0; j < canviewplats.length; j++) {
                  if(canviewplats[j]== plat.options[i].value||plat.options[i].value=="0")
                    plat.options[i].disabled=false;
            }
        }
       
        ///该勾选的要勾选上
        plats = $(".inputplatformselect").val().split(',');
        for (var  i = 0; i < platselect.options.length; i++) {
            for (var j = 0; j < plats.length; j++) {
                if (plats[j] == platselect.options[i].value&&!platselect.options[i].disabled) {
                    platselect.options[i].selected = true;
                }
            }

        }
        ///若什么都没选中
        if(platselect.value==""&&platselect.options.length!=0)
        {
              for (var  i = 0; i < platselect.options.length; i++) {
                    if(!platselect.options[i].disabled)
                    {
                        platselect.options[i].selected = true;
                        break;
                    }
              }
        }
        ///第一次不需要这个
        if (!first) {
            platbox.multiselect('refresh');
            $(".inputplatformselect").val($("#myselectplat").val() == null ? "-1" : $("#myselectplat").val());
        }
        ///真正的数据，这句不能删去
        plats = $(".inputplatformselect").val().split(',');
        ///重绘版本和功能
        if (plats.length != 0) {
         <%if(!HiddenVersion){ %>
            drawversionsdoms(mysofts[0], plats[0]);
            <%} %>
        <%if(!HiddenFunction){ %>
            drawfunctionsdoms(mysofts[0], plats[0]);
            <%} %>
        }
        ///相当于重置版本和功能
        else {
                <%if(!HiddenVersion){ %>
                    drawversionsdoms(-1, -1);
                    <%}%>
                <%if(!HiddenFunction){ %>
                    drawfunctionsdoms(-1, -1);
                <%} %>
        }
    }

    ///获取版本数组，重绘dom
    function drawversionsdoms(softid, platid,first) {
        var myselectversion = $("#myselectversion")[0];
        myselectversion.options.length = 0;
        $.ajax({
            type: "get",
            url: "/Reports/HanderForOption.ashx?action=getversionforsession&soft=" + softid + "&platform=" + platid+"&versiontype="+<%=VersionType %>,
            success: function (data) {
                var mydata = eval("(" + data + ")");
                canviewversion = mydata;
                 <%if(IsHasNoVersion) {%>
                    myselectversion.add(new Option('不区分版本', '0'));
                 <%} %> 
                for (var  i = 0; i < mydata.length; i++) {
                    myselectversion.options.add(new Option(mydata[i].VersionCode, mydata[i].VersionID));
                }
                
                versions = $(".inputversionselect").val().split(',');
                for (var  i = 0; i < myselectversion.options.length; i++) {
                    for (var j = 0; j < versions.length; j++) {
                        if (versions[j] == myselectversion.options[i].value) {
                            myselectversion.options[i].selected = true;
                        }
                    }

                }
                ///第一次不需要这个
                if (versionbox) {
                    versionbox.multiselect('refresh');
                    $(".inputversionselect").val($("#myselectversion").val() == null ? "-1" : $("#myselectversion").val());
                }


            }
        });
    }


    ///获取功能数组，重绘dom
    function drawfunctionsdoms(softid, platid, first) {
        var myselectfunction = $("#myselectfunction")[0];
        myselectfunction.options.length = 0;
        $.ajax({
            type: "get",
            url: "/Reports/HanderForOption.ashx?action=getfunctionsforstat&soft=" + softid + "&plat=" + platid,
            success: function (data) {
                var mydata = eval("(" + data + ")");
                canviewfunction = mydata;
                <%if(IsHasNoFunction) {%>
                    myselectfunction.add(new Option('不区分功能', '0'));
                 <%} %> 
                for (var  i = 0; i < mydata.length; i++) {
                    myselectfunction.options.add(new Option(mydata[i].FunctionName, mydata[i].FunctionID));
                }
                 
                functions = $(".inputfunctionselect").val().split(',');
                for (var  i = 0; i < myselectfunction.options.length; i++) {
                    for (var j = 0; j < functions.length; j++) {
                        if (functions[j] == myselectfunction.options[i].value) {
                            myselectfunction.options[i].selected = true;
                        }
                    }
                }
                ///第一次不需要这个
                if (functionbox) {
                    functionbox.multiselect('refresh');
                    $(".inputfunctionselect").val($("#myselectfunction").val() == null ? "-1" : $("#myselectfunction").val());
                }
            }
        });
    }



    $(function () {
        

        var softselect = $("#myselectsoft")[0];

        //平台加载上去
        var platselect=$("#myselectplat")[0];

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


        <%if(!HiddenPlat) {%>
         for (var  i = 0; i < allplats.length; i++) {
            platselect.add(new Option(allplats[i].name, allplats[i].id));
        }
        <%} %>
          ///加上不区分平台
        <%if(IsHasNoPlat&&!HiddenPlat) {%>
            platselect.add(new Option('不区分平台', '0'),0);
            <%} %> 
        //默认选中项目,软件，平台，版本，功能
        var selectedsoft = $(".inputsoftselect").val().split(',');

        if (selectedsoft.length != 0) {
            for (var  i = 0; i < softselect.options.length; i++) {
                if (selectedsoft.contains(softselect.options[i].value)) {
                    softselect.options[i].selected = true;
                }
            }
       <%if(!HiddenPeriod){ %>
            var period=$(".inputperiod").val();
            var periodoptions=$("#myselectperiod")[0];
         
            for (var  i = 0; i < periodoptions.options.length; i++) {
                if (periodoptions.options[i].value==period) {
                    periodoptions.options[i].selected = true;
                }
            }

        <%} %>
        <%if(!HiddenPlat) {%>
        ///重绘平台
        drawplatsdoms(selectedsoft, true);

        <%} %>
       }


        ///初始化selectbox
        softbox = $("#myselectsoft").multiselect({
            multiple:!<%=IsSoftSingle.ToString().ToLower() %>,
            noneSelectedText: "选择软件",
            header: "",
            height:275,
            minWidth: 170,
            selectedList: 100,
            beforeclose: function () {
                if ($("#myselectsoft").val() == null) {
                    alert("至少选择一个选项！");
                    return false;
                }

                $(".inputsoftselect").val($("#myselectsoft").val().join(','));
                
                <%if(!HiddenPlat){ %>
                if (ischangedsoft != $(".inputsoftselect").val()) {
                    ///软件一变，平台重绘，版本重绘，功能重绘
                    //平台重绘
                    drawplatsdoms($("#myselectsoft").val());

                    softids_LinkTag= $(".inputsoftselect").val();
                    clear();
                    
                    hasloadData_LinkTag=true;
                }
                <%} %>

            },
            beforeopen: function () {
                ischangedsoft = $(".inputsoftselect").val();
            }

        }).multiselectfilter();
       <%if(!HiddenPlat){ %>

        ///初始化platbox
        platbox = $("#myselectplat").multiselect({
            multiple:!<%=IsPlatSingle.ToString().ToLower()%>,
            noneSelectedText: "选择平台",
            header: false,
            minWidth: 140,
            selectedList: 100,
            height:330,
            beforeclose: function () {
               
                if ($("#myselectplat").val() == null) {
                    alert("至少选择一个选项！");
                    return false;
                }
                $(".inputplatformselect").val($("#myselectplat").val() == null ? "-1" : $("#myselectplat").val());
               
                if (ischangedplat != $(".inputplatformselect").val()) {
                     ///设置渠道参数
                    platforms_LinkTag= $(".inputplatformselect").val();
                    ///设置平台他的平台传入底层
                    $("#<%=LinkTag1.ClientID %>_hidplatform").val(platforms_LinkTag);
                    clear();
                    hasloadData_LinkTag=true;
                   <%if(!HiddenVersion){ %>
                    ///获取版本
                    drawversionsdoms($("#myselectsoft").val()[0], $(".inputplatformselect").val().split(',')[0]);
                    <%} %>
                    <%if(!HiddenFunction){ %>
                    drawfunctionsdoms($("#myselectsoft").val()[0], $(".inputplatformselect").val().split(',')[0]);
                    <%} %>

                }


            },
            beforeopen: function () {
                if (canviewplats.length == 0) {
                    alert("请先选择软件！");
                    return false;
                }
                ischangedplat = $(".inputplatformselect").val();
            }

        });
        <%if(!HiddenVersion){ %>
        ///初始化版本
        versionbox = $("#myselectversion").multiselect({
            multiple:!<%=IsVersionSingle.ToString().ToLower()%>,
            noneSelectedText: "选择版本",
            header: "",
            height:275,
            minWidth: 170,
            selectedList: 100,
            beforeclose: function () {
                
                if ($("#myselectversion").val() == null) {
                    alert("至少选择一个选项！");
                    return false;
                }
              
                $(".inputversionselect").val($("#myselectversion").val() == null ? "-1" : $("#myselectversion").val());

            },
            beforeopen: function () {
                 <%if(!IsHasNoVersion) {%>
                    if (canviewversion.length == 0) {
                        alert("暂无版本！");
                        return false;
                    }
                 <%} %>
            }

        }).multiselectfilter();
        <%} %>
        <%if(!HiddenFunction){ %>
        ///初始化功能
        functionbox = $("#myselectfunction").multiselect({
            multiple:!<%=IsFunctionSingle.ToString().ToLower()%>,
            noneSelectedText: "选择功能",
            header: "",
            height:275,
            minWidth: 170,
            selectedList: 100,
            beforeclose: function () {
                
                if ($("#myselectfunction").val() == null) {
                    alert("至少选择一个选项！");
                    return false;
                }
              
                $(".inputfunctionselect").val($("#myselectfunction").val() == null ? "-1" : $("#myselectfunction").val());

            },
            beforeopen: function () {
                <%if(!IsHasNoFunction) {%>
                    if (canviewfunction.length == 0) {
                        alert("暂无功能！");
                        return false;
                    }
                <%} %>
            }

        }).multiselectfilter();
        <%} %>
        <%} %>

       <%if(!HiddenPeriod){ %>
        periodbox=$("#myselectperiod").multiselect({
            multiple:false,
            noneSelectedText:"选择周期",
            header:false,
            minWidth:150,
            selectedList:100,
            beforeclose:function()
            {
                $(".inputperiod").val($("#myselectperiod").val() == null ? "-1" : $("#myselectperiod").val());
            }
        });
        <%} %>


          <%if(!HiddenCustomType){ %>
             customtypebox = $("#myselectcustomtype").multiselect({
                    multiple:!<%=IsCustomTypeSingle.ToString().ToLower()%>,
                    noneSelectedText: "选择类型",
                    header: "",
                    height:275,
                    minWidth: 170,
                    selectedList: 100,
                    beforeclose: function () {
                        if ($("#myselectcustomtype").val() == null) {
                            alert("至少选择一个选项！");
                            return false;
                        }
                       $(".inputcustomtypeselect").val($("#myselectcustomtype").val() == null ? "-1" : $("#myselectcustomtype").val());
               

                    } 
                }).multiselectfilter();

        <%} %>

        ///渠道设置
         platforms_LinkTag=$(".inputplatformselect").val();
         softids_LinkTag=$(".inputsoftselect").val();

        ////------------其他设置------------------------

        setDivPos();
        if($("#btnComparetime")[0])
        {
              
            showCompareTxt();
              
        }
        ///时间设置
        var fromtime = $(".fromtime").val();
        var totime = $(".totime").val();
        if (fromtime != "" && totime != "") {
            $(".inputbegintime").val(fromtime);
            $(".inputendtime").val(totime);
            $("#timetxt").text(fromtime.substring(2, fromtime.length).replace(/-/g, '/') + "至" + totime.substring(2, fromtime.length).replace(/-/g, '/'));
        }

        $(document).bind('mousedown', function (e) {
            var timeid = $("#timeid");
            var isopen = timeid.is(":visible");
            var timebtn=$("#timebtn");
            if (isopen && !$.contains(timeid[0], e.target) && !$.contains(timebtn[0], e.target) && e.target !== timebtn[0]) {
                var fromtime = $(".fromtime").val();
                var totime = $(".totime").val();
                if (fromtime != "" && totime != "") {
                    var starttime1=fromtime.replace(/-/g,'/');
                    var endtime1=totime.replace(/-/g,'/');
                    if (Date.parse(starttime1) > Date.parse(endtime1)) {
                        alert("日期范围有问题！");
                        return;
                    }
                    $(".inputbegintime").val(fromtime);
                    $(".inputendtime").val(totime);
                    $("#timetxt").text(fromtime.substring(2, fromtime.length).replace(/-/g, '/') + "至" + totime.substring(2, fromtime.length).replace(/-/g, '/'));
                }
                setStart();
                timeid.hide();
                clearCompare();
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
        ///绑定窗体大小改变事件
        $(window).bind("resize",function(){
              if($("#timebtn")[0]||$("#comparediv")[0])
              {
                 setDivPos();
              }
              if(softbox)
              { 
                    
                   softbox.multiselect('refreshmenupos');
			  }
              if(platbox)
              {
                
                platbox.multiselect('refreshmenupos');
              }
               if(versionbox)
              {
                
                versionbox.multiselect('refreshmenupos');
              }
              if(functionbox)
              {
               
                functionbox.multiselect('refreshmenupos'); 
              }
              if(customtypebox)
              {
                 customtypebox.multiselect('refreshmenupos'); 
              }
              if(periodbox)
              {
                 periodbox.multiselect('refreshmenupos'); 
              }
            
        });

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




    });
    function checkedforcontrol()
    {
         if($("#timebtn")[0])
            { 
                var starttime1=$.trim($(".inputbegintime").val()).replace(/-/g,'/');
                var endtime1=$.trim($(".inputendtime").val()).replace(/-/g,'/');
                if (Date.parse(starttime1) > Date.parse(endtime1)) {
                    alert("日期范围有问题！");
                    ischecked=false;

                }
            }
            if($("#myselectsoft")[0])
            {
                if( $(".inputsoftselect").val()=="-1")
                {
                    alert("请选择软件！");
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
            if($("#myselectversion")[0])
            {
                if( $(".inputversionselect").val()=="-1")
                {
                    alert("请选择版本！");
                    ischecked=false;
                    return;
                }
             
            }
            if($("#myselectfunction")[0])
            {
                if( $(".inputfunctionselect").val()=="-1")
                {
                    alert("请选择功能！");
                    ischecked=false;
                    return;
                }
               
            }
        
    }

    function setDivPos()
    {
         ///设置时间div 的位置
        var timebtn = $("#timebtn");
        if(timebtn[0])
        {
            var pos = timebtn.offset();
            $("#timeid").css({ top: pos.top + $("#timebtn").outerHeight()+6, left: pos.left })
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
 

</script>
<div class="head">
    <div class="divitem" style="width: <%=SoftWidth%>;">
        <select id="myselectsoft" style="display: none" size="12" multiple="multiple">
            <%for (int index = 0; index < MySupportSoft.Count; index++)
              {%>
            <option value="<%= MySupportSoft[index].ID%>">
                <%= MySupportSoft[index].Name%></option>
            <%} %>
        </select>
    </div>
    <%if (!HiddenPlat)
      { %>
    <div class="divitem" style="width: <%=PlatWidth%>">
        <select id="myselectplat" style="display: none" size="12" multiple="multiple">
        </select>
    </div>
    <%if (!HiddenVersion)
      { %>
    <div class="divitem" style="width: <%=VersionWidth%>">
        <select id="myselectversion" style="display: none" size="12" multiple="multiple">
        </select>
    </div>
    <%} %>
    <%if (!HiddenFunction)
      { %>
    <div class="divitem" style="width: <%=FunctionWidth%>">
        <select id="myselectfunction" style="display: none" size="12" multiple="multiple">
        </select>
    </div>
    <%} %>
    <%} %>
    <uc3:LinkTag ID="LinkTags" runat="server" />
    <%if (ShowCheckBox)
      { %>
    <div class="divitem" style="width: <%=CheckboxWidth%>">
        <asp:CheckBox ID="checkbox" Text="" runat="server" />
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

    <%if (!HiddenCustomType)
      { %>
    <div class="divitem" style="width: <%=CustomTypeWidth%>;">
        <select id="myselectcustomtype" style="display: none" size="12" multiple="multiple">
            <%   foreach (var item in CustomTypeSource.Keys)
                 {%>
            <option value="<%= item %>">
                <%=CustomTypeSource[item] %></option>
            <%} %>
        </select>
    </div>
    <%} %>
    <%if (!HiddenPeriod)
      { %>
    <div class="divitem" style="width: <%=PeriodWidth%>;">
        <select id="myselectperiod" size="12" multiple="multiple">
            <%   foreach (var item in CustomPeriodDic.Keys)
                 {%>
            <option value="<%=(int)item %>">
                <%=CustomPeriodDic[item]%></option>
            <%} %>
        </select>
    </div>
    <%} %>
    <div class="divitem" style="position: absolute; right: 10px; margin-right: 1px; width: <%=BtnWidth%>;">
        <button id="btnpost" class="ui-multiselect ui-widget ui-state-default ui-corner-all"
            type="button" aria-haspopup="true" tabindex="0">
            <span id="Span1">查询</span>
        </button>
    </div>
</div>
<input id="inputsoftselect" class="inputsoftselect" runat="server" value="-1" type="hidden" />
<input id="inputplatformselect" class="inputplatformselect" runat="server" type="hidden"
    value="-1" />
<input id="inputversionselect" class="inputversionselect" runat="server" type="hidden"
    value="-1" />
<input id="inputfunctionselect" class="inputfunctionselect" runat="server" type="hidden"
    value="-1" />
<input id="inputbegintime" class="inputbegintime" runat="server" type="hidden" />
<input id="inputendtime" class="inputendtime" runat="server" type="hidden" />
<input id="inputisfirstload" runat="server" type="hidden" value="0" />
<input id="inputcustomtypeselect" class="inputcustomtypeselect" runat="server" type="hidden"
    value="-1" />
<input id="inputperiod" class="inputperiod" runat="server" type="hidden" value="0" />
 
<input id="inputcomparetimetype" class="inputcomparetimetype" runat="server" type="hidden" value="-1" />
<div id="timeid"  class="ui-multiselect-menu ui-widget ui-widget-content ui-corner-all"
    style="none: block; top: 37px;">
     <ul class="ui-helper-reset">

            <li style=" margin-bottom:2px; " > 
                <label  class="ui-corner-all ui-state-active" style="display:block;height:30px; padding-top:3px; padding-bottom:3px "  title="" >
                 <span style="margin:3px; color:Black;cursor: pointer;  " onclick="setQuickTime(1)" >近7天</span>
                 <span style="color:Black;cursor: pointer; margin:3px;  " onclick="setQuickTime(2)">近30天</span>
                 
                 <select id="myQuickMonth" onchange="changeSelectMonth()" >
                 <%   foreach (var item in DateMonthList.Keys)
                 {%>
                    <option value="<%=(int)item %>">
                        <%=DateMonthList[item]%></option>
                <%} %>
                  
                 </select>
                </label>
            </li>
           
     </ul>
    
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
<div id="compareTimeDiv"  class="ui-multiselect-menu ui-widget ui-widget-content ui-corner-all ui-multiselect-single"
    style="display: none; top: 37px;">

    <ul class="ui-helper-reset">
            <li style=" margin-bottom:2px; " > 
                <label  class="ui-corner-all ui-state-active" style="display:block;height:20px; padding-top:5px "  title="" >
                 <span style="margin:5px; color:Black;cursor: pointer;  " onclick="setCompareType(1)" >上周</span>
                 <span style="color:Black;cursor: pointer; margin:5px;  " onclick="setCompareType(2)">上月</span>
                 <span style="color:Black;cursor: pointer; margin:5px;  " onclick="setCompareType(0)">不对比</span>
                </label>
            </li>
           
     </ul>
   
    <div class="ui-widget-header ui-corner-all ui-helper-clearfix ui-multiselect-hasfilter">
        <div class="ui-multiselect-filter" style="margin-right: 0px; padding-top:10px;">
            <span>自定义对比开始时间:</span>
            <input type="text" id="txtCompareTime" style=" width:100px;" class="txtCompareTime" onclick="WdatePicker()" runat="server" /> <span onclick="sureCompareTime()" style=" cursor:pointer;">确定</span>
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
                 
    function showCompareTxt() {
        var a = $(".inputcomparetimetype").val();
        var begintime =new Date( Date.parse($(".fromtime").val().replace(/-/g, '/')));
        var endtime =new Date( Date.parse($(".totime").val().replace(/-/g, '/')));
        var spanday =endtime.getTime() - begintime.getTime();
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
