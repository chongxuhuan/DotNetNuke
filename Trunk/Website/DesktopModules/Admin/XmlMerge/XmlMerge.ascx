<%@ Control language="vb" Inherits="DotNetNuke.Modules.XmlMerge.XmlMerge" CodeFile="XmlMerge.ascx.vb" AutoEventWireup="false" Explicit="True" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="SectionHead" Src="~/controls/SectionHeadControl.ascx" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.UI.WebControls" Assembly="DotNetNuke.Web" %>

<dnn:sectionhead ID="dshConfigEditor" runat="server" CssClass="Head" Text="Configuration" Section="tblConfiguration" ResourceKey="Configuration" />
<table id="tblConfiguration" runat="server" width="100%" cellpadding="2" cellspacing="2" summary="Edit Configuration Files Table" border="0">
    <tr>
        <td class="SubHead" valign="middle" width="200">
            <dnn:Label ID="plConfig" runat="server" ControlName="ddlConfig" Suffix=":"></dnn:Label>
        </td>
        <td valign="middle">
            <dnn:DnnComboBox ID="ddlConfig" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlConfig_SelectedIndexChanged" />                
        </td>    
    </tr>
    <tr>
        <td colspan="2">
            <asp:TextBox ID="txtConfiguration" runat="server" TextMode="MultiLine" width="100%" Rows="20" EnableViewState="True" Enabled="false"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td colspan="2" align="center">
            <asp:LinkButton ID="cmdSave" resourcekey="cmdSave" EnableViewState="True" CssClass="CommandButton" runat="server" Enabled="false" >Save</asp:LinkButton>
        </td>
    </tr>
</table>
<br />
<hr style="margin: 10px 0"/>
<dnn:sectionhead ID="dshScriptMerge" runat="server" CssClass="Head" Text="Configuration" Section="tblScriptMerge" ResourceKey="Merge" IsExpanded="false" />
<table id="tblScriptMerge" runat="server" width="100%" cellspacing="2" cellpadding="2" summary="Load Merge Script Design Table" border="0">
    <tr>
        <td class="SubHead" width="200" valign="middle">
            <dnn:Label ID="plScript" runat="server" ControlName="uplScript" Suffix=""></dnn:Label>
        </td>
        <td valign="middle">
            <asp:FileUpload ID="uplScript" runat="server" />

            <asp:LinkButton ID="cmdUpload" resourcekey="cmdUpload" EnableViewState="False" CssClass="CommandButton" runat="server" ToolTip="Load the selected file.">Load</asp:LinkButton>
        </td>
    </tr>
    <tr>
        <td colspan="2">
            <asp:TextBox ID="txtScript" runat="server" TextMode="MultiLine" width="100%" Rows="20" EnableViewState="False"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td colspan="2" align="center">
            <asp:LinkButton ID="cmdExecute" resourcekey="cmdExecute" EnableViewState="True" CssClass="CommandButton" runat="server" Enabled="false" >Execute</asp:LinkButton>
        </td>
    </tr>
</table>
<asp:Label ID="lblMessage" runat="server" CssClass="NormalRed" EnableViewState="False"></asp:Label>