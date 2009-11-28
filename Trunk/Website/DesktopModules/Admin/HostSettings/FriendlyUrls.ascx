<%@ Control Language="VB" AutoEventWireup="false" CodeFile="FriendlyUrls.ascx.vb" Inherits="DotNetNuke.Modules.Admin.Host.FriendlyUrls" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.UI.WebControls" Assembly="DotNetNuke" %>
<asp:DataGrid ID="grdRules" AutoGenerateColumns="false" width="475px" 
    CellPadding="2" GridLines="None" cssclass="DataGrid_Container" Runat="server">
    <headerstyle cssclass="NormalBold" verticalalign="Top" horizontalalign="Center"/>
    <itemstyle cssclass="Normal" horizontalalign="Left" />
    <alternatingitemstyle cssclass="Normal" />
    <edititemstyle cssclass="NormalTextBox" />
    <selecteditemstyle cssclass="NormalRed" />
    <footerstyle cssclass="DataGrid_Footer" />
    <Columns>
		<dnn:imagecommandcolumn commandname="Edit" imageurl="~/images/edit.gif"/>
		<dnn:imagecommandcolumn commandname="Delete" imageurl="~/images/delete.gif" />
		<asp:TemplateColumn HeaderText="Match">
		    <HeaderStyle  Width="200px" HorizontalAlign="Left" />
		    <ItemStyle  Width="200px" HorizontalAlign="Left" />
		    <ItemTemplate>
                <asp:label runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "LookFor") %>' CssClass="Normal" ID="lblMatch" Width="250px"/>
		    </ItemTemplate>
		    <EditItemTemplate>
                <asp:textbox runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "LookFor") %>' CssClass="NormalTextBox" ID="txtMatch" Width="250px"/>
		    </EditItemTemplate>
		</asp:TemplateColumn>
		<asp:TemplateColumn HeaderText="ReplaceWith">
		    <HeaderStyle  Width="200px" HorizontalAlign="Left" />
		    <ItemStyle  Width="200px" HorizontalAlign="Left" />
		    <ItemTemplate>
                <asp:label runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "SendTo") %>' CssClass="Normal" ID="lblReplace" Width="250px"/>
		    </ItemTemplate>
		    <EditItemTemplate>
                <asp:textbox runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "SendTo") %>' CssClass="NormalTextBox" ID="txtReplace" Width="250px"/>
		    </EditItemTemplate>
		</asp:TemplateColumn>
        <asp:TemplateColumn>
            <ItemStyle HorizontalAlign="Right"  width="20px"></ItemStyle>
            <EditItemTemplate>
	            <asp:ImageButton Runat="server" ID="lnkSave" resourcekey="saveRule" OnCommand="SaveRule" ImageUrl="~/images/save.gif" />
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
<dnn:CommandButton ID="cmdAddRule" runat="server" ResourceKey="cmdAdd" ImageUrl="~/images/add.gif" />
