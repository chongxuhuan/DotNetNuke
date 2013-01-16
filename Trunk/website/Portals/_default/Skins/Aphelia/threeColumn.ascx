<%@ Control language="vb" AutoEventWireup="false" Explicit="True" Inherits="DotNetNuke.UI.Skins.Skin" %>
<%@ Register TagPrefix="dnn" TagName="BREADCRUMB" Src="~/Admin/Skins/BreadCrumb.ascx" %>
<%@ Register TagPrefix="dnn" TagName="LANGUAGE" Src="~/Admin/Skins/Language.ascx" %>
<%@ Register TagPrefix="dnn" TagName="COPYRIGHT" Src="~/Admin/Skins/Copyright.ascx" %>
<%@ Register TagPrefix="dnn" TagName="STYLES" Src="~/Admin/Skins/Styles.ascx" %>
<%@ Register TagPrefix="dnn" TagName="MENU" src="~/DesktopModules/DDRMenu/Menu.ascx" %>
<%@ Register TagPrefix="dnn" TagName="USERANDLOGIN" Src="~/Admin/Skins/UserAndLogin.ascx" %>
<%@ Register TagPrefix="dnn" TagName="LOGO" Src="~/Admin/Skins/Logo.ascx" %>
<%@ Register TagPrefix="dnn" TagName="Search" Src="~/Admin/Skins/search.ascx" %>

<dnn:STYLES runat="server" ID="StylesIE" Name="IE" StyleSheet="ieStylesskin.css" UseSkinPath="true"/>


	<div id="mainBanner" class="container">
	    <dnn:LANGUAGE ID="languageSelector" runat="server" ShowLinks="True" ShowMenu="False" />	
        <dnn:Search runat="server" id="dnnSearch" />
		<div class="menuBar">
		    <h1><dnn:LOGO ID="logo" runat="server"/></h1>
		    <dnn:MENU id="nameMenu" MenuStyle="Simple" runat="Server"/>
 		    <dnn:USERANDLOGIN id="userLogin" runat="Server"/>

		</div><!--close menuBar-->
		
		<div id="Breadcrumb"><dnn:BREADCRUMB ID="dnnBreadcrumb" runat="server" RootLevel="0" Separator=" > " /></div> 
	</div><!--close mainContainer-->
	<div id="mainContent" class="container">
		 <div class="columns">                
                <div class="paneGroup">
                    <div id="RightPane" runat="server" class="rightPane"></div>
                    <div id="LeftPane" runat="server" class="leftPane"></div>
                    <div id="ContentPane" runat="server" class="contentPane"></div>                   
                    <div class="dnnClear"></div><!--close dnnClear-->
                </div>
            </div><!--close columns-->
	</div><!--close mainContainer-->
	<div id="mainFooter" class="container">
    	<div id="copyright"><dnn:COPYRIGHT ID="dnnCopyright" runat="server" /></div>
    </div><!--close mainContainer-->
    
   <script type="text/javascript">
       $(window).load(function () {

           /* Toggle User Properties Menu
           -------------------------------------------*/
           $('.userName a').click(function (e) {
               $(this).toggleClass('active');
               $('.userMenu').fadeToggle('fast');
               e.stopPropagation();
           });
           $(document.body).click(function () {
               $('.userMenu').hide();
               $('.userName a').removeClass("active");
           });
           $('#dnn_pnav li').mouseenter(function () {
               $('.userMenu').hide();
               $('.userName a').removeClass("active");
           });
           $('.userMenu').click(function (e) {
               e.stopPropagation();
           });


           /* Set CSS3 Animations via class: http://daneden.me/animate/ 
           ---------------------------------------------*/
           $("#dnn_pnav li").hover(
				function () { $("ul", this).removeClass().addClass('fadeInDown'); },
				function () { $("ul", this).removeClass().addClass('fadeOutUp'); }
			);
           $("#dnn_pnav li ul li").hover(
				function () { $("ul", this).removeClass().addClass('fadeInLeft'); },
				function () { $("ul", this).removeClass().addClass('fadeOutRight'); }
			);


       }); 
    </script>
    
	



