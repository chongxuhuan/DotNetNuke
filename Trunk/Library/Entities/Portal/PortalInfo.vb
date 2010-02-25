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
Imports System.Configuration
Imports System.Data
Imports System.XML
Imports System.IO
Imports System.Xml.Serialization
Imports DotNetNuke.Entities.Modules
Imports System.Xml.Schema
Imports DotNetNuke.Security.Roles

Namespace DotNetNuke.Entities.Portals

    <XmlRoot("settings", IsNullable:=False)> <Serializable()> Public Class PortalInfo
        Inherits BaseEntityInfo
        Implements IHydratable

#Region "Private Members"

        Private _PortalID As Integer
        Private _PortalName As String
        Private _LogoFile As String
        Private _FooterText As String
        Private _ExpiryDate As Date
        Private _UserRegistration As Integer
        Private _BannerAdvertising As Integer
        Private _AdministratorId As Integer
        Private _Currency As String
        Private _HostFee As Single
        Private _HostSpace As Integer
        Private _PageQuota As Integer
        Private _UserQuota As Integer
        Private _AdministratorRoleId As Integer
        Private _AdministratorRoleName As String
        Private _RegisteredRoleId As Integer
        Private _RegisteredRoleName As String
        Private _Description As String
        Private _KeyWords As String
        Private _BackgroundFile As String
        Private _GUID As Guid
        Private _PaymentProcessor As String
        Private _ProcessorUserId As String
        Private _ProcessorPassword As String
        Private _SiteLogHistory As Integer
        Private _Email As String
        Private _AdminTabId As Integer
        Private _SuperTabId As Integer
        Private _Users As Integer = Null.NullInteger
        Private _Pages As Integer = Null.NullInteger
        Private _SplashTabId As Integer
        Private _HomeTabId As Integer
        Private _LoginTabId As Integer
        Private _UserTabId As Integer
        Private _DefaultLanguage As String
        Private _TimeZoneOffset As Integer
        Private _HomeDirectory As String
        Private _Version As String

#End Region

#Region "Constructors"

        ' initialization
        Public Sub New()
        End Sub

#End Region

#Region "Public Properties"

        <XmlElement("footertext")> Public Property FooterText() As String
            Get
                Return _FooterText
            End Get
            Set(ByVal Value As String)
                _FooterText = Value
            End Set
        End Property

        <XmlElement("logofile")> Public Property LogoFile() As String
            Get
                Return _LogoFile
            End Get
            Set(ByVal Value As String)
                _LogoFile = Value
            End Set
        End Property

        <XmlElement("portalid")> Public Property PortalID() As Integer
            Get
                Return _PortalID
            End Get
            Set(ByVal Value As Integer)
                _PortalID = Value
            End Set
        End Property

        <XmlElement("portalname")> Public Property PortalName() As String
            Get
                Return _PortalName
            End Get
            Set(ByVal Value As String)
                _PortalName = Value
            End Set
        End Property

        <XmlElement("expirydate")> Public Property ExpiryDate() As Date
            Get
                Return _ExpiryDate
            End Get
            Set(ByVal Value As Date)
                _ExpiryDate = Value
            End Set
        End Property

        <XmlElement("userregistration")> Public Property UserRegistration() As Integer
            Get
                Return _UserRegistration
            End Get
            Set(ByVal Value As Integer)
                _UserRegistration = Value
            End Set
        End Property

        <XmlElement("banneradvertising")> Public Property BannerAdvertising() As Integer
            Get
                Return _BannerAdvertising
            End Get
            Set(ByVal Value As Integer)
                _BannerAdvertising = Value
            End Set
        End Property

        <XmlElement("administratorid")> Public Property AdministratorId() As Integer
            Get
                Return _AdministratorId
            End Get
            Set(ByVal Value As Integer)
                _AdministratorId = Value
            End Set
        End Property

        <XmlElement("currency")> Public Property Currency() As String
            Get
                Return _Currency
            End Get
            Set(ByVal Value As String)
                _Currency = Value
            End Set
        End Property

        <XmlElement("hostfee")> Public Property HostFee() As Single
            Get
                Return _HostFee
            End Get
            Set(ByVal Value As Single)
                _HostFee = Value
            End Set
        End Property

        <XmlElement("hostspace")> Public Property HostSpace() As Integer
            Get
                Return _HostSpace
            End Get
            Set(ByVal Value As Integer)
                _HostSpace = Value
            End Set
        End Property

        <XmlElement("pagequota")> Public Property PageQuota() As Integer
            Get
                Return _PageQuota
            End Get
            Set(ByVal Value As Integer)
                _PageQuota = Value
            End Set
        End Property

        <XmlElement("userquota")> Public Property UserQuota() As Integer
            Get
                Return _UserQuota
            End Get
            Set(ByVal Value As Integer)
                _UserQuota = Value
            End Set
        End Property

        <XmlElement("administratorroleid")> Public Property AdministratorRoleId() As Integer
            Get
                Return _AdministratorRoleId
            End Get
            Set(ByVal Value As Integer)
                _AdministratorRoleId = Value
            End Set
        End Property

        <XmlElement("administratorrolename")> Public Property AdministratorRoleName() As String
            Get
                If _AdministratorRoleName = Null.NullString AndAlso AdministratorRoleId > Null.NullInteger Then
                    'Get Role Name
                    Dim adminRole As RoleInfo = New RoleController().GetRole(AdministratorRoleId, PortalID)
                    If adminRole IsNot Nothing Then
                        _AdministratorRoleName = adminRole.RoleName
                    End If
                End If
                Return _AdministratorRoleName
            End Get
            Set(ByVal Value As String)
                _AdministratorRoleName = Value
            End Set
        End Property

        <XmlElement("registeredroleid")> Public Property RegisteredRoleId() As Integer
            Get
                Return _RegisteredRoleId
            End Get
            Set(ByVal Value As Integer)
                _RegisteredRoleId = Value
            End Set
        End Property

        <XmlElement("registeredrolename")> Public Property RegisteredRoleName() As String
            Get
                If _RegisteredRoleName = Null.NullString AndAlso RegisteredRoleId > Null.NullInteger Then
                    'Get Role Name
                    Dim regUsersRole As RoleInfo = New RoleController().GetRole(RegisteredRoleId, PortalID)
                    If regUsersRole IsNot Nothing Then
                        _RegisteredRoleName = regUsersRole.RoleName
                    End If
                End If
                Return _RegisteredRoleName
            End Get
            Set(ByVal Value As String)
                _RegisteredRoleName = Value
            End Set
        End Property

        <XmlElement("description")> Public Property Description() As String
            Get
                Return _Description
            End Get
            Set(ByVal Value As String)
                _Description = Value
            End Set
        End Property

        <XmlElement("keywords")> Public Property KeyWords() As String
            Get
                Return _KeyWords
            End Get
            Set(ByVal Value As String)
                _KeyWords = Value
            End Set
        End Property

        <XmlElement("backgroundfile")> Public Property BackgroundFile() As String
            Get
                Return _BackgroundFile
            End Get
            Set(ByVal Value As String)
                _BackgroundFile = Value
            End Set
        End Property

        <XmlIgnore()> Public Property GUID() As Guid
            Get
                Return _GUID
            End Get
            Set(ByVal Value As Guid)
                _GUID = Value
            End Set
        End Property

        <XmlElement("paymentprocessor")> Public Property PaymentProcessor() As String
            Get
                Return _PaymentProcessor
            End Get
            Set(ByVal Value As String)
                _PaymentProcessor = Value
            End Set
        End Property

        <XmlElement("processorpassword")> Public Property ProcessorPassword() As String
            Get
                Return _ProcessorPassword
            End Get
            Set(ByVal Value As String)
                _ProcessorPassword = Value
            End Set
        End Property

        <XmlElement("processoruserid")> Public Property ProcessorUserId() As String
            Get
                Return _ProcessorUserId
            End Get
            Set(ByVal Value As String)
                _ProcessorUserId = Value
            End Set
        End Property

        <XmlElement("siteloghistory")> Public Property SiteLogHistory() As Integer
            Get
                Return _SiteLogHistory
            End Get
            Set(ByVal Value As Integer)
                _SiteLogHistory = Value
            End Set
        End Property

        <XmlElement("email")> Public Property Email() As String
            Get
                Return _Email
            End Get
            Set(ByVal Value As String)
                _Email = Value
            End Set
        End Property

        <XmlElement("admintabid")> Public Property AdminTabId() As Integer
            Get
                Return _AdminTabId
            End Get
            Set(ByVal Value As Integer)
                _AdminTabId = Value
            End Set
        End Property

        <XmlElement("supertabid")> Public Property SuperTabId() As Integer
            Get
                Return _SuperTabId
            End Get
            Set(ByVal Value As Integer)
                _SuperTabId = Value
            End Set
        End Property

        <XmlElement("users")> Public Property Users() As Integer
            Get
                Return _Users
            End Get
            Set(ByVal Value As Integer)
                _Users = Value
            End Set
        End Property

        <XmlElement("pages")> Public Property Pages() As Integer
            Get
                If _Pages < 0 Then
                    Dim objTabController As New TabController
                    _Pages = objTabController.GetTabCount(PortalID)
                End If
                Return _Pages
            End Get
            Set(ByVal Value As Integer)
                _Pages = Value
            End Set
        End Property

        <XmlElement("splashtabid")> Public Property SplashTabId() As Integer
            Get
                Return _SplashTabId
            End Get
            Set(ByVal Value As Integer)
                _SplashTabId = Value
            End Set
        End Property

        <XmlElement("hometabid")> Public Property HomeTabId() As Integer
            Get
                Return _HomeTabId
            End Get
            Set(ByVal Value As Integer)
                _HomeTabId = Value
            End Set
        End Property

        <XmlElement("logintabid")> Public Property LoginTabId() As Integer
            Get
                Return _LoginTabId
            End Get
            Set(ByVal Value As Integer)
                _LoginTabId = Value
            End Set
        End Property

        <XmlElement("usertabid")> Public Property UserTabId() As Integer
            Get
                Return _UserTabId
            End Get
            Set(ByVal Value As Integer)
                _UserTabId = Value
            End Set
        End Property

        <XmlElement("defaultlanguage")> Public Property DefaultLanguage() As String
            Get
                Return _DefaultLanguage
            End Get
            Set(ByVal Value As String)
                _DefaultLanguage = Value
            End Set
        End Property

        <XmlElement("timezoneoffset")> Public Property TimeZoneOffset() As Integer
            Get
                Return _TimeZoneOffset
            End Get
            Set(ByVal Value As Integer)
                _TimeZoneOffset = Value
            End Set
        End Property

        <XmlElement("homedirectory")> Public Property HomeDirectory() As String
            Get
                Return _HomeDirectory
            End Get
            Set(ByVal Value As String)
                _HomeDirectory = Value
            End Set
        End Property

        <XmlIgnore()> Public ReadOnly Property HomeDirectoryMapPath() As String
            Get
                Dim objFolderController As New Services.FileSystem.FolderController
                Return objFolderController.GetMappedDirectory(Common.Globals.ApplicationPath + "/" + HomeDirectory + "/")
            End Get

        End Property

        <XmlElement("version")> Public Property Version() As String
            Get
                Return _Version
            End Get
            Set(ByVal Value As String)
                _Version = Value
            End Set
        End Property

#End Region

#Region "IHydratable Implementation"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Fills a PortalInfo from a Data Reader
        ''' </summary>
        ''' <param name="dr">The Data Reader to use</param>
        ''' <history>
        ''' 	[cnurse]	01/28/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub Fill(ByVal dr As System.Data.IDataReader) Implements IHydratable.Fill
            PortalID = Null.SetNullInteger(dr("PortalID"))
            PortalName = Null.SetNullString(dr("PortalName"))
            LogoFile = Null.SetNullString(dr("LogoFile"))
            FooterText = Null.SetNullString(dr("FooterText"))
            ExpiryDate = Null.SetNullDateTime(dr("ExpiryDate"))
            UserRegistration = Null.SetNullInteger(dr("UserRegistration"))
            BannerAdvertising = Null.SetNullInteger(dr("BannerAdvertising"))
            AdministratorId = Null.SetNullInteger(dr("AdministratorID"))
            Email = Null.SetNullString(dr("Email"))
            Currency = Null.SetNullString(dr("Currency"))
            HostFee = Null.SetNullInteger(dr("HostFee"))
            HostSpace = Null.SetNullInteger(dr("HostSpace"))
            PageQuota = Null.SetNullInteger(dr("PageQuota"))
            UserQuota = Null.SetNullInteger(dr("UserQuota"))
            AdministratorRoleId = Null.SetNullInteger(dr("AdministratorRoleID"))
            RegisteredRoleId = Null.SetNullInteger(dr("RegisteredRoleID"))
            Description = Null.SetNullString(dr("Description"))
            KeyWords = Null.SetNullString(dr("KeyWords"))
            BackgroundFile = Null.SetNullString(dr("BackGroundFile"))
            GUID = New Guid(Null.SetNullString(dr("GUID")))
            PaymentProcessor = Null.SetNullString(dr("PaymentProcessor"))
            ProcessorUserId = Null.SetNullString(dr("ProcessorUserId"))
            ProcessorPassword = Null.SetNullString(dr("ProcessorPassword"))
            SiteLogHistory = Null.SetNullInteger(dr("SiteLogHistory"))
            SplashTabId = Null.SetNullInteger(dr("SplashTabID"))
            HomeTabId = Null.SetNullInteger(dr("HomeTabID"))
            LoginTabId = Null.SetNullInteger(dr("LoginTabID"))
            UserTabId = Null.SetNullInteger(dr("UserTabID"))
            DefaultLanguage = Null.SetNullString(dr("DefaultLanguage"))
            TimeZoneOffset = Null.SetNullInteger(dr("TimeZoneOffset"))
            AdminTabId = Null.SetNullInteger(dr("AdminTabID"))
            HomeDirectory = Null.SetNullString(dr("HomeDirectory"))
            SuperTabId = Null.SetNullInteger(dr("SuperTabId"))
            FillInternal(dr)
            AdministratorRoleName = Null.NullString
            RegisteredRoleName = Null.NullString

            'Aggressively load Users
            Users = UserController.GetUserCountByPortal(PortalID)
            Pages = Null.NullInteger
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the Key ID
        ''' </summary>
        ''' <returns>An Integer</returns>
        ''' <history>
        ''' 	[cnurse]	01/28/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property KeyID() As Integer Implements IHydratable.KeyID
            Get
                Return PortalID
            End Get
            Set(ByVal value As Integer)
                PortalID = value
            End Set
        End Property

#End Region

    End Class


End Namespace