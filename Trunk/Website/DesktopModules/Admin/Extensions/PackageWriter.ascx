<%@ Control Language="vb" AutoEventWireup="false" Explicit="True" Inherits="DotNetNuke.Modules.Admin.Extensions.PackageWriter"
    CodeFile="PackageWriter.ascx.vb" %>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke" Namespace="DotNetNuke.UI.WebControls" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/Controls/LabelControl.ascx" %>
<asp:Label ID="lblTitle" runat="server" CssClass="Head" />
<br />
<asp:Wizard ID="wizPackage" runat="server" DisplaySideBar="false" ActiveStepIndex="0"
    CellPadding="5" CellSpacing="5" DisplayCancelButton="True" CancelButtonType="Link"
    StartNextButtonType="Link" StepNextButtonType="Link" StepPreviousButtonType="Link"
    FinishCompleteButtonType="Link">
    <StepStyle VerticalAlign="Top" />
    <NavigationButtonStyle CssClass="CommandButton" BorderStyle="None" BackColor="Transparent" />
    <HeaderTemplate>
        <div>
            <asp:Label ID="lblTitle" CssClass="Head" runat="server"><% =GetText("Title") %></asp:Label><br />
            <br />
            <asp:Label ID="lblHelp" CssClass="WizardText" runat="server"><% =GetText("Help") %></asp:Label>
        </div>
    </HeaderTemplate>
    <WizardSteps>
        <asp:WizardStep ID="Step0" runat="Server" Title="Introduction" StepType="Start" AllowReturn="true">
            <table cellspacing="2" cellpadding="2" style="width: 650px">
                <tr>
                    <td colspan="2">
                        <dnn:PropertyEditorControl ID="ctlPackage" runat="Server" AutoGenerate="false" SortMode="SortOrderAttribute"
                            EditControlStyle-CssClass="NormalTextBox" EditControlWidth="400px" ErrorStyle-CssClass="NormalRed"
                            HelpStyle-CssClass="Help" LabelStyle-CssClass="SubHead" LabelWidth="200px" Width="650px">
                            <Fields>
                                <dnn:FieldEditorControl ID="fldName" runat="server" DataField="Name" />
                                <dnn:FieldEditorControl ID="fldPackageType" runat="server" DataField="PackageType" />
                                <dnn:FieldEditorControl ID="fldFriendlyName" runat="server" DataField="FriendlyName" />
                                <dnn:FieldEditorControl ID="fldVersion" runat="server" DataField="Version" EditorTypeName="DotNetNuke.UI.WebControls.VersionEditControl"  />
                            </Fields>
                        </dnn:PropertyEditorControl>
                    </td>
                </tr>
                <tr style="height:15px;"><td>&nbsp;</td></tr>
                <tr>
                    <td colspan="2"><asp:Label ID="lblManifestHelp" CssClass="WizardText" runat="server" resourcekey="ManifestHelp"/></td>
                </tr>
                <tr style="height:15px"><td>&nbsp;</td></tr>
                <tr>
                    <td style="width:200px">
                        <dnn:Label ID="plUseManifest" runat="server" CssClass="SubHead" ControlName="chkUseManifest" />
                    </td>
                    <td style="width: 450px">
                        <asp:CheckBox ID="chkUseManifest" runat="server" CssClass="SubHead" AutoPostBack="true" />
                    </td>
                </tr>
                <tr id="trManifests" runat="server" visible="false">
                    <td style="width:200px">
                        <dnn:Label ID="plChooseManifest" runat="server" CssClass="SubHead" ControlName="cboManifests" />
                    </td>
                    <td style="width: 450px">
                        <asp:DropDownList ID="cboManifests" runat="server" CssClass="NormalTextBox" />
                    </td>
                </tr>
                <tr>
                    <td style="width:200px">
                        <dnn:Label ID="plReviewManifest" runat="server" CssClass="SubHead" ControlName="chkReviewManifest" />
                    </td>
                    <td style="width: 450px">
                        <asp:CheckBox ID="chkReviewManifest" runat="server" CssClass="SubHead" Checked="true" />
                    </td>
                </tr>
            </table>        
        </asp:WizardStep>
        <asp:WizardStep ID="Step1" runat="server" Title="ChooseFiles" StepType="Step">
            <table cellspacing="2" cellpadding="2" style="width: 600px">
                <tr>
                    <td rowspan="2" style="vertical-align: top; width: 100px">
                        <dnn:Label ID="plBasePath" runat="server" CssClass="SubHead" ControlName="txtBasePath" />
                    </td>
                    <td style="width: 500px">
                        <asp:TextBox ID="txtBasePath" runat="server" Style="width: 300px" />
                        <dnn:CommandButton ID="cmdGetFiles" runat="server" ResourceKey="cmdGetFiles" ImageUrl="~/images/action_refresh.gif" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:CheckBox ID="chkIncludeSource" runat="server" CssClass="SubHead" resourceKey="chkIncludeSource" TextAlign="Left" />
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:TextBox ID="txtFiles" runat="server" CssClass="NormalTextBox" TextMode="MultiLine" Style="height: 250px; width: 500px" />
                    </td>
                </tr>
            </table>
        </asp:WizardStep>
        <asp:WizardStep ID="Step2" runat="server" Title="ChooseAssemblies" StepType="Step">
            <table cellspacing="2" cellpadding="2" style="width: 600px">
                <tr>
                    <td colspan="2">
                        <asp:TextBox ID="txtAssemblies" runat="server" CssClass="NormalTextBox" TextMode="MultiLine" Style="height: 100px; width: 500px" />
                    </td>
                </tr>
            </table>
        </asp:WizardStep>
        <asp:WizardStep ID="Step3" runat="server" Title="CreateManifest" StepType="Step">
            <table cellspacing="2" cellpadding="2" style="width: 600px">
                <tr>
                    <td colspan="2">
                        <asp:TextBox ID="txtManifest" runat="server" CssClass="NormalTextBox" TextMode="MultiLine" Style="height: 300px; width: 650px" />
                    </td>
                </tr>
            </table>
        </asp:WizardStep>
        <asp:WizardStep ID="StepLast" runat="Server" Title="FinalStep" StepType="Step" AllowReturn="false">
            <table cellspacing="2" cellpadding="2" style="width: 600px">
                <tr id="trManifest1" runat="server">
                    <td style="vertical-align: top; width: 200px">
                        <dnn:label id="plManifest" controlname="chkManifest" runat="server" CssClass="SubHead" />
                    </td>
                    <td style="width: 400px">
                        <asp:CheckBox ID="chkManifest" runat="server" Checked="true" />
                    </td>
                </tr>
                <tr id="trManifest2" runat="server">
                    <td style="vertical-align: top; width: 200px">
                        <dnn:label id="plManifestName" controlname="txtManifestName" runat="server" CssClass="SubHead" />
                    </td>
                    <td style="width: 400px">
                        <asp:TextBox ID="txtManifestName" runat="server" Style="width: 250px" />
                    </td>
                </tr>
                <tr>
                    <td style="vertical-align: top; width: 200px">
                        <dnn:label id="plPackage" controlname="chkPackage" runat="server" CssClass="SubHead" />
                    </td>
                    <td style="width: 400px">
                        <asp:CheckBox ID="chkPackage" runat="server" Checked="true" />
                    </td>
                </tr>
                <tr>
                    <td style="vertical-align: top; width: 200px">
                        <dnn:label id="plArchiveName" controlname="txtArchiveName" runat="server" CssClass="SubHead" />
                    </td>
                    <td style="width: 400px">
                        <asp:TextBox ID="txtArchiveName" runat="server" Style="width: 250px" />
                    </td>
                </tr>
                <tr><td align="left" colspan="2"><asp:Label ID="lblMessage" runat="server" EnableViewState="False" CssClass="NormalRed" /></td></tr>
            </table>
        </asp:WizardStep>
        <asp:WizardStep ID="StepFinish" runat="Server" Title="WriterResults" StepType="Finish">
            <table class="Settings" cellspacing="2" cellpadding="2">
                <tr><td align="left" colspan="2"><asp:Label ID="lblInstallMessage" runat="server" EnableViewState="False" CssClass="NormalRed" /></td></tr>
                <tr><td><asp:PlaceHolder ID="phInstallLogs" runat="server" /></td></tr>
            </table>
        </asp:WizardStep>
    </WizardSteps>
</asp:Wizard>
