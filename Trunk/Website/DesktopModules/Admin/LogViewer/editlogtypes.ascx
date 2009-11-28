<%@ Control language="vb" AutoEventWireup="false" Explicit="True" Inherits="DotNetNuke.Modules.Admin.LogViewer.EditLogTypes" CodeFile="EditLogTypes.ascx.vb" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.UI.WebControls" Assembly="DotNetNuke" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="SectionHead" Src="~/controls/SectionHeadControl.ascx" %>
<asp:panel id="pnlLogTypeConfigInfo" runat="server">
	<asp:datagrid id="dgLogTypeConfigInfo" runat="server" autogeneratecolumns="false" cellpadding="4"
		datakeyfield="ID" border="0" summary="This table shows the log configuration entries.">
		<itemstyle horizontalalign="Left" />
		<columns>
		    <dnn:imagecommandcolumn CommandName="Edit" ImageUrl="~/images/edit.gif" KeyField="ID" />
			<asp:boundcolumn headertext="LogType" datafield="LogTypeKey" headerstyle-cssclass="NormalBold" itemstyle-cssclass="Normal" />
			<asp:boundcolumn headertext="Portal" datafield="LogTypePortalID" headerstyle-cssclass="NormalBold"
				itemstyle-cssclass="Normal" />
			<asp:boundcolumn headertext="Active" datafield="LoggingIsActive" headerstyle-cssclass="NormalBold"
				itemstyle-cssclass="Normal" />
			<asp:boundcolumn headertext="FileName" datafield="LogFilename" headerstyle-cssclass="NormalBold"
				itemstyle-cssclass="Normal" />
		</columns>
	</asp:datagrid>
	<P>
		<asp:linkbutton class="CommandButton" id="cmdReturn" runat="server" resourcekey="cmdReturn" text="Return"
			causesvalidation="False" borderstyle="none">Return</asp:linkbutton></P>
</asp:panel>
<asp:panel id="pnlEditLogTypeConfigInfo" runat="server">
	<dnn:sectionhead id="dshSettings" runat="server" cssclass="Head" includeRule="True" resourcekey="Settings"
		section="tblSettings" text="Logging Settings"></dnn:sectionhead>
	<TABLE id="tblSettings" cellSpacing="2" cellPadding="2" border="0" runat="server">
		<TR>
			<TD class="SubHead" width="150">
				<dnn:label id="plIsActive" runat="server" controlname="chkIsActive" suffix=":"></dnn:label></TD>
			<TD class="Normal">
				<asp:checkbox id="chkIsActive" runat="server" autopostback="True"></asp:checkbox></TD>
		</TR>
		<TR>
			<TD class="SubHead" width="150">
				<dnn:label id="plLogTypeKey" runat="server" controlname="ddlLogTypeKey" suffix=":"></dnn:label></TD>
			<TD class="Normal">
				<asp:dropdownlist id="ddlLogTypeKey" runat="server"></asp:dropdownlist></TD>
		</TR>
		<TR>
			<TD class="SubHead" width="150">
				<dnn:label id="plLogTypePortalID" runat="server" controlname="ddlLogTypePortalID" suffix=":"></dnn:label></TD>
			<TD class="Normal">
				<asp:dropdownlist id="ddlLogTypePortalID" runat="server"></asp:dropdownlist></TD>
		</TR>
		<TR>
			<TD class="SubHead" width="150">
				<dnn:label id="plKeepMostRecent" runat="server" controlname="ddlKeepMostRecent" suffix=":"></dnn:label></TD>
			<TD class="Normal">
				<asp:dropdownlist id="ddlKeepMostRecent" runat="server"></asp:dropdownlist></TD>
		</TR>
		<TR>
			<TD class="SubHead" width="150">
				<dnn:label id="plFileName" runat="server" controlname="txtFileName" suffix=":"></dnn:label></TD>
			<TD class="Normal">
				<asp:textbox id="txtFileName" runat="server" width="300"></asp:textbox></TD>
		</TR>
	</TABLE>
	<BR>
	<dnn:sectionhead id="dshEmailSettings" runat="server" cssclass="Head" includeRule="True" resourcekey="EmailSettings"
		section="tblEmailSettings" text="Email Notification Settings"></dnn:sectionhead>
	<TABLE id="tblEmailSettings" cellSpacing="2" cellPadding="2" border="0" runat="server">
		<TR>
			<TD class="SubHead" width="180">
				<dnn:label id="plEmailNotificationStatus" runat="server" controlname="chkEmailNotificationStatus"
					suffix=":"></dnn:label></TD>
			<TD class="Normal">
				<asp:checkbox id="chkEmailNotificationStatus" runat="server" autopostback="True"></asp:checkbox></TD>
		</TR>
		<TR>
			<TD class="SubHead" width="180">
				<dnn:label id="plThreshold" runat="server" controlname="ddlThreshold" suffix=":"></dnn:label></TD>
			<TD class="Normal">
				<asp:dropdownlist id="ddlThreshold" runat="server"></asp:dropdownlist>&nbsp;
				<asp:Label id="lblIn" runat="server" resourcekey="In" Font-Bold="True">in</asp:Label>&nbsp;
				<asp:dropdownlist id="ddlThresholdNotificationTime" runat="server">
					<asp:listitem value="1" />
					<asp:listitem value="2" />
					<asp:listitem value="3" />
					<asp:listitem value="4" />
					<asp:listitem value="5" />
					<asp:listitem value="6" />
					<asp:listitem value="7" />
					<asp:listitem value="8" />
					<asp:listitem value="9" />
					<asp:listitem value="10" />
					<asp:listitem value="20" />
					<asp:listitem value="30" />
					<asp:listitem value="60" />
					<asp:listitem value="90" />
					<asp:listitem value="120" />
				</asp:dropdownlist>
				<asp:dropdownlist id="ddlThresholdNotificationTimeType" runat="server">
					<asp:listitem value="1" resourcekey="Seconds">Seconds</asp:listitem>
					<asp:listitem value="2" resourcekey="Minutes">Minutes</asp:listitem>
					<asp:listitem value="3" resourcekey="Hours">Hours</asp:listitem>
					<asp:listitem value="4" resourcekey="Days">Days</asp:listitem>
				</asp:dropdownlist></TD>
		</TR>
		<TR>
			<TD class="SubHead" width="180">
				<dnn:label id="plMailFromAddress" runat="server" controlname="txtMailFromAddress" suffix=":"></dnn:label></TD>
			<TD class="Normal">
				<asp:textbox id="txtMailFromAddress" runat="server" width="300"></asp:textbox></TD>
		</TR>
		<TR>
			<TD class="SubHead" width="180">
				<dnn:label id="plMailToAddress" runat="server" controlname="txtMailToAddress" suffix=":"></dnn:label></TD>
			<TD class="Normal">
				<asp:textbox id="txtMailToAddress" runat="server" width="300"></asp:textbox></TD>
		</TR>
	</TABLE>
	<P>
		<asp:linkbutton class="CommandButton" id="cmdUpdate" runat="server" resourcekey="cmdUpdate" text="Update"></asp:linkbutton>&nbsp;
		<asp:linkbutton class="CommandButton" id="cmdCancel" runat="server" resourcekey="cmdCancel" text="Cancel"
			causesvalidation="False" borderstyle="none"></asp:linkbutton>&nbsp;
		<asp:linkbutton class="CommandButton" id="cmdDelete" runat="server" resourcekey="cmdDelete" text="Delete"
			causesvalidation="False"></asp:linkbutton>&nbsp;
	</P>
</asp:panel>
