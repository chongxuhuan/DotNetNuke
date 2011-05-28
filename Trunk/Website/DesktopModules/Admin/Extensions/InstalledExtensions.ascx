<%@ Control Language="C#" AutoEventWireup="false" Inherits="DotNetNuke.Modules.Admin.Extensions.InstalledExtensions" CodeFile="InstalledExtensions.ascx.cs" %>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke" Namespace="DotNetNuke.UI.WebControls"%>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke.WebControls" Namespace="DotNetNuke.UI.WebControls"%>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>                
<div class="dnnFormMessage dnnFormInfo"><asp:Label ID="lblUpdate" runat="server" resourceKey="lblUpdate" /></div>
<div class="dnnFormItem" id="languageSelectorRow" runat="server">
    <dnn:Label ID="plLocales" runat="server" ControlName="cboLocales" />
    <asp:DropDownList ID="cboLocales" runat="server" DataTextField="Text" DataValueField="Code" AutoPostBack="true" />
</div>
<asp:Repeater ID="extensionTypeRepeater" runat="server">
    <ItemTemplate>
        <div class="dnnForm exieContent dnnClear">
            <h2 id="Panel-<%# GetPackageType(Container.DataItem) %>" class="dnnFormSectionHead"><a href="" class=""><%# GetPackageType(Container.DataItem) %></a></h2>
            <fieldset>
                <asp:DataGrid ID="extensionsGrid" CellPadding="0" CellSpacing="0" AutoGenerateColumns="false" runat="server" GridLines="None" Width="100%" CssClass="dnnGrid">
                    <HeaderStyle Wrap="False" CssClass="dnnGridHeader" />
                    <ItemStyle CssClass="dnnGridItem" VerticalAlign="Top" />
                    <AlternatingItemStyle CssClass="dnnGridAltItem" />
                    <Columns>
                        <dnn:imagecommandcolumn headerStyle-width="18px" CommandName="Edit" ImageUrl="~/images/edit.gif" EditMode="URL" KeyField="PackageID" />
                        <dnn:imagecommandcolumn headerStyle-width="18px" commandname="Delete" imageurl="~/images/delete.gif" EditMode="URL" keyfield="PackageID" />
					    <asp:TemplateColumn>
                            <ItemStyle HorizontalAlign="Center" Width="32px" Height="32px"/>
                            <ItemTemplate><asp:Image ID="imgIcon" runat="server" Width="32px" Height="32px" ImageUrl='<%# GetPackageIcon(Container.DataItem) %>' ToolTip='<%# GetPackageDescription(Container.DataItem) %>' /></ItemTemplate>
                        </asp:TemplateColumn>
                        <dnn:textcolumn headerStyle-width="150px" DataField="FriendlyName" HeaderText="Name">
                            <itemstyle Font-Bold="true" />
                        </dnn:textcolumn>
                        <asp:TemplateColumn FooterText="Portal">
                            <HeaderStyle HorizontalAlign="Left" Wrap="False" Width="18px" />
                            <ItemStyle HorizontalAlign="Left" />
                            <ItemTemplate>
							    <asp:Image ID="imgAbout" runat="server" ToolTip='<%# GetAboutTooltip(Container.DataItem) %>' ImageUrl="~/images/about.gif" Visible='<%# ((String)(DataBinder.Eval(Container.DataItem, "PackageType")) == "Skin" || ((String)DataBinder.Eval(Container.DataItem, "PackageType")) == "Container") %>' />
                            </ItemTemplate>
                        </asp:TemplateColumn>
                        <dnn:textcolumn headerStyle-width="275px" ItemStyle-HorizontalAlign="Left" DataField="Description" HeaderText="Description" />
                        <asp:TemplateColumn HeaderText="Version">
                            <HeaderStyle HorizontalAlign="Left" Wrap="False" />
                            <ItemStyle HorizontalAlign="Left" />
                            <ItemTemplate><asp:Label ID="lblVersion" runat="server" Text='<%# FormatVersion(Container.DataItem) %>' /></ItemTemplate>
                        </asp:TemplateColumn>
                        <asp:TemplateColumn HeaderText="In Use">
                            <HeaderStyle HorizontalAlign="Left" Wrap="False" />
                            <ItemStyle HorizontalAlign="Left" />
                            <ItemTemplate>
                                <asp:Label ID="lblUserInfo" runat="server" Text='<%# GetIsPackageInUseInfo(Container.DataItem) %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateColumn>
                        <asp:TemplateColumn HeaderText="Upgrade" >
                            <HeaderStyle HorizontalAlign="Left" Wrap="False" Width="120px" />
                            <ItemStyle HorizontalAlign="Left" />
                            <ItemTemplate><asp:Label ID="lblUpgrade" runat="server" Text='<%# UpgradeService((Version)DataBinder.Eval(Container.DataItem,"Version"),DataBinder.Eval(Container.DataItem,"PackageType").ToString(),DataBinder.Eval(Container.DataItem,"Name").ToString()) %>' ></asp:Label></ItemTemplate>
                        </asp:TemplateColumn>
                    </Columns>
                </asp:DataGrid>
                <asp:Label ID="noResultsLabel" runat="server" resourcekey="NoResults" Visible="false" />
            </fieldset>
        </div>
    </ItemTemplate>
</asp:Repeater>