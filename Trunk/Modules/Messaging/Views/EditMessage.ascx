<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="EditMessage.ascx.cs" Inherits="DotNetNuke.Modules.Messaging.Views.EditMessage" %>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke.Web" Namespace="DotNetNuke.Web.UI.WebControls" %>
<%@ Register TagPrefix="dnn" TagName="TextEditor" Src="~/controls/TextEditor.ascx" %>
<table cellpadding="2" cellspacing="2">
    <tr>
        <td class="SubHead" style="vertical-align:top">
            <dnn:DnnFieldLabel id="toFieldLabel" runat="server" Text="EmailTo.Text" ToolTip="EmailTo.ToolTip" />
        </td>
        <td class="NormalTextBox">
            <asp:Label ID="toLabel" runat="server" />
            <dnn:DnnTextBox ID="toTextBox" runat="server" width="350px" />
			<asp:LinkButton ID="validateUserButton" Runat="server" CssClass="CommandButton" resourceKey="Validate" CausesValidation="false"/>
            <asp:RequiredFieldValidator ID="toValidator" ControlToValidate="toTextBox" runat="server" ResourceKey="EmailTo.Required" Display="Dynamic" />
        </td>
    </tr>
    <tr> 
        <td class="SubHead" style="vertical-align:top">
            <dnn:DnnFieldLabel id="subjectFieldLabel" runat="server" Text="Subject.Text" ToolTip="Subject.ToolTip" />
        </td>
        <td class="NormalTextBox">  
            <dnn:DnnTextBox ID="subjectTextBox" runat="server" cssClass="NormalTextBox" width="550" MaxLength="100"/>
            <asp:RequiredFieldValidator ID="subjectValidator" ControlToValidate="subjectTextBox" runat="server" ResourceKey="Subject.Required" Display="Dynamic" />
        </td>
    </tr>
    <tr>
        <td class="SubHead" style="vertical-align:top">
            <dnn:DnnFieldLabel id="messageFieldLabel" runat="server" Text="Message.Text" ToolTip="Message.ToolTip"/>
        </td>
        <td class="NormalTextBox">
           <dnn:TextEditor ID="messageEditor" runat="server" Width="550" TextRenderMode="Raw" HtmlEncode="False"
				defaultmode="Rich" height="300" choosemode="False" chooserender="False" />
       </td>
    </tr>
</table>
<br />
<p>
    <asp:Button ID="sendMessageButton" runat="server" resourceKey="SendMessage"/>&nbsp;&nbsp;&nbsp;&nbsp;
    <asp:LinkButton ID="saveDraftButton" runat="server" resourceKey="SaveDraft"/>&nbsp;&nbsp;&nbsp;&nbsp;
    <asp:PlaceHolder ID="deleteHolder" runat="server">
        <asp:LinkButton ID="deleteMessage" runat="server" resourceKey="DeleteMessage" CausesValidation="false" />&nbsp;&nbsp;&nbsp;&nbsp;
    </asp:PlaceHolder>
    <asp:LinkButton ID="cancelEdit" runat="server" resourceKey="CancelEdit" CausesValidation="false" />
</p>
