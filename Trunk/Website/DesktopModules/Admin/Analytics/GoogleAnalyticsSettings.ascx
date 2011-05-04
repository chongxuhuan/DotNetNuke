<%@ Control Language="C#" Inherits="DotNetNuke.Modules.Admin.Analytics.GoogleAnalyticsSettings" AutoEventWireup="false" CodeFile="GoogleAnalyticsSettings.ascx.cs" %>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke" Namespace="DotNetNuke.UI.WebControls" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<table cellspacing="2" cellpadding="2" border="0">
    <tr>
        <td class="SubHead"><dnn:label id="lblTrackingId" runat="server" 
                controlname="txtTrackingId" suffix=":"></dnn:label></td>
    </tr>
    <tr>
        <td><asp:textbox id="txtTrackingId" runat="server" width="200px" cssclass="NormalTextBox"></asp:textbox></td>
    </tr>
    <tr>
        <td class="SubHead"><dnn:label id="lblUrlParameter" runat="server" controlname="txtUrlParameter" suffix=":"></dnn:label></td>
    </tr>
    <tr>
        <td><asp:textbox id="txtUrlParameter" runat="server" textmode="multiline" rows="3" width="600px" columns="75" cssclass="NormalTextBox"></asp:textbox></td>
    </tr>
</table>
<p>
	<dnn:commandbutton id="cmdUpdate" runat="server" CssClass="CommandButton" resourcekey="cmdUpdate" ImageUrl="~/images/save.gif" />&nbsp;
</p>

