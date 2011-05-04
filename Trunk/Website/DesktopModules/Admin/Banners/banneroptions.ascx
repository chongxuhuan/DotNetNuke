<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register Assembly="DotNetNuke.WebControls" Namespace="DotNetNuke.UI.WebControls" TagPrefix="DNN" %>
<%@ Control Language="C#" AutoEventWireup="false" Inherits="DotNetNuke.Modules.Admin.Vendors.BannerOptions" CodeFile="BannerOptions.ascx.cs" %>
<style type="text/css">
.GroupSuggestMenu {
    width: 250px;
}
</style>
<table cellSpacing="2" cellPadding="0" width="560" summary="Banner Options Design Table">
	<tr valign="bottom">
		<td class="SubHead" width="125" vAlign="top"><dnn:label id="plSource" runat="server" controlname="optSource" suffix=":"></dnn:label></td>
		<td vAlign="top">
			<asp:RadioButtonList id="optSource" runat="server" CssClass="NormalBold" RepeatDirection="Horizontal">
				<asp:ListItem Value="G" resourcekey="Host">Host</asp:ListItem>
				<asp:ListItem Value="L" resourcekey="Site">Site</asp:ListItem>
			</asp:RadioButtonList>
		</td>
	</tr>
	<tr valign="bottom">
		<td class="SubHead" width="125" vAlign="top"><dnn:label id="plType" runat="server" controlname="cboType" suffix=":"></dnn:label></td>
		<td vAlign="top">
			<asp:DropDownList ID="cboType" Runat="server" CssClass="NormalTextBox" Width="250px" DataTextField="BannerTypeName"
				DataValueField="BannerTypeId"></asp:DropDownList>
		</td>
	</tr>
	<tr valign="bottom">
		<td class="SubHead" width="125" vAlign="top"><dnn:label id="plGroup" runat="server" controlname="DNNTxtBannerGroup" suffix=":"></dnn:label></td>
		<td vAlign="top"><dnn:DNNTextSuggest id="DNNTxtBannerGroup" runat="server" Columns="30" CssClass="NormalTextBox"
                LookupDelay="500" MaxLength="100" Width="250px" TextSuggestCssClass="SuggestTextMenu GroupSuggestMenu" DefaultNodeCssClassOver="SuggestNodeOver">
            </dnn:DNNTextSuggest></td>
	</tr>
	<tr valign="bottom">
		<td class="SubHead" width="125" vAlign="top"><dnn:label id="plCount" runat="server" controlname="txtCount" suffix=":"></dnn:label></td>
		<td vAlign="top">
			<asp:TextBox id="txtCount" Runat="server" CssClass="NormalTextBox" Columns="30" Width="250px"></asp:TextBox>
			<asp:RegularExpressionValidator id="valCount" ControlToValidate="txtCount" ValidationExpression="^[0-9]*$" Display="Dynamic"
				resourcekey="valCount.ErrorMessage" runat="server" CssClass="NormalRed" />
		</td>
	</tr>
	<tr valign="bottom">
		<td class="SubHead" width="125" vAlign="top"><dnn:label id="plOrientation" runat="server" controlname="optOrientation" suffix=":"></dnn:label></td>
		<td vAlign="top">
			<asp:RadioButtonList id="optOrientation" runat="server" CssClass="NormalBold" RepeatDirection="Horizontal">
				<asp:ListItem Value="V" resourcekey="Vertical">Vertical</asp:ListItem>
				<asp:ListItem Value="H" resourcekey="Horizontal">Horizontal</asp:ListItem>
			</asp:RadioButtonList>
		</td>
	</tr>
	<tr valign="bottom">
		<td class="SubHead" width="125" vAlign="top"><dnn:label id="plBorder" runat="server" controlname="txtBorder" suffix=":"></dnn:label></td>
		<td vAlign="top">
			<asp:TextBox id="txtBorder" Runat="server" CssClass="NormalTextBox" Columns="30" Width="250px"></asp:TextBox>
			<asp:RegularExpressionValidator id="valBorder" ControlToValidate="txtBorder" ValidationExpression="^[0-9]*$" Display="Dynamic"
				resourcekey="valBorder.ErrorMessage" runat="server" CssClass="NormalRed" />
		</td>
	</tr>
	<tr valign="bottom">
		<td class="SubHead" width="125" vAlign="top"><dnn:label id="plBorderColor" runat="server" controlname="txtBorderColor" suffix=":"></dnn:label></td>
		<td vAlign="top">
			<asp:TextBox id="txtBorderColor" Runat="server" CssClass="NormalTextBox" Columns="30" Width="250px"></asp:TextBox>
		</td>
	</tr>
	<TR>
		<TD class="SubHead" vAlign="top" width="125">
			<dnn:label id="plPadding" suffix=":" controlname="txtPadding" runat="server"></dnn:label></TD>
		<TD vAlign="top">
			<asp:TextBox id="txtPadding" CssClass="NormalTextBox" Width="250px" Runat="server" Columns="30">4</asp:TextBox>
			<asp:CompareValidator id="valPadding" runat="server" CssClass="NormalRed" resourcekey="valPadding.ErrorMessage"
				Display="Dynamic" ControlToValidate="txtPadding" Operator="DataTypeCheck" Type="Integer"></asp:CompareValidator></TD>
	</TR>
	<tr valign="bottom">
		<td class="SubHead" width="125" vAlign="top"><dnn:label id="plRowHeight" runat="server" controlname="txtRowHeight" suffix=":"></dnn:label></td>
		<td vAlign="top">
			<asp:TextBox id="txtRowHeight" Runat="server" CssClass="NormalTextBox" Columns="30" Width="250px"></asp:TextBox>
			<asp:RegularExpressionValidator id="valRowHeight" ControlToValidate="txtRowHeight" ValidationExpression="^[0-9]*$"
				Display="Dynamic" resourcekey="valRowHeight.ErrorMessage" runat="server" CssClass="NormalRed" />
		</td>
	</tr>
	<tr valign="bottom">
		<td class="SubHead" width="125" vAlign="top"><dnn:label id="plColWidth" runat="server" controlname="txtColWidth" suffix=":"></dnn:label></td>
		<td vAlign="top">
			<asp:TextBox id="txtColWidth" Runat="server" CssClass="NormalTextBox" Columns="30" Width="250px"></asp:TextBox>
			<asp:RegularExpressionValidator id="valColWidth" ControlToValidate="txtColWidth" ValidationExpression="^[0-9]*$"
				Display="Dynamic" resourcekey="valColWidth.ErrorMessage" runat="server" CssClass="NormalRed" />
		</td>
	</tr>
	<tr valign="bottom">
		<td class="SubHead" width="125" vAlign="top"><dnn:label id="plBannerClickThroughURL" runat="server" controlname="txtBannerClickThroughURL" suffix=":"></dnn:label></td>
		<td vAlign="top">
			<asp:TextBox id="txtBannerClickThroughURL" Runat="server" CssClass="NormalTextBox" Columns="30" Width="250px"></asp:TextBox>
			<asp:RegularExpressionValidator id="valBannerClickThroughURL" ControlToValidate="txtBannerClickThroughURL" ValidationExpression="(http|https)://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?"
				Display="Dynamic" resourcekey="valBannerClickThroughURL.ErrorMessage" runat="server" CssClass="NormalRed" />
		</td>
	</tr>
</table>
<p>
	<asp:LinkButton id="cmdUpdate" Text="Update" resourcekey="cmdUpdate" runat="server" class="CommandButton"
		BorderStyle="none" />
	&nbsp;
	<asp:LinkButton id="cmdCancel" Text="Cancel" resourcekey="cmdCancel" CausesValidation="False" runat="server"
		class="CommandButton" BorderStyle="none" />
</p>
