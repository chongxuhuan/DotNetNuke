<%@ Register TagPrefix="dnn" TagName="SectionHead" Src="~/controls/SectionHeadControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke" Namespace="DotNetNuke.UI.WebControls"%>
<%@ Control Language="C#" AutoEventWireup="false" Inherits="DotNetNuke.Modules.Admin.Users.UserSettings" CodeFile="UserSettings.ascx.cs" %>

<table>
	<tr>
		<td>
			<dnn:sectionhead id="dshProvider" cssclass="Head" runat="server" 
				text="Provider Settings" section="tblProvider" resourcekey="ProviderSettings" 
				isexpanded="True" includerule="True" />
			<table id="tblProvider" runat="server">
				<tr>
					<td class="Normal"><asp:label id="lblprovider" runat="server" resourcekey="ProviderSettingsHelp" /></td>
				</tr>
				<tr>
					<td>
						<dnn:propertyeditorcontrol id="ProviderSettings" runat="Server"
                			valuedatafield="PropertyValue" 
                            namedatafield="Name" 
                            helpstyle-cssclass="dnnFormHelpContent dnnClear" 
							SortMode="SortOrderAttribute"/>
					</td>
				</tr>
			</table>
		</td>
	</tr>
	<tr><td height="10"></td></tr>
	<tr>
		<td>
			<dnn:sectionhead id="dshPassword" cssclass="Head" runat="server" 
				text="Password Settings" section="tblPassword" resourcekey="PasswordSettings" 
				isexpanded="True" includerule="True" />
			<table id="tblPassword" runat="server">
				<tr>
					<td class="Normal"><asp:label id="lblPassword" runat="server" resourcekey="PasswordSettingsHelp" /></td>
				</tr>
				<tr>
					<td>
						<dnn:propertyeditorcontrol id="PasswordSettings" runat="Server"
							valuedatafield="PropertyValue" 
                            namedatafield="Name" 
                            helpstyle-cssclass="dnnFormHelpContent dnnClear" 
							sortmode="SortOrderAttribute"/>
					</td>
				</tr>
			</table>
		</td>
	</tr>
	<tr><td height="10"></td></tr>
	<tr>
		<td>
			<dnn:sectionhead id="Sectionhead1" cssclass="Head" runat="server" 
				text="User Accounts Settings" section="tblUserAccounts" resourcekey="UserAccounts.Text" 
				isexpanded="True" includerule="True" />
			<table id="tblUserAccounts" runat="server">
				<tr>
					<td>
						<dnn:settingseditorcontrol id="UserSettingsEditor" runat="Server" 
                            helpstyle-cssclass="dnnFormHelpContent dnnClear" 
							editmode="Edit"/>
					</td>
				</tr>
			</table>
		</td>
	</tr>
</table>
<p>
	<dnn:commandbutton class="CommandButton" id="cmdUpdate" resourcekey="cmdUpdate" runat="server" ImageUrl="~/images/add.gif" />&nbsp;
	<dnn:commandbutton class="CommandButton" id="cmdCancel" resourcekey="cmdCancel" runat="server" ImageUrl="~/images/lt.gif" causesvalidation="False" />
</p>
