<%@ Control Language="C#" AutoEventWireup="false" Inherits="DotNetNuke.Modules.Admin.ModuleDefinitions.EditModuleControl" CodeFile="EditModuleControl.ascx.cs" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke" Namespace="DotNetNuke.UI.WebControls" %>

<div class="dnnForm dnnEditModuleControl dnnClear" id="dnnEditModuleControl">
    <fieldset>
        <legend></legend>
        <div class="dnnFormItem">
            <dnn:label id="plModule" cssclass="dnnFormLabel" controlname="lblModule" runat="server" />
            <asp:Label id="lblModule" cssclass="dnnFormInput" runat="server" />
        </div>
        <div class="dnnFormItem">
            <dnn:label id="plDefinition" cssclass="dnnFormLabel" controlname="lblDefinition" runat="server" />
            <asp:Label id="lblDefinition" cssclass="dnnFormInput"	runat="server" />
        </div>
        <div class="dnnFormItem">
            <dnn:label id="plKey" cssclass="dnnFormLabel" controlname="txtKey" runat="server" />
            <asp:textbox id="txtKey" cssclass="dnnFormInput" columns="30" maxlength="50" runat="server" />
        </div>
        <div class="dnnFormItem">
            <dnn:label id="plTitle" cssclass="dnnFormLabel" controlname="txtTitle" runat="server" />
            <asp:textbox id="txtTitle" cssclass="dnnFormInput" columns="30" maxlength="50" runat="server" />
        </div>
        <div class="dnnFormItem">
            <dnn:label id="plSource" cssclass="dnnFormLabel" controlname="cboSource" runat="server" />
            <asp:dropdownlist id="cboSource" runat="server" cssclass="dnnFormInput" autopostback="True" />
        </div>
        <div class="dnnFormItem">
            <label class="dnnFormLabel" ></label>
	        <asp:textbox id="txtSource" cssclass="dnnFormInput" columns="30" maxlength="100" runat="server" />
        </div>
        <div class="dnnFormItem">
            <dnn:label id="plType" cssclass="dnnFormLabel" controlname="cboType" runat="server" />
            <asp:dropdownlist id="cboType" runat="server" width="390" cssclass="dnnFormInput">
				<asp:listitem resourcekey="Skin" value="-2" />
				<asp:listitem resourcekey="Anonymous" value="-1" />
				<asp:listitem resourcekey="View" value="0" />
				<asp:listitem resourcekey="Edit" value="1" />
				<asp:listitem resourcekey="Admin" value="2" />
				<asp:listitem resourcekey="Host" value="3" />
			</asp:dropdownlist>
        </div>
        <div class="dnnFormItem">
            <dnn:label id="plViewOrder" cssclass="dnnFormLabel" controlname="txtViewOrder" runat="server" />
            <asp:textbox id="txtViewOrder" cssclass="dnnFormInput" columns="30" maxlength="2" runat="server" />
        </div>
        <div class="dnnFormItem">
            <dnn:label id="plIcon" cssclass="dnnFormLabel" controlname="cboIcon" runat="server" />
            <asp:dropdownlist id="cboIcon" runat="server" cssclass="dnnFormInput" datavaluefield="Value" datatextfield="Text" />
        </div>
        <div class="dnnFormItem">
            <dnn:label id="plHelpURL" cssclass="dnnFormLabel" controlname="txtHelpURL" runat="server" />
            <asp:textbox id="txtHelpURL" runat="server" maxlength="200" columns="30" cssclass="dnnFormInput" />
        </div>
        <div class="dnnFormItem">
            <dnn:label id="supportsModalPopUpsLabel" cssclass="dnnFormLabel" controlname="supportsModalPopUpsCheckBox" runat="server" />
            <asp:checkbox id="supportsModalPopUpsCheckBox" runat="server" cssclass="dnnFormInput"/>
        </div>
        <div class="dnnFormItem">
            <dnn:label id="plSupportsPartialRendering" cssclass="dnnFormLabel" controlname="chkSupportsPartialRendering" runat="server" />
            <asp:checkbox id="chkSupportsPartialRendering" runat="server" cssclass="dnnFormInput"/>
        </div>
    </fieldset>
    <ul class="dnnActions dnnClear">
        <li><asp:linkbutton id="cmdUpdate" resourcekey="cmdUpdate" runat="server" cssclass="dnnPrimaryAction"/></li>
        <li><asp:linkbutton id="cmdDelete" resourcekey="cmdDelete" runat="server" cssclass="dnnSecondaryAction" CausesValidation="false" /></li>
        <li><asp:linkbutton id="cmdCancel" resourcekey="cmdCancel" runat="server" cssclass="dnnSecondaryAction" CausesValidation="false" /></li>
    </ul>
</div>