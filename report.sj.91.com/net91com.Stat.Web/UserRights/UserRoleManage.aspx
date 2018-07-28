<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UserRoleManage.aspx.cs" Inherits="net91com.Stat.Web.UserRights.UserRoleManage"  EnableEventValidation="false" EnableViewStateMac="false"%>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>用户角色分配</title>
     <script src="../Scripts/jquery-1.4.1.min.js" type="text/javascript"></script>
    <link href="../css/version_1.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">
      <div class="mcol2">
        <div class="mcol2_cnt">
            <h3 class="t3">
                角色设置</h3>
            <div id="Background">
                <div class="large" id="Content">
                    <div class="RightColumn">
                        <div id="formGarden">
                        系统：<asp:DropDownList ID="ddlSystems" runat="server" OnSelectedIndexChanged="ddlSystems_SelectedIndexChanged"></asp:DropDownList>
                        <table cellpadding="0" cellspacing="0" border="0">
                            <tr><td class="td2">选择角色：</td><td class="tdline">  
                            <asp:CheckBoxList ID="cbkListrole" runat="server" RepeatDirection="Horizontal" RepeatColumns="4">
    </asp:CheckBoxList></td></tr>
                        <tr>
                            <td colspan="2">
    <asp:Button runat="server" ID="btnSave" Text="保   存" onclick="btnSave_Click" class="primaryAction" />
    &nbsp;&nbsp;&nbsp;<input class="primaryAction" type="reset" value="重   填" />&nbsp;&nbsp;&nbsp;&nbsp;
    <input class="primaryAction" type="button" value="返    回" onclick="javascript:location.href='<%= ReturnUrl %>'"/></td>
                        </tr>
                        </table>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
  
    </form>
</body>
</html>
