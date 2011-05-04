<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Control Language="C#" AutoEventWireup="false" Inherits="DotNetNuke.Modules.SearchInput.InputSettings" CodeFile="InputSettings.ascx.cs" %>
<table cellSpacing="2" cellPadding="2" border="0">
	<tr>
		<td nowrap width="180" class="SubHead"><dnn:label id="plModuleCombo" runat="server" controlname="cboModule" text="Search Results Module:"></dnn:label></td>
		<td  width="300">
			<asp:label id="txtModule" runat="server" cssClass="NormalBold"></asp:label>
			<asp:dropdownlist id="cboModule" runat="server" Width="300"></asp:dropdownlist>
		</td>
	</tr>
	<TR>
		<TD nowrap width="180" class="SubHead"><dnn:label id="plGoCheck" runat="server" controlname="chkGo" text="Show Go Image:"></dnn:label></TD>
		<TD width="300"><asp:CheckBox id="chkGo" runat="server"></asp:CheckBox></TD>
	</TR>
	<TR>
		<TD nowrap width="180" class="SubHead"><dnn:label id="plSearchCheck" runat="server" controlname="chkSearchImage" text="Show Search Image:"></dnn:label></TD>
		<TD width="300"><asp:CheckBox id="chkSearchImage" runat="server"></asp:CheckBox></TD>
	</TR>
</table>
