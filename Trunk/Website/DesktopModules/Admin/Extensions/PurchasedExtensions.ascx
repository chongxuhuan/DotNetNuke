<%@ Control Language="C#" AutoEventWireup="false" Inherits="DotNetNuke.Modules.Admin.Extensions.PurchasedExtensions" CodeFile="PurchasedExtensions.ascx.cs" %>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke" Namespace="DotNetNuke.UI.WebControls"%>
<asp:Button ID="btnSnow" runat="server" Text="Snowcovered records" onclick="BtnSnowClick" />
<asp:Label ID="lblMessage" runat="server" Text=""></asp:Label>
<asp:GridView ID="grdSnow" runat="server"></asp:GridView>