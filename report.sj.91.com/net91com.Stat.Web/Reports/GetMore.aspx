<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GetMore.aspx.cs" Inherits="net91com.Stat.Web.Reports.GetMore" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script src="../Scripts/jquery-1.5.2.min.js" type="text/javascript"></script>
</head>
<body>
    <form id="form1" action="<%=url %>" method="post">
    <div>
    <input type="hidden" value="<%= begintime.ToString("yyyy-MM-dd")%>" name="inputtimestart" />
    <input type="hidden" value="<%=endtime.ToString("yyyy-MM-dd") %>" name="inputtimeend" />
    <input type="hidden" value="<%=softStr %>" name="inputsoftselect" class="inputsoftselect" />
    <input type="hidden" value="<%=platformStr %>" name="inputplatformselect" class="inputplatformselect" />
    <input type="hidden" value="<%=MyPeriod %>" name="inputzhouqi" class="inputzhouqi" />
    <input type="hidden" value="<%=Channelid %>" name="channelid" class="channelid" />
    <input type="hidden" value="<%=Channeltype %>" name="channeltype" />
    <input type="hidden" value="<%=UserCate %>" name="t" />
    <input type="hidden" value="<%= Area %>" name="area" />
    </div>
    </form>
</body>
  <script type="text/javascript">
      

      var form = document.getElementById("form1");
      form.submit();
    </script>
</html>
