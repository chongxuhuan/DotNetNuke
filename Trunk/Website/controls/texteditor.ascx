<%@ Control Language="C#" AutoEventWireup="false" Inherits="DotNetNuke.UI.UserControls.TextEditor" %>
<%@ Register TagPrefix="dnn" TagName="label" Src="~/controls/labelControl.ascx" %>
<table cellSpacing="2" cellPadding="2" summary="Edit HTML Design Table" border="0" id="tblTextEditor" Runat="server">
	<tr vAlign="top" id="trView" runat="server">
	    <td><dnn:label id="plView" runat="server" controlname="optView"/></td>
	    <td align="left">
			<asp:RadioButtonList id="optView" Runat="server" AutoPostBack="True" RepeatDirection="Horizontal" CssClass="NormalTextBox"></asp:RadioButtonList>
		</td>
	</tr>
	<tr vAlign="top">
		<td id="celTextEditor" Runat="Server" colspan="2">
		    <asp:panel id="pnlBasicTextBox" Visible="False" Runat="server" Width="100%">
				<asp:TextBox id="txtDesktopHTML" CssClass="NormalTextBox" runat="server" textmode="multiline" rows="12" width="100%" columns="75"></asp:TextBox>
				<br/>
				<asp:Panel id="pnlBasicRender" Runat="server" Visible="True">
				    <table cellSpacing="2" cellPadding="2">
				        <tr>
				            <td><dnn:label id="plRender" runat="server" controlname="optRender"/></td>
				            <td><asp:RadioButtonList id="optRender" Runat="server" AutoPostBack="True" RepeatDirection="Horizontal" CssClass="NormalTextBox"></asp:RadioButtonList></td>
				        </tr>
				    </table>
				</asp:Panel>
			</asp:panel><asp:panel id="pnlRichTextBox" Visible="False" Runat="server">
				<asp:PlaceHolder id="plcEditor" runat="server"></asp:PlaceHolder>
			</asp:panel>
		</td>
	</tr>
</table>
