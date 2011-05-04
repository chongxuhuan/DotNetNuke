<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="ViewMessage.ascx.cs" Inherits="DotNetNuke.Modules.Messaging.Views.ViewMessage" %>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke.Web" Namespace="DotNetNuke.Web.UI.WebControls" %>
<%@ Register TagPrefix="dnn" TagName="TextEditor" Src="~/controls/TextEditor.ascx" %>
<table cellpadding="2" cellspacing="2">
    <tr>
        <td style="vertical-align: top">
            <dnn:DnnFieldLabel ID="fromFieldLabel" runat="server" Text="EmailFrom.Text" ToolTip="EmailFrom.ToolTip"
                class="SubHead" />
            <asp:Label ID="fromLabel" runat="server" class="NormalTextBox" />
        </td>
    </tr>
    <tr>
        <td style="vertical-align: top">
            <dnn:DnnFieldLabel ID="subjectFieldLabel" runat="server" Text="Subject.Text" ToolTip="Subject.ToolTip"
                class="SubHead" />
            <asp:Label ID="subjectLabel" runat="server" class="NormalTextBox" />
        </td>
    </tr>
    <tr>
        <td class="NormalTextBox">
            <br />
            <asp:Label ID="messageLabel" runat="server" />
        </td>
    </tr>
</table>
<br />
<p>
    <asp:Button ID="replyMessage" runat="server" resourceKey="ReplyMessage" CausesValidation="false" />&nbsp;&nbsp;&nbsp;&nbsp;
    <asp:LinkButton ID="deleteMessage" runat="server" resourceKey="DeleteMessage" CausesValidation="false" />&nbsp;&nbsp;&nbsp;&nbsp;
    <asp:LinkButton ID="cancelView" runat="server" resourceKey="CancelView" CausesValidation="false" />
</p>
