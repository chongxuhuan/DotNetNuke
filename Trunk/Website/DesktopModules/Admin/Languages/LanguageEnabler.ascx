<%@ Control Language="vb" Inherits="DotNetNuke.Modules.Admin.Languages.LanguageEnabler"
    AutoEventWireup="false" Explicit="True" CodeFile="LanguageEnabler.ascx.vb" %>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke" Namespace="DotNetNuke.UI.WebControls" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<div class="le_label subHead">
    <dnn:Label ID="lblViewType" runat="server" CssClass="Head" />
</div>
<div>
    <asp:RadioButtonList ID="rbViewType" runat="server" RepeatLayout="Flow" RepeatDirection="Horizontal"
        AutoPostBack="True">
        <asp:ListItem Text="Native Name" Value="NATIVE" resourcekey="NativeName"></asp:ListItem>
        <asp:ListItem Text="English Name" Value="ENGLISH" resourcekey="EnglishName"></asp:ListItem>
    </asp:RadioButtonList>
</div>
<div class="le_newline">
</div>
<div class="le_label">
    <dnn:Label ID="lblSiteDefault" runat="server" CssClass="Head" />
</div>
<div>
    <asp:DropDownList ID="ddlPortalDefaultLanguage" runat="server" CssClass="NormalTextBox le_languages"
        AutoPostBack="true" />
</div>
<br />
<br />
<asp:DataGrid ID="dgLanguages" runat="server" AutoGenerateColumns="false" GridLines="None" BorderStyle="None" BorderWidth="0" CellPadding="10">
 <headerstyle cssclass="DataGrid_Header" verticalalign="Top" horizontalalign="Center"/>
	<itemstyle CssClass="DataGrid_Item" horizontalalign="Center" />
	<alternatingitemstyle cssclass="DataGrid_AlternatingItem" />
	<footerstyle cssclass="DataGrid_Footer" />
	<pagerstyle cssclass="DataGrid_Pager" />
    <Columns>
        <asp:TemplateColumn Visible="false">
            <ItemTemplate>
                <asp:Label ID="lblLanguageCode" runat="server" Text='<%# bind("Code") %>' />
            </ItemTemplate>
        </asp:TemplateColumn>
        <asp:TemplateColumn>
            <HeaderTemplate>
                <dnn:Label ID="plLanguage" runat="server" cssclass="Head" /></HeaderTemplate>
            <ItemTemplate>
                <asp:Label ID="lblLanguageName" runat="server" Text='<%# GetLanguageName(eval("Code")) %>' />
            </ItemTemplate>
        </asp:TemplateColumn>
        <asp:TemplateColumn>
            <HeaderTemplate>
                <asp:Label ID="lblFallback" runat="server" resourcekey="Fallback" Text="Fallback"
                    Font-Bold="True" /></HeaderTemplate>
            <ItemTemplate>
                <asp:DropDownList ID="ddlFallback" runat="server" CssClass="NormalTextBox le_languages"
                    OnSelectedIndexChanged="ddlFallback_SelectedIndexChanged" AutoPostBack="true" />
            </ItemTemplate>
        </asp:TemplateColumn>
        <asp:TemplateColumn>
            <HeaderTemplate>
                 <dnn:Label ID="plEnabled" runat="server" cssclass="Head" /></HeaderTemplate>
           <ItemTemplate>
                <asp:CheckBox ID="chkEnabled" runat="server" Enabled='<%# GridviewEditMode(eval("Code")) %>'
                    Checked='<%# isLanguageEnabled(eval("Code"),True, True)%>' AutoPostBack="True"
                    OnCheckedChanged="chkEnabled_CheckedChanged" />
                 <asp:Label ID="lblLanguageHint" runat="server" Text='<%# GetLanguageHint(eval("Code")) %>' />
           </ItemTemplate>
        </asp:TemplateColumn>
         <asp:TemplateColumn>
           <HeaderTemplate>
                <dnn:Label ID="plPortal" runat="server" cssclass="Head" /></HeaderTemplate>
            <ItemTemplate>
                <asp:ImageButton ID="cmdEditPortal" runat="server" ImageUrl="~/images/edit.gif" resourcekey="plPortal.Help"
                    CommandArgument='<%# bind("Code") %>' CommandName="Portal" OnClick="cmdEdit_Click" />
            </ItemTemplate>
        </asp:TemplateColumn>
        <asp:TemplateColumn>
           <HeaderTemplate>
                <dnn:Label ID="plHost" runat="server" cssclass="Head" /></HeaderTemplate>
            <ItemTemplate>
                <asp:ImageButton ID="cmdEditHost" runat="server" ImageUrl="~/images/edit.gif" resourcekey="plHost.Help"
                    CommandArgument='<%# bind("Code") %>' CommandName="Host" OnClick="cmdEdit_Click" />
            </ItemTemplate>
        </asp:TemplateColumn>
                <asp:TemplateColumn>
           <HeaderTemplate>
                <dnn:Label ID="plSystem" runat="server" cssclass="Head" /></HeaderTemplate>
            <ItemTemplate>
                <asp:ImageButton ID="cmdEditSystem" runat="server" ImageUrl="~/images/edit.gif" resourcekey="plSystem.Help"
                    CommandArgument='<%# bind("Code") %>' CommandName="System" OnClick="cmdEdit_Click" />
            </ItemTemplate>
        </asp:TemplateColumn>
            </Columns>
</asp:DataGrid>
<asp:Literal ID="litStatus" runat="server" />
<div>
<br />
    <dnn:CommandButton ID="cmdUpdate" runat="server" ImageUrl="~/images/save.gif" ResourceKey="cmdUpdate" CssClass="CommandButton" />
    &nbsp;&nbsp;<dnn:CommandButton ID="cmdCancel" runat="server" ImageUrl="~/images/lt.gif"
        ResourceKey="cmdCancel" CssClass="CommandButton" CausesValidation="false"  />
</div>
<br />
<br />
