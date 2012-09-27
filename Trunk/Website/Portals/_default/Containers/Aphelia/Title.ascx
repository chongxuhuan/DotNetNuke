<%@ Control language="vb" AutoEventWireup="false" Explicit="True" Inherits="DotNetNuke.UI.Containers.Container" %>
<%@ Register TagPrefix="dnn" TagName="TITLE" Src="~/Admin/Containers/Title.ascx" %>
<%@ Register TagPrefix="dnn" TagName="VISIBILITY" Src="~/Admin/Containers/Visibility.ascx" %>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke" Namespace="DotNetNuke.UI.Containers" %>
<div class="DNNContainer_with_title">
    <h2><dnn:TITLE runat="server" id="dnnTITLE" /></h2>
    <div id="ContentPane" runat="server"></div>
    <div class="dnnActionButtons">
        <dnn:ActionCommandButton runat="server" CommandName="PrintModule.Action" DisplayIcon="True" DisplayLink="False" />
		<dnn:ActionCommandButton runat="server" CommandName="SyndicateModule.Action" DisplayIcon="True" DisplayLink="False" />
    </div>
</div>