<%@ Control language="vb" AutoEventWireup="false" Explicit="True" Inherits="DotNetNuke.UI.UserControls.Address" %>
<%@ Register TagPrefix="wc" Namespace="DotNetNuke.UI.WebControls" Assembly="CountryListBox" %>
<%@ Register TagPrefix="dnn" TagName="label" Src="~/controls/labelControl.ascx" %>
<table cellSpacing="0" cellPadding="1" border="0" summary="Address Design Table">
	<tr id="rowStreet" runat="server">
		<td class="SubHead" width="120"><dnn:label id="plStreet" runat="server" controlname="txtStreet" text="Street:"></dnn:label></td>
		<td valign="top" nowrap>
			<asp:textbox id="txtStreet" runat="server" MaxLength="50" cssclass="NormalTextBox" Width="200px"></asp:textbox>
			<asp:checkbox ID="chkStreet" Runat="server" CssClass="NormalTextBox" Visible="False" AutoPostBack="True"></asp:checkbox>
			<asp:label ID="lblStreetRequired" Runat="server" CssClass="NormalBold"></asp:label><label class="SubHead" style="display:none;" for="<%=chkStreet.ClientID%>">Street Is Required.</label>
			<asp:requiredfieldvalidator id="valStreet" runat="server" CssClass="NormalRed" ControlToValidate="txtStreet" ErrorMessage="<br>Street Is Required." Display="Dynamic"></asp:requiredfieldvalidator>
		</td>
	</tr>
	<tr id="rowUnit" runat="server">
		<td class="SubHead" width="120"><dnn:label id="plUnit" runat="server" controlname="txtUnit" text="Unit:"></dnn:label></td>
		<td valign="top" nowrap>
			<asp:textbox id="txtUnit" runat="server" MaxLength="50" cssclass="NormalTextBox" Width="200px"></asp:textbox>
		</td>
	</tr>
	<tr id="rowCity" runat="server">
		<td class="SubHead" width="120"><dnn:label id="plCity" runat="server" controlname="txtCity" text="City:"></dnn:label></td>
		<td valign="top" nowrap>
			<asp:textbox id="txtCity" runat="server" MaxLength="50" cssclass="NormalTextBox" Width="200px"></asp:textbox>
			<asp:checkbox ID="chkCity" Runat="server" CssClass="NormalTextBox" Visible="False" AutoPostBack="True"></asp:checkbox>
			<asp:label ID="lblCityRequired" Runat="server" CssClass="NormalBold"></asp:label><label class="SubHead" style="display:none;" for="<%=chkCity.ClientID%>">City Is Required.</label>
			<asp:requiredfieldvalidator id="valCity" runat="server" CssClass="NormalRed" ControlToValidate="txtCity" ErrorMessage="<br>City Is Required." Display="Dynamic"></asp:requiredfieldvalidator>
		</td>
	</tr>
	<tr id="rowCountry" runat="server">
		<td class="SubHead" width="120"><dnn:label id="plCountry" runat="server" controlname="cboCountry" text="Country:"></dnn:label></td>
		<td valign="top" nowrap>
			<wc:CountryListBox TestIP="" LocalhostCountryCode="US" id="cboCountry" CssClass="NormalTextBox" Width="200px"
				DataValueField="Value" DataTextField="Text" AutoPostBack="True" runat="server"></wc:CountryListBox>
			<asp:checkbox ID="chkCountry" Runat="server" CssClass="NormalTextBox" Visible="False" AutoPostBack="True"></asp:checkbox>
			<asp:label ID="lblCountryRequired" Runat="server" CssClass="NormalBold"></asp:label><label  class="SubHead" style="display:none;" for="<%=chkCountry.ClientID%>">Country is Required.</label>
			<asp:requiredfieldvalidator id="valCountry" runat="server" CssClass="NormalRed" ControlToValidate="cboCountry" ErrorMessage="<br>Country Is Required." Display="Dynamic"></asp:requiredfieldvalidator>
		</td>
	</tr>
	<tr id="rowRegion" runat="server">
		<td class="SubHead" width="120"><dnn:label id="plRegion" runat="server" controlname="cboRegion" text="Region:"></dnn:label></td>
		<td valign="top" nowrap>
			<asp:DropDownList id="cboRegion" runat="server" cssclass="NormalTextBox" Width="200px" DataValueField="Value"
				DataTextField="Text" Visible="False"></asp:DropDownList>
			<asp:textbox id="txtRegion" runat="server" MaxLength="50" cssclass="NormalTextBox" Width="200px"></asp:textbox>
			<asp:checkbox ID="chkRegion" Runat="server" CssClass="NormalTextBox" Visible="False" AutoPostBack="True"></asp:checkbox>
			<asp:label ID="lblRegionRequired" Runat="server" CssClass="NormalBold"></asp:label><label class="SubHead" style="display:none;" for="<%=chkRegion.ClientID%>">Region Is Required.</label>
			<asp:requiredfieldvalidator id="valRegion1" runat="server" CssClass="NormalRed" ControlToValidate="cboRegion" ErrorMessage="<br>Region Is Required." Display="Dynamic"></asp:requiredfieldvalidator>
			<asp:requiredfieldvalidator id="valRegion2" runat="server" CssClass="NormalRed" ControlToValidate="txtRegion" ErrorMessage="<br>Region Is Required." Display="Dynamic"></asp:requiredfieldvalidator>
		</td>
	</tr>
	<tr id="rowPostal" runat="server">
		<td class="SubHead" width="120"><dnn:label id="plPostal" runat="server" controlname="txtPostal" text="Postal Code:"></dnn:label></td>
		<td valign="top" nowrap>
			<asp:textbox id="txtPostal" runat="server" MaxLength="50" cssclass="NormalTextBox" Width="200px"></asp:textbox>
			<asp:checkbox ID="chkPostal" Runat="server" CssClass="NormalTextBox" Visible="False" AutoPostBack="True"></asp:checkbox>
			<asp:label ID="lblPostalRequired" Runat="server" CssClass="NormalBold"></asp:label><label class="SubHead" style="display:none;" for="<%=chkPostal.ClientID%>">Postal Code Is Required.</label>
			<asp:requiredfieldvalidator id="valPostal" runat="server" CssClass="NormalRed" ControlToValidate="txtPostal" ErrorMessage="<br>Postal Code Is Required." Display="Dynamic"></asp:requiredfieldvalidator>
		</td>
	</tr>
	<tr id="rowTelephone" runat="server">
		<td class="SubHead" width="120"><dnn:label id="plTelephone" runat="server" controlname="txtTelephone" text="Telephone:"></dnn:label></td>
		<td valign="top" nowrap>
			<asp:textbox id="txtTelephone" runat="server" MaxLength="50" cssclass="NormalTextBox" Width="200px"></asp:textbox>
			<asp:checkbox ID="chkTelephone" Runat="server" CssClass="NormalTextBox" Visible="False" AutoPostBack="True"></asp:checkbox>
			<asp:label ID="lblTelephoneRequired" Runat="server" CssClass="NormalBold"></asp:label><label class="SubHead" style="display:none;" for="<%=chkTelephone.ClientID%>">Telephone Number Is Required.</label>
			<asp:requiredfieldvalidator id="valTelephone" runat="server" CssClass="NormalRed" ControlToValidate="txtTelephone" ErrorMessage="<br>Telephone Number Is Required." Display="Dynamic"></asp:requiredfieldvalidator>
		</td>
	</tr>
	<tr id="rowCell" runat="server">
		<td class="SubHead" width="120"><dnn:label id="plCell" runat="server" controlname="txtCell" text="Cell:"></dnn:label></td>
		<td valign="top" nowrap>
			<asp:textbox id="txtCell" runat="server" MaxLength="50" cssclass="NormalTextBox" Width="200px"></asp:textbox>
			<asp:checkbox ID="chkCell" Runat="server" CssClass="NormalTextBox" Visible="False" AutoPostBack="True"></asp:checkbox>
			<asp:label ID="lblCellRequired" Runat="server" CssClass="NormalBold"></asp:label><label class="SubHead" style="DISPLAY:none" for="<%=chkCell.ClientID%>">Cell Phone Number Is Required.</label>
			<asp:requiredfieldvalidator id="valCell" runat="server" CssClass="NormalRed" ControlToValidate="txtCell" ErrorMessage="<br>Cell Phone Number Is Required." Display="Dynamic"></asp:requiredfieldvalidator>
		</td>
	</tr>
	<tr id="rowFax" runat="server">
		<td class="SubHead" width="120"><dnn:label id="plFax" runat="server" controlname="txtFax" text="Fax:"></dnn:label></td>
		<td valign="top" nowrap>
			<asp:textbox id="txtFax" runat="server" MaxLength="50" cssclass="NormalTextBox" Width="200px"></asp:textbox>
			<asp:checkbox ID="chkFax" Runat="server" CssClass="NormalTextBox" Visible="False" AutoPostBack="True"></asp:checkbox>
			<asp:label ID="lblFaxRequired" Runat="server" CssClass="NormalBold"></asp:label><label class="SubHead" style="DISPLAY:none" for="<%=chkFax.ClientID%>">Fax Number Is Required.</label>
			<asp:requiredfieldvalidator id="valFax" runat="server" CssClass="NormalRed" ControlToValidate="txtFax" ErrorMessage="<br>Fax Number Is Required." Display="Dynamic"></asp:requiredfieldvalidator>
		</td>
	</tr>
</table>
