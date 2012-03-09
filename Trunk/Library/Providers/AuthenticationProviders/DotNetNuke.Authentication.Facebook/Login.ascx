<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Login.ascx.cs" Inherits="DotNetNuke.Authentication.Facebook.Login" %>

<style>
    a { cursor: pointer; color: #3B5998; text-decoration: none;}
    .fb_button, .fb_button_rtl {
        background: #29447E url(https://s-static.ak.facebook.com/rsrc.php/v1/yL/r/FGFbc80dUKj.png);
        background-repeat: no-repeat;
        cursor: pointer;
        display: inline-block;
        padding: 0 0 0 1px;
        text-decoration: none;
        outline: none;      
    }
    a.fb_button, a.fb_button_rtl, .fb_button, .fb_button_rtl {
        text-decoration: none;
    }   

    .fb_button_medium, .fb_button_medium_rtl {
        background-position: left -188px;
        font-size: 11px;
        line-height: 14px;
    }

    a.fb_button, a.fb_button_rtl, .fb_button, .fb_button_rtl {
        text-decoration: none;
    }

    .fb_button .fb_button_text, .fb_button_rtl .fb_button_text {
        background: #5F78AB url(https://s-static.ak.facebook.com/rsrc.php/v1/yL/r/FGFbc80dUKj.png);
        border-top: solid 1px #879AC0;
        border-bottom: solid 1px #1A356E;
        color: white;
        display: block;
        font-family: "lucida grande",tahoma,verdana,arial,sans-serif;
        font-weight: bold;
        padding: 2px 6px 3px 6px;
        margin: 1px 1px 0 21px;
        text-shadow: none;
    }
</style>

<div data-show-faces="false" class="fb-login-button">
    <asp:LinkButton runat="server" ID="loginButton" CssClass="fb_button fb_button_medium">
        <span class="fb_button_text">Log In With Facebook</span>
    </asp:LinkButton>
</div>