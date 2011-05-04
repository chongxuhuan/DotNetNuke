<%@ Control Language="C#" AutoEventWireup="false" Inherits="DotNetNuke.Common.Lists.ListEntries" CodeFile="ListEntries.ascx.cs" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.UI.WebControls" Assembly="DotNetNuke" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="dnntv" Namespace="DotNetNuke.UI.WebControls" Assembly="DotNetNuke.WebControls" %>

<div>
    <div id="rowListdetails" runat="server">
        <div id="rowListParent" runat="server" class="dnnFormItem">
            <dnn:Label ID="plListParent" runat="server" ControlName="lblListParent"/>
            <asp:Label ID="lblListParent" runat="server"/>
        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="plListName" runat="server" ControlName="lblListName"/>
            <asp:Label ID="lblListName" runat="server"/>
        </div>    
        <div class="dnnFormItem">
            <dnn:Label ID="plEntryCount" runat="server" ControlName="lblEntryCount"/>
            <asp:Label ID="lblEntryCount" runat="server"/>
        </div>
        <ul class="dnnActions dnnClear">
    	    <li><asp:LinkButton id="cmdAddEntry" runat="server" CssClass="dnnPrimaryAction" resourcekey="cmdAddEntry" /></li>
    	    <li><asp:LinkButton id="cmdDeleteList" runat="server" CssClass="dnnSecondaryAction" resourcekey="cmdDeleteList" /></li>
        </ul>   
    </div>
    <div  id="rowEntryGrid" runat="server">
		<asp:DataGrid ID="grdEntries" runat="server" DataKeyField="EntryID" AutoGenerateColumns="false"
			Width="100%" CellPadding="2" Border="0" BorderWidth="0px" GridLines="None">
			<Columns>
				<dnn:ImageCommandColumn CommandName="Edit" ImageURL="~/images/edit.gif" EditMode="Command"
					KeyField="EntryID" />
				<dnn:ImageCommandColumn CommandName="Delete" ImageURL="~/images/delete.gif" EditMode="Command"
					KeyField="EntryID" />
				<asp:BoundColumn DataField="Text" HeaderText="Text">
					<HeaderStyle CssClass="NormalBold"></HeaderStyle>
					<ItemStyle CssClass="Normal"></ItemStyle>
				</asp:BoundColumn>
				<asp:BoundColumn DataField="Value" HeaderText="Value">
					<HeaderStyle HorizontalAlign="Center" CssClass="NormalBold"></HeaderStyle>
					<ItemStyle HorizontalAlign="Center" CssClass="Normal"></ItemStyle>
				</asp:BoundColumn>
				<asp:TemplateColumn>
					<HeaderStyle Width="18px" CssClass="NormalBold"></HeaderStyle>
					<ItemStyle CssClass="Normal"></ItemStyle>
					<ItemTemplate>
						<asp:ImageButton ID="btnUp" Visible="<%# EnableSortOrder %>" ImageUrl="~/Images/up.gif"
							runat="server" CssClass="CommandButton" AlternateText="Move entry up" resourcekey="btnUp.AlternateText"
							CommandName="up"></asp:ImageButton>
					</ItemTemplate>
				</asp:TemplateColumn>
				<asp:TemplateColumn>
					<HeaderStyle Width="18px" CssClass="NormalBold"></HeaderStyle>
					<ItemStyle CssClass="Normal"></ItemStyle>
					<ItemTemplate>
						<asp:ImageButton ID="btnDown" Visible="<%# EnableSortOrder %>" ImageUrl="~/Images/dn.gif"
							runat="server" CssClass="CommandButton" AlternateText="Move entry down" resourcekey="btnDown.AlternateText"
							CommandName="down"></asp:ImageButton>
					</ItemTemplate>
				</asp:TemplateColumn>
			</Columns>
		</asp:DataGrid>
    </div>
    <div  id="rowEntryEdit" runat="server">
        <div id="rowParentKey" runat="server" class="dnnFormItem">
            <dnn:Label ID="plParentKey" runat="server" ControlName="txtParentKey"/>
            <asp:TextBox ID="txtParentKey" runat="server" MaxLength="100" ReadOnly="true"/>
        </div>
        <div id="rowListName" runat="server" class="dnnFormItem">
            <dnn:Label ID="plEntryName" Text="Entry Name:" runat="server" ControlName="txtEntryName"/>
			<asp:TextBox ID="txtEntryName" runat="server" MaxLength="100"/>
			<asp:TextBox ID="txtEntryID" runat="server" Visible="false"/>
        </div>
        <div  id="rowSelectList" runat="server" class="dnnFormItem">
            <dnn:Label ID="plSelectList" runat="server" ControlName="ddlSelectList"/>
			<asp:DropDownList ID="ddlSelectList" AutoPostBack="true" runat="server" Enabled="False"/>
        </div>
        <div  id="rowSelectParent" runat="server" class="dnnFormItem">
            <dnn:Label ID="plSelectParent" runat="server" ControlName="ddlSelectParent"/>
			<asp:DropDownList ID="ddlSelectParent" runat="server" Enabled="False"/>
        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="plEntryText" runat="server" ControlName="txtEntryText"/>
			<asp:TextBox ID="txtEntryText" CssClass="dnnFormRequired" runat="server" MaxLength="100"/>
			<asp:RequiredFieldValidator ID="valEntryText" CssClass="dnnFormMessage dnnFormError" runat="server"
				resourcekey="valEntryText.ErrorMessage" Display="Dynamic" ControlToValidate="txtEntryText"/>
        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="plEntryValue" runat="server" ControlName="txtEntryValue"/>
			<asp:TextBox ID="txtEntryValue" CssClass="dnnFormRequired" runat="server" MaxLength="100"/>
			<asp:RequiredFieldValidator ID="valEntryValue" CssClass="dnnFormMessage dnnFormError" runat="server"
				resourcekey="valEntryValue.ErrorMessage" Display="Dynamic" ControlToValidate="txtEntryValue"/>
        </div>
        <div id="rowEnableSortOrder" runat="server" class="dnnFormItem">
			<dnn:Label ID="plEnableSortOrder" runat="server" ControlName="chkEnableSortOrder"/>
			<asp:CheckBox ID="chkEnableSortOrder" runat="server"/>
        </div>
        <ul class="dnnActions dnnClear">
    	    <li><asp:LinkButton id="cmdSaveEntry" runat="server" CssClass="dnnPrimaryAction" resourcekey="cmdSave" /></li>
    	    <li><asp:LinkButton id="cmdDelete" runat="server" CssClass="dnnSecondaryAction" resourcekey="cmdDeleteEntry" /></li>
    	    <li><asp:LinkButton id="cmdCancel" runat="server" CssClass="dnnSecondaryAction" resourcekey="cmdCancel" /></li>
        </ul>   
    </div>
</div>
