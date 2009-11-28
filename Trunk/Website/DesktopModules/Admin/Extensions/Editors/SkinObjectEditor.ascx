<%@ Control language="vb" AutoEventWireup="false" Explicit="True" Inherits="DotNetNuke.Modules.Admin.SkinObjects.SkinObjectEditor" CodeFile="SkinObjectEditor.ascx.vb" %>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke" Namespace="DotNetNuke.UI.WebControls" %>
<asp:Label ID="lblTitle" runat="server" cssClass="SubHead" resourcekey="Title" />
<asp:Panel ID="pnlHelp" runat="server">
    <br />
    <asp:Label ID="lblHelp" runat="server" cssClass="Normal" resourcekey="Help" />
    <br /><br />
</asp:Panel>
<table cellspacing="0" cellpadding="4" border="0" summary="Module Definitions Design Table" style="width:100%;">
	<tr>
		<td>
            <dnn:propertyeditorcontrol id="ctlSkinObject" runat="Server"
                AutoGenerate="false"
                SortMode="SortOrderAttribute"
                editcontrolstyle-cssclass="NormalTextBox" 
                editcontrolwidth="325px" 
                ErrorStyle-cssclass="NormalRed"
                helpstyle-cssclass="Help" 
                labelstyle-cssclass="SubHead" 
                labelwidth = "250px" 
                width= "600px">
                <Fields>
                    <dnn:FieldEditorControl ID="fldKey" runat="server" DataField="ControlKey" EditControlStyle-Width="100px" />
                    <dnn:FieldEditorControl ID="fldSrc" runat="server" DataField="ControlSrc" EditControlStyle-Width="300px" />
                    <dnn:FieldEditorControl ID="fldPartial" runat="server" DataField="SupportsPartialRendering" />
                </Fields>
            </dnn:propertyeditorcontrol>
		</td>
	</tr>
</table>
