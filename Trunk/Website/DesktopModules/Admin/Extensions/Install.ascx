<%@ Control Language="vb" AutoEventWireup="false" Explicit="True" Inherits="DotNetNuke.Modules.Admin.Extensions.Install" CodeFile="Install.ascx.vb" %>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke" Namespace="DotNetNuke.UI.WebControls"%>
<asp:Wizard ID="wizInstall" runat="server"  DisplaySideBar="false" ActiveStepIndex="0"
    CellPadding="5" CellSpacing="5" 
    DisplayCancelButton="True"
    CancelButtonType="Link"
    StartNextButtonType="Link"
    StepNextButtonType="Link" 
    FinishCompleteButtonType="Link"
    >
    <StepStyle VerticalAlign="Top" />
    <NavigationButtonStyle CssClass="CommandButton" BorderStyle="None" BackColor="Transparent" />
    <HeaderTemplate>
        <asp:Label ID="lblTitle" CssClass="Head" runat="server"><% =GetText("Title") %></asp:Label><br /><br />
        <asp:Label ID="lblHelp" CssClass="WizardText" runat="server"><% =GetText("Help") %></asp:Label>
    </HeaderTemplate>
    <WizardSteps>
        <asp:WizardStep ID="Step0" runat="Server" Title="Introduction" StepType="Start" AllowReturn="false">
            <table class="Settings" cellspacing="2" cellpadding="2" summary="Extensions Install Design Table" style="width:100%;">
                <tr>
                    <td><asp:Label ID="lblBrowseFileHelp" runat="server" resourcekey="BrowseFileHelp" CssClass="WizardText" /></td>
                </tr>
                <tr>
                    <td><input id="cmdBrowse" type="file" size="50" name="cmdBrowse" runat="server" /></td>
                </tr>
                <tr>
                    <td><asp:Label ID="lblLoadMessage" runat="server" CssClass="NormalRed" Visible="false" /></td>
                </tr>
            </table>
        </asp:WizardStep>
        <asp:WizardStep ID="Step1" runat="server" Title="Warnings" StepType="Step" AllowReturn="false">
            <asp:Label ID="lblWarningMessage" runat="server" EnableViewState="False" CssClass="NormalRed" /><br /><br />
            <asp:Panel ID="pnlRepair" runat="server" Visible="false">
                <asp:Label ID="lblRepairInstallHelp" runat="server" resourcekey="RepairInstallHelp" CssClass="WizardText" /><br />
                <asp:CheckBox ID="chkRepairInstall" runat="server" resourcekey="RepairInstall" CssClass="SubHead" TextAlign="Left" AutoPostBack="true" />
            </asp:Panel>
            <asp:Panel ID="pnlLegacy" runat="server" Visible="false">
                <asp:RadioButtonList ID="rblLegacySkin" runat="server" CssClass="NormalTextBox" RepeatDirection="Horizontal" >
                    <asp:ListItem Value="Skin" resourcekey="Skin"/>
                    <asp:ListItem Value="Container" resourcekey="Container"/>
                    <asp:ListItem Value="None" Selected="True" resourcekey="None"/>
                </asp:RadioButtonList>
            </asp:Panel>
            <asp:Panel ID="pnlWhitelist" runat = "server" Visible="false">
                <asp:Label ID="lblIgnoreWhiteListHelp" runat="server" resourcekey="IgnoreWhiteListHelp" CssClass="WizardText" /><br />
                <asp:CheckBox ID="chkIgnoreWhiteList" runat="server" resourcekey="IgnoreWhiteList" CssClass="SubHead" TextAlign="Left" AutoPostBack="true" />
            </asp:Panel>
            <asp:PlaceHolder ID="phLoadLogs" runat="server" />
       </asp:WizardStep>
        <asp:WizardStep ID="Step2" runat="Server" Title="PackageInfo" StepType="Step" AllowReturn="false">
            <table class="Settings" cellspacing="2" cellpadding="2" summary="Packages Install Design Table">
                <tr>
                    <td>
                        <dnn:propertyeditorcontrol id="ctlPackage" runat="Server"
                            autoGenerate="false"
                            editcontrolstyle-cssclass="NormalTextBox" 
                            editcontrolwidth="450px" 
                            errorStyle-cssclass="NormalRed"
                            helpstyle-cssclass="Help" 
                            labelstyle-cssclass="SubHead" 
                            labelwidth="175px" 
                            width="650px">
                            <Fields>
                                <dnn:FieldEditorControl ID="fldName" runat="server" DataField="Name" />
                                <dnn:FieldEditorControl ID="fldPackageType" runat="server" DataField="PackageType" />
                                <dnn:FieldEditorControl ID="fldFriendlyName" runat="server" DataField="FriendlyName" />
                                <dnn:FieldEditorControl ID="fldDescription" runat="server" DataField="Description" />
                                <dnn:FieldEditorControl ID="fldVersion" runat="server" DataField="Version" EditorTypeName="DotNetNuke.UI.WebControls.VersionEditControl" />
                                <dnn:FieldEditorControl ID="fldOwner" runat="server" DataField="Owner"  />
                                <dnn:FieldEditorControl ID="fldOrganization" runat="server" DataField="Organization"  />
                                <dnn:FieldEditorControl ID="fldUrl" runat="server" DataField="Url"  />
                                <dnn:FieldEditorControl ID="fldEmail" runat="server" DataField="Email"  />
                            </Fields>
                        </dnn:propertyeditorcontrol>
                    </td>
                </tr>
            </table>
        </asp:WizardStep>
        <asp:WizardStep ID="Step3" runat="Server" Title="ReleaseNotes" StepType="Step" AllowReturn="false">
            <table class="Settings" cellspacing="2" cellpadding="2" summary="Packages Install Design Table">
                <tr>
                    <td>
                        <dnn:propertyeditorcontrol id="ctlReleaseNotes" runat="Server"
                            AutoGenerate="false"
                            ErrorStyle-cssclass="NormalRed"
                            labelstyle-cssclass="SubHead" 
                            helpstyle-cssclass="Help" 
                            editcontrolstyle-cssclass="NormalTextBox" 
                            labelwidth="150px" 
                            editcontrolwidth="450px" 
                            width="600px">
                            <Fields>
                                <dnn:FieldEditorControl ID="fldReleaseNotes" runat="server" DataField="ReleaseNotes" EditorTypeName="DotNetNuke.UI.WebControls.DNNRichTextEditControl" />
                            </Fields>
                        </dnn:propertyeditorcontrol>
                    </td>
                </tr>
            </table>        
        </asp:WizardStep>
        <asp:WizardStep ID="Step4" runat="server" Title="License" StepType="Step" AllowReturn="false">
            <table class="Settings" cellspacing="2" cellpadding="2" summary="Packages Install Design Table">
                <tr>
                    <td>
                        <dnn:propertyeditorcontrol id="ctlLicense" runat="Server"
                            AutoGenerate="false"
                            ErrorStyle-cssclass="NormalRed"
                            labelstyle-cssclass="SubHead" 
                            helpstyle-cssclass="Help" 
                            editcontrolstyle-cssclass="NormalTextBox" 
                            labelwidth="150px" 
                            editcontrolwidth="450px" 
                            width="600px">
                            <Fields>
                                <dnn:FieldEditorControl ID="fldLicense" runat="server" DataField="License" EditorTypeName="DotNetNuke.UI.WebControls.DNNRichTextEditControl" />
                            </Fields>
                        </dnn:propertyeditorcontrol>
                    </td>
                </tr>
                <tr><td><asp:CheckBox ID="chkAcceptLicense" runat="server" resourcekey="AcceptLicense" CssClass="SubHead" TextAlign="Left" /></td></tr>
                <tr><td align="left" colspan="2"><asp:Label ID="lblAcceptMessage" runat="server" EnableViewState="False" CssClass="NormalRed" /></td></tr>
                <tr><td><asp:PlaceHolder ID="phAcceptLogs" runat="server" /></td></tr>
            </table>
        </asp:WizardStep>
        <asp:WizardStep ID="Step5" runat="Server" Title="InstallResults" StepType="Finish">
            <table class="Settings" cellspacing="2" cellpadding="2" summary="Packages Install Design Table">
                <tr><td align="left" colspan="2"><asp:Label ID="lblInstallMessage" runat="server" EnableViewState="False" CssClass="NormalRed" /></td></tr>
                <tr><td><asp:PlaceHolder ID="phInstallLogs" runat="server" /></td></tr>
            </table>
        </asp:WizardStep>
    </WizardSteps>
</asp:Wizard>
