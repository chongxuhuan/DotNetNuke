<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Register.ascx.cs" Inherits="DotNetNuke.Modules.Admin.Users.Register" %>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke.Web" Namespace="DotNetNuke.Web.UI.WebControls" %>

<div class="dnnFormItem">
    <asp:label id="userHelpLabel" runat="server" />
</div>
<br/>
<div class="dnnFormItem dnnFormHelp dnnClear"><p class="dnnFormRequired"><span><%=LocalizeString("RequiredFields")%></span></p></div>
<div class="dnnRegistrationForm">
    <dnn:DnnFormEditor id="userForm" runat="Server" FormMode="Short">
        <Items>
            <dnn:DnnFormTextBoxItem ID="userName" runat="server" DataField="Username" Required="true"/>
            <dnn:DnnFormTextBoxItem ID="email" runat="server" DataField="Email" Required="true" />
            <dnn:DnnFormTextBoxItem ID="password" runat="server" TextMode="Password" DataMember="Membership" DataField="Password" Required="true" AutoCompleteType="Disabled"/>
            <dnn:DnnFormTextBoxItem ID="passwordConfirm" runat="server" TextMode="Password" DataMember="Membership" DataField="PasswordConfirm" Required="true" AutoCompleteType="Disabled" />
            <dnn:DnnFormTextBoxItem ID="passwordQuestion" runat="server" DataMember="Membership" DataField="PasswordQuestion" />
            <dnn:DnnFormTextBoxItem ID="passwordAnswer" runat="server" DataMember="Membership" DataField="PasswordAnswer" />
            <dnn:DnnFormTextBoxItem ID="displayName" runat="server" DataField="DisplayName" Visible="True" />
       </Items>
    </dnn:DnnFormEditor>
    <div class="dnnSocialRegistration">
        <asp:PlaceHolder ID="socialLoginControls" runat="server"/>
    </div>
</div>
<div class="dnnForm">
    <ul id="actionsRow" runat="server" class="dnnActions dnnClear">
        <li><asp:LinkButton id="registerButton" runat="server" CssClass="dnnPrimaryAction" resourcekey="cmdRegister" /></li>
        <li><asp:LinkButton id="cancelButton" runat="server" CssClass="dnnSecondaryAction" resourcekey="cmdCancel" CausesValidation="false" /></li>
    </ul>
</div>	    