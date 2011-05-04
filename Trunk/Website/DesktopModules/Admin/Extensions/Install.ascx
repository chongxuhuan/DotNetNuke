<%@ Control Language="C#" AutoEventWireup="false" Inherits="DotNetNuke.Modules.Admin.Extensions.Install" CodeFile="Install.ascx.cs" %>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke" Namespace="DotNetNuke.UI.WebControls"%>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke.Web" Namespace="DotNetNuke.Web.UI.WebControls" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>

<div class="dnnForm dnnInstallExtension dnnClear" id="dnnInstallExtension">
    <asp:Wizard ID="wizInstall" runat="server"  DisplaySideBar="false" ActiveStepIndex="0"
        CellPadding="5" CellSpacing="5" width="100%"
        DisplayCancelButton="True"
        CancelButtonType="Link"
        StartNextButtonType="Link"
        StepNextButtonType="Link" 
        FinishCompleteButtonType="Link"
        >
        <CancelButtonStyle CssClass="dnnSecondaryAction" />
        <StartNextButtonStyle CssClass="dnnPrimaryAction" />
        <StepNextButtonStyle CssClass="dnnPrimaryAction" />
        <FinishCompleteButtonStyle CssClass="dnnPrimaryAction" />
        <StepStyle VerticalAlign="Top" />
        <NavigationButtonStyle CssClass="CommandButton" BorderStyle="None" BackColor="Transparent" />
        <HeaderTemplate>
            <asp:Label ID="lblTitle" CssClass="Head" runat="server"><% =GetText("Title") %></asp:Label><br /><br />
            <asp:Label ID="lblHelp" CssClass="WizardText" runat="server"><% =GetText("Help") %></asp:Label>
        </HeaderTemplate>
        <StartNavigationTemplate>
            <ul class="dnnActions dnnClear">
    	        <li><asp:LinkButton id="nextButtonStart" runat="server" CssClass="dnnPrimaryAction" CommandName="MoveNext" resourcekey="Next" /></li>
                <li><asp:LinkButton id="cancelButtonStart" runat="server" CssClass="dnnSecondaryAction" CommandName="Cancel" resourcekey="Cancel" Causesvalidation="False" /></li>
            </ul>
        </StartNavigationTemplate>
        <StepNavigationTemplate>
            <ul class="dnnActions dnnClear">
    	        <li><asp:LinkButton id="nextButtonStep" runat="server" CssClass="dnnPrimaryAction" CommandName="MoveNext" resourcekey="Next" /></li>
                <li><asp:LinkButton id="cancelButtonStep" runat="server" CssClass="dnnSecondaryAction" CommandName="Cancel" resourcekey="Cancel" Causesvalidation="False" /></li>
            </ul>
        </StepNavigationTemplate>
        <FinishNavigationTemplate>
            <ul class="dnnActions dnnClear">
    	        <li><asp:LinkButton id="finishButtonStep" runat="server" CssClass="dnnPrimaryAction" CommandName="Cancel" resourcekey="Return" /></li>
            </ul>
        </FinishNavigationTemplate>
        <WizardSteps>
            <asp:WizardStep ID="Step0" runat="Server" Title="Introduction" StepType="Start" AllowReturn="false">
                <div class="dnnForm">
                    <div class="dnnFormItem">
                        <asp:Label ID="lblBrowseFileHelp" runat="server" resourcekey="BrowseFileHelp" CssClass="WizardText" />
                    </div>
                    <div class="dnnFormItem">
                        <input id="cmdBrowse" type="file" size="50" name="cmdBrowse" runat="server" cssClass="dnnFormInput" />
                        <asp:Label ID="lblLoadMessage" runat="server" CssClass="dnnFormMessage dnnFormError" Visible="false" />
                    </div>
                </div>
            </asp:WizardStep>
            <asp:WizardStep ID="Step1" runat="server" Title="Warnings" StepType="Step" AllowReturn="false">
                <asp:Panel ID="pnlRepair" runat="server" Visible="false">
                    <asp:Label ID="lblWarningMessage" runat="server" EnableViewState="False" CssClass="NormalRed" />
                    <br />
                    <asp:Label ID="lblRepairInstallHelp" runat="server" resourcekey="RepairInstallHelp" CssClass="WizardText" />
                    <br />
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
                <dnn:DnnFormEditor id="packageForm" runat="Server" FormMode="Short">
                    <Items>
                        <dnn:DnnFormLiteralItem ID="moduleName" runat="server" DataField = "Name" />
                        <dnn:DnnFormLiteralItem ID="packageType" runat="server" DataField = "PackageType" />
                        <dnn:DnnFormLiteralItem ID="packageFriendlyName" runat="server" DataField = "FriendlyName" />
                        <dnn:DnnFormLiteralItem ID="iconFile" runat="server" DataField = "IconFile" />
                        <dnn:DnnFormLiteralItem ID="description" runat="server" DataField = "Description" />
                        <dnn:DnnFormLiteralItem ID="version" runat="server" DataField = "Version" />
                        <dnn:DnnFormLiteralItem ID="owner" runat="server" DataField = "Owner" />
                        <dnn:DnnFormLiteralItem ID="organization" runat="server" DataField = "Organization" />
                        <dnn:DnnFormLiteralItem ID="url" runat="server" DataField = "Url" />
                        <dnn:DnnFormLiteralItem ID="email" runat="server" DataField = "Email" />
                    </Items>
                </dnn:DnnFormEditor>
            </asp:WizardStep>
            <asp:WizardStep ID="Step3" runat="Server" Title="ReleaseNotes" StepType="Step" AllowReturn="false">
                <dnn:DnnFormEditor id="releaseNotesForm" runat="Server" FormMode="Short">
                    <Items>
                        <dnn:DnnFormLiteralItem ID="releaseNotes" runat="server" DataField = "ReleaseNotes" />
                    </Items>
                </dnn:DnnFormEditor>
            </asp:WizardStep>
            <asp:WizardStep ID="Step4" runat="server" Title="License" StepType="Step" AllowReturn="false">
                <dnn:DnnFormEditor id="licenseForm" runat="Server" FormMode="Short">
                    <Items>
                        <dnn:DnnFormLiteralItem ID="license" runat="server" DataField = "License" />
                    </Items>
                </dnn:DnnFormEditor>
                <div class="dnnForm">
                    <div class="dnnFormItem">
                        <dnn:Label ID="plAcceptLicense" runat="server" resourcekey="AcceptLicense" CssClass="dnnFormLabel" ControlName="chkAcceptLicense" />
                        <asp:CheckBox ID="chkAcceptLicense" runat="server"  CssClass="dnnFormLabel" />
                        <asp:Label ID="lblAcceptMessage" runat="server" Visible="false" EnableViewState="False" CssClass="dnnFormMessage dnnFormError" />
                    </div>
                    <div class="dnnFormItem">
                        <asp:PlaceHolder ID="phAcceptLogs" runat="server" />
                    </div>
                </div>
            </asp:WizardStep>
            <asp:WizardStep ID="Step5" runat="Server" Title="InstallResults" StepType="Finish">
                <table class="Settings" cellspacing="2" cellpadding="2" summary="Packages Install Design Table">
                    <tr><td align="left" colspan="2"><asp:Label ID="lblInstallMessage" runat="server" EnableViewState="False" CssClass="NormalRed" /></td></tr>
                    <tr><td><asp:PlaceHolder ID="phInstallLogs" runat="server" /></td></tr>
                </table>
            </asp:WizardStep>
        </WizardSteps>
    </asp:Wizard>
</div>
