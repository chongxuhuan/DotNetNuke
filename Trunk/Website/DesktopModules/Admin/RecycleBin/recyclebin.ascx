<%@ Control Language="vb" AutoEventWireup="false" Inherits="DotNetNuke.Modules.Admin.RecycleBin.RecycleBin"
    CodeFile="RecycleBin.ascx.vb" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="HelpButton" Src="~/controls/HelpButtonControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="SectionHead" Src="~/controls/SectionHeadControl.ascx" %>
<table class="Settings" cellspacing="2" cellpadding="2" summary="Recycle Bin Design Table" border="0">
    <tr>
        <td width="760">
            <asp:PlaceHolder ID="modePlaceHolder" runat="server" Visible = "false">
                <asp:RadioButtonList ID="modeButtonList" runat="server" RepeatDirection="Horizontal" AutoPostBack="true" CssClass="SubHead">
                    <asp:ListItem Value="ALL" Selected="True" resourcekey="allLocales" />
                    <asp:ListItem Value="SINGLE" resourcekey="singleLocale" />
                </asp:RadioButtonList>
                <br />
            </asp:PlaceHolder>
            <asp:Panel ID="pnlTabs" runat="server" CssClass="WorkPanel" Visible="True">
                <dnn:SectionHead ID="dshBasic" CssClass="Head" runat="server" IncludeRule="True" ResourceKey="Tabs" Section="tblTabs" Text="Tabs"/>
                <table id="tblTabs" cellspacing="0" cellpadding="2" width="525" summary="Tbas Design Table" border="0" runat="server">
                    <tr valign="top">
                        <td width="250"><asp:ListBox ID="lstTabs" runat="server" CssClass="Normal" Width="350px" Rows="5" DataTextField="TabName" DataValueField="TabId" SelectionMode="Multiple"/></td>
                        <td valign="top">
                            <table id="tblTabButtons" runat="server" summary="Tabs Design Table">
                                <tr>
                                    <td valign="top"><asp:ImageButton ID="cmdRestoreTab" runat="server" resourcekey="cmdRestoreTab" ImageUrl="~/images/restore.gif" /></td>
                                    <td valign="top"><dnn:HelpButton ID="hbtnRestoreTabHelp" ResourceKey="cmdRestoreTab" runat="server" /></td>
                                </tr>
                                <tr>
                                    <td valign="top"><asp:ImageButton ID="cmdDeleteTab" runat="server" resourcekey="cmdDeleteTab" ImageUrl="~/images/delete.gif" /></td>
                                    <td valign="top"><dnn:HelpButton ID="hbtnDeleteTabHelp" ResourceKey="cmdDeleteTab" runat="server" /></td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
                <br />
                <dnn:SectionHead ID="Sectionhead1" CssClass="Head" runat="server" IncludeRule="True" ResourceKey="Modules" Section="tblModules" Text="Modules"></dnn:SectionHead>
                <table id="tblModules" cellspacing="0" cellpadding="0" width="525" summary="Basic Settings Design Table" border="0" runat="server">
                    <tr valign="Top">
                        <td width="250"><asp:ListBox ID="lstModules" runat="server" CssClass="Normal" Width="350px" Rows="5" SelectionMode="Multiple" /></td>
                        <td valign="top">
                            <table id="tblModuleButtons" runat="server" summary="Tabs Design Table">
                                <tr>
                                    <td valign="top"><asp:ImageButton ID="cmdRestoreModule" runat="server" AlternateText="Restore Module" ImageUrl="~/images/restore.gif"/></td>
                                    <td valign="top"><dnn:HelpButton ID="hbtnRestoreModuleHelp" ResourceKey="cmdRestoreModule" runat="server" /></td>
                                </tr>
                                <tr>
                                    <td valign="top"><asp:ImageButton ID="cmdDeleteModule" runat="server" AlternateText="Delete Module" ImageUrl="~/images/delete.gif"/></td>
                                    <td valign="top"><dnn:HelpButton ID="hbtnDeleteModuleHelp" ResourceKey="cmdDeleteModule" runat="server" /></td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </asp:Panel>
        </td>
        <td width="10">&nbsp;</td>
    </tr>
</table>
<p>
    <asp:LinkButton ID="cmdEmpty" resourcekey="cmdEmpty" CssClass="CommandButton" runat="server">Empty Recycle Bin</asp:LinkButton>
</p>
