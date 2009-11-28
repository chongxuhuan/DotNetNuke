<%@ Control Language="vb" AutoEventWireup="false" Explicit="true" Inherits="DotNetNuke.Modules.SearchResults.ResultsSettings" CodeFile="ResultsSettings.ascx.vb" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<table cellSpacing="0" cellPadding="2" summary="Edit Search Design Table" border="0">
	<tr>
        <td class="SubHead" width="250"><dnn:label id="plResults" runat="server" controlname="txtresults" text="Maximum Search Results:"></dnn:label></td>
		<td class="NormalTextBox"><asp:textbox id="txtresults" runat="server" MaxLength="5" CssClass="NormalTextBox"></asp:textbox>
            <asp:CompareValidator ID="CompareValidator5" runat="server" CssClass="NormalRed"
                resourcekey="Validation.ErrorMessage" Display="Dynamic" ControlToValidate="txtresults" Operator="GreaterThan" Type="Integer" ValueToCompare="0"></asp:CompareValidator></td>
	</tr>
	<tr>
        <td class="SubHead" width="250"><dnn:label id="plPage" runat="server" controlname="txtPage" text="Results per Page:"></dnn:label></td>
		<td class="NormalTextBox"><asp:textbox id="txtPage" runat="server" MaxLength="5" CssClass="NormalTextBox"></asp:textbox>
            <asp:CompareValidator ID="CompareValidator1" runat="server" ControlToValidate="txtPage"
                CssClass="NormalRed" Display="Dynamic" Operator="GreaterThan" resourcekey="Validation.ErrorMessage"
                Type="Integer" ValueToCompare="0"></asp:CompareValidator></td>
	</tr>
	<tr>
        <td class="SubHead" width="250"><dnn:label id="plTitle" runat="server" controlname="txtTitle" text="Maximum Title Length:"></dnn:label></td>
		<td class="NormalTextBox"><asp:textbox id="txtTitle" runat="server" MaxLength="5" CssClass="NormalTextBox"></asp:textbox>
            <asp:CompareValidator ID="CompareValidator2" runat="server" ControlToValidate="txtTitle"
                CssClass="NormalRed" Display="Dynamic" Operator="GreaterThan" resourcekey="Validation.ErrorMessage"
                Type="Integer" ValueToCompare="0"></asp:CompareValidator></td>
	</tr>
	<tr>
        <td class="SubHead" width="250" ><dnn:label id="plDescription" runat="server" controlname="txtdescription" text="Maximum Description Length:"></dnn:label></td>
		<td class="NormalTextBox" ><asp:textbox id="txtdescription" runat="server" MaxLength="5" CssClass="NormalTextBox"></asp:textbox>
            <asp:CompareValidator ID="CompareValidator3" runat="server" ControlToValidate="txtdescription"
                CssClass="NormalRed" Display="Dynamic" Operator="GreaterThan" resourcekey="Validation.ErrorMessage"
                Type="Integer" ValueToCompare="0"></asp:CompareValidator></td>
	</tr>
	<tr>
		<td colSpan="2">&nbsp;</td>
	</tr>
	<tr>
        <td class="SubHead" width="250"><dnn:label id="plShowDescription" runat="server" controlname="chkDescription" text="Show Description?"></dnn:label></td>
		<td class="NormalTextBox"><asp:checkbox id="chkDescription" runat="server" CssClass="NormalTextBox"></asp:checkbox></td>
	</tr>
</table>
