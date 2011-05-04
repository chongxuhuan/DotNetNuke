<%@ Control Language="C#" AutoEventWireup="false" Inherits="DotNetNuke.Modules.Admin.Languages.LanguagePackWriter" CodeFile="LanguagePackWriter.ascx.cs" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.UI.WebControls" Assembly="DotNetNuke" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="SectionHead" Src="~/controls/SectionHeadControl.ascx" %>

<div class="dnnForm dnnLanguagePackWriter dnnClear" id="dnnLanguagePackWriter">
    <fieldset>
        <legend></legend>
        <div class="dnnFormItem">
            <dnn:label id="lbLocale" text="Resource Locale" controlname="cboLanguage" runat="server"/>
            <asp:dropdownlist id="cboLanguage" runat="server"/>
        </div>
        <div class="dnnFormItem">
            <dnn:label id="lblType" text="Resource Locale" controlname="cboLanguage" runat="server"/>
            <asp:radiobuttonlist id="rbPackType" runat="server" AutoPostBack="true" Repeatdirection="Horizontal" CssClass="lpwRadioButtons">
				<asp:ListItem resourcekey="Core.LangPackType" Value="Core" Selected="true">Core</asp:ListItem>
				<asp:ListItem resourcekey="Module.LangPackType" Value="Module">Module</asp:ListItem>
				<asp:ListItem resourcekey="Provider.LangPackType" Value="Provider">Provider</asp:ListItem>
				<asp:ListItem resourcekey="AuthSystem.LangPackType" Value="AuthSystem">Authentication System</asp:ListItem>
				<asp:ListItem resourcekey="Full.LangPackType" Value="Full">Full</asp:ListItem>
			</asp:radiobuttonlist>
        </div>
        <div  id="rowitems" runat="server" class="dnnFormItem">
 		    <asp:label id="lblItems" runat="server" CssClass="SubHead"></asp:label><br/>
			<asp:checkboxlist id="lstItems" runat="server" RepeatColumns="2" RepeatDirection="Horizontal"  CssClass="lpwCheckBoxes"/>
        </div>
        <div id="rowFileName" runat="server" class="dnnFormItem">
            <dnn:label id="lblName" text="Resource Locale" controlname="cboLanguage" runat="server"></dnn:label>
			<asp:Label id="Label2" runat="server" CssClass="Normal">ResourcePack.</asp:Label><asp:textbox id="txtFileName" runat="server" Width="200px">Core</asp:textbox>
			<asp:Label id="lblFilenameFix" runat="server" CssClass="Normal">.&lt;version&gt;.&lt;locale&gt;.zip</asp:Label>
        </div>
        <div class="dnnFormItem">
        </div>
    </fieldset>
    <ul class="dnnActions dnnClear">
    	<li><asp:LinkButton id="cmdCreate" runat="server" CssClass="dnnPrimaryAction" resourcekey="cmdCreate"/></li>
    	<li><asp:LinkButton id="cmdCancel" runat="server" CssClass="dnnSecondaryAction" ResourceKey="cmdCancel" CausesValidation="false" /></li>
    </ul>
</div>
<asp:panel id="pnlLogs" runat="server" Visible="False">
	<dnn:sectionhead id="dshBasic" runat="server" text="Language Pack Log" resourcekey="LogTitle" cssclass="Head"
		includerule="true" section="divLog"></dnn:sectionhead>
	<DIV id="divLog" runat="server">
		<asp:HyperLink id="hypLink" runat="server" CssClass="CommandButton"></asp:HyperLink>
		<HR>
		<asp:Label id="lblMessage" runat="server"></asp:Label></DIV>
</asp:panel>