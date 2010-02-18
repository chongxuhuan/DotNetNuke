<%@ Control language="vb" AutoEventWireup="false" Explicit="True" Inherits="DotNetNuke.Modules.Admin.ModuleDefinitions.CreateModuleDefinition" CodeFile="EditModuleDefinition.ascx.vb" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke" Namespace="DotNetNuke.UI.WebControls" %>
<br>
<table width="750" cellspacing="4" cellpadding="4" border="0" summary="Create Module Design Table">
    <tr>
        <td class="SubHead" style="width:175px" valign="top"><dnn:label id="plCreate" controlname="optCreate" runat="server" /></td>
        <td valign="top" style="width:575px">
            <asp:DropDownList ID="cboCreate" runat="server" CssClass="NormalTextBox" Width="300" AutoPostBack="True">
                <asp:ListItem Value="New" resourcekey="New"/>
                <asp:ListItem Value="Control" resourcekey="Control"/>
                <asp:ListItem Value="Manifest" resourcekey="Manifest"/>
            </asp:DropDownList>
        </td>
    </tr>
    <tr id="rowOwner1" runat="server" visible="false">
		<td class="SubHead" style="width:175px"><dnn:label id="plOwner1" runat="server" controlname="cboOwner"/></td>
		<td style="width:575px">
            <asp:DropDownList ID="cboOwner" runat="server" CssClass="NormalTextBox" Width="300" AutoPostBack="True" />&nbsp;&nbsp;
            <dnn:commandbutton cssclass="CommandButton" id="cmdAddOwner" resourcekey="cmdAdd" runat="server" Visible= "false" ImageUrl="~/images/add.gif"/>
        </td>
	</tr>
	<tr id="rowOwner2" runat="server" visible="false">
		<td class="SubHead" style="width:175px"><dnn:label id="plOwner2" runat="server" controlname="txtOwner"/></td>
		<td style="width:575px">
            <asp:TextBox ID="txtOwner" Runat="server" CssClass="NormalTextBox" MaxLength="255" Width="300"/>&nbsp;&nbsp;
            <dnn:commandbutton cssclass="CommandButton" id="cmdSaveOwner" resourcekey="cmdSave" runat="server" ImageUrl="~/images/save.gif"/>
            <dnn:commandbutton cssclass="CommandButton" id="cmdCancelOwner" resourcekey="cmdCancel" runat="server" ImageUrl="~/images/lt.gif"/>
        </td>
	</tr>
	<tr id="rowModule1" runat="server" visible="false">
		<td class="SubHead" style="width:175px"><dnn:label id="plModule1" runat="server" controlname="cboModule" /></td>
		<td style="width:575px">
            <asp:DropDownList ID="cboModule" runat="server" CssClass="NormalTextBox" Width="300" AutoPostBack="true" />&nbsp;&nbsp;
            <dnn:commandbutton cssclass="CommandButton" id="cmdAddModule" resourcekey="cmdAdd" runat="server" Visible="false" ImageUrl="~/images/add.gif"/>
        </td>
	</tr>
	<tr id="rowModule2" runat="server" visible="false">
		<td class="SubHead" style="width:175px"><dnn:label id="plModule2" runat="server" controlname="txtModule" /></td>
		<td style="width:575px">
            <asp:TextBox ID="txtModule" Runat="server" CssClass="NormalTextBox" MaxLength="255" Width="300" ValidationGroup="first"/>&nbsp;&nbsp; 
            <dnn:commandbutton cssclass="CommandButton" id="cmdSaveModule" resourcekey="cmdSave" runat="server" ImageUrl="~/images/save.gif"/>
            <dnn:commandbutton cssclass="CommandButton" id="cmdCancelModule" resourcekey="cmdCancel" runat="server" ImageUrl="~/images/lt.gif"/>
        </td>
	</tr>
	<tr id="rowFile1" runat="server" visible="false">
        <td class="SubHead" style="width:175px" valign="top"><dnn:label id="plFile1" controlname="cboFile" runat="server" /></td>
        <td valign="top" style="width:575px">
            <asp:dropdownlist id="cboFile" runat="server" width="300" cssclass="NormalTextBox" />
       </td>
    </tr>
	<tr id="rowFile2" runat="server" visible="false">
        <td class="SubHead" style="width:175px" valign="top"><dnn:label id="plLang" controlname="rblLanguage" runat="server" /></td>
        <td valign="top" style="width:575px">
            <asp:RadioButtonList ID="rblLanguage" runat="server" RepeatDirection="Horizontal">
                <asp:ListItem Value="C#" resourcekey="CSharp" />
                <asp:ListItem Value="VB" resourcekey="VisualBasic" />
            </asp:RadioButtonList>
       </td>
    </tr>
    <tr id="rowLang" runat="server" visible="false">
        <td class="SubHead" style="width:175px" valign="top"><dnn:label id="plFile2" controlname="txtFile" runat="server" /></td>
        <td valign="top" style="width:575px">
            <asp:TextBox ID="txtFile" Runat="server" CssClass="NormalTextBox" MaxLength="255" Width="300"/>&nbsp;&nbsp;
       </td>
    </tr>
    <tr id="rowName" runat="server" visible="false">
        <td class="SubHead" style="width:175px" valign="top"><dnn:label id="plName" controlname="txtName" runat="server" /></td>
        <td valign="top" style="width:575px"><asp:TextBox ID="txtName" runat="server" width="300" CssClass="NormalTextBox"></asp:TextBox></td>
    </tr>
    <tr id="rowDescription" runat="server" visible="false">
        <td class="SubHead" style="width:175px" valign="top"><dnn:label id="plDescription" controlname="txtDescription" runat="server" /></td>
        <td valign="top" style="width:575px"><asp:TextBox ID="txtDescription" runat="server" width="300" CssClass="NormalTextBox" TextMode="MultiLine" Rows="5"></asp:TextBox></td>
    </tr>
    <tr id="rowSource" runat="server" visible="false">
        <td class="SubHead" style="width:175px" valign="top"><dnn:label id="plSource" controlname="txtSource" runat="server" /></td>
        <td valign="top" style="width:575px"><asp:TextBox ID="txtSource" runat="server" width="300" CssClass="NormalTextBox" TextMode="MultiLine" Rows="5"></asp:TextBox></td>
    </tr>
    <tr id="rowAddPage" runat="server" visible="false">
        <td class="SubHead" style="width:175px" valign="top"><dnn:label id="plAddPage" controlname="chkAddPage" runat="server" /></td>
        <td valign="top" style="width:575px"><asp:CheckBox ID="chkAddPage" runat="server" /></td>
    </tr>
</table>
<br />
<p>
	<dnn:commandbutton id="cmdCreate" runat="server" CssClass="CommandButton" ImageUrl="~/images/save.gif" ResourceKey="cmdCreate" causesvalidation="False" Visible="false" />&nbsp;&nbsp;
	<dnn:commandbutton id="cmdCancel" runat="server" CssClass="CommandButton" ImageUrl="~/images/lt.gif" ResourceKey="cmdCancel" causesvalidation="False" />
</p>
<table class="Settings" cellspacing="2" cellpadding="2" summary="Packages Install Design Table">
    <tr><td><asp:PlaceHolder ID="phInstallLogs" runat="server" /></td></tr>
</table>
