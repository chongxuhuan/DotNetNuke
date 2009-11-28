<%@ Control language="vb" AutoEventWireup="false" Explicit="True" Inherits="DotNetNuke.Modules.Admin.Portals.PortalAliases" CodeFile="PortalAliases.ascx.vb" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.UI.WebControls" Assembly="DotNetNuke" %>
<asp:DataGrid ID="dgPortalAlias" Runat="server" AutoGenerateColumns="false" width="475px" 
    CellPadding="2" GridLines="None" cssclass="DataGrid_Container">
    <headerstyle cssclass="NormalBold" verticalalign="Top" horizontalalign="Center"/>
    <itemstyle cssclass="Normal" horizontalalign="Left" />
    <alternatingitemstyle cssclass="Normal" />
    <edititemstyle cssclass="NormalTextBox" />
    <selecteditemstyle cssclass="NormalRed" />
    <footerstyle cssclass="DataGrid_Footer" />
	<Columns>
		<dnn:imagecommandcolumn commandname="Edit" imageurl="~/images/edit.gif"/>
		<dnn:imagecommandcolumn commandname="Delete" imageurl="~/images/delete.gif" />
		<asp:TemplateColumn HeaderText="HTTPAlias">
		    <HeaderStyle  Width="350px" HorizontalAlign="Left" />
		    <ItemStyle  Width="350px" HorizontalAlign="Left" />
		    <ItemTemplate>
                <asp:label runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "HTTPAlias") %>' CssClass="Normal" ID="lbHTTPAlias" Width="350px"/>
		    </ItemTemplate>
		    <EditItemTemplate>
                <asp:textbox runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "HTTPAlias") %>' CssClass="NormalTextBox" ID="txtHTTPAlias" Width="350px"/>
		    </EditItemTemplate>
		</asp:TemplateColumn>
        <asp:TemplateColumn>
            <ItemStyle HorizontalAlign="Right"  width="20px"></ItemStyle>
            <EditItemTemplate>
	            <asp:ImageButton Runat="server" ID="lnkSave" resourcekey="saveRule" OnCommand="SaveAlias" ImageUrl="~/images/save.gif" />
            </EditItemTemplate>
        </asp:TemplateColumn>
        <asp:TemplateColumn>
            <ItemStyle HorizontalAlign="Right"  width="20px"></ItemStyle>
            <EditItemTemplate>
	            <asp:ImageButton Runat="server" ID="lnkCancelEdit" resourcekey="cmdCancel" OnCommand="CancelEdit" ImageUrl="~/images/delete.gif" />
            </EditItemTemplate>
        </asp:TemplateColumn>
	</Columns>
</asp:DataGrid>
<br />
<asp:Label ID="lblError" runat="server" Visible="false" CssClass="NormalRed" />
<dnn:CommandButton ID="cmdAddAlias" runat="server" ResourceKey="cmdAdd" ImageUrl="~/images/add.gif" />
