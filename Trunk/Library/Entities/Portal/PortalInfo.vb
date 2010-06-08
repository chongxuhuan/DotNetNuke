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
    ''' <summary>
    ''' PortalInfo provides a base class for Portal information
    ''' This class inherites from the <c>BaseEntityInfo</c> and is <c>Hydratable</c>
    ''' </summary>
    ''' <remarks><seealso cref="IHydratable"/>
    ''' <example>This example shows how the <c>PortalInfo</c> class is used to get physical file names
    '''  <code lang="vbnet">
    ''' Public ReadOnly Property PhysicalPath() As String
    '''        Get
    '''            Dim _PhysicalPath As String
    '''            Dim PortalSettings As PortalSettings = Nothing
    '''            If Not HttpContext.Current Is Nothing Then
    '''                PortalSettings = PortalController.GetCurrentPortalSettings()
    '''            End If
    '''            If PortalId = Null.NullInteger Then
    '''                _PhysicalPath = DotNetNuke.Common.Globals.HostMapPath + RelativePath
    '''            Else
    '''                If PortalSettings Is Nothing OrElse PortalSettings.PortalId &lt;&gt; PortalId Then
    '''                    ' Get the PortalInfo  based on the Portalid
    '''                    Dim objPortals As New PortalController()
    '''                    Dim objPortal As PortalInfo = objPortals.GetPortal(PortalId)
    '''                    _PhysicalPath = objPortal.HomeDirectoryMapPath + RelativePath
    '''                Else
    '''                    _PhysicalPath = PortalSettings.HomeDirectoryMapPath + RelativePath
    '''                End If
    '''            End If
    '''            Return _PhysicalPath.Replace("/", "\")
    '''        End Get
    ''' End Property 
    ''' </code>
    ''' </example>
    ''' </remarks>
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
        Private _RegisterTabId As Integer
        Private _UserTabId As Integer
        Private _DefaultLanguage As String
        Private _TimeZoneOffset As Integer
        Private _HomeDirectory As String
        Private _Version As String

#End Region

#Region "Constructors"

        ''' <summary>
        ''' Create new Portalinfo instance
        ''' </summary>
        ''' <remarks>
        ''' <example>This example illustrates the creation of a new <c>PortalInfo</c> object
        ''' <code lang="vbnet">
        ''' For Each portal As PortalInfo In New PortalController().GetPortals
        '''     Dim portalID As Integer = portal.PortalID
        '''     ...
        ''' Next
        ''' </code>
        ''' </example>
        ''' </remarks>
        Public Sub New()
        End Sub

#End Region

#Region "Public Properties"
        ''' <summary>
        ''' The footer text as specified in the Portal settings
        ''' </summary>
        ''' <value>Footer text of the portal</value>
        ''' <returns>Returns the the footer text of the portal</returns>
        ''' <remarks>
        ''' <example>This show the usage of the <c>FooterText</c> property
        ''' <code lang="vbnet">
        ''' txtFooterText.Text = objPortal.FooterText
        ''' </code>
        ''' </example>
        ''' </remarks>
        <XmlElement("footertext")> Public Property FooterText() As String
            Get
                Return _FooterText
            End Get
            Set(ByVal Value As String)
                _FooterText = Value
            End Set
        End Property
        ''' <summary>
        ''' The portal has a logo (bitmap) associated with the portal. Teh admin can set the logo in the portal settings
        ''' </summary>
        ''' <value>URL of the logo</value>
        ''' <returns>URL of the Portal logo</returns>
        ''' <remarks><example><code lang="vbnet">
        '''  urlLogo.Url = objPortal.LogoFile
        '''  urlLogo.FileFilter = glbImageFileTypes
        '''</code></example></remarks>
        <XmlElement("logofile")> Public Property LogoFile() As String
            Get
                Return _LogoFile
            End Get
            Set(ByVal Value As String)
                _LogoFile = Value
            End Set
        End Property
        ''' <summary>
        ''' Unique idenitifier of the Portal within the site
        ''' </summary>
        ''' <value>Portal identifier</value>
        ''' <returns>Portal Identifier</returns>
        ''' <remarks></remarks>
        <XmlElement("portalid")> Public Property PortalID() As Integer
            Get
                Return _PortalID
            End Get
            Set(ByVal Value As Integer)
                _PortalID = Value
            End Set
        End Property
        ''' <summary>
        ''' Name of the portal. Can be set at creation time, Admin can change the name in the portal settings
        ''' </summary>
        ''' <value>Name of the portal</value>
        ''' <returns>Name of the portal</returns>
        ''' <remarks><example>This show the usage of the <c>PortalName</c> property
        ''' <code lang="vbnet">
        ''' Dim objPortalController As New PortalController
        ''' Dim objPortal As PortalInfo = objPortalController.GetPortal(PortalID)
        '''      txtPortalName.Text = objPortal.PortalName
        '''      txtDescription.Text = objPortal.Description
        '''      txtKeyWords.Text = objPortal.KeyWords
        '''  </code></example></remarks>
        <XmlElement("portalname")> Public Property PortalName() As String
            Get
                Return _PortalName
            End Get
            Set(ByVal Value As String)
                _PortalName = Value
            End Set
        End Property
        ''' <summary>
        ''' Date at which the portal expires
        ''' </summary>
        ''' <value>Date of expiration of the portal</value>
        ''' <returns>Date of expiration of the portal</returns>
        ''' <remarks><example>This show the Portal expiration date usage
        ''' <code lang="vbnet">
        ''' If Not Null.IsNull(objPortal.ExpiryDate) Then
        '''     txtExpiryDate.Text = objPortal.ExpiryDate.ToShortDateString
        ''' End If
        ''' txtHostFee.Text = objPortal.HostFee.ToString
        ''' txtHostSpace.Text = objPortal.HostSpace.ToString
        ''' txtPageQuota.Text = objPortal.PageQuota.ToString
        ''' txtUserQuota.Text = objPortal.UserQuota.ToString
        ''' If Not IsDBNull(objPortal.SiteLogHistory) Then
        '''     txtSiteLogHistory.Text = objPortal.SiteLogHistory.ToString
        ''' End If
        ''' </code></example></remarks>
        <XmlElement("expirydate")> Public Property ExpiryDate() As Date
            Get
                Return _ExpiryDate
            End Get
            Set(ByVal Value As Date)
                _ExpiryDate = Value
            End Set
        End Property
        ''' <summary>
        ''' Type of registration that the portal supports
        ''' </summary>
        ''' <value>Type of registration</value>
        ''' <returns>Type of registration</returns>
        ''' <remarks><example>Registration type
        ''' <code lang="vbnet">
        ''' optUserRegistration.SelectedIndex = objPortal.UserRegistration
        ''' </code></example></remarks>
        <XmlElement("userregistration")> Public Property UserRegistration() As Integer
            Get
                Return _UserRegistration
            End Get
            Set(ByVal Value As Integer)
                _UserRegistration = Value
            End Set
        End Property
        ''' <summary>
        ''' Setting for the type of banner advertising in the portal
        ''' </summary>
        ''' <value>Type of banner advertising</value>
        ''' <returns>Type of banner advertising</returns>
        ''' <remarks><example>This show the usage of BannerAdvertising setting
        ''' <code lang="vbnet">
        ''' optBanners.SelectedIndex = objPortal.BannerAdvertising
        ''' </code></example></remarks>
        <XmlElement("banneradvertising")> Public Property BannerAdvertising() As Integer
            Get
                Return _BannerAdvertising
            End Get
            Set(ByVal Value As Integer)
                _BannerAdvertising = Value
            End Set
        End Property
        ''' <summary>
        ''' UserID of the user who is the admininistrator of the portal
        ''' </summary>
        ''' <value>UserId of the user who is the portal admin</value>
        ''' <returns>UserId of the user who is the portal admin</returns>
        ''' <remarks><example>This show the usage of the <c>AdministratorId</c>
        ''' <code lang="vbnet">
        ''' Dim Arr As ArrayList = objRoleController.GetUserRolesByRoleName(intPortalId, objPortal.AdministratorRoleName)
        ''' Dim i As Integer
        '''       For i = 0 To Arr.Count - 1
        '''             Dim objUser As UserRoleInfo = CType(Arr(i), UserRoleInfo)
        '''             cboAdministratorId.Items.Add(New ListItem(objUser.FullName, objUser.UserID.ToString))
        '''      Next
        '''      If Not cboAdministratorId.Items.FindByValue(objPortal.AdministratorId.ToString) Is Nothing Then
        '''          cboAdministratorId.Items.FindByValue(objPortal.AdministratorId.ToString).Selected = True
        '''      End If
        '''</code></example></remarks>
        <XmlElement("administratorid")> Public Property AdministratorId() As Integer
            Get
                Return _AdministratorId
            End Get
            Set(ByVal Value As Integer)
                _AdministratorId = Value
            End Set
        End Property
        ''' <summary>
        ''' Curreny format that is used in the portal
        ''' </summary>
        ''' <value>Currency of the portal</value>
        ''' <returns>Currency of the portal</returns>
        ''' <remarks><example>This exampels show the usage of the Currentcy property
        ''' <code lang="vbnet">
        ''' cboCurrency.DataSource = colList
        ''' cboCurrency.DataBind()
        ''' If Null.IsNull(objPortal.Currency) Or cboCurrency.Items.FindByValue(objPortal.Currency) Is Nothing Then
        '''     cboCurrency.Items.FindByValue("USD").Selected = True
        ''' Else
        '''     cboCurrency.Items.FindByValue(objPortal.Currency).Selected = True
        ''' End If
        ''' </code></example></remarks>
        <XmlElement("currency")> Public Property Currency() As String
            Get
                Return _Currency
            End Get
            Set(ByVal Value As String)
                _Currency = Value
            End Set
        End Property
        ''' <summary>
        ''' Amount of currency that is used as a hosting fee of the portal
        ''' </summary>
        ''' <value>Currency amount hosting fee</value>
        ''' <returns>Currency amount hosting fee</returns>
        ''' <remarks><example>This show the Portal <c>HostFee</c>usage
        ''' <code lang="vbnet">
        ''' If Not Null.IsNull(objPortal.ExpiryDate) Then
        '''     txtExpiryDate.Text = objPortal.ExpiryDate.ToShortDateString
        ''' End If
        ''' txtHostFee.Text = objPortal.HostFee.ToString
        ''' txtHostSpace.Text = objPortal.HostSpace.ToString
        ''' txtPageQuota.Text = objPortal.PageQuota.ToString
        ''' txtUserQuota.Text = objPortal.UserQuota.ToString
        ''' If Not IsDBNull(objPortal.SiteLogHistory) Then
        '''     txtSiteLogHistory.Text = objPortal.SiteLogHistory.ToString
        ''' End If
        ''' </code></example></remarks>
        <XmlElement("hostfee")> Public Property HostFee() As Single
            Get
                Return _HostFee
            End Get
            Set(ByVal Value As Single)
                _HostFee = Value
            End Set
        End Property
        ''' <summary>
        ''' Total disk space allowed for the portal (Mb). 0 means not limited
        ''' </summary>
        ''' <value>Diskspace allowed for the portal</value>
        ''' <returns>Diskspace allowed for the portal</returns>
        ''' <remarks><example>This show the Portal <c>HostSpace</c>usage
        ''' <code lang="vbnet">
        ''' If Not Null.IsNull(objPortal.ExpiryDate) Then
        '''     txtExpiryDate.Text = objPortal.ExpiryDate.ToShortDateString
        ''' End If
        ''' txtHostFee.Text = objPortal.HostFee.ToString
        ''' txtHostSpace.Text = objPortal.HostSpace.ToString
        ''' txtPageQuota.Text = objPortal.PageQuota.ToString
        ''' txtUserQuota.Text = objPortal.UserQuota.ToString
        ''' If Not IsDBNull(objPortal.SiteLogHistory) Then
        '''     txtSiteLogHistory.Text = objPortal.SiteLogHistory.ToString
        ''' End If
        ''' </code></example></remarks>
        <XmlElement("hostspace")> Public Property HostSpace() As Integer
            Get
                Return _HostSpace
            End Get
            Set(ByVal Value As Integer)
                _HostSpace = Value
            End Set
        End Property
        ''' <summary>
        ''' Number of portal pages allowed in the portal. 0 means not limited
        ''' </summary>
        ''' <value>Number of portal pages allowed</value>
        ''' <returns>Number of portal pages allowed</returns>
        ''' <remarks><example>This show the Portal <c>PageQuota</c>usage
        ''' <code lang="vbnet">
        ''' If Not Null.IsNull(objPortal.ExpiryDate) Then
        '''     txtExpiryDate.Text = objPortal.ExpiryDate.ToShortDateString
        ''' End If
        ''' txtHostFee.Text = objPortal.HostFee.ToString
        ''' txtHostSpace.Text = objPortal.HostSpace.ToString
        ''' txtPageQuota.Text = objPortal.PageQuota.ToString
        ''' txtUserQuota.Text = objPortal.UserQuota.ToString
        ''' If Not IsDBNull(objPortal.SiteLogHistory) Then
        '''     txtSiteLogHistory.Text = objPortal.SiteLogHistory.ToString
        ''' End If
        ''' </code></example></remarks>
        <XmlElement("pagequota")> Public Property PageQuota() As Integer
            Get
                Return _PageQuota
            End Get
            Set(ByVal Value As Integer)
                _PageQuota = Value
            End Set
        End Property
        ''' <summary>
        ''' Number of registered users allowed in the portal. 0 means not limited
        ''' </summary>
        ''' <value>Number of registered users allowed </value>
        ''' <returns>Number of registered users allowed </returns>
        ''' <remarks><example>This show the Portal userQuota usage
        ''' <code lang="vbnet">
        ''' If Not Null.IsNull(objPortal.ExpiryDate) Then
        '''     txtExpiryDate.Text = objPortal.ExpiryDate.ToShortDateString
        ''' End If
        ''' txtHostFee.Text = objPortal.HostFee.ToString
        ''' txtHostSpace.Text = objPortal.HostSpace.ToString
        ''' txtPageQuota.Text = objPortal.PageQuota.ToString
        ''' txtUserQuota.Text = objPortal.UserQuota.ToString
        ''' If Not IsDBNull(objPortal.SiteLogHistory) Then
        '''     txtSiteLogHistory.Text = objPortal.SiteLogHistory.ToString
        ''' End If
        ''' </code></example></remarks>
        <XmlElement("userquota")> Public Property UserQuota() As Integer
            Get
                Return _UserQuota
            End Get
            Set(ByVal Value As Integer)
                _UserQuota = Value
            End Set
        End Property
        ''' <summary>
        ''' The RoleId of the Security Role of the Administrators group of the portal
        ''' </summary>
        ''' <value>RoleId of de Administrators Security Role</value>
        ''' <returns>RoleId of de Administrators Security Role</returns>
        ''' <remarks><example>This shows the usage of the AdministratoprRoleId
        ''' <code lang="vbnet">
        ''' Dim objPortal As PortalInfo = New PortalController().GetPortal(PortalID)
        '''     If RoleID = objPortal.AdministratorRoleId Then
        '''         _RoleType = Roles.RoleType.Administrator
        '''     ElseIf RoleID = objPortal.RegisteredRoleId Then
        '''         _RoleType = Roles.RoleType.RegisteredUser
        '''     ElseIf RoleName = "Subscribers" Then
        '''         _RoleType = Roles.RoleType.Subscriber
        '''     End If
        ''' </code>
        ''' </example>
        ''' </remarks>
        <XmlElement("administratorroleid")> Public Property AdministratorRoleId() As Integer
            Get
                Return _AdministratorRoleId
            End Get
            Set(ByVal Value As Integer)
                _AdministratorRoleId = Value
            End Set
        End Property
        ''' <summary>
        ''' The actual name of the Administrators group of the portal.
        ''' This name is retrieved from the RoleController object
        ''' </summary>
        ''' <value>The name of the Administrators group</value>
        ''' <returns>The name of the Administrators group</returns>
        ''' <remarks></remarks>
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
        ''' <summary>
        ''' The RoleId of the Registered users group of the portal.
        ''' </summary>
        ''' <value>RoleId of the Registered users </value>
        ''' <returns>RoleId of the Registered users </returns>
        ''' <remarks></remarks>
        <XmlElement("registeredroleid")> Public Property RegisteredRoleId() As Integer
            Get
                Return _RegisteredRoleId
            End Get
            Set(ByVal Value As Integer)
                _RegisteredRoleId = Value
            End Set
        End Property
        ''' <summary>
        ''' The actual name of the Registerd Users group of the portal.
        ''' This name is retrieved from the RoleController object
        ''' </summary>
        ''' <value>The name of the Registerd Users group</value>
        ''' <returns>The name of the Registerd Users group</returns>
        ''' <remarks></remarks>
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
        ''' <summary>
        ''' Description of the portal
        ''' </summary>
        ''' <value>Description of the portal</value>
        ''' <returns>Description of the portal</returns>
        ''' <remarks><example>This show the usage of the <c>Description</c> property
        ''' <code lang="vbnet">
        ''' Dim objPortalController As New PortalController
        ''' Dim objPortal As PortalInfo = objPortalController.GetPortal(PortalID)
        '''      txtPortalName.Text = objPortal.PortalName
        '''      txtDescription.Text = objPortal.Description
        '''      txtKeyWords.Text = objPortal.KeyWords
        '''  </code></example></remarks>
        <XmlElement("description")> Public Property Description() As String
            Get
                Return _Description
            End Get
            Set(ByVal Value As String)
                _Description = Value
            End Set
        End Property
        ''' <summary>
        ''' Keywords (separated by ,) for this portal
        ''' </summary>
        ''' <value>Keywords seperated by ,</value>
        ''' <returns>Keywords for this portal</returns>
        ''' <remarks><example>This show the usage of the <c>KeyWords</c> property
        ''' <code lang="vbnet">
        ''' Dim objPortalController As New PortalController
        ''' Dim objPortal As PortalInfo = objPortalController.GetPortal(PortalID)
        '''      txtPortalName.Text = objPortal.PortalName
        '''      txtDescription.Text = objPortal.Description
        '''      txtKeyWords.Text = objPortal.KeyWords
        '''  </code></example></remarks>
        <XmlElement("keywords")> Public Property KeyWords() As String
            Get
                Return _KeyWords
            End Get
            Set(ByVal Value As String)
                _KeyWords = Value
            End Set
        End Property
        ''' <summary>
        ''' Image (bitmap) file that is used as background for the portal
        ''' </summary>
        ''' <value>Name of the file that is used as background</value>
        ''' <returns>Name of the file that is used as background</returns>
        ''' <remarks></remarks>
        <XmlElement("backgroundfile")> Public Property BackgroundFile() As String
            Get
                Return _BackgroundFile
            End Get
            Set(ByVal Value As String)
                _BackgroundFile = Value
            End Set
        End Property
        ''' <summary>
        ''' GUID of the portal info object
        ''' </summary>
        ''' <value>Portal info Object GUID</value>
        ''' <returns>GUD of the portal info object</returns>
        ''' <remarks></remarks>
        <XmlIgnore()> Public Property GUID() As Guid
            Get
                Return _GUID
            End Get
            Set(ByVal Value As Guid)
                _GUID = Value
            End Set
        End Property
        ''' <summary>
        ''' Name of the Payment processor that is used for portal payments, e.g. PayPal
        ''' </summary>
        ''' <value>Name of the portal payment processor</value>
        ''' <returns>Name of the portal payment processor</returns>
        ''' <remarks></remarks>
        <XmlElement("paymentprocessor")> Public Property PaymentProcessor() As String
            Get
                Return _PaymentProcessor
            End Get
            Set(ByVal Value As String)
                _PaymentProcessor = Value
            End Set
        End Property
        ''' <summary>
        ''' Password to use in the payment processor
        ''' </summary>
        ''' <value>Payment Processor password</value>
        ''' <returns></returns>
        ''' <remarks><example>This shows the usage of the payment processing
        ''' <code lang="vbnet">
        ''' If objPortal.PaymentProcessor &lt;&gt; "" Then
        '''     If Not cboProcessor.Items.FindByText(objPortal.PaymentProcessor) Is Nothing Then
        '''         cboProcessor.Items.FindByText(objPortal.PaymentProcessor).Selected = True
        '''     Else       ' default
        '''          If Not cboProcessor.Items.FindByText("PayPal") Is Nothing Then
        '''                cboProcessor.Items.FindByText("PayPal").Selected = True
        '''           End If
        '''      End If
        '''      Else
        '''      cboProcessor.Items.FindByValue("").Selected = True
        ''' End If
        ''' txtUserId.Text = objPortal.ProcessorUserId
        ''' txtPassword.Attributes.Add("value", objPortal.ProcessorPassword)
        ''' </code></example></remarks>
        <XmlElement("processorpassword")> Public Property ProcessorPassword() As String
            Get
                Return _ProcessorPassword
            End Get
            Set(ByVal Value As String)
                _ProcessorPassword = Value
            End Set
        End Property
        ''' <summary>
        ''' Payment Processor userId
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks> <seealso cref="PaymentProcessor"></seealso>
        ''' <example>This shows the usage of the payment processing
        ''' <code lang="vbnet">
        ''' If objPortal.PaymentProcessor &lt;&gt; "" Then
        '''     If Not cboProcessor.Items.FindByText(objPortal.PaymentProcessor) Is Nothing Then
        '''         cboProcessor.Items.FindByText(objPortal.PaymentProcessor).Selected = True
        '''     Else       ' default
        '''          If Not cboProcessor.Items.FindByText("PayPal") Is Nothing Then
        '''                cboProcessor.Items.FindByText("PayPal").Selected = True
        '''           End If
        '''      End If
        '''      Else
        '''      cboProcessor.Items.FindByValue("").Selected = True
        ''' End If
        ''' txtUserId.Text = objPortal.ProcessorUserId
        ''' txtPassword.Attributes.Add("value", objPortal.ProcessorPassword)
        ''' </code></example></remarks>
        <XmlElement("processoruserid")> Public Property ProcessorUserId() As String
            Get
                Return _ProcessorUserId
            End Get
            Set(ByVal Value As String)
                _ProcessorUserId = Value
            End Set
        End Property
        ''' <summary>
        ''' # of days that Site log history should be kept. 0 means unlimited
        ''' </summary>
        ''' <value># of days sitelog history</value>
        ''' <returns># of days sitelog history</returns>
        ''' <remarks><example>This show the Portal <c>SiteLogHistory</c>usage
        ''' <code lang="vbnet">
        ''' If Not Null.IsNull(objPortal.ExpiryDate) Then
        '''     txtExpiryDate.Text = objPortal.ExpiryDate.ToShortDateString
        ''' End If
        ''' txtHostFee.Text = objPortal.HostFee.ToString
        ''' txtHostSpace.Text = objPortal.HostSpace.ToString
        ''' txtPageQuota.Text = objPortal.PageQuota.ToString
        ''' txtUserQuota.Text = objPortal.UserQuota.ToString
        ''' If Not IsDBNull(objPortal.SiteLogHistory) Then
        '''     txtSiteLogHistory.Text = objPortal.SiteLogHistory.ToString
        ''' End If
        ''' </code></example></remarks>
        <XmlElement("siteloghistory")> Public Property SiteLogHistory() As Integer
            Get
                Return _SiteLogHistory
            End Get
            Set(ByVal Value As Integer)
                _SiteLogHistory = Value
            End Set
        End Property
        ''' <summary>
        ''' The default e-mail to be used in the porta;
        ''' </summary>
        ''' <value>E-mail of the portal</value>
        ''' <returns>E-mail of the portal</returns>
        ''' <remarks></remarks>
        <XmlElement("email")> Public Property Email() As String
            Get
                Return _Email
            End Get
            Set(ByVal Value As String)
                _Email = Value
            End Set
        End Property
        ''' <summary>
        ''' TabId at which admin tasks start
        ''' </summary>
        ''' <value>TabID of admin tasks</value>
        ''' <returns>TabID of admin tasks</returns>
        ''' <remarks></remarks>
        <XmlElement("admintabid")> Public Property AdminTabId() As Integer
            Get
                Return _AdminTabId
            End Get
            Set(ByVal Value As Integer)
                _AdminTabId = Value
            End Set
        End Property
        ''' <summary>
        ''' TabId at which Host tasks start
        ''' </summary>
        ''' <value>TabId of Host tasks</value>
        ''' <returns>TabId of Host tasks</returns>
        ''' <remarks></remarks>
        <XmlElement("supertabid")> Public Property SuperTabId() As Integer
            Get
                Return _SuperTabId
            End Get
            Set(ByVal Value As Integer)
                _SuperTabId = Value
            End Set
        End Property
        ''' <summary>
        ''' Actual number of actual users for this portal
        ''' </summary>
        ''' <value>Number of users for the portal</value>
        ''' <returns>Number of users for the portal</returns>
        ''' <remarks></remarks>
        <XmlElement("users")> Public Property Users() As Integer
            Get
                Return _Users
            End Get
            Set(ByVal Value As Integer)
                _Users = Value
            End Set
        End Property
        ''' <summary>
        ''' Actual number of pages of the portal
        ''' </summary>
        ''' <value>Number of pages of the portal</value>
        ''' <returns>Number of pages of the portal</returns>
        ''' <remarks></remarks>
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
        ''' <summary>
        ''' TabdId of the splash page. If 0, there is no splash page
        ''' </summary>
        ''' <value>TabdId of the Splash page</value>
        ''' <returns>TabdId of the Splash page</returns>
        ''' <remarks></remarks>
        <XmlElement("splashtabid")> Public Property SplashTabId() As Integer
            Get
                Return _SplashTabId
            End Get
            Set(ByVal Value As Integer)
                _SplashTabId = Value
            End Set
        End Property
        ''' <summary>
        ''' TabdId of the Home page
        ''' </summary>
        ''' <value>TabId of the Home page</value>
        ''' <returns>TabId of the Home page</returns>
        ''' <remarks></remarks>
        <XmlElement("hometabid")> Public Property HomeTabId() As Integer
            Get
                Return _HomeTabId
            End Get
            Set(ByVal Value As Integer)
                _HomeTabId = Value
            End Set
        End Property
        ''' <summary>
        ''' TabId with the login control, page to login
        ''' </summary>
        ''' <value>TabId of the Login page</value>
        ''' <returns>TabId of the Login page</returns>
        ''' <remarks></remarks>
        <XmlElement("logintabid")> Public Property LoginTabId() As Integer
            Get
                Return _LoginTabId
            End Get
            Set(ByVal Value As Integer)
                _LoginTabId = Value
            End Set
        End Property

        ''' <summary>
        ''' Tabid of the Registration page
        ''' </summary>
        ''' <value>TabId of the Registration page</value>
        ''' <returns>TabId of the Registration page</returns>
        ''' <remarks></remarks>
        <XmlElement("registertabid")> Public Property RegisterTabId() As Integer
            Get
                Return _RegisterTabId
            End Get
            Set(ByVal Value As Integer)
                _RegisterTabId = Value
            End Set
        End Property

        ''' <summary>
        ''' Tabid of the User profile page
        ''' </summary>
        ''' <value>TabdId of the User profile page</value>
        ''' <returns>TabdId of the User profile page</returns>
        ''' <remarks></remarks>
        <XmlElement("usertabid")> Public Property UserTabId() As Integer
            Get
                Return _UserTabId
            End Get
            Set(ByVal Value As Integer)
                _UserTabId = Value
            End Set
        End Property

        ''' <summary>
        ''' Default language for the portal
        ''' </summary>
        ''' <value>Default language of the portal</value>
        ''' <returns>Default language of the portal</returns>
        ''' <remarks></remarks>
        <XmlElement("defaultlanguage")> Public Property DefaultLanguage() As String
            Get
                Return _DefaultLanguage
            End Get
            Set(ByVal Value As String)
                _DefaultLanguage = Value
            End Set
        End Property

        ''' <summary>
        ''' Default Timezone offset for the portal
        ''' </summary>
        ''' <value>Default Timezone offset for the portal</value>
        ''' <returns>Default Timezone offset for the portal</returns>
        ''' <remarks></remarks>
        <XmlElement("timezoneoffset")> Public Property TimeZoneOffset() As Integer
            Get
                Return _TimeZoneOffset
            End Get
            Set(ByVal Value As Integer)
                _TimeZoneOffset = Value
            End Set
        End Property

        ''' <summary>
        ''' Home directory of the portal (logical path)
        ''' </summary>
        ''' <value>Portal home directory</value>
        ''' <returns>Portal home directory</returns>
        ''' <remarks><seealso cref="HomeDirectoryMapPath"></seealso></remarks>
        <XmlElement("homedirectory")> Public Property HomeDirectory() As String
            Get
                Return _HomeDirectory
            End Get
            Set(ByVal Value As String)
                _HomeDirectory = Value
            End Set
        End Property

        ''' <summary>
        ''' Fysical path on disk of the home directory of the portal
        ''' </summary>
        ''' <value></value>
        ''' <returns>Fully qualified path of the home directory</returns>
        ''' <remarks><seealso cref="HomeDirectory"></seealso></remarks>
        <XmlIgnore()> Public ReadOnly Property HomeDirectoryMapPath() As String
            Get
                Dim objFolderController As New Services.FileSystem.FolderController
                Return objFolderController.GetMappedDirectory(String.Format("{0}/{1}/", Common.Globals.ApplicationPath, HomeDirectory))
            End Get

        End Property

        ''' <summary>
        ''' DNN Version # of the portal installation
        ''' </summary>
        ''' <value>Version # of the portal installation</value>
        ''' <returns>Version # of the portal installation</returns>
        ''' <remarks></remarks>
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

        ''' <summary>
        ''' Fills a PortalInfo from a Data Reader
        ''' </summary>
        ''' <param name="dr">The Data Reader to use</param>
        ''' <remarks>Standard IHydratable.Fill implementation
        ''' <seealso cref="KeyID"></seealso></remarks>
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
            RegisterTabId = Null.SetNullInteger(dr("RegisterTabID"))
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

        ''' <summary>
        ''' Gets and sets the Key ID
        ''' </summary>
        ''' <returns>KeyId of the IHydratable.Key</returns>
        ''' <remarks><seealso cref="Fill"></seealso></remarks>
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