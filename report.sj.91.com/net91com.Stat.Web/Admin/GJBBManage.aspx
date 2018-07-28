<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GJBBManage.aspx.cs" Inherits="net91com.Stat.Web.Admin.GJBBManage" %>
<%@ Import Namespace="Ext.Net.Utilities" %>
<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>固件版本编辑</title>
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
            txtgjbb.setValue("");
            txtgjbbname.setValue("");
            
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <ext:ResourceManager ID="ResourceManager1" runat="server" RethrowAjaxExceptions="True">
    </ext:ResourceManager>
    <div>
       
       <ext:Hidden ID="hiddenplat" runat="server" Text="0" />
       <ext:Hidden ID="hiddengjbb" runat="server" Text="" />
       <ext:Hidden ID="hiddengjbbname" runat="server" Text="" />
       <ext:Hidden ID="isSearch" runat="server" Text="0" />

        <ext:Viewport ID="Viewport2" runat="server" Layout="border"  >
            <Items>
              <ext:Panel ID="Panel2"  Border="true" Header="false"  runat="server" Region="North" Margins="4 4 4 4" Layout="FitLayout" Height="30"  >
                 <Items>
                 <ext:Toolbar ID="Toolbar1" runat="server">
                    <Items>
                        
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
                       
                        <ext:Label ID="Label3" StyleSpec="margin-left:15px;margin-right:8px" Text="固件版本:"
                            runat="server" />
                        <ext:TextField ID="txtgjbb" runat="server" />

                        <ext:Label ID="Label5" StyleSpec="margin-left:15px;margin-right:8px" Text="固件版本名称:"
                            runat="server" />
                        <ext:TextField ID="txtgjbbname" runat="server" />

                       

                        <ext:Button ID="Button1" runat="server" Text="查找" Icon="Find">
                            <Listeners>
                                <Click Handler="reload()" />
                            </Listeners>
                            
                        </ext:Button>
                    </Items>
                </ext:Toolbar>
                </Items>
            </ext:Panel>

          <ext:Panel ID="Panel3" runat="server" Title="固件版本编辑(双击列进行修改)" Region="Center" Margins="0 4 0 4"    Layout="FitLayout"  Height="30" >
            <Items>
                 <ext:GridPanel Border="false" ID="GridPanel1" runat="server" AutoWidth="true" Layout="FitLayout"
                    Frame="false"  Height="550" >
                    <Store>
                    <ext:Store runat="server" ID="Store3"  AutoSave="false" OnBeforeStoreChanged="HandleChanges"
                        ShowWarningOnFailure="false" AutoLoad="true"  OnRefreshData="MyData_Refresh" UseIdConfirmation="true">
                        <Reader>
                            <ext:JsonReader IDProperty="ID">
                                <Fields>
                                    <ext:RecordField Name="ID" Type="Int"   />
                                    <ext:RecordField Name="Platform" Type="String"   />
                                    <ext:RecordField Name="GJBB" Type="String"  />
                                    <ext:RecordField Name="E_GJBB" Type="String"  />
                                  
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
                      </ext:Store></Store>
                    <ColumnModel ID="ColumnModel1" runat="server">
                        <Columns>
                            <ext:Column DataIndex="ID" Width="120" Header="固件版本ID" Editable="false" >
                            </ext:Column>
                            <ext:Column DataIndex="GJBB" Width="150" Header="固件版本" />
                            <ext:Column DataIndex="E_GJBB" Width="150" Header="固件版本名称"  >
                            <Editor>
                                <ext:TextField ID="TextField1" runat="server" AllowBlank="false" ></ext:TextField>
                            </Editor>
                            </ext:Column>
                          
                            <ext:CommandColumn Width="80">
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
                         <LoadMask ShowMask="true" Msg="正在加载..." />
                    <BottomBar>
                    
                        <ext:PagingToolbar ID="pagecut" DisplayInfo="true" runat="server"  PageSize="30">
                        
                        </ext:PagingToolbar>
                     </BottomBar>
                    <TopBar>
                    <ext:Toolbar ID="Toolbar2" runat="server">
                       <Items>
                           <ext:Button ID="Button2" runat="server"  Text="保存修改" Icon="Disk">
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
