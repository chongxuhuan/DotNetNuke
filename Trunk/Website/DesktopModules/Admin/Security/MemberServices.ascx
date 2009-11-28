<%@ Control language="vb" CodeFile="MemberServices.ascx.vb" AutoEventWireup="false" Explicit="True" Inherits="DotNetNuke.Modules.Admin.Security.MemberServices" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.UI.WebControls" Assembly="DotNetNuke" %>
<table id="tblServices" cellspacing="0" cellpadding="4" width="100%" summary="Register Design Table" border="0" runat="server">
    <tr>
        <td class="normal"><asp:Label ID="lblServicesHelp" runat="server" resourcekey="ServicesHelp" /></td>
    </tr>
	<tr id="ServicesRow" runat="Server">
		<td align="left">
			<asp:datagrid id="grdServices" runat="server" GridLines="None" enableviewstate="true" 
				autogeneratecolumns="false" cellpadding="4" border="0" summary="Register Design Table">
				<HeaderStyle CssClass = "NormalBold" />
				<ItemStyle CssClass = "Normal" />
				<columns>
					<asp:templatecolumn>
						<itemtemplate>
							<asp:LinkButton 
							    text='<%# ServiceText(DataBinder.Eval(Container.DataItem,"Subscribed"), DataBinder.Eval(Container.DataItem, "ExpiryDate")) %>'
							    CommandName='<%# ServiceText(DataBinder.Eval(Container.DataItem,"Subscribed"), DataBinder.Eval(Container.DataItem, "ExpiryDate")) %>'
							    CommandArgument = '<%# DataBinder.Eval(Container.DataItem,"RoleID") %>'
							    cssclass="CommandButton" 
							    runat="server" 
							    visible =  '<%# ShowSubscribe(DataBinder.Eval(Container.DataItem,"RoleID")) %>'
							    id="lnkSubscribe"/>
						</itemtemplate>
					</asp:templatecolumn>
					<asp:templatecolumn>
						<itemtemplate>
							<asp:LinkButton 
							    CommandName="UseTrial"
							    CommandArgument = '<%# DataBinder.Eval(Container.DataItem,"RoleID") %>'
							    cssclass="CommandButton" 
							    runat="server"
							    resourcekey = "UseTrial"
							    visible =  '<%# ShowTrial(DataBinder.Eval(Container.DataItem,"RoleID")) %>'
							    id="lnkTrial"/>
						</itemtemplate>
					</asp:templatecolumn>
					<asp:boundcolumn headertext="Name" datafield="RoleName"/>
					<asp:boundcolumn headertext="Description" datafield="Description"/>
					<asp:templatecolumn headertext="Fee">
						<itemtemplate>
							<asp:label runat="server" text='<%#FormatPrice(DataBinder.Eval(Container.DataItem, "ServiceFee"), DataBinder.Eval(Container.DataItem, "BillingPeriod"), DataBinder.Eval(Container.DataItem, "BillingFrequency")) %>' id="Label2"/>
						</itemtemplate>
					</asp:templatecolumn>
					<asp:templatecolumn headertext="Trial">
						<itemtemplate>
							<asp:label runat="server" text='<%#FormatTrial(DataBinder.Eval(Container.DataItem, "TrialFee"), DataBinder.Eval(Container.DataItem, "TrialPeriod"), DataBinder.Eval(Container.DataItem, "TrialFrequency")) %>' id="Label4"/>
						</itemtemplate>
					</asp:templatecolumn>
					<asp:templatecolumn headertext="ExpiryDate">
						<itemtemplate>
							<asp:label runat="server" text='<%#FormatExpiryDate(DataBinder.Eval(Container.DataItem, "ExpiryDate")) %>' id="Label1"/>
						</itemtemplate>
					</asp:templatecolumn>
				</columns>
			</asp:datagrid>
			<asp:label id="lblServices" runat="server" cssclass="Normal" visible="False"></asp:label>
		</td>
	</tr>
    <tr>
        <td class="normal"><asp:Label ID="lblRSVPHelp" runat="server" resourcekey="RSVPHelp" /></td>
    </tr>
	<tr>
		<td align="left">
			<table cellspacing="0" cellpadding="4" border="0" width="325px">
				<tr>
					<td class="SubHead" width="125px"><dnn:label id="plRSVPCode" runat="server" controlname="txtRSVPCode"></dnn:label></td>
		    			<td width="200px">
		    			    <asp:textbox id="txtRSVPCode" runat="server" width="100" cssclass="NormalTextBox" maxlength="50" columns="30"/>&nbsp;
		    			    <asp:linkbutton class="CommandButton" id="cmdRSVP" runat="server" text="Subscribe" resourcekey="cmdRSVP" />
		    			</td>
				</tr>
				<tr>
					<td colspan="2"><asp:Label ID="lblRSVP" runat="server" CssClass="NormalRed"></asp:Label></td>
				</tr>
			</table>
		</td>
    </tr>
</table>
