<%@ Control language="vb" AutoEventWireup="false" Explicit="True" Inherits="DotNetNuke.Modules.Admin.Skins.EditSkins" CodeFile="EditSkins.ascx.vb" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.UI.WebControls" Assembly="DotNetNuke" %>
<table cellSpacing="0" cellPadding="0" width="100%" border="0" style="text-align:center">
	<tr id="typeRow" runat="server">
		<td class="SubHead" vAlign="middle" align="right" colspan="2"><dnn:label id="plType" text="Skin Type:" runat="server"></dnn:label></td>
		<td align="left" colSpan="2">&nbsp;&nbsp;
			<asp:checkbox id="chkHost" CssClass="SubHead" Runat="server" resourcekey="Host" AutoPostBack="True" Checked="True" Text="Host"></asp:checkbox>&nbsp;&nbsp;
			<asp:checkbox id="chkSite" CssClass="SubHead" Runat="server" resourcekey="Site" AutoPostBack="True" Checked="True" Text="Site"></asp:checkbox>
		</td>
	</tr>
	<tr><td colSpan="4">&nbsp;</td></tr>
	<tr>
		<td class="SubHead" vAlign="middle"><dnn:label id="plSkins" suffix=":" controlname="cboSkins" runat="server"></dnn:label></td>
		<td><asp:dropdownlist id="cboSkins" Runat="server" AutoPostBack="True"></asp:dropdownlist></td>
		<td class="SubHead" vAlign="middle"><dnn:label id="plContainers" suffix=":" controlname="cboContainers" runat="server"></dnn:label></td>
		<td><asp:dropdownlist id="cboContainers" Runat="server" AutoPostBack="True"></asp:dropdownlist></td>
	</tr>
	<tr><td colSpan="4">&nbsp;</td></tr>
	<tr>
	    <td colspan="2" align="center"><asp:Label ID="lblLegacy" runat="server" class="NormalRed" resourcekey="LegacySkin" Visible="false" /></td>
	</tr>
	<tr><td colSpan="4">&nbsp;</td></tr>
	<tr>
		<td class="SubHead" align="center" colSpan="4">
		    <asp:panel id="pnlSkin" Runat="server" Visible="False">
                <asp:label id="lblApply" runat="server" resourcekey="ApplyTo">Apply To</asp:label>:&nbsp;&nbsp; 
                <asp:CheckBox id="chkPortal" Text="Portal" Checked="True" resourcekey="Portal" Runat="server" CssClass="SubHead"></asp:CheckBox>&nbsp;&nbsp; 
                <asp:CheckBox id="chkAdmin" Text="Admin" Checked="True" resourcekey="Admin" Runat="server" CssClass="SubHead"></asp:CheckBox><br/><br/>
            </asp:panel>
        </td>
	</tr>
	<tr>
	    <td colSpan="4">
	        <table border="1" cellspacing="0" cellpadding="2" width="100%">
	            <tr>
	                <td align="center" bgcolor="CCCCCC" class="Head">
	                    <asp:Label ID="lblSkinTitle" runat="server" resourcekey="plSkins" />
	                </td>
	            </tr>
	            <tr>
	                <td align="center"><table id="tblSkins" runat="server" cellspacing="4" cellpadding="4" /></td>
	            </tr>
	        </table>
	        <table border="1" cellspacing="0" cellpadding="2" width="100%">
	            <tr>
	                <td align="center" bgcolor="CCCCCC" class="Head">
	                    <asp:Label ID="lblContainerTitle" runat="server" resourcekey="plContainers" />
	                </td>
	            </tr>
	            <tr>
	                <td align="center"><table id="tblContainers" runat="server" cellspacing="4" cellpadding="4" /></td>
	            </tr>
	        </table>
	    </td>
	</tr>
	<tr>
		<td align="center" colSpan="4"><asp:panel id="pnlParse" Runat="server" Visible="False">
			<table cellSpacing="0" cellPadding="0" border="0">
				<tr>
					<td class="SubHead"><asp:label id="lblParseOptions" runat="server" resourcekey="ParseOptions">Parse Options</asp:label>:</td>
					<td>
						<asp:RadioButtonList id="optParse" Runat="server" CssClass="SubHead" RepeatDirection="Horizontal">
							<asp:ListItem resourcekey="Localized" Value="L" Selected="True">Localized</asp:ListItem>
							<asp:ListItem resourcekey="Portable" Value="P">Portable</asp:ListItem>
						</asp:RadioButtonList>
					</td>
				</tr>
			</table>
			</asp:panel>
		</td>
	</tr>
	<tr>
		<td colSpan="4"><asp:label id="lblOutput" CssClass="Normal" Runat="server" EnableViewState="False"></asp:label></td>
	</tr>
</table>
<p align="center">
    <dnn:CommandButton id="cmdParse" CssClass="CommandButton" Runat="server"  resourcekey="cmdParse" ImageUrl="~/images/settings.gif" />&nbsp;
    <dnn:CommandButton id="cmdDelete" CssClass="CommandButton" Runat="server" resourcekey="cmdDelete" ImageUrl="~/images/delete.gif" />
    <dnn:CommandButton id="cmdRestore" CssClass="CommandButton" Runat="server" resourcekey="cmdRestore" ImageUrl="~/images/synchronize.gif" />
</p>
