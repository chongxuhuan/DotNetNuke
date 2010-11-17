<%@ Control Inherits="DotNetNuke.Modules.Html.Settings" CodeBehind="Settings.ascx.vb" language="vb" AutoEventWireup="false" Explicit="true" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<table width="550" cellspacing="0" cellpadding="4" border="0" width=100%>
	<tr>
		<td class="SubHead" width="150" valign="top"><dnn:label id="plReplaceTokens" controlname="chkReplaceTokens" runat="server" /></td>
		<td><asp:CheckBox ID="chkReplaceTokens" runat="server" CssClass="NormalTextBox" Checked="false" /></td>
	</tr> 
	<tr>
		<td class="SubHead" width="150" valign="top"><dnn:label id="plWorkflow" controlname="cboWorkflow" runat="server" Text="Workflow" suffix=":" /></td>
		<td valign="top">
		    <asp:DropDownList ID="cboWorkflow" runat="server" CssClass="NormalTextBox" Width="300" DataTextField="WorkflowName" DataValueField="WorkflowID" AutoPostBack="True" />
            <br /><br /><asp:Label ID="lblDescription" runat="server" CssClass="Normal"  />
		</td>
	</tr>
	<tr id="rowApplyTo" runat="server" visible="false">
	    <td class="SubHead" width="150" valign="top"><dnn:label id="plApplyTo" controlname="cboWorkflow" runat="server" Text="Apply" /></td>
	    <td valign="top">
	        <asp:RadioButtonList ID="rblApplyTo" runat="server" RepeatDirection="Horizontal" RepeatColumns="3">
	            <asp:ListItem Value="Module" ResourceKey="Module" />
	            <asp:ListItem Value="Page" ResourceKey="Page" />
	            <asp:ListItem Value="Site" ResourceKey="Site" />
	        </asp:RadioButtonList>
            <asp:CheckBox ID="chkReplace" runat="server" CssClass="NormalBold" resourcekey="chkReplace" Text="Replace Existing Settings?" Checked="False" />
	    </td>
	</tr>
</table>

