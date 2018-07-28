<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="HeadAdminControl.ascx.cs" Inherits="net91com.Stat.Web.Reports.Controls.HeadFuncControl" %>

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
    <%=DataSource %>
    var allplats = <%=AllPlats %>;
    var ischecked=false;
    var canviewplats = []; 
   
    var softbox;
    var platbox; 
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
        //plat.options.length=0;
        ///不支持的平台disable掉
         for (var i = 0; i < plat.options.length; i++) {
             plat.options[i].disabled=true;
             plat.options[i].selected=false; 
             for (var j = 0; j < canviewplats.length; j++) {
                  if(canviewplats[j]== plat.options[i].value|| plat.options[i].value=="0")
                    plat.options[i].disabled=false;
            }
        }
        
       ///该勾选的要勾选上
        plats = $(".inputplatformselect").val().split(',');
        for (var  i = 0; i < platselect.options.length; i++) {
            for (var j = 0; j < plats.length; j++) {
                if (plats[j] == platselect.options[i].value&&!platselect.options[i].disabled) {
                    platselect.options[i].selected = true;
                    break;
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
     
    } 


    $(function () {
        

        var softselect = $("#myselectsoft")[0];
         //平台加载上去
        var platselect=$("#myselectplat")[0];
        <%if(!HiddenPlat) {%>
         for (var  i = 0; i < allplats.length; i++) {
            platselect.add(new Option(allplats[i].name, allplats[i].id));
        }
       
        <%} %>
        ///加上不区分平台
        <%if(IsHasNoPlat&&!HiddenPlat) {%>
            platselect.add(new Option('不区分平台', '0'));
            <%} %> 
        //默认选中项目,软件，平台，版本，功能
        var selectedsoft = $(".inputsoftselect").val().split(',');

        if (selectedsoft.length != 0) {
            for (var  i = 0; i < softselect.options.length; i++) {
                if (selectedsoft.contains(softselect.options[i].value)) {
                    softselect.options[i].selected = true;
                }
            } 
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
                    alert("至少选择一款软件！");
                    return false;
                }

                $(".inputsoftselect").val($("#myselectsoft").val().join(','));
                <%if(!HiddenPlat){ %>
                if (ischangedsoft != $(".inputsoftselect").val()) {
                    ///软件一变，平台重绘，版本重绘，功能重绘
                    //平台重绘
                    drawplatsdoms($("#myselectsoft").val());
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
            height:300,
            minWidth: 170,
            selectedList: 100,
            beforeclose: function () {
                
                if ($("#myselectplat").val() == null) {
                    alert("至少选择一个选项！");
                    return false;
                }
               
                $(".inputplatformselect").val($("#myselectplat").val() == null ? "-1" : $("#myselectplat").val());
               


            },
            beforeopen: function () {
                if (canviewplats.length == 0) {
                    alert("请先选择软件！");
                    return false;
                }
                ischangedplat = $(".inputplatformselect").val();
            }

        });
         <%} %>

      
          


        ////------------其他设置------------------------

        
        
       
        ///绑定窗体大小改变事件
        $(window).bind("resize",function(){
             
              if(softbox)
              { 
                  softbox.multiselect('refreshmenupos');
			  }
              if(platbox)
              {
                
                platbox.multiselect('refreshmenupos');
              }
               
            
        });
        
    


        ///提交表单检查并提交
        $("#btnpost").bind("click", function () {
            checkCondition();
            checkedforcontrol();
            if(ischecked)
                $("#form1").submit();
        });




    });
    ///内部检查
    function checkedforcontrol()
    {
         
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
       
            ischecked=true;
        
    }

 

</script>
<div class="head">
    <div class="divitem" style="width: <%=SoftWidth%>;">
        <select id="myselectsoft" style="display:none" size="12" multiple="multiple">
          <%for (int index = 0; index < MySupportSoft.Count; index++)
            {%>
              <option value="<%= MySupportSoft[index].ID%>"><%= MySupportSoft[index].Name%></option>
         <%} %>
        </select>
    </div>
    <%if (!HiddenPlat)
      { %>
    <div class="divitem" style="width: <%=PlatWidth%>">
        <select id="myselectplat" style="display:none"  size="12" multiple="multiple">
        </select>
    </div>
    <%} %>
    <%if (ShowCheckBox)
      { %>
    <div class="divitem" style="width: <%=CheckboxWidth%>">
        <asp:CheckBox ID="checkbox"  Text="" runat="server" /> 
    </div>
    <%} %>
     <%if (ShowDeleteBtn)
       { %>
      <div class="divitem" style="width: <%=BtnWidth%>;">
        <button id="btndelete" class="ui-multiselect ui-widget ui-state-default ui-corner-all"
            type="button" aria-haspopup="true" tabindex="0">

            <span id="Span3"><%=DeleteBtnText%></span>
        </button>
     </div>

     <%} %>
    <%if (ShowAddBtn)
      { %>
     <div class="divitem" style=" width: <%=BtnWidth%>;">
        <button id="btnadd" class="ui-multiselect ui-widget ui-state-default ui-corner-all"
            type="button" aria-haspopup="true" tabindex="0">

            <span id="Span2"><%=AddBtnText%></span>
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
<input id="inputsoftselect" class="inputsoftselect" runat="server" value="-1" type="hidden" />
<input id="inputplatformselect" class="inputplatformselect" runat="server" type="hidden" value="-1" />
<input id="inputisfirstload"   runat="server" type="hidden" value="0" />



 
 
