<%@ Control Language="VB" AutoEventWireup="false" CodeFile="Host.ascx.vb" Inherits="DotNetNuke.Modules.Dashboard.Controls.Host" %>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke" Namespace="DotNetNuke.UI.WebControls"%>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>

<dnn:Label id="plHost" runat="Server" CssClass="Head" ControlName="ctlHost" />
<dnn:propertyeditorcontrol id="ctlHost" runat="Server"
    autogenerate = "false"
    enableclientvalidation = "true"
    sortmode="SortOrderAttribute" 
    labelstyle-cssclass="SubHead" 
    helpstyle-cssclass="Help" 
    editcontrolstyle-cssclass="NormalTextBox" 
    labelwidth="200px" 
    editcontrolwidth="450px" 
    width="650px" 
    editmode="Edit" 
    errorstyle-cssclass="NormalRed">
    <Fields>
        <dnn:FieldEditorControl ID="fldProduct" runat="server" DataField="Product" />
        <dnn:FieldEditorControl ID="fldVersion" runat="server" DataField="Version" />
        <dnn:FieldEditorControl ID="fldHostGUID" runat="server" DataField="HostGUID" />
        <dnn:FieldEditorControl ID="fldPermissions" runat="server" DataField="Permissions" />
        <dnn:FieldEditorControl ID="fldDataProvider" runat="server" DataField="DataProvider" />
        <dnn:FieldEditorControl ID="fldCachingProvider" runat="server" DataField="CachingProvider" />
        <dnn:FieldEditorControl ID="fldLoggingProvider" runat="server" DataField="LoggingProvider" />
        <dnn:FieldEditorControl ID="fldHtmlEditorProvider" runat="server" DataField="HtmlEditorProvider" />
        <dnn:FieldEditorControl ID="fldFriendlyUrlProvider" runat="server" DataField="FriendlyUrlProvider" />
        <dnn:FieldEditorControl ID="fldFriendlyUrlEnabled" runat="server" DataField="FriendlyUrlEnabled" />
        <dnn:FieldEditorControl ID="fldFriendlyUrlType" runat="server" DataField="FriendlyUrlType" />
        <dnn:FieldEditorControl ID="fldSchedulerMode" runat="server" DataField="SchedulerMode" />
        <dnn:FieldEditorControl ID="fldWebFarmEnabled" runat="server" DataField="WebFarmEnabled" />
    </Fields>
</dnn:propertyeditorcontrol>
