<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ViewMessage.ascx.vb" Inherits="DotNetNuke.Modules.Messaging.Views.ViewMessage" %>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke.Web" Namespace="DotNetNuke.Web.UI.WebControls" %>
<%@ Register TagPrefix="dnn" TagName="TextEditor" Src="~/controls/TextEditor.ascx" %>
<table cellpadding="2" cellspacing="2">
    <tr>
        <td class="SubHead" style="vertical-align:top">
            <dnn:DnnFieldLabel id="fromFieldLabel" runat="server" Text="EmailFrom.Text" ToolTip="EmailFrom.ToolTip" />
        </td>
        <td class="NormalTextBox">
            <asp:Label ID="fromLabel" runat="server" />
        </td>
    </tr>
    <tr> 
        <td class="SubHead" style="vertical-align:top">
            <dnn:DnnFieldLabel id="subjectFieldLabel" runat="server" Text="Subject.Text" ToolTip="Subject.ToolTip" />
        </td>
        <td class="NormalTextBox">  
            <asp:Label ID="subjectLabel" runat="server" />
        </td>
    </tr>
    <tr>
        <td class="SubHead" style="vertical-align:top">
            <dnn:DnnFieldLabel id="messageFieldLabel" runat="server" Text="Message.Text" ToolTip="Message.ToolTip"/>
        </td>
        <td class="NormalTextBox">
            <asp:Label ID="messageLabel" runat="server" />
        </td>
    </tr>
</table>
<br />
<p>
    <asp:Button ID="replyMessage" runat="server" resourceKey="ReplyMessage" CausesValidation="false" />&nbsp;&nbsp;&nbsp;&nbsp;
    <asp:LinkButton ID="cancelView" runat="server" resourceKey="CancelView" CausesValidation="false" />
</p>
