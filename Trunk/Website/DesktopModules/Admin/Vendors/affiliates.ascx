<%@ Control Language="C#" AutoEventWireup="false" Inherits="DotNetNuke.Modules.Admin.Vendors.Affiliates"
    CodeFile="Affiliates.ascx.cs" %>
<asp:DataGrid ID="grdAffiliates" runat="server" Width="100%" Border="0" CellPadding="4"
    AutoGenerateColumns="false" EnableViewState="true" BorderStyle="None" BorderWidth="0px"
    GridLines="None" CssClass="dnnAffiliatesGrid">
    <headerstyle CssClass="dnnAffiliatesGridHeader" />
    <itemstyle CssClass="dnnAffiliatesGridItem" />
    <alternatingitemstyle CssClass="dnnAffiliatesGridAltItem" />  
    <Columns>
        <asp:TemplateColumn>
            <ItemStyle Width="20px"></ItemStyle>
            <ItemTemplate>
                <asp:HyperLink NavigateUrl='<%# FormatURL("AffilId",DataBinder.Eval(Container.DataItem,"AffiliateId").ToString()) %>'
                    runat="server" ID="Hyperlink1">
                    <asp:Image ImageUrl="~/images/edit.gif" resourcekey="Edit" AlternateText="Edit" runat="server"
                        ID="Hyperlink1Image" />
                </asp:HyperLink>
            </ItemTemplate>
        </asp:TemplateColumn>
        <asp:TemplateColumn HeaderText="Start">
            <ItemTemplate>
                <asp:Label ID="lblStartDate" runat="server" Text='<%# DisplayDate((DateTime)DataBinder.Eval(Container.DataItem, "StartDate")) %>'></asp:Label>
            </ItemTemplate>
        </asp:TemplateColumn>
        <asp:TemplateColumn HeaderText="End">
            <ItemTemplate>
                <asp:Label ID="lblEndDate" runat="server" Text='<%# DisplayDate((DateTime)DataBinder.Eval(Container.DataItem, "EndDate")) %>'></asp:Label>
            </ItemTemplate>
        </asp:TemplateColumn>
        <asp:BoundColumn DataField="CPC" HeaderText="CPC" DataFormatString="{0:#,##0.0####}"/>
        <asp:BoundColumn DataField="Clicks" HeaderText="Clicks"/>
        <asp:BoundColumn DataField="CPCTotal" HeaderText="Total" DataFormatString="{0:#,##0.0####}"/>
        <asp:BoundColumn DataField="CPA" HeaderText="CPA" DataFormatString="{0:#,##0.0####}"/>
        <asp:BoundColumn DataField="Acquisitions" HeaderText="Acquisitions"/>
        <asp:BoundColumn DataField="CPATotal" HeaderText="Total" DataFormatString="{0:#,##0.0####}"/>
    </Columns>
</asp:DataGrid>
<ul class="dnnActions dnnClear">
    <li><asp:hyperlink CssClass="dnnPrimaryAction" id="cmdAdd" resourcekey="cmdAdd" runat="server"/></li>
</ul>
