<%@ Control Language="C#" Inherits="DotNetNuke.Modules.Admin.AppGallery.AppGallerySnowcovered" AutoEventWireup="false" CodeFile="AppGallerySnowcovered.ascx.cs" %>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke" Namespace="DotNetNuke.UI.WebControls"%>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<div class="dnnForm dnnAppgallerySnowcoveredService dnnClear">
    <div class="dnnFormItem">
        <asp:label id="plUsername" AssociatedControlID="txtUsername" runat="server" resourcekey="Username" CssClass="dnnFormLabel" />
        <asp:textbox id="txtUsername" runat="server" />
    </div>
    <div class="dnnFormItem">
        <asp:label id="plPassword" AssociatedControlID="txtPassword" runat="server" resourcekey="Password" CssClass="dnnFormLabel" />
        <asp:textbox id="txtPassword" textmode="Password" runat="server" />
    </div>
    <p><asp:LinkButton id="cmdSave" resourcekey="cmdSave" cssclass="dnnPrimaryAction" text="Save" runat="server" /></p>
</div>