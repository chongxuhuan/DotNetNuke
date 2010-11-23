<%@ Control language="vb" Inherits="DotNetNuke.Modules.Html.EditHtml" CodeBehind="EditHtml.ascx.vb" AutoEventWireup="false" Explicit="True" %>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke.Web" Namespace="DotNetNuke.Web.UI.WebControls" %>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke" Namespace="DotNetNuke.UI.WebControls" %>
<%@ Register TagPrefix="dnn" TagName="sectionhead" Src="~/controls/SectionHeadControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="texteditor" Src="~/controls/texteditor.ascx" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>

<dnn:sectionhead cssclass="Head" id="dshMaster" includerule="True" isexpanded="True" resourcekey="dshMaster" runat="server" section="tblMaster" text="" visible="false" />
<table id="tblMaster" cellspacing="2" cellpadding="2" summary="Master Design Table" border="0" runat="server" visible="false">
	<tr>
		<td colspan="2"><div class="html_preview"><asp:placeholder id="placeMasterContent" runat="server"/></div></td>
	</tr>
</table>
 <br /><br />
<dnn:sectionhead id="dshCurrentContent" cssclass="Head" runat="server" section="tblEdit" resourcekey="dshCurrentContent" includerule="True" isexpanded="True" />
<table id="tblEdit" cellspacing="2" cellpadding="2" summary="Review Design Table" border="0" runat="server" width="100%">
	<tr valign="bottom">
		<td>
			<table cellspacing="2" cellpadding="2" summary="Edit HTML Design Table" border="0" width="100%">
				<tr>
					<td colspan="2" valign="top">
						<dnn:texteditor id="txtContent" runat="server" height="400" width="100%"/>
					</td>
				</tr>
				<tr id="rowSubmittedContent" runat="server" visible="false">
					<td colspan="2">
						<div id="Div1" class="html_preview" runat="server"><asp:Literal ID="litCurrentContentPreview" runat="server" /></div>        
					</td>
				</tr>
				<tr id="rowCurrentVersion" runat="server">
					<td class="SubHead" valign="top" ><dnn:label id="plCurrentWorkVersion" runat="server" controlname="lblCurrentVersion" text="Version" suffix=":"/></td>
					<td><asp:Label ID="lblCurrentVersion" runat="server" /></td>
				</tr>
				<tr>
					<td class="SubHead" valign="top" width="150px"><dnn:label id="plCurrentWorkflowInUse" runat="server" controlname="lblCurrentWorkflowInUse" text="Workflow in Use" suffix=":"/></td>
					<td><asp:Label ID="lblCurrentWorkflowInUse" runat="server" /></td>
				</tr>
				<tr id="rowCurrentWorkflowState" runat="server">
					<td class="SubHead" valign="top" ><dnn:label id="plCurrentWorkflowState" runat="server" controlname="lblCurrentWorkflowState" text="Workflow State" suffix=":"/></td>
					<td><asp:Label ID="lblCurrentWorkflowState" runat="server" /></td>
				</tr>
				<tr id="rowPublish" runat="server">
					<td class="SubHead" valign="top"><dnn:label id="plActionOnSave" runat="server" text="On Save" suffix="?"/></td>
					<td valign="top">
						<asp:checkbox id="chkPublish" runat="server" class="CommandButton" resourcekey="chkPublish"/><br />
					</td>
				</tr>
			</table>
			<br />
			<p>
				<dnn:commandbutton id="cmdSave" runat="server" class="CommandButton" resourcekey="cmdSave" imageurl="~/images/save.gif" />&nbsp;
				<dnn:commandbutton id="cmdCancel" runat="server" class="CommandButton" resourcekey="cmdCancel" causesvalidation="False" imageurl="~/images/lt.gif"  />&nbsp;
				<dnn:commandbutton id="cmdPreview" runat="server" class="CommandButton" resourcekey="cmdPreview" imageurl="~/images/view.gif" />&nbsp;
              </p>
		</td>
	</tr>
</table>

<dnn:sectionhead id="dshPreview" cssclass="Head" runat="server" text="Preview Content" section="tblPreview" resourcekey="dshPreview" includerule="True" isexpanded="False" />
<table id="tblPreview" cellspacing="2" cellpadding="2" summary="Preview Design Table" border="0" runat="server" width="550px">
	<tr id="rowPreviewVersion" runat="server">
		<td class="SubHead" width="150" valign="top"><dnn:label id="plPreviewVersion" runat="server" controlname="lblPreviewVersion" text="Version" suffix=":"></dnn:label></td>
		<td valign="top"><asp:label id="lblPreviewVersion" runat="server" cssclass="Normal" /></td>
	</tr>
	<tr id="rowPreviewWorlflow" runat="server">
		<td class="SubHead" width="150" valign="top"><dnn:label id="plPreviewWorkflowInUse" runat="server" controlname="lblPreviewWorkflowInUse" text="Workflow" suffix=":"></dnn:label></td>
		<td align="left"><asp:label id="lblPreviewWorkflowInUse" runat="server" cssclass="Normal" /></td>
	</tr>
	<tr id="rowPreviewWorkflowState" runat="server">
		<td class="SubHead" width="150" valign="top"><dnn:label id="plPreviewWorkflowState" runat="server" controlname="lblPreviewWorkflowState" text="State" suffix=":"/></td>
		<td align="left"><asp:Label ID="lblPreviewWorkflowState" runat="server" /></td>
	</tr>

	<tr>
		<td colspan="2"><div id="Div2" class="html_preview" runat="server"><asp:Literal ID="litPreview" runat="server" /></div></td>
	</tr>
	<tr>
		<td colspan="2">
		<table>
			<tr>
				<td width="20px">&nbsp;</td>
				<td>
					<dnn:sectionhead id="dshHistory" cssclass="Head" runat="server" text="Item History" section="tblHistory" resourcekey="dshHistory" isexpanded="False" />
					<table id="tblHistory" width="100%" cellspacing="2" cellpadding="2" summary="History Design Table" border="0" runat="server">
						<tr valign="bottom">
							<td>
								<dnn:dnngrid ID="grdLog" runat="server" AutoGenerateColumns="false">
								  <MasterTableView>
									<Columns>
										<dnn:DnnGridBoundColumn HeaderText="Date" DataField="CreatedOnDate" />
										<dnn:DnnGridBoundColumn HeaderText="User" DataField="DisplayName"/>
										<dnn:DnnGridBoundColumn HeaderText="State" DataField="StateName"/>
									</Columns>
									</MasterTableView>
								 </dnn:dnngrid>
							</td>
						</tr>
					</table>
				</td>
			</tr>
		</table>
		</td>
	</tr>
</table>

<dnn:sectionhead id="dshVersions" cssclass="Head" runat="server" text="Version History" section="tblVersions" resourcekey="dshVersions" includerule="True" isexpanded="False" />
<table id="tblVersions" cellspacing="2" cellpadding="2" summary="History Design Table" border="0" runat="server" width="550px">
	<tr>
		<td class="SubHead" valign="top" width="300px"><dnn:label id="plMaxVersions" runat="server" controlname="lblMaxVersions" text="Maximum number of published versions" suffix=":"></dnn:label></td>
		<td>
			<asp:Label ID="lblMaxVersions" runat="server" />
		</td>
	</tr>
	<tr valign="bottom">
		<td colspan="2">
			<dnn:dnngrid ID="grdVersions" runat="server" AutoGenerateColumns="false" AllowPaging="True" >
				<PagerStyle Mode="NextPrevAndNumeric"></PagerStyle>
				 <MasterTableView>
					<Columns>
						<dnn:DnnGridBoundColumn HeaderText="Version" DataField="Version" />
						<dnn:DnnGridBoundColumn HeaderText="Date" DataField="LastModifiedOnDate"  />
						<dnn:DnnGridBoundColumn HeaderText="User" DataField="DisplayName" />
						<dnn:DnnGridBoundColumn HeaderText="State" DataField="StateName" />
                        <dnn:DnnGridTemplateColumn>
                            <HeaderTemplate>
                                <table cellpadding="2px" class="DnnGridNestedTable">
                                    <tr>
                                        <td><asp:Image ID="imgDelete" runat="server" ImageUrl="~/images/action_delete.gif" resourcekey="VersionsRemove"/></td>
                                        <td><asp:Image ID="imgPreview" runat="server" ImageUrl="~/images/view.gif"  resourcekey="VersionsPreview"/></td>
                                        <td><asp:Image ID="imgRollback" runat="server" ImageUrl="~/images/restore.gif"  resourcekey="VersionsRollback"/></td>
                                    </tr>
                                </table>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <table cellpadding="2px" class="DnnGridNestedTable">
                                    <tr style="vertical-align: top;">
                                        <td><asp:ImageButton ID="btnRemove" runat="server" CommandName="Remove" ImageUrl="~/images/action_delete.gif" Text="Delete" resourcekey="VersionsRemove"/></td>
                                        <td><asp:ImageButton ID="btnPreview" runat="server" CommandName="Preview"  ImageUrl="~/images/view.gif" Text="Preview" resourcekey="VersionsPreview"/></td>
                                        <td><asp:ImageButton ID="btnRollback" runat="server" CommandName="RollBack" ImageUrl="~/images/restore.gif" Text="Rollback" resourcekey="VersionsRollback" /></td>
                                    </tr>
                                </table>
                            </ItemTemplate>
                        </dnn:DnnGridTemplateColumn>
					</Columns>
				</MasterTableView>
			</dnn:dnngrid>
		</td>
	</tr>
</table>


