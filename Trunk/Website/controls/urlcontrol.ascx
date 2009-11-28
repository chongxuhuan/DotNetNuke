<%@ Control Language="vb" AutoEventWireup="false" Explicit="True" Inherits="DotNetNuke.UI.UserControls.UrlControl" %>
<table cellspacing="0" cellpadding="0" border="0">
    <tr id="TypeRow" runat="server">
        <td style="white-space:nowrap">
            <br/>
            <asp:Label ID="lblURLType" runat="server" EnableViewState="False" resourcekey="Type" CssClass="NormalBold" />
            <br/>
            <asp:RadioButtonList ID="optType" CssClass="NormalBold" AutoPostBack="True" RepeatDirection="Vertical" runat="server" />
            <br/>
        </td>
    </tr>
    <tr id="URLRow" runat="server">
        <td style="white-space:nowrap">
            <asp:Label ID="lblURL" runat="server" EnableViewState="False" resourcekey="URL" CssClass="NormalBold" />
            <asp:DropDownList ID="cboUrls" runat="server" DataTextField="Url" DataValueField="Url" CssClass="NormalTextBox" Width="300" />
            <asp:TextBox ID="txtUrl" runat="server" CssClass="NormalTextBox" Width="300" />
            <br/>
            <asp:LinkButton ID="cmdSelect" resourcekey="Select" CssClass="CommandButton" runat="server" CausesValidation="False" />
            <asp:LinkButton ID="cmdDelete" resourcekey="Delete" CssClass="CommandButton" runat="server" CausesValidation="False" />
            <asp:LinkButton ID="cmdAdd" resourcekey="Add" CssClass="CommandButton" runat="server" CausesValidation="False" />
        </td>
    </tr>
    <tr id="TabRow" runat="server">
        <td style="white-space:nowrap">
            <asp:Label ID="lblTab" runat="server" EnableViewState="False" resourcekey="Tab" CssClass="NormalBold" />
            <asp:DropDownList ID="cboTabs" runat="server" DataTextField="IndentedTabName" DataValueField="TabId" CssClass="NormalTextBox" Width="300" />
        </td>
    </tr>
    <tr id="FileRow" runat="server">
        <td style="white-space:nowrap">
            <asp:Label ID="lblFolder" runat="server" EnableViewState="False" resourcekey="Folder" CssClass="NormalBold" />
            <asp:DropDownList ID="cboFolders" runat="server" AutoPostBack="True" CssClass="NormalTextBox" Width="300" />
            <asp:Image ID="imgStorageLocationType" runat="server" Visible="False" />
            <br />
            <asp:Label ID="lblFile" runat="server" EnableViewState="False" resourcekey="File" CssClass="NormalBold" />
            <asp:DropDownList ID="cboFiles" runat="server" DataTextField="Text" DataValueField="Value" CssClass="NormalTextBox" Width="300" />
            <input id="txtFile" type="file" size="30" name="txtFile" runat="server" width="300" />
            <br />
            <asp:LinkButton ID="cmdUpload" resourcekey="Upload" CssClass="CommandButton" runat="server" CausesValidation="False" />
            <asp:LinkButton ID="cmdSave" resourcekey="Save" CssClass="CommandButton" runat="server" CausesValidation="False" />
            <asp:LinkButton ID="cmdCancel" resourcekey="Cancel" CssClass="CommandButton" runat="server" CausesValidation="False" />
        </td>
    </tr>
    <tr id="ImagesRow" runat="server">
        <td style="white-space:nowrap">
            <asp:Label ID="lblImages" runat="server" EnableViewState="False" resourcekey="Image" CssClass="NormalBold" />
            <asp:DropDownList ID="cboImages" runat="server" CssClass="NormalTextBox" Width="300" />
        </td>
    </tr>
    <tr id="UserRow" runat="server">
        <td style="white-space:nowrap">
            <asp:Label ID="lblUser" runat="server" EnableViewState="False" resourcekey="User" CssClass="NormalBold" />
            <asp:TextBox ID="txtUser" runat="server" CssClass="NormalTextBox" Width="300" />
        </td>
    </tr>
    <tr id="ErrorRow" runat="server">
        <td>
            <asp:Label ID="lblMessage" runat="server" EnableViewState="False" CssClass="NormalRed" />
            <br />
        </td>
    </tr>
    <tr>
        <td style="white-space:nowrap">
            <asp:CheckBox ID="chkTrack" resourcekey="Track" CssClass="NormalBold" runat="server" Text="Track?" TextAlign="Right" />
            <asp:CheckBox ID="chkLog" resourcekey="Log" CssClass="NormalBold" runat="server" Text="Log?" TextAlign="Right" />
            <asp:CheckBox ID="chkNewWindow" resourcekey="NewWindow" CssClass="NormalBold" runat="server" Text="New Window?" TextAlign="Right" Visible="False" />
            <br />
        </td>
    </tr>
</table>
