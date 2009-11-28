<%@ Control language="vb" CodeFile="Attributes.ascx.vb" AutoEventWireup="false" Explicit="True" Inherits="DotNetNuke.Modules.Admin.Skins.Attributes" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<table cellSpacing="2" cellPadding="0" width="560" summary="Attributes Design Table">
	<tr>
	    <td colspan="2">&nbsp;</td>
	</tr>
	<tr valign="bottom">
		<td class="SubHead" width="125" vAlign="top"><dnn:label id="plSkin" runat="server" controlname="cboSkins" suffix=":"></dnn:label></td>
		<td vAlign="top">
		    <asp:DropDownList ID="cboSkins" runat="server" CssClass="NormalTextBox" Width="250px" AutoPostBack="True"></asp:DropDownList>
		</td>
	</tr>
	<tr valign="bottom">
		<td class="SubHead" width="125" vAlign="top"><dnn:label id="plContainer" runat="server" controlname="cboContainers" suffix=":"></dnn:label></td>
		<td vAlign="top">
		    <asp:DropDownList ID="cboContainers" runat="server" CssClass="NormalTextBox" Width="250px" AutoPostBack="True"></asp:DropDownList>
		</td>
	</tr>
	<tr valign="bottom">
		<td class="SubHead" width="125" vAlign="top"><dnn:label id="plFile" runat="server" controlname="cboFiles" suffix=":"></dnn:label></td>
		<td vAlign="top">
		    <asp:DropDownList ID="cboFiles" runat="server" CssClass="NormalTextBox" Width="250px" AutoPostBack="True"></asp:DropDownList>
		</td>
	</tr>
	<tr valign="bottom">
		<td class="SubHead" width="125" vAlign="top"><dnn:label id="plToken" runat="server" controlname="cboTokens" suffix=":"></dnn:label></td>
		<td vAlign="top">
		    <asp:DropDownList ID="cboTokens" runat="server" CssClass="NormalTextBox" Width="250px" DataTextField="ControlKey" DataValueField="ControlSrc" AutoPostBack="True"></asp:DropDownList>
		</td>
	</tr>
	<tr valign="bottom">
		<td class="SubHead" width="125" vAlign="top"><dnn:label id="plSetting" runat="server" controlname="cboSettings" suffix=":"></dnn:label></td>
		<td vAlign="top">
		    <asp:DropDownList ID="cboSettings" runat="server" CssClass="NormalTextBox" Width="250px" AutoPostBack="True"></asp:DropDownList>
		</td>
	</tr>
	<tr valign="bottom">
		<td class="SubHead" width="125" vAlign="top"><dnn:label id="plValue" runat="server" controlname="cboValue" suffix=":"></dnn:label></td>
		<td vAlign="top">
		    <asp:TextBox ID="txtValue" runat="server" CssClass="NormalTextBox" Width="250px" Visible="false"></asp:TextBox>
		    <asp:DropDownList ID="cboValue" runat="server" CssClass="NormalTextBox" Width="250px" Visible="false"></asp:DropDownList>
		    <asp:Label ID="lblHelp" runat="server" CssClass="Normal" Width="250px"></asp:Label>
		</td>
	</tr>
</table>
<p>
	<asp:LinkButton id="cmdUpdate" Text="Update" resourcekey="cmdUpdate" runat="server" class="CommandButton" BorderStyle="none" />
</p>
