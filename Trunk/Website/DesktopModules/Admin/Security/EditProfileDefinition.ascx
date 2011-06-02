<%@ Control Inherits="DotNetNuke.Modules.Admin.Users.EditProfileDefinition" 
    Language="C#" AutoEventWireup="false" CodeFile="EditProfileDefinition.ascx.cs" %>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke" Namespace="DotNetNuke.UI.WebControls"%>
<%@ Register TagPrefix="dnn" TagName="SectionHead" Src="~/controls/SectionHeadControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="ListEntries" Src="~/DesktopModules/Admin/Lists/ListEntries.ascx" %>
<asp:Wizard ID="Wizard" runat="server" DisplaySideBar="false" ActiveStepIndex="0"
    CellPadding="5" CellSpacing="5" CssClass="Wizard" 
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
        <asp:WizardStep ID="wizIntroduction" runat="server" Title="Introduction" StepType="Start" AllowReturn="false">
            <dnn:propertyeditorcontrol id="Properties" runat="Server"
                SortMode="SortOrderAttribute"
                ErrorStyle-cssclass="NormalRed"
                helpstyle-cssclass="dnnFormHelpContent dnnClear"
                />
        </asp:WizardStep>
        <asp:WizardStep ID="wizLists" runat="server" Title="List" AllowReturn="false">
            <dnn:ListEntries id="lstEntries" runat="server" />
       </asp:WizardStep>
        <asp:WizardStep ID="wizLocalization" runat="server" Title="Localization" AllowReturn="false">
            <table cellpadding="0" cellspacing="0" border="0">
                <tr height="25">
                    <td style="width:200px; vertical-align:top" class="SubHead"><dnn:Label ID="plLocales" runat="server" ControlName="cboLocales"/></td>
                    <td valign="top">
                        <asp:DropDownList ID="cboLocales" runat="server" AutoPostBack="True" Width="200px" DataValueField="key" DataTextField="name" CssClass="NormalTextBox" />
                        <asp:Label ID="lblLocales" runat="server" CssClass="Normal" />
                    </td>
                </tr>
                <tr>
                    <td style="width:200px; vertical-align:top" class="SubHead"><dnn:Label ID="plPropertyName" runat="server" ControlName="txtPropertyName"/></td>
                    <td><asp:TextBox ID="txtPropertyName" runat="Server" Width="200px" /></td>
                </tr>
                <tr>
                    <td style="width:200px; vertical-align:top" class="SubHead"><dnn:Label ID="plPropertyHelp" runat="server" ControlName="txtPropertyHelp"/></td>
                    <td><asp:TextBox ID="txtPropertyHelp" runat="Server" CssClass="NormalTextBox" Width="200px" Columns="25" Rows="3" TextMode="MultiLine" Wrap="true" /></td>
                </tr>
                <tr>
                    <td style="width:200px; vertical-align:top" class="SubHead"><dnn:Label ID="plPropertyRequired" runat="server" ControlName="txtPropertyRequired"/></td>
                    <td><asp:TextBox ID="txtPropertyRequired" runat="Server"  Width="200px" /></td>
                </tr>
                <tr>
                    <td style="width:200px; vertical-align:top" class="SubHead"><dnn:Label ID="plPropertyValidation" runat="server" ControlName="txtPropertyValidation"/></td>
                    <td><asp:TextBox ID="txtPropertyValidation" runat="Server" Width="200px" /></td>
                </tr>
                <tr>
                    <td style="width:200px; vertical-align:top" class="SubHead"><dnn:Label ID="plCategoryName" runat="server" ControlName="txtCategoryName"/></td>
                    <td><asp:TextBox ID="txtCategoryName" runat="Server" Width="200px" /></td>
                </tr>
                <tr>
                    <td colspan="2" align="center"><dnn:commandbutton class="CommandButton" id="cmdSaveKeys" IconKey="Save" resourcekey="cmdSaveKeys" runat="server" text="Save"/></td>
                </tr>
            </table>
        </asp:WizardStep>
    </WizardSteps>
</asp:Wizard>

