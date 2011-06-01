<%@ Control Language="C#" Inherits="DotNetNuke.Modules.Admin.Authentication.Login" AutoEventWireup="false" CodeFile="Login.ascx.cs" %>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke" Namespace="DotNetNuke.UI.WebControls"%>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<div class="dnnForm dnnLoginService dnnClear">
    <div class="dnnFormItem">
        <dnn:label id="plUsername" controlname="txtUsername" runat="server" resourcekey="Username" />
        <asp:textbox id="txtUsername" runat="server" />
    </div>
    <div class="dnnFormItem">
        <dnn:label id="plPassword" controlname="txtPassword" runat="server" resourcekey="Password" />
        <asp:textbox id="txtPassword" textmode="Password" runat="server" />
    </div>
    <div class="dnnFormItem" id="divVerify1" runat="server" visible="false">
        <dnn:label id="plVerification" controlname="txtVerification" runat="server"/>
    </div>
    <div class="dnnFormItem" id="divVerify2" runat="server" visible="false">
        <asp:textbox id="txtVerification" runat="server" />
    </div>
    <div class="dnnFormItem" id="divCaptcha1" runat="server" visible="false">
        <dnn:label id="plCaptcha" controlname="ctlCaptcha" runat="server" resourcekey="Captcha" />
    </div>
    <div class="dnnFormItem" id="divCaptcha2" runat="server" visible="false">
        <dnn:captchacontrol id="ctlCaptcha" captchawidth="130" captchaheight="40" runat="server" errorstyle-cssclass="dnnFormMessage dnnFormError" />
    </div>
    <p><asp:LinkButton id="cmdLogin" resourcekey="cmdLogin" cssclass="dnnPrimaryAction" text="Login" runat="server" /></p>
</div>