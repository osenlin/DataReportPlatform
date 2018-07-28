<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="HeadDownTop100.ascx.cs"
    Inherits="net91com.Stat.Web.Reports.Controls.HeadDownTop100" %>
<%@ Import Namespace="net91com.Core.Extensions" %>
<%@ Import Namespace="net91com.Core" %>
<link href="../../css/headcss/head.css?rd=<%= net91com.Stat.Web.Reports.Services.Utility.CssVersion %>" rel="stylesheet" type="text/css" />
<link href="../../css/headcss/jquery-ui.css" rel="stylesheet" type="text/css" />
<link href="../../css/headcss/style.css?rd=<%= net91com.Stat.Web.Reports.Services.Utility.CssVersion %>" rel="stylesheet" type="text/css" />
<link href="../../css/headcss/jquery.multiselect.css?rd=<%= net91com.Stat.Web.Reports.Services.Utility.CssVersion %>" rel="stylesheet" type="text/css" />
<link href="../../css/headcss/jquery.multiselect.filter.css" rel="stylesheet" type="text/css" />
<script src="../../Scripts/HeadScript/jquery-ui.min.js" type="text/javascript"></script>
<script src="../../Scripts/HeadScript/jquery.multiselect.js?rd=<%= net91com.Stat.Web.Reports.Services.Utility.JsVersion %>" type="text/javascript"
    charset="GBK"></script>
<script src="../../Scripts/HeadScript/jquery.multiselect.filter.js?rd=<%= net91com.Stat.Web.Reports.Services.Utility.JsVersion %>" type="text/javascript"
    charset="GBK"></script>
<script src="../../My97DatePicker/WdatePicker.js" type="text/javascript"></script>
<script type="text/javascript">
    var ischecked=false;
    var allplats = <%=AllPlats %>;
    var restypebox;
    var cateparentbox;
    var catechildbox;
    var grantbox;
    var platbox;
    var customtypebox;
    var ischangedrestype;
    var ischangedbigcate;
    var canviewbigcate=[];
    var canviewsmallcate=[];
    Array.prototype.contains = function(item) {
        for (var i = 0; i < this.length; i++) {
            if (this[i] == item) {
                return true;
            }
        }
        return false;
    };
    function drawbigcate(restype,first)
    {
        var myselectbigcate=$("#myselectcateparent")[0];
        myselectbigcate.options.length=0;
        var bigcate=$(".inputcateparentselect").val();
        $.ajax({
            type:"get",
            url:"/Services/DownLoadResService.ashx?act=getresbigcate&restype="+restype+"&type="+'<%=VersionType %>',
            success:function(data){
                var mydata=eval("("+data+")");
                canviewbigcate=mydata;
                for (var i = 0; i < canviewbigcate.length; i++) {
                    myselectbigcate.add(new Option(canviewbigcate[i].Name,canviewbigcate[i].CateID));
                   
                }
                 <%if(IsHasNoBigCate){ %>
                    myselectbigcate.add(new Option('不区分大分类','0'));
                <%} %>
                if(first)
                {
                  for (var i = 0; i < myselectbigcate.options.length; i++) {
                        if(myselectbigcate.options[i].value==bigcate)
                        {
                                myselectbigcate.options[i].selected=true;
                        }
                   }
                }
              cateparentbox.multiselect('refresh');
              $(".inputcateparentselect").val($("#myselectcateparent").val() == null ? "-1" : $("#myselectcateparent").val());
            }
        });
       
        
    }
    //isfirst 就是是否为首次页面加载
    function drawsmallcate(bigcate,restype,isfirst)
    {
        var myselectsmallcate=$("#myselectcatechild")[0];
        
        myselectsmallcate.options.length=0;
        var smallcate=$(".inputcatechildselect").val();
        $.ajax({
            type:"get",
            url:"/Services/DownLoadResService.ashx?act=getressmallcate&restype="+restype+"&bigcateid="+bigcate+"&type="+'<%=VersionType %>',
            success:function(data)
            {
                var mydata=eval("("+data+")");
                canviewsmallcate=mydata;
              
                if(bigcate>0)
                {
                    var weizhi={"Name":"未知","CateID":-1};
                    canviewsmallcate.push(weizhi);
                }
                for (var i = 0; i < mydata.length; i++) {
                    myselectsmallcate.add(new Option(canviewsmallcate[i].Name,canviewsmallcate[i].CateID));
                            
                }
                  <%if(IsHasNoSmallCate){ %>
                     myselectsmallcate.add(new Option("不区分小分类",0));
                <%} %>
                 
                ///之所以这么做，不像以前那样只要和以前的匹配就好，是因为不同的res ，可能cateid 一样
                if(isfirst)
                {
                    for (var j = 0; j < myselectsmallcate.options.length; j++) {
                        if(myselectsmallcate.options[j].value==smallcate)
                        {
                            myselectsmallcate.options[j].selected=true;
                        }
                            
                    }
                }
                else if( myselectsmallcate.options[0])
                {
                        myselectsmallcate.options[0].selected=true;
                }
                catechildbox.multiselect('refresh');
                $(".inputcatechildselect").val($("#myselectcatechild").val() == null ? "-2" : $("#myselectcatechild").val());

                 
            }
        });
    }

    $(function () { 
       
        
        var restypeselect=$("#myselectrestype")[0];
        var selectedrestype=$(".inputrestypeselect").val();
        for (var i = 0; i < restypeselect.options.length; i++) {
                if(selectedrestype==restypeselect.options[i].value)
                    restypeselect.options[i].selected=true;
    
         }
       

        <%if(!IsHiddenGrant) {%>
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


        //加载平台可以支持多平台
        var myplat=$(".inputplatformselect").val().split(',');
        var platselect=$("#myselectplat")[0];
       
        for (var  i = 0; i < allplats.length; i++) {
            platselect.add(new Option(allplats[i].name, allplats[i].id));
        }
         <%if(IsHasNoPlat){ %>
            platselect.add(new Option('不区分平台', '0'),0);
        <%} %>
           for (var  i = 0; i < platselect.options.length; i++) {
                        for (var j = 0; j < myplat.length; j++) {
                            if (myplat[j] == platselect.options[i].value) {
                                platselect.options[i].selected = true;
                            }
                      }

            }
      

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
                var restype=$("#myselectrestype").val() == null ? "-1" : $("#myselectrestype").val()
                if(parseInt(restype)>4||parseInt(restype)<0)
                {
                    alert("该类型不支持分类查找，请重新选择");
                    return false;
                }
                $(".inputrestypeselect").val(restype);
                
                ///若选中的资源类型改变了
                var temprestype=$(".inputrestypeselect").val();
                if(ischangedrestype!=temprestype)
                {
                    drawbigcate(temprestype);
                    drawsmallcate(-1,-2);
                }

            },
            beforeopen:function()
            {
                ischangedrestype=$(".inputrestypeselect").val();
            }
          });
           
            cateparentbox = $("#myselectcateparent").multiselect({
                    multiple:false,
                    noneSelectedText: "选择大分类",
                    header: false,
                    height:275,
                    minWidth: 170,
                    selectedList: 100,
                    beforeclose: function () {
                        if ($("#myselectcateparent").val() == null) {
                            alert("至少选择一个选项！");
                            return false;
                        }
                        $(".inputcateparentselect").val($("#myselectcateparent").val() == null ? "-1" : $("#myselectcateparent").val());
                        
                        ///若是改变选中项
                        if( $(".inputcateparentselect").val()!=ischangedbigcate)
                        {
                             drawsmallcate( $(".inputcateparentselect").val(),$(".inputrestypeselect").val());
                        }
                       

                    },
                    beforeopen:function(){
                         ischangedbigcate=$(".inputcateparentselect").val();
                         <%if(!IsHasNoSmallCate){ %>
                             if(canviewbigcate.length==0){
                                alert("请先选择资源类型！");
                                return false;
                            }
                         <%} %>

                    } 
                });

            catechildbox=$("#myselectcatechild").multiselect({
                    multiple:false,
                    noneSelectedText: "选择小分类",
                    header: false,
                    height:275,
                    minWidth: 170,
                    selectedList: 100,
                    beforeclose: function () {
                        if ($("#myselectcatechild").val() == null) {
                            alert("至少选择一个选项！");
                            return false;
                        }
                        $(".inputcatechildselect").val($("#myselectcatechild").val() == null ? "-2" : $("#myselectcatechild").val());
                    },
                    beforeopen: function () {
                     <%if(!IsHasNoSmallCate) {%>
                        if (canviewsmallcate.length == 0) {
                            alert("暂无小分类！");
                            return false;
                        }
                    <%} %>
                    }
                });

            platbox = $("#myselectplat").multiselect({
                multiple:!<%=IsPlatSingle.ToString().ToLower()%>,
                noneSelectedText: "选择平台",
                header: false,
                height:300,
                minWidth: 130,
                selectedList: 100,
                beforeclose: function () {
                
                    if ($("#myselectplat").val() == null) {
                        alert("至少选择一个选项！");
                        return false;
                    }
                    $(".inputplatformselect").val($("#myselectplat").val() == null ? "-1" : $("#myselectplat").val());
               

                }
            

            });
            <%if(!IsHiddenGrant){ %>
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
              <%if(!HiddenCustomType){ %>
                   customtypebox = $("#myselectcustomtype").multiselect({
                    multiple:!<%=IsCustomTypeSingle.ToString().ToLower()%>,
                    noneSelectedText: "选择类型",
                    header: false,
                    height:150,
                    minWidth: 170,
                    selectedList: 100,
                    beforeclose: function () {
                
                        if ($("#myselectcustomtype").val() == null) {
                            alert("至少选择一个选项！");
                            return false;
                        }
               
                        $(".inputcustomtypeselect").val($("#myselectcustomtype").val() == null ? "-1" : $("#myselectcustomtype").val());
               

                    } 
                });

        <%} %>



              var bigcate=$(".inputcateparentselect").val();
              var smallcate=$(".inputcatechildselect").val();
              drawbigcate(selectedrestype,true);
              drawsmallcate(bigcate,selectedrestype,true);

           

   

        
        ///初始位置设置
         setDivPos();
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
                }
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
              if(cateparentbox)
              {
                  cateparentbox.multiselect('refreshmenupos');
              }
              if(catechildbox)
              {
                  catechildbox.multiselect('refreshmenupos');
              }
              if(platbox)
              {
                  platbox.multiselect('refreshmenupos');
              }
              
              if(grantbox)
              {
                  grantbox.multiselect('refreshmenupos'); 
              }
               if(customtypebox)
              {
               
                customtypebox.multiselect('refreshmenupos'); 
              }

              
            
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
            
            if($("#myselectcateparent")[0])
            {
                if( $(".inputcateparentselect").val()=="-1")
                {
                    alert("请选择一个分类！");
                    ischecked=false;
                    return;
                }
             
            }
            if($("#myselectcatechild")[0])
            {
                if( $(".inputcatechildselect").val()=="-2")
                {
                    alert("请选择一个小分类！");
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
             if($("#myselectplat")[0])
            {
                if( $(".inputplatformselect").val()=="-1")
                {
                    alert("请选择平台！");
                    ischecked=false;
                    return;
                }
             
            }
            
            ischecked=true;

         
    }
    
</script>
<div class="head">
    <div class="divitem" style="width: <%= ResTypeWidth %>;">
        <select id="myselectrestype" style="display:none" size="12" multiple="multiple">
            <% net91com.Reports.UserRights.URLoginService loginService = new net91com.Reports.UserRights.URLoginService();
               foreach (net91com.Reports.UserRights.ResourceType resType in loginService.GetResourceTypes())
                  { %>
                  <option value="<%= resType.TypeID %>"><%= resType.TypeName %></option>
          <%} %>
        </select>
    </div>
    <div id="divcateparent" class="divitem" style="width: <%=CateParentWidth%>;">
        <select id="myselectcateparent" size="12" style="display:none" multiple="multiple">
        </select>
    </div>
    <div id="divcatechild" class="divitem" style="width: <%=CateChildWidth%>;">
        <select id="myselectcatechild" size="12" style="display:none" multiple="multiple">
        </select>
    </div>
    <div class="divitem" style="width: <%=PlatWidth%>;">
        <select id="myselectplat" size="12" style="display:none" multiple="multiple">
        </select>
    </div>

     <%if (!HiddenCustomType)
      { %>
    <div class="divitem" style="width: <%=CustomTypeWidth%>;">
        <select id="myselectcustomtype" size="12" style="display:none" multiple="multiple">
            <%   foreach (var item in CustomTypeSource.Keys)
                 {%>
                <option value="<%=(int)item %>"><%=CustomTypeSource[item] %></option>
                
            <%} %>
        </select>
    </div>
    <%} %>
    <div class="divitem" style="width: <%=TimeWidth%>">
        <button value="选择时间" id="timebtn" class="ui-multiselect ui-widget ui-state-default ui-corner-all"
            type="button" aria-haspopup="true" tabindex="0" style="width: 155px;">
            <span class="ui-icon ui-icon-triangle-2-n-s"></span><span id="timetxt">选择时间</span>
        </button>
    </div>
    <%if (!IsHiddenGrant)
      { %>
    <div class="divitem" style="width: <%=ResGrantWidth%>;">
        <select id="myselectresgrant" style="display:none" size="12" multiple="multiple">
            <option value="-1">全部</option>
            <option value="1">已授权</option>
            <option value="0">未授权</option>
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
<input id="inputrestypeselect" class="inputrestypeselect" runat="server" value="-1"
    type="hidden" />
<input id="inputcateparentselect" class="inputcateparentselect" runat="server" type="hidden"
    value="-1" />
<input id="inputcatechildselect" class="inputcatechildselect" runat="server" type="hidden"
    value="-2" />
<input id="inputgrantselect" class="inputgrantselect" runat="server" type="hidden"
    value="-1" />
<input id="inputbegintime" class="inputbegintime" runat="server" type="hidden" />
<input id="inputendtime" class="inputendtime" runat="server" type="hidden" />
<input id="inputisfirstload" runat="server" type="hidden" value="0" />
<input id="inputplatformselect" class="inputplatformselect" runat="server" type="hidden"
    value="-1" />
<input id="inputcustomtypeselect" class="inputcustomtypeselect" runat="server" type="hidden"
    value="-1" />
<div id="timeid" style="display: none" class="ui-multiselect-menu ui-widget ui-widget-content ui-corner-all"
    style="display: block; top: 37px;">
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
