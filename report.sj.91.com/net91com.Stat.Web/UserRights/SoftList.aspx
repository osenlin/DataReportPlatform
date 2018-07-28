<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SoftList.aspx.cs" Inherits="net91com.Stat.Web.UserRights.SoftList" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<%@ Import Namespace="Ext.Net.Utilities" %>
<%@ Import Namespace="net91com.Reports.UserRights" %>
<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
     <script type="text/javascript">        
         
         var reload = function () {
             isSearch.setValue("1");
             GridPanel1.store.load();
         }

         var ValFilde = function () {
             if (!SoftForm.getForm().isValid()) {
                 Ext.Msg.show({ icon: Ext.MessageBox.ERROR, msg: "表单没有填写完整", buttons: Ext.Msg.OK });
                 return false;
             }
             var plats = "";
             for (var i = 0; i < platChecksGroup.items.length; i++) {
                 var ck = platChecksGroup.items.itemAt(i);
                 if (ck.checked == true) {
                     plats += ck.rawValue + "-";
                 }
             }
             myselectplat.setValue(plats);
             return true;
         };
         ValFildeForEdit = function () {
             if (ValFilde()) {
                 if (SoftPID.getValue() == "") {
                     Ext.Msg.show({ icon: Ext.MessageBox.ERROR, msg: "软件ID不能为空", buttons: Ext.Msg.OK });
                     return false;
                 }
                 
                 return true;
             }
             return false;
         }

         ValFildeForAdd = function () {
             if (ValFilde()) {
               
                 return true;
             }
             return false;
         }

         var CallBack = function (response, result, el, type, action, extraParams, o) {
             if (result.extraParamsResponse.plats != "") {
                 clearAllChecks();
                 var checks = result.extraParamsResponse.plats.split(",");
                 for (var i = 0; i < platChecksGroup.items.length; i++) {
                     var ck = platChecksGroup.items.itemAt(i);
                     for (var j = 0; j < checks.length; j++) {
                         if (ck.boxLabel == checks[j]) {
                             ck.setValue(true);
                         }
                     }

                 }
                 SoftPID.setDisabled(true);
                 btnAdd.setDisabled(true);
                 btnSave.setDisabled(false);
             }

         }
         function clearAllChecks() {
             for (var i = 0; i < platChecksGroup.items.length; i++) {
                 var ck = platChecksGroup.items.itemAt(i);
                 ck.setValue(false);
             }
         }
         var CallFormSuccess = function (response, result, el, type, action, extraParams, o) {
             if (result.extraParamsResponse.success == "1")
                 GridPanel1.store.load();
             if (extraParams.Command == "delete") { 
                SoftForm.getForm().reset();SoftPID.setDisabled(false);btnAdd.setDisabled(false);btnSave.setDisabled(true);
             }

         }

         ///确定弹出框是否执行config.extraParams 里包含了额外的参数
         var ConfirmationDel = function (config) {
              return true;
         };

     </script>
</head>
<body>
    <form id="form1" runat="server">
        <ext:ResourceManager ID="ResourceManager1" runat="server" RethrowAjaxExceptions="True">
    </ext:ResourceManager>
       <ext:Hidden ID="hiddensoft" runat="server" Text="0" />
       <ext:Hidden ID="hiddensofttype" runat="server" Text="0" />
       <ext:Hidden ID="myselectplat" runat="server" Text="" />
       <ext:Hidden ID="isSearch" runat="server" Text="1" />
    <div>
     <ext:Viewport ID="Viewport2" runat="server" Layout="BorderLayout">
            <Items>
           
          <ext:Panel ID="Panel1" runat="server" Header="false" CollapseMode="Mini" Collapsible="true"
                Height="200" MinHeight="150" Region="North" Split="true" Margins="4 4 0 4" Layout="FitLayout"
                Border="false">
                <Items>


                 <ext:FormPanel 
                        ID="SoftForm" 
                        runat="server"
                        Icon="ApplicationEdit"
                        Frame="true" Border="false" 
                        Title="产品编辑">
                        <Items>
                           <ext:Container runat="server" Layout="ColumnLayout" Height="50">
                                <Items>
                                      <ext:Container runat="server" Layout="FormLayout" ColumnWidth="0.25">
                                    <Items>
                                     <ext:TextField ID="SoftName" runat="server"
                                            AllowBlank="false"
                                            FieldLabel="软件名称"
                                        />
                                    </Items>
                                    </ext:Container>
                                      <ext:Container ID="Container1" runat="server" Layout="FormLayout" ColumnWidth="0.25">
                                    <Items>
                                           <ext:NumberField  ID="SoftPID" runat="server"
                                            FieldLabel="软件ID" AllowBlank="false"  />
                
                                    </Items>
                                    </ext:Container>
                                      <ext:Container ID="Container2" runat="server" Layout="FormLayout" ColumnWidth="0.25">
                                    <Items>
                                     <ext:SelectBox ID="softType"  FieldLabel="产品类型" Width="130" AllowBlank="false"  runat="server">
                                    <Items>
                                        <ext:ListItem Text="内部产品" Value="1" />
                                        <ext:ListItem Text="外部产品" Value="2" />
                                        <ext:ListItem Text="产品组合" Value="3" />
                                    </Items>
                            
                                    </ext:SelectBox>
                                    </Items>
                                    </ext:Container>
                                      <ext:Container ID="Container3" runat="server" Layout="FormLayout" ColumnWidth="0.25">
                                    <Items>
                                         <ext:NumberField ID="SoftOutID" runat="server"
                                            FieldLabel="软件备份ID"
                                            AllowBlank="false"  />
                                    </Items>
                                    </ext:Container>
                                </Items>
                           </ext:Container>
                           
                            <ext:Container ID="Container4" runat="server" Layout="ColumnLayout"  Height="30">
                                <Items>
                                    <ext:Container ID="Container5" runat="server" Layout="FormLayout" ColumnWidth="0.33">
                                        <Items>
                                        <ext:NumberField ID="SortNumID" runat="server" Text="100"
                                            FieldLabel="排序ID(值小的排前面)"
                                            AllowBlank="false"  />
                                            </Items>
                                      </ext:Container>
                                    <ext:Container ID="Container6" runat="server" Layout="FormLayout" ColumnWidth="0.33">
                                        <Items>
                                       <ext:SelectBox ID="onlyinternalselect"  FieldLabel="是否内部查看"  Width="150px" SelectedIndex="0"  AllowBlank="false"  runat="server" LabelAlign="Right">
                                                    <Items>
                                                        <ext:ListItem Text="是" Value="1"/>
                                                        <ext:ListItem Text="否" Value="0"/> 
                                                     </Items>
                                       </ext:SelectBox>
                                     </Items>
                                    </ext:Container>
                                     <ext:Container ID="Container7" runat="server" Layout="FormLayout" ColumnWidth="0.33">
                                        <Items>
                                       <ext:SelectBox ID="softareatype"  FieldLabel="区域:"  Width="150px" SelectedIndex="0"  AllowBlank="false"  runat="server" LabelAlign="Right">
                                                    <Items>
                                                        <ext:ListItem Text="国内" Value="1"/>
                                                        <ext:ListItem Text="海外" Value="2"/> 
                                                        <ext:ListItem Text="台湾" Value="3"/> 
                                                     </Items>
                                       </ext:SelectBox>
                                     </Items>
                                    </ext:Container>
                                </Items>
                            </ext:Container>
                            <ext:Container runat="server" Layout="FormLayout"  Height="60">
                                <Items>
                                       <ext:CheckboxGroup  ID="platChecksGroup" runat="server" AllowBlank="false">
                                        <Items>
                                             <ext:Checkbox ID="platcheck" runat="server" BoxLabel="平台1" Checked="true" />
                                        </Items>
                                         </ext:CheckboxGroup>
                                </Items>
                            </ext:Container>
                           
                           
                           
                        
                      
                        </Items>
            
                        <Buttons>
                            <ext:Button ID="btnSave" 
                                runat="server"
                                Text="保存修改"
                                Icon="Disk">
                                <DirectEvents>
                                    <Click OnEvent="OnSave" Success="CallFormSuccess(response, result, el, type, action, extraParams, o)">
                                     <ExtraParams>
                                            <ext:Parameter Name="plats" Value="#{myselectplat}.getValue()"  Mode="Raw">
                                            </ext:Parameter>
                                    </ExtraParams>
                                    </Click>
                                </DirectEvents>
                                <Listeners>
                                     <Click Fn="ValFildeForEdit" />
                                </Listeners>

                            </ext:Button>
                            
                        <ext:Button ID="btnAdd" 
                                runat="server"
                                Text="创建"
                                Icon="UserAdd">
                                <DirectEvents>
                                    <Click OnEvent="AddSoft" Success="CallFormSuccess(response, result, el, type, action, extraParams, o)">
                                     <ExtraParams>
                                            <ext:Parameter Name="plats" Value="#{myselectplat}.getValue()" Mode="Raw">
                                            </ext:Parameter>
                                    </ExtraParams>
                                     </Click>
                                </DirectEvents>
                                <Listeners>
                                     <Click Fn="ValFildeForAdd"  />
                                </Listeners>
                             </ext:Button>
                
                            <ext:Button ID="Button5" 
                                runat="server"
                                Text="重置">
                                <Listeners>
                                    <Click Handler="#{SoftForm}.getForm().reset();SoftPID.setDisabled(false);btnAdd.setDisabled(false);btnSave.setDisabled(true);" />
                                </Listeners>
                            </ext:Button>
                        </Buttons>
                 
                    </ext:FormPanel> 
               
                 </Items>
            </ext:Panel>     
                
                 
       <ext:Panel ID="Panel2" runat="server" Title="软件列表" Region="Center" Margins="0 4 0 4"    Layout="FitLayout" >
                         <Items>
               <ext:GridPanel Border="false" ID="GridPanel1" runat="server" AutoWidth="true" Layout="FitLayout"
                    Frame="false" >
                    <Store>
                    <ext:Store runat="server" ID="Store3"  AutoSave="false"  
                        ShowWarningOnFailure="false" AutoLoad="true"  OnRefreshData="MyData_Refresh"  >
                        <Reader> 
                            <ext:JsonReader IDProperty="ID">
                                <Fields>
                                    <ext:RecordField Name="SoftType" Type="String"     />
                                    <ext:RecordField Name="ID" Type="Int"   />
                                    <ext:RecordField Name="Name" Type="String"   />
                                    <ext:RecordField Name="OutID" Type="Int"    />
                                    <ext:RecordField Name="SortIndex"  Type="Int"   />
                                </Fields>
                            </ext:JsonReader>
                        </Reader>
                        <Proxy>
                               <ext:PageProxy></ext:PageProxy>
                        </Proxy>
                         <AutoLoadParams>
                                    <ext:Parameter Name="start" Value="={0}">
                                    </ext:Parameter>
                                    <ext:Parameter Name="limit" Value="={20}">
                                    </ext:Parameter>
                         </AutoLoadParams>
                       
                        
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
                         
                        </Listeners>
                    </ext:Store></Store>
                    <ColumnModel ID="ColumnModel1" runat="server">
                        <Columns>
                            <ext:Column DataIndex="SoftType" Width="120" Header="产品类型"   >
                            </ext:Column>
                            <ext:Column DataIndex="ID" Width="150" Header="产品ID" />
                            <ext:Column DataIndex="Name" Width="150" Header="产品名称" />
                            <ext:Column DataIndex="OutID" Width="150" Header="产品备份ID"  >
                            </ext:Column>
                            <ext:Column DataIndex="SortIndex" Width="100" Header="排序ID"  >
                            </ext:Column>
                            <ext:CommandColumn Width="80">
                            <Commands>
                           <ext:GridCommand Text="删除" ToolTip-Text="删除一行"  CommandName="delete" Icon="Delete" />
                                
                            </Commands>
                             </ext:CommandColumn>
                           
                             
                        </Columns>
                         
                    </ColumnModel>


                    <SelectionModel>
                        <ext:RowSelectionModel runat="server" SingleSelect="true">
                          <DirectEvents>
                                <RowSelect OnEvent="RowSelect" Success="CallBack(response, result, el, type, action, extraParams, o)" Buffer="100">
                                    <EventMask ShowMask="true" Target="CustomTarget" CustomTarget="#{SoftForm}" />
                                    <ExtraParams>
                                       <ext:Parameter Name="SoftID" Value="this.getSelected().id" Mode="Raw" />
                                    </ExtraParams>
                                </RowSelect>
                            </DirectEvents>
                               
                                
                        </ext:RowSelectionModel>
                    </SelectionModel>
                     
                     <DirectEvents>
                          <Command OnEvent="OnCommand" Success="CallFormSuccess(response, result, el, type, action, extraParams, o)">
                                <Confirmation ConfirmRequest="true" Title="提示" Message="确认要删除吗？" BeforeConfirm="return ConfirmationDel(config);" />
                               <ExtraParams>
                                  <ext:Parameter Name="SoftID" Value="record.id" Mode="Raw">
                                    </ext:Parameter>
                                   <ext:Parameter Name="Command" Value="command" Mode="Raw">
                                    </ext:Parameter>

                                </ExtraParams>
                            </Command>
                            
                     </DirectEvents>
                    <LoadMask ShowMask="true" Msg="正在加载..." />
                    <BottomBar>
                    
                        <ext:PagingToolbar ID="pagecut" DisplayInfo="true" runat="server"  PageSize="20">
                        
                        </ext:PagingToolbar>
                     </BottomBar>
                    <TopBar>
                          <ext:Toolbar  ID="toptool" runat="server" HideBorders="false" >
                    <Items>
                        
                       
                         <ext:SelectBox ID="SelectSoftType" FieldLabel="产品类型" Width="200" runat="server">
                            <Items>
                                <ext:ListItem Text="内部产品" Value="1"  />
                                <ext:ListItem Text="外部产品" Value="2" />
                                <ext:ListItem Text="产品组合" Value="3" />
                            </Items>
                            
                            </ext:SelectBox>
                       <ext:Label ID="Label3" StyleSpec="margin-left:15px;margin-right:8px" Text="产品名称:"
                            runat="server" />
                        <ext:TextField ID="txtsoftName" runat="server" />
                        <ext:Button ID="Button1" runat="server" Text="查找" Icon="Find">
                            <Listeners>
                                <Click Handler="reload()" />
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
