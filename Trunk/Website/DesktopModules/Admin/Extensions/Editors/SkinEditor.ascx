<%@ Control language="vb" AutoEventWireup="false" Explicit="True" Inherits="DotNetNuke.Modules.Admin.Features.SkinEditor" CodeFile="SkinEditor.ascx.vb" %>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke" Namespace="DotNetNuke.UI.WebControls" %>
<asp:Label ID="lblTitle" runat="server" cssClass="SubHead"/>
<asp:Panel ID="pnlHelp" runat="server">
    <br />
    <asp:Label ID="lblHelp" runat="server" cssClass="Normal" />
    <br /><br />
</asp:Panel>
<table cellspacing="0" cellpadding="4" border="0" summary="Module Definitions Design Table" style="width:100%;">
	<tr>
		<td>
            <dnn:propertyeditorcontrol id="ctlSkin" runat="Server"
                AutoGenerate="false"
                SortMode="SortOrderAttribute"
                editcontrolstyle-cssclass="NormalTextBox" 
                editcontrolwidth="425px" 
                ErrorStyle-cssclass="NormalRed"
                helpstyle-cssclass="Help" 
                labelstyle-cssclass="SubHead" 
                labelwidth = "200px" 
                width= "650px">
                <Fields>
                    <dnn:FieldEditorControl ID="fldName" runat="server" DataField="SkinName" EditControlStyle-Width="400px" />
                </Fields>
            </dnn:propertyeditorcontrol>
		</td>
	</tr>
</table>
