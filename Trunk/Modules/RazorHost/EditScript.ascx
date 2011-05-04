<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="EditScript.ascx.cs" Inherits="DotNetNuke.Modules.RazorHost.EditScript" %>
<%@ Register Assembly="DotnetNuke" Namespace="DotNetNuke.UI.WebControls" TagPrefix="dnn" %>
<%@ Register Assembly="DotnetNuke.Web" Namespace="DotNetNuke.Web.UI.WebControls" TagPrefix="dnn" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<dnn:DnnToolTipManager ID="toolTipManager" runat="server" Position="Center" RelativeTo="BrowserWindow"
	Width="500px" Height="200px" HideEvent="ManualClose" ShowEvent="OnClick" Modal="true"
	Skin="Default" RenderInPageRoot="true" AnimationDuration="200" ManualClose="true"
	ManualCloseButtonText="Close" />

<div class="dnnForm dnnRazorHost dnnClear" id="dnnRazorHost">
    <fieldset>
        <legend></legend>
        <div class="dnnFormItem">
            <dnn:Label id="scriptsLabel" cssClass="dnnFormLabel" runat="Server" controlname="scriptList" />
            <asp:DropDownList ID="scriptList" cssClass="dnnFormInput" runat="server" AutoPostBack="true" />
			&nbsp;&nbsp;&nbsp;
			<asp:linkbutton ID="addNewButton" runat="server" ResourceKey="AddNew" cssclass="SecondaryAction" />
        </div>
        <div class="dnnFormItem">
            <label class="dnnFormLabel">&nbsp;</label>
            <asp:Label ID="lblSourceFile" cssClass="dnnFormInput" runat="server" />
        </div>
        <div class="dnnFormItem">
            <dnn:label id="plSource" cssClass="dnnFormLabel" controlname="txtSource" runat="server" />
            <asp:TextBox ID="txtSource" runat="server" cssClass="dnnFormInput" TextMode="MultiLine" Rows="20" Columns="80" />
        </div>
        <div class="dnnFormItem">
            <dnn:Label id="currentScriptLabel" cssClass="dnnFormLabel" runat="Server" controlname="isCurrentScript" />
            <asp:CheckBox ID="isCurrentScript" cssClass="dnnFormInput" runat="server" />
        </div>
    </fieldset>
    <ul class="dnnActions dnnClear">
        <li><asp:linkbutton id="cmdSave" resourcekey="cmdSave" runat="server" cssclass="PrimaryAction"/></li>
        <li><asp:linkbutton id="cmdSaveClose" resourcekey="cmdSaveClose" runat="server" cssclass="SecondaryAction" /></li>
        <li><asp:linkbutton id="cmdCancel" resourcekey="cmdCancel" runat="server" cssclass="SecondaryAction" causesvalidation="False" /></li>
    </ul>
</div>
