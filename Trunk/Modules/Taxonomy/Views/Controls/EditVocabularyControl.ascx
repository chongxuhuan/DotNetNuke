<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="EditVocabularyControl.ascx.cs" Inherits="DotNetNuke.Modules.Taxonomy.Views.Controls.EditVocabularyControl" %>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke.Web" Namespace="DotNetNuke.Web.UI.WebControls" %>
<table cellpadding="2" cellspacing="2">
    <tr>
        <td class="SubHead" style="vertical-align:top">
            <dnn:DnnFieldLabel id="nameFieldLabel" runat="server" Text="Name.Text" ToolTip="Name.ToolTip" />
        </td>
        <td class="NormalTextBox">
            <asp:Label ID="nameLabel" runat="server" />
            <dnn:DnnTextBox ID="nameTextBox" runat="server" style="width:150px" />
            <asp:RequiredFieldValidator ID="nameValidator" ControlToValidate="nameTextBox" runat="server" ResourceKey="Name.Required" Display="Dynamic" />
        </td>
    </tr>
    <tr> 
        <td class="SubHead" style="vertical-align:top">
            <dnn:DnnFieldLabel id="descriptionFieldLabel" runat="server" Text="Description.Text" ToolTip="Description.ToolTip" />
        </td>
        <td class="NormalTextBox">  
            <dnn:DnnTextBox ID="descriptionTextBox" runat="server" TextMode="MultiLine" cssClass="NormalTextBox" style="height:50px;width:150px" />
        </td>
    </tr>
    <tr>
        <td class="SubHead" style="vertical-align:top">
            <dnn:DnnFieldLabel id="typeFieldLabel" runat="server" Text="Type.Text" ToolTip="Type.ToolTip"/>
        </td>
        <td class="NormalTextBox">
            <asp:RadioButtonList ID="typeList" runat="server" RepeatDirection="Horizontal">
                <asp:ListItem Value="Simple" resourceKey="Simple" />
                <asp:ListItem Value="Hierarchy" resourceKey="Hierarchy" />
            </asp:RadioButtonList>
            <asp:Label ID="typeLabel" runat="server" />
        </td>
    </tr>
    <tr id="scopeRow" runat="server">
        <td class="SubHead" style="vertical-align:top">
            <dnn:DnnFieldLabel id="scopeFieldLabel" runat="server" Text="Scope.Text" ToolTip="Scope.ToolTip"/>
        </td>
        <td class="NormalTextBox">
            <asp:RadioButtonList ID="scopeList" runat="server" RepeatDirection="Horizontal">
                <asp:ListItem Value="Application" resourceKey="Application" />
                <asp:ListItem Value="Portal" resourceKey="Portal" />
            </asp:RadioButtonList>
            <asp:Label ID="scopeLabel" runat="server" />
        </td>
    </tr>
</table>
