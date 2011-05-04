<%@ Control Language="C#" AutoEventWireup="false" Inherits="DotNetNuke.Modules.Admin.Search.SearchAdmin" CodeFile="SearchAdmin.ascx.cs" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.UI.WebControls" Assembly="DotNetNuke" %>

<div class="dnnForm dnnEditSchedule dnnClear" id="dnnEditSchedule">
    <fieldset>
        <div class="dnnFormItem">
            <dnn:label id="plMaxWordLength" runat="server" controlname="txtMaxWordLength" />
            <asp:textbox id="txtMaxWordLength" runat="server" maxlength="128"/>
        </div>
        <div class="dnnFormItem">
            <dnn:label id="plMinWordLength" runat="server" controlname="txtMinWordLength" />
            <asp:textbox id="txtMinWordLength" runat="server" maxlength="128" />
        </div>
        <div class="dnnFormItem">
            <dnn:label id="plIncludeCommon" runat="server" controlname="chkIncludeCommon" text="Include Common Words:"></dnn:label>
            <asp:CheckBox ID="chkIncludeCommon" Runat="server" CssClass="Normal"></asp:CheckBox>
        </div>
        <div class="dnnFormItem">
            <dnn:label id="plIncludeNumeric" runat="server" controlname="chkIncludeNumeric" text="Include Numbers:"></dnn:label>
            <asp:CheckBox ID="chkIncludeNumeric" Runat="server" CssClass="Normal"></asp:CheckBox>
        </div>
    </fieldset>
    <ul class="dnnActions dnnClear">
    	<li><asp:LinkButton id="cmdUpdate" runat="server" CssClass="dnnPrimaryAction" resourcekey="cmdUpdate" /></li>
        <li><asp:LinkButton id="cmdReIndex" runat="server" CssClass="dnnSecondaryAction" resourcekey="cmdReIndex" Causesvalidation="False" /></li>
    </ul>
</div>