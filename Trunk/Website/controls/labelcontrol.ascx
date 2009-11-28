<%@ Control Language="vb" AutoEventWireup="false" Inherits="DotNetNuke.UI.UserControls.LabelControl" targetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<label id=label runat="server">
  <span style="width:15px">
  <asp:linkbutton id=cmdHelp tabindex="-1" runat="server" CausesValidation="False" enableviewstate="False">
    <asp:image id="imgHelp"  runat="server" imageurl="~/images/help.gif" enableviewstate="False" />
  </asp:linkbutton>
  </span>
  <asp:label id=lblLabel runat="server" enableviewstate="False" />
</label>
<br/>
<asp:panel id=pnlHelp runat="server" cssClass="Help" enableviewstate="False">
  <asp:label id=lblHelp runat="server" enableviewstate="False" />
</asp:panel>
