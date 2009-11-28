<%@ Control language="vb" CodeFile="AuthenticationEditor.ascx.vb" AutoEventWireup="false" Explicit="True" Inherits="DotNetNuke.Modules.Admin.Authentication.AuthenticationEditor" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke" Namespace="DotNetNuke.UI.WebControls" %>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke.WebControls" Namespace="DotNetNuke.UI.WebControls"%>
<asp:Label ID="lblTitle" runat="server" cssClass="SubHead" resourcekey="Title" />
<br />
<asp:Label ID="lblHelp" runat="server" cssClass="Normal" />
<br /><br />
<dnn:propertyeditorcontrol id="ctlAuthentication" runat="Server"
    AutoGenerate="false"
    SortMode="SortOrderAttribute"
    editcontrolstyle-cssclass="NormalTextBox" 
    editcontrolwidth="400px" 
    ErrorStyle-cssclass="NormalRed"
    helpstyle-cssclass="Help" 
    labelstyle-cssclass="SubHead" 
    labelwidth="175px" 
    width="575px">
    <Fields>
        <dnn:FieldEditorControl ID="fldType" runat="server" DataField="AuthenticationType" />
        <dnn:FieldEditorControl ID="fldLoginControlSrc" runat="server" DataField="LoginControlSrc" EditControlStyle-Width="375px"  />
        <dnn:FieldEditorControl ID="fldLogoffControlSrc" runat="server" DataField="LogoffControlSrc" EditControlStyle-Width="375px" />
        <dnn:FieldEditorControl ID="fldSettingsControlSrc" runat="server" DataField="SettingsControlSrc" EditControlStyle-Width="375px" />
        <dnn:FieldEditorControl ID="fldIsEnabled" runat="server" DataField="IsEnabled" />
    </Fields>
</dnn:propertyeditorcontrol>
<asp:Panel ID="pnlSettings" runat="server" Visible="false">
    <p style="text-align:center">
        <dnn:commandbutton id="cmdUpdate" text="Update" runat="server" class="CommandButton" ImageUrl="~/images/save.gif"  ResourceKey="cmdUpdate" />
    </p>
</asp:Panel>

