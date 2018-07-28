<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="LoadingControl.ascx.cs" Inherits="net91com.Stat.Web.Reports.Controls.LoadingControl" %>
<script type="text/javascript">
    $(function () {
        $("form").submit(function () {
            $("#myloadimg").show();
        });
    });
    
</script>
<div id="myloadimg" style="display:none; text-align:center; left:400px; top:200px; position:absolute; z-index:10; "><img height='25px' src='/Images/defaultloading.gif'/><div>  正在加载数据...</div></div>