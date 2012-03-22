<%@ Control language="C#" Inherits="DotNetNuke.Providers.FiftyOneClientCapabilityProvider.PropertyDictionary, DotNetNuke.Providers.FiftyOneClientCapabilityProvider" AutoEventWireup="true" Codebehind="PropertyDictionary.ascx.cs" %>

<div class="property-dictionary">
    <div class="header">
        <div class="copy">
            <h3><%= LocalizeString("Introduction.Header") %></h3>
            <p><%= LocalizeString("Introduction.Text") %></p>
        </div>
        <div class="help">
            <a href="#"><%= LocalizeString("Help") %></a>
        </div>
        <a class="back" href="#"><%= LocalizeString("BackToVendorList") %></a>
    </div>
    <div class="body">
        <h2><%= LocalizeString("Glossary.Header") %></h2>
        <p class="help-key"><%= LocalizeString("Glossary.Text") %></p>
    </div>
    <asp:Repeater runat="server" ID="HardwareList">
        <HeaderTemplate>
             <table>
                <thead>
                    <tr>
                        <td colspan="2"><%= LocalizeString("HardwareList.Header") %></td>
                    </tr>
                </thead>
                <tbody>
        </HeaderTemplate>
        <ItemTemplate>
                    <tr>
                        <td>
                            <%# DataBinder.Eval(Container.DataItem, "Name") %>
                            <span class="premium" id="Premium" runat="server">*</span>
                        </td>
                        <td>
                            <p><%# DataBinder.Eval(Container.DataItem, "Description") %></p>
                            <p class="values" id="Values" runat="server"></p>
                        </td>
                    </tr>
        </ItemTemplate>
        <FooterTemplate>
                </tbody>
            </table>
        </FooterTemplate>
    </asp:Repeater>
    <asp:Repeater runat="server" ID="SoftwareList">
        <HeaderTemplate>
             <table>
                <thead>
                    <tr>
                        <td colspan="2"><%= LocalizeString("SoftwareList.Header") %></td>
                    </tr>
                </thead>
                <tbody>
        </HeaderTemplate>
        <ItemTemplate>
                    <tr>
                        <td><%# DataBinder.Eval(Container.DataItem, "Name") %></td>
                        <td><%# DataBinder.Eval(Container.DataItem, "Description") %></td>
                    </tr>
        </ItemTemplate>
        <FooterTemplate>
                </tbody>
            </table>
        </FooterTemplate>
    </asp:Repeater>
    <asp:Repeater runat="server" ID="BrowserList">
        <HeaderTemplate>
             <table>
                <thead>
                    <tr>
                        <td colspan="2"><%= LocalizeString("BrowserList.Header") %></td>
                    </tr>
                </thead>
                <tbody>
        </HeaderTemplate>
        <ItemTemplate>
                    <tr>
                        <td><%# DataBinder.Eval(Container.DataItem, "Name") %></td>
                        <td><%# DataBinder.Eval(Container.DataItem, "Description") %></td>
                    </tr>
        </ItemTemplate>
        <FooterTemplate>
                </tbody>
            </table>
        </FooterTemplate>
    </asp:Repeater>
    <asp:Repeater runat="server" ID="ContentList">
        <HeaderTemplate>
             <table>
                <thead>
                    <tr>
                        <td colspan="2"><%= LocalizeString("ContentList.Header") %></td>
                    </tr>
                </thead>
                <tbody>
        </HeaderTemplate>
        <ItemTemplate>
                    <tr>
                        <td><%# DataBinder.Eval(Container.DataItem, "Name") %></td>
                        <td><%# DataBinder.Eval(Container.DataItem, "Description") %></td>
                    </tr>
        </ItemTemplate>
        <FooterTemplate>
                </tbody>
            </table>
        </FooterTemplate>
    </asp:Repeater>
</div>