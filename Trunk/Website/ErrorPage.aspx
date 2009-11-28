<%@ Page Language="vb" AutoEventWireup="false" Explicit="True" Inherits="DotNetNuke.Services.Exceptions.ErrorPage"
    CodeFile="ErrorPage.aspx.vb" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" lang="en-US">
<head>
    <meta name="revisit-after" content="1 days" />
    <meta name="robots" content="noindex,nofollow" />
    <title runat="server" id="Title">Error</title>
    <link id="StyleSheet" runat="server" href="~/Install/install.css" type="text/css" rel="stylesheet"/>
</head>
<body>
    <form id="Form" runat="server">
        <table cellspacing="5" cellpadding="5" border="0" class="Error">
	        <tr>
		        <td><asp:Image ID="Image1" runat="server" ImageUrl="~/logo.gif" BorderStyle="None" AlternateText="DotNetNuke" /></td>
	        </tr>
	        <tr style="height:100%;">
		        <td valign="top" style="width:650px;">
                    <h2>DotNetNuke Error</h2>
                    <hr />
                    <p><asp:PlaceHolder ID="ErrorPlaceHolder" runat="server" /></p>
                </td>
	        </tr>
	        <tr>
	            <td align="right"><asp:Hyperlink ID="hypReturn" runat="Server" NavigateUrl="~/Default.aspx" cssClass="WizardButton" text="Return to Site"/></td>
	        </tr>
	        <tr><td height="10px"></td></tr>
        </table>
    </form>
</body>
</html>
