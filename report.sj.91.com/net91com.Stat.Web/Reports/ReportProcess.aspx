<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ReportProcess.aspx.cs"
    Inherits="net91com.Stat.Web.ReportProcess" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="description" content="" />
    <meta name="keywords" content="" />
    <title>举报处理</title>
    <script src="/Scripts/HeadScript/jquery.js" type="text/javascript"></script>
    <script src="/Scripts/jquery.query-2.1.7.js" type="text/javascript"></script>
    <script src="/Scripts/jquery.form.js" type="text/javascript"></script>
    <script src="/Scripts/jquery.validate.min.js" type="text/javascript"></script>
    <script type="text/javascript">
        var serverUrl = '/Services/HttpReportService.ashx';
        var projectSource = $.query.get("projectsource");
        var platform = $.query.get("platform");
        var resType = $.query.get("restype");
        var resId = $.query.get("resid");
        var resVersion = $.query.get("resversion");
        var option = $.query.get("option");
        var state = $.query.get("state");

        $(function () {
            init();
        });

        // 初始化
        function init() {
            $('#form1').validate({
                rules: {
                    txtDetail: { maxlength: 100 }
                },
                messages: {
                    txtDetail: { maxlength: "内容长度不能大于100个字符" }
                },
                submitHandler: function (form) {
                    $(form).ajaxSubmit({
                        beforeSerialize: getValue,
                        beforeSubmit: function () {
                            $('#btnSubmit').enable(false);
                        },
                        success: showResponse,
                        url: serverUrl,
                        type: 'post',
                        dataType: 'json'
                    });
                }
            });

            $('#ddlState').val((parseInt(state) + 1) % 4);
            $('#txtDetail').focus();
        }

        function getValue($form, options) {
            if (!window.confirm('请确定要保存？')) {
                return false;
            }
            options.data = {
                'act': 'process',
                'projectsource': projectSource,
                'platform': platform,
                'restype': resType,
                'resid': resId,
                'resversion': resVersion,
                'option': option,
                'state': $('#ddlState').val(),
                'info': $('#txtDetail').val()
            };
        }

        function showResponse(responseText, statusText, xhr, $form) {
            $('#btnSubmit').enable(true);
            if (responseText) {
                if (responseText.resultType == 1) {
                    onClose();
                }
                else {
                    alert(responseText.message);
                }
            }
        }
        function onClose() {
            parent.$.fn.colorbox.close();
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <table cellpadding="5" cellspacing="0" border="0" width="95%">
        <tr>
            <td style="width: 80px">
                举报状态：
            </td>
            <td>
                <select id="ddlState">
                    <option value="0">待处理</option>
                    <option value="1">处理中</option>
                    <option value="2">已处理</option>
                    <option value="3">忽略中</option>
                </select>
            </td>
        </tr>
        <tr>
            <td valign="top">
                内容反馈：
            </td>
            <td>
                <textarea style="width: 100%;" rows="8" id="txtDetail" name="txtDetail"></textarea>
            </td>
        </tr>
        <tr>
            <td>
            </td>
            <td>
                <input id="btnSubmit" type="submit" value="保 存" />
                <input id="btnClose" type="button" value="取 消" onclick="onClose()" />
            </td>
        </tr>
    </table>
    </form>
</body>
</html>
