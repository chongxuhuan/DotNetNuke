<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Control Inherits="DotNetNuke.Modules.Admin.SiteLog.SiteLog" Language="C#" AutoEventWireup="false" CodeFile="SiteLog.ascx.cs" %>
<div class="dnnForm dnnSiteLog dnnClear" id="dnnSiteLog">
    <div class="slContent dnnClear">
        <fieldset>
            <div class="dnnFormItem dnnClear"><asp:Label ID="lblMessage" runat="server" Visible="false" resourcekey="NoRecords" CssClass="dnnFormMessage dnnFormWarning" /></div>
            <div class="dnnFormItem">
                <dnn:label id="plReportType" runat="server" controlname="cboReportType" suffix=":" />
                <asp:DropDownList ID="cboReportType" Runat="server" DataValueField="value" DataTextField="text" />
            </div>
            <div class="dnnFormItem">
                <dnn:label id="plStartDate" runat="server" controlname="txtStartDate" suffix=":" />
                <asp:TextBox id="txtStartDate" runat="server" Columns="20" />
                <asp:HyperLink id="cmdStartCalendar" resourcekey="Calendar" Runat="server" CssClass="dnnSecondaryAction" />
                <asp:comparevalidator id="valStartDate" cssclass="NormalRed" runat="server" resourcekey="valStartDate" display="Dynamic" type="Date" operator="DataTypeCheck" controltovalidate="txtStartDate" />
            </div>
            <div class="dnnFormItem">
                <dnn:label id="plEndDate" runat="server" controlname="txtEndDate" suffix=":" />
                <asp:TextBox id="txtEndDate" runat="server" Columns="20" />
                <asp:HyperLink id="cmdEndCalendar" resourcekey="Calendar" Runat="server" CssClass="dnnSecondaryAction" />
                <asp:comparevalidator id="valEndDate" cssclass="NormalRed" runat="server" resourcekey="valEndDate" display="Dynamic" type="Date" operator="DataTypeCheck" controltovalidate="txtEndDate" />
            </div>
            <div class="dnnFormItem">
                <asp:comparevalidator id="valDates" cssclass="NormalRed" runat="server" resourcekey="valDates" display="Dynamic" type="Date" operator="GreaterThan" controltovalidate="txtEndDate" controltocompare="txtStartDate" />
            </div>
            <ul class="dnnActions dnnClear">
                <li><asp:LinkButton id="cmdDisplay" resourcekey="cmdDisplay" cssclass="dnnPrimaryAction" Text="Display" runat="server"  /></li>
            </ul>
        </fieldset>        
        <asp:datagrid id="grdLog" Runat="server" Border="0" AutoGenerateColumns="true" BorderStyle="None" GridLines="None">
        	    <headerstyle cssclass="dnnGridHeader" verticalalign="Top"/>
	            <footerstyle cssclass="dnnGridFooter" />
	            <pagerstyle cssclass="dnnGridPager" />
        </asp:datagrid>
    </div>
</div>


