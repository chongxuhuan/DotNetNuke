<%@ Control Language="C#" AutoEventWireup="false" Inherits="DotNetNuke.Modules.Admin.Users.ManageUsers" CodeFile="ManageUsers.ascx.cs" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="SectionHead" Src="~/controls/SectionHeadControl.ascx" %>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke" Namespace="DotNetNuke.UI.WebControls"%>
<%@ Register TagPrefix="dnn" TagName="Membership" Src="~/DesktopModules/Admin/Security/Membership.ascx" %>
<%@ Register TagPrefix="dnn" TagName="User" Src="~/DesktopModules/Admin/Security/User.ascx" %>
<%@ Register TagPrefix="dnn" TagName="Roles" Src="~/DesktopModules/Admin/Security/SecurityRoles.ascx" %>
<%@ Register TagPrefix="dnn" TagName="Password" Src="~/DesktopModules/Admin/Security/Password.ascx" %>
<%@ Register TagPrefix="dnn" TagName="Profile" Src="~/DesktopModules/Admin/Security/Profile.ascx" %>
<%@ Register TagPrefix="dnn" TagName="MemberServices" Src="~/DesktopModules/Admin/Security/MemberServices.ascx" %>

<script language="javascript" type="text/javascript">
    function setUpDnnManageUsers() {
        $('#dnnManageUsers').dnnTabs().dnnPanels();
    }

    $(document).ready(function () {
        setUpDnnManageUsers();
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
            setUpDnnManageUsers();
        });
    });
</script>

<div class="dnnForm dnnManageUsers dnnClear" id="dnnManageUsers">
    <ul class="dnnAdminTabNav dnnClear" id="adminTabNav" runat="server">
        <li><a href="#dnnUserDetails"><%=LocalizeString("cmdUser")%></a></li>
        <li id="rolesTab" runat="server"><a href="#<%=dnnRoleDetails.ClientID%>"><%=LocalizeString("cmdRoles")%></a></li>
        <li id="passwordTab" runat="server"><a href="#<%=dnnPasswordDetails.ClientID%>"><%=LocalizeString("cmdPassword")%></a></li>
        <li><a href="#<%=dnnProfileDetails.ClientID%>"><%=LocalizeString("cmdProfile")%></a></li>
        <li id="servicesTab" runat="server"><a href="#<%=dnnServicesDetails.ClientID%>"><%=LocalizeString("cmdServices")%></a></li>
    </ul>
    <div id="dnnUserDetails" class="dnnUserDetails dnnClear">
        <div class="udContent dnnClear">
            <div class="dnnFormItem dnnFormHelp dnnClear"><p class="dnnFormRequired"><span><%=LocalizeString("RequiredFields")%></span></p></div>
            <fieldset>
                <div id="titleRow" runat="server" class="dnnFormItem">
				    <asp:label id="lblTitle" cssclass="Head" runat="server"/>
				    <asp:image id="imgLockedOut" imageurl="~/images/icon_securityroles_16px.gif" runat="server" visible="False" />
				    <asp:image id="imgOnline" imageurl="~/images/userOnline.gif" runat="server" visible="False" />
                </div>
                <div id="helpRow" runat="server" visible="false" class="dnnFormItem">
                    <asp:label id="lblUserHelp" runat="server"/>
                </div>
                <div id="userRow" runat="server" class="dnnFormItem">
                    <div class="dnnUser">
                        <dnn:user id="ctlUser" runat="Server" />
                    </div>
                    <div id="membershipRow" runat="server" class="dnnMembership">
                        <dnn:membership id="ctlMembership" runat="Server" />
                    </div>
                </div>
                <div id="captchaRow" runat="server" visible="false">
                    <dnn:label id="plCaptcha" controlname="ctlCaptcha" runat="server" />
                    <dnn:captchacontrol id="ctlCaptcha" captchawidth="130" captchaheight="40" ErrorStyle-CssClass="NormalRed" runat="server" />
                </div>                       
            </fieldset>
        </div>
    </div>
    <div id="dnnRoleDetails" runat="server" class="dnnRoleDetails dnnClear">
 	    <dnn:roles id="ctlRoles" runat="server"></dnn:roles>
    </div>
    <div id="dnnPasswordDetails" runat="server" class="dnnPasswordDetails dnnClear">
    	<dnn:Password id="ctlPassword" runat="server"></dnn:Password>
    </div>
    <div id="dnnProfileDetails" runat="server" class="dnnProfileDetails dnnClear">
    	<dnn:Profile id="ctlProfile" runat="server"></dnn:Profile>
    </div>
    <div id="dnnServicesDetails" runat="server" visible="false" class="dnnServicesDetails dnnClear">
    	<dnn:MemberServices id="ctlServices" runat="server"></dnn:MemberServices>
    </div>
</div>
<ul id="actionsRow" runat="server" visible="false" class="dnnActions dnnClear">
    <li><asp:LinkButton id="cmdRegister" runat="server" CssClass="dnnPrimaryAction" resourcekey="cmdRegister" /></li>
    <li><asp:LinkButton id="cmdCancel" runat="server" CssClass="dnnSecondaryAction" resourcekey="cmdCancel" CausesValidation="false" /></li>
    <li><asp:LinkButton id="cmdLogin" runat="server" CssClass="dnnSecondaryAction" resourcekey="cmdLogin" Visible="false" /></li>
</ul>  