<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="net91com.Stat.Web.Login" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <script type="text/javascript" language="javascript">
        if (top.location != self.location) {
            top.location = self.location;
            location.href = '<%=LoginUrl %>';
        }
        location.href = '<%= LoginUrl%>';
     </script>
</body>
</html>
