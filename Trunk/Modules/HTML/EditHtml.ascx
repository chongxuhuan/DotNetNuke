<%@ Control language="vb" Inherits="DotNetNuke.Modules.Html.EditHtml" CodeBehind="EditHtml.ascx.vb" AutoEventWireup="false" Explicit="True" %>
<%@ Register TagPrefix="dnn" TagName="SectionHead" Src="~/controls/SectionHeadControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="TextEditor" Src="~/controls/TextEditor.ascx"%>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.UI.WebControls" Assembly="DotNetNuke" %>

<dnn:sectionhead id="dshMaster" cssclass="Head" runat="server" section="tblMaster" resourcekey="dshMaster" includerule="True" isexpanded="True" />
<table id="tblMaster" cellspacing="2" cellpadding="2" summary="Master Design Table" border="0" runat="server">
    <tr>
        <td colspan="2"><asp:label id="lblMaster" runat="server"/></td>
    </tr>
</table>
 
<table id="tblEdit" cellspacing="2" cellpadding="2" summary="Review Design Table" border="0" runat="server" width ="100%">
    <tr valign="bottom">
        <td>
            <p>
            <table width="100%" cellspacing="2" cellpadding="2" summary="Edit HTML Design Table" border="0">
                <tr>
                    <td colspan="2" valign="top"><dnn:texteditor id="txtContent" runat="server" height="400" width="100%"/></td>
                </tr>
                <tr id="rowPublish" runat="server">
                    <td class="SubHead" width="150" valign="top"><dnn:label id="plPublish" runat="server" controlname="chkPublish" text="Publish" suffix="?"/></td>
                    <td width="400" valign="top"><asp:checkbox id="chkPublish" runat="server" class="CommandButton" /></td>
                </tr>
            </table>
            </p>
            <br />
            <p>
                <dnn:commandbutton id="cmdSave" runat="server" class="CommandButton" resourcekey="cmdSave" imageurl="~/images/save.gif" />&nbsp;
                <dnn:commandbutton id="cmdCancel" runat="server" class="CommandButton" resourcekey="cmdCancel" causesvalidation="False" imageurl="~/images/lt.gif"  />&nbsp;
                <dnn:commandbutton id="cmdPreview" runat="server" class="CommandButton" resourcekey="cmdPreview" imageurl="~/images/view.gif" />&nbsp;
            </p>
        </td>
    </tr>
</table>
<br/>
<dnn:sectionhead id="dshReview" cssclass="Head" runat="server" text="Review Content" section="tblReview" resourcekey="dshReview" includerule="True" isexpanded="True" />
<table id="tblReview" width="550" cellspacing="2" cellpadding="2" summary="Review Design Table" border="0" runat="server">
    <tr valign="bottom">
        <td>
            <table width="550" cellspacing="2" cellpadding="2" summary="Review HTML Design Table" border="0">
                <tr>
                    <td class="SubHead" width="150" valign="top"><dnn:label id="plComment" runat="server" controlname="txtComment" text="Comment" suffix=":"></dnn:label></td>
                    <td valign="top"><asp:textbox id="txtComment" runat="server" cssclass="NormalTextBox" style="width:400px" textmode="multiline" rows="5"></asp:textbox></td>
                </tr>
            </table>
            <p>
                <dnn:commandbutton id="cmdReview" runat="server" class="CommandButton" imageurl="~/images/save.gif" />&nbsp;
                <dnn:commandbutton id="cmdCancel2" runat="server" class="CommandButton" resourcekey="cmdCancel" imageurl="~/images/lt.gif" causesvalidation="False"/>&nbsp;
            </p>
        </td>
    </tr>
</table>
<br/>
<dnn:sectionhead id="dshVersions" cssclass="Head" runat="server" text="Version History" section="tblVersions" resourcekey="dshVersions" includerule="True" isexpanded="False" />
<table id="tblVersions" width="550" cellspacing="2" cellpadding="2" summary="History Design Table" border="0" runat="server">
    <tr valign="bottom">
        <td>
            <asp:DataGrid ID="grdVersions" runat="server" Width="100%" AutoGenerateColumns="false" CellPadding="2" GridLines="None" cssclass="DataGrid_Container">
	            <headerstyle cssclass="DataGrid_Header" verticalalign="Top" horizontalalign="Center"/>
	            <itemstyle CssClass="DataGrid_Item" horizontalalign="Center" />
	            <alternatingitemstyle cssclass="DataGrid_AlternatingItem" />
	            <Columns>
		            <dnn:imagecommandcolumn CommandName="Item" ImageUrl="~/images/view.gif" KeyField="ItemID" Text="Preview" />
		            <dnn:imagecommandcolumn CommandName="Edit" ImageUrl="~/images/restore.gif" KeyField="ItemID" Text="Rollback" />
		            <asp:BoundColumn HeaderText="Version" DataField="Version" ItemStyle-CssClass="Normal" HeaderStyle-Cssclass="NormalBold" />
		            <asp:BoundColumn HeaderText="Date" DataField="LastModifiedOnDate" ItemStyle-CssClass="Normal" HeaderStyle-Cssclass="NormalBold" />
		            <asp:BoundColumn HeaderText="User" DataField="DisplayName" ItemStyle-CssClass="Normal" HeaderStyle-Cssclass="NormalBold" />
		            <asp:BoundColumn HeaderText="State" DataField="StateName" ItemStyle-CssClass="Normal" HeaderStyle-Cssclass="NormalBold" />
	            </Columns>
            </asp:DataGrid>
        </td>
    </tr>
</table>
<dnn:sectionhead id="dshHistory" cssclass="Head" runat="server" text="Item History" section="tblHistory" resourcekey="dshHistory" includerule="True" isexpanded="False" />
<table id="tblHistory" width="550" cellspacing="2" cellpadding="2" summary="History Design Table" border="0" runat="server">
    <tr valign="bottom">
        <td>
            <table cellspacing="2" width="550" cellpadding="2" summary="History Design Table" border="0" width="100%">
                <tr>
                    <td class="SubHead" width="150" valign="top"><dnn:label id="plVersion" runat="server" controlname="lblVersion" text="Version" suffix=":"></dnn:label></td>
                    <td valign="top"><asp:label id="lblVersion" runat="server" cssclass="Normal" /></td>
                </tr>
                <tr>
                    <td class="SubHead" width="150" valign="top"><dnn:label id="plWorkflow" runat="server" controlname="lblWorkflow" text="Workflow" suffix=":"></dnn:label></td>
                    <td valign="top"><asp:label id="lblWorkflow" runat="server" cssclass="Normal" /></td>
                </tr>
                <tr>
                    <td class="SubHead" width="150" valign="top"><dnn:label id="plState" runat="server" controlname="lblState" text="Current State" suffix=":"></dnn:label></td>
                    <td valign="top"><asp:label id="lblState" runat="server" cssclass="Normal" /></td>
                </tr>
            </table>
            <br/>
            <asp:DataGrid ID="grdLog" runat="server" Width="100%" AutoGenerateColumns="false" CellPadding="2" GridLines="None" cssclass="DataGrid_Container">
	            <headerstyle cssclass="DataGrid_Header" verticalalign="Top" horizontalalign="Center"/>
	            <itemstyle CssClass="DataGrid_Item" horizontalalign="Center" />
	            <alternatingitemstyle cssclass="DataGrid_AlternatingItem" />
	            <Columns>
		            <asp:BoundColumn HeaderText="Date" DataField="CreatedOnDate" ItemStyle-CssClass="Normal" HeaderStyle-Cssclass="NormalBold" />
		            <asp:BoundColumn HeaderText="User" DataField="DisplayName" ItemStyle-CssClass="Normal" HeaderStyle-Cssclass="NormalBold" />
		            <asp:BoundColumn HeaderText="State" DataField="StateName" ItemStyle-CssClass="Normal" HeaderStyle-Cssclass="NormalBold" />
		            <asp:BoundColumn HeaderText="Approved" DataField="Approved" ItemStyle-CssClass="Normal" HeaderStyle-Cssclass="NormalBold" />
		            <asp:BoundColumn HeaderText="" DataField="Comment" ItemStyle-CssClass="Normal" HeaderStyle-Cssclass="NormalBold" />
	            </Columns>
            </asp:datagrid>
        </td>
    </tr>
</table>
<dnn:sectionhead id="dshPreview" cssclass="Head" runat="server" text="Preview Content" section="tblPreview" resourcekey="dshPreview" includerule="True" isexpanded="False" />
<table id="tblPreview" cellspacing="2" cellpadding="2" summary="Preview Design Table" border="0" runat="server">
    <tr>
        <td colspan="2"><asp:label id="lblPreview" runat="server"/></td>
    </tr>
</table>

