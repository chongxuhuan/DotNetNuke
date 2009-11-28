'
' DotNetNuke® - http://www.dotnetnuke.com
' Copyright (c) 2002-2009
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
Imports System.IO
Imports System.Xml.Serialization

Imports DotNetNuke.Services.Tokens
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Security.Permissions
Imports System.Xml

Namespace DotNetNuke.Entities.Tabs

    <XmlRoot("tab", IsNullable:=False)> <Serializable()> Public Class TabInfo
        Inherits BaseEntityInfo
        Implements IHydratable
        Implements IPropertyAccess

#Region "Private Members"

        Private _TabID As Integer
        Private _TabOrder As Integer
        Private _PortalID As Integer
        Private _TabName As String
        Private _IsVisible As Boolean
        Private _ParentId As Integer
        Private _Level As Integer
        Private _IconFile As String
        Private _IconFileLarge As String
        Private _DisableLink As Boolean
        Private _Title As String
        Private _Description As String
        Private _KeyWords As String
        Private _IsDeleted As Boolean
        Private _Url As String
        Private _SkinSrc As String
        Private _SkinDoctype As String
        Private _ContainerSrc As String
        Private _TabPath As String
        Private _StartDate As Date
        Private _EndDate As Date
        Private _HasChildren As Boolean
        Private _RefreshInterval As Integer
        Private _PageHeadText As String
        Private _IsSecure As Boolean
        Private _PermanentRedirect As Boolean
        Private _SiteMapPriority As Single = 0.5

        Private _SuperTabIdSet As Boolean = Null.NullBoolean

        Private _TabPermissions As Security.Permissions.TabPermissionCollection
        Private _TabSettings As Hashtable
        Private _AuthorizedRoles As String
        Private _AdministratorRoles As String

        ' properties loaded in PortalSettings
        Private _SkinPath As String
        Private _ContainerPath As String
        Private _BreadCrumbs As ArrayList
        Private _Panes As ArrayList
        Private _Modules As ArrayList
        Private _IsSuperTab As Boolean

#End Region

#Region "Constructors"

        Public Sub New()
            'initialize the properties that
            'can be null in the database
            _PortalID = Null.NullInteger
            _AuthorizedRoles = Null.NullString
            _ParentId = Null.NullInteger
            _IconFile = Null.NullString
            _IconFileLarge = Null.NullString
            _AdministratorRoles = Null.NullString
            _Title = Null.NullString
            _Description = Null.NullString
            _KeyWords = Null.NullString
            _Url = Null.NullString
            _SkinSrc = Null.NullString
            _SkinDoctype = Null.NullString
            _ContainerSrc = Null.NullString
            _TabPath = Null.NullString
            _StartDate = Null.NullDate
            _EndDate = Null.NullDate
            _RefreshInterval = Null.NullInteger
            _PageHeadText = Null.NullString
        End Sub

#End Region

#Region "Public Properties"

#Region "Tab Properties"

        <XmlElement("tabid")> Public Property TabID() As Integer
            Get
                Return _TabID
            End Get
            Set(ByVal Value As Integer)
                _TabID = Value
            End Set
        End Property

        <XmlElement("taborder")> Public Property TabOrder() As Integer
            Get
                Return _TabOrder
            End Get
            Set(ByVal Value As Integer)
                _TabOrder = Value
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

        <XmlElement("name")> Public Property TabName() As String
            Get
                Return _TabName
            End Get
            Set(ByVal Value As String)
                _TabName = Value
            End Set
        End Property

        <XmlElement("visible")> Public Property IsVisible() As Boolean
            Get
                Return _IsVisible
            End Get
            Set(ByVal Value As Boolean)
                _IsVisible = Value
            End Set
        End Property

        <XmlElement("parentid")> Public Property ParentId() As Integer
            Get
                Return _ParentId
            End Get
            Set(ByVal Value As Integer)
                _ParentId = Value
            End Set
        End Property

        <XmlIgnore()> Public Property Level() As Integer
            Get
                Return _Level
            End Get
            Set(ByVal Value As Integer)
                _Level = Value
            End Set
        End Property

        <XmlElement("iconfile")> Public Property IconFile() As String
            Get
                Return _IconFile
            End Get
            Set(ByVal Value As String)
                _IconFile = Value
            End Set
        End Property

        <XmlElement("iconfilelarge")> Public Property IconFileLarge() As String
            Get
                Return _IconFileLarge
            End Get
            Set(ByVal Value As String)
                _IconFileLarge = Value
            End Set
        End Property

        <XmlElement("disabled")> Public Property DisableLink() As Boolean
            Get
                Return _DisableLink
            End Get
            Set(ByVal Value As Boolean)
                _DisableLink = Value
            End Set
        End Property

        <XmlElement("title")> Public Property Title() As String
            Get
                Return _Title
            End Get
            Set(ByVal Value As String)
                _Title = Value
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

        <XmlElement("isdeleted")> Public Property IsDeleted() As Boolean
            Get
                Return _IsDeleted
            End Get
            Set(ByVal Value As Boolean)
                _IsDeleted = Value
            End Set
        End Property

        <XmlElement("url")> Public Property Url() As String
            Get
                Return _Url
            End Get
            Set(ByVal Value As String)
                _Url = Value
            End Set
        End Property

        <XmlElement("skinsrc")> Public Property SkinSrc() As String
            Get
                Return _SkinSrc
            End Get
            Set(ByVal Value As String)
                _SkinSrc = Value
            End Set
        End Property

        <XmlElement("skindoctype")> Public Property SkinDoctype() As String
            Get
                If String.IsNullOrEmpty(Me.SkinSrc) = False AndAlso String.IsNullOrEmpty(_SkinDoctype) Then
                    _SkinDoctype = GetDocTypeValue()
                End If
                Return _SkinDoctype
            End Get
            Set(ByVal Value As String)
                _SkinDoctype = Value
            End Set
        End Property

        <XmlElement("containersrc")> Public Property ContainerSrc() As String
            Get
                Return _ContainerSrc
            End Get
            Set(ByVal Value As String)
                _ContainerSrc = Value
            End Set
        End Property

        <XmlElement("tabpath")> Public Property TabPath() As String
            Get
                Return _TabPath
            End Get
            Set(ByVal Value As String)
                _TabPath = Value
            End Set
        End Property

        <XmlElement("startdate")> Public Property StartDate() As Date
            Get
                Return _StartDate
            End Get
            Set(ByVal Value As Date)
                _StartDate = Value
            End Set
        End Property

        <XmlElement("enddate")> Public Property EndDate() As Date
            Get
                Return _EndDate
            End Get
            Set(ByVal Value As Date)
                _EndDate = Value
            End Set
        End Property

        <XmlElement("haschildren")> Public Property HasChildren() As Boolean
            Get
                Return _HasChildren
            End Get
            Set(ByVal value As Boolean)
                _HasChildren = value
            End Set
        End Property

        <XmlElement("refreshinterval")> Public Property RefreshInterval() As Integer
            Get
                Return _RefreshInterval
            End Get
            Set(ByVal Value As Integer)
                _RefreshInterval = Value
            End Set
        End Property

        <XmlElement("pageheadtext")> Public Property PageHeadText() As String
            Get
                Return _PageHeadText
            End Get
            Set(ByVal Value As String)
                _PageHeadText = Value
            End Set
        End Property

        <XmlElement("issecure")> Public Property IsSecure() As Boolean
            Get
                Return _IsSecure
            End Get
            Set(ByVal Value As Boolean)
                _IsSecure = Value
            End Set
        End Property

        <XmlElement("permanentredirect")> Public Property PermanentRedirect() As Boolean
            Get
                Return _PermanentRedirect
            End Get
            Set(ByVal Value As Boolean)
                _PermanentRedirect = Value
            End Set
        End Property

        <XmlElement("sitemappriority")> Public Property SiteMapPriority() As Single
            Get
                Return _SiteMapPriority
            End Get
            Set(ByVal Value As Single)
                _SiteMapPriority = Value
            End Set
        End Property

#End Region

#Region "Tab Permission Properties"

        <XmlArray("tabpermissions"), XmlArrayItem("permission")> Public ReadOnly Property TabPermissions() As Security.Permissions.TabPermissionCollection
            Get
                If _TabPermissions Is Nothing Then
                    _TabPermissions = New TabPermissionCollection(TabPermissionController.GetTabPermissions(TabID, PortalID))
                End If
                Return _TabPermissions
            End Get
        End Property

#End Region

#Region "Tab Setting Properties"
        <XmlIgnore()> Public ReadOnly Property TabSettings() As Hashtable
            Get
                If _TabSettings Is Nothing Then
                    If TabID = Null.NullInteger Then
                        _TabSettings = New Hashtable
                    Else
                        Dim oTabCtrl As New TabController
                        _TabSettings = oTabCtrl.GetTabSettings(TabID)
                        oTabCtrl = Nothing
                    End If
                End If
                Return _TabSettings
            End Get
        End Property
#End Region


#Region "Other Properties"

        <XmlIgnore()> Public Property BreadCrumbs() As ArrayList
            Get
                Return _BreadCrumbs
            End Get
            Set(ByVal Value As ArrayList)
                _BreadCrumbs = Value
            End Set
        End Property

        <XmlIgnore()> Public Property ContainerPath() As String
            Get
                Return _ContainerPath
            End Get
            Set(ByVal Value As String)
                _ContainerPath = Value
            End Set
        End Property

        <XmlIgnore()> Public ReadOnly Property FullUrl() As String
            Get
                Dim strUrl As String = ""

                Select Case TabType
                    Case TabType.Normal
                        ' normal tab
                        strUrl = NavigateURL(TabID, IsSuperTab)
                    Case TabType.Tab
                        ' alternate tab url
                        strUrl = NavigateURL(Convert.ToInt32(_Url))
                    Case TabType.File
                        ' file url
                        strUrl = LinkClick(_Url, TabID, Null.NullInteger)
                    Case TabType.Url
                        ' external url
                        strUrl = _Url
                End Select

                Return strUrl
            End Get
        End Property

        <XmlIgnore()> Public Property IsSuperTab() As Boolean
            Get
                If _SuperTabIdSet Then
                    Return _IsSuperTab
                Else
                    Return (PortalID = Null.NullInteger)
                End If
            End Get
            Set(ByVal Value As Boolean)
                _IsSuperTab = Value
                _SuperTabIdSet = True
            End Set
        End Property

        <XmlIgnore()> Public ReadOnly Property IndentedTabName() As String
            Get
                Dim _IndentedTabName As String = Null.NullString
                For intCounter As Integer = 1 To Level
                    _IndentedTabName += "..."
                Next
                _IndentedTabName += LocalizedTabName
                Return _IndentedTabName ' + " (" + TabOrder.ToString() + ")"
            End Get
        End Property

        <XmlIgnore()> Public ReadOnly Property LocalizedTabName() As String
            Get
                Dim _LocalizedTabName As String = Localization.GetString(TabPath + ".String", Localization.GlobalResourceFile, True)
                If String.IsNullOrEmpty(_LocalizedTabName) Then
                    _LocalizedTabName = TabName
                End If
                Return _LocalizedTabName
            End Get
        End Property

        <XmlIgnore()> Public Property Modules() As ArrayList
            Get
                Return _Modules
            End Get
            Set(ByVal Value As ArrayList)
                _Modules = Value
            End Set
        End Property

        <XmlIgnore()> Public Property Panes() As ArrayList
            Get
                Return _Panes
            End Get
            Set(ByVal Value As ArrayList)
                _Panes = Value
            End Set
        End Property



        <XmlIgnore()> Public Property SkinPath() As String
            Get
                Return _SkinPath
            End Get
            Set(ByVal Value As String)
                _SkinPath = Value
            End Set
        End Property

        <XmlIgnore()> Public ReadOnly Property TabType() As TabType
            Get
                Return GetURLType(_Url)
            End Get
        End Property

#End Region

#End Region

#Region "Private Methods"

        'this function will return the host level doctype setting is set - this value can still be overriden by .doctype.xml definition
        Private Function GetDocTypeValue() As String
            Dim doctype As String = String.Empty
            doctype = CheckIfDoctypeConfigExists()
            If String.IsNullOrEmpty(doctype) = True Then
                doctype = Host.Host.DefaultDocType
            End If
            Return doctype
        End Function

        ''' <summary>
        ''' Look for skin level doctype configuration file, and inject the value into the top of default.aspx
        ''' when no configuration if found, the doctype for versions prior to 4.4 is used to maintain backwards compatibility with existing skins.
        ''' Adds xmlns and lang parameters when appropiate.
        ''' </summary>
        ''' <remarks></remarks>
        ''' <history>
        ''' 	[cathal]	28/05/2006	Created
        ''' </history>
        Private Function CheckIfDoctypeConfigExists() As String
            Dim doctypeConfig As String = String.Empty
            If Not String.IsNullOrEmpty(SkinSrc) Then
                Dim FileName As String = HttpContext.Current.Server.MapPath(SkinSrc.Replace(".ascx", ".doctype.xml"))
                'set doctype to legacy default
                '  Dim legacyDocTypeValue As String = "<!DOCTYPE HTML PUBLIC ""-//W3C//DTD HTML 4.0 Transitional//EN"">"
                Dim strLang As String = System.Globalization.CultureInfo.CurrentCulture.ToString()
                If File.Exists(FileName) Then
                    Try
                        Dim xmlSkinDocType As New System.Xml.XmlDocument
                        xmlSkinDocType.Load(FileName)
                        Dim strDocType As String = xmlSkinDocType.FirstChild.InnerText.ToString()
                        doctypeConfig = strDocType
                    Catch ex As Exception
                        'if exception is thrown, the xml is not formatted correctly, so use legacy default
                    End Try
                End If
            End If
            Return doctypeConfig
        End Function

#End Region

#Region "Public Methods"

        Public Function Clone() As TabInfo

            ' create the object
            Dim objTabInfo As New TabInfo

            ' assign the property values
            objTabInfo.TabID = Me.TabID
            objTabInfo.TabOrder = Me.TabOrder
            objTabInfo.PortalID = Me.PortalID
            objTabInfo.TabName = Me.TabName
            objTabInfo.IsVisible = Me.IsVisible
            objTabInfo.ParentId = Me.ParentId
            objTabInfo.Level = Me.Level
            objTabInfo.IconFile = Me.IconFile
            objTabInfo.IconFileLarge = Me.IconFileLarge
            objTabInfo.DisableLink = Me.DisableLink
            objTabInfo.Title = Me.Title
            objTabInfo.Description = Me.Description
            objTabInfo.KeyWords = Me.KeyWords
            objTabInfo.IsDeleted = Me.IsDeleted
            objTabInfo.Url = Me.Url
            objTabInfo.SkinSrc = Me.SkinSrc
            objTabInfo.ContainerSrc = Me.ContainerSrc
            objTabInfo.TabPath = Me.TabPath
            objTabInfo.StartDate = Me.StartDate
            objTabInfo.EndDate = Me.EndDate
            objTabInfo.HasChildren = Me.HasChildren
            objTabInfo.SkinPath = Me.SkinPath
            objTabInfo.ContainerPath = Me.ContainerPath
            objTabInfo.IsSuperTab = Me.IsSuperTab
            objTabInfo.RefreshInterval = Me.RefreshInterval
            objTabInfo.PageHeadText = Me.PageHeadText
            objTabInfo.IsSecure = Me.IsSecure
            objTabInfo.PermanentRedirect = Me.PermanentRedirect
            If Not Me.BreadCrumbs Is Nothing Then
                objTabInfo.BreadCrumbs = New ArrayList
                For Each t As TabInfo In Me.BreadCrumbs
                    objTabInfo.BreadCrumbs.Add(t.Clone())
                Next
            End If

            ' initialize collections which are populated later
            objTabInfo.Panes = New ArrayList
            objTabInfo.Modules = New ArrayList

            Return objTabInfo

        End Function

#End Region

#Region "IPropertyAccess Implementation"

        Public Function GetProperty(ByVal strPropertyName As String, ByVal strFormat As String, ByVal formatProvider As System.Globalization.CultureInfo, ByVal AccessingUser As UserInfo, ByVal CurrentScope As Scope, ByRef PropertyNotFound As Boolean) As String Implements IPropertyAccess.GetProperty
            Dim OutputFormat As String = String.Empty
            If strFormat = String.Empty Then OutputFormat = "g"
            Dim lowerPropertyName As String = strPropertyName.ToLower

            'Content locked for NoSettings
            If CurrentScope = Scope.NoSettings Then PropertyNotFound = True : Return PropertyAccess.ContentLocked

            PropertyNotFound = True
            Dim result As String = String.Empty
            Dim PublicProperty As Boolean = True

            Select Case lowerPropertyName
                Case "tabid"
                    PublicProperty = True : PropertyNotFound = False : result = (Me.TabID.ToString(OutputFormat, formatProvider))
                Case "taborder"
                    PublicProperty = False : PropertyNotFound = False : result = (Me.TabOrder.ToString(OutputFormat, formatProvider))
                Case "portalid"
                    PublicProperty = True : PropertyNotFound = False : result = (Me.PortalID.ToString(OutputFormat, formatProvider))
                Case "tabname"
                    PublicProperty = True : PropertyNotFound = False : result = PropertyAccess.FormatString(Me.LocalizedTabName, strFormat)
                Case "isvisible"
                    PublicProperty = False : PropertyNotFound = False : result = (PropertyAccess.Boolean2LocalizedYesNo(Me.IsVisible, formatProvider))
                Case "parentid"
                    PublicProperty = False : PropertyNotFound = False : result = (Me.ParentId.ToString(OutputFormat, formatProvider))
                Case "level"
                    PublicProperty = False : PropertyNotFound = False : result = (Me.Level.ToString(OutputFormat, formatProvider))
                Case "iconfile"
                    PublicProperty = True : PropertyNotFound = False : result = PropertyAccess.FormatString(Me.IconFile, strFormat)
                Case "iconfilelarge"
                    PublicProperty = True : PropertyNotFound = False : result = PropertyAccess.FormatString(Me.IconFileLarge, strFormat)
                Case "disablelink"
                    PublicProperty = False : PropertyNotFound = False : result = (PropertyAccess.Boolean2LocalizedYesNo(Me.DisableLink, formatProvider))
                Case "title"
                    PublicProperty = True : PropertyNotFound = False : result = PropertyAccess.FormatString(Me.Title, strFormat)
                Case "description"
                    PublicProperty = True : PropertyNotFound = False : result = PropertyAccess.FormatString(Me.Description, strFormat)
                Case "keywords"
                    PublicProperty = True : PropertyNotFound = False : result = PropertyAccess.FormatString(Me.KeyWords, strFormat)
                Case "isdeleted"
                    PublicProperty = False : PropertyNotFound = False : result = (PropertyAccess.Boolean2LocalizedYesNo(Me.IsDeleted, formatProvider))
                Case "url"
                    PublicProperty = True : PropertyNotFound = False : result = PropertyAccess.FormatString(Me.Url, strFormat)
                Case "skinsrc"
                    PublicProperty = False : PropertyNotFound = False : result = PropertyAccess.FormatString(Me.SkinSrc, strFormat)
                Case "containersrc"
                    PublicProperty = False : PropertyNotFound = False : result = PropertyAccess.FormatString(Me.ContainerSrc, strFormat)
                Case "tabpath"
                    PublicProperty = True : PropertyNotFound = False : result = PropertyAccess.FormatString(Me.TabPath, strFormat)
                Case "startdate"
                    PublicProperty = False : PropertyNotFound = False : result = (Me.StartDate.ToString(OutputFormat, formatProvider))
                Case "enddate"
                    PublicProperty = False : PropertyNotFound = False : result = (Me.EndDate.ToString(OutputFormat, formatProvider))
                Case "haschildren"
                    PublicProperty = False : PropertyNotFound = False : result = (PropertyAccess.Boolean2LocalizedYesNo(Me.HasChildren, formatProvider))
                Case "refreshinterval"
                    PublicProperty = False : PropertyNotFound = False : result = (Me.RefreshInterval.ToString(OutputFormat, formatProvider))
                Case "pageheadtext"
                    PublicProperty = False : PropertyNotFound = False : result = PropertyAccess.FormatString(Me.PageHeadText, strFormat)
                Case "skinpath"
                    PublicProperty = False : PropertyNotFound = False : result = PropertyAccess.FormatString(Me.SkinPath, strFormat)
                Case "skindoctype"
                    PublicProperty = False : PropertyNotFound = False : result = PropertyAccess.FormatString(Me.SkinDoctype, strFormat)
                Case "containerpath"
                    PublicProperty = False : PropertyNotFound = False : result = PropertyAccess.FormatString(Me.ContainerPath, strFormat)
                Case "issupertab"
                    PublicProperty = False : PropertyNotFound = False : result = (PropertyAccess.Boolean2LocalizedYesNo(Me.IsSuperTab, formatProvider))
                Case "fullurl"
                    PublicProperty = True : PropertyNotFound = False : result = PropertyAccess.FormatString(Me.FullUrl, strFormat)
                Case "sitemappriority"
                    PublicProperty = True : PropertyNotFound = False : result = PropertyAccess.FormatString(Me.SiteMapPriority.ToString(), strFormat)
            End Select

            If Not PublicProperty And CurrentScope <> Scope.Debug Then
                PropertyNotFound = True
                result = PropertyAccess.ContentLocked
            End If

            Return result
        End Function

        Public ReadOnly Property Cacheability() As CacheLevel Implements Services.Tokens.IPropertyAccess.Cacheability
            Get
                Return CacheLevel.fullyCacheable
            End Get
        End Property

#End Region

#Region "IHydratable Implementation"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Fills a TabInfo from a Data Reader
        ''' </summary>
        ''' <param name="dr">The Data Reader to use</param>
        ''' <history>
        ''' 	[cnurse]	01/15/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub Fill(ByVal dr As System.Data.IDataReader) Implements IHydratable.Fill
            TabID = Null.SetNullInteger(dr("TabID"))
            TabOrder = Null.SetNullInteger(dr("TabOrder"))
            PortalID = Null.SetNullInteger(dr("PortalID"))
            TabName = Null.SetNullString(dr("TabName"))
            IsVisible = Null.SetNullBoolean(dr("IsVisible"))
            ParentId = Null.SetNullInteger(dr("ParentId"))
            Level = Null.SetNullInteger(dr("Level"))
            IconFile = Null.SetNullString(dr("IconFile"))
            IconFileLarge = Null.SetNullString(dr("IconFileLarge"))
            DisableLink = Null.SetNullBoolean(dr("DisableLink"))
            Title = Null.SetNullString(dr("Title"))
            Description = Null.SetNullString(dr("Description"))
            KeyWords = Null.SetNullString(dr("KeyWords"))
            IsDeleted = Null.SetNullBoolean(dr("IsDeleted"))
            Url = Null.SetNullString(dr("Url"))
            SkinSrc = Null.SetNullString(dr("SkinSrc"))
            ContainerSrc = Null.SetNullString(dr("ContainerSrc"))
            TabPath = Null.SetNullString(dr("TabPath"))
            StartDate = Null.SetNullDateTime(dr("StartDate"))
            EndDate = Null.SetNullDateTime(dr("EndDate"))
            HasChildren = Null.SetNullBoolean(dr("HasChildren"))
            RefreshInterval = Null.SetNullInteger(dr("RefreshInterval"))
            PageHeadText = Null.SetNullString(dr("PageHeadText"))
            IsSecure = Null.SetNullBoolean(dr("IsSecure"))
            PermanentRedirect = Null.SetNullBoolean(dr("PermanentRedirect"))
            SiteMapPriority = Null.SetNullSingle(dr("SiteMapPriority"))

            BreadCrumbs = Nothing
            Panes = Nothing
            Modules = Nothing
            'Call the base classes fill method to populate base class proeprties
            MyBase.FillInternal(dr)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the Key ID
        ''' </summary>
        ''' <returns>An Integer</returns>
        ''' <history>
        ''' 	[cnurse]	01/15/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <XmlIgnore()> Public Property KeyID() As Integer Implements IHydratable.KeyID
            Get
                Return TabID
            End Get
            Set(ByVal value As Integer)
                TabID = value
            End Set
        End Property

#End Region

#Region "Obsolete Methods"

        <Obsolete("Deprecated in DNN 5.0. The artificial differences between Regular and Admin pages was removed.")> _
        Public ReadOnly Property IsAdminTab() As Boolean
            Get
                If IsSuperTab Then
                    'Host Tab
                    Return True
                Else
                    'Portal Tab
                    Return False
                End If
            End Get
        End Property

        <Obsolete("Deprecated in DNN 5.1. All permission checks are done through Permission Collections")> _
        <XmlIgnore()> Public Property AdministratorRoles() As String
            Get
                If String.IsNullOrEmpty(_AdministratorRoles) Then
                    _AdministratorRoles = TabPermissions.ToString("EDIT")
                End If
                Return _AdministratorRoles
            End Get
            Set(ByVal value As String)
                _AdministratorRoles = value
            End Set
        End Property

        <Obsolete("Deprecated in DNN 5.1. All permission checks are done through Permission Collections")> _
        <XmlIgnore()> Public Property AuthorizedRoles() As String
            Get
                If String.IsNullOrEmpty(_AuthorizedRoles) Then
                    _AuthorizedRoles = TabPermissions.ToString("VIEW")
                End If
                Return _AuthorizedRoles
            End Get
            Set(ByVal value As String)
                _AuthorizedRoles = value
            End Set
        End Property


#End Region

       
    End Class

End Namespace

