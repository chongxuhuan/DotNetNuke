<%@ Control language="vb" CodeFile="EditExtension.ascx.vb" AutoEventWireup="false" Explicit="True" Inherits="DotNetNuke.Modules.Admin.Extensions.EditExtension" %>
<%@ Register TagPrefix="dnn" TagName="SectionHead" Src="~/controls/SectionHeadControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke" Namespace="DotNetNuke.UI.WebControls" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Security.Permissions.Controls" Assembly="DotNetNuke" %>
<%@ Register TagPrefix="dnn" TagName="Audit" Src="~/controls/ModuleAuditControl.ascx" %>

<table cellspacing="0" cellpadding="4" border="0">
    <tr>
        <td style="text-align:left"><asp:Label ID="lblTitle" runat="server" CssClass="Head" /></td>
    </tr>
    <tr><td style="height:15px;"></td></tr>
    <tr>
        <td>
            <dnn:SectionHead ID="dshExtension" CssClass="Head" runat="server" Section="tblExtension" ResourceKey="ExtensionSettings" IncludeRule="True" />
            <table id="tblExtension" runat="server" cellspacing="0" cellpadding="4" broder="0">
                <tr>
                    <td style="width:20px"></td>
                    <td style="width:600px">
                        <asp:PlaceHolder ID="phEditor" runat="server" />
                    </td>
                </tr>
                <tr style="height:15px"><td></td></tr>
            </table>
            <dnn:SectionHead ID="dshPackage" CssClass="Head" runat="server" Section="tblPackage" ResourceKey="PackageSettings" IncludeRule="True" IsExpanded="true" />
            <table id="tblPackage" runat="server" cellspacing="0" cellpadding="4" broder="0">
                <tr>
                    <td style="width:20px"></td>
                    <td colspan="2"><asp:Label ID="lblHelp" runat="server" cssClass="Normal" /></td>
                </tr>
                <tr id="trLanguagePackType" runat="server">
                    <td style="width:20px"></td>
                    <td class="SubHead" style="width:200px"><dnn:Label ID="plPackageType" runat="server" ControlName="rbPackageType /></td>
                    <td class="NormalTextBox" style="width:325px">
                        <asp:RadioButtonList ID="rbPackageType" runat="server" RepeatDirection="Horizontal">
                            <asp:ListItem Value="Core" resourcekey="Core" />
                            <asp:ListItem Value="Package" resourcekey="Package" />
                        </asp:RadioButtonList>
                        <asp:RequiredFieldValidator ID="valPackageType" runat="server" CssClass="NormalRed" Display="Dynamic" ControlToValidate="rbPackageType" ResourceKey="PackageType.Error" />
                    </td>
                </tr>
                <tr>
                    <td style="width:20px"></td>
                    <td colspan="2" style="width:600px">
                        <dnn:propertyeditorcontrol id="ctlExtension" runat="Server"
                            AutoGenerate="false"
                            SortMode="SortOrderAttribute"
                            editcontrolstyle-cssclass="NormalTextBox" 
                            editcontrolwidth="450px" 
                            ErrorStyle-cssclass="NormalRed"
                            helpstyle-cssclass="Help" 
                            labelstyle-cssclass="SubHead" 
                            labelwidth="175px" 
                            width="650px">
                                <Fields>
                                    <dnn:FieldEditorControl ID="fldName" runat="server" DataField="Name" EditControlStyle-Width="250px" Required="true" EditMode="View"/>
                                    <dnn:FieldEditorControl ID="fldPackageType" runat="server" DataField="PackageType" EditControlStyle-Width="150px" EditorTypeName="DotNetNuke.Services.Installer.Packages.WebControls.PackageTypeEditControl" Required="true" EditMode="View"/>
                                    <dnn:FieldEditorControl ID="fldFriendlyName" runat="server" DataField="FriendlyName" EditControlStyle-Width="400px" Required="true" />
                                    <dnn:FieldEditorControl ID="fldDescription" runat="server" DataField="Description" EditorTypeName="DotNetNuke.UI.WebControls.MultiLineTextEditControl" EditControlStyle-Width="400px" EditControlStyle-Height="80px" />
                                    <dnn:FieldEditorControl ID="fldVersion" runat="server" DataField="Version" EditorTypeName="DotNetNuke.UI.WebControls.VersionEditControl"/>
                                    <dnn:FieldEditorControl ID="fldLicense" runat="server" DataField="License" EditorTypeName="DotNetNuke.UI.WebControls.MultiLineTextEditControl" EditControlStyle-Width="400px" EditControlStyle-Height="250px" />
                                    <dnn:FieldEditorControl ID="fldReleaseNotes" runat="server" DataField="ReleaseNotes" EditorTypeName="DotNetNuke.UI.WebControls.MultiLineTextEditControl" EditControlStyle-Width="400px" EditControlStyle-Height="250px" />
                                    <dnn:FieldEditorControl ID="fldOwner" runat="server" DataField="Owner"  EditControlStyle-Width="250px" />
                                    <dnn:FieldEditorControl ID="fldOrganization" runat="server" DataField="Organization"  EditControlStyle-Width="250px" />
                                    <dnn:FieldEditorControl ID="fldUrl" runat="server" DataField="Url"  EditControlStyle-Width="250px" />
                                    <dnn:FieldEditorControl ID="fldEmail" runat="server" DataField="Email"  EditControlStyle-Width="250px" />
                                </Fields>
                        </dnn:propertyeditorcontrol>
                    </td>
                </tr>
            </table>
        </td>
    </tr>
</table>
<p align="center">
	<dnn:commandbutton id="cmdUpdate" runat="server" CssClass="CommandButton" ImageUrl="~/images/save.gif" />&nbsp;
	<dnn:commandbutton id="cmdDelete" runat="server" CssClass="CommandButton" ImageUrl="~/images/delete.gif" resourcekey="cmdDelete" causesvalidation="False"  />&nbsp;
	<dnn:commandbutton id="cmdPackage" runat="server" CssClass="CommandButton" ImageUrl="~/images/icon_sitesettings_16px.gif" resourcekey="cmdPackage" causesvalidation="False"  />&nbsp;
	<dnn:commandbutton id="cmdCancel" runat="server" CssClass="CommandButton" ImageUrl="~/images/lt.gif" ResourceKey="cmdCancel" causesvalidation="False" />&nbsp;
</p>
<dnn:audit id="ctlAudit" runat="server" />