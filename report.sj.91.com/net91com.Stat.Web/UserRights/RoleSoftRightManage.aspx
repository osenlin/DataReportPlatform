<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RoleSoftRightManage.aspx.cs"
    Inherits="net91com.Stat.Web.UserRights.RoleSoftRightManage" EnableEventValidation="false"
    EnableViewStateMac="false" %>
<%@ Import Namespace="net91com.Reports.UserRights" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>设置产品权限</title>
    <link href="../css/version_1.css" rel="stylesheet" type="text/css" />
    <script src="../Scripts/jquery-1.4.1.min.js" type="text/javascript"></script>
    <style>
        .userselecttype
        {
            cursor: pointer;
            outline-style: none;
            text-decoration: none;
            color: #1E6FAF;
        }
        .userselecttype2
        {
            cursor: pointer;
            outline-style: none;
            text-decoration: none;
            color: #1E1F0F;
        }
    </style>
    <script type="text/javascript" language="javascript">
        var tbshowid = new Array();
        tbshowid[0] = -1;
        tbshowid[1] = -1;
        $(function () {
            softtype(1);

            $("#cbkProjectSource").attr("disabled", $("#cbkAllProjectSource").attr("checked"));
            $("#cbkAllProjectSource").change(function () {
                $("#cbkProjectSource").attr("disabled", $("#cbkAllProjectSource").attr("checked"));
            });
        });

        
        function getTab(id, obj) {
            $("a[atype=rty]").each(function () {
                if ($(this).hasClass("userselecttype2")) {
                    $(this).removeClass("userselecttype2");
                    $(this).addClass("userselecttype");
                }
            });
            $(obj).removeClass("userselecttype");
            $(obj).addClass("userselecttype2");
            switch (id) {
                case 0:
                    $("#tdsoft").show();
                    $("#tdProjectSource").hide();
                    $("#trResId").hide();
                    $("#tr0").show();
                    $("#tr1").hide();
                    $("#td0").show();
                    $("#td1").hide();
                    $("#td2").hide();
                    $("#td3").hide();
                    if (tbshowid[0] == -1) {
                        $("#tb0").show();
                        $("#tb1").hide();
                        $("#tb2").hide();
                        $("#tb3").hide();
                    }
                    else {
                        clickshow(tbshowid[0], true);
                    }
                    break;
                case 1:
                    $("#tdsoft").show();
                    $("#tdProjectSource").hide();
                    $("#trResId").hide();
                    $("#tr1").show();
                    $("#tr0").hide();
                    $("#td1").show();
                    $("#td0").hide();
                    $("#td2").hide();
                    $("#td3").hide();
                    if (tbshowid[1] == -1) {
                        $("#tb2").show();
                        $("#tb1").hide();
                        $("#tb0").hide();
                        $("#tb3").hide();
                    }
                    else {
                        clickshow(tbshowid[1], true);
                    }
                    break;
                case 2:
                    $("#tr1").hide();
                    $("#tr0").hide();
                    $("#td1").hide();
                    $("#td0").hide();
                    $("#td2").show();
                    $("#td3").hide();
                    $("#tb2").hide();
                    $("#tb1").hide();
                    $("#tb0").hide();
                    $("#tb3").hide();
                    $("#tdsoft").hide();
                    $("#tdProjectSource").hide();
                    $("#trResId").hide();
                    break;
                case 3:
                    $("#tr1").hide();
                    $("#tr0").hide();
                    $("#td1").hide();
                    $("#td0").hide();
                    $("#td2").hide();
                    $("#td3").show();
                    $("#tb2").hide();
                    $("#tb1").hide();
                    $("#tb0").hide();
                    $("#tb3").hide();
                    $("#tdsoft").hide();
                    $("#tdProjectSource").show();
                    $("#trResId").show();
                    break;
            }
            var id = "";

            $("input[type=radio]").each(function () {
                if ($(this).is(":checked")) {
                    id = $(this).val();
                }
            });
            softtype(id);
        }

        function softtype(val) {
            $("tr[id=idtype" + val + "]").show();
            if (val == 1) {
                $("tr[id=idtype2]").hide();
            }
            else {
                $("tr[id=idtype1]").hide();
            }
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div class="mcol2">
        <div class="mcol2_cnt">
            <h3 class="t3">
                产品权限设置 &nbsp;&nbsp;&nbsp;<span style="font-weight: normal"><%=PageTitle%></span></h3>
            <div id="Background">
                <div class="large" id="Content">
                    <div class="RightColumn">
                        <div id="formGarden">
                            <table width="100%" cellpadding="0" cellspacing="0" border="0">
                                <tr>
                                    <td class="tdline" width="100%" colspan="2" align="center">
                                        <a onclick="javascript:getTab(0,this);" class="userselecttype2" style="cursor: pointer"
                                            atype="rty">产品权限</a> | <a onclick="javascript:getTab(3,this);" class="userselecttype"
                                                style="cursor: pointer" atype="rty">下载项目权限</a> 
                                    </td>
                                </tr>
                                <tr id="tdsoft">
                                    <td valign="top" align="center" width="15%" class="td2">
                                        产品权限：
                                    </td>
                                    <td  width="75%">
                                        <table cellpadding="0" cellspacing="0" width="100%" align="left">
                                            <tr>
                                                <td class="tdline">
                                                    <input type="radio" id="radio_1" name="radiogroup" value="1" checked="checked" onclick="softtype(1)" /><label
                                                        for="radio_1">内部产品</label>
                                                    <input type="radio" id="radio_2" name="radiogroup" value="2" onclick="softtype(2)" /><label
                                                        for="radio_2">外部产品</label>
                                                    <table>
                                                        <tr id="tr0">
                                                            <td>
                                                                <table cellpadding="0" cellspacing="0" border="0" id="tb0" width="100%">
                                                                    <tr id="idtype1">
                                                                        <td style="color: Black; font-weight: bold">
                                                                            <asp:CheckBoxList ID="cbkListSoft" runat="server" RepeatColumns="4" RepeatDirection="Horizontal"
                                                                                Width="100%">
                                                                            </asp:CheckBoxList>
                                                                        </td>
                                                                    </tr>
                                                                    <tr id="idtype2">
                                                                        <td style="color: Black; font-weight: bold">
                                                                            <asp:CheckBoxList ID="cbkListSoftout" runat="server" RepeatColumns="4" RepeatDirection="Horizontal"
                                                                                Width="100%">
                                                                            </asp:CheckBoxList>
                                                                        </td>
                                                                    </tr>
                                                                </table>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                
                                <tr id="tdProjectSource" style="display: none">
                                    <td valign="top" align="center" width="15%" class="td2">
                                        下载项目权限：
                                    </td>
                                    <td align="left" width="75%">
                                        <table cellpadding="0" cellspacing="0" align="left" width="100%">
                                            <tr>
                                                <td valign="top">
                                                    <asp:CheckBoxList ID="cbkProjectSource" runat="server" RepeatColumns="4" RepeatDirection="Horizontal"
                                                        Width="100%">
                                                    </asp:CheckBoxList>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>                                
                                <%if (loginService.LoginUser.AccountType != UserTypeOptions.ProductAdmin && UserID > 0)
                                  { %>
                                <tr id="trResId" style="display: none">
                                    <td valign="top" align="center" width="15%" class="td2">
                                        资源权限：
                                    </td>
                                    <td align="left" width="75%">
                                        <table cellpadding="0" cellspacing="0" align="left" width="100%">
                                            <tr>
                                                <td valign="top">
                                                    请输入资源id（以英文逗号隔开）
                                                </td>
                                            </tr>
                                            <tr>
                                                <td valign="top">
                                                    <asp:TextBox TextMode="MultiLine" ID="txtResIdList" runat="server" Width="80%" Rows="6"></asp:TextBox>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <%} %>
                                <tr>
                                    <td colspan="2" align="center">
                                        <br />
                                        <asp:Button runat="server" ID="btnSave" Text="保   存" OnClick="btnSave_Click" class="primaryAction" />
                                        &nbsp;&nbsp;&nbsp;<input type="button" value="返   回" onclick="location='<%= ReturnUrl %>';"
                                    class="primaryAction" />
                                    </td>
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
