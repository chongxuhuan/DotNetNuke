<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.UI.WebControls" Assembly="DotNetNuke" %>
<%@ Control Language="vb" AutoEventWireup="false" CodeFile="ListEditor.ascx.vb" Inherits="DotNetNuke.Common.Lists.ListEditor" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="SectionHead" Src="~/controls/SectionHeadControl.ascx" %>
<%@ Register TagPrefix="dnntv" Namespace="DotNetNuke.UI.WebControls" Assembly="DotNetNuke.WebControls" %>
<%@ Register TagPrefix="dnn" TagName="ListEntries" Src="~/DesktopModules/Admin/Lists/ListEntries.ascx" %>
<table id="tblList" cellSpacing="5" width="660" border="0">
	<tr>
		<td vAlign="top" width="240">
			<dnn:sectionhead id="dshtree" includerule="true" section="tbltree" cssclass="Head" text="Lists" 
			    resourcekey="Lists" runat="server"></dnn:sectionhead>
			<table id="tbltree" cellSpacing="0" cellPadding="0" width="100%" border="0" runat="server">
				<tr>
					<td>
						<dnntv:dnntree id="DNNtree" runat="server" DefaultNodeCssClassOver="Normal" DefaultNodeCssClass="Normal"
							cssClass="Normal"></dnntv:dnntree>
					</td>
				</tr>
				<tr height="5">
					<td></td>
				</tr>
				<tr>
					<td>
						<dnn:commandbutton id="cmdAddList" runat="server" resourcekey="AddList" CssClass="CommandButton" imageurl="~/images/add.gif"
							causesvalidation="False" />
					</td>
				</tr>
			</table>
		</td>
		<td vAlign="top" width="420">
			<dnn:sectionhead id="dshDetails" includerule="true" section="divDetails" cssclass="Head" runat="server"></dnn:sectionhead>
			<div id="divDetails" runat="server">
			    <dnn:ListEntries id="lstEntries" runat="Server" />
			</div>
		</td>
	</tr>
</table>
