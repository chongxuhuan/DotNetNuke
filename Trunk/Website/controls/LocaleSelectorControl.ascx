<%@ Control language="vb" AutoEventWireup="false" Explicit="True" Inherits="DotNetNuke.UI.UserControls.LocaleSelectorControl" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.UI.WebControls" Assembly="DotNetNuke" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%--<dnn:LanguageSelector runat="server" ID="selLanguage" SelectionMode="Single" ListDirection="Vertical" ItemStyle="FlagAndCaption" />
<asp:Button ID="btnLoadLocale" runat=server Text="load locale" />
<dnn:Label ID="lblDefaults" runat="server" CssClass="Head" --%>/>
<div>
    <asp:RadioButtonList ID="rbViewType" runat="server" RepeatLayout="Flow" RepeatDirection="Horizontal"
        AutoPostBack="True">
        <asp:ListItem Text="Native Name" Value="NATIVE" resourcekey="NativeName"></asp:ListItem>
        <asp:ListItem Text="English Name" Value="ENGLISH" resourcekey="EnglishName"></asp:ListItem>
    </asp:RadioButtonList>
</div>
<br />
<br /><div>
    <asp:DropDownList ID="ddlPortalDefaultLanguage" runat="server" CssClass="NormalTextBox le_languages"
        AutoPostBack="true" />
</div>
<asp:Literal ID="litStatus" runat="server" />


