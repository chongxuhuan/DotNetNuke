'
' DotNetNuke® - http://www.dotnetnuke.com
' Copyright (c) 2002-2010
' by DotNetNuke Corporation
'
' Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
' documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
' the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
' to permit persons to whom the Software is furnished to do so, subject to the following conditions:
'
' The above copyright notice and this permission notice shall be included in all copies or substantial portions 
' of the Software.
'
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
' DEALINGS IN THE SOFTWARE.
'

Imports System
Imports System.Data
Imports System.Web
Imports System.Collections.Generic
Imports DotNetNuke.Services.Scheduling
Imports DotNetNuke.UI.Skins
Imports DotNetNuke.Services.Cache
Imports System.Threading
Imports DotNetNuke.Entities.Controllers

Namespace DotNetNuke.Entities.Host

    Public Class Host
        Inherits BaseEntityInfo

        Private Shared hostCont As New HostController()

#Region "Public Shared Properties"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the AutoAccountUnlockDuration
        ''' </summary>
        ''' <remarks>Defaults to 10</remarks>
        ''' <history>
        ''' 	[cnurse]	01/28/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared ReadOnly Property AutoAccountUnlockDuration() As Integer
            Get
                Return HostController.Instance.GetInteger("AutoAccountUnlockDuration", 10)
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the AuthenticatedCacheability
        ''' </summary>
        ''' <remarks>Defaults to HttpCacheability.ServerAndNoCache</remarks>
        ''' <history>
        ''' 	[cnurse]	03/05/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared ReadOnly Property AuthenticatedCacheability() As String
            Get
                Return HostController.Instance.GetString("AuthenticatedCacheability", "4")
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets whether the Upgrade Indicator is enabled
        ''' </summary>
        ''' <remarks>Defaults to True</remarks>
        ''' <history>
        ''' 	[cnurse]	01/29/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared ReadOnly Property CheckUpgrade() As Boolean
            Get
                Return HostController.Instance.GetBoolean("CheckUpgrade", True)
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the Control Panel
        ''' </summary>
        ''' <remarks>Defaults to glbDefaultControlPanel constant</remarks>
        ''' <history>
        ''' 	[cnurse]	01/29/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared ReadOnly Property ControlPanel() As String
            Get
                Dim setting As String = HostController.Instance.GetString("ControlPanel")
                If String.IsNullOrEmpty(setting) Then
                    setting = glbDefaultControlPanel
                End If
                Return setting
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the Default Admin Container
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	01/28/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared ReadOnly Property DefaultAdminContainer() As String
            Get
                Dim setting As String = HostController.Instance.GetString("DefaultAdminContainer")
                If String.IsNullOrEmpty(setting) Then
                    setting = SkinController.GetDefaultAdminContainer()
                End If
                Return setting
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the Default Admin Skin
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	01/28/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared ReadOnly Property DefaultAdminSkin() As String
            Get
                Dim setting As String = HostController.Instance.GetString("DefaultAdminSkin")
                If String.IsNullOrEmpty(setting) Then
                    setting = SkinController.GetDefaultAdminSkin()
                End If
                Return setting
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the Default Doc Type
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	07/14/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared ReadOnly Property DefaultDocType() As String
            Get
                Dim doctype As String = "<!DOCTYPE HTML PUBLIC ""-//W3C//DTD HTML 4.0 Transitional//EN"">"
                Dim setting As String = HostController.Instance.GetString("DefaultDocType")
                If Not String.IsNullOrEmpty(setting) Then
                    Select Case setting
                        Case "0" : doctype = "<!DOCTYPE HTML PUBLIC ""-//W3C//DTD HTML 4.0 Transitional//EN"">"
                        Case "1" : doctype = "<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Transitional//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"">"
                        Case "2" : doctype = "<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Strict//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd"">"
                    End Select
                End If
                Return doctype
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the Default Portal Container
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	01/28/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared ReadOnly Property DefaultPortalContainer() As String
            Get
                Dim setting As String = HostController.Instance.GetString("DefaultPortalContainer")
                If String.IsNullOrEmpty(setting) Then
                    setting = SkinController.GetDefaultPortalContainer()
                End If
                Return setting
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the Default Portal Skin
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	01/28/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared ReadOnly Property DefaultPortalSkin() As String
            Get
                Dim setting As String = HostController.Instance.GetString("DefaultPortalSkin")
                If String.IsNullOrEmpty(setting) Then
                    setting = SkinController.GetDefaultPortalSkin()
                End If
                Return setting
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the Demo Period for new portals
        ''' </summary>
        ''' <remarks>Defaults to -1</remarks>
        ''' <history>
        ''' 	[cnurse]	01/29/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared ReadOnly Property DemoPeriod() As Integer
            Get
                Return HostController.Instance.GetInteger("DemoPeriod")
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets whether demo signups are enabled
        ''' </summary>
        ''' <remarks>Defaults to False</remarks>
        ''' <history>
        ''' 	[cnurse]	04/14/2009   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared ReadOnly Property DemoSignup() As Boolean
            Get
                Return HostController.Instance.GetBoolean("DemoSignup", False)
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets whether to dislpay the beta notice
        ''' </summary>
        ''' <remarks>Defaults to True</remarks>
        ''' <history>
        ''' 	[cnurse]	05/19/2009   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared ReadOnly Property DisplayBetaNotice() As Boolean
            Get
                Return HostController.Instance.GetBoolean("DisplayBetaNotice", True)
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets whether to dislpay the copyright
        ''' </summary>
        ''' <remarks>Defaults to True</remarks>
        ''' <history>
        ''' 	[cnurse]	03/05/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared ReadOnly Property DisplayCopyright() As Boolean
            Get
                Return HostController.Instance.GetBoolean("Copyright", True)
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets whether AJAX is Enabled
        ''' </summary>
        ''' <remarks>Defaults to False</remarks>
        ''' <history>
        ''' 	[cnurse]	01/28/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <Obsolete("MS AJax is now required for DotNetNuke 5.0 and above")> _
        Public Shared ReadOnly Property EnableAJAX() As Boolean
            Get
                Return HostController.Instance.GetBoolean("EnableAJAX", True)
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets whether Browser Language Detection is Enabled
        ''' </summary>
        ''' <remarks>Defaults to True</remarks>
        ''' <history>
        ''' 	[cnurse]	02/19/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared ReadOnly Property EnableBrowserLanguage() As Boolean
            Get
                Return HostController.Instance.GetBoolean("EnableBrowserLanguage", True)
            End Get
        End Property

        Public Shared ReadOnly Property EnableContentLocalization() As Boolean
            Get
                Return HostController.Instance.GetBoolean("EnableContentLocalization", True)
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets whether a css class based on the Module Name is automatically rendered
        ''' </summary>
        ''' <remarks>Defaults to True</remarks>
        ''' <history>
        ''' 	[cnurse]	06/30/2010   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared ReadOnly Property EnableCustomModuleCssClass() As Boolean
            Get
                Return HostController.Instance.GetBoolean("EnableCustomModuleCssClass", True)
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets whether File AutoSync is Enabled
        ''' </summary>
        ''' <remarks>Defaults to False</remarks>
        ''' <history>
        ''' 	[cnurse]	01/28/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared ReadOnly Property EnableFileAutoSync() As Boolean
            Get
                Return HostController.Instance.GetBoolean("EnableFileAutoSync", False)
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets whether Module Online Help is Enabled
        ''' </summary>
        ''' <remarks>Defaults to False</remarks>
        ''' <history>
        ''' 	[cnurse]	01/28/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared ReadOnly Property EnableModuleOnLineHelp() As Boolean
            Get
                Return HostController.Instance.GetBoolean("EnableModuleOnLineHelp", False)
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets whether the Request Filters are Enabled
        ''' </summary>
        ''' <remarks>Defaults to False</remarks>
        ''' <history>
        ''' 	[cnurse]	01/28/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared ReadOnly Property EnableRequestFilters() As Boolean
            Get
                Return HostController.Instance.GetBoolean("EnableRequestFilters", False)
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets whether to use the Language in the Url
        ''' </summary>
        ''' <remarks>Defaults to True</remarks>
        ''' <history>
        ''' 	[cnurse]	02/19/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared ReadOnly Property EnableUrlLanguage() As Boolean
            Get
                Return HostController.Instance.GetBoolean("EnableUrlLanguage", True)
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets whether Users Online are Enabled
        ''' </summary>
        ''' <remarks>Defaults to False</remarks>
        ''' <history>
        ''' 	[cnurse]	01/28/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared ReadOnly Property EnableUsersOnline() As Boolean
            Get
                Return Not HostController.Instance.GetBoolean("DisableUsersOnline", True)
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets whether SSL is Enabled for SMTP
        ''' </summary>
        ''' <remarks>Defaults to False</remarks>
        ''' <history>
        ''' 	[cnurse]	01/28/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared ReadOnly Property EnableSMTPSSL() As Boolean
            Get
                Return HostController.Instance.GetBoolean("SMTPEnableSSL", False)
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets whether the Event Log Buffer is Enabled
        ''' </summary>
        ''' <remarks>Defaults to False</remarks>
        ''' <history>
        ''' 	[cnurse]	01/28/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared ReadOnly Property EventLogBuffer() As Boolean
            Get
                Return HostController.Instance.GetBoolean("EventLogBuffer", False)
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the Allowed File Extensions
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	01/28/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared ReadOnly Property FileExtensions() As String
            Get
                Return HostController.Instance.GetString("FileExtensions")
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the GUID
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	12/10/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared ReadOnly Property GUID() As String
            Get
                Return HostController.Instance.GetString("GUID")
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the Help URL
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	01/28/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared ReadOnly Property HelpURL() As String
            Get
                Return HostController.Instance.GetString("HelpURL")
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the Host Currency
        ''' </summary>
        ''' <remarks>Defaults to USD</remarks>
        ''' <history>
        ''' 	[cnurse]	01/29/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared ReadOnly Property HostCurrency() As String
            Get
                Dim setting As String = HostController.Instance.GetString("HostCurrency")
                If String.IsNullOrEmpty(setting) Then
                    setting = "USD"
                End If
                Return setting
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the Host Email
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	01/28/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared ReadOnly Property HostEmail() As String
            Get
                Return HostController.Instance.GetString("HostEmail")
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the Host Fee
        ''' </summary>
        ''' <remarks>Defaults to 0</remarks>
        ''' <history>
        ''' 	[cnurse]	01/28/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared ReadOnly Property HostFee() As Double
            Get
                Return HostController.Instance.GetDouble("HostFee", 0)
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the Host Portal's PortalId
        ''' </summary>
        ''' <remarks>Defaults to Null.NullInteger</remarks>
        ''' <history>
        ''' 	[cnurse]	01/28/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared ReadOnly Property HostPortalID() As Integer
            Get
                Return HostController.Instance.GetInteger("HostPortalId")
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the Host Space
        ''' </summary>
        ''' <remarks>Defaults to 0</remarks>
        ''' <history>
        ''' 	[cnurse]	01/28/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared ReadOnly Property HostSpace() As Double
            Get
                Return HostController.Instance.GetDouble("HostSpace", 0)
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the Host Title
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	12/10/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared ReadOnly Property HostTitle() As String
            Get
                Return HostController.Instance.GetString("HostTitle")
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the Host URL
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	04/29/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared ReadOnly Property HostURL() As String
            Get
                Return HostController.Instance.GetString("HostURL")
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the HttpCompression Algorithm
        ''' </summary>
        ''' <remarks>Defaults to Null.NullInteger(None)</remarks>
        ''' <history>
        ''' 	[cnurse]	01/28/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared ReadOnly Property HttpCompressionAlgorithm() As Integer
            Get
                Return HostController.Instance.GetInteger("HttpCompression")
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the Module Caching method
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	01/28/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared ReadOnly Property ModuleCachingMethod() As String
            Get
                Return HostController.Instance.GetString("ModuleCaching")
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the Page Caching method
        ''' </summary>
        ''' <history>
        ''' 	[jbrinkman]	11/17/2009   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared ReadOnly Property PageCachingMethod() As String
            Get
                Return HostController.Instance.GetString("PageCaching")
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the Page Quota
        ''' </summary>
        ''' <remarks>Defaults to 0</remarks>
        ''' <history>
        ''' 	[cnurse]	01/29/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared ReadOnly Property PageQuota() As Integer
            Get
                Return HostController.Instance.GetInteger("PageQuota", 0)
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the PageState Persister
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	01/28/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared ReadOnly Property PageStatePersister() As String
            Get
                Dim setting As String = HostController.Instance.GetString("PageStatePersister")
                If String.IsNullOrEmpty(setting) Then
                    setting = "P"
                End If
                Return setting
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the Password Expiry
        ''' </summary>
        ''' <remarks>Defaults to 0</remarks>
        ''' <history>
        ''' 	[cnurse]	01/28/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared ReadOnly Property PasswordExpiry() As Integer
            Get
                Return HostController.Instance.GetInteger("PasswordExpiry", 0)
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the Password Expiry Reminder window
        ''' </summary>
        ''' <remarks>Defaults to 7 (1 week)</remarks>
        ''' <history>
        ''' 	[cnurse]	01/28/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared ReadOnly Property PasswordExpiryReminder() As Integer
            Get
                Return HostController.Instance.GetInteger("PasswordExpiryReminder", 7)
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the Payment Processor
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	04/14/2009   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared ReadOnly Property PaymentProcessor() As String
            Get
                Return HostController.Instance.GetString("PaymentProcessor")
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the PerformanceSettings
        ''' </summary>
        ''' <remarks>Defaults to PerformanceSettings.ModerateCaching</remarks>
        ''' <history>
        ''' 	[cnurse]	01/28/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared ReadOnly Property PerformanceSetting() As PerformanceSettings
            Get
                Dim setting As PerformanceSettings = PerformanceSettings.ModerateCaching
                Dim s As String = HostController.Instance.GetString("PerformanceSetting")
                If Not String.IsNullOrEmpty(s) Then
                    setting = CType(s, PerformanceSettings)
                End If
                Return setting
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the Payment Processor Password
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	04/14/2009   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared ReadOnly Property ProcessorPassword() As String
            Get
                Return HostController.Instance.GetString("ProcessorPassword")
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the Payment Processor User Id
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	04/14/2009   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared ReadOnly Property ProcessorUserId() As String
            Get
                Return HostController.Instance.GetString("ProcessorUserId")
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the Proxy Server Password
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	01/28/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared ReadOnly Property ProxyPassword() As String
            Get
                Return HostController.Instance.GetString("ProxyPassword")
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the Proxy Server Port
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	01/28/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared ReadOnly Property ProxyPort() As Integer
            Get
                Return HostController.Instance.GetInteger("ProxyPort")
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the Proxy Server
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	01/28/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared ReadOnly Property ProxyServer() As String
            Get
                Return HostController.Instance.GetString("ProxyServer")
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the Proxy Server UserName
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	01/28/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared ReadOnly Property ProxyUsername() As String
            Get
                Return HostController.Instance.GetString("ProxyUsername")
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets whether to use the remember me checkbox
        ''' </summary>
        ''' <remarks>Defaults to False</remarks>
        ''' <history>
        ''' 	[cnurse]	04/14/2009   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared ReadOnly Property RememberCheckbox() As Boolean
            Get
                Return HostController.Instance.GetBoolean("RememberCheckbox", True)
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the Scheduler Mode
        ''' </summary>
        ''' <remarks>Defaults to SchedulerMode.TIMER_METHOD</remarks>
        ''' <history>
        ''' 	[cnurse]	01/28/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared ReadOnly Property SchedulerMode() As SchedulerMode
            Get
                Dim setting As SchedulerMode = Scheduling.SchedulerMode.TIMER_METHOD
                Dim s As String = HostController.Instance.GetString("SchedulerMode")
                If Not String.IsNullOrEmpty(s) Then
                    setting = CType(s, Services.Scheduling.SchedulerMode)
                End If
                Return setting
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets whether to inlcude Common Words in the Search Index
        ''' </summary>
        ''' <remarks>Defaults to False</remarks>
        ''' <history>
        ''' 	[cnurse]	03/10/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared ReadOnly Property SearchIncludeCommon() As Boolean
            Get
                Return HostController.Instance.GetBoolean("SearchIncludeCommon", False)
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets whether to inlcude Numbers in the Search Index
        ''' </summary>
        ''' <remarks>Defaults to False</remarks>
        ''' <history>
        ''' 	[cnurse]	03/10/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared ReadOnly Property SearchIncludeNumeric() As Boolean
            Get
                Return HostController.Instance.GetBoolean("SearchIncludeNumeric", True)
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the maximum Search Word length to index
        ''' </summary>
        ''' <remarks>Defaults to 25</remarks>
        ''' <history>
        ''' 	[cnurse]	03/10/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared ReadOnly Property SearchMaxWordlLength() As Integer
            Get
                Return HostController.Instance.GetInteger("MaxSearchWordLength", 50)
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the maximum Search Word length to index
        ''' </summary>
        ''' <remarks>Defaults to 3</remarks>
        ''' <history>
        ''' 	[cnurse]	03/10/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared ReadOnly Property SearchMinWordlLength() As Integer
            Get
                Return HostController.Instance.GetInteger("MinSearchWordLength", 4)
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the Site Log Buffer size
        ''' </summary>
        ''' <remarks>Defaults to 1</remarks>
        ''' <history>
        ''' 	[cnurse]	01/29/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared ReadOnly Property SiteLogBuffer() As Integer
            Get
                Return HostController.Instance.GetInteger("SiteLogBuffer", 1)
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the Site Log History
        ''' </summary>
        ''' <remarks>Defaults to -1</remarks>
        ''' <history>
        ''' 	[cnurse]	01/29/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared ReadOnly Property SiteLogHistory() As Integer
            Get
                Return HostController.Instance.GetInteger("SiteLogHistory")
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the Site Log Storage location
        ''' </summary>
        ''' <remarks>Defaults to "D"</remarks>
        ''' <history>
        ''' 	[cnurse]	03/05/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared ReadOnly Property SiteLogStorage() As String
            Get
                Dim setting As String = HostController.Instance.GetString("SiteLogStorage")
                If String.IsNullOrEmpty(setting) Then
                    setting = "D"
                End If
                Return setting
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the SMTP Authentication
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	01/28/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared ReadOnly Property SMTPAuthentication() As String
            Get
                Return HostController.Instance.GetString("SMTPAuthentication")
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the SMTP Password
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	01/28/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared ReadOnly Property SMTPPassword() As String
            Get
                Return HostController.Instance.GetString("SMTPPassword")
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the SMTP Server
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	01/28/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared ReadOnly Property SMTPServer() As String
            Get
                Return HostController.Instance.GetString("SMTPServer")
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the SMTP Username
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	01/28/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared ReadOnly Property SMTPUsername() As String
            Get
                Return HostController.Instance.GetString("SMTPUsername")
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets whether Exceptions are rethrown
        ''' </summary>
        ''' <remarks>Defaults to False</remarks>
        ''' <history>
        ''' 	[cnurse]	07/24/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared ReadOnly Property ThrowCBOExceptions() As Boolean
            Get
                Return HostController.Instance.GetBoolean("ThrowCBOExceptions", False)
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets whether Friendly Urls is Enabled
        ''' </summary>
        ''' <remarks>Defaults to False</remarks>
        ''' <history>
        ''' 	[cnurse]	01/28/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared ReadOnly Property UseFriendlyUrls() As Boolean
            Get
                Return HostController.Instance.GetBoolean("UseFriendlyUrls", False)
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets whether Custom Error Messages is Enabled
        ''' </summary>
        ''' <remarks>Defaults to False</remarks>
        ''' <history>
        ''' 	[cnurse]	01/28/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared ReadOnly Property UseCustomErrorMessages() As Boolean
            Get
                Return HostController.Instance.GetBoolean("UseCustomErrorMessages", False)
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the User Quota
        ''' </summary>
        ''' <remarks>Defaults to 0</remarks>
        ''' <history>
        ''' 	[cnurse]	01/29/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared ReadOnly Property UserQuota() As Integer
            Get
                Return HostController.Instance.GetInteger("UserQuota", 0)
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the window to use in minutes when determining if the user is online
        ''' </summary>
        ''' <remarks>Defaults to 15</remarks>
        ''' <history>
        ''' 	[cnurse]	01/29/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared ReadOnly Property UsersOnlineTimeWindow() As Integer
            Get
                Return HostController.Instance.GetInteger("UsersOnlineTime", 15)
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the WebRequest Timeout value
        ''' </summary>
        ''' <remarks>Defaults to 10000</remarks>
        ''' <history>
        ''' 	[cnurse]	01/28/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared ReadOnly Property WebRequestTimeout() As Integer
            Get
                Return HostController.Instance.GetInteger("WebRequestTimeout", 10000)
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets whether the Whitespace Filter is Enabled
        ''' </summary>
        ''' <remarks>Defaults to False</remarks>
        ''' <history>
        ''' 	[cnurse]	01/28/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared ReadOnly Property WhitespaceFilter() As Boolean
            Get
                Return HostController.Instance.GetBoolean("WhitespaceFilter", False)
            End Get
        End Property

        ''' <summary>
        ''' Gets whether to use the minified or debug version of the jQuery scripts
        ''' </summary>
        ''' <remarks>Defaults to False</remarks>
        ''' <history>
        '''     [jbrinkman]    09/30/2008    Created
        ''' </history>
        Public Shared ReadOnly Property jQueryDebug() As Boolean
            Get
                Return HostController.Instance.GetBoolean("jQueryDebug", False)
            End Get
        End Property

        ''' <summary>
        ''' Gets whether to use a hosted version of the jQuery script file
        ''' </summary>
        ''' <remarks>Defaults to False</remarks>
        ''' <history>
        '''     [jbrinkman]    09/30/2008    Created
        ''' </history>
        Public Shared ReadOnly Property jQueryHosted() As Boolean
            Get
                Return HostController.Instance.GetBoolean("jQueryHosted", False)
            End Get
        End Property

        ''' <summary>
        ''' Gets the Url for a hosted version of jQuery
        ''' </summary>
        ''' <remarks>Defaults to the DefaultHostedUrl constant in the jQuery class.
        ''' The framework will default to the latest released 1.x version hosted on Google.
        ''' </remarks>
        ''' <history>
        '''     [jbrinkman]    09/30/2008    Created
        ''' </history>
        Public Shared ReadOnly Property jQueryUrl() As String
            Get
                If HttpContext.Current.Request.IsSecureConnection = True Then
                    Return HostController.Instance.GetString("jQueryUrl", jQuery.DefaultHostedUrl).Replace("http://", "https://")
                Else
                    Return HostController.Instance.GetString("jQueryUrl", jQuery.DefaultHostedUrl)
                End If
            End Get
        End Property

#End Region

#Region "Public Shared Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' GetHostSetting gets a single Host Setting
        ''' </summary>
        ''' <param name="key">The Setting key</param>
        ''' <history>
        ''' 	[cnurse]	01/28/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <Obsolete("Replaced in DotNetNuke 5.5 by HostController.GetHostSetting()")> _
        Private Shared Function GetHostSetting(ByVal key As String) As String
            Return HostController.Instance.GetString(key)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' GetHostSettingDictionary gets a Dictionary of Host Settings from the cache or
        ''' from the the Database.
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	07/15/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <Obsolete("Replaced in DotNetNuke 5.5 by HostController.GetHostSettingsDictionary()")> _
        Public Shared Function GetHostSettingsDictionary() As Dictionary(Of String, String)
            Dim dicSettings As Dictionary(Of String, String) = DataCache.GetCache(Of Dictionary(Of String, String))(DataCache.UnSecureHostSettingsCacheKey)

            If dicSettings Is Nothing Then
                dicSettings = New Dictionary(Of String, String)
                Dim dr As IDataReader = DataProvider.Instance().GetHostSettings
                Try
                    While dr.Read()
                        If Not dr.IsDBNull(1) Then
                            dicSettings.Add(dr.GetString(0), dr.GetString(1))
                        End If
                    End While
                Finally
                    CBO.CloseDataReader(dr, True)
                End Try

                'Save settings to cache
                Dim objDependency As DNNCacheDependency = Nothing
                DataCache.SetCache(DataCache.UnSecureHostSettingsCacheKey, dicSettings, objDependency, System.Web.Caching.Cache.NoAbsoluteExpiration, _
                                   TimeSpan.FromMinutes(DataCache.HostSettingsCacheTimeOut), DataCache.HostSettingsCachePriority, Nothing)
            End If

            Return dicSettings
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' GetHostSetting gets a single Secure Host Setting
        ''' </summary>
        ''' <param name="key">The Setting key</param>
        ''' <history>
        ''' 	[cnurse]	01/28/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <Obsolete("Replaced in DotNetNuke 5.5 by HostController.GetSecureHostSetting()")> _
        Public Shared Function GetSecureHostSetting(ByVal key As String) As String
            Return HostController.Instance.GetString(key)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' GetSecureHostSettingDictionary gets a Dictionary of SecureHost Settings from the cache or
        ''' from the the Database.
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	01/28/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <Obsolete("Replaced in DotNetNuke 5.5 by HostController.GetSecureHostSettingsDictionary()")> _
        Public Shared Function GetSecureHostSettingsDictionary() As Dictionary(Of String, String)
            Dim dicSettings As Dictionary(Of String, String) = DataCache.GetCache(Of Dictionary(Of String, String))(DataCache.SecureHostSettingsCacheKey)

            If dicSettings Is Nothing Then
                dicSettings = New Dictionary(Of String, String)
                Dim dr As IDataReader = DataProvider.Instance().GetHostSettings
                Try
                    While dr.Read()
                        If Not Convert.ToBoolean(dr(2)) Then
                            Dim settingName As String = dr.GetString(0)
                            If settingName.ToLower.IndexOf("password") = -1 Then
                                If Not dr.IsDBNull(1) Then
                                    dicSettings.Add(settingName, dr.GetString(1))
                                Else
                                    dicSettings.Add(settingName, "")
                                End If
                            End If
                        End If
                    End While
                Finally
                    CBO.CloseDataReader(dr, True)
                End Try

                'Save settings to cache
                Dim objDependency As DNNCacheDependency = Nothing
                DataCache.SetCache(DataCache.SecureHostSettingsCacheKey, dicSettings, objDependency, System.Web.Caching.Cache.NoAbsoluteExpiration, _
                                   TimeSpan.FromMinutes(DataCache.HostSettingsCacheTimeOut), DataCache.HostSettingsCachePriority, Nothing)
            End If

            Return dicSettings
        End Function

#End Region

#Region "Obsolete Properties"

        <Obsolete("Deprecated in 5.5.  This setting was never used and has been replaced in 5.5 by a Portal Setting as Content Localization is Portal based.")> _
        Public Shared ReadOnly Property ContentLocalization() As Boolean
            Get
                Return HostController.Instance.GetBoolean("ContentLocalization", False)
            End Get
        End Property

        <Obsolete("property obsoleted in 5.4.0 - code updated to use portalcontroller method")> _
        Public Shared ReadOnly Property ContentLocale() As String
            Get
                Return "en-us"

            End Get
        End Property

#End Region


    End Class

End Namespace
