<%@ Control Language="C#" AutoEventWireup="false" Inherits="DotNetNuke.Modules.Admin.Host.FriendlyUrls" CodeFile="FriendlyUrls.ascx.cs" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.UI.WebControls" Assembly="DotNetNuke" %>
<asp:DataGrid ID="grdRules" AutoGenerateColumns="false" width="100%" GridLines="None" 
    CssClass="dnnUrlFriendlyGrid" Runat="server">
    <headerstyle CssClass="dnnUrlFriendlyGridHeader" />
    <itemstyle CssClass="dnnUrlFriendlyGridItem" />
    <alternatingitemstyle CssClass="dnnUrlFriendlyGridAltItem" />
    <edititemstyle />
    <selecteditemstyle />
    <footerstyle />
    <Columns>
		<asp:TemplateColumn HeaderText="Match">
		    <HeaderStyle  Width="47%" HorizontalAlign="Left" />
		    <ItemStyle  Width="47%" HorizontalAlign="Left" />
		    <ItemTemplate>
                <asp:label runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "LookFor") %>' ID="lblMatch" />
		    </ItemTemplate>
		    <EditItemTemplate>
                <asp:textbox runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "LookFor") %>' ID="txtMatch" />
		    </EditItemTemplate>
		</asp:TemplateColumn>
		<asp:TemplateColumn HeaderText="ReplaceWith">
		    <HeaderStyle  Width="200px" HorizontalAlign="Left" />
		    <ItemStyle  Width="200px" HorizontalAlign="Left" />
		    <ItemTemplate>
                <asp:label runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "SendTo") %>' ID="lblReplace" />
		    </ItemTemplate>
		    <EditItemTemplate>
                <asp:textbox runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "SendTo") %>' ID="txtReplace" />
		    </EditItemTemplate>
		</asp:TemplateColumn>
        <asp:TemplateColumn>
            <ItemStyle HorizontalAlign="Right"></ItemStyle>
            <EditItemTemplate>
	            <asp:ImageButton Runat="server" ID="lnkSave" resourcekey="saveRule" OnCommand="SaveRule" ImageUrl="~/images/save.gif" />
            </EditItemTemplate>
        </asp:TemplateColumn>
        <asp:TemplateColumn>
            <ItemStyle HorizontalAlign="Right"></ItemStyle>
            <EditItemTemplate>
	            <asp:ImageButton Runat="server" ID="lnkCancelEdit" resourcekey="cmdCancel" OnCommand="CancelEdit" ImageUrl="~/images/delete.gif" />
            </EditItemTemplate>
        </asp:TemplateColumn>
		<dnn:imagecommandcolumn commandname="Edit" imageurl="~/images/edit.gif" />
		<dnn:imagecommandcolumn commandname="Delete" imageurl="~/images/delete.gif" />
    </Columns>
</asp:DataGrid>
<ul class="dnnActions rfAddRule dnnClear"><li><asp:LinkButton ID="cmdAddRule" runat="server" resourcekey="cmdAdd" CssClass="dnnPrimaryAction" /></li></ul>