<%@ Control Language="C#" AutoEventWireup="false" CodeFile="foldermappings.ascx.cs" Inherits="DotNetNuke.Modules.Admin.FileManager.FolderMappings" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.UI.WebControls" Assembly="DotNetNuke" %>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke.Web" Namespace="DotNetNuke.Web.UI.WebControls" %>
<script language="javascript" type="text/javascript">
<!--
    function grdMappings_OnRowDragStarted(sender, args) {
        if (sender.get_id() == "<%=grdMappings.ClientID %>") {
            var node = args.get_gridDataItem();
            if (node._itemIndexHierarchical < 3) {
                args.set_cancel(true);
            }
        }
    }

    function grdMappings_OnRowDropping(sender, args) {
        if (sender.get_id() == "<%=grdMappings.ClientID %>") {
            if (args._targetItemIndexHierarchical == "0" ||
            args._targetItemIndexHierarchical == "1" ||
            (args._targetItemIndexHierarchical == "2" && args._dropPosition == "above")) {
                args.set_cancel(true);
            }
        }
    }
-->
</script>
<table cellpadding="2" cellspacing="2" border="0" width="500px">
    <tr>
        <td>
            <asp:Label ID="lbDescription" runat="server" CssClass="Normal" resourcekey="Description.Text"></asp:Label>
        </td>
    </tr>
    <tr>
        <td>
            <h2><asp:Label ID="lbTableHeader" runat="server" resourcekey="TableHeader.Text"></asp:Label></h2>
            <dnn:dnngrid id="grdMappings" runat="server" autogeneratecolumns="false" Width="98%"
                OnRowDrop="grdMappings_OnRowDrop" OnNeedDataSource="grdMappings_NeedDataSource"
                AllowAutomaticUpdates="false" AllowAutomaticDeletes="false"
                OnItemCommand="grdMappings_ItemCommand" OnItemDataBound="grdMappings_ItemDataBound">
                <MasterTableView DataKeyNames="FolderMappingID">
                    <Columns>
                        <dnn:DnnGridTemplateColumn>
                            <ItemStyle Width="60px" />
                            <ItemTemplate>
                                <dnn:commandbutton id="cmdEditMapping" runat="server" imageurl="~/images/edit.gif" commandname="Edit" commandargument='<%# Eval("FolderMappingID") %>' causesvalidation="false" visible='<%# Eval("IsEditable") %>' />
                                <dnn:commandbutton id="cmdDeleteMapping" runat="server" imageurl="~/images/delete.gif" commandname="Delete" commandargument='<%# Eval("FolderMappingID") %>' causesvalidation="false" visible='<%# Eval("IsEditable") %>' />
                            </ItemTemplate>
                        </dnn:DnnGridTemplateColumn>
                        <dnn:DnnGridBoundColumn DataField="MappingName" HeaderText="Name" />
                        <dnn:DnnGridBoundColumn DataField="FolderProviderType" HeaderText="Type" />
                        <dnn:DnnGridTemplateColumn HeaderText="Enabled">
                            <HeaderStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" Width="70px" />
                            <ItemTemplate>
                                <asp:Button ID="btnChangeAvailability" runat="server" CommandName="ChangeAvailability" CommandArgument='<%# Eval("FolderMappingID") %>' style="display:none" />
                                <asp:CheckBox ID="chkEnabled" runat="server" Checked='<%# Eval("IsEnabled") %>' Enabled='<%# Eval("IsEditable") %>' />
                            </ItemTemplate>
                        </dnn:DnnGridTemplateColumn>
                    </Columns>
                </MasterTableView>
                <ClientSettings AllowRowsDragDrop="true">
                    <Selecting AllowRowSelect="True" EnableDragToSelectRows="false" />
                    <ClientEvents OnRowDragStarted="grdMappings_OnRowDragStarted" OnRowDropping="grdMappings_OnRowDropping" />
                    <Scrolling AllowScroll="true" UseStaticHeaders="true" ScrollHeight="191" />
                </ClientSettings>
            </dnn:dnngrid>
        </td>
    </tr>
</table>
<p>
    <dnn:commandbutton id="cmdNewMapping" resourcekey="cmdNewMapping" runat="server" cssclass="CommandButton" imageurl="~/images/add.gif" causesvalidation="False" />
    <dnn:commandbutton id="cmdCancel" resourcekey="cmdCancel" runat="server" cssclass="CommandButton" imageurl="~/images/lt.gif" causesvalidation="False" />
</p>