<%@ Control Language="C#" AutoEventWireup="true" Inherits="DotNetNuke.Providers.FiftyOneClientCapabilityProvider.DeviceExplorer, DotNetNuke.Providers.FiftyOneClientCapabilityProvider" Codebehind="DeviceExplorer.ascx.cs" %>
<%@ Register Assembly="FiftyOne.Foundation" Namespace="FiftyOne.Foundation.UI.Web" TagPrefix="fiftyOne" %>

<fiftyOne:LiteMessage runat="server" ID="Message" CssClass="message" LogoEnabled="false" FooterEnabled="false" />
<fiftyOne:DeviceExplorer runat="server" ID="Explorer" CssClass="deviceExplorer" LogoEnabled="false" />