``  <%@ Register TagPrefix="Portal" TagName="URL" Src="~/controls/URLControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="Audit" Src="~/controls/ModuleAuditControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Control language="vb" AutoEventWireup="false" Explicit="True" Inherits="DotNetNuke.Modules.Admin.Vendors.EditBanner" CodeFile="EditBanner.ascx.vb" %>
<%@ Register Assembly="DotNetNuke.WebControls" Namespace="DotNetNuke.UI.WebControls" TagPrefix="DNN" %>
<style type="text/css">
.GroupSuggestMenu {
    width: 300px;
}
</style>
<br>
<table cellSpacing="2" cellPadding="0" width="560" summary="Edit Banner Design Table">
	<tr vAlign="top">
		<td colspan="2">
			<asp:DataList id="lstBanners" runat="server" CellPadding="4" Width="100%" Summary="Banner Design Table"
				EnableViewState="true">
				<itemstyle horizontalalign="Center" borderwidth="1" bordercolor="#000000"></ItemStyle>
				<itemtemplate>
					<asp:Label ID="lblItem" Runat="server" Text='<%# FormatItem(DataBinder.Eval(Container.DataItem,"VendorId"),DataBinder.Eval(Container.DataItem,"BannerId"),DataBinder.Eval(Container.DataItem,"BannerTypeId"),DataBinder.Eval(Container.DataItem,"BannerName"),DataBinder.Eval(Container.DataItem,"ImageFile"),DataBinder.Eval(Container.DataItem,"Description"),DataBinder.Eval(Container.DataItem,"Url"),DataBinder.Eval(Container.DataItem,"Width"),DataBinder.Eval(Container.DataItem,"Height")) %>'>
					</asp:Label>
				</ItemTemplate>
			</asp:DataList>
			<br>
		</td>
	</tr>
	<tr vAlign="top">
		<td class="SubHead" width="125"><dnn:Label id="plBannerName" runat="server" controlname="txtBannerName" suffix=":"></dnn:Label></td>
		<td width="400">
			<asp:textbox id="txtBannerName" runat="server" maxlength="100" Columns="30" width="300" cssclass="NormalTextBox"></asp:textbox>
			<asp:requiredfieldValidator id="valBannerName" resourcekey="BannerName.ErrorMessage" runat="server" ControlToValidate="txtBannerName"
				ErrorMessage="You Must Enter a Banner Name" Display="Dynamic" CssClass="NormalRed"></asp:requiredfieldValidator>
		</td>
	</tr>
	<tr vAlign="top">
		<td class="SubHead" width="125"><dnn:Label id="plBannerType" runat="server" controlname="cboBannerType" suffix=":"></dnn:Label></td>
		<td width="400">
			<asp:DropDownList id="cboBannerType" DataTextField="BannerTypeName" DataValueField="BannerTypeId"
				width="300" cssclass="NormalTextBox" runat="server" />
		</td>
	</tr>
	<tr vAlign="top">
		<td class="SubHead" width="125"><dnn:Label id="plBannerGroup" runat="server" controlname="txtBannerGroup" suffix=":"></dnn:Label></td>
		<td width="400"><dnn:DNNTextSuggest id="DNNTxtBannerGroup" runat="server" Columns="30" CssClass="NormalTextBox"
                LookupDelay="500" MaxLength="100" Width="300px" TextSuggestCssClass="NormalTextBox SuggestTextMenu GroupSuggestMenu" DefaultNodeCssClassOver="SuggestNodeOver">
            </dnn:DNNTextSuggest></td>
	</tr>
	<tr>
		<td colspan="2">&nbsp;</td>
	</tr>
	<tr>
		<td class="SubHead" width="125"><dnn:Label id="plImage" runat="server" controlname="ctlImage" suffix=":"></dnn:Label></td>
		<td width="400">
			<portal:url id="ctlImage" runat="server" width="250" Required="False" ShowFiles="True" ShowTabs="False" ShowUrls="True"
				ShowTrack="False" ShowLog="False" UrlType="F" />
		</td>
	</tr>
	<tr vAlign="top">
		<td class="SubHead" width="125"><dnn:Label id="plWidth" runat="server" controlname="txtWidth" suffix=":"></dnn:Label></td>
		<td width="400">
			<asp:textbox id="txtWidth" runat="server" maxlength="100" Columns="30" width="300" cssclass="NormalTextBox"></asp:textbox>
		</td>
	</tr>
	<tr vAlign="top">
		<td class="SubHead" width="125"><dnn:Label id="plHeight" runat="server" controlname="txtHeight" suffix=":"></dnn:Label></td>
		<td width="400">
			<asp:textbox id="txtHeight" runat="server" maxlength="100" Columns="30" width="300" cssclass="NormalTextBox"></asp:textbox>
		</td>
	</tr>
	<tr vAlign="top">
		<td class="SubHead" width="125" valign="middle"><dnn:Label id="plDescription" runat="server" controlname="txtDescription" suffix=":"></dnn:Label></td>
		<td width="400">
			<asp:textbox id="txtDescription" runat="server" maxlength="2000" Columns="30" width="300" cssclass="NormalTextBox"
				TextMode="MultiLine" Rows="5"></asp:textbox>
		</td>
	</tr>
	<tr>
		<td class="SubHead" width="125"><dnn:Label id="plURL" runat="server" controlname="ctlURL" suffix=":"></dnn:Label></td>
		<td width="400">
			<portal:url id="ctlURL" runat="server" width="250" Required="False" ShowFiles="True" ShowTabs="True" ShowUrls="True"
				ShowTrack="False" ShowLog="False" UrlType="U" />
		</td>
	</tr>
	<tr>
		<td colspan="2">&nbsp;</td>
	</tr>
	<tr>
		<td class="SubHead" width="125"><dnn:Label id="plCPM" runat="server" controlname="txtCPM" suffix=":"></dnn:Label></td>
		<td width="400">
			<asp:TextBox id="txtCPM" runat="server" maxlength="7" Columns="30" width="300" cssclass="NormalTextBox"></asp:TextBox>
			<asp:requiredfieldValidator id="valCPM" runat="server" resourcekey="CPM.ErrorMessage" ControlToValidate="txtCPM"
				ErrorMessage="You Must Enter a Valid CPM" Display="Dynamic" CssClass="NormalRed"></asp:requiredfieldValidator>
			<asp:compareValidator id="compareCPM" runat="server" resourcekey="CPM.ErrorMessage" ControlToValidate="txtCPM"
				ErrorMessage="You Must Enter a Valid CPM" Display="Dynamic" Type="Currency" Operator="DataTypeCheck"
				CssClass="NormalRed"></asp:compareValidator>
		</td>
	</tr>
	<tr>
		<td class="SubHead" width="125"><dnn:Label id="plImpressions" runat="server" controlname="txtImpressions" suffix=":"></dnn:Label></td>
		<td width="400">
			<asp:TextBox id="txtImpressions" runat="server" maxlength="10" Columns="30" width="300" cssclass="NormalTextBox"></asp:TextBox>
			<asp:requiredfieldValidator id="valImpressions" resourcekey="Impressions.ErrorMessage" runat="server" ControlToValidate="txtImpressions"
				ErrorMessage="You Must Enter a Valid Number of Impressions" Display="Dynamic" CssClass="NormalRed"></asp:requiredfieldValidator>
			<asp:compareValidator id="compareImpressions" resourcekey="Impressions.ErrorMessage" runat="server" Display="Dynamic"
				ErrorMessage="You Must Enter a Valid Number of Impressions" ControlToValidate="txtImpressions" Operator="DataTypeCheck"
				Type="Integer" CssClass="NormalRed"></asp:compareValidator>
		</td>
	</tr>
	<tr>
		<td class="SubHead" width="125"><dnn:Label id="plStartDate" runat="server" controlname="txtStartDate" suffix=":"></dnn:Label></td>
		<td width="400">
			<asp:TextBox id="txtStartDate" runat="server" cssclass="NormalTextBox" width="250" Columns="30"
				maxlength="11"></asp:TextBox>&nbsp;
			<asp:hyperlink id="cmdStartCalendar" resourcekey="Calendar" CssClass="CommandButton" Runat="server">Calendar</asp:hyperlink>
		</td>
	</tr>
	<tr>
		<td class="SubHead" width="125"><dnn:Label id="plEndDate" runat="server" controlname="txtEndDate" suffix=":"></dnn:Label></td>
		<td width="400">
			<asp:TextBox id="txtEndDate" runat="server" cssclass="NormalTextBox" width="250" Columns="30"
				maxlength="11"></asp:TextBox>&nbsp;
			<asp:hyperlink id="cmdEndCalendar" resourcekey="Calendar" CssClass="CommandButton" Runat="server">Calendar</asp:hyperlink>
		</td>
	</tr>
	<tr>
		<td class="SubHead" width="125"><dnn:Label id="plCriteria" runat="server" controlname="optCriteria" suffix=":"></dnn:Label></td>
		<td>
			<asp:RadioButtonList id="optCriteria" runat="server" CssClass="NormalBold" RepeatDirection="Horizontal">
				<asp:ListItem Value="1">OR</asp:ListItem>
				<asp:ListItem Value="0">AND</asp:ListItem>
			</asp:RadioButtonList>
		</td>
	</tr>
</table>
<p>
	<asp:linkbutton class="CommandButton" id="cmdUpdate" resourcekey="cmdUpdate" runat="server" text="Update"
		borderstyle="none"></asp:linkbutton>&nbsp;
	<asp:linkbutton class="CommandButton" id="cmdCancel" resourcekey="cmdCancel" runat="server" text="Cancel"
		borderstyle="none" causesvalidation="False"></asp:linkbutton>&nbsp;
	<asp:linkbutton class="CommandButton" id="cmdDelete" resourcekey="cmdDelete" runat="server" text="Delete"
		borderstyle="none" causesvalidation="False"></asp:linkbutton>&nbsp;
	<asp:linkbutton class="CommandButton" id="cmdCopy" resourcekey="cmdCopy" runat="server" text="Copy"
		borderstyle="none" causesvalidation="False"></asp:linkbutton>&nbsp;
	<asp:linkbutton class="CommandButton" id="cmdEmail" resourcekey="cmdEmail" runat="server" text="Email Status to Vendor" borderstyle="none" causesvalidation="False"></asp:linkbutton>
</p>
<dnn:audit id="ctlAudit" runat="server" />
