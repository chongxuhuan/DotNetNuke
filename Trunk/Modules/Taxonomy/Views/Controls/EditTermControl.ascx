<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="EditTermControl.ascx.cs" Inherits="DotNetNuke.Modules.Taxonomy.Views.Controls.EditTermControl" %>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke.Web" Namespace="DotNetNuke.Web.UI.WebControls" %>
<table cellpadding="2" cellspacing="2">
    <tr>
        <td class="SubHead" style="vertical-align:top">
            <dnn:DnnFieldLabel id="nameFieldLabel" runat="server" Text="TermName.Text" ToolTip="TermName.ToolTip" />
        </td>
        <td class="NormalTextBox">
            <dnn:DnnTextBox ID="nameTextBox" runat="server" />
            <asp:RequiredFieldValidator ID="nameValidator" ControlToValidate="nameTextBox" runat="server" ResourceKey="TermName.Required" Display="Dynamic" />
        </td>
    </tr>
    <tr> 
        <td class="SubHead" style="vertical-align:top">
            <dnn:DnnFieldLabel id="descriptionFieldLabel" runat="server" Text="Description.Text" ToolTip="Description.ToolTip" />
        </td>
        <td class="NormalTextBox">  
            <dnn:DnnTextBox ID="descriptionTextBox" runat="server" TextMode="MultiLine" cssClass="NormalTextBox"/>
        </td>
    </tr>
    <tr id="parentTermRow" runat="server"> 
        <td class="SubHead" style="vertical-align:top">
            <dnn:DnnFieldLabel id="parentTermLabel" runat="server" Text="ParentTerm.Text" ToolTip="ParentTerm.ToolTip" />
        </td>
        <td class="NormalTextBox"> 
             <dnn:DnnComboBox ID="parentTermCombo" runat="server" DataTextField="Name" DataValueField="TermId" Width="125px"/>
        </td>
    </tr>
</table>
