<%@ Page Language="VB" AutoEventWireup="false" CodeFile="InstallWizard.aspx.vb" Inherits="DotNetNuke.Services.Install.InstallWizard" %>

<%@ Import Namespace="DotNetNuke.UI.Utilities" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="WizardUser" Src="~/Install/WizardUser.ascx" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link rel="stylesheet" type="text/css" href="Install.css" />

    <script language="javascript" type="text/javascript">
        var BARS = 20;  // The number of bars to display
        var CLASS = "Progress"; // Style class name when bar is off
        var CLASS_ON_SUFFIX = "On";  // Suffix to append to style class name when bar is on
        var SPEED = 200; // Delay in milliseconds for animation

        var ELEMENT_ID_PREFIX = "Progress_"; // ID prefix for progress bar elements
        var current = 0; // Last rendered progress bar element
        var pageNo = <%=ClientAPI.GetSafeJSString(wizInstall.ActiveStepIndex)%>;
        var initialAction = '<%=GetBaseDataBaseVersion()%>';
        var nextButtonID = 'wizInstall_StepNavigationTemplateContainerID_StepNextButton';
        var prevButtonID = 'wizInstall_StepNavigationTemplateContainerID_StepPreviousButton';
        var timeOut;

         //Localization Vars
        var m_sLocaleComplete = '<%=ClientAPI.GetSafeJSString(LocalizeString("Complete"))%>';
        var m_sLocaleSuccess = '<%=ClientAPI.GetSafeJSString(LocalizeString("Success"))%>';
        var m_sLocaleFailure = '<%=ClientAPI.GetSafeJSString(LocalizeString("Failure"))%>';
        var m_sLocaleInstalling = '<%=ClientAPI.GetSafeJSString(LocalizeString("Installing"))%>';
        var m_sLocaleInstallComplete = '<%=ClientAPI.GetSafeJSString(LocalizeString("InstallComplete"))%>';
        var m_sLocaleInstallDatabase = '<%=ClientAPI.GetSafeJSString(LocalizeString("InstallDatabase"))%>';
        var m_sLocaleInstallScript = '<%=ClientAPI.GetSafeJSString(LocalizeString("InstallScript"))%>';
        var m_sLocaleInstallFailed = '<%=ClientAPI.GetSafeJSString(LocalizeString("InstallFailed"))%>';
        
        function animate()
        {
           var progressObj = document.getElementById(ELEMENT_ID_PREFIX + current);
           progressObj.className = CLASS + (progressObj.className == CLASS ? CLASS_ON_SUFFIX : "");

           current++;
           if (current == BARS)
               current = 0;      

           timeOut = window.setTimeout("animate()", SPEED);
        }

        function createAnimation()
        {
           // Create the progress bar
           var progress = document.getElementById("Progress");
           for(var p=0;p<BARS;p++)
           {
               var pDiv = document.createElement("div");
               pDiv.innerHTML = "&nbsp;&nbsp;";
               pDiv.className = CLASS;
               pDiv.id = "Progress_" + p;
               progress.appendChild(pDiv);
           }

           // Start the animation
           animate();
        }
        
        function stopAnimation()
        {
            window.clearTimeout(timeOut);                 
        }
        
        function installScripts()
        {
            if (pageNo == 3)
            {
                createAnimation();
                doCallbackAction(initialAction);
            }
        }

        function doCallbackAction(sAction)
        {
            var sFeedback;
            if (sAction == initialAction)
            {
                sFeedback = m_sLocaleInstallDatabase.replace('{0}', sAction);
            }
            else 
            {
                if (sAction.indexOf(".") > 0)
                {
                    sFeedback = m_sLocaleInstallScript.replace('{0}', sAction);
                }
            }
            reportAction(sFeedback + '...');
            var sCB = dnn.getVar('ActionCallback');
            eval(sCB.replace('[ACTIONTOKEN]', "'" + sAction + "'"));
        }
        
        function checkDisabled(button)
        {
            //return false;
            if (button.className == "WizardButtonDisabled")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        function reportAction(s)
        {
            $get('txtFeedback').value += s;
        }

        function successFunc(result, ctx)
        {
	        if (result != 'Done')
            {
                if (result.indexOf("ERROR") < 0)
                {
                    reportAction(m_sLocaleSuccess + '\n');
                    doCallbackAction(result);
                }
                else
                    errorFunc(result, ctx);
            }
            else
            {
                reportAction(m_sLocaleSuccess + '\n');
	            reportAction(m_sLocaleComplete + '\n');
                var progress = document.getElementById("Progress");
                progress.innerHTML = m_sLocaleInstallComplete;
                stopAnimation();
                var nextButton = document.getElementById(nextButtonID);
                nextButton.disabled = false;
                nextButton.className = "WizardButton";
	        }
        }

        function errorFunc(result, ctx)
        {
	        reportAction(m_sLocaleFailure + ' ' + result + '\n');
            var progress = document.getElementById("Progress");
            progress.innerHTML = m_sLocaleInstallFailed;
            stopAnimation();
        }
    </script>

    <style type="text/css">
        #Progress div
        {
            height: 36px;
            margin: 2px;
            border: 1px solid #ececec;
            display: inline;
        }
        .Progress
        {
            background-color: white;
        }
        .ProgressOn
        {
            background-color: #cc0000;
        }
    </style>
</head>
<body onload="installScripts()">
    <form id="form1" runat="server">
    <asp:Wizard ID="wizInstall" runat="server" CssClass="Wizard" ActiveStepIndex="0"
        Font-Names="Verdana" CellPadding="5" CellSpacing="5" FinishCompleteButtonType="Link"
        FinishPreviousButtonType="Link" StartNextButtonType="Link" StepNextButtonType="Link"
        StepPreviousButtonType="Link" DisplaySideBar="false">
        <StepStyle VerticalAlign="Top" Width="650px" />
        <NavigationButtonStyle CssClass="WizardButton" />
        <StepNavigationTemplate>
            <table border="0" cellpadding="5" cellspacing="5">
                <tr>
                    <td align="right">
                        <asp:LinkButton ID="CustomButton" runat="server" Text="Custom" CssClass="WizardButton"
                            Visible="False" />
                    </td>
                    <td align="right">
                        <asp:LinkButton ID="StepPreviousButton" runat="server" CommandName="MovePrevious"
                            Text="Previous" CssClass="WizardButton" />
                    </td>
                    <td align="right">
                        <asp:LinkButton ID="StepNextButton" runat="server" CommandName="MoveNext" Text="Next"
                            CssClass="WizardButton" />
                    </td>
                </tr>
            </table>
        </StepNavigationTemplate>
        <HeaderTemplate>
            <img src="logo.gif" border="0" alt="DotNetNuke">
        </HeaderTemplate>
        <WizardSteps>
            <asp:WizardStep ID="Step0" runat="Server" Title="Welcome">
                <h2>
                    <asp:Label ID="lblStep0Title" runat="server" /></h2>
                <asp:Label ID="lblStep0Detail" runat="Server" />
                <hr />
                <table id="tblLanguage" runat="server" cellpadding="0" cellspacing="0" border="0">
                    <tr>
                        <td class="NormalBold" width="200" valign="top">
                            <asp:Label ID="lblChooseInstall" runat="server" />
                        </td>
                        <td class="Normal" width="450">
                            <asp:RadioButtonList ID="rblInstall" runat="Server" RepeatDirection="Vertical" />
                        </td>
                    </tr>
                    <tr>
                        <td class="NormalBold" width="200" valign="top">
                            <asp:Label ID="lblChooseLanguage" runat="server" />
                        </td>
                        <td class="Normal" width="450">
                            <asp:DropDownList ID="cboLanguages" AutoPostBack="true" runat="Server" DataTextField="Text"
                                DataValueField="Code" />
                        </td>
                    </tr>
                </table>
                <asp:Label ID="lblDataBaseWarning" runat="server" CssClass="NormalRed" ResourceKey="DatabaseWarning" />
                <asp:Label ID="lblHostWarning" runat="server" CssClass="NormalRed" ResourceKey="HostWarning" />
            </asp:WizardStep>
            <asp:WizardStep ID="Step1" runat="server" Title="FilePermissions">
                <h2>
                    <asp:Label ID="lblStep1Title" runat="server" /></h2>
                <asp:Label ID="lblStep1Detail" runat="Server" />
                <hr />
                <h3>
                    <asp:Label ID="lblPermissions" runat="server" /></h3>
                <asp:CheckBoxList ID="lstPermissions" runat="server" DataTextField="Name" DataValueField="Permission"
                    TextAlign="Left" />
                <hr />
                <asp:Label ID="lblPermissionsError" runat="server" />
            </asp:WizardStep>
            <asp:WizardStep ID="Step2" runat="server" Title="ConnectionString" AllowReturn="false">
                <h2>
                    <asp:Label ID="lblStep2Title" runat="server" /></h2>
                <asp:Label ID="lblStep2Detail" runat="Server" />
                <hr />
                <table cellpadding="0" cellspacing="0" border="0">
                    <tr>
                        <td class="NormalBold" style="width: 150px;">
                            <asp:Label ID="lblChooseDatabase" runat="server" />
                        </td>
                        <td class="Normal" width="550">
                            <asp:RadioButtonList ID="rblDatabases" runat="Server" AutoPostBack="true" RepeatDirection="Horizontal"
                                RepeatColumns="3" />
                        </td>
                    </tr>
                </table>
                <br />
                <table id="tblDatabase" runat="Server" visible="False" cellpadding="0" cellspacing="0"
                    border="0">
                    <tr>
                        <td class="NormalBold" style="width: 150px;" height="30" valign="top">
                            <asp:Label ID="lblServer" runat="Server" />
                        </td>
                        <td class="Normal" style="width: 150px;" valign="top">
                            <asp:TextBox ID="txtServer" runat="Server" Width="150px" />
                        </td>
                        <td class="Help" valign="middle">
                            <asp:Label ID="lblServerHelp" runat="Server" />
                        </td>
                    </tr>
                    <tr id="trFile" runat="server">
                        <td class="NormalBold" style="width: 150px;" height="30" valign="top">
                            <asp:Label ID="lblFile" runat="Server" />
                        </td>
                        <td class="Normal" style="width: 150px;" valign="top">
                            <asp:TextBox ID="txtFile" runat="Server" Width="150px" />
                        </td>
                        <td class="Help" valign="middle">
                            <asp:Label ID="lblDatabaseFileHelp" runat="Server" />
                        </td>
                    </tr>
                    <tr id="trDatabase" runat="server">
                        <td class="NormalBold" style="width: 150px;" height="30" valign="top">
                            <asp:Label ID="lblDataBase" runat="Server" />
                        </td>
                        <td class="Normal" style="width: 150px;" valign="top">
                            <asp:TextBox ID="txtDatabase" runat="Server" Width="150px" />
                        </td>
                        <td class="Help" valign="middle">
                            <asp:Label ID="lblDatabaseHelp" runat="Server" />
                        </td>
                    </tr>
                    <tr id="trIntegrated" runat="server">
                        <td class="NormalBold" style="width: 150px;" height="30" valign="top">
                            <asp:Label ID="lblIntegrated" runat="Server" />
                        </td>
                        <td valign="top">
                            <asp:CheckBox ID="chkIntegrated" runat="Server" AutoPostBack="True" />
                        </td>
                        <td class="Help" valign="middle">
                            <asp:Label ID="lblIntegratedHelp" runat="Server" />
                        </td>
                    </tr>
                    <tr id="trUser" runat="Server">
                        <td class="NormalBold" style="width: 150px;" height="30" valign="top">
                            <asp:Label ID="lblUserId" runat="Server" />
                        </td>
                        <td class="Normal" style="width: 150px;" valign="top">
                            <asp:TextBox ID="txtUserId" runat="Server" Width="150px" />
                        </td>
                        <td class="Help" valign="middle">
                            <asp:Label ID="lblUserHelp" runat="Server" />
                        </td>
                    </tr>
                    <tr id="trPassword" runat="Server">
                        <td class="NormalBold" style="width: 150px;" height="30" valign="top">
                            <asp:Label ID="lblPassword" runat="Server" />
                        </td>
                        <td class="Normal" style="width: 150px;" valign="top">
                            <asp:TextBox ID="txtPassword" runat="Server" TextMode="Password" Width="150px" />
                        </td>
                        <td class="Help" valign="middle">
                            <asp:Label ID="lblPasswordHelp" runat="Server" />
                        </td>
                    </tr>
                    <tr>
                        <td class="NormalBold" style="width: 150px;" height="30" valign="top">
                            <asp:Label ID="lblOwner" runat="Server" />
                        </td>
                        <td class="Normal" style="width: 150px;" valign="top">
                            <asp:CheckBox ID="chkOwner" runat="Server" />
                        </td>
                        <td class="Help" valign="middle">
                            <asp:Label ID="lblOwnerHelp" runat="Server" />
                        </td>
                    </tr>
                    <tr>
                        <td class="NormalBold" style="width: 150px;" height="30" valign="top">
                            <asp:Label ID="lblQualifier" runat="Server" />
                        </td>
                        <td class="Normal" style="width: 150px;" valign="top">
                            <asp:TextBox ID="txtqualifier" runat="Server" Width="150px" />
                        </td>
                        <td class="Help" valign="middle">
                            <asp:Label ID="lblQualifierHelp" runat="Server" />
                        </td>
                    </tr>
                </table>
                <hr />
                <asp:Label ID="lblDataBaseError" runat="server" />
            </asp:WizardStep>
            <asp:WizardStep ID="Step3" runat="server" Title="Database" AllowReturn="false">
                <h2>
                    <asp:Label ID="lblStep3Title" runat="server" /></h2>
                <asp:Label ID="lblStep3Detail" runat="Server" />
                <hr />
                <br />
                <div id="Progress">
                    <%=LocalizeString("Installing")%></div>
                <br />
                <textarea id="txtFeedback" class="FeedBack" cols="80" rows="15"></textarea>
                <hr />
                <asp:Label ID="lblInstallError" runat="server" />
            </asp:WizardStep>
            <asp:WizardStep ID="Step4" runat="server" Title="HostSettings" AllowReturn="false">
                <h2>
                    <asp:Label ID="lblStep4Title" runat="server" /></h2>
                <asp:Label ID="lblStep4Detail" runat="Server" />
                <hr />
                <dnn:WizardUser ID="usrHost" runat="server" />
                <hr />
                <h3>
                    <asp:Label ID="lblSMTPSettings" runat="server" /></h3>
                <asp:Label ID="lblSMTPSettingsHelp" runat="Server" />
                <table>
                    <tr>
                        <td class="NormalBold" style="width: 250px">
                            <asp:Label ID="lblSMTPServer" runat="server" />
                        </td>
                        <td class="Normal" align="left">
                            <asp:TextBox ID="txtSMTPServer" runat="server" MaxLength="256" Width="225" />
                        </td>
                    </tr>
                    <tr>
                        <td class="NormalBold" style="width: 250px">
                            <asp:Label ID="lblSMTPAuthentication" runat="server" />
                        </td>
                        <td class="Normal">
                            <asp:RadioButtonList ID="optSMTPAuthentication" runat="server" RepeatDirection="Horizontal"
                                AutoPostBack="true">
                                <asp:ListItem Value="0" resourcekey="SMTPAnonymous" Selected="True" />
                                <asp:ListItem Value="1" resourcekey="SMTPBasic" />
                                <asp:ListItem Value="2" resourcekey="SMTPNTLM" />
                            </asp:RadioButtonList>
                        </td>
                    </tr>
                    <tr>
                        <td class="NormalBold" style="width: 250px">
                            <asp:Label ID="lblSMTPEnableSSL" runat="server" />
                        </td>
                        <td class="Normal">
                            <asp:CheckBox ID="chkSMTPEnableSSL" runat="server" />
                        </td>
                    </tr>
                    <tr id="trSMTPUserName" runat="server">
                        <td class="NormalBold" style="width: 250px">
                            <asp:Label ID="lblSMTPUsername" runat="server" />
                        </td>
                        <td class="Normal">
                            <asp:TextBox ID="txtSMTPUsername" runat="server" MaxLength="256" Width="300" />
                        </td>
                    </tr>
                    <tr id="trSMTPPassword" runat="server">
                        <td class="NormalBold" style="width: 250px">
                            <asp:Label ID="lblSMTPPassword" runat="server" />
                        </td>
                        <td class="Normal">
                            <asp:TextBox ID="txtSMTPPassword" runat="server" MaxLength="256" Width="300" TextMode="Password" />
                        </td>
                    </tr>
                </table>
                <asp:Label ID="lblHostUserError" runat="server" />
            </asp:WizardStep>
            <asp:WizardStep ID="Step5" runat="server" Title="Modules" AllowReturn="false">
                <h2>
                    <asp:Label ID="lblStep5Title" runat="server" /></h2>
                <asp:Label ID="lblStep5Detail" runat="Server" />
                <hr />
                <h3>
                    <asp:Label ID="lblModules" runat="server" /></h3>
                <asp:CheckBoxList ID="lstModules" runat="server" RepeatColumns="3" RepeatDirection="Horizontal" />
                <asp:Label ID="lblNoModules" runat="server" />
                <hr />
                <asp:Label ID="lblModulesError" runat="server" />
            </asp:WizardStep>
            <asp:WizardStep ID="Step6" runat="server" Title="Skins" AllowReturn="false">
                <h2>
                    <asp:Label ID="lblStep6Title" runat="server" /></h2>
                <asp:Label ID="lblStep6Detail" runat="Server" />
                <hr />
                <h3>
                    <asp:Label ID="lblSkins" runat="server" /></h3>
                <asp:CheckBoxList ID="lstSkins" runat="server" RepeatColumns="3" RepeatDirection="Horizontal" />
                <asp:Label ID="lblNoSkins" runat="server" />
                <br />
                <h3>
                    <asp:Label ID="lblContainers" runat="server" /></h3>
                <asp:CheckBoxList ID="lstContainers" runat="server" RepeatColumns="3" RepeatDirection="Horizontal" />
                <asp:Label ID="lblNoContainers" runat="server" />
                <hr />
                <asp:Label ID="lblSkinsError" runat="server" />
            </asp:WizardStep>
            <asp:WizardStep ID="Step7" runat="server" Title="Languages" AllowReturn="false">
                <h2>
                    <asp:Label ID="lblStep7Title" runat="server" /></h2>
                <asp:Label ID="lblStep7Detail" runat="Server" />
                <hr />
                <h3>
                    <asp:Label ID="lblLanguages" runat="server" /></h3>
                <asp:CheckBoxList ID="lstLanguages" runat="server" RepeatColumns="3" RepeatDirection="Horizontal" />
                <asp:Label ID="lblNoLanguages" runat="server" />
                <hr />
                <asp:Label ID="lblLanguagesError" runat="server" />
            </asp:WizardStep>
            <asp:WizardStep ID="Step8" runat="server" Title="AuthSystems" AllowReturn="false">
                <h2>
                    <asp:Label ID="lblStep8Title" runat="server" /></h2>
                <asp:Label ID="lblStep8Detail" runat="Server" />
                <hr />
                <h3>
                    <asp:Label ID="lblAuthSystems" runat="server" /></h3>
                <asp:CheckBoxList ID="lstAuthSystems" runat="server" RepeatColumns="3" RepeatDirection="Horizontal" />
                <asp:Label ID="lblNoAuthSystems" runat="server" />
                <hr />
                <asp:Label ID="lblAuthSystemsError" runat="server" />
            </asp:WizardStep>
            <asp:WizardStep ID="Step9" runat="server" Title="Providers" AllowReturn="false">
                <h2>
                    <asp:Label ID="lblStep9Title" runat="server" /></h2>
                <asp:Label ID="lblStep9Detail" runat="Server" />
                <hr />
                <h3>
                    <asp:Label ID="lblProviders" runat="server" /></h3>
                <asp:CheckBoxList ID="lstProviders" runat="server" RepeatColumns="3" RepeatDirection="Horizontal" />
                <asp:Label ID="lblNoProviders" runat="server" />
                <hr />
                <asp:Label ID="lblProvidersError" runat="server" />
            </asp:WizardStep>
            <asp:WizardStep ID="Step10" runat="server" Title="Portal" AllowReturn="false">
                <h2>
                    <asp:Label ID="lblStep10Title" runat="server" /></h2>
                <asp:Label ID="lblStep10Detail" runat="Server" />
                <hr />
                <h3>
                    <asp:Label ID="lblAdminUser" runat="server" /></h3>
                <dnn:WizardUser ID="usrAdmin" runat="server" />
                <hr />
                <h3>
                    <asp:Label ID="lblPortal" runat="server" /></h3>
                <table cellpadding="0" cellspacing="0" border="0">
                    <tr>
                        <td class="NormalBold" style="width: 150px;">
                            <asp:Label ID="lblPortalTitle" runat="server" />
                        </td>
                        <td class="Normal">
                            <asp:TextBox ID="txtPortalTitle" runat="server" Style="width: 250px;" MaxLength="128"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="NormalBold" style="width: 150px;">
                            <asp:Label ID="lblPortalTemplate" runat="server" />
                        </td>
                        <td class="Normal">
                            <asp:DropDownList ID="cboPortalTemplate" runat="server" Style="width: 250px;" />
                        </td>
                    </tr>
                </table>
                <hr />
                <asp:Label ID="lblPortalError" runat="server" />
            </asp:WizardStep>
            <asp:WizardStep ID="Complete" runat="server" StepType="Finish" Title="Installation Complete">
                <h2>
                    <asp:Label ID="lblCompleteTitle" runat="server" /></h2>
                <asp:Label ID="lblCompleteDetail" runat="server" />
            </asp:WizardStep>
        </WizardSteps>
    </asp:Wizard>
    <input id="ScrollTop" runat="server" name="ScrollTop" type="hidden" />
    <input type="hidden" id="__dnnVariable" runat="server" />
    <asp:Label ID="txtErrorMessage" runat="server" />
    </form>

    <script language="javascript" type="text/javascript">
        installScripts();
    </script>

</body>
</html>
