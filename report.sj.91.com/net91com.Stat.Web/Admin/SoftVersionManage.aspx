<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SoftVersionManage.aspx.cs"
    Inherits="net91com.Stat.Web.Admin.SoftVersionManage" %>

<%@ Import Namespace="Ext.Net.Utilities" %>
<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>软件版本编辑</title>
    <script type="text/javascript">
        var SaveChanged = function () {
            GridPanel1.save();
        }
        var callBackCommand = function () {
            record.reject();
        }
        var reload = function () {
            isSearch.setValue("1");
            GridPanel1.store.load();
        }
        var clearData = function () {
            add_txtVersion.setValue("");

        }
        ValFildeForAdd = function () {
            return MyForm.getForm().isValid();
        }

        var CallFormSuccess = function (response, result, el, type, action, extraParams, o) {
            if (result.extraParamsResponse.success == "1") {
                GridPanel1.store.load();

            }
        }


        ///确定弹出框是否执行config.extraParams 里包含了额外的参数
        var ConfirmationDel = function (config) {
            ///只有删除的时候才有弹出框
            if (config.extraParams.Command == "delete")
                return true;
            else
                return false;
        };
        var IsDelete = function (cmd) {
            ///只有删除的时候才有弹出框
            if (cmd == "delete")
                return true;
            else
                return false;
        };
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <ext:ResourceManager ID="ResourceManager1" runat="server" RethrowAjaxExceptions="True">
        </ext:ResourceManager>
        <div>
            <ext:Hidden ID="hiddensoft" runat="server" Text="0" />
            <ext:Hidden ID="hiddenplat" runat="server" Text="0" />
            <ext:Hidden ID="hiddenversion" runat="server" Text="" />
            <ext:Hidden ID="isSearch" runat="server" Text="0" />
            <ext:Viewport ID="Viewport2" runat="server" Layout="border">
                <Items>
                    <ext:Panel ID="Panel3" runat="server" Header="false" CollapseMode="Mini" Collapsible="true"
                        Height="100" MinHeight="100" Region="North" Split="true" Margins="4 4 0 4" Layout="FitLayout"
                        Border="false">
                        <Items>
                            <ext:FormPanel ID="MyForm" runat="server" Icon="ApplicationEdit" Frame="true" Border="false"
                                Title="添加版本信息（注：添加iPhone版本时，会自动添加iPad版本）">
                                <Items>
                                    <ext:Container ID="Container1" runat="server" Layout="ColumnLayout" Height="70">
                                        <Items>
                                            <ext:Container ID="Container2" runat="server" Layout="FormLayout" ColumnWidth="0.20">
                                                <Items>

                                                    <ext:ComboBox Width="150" ID="add_comboxSofts" DisplayField="name" FieldLabel="产品" ValueField="id" runat="server">
                                                        <Listeners>
                                                            <Select Handler="Ext.net.DirectMethods.SetPlat2()" />
                                                        </Listeners>
                                                        <Store>
                                                            <ext:Store runat="server" ID="Store4">
                                                                <Reader>
                                                                    <ext:ArrayReader>
                                                                        <Fields>
                                                                            <ext:RecordField Name="id" Type="String" Mapping="ID">
                                                                            </ext:RecordField>
                                                                            <ext:RecordField Name="name" Type="String" Mapping="Name">
                                                                            </ext:RecordField>
                                                                        </Fields>
                                                                    </ext:ArrayReader>
                                                                </Reader>

                                                            </ext:Store>
                                                        </Store>
                                                    </ext:ComboBox>
                                                </Items>
                                            </ext:Container>
                                            <ext:Container ID="Container3" runat="server" Layout="FormLayout" ColumnWidth="0.20">
                                                <Items>

                                                    <ext:ComboBox ID="add_comboxPlats" Width="100" runat="server" FieldLabel="平台" DisplayField="name" ValueField="id">
                                                        <Store>
                                                            <ext:Store ID="Store5" runat="server">
                                                                <Reader>
                                                                    <ext:ArrayReader>
                                                                        <Fields>
                                                                            <ext:RecordField Name="id" Type="Int" Mapping="PlatID">
                                                                            </ext:RecordField>
                                                                            <ext:RecordField Name="name" Type="String" Mapping="PlatName">
                                                                            </ext:RecordField>
                                                                        </Fields>
                                                                    </ext:ArrayReader>
                                                                </Reader>
                                                            </ext:Store>
                                                        </Store>
                                                        <Listeners>
                                                            <Select Handler="clearData()" />
                                                        </Listeners>
                                                    </ext:ComboBox>
                                                </Items>
                                            </ext:Container>
                                            <ext:Container ID="Container4" runat="server" Layout="FormLayout" ColumnWidth="0.20">
                                                <Items>
                                                    <ext:TextField ID="add_txtVersion" AllowBlank="False" FieldLabel="版本" runat="server" />
                                                </Items>
                                            </ext:Container>
                                            <ext:Container ID="Container5" runat="server" Layout="FormLayout" ColumnWidth="0.20">
                                                <Items>
                                                    <ext:Checkbox runat="server" ID="add_HiddenCheckbox" FieldLabel="是否隐藏"></ext:Checkbox>
                                                </Items>
                                            </ext:Container>
                                        </Items>
                                    </ext:Container>

                                </Items>

                                <Buttons>
                                    <ext:Button ID="Button3" runat="server" Text="添加" Icon="Disk">
                                        <DirectEvents>
                                            <Click OnEvent="OnSave" Success="CallFormSuccess(response, result, el, type, action, extraParams, o)">
                                            </Click>
                                        </DirectEvents>
                                        <Listeners>
                                            <Click Fn="ValFildeForAdd" />
                                        </Listeners>
                                    </ext:Button>
                                </Buttons>
                            </ext:FormPanel>
                        </Items>
                    </ext:Panel>


                    <ext:Panel ID="Panel2" runat="server" Title="更改版本信息（双击单元格进行修改）（注：修改或删除iPhone版本时，会自动修改或删除iPad版本）" Region="Center" Margins="0 4 0 4" Layout="FitLayout">
                        <Items>
                            <ext:GridPanel Border="false" ID="GridPanel1" runat="server" AutoWidth="true" Layout="FitLayout"
                                Frame="false">
                                <Store>
                                    <ext:Store runat="server" ID="Store3" AutoSave="false" OnBeforeStoreChanged="HandleChanges"
                                        ShowWarningOnFailure="false" AutoLoad="true" OnRefreshData="MyData_Refresh" UseIdConfirmation="true">
                                        <Reader>
                                            <ext:JsonReader IDProperty="ID">
                                                <Fields>
                                                    <ext:RecordField Name="ID" Type="Int" />
                                                    <ext:RecordField Name="Version" Type="String" />
                                                    <ext:RecordField Name="Hidden" Type="Boolean" />
                                                </Fields>
                                            </ext:JsonReader>
                                        </Reader>
                                        <Proxy>
                                            <ext:PageProxy></ext:PageProxy>
                                        </Proxy>


                                        <Listeners>
                                            <Exception Handler="
                                Ext.net.Notification.show({
                                    iconCls    : 'icon-exclamation', 
                                    html       : e.message, 
                                    title      : 'EXCEPTION', 
                                    autoScroll : true, 
                                    hideDelay  : 5000, 
                                    width      : 300, 
                                    height     : 200
                                });" />
                                            <BeforeSave Handler="var valid = true; this.each(function(r){if(r.dirty && !r.isValid()){valid=false;}}); return valid;" />
                                        </Listeners>
                                    </ext:Store>
                                </Store>
                                <ColumnModel ID="ColumnModel1" runat="server">
                                    <Columns>
                                        <ext:Column DataIndex="ID" Width="120" Header="版本ID" Editable="false">
                                        </ext:Column>
                                        <ext:Column DataIndex="Version" Width="150" Header="版本">
                                            <Editor>
                                                <ext:TextField ID="TextField1" runat="server" AllowBlank="false"></ext:TextField>
                                            </Editor>
                                        </ext:Column>

                                        <ext:CheckColumn DataIndex="Hidden" Editable="true" Width="120" Header="是否隐藏">
                                        </ext:CheckColumn>


                                        <ext:CommandColumn Width="120">
                                            <Commands>
                                                <ext:GridCommand Text="删除" ToolTip-Text="删除一行" CommandName="delete" Icon="Delete" />
                                            </Commands>

                                        </ext:CommandColumn>
                                        <ext:CommandColumn Width="120">
                                            <Commands>
                                                <ext:GridCommand Text="回撤" ToolTip-Text="回撤一行" CommandName="reject" Icon="ArrowUndo" />
                                            </Commands>

                                            <PrepareToolbar Handler="toolbar.items.get(0).setVisible(record.dirty);" />
                                        </ext:CommandColumn>


                                    </Columns>

                                </ColumnModel>
                                <Listeners>
                                    <Command Handler="record.reject();" />
                                </Listeners>
                                <DirectEvents>
                                    <Command OnEvent="OnCommand" Before="return IsDelete(command);" Success="CallFormSuccess(response, result, el, type, action, extraParams, o)">
                                        <Confirmation ConfirmRequest="true" Title="提示" Message="确认要删除吗？" BeforeConfirm="return ConfirmationDel(config);" />
                                        <ExtraParams>
                                            <ext:Parameter Name="ID" Value="record.id" Mode="Raw">
                                            </ext:Parameter>
                                            <ext:Parameter Name="Command" Value="command" Mode="Raw">
                                            </ext:Parameter>
                                        </ExtraParams>
                                    </Command>
                                </DirectEvents>
                                <LoadMask ShowMask="true" Msg="正在加载..." />
                                <BottomBar>

                                    <ext:PagingToolbar ID="pagecut" DisplayInfo="true" runat="server" PageSize="30">
                                    </ext:PagingToolbar>
                                </BottomBar>
                                <TopBar>
                                    <ext:Toolbar ID="Toolbar1" runat="server">
                                        <Items>

                                            <ext:Label ID="Label1" StyleSpec="margin-left:15px;margin-right:8px" Text="产品:" runat="server" />
                                            <ext:ComboBox Width="150" ID="comboxSofts" DisplayField="name" ValueField="id" runat="server">
                                                <Listeners>
                                                    <Select Handler="Ext.net.DirectMethods.SetPlat()" />
                                                </Listeners>
                                                <Store>
                                                    <ext:Store runat="server" ID="Store1">
                                                        <Reader>
                                                            <ext:ArrayReader>
                                                                <Fields>
                                                                    <ext:RecordField Name="id" Type="String" Mapping="ID">
                                                                    </ext:RecordField>
                                                                    <ext:RecordField Name="name" Type="String" Mapping="Name">
                                                                    </ext:RecordField>
                                                                </Fields>
                                                            </ext:ArrayReader>
                                                        </Reader>

                                                    </ext:Store>
                                                </Store>
                                            </ext:ComboBox>
                                            <ext:Label ID="Label2" StyleSpec="margin-left:15px;margin-right:8px" Text="平台:" runat="server" />
                                            <ext:ComboBox ID="comboxPlats" Width="100" runat="server" DisplayField="name" ValueField="id">
                                                <Store>
                                                    <ext:Store ID="Store2" runat="server">
                                                        <Reader>
                                                            <ext:ArrayReader>
                                                                <Fields>
                                                                    <ext:RecordField Name="id" Type="Int" Mapping="PlatID">
                                                                    </ext:RecordField>
                                                                    <ext:RecordField Name="name" Type="String" Mapping="PlatName">
                                                                    </ext:RecordField>
                                                                </Fields>
                                                            </ext:ArrayReader>
                                                        </Reader>
                                                    </ext:Store>
                                                </Store>
                                                <Listeners>
                                                    <Select Handler="clearData()" />
                                                </Listeners>
                                            </ext:ComboBox>
                                            <ext:Label ID="Label4" StyleSpec="margin-left:15px;margin-right:8px" Text="版本:"
                                                runat="server" />
                                            <ext:TextField ID="txtversion" runat="server" />
                                            <ext:Button ID="Button1" runat="server" Text="查找" Icon="Find">
                                                <Listeners>
                                                    <Click Handler="reload()" />
                                                </Listeners>

                                            </ext:Button>
                                            <ext:Button ID="Button2" runat="server" Text="保存所有更改" Icon="Disk">
                                                <Listeners>
                                                    <Click Handler="SaveChanged()" />
                                                </Listeners>
                                            </ext:Button>
                                        </Items>
                                    </ext:Toolbar>
                                </TopBar>

                            </ext:GridPanel>
                        </Items>
                    </ext:Panel>
                </Items>
            </ext:Viewport>







        </div>
    </form>
</body>
</html>
