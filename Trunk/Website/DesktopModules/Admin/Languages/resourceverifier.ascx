<%@ Control Language="vb" AutoEventWireup="false" Inherits="DotNetNuke.Modules.Admin.Languages.ResourceVerifier" CodeFile="ResourceVerifier.ascx.vb" %>
<%@ Register TagPrefix="dnntv" Namespace="DotNetNuke.UI.WebControls" Assembly="DotNetNuke.WebControls" %>
<%@ Register TagPrefix="dnn" TagName="SectionHead" Src="~/controls/SectionHeadControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<P><asp:linkbutton id="cmdVerify" runat="server" CssClass="CommandButton" resourcekey="cmdVerify">Verify Resource Files</asp:linkbutton>&nbsp;
	<asp:LinkButton id="cmdCancel" runat="server" CssClass="CommandButton" resourcekey="cmdCancel">Cancel</asp:LinkButton></P>
<P><asp:placeholder id="PlaceHolder1" runat="server"></asp:placeholder></P>
