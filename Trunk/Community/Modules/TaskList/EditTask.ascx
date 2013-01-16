<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EditTask.ascx.cs" Inherits="DotNetNuke.Modules.TaskList.EditTask" %>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke.Web" Namespace="DotNetNuke.Web.UI.WebControls" %>

<dnn:DnnFormEditor id="editTask" runat="server" FormMode="Short">
    <Items>
        <dnn:DnnFormTextBoxItem runat="server" DataField="Title"/>
        <dnn:DnnFormTextBoxItem runat="server" DataField="Description"/>
        <dnn:DnnFormToggleButtonItem runat="server" DataField="IsComplete"/>
    </Items>
</dnn:DnnFormEditor>

<ul class="dnnActions dnnClear">
	<li><asp:LinkButton id="save" runat="server" class="dnnPrimaryAction" resourcekey="Save" /></li>
	<li><asp:LinkButton id="delete" runat="server" class="dnnSecondaryAction" resourcekey="Delete" /></li>
	<li><asp:HyperLink id="cancel" runat="server" class="dnnSecondaryAction" resourcekey="Cancel" /></li>
</ul>
