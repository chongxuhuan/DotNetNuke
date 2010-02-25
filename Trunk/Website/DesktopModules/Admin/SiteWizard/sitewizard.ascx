<%@ Control Inherits="DotNetNuke.Modules.Admin.Portals.SiteWizard" Language="vb"
    AutoEventWireup="false" Explicit="True" EnableViewState="True" CodeFile="SiteWizard.ascx.vb" %>
<%@ Register TagPrefix="dnn" TagName="Skin" Src="~/controls/SkinThumbNailControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="url" Src="~/controls/UrlControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="label" Src="~/controls/LabelControl.ascx" %>
<asp:Wizard ID="Wizard" runat="server" DisplaySideBar="false" ActiveStepIndex="0"
    CellPadding="5" CellSpacing="5" CssClass="Wizard" 
    StartNextButtonType="Link"
    StepNextButtonType="Link" 
    StepPreviousButtonType="Link" 
    FinishPreviousButtonType="Link"
    FinishCompleteButtonType="Link"
    
    >
    <StepStyle VerticalAlign="Top" />
    <NavigationButtonStyle CssClass="CommandButton" BorderStyle="None" BackColor="Transparent" />
    <HeaderTemplate>
        <asp:Label ID="lblTitle" CssClass="Head" runat="server"><% =Localization.GetString(Wizard.ActiveStep.Title + ".Title", Me.LocalResourceFile)%></asp:Label><br /><br />
        <asp:Label ID="lblHelp" CssClass="WizardText" runat="server"><% =Localization.GetString(Wizard.ActiveStep.Title + ".Help", Me.LocalResourceFile)%></asp:Label>
    </HeaderTemplate>
    <WizardSteps>
        <asp:WizardStep ID="wizIntroduction" runat="server" Title="Introduction" StepType="Start" AllowReturn="false" />
        <asp:WizardStep ID="wizTemplate" runat="server" Title="Template">
            <table cellspacing="0" cellpadding="0" border="0">
                <tr>
                    <td colspan="3" height="5"></td>
                </tr>
                <tr>
                    <td colspan="3">
                        <asp:CheckBox ID="chkTemplate" runat="server" CssClass="WizardText" AutoPostBack="True"
                            resourcekey="TemplateDetail" Text="Build your site from a template (below)" />
                    </td>
                </tr>
                <tr>
                    <td colspan="3" height="5"></td>
                </tr>
                <tr>
                    <td width="150" align="center">
                        <asp:ListBox ID="lstTemplate" runat="server" Width="150" Rows="8" AutoPostBack="True" />
                    </td>
                    <td colspan="2" valign="top" align="left" width="300">
                        <asp:Label ID="lblTemplateMessage" runat="server" CssClass="NormalRed" Style="overflow: auto;
                            width: 280px; height: 150px" />
                    </td>
                </tr>
                <tr>
                    <td colspan="3" height="5"></td>
                </tr>
                <tr>
                    <td colspan="3">
                        <asp:Label ID="lblMergeTitle" runat="server" resourcekey="MergeDetail" CssClass="WizardText"/>
                    </td>
                </tr>
                <tr>
                    <td colspan="3" height="5"></td>
                </tr>
                <tr>
                    <td colspan="3" align="center">
                        <asp:RadioButtonList ID="optMerge" CssClass="WizardText" runat="server" RepeatDirection="Horizontal">
                            <asp:ListItem Selected Value="Ignore" resourcekey="Ignore">Ignore</asp:ListItem>
                            <asp:ListItem Value="Replace" resourcekey="Replace">Replace</asp:ListItem>
                            <asp:ListItem Value="Merge" resourcekey="Merge">Merge</asp:ListItem>
                        </asp:RadioButtonList>
                    </td>
                </tr>
                <tr>
                    <td colspan="3" height="5"></td>
                </tr>
                <tr>
                    <td colspan="3">
                        <asp:Label ID="lblMergeWarning" runat="server" resourcekey="MergeWarning" CssClass="WizardText"/>
                    </td>
                </tr>
            </table>
        </asp:WizardStep>
        <asp:WizardStep ID="wizSkin" runat="server" Title="Skin">
            <table cellspacing="0" cellpadding="0" border="0">
                <tr>
                    <td height="5"></td>
                </tr>
                <tr>
                    <td align="center">
                        <dnn:Skin ID="ctlPortalSkin" runat="server"></dnn:Skin>
                    </td>
                </tr>
            </table>
        </asp:WizardStep>
        <asp:WizardStep ID="wizContainer" runat="server" Title="Container">
            <table cellspacing="0" cellpadding="0" border="0">
                <tr>
                    <td height="5"></td>
                </tr>
                <tr>
                    <td>
                        <asp:CheckBox ID="chkIncludeAll" CssClass="WizardText" runat="server" resourcekey="IncludeAll"
                            TextAlign="Left" Text="Show All Containers:" AutoPostBack="True"></asp:CheckBox></td>
                </tr>
                <tr>
                    <td height="5"></td>
                </tr>
                <tr>
                    <td align="center" colspan="2">
                        <dnn:Skin ID="ctlPortalContainer" runat="server"></dnn:Skin>
                    </td>
                </tr>
            </table>
        </asp:WizardStep>
        <asp:WizardStep ID="wizDetails" runat="server" Title="Details">
            <table cellspacing="0" cellpadding="0" border="0">
                <tr><td colspan="2" height="5"></td></tr>
                <tr>
                    <td class="SubHead" width="150">
                        <dnn:label ID="lblPortalName" runat="server" Text="Name/Title:" ControlName="txtPortalName" />
                    </td>
                    <td class="NormalTextBox" valign="top" align="left">
                        <asp:TextBox ID="txtPortalName" CssClass="NormalTextBox" runat="server" Width="300"
                            MaxLength="128"></asp:TextBox></td>
                </tr>
                <tr><td colspan="2" height="5"></td></tr>
                <tr>
                    <td class="SubHead" valign="top" width="150">
                        <dnn:label ID="lblDescription" runat="server" Text="Description:" />
                    </td>
                    <td class="NormalTextBox" align="left">
                        <asp:TextBox ID="txtDescription" CssClass="NormalTextBox" runat="server" Width="300"
                            MaxLength="475" Rows="3" TextMode="MultiLine"></asp:TextBox></td>
                </tr>
                <tr><td colspan="2" height="5"></td></tr>
                <tr>
                    <td class="SubHead" valign="top" width="150">
                        <dnn:label ID="lblKeyWords" runat="server" Text="Key Words:" />
                    </td>
                    <td class="NormalTextBox" align="left">
                        <asp:TextBox ID="txtKeyWords" CssClass="NormalTextBox" runat="server" Width="300"
                            MaxLength="475" Rows="3" TextMode="MultiLine"></asp:TextBox></td>
                </tr>
                <tr><td colspan="2" height="5"></td></tr>
                <tr>
                    <td class="SubHead" valign="top" width="120">
                        <dnn:label ID="lblLogo" runat="server" Text="Logo:" />
                    </td>
                    <td class="NormalTextBox" align="left">
                        <dnn:url ID="urlLogo" runat="server" ShowLog="False" ShowTabs="False" ShowUrls="False"
                            ShowTrack="false" Required="false" />
                    </td>
                </tr>
            </table>
        </asp:WizardStep>
        <asp:WizardStep ID="wizComplete" runat="server" StepType="Complete">
            <asp:Label ID="lblWizardTitle" CssClass="Head" resourcekey="Complete.Title" runat="server" /><br /><br />
            <asp:Label ID="lblHelp" CssClass="WizardText" resourcekey="Complete.Help" runat="server" />
        </asp:WizardStep>
    </WizardSteps>
</asp:Wizard>
