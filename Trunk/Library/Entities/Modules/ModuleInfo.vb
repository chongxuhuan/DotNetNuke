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
Imports System.Xml.Serialization
Imports DotNetNuke.Services.Tokens
Imports DotNetNuke.Entities.Modules.Definitions
Imports DotNetNuke.Security.Permissions
Imports System.Collections.Generic
Imports DotNetNuke.Entities.Content

Namespace DotNetNuke.Entities.Modules

    ''' -----------------------------------------------------------------------------
    ''' Project	 : DotNetNuke
    ''' Namespace: DotNetNuke.Entities.Modules
    ''' Class	 : ModuleInfo
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' ModuleInfo provides the Entity Layer for Modules
    ''' </summary>
    ''' <history>
    ''' 	[cnurse]	01/14/2008   Documented
    ''' </history>
    ''' -----------------------------------------------------------------------------
    <XmlRoot("module", IsNullable:=False)> <Serializable()> Public Class ModuleInfo
        Inherits ContentItem
        Implements IHydratable
        Implements IPropertyAccess

#Region "Private Members"

        'Module Properties
        Private _PortalID As Integer
        Private _ModuleTitle As String
        Private _AllTabs As Boolean
        Private _IsDeleted As Boolean
        Private _InheritViewPermissions As Boolean
        Private _Header As String
        Private _Footer As String
        Private _StartDate As Date
        Private _EndDate As Date

        'TabModule Properties
        Private _TabModuleID As Integer
        Private _PaneName As String
        Private _ModuleOrder As Integer
        Private _CacheTime As Integer
        Private _CacheMethod As String
        Private _Alignment As String
        Private _Color As String
        Private _Border As String
        Private _IconFile As String
        Private _Visibility As VisibilityState
        Private _ContainerSrc As String
        Private _DisplayTitle As Boolean
        Private _DisplayPrint As Boolean
        Private _DisplaySyndicate As Boolean
        Private _IsWebSlice As Boolean
        Private _WebSliceTitle As String
        Private _WebSliceExpiryDate As DateTime
        Private _WebSliceTTL As Integer

        'DesktopModule Properties
        Private _DesktopModuleID As Integer
        Private _DesktopModule As DesktopModuleInfo

        'ModuleDefinition Properties
        Private _ModuleDefID As Integer
        Private _ModuleDefinition As ModuleDefinitionInfo

        'ModuleControl Properties
        Private _ModuleControlId As Integer
        Private _ModuleControl As ModuleControlInfo

        'Module Permissions
        Private _AuthorizedEditRoles As String
        Private _AuthorizedViewRoles As String
        Private _AuthorizedRoles As String
        Private _ModulePermissions As ModulePermissionCollection
        Private _ModuleSettings As Hashtable
        Private _TabModuleSettings As Hashtable
        Private _TabPermissions As TabPermissionCollection

        Private _ContainerPath As String
        Private _PaneModuleIndex As Integer
        Private _PaneModuleCount As Integer
        Private _IsDefaultModule As Boolean
        Private _AllModules As Boolean


#End Region

#Region "Constructors"

        Public Sub New()
            'initialize the properties that can be null
            'in the database
            _PortalID = Null.NullInteger
            _TabModuleID = Null.NullInteger
            _DesktopModuleID = Null.NullInteger
            _ModuleDefID = Null.NullInteger
            _ModuleTitle = Null.NullString
            _AuthorizedEditRoles = Null.NullString
            _AuthorizedViewRoles = Null.NullString
            _Alignment = Null.NullString
            _Color = Null.NullString
            _Border = Null.NullString
            _IconFile = Null.NullString
            _Header = Null.NullString
            _Footer = Null.NullString
            _StartDate = Null.NullDate
            _EndDate = Null.NullDate
            _ContainerSrc = Null.NullString
            _DisplayTitle = True
            _DisplayPrint = True
            _DisplaySyndicate = False
        End Sub

#End Region

#Region "Public Properties"

#Region "Module Properties"

        <XmlElement("portalid")> Public Property PortalID() As Integer
            Get
                Return _PortalID
            End Get
            Set(ByVal Value As Integer)
                _PortalID = Value
            End Set
        End Property

        <XmlElement("title")> Public Property ModuleTitle() As String
            Get
                Return _ModuleTitle
            End Get
            Set(ByVal Value As String)
                _ModuleTitle = Value
            End Set
        End Property

        <XmlElement("alltabs")> Public Property AllTabs() As Boolean
            Get
                Return _AllTabs
            End Get
            Set(ByVal Value As Boolean)
                _AllTabs = Value
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

        <XmlElement("inheritviewpermissions")> Public Property InheritViewPermissions() As Boolean
            Get
                Return _InheritViewPermissions
            End Get
            Set(ByVal Value As Boolean)
                _InheritViewPermissions = Value
            End Set
        End Property

        <XmlElement("header")> Public Property Header() As String
            Get
                Return _Header
            End Get
            Set(ByVal Value As String)
                _Header = Value
            End Set
        End Property

        <XmlElement("footer")> Public Property Footer() As String
            Get
                Return _Footer
            End Get
            Set(ByVal Value As String)
                _Footer = Value
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

#End Region

#Region "Tab Module Properties"

        <XmlElement("tabmoduleid")> Public Property TabModuleID() As Integer
            Get
                Return _TabModuleID
            End Get
            Set(ByVal Value As Integer)
                _TabModuleID = Value
            End Set
        End Property

        <XmlElement("panename")> Public Property PaneName() As String
            Get
                Return _PaneName
            End Get
            Set(ByVal Value As String)
                _PaneName = Value
            End Set
        End Property

        <XmlElement("moduleorder")> Public Property ModuleOrder() As Integer
            Get
                Return _ModuleOrder
            End Get
            Set(ByVal Value As Integer)
                _ModuleOrder = Value
            End Set
        End Property

        <XmlElement("cachetime")> Public Property CacheTime() As Integer
            Get
                Return _CacheTime
            End Get
            Set(ByVal Value As Integer)
                _CacheTime = Value
            End Set
        End Property

        <XmlElement("cachemethod")> Public Property CacheMethod() As String
            Get
                Return _CacheMethod
            End Get
            Set(ByVal Value As String)
                _CacheMethod = Value
            End Set
        End Property

        <XmlElement("alignment")> Public Property Alignment() As String
            Get
                Return _Alignment
            End Get
            Set(ByVal Value As String)
                _Alignment = Value
            End Set
        End Property

        <XmlElement("color")> Public Property Color() As String
            Get
                Return _Color
            End Get
            Set(ByVal Value As String)
                _Color = Value
            End Set
        End Property

        <XmlElement("border")> Public Property Border() As String
            Get
                Return _Border
            End Get
            Set(ByVal Value As String)
                _Border = Value
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

        <XmlElement("visibility")> Public Property Visibility() As VisibilityState
            Get
                Return _Visibility
            End Get
            Set(ByVal Value As VisibilityState)
                _Visibility = Value
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

        <XmlElement("displaytitle")> Public Property DisplayTitle() As Boolean
            Get
                Return _DisplayTitle
            End Get
            Set(ByVal Value As Boolean)
                _DisplayTitle = Value
            End Set
        End Property

        <XmlElement("displayprint")> Public Property DisplayPrint() As Boolean
            Get
                Return _DisplayPrint
            End Get
            Set(ByVal Value As Boolean)
                _DisplayPrint = Value
            End Set
        End Property

        <XmlElement("displaysyndicate")> Public Property DisplaySyndicate() As Boolean
            Get
                Return _DisplaySyndicate
            End Get
            Set(ByVal Value As Boolean)
                _DisplaySyndicate = Value
            End Set
        End Property

        <XmlElement("iswebslice")> Public Property IsWebSlice() As Boolean
            Get
                Return _IsWebSlice
            End Get
            Set(ByVal Value As Boolean)
                _IsWebSlice = Value
            End Set
        End Property

        <XmlElement("webslicetitle")> Public Property WebSliceTitle() As String
            Get
                Return _WebSliceTitle
            End Get
            Set(ByVal Value As String)
                _WebSliceTitle = Value
            End Set
        End Property

        <XmlElement("websliceexpirydate")> Public Property WebSliceExpiryDate() As DateTime
            Get
                Return _WebSliceExpiryDate
            End Get
            Set(ByVal Value As DateTime)
                _WebSliceExpiryDate = Value
            End Set
        End Property

        <XmlElement("webslicettl")> Public Property WebSliceTTL() As Integer
            Get
                Return _WebSliceTTL
            End Get
            Set(ByVal Value As Integer)
                _WebSliceTTL = Value
            End Set
        End Property

        <XmlIgnore()> Public ReadOnly Property HideAdminBorder() As Boolean
            Get
                Dim setting As Object = TabModuleSettings("hideadminborder")
                If setting Is Nothing OrElse _
                    String.IsNullOrEmpty(setting.ToString) Then
                    Return False
                End If

                Dim val As Boolean
                Boolean.TryParse(setting.ToString, val)
                Return val
            End Get
        End Property

#End Region

#Region "Desktop Module Properties"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the ID of the Associated Desktop Module
        ''' </summary>
        ''' <returns>An Integer</returns>
        ''' <history>
        ''' 	[cnurse]	01/14/2008   Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <XmlIgnore()> Public Property DesktopModuleID() As Integer
            Get
                Return _DesktopModuleID
            End Get
            Set(ByVal Value As Integer)
                _DesktopModuleID = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the Associated Desktop Module
        ''' </summary>
        ''' <returns>An Integer</returns>
        ''' <history>
        ''' 	[cnurse]	01/14/2008   Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <XmlIgnore()> Public ReadOnly Property DesktopModule() As DesktopModuleInfo
            Get
                If _DesktopModule Is Nothing Then
                    If DesktopModuleID > Null.NullInteger Then
                        _DesktopModule = DesktopModuleController.GetDesktopModule(DesktopModuleID, PortalID)
                    Else
                        _DesktopModule = New DesktopModuleInfo()
                    End If
                End If
                Return _DesktopModule
            End Get
        End Property

#End Region

#Region "Module Definition Properties"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the ID of the Associated Module Definition
        ''' </summary>
        ''' <returns>An Integer</returns>
        ''' <history>
        ''' 	[cnurse]	01/14/2008   Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <XmlIgnore()> Public Property ModuleDefID() As Integer
            Get
                Return _ModuleDefID
            End Get
            Set(ByVal Value As Integer)
                _ModuleDefID = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the Associated Module Definition
        ''' </summary>
        ''' <returns>A ModuleDefinitionInfo</returns>
        ''' <history>
        ''' 	[cnurse]	01/14/2008   Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <XmlIgnore()> Public ReadOnly Property ModuleDefinition() As ModuleDefinitionInfo
            Get
                If _ModuleDefinition Is Nothing Then
                    If ModuleDefID > Null.NullInteger Then
                        _ModuleDefinition = ModuleDefinitionController.GetModuleDefinitionByID(ModuleDefID)
                    Else
                        _ModuleDefinition = New ModuleDefinitionInfo()
                    End If
                End If
                Return _ModuleDefinition
            End Get
        End Property

#End Region

#Region "Module Control Properties"

        <XmlIgnore()> Public Property ModuleControlId() As Integer
            Get
                Return _ModuleControlId
            End Get
            Set(ByVal Value As Integer)
                _ModuleControlId = Value
            End Set
        End Property

        Public ReadOnly Property ModuleControl() As ModuleControlInfo
            Get
                If _ModuleControl Is Nothing Then
                    If ModuleControlId > Null.NullInteger Then
                        _ModuleControl = ModuleControlController.GetModuleControl(ModuleControlId)
                    Else
                        _ModuleControl = New ModuleControlInfo()
                    End If
                End If
                Return _ModuleControl
            End Get
        End Property

#End Region

#Region "Module Permission Properties"

        <XmlArray("modulepermissions"), XmlArrayItem("permission")> Public Property ModulePermissions() As ModulePermissionCollection
            Get
                If _ModulePermissions Is Nothing Then
                    _ModulePermissions = New ModulePermissionCollection(ModulePermissionController.GetModulePermissions(ModuleID, TabID))
                End If
                Return _ModulePermissions
            End Get
            <Obsolete("The ModulePermissions Setter has been deprecated in 5.0 - a .NET Best practice is that Collection properties should be read-only")> _
            Set(ByVal value As ModulePermissionCollection)
                _ModulePermissions = value
            End Set
        End Property

#End Region

#Region "Module Setting Properties"

        <XmlIgnore()> Public ReadOnly Property ModuleSettings() As Hashtable
            Get
                If _ModuleSettings Is Nothing Then
                    If ModuleID = Null.NullInteger Then
                        _ModuleSettings = New Hashtable
                    Else
                        Dim oModuleCtrl As New ModuleController()
                        _ModuleSettings = oModuleCtrl.GetModuleSettings(ModuleID)
                        oModuleCtrl = Nothing
                    End If
                End If
                Return _ModuleSettings
            End Get
        End Property

#End Region

#Region "TabModule Setting Properties"
        <XmlIgnore()> Public ReadOnly Property TabModuleSettings() As Hashtable
            Get
                If _TabModuleSettings Is Nothing Then
                    If TabModuleID = Null.NullInteger Then
                        _TabModuleSettings = New Hashtable
                    Else
                        Dim oModuleCtrl As New ModuleController
                        _TabModuleSettings = oModuleCtrl.GetTabModuleSettings(TabModuleID)
                        oModuleCtrl = Nothing
                    End If
                End If
                Return _TabModuleSettings
            End Get
        End Property
#End Region

#Region "Other Properties"

        <XmlIgnore()> Public Property ContainerPath() As String
            Get
                Return _ContainerPath
            End Get
            Set(ByVal Value As String)
                _ContainerPath = Value
            End Set
        End Property

        <XmlIgnore()> Public Property PaneModuleIndex() As Integer
            Get
                Return _PaneModuleIndex
            End Get
            Set(ByVal Value As Integer)
                _PaneModuleIndex = Value
            End Set
        End Property

        <XmlIgnore()> Public Property PaneModuleCount() As Integer
            Get
                Return _PaneModuleCount
            End Get
            Set(ByVal Value As Integer)
                _PaneModuleCount = Value
            End Set
        End Property

        <XmlIgnore()> Public Property IsDefaultModule() As Boolean
            Get
                Return _IsDefaultModule
            End Get
            Set(ByVal Value As Boolean)
                _IsDefaultModule = Value
            End Set
        End Property

        <XmlIgnore()> Public Property AllModules() As Boolean
            Get
                Return _AllModules
            End Get
            Set(ByVal Value As Boolean)
                _AllModules = Value
            End Set
        End Property

#End Region

#End Region

#Region "Public Methods"

        Public Function Clone() As ModuleInfo

            ' create the object
            Dim objModuleInfo As New ModuleInfo

            ' assign the property values
            objModuleInfo.PortalID = Me.PortalID
            objModuleInfo.TabID = Me.TabID
            objModuleInfo.TabModuleID = Me.TabModuleID
            objModuleInfo.ModuleID = Me.ModuleID
            objModuleInfo.ModuleOrder = Me.ModuleOrder
            objModuleInfo.PaneName = Me.PaneName
            objModuleInfo.ModuleTitle = Me.ModuleTitle
            objModuleInfo.CacheTime = Me.CacheTime
            objModuleInfo.CacheMethod = Me.CacheMethod
            objModuleInfo.Alignment = Me.Alignment
            objModuleInfo.Color = Me.Color
            objModuleInfo.Border = Me.Border
            objModuleInfo.IconFile = Me.IconFile
            objModuleInfo.AllTabs = Me.AllTabs
            objModuleInfo.Visibility = Me.Visibility
            objModuleInfo.IsDeleted = Me.IsDeleted
            objModuleInfo.Header = Me.Header
            objModuleInfo.Footer = Me.Footer
            objModuleInfo.StartDate = Me.StartDate
            objModuleInfo.EndDate = Me.EndDate
            objModuleInfo.ContainerSrc = Me.ContainerSrc
            objModuleInfo.DisplayTitle = Me.DisplayTitle
            objModuleInfo.DisplayPrint = Me.DisplayPrint
            objModuleInfo.DisplaySyndicate = Me.DisplaySyndicate
            objModuleInfo.IsWebSlice = Me.IsWebSlice
            objModuleInfo.WebSliceTitle = Me.WebSliceTitle
            objModuleInfo.WebSliceExpiryDate = Me.WebSliceExpiryDate
            objModuleInfo.WebSliceTTL = Me.WebSliceTTL
            objModuleInfo.InheritViewPermissions = Me.InheritViewPermissions

            objModuleInfo.DesktopModuleID = Me.DesktopModuleID

            objModuleInfo.ModuleDefID = Me.ModuleDefID

            objModuleInfo.ModuleControlId = Me.ModuleControlId

            objModuleInfo.ContainerPath = Me.ContainerPath
            objModuleInfo.PaneModuleIndex = Me.PaneModuleIndex
            objModuleInfo.PaneModuleCount = Me.PaneModuleCount
            objModuleInfo.IsDefaultModule = Me.IsDefaultModule
            objModuleInfo.AllModules = Me.AllModules

            objModuleInfo._ModulePermissions = Me._ModulePermissions
            objModuleInfo._TabPermissions = Me._TabPermissions

            objModuleInfo.ContentItemId = Me.ContentItemId

            Return objModuleInfo

        End Function

        Public Function GetEffectiveCacheMethod() As String

            Dim effectiveCacheMethod As String
            If Not String.IsNullOrEmpty(_CacheMethod) Then
                effectiveCacheMethod = _CacheMethod
            ElseIf Not String.IsNullOrEmpty(Entities.Host.Host.ModuleCachingMethod) Then
                'fallback to the host settings
                effectiveCacheMethod = Entities.Host.Host.ModuleCachingMethod
            Else
                'fallback to default value in web.config
                Dim defaultModuleCache As ModuleCache.ModuleCachingProvider = DotNetNuke.ComponentModel.ComponentFactory.GetComponent(Of ModuleCache.ModuleCachingProvider)()
                effectiveCacheMethod = (From provider In ModuleCache.ModuleCachingProvider.GetProviderList() _
                                        Where provider.Value.Equals(defaultModuleCache) _
                                        Select provider.Key).SingleOrDefault
            End If

            If String.IsNullOrEmpty(effectiveCacheMethod) Then
                Throw New InvalidOperationException(DotNetNuke.Services.Localization.Localization.GetString("EXCEPTION_ModuleCacheMissing"))
            End If
            Return effectiveCacheMethod

        End Function

        Public Sub Initialize(ByVal PortalId As Integer)
            _PortalID = PortalId
            '_TabID = -1
            '_ModuleID = -1
            _ModuleDefID = Null.NullInteger
            _ModuleOrder = Null.NullInteger
            _PaneName = Null.NullString
            _ModuleTitle = Null.NullString
            _CacheTime = 0
            _CacheMethod = Null.NullString
            _Alignment = Null.NullString
            _Color = Null.NullString
            _Border = Null.NullString
            _IconFile = Null.NullString
            _AllTabs = Null.NullBoolean
            _Visibility = VisibilityState.Maximized
            _IsDeleted = Null.NullBoolean
            _Header = Null.NullString
            _Footer = Null.NullString
            _StartDate = Null.NullDate
            _EndDate = Null.NullDate
            _DisplayTitle = True
            _DisplayPrint = True
            _DisplaySyndicate = Null.NullBoolean
            _IsWebSlice = Null.NullBoolean
            _WebSliceTitle = ""
            _WebSliceExpiryDate = Null.NullDate
            _WebSliceTTL = 0
            _InheritViewPermissions = Null.NullBoolean
            _ContainerSrc = Null.NullString
            _DesktopModuleID = Null.NullInteger
            _ModuleControlId = Null.NullInteger
            _ContainerPath = Null.NullString
            _PaneModuleIndex = 0
            _PaneModuleCount = 0
            _IsDefaultModule = Null.NullBoolean
            _AllModules = Null.NullBoolean

            ' get default module settings
            If PortalSettings.Current.DefaultModuleId > Null.NullInteger AndAlso PortalSettings.Current.DefaultTabId > Null.NullInteger Then
                Dim objModules As New ModuleController
                Dim objModule As ModuleInfo = objModules.GetModule(PortalSettings.Current.DefaultModuleId, PortalSettings.Current.DefaultTabId, True)
                If Not objModule Is Nothing Then
                    _Alignment = objModule.Alignment
                    _Color = objModule.Color
                    _Border = objModule.Border
                    _IconFile = objModule.IconFile
                    _Visibility = objModule.Visibility
                    _ContainerSrc = objModule.ContainerSrc
                    _DisplayTitle = objModule.DisplayTitle
                    _DisplayPrint = objModule.DisplayPrint
                    _DisplaySyndicate = objModule.DisplaySyndicate
                End If
            End If
        End Sub

#End Region

#Region "IHydratable Implementation"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Fills a ModuleInfo from a Data Reader
        ''' </summary>
        ''' <param name="dr">The Data Reader to use</param>
        ''' <history>
        ''' 	[cnurse]	01/14/2008   Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overrides Sub Fill(ByVal dr As System.Data.IDataReader)
            'Call the base classes fill method to populate base class properties
            MyBase.FillInternal(dr)

            PortalID = Null.SetNullInteger(dr("PortalID"))
            ModuleDefID = Null.SetNullInteger(dr("ModuleDefID"))
            ModuleTitle = Null.SetNullString(dr("ModuleTitle"))
            AllTabs = Null.SetNullBoolean(dr("AllTabs"))
            IsDeleted = Null.SetNullBoolean(dr("IsDeleted"))
            InheritViewPermissions = Null.SetNullBoolean(dr("InheritViewPermissions"))
            Header = Null.SetNullString(dr("Header"))
            Footer = Null.SetNullString(dr("Footer"))
            StartDate = Null.SetNullDateTime(dr("StartDate"))
            EndDate = Null.SetNullDateTime(dr("EndDate"))

            Try
                TabModuleID = Null.SetNullInteger(dr("TabModuleID"))
                ModuleOrder = Null.SetNullInteger(dr("ModuleOrder"))
                PaneName = Null.SetNullString(dr("PaneName"))
                CacheTime = Null.SetNullInteger(dr("CacheTime"))
                CacheMethod = Null.SetNullString(dr("CacheMethod"))
                Alignment = Null.SetNullString(dr("Alignment"))
                Color = Null.SetNullString(dr("Color"))
                Border = Null.SetNullString(dr("Border"))
                IconFile = Null.SetNullString(dr("IconFile"))
                Select Case Null.SetNullInteger(dr("Visibility"))
                    Case 0, Null.NullInteger : Visibility = VisibilityState.Maximized
                    Case 1 : Visibility = VisibilityState.Minimized
                    Case 2 : Visibility = VisibilityState.None
                End Select
                ContainerSrc = Null.SetNullString(dr("ContainerSrc"))
                DisplayTitle = Null.SetNullBoolean(dr("DisplayTitle"))
                DisplayPrint = Null.SetNullBoolean(dr("DisplayPrint"))
                DisplaySyndicate = Null.SetNullBoolean(dr("DisplaySyndicate"))
                IsWebSlice = Null.SetNullBoolean(dr("IsWebSlice"))
                If IsWebSlice Then
                    WebSliceTitle = Null.SetNullString(dr("WebSliceTitle"))
                    WebSliceExpiryDate = Null.SetNullDateTime(dr("WebSliceExpiryDate"))
                    WebSliceTTL = Null.SetNullInteger(dr("WebSliceTTL"))
                End If

                DesktopModuleID = Null.SetNullInteger(dr("DesktopModuleID"))

                ModuleControlId = Null.SetNullInteger(dr("ModuleControlID"))
            Catch ex As Exception

            End Try
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the Key ID
        ''' </summary>
        ''' <returns>An Integer</returns>
        ''' <history>
        ''' 	[cnurse]	01/14/2008   Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <XmlIgnore()> Public Overrides Property KeyID() As Integer
            Get
                Return ModuleID
            End Get
            Set(ByVal value As Integer)
                ModuleID = value
            End Set
        End Property

#End Region

#Region "IPropertyAccess Implementation"

        Public Function GetProperty(ByVal strPropertyName As String, ByVal strFormat As String, ByVal formatProvider As System.Globalization.CultureInfo, ByVal AccessingUser As Users.UserInfo, ByVal CurrentScope As Scope, ByRef PropertyNotFound As Boolean) As String Implements Services.Tokens.IPropertyAccess.GetProperty

            Dim OutputFormat As String = String.Empty
            If strFormat = String.Empty Then OutputFormat = "g"

            'Content locked for NoSettings
            If CurrentScope = Scope.NoSettings Then PropertyNotFound = True : Return PropertyAccess.ContentLocked

            PropertyNotFound = True
            Dim result As String = String.Empty
            Dim PublicProperty As Boolean = True

            Select Case strPropertyName.ToLower
                Case "portalid"
                    PublicProperty = True : PropertyNotFound = False : result = (Me.PortalID.ToString(OutputFormat, formatProvider))
                Case "tabid"
                    PublicProperty = True : PropertyNotFound = False : result = (Me.TabID.ToString(OutputFormat, formatProvider))
                Case "tabmoduleid"
                    PublicProperty = True : PropertyNotFound = False : result = (Me.TabModuleID.ToString(OutputFormat, formatProvider))
                Case "moduleid"
                    PublicProperty = True : PropertyNotFound = False : result = (Me.ModuleID.ToString(OutputFormat, formatProvider))
                Case "moduledefid"
                    PublicProperty = False : PropertyNotFound = False : result = (Me.ModuleDefID.ToString(OutputFormat, formatProvider))
                Case "moduleorder"
                    PublicProperty = False : PropertyNotFound = False : result = (Me.ModuleOrder.ToString(OutputFormat, formatProvider))
                Case "panename"
                    PublicProperty = True : PropertyNotFound = False : result = PropertyAccess.FormatString(Me.PaneName, strFormat)
                Case "moduletitle"
                    PublicProperty = True : PropertyNotFound = False : result = PropertyAccess.FormatString(Me.ModuleTitle, strFormat)
                Case "cachetime"
                    PublicProperty = False : PropertyNotFound = False : result = (Me.CacheTime.ToString(OutputFormat, formatProvider))
                Case "cachemethod"
                    PublicProperty = False : PropertyNotFound = False : result = PropertyAccess.FormatString(Me.CacheMethod, strFormat)
                Case "alignment"
                    PublicProperty = True : PropertyNotFound = False : result = PropertyAccess.FormatString(Me.Alignment, strFormat)
                Case "color"
                    PublicProperty = True : PropertyNotFound = False : result = PropertyAccess.FormatString(Me.Color, strFormat)
                Case "border"
                    PublicProperty = True : PropertyNotFound = False : result = PropertyAccess.FormatString(Me.Border, strFormat)
                Case "iconfile"
                    PublicProperty = True : PropertyNotFound = False : result = PropertyAccess.FormatString(Me.IconFile, strFormat)
                Case "alltabs"
                    PublicProperty = False : PropertyNotFound = False : result = (PropertyAccess.Boolean2LocalizedYesNo(Me.AllTabs, formatProvider))
                Case "isdeleted"
                    PublicProperty = False : PropertyNotFound = False : result = (PropertyAccess.Boolean2LocalizedYesNo(Me.IsDeleted, formatProvider))
                Case "header"
                    PublicProperty = True : PropertyNotFound = False : result = PropertyAccess.FormatString(Me.Header, strFormat)
                Case "footer"
                    PublicProperty = True : PropertyNotFound = False : result = PropertyAccess.FormatString(Me.Footer, strFormat)
                Case "startdate"
                    PublicProperty = False : PropertyNotFound = False : result = (Me.StartDate.ToString(OutputFormat, formatProvider))
                Case "enddate"
                    PublicProperty = False : PropertyNotFound = False : result = (Me.EndDate.ToString(OutputFormat, formatProvider))
                Case "containersrc"
                    PublicProperty = False : PropertyNotFound = False : result = PropertyAccess.FormatString(Me.ContainerSrc, strFormat)
                Case "displaytitle"
                    PublicProperty = False : PropertyNotFound = False : result = (PropertyAccess.Boolean2LocalizedYesNo(Me.DisplayTitle, formatProvider))
                Case "displayprint"
                    PublicProperty = False : PropertyNotFound = False : result = (PropertyAccess.Boolean2LocalizedYesNo(Me.DisplayPrint, formatProvider))
                Case "displaysyndicate"
                    PublicProperty = False : PropertyNotFound = False : result = (PropertyAccess.Boolean2LocalizedYesNo(Me.DisplaySyndicate, formatProvider))
                Case "iswebslice"
                    PublicProperty = False : PropertyNotFound = False : result = (PropertyAccess.Boolean2LocalizedYesNo(Me.IsWebSlice, formatProvider))
                Case "webslicetitle"
                    PublicProperty = False : PropertyNotFound = False : result = PropertyAccess.FormatString(Me.WebSliceTitle, strFormat)
                Case "websliceexpirydate"
                    PublicProperty = False : PropertyNotFound = False : result = (Me.WebSliceExpiryDate.ToString(OutputFormat, formatProvider))
                Case "webslicettl"
                    PublicProperty = False : PropertyNotFound = False : result = (Me.WebSliceTTL.ToString(OutputFormat, formatProvider))
                Case "inheritviewpermissions"
                    PublicProperty = False : PropertyNotFound = False : result = (PropertyAccess.Boolean2LocalizedYesNo(Me.InheritViewPermissions, formatProvider))
                Case "desktopmoduleid"
                    PublicProperty = False : PropertyNotFound = False : result = (Me.DesktopModuleID.ToString(OutputFormat, formatProvider))
                Case "friendlyname"
                    PublicProperty = True : PropertyNotFound = False : result = PropertyAccess.FormatString(Me.DesktopModule.FriendlyName, strFormat)
                Case "foldername"
                    PublicProperty = True : PropertyNotFound = False : result = PropertyAccess.FormatString(Me.DesktopModule.FolderName, strFormat)
                Case "description"
                    PublicProperty = True : PropertyNotFound = False : result = PropertyAccess.FormatString(Me.DesktopModule.Description, strFormat)
                Case "version"
                    PublicProperty = True : PropertyNotFound = False : result = PropertyAccess.FormatString(Me.DesktopModule.Version, strFormat)
                Case "ispremium"
                    PublicProperty = False : PropertyNotFound = False : result = (PropertyAccess.Boolean2LocalizedYesNo(Me.DesktopModule.IsPremium, formatProvider))
                Case "isadmin"
                    PublicProperty = False : PropertyNotFound = False : result = (PropertyAccess.Boolean2LocalizedYesNo(Me.DesktopModule.IsAdmin, formatProvider))
                Case "businesscontrollerclass"
                    PublicProperty = True : PropertyNotFound = False : result = PropertyAccess.FormatString(Me.DesktopModule.BusinessControllerClass, strFormat)
                Case "modulename"
                    PublicProperty = True : PropertyNotFound = False : result = PropertyAccess.FormatString(Me.DesktopModule.ModuleName, strFormat)
                Case "supportedfeatures"
                    PublicProperty = False : PropertyNotFound = False : result = (Me.DesktopModule.SupportedFeatures.ToString(OutputFormat, formatProvider))
                Case "compatibleversions"
                    PublicProperty = False : PropertyNotFound = False : result = PropertyAccess.FormatString(Me.DesktopModule.CompatibleVersions, strFormat)
                Case "dependencies"
                    PublicProperty = False : PropertyNotFound = False : result = PropertyAccess.FormatString(Me.DesktopModule.Dependencies, strFormat)
                Case "permissions"
                    PublicProperty = False : PropertyNotFound = False : result = PropertyAccess.FormatString(Me.DesktopModule.Permissions, strFormat)
                Case "defaultcachetime"
                    PublicProperty = False : PropertyNotFound = False : result = (Me.ModuleDefinition.DefaultCacheTime.ToString(OutputFormat, formatProvider))
                Case "modulecontrolid"
                    PublicProperty = False : PropertyNotFound = False : result = (Me.ModuleControlId.ToString(OutputFormat, formatProvider))
                Case "controlsrc"
                    PublicProperty = False : PropertyNotFound = False : result = PropertyAccess.FormatString(Me.ModuleControl.ControlSrc, strFormat)
                Case "controltitle"
                    PublicProperty = True : PropertyNotFound = False : result = PropertyAccess.FormatString(Me.ModuleControl.ControlTitle, strFormat)
                Case "helpurl"
                    PublicProperty = True : PropertyNotFound = False : result = PropertyAccess.FormatString(Me.ModuleControl.HelpURL, strFormat)
                Case "supportspartialrendering"
                    PublicProperty = True : PropertyNotFound = False : result = (PropertyAccess.Boolean2LocalizedYesNo(Me.ModuleControl.SupportsPartialRendering, formatProvider))
                Case "containerpath"
                    PublicProperty = False : PropertyNotFound = False : result = PropertyAccess.FormatString(Me.ContainerPath, strFormat)
                Case "panemoduleindex"
                    PublicProperty = False : PropertyNotFound = False : result = (Me.PaneModuleIndex.ToString(OutputFormat, formatProvider))
                Case "panemodulecount"
                    PublicProperty = False : PropertyNotFound = False : result = (Me.PaneModuleCount.ToString(OutputFormat, formatProvider))
                Case "isdefaultmodule"
                    PublicProperty = False : PropertyNotFound = False : result = (PropertyAccess.Boolean2LocalizedYesNo(Me.IsDefaultModule, formatProvider))
                Case "allmodules"
                    PublicProperty = False : PropertyNotFound = False : result = (PropertyAccess.Boolean2LocalizedYesNo(Me.AllModules, formatProvider))
                Case "isportable"
                    PublicProperty = False : PropertyNotFound = False : result = (PropertyAccess.Boolean2LocalizedYesNo(Me.DesktopModule.IsPortable, formatProvider))
                Case "issearchable"
                    PublicProperty = False : PropertyNotFound = False : result = (PropertyAccess.Boolean2LocalizedYesNo(Me.DesktopModule.IsSearchable, formatProvider))
                Case "isupgradeable"
                    PublicProperty = False : PropertyNotFound = False : result = (PropertyAccess.Boolean2LocalizedYesNo(Me.DesktopModule.IsUpgradeable, formatProvider))
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

#Region "Obsolete"

        <Obsolete("Deprecated in DNN 5.1. All permission checks are done through Permission Collections")> _
        <XmlIgnore()> Public ReadOnly Property AuthorizedEditRoles() As String
            Get
                If String.IsNullOrEmpty(_AuthorizedEditRoles) Then
                    _AuthorizedEditRoles = ModulePermissions.ToString("EDIT")
                End If
                Return _AuthorizedEditRoles
            End Get
        End Property

        <Obsolete("Deprecated in DNN 5.1. All permission checks are done through Permission Collections")> _
        <XmlIgnore()> Public ReadOnly Property AuthorizedViewRoles() As String
            Get
                If String.IsNullOrEmpty(_AuthorizedViewRoles) Then
                    If InheritViewPermissions Then
                        _AuthorizedViewRoles = TabPermissionController.GetTabPermissions(TabID, PortalID).ToString("VIEW")
                    Else
                        _AuthorizedViewRoles = ModulePermissions.ToString("VIEW")
                    End If
                End If
                Return _AuthorizedViewRoles
            End Get
        End Property

        <Obsolete("Deprecated in DNN 5.0. Replaced by DesktopModule.ModuleName")> _
        <XmlIgnore()> Public Property ModuleName() As String
            Get
                Return DesktopModule.ModuleName
            End Get
            Set(ByVal Value As String)
                DesktopModule.ModuleName = Value
            End Set
        End Property

        <Obsolete("Deprecated in DNN 5.0. Replaced by DesktopModule.FriendlyName")> _
        <XmlIgnore()> Public Property FriendlyName() As String
            Get
                Return DesktopModule.FriendlyName
            End Get
            Set(ByVal Value As String)
                DesktopModule.FriendlyName = Value
            End Set
        End Property

        <Obsolete("Deprecated in DNN 5.0. Replaced by DesktopModule.FolderName")> _
        <XmlIgnore()> Public Property FolderName() As String
            Get
                Return DesktopModule.FolderName
            End Get
            Set(ByVal Value As String)
                DesktopModule.FolderName = Value
            End Set
        End Property

        <Obsolete("Deprecated in DNN 5.0. Replaced by DesktopModule.Description")> _
        <XmlIgnore()> Public Property Description() As String
            Get
                Return DesktopModule.Description
            End Get
            Set(ByVal Value As String)
                DesktopModule.Description = Value
            End Set
        End Property

        <Obsolete("Deprecated in DNN 5.0. Replaced by by DesktopModule.Version")> _
        <XmlIgnore()> Public Property Version() As String
            Get
                Return DesktopModule.Version
            End Get
            Set(ByVal Value As String)
                DesktopModule.Version = Value
            End Set
        End Property

        <Obsolete("Deprecated in DNN 5.0. Replaced by DesktopModule.IsPremium")> _
        <XmlIgnore()> Public Property IsPremium() As Boolean
            Get
                Return DesktopModule.IsPremium
            End Get
            Set(ByVal Value As Boolean)
                DesktopModule.IsPremium = Value
            End Set
        End Property

        <Obsolete("Deprecated in DNN 5.0. Replaced by DesktopModule.IsAdmin")> _
        <XmlIgnore()> Public Property IsAdmin() As Boolean
            Get
                Return DesktopModule.IsAdmin
            End Get
            Set(ByVal Value As Boolean)
                DesktopModule.IsAdmin = Value
            End Set
        End Property

        <Obsolete("Deprecated in DNN 5.0. Replaced by DesktopModule.BusinessControllerClass")> _
        <XmlIgnore()> Public Property BusinessControllerClass() As String
            Get
                Return DesktopModule.BusinessControllerClass
            End Get
            Set(ByVal Value As String)
                DesktopModule.BusinessControllerClass = Value
            End Set
        End Property

        <Obsolete("Deprecated in DNN 5.0. Replaced by DesktopModule.SupportedFeatures")> _
        <XmlIgnore()> Public Property SupportedFeatures() As Integer
            Get
                Return DesktopModule.SupportedFeatures
            End Get
            Set(ByVal Value As Integer)
                DesktopModule.SupportedFeatures = Value
            End Set
        End Property

        <Obsolete("Deprecated in DNN 5.0. Replaced by DesktopModule.IsPortable")> _
        <XmlIgnore()> Public ReadOnly Property IsPortable() As Boolean
            Get
                Return DesktopModule.IsPortable
            End Get
        End Property

        <Obsolete("Deprecated in DNN 5.0. Replaced by DesktopModule.IsSearchable")> _
        <XmlIgnore()> Public ReadOnly Property IsSearchable() As Boolean
            Get
                Return DesktopModule.IsSearchable
            End Get
        End Property

        <Obsolete("Deprecated in DNN 5.0. Replaced by DesktopModule.IsUpgradeable")> _
        <XmlIgnore()> Public ReadOnly Property IsUpgradeable() As Boolean
            Get
                Return DesktopModule.IsUpgradeable
            End Get
        End Property

        <Obsolete("Deprecated in DNN 5.0. Replaced by DesktopModule.CompatibleVersions")> _
        <XmlIgnore()> Public Property CompatibleVersions() As String
            Get
                Return DesktopModule.CompatibleVersions
            End Get
            Set(ByVal value As String)
                DesktopModule.CompatibleVersions = value
            End Set
        End Property

        <Obsolete("Deprecated in DNN 5.0. Replaced by DesktopModule.Dependencies")> _
        <XmlIgnore()> Public Property Dependencies() As String
            Get
                Return DesktopModule.Dependencies
            End Get
            Set(ByVal value As String)
                DesktopModule.Dependencies = value
            End Set
        End Property

        <Obsolete("Deprecated in DNN 5.0. Replaced by DesktopModule.Permisssions")> _
        <XmlIgnore()> Public Property Permissions() As String
            Get
                Return DesktopModule.Permissions
            End Get
            Set(ByVal value As String)
                DesktopModule.Permissions = value
            End Set
        End Property

        <Obsolete("Deprecated in DNN 5.0. Replaced by ModuleDefinition.DefaultCacheTime")> _
        <XmlIgnore()> Public Property DefaultCacheTime() As Integer
            Get
                Return ModuleDefinition.DefaultCacheTime
            End Get
            Set(ByVal Value As Integer)
                ModuleDefinition.DefaultCacheTime = Value
            End Set
        End Property

        <Obsolete("Deprecated in DNN 5.0. Replaced by ModuleControl.ControlSrc")> _
        <XmlIgnore()> Public Property ControlSrc() As String
            Get
                Return ModuleControl.ControlSrc
            End Get
            Set(ByVal Value As String)
                ModuleControl.ControlSrc = Value
            End Set
        End Property

        <Obsolete("Deprecated in DNN 5.0. Replaced by ModuleControl.ControlType")> _
        <XmlIgnore()> Public Property ControlType() As SecurityAccessLevel
            Get
                Return ModuleControl.ControlType
            End Get
            Set(ByVal Value As SecurityAccessLevel)
                ModuleControl.ControlType = Value
            End Set
        End Property

        <Obsolete("Deprecated in DNN 5.0. Replaced by ModuleControl.ControlTitle")> _
        <XmlIgnore()> Public Property ControlTitle() As String
            Get
                Return ModuleControl.ControlTitle
            End Get
            Set(ByVal Value As String)
                ModuleControl.ControlTitle = Value
            End Set
        End Property

        <Obsolete("Deprecated in DNN 5.0. Replaced by ModuleControl.HelpUrl")> _
        <XmlIgnore()> Public Property HelpUrl() As String
            Get
                Return ModuleControl.HelpURL
            End Get
            Set(ByVal Value As String)
                ModuleControl.HelpURL = Value
            End Set
        End Property

        <Obsolete("Deprecated in DNN 5.0. Replaced by ModuleControl.SupportsPartialRendering")> _
        <XmlIgnore()> Public Property SupportsPartialRendering() As Boolean
            Get
                Return ModuleControl.SupportsPartialRendering
            End Get
            Set(ByVal value As Boolean)
                ModuleControl.SupportsPartialRendering = value
            End Set
        End Property

        <Obsolete("Deprecated in DNN 5.1.")> _
        <XmlIgnore()> Protected ReadOnly Property TabPermissions() As TabPermissionCollection
            Get
                If _TabPermissions Is Nothing Then
                    _TabPermissions = TabPermissionController.GetTabPermissions(TabID, PortalID)
                End If
                Return _TabPermissions
            End Get
        End Property

#End Region

    End Class


End Namespace
