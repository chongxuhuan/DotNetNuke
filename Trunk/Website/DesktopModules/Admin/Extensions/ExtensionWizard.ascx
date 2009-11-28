<%@ Control language="vb" CodeFile="ExtensionWizard.ascx.vb" AutoEventWireup="false" Explicit="True" Inherits="DotNetNuke.Modules.Admin.Extensions.ExtensionWizard" %>
<%@ Register TagPrefix="dnn" TagName="SectionHead" Src="~/controls/SectionHeadControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke" Namespace="DotNetNuke.UI.WebControls" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Security.Permissions.Controls" Assembly="DotNetNuke" %>

<asp:Wizard ID="wizNewExtension" runat="server"  DisplaySideBar="false" ActiveStepIndex="0"
    CellPadding="5" CellSpacing="5" 
    DisplayCancelButton="True"
    CancelButtonType="Link"
    StartNextButtonType="Link"
    StepNextButtonType="Link" 
    StepPreviousButtonType="Link"
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
            <table id="tblPackage" runat="server" cellspacing="2" cellpadding="4" broder="0">
                <tr id="trExtensionType" runat="server">
                    <td style="width:20px"></td>
                    <td class="SubHead" style="width:200px"><dnn:Label ID="plExtensionType" runat="server" ControlName="cboExtensionType /></td>
                    <td class="NormalTextBox" style="width:325px">
                        <asp:DropDownList ID="cboExtensionType" runat="server" DataTextField="Description" DataValueField="PackageType" AutoPostBack="true"/>
                    </td>
                </tr>
                <tr>
                    <td colspan="3">
                        <asp:Label ID="lblHelp" runat="server" cssClass="WizardText" resourcekey="IntroductionHelp" />
                        <br /><br />
                        <asp:Label ID="lblLanguageHelp" runat="server" cssClass="WizardText" resourcekey="LanguageHelp" />
                        <br /><br />
                        <asp:Label ID="lblExtensionLanguageHelp" runat="server" cssClass="WizardText" resourcekey="ExtensionLanguageHelp" />
                    </td>
                </tr>
                <tr>
                    <td style="width:20px"></td>
                    <td colspan="2">
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
                                    <dnn:FieldEditorControl ID="fldFriendlyName" runat="server" DataField="FriendlyName" EditControlStyle-Width="400px" Required="true" />
                                    <dnn:FieldEditorControl ID="fldDescription" runat="server" DataField="Description" EditorTypeName="DotNetNuke.UI.WebControls.MultiLineTextEditControl" EditControlStyle-Width="400px" EditControlStyle-Height="80px" />
                                    <dnn:FieldEditorControl ID="fldVersion" runat="server" DataField="Version" EditorTypeName="DotNetNuke.UI.WebControls.VersionEditControl"/>
                                </Fields>
                        </dnn:propertyeditorcontrol>
                    </td>
                </tr>
                 <tr>
                    <td colspan="3"><asp:Label ID="lblError" runat="server" cssClass="NormalRed" /></td>
                </tr>
           </table>
        </asp:WizardStep>
        <asp:WizardStep ID="Step1" runat="Server" Title="Specific" StepType="Step" AllowReturn="false">
            <asp:PlaceHolder ID="phEditor" runat="server" />
        </asp:WizardStep>
        <asp:WizardStep ID="Step2" runat="server" Title="OwnerInfo" StepType="Step" AllowReturn="false">
            <dnn:propertyeditorcontrol id="ctlOwner" runat="Server"
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
                        <dnn:FieldEditorControl ID="fldOwner" runat="server" DataField="Owner"  EditControlStyle-Width="250px" />
                        <dnn:FieldEditorControl ID="fldOrganization" runat="server" DataField="Organization"  EditControlStyle-Width="250px" />
                        <dnn:FieldEditorControl ID="fldUrl" runat="server" DataField="Url"  EditControlStyle-Width="250px" />
                        <dnn:FieldEditorControl ID="fldEmail" runat="server" DataField="Email"  EditControlStyle-Width="250px" />
                    </Fields>
            </dnn:propertyeditorcontrol>        
        </asp:WizardStep>
    </WizardSteps>
</asp:Wizard>