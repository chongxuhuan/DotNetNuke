<%@ Control language="vb" CodeFile="ModuleEditor.ascx.vb" AutoEventWireup="false" Explicit="True" Inherits="DotNetNuke.Modules.Admin.Features.ModuleEditor" %>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke" Namespace="DotNetNuke.Security.Permissions.Controls" %>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke" Namespace="DotNetNuke.UI.WebControls" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<asp:Panel ID="pnlHelp" runat="server">
    <asp:Label ID="lblHelp" runat="server" cssClass="Normal" />
    <br /><br />
</asp:Panel>
<asp:Panel ID="pnlDesktopModule" runat="server" Visible="False">
    <table>
        <tr>
            <td>
                <dnn:propertyeditorcontrol id="ctlDesktopModule" runat="Server"
                    AutoGenerate="false"
                    SortMode="SortOrderAttribute"
                    editcontrolstyle-cssclass="NormalTextBox" 
                    editcontrolwidth="450px" 
                    ErrorStyle-cssclass="NormalRed"
                    helpstyle-cssclass="Help" 
                    labelstyle-cssclass="SubHead" 
                    labelwidth="175px" 
                    width="650px">
                    <Fields>
                        <dnn:FieldEditorControl ID="fldName" runat="server" DataField="ModuleName" EditControlStyle-Width="250px" EditMode="View" />
                        <dnn:FieldEditorControl ID="fldFolder" runat="server" DataField="FolderName" EditControlStyle-Width="250px" />
                        <dnn:FieldEditorControl ID="fldControllerClass" runat="server" DataField="BusinessControllerClass" EditControlStyle-Width="400px"  />
                        <dnn:FieldEditorControl ID="fldDependencies" runat="server" DataField="Dependencies" EditControlStyle-Width="400px" />
                        <dnn:FieldEditorControl ID="fldPermissions" runat="server" DataField="Permissions" EditControlStyle-Width="400px" />
                        <dnn:FieldEditorControl ID="fldIsPortable" runat="server" DataField="IsPortable" EditMode="View" />
                        <dnn:FieldEditorControl ID="fldIsSearchable" runat="server" DataField="IsSearchable" EditMode="View" />
                        <dnn:FieldEditorControl ID="fldIsUpgradable" runat="server" DataField="IsUpgradeable" EditMode="View" />
                        <dnn:FieldEditorControl ID="fldIsPremium" runat="server" DataField="IsPremium" />
                    </Fields>
                </dnn:propertyeditorcontrol>
            </td>
        </tr>
        <tr>
            <td style="text-align:center">
                <dnn:DualListBox id="ctlPortals" runat="server" DataValueField="PortalID" DataTextField="PortalName" 
                    AddKey="AddPortal" RemoveKey="RemovePortal" AddAllKey="AddAllPortals" RemoveAllKey="RemoveAllPortals"
                    AddImageURL="~/images/rt.gif" AddAllImageURL="~/images/ffwd.gif" RemoveImageURL="~/images/lt.gif" 
                    RemoveAllImageURL="~/images/frev.gif" ContainerStyle-HorizontalAlign="Center" >
                    <AvailableListBoxStyle CssClass="NormalTextBox" Height="130px" Width="275px" />
                    <HeaderStyle CssClass="NormalBold" />
                    <SelectedListBoxStyle CssClass="NormalTextBox" Height="130px" Width="275px"  />
                </dnn:DualListBox>
            </td>
        </tr>
    </table>
</asp:Panel>
<asp:Panel ID="pnlPermissions" runat="server" Visible="false">
    <table cellspacing="0" cellpadding="4" border="0" style="width:475px">
        <tr>
            <td><dnn:Label ID="plPermissions" runat="server" cssClass="SubHead" ResourceKey="Permissions" ControlName="dgPermissions" /></td>
        </tr>
        <tr>
            <td colspan="2"><dnn:DesktopModulePermissionsGrid ID="dgPermissions" runat="server"  /></td>
        </tr>
    </table>
</asp:Panel>
<p style="text-align:center">
    <dnn:commandbutton id="cmdUpdate" text="Update" runat="server" class="CommandButton" ImageUrl="~/images/save.gif"  ResourceKey="cmdUpdate" />
</p>
<asp:Panel ID="pnlDefinitions" runat="server" Visible="False">
    <hr style="width:95%" />
    <table cellspacing="0" cellpadding="4" border="0" style="width:475px">
        <tr>
            <td colspan="2"><asp:Label ID="lblDefinitions" runat="server" cssClass="SubHead" ResourceKey="Definitions" /></td>
        </tr>
        <tr>
	        <td class="SubHead" style="width:150px"><dnn:label id="plSelectDefinition" controlname="cboDefinitions" runat="server" /></td>
	        <td style="width:325px">
	            <asp:dropdownlist id="cboDefinitions" runat="server" width="150px" cssclass="NormalTextBox" datatextfield="FriendlyName" 
	                    datavaluefield="ModuleDefId" autopostback="True" />&nbsp;&nbsp;
                <dnn:commandbutton id="cmdAddDefinition" resourcekey="cmdAddDefinition" runat="server"
	                class="CommandButton" ImageUrl="~/images/add.gif" CausesValidation="false" />
	        </td>
        </tr>
        <tr><td style="height:10px"></td></tr>
    </table>
    <asp:Panel ID="pnlDefinition" runat="server" Visible="false">
        <dnn:propertyeditorcontrol id="ctlDefinition" runat="Server"
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
                <dnn:FieldEditorControl ID="fldFriendlyName" runat="server" DataField="FriendlyName" EditControlStyle-Width="250px" Required="true" />
                <dnn:FieldEditorControl ID="fldCacheTime" runat="server" DataField="DefaultCacheTime" EditControlStyle-Width="100px"  />
            </Fields>
        </dnn:propertyeditorcontrol>    
        <asp:Label ID="lblDefinitionError" runat="server" CssClass="NormalRed" Visible="false" ResourceKey="DuplicateName" />  
        <p style="text-align:center">
            <dnn:commandbutton id="cmdDeleteDefinition" resourcekey="cmdDeleteDefinition" runat="server" class="CommandButton" ImageUrl="~/images/delete.gif" />&nbsp;&nbsp;
            <dnn:commandbutton id="cmdUpdateDefinition" runat="server" class="CommandButton" ImageUrl="~/images/save.gif" />
        </p>
        <asp:Panel ID="pnlControls" runat="server" Visible="false">
            <hr style="width:95%" />
            <table cellspacing="0" cellpadding="4" border="0" style="width:475px">
                <tr>
                    <td><asp:Label ID="lblControls" runat="server" cssClass="SubHead" ResourceKey="Controls" /></td>
                </tr>
                <tr>
	                <td>
			            <asp:datagrid id="grdControls" runat="server" width="100%" border="0" cellspacing="3" 
			                autogeneratecolumns="false" enableviewstate="true" summary="Module Controls Design Table" 
			                GridLines="None" BorderWidth="0px">
				            <HeaderStyle CssClass="NormalBold" />
				            <ItemStyle CssClass="Normal" />
				            <Columns>
                                <dnn:imagecommandcolumn headerStyle-width="20px" CommandName="Edit" ImageUrl="~/images/edit.gif" EditMode="URL" KeyField="ModuleControlID" />
                                <dnn:imagecommandcolumn headerStyle-width="20px" commandname="Delete" imageurl="~/images/delete.gif" keyfield="ModuleControlID" />
                                <dnn:textcolumn  DataField="ControlKey" HeaderText="Control" />
                                <dnn:textcolumn  DataField="ControlTitle" HeaderText="Title" />
                                <dnn:textcolumn  DataField="ControlSrc" HeaderText="Source" />
				            </Columns>
			            </asp:datagrid>
	                </td>
                </tr>
            </table>
            <p style="text-align:center">
                <dnn:commandbutton id="cmdAddControl" resourcekey="cmdAddControl" runat="server" class="CommandButton" ImageUrl="~/images/add.gif" />&nbsp;&nbsp;
        </asp:Panel>
    </asp:Panel>
</asp:Panel>
