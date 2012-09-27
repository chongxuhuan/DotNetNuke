<%@ Page Language="C#" AutoEventWireup="false" Inherits="DotNetNuke.Services.Install.UpgradeWizard" CodeFile="UpgradeWizard.aspx.cs" %>
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
            <asp:LinkButton  id="lang_en_US" class="flag" runat="server" value="en-US" OnClientClick="upgradeWizard.changePageLocale('en-US');"><img src="../images/flags/en-US.gif" alt="en-US" /></asp:LinkButton>
            <asp:LinkButton  id="lang_de_DE" class="flag" runat="server" value="de-DE" OnClientClick="upgradeWizard.changePageLocale('de-DE');"><img src="../images/flags/de-DE.gif" alt="de-DE" /></asp:LinkButton>
            <asp:LinkButton  id="lang_es_ES" class="flag" runat="server" value="es-ES" OnClientClick="upgradeWizard.changePageLocale('es-ES');"><img src="../images/flags/es-ES.gif" alt="es-ES" /></asp:LinkButton> 
            <asp:LinkButton  id="lang_fr_FR" class="flag" runat="server" value="fr-FR" OnClientClick="upgradeWizard.changePageLocale('fr-FR');"><img src="../images/flags/fr-FR.gif" alt="fr-FR" /></asp:LinkButton>             
            <asp:LinkButton  id="lang_it_IT" class="flag" runat="server" value="it-IT" OnClientClick="upgradeWizard.changePageLocale('it-IT');"><img src="../images/flags/it-IT.gif" alt="it-IT" /></asp:LinkButton> 
            <asp:LinkButton  id="lang_nl_NL" class="flag" runat="server" value="nl-NL" OnClientClick="upgradeWizard.changePageLocale('nl-NL');"><img src="../images/flags/nl-NL.gif" alt="nl-NL" /></asp:LinkButton>
        </div>

        <div class="install">
            <h2 class="dnnForm dnnInstall dnnClear">
                <asp:Label ID="lblDotNetNukeUpgrade" runat="server" ResourceKey="Title" />
                <h5><asp:Label ID="currentVersionLabel" runat="server" /></h5>
                <h5><asp:Label ID="versionLabel" runat="server" /></h5>                
            </h2>
            <br/>
            <div class="dnnForm dnnInstall dnnClear" id="dnnInstall" runat="server">
                <hr />
                <asp:Label ID="lblIntroDetail" runat="Server" ResourceKey="BestPractices" />
            </div>

            <div id="tabs" class="dnnWizardTab">
                <ul>
                    <li><a href="#upgradeAccountInfo">
                        <div class="dnnWizardStep">
                            <span class="dnnWizardStepNumber">1</span>
                            <span class="dnnWizardStepTitle"><%= LocalizeString("AccountInfo")%></span>
                            <span class="dnnWizardStepArrow"></span>
                        </div>                    
                        </a>
                    </li>
                    <li><a href="#upgradeInstallation">
                         <div class="dnnWizardStep">
                            <span class="dnnWizardStepNumber">2</span>
                            <span class="dnnWizardStepTitle"><%= LocalizeString("Upgrade")%></span>
                            <span class="dnnWizardStepArrow"></span>
                        </div>      
                        </a>                    
                     </li>
                    <li><a href="#upgradeViewWebsite">
                         <div class="dnnWizardStep">
                            <span class="dnnWizardStepNumber">3</span>
                            <span class="dnnWizardStepTitle"><%= LocalizeString("ViewWebsite")%></span>
                            <span class="dnnWizardStepArrow"></span>
                        </div>      
                    </a>
                    </li>
                </ul>            
                <div class="upgradeAccountInfo dnnClear" id="upgradeAccountInfo">
                    <asp:Label ID="lblAccountInfoError" runat="server" CssClass="NormalRed"/>
                    <div class="dnnFormItem">
                        <div class="dnnFormItem">
                            <asp:Label ID="lblUsername" runat="server" ControlName="txtUsername" ResourceKey="Username" CssClass="dnnFormRequired dnnLabel" />
                            <asp:TextBox ID="txtUsername" runat="server" />
                        </div>
                        <div class="dnnFormItem">
                            <asp:Label ID="lblPassword" runat="server" ControlName="txtPassword" ResourceKey="Password" CssClass="dnnFormRequired dnnLabel" />
                            <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" />
                        </div>
                        <hr />
                        <ul class="dnnActions dnnClear">
                            <li>
                                <asp:LinkButton ID="continueLink" runat="server" CssClass="dnnPrimaryAction" resourcekey="Next" />
                            </li>
                        </ul>
                    </div>
                </div>            
                <div class="upgradeInstallation dnnClear" id="upgradeInstallation">
                    <asp:Label ID="lblUpgradeIntroInfo" runat="server" CssClass="installIntro" ResourceKey="UpgradeIntroInfo"/>
                    <div id="upgrade" runat="Server" visible="True" class="dnnForm">
                        <div class="dnnFormItem">
                            <div id="installation-progress">
                                    <div id="percentage" style="height: auto; max-height: 200px; overflow: auto"></div>
                                    <div class="dnnProgressbar">
                                        <div id="progressbar"></div>
                                    </div>
                                    <div id="installation-buttons">
                                    <a id="retry" href="javascript:void(0)" class="dnnPrimaryAction"><%= LocalizeString("Retry") %></a>
                                    <a id="seeLogs" href="javascript:void(0)" class="dnnPrimaryAction"><%= LocalizeString("SeeLogs") %></a>
                                    <a id="visitSite" href="javascript:void(0)" class="dnnPrimaryAction"><%= LocalizeString("VisitWebsite") %></a>
                                </div>    
                                <div id="installation-log-container" class="dnnScroll">                                
                                    <div id="installation-log">                              
                                    </div>
                                </div>
                            </div>                                                      
                            <div id="installation-steps">   
                                <p class="step-notstarted" id="DatabaseUpgrade"><span class="states-icons"></span><%= LocalizeString("DatabaseUpgrade")%></p>
                                <p class="step-notstarted" id="ExtensionsUpgrade"><span class="states-icons"></span><%= LocalizeString("ExtensionsUpgrade")%></p>
                            </div>  
                        </div>
                    </div>
                </div>

                <div class="upgradeViewWebsite dnnClear" id="upgradeViewWebsite"></div>
        
            </div>
        </div>
        
        <input id="PageLocale" runat="server" name="PageLanguage" type="hidden" value="" />
        <asp:Label ID="txtErrorMessage" runat="server" />
    </form>
    
    <script type="text/javascript">
        var upgradeWizard = new UpgradeWizard();
        function UpgradeWizard() {
            this.accountInfo = {};

            this.changePageLocale = function(locale) {
                $("#PageLocale")[0].value = locale;
            };

            this.showInstallationTab = function () {
                $("#tabs").tabs('enable', 1);
                $("#tabs").tabs('select', 1);
                $("#tabs").tabs('disable', 0);
                $("#languageFlags").hide();
                $('#<%= dnnInstall.ClientID %>').css('display', 'none');
            };

            this.upgrade = function () {
                //Call PageMethod which triggers long running operation
                PageMethods.RunUpgrade(function() {
                }, function(err) {
                    $.stopProgressbarOnError();
                });
                $('#seeLogs, #visitSite, #retry').attr('disabled', 'true');
                //Making sure that progress indicate 0
                $("#progressbar").progressbar('value', 0);
                $("#percentage").text('0% ');
                $.updateProgressbar();
            };
        }

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
            
            //****************************************************************************************
            // EVENT HANDLER FUNCTIONS
            //****************************************************************************************
            //Next Step
            $('#<%= continueLink.ClientID %>').click(function () {
                upgradeWizard.accountInfo = {
                    username: $('#<%= txtUsername.ClientID %>')[0].value,
                    password: $('#<%= txtPassword.ClientID %>')[0].value
                };
                
                $('#seeLogs, #visitSite, #retry').attr('disabled', 'true');

                PageMethods.ValidateInput(upgradeWizard.accountInfo, function (result) {
                    if (result.Item1) {
                        $('#<%= lblAccountInfoError.ClientID %>')[0].textContent = "";
                        upgradeWizard.showInstallationTab();
                        upgradeWizard.upgrade();
                    } else {
                        $('#<%= lblAccountInfoError.ClientID %>')[0].textContent = result.Item2;
                        $('#<%= lblAccountInfoError.ClientID %>').css('display', 'block');
                        setTimeout(function () { $('#<%= lblAccountInfoError.ClientID %>').css('display', 'none') }, 3000);
                    }
                });

                return false;
            });            
        } (jQuery, window.Sys));
    </script>
    
    <!-- Progressbar -->
    <script type="text/javascript">
        $.updateProgressbar = function () {
            var cbInterval = 2000; // call back interval

            //PageMethod GetUpgradeProgess success call back
            var onSuccess = function (output) {
                var result = jQuery.parseJSON(output);
                if (result !== null) {
                    //Updating progress
                    $("#progressbar").progressbar('value', result.progress);
                    $("#percentage").text(result.progress + '% ' + result.details);
                    var upgradeError = result.details.toUpperCase().indexOf('<%= LocalizeString("Error")%>') > -1;
                    if (upgradeError) {
                        // go through all the result and mark error state
                        var i = 0;
                        for (i = 0; i < 2; i++) {
                            var done = result["check" + i] === 'Done';
                            if (done) {
                                if (i < 1) {
                                    result["check" + (i + 1)] = "Error";
                                }
                                break;
                            }
                        }
                        if (i == 1) {
                            result.check0 = "Error";
                        }
                    }
                    applyCssStyle(result.check0, $('#DatabaseUpgrade'));
                    applyCssStyle(result.check1, $('#ExtensionsUpgrade'));


                    //If operation is complete
                    if (result.progress >= 100 || result.details == '<%= LocalizeString("UpgradeDone")%>') {
                        //Enable button                        
                        $('#seeLogs, #visitSite').removeAttr('disabled');
                        $('#installation-steps > p').attr('class', 'step-done');
                    }
                    //If not
                    else {
                        if (upgradeError) { // if error in upgrade
                            $.stopProgressbarOnError();
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

            //PageMethod GetUpgradeProgress fail call back
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
            PageMethods.GetUpgradeProgress(onSuccess, onFail);
        };

        //Start progress bar
        $.startProgressbar = function () {
            //Disabling button
            $('#seeLogs, #visitSite, #retry').attr('disabled', 'true');
            //Making sure that progress indicate 0            
            $("#progressbar").progressbar('value', 0);
            $("#percentage").text('0%');
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

                    //Call PageMethod which triggers long running operation
                    PageMethods.RunUpgrade(function () {
                    }, function (err) {
                        $.stopProgressbarOnError();
                    });
                    $.startProgressbar();
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

            $('#visitSite').click(function (e) {
                e.preventDefault();
                if (!$(this).attr('disabled'))
                    window.location.href = "../Default.aspx";
            });
        });
    </script>    
    
</body>
</html>
