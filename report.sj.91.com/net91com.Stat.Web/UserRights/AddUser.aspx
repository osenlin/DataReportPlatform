<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AddUser.aspx.cs" Inherits="net91com.Stat.Web.UserRights.AddUser" EnableEventValidation="false" EnableViewStateMac="false" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>添加用户</title>
     <link href="../css/version_1.css" rel="stylesheet" type="text/css" />
     
    <script src="/Reports_New/js/My97DatePicker/WdatePicker.js" type="text/javascript"></script>
    <script type="text/javascript" language="javascript">
        var focusBgColor = "#FFFFFF"; //文本框获得焦点时的背景色
        function check() {
            var val = document.getElementById("txtName").value;
            if (val.length < 6) {
                alert('请输入用户名');
                return false;
            }
            else if (document.getElementById("TxtTrueName").value.length == 0) {
                alert('请输入真实姓名');
                return false;
            }
            else if (document.getElementById("beginTime").value.length == 0 || document.getElementById("endTime").value.length == 0) {
                alert("请输入开始截止日期!");
                return false;
            }
            else if (document.getElementById("txtemail").value.length == 0) {
                alert("请输入邮箱!");
                return false;
            }else if (document.getElementById("txtDept").value.length == 0) {
                alert("请输入部门!");
                return false;
            }

            var email = document.getElementById("txtemail").value;
            var reg = /^\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$/;
            if (!reg.test(email)) {
                alert("邮箱格式不正确!");
                return false;
            } 
            var s = document.getElementById("beginTime").value.split('-');
            var sdate = new Date(s[0], s[1] - 1, s[2]);
            var e = document.getElementById("endTime").value.split('-');
            var edate = new Date(e[0], e[1] - 1, e[2]);
            // 开始日期与结束日期的时间差
            var t = edate.getTime() - sdate.getTime();
            if (t < 0) {
                alert("开始日期不能大于结束日期");
                return false;
            }
            return true;
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div class="mcol2">
        <div class="mcol2_cnt">
            <h3 class="t3">
                添加用户</h3>
            <div id="Div1">
                <div class="large" id="Div2">
                    <div class="RightColumn">
                        <div id="Div3">
                            <table cellspacing="0" cellpadding="0" border="0">
                                <tr>
                                    <td align="right" height="30" class="td2">
                                        用户名：
                                    </td>
                                    <td class="tdline">
                                        <asp:TextBox ID="txtName" runat="server" CssClass="required" Font-Bold="True" Width="232px" MaxLength="30"></asp:TextBox>
                                    </td>
                                    <td><span style="color:#747474">登录时使用的91账号，用户名长度不能小于6个字符</span>
                                    
                                    </td>
                                </tr>
                                <tr>
                                    <td align="right" height="30" class="td2">
                                        真实姓名：
                                    </td>
                                    <td class="tdline">
                                        <asp:TextBox ID="TxtTrueName" runat="server" CssClass="required" Font-Bold="True" Width="232px" MaxLength="30"></asp:TextBox>
                                    </td>
                                    <td><span style="color:#747474"></span>
                                    
                                    </td>
                                </tr>
                                <tr>
                                    <td align="right" height="30" class="td2">
                                        用户类型：
                                    </td>
                                    <td class="tdline">
                                        <font face="宋体">
                                            <asp:RadioButtonList ID="rblType" runat="server" RepeatDirection="Horizontal"
                                                RepeatLayout="Flow" Height="29px" CssClass="required">
                                                <asp:ListItem Value="0" Selected="True">普通用户</asp:ListItem>
                                                <asp:ListItem Value="1">管理员</asp:ListItem>
                                                <asp:ListItem Value="3">产品管理员</asp:ListItem>
                                                <asp:ListItem Value="4">渠道内部用户</asp:ListItem>
                                                <asp:ListItem Value="5">渠道合作方</asp:ListItem>
                                            </asp:RadioButtonList>
                                        </font>
                                    </td>
                                    <td><span style="color:#747474">1、渠道合作方,渠道内部用户,普通用户只能使用报表相关的查看功能，不能进行系统管理。<br />2、管理员具备普通用户操作，并能具备系统管理。</span></td>
                                </tr>
                                <tr>
                                    <td align="right" height="30" class="td2">
                                        用户状态：
                                    </td>
                                    <td class="tdline">
                                        <asp:RadioButtonList ID="rblStatus" runat="server" Width="120px" RepeatDirection="Horizontal"
                                            RepeatLayout="Flow" Height="29px">
                                            <asp:ListItem Value="1" Selected="True">启用</asp:ListItem>
                                            <asp:ListItem Value="0">禁用</asp:ListItem>
                                        </asp:RadioButtonList>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="right" height="30" class="td2">
                                        日期：
                                    </td>
                                     <td>
                                        从：
                                         <asp:TextBox ID="beginTime" runat="server"  class="Wdate" onclick="WdatePicker()" ></asp:TextBox>
                                        至:
                                        <asp:TextBox ID="endTime" runat="server"  class="Wdate" onclick="WdatePicker()" ></asp:TextBox>
                                     </td>
                                       
                                </tr>
                                 <tr>
                                    <td align="right" height="30" class="td2">
                                        特别用户：
                                    </td>
                                     <td>
                                         <asp:CheckBox ID="ckbspecialuser" runat="server" />
                                     </td>
                                     <td><span style="color:#747474">特别用户的使用权限将永不过期</span></td>   
                                </tr>
                                <tr>
                                    <td align="right" height="30" class="td2">
                                        邮箱：
                                    </td>
                                     <td>
                                         <asp:TextBox ID="txtemail" runat="server" Width="232px" />
                                     </td>
                                    
                                </tr>
                                <tr>
                                    <td align="right" height="30" class="td2">
                                        部门：
                                    </td>
                                     <td>
                                         <asp:TextBox ID="txtDept" runat="server" Width="232px" />
                                     </td>
                                    
                                </tr>
                                  <tr>
                                    <td align="right" height="30" class="td2">
                                        白名单用户：
                                    </td>
                                     <td>
                                         <asp:CheckBox ID="chbwhiteuser" runat="server" />
                                         
                                     </td>
                                     <td><span style="color:#747474">加入白名单后,内外网查看不受限制,请谨慎使用</span></td>   
                                </tr>
                                <tr>
                                    <td height="30" colspan="2" align="center">
                                        <br>
                                        <asp:Button ID="btnSave" runat="server" CssClass="primaryAction" Text="保   存" 
                                            OnClientClick="return check();" onclick="btnSave_Click">
                                        </asp:Button>
                                        &nbsp;&nbsp;&nbsp;<input class="primaryAction" type="button" value="返  回" onclick="javascript:location.href='<%= ReturnUrl %>'" />
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
