<%@ Control Language="C#" AutoEventWireup="false" Inherits="DotNetNuke.UI.Skins.Controls.User" CodeFile="User.ascx.cs" %>

<div class="registerGroup">
    <ul class="buttonGroup">
        <li class="userMessages alpha"><asp:HyperLink ID="messageLink" runat="server"/></li>
        <li class="userNotifications omega"><asp:HyperLink ID="notificationLink" runat="server"/></li>
    	<li class="userDisplayName"><asp:HyperLink ID="registerLink" runat="server"/></li>
        <li class="userProfileImg"><asp:HyperLink ID="avatar" runat="server"/></li>                                       
    </ul>
</div><!--close registerGroup-->