<%@ Control language="vb" AutoEventWireup="false" Explicit="True" Inherits="DotNetNuke.Modules.Admin.Languages.EditLanguage" CodeFile="EditLanguage.ascx.vb" %>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke" Namespace="DotNetNuke.UI.WebControls" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<table cellspacing="0" cellpadding="0" border="0">
    <tr id="trCoreLanguage" runat="server">
        <td colspan="2">
            <dnn:propertyeditorcontrol id="ctlLanguage" runat="Server"
                AutoGenerate="false"
                SortMode="SortOrderAttribute"
                editcontrolstyle-cssclass="NormalTextBox" 
                editcontrolwidth="325px" 
                ErrorStyle-cssclass="NormalRed"
                helpstyle-cssclass="Help" 
                labelstyle-cssclass="SubHead" 
                labelwidth="150px" 
                width="475px">
                <Fields>
                    <dnn:FieldEditorControl ID="fldCode" runat="server" DataField="Code" EditControlStyle-Width="300px" LabelMode="Top" EditorTypeName="DotNetNuke.UI.WebControls.DNNLocaleEditControl" />
                    <dnn:FieldEditorControl ID="fldFallback" runat="server" DataField="Fallback" EditControlStyle-Width="300px" LabelMode="Top" EditorTypeName="DotNetNuke.UI.WebControls.DNNLocaleEditControl" />
                </Fields>
            </dnn:propertyeditorcontrol>
        </td>
    </tr>
    <tr>
        <td class="SubHead" style="width:200px"><dnn:Label ID="plEnabled" runat="server" ControlName="chkEnabled" /></td>
        <td class="NormalTextBox" style="width:325px"><asp:CheckBox ID="chkEnabled" runat="server" /></td>
    </tr>
</table>
<p style="text-align:center">
    <dnn:commandbutton id="cmdUpdate" runat="server" class="CommandButton" ImageUrl="~/images/save.gif"  ResourceKey="cmdUpdate" />
    <dnn:commandbutton id="cmdDelete" runat="server" class="CommandButton" ImageUrl="~/images/delete.gif"  ResourceKey="cmdDelete" />
    <dnn:commandbutton id="cmdCancel" runat="server" class="CommandButton" ImageUrl="~/images/lt.gif"  ResourceKey="cmdCancel" />
</p>
