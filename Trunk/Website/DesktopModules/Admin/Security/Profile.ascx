<%@ Control Language="C#" AutoEventWireup="false" Inherits="DotNetNuke.Modules.Admin.Users.DNNProfile" CodeFile="Profile.ascx.cs" %>
<%@ Register TagPrefix="dnn" TagName="Sectionhead" Src="~/controls/SectionHeadControl.ascx" %>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke" Namespace="DotNetNuke.UI.WebControls"%>
<div class="dnnForm dnnProfile dnnClear">
    <h2 id="divTitle" runat="server" class="dnnFormSectionHead"><asp:label id="lblTitle" runat="server" /></h2>
	<div class="propertyList">
		<dnn:ProfileEditorControl id="ProfileProperties" runat="Server" enableClientValidation="true" CssClass="dnnFormItem" />
	</div>
    <ul class="dnnActions dnnClear">
        <li><asp:LinkButton class="dnnPrimaryAction" id="cmdUpdate" runat="server" resourcekey="cmdUpdate" /></li>
    </ul>
</div>