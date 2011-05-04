<%@ Control Language="C#" AutoEventWireup="false" Inherits="DotNetNuke.Modules.Admin.Languages.LanguageEditor" CodeFile="LanguageEditor.ascx.cs" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.UI.WebControls" Assembly="DotNetNuke" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.UI.WebControls" Assembly="DotNetNuke.Web" %>

<div class="dnnForm dnnLanguageEditor dnnClear" id="dnnLanguageEditor">
    <div class="dnnTree">
        <div class="dnnFormItem">
            <dnn:Label ID="plResources" runat="server" ControlName="DNNTree" />
            <br />
            <dnn:DnnTreeView ID="resourceFiles" runat="server">
            </dnn:DnnTreeView>
        </div>
    </div>
    <div class="dnnEditor">
        <fieldset>
            <legend></legend>
            <div id="rowMode" runat="server" class="dnnFormItem">
                <dnn:Label ID="plMode" runat="server" Text="Available Locales" ControlName="cboLocales" />
                <asp:RadioButtonList ID="rbMode" runat="server" CssClass="Normal" AutoPostBack="True"
                    RepeatColumns="3" RepeatDirection="Horizontal">
                    <asp:ListItem resourcekey="ModeSystem" Value="System" Selected="True" />
                    <asp:ListItem resourcekey="ModeHost" Value="Host" />
                    <asp:ListItem resourcekey="ModePortal" Value="Portal" />
                </asp:RadioButtonList>
            </div>
            <div class="dnnFormItem">
                <dnn:Label ID="highlightLabel" runat="server" ControlName="lblEditingLanguage" />
                <asp:CheckBox ID="chkHighlight" runat="server" AutoPostBack="True" />
             </div>
            <div class="dnnFormItem">
                <dnn:Label ID="plEditingLanguage" runat="server" ControlName="lblEditingLanguage" />
                <dnn:DnnLanguageLabel ID="languageLabel" runat="server" />
            </div>
            <div class="dnnFormItem">
                <dnn:Label ID="plFolder" runat="server" ControlName="lblFolder" />
                <asp:Label ID="lblFolder" runat="server" />
            </div>
            <div class="dnnFormItem">
                <dnn:Label ID="plSelected" runat="server" ControlName="lblResourceFile" />
                <asp:Label ID="lblResourceFile" runat="server" CssClass="NormalBold" />
            </div>
            <div class="dnnFormItem">
                <dnn:DnnGrid ID="resourcesGrid" runat="server" AutoGenerateColumns="false" Width="620px">
                    <MasterTableView>
                        <ItemStyle VerticalAlign="Top" HorizontalAlign="Center" Width="600px" />
                        <AlternatingItemStyle  VerticalAlign="Top" HorizontalAlign="Center" Width="600px" />
                        <HeaderStyle VerticalAlign="Bottom" HorizontalAlign="Left" Wrap="false" Width="600px" />
                        <Columns>
                            <dnn:DnnGridTemplateColumn>
                                <HeaderTemplate>
                                    <table cellpadding="0" class="DnnGridNestedTable">
                                        <tr>
                                            <td style="width: 300px;">
                                                <asp:Label ID="Label5" runat="server" CssClass="NormalBold" resourcekey="DefaultValue" />
                                            </td>
                                            <td style="width: 300px;">
                                                <asp:Label ID="Label4" runat="server" CssClass="NormalBold" resourcekey="Value" />
                                            </td>
                                        </tr>
                                    </table>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <table cellpadding="0" class="DnnGridNestedTable">
                                        <tr>
                                            <td colspan="2" style="text-align: left;">
                                                <asp:Label ID="resourceKeyLabel" runat="server"
                                                        CssClass="NormalBold"
                                                        resourcekey="ResourceName" />
                                                <asp:Label ID="resourceKey" runat="server"
                                                        CssClass="Normal"
                                                        Text='<%# Eval("key") %>' />
                                            </td>
                                        </tr>
                                        <tr style="vertical-align: top;">
                                            <td style="width: 300px;">
                                                <asp:TextBox ID="txtDefault" runat="server" Width="270px" Enabled="false" />
                                            </td>
                                            <td style="width: 300px;">
                                                <span style="white-space:nowrap;">
                                                    <asp:TextBox ID="txtValue" runat="server" Width="250px" />
                                                    <asp:HyperLink ID="lnkEdit" runat="server" CssClass="CommandButton" NavigateUrl='<%# OpenFullEditor(Eval("key").ToString()) %>'>
                                                        <asp:Image runat="server" AlternateText="Edit" ID="imgEdit" ImageUrl="~/images/edit.gif"
                                                            resourcekey="cmdEdit" Style="vertical-align: top"></asp:Image>
                                                    </asp:HyperLink>
                                                </span>
                                            </td>
                                        </tr>
                                    </table>
                                </ItemTemplate>
                            </dnn:DnnGridTemplateColumn>
                        </Columns>
                    </MasterTableView>
                    <PagerStyle Mode="NextPrevAndNumeric" />
                </dnn:DnnGrid>
            </div>
        </fieldset>
        <ul class="dnnActions dnnClear">
    	    <li><asp:LinkButton id="cmdUpdate" runat="server" CssClass="dnnPrimaryAction" resourcekey="cmdUpdate"/></li>
    	    <li><asp:LinkButton id="cmdDelete" runat="server" CssClass="dnnSecondaryAction" ResourceKey="cmdDelete" CausesValidation="false"/></li>
    	    <li><asp:LinkButton id="cmdCancel" runat="server" CssClass="dnnSecondaryAction" ResourceKey="cmdCancel" CausesValidation="false" /></li>
        </ul>    
    </div>
</div>
