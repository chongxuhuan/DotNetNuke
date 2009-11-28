<%@ Control language="vb" AutoEventWireup="false" Explicit="True" Inherits="DotNetNuke.Modules.Admin.ModuleDefinitions.EditModuleControl" CodeFile="EditModuleControl.ascx.vb" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke" Namespace="DotNetNuke.UI.WebControls" %>
<table cellspacing="0" cellpadding="4" border="0" summary="Module Controls Design Table">
	<tr>
		<td class="SubHead" style="width:200px"><dnn:label id="plModule" text="Module:" controlname="lblModule" runat="server" /></td>
		<td><asp:Label id="lblModule" cssclass="NormalTextBox" runat="server" /></td>
	</tr>
	<tr>
		<td class="SubHead" style="width:200px"><dnn:label id="plDefinition" text="Definition:" controlname="lblDefinition" runat="server" /></td>
		<td><asp:Label id="lblDefinition" cssclass="NormalTextBox"	runat="server" /></td>
	</tr>
	<tr>
		<td class="SubHead" style="width:200px"><dnn:label id="plKey" text="Key:" controlname="txtKey" runat="server" /></td>
		<td><asp:textbox id="txtKey" cssclass="NormalTextBox" width="390" columns="30" maxlength="50" runat="server" /></td>
	</tr>
	<tr>
		<td class="SubHead" style="width:200px"><dnn:label id="plTitle" text="Title:" controlname="txtTitle" runat="server" /></td>
		<td><asp:textbox id="txtTitle" cssclass="NormalTextBox" width="390" columns="30" maxlength="50" runat="server" /></td>
	</tr>
	<tr>
		<td class="SubHead" style="width:200px" valign="top"><dnn:label id="plSource" text="Source:" controlname="cboSource" runat="server" /></td>
		<td>
		    <asp:dropdownlist id="cboSource" runat="server" width="390" cssclass="NormalTextBox" autopostback="True" />
		    <br />
	        <asp:textbox id="txtSource" cssclass="NormalTextBox" width="390" columns="30" maxlength="100" runat="server" />
	    </td>
	</tr>
	<tr>
		<td class="SubHead" style="width:200px"><dnn:label id="plType" text="Type:" controlname="cboType" runat="server" /></td>
		<td>
			<asp:dropdownlist id="cboType" runat="server" width="390" cssclass="NormalTextBox">
				<asp:listitem resourcekey="Skin" value="-2" />
				<asp:listitem resourcekey="Anonymous" value="-1" />
				<asp:listitem resourcekey="View" value="0" />
				<asp:listitem resourcekey="Edit" value="1" />
				<asp:listitem resourcekey="Admin" value="2" />
				<asp:listitem resourcekey="Host" value="3" />
			</asp:dropdownlist>
		</td>
	</tr>
	<tr>
		<td class="SubHead" style="width:200px"><dnn:label id="plViewOrder" text="View Order:" controlname="txtViewOrder" runat="server" /></td>
		<td><asp:textbox id="txtViewOrder" cssclass="NormalTextBox" width="390" columns="30" maxlength="2" runat="server" /></td>
	</tr>
	<tr>
		<td class="SubHead" style="width:200px"><dnn:label id="plIcon" text="Icon:" controlname="cboIcon" runat="server" /></td>
		<td height="23"><asp:dropdownlist id="cboIcon" runat="server" width="390" cssclass="NormalTextBox" datavaluefield="Value" datatextfield="Text" /></td>
	</tr>
	<tr>
		<td class="SubHead" style="width:200px"><dnn:label id="plHelpURL" text="Help URL:" controlname="txtHelpURL" runat="server" /></td>
		<td><asp:textbox id="txtHelpURL" runat="server" maxlength="200" columns="30" width="390" cssclass="NormalTextBox" /></td>
	</tr>
	<tr>
		<td class="SubHead" style="width:200px"><dnn:label id="plSupportsPartialRendering" text="Supports Partial Rendering?" controlname="chkSupportsPartialRendering" runat="server" /></td>
		<td><asp:checkbox id="chkSupportsPartialRendering" runat="server" cssclass="NormalTextBox"/></td>
	</tr>
</table>
<p>
	<dnn:commandbutton id="cmdUpdate" resourcekey="cmdUpdate" runat="server" class="CommandButton" borderstyle="none" ImageUrl="~/images/save.gif" />&nbsp;
	<dnn:commandbutton id="cmdDelete" resourcekey="cmdDelete" causesvalidation="False" runat="server" class="CommandButton" borderstyle="none" ImageUrl="~/images/delete.gif" />&nbsp;
	<dnn:commandbutton id="cmdCancel" resourcekey="cmdCancel" causesvalidation="False" runat="server" class="CommandButton" borderstyle="none" ImageUrl="~/images/lt.gif" />
</p>
