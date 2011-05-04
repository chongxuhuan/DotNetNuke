<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="CreateModule.ascx.cs" Inherits="DotNetNuke.Modules.RazorHost.CreateModule" %>
<%@ Register Assembly="DotnetNuke" Namespace="DotNetNuke.UI.WebControls" TagPrefix="dnn" %>
<%@ Register Assembly="DotnetNuke.Web" Namespace="DotNetNuke.Web.UI.WebControls" TagPrefix="dnn" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<dnn:DnnToolTipManager ID="toolTipManager" runat="server" Position="Center" RelativeTo="BrowserWindow"
	Width="500px" Height="200px" HideEvent="ManualClose" ShowEvent="OnClick" Modal="true"
	Skin="Default" RenderInPageRoot="true" AnimationDuration="200" ManualClose="true"
	ManualCloseButtonText="Close" />
<asp:RequiredFieldValidator ID="valFolder" runat="server" resourceKey="valFolder" ControlToValidate="txtFolder"
		CssClass="NormalRed" EnableClientScript="true" Display="Dynamic" />
<asp:RequiredFieldValidator ID="valName" runat="server" resourceKey="valName" ControlToValidate="txtName"
		CssClass="NormalRed" EnableClientScript="true" Display="Dynamic" />

<div class="dnnForm dnnRazorHost dnnClear" id="dnnRazorHost">
    <fieldset>
        <legend></legend>
        <div class="dnnFormItem">
            <dnn:Label id="scriptsLabel" cssClass="dnnFormLabel" runat="Server" controlname="scriptList" />
            <asp:DropDownList ID="scriptList" cssClass="dnnFormInput" runat="server" AutoPostBack="true" />
        </div>
        <div class="dnnFormItem">
            <asp:Label ID="lblSourceFile" runat="server" cssClass="NormalBold"/>
        </div>
        <div class="dnnFormItem">
            <asp:Label ID="lblModuleControl" runat="server" cssClass="NormalBold"/>
        </div>
        <div class="dnnFormItem">
            <dnn:label id="plFolder" cssClass="dnnFormLabel" controlname="txtFolder" runat="server" />
            <asp:TextBox ID="txtFolder" runat="server" cssClass="dnnFormInput"  />
        </div>
        <div class="dnnFormItem">
            <dnn:label id="plName" cssClass="dnnFormLabel" controlname="txtName" runat="server" />
            <asp:TextBox ID="txtName" runat="server" cssClass="dnnFormInput"  />
        </div>
        <div class="dnnFormItem">
            <dnn:label id="plDescription" cssClass="dnnFormLabel" controlname="txtDescription" runat="server" />
            <asp:TextBox ID="txtDescription" runat="server" cssClass="dnnFormInput"  TextMode="MultiLine" Rows="5"></asp:TextBox>
        </div>
        <div class="dnnFormItem">
            <dnn:label id="plAddPage" cssClass="dnnFormLabel" controlname="chkAddPage" runat="server" />
            <asp:CheckBox ID="chkAddPage" cssClass="dnnFormInput" runat="server" />
        </div>
    </fieldset>
    <ul class="dnnActions dnnClear">
        <li><asp:linkbutton id="cmdCreate" resourcekey="cmdCreate" runat="server" cssclass="PrimaryAction"/></li>
        <li><asp:linkbutton id="cmdCancel" resourcekey="cmdCancel" runat="server" cssclass="SecondaryAction" CausesValidation="false" /></li>
    </ul>
</div>
<table class="Settings" cellspacing="2" cellpadding="2" summary="Packages Install Design Table">
	<tr><td><asp:PlaceHolder ID="phInstallLogs" runat="server" /></td></tr>
</table>