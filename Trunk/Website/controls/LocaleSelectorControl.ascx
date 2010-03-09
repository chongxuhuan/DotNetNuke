<%@ Control language="vb" AutoEventWireup="false" Explicit="True" Inherits="DotNetNuke.UI.UserControls.LocaleSelectorControl" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.UI.WebControls" Assembly="DotNetNuke" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<div>
    <asp:RadioButtonList ID="rbViewType" runat="server" RepeatLayout="Flow" RepeatDirection="Horizontal" AutoPostBack="True"/>
</div>
<br />
<div>
    <asp:DropDownList ID="ddlPortalDefaultLanguage" runat="server" CssClass="NormalTextBox le_languages"
        AutoPostBack="true" />
</div>
<asp:Literal ID="litStatus" runat="server" />


