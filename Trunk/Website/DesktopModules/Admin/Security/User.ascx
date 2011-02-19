<%@ Control language="vb" CodeFile="User.ascx.vb" AutoEventWireup="false" Explicit="True" Inherits="DotNetNuke.Modules.Admin.Users.User" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke" Namespace="DotNetNuke.UI.WebControls"%>
<%@ Register TagPrefix="dnn" TagName="SectionHead" Src="~/controls/SectionHeadControl.ascx" %>

<dnn:propertyeditorcontrol id="UserEditor" runat="Server"
	enableClientValidation = "true"
	sortmode="SortOrderAttribute" 
	labelstyle-cssclass="SubHead" 
	helpstyle-cssclass="Help" 
	editcontrolstyle-cssclass="NormalTextBox" 
	labelwidth="175px" 
	editcontrolwidth="200px" 
	width="375px" 
	editmode="Edit" 
	errorstyle-cssclass="NormalRed"/>
<asp:panel id="pnlAddUser" runat="server" visible="False">
	<table id="tblAddUser" runat="server" width="350px">
		<tr height="25">
			<td style="width:175px" class="SubHead"><dnn:label id="plAuthorize" runat="server" controlname="chkAuthorize" /></td>
			<td style="width:175px" class="normalTextBox"><asp:checkbox id="chkAuthorize" runat="server" checked="True" /></td>
		</tr>
		<tr height="25">
			<td style="width:175px" class="SubHead"><dnn:label id="plNotify" runat="server" controlname="chkNotify" /></td>
			<td style="width:175px" class="normalTextBox"><asp:checkbox id="chkNotify" runat="server" checked="True" /></td>
		</tr>
	</table>
	<br/>
	<table id="tblPassword" runat="server" cellspacing="0" cellpadding="0" width="350px" summary="Password Management" border="0">
		<tr><td colspan="2" valign="bottom"><asp:label id="lblPasswordHelp" cssclass="SubHead" runat="server" /></td></tr>
		<tr><td colspan="2" height="10"></td></tr>
		<tr id="trRandom" runat="server">
			<td style="width:175px" class="SubHead"><dnn:label id="plRandom" runat="server" controlname="chkRandom" /></td>
			<td style="width:175px" class="normalTextBox"><asp:checkbox id="chkRandom" runat="server" checked="True" /></td>
		</tr>
		<tr height="25">
			<td class="SubHead" style="width:175px"><dnn:label id="plPassword" runat="server" controlname="txtPassword" text="Password:"></dnn:label></td>
			<td style="width:175px">
			    <asp:textbox id="txtPassword" runat="server" cssclass="NormalTextBox" textmode="Password" size="12" maxlength="20" />
			    <asp:Image ImageUrl="~/images/required.gif" BorderStyle="None" runat="server"/>
			</td>
		</tr>
		<tr height="25">
			<td class="SubHead" style="width:175px" valign="top"><dnn:label id="plConfirm" runat="server" controlname="txtConfirm" text="Confirm Password:"></dnn:label></td>
			<td style="width:175px">
			    <asp:textbox id="txtConfirm" runat="server" cssclass="NormalTextBox" textmode="Password" size="12" maxlength="20" />
			    <asp:Image  ImageUrl="~/images/required.gif" BorderStyle="None" runat="server"/>
			</td>
		</tr>
		<tr id="trQuestion" runat="server" height="25" visible="false">
			<td class="SubHead" style="width:175px"><dnn:label id="plQuestion" runat="server" controlname="lblQuestion" text="Password Question:"></dnn:label></td>
			<td style="width:175px">
			    <asp:textbox id="txtQuestion" runat="server" cssclass="NormalTextBox" size="25" maxlength="256" />
			    <asp:Image ImageUrl="~/images/required.gif" BorderStyle="None" runat="server"/>
			</td>
		</tr>
		<tr id="trAnswer" runat="server" height="25" visible="false">
			<td class="SubHead" style="width:175px"><dnn:label id="plAnswer" runat="server" controlname="txtAnswer" text="Password Answer:"></dnn:label></td>
			<td style="width:175px">
			    <asp:textbox id="txtAnswer" runat="server" cssclass="NormalTextBox" size="25" maxlength="128" />
			    <asp:Image ImageUrl="~/images/required.gif" BorderStyle="None" runat="server"/>
			</td>
		</tr>
		<tr>
		    <td colspan="2">
			    <asp:CustomValidator ID="valPassword" runat="Server" CssClass="NormalRed" />
		    </td>
		</tr>
	</table>
</asp:panel>
<asp:panel id="pnlUpdate" runat="server" align="center">
	<dnn:commandbutton id="cmdDelete" runat="server" 
		imageurl="~/images/action_delete.gif" 
		causesvalidation="False" />
	&nbsp;&nbsp;&nbsp;
	<dnn:commandbutton id="cmdRemove" runat="server" 
		resourcekey="cmdRemove" imageurl="~/images/delete.gif" 
		causesvalidation="False" />
	&nbsp;&nbsp;&nbsp;
	<dnn:commandbutton id="cmdRestore" runat="server" 
		resourcekey="cmdRestore" imageurl="~/images/restore.gif" 
		causesvalidation="False" />
	&nbsp;&nbsp;&nbsp;
	<dnn:commandbutton id="cmdUpdate" runat="server" 
		resourcekey="cmdUpdate" imageurl="~/images/save.gif" 
		causesvalidation="True" />
</asp:panel>
