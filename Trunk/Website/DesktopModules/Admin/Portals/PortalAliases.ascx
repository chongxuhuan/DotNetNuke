<%@ Control Language="C#" AutoEventWireup="false" Inherits="DotNetNuke.Modules.Admin.Portals.PortalAliases" CodeFile="PortalAliases.ascx.cs" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.UI.WebControls" Assembly="DotNetNuke" %>
<asp:DataGrid ID="dgPortalAlias" Runat="server" AutoGenerateColumns="false" width="100%" GridLines="None" CssClass="dnnGrid">
    <headerstyle CssClass="dnnGridHeader" />
    <itemstyle CssClass="dnnGridItem" horizontalalign="Left" />
    <alternatingitemstyle CssClass="dnnGridAltItem" />
    <edititemstyle />
    <selecteditemstyle />
    <footerstyle />
	<Columns>
		<dnn:imagecommandcolumn commandname="Edit" imageurl="~/images/edit.gif"/>
		<dnn:imagecommandcolumn commandname="Delete" imageurl="~/images/delete.gif" />
		<asp:TemplateColumn HeaderText="HTTPAlias">
		    <HeaderStyle  HorizontalAlign="Left" />
		    <ItemStyle  HorizontalAlign="Left" Width="85%" />
		    <ItemTemplate>
                <asp:label runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "HTTPAlias") %>' ID="lbHTTPAlias" />
		    </ItemTemplate>
		    <EditItemTemplate>
                <asp:textbox runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "HTTPAlias") %>' ID="txtHTTPAlias" CssClass="dnnFormInput" />
                <asp:CheckBox ID="chkChild" runat="server" resourcekey="createChild" />
		    </EditItemTemplate>
		</asp:TemplateColumn>
        <asp:TemplateColumn>
            <EditItemTemplate>
	            <asp:ImageButton Runat="server" ID="lnkSave" resourcekey="saveRule" OnCommand="SaveAlias" ImageUrl="~/images/save.gif" />
            </EditItemTemplate>
        </asp:TemplateColumn>
        <asp:TemplateColumn>
            <EditItemTemplate>
	            <asp:ImageButton Runat="server" ID="lnkCancelEdit" resourcekey="cmdCancel" OnCommand="CancelEdit" ImageUrl="~/images/delete.gif" />
            </EditItemTemplate>
        </asp:TemplateColumn>
	</Columns>
</asp:DataGrid>
<asp:Label ID="lblError" runat="server" Visible="false" CssClass="dnnFormMessage dnnFormError" />
<dnn:CommandButton ID="cmdAddAlias" runat="server" ResourceKey="cmdAdd" ImageUrl="~/images/add.gif" />