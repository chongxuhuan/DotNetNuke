<%@ Page Language="C#" AutoEventWireup="false" Inherits="DotNetNuke.Services.Install.InstallWizard" CodeFile="InstallWizard.aspx.cs" %>
<%@ Import Namespace="DotNetNuke.UI.Utilities" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="dnncrm" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.UI.WebControls" Assembly="DotNetNuke.Web" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <asp:PlaceHolder runat="server" ID="ClientDependencyHeadCss"></asp:PlaceHolder>
    <asp:PlaceHolder runat="server" ID="ClientDependencyHeadJs"></asp:PlaceHolder>
    <link rel="stylesheet" type="text/css" href="../Portals/_default/reset.css?refresh" />
    <link rel="stylesheet" type="text/css" href="../Portals/_default/typography.css?refresh" />
    <link rel="stylesheet" type="text/css" href="../Portals/_default/default.css?refresh" />    
    <link rel="stylesheet" type="text/css" href="Install.css?refresh" />    
    <link rel="stylesheet" type="text/css" href="../Portals/_default/skins/_default/WebControlSkin/default/combobox.default.css?refresh" />
    <script type="text/javascript" src="../Resources/Shared/scripts/jquery/jquery.min.js"></script>
    <script type="text/javascript" src="../Resources/Shared/Scripts/jquery/jquery-ui.min.js"></script>
    <script type="text/javascript" src="../Resources/Shared/Scripts/jquery/jquery.hoverIntent.min.js"></script>
    <asp:placeholder id="SCRIPTS" runat="server"></asp:placeholder>
</head>  
<body>
    <asp:placeholder runat="server" id="ClientResourceIncludes" />
    <form id="form1" runat="server">
        <asp:ScriptManager ID="scManager" runat="server" EnablePageMethods="true"></asp:ScriptManager>       
        <asp:placeholder id="BodySCRIPTS" runat="server">    
              <script type="text/javascript" src="../Resources/Shared/Scripts/dnn.jquery.js"></script>                          
        </asp:placeholder>        
                          
    <br/>
    <img src="../images/Branding/DNN_logo.png" alt="DotNetNuke" />
            
    <div id="languageFlags" style="float: right;">       
        <asp:LinkButton  id="lang_en_US" class="flag" runat="server" value="en-US" OnClientClick="installWizard.changePageLocale('en-US');"><img src="../images/flags/en-US.gif" alt="en-US" /></asp:LinkButton>
        <asp:LinkButton  id="lang_de_DE" class="flag" runat="server" value="de-DE" OnClientClick="installWizard.changePageLocale('de-DE');"><img src="../images/flags/de-DE.gif" alt="de-DE" /></asp:LinkButton>
        <asp:LinkButton  id="lang_es_ES" class="flag" runat="server" value="es-ES" OnClientClick="installWizard.changePageLocale('es-ES');"><img src="../images/flags/es-ES.gif" alt="es-ES" /></asp:LinkButton> 
        <asp:LinkButton  id="lang_fr_FR" class="flag" runat="server" value="fr-FR" OnClientClick="installWizard.changePageLocale('fr-FR');"><img src="../images/flags/fr-FR.gif" alt="fr-FR" /></asp:LinkButton>             
        <asp:LinkButton  id="lang_it_IT" class="flag" runat="server" value="it-IT" OnClientClick="installWizard.changePageLocale('it-IT');"><img src="../images/flags/it-IT.gif" alt="it-IT" /></asp:LinkButton> 
        <asp:LinkButton  id="lang_nl_NL" class="flag" runat="server" value="nl-NL" OnClientClick="installWizard.changePageLocale('nl-NL');"><img src="../images/flags/nl-NL.gif" alt="nl-NL" /></asp:LinkButton>
    </div>

    <div class="install">
        <h2 class="dnnForm dnnInstall dnnClear" >
            <asp:Label id="lblDotNetNukeInstalltion" runat="server" ResourceKey="InstallTitle" />
            <hr/>
        </h2>
        <div class="dnnForm dnnInstall dnnClear" id="dnnInstall">
            <asp:Label ID="lblIntroDetail" runat="Server" ResourceKey="IntroDetail" />
        </div>
        <br />
        <asp:Label ID="lblError" runat="server" CssClass="dnnFormMessage dnnFormError" />
        <div id="tabs" class="dnnWizardTab">
            <ul>
                <li><a href="#installAccountInfo">
                    <div class="dnnWizardStep">
                        <span class="dnnWizardStepNumber">1</span>
                        <span class="dnnWizardStepTitle"><%= LocalizeString("AccountInfo")%></span>
                        <span class="dnnWizardStepArrow"></span>
                    </div>                    
                    </a>
                </li>
                <li><a href="#installInstallation">
                     <div class="dnnWizardStep">
                        <span class="dnnWizardStepNumber">2</span>
                        <span class="dnnWizardStepTitle"><%= LocalizeString("Installation")%></span>
                        <span class="dnnWizardStepArrow"></span>
                    </div>      
                    </a>                    
                 </li>
                <li><a href="#installViewWebsite">
                     <div class="dnnWizardStep">
                        <span class="dnnWizardStepNumber">3</span>
                        <span class="dnnWizardStepTitle"><%= LocalizeString("ViewWebsite")%></span>
                        <span class="dnnWizardStepArrow"></span>
                    </div>      
                </a>
                </li>
            </ul>
            <div class="installAccountInfo dnnClear" id="installAccountInfo">
                <asp:Label ID="lblAccountInfoIntro" runat="server" ResourceKey="AccountInfoIntro" />
                <p style="display: block; margin: 10px 0 10px 0;">
                    <asp:Label ID="lblAccountInfoError" runat="server" CssClass="dnnFormMessage dnnFormError" />                 
                </p>
                <div id="adminInfo" runat="Server" visible="True" class="dnnForm">
                    <dnn:Label ID="lblAdminInfo" runat="server" CssClass="tabSubTitle" ResourceKey="AdminInfo" />
                    <asp:Label ID="lblAdminInfoError" runat="server" CssClass="NormalRed"/>
                    <div class="dnnFormItem">
                        <div class="dnnFormItem">
                            <dnn:Label ID="lblUsername" runat="server" ControlName="txtUsername" ResourceKey="UserName" CssClass="dnnFormRequired"/>
                            <asp:TextBox ID="txtUsername" runat="server"/>
                        </div>
                        <div class="dnnFormItem">
                            <dnn:Label ID="lblPassword" runat="server" ControlName="txtPassword" ResourceKey="Password" CssClass="dnnFormRequired"/>
                            <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" />
                        </div>
                        <div class="dnnFormItem">
                            <dnn:Label ID="lblConfirmPassword" runat="server" ControlName="txtConfirmPassword" ResourceKey="Confirm" CssClass="dnnFormRequired" />
                            <asp:TextBox ID="txtConfirmPassword" runat="server" TextMode="Password" />
                        </div>                      
                    </div>
                </div>
                <div id="websiteInfo" runat="Server" visible="True" class="dnnForm">                    
                    <dnn:Label ID="lblWebsiteInfo" runat="server" CssClass="tabSubTitle" ResourceKey="WebsiteInfo"/>
                    <asp:Label ID="lblWebsiteInfoError" runat="server" CssClass="dnnFormMessage dnnFormError" />
                    <div class="dnnFormItem">
                        <div class="dnnFormItem">
                            <dnn:Label ID="lblWebsiteName" runat="server" ControlName="txtWebsiteName" ResourceKey="WebsiteName" CssClass="dnnFormRequired" />
                            <asp:TextBox ID="txtWebsiteName" runat="server"/>
                        </div>
                        <div class="dnnFormItem">
                            <dnn:Label ID="lblTemplate" runat="server" ControlName="ddlTemplate" ResourceKey="WebsiteTemplate" CssClass="dnnFormRequired" />                      
                            <dnn:DnnComboBox ID="templateList" runat="server">
                                <Items>
                                    <dnn:DnnComboBoxItem ResourceKey="TemplateDefault" Value="Default Website.template"/>
                                    <dnn:DnnComboBoxItem ResourceKey="TemplateMobile" Value="Mobile Website.template"/>
                                    <dnn:DnnComboBoxItem ResourceKey="TemplateBlank" Value="Blank Website.template" />
                                </Items>
                            </dnn:DnnComboBox>
                        </div>
                        <div class="dnnFormItem">
                            <dnn:Label ID="lblLanguage" runat="server" ControlName="ddlLanguage" ResourceKey="Language" CssClass="dnnFormRequired"/>
                            <dnn:DnnComboBox ID="languageList" runat="server" DataTextField="Text" DataValueField="Code">
                            </dnn:DnnComboBox>
                            <br/>
                            <asp:Label ID="lblLegacyLangaugePack" runat="server" CssClass="NormalBold" />      
                        </div>
                    </div>
                </div>
                <div id="databaseInfo" runat="Server" visible="True" class="dnnForm">
                    <dnn:Label id="lblDatabaseInfo" runat="server" CssClass="tabSubTitle" ResourceKey="DatabaseInfo" />
                    <asp:Label ID="lblDatabaseInfoMsg" runat="server" CssClass="loading NormalBold" />      
                    <div class="dnnFormItem">
                        <div class="dnnFormItem">
                            <dnn:Label ID="lblDatabaseSetup" runat="server" ControlName="rblDatabaseSetup" ResourceKey="DatabaseSetup"/>
                            <asp:RadioButtonList ID="databaseSetupType" CssClass="dnnFormRadioButtons" runat="server" RepeatDirection="Horizontal" >
                                <asp:ListItem Value="standard" ResourceKey="DatabaseStandard" Selected="True" />
                                <asp:ListItem Value="advanced" ResourceKey="DatabaseAdvanced" />
                            </asp:RadioButtonList>                            
                        </div>
                        <div id="StandardDatabaseMsg" class="dnnFormItem">
                            <dnn:Label ID="lblStandardDatabase" runat="server"/>
                            <asp:Label ID="lblStandardDatabaseMsg" runat="server" CssClass="dnnFormMessage" ResourceKey="StandardDatabaseMsg" />
                        </div>
                        <div id="advancedDatabase" class="dnnFormItem" style="display:none">
                            <div class="dnnFormItem">
                                <dnn:Label ID="lblDatabaseType" runat="server" ControlName="rblDatabaseType" ResourceKey="DatabaseType"/>
                                <asp:RadioButtonList ID="databaseType" runat="server" RepeatDirection="Horizontal">
                                    <asp:ListItem Value="express" ResourceKey="SqlTypeExpress" Selected="True" />
                                    <asp:ListItem Value="server" ResourceKey="SqlTypeServer"/>
                                </asp:RadioButtonList>
                            </div>
                            <div class="dnnFormItem">
                                <dnn:Label ID="lblDatabaseServerName" runat="server" ControlName="txtDatabaseServerName" ResourceKey="DatabaseServer" CssClass="dnnFormRequired"/>
                                <asp:TextBox ID="txtDatabaseServerName" runat="server"/>
                            </div>                        
                            <div class="dnnFormItem" id="databaseFilename">
                                <dnn:Label ID="lblDatabaseFilename" runat="server" ControlName="txtDatabaseFilename" ResourceKey="DatabaseFilename" CssClass="dnnFormRequired"/>
                                <asp:TextBox ID="txtDatabaseFilename" runat="server"/>
                            </div>
                            <div class="dnnFormItem" id="databaseName" style="display: none">
                                <dnn:Label ID="lblDatabaseName" runat="server" ControlName="txtDatabaseName" ResourceKey="DatabaseName" CssClass="dnnFormRequired"/>
                                <asp:TextBox ID="txtDatabaseName" runat="server"/>
                            </div>

                            <div class="dnnFormItem">
                                <dnn:Label ID="lblDatabaseObjectQualifier" runat="server" ControlName="txtDatabaseObjectQualifier" ResourceKey="DatabaseObjectQualifier"/>
                                <asp:TextBox ID="txtDatabaseObjectQualifier" runat="Server" MaxLength="20" />
                                <asp:RegularExpressionValidator ID="valQualifier" runat="server"
                                  resourcekey="InvalidQualifier.Text" 
                                  CssClass="dnnFormMessage dnnFormError"                                   
                                  ControlToValidate="txtDatabaseObjectQualifier"
                                  ValidationExpression="^[a-zA-Z][a-zA-Z0-9_]{0,19}$"
                                  Display="Dynamic"
                                ></asp:RegularExpressionValidator>
                            </div>
                            <div class="dnnFormItem">
                                <dnn:Label ID="lblDatabaseSecurity" runat="server" ControlName="rblDatabaseSecurity" ResourceKey="DatabaseSecurity"/>
                                <asp:RadioButtonList ID="databaseSecurityType" runat="server" RepeatDirection="Horizontal">
                                    <asp:ListItem Value="integrated" ResourceKey="DbSecurityIntegrated" Selected="True" />
                                    <asp:ListItem Value="userDefined" ResourceKey="DbSecurityUserDefined" />
                                </asp:RadioButtonList>
                            </div>
                            <div id="securityUserDefined" class="dnnFormItem" style="display:none; padding-top: 5px;">
                                <div class="dnnFormItem"> 
                                    <dnn:Label ID="lblDatabaseUsername" runat="server" ControlName="txtDatabaseUsername" ResourceKey="DatabaseUsername" CssClass="dnnFormRequired" />
                                    <asp:TextBox ID="txtDatabaseUsername" runat="server"/>
                                </div>
                                <div class="dnnFormItem">
                                    <dnn:Label ID="lblDatabasePassword" runat="server" ControlName="txtDatabasePassword" ResourceKey="DatabasePassword" CssClass="dnnFormRequired"/>
                                    <asp:TextBox ID="txtDatabasePassword" runat="server" TextMode="Password"/>
                                </div>
                            </div>
                            <div class="dnnFormItem">
                                <dnn:Label ID="lblDatabaseRunAs" runat="server" ControlName="txtDatabaseRunAs" ResourceKey="DatabaseRunAs"  />
                                <asp:CheckBox ID="databaseRunAs" runat="server" ResourceKey="DatabaseOwner" />
                            </div>
                        </div>                        
                        <div id="databaseError" class="dnnFormItem">
                            <dnn:Label ID="lblDatabaseConnectionError" runat="server" ResourceKey="DatabaseConnectionError"/>
                            <asp:Label ID="lblDatabaseError" runat="server" CssClass="NormalRed" />
                        </div>
                        
                    </div>
                </div> 
                <hr/>
                <ul class="dnnActions dnnClear">                    
                    <li><asp:LinkButton id="continueLink" runat="server" CssClass="dnnPrimaryAction" resourcekey="cmdContinue" /></li>
                </ul>                           
            </div>
            <div class="installInstallation dnnClear" id="installInstallation">
                <asp:Label ID="lblInstallationIntroInfo" runat="server" CssClass="installIntro" ResourceKey="InstallationIntroInfo" />
                <div id="installInstallation" runat="Server" visible="True" class="dnnForm">
                    <div class="dnnFormItem">
                        <div id="installation-progress">
                                <div id="percentage" style="height: auto; max-height: 200px; overflow: auto"></div>
                                <div id="timer" style="height: auto; max-height: 200px; overflow: auto"></div>
                                <div class="dnnProgressbar">
                                    <div id="progressbar"></div>
                                </div>
                                <a id="bannerLink" runat="server" href="" target="">
                                    <img id="banner" runat="server" class="banner" src="http://www.charlesnurse.com/Portals/6/Blog/Files/1/44/Windows-Live-Writer-My-Fall-Speaking-Tour_F7DC-clip_image001_22f3a1d6-fc65-42b8-b71b-420489ca5dca.jpg" alt="" onerror="installWizard.bannerError(this);"/>
                                </a>
                                <div id="installation-buttons">
                                <a id="retry" href="javascript:void(0)" class="dnnPrimaryAction"><%= LocalizeString("Retry") %></a>
                                <a id="seeLogs" href="javascript:void(0)" class="dnnPrimaryAction"><%= LocalizeString("SeeLogs") %></a>                               
                                <asp:LinkButton ID="visitSite" runat="server" resourcekey="VisitWebsite" CssClass="dnnPrimaryAction" />
                            </div>    
                            <div id="installation-log-container" class="dnnScroll">                                
                                <div id="installation-log">                              
                                </div>
                            </div>
                        </div>               
                        <div id="installation-steps">   
                            <p class="step-notstarted" id="FileAndFolderPermissionCheck"><span class="states-icons"></span><%= LocalizeString("FileAndFolderPermissionCheck")%></p>
                            <p class="step-notstarted" id="DatabaseInstallation"><span class="states-icons"></span><%= LocalizeString("DatabaseInstallation") %></p>                                    
                            <p class="step-notstarted" id="ExtensionsInstallation"><span class="states-icons"></span><%= LocalizeString("ExtensionsInstallation") %></p>
                            <p class="step-notstarted" id="WebsiteCreation"><span class="states-icons"></span><%= LocalizeString("WebsiteCreation") %></p>                                
                            <p class="step-notstarted" id="SuperUserCreation"><span class="states-icons"></span><%= LocalizeString("SuperUserCreation") %></p>
                            <p class="step-notstarted" id="LicenseActivation" runat="server"><span class="states-icons"></span><%= LocalizeString("LicenseActivation") %></p>                                                                                
                        </div>  
                    </div>
                </div>
            </div>

            <div class="installViewWebsite" id="installViewWebsite">
                <div id="installViewWebsite" runat="Server" visible="True" class="dnnForm">
                </div>
            </div>
       </div>
    </div>
 
    <br/><br/><br/>

        <input id="ScrollTop" runat="server" name="ScrollTop" type="hidden" />
        <input type="hidden" id="__dnnVariable" runat="server" />
        <input id="PageLocale" runat="server" name="PageLanguage" type="hidden" value="" />
        <asp:Label ID="txtErrorMessage" runat="server" />
    </form> 
      
    <!-- InstallWizard() -->
    <script type="text/javascript">
        var installWizard = new InstallWizard();
        function InstallWizard() {
            this.installInfo = { };
            //****************************************************************************************
            // PAGE FUNCTIONS
            //****************************************************************************************
            this.changePageLocale = function(locale) {
                $("#PageLocale")[0].value = locale;
            };
            this.confirmPasswordCheck = function() {
                if ($('#<%= txtConfirmPassword.ClientID %>')[0].value != '' && ($('#<%= txtPassword.ClientID %>')[0].value !== $('#<%= txtConfirmPassword.ClientID %>')[0].value)) {
                    $('#<%= lblAdminInfoError.ClientID %>')[0].textContent = '<%= LocalizeString("PasswordMismatch")%>';
                } else {
                    $('#<%= lblAdminInfoError.ClientID %>')[0].textContent = "";
                }
            };
            this.InitializeInstallScreen = function () {
                this.toggleDatabaseType(true);
                this.toggleDatabaseSecurity(true);
                this.checkingDatabase();
                $('#<%= lblDatabaseInfoMsg.ClientID %>').removeClass("loading");
                $('#<%= lblDatabaseInfoMsg.ClientID %>')[0].textContent = "";
                $("#databaseError").hide();
                $('#StandardDatabaseMsg').hide();
                $('#valQualifier').hide();
                PageMethods.VerifyDatabaseConnectionOnLoad(function (result) {
                    $('#<%= lblDatabaseInfoMsg.ClientID %>')[0].textContent = "";
                    if (result) {
                        $('#<%= lblDatabaseInfoMsg.ClientID %>').removeClass("loading");
                        $('#advancedDatabase').slideUp('fast');
                        $('#advancedDatabase').hide();
                        $('#StandardDatabaseMsg').hide();
                        installWizard.ValidDatabaseConnection = true;
                    } else {
                        $('#<%= lblDatabaseInfoMsg.ClientID %>').removeClass("loading");
                        $('#StandardDatabaseMsg').show();
                        $('#advancedDatabase').slideDown();
                        $('#advancedDatabase').show();
                        $('#databaseSetupType').find("input[value='advanced']").attr("checked", "checked");
                        $('#databaseSetupType').find("input[value='standard']").attr("disabled", "true");
                        $('#databaseSetupType input:radio:checked').trigger('click');
                    }
                });
            };
            this.toggleAdvancedDatabase = function(animation) {
                var databaseType = $('#<%= databaseSetupType.ClientID %> input:checked').val(); /*standard, advanced*/
                if (databaseType == "advanced") {
                    animation ? $('#advancedDatabase').slideDown() : $('#advancedDatabase').show();
                } else {
                    animation ? $('#advancedDatabase').slideUp('fast') : $('#advancedDatabase').hide();
                }
            };
            this.toggleDatabaseType = function() {
                var databaseType = $('#<%= databaseType.ClientID %> input:checked').val(); /*express, server*/
                if (databaseType == "express") {
                    $('#databaseFilename').show();
                    $('#databaseName').hide();
                } else {
                    $('#databaseName').show();
                    $('#databaseFilename').hide();
                }
            };
            this.toggleDatabaseSecurity = function(animation) {
                var databaseSecurityType = $('#<%= databaseSecurityType.ClientID %> input:checked').val(); /*integrated, userDefined*/
                if (databaseSecurityType == "userDefined") {
                    animation ? $('#securityUserDefined').slideDown() : $('#securityUserDefined').show();
                } else {
                    animation ? $('#securityUserDefined').slideUp('fast') : $('#securityUserDefined').hide();
                }
            };
            this.checkingDatabase = function () {
                $('#<%= lblDatabaseInfoMsg.ClientID %>').removeClass("NormalRed");
                $('#<%= lblDatabaseInfoMsg.ClientID %>').addClass("loading");
                var i = 0;
                $(".loading").html('<%= LocalizeString("TestingDatabase")%>');
                var origtext = $(".loading").html();
                var text = origtext;
                setInterval(function () {
                    $(".loading").html(text + Array((++i % 6) + 1).join("."));
                    if (i === 6) text = origtext;
                }, 500);
            };
            this.showInstallationTab = function() {
                $("#tabs").tabs('enable', 1);
                $("#tabs").tabs('select', 1);
                $("#tabs").tabs('disable', 0);
                $("#languageFlags").hide();
            };
            this.showAccountInfoTab = function() {
                $("#tabs").tabs('enable', 0);
                $("#tabs").tabs('select', 0);
                $("#tabs").tabs('disable', 1);
                $("#languageFlags").show();
            };
            this.timerIntervalId ={};
            this.startTimer = function () {
                $("#timer").html('0 ' + '<%=LocalizeString("TimerSeconds") %>');
                var time = 0;
                timerIntervalId = setInterval(function () {
                    time = time + 1;
                    $("#timer").html(time + ' <%=LocalizeString("TimerSeconds") %>');
                }, 1000);
            };
            this.stopTimer = function () {
                clearInterval(timerIntervalId);
            };
            this.install = function () {
                //Call PageMethod which triggers long running operation
                PageMethods.RunInstall(function () {
                }, function (err) {
                    $.stopProgressbarOnError();
                });
                $('#seeLogs, #visitSite, #retry').attr('disabled', 'true');
                //Making sure that progress indicate 0
                $("#progressbar").progressbar('value', 0);
                $("#percentage").text('0% ');
                $("#timer").html('0 ' + '<%=LocalizeString("TimerSeconds") %>');
                $.installationStartTime = new Date();
                $.updateProgressbar();
            };

            // Banner Rotator
            this.online = navigator.onLine;
            this.bannerIndex = 1;
            this.bannerError = function (image) {
                this.bannerIndex = 1;
                image.src = "http://www.dotnetnuke.com/Portals/_default/Skins/DNN-Skin/images/dnnLogo.png";
            };
        }
    </script>
    
    <!-- Page Level -->
    <script type="text/javascript">
            
        function LegacyLangaugePack(version) {
            $('#<%= lblLegacyLangaugePack.ClientID %>')[0].innerText = '<%= LocalizeString("LegacyLangaugePack")%>' + version;
        }
        function ClearLegacyLangaugePack() {
            $('#<%= lblLegacyLangaugePack.ClientID %>')[0].innerText = '';
        }
        
        // Banner Rotator
        jQuery(document).ready(function ($) {
            if (installWizard.online) {
                setInterval(function() {
                    $("#bannerLink").hide();
                    $("#bannerLink").show('slow');
                    $("#banner").attr("src", "http://www.dotnetnuke_enterprise.com/bannerimages/banner" + installWizard.bannerIndex + ".png");
                    installWizard.bannerIndex += 1;
                }, 5000);
            }
        });

        /*globals jQuery, window, Sys */
        (function ($, Sys) {
            $(function () {
                $("#tabs").bind("tabscreate", function (event, ui) {
                    var index = 0, selectedIndex = 0;
                    $('.ui-tabs-nav li', $(this)).each(function () {
                        if ($(this).hasClass('ui-tabs-selected'))
                            selectedIndex = index;
                        index++;
                    });
                    $('.dnnWizardStepArrow', $(this)).eq(selectedIndex).css('background-position', '0 -299px');
                    if (selectedIndex)
                        $('.dnnWizardStepArrow', $(this)).eq(selectedIndex - 1).css('background-position', '0 -201px');
                });

                $("#tabs").bind("tabsselect", function (event, ui) {
                    var index = ui.index;
                    $('.dnnWizardStepArrow', $(this)).css('background-position', '0 -401px');
                    $('.dnnWizardStepArrow', $(this)).eq(index).css('background-position', '0 -299px');
                    if (index) {
                        $('.dnnWizardStepArrow', $(this)).eq(index - 1).css('background-position', '0 -201px');
                    }
                });
                $("#tabs").tabs();
                $("#tabs").tabs({ disabled: [1, 2] });
                $('.dnnFormMessage.dnnFormError').each(function () {
                    if ($(this).html().length)
                        $(this).css('display', 'block');
                });
                $(".dnnProgressbar").dnnProgressbar();
            });

            $(document).ready(function () {
                if(window.location.href.indexOf("?executeinstall")>-1) {                    
                    installWizard.showInstallationTab();
                    installWizard.install();
                    $.startProgressbar();
                }
                else {
                    //Go to installation page when installation is already in progress
                    PageMethods.IsInstallerRunning(function(result) {
                        if (result == true) {
                            installWizard.showInstallationTab();
                            $.startProgressbar();
                        } else {
                            installWizard.InitializeInstallScreen();
                        }
                    });
                }
            });

            //****************************************************************************************
            // EVENT HANDLER FUNCTIONS
            //****************************************************************************************
            //Database Functions
            $('#<%= databaseSetupType.ClientID %>').change(function () {
                installWizard.toggleAdvancedDatabase(true);
            });
            $('#<%= databaseType.ClientID %>').change(function () {
                installWizard.toggleDatabaseType(true);
            });
            $('#<%= databaseSecurityType.ClientID %>').change(function () {
                installWizard.toggleDatabaseSecurity(true);
            });
            //Password Check
            $('#<%= txtPassword.ClientID %>').focusout(function () {
                installWizard.confirmPasswordCheck();
            });
            $('#<%= txtConfirmPassword.ClientID %>').focusout(function () {
                installWizard.confirmPasswordCheck();
            });
            //Next Step
            $('#<%= continueLink.ClientID %>').click(function () {
                $("#continueLink").attr('disabled', 'disabled');
                
                installWizard.installInfo = {
                    username: $('#<%= txtUsername.ClientID %>')[0].value,
                    password: $('#<%= txtPassword.ClientID %>')[0].value,
                    websiteName: $('#<%= txtWebsiteName.ClientID %>')[0].value,
                    template: $find('<%= templateList.ClientID %>').get_value(),
                    language: $find('<%= languageList.ClientID %>').get_value(),
                    databaseSetup: $('#<%= databaseSetupType.ClientID %> input:checked').val(),
                    threadCulture: $("#PageLocale")[0].value,
                    databaseServerName: "",
                    databaseFilename: "",
                    databaseType: "",
                    databaseName: "",
                    databaseObjectQualifier: "",
                    databaseSecurity: "",
                    databaseUsername: "",
                    databasePassword: "",
                    databaseRunAsOwner: null
                };
                $('#<%= lblAccountInfoError.ClientID %>').css('display', 'none');
                var databaseType = $('#<%= databaseSetupType.ClientID %> input:checked').val();
                if (databaseType == "advanced") {
                    installWizard.installInfo.databaseServerName = $('#<%= txtDatabaseServerName.ClientID %>')[0].value;
                    installWizard.installInfo.databaseFilename = $('#<%= txtDatabaseFilename.ClientID %>')[0].value;
                    installWizard.installInfo.databaseType = $('#<%= databaseType.ClientID %> input:checked').val();
                    installWizard.installInfo.databaseName = $('#<%= txtDatabaseName.ClientID %>')[0].value;
                    installWizard.installInfo.databaseObjectQualifier = $('#<%= txtDatabaseObjectQualifier.ClientID %>')[0].value;
                    installWizard.installInfo.databaseSecurity = $('#<%= databaseSecurityType.ClientID %> input:checked').val();
                    installWizard.installInfo.databaseUsername = $('#<%= txtDatabaseUsername.ClientID %>')[0].value;
                    installWizard.installInfo.databasePassword = $('#<%= txtDatabasePassword.ClientID %>')[0].value;
                    installWizard.installInfo.databaseRunAsOwner = $('#<%= databaseRunAs.ClientID %>')[0].value;
                }                

                PageMethods.ValidateInput(installWizard.installInfo, function (result) {
                    if (result.Item1) {
                        $('#<%= lblAccountInfoError.ClientID %>')[0].textContent = "";
                        $('#<%= lblDatabaseConnectionError.ClientID %>').html("");
                        $("#databaseError").hide();

                        installWizard.checkingDatabase();
                        PageMethods.VerifyDatabaseConnection(installWizard.installInfo, function(valid) {
                            $('#<%= lblDatabaseInfoMsg.ClientID %>')[0].textContent = "";
                            if (valid.Item1) {
                                $('#<%= lblDatabaseInfoMsg.ClientID %>').removeClass("loading");
                                //Restart app to refresh config from web.config
                                window.location.replace(window.location + "?initiateinstall");
                            } else {
                                $("#databaseError").show();
                                $('#<%= lblDatabaseInfoMsg.ClientID %>').removeClass("loading");
                                $('#<%= lblDatabaseInfoMsg.ClientID %>').addClass("NormalRed");
                                $('#<%= lblDatabaseInfoMsg.ClientID %>')[0].textContent = '<%= LocalizeString("DatabaseError")%>';
                                $('#<%= lblDatabaseError.ClientID %>').html(valid.Item2);
                            }
                            $("#continueLink").removeAttr('disabled');
                        });
                    } else {
                        $('#<%= lblAccountInfoError.ClientID %>')[0].textContent = result.Item2;
                        $('#<%= lblAccountInfoError.ClientID %>').css('display', 'block');
                        $("#continueLink").removeAttr('disabled');
                    }                                        
                });                
                return false;
            });
            
        } (jQuery, window.Sys));
    </script>
       
    <!-- Progressbar -->
    <script type="text/javascript">
        $.updateProgressbar = function () {
            var cbInterval = 1000; // call back interval

            //PageMethod GetInstallationProgess success call back
            var onSuccess = function (output) {
                var result = jQuery.parseJSON(output);
                if (result !== null) {
                    //Updating progress
                    $("#progressbar").progressbar('value', result.progress);
                    $("#percentage").text(result.progress + '% ' + result.details);
                    var installationError = result.details.toUpperCase().indexOf('ERROR') > -1;
                    if (installationError) {
                        // go through all the result and mark error state
                        for (var i = 0; i < 6; i++) {
                            var done = result["check" + i] === "Done";
                            if (!done) { break; }
                        }
                    }
                    applyCssStyle(result.check0, $('#FileAndFolderPermissionCheck'));
                    applyCssStyle(result.check1, $('#DatabaseInstallation'));
                    applyCssStyle(result.check2, $('#ExtensionsInstallation'));
                    applyCssStyle(result.check3, $('#WebsiteCreation'));
                    applyCssStyle(result.check4, $('#SuperUserCreation'));
                    applyCssStyle(result.check5, $('#LicenseActivation'));

                    //If operation is complete
                    if (result.progress >= 100 || result.details == '<%= LocalizeString("InstallationDone")%>') {
                        installWizard.stopTimer();
                        $('#<%= lblInstallationIntroInfo.ClientID %>')[0].textContent = '<%= LocalizeString("InstallationComplete")%>';
                        //Enable button                        
                        $('#seeLogs, #visitSite').removeAttr('disabled');
                        $('#installation-steps > p').attr('class', 'step-done');
                    }
                        //If not
                    else {
                        if (installationError) { // if error in installation
                            $.stopProgressbarOnError();
                            //Allow user to visit site even if only license step error occurs.
                            if (result["check4"] === "Done" && result.check5.indexOf("Error" > -1)) {
                                applyCssStyle("Error", $('#LicenseActivation'));
                                $('#visitSite').removeAttr('disabled');
                            }
                        }
                        else {
                            $.updateProgressbarTimeId = setTimeout($.updateProgressbar, cbInterval);
                        }
                    }
                }
                else {
                    //Sometimes server return empty string, just ignore it
                    $.updateProgressbarTimeId = setTimeout($.updateProgressbar, cbInterval);
                }
            };

            //PageMethod GetInstallationProgress fail call back
            var onFail = function (err) {
                $.updateProgressbarTimeId = setTimeout($.updateProgressbar, cbInterval);
            };

            var applyCssStyle = function (state, ele) {
                if (!state) state = '';
                switch (state.toLowerCase()) {
                case 'done':
                    ele.attr('class', 'step-done');
                    break;
                case 'running':
                    ele.attr('class', 'step-running');
                    break;
                case 'error':
                    ele.attr('class', 'step-error');
                    break;
                default:
                    ele.attr('class', 'step-notstarted');
                    break;
                }
            };

            //Calling PageMethod for current progress
            PageMethods.GetInstallationProgress(onSuccess, onFail);
        };

        //Start progress bar
        $.startProgressbar = function () {
            //Disabling button
            $('#seeLogs, #visitSite, #retry').attr('disabled', 'true');
            //Making sure that progress indicate 0            
            $("#progressbar").progressbar('value', 0);
            $("#percentage").text('0%');
            installWizard.startTimer();
            $.updateProgressbar();
        };

        //Stop update progress bar on errors
        $.stopProgressbarOnError = function () {
            $("#seeLogs, #retry").removeAttr('disabled');           
            if ($.updateProgressbarTimeId) {
                clearTimeout($.updateProgressbarTimeId);
                $.updateProgressbarTimeId = null;
            }
            $('#installation-steps > p.step-running').attr('class', 'step-error');
            installWizard.stopTimer();
        };

        $(document).ready(function () {
            //Progressbar and button initialization                   
            $("#progressbar").progressbar({ value: 0 });
            $('#visitSite, #seeLogs, #retry').attr('disabled', 'true');

            $("#retry").click(function (e) {
                e.preventDefault();
                if (!$(this).attr('disabled')) {
                    $('#retry').attr('disabled', 'true');
                    $('#installation-log-container').hide();
                    installWizard.startTimer();
                    //Call PageMethod which triggers long running operation
                    PageMethods.RunInstall(function () {
                    }, function (err) {
                        $.stopProgressbarOnError();
                    });
                    $.startProgressbar();
                    installWizard.stopTimer();
                }
            });

            var installationLogStartLine = 0;
            var getInstallationLog = function () {
                PageMethods.GetInstallationLog(installationLogStartLine, function (result) {
                    if (result) {
                        if (installationLogStartLine === 0)
                            $('#installation-log').html(result);
                        else
                            $('#installation-log').append(result);
                        
                        installationLogStartLine += 500;
                        setTimeout(getInstallationLog, 100);
                    } else {
                        if (installationLogStartLine === 0)
                            $('#installation-log').html('<%= LocalizeString("NoInstallationLog")%>');
                    }

                    $('#installation-log-container').jScrollPane();

                }, function (err) {
                });
            };

            $('#seeLogs').click(function (e) {
                e.preventDefault();
                if (!$(this).attr('disabled')) {
                    $(this).attr('disabled', 'true');
                    $('#installation-log-container').show();
                    getInstallationLog();
                }
            });
        });
    </script>    

</body>
</html>
