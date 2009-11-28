<%@ Control language="vb" AutoEventWireup="false" Explicit="True" Inherits="DotNetNuke.Modules.Admin.Modules.ViewSource" CodeFile="viewsource.ascx.vb" %>
<%@ Register Assembly="DotnetNuke" Namespace="DotNetNuke.UI.WebControls" TagPrefix="dnn" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>

<table>
    <tr>
        <td style="width:150px;" class="SubHead"><dnn:Label id="plFile" runat="Server"/></td>
        <td style="width:600px;" class="NormalTextBox"><asp:DropDownList ID="cboFile" runat="server" AutoPostBack="true" /></td>
    </tr>
    <tr>
        <td colspan="2"><asp:Label ID="lblSourceFile" runat="server" cssClass="NormalBold" Visible="false"/></td>
    </tr>
    <tr id="trSource" runat="server" visible="false">
        <td colspan="2">
            <dnn:label id="plSource" controlname="txtSource" runat="server" cssClass="SubHead"/>
            <asp:TextBox ID="txtSource" runat="server" cssClass="NormalTextBox" TextMode="MultiLine" Rows="20" Columns="80" style="width:750px" />
        </td>
    </tr>
</table>
<p>
    <dnn:commandbutton id="cmdUpdate" resourcekey="cmdUpdate" runat="server" cssclass="CommandButton" ImageUrl="~/images/save.gif"/>&nbsp;
    <dnn:commandbutton id="cmdCancel" resourcekey="cmdCancel" runat="server" cssclass="CommandButton" ImageUrl="~/images/lt.gif" causesvalidation="False" />
</p>
