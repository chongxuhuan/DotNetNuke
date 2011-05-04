<%@ Control Language="C#" AutoEventWireup="false" Inherits="DotNetNuke.Modules.Dashboard.Controls.Database" CodeFile="Database.ascx.cs" %>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke" Namespace="DotNetNuke.UI.WebControls"%>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>

<dnn:Label id="plDbInfo" runat="Server" CssClass="Head" ControlName="ctlDbInfo" />
<dnn:propertyeditorcontrol id="ctlDbInfo" runat="Server"
    autogenerate = "false"
    enableclientvalidation = "true"
    sortmode="SortOrderAttribute" 
    labelstyle-cssclass="SubHead" 
    helpstyle-cssclass="Help" 
    editcontrolstyle-cssclass="NormalTextBox" 
    labelwidth="200px" 
    editcontrolwidth="450px" 
    editmode="View" 
    errorstyle-cssclass="NormalRed">
    <Fields>
        <dnn:FieldEditorControl ID="fldProductVersion" runat="server" DataField="ProductVersion" />
        <dnn:FieldEditorControl ID="fldServicePack" runat="server" DataField="ServicePack" />
        <dnn:FieldEditorControl ID="fldProductEdition" runat="server" DataField="ProductEdition" />
        <dnn:FieldEditorControl ID="fldSoftwarePlatform" runat="server" DataField="SoftwarePlatform" />
    </Fields>
</dnn:propertyeditorcontrol>    
<br />
<dnn:Label id="plBackups" runat="Server" CssClass="Head" ControlName="grdBackups" />
<asp:GridView ID="grdBackups" runat="server" GridLines="None" 
    AutoGenerateColumns="false"  EnableViewState="False">
    <Columns>
        <asp:BoundField DataField="name" HeaderText="BackupName" />
        <asp:BoundField DataField="StartDate" DataFormatString="{0:d}" HeaderText="Started" />
        <asp:BoundField DataField="FinishDate" DataFormatString="{0:d}" HeaderText="Finished" />
        <asp:BoundField DataField="Size" DataFormatString="{0:n}" HeaderText="BackupSize" />
        <asp:BoundField DataField="BackupType" HeaderText="BackupType" />
    </Columns>
    <EmptyDataTemplate>
        <%= NoBackups %>
    </EmptyDataTemplate>
</asp:GridView>
<br />
<dnn:Label id="plFiles" runat="Server" CssClass="Head" ControlName="grdFiles" />
<asp:GridView ID="grdFiles" runat="server" GridLines="None" 
    AutoGenerateColumns="False" EnableViewState="False">
    <Columns>
        <asp:BoundField DataField="Name" HeaderText="Name" />
        <asp:BoundField DataField="Megabytes" DataFormatString="{0:n} Mb" HeaderText="Size" />
        <asp:BoundField DataField="FileType" HeaderText="FileType" />
        <asp:TemplateField HeaderText="FileName">
            <ItemTemplate>
                <asp:Label ID="Label1" runat="server" Text='<%# Bind("ShortFileName") %>'></asp:Label>
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
    <EmptyDataTemplate>
        <%= NoBackups %>
    </EmptyDataTemplate>
</asp:GridView>