<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Index.aspx.cs" Inherits="net91com.Stat.Web.Index"
    EnableViewStateMac="false" EnableViewState="false" EnableEventValidation="false" %>

<%@ Import Namespace="Ext.Net.Utilities" %>
<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <script src="Scripts/jquery-1.5.2.min.js" type="text/javascript"></script>
    <meta http-equiv="Content-Type" content="text/html; charset=UTF-8" />
    <title>风灵数据统计</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <ext:ResourceManager ID="ResourceManager1" DirectMethodNamespace="CompanyX" runat="server">
        </ext:ResourceManager>
        <ext:Viewport ID="Viewport1" runat="server">
            <Items>
                <ext:BorderLayout ID="BorderLayout1" runat="server">
                    <North>
                        <ext:Panel ID="pnlNorth" runat="server" Height="110" Border="false" Header="false"
                            BodyStyle="background-color:#1C3E7E;">
                            <Content>
                                <div style="font-size: 24px; color: White; font-weight: bold;">
                                    <image src="Images/indexhead.jpg" style="width: 100%;"></image>
                                </div>
                            </Content>
                            <BottomBar>
                                <ext:Toolbar ID="Toolbar1" Region="South" runat="server" Height="25px">
                                    <Items>
                                       

                                        <ext:Label ID="label1" Text="欢迎您：" runat="server">
                                        </ext:Label>
                                      
                                         <ext:Label ID="label2" StyleSpec="color:blue; margin-left:110px;" Text="建议使用Chrome浏览器" LabelStyle="color:blue" runat="server">
                                        </ext:Label>
                                         <ext:ToolbarFill/>
                                         
                                       <ext:Button runat="server" OnClientClick="GetAboatUs()" ID="btnaboatus" StyleSpec="margin-right:20px" Icon="ApplicationHome" Text="最近更新">
                                         </ext:Button>
                                        <ext:Button runat="server" ID="btnexit" Icon="ApplicationForm" Text="退出">
                                            <DirectEvents>
                                                <Click OnEvent="BtnExit" />
                                            </DirectEvents>
                                        </ext:Button>
                                    </Items>
                                </ext:Toolbar>
                            </BottomBar>
                        </ext:Panel>
                    </North>
                    <West Collapsible="true" Split="true" MinWidth="175" MaxWidth="400" MarginsSummary="5 0 5 5"
                        CMarginsSummary="5 5 5 5">
                        <ext:Panel ID="Panel1" runat="server" Title="操作菜单" Layout="AccordionLayout" Width="200">
                            <TopBar>
                                <ext:Toolbar ID="Toolbar2" runat="server" Height="24">
                                    <Items>
                                        <ext:TextField runat="server" ID="txtField" EmptyText="搜索.." EnableKeyEvents="true"
                                            Width="190">
                                            <Listeners>
                                                <SpecialKey Fn="TxtFieldSpecialKey" />
                                                <KeyUp Fn="TxtFieldKeyUp" />
                                            </Listeners>
                                        </ext:TextField>
                                    </Items>
                                </ext:Toolbar>
                            </TopBar>
                            <Items>
                            </Items>
                        </ext:Panel>
                    </West>
                    <Center MarginsSummary="5 5 5 0">
                        <ext:TabPanel ID="TabPanel1" runat="server" Region="Center" EnableTabScroll="true">
                            <Plugins>
                                <ext:TabCloseMenu ID="TabCloseMenu1" runat="server" CloseAllTabsText="关闭所有选项卡" CloseOtherTabsText="关闭其他选项卡"
                                    CloseTabText="关闭选项卡" />
                                <ext:TabScrollerMenu ID="TabScrollerMenu1" runat="server" PageSize="5">
                                </ext:TabScrollerMenu>
                            </Plugins>
                        </ext:TabPanel>
                    </Center>
                </ext:BorderLayout>
            </Items>
        </ext:Viewport>
        <ext:Window ID="Window1" runat="server" Title="Hello World!" Icon="Application" Height="185px"
            Width="200px" BodyStyle="background-color: #fff;" Padding="5" Collapsible="true"
            Modal="true" Hidden="true" AutoScroll="true" AutoShow="false">
            <Content>
            </Content>
        </ext:Window>
        <%-- <table>
      <tr style="border-bottom:solid 1px #cccc"></tr>
      </table>--%>
    </div>
    <script type="text/javascript" language="javascript">
      var treeNodeJson = <%=TreeNodeString %>;
      var _window ;
      var searchText='';
     function TxtFieldSpecialKey(field, e){
      
      }
      function TxtFieldKeyUp(field, e) {
          var txt=$("#txtField").val();
          if(_window==undefined||_window==null)
          {
             _window = new Ext.Window({
                                      width:200,
                                      autoHeight:true,
                                      height:250,
                                      bodyStyle:'padding:3px',
                                      resizable:false,
                                      draggable:false,
                                      plain:false,
                                      autoScroll:true,
                                      id:'wind1',
                                      //closeAction:'close',
                                      html:'<div id="loaddiv"></div>',
                                      X:250,
                                      Y:20
                                      });
                                      
           
          }
          if(txt.length>0&&searchText!=txt){
                searchText=txt;
                var re = new RegExp(".*"+txt+".*");
                var str=' <table width="162px">';
                var count=0;
                for(var i = 0;i<treeNodeJson.length;i++)
                {
                    if(treeNodeJson[i].Title.search(re)!=-1)
                    {
                        var json = 'pid:"'+treeNodeJson[i].pid+'",id:"'+treeNodeJson[i].ID+'",title:"'+treeNodeJson[i].Title+'",url:"'+treeNodeJson[i].url+'"';
                        str+="<tr onclick='Checked(this)' val='"+json+"' style='cursor:pointer;height:18px;'><td style='cursor:pointer;'>"+treeNodeJson[i].Title+"</td></tr><tr height='2'><td width='100%'><hr style='width:100%'/></td></tr>";
                        count++;
                    }
                }
                str+='</table>';
              if(count>0){
                    if(count>10)
                    {
                        _window.setHeight(250);
                        _window.setautoScroll(true);
                    }
                   _window.show();
                   $("#loaddiv").html(str);
                   _window.setPosition(5,158);
                   $("#loaddiv").mouseleave(function(){
                        _window.hide();
                   });
               }
            }
            else
                _window.hide();
      };
      function Checked(obj)
      {
            _window.hide();
            
            var json = eval('({'+$(obj).attr("val")+'})');
            var father = window.parent;
            var id = json.id;
            var url = json.url;
            var title =json.title;
            var menupanleid = json.pid;
            var menuitem = father.Ext.getCmp(id);
            var menupanel = father.Ext.getCmp(menupanleid);
            father.moveTab(id);
            father.addTab(father.TabPanel1, 'idClt' + id, url, title, menuitem, menupanel);
      }
    </script>
    <script type="text/javascript">
        var id = '<%=id %>';
        var url = '<%=url%>';
        var title = '<%=title %>';
        var aboatusurl = '<%=aboatusurl %>';
        var menupanleid = '<%=menupanleid %>';
        var moveTab = function (id) {
            var menupanel = Ext.getCmp('idClt' + id);
            if(menupanel)
                TabPanel1.remove(menupanel);
        }


        var addTab = function (tabPanel, id, url, title, menuItem, menupanel) {

            var tab = tabPanel.getComponent(id);
            if (!tab) {
                tab = tabPanel.add({
                    id: id,
                    title: title,
                    closable: true,
                    menuItem: menuItem,
                    autoLoad: {
                        showMask: true,
                        url: url,
                        mode: "iframe",
                        maskMsg: "正在加载 " + title + "..."
                    }

                });
                tab.on("activate", function (tab) { 
                    if (menupanel) {
                        menupanel.expand();
                        menupanel.setSelection(menuItem);
                    }
                });

            }
            else {
                tab.reload();
            }
            if (menupanel)
                 tabPanel.setActiveTab(tab);
        }
        ///左边没有菜单对象
        var addOtherTab = function (tabPanel, id, url, title, menuItem, menupanel) {

                var tab = tabPanel.getComponent(id);
                if (!tab) {
                    tab = tabPanel.add({
                        id: id,
                        title: title,
                        closable: true,
                        menuItem: menuItem,
                        autoLoad: {
                            showMask: true,
                            url: url,
                            mode: "iframe",
                            maskMsg: "正在加载 " + title + "..."
                        }

                    });

                }
                else {
                    tab.reload();
                }
                tabPanel.setActiveTab(tab);

        }
       
    </script>
    </form>
    <script type="text/javascript">

        Ext.onReady(function () {
            var menuitem = Ext.getCmp(id);
            var menupanel = Ext.getCmp(menupanleid);
            if (menuitem) {
                addTab(TabPanel1, 'idClt' + id, url, title, menuitem, menupanel);
            }
        });
     
        function GetAboatUs() {
            var menuitem = Ext.getCmp(id);
            var menupanel = Ext.getCmp(menupanleid);
            if (menuitem) {
                addTab(TabPanel1, 'id_aboartus', aboatusurl, '最近更新', menuitem, menupanel);
            }
        }
    
    </script>
</body>
</html>
