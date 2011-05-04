<%@ Control Language="C#" AutoEventWireup="false" Inherits="DotNetNuke.Modules.Admin.Extensions.InstalledExtensions" CodeFile="InstalledExtensions.ascx.cs" %>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke" Namespace="DotNetNuke.UI.WebControls"%>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke.WebControls" Namespace="DotNetNuke.UI.WebControls"%>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>                

<div class="dnnFormItem">
    <asp:Label ID="lblUpdate" runat="server" CssClass="Normal" resourceKey="lblUpdate" />
</div>
<div class="dnnFormItem" id="languageSelectorRow" runat="server">
    <dnn:Label ID="plLocales" CssClass="dnnFormLabel" runat="server" ControlName="cboLocales" />
    <asp:DropDownList ID="cboLocales" runat="server" Width="200px" DataTextField="Text" DataValueField="Code" AutoPostBack="true" CssClass="dnnFormInput" />
</div>
<asp:Repeater ID="extensionTypeRepeater" runat="server">
    <ItemTemplate>
        <div class="exieContent dnnClear">
            <h2 id="Panel-<%# GetPackageType(Container.DataItem) %>" class="dnnFormSectionHead"><a href="" class=""><%# GetPackageType(Container.DataItem) %></a></h2>
            <fieldset>
                <legend></legend>
                    <div class="dnnFormItem">
                    <asp:DataGrid ID="extensionsGrid" CellPadding="4" CellSpacing="0" AutoGenerateColumns="false" runat="server"
                        GridLines="None" Width="100%" style="border:solid 1px #ececec; margin-bottom:10px;" >
                        <HeaderStyle Wrap="False" CssClass="NormalBold" BackColor="#f1f6f9" Height="25px" />
                        <ItemStyle CssClass="Normal" VerticalAlign="Top" />
                        <Columns>
                            <dnn:imagecommandcolumn headerStyle-width="18px" CommandName="Edit" ImageUrl="~/images/edit.gif" EditMode="URL" KeyField="PackageID" />
                            <dnn:imagecommandcolumn headerStyle-width="18px" commandname="Delete" imageurl="~/images/delete.gif" EditMode="URL" keyfield="PackageID" />
					        <asp:TemplateColumn>
                                <ItemStyle HorizontalAlign="Center" Width="48px" Height="48px"/>
                                <ItemTemplate>
                                    <asp:Image ID="imgIcon" runat="server" Width="48px" Height="48px" ImageUrl='<%# GetPackageIcon(Container.DataItem) %>' ToolTip='<%# GetPackageDescription(Container.DataItem) %>' />
                                </ItemTemplate>
                            </asp:TemplateColumn>
                            <dnn:textcolumn headerStyle-width="150px" DataField="FriendlyName" HeaderText="Name" />
                            <asp:TemplateColumn FooterText="Portal">
                                <HeaderStyle HorizontalAlign="Left" Wrap="False" Width="18px"/>
                                <ItemStyle HorizontalAlign="Left"/>
                                <ItemTemplate>
							        <asp:Image ID="imgAbout" runat="server" ToolTip='<%# GetAboutTooltip(Container.DataItem) %>' ImageUrl="~/images/about.gif" Visible='<%# ((String)(DataBinder.Eval(Container.DataItem, "PackageType")) == "Skin" || ((String)DataBinder.Eval(Container.DataItem, "PackageType")) == "Container") %>' />
                                </ItemTemplate>
                            </asp:TemplateColumn>
                            <dnn:textcolumn headerStyle-width="275px" ItemStyle-HorizontalAlign="Left" DataField="Description" HeaderText="Description" />
                            <asp:TemplateColumn HeaderText="Version" >
                                <HeaderStyle HorizontalAlign="Left" Wrap="False" Width="40px"/>
                                <ItemStyle HorizontalAlign="Left"/>
                                <ItemTemplate>
                                    <asp:Label ID="lblVersion" runat="server" Text='<%# FormatVersion(Container.DataItem) %>' />
                                </ItemTemplate>
                            </asp:TemplateColumn>
                            <asp:TemplateColumn HeaderText="In Use">
                                <HeaderStyle HorizontalAlign="Left" Wrap="False" />
                                <ItemStyle HorizontalAlign="Left"/>
                                <ItemTemplate>
                                    <asp:Label ID="lblUserInfo" runat="server" Text='<%# GetIsPackageInUseInfo(Container.DataItem) %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateColumn>
                            <asp:TemplateColumn HeaderText="Upgrade" >
                                <HeaderStyle HorizontalAlign="Left" Wrap="False" Width="120px"/>
                                <ItemStyle HorizontalAlign="Left"/>
                                <ItemTemplate>
                                        <asp:Label ID="lblUpgrade" runat="server" Text='<%# UpgradeService((Version)DataBinder.Eval(Container.DataItem,"Version"),DataBinder.Eval(Container.DataItem,"PackageType").ToString(),DataBinder.Eval(Container.DataItem,"Name").ToString()) %>' ></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateColumn>
                        </Columns>
                    </asp:DataGrid>
                    <asp:Label ID="noResultsLabel" runat="server" CssClass="Normal" resourcekey="NoResults" Visible="false" />
                </div>            
            </fieldset>
        </div>
    </ItemTemplate>
</asp:Repeater>
