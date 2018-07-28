<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ProjectSourceManage.aspx.cs"
    Inherits="net91com.Stat.Web.UserRights.ProjectSourceManage" %>

<%@ Import Namespace="Ext.Net.Utilities" %>
<%@ Import Namespace="net91com.Reports.UserRights" %>
<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>项目来源管理</title>
    <script type="text/javascript">
        ///保存修改
        var SaveChanged = function () {
            GridPanel1.save();
        }

        var reload = function () {
            isSearch.setValue("1");
            GridPanel1.store.load();
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
        <ext:Viewport ID="Viewport2" runat="server" Layout="BorderLayout">
            <Items>
                <ext:Panel ID="Panel1" runat="server" Header="false" CollapseMode="Mini" Collapsible="true"
                    MinHeight="80" Height="120" Region="North" Split="true" Margins="4 4 0 4" Layout="FitLayout"
                    Border="false">
                    <Items>
                        <ext:FormPanel ID="MyForm" runat="server" Icon="ApplicationEdit" Frame="true" Border="false"
                            Title="添加项目来源">
                            <Items>
                                <ext:Container ID="Container1" runat="server" Layout="ColumnLayout" Height="50">
                                    <Items>
                                        <ext:Container ID="Container2" runat="server" Layout="TableLayout">
                                            <Items>
                                                <ext:NumberField ID="txtSourceID" runat="server" FieldLabel="来源ID" AllowBlank="false"
                                                    MaxValue="9999" LabelAlign="Right">
                                                </ext:NumberField>
                                                <ext:TextField ID="txtSourceName" runat="server" FieldLabel="来源名称" AllowBlank="false"
                                                    MaxLength="50" LabelAlign="Right">
                                                </ext:TextField>
                                                <ext:TextField ID="txtSoftID" runat="server" FieldLabel="软件ID" AllowBlank="false"
                                                    MaxValue="999999" LabelAlign="Right">
                                                </ext:TextField> 
                                                 <ext:NumberField ID="txtNumSort" runat="server" Text="0"  StyleSpec="margin-left:8px;"  FieldLabel="排序ID(降序)" AllowBlank="false" BlankText="0"
                                                    MaxValue="9999" LabelAlign="Right" >
                                                </ext:NumberField>

                                                 <%-- <ext:SelectBox ID="hiddenselect"  FieldLabel="启用" Text="是"  AllowBlank="false"  runat="server" LabelAlign="Right">
                                                    <Items>
                                                        <ext:ListItem Text="是" Value="1"  />
                                                        <ext:ListItem Text="否" Value="0" /> 
                                                    </Items>
                                                 </ext:SelectBox>--%>
                                             
                                            </Items>
                                        </ext:Container>
                                    </Items>
                                </ext:Container>
                                
                                <ext:Container ID="Container4" runat="server" Layout="FormLayout"  Height="50">
                                <Items>
                                       <ext:Container ID="Container3" runat="server" Layout="TableLayout">
                                            <Items>
                                        
                                                 <ext:SelectBox ID="sourcetypeselect"  FieldLabel="项目类型" SelectedIndex="0"  Width="250px"  AllowBlank="false"  runat="server" LabelAlign="Right">
                                                    <Items>
                                                        <ext:ListItem Text="不限" Value="0" />
                                                        <ext:ListItem Text="国内" Value="1"  />
                                                        <ext:ListItem Text="海外" Value="2"  />
                                                        <ext:ListItem Text="台湾" Value="3"  />
                                                     </Items>
                                                 </ext:SelectBox>
                                                 
                                                 <ext:SelectBox ID="onlyinternalselect"  FieldLabel="仅内部查看" SelectedIndex="0"   Width="250px"   AllowBlank="false"  runat="server" LabelAlign="Right">
                                                    <Items>
                                                        <ext:ListItem Text="是" Value="1"  /> 
                                                        <ext:ListItem Text="否" Value="0" />
                                                        
                                                     </Items>
                                                 </ext:SelectBox>

                                                <ext:Button ID="btnSave" runat="server" StyleSpec=" margin-left:150px" Width="75px" Text="添加" Icon="Disk">
                                                    <DirectEvents>
                                                        <Click OnEvent="OnSave" Success="CallFormSuccess(response, result, el, type, action, extraParams, o)">
                                                        </Click>
                                                    </DirectEvents>
                                                    <Listeners>
                                                        <Click Fn="ValFildeForAdd" />
                                                    </Listeners>
                                                </ext:Button>
                               </Items>
                               </ext:Container>
                                 </Items>
                               </ext:Container>
                               

                            </Items>
                        </ext:FormPanel>
                    </Items>
                </ext:Panel>
                <ext:Panel ID="Panel2" runat="server" Title="项目来源配置" Region="Center" Margins="0 4 0 4"
                    Layout="FitLayout">
                    <Items>
                        <ext:GridPanel Border="false" ID="GridPanel1" runat="server" AutoWidth="true" Layout="FitLayout"
                            Frame="false" Height="550">
                            <Store>
                                <ext:Store runat="server" ID="Store4" AutoSave="false" OnBeforeStoreChanged="HandleChanges" 
                                    ShowWarningOnFailure="false" AutoLoad="true" OnRefreshData="MyData_Refresh" UseIdConfirmation="true">
                                    <Reader>
                                        <ext:JsonReader IDProperty="ProjectSourceID">
                                            <Fields>
                                                <ext:RecordField Name="ProjectSourceID" Type="Int" />
                                                <ext:RecordField Name="Name" Type="String" />
                                                <ext:RecordField Name="SoftID" Type="Int" />
                                                <ext:RecordField Name="SortIndex" Type="Int" />
                                                <ext:RecordField Name="ProjectSourceType" Type="String" />
                                                <ext:RecordField Name="OnlyInternal" Type="Boolean" />
                                                <ext:RecordField Name="Status" Type="Boolean" />
                                            </Fields>
                                        </ext:JsonReader>
                                    </Reader>
                                    <Proxy>
                                        <ext:PageProxy>
                                        </ext:PageProxy>
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
                                    <ext:Column DataIndex="ProjectSourceID" Width="100" Header="来源ID">
                                        <Editor>
                                            <ext:NumberField ID="TextField1" runat="server" AllowBlank="false" MaxValue="9999">
                                            </ext:NumberField>
                                        </Editor>
                                    </ext:Column>
                                    <ext:Column DataIndex="Name" Width="200" Header="来源名称">
                                        <Editor>
                                            <ext:TextField ID="TextField2" runat="server" AllowBlank="false" MaxLength="50">
                                            </ext:TextField>
                                        </Editor>
                                    </ext:Column>
                                    <ext:Column DataIndex="SoftID" Width="100" Header="软件ID">
                                        <Editor>
                                            <ext:NumberField ID="NumberField1" runat="server" AllowBlank="false" MaxValue="99999999">
                                            </ext:NumberField>
                                        </Editor>
                                    </ext:Column>

                                     <ext:Column DataIndex="SortIndex" Width="100" Header="排序ID">
                                        <Editor> 
                                            <ext:NumberField ID="TextField3" runat="server" AllowBlank="false" MaxLength="50">
                                            </ext:NumberField>
                                        </Editor>
                                    </ext:Column>
                                    
                                    <ext:Column DataIndex="ProjectSourceType" Width="200" Header="项目类型">
                                        <Editor> 
                                            <ext:SelectBox ID="SelectBox1"  FieldLabel="项目类型" Text="0" Width="150px"  AllowBlank="false"  runat="server" LabelAlign="Right">
                                                    <Items>
                                                        <ext:ListItem Text="未知" Value="None" />
                                                        <ext:ListItem Text="国内" Value="Domestic"  />
                                                        <ext:ListItem Text="海外" Value="Oversea"  />
                                                        <ext:ListItem Text="台湾" Value="Traditional"  />
                                                     </Items>
                                             </ext:SelectBox>
                                        </Editor>
                                    </ext:Column>
                                    
                                     <ext:CheckColumn DataIndex="OnlyInternal" Editable="true"  Width="100" Header="仅内部查看" >
                                     </ext:CheckColumn>

                                     <ext:CheckColumn DataIndex="Status" Editable="true"  Width="100" Header="是否启用" >
                                     </ext:CheckColumn>

                                    <ext:CommandColumn Width="70">
                                        <Commands>
                                            <ext:GridCommand Text="回撤" ToolTip-Text="回撤一行" CommandName="reject" Icon="ArrowUndo" />
                                        </Commands>
                                        <PrepareToolbar Handler="toolbar.items.get(0).setVisible(record.dirty);" />
                                    </ext:CommandColumn>
                                    <ext:CommandColumn Width="70">
                                        <Commands>
                                            <ext:GridCommand Text="删除" ToolTip-Text="删除一行" CommandName="delete" Icon="Delete" />
                                        </Commands>
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
                                        <ext:Parameter Name="SourceID" Value="record.data.ProjectSourceID" Mode="Raw">
                                        </ext:Parameter>
                                        <ext:Parameter Name="SoftID" Value="record.data.SoftID" Mode="Raw">
                                        </ext:Parameter>
                                        <ext:Parameter Name="Command" Value="command" Mode="Raw">
                                        </ext:Parameter>
                                    </ExtraParams>
                                </Command>
                            </DirectEvents>
                            <LoadMask ShowMask="true" Msg="正在加载..." />
                            <TopBar>
                                <ext:Toolbar ID="Toolbar2" runat="server">
                                    <Items>
                                        <ext:Button ID="Button2" runat="server" Text="保存修改" Icon="Disk">
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
