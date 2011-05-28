<%@ Control Language="C#" AutoEventWireup="false" Inherits="DotNetNuke.Modules.Admin.Extensions.AvailableExtensions" CodeFile="AvailableExtensions.ascx.cs" %>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke" Namespace="DotNetNuke.UI.WebControls"%>
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
		                <asp:TemplateColumn>
                            <ItemStyle HorizontalAlign="Center" Width="32px" Height="32px"/>
                            <ItemTemplate>
                                <asp:Image ID="imgIcon" runat="server" Width="32px" Height="32px" ImageUrl='<%# GetPackageIcon(Container.DataItem) %>' ToolTip='<%# GetPackageDescription(Container.DataItem) %>' />
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
            </fieldset>
        </div>
    </ItemTemplate>
</asp:Repeater>
<p><dnn:commandbutton id="cmdInstall" runat="server" CssClass="CommandButton" ImageUrl="~/images/save.gif" ResourceKey="cmdInstall" /></p>
<asp:Button ID="btnSnow" runat="server" Text="Snowcovered records" onclick="BtnSnowClick" />
<asp:Label ID="lblMessage" runat="server" Text=""></asp:Label>
<asp:GridView ID="grdSnow" runat="server"></asp:GridView>