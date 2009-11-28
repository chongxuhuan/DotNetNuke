<%@ Control CodeFile="Extensions.ascx.vb" Language="vb" AutoEventWireup="false" Explicit="True" Inherits="DotNetNuke.Modules.Admin.Extensions.Extensions" %>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke" Namespace="DotNetNuke.UI.WebControls"%>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke.WebControls" Namespace="DotNetNuke.UI.WebControls"%>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>                

<table width="100%" cellpadding="0" cellspacing="0">
    <tr>
        <td colspan="2" align="left">
            <asp:Label ID="lblUpdate" runat="server" CssClass="Normal" resourceKey="lblUpdate" />
        </td> 
    </tr>
    <tr><td style="height:10px;">&nbsp;</td></tr>
    <tr>
        <td colspan="2">
			<table>
                <tr id="trPackageType" runat="server">
                    <td valign="top" class="SubHead" style="width:150px"><dnn:Label ID="plPackageTypes" runat="server" ControlName="cboPackageTypes" /></td>
                    <td valign="top">
                        <asp:DropDownList ID="cboPackageTypes" runat="server" DataTextField="Description" Width="200px" DataValueField="PackageType" AutoPostBack="true" CssClass="NormalTextBox" />
                    </td>
                </tr>
                <tr><td>&nbsp;</td></tr>
			    <tr id="trLanguageSelector" runat="server">
                    <td valign="top" class="SubHead" style="width:150px"><dnn:Label ID="plLocales" runat="server" ControlName="cboLocales" /></td>
                    <td valign="top">
                        <asp:DropDownList ID="cboLocales" runat="server" Width="200px" DataTextField="Text" DataValueField="Code" AutoPostBack="true" CssClass="NormalTextBox" />
                    </td>
                </tr>
            </table>
            <br />
            <asp:Label ID="lblNoResults" runat="server" CssClass="Normal" resourcekey="NoResults" Visible="false" />
            <asp:DataGrid ID="grdExtensions" BorderWidth="0" BorderStyle="None" CellPadding="4"
                CellSpacing="0" AutoGenerateColumns="false" runat="server"
                GridLines="None" Width="100%" style="border:solid 1px #ececec; margin-bottom:10px;">
                <HeaderStyle Wrap="False" CssClass="NormalBold" BackColor="#f1f6f9" Height="25px" />
                <ItemStyle CssClass="Normal" VerticalAlign="Top" />
                <Columns>
                    <dnn:imagecommandcolumn headerStyle-width="18px" CommandName="Edit" ImageUrl="~/images/edit.gif" EditMode="URL" KeyField="PackageID" />
                    <dnn:imagecommandcolumn headerStyle-width="18px" commandname="Delete" imageurl="~/images/delete.gif" EditMode="URL" keyfield="PackageID" />
                    <dnn:textcolumn headerStyle-width="150px" DataField="FriendlyName" HeaderText="Name" />
                    <asp:TemplateColumn HeaderText="Type">
                        <HeaderStyle HorizontalAlign="Left" Wrap="False" Width="50px"/>
                        <ItemStyle HorizontalAlign="Left"/>
                        <ItemTemplate>
                            <asp:Label ID="lblType" runat="server" Text='<%# GetPackageTypeDescription(DataBinder.Eval(Container.DataItem, "PackageType")) %>' />
                        </ItemTemplate>
                    </asp:TemplateColumn>
                    <asp:TemplateColumn FooterText="Portal">
                        <HeaderStyle HorizontalAlign="Left" Wrap="False" Width="18px"/>
                        <ItemStyle HorizontalAlign="Left"/>
                        <ItemTemplate>
							<asp:Image ID="imgAbout" runat="server" ToolTip='<%# GetAboutTooltip(Container.DataItem) %>' ImageUrl="~/images/about.gif" Visible='<%# (DataBinder.Eval(Container.DataItem, "PackageType") = "Skin" OR DataBinder.Eval(Container.DataItem, "PackageType") = "Container") %>' />
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
                            <%#GetIsPackageInUseInfo(Container.DataItem)%>
                        </ItemTemplate>
                    </asp:TemplateColumn>
                    <asp:TemplateColumn HeaderText="Upgrade" >
                        <HeaderStyle HorizontalAlign="Left" Wrap="False" Width="120px"/>
                        <ItemStyle HorizontalAlign="Left"/>
                        <ItemTemplate>
                                <asp:Label ID="lblUpgrade" runat="server" Text='<%# UpgradeService(DataBinder.Eval(Container.DataItem,"Version"),DataBinder.Eval(Container.DataItem,"PackageType"),DataBinder.Eval(Container.DataItem,"Name")) %>' ></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateColumn>
                </Columns>
            </asp:DataGrid>
        </td>
    </tr>
</table>