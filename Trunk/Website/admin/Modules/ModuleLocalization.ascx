<%@ Control Language="c#" AutoEventWireup="false" Explicit="True" CodeFile="ModuleLocalization.ascx.cs" Inherits="DotNetNuke.Admin.Modules.ModuleLocalization" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.UI.WebControls" Assembly="DotNetNuke.Web" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.UI.WebControls" Assembly="DotNetNuke" %>
<dnn:DnnGrid ID="localizedModulesGrid" runat="server"  AutoGenerateColumns="false" AllowRowSelect="false" AllowMultiRowSelection="true">
    <MasterTableView DataKeyNames="ModuleId, TabId">
        <Columns>
            <dnn:DnnGridTemplateColumn UniqueName="CheckBoxTemplateColumn" HeaderStyle-Width="50px">
                <HeaderTemplate>
                    <asp:CheckBox ID="headerCheckBox" runat="server" OnCheckedChanged="ToggleSelectedState" Visible = '<%# ShowHeaderCheckBox() %>' AutoPostBack="True" />
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:CheckBox ID="rowCheckBox" runat="server" OnCheckedChanged="ToggleRowSelection" AutoPostBack="True" />
                </ItemTemplate>
            </dnn:DnnGridTemplateColumn> 
            <dnn:DnnGridTemplateColumn UniqueName="Language" HeaderText="Language" ItemStyle-VerticalAlign="Middle" ItemStyle-Width="200px">
                <ItemTemplate>
                    <%# Convert.ToBoolean(Eval("IsDefaultLanguage")) ? "" : "&nbsp;&nbsp;&nbsp;&nbsp;"%>
                    <dnn:DnnLanguageLabel ID="moduleLanguageLabel" runat="server" Language='<%# Eval("CultureCode") %>'  />
                </ItemTemplate>
            </dnn:DnnGridTemplateColumn>
            <dnn:DnnGridBoundColumn HeaderText="ModuleType" DataField="DesktopModule.FriendlyName" ItemStyle-Width="80px" ItemStyle-VerticalAlign="Middle" />
            <dnn:DnnGridBoundColumn HeaderText="ModuleTitle" DataField="ModuleTitle" ItemStyle-Width="200px" ItemStyle-VerticalAlign="Middle"  />
            <dnn:DnnGridTemplateColumn UniqueName="Edit" HeaderText="Edit"  ItemStyle-Width="40px" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" ItemStyle-VerticalAlign="Middle">
                <ItemTemplate>
                    <a href='<%# DotNetNuke.Common.Globals.NavigateURL(Convert.ToInt32(Eval("TabId")), DotNetNuke.Common.Utilities.Null.NullBoolean, PortalSettings, "Module", Eval("CultureCode").ToString(), "ModuleId=" + Eval("ModuleID")) %>' >
                        <asp:Image ID="editCultureImage" runat="server" ResourceKey="edit" ImageUrl="~/images/edit.gif" />
                    </a>
                </ItemTemplate>
            </dnn:DnnGridTemplateColumn>
            <dnn:DnnGridTemplateColumn UniqueName="IsLocalized" HeaderText="UnBound" ItemStyle-Width="100px" ItemStyle-HorizontalAlign="Center" ItemStyle-VerticalAlign="Middle">
                <ItemTemplate>
                    <asp:Label ID="defaultLocalizedLabel" runat="server" resourcekey="NA" Visible='<%# Eval("IsDefaultLanguage")%>' />
                    <asp:Image ID="localizedImage" runat="server" ImageUrl="~/images/grant.gif" Visible='<%# Convert.ToBoolean(Eval("IsLocalized")) && !Convert.ToBoolean(Eval("IsDefaultLanguage"))%>' />
                    <asp:Image ID="notLocalizedImage" runat="server" ImageUrl="~/images/deny.gif" Visible='<%# !Convert.ToBoolean(Eval("IsLocalized")) && !Convert.ToBoolean(Eval("IsDefaultLanguage"))%>' />
                </ItemTemplate>
            </dnn:DnnGridTemplateColumn>
                <dnn:DnnGridTemplateColumn UniqueName="IsTranslated" HeaderText="Translated" ItemStyle-Width="60px" ItemStyle-HorizontalAlign="Center" ItemStyle-VerticalAlign="Middle">
                <ItemTemplate>
                    <asp:Label ID="defaultTranslatedLabel" runat="server" resourcekey="NA" Visible='<%# Eval("IsDefaultLanguage")%>' />
                    <asp:Image ID="translatedImage" runat="server" ImageUrl="~/images/grant.gif" Visible='<%# Convert.ToBoolean(Eval("IsTranslated")) && !Convert.ToBoolean(Eval("IsDefaultLanguage"))%>' />
                    <asp:Image ID="notTranslatedImage" runat="server" ImageUrl="~/images/deny.gif" Visible='<%# !Convert.ToBoolean(Eval("IsTranslated")) && !Convert.ToBoolean(Eval("IsDefaultLanguage"))%>' />
                </ItemTemplate>
            </dnn:DnnGridTemplateColumn>
        </Columns>
    </MasterTableView>
</dnn:DnnGrid>
<asp:PlaceHolder ID="footerPlaceHolder" runat="server">
<ul class="dnnActions dnnClear">
	<li><asp:LinkButton ID="localizeModuleButton" resourcekey="unbindModule" runat="server" CssClass="dnnSecondaryAction" CausesValidation="False" /></li>
	<li><asp:LinkButton ID="delocalizeModuleButton" resourcekey="bindModule" runat="server" CssClass="dnnSecondaryAction" CausesValidation="False" /></li>
	<li><asp:LinkButton ID="markModuleTranslatedButton" resourcekey="markModuleTranslated" runat="server" CssClass="dnnSecondaryAction" CausesValidation="False" />
	<li><asp:LinkButton ID="markModuleUnTranslatedButton" resourcekey="markModuleUnTranslated" runat="server" CssClass="dnnSecondaryAction" CausesValidation="False" /></li>
</li>    
</asp:PlaceHolder>