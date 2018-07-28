<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DataLogControl.ascx.cs"
    Inherits="net91com.Stat.Web.Monitor.Controls.DataLogControl" %>
<link href="/css/headcss/head.css"
    rel="stylesheet" type="text/css" />
<link href="/css/headcss/jquery-ui.css" rel="stylesheet" type="text/css" />
<link href="/css/headcss/style.css"
    rel="stylesheet" type="text/css" />
<link href="/css/headcss/jquery.multiselect.css"
    rel="stylesheet" type="text/css" />
<link href="/css/headcss/jquery.multiselect.filter.css" rel="stylesheet" type="text/css" />
<script src="/Scripts/HeadScript/jquery-ui.min.js" type="text/javascript"></script>
<script src="/Scripts/HeadScript/jquery.multiselect.js"
    type="text/javascript" charset="GBK"></script>
<script src="/Scripts/HeadScript/jquery.multiselect.filter.js" type="text/javascript"
    charset="GBK"></script>
<script src="/My97DatePicker/WdatePicker.js" type="text/javascript"></script>
<script type="text/javascript">
    var lognamebox;
    var serveripbox;
    var ischangelogname;
    var ischecked=false;
    var needdatecheck=false;
    function drawserversipdoms(logname)
    {
         var myselectserverip = $("#myselectserverip")[0];
         myselectserverip.options.length = 0;
         $.ajax({
            type: "get",
            url: "/Monitor/MonitorService.ashx?act=getserverip",
            success: function (data) {
                var mydata = eval("(" + data + ")"); 
                <%if(IsHasNoServerIp){ %>
                    myselectserverip.add(new Option('不区分服务器', '0'));
                <%} %>
                for (var  i = 0; i < mydata.length; i++) {
                    myselectserverip.options.add(new Option(mydata[i].ServerIp, mydata[i].ServerID));
                }
                serverips = $(".inputserveripselect").val().split(',');
                for (var  i = 0; i < myselectserverip.options.length; i++) {
                    for (var j = 0; j < serverips.length; j++) {
                        if (serverips[j] == myselectserverip.options[i].value) {
                            myselectserverip.options[i].selected = true;
                        }
                    }

                }
                if(serveripbox) 
                {
                    serveripbox.multiselect('refresh');
                    $(".inputserveripselect").val($("#myselectserverip").val() == null ? "-1" : $("#myselectserverip").val());
                }
                


            }
        });
    }

    $(function () { 
       
        var lognameselect=$("#myselectlogname")[0];
        ///默认的选择上
        var selectedlogname = $(".inputlognameselect").val();
        if (selectedlogname!= -1) {
            for (var  i = 0; i < lognameselect.options.length; i++) {
                if (selectedlogname==lognameselect.options[i].value) {
                    lognameselect.options[i].selected = true;
                }
            }
        }
         ///没有隐藏的情况下
         if($("#myselectserverip")[0])
         {
             drawserversipdoms(selectedlogname);
         }
         serveripbox = $("#myselectserverip").multiselect({
            multiple:!<%=ServerIpSingle.ToString().ToLower()%>,
            noneSelectedText: "选择服务器",
            header: false,
            height:300,
            minWidth: 170,
            selectedList: 100,
            beforeclose: function () {
                
                if ($("#myselectserverip").val() == null) {
                    alert("至少选择一个选项！");
                    return false;
                }
                $(".inputserveripselect").val($("#myselectserverip").val() == null ? "-1" : $("#myselectserverip").val());
                
            },
            beforeopen: function () {
                if ($("#myselectserverip")[0].options.length==0 ) {
                    alert("请先选择软件！");
                    return false;
                }
              
            }

        });

        lognamebox=$("#myselectlogname").multiselect({
          
            multiple:false,
            noneSelectedText: "选择日志名称",
            header: false,
            height:275,
            minWidth: 170,
            selectedList: 100,
            beforeclose: function () {
                if ($("#myselectlogname").val() == null) {
                    alert("至少选择一个服务器！");
                    return false;
                }
                $(".inputlognameselect").val($("#myselectlogname").val());
                <%if(!HiddenServerIp) {%>
                drawserversipdoms($("#myselectlogname").val());
                <%} %>
               
            },
            beforeopen: function () {
                ischangedsoft = $(".inputlognameselect").val();
            }

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
         ///单个时间点击
         $("#singletimebtn").bind('click', function () {
            var singletimeid = $("#singletimeid");
            var isopen = singletimeid.is(":visible");
            if (isopen) {
                var singlefromtime = $(".singlefromtime").val();
                
                if (singlefromtime != "") {
                    $(".inputsingletime").val(singlefromtime);
                    
                    $("#singletimetxt").text(singlefromtime.substring(2, singlefromtime.length).replace(/-/g, '/'));
                }
                singletimeid.hide();
            }
            else
                singletimeid.show();
        });
       ///提交表单检查并提交
        $("#btnpost").bind("click", function () {
            checkCondition();
            checkedforcontrol();
            if(ischecked)
                $("#form1").submit();
        });
         ///绑定窗体大小改变事件
        $(window).bind("resize",function(){
              if($("#timebtn")[0])
              {
                 setDivPos();
              }
              if($("#singletimebtn")[0])
              {
                 setDivPos();
              }
              if(lognamebox)
              { 
                  lognamebox.multiselect('refreshmenupos');
			  }
              if(serveripbox)
              {
                  serveripbox.multiselect('refreshmenupos');
              }
            
            
        });
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
                        $(".inputbegintime").val(fromtime);
                        $(".inputendtime").val(totime);
                        $("#timetxt").text(fromtime.substring(2, fromtime.length).replace(/-/g, '/') + "至" + totime.substring(2, fromtime.length).replace(/-/g, '/'));
                    }
                    timeid.hide();
                }
            }
            if(singletimeid[0])
            {
                var isopen = singletimeid.is(":visible");
                var singletimebtn=$("#singletimebtn");
                if (isopen && !$.contains(singletimeid[0], e.target) && !$.contains(singletimebtn[0], e.target) && e.target !== singletimebtn[0]) {
                    var fromtime = $(".singlefromtime").val();
                   
                    if (fromtime != "") {
                        $(".inputsingletime").val(fromtime);
                        $("#singletimetxt").text(fromtime.substring(2, fromtime.length).replace(/-/g, '/') );
                    }
                    singletimeid.hide();
                }
                
            }
        });
        ////其他设置
        setDivPos();
        ///时间设置
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
        if($("#singletimetxt")[0])
        {
            var fromtime = $(".singlefromtime").val();
           
            if (fromtime != "" ) {
                $(".inputsingletime").val(fromtime);
                $("#singletimetxt").text(fromtime.substring(2, fromtime.length).replace(/-/g, '/') );
            }
        }
        
       
    });


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
        var singletimebtn = $("#singletimebtn");
        if(singletimebtn[0])
        {
            var pos = singletimebtn.offset();
            $("#singletimeid").css({ top: pos.top + $("#singletimebtn").outerHeight()+6, left: pos.left })
            ///设置宽度
            var m = $("#singletimeid");
            width = singletimebtn.outerWidth() -
				    parseInt(m.css('padding-left'), 10) -
				    parseInt(m.css('padding-right'), 10) -
				    parseInt(m.css('border-right-width'), 10) -
				    parseInt(m.css('border-left-width'), 10);

            m.width(width || timebtn.outerWidth());
        }
    }

    function checkedforcontrol()
    {
           
           if($("#timebtn")[0])
            {
                var starttime1=$.trim($(".inputbegintime").val()).replace(/-/g,'/');
                var endtime1=$.trim($(".inputendtime").val()).replace(/-/g,'/');
                var start=Date.parse(starttime1);
                var end= Date.parse(endtime1);
                if (start > end) {
                    alert("日期范围有问题！");
                    ischecked=false;
                    return;
                }
                if(needdatecheck&&Math.abs(end - start)/(1000*60*60*24)>15)
                {
                    alert("日期范围不能超过半个月");
                    ischecked=false;
                    needdatecheck=false;
                    return;
                }
               
            }
            if($("#singletimebtn")[0])
            {
                var starttime1=$.trim($(".inputsingletime").val()).replace(/-/g,'/');
               
                if (starttime1=="") {
                    alert("日期不能空！");
                    ischecked=false;
                    return;
                }
            }
            if($("#myselectlogname")[0])
            {
                if( $(".inputlognameselect").val()=="-1")
                {
                    alert("请选择日志名称！");
                    ischecked=false;
                    return;
                }
            }
            if($("#myselectserverip")[0])
            {
                if( $(".inputserveripselect").val()=="-1")
                {
                    alert("请选择服务器！");
                    ischecked=false;
                    return;
                }
            }
            ischecked=true;

        
    }

</script>
<div class="head">
    <div class="divitem" style="width: <%= LogNameWidth %>;">
        <select id="myselectlogname" size="12" multiple="multiple">
            <%foreach (var item in LogNameSource.Keys)
              {%>
            <option value="<%=item %>">
                <%=LogNameSource[item] %></option>
            <% } %>
        </select>
    </div>
    <%if (!HiddenServerIp)
      {%>
    <div class="divitem" style="width: <%= ServerIpWidth %>;">
        <select id="myselectserverip" size="12" multiple="multiple">
        </select>
    </div>
   <%} %>
    <%if (!HiddenDoubleTime)
      { %>
    <div class="divitem" style="width: <%=TimeWidth%>">
        <button value="选择时间" id="timebtn" class="ui-multiselect ui-widget ui-state-default ui-corner-all"
            type="button" aria-haspopup="true" tabindex="0" style="width: 155px;">
            <span class="ui-icon ui-icon-triangle-2-n-s"></span><span id="timetxt">选择时间</span>
        </button>
    </div>
    <%} %>
    <%if (!HiddenSingleTime)
      { %>
    <div class="divitem" style="width: <%=TimeWidth%>">
        <button value="选择时间" id="singletimebtn" class="ui-multiselect ui-widget ui-state-default ui-corner-all"
            type="button" aria-haspopup="true" tabindex="0" style="width: 145px;">
            <span class="ui-icon ui-icon-triangle-2-n-s"></span><span id="singletimetxt">选择时间</span>
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
<input id="inputlogname" class="inputlognameselect" runat="server" value="-1" type="hidden" />
<input id="inputserveripselect" class="inputserveripselect" runat="server" type="hidden"
    value="-1" />
<input id="inputbegintime" class="inputbegintime" runat="server" type="hidden" />
<input id="inputendtime" class="inputendtime" runat="server" type="hidden" />
<input id="inputsingletime" class="inputsingletime" runat="server" type="hidden" />
<input id="inputisfirstload" runat="server" type="hidden" value="0" />
<div id="timeid" class="ui-multiselect-menu ui-widget ui-widget-content ui-corner-all"
    style="display: block; top: 37px; display: none">
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
<!--只有一个时间的时候-->
<div id="singletimeid" class="ui-multiselect-menu ui-widget ui-widget-content ui-corner-all"
    style="display: block; top: 37px; display: none">
    <div class="ui-widget-header ui-corner-all ui-multiselect-header ui-helper-clearfix ui-multiselect-hasfilter">
        <div class="ui-multiselect-filter" style="margin-right: 0px;">
            <input type="text" id="singlefromtime" class="Wdate singlefromtime" onclick="WdatePicker()"
                runat="server" />
        </div>
    </div>
</div>
