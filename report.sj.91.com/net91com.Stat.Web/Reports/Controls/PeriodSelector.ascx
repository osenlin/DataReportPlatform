<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PeriodSelector.ascx.cs"
    Inherits="net91com.Stat.Web.Reports.Controls.PeriodSelector" %>
<%@ Import Namespace="net91com.Core.Extensions" %>
<script type="text/javascript">
    ///选择周期
    function selectPeriod(a) {
        //设置表单值
        $("#hPeriod").val($(a).attr("p"));
        $("form:first").submit();
    }
</script>
<input type="hidden" id="hPeriod" name="hPeriod" value="<%= (int)SelectedPeriod %>" />
<div class="userselected">
    <% for (int i = 0; i < PeriodList.Count; i++)
       { %>
    <a class="<%= SelectedPeriod==PeriodList[i] ? "userselecttype2" : "userselecttype" %> zhouqi" href="javascript:void(0);" onclick="selectPeriod(this)" p="<%= (int)PeriodList[i] %>">
        <%= PeriodList[i].GetDescription()%></a>
    <% if (i < PeriodList.Count - 1)
       { %>
    &nbsp;&nbsp;|&nbsp;&nbsp;
    <%}
       }%>
</div>
