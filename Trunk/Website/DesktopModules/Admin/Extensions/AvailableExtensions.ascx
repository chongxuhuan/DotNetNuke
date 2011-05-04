<%@ Control Language="C#" AutoEventWireup="false" Inherits="DotNetNuke.Modules.Admin.Extensions.AvailableExtensions" CodeFile="AvailableExtensions.ascx.cs" %>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke" Namespace="DotNetNuke.UI.WebControls"%>

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
					        <asp:TemplateColumn>
                                <ItemStyle HorizontalAlign="Center" Width="48px" Height="48px"/>
                                <ItemTemplate>
                                    <asp:Image ID="imgIcon" runat="server" Width="48px" Height="48px" ImageUrl='<%# GetPackageIcon(Container.DataItem) %>' ToolTip='<%# GetPackageDescription(Container.DataItem) %>' />
                                </ItemTemplate>
                            </asp:TemplateColumn>
                            <dnn:textcolumn headerStyle-width="150px" DataField="FriendlyName" HeaderText="Name" />
                            <dnn:textcolumn headerStyle-width="275px" ItemStyle-HorizontalAlign="Left" DataField="Description" HeaderText="Description" />
                            <asp:TemplateColumn HeaderText="Version" >
                                <HeaderStyle HorizontalAlign="Left" Wrap="False" Width="40px"/>
                                <ItemStyle HorizontalAlign="Left"/>
                                <ItemTemplate>
                                    <asp:Label ID="lblVersion" runat="server" Text='<%# FormatVersion(Container.DataItem) %>' />
                                </ItemTemplate>
                            </asp:TemplateColumn>
                        </Columns>
                    </asp:DataGrid>
                </div> 
            </fieldset>
        </div>
    </ItemTemplate>
</asp:Repeater>

<p>
	<dnn:commandbutton id="cmdInstall" runat="server" CssClass="CommandButton" ImageUrl="~/images/save.gif" ResourceKey="cmdInstall" />
</p>
