<%@ Control Language="C#" AutoEventWireup="false" Inherits="DotNetNuke.Modules.Dashboard.Controls.Server" CodeFile="Server.ascx.cs" %>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke" Namespace="DotNetNuke.UI.WebControls"%>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>

<dnn:Label id="plServer" runat="Server" CssClass="Head" ControlName="ctlServer" />
<dnn:propertyeditorcontrol id="ctlServer" runat="Server"
    autogenerate = "false"
    enableclientvalidation = "true"
    sortmode="SortOrderAttribute" 
    labelstyle-cssclass="SubHead" 
    helpstyle-cssclass="Help" 
    editcontrolstyle-cssclass="NormalTextBox" 
    labelwidth="200px" 
    editcontrolwidth="450px" 
    editmode="Edit" 
    errorstyle-cssclass="NormalRed">
    <Fields>
        <dnn:FieldEditorControl ID="fldOsVersion" runat="server" DataField="OSVersion" />
        <dnn:FieldEditorControl ID="fldIISVersion" runat="server" DataField="IISVersion" />
        <dnn:FieldEditorControl ID="fldFramework" runat="server" DataField="Framework" />
        <dnn:FieldEditorControl ID="fldIdentity" runat="server" DataField="Identity" />
        <dnn:FieldEditorControl ID="fldHostName" runat="server" DataField="HostName" />
        <dnn:FieldEditorControl ID="fldPhysicalPath" runat="server" DataField="PhysicalPath" />
        <dnn:FieldEditorControl ID="fldUrl" runat="server" DataField="Url" />
        <dnn:FieldEditorControl ID="fldRelativePath" runat="server" DataField="RelativePath" />
        <dnn:FieldEditorControl ID="fldServerTime" runat="server" DataField="ServerTime" />
    </Fields>
</dnn:propertyeditorcontrol>
