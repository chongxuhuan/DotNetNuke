<%@ Control Language="VB" AutoEventWireup="false" CodeFile="WizardUser.ascx.vb" Inherits="DotNetNuke.Services.Install.WizardUser" %>
<table cellpadding="0" cellspacing="0" border="0">
     <tr>
        <td class="NormalBold" width="150"><asp:Label ID="lblFirstName" runat="server" /></td>
        <td class="Normal" width="250"><asp:TextBox ID="txtFirstName" runat="Server" /></td>
    </tr>
     <tr>
        <td class="NormalBold" width="150"><asp:Label ID="lblLastName" runat="server" /></td>
        <td class="Normal" width="250"><asp:TextBox ID="txtLastName" runat="Server" /></td>
    </tr>
     <tr>
        <td class="NormalBold" width="150"><asp:Label ID="lblUserName" runat="server" /></td>
        <td class="Normal" width="250"><asp:TextBox ID="txtUserName" runat="Server" /></td>
    </tr>
     <tr>
        <td class="NormalBold" width="150"><asp:Label ID="lblPassword" runat="server" /></td>
        <td class="Normal" width="250"><asp:TextBox ID="txtPassword" runat="Server" TextMode="password" /></td>
    </tr>
     <tr>
        <td class="NormalBold" width="150"><asp:Label ID="lblConfirm" runat="server" /></td>
        <td class="Normal" width="250"><asp:TextBox ID="txtConfirm" runat="Server" TextMode="password" /></td>
    </tr>
     <tr>
        <td class="NormalBold" width="150"><asp:Label ID="lblEmail" runat="server" /></td>
        <td class="Normal" width="250"><asp:TextBox ID="txtEmail" runat="Server" Width="250px" /></td>
    </tr>
</table>
