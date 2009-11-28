<%@ Control language="vb" AutoEventWireup="false" Explicit="True" Inherits="DotNetNuke.UI.UserControls.Help" %>
<div align="left">
	<asp:Label ID="lblHelp" Runat="server" CssClass="Normal" Width="100%" enableviewstate="False" />
    <br/><br/>
    <asp:linkbutton id="cmdCancel" runat="server" class="CommandButton" resourcekey="cmdCancel" borderstyle="none" text="Cancel" causesvalidation="False" enableviewstate="False" />
    &nbsp;&nbsp;
    <asp:HyperLink id="cmdHelp" Runat="server" CssClass="CommandButton" resourcekey="cmdHelp" Target="_new" Text="View Online Help" enableviewstate="False" />
    <br/><br/><br />
	<asp:Label ID="lblInfo" Runat="server" CssClass="Normal" Width="100%" enableviewstate="False" />
</div>
