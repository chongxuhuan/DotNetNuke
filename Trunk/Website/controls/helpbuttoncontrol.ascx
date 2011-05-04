<%@ Control Language="C#" AutoEventWireup="false" Inherits="DotNetNuke.UI.UserControls.HelpButtonControl" targetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<asp:linkbutton id=cmdHelp tabindex="-1" runat="server" CausesValidation="False" enableviewstate="False">
  <asp:image id="imgHelp"  runat="server" imageurl="~/images/help.gif" enableviewstate="False" />
</asp:linkbutton>
<br/>
<asp:panel id=pnlHelp runat="server" cssClass="Help" enableviewstate="False">
  <asp:label id=lblHelp runat="server" enableviewstate="False" />
</asp:panel>
