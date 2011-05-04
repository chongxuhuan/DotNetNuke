<%@ Control Language="C#" AutoEventWireup="false" Inherits="DotNetNuke.Modules.Admin.Host.WhatsNew" CodeFile="WhatsNew.ascx.cs" %>
<p class="Normal" id="header" runat="server"></p>

<script language="javascript" type="text/javascript">
    $(document).ready(function () {
        $('.dnnWhatsNew').dnnPanels();
    });
</script>
<div class="dnnForm dnnWhatsNew dnnClear" id="dnnWhatsNew">
    <asp:Repeater ID="WhatsNewList" runat="server">
        <ItemTemplate>
            <div class="wnContent dnnClear">
                <h2 id="Panel-<%# Eval("Version") %>" class="dnnFormSectionHead"><a href="" class=""><%# Eval("Version") %></a></h2>
                <fieldset>
                    <legend></legend>
                    <div class="dnnFormItem">
                        <%#Eval("Notes")%>
                    </div>
                </fieldset>
            </div>
        </ItemTemplate>
    </asp:Repeater>
</div>

<p class="NormalBold" id="footer" runat="server"></p>