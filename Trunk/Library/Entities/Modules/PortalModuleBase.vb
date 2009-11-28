''
'' DotNetNuke® - http://www.dotnetnuke.com
'' Copyright (c) 2002-2009
'' by DotNetNuke Corporation
''
'' Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
'' documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
'' the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
'' to permit persons to whom the Software is furnished to do so, subject to the following conditions:
''
'' The above copyright notice and this permission notice shall be included in all copies or substantial portions 
'' of the Software.
''
'' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
'' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
'' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
'' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
'' DEALINGS IN THE SOFTWARE.
''

Imports System
Imports System.Configuration
Imports System.Data
Imports System.IO

Imports DotNetNuke.Entities.Modules.Actions
Imports DotNetNuke.Services.Localization
Imports DotNetNuke.UI.Skins
Imports DotNetNuke.UI.Modules
Imports DotNetNuke.Security.Permissions

Namespace DotNetNuke.Entities.Modules

    ''' -----------------------------------------------------------------------------
    ''' Project	 : DotNetNuke
    ''' Class	 : PortalModuleBase
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The PortalModuleBase class defines a custom base class inherited by all
    ''' desktop portal modules within the Portal.
    ''' 
    ''' The PortalModuleBase class defines portal specific properties
    ''' that are used by the portal framework to correctly display portal modules
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    '''		[cnurse]	09/17/2004	Added Documentation
    '''								Modified LocalResourceFile to be Writeable
    '''		[cnurse]	10/21/2004	Modified Settings property to get both
    '''								TabModuleSettings and ModuleSettings
    '''     [cnurse]    12/15/2007  Refactored to support the new IModuleControl 
    '''                             Interface
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class PortalModuleBase
        Inherits UserControlBase
        Implements IModuleControl

#Region "Private Members"

        Private _helpfile As String
        Private _localResourceFile As String
        Private _moduleContext As ModuleInstanceContext

#End Region

#Region "IModuleControl Members"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the underlying base control for this ModuleControl
        ''' </summary>
        ''' <returns>A String</returns>
        ''' <history>
        ''' 	[cnurse]	12/17/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public ReadOnly Property Control() As Control Implements IModuleControl.Control
            Get
                Return Me
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the Path for this control (used primarily for UserControls)
        ''' </summary>
        ''' <returns>A String</returns>
        ''' <history>
        ''' 	[cnurse]	12/16/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public ReadOnly Property ControlPath() As String Implements IModuleControl.ControlPath
            Get
                Return Me.TemplateSourceDirectory & "/"
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the Name for this control
        ''' </summary>
        ''' <returns>A String</returns>
        ''' <history>
        ''' 	[cnurse]	12/16/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public ReadOnly Property ControlName() As String Implements IModuleControl.ControlName
            Get
                Return Me.GetType.Name.Replace("_", ".")
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the local resource file for this control
        ''' </summary>
        ''' <returns>A String</returns>
        ''' <history>
        ''' 	[cnurse]	12/16/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property LocalResourceFile() As String Implements IModuleControl.LocalResourceFile
            Get
                Dim fileRoot As String

                If String.IsNullOrEmpty(_localResourceFile) Then
                    fileRoot = Path.Combine(Me.ControlPath, Services.Localization.Localization.LocalResourceDirectory & "/" & Me.ID)
                Else
                    fileRoot = _localResourceFile
                End If
                Return fileRoot
            End Get
            Set(ByVal Value As String)
                _localResourceFile = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the Module Context for this control
        ''' </summary>
        ''' <returns>A ModuleInstanceContext</returns>
        ''' <history>
        ''' 	[cnurse]	12/16/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public ReadOnly Property ModuleContext() As ModuleInstanceContext Implements IModuleControl.ModuleContext
            Get
                If _moduleContext Is Nothing Then
                    _moduleContext = New ModuleInstanceContext(Me)
                End If
                Return _moduleContext
            End Get
        End Property


#End Region

#Region "Public Properties"

        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public Property Actions() As ModuleActionCollection
            Get
                Return ModuleContext.Actions
            End Get
            Set(ByVal Value As ModuleActionCollection)
                Me.ModuleContext.Actions = Value
            End Set
        End Property

        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public ReadOnly Property ContainerControl() As Control
            Get
                Return FindControlRecursive(Me, "ctr" & ModuleId.ToString)
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The CacheMethod property is used to store the Method used for this Module's
        ''' Cache
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        '''   [cnurse] 04/28/2005  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <Obsolete("The CacheMethod property has been internalized in version 5.0, it should not be used by API consumers", True)> _
        Public ReadOnly Property CacheMethod() As String
            Get
                Return Host.Host.ModuleCachingMethod
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The EditMode property is used to determine whether the user is in the 
        ''' Administrator role
        ''' Cache
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        '''   [cnurse] 01/19/2006  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public ReadOnly Property EditMode() As Boolean
            Get
                Return Me.ModuleContext.EditMode
            End Get
        End Property

        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public Function EditUrl() As String
            Return Me.ModuleContext.EditUrl()
        End Function

        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public Function EditUrl(ByVal ControlKey As String) As String
            Return Me.ModuleContext.EditUrl(ControlKey)
        End Function

        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public Function EditUrl(ByVal KeyName As String, ByVal KeyValue As String) As String
            Return Me.ModuleContext.EditUrl(KeyName, KeyValue)
        End Function

        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public Function EditUrl(ByVal KeyName As String, ByVal KeyValue As String, ByVal ControlKey As String) As String
            Return Me.ModuleContext.EditUrl(KeyName, KeyValue, ControlKey)
        End Function

        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public Function EditUrl(ByVal KeyName As String, ByVal KeyValue As String, ByVal ControlKey As String, ByVal ParamArray AdditionalParameters As String()) As String
            Return Me.ModuleContext.EditUrl(KeyName, KeyValue, ControlKey, AdditionalParameters)
        End Function

        Public Property HelpURL() As String
            Get
                Return Me.ModuleContext.HelpURL
            End Get
            Set(ByVal Value As String)
                Me.ModuleContext.HelpURL = Value
            End Set
        End Property

        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public ReadOnly Property IsEditable() As Boolean
            Get
                Return Me.ModuleContext.IsEditable
            End Get
        End Property

        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public Property ModuleConfiguration() As ModuleInfo
            Get
                Return Me.ModuleContext.Configuration
            End Get
            Set(ByVal Value As ModuleInfo)
                Me.ModuleContext.Configuration = Value
            End Set
        End Property

        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public ReadOnly Property PortalId() As Integer
            Get
                Return Me.ModuleContext.PortalId
            End Get
        End Property

        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public ReadOnly Property TabId() As Integer
            Get
                Return Me.ModuleContext.TabId
            End Get
        End Property

        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public Property TabModuleId() As Integer
            Get
                Return Me.ModuleContext.TabModuleId
            End Get
            Set(ByVal value As Integer)
                Me.ModuleContext.TabModuleId = value
            End Set
        End Property

        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public Property ModuleId() As Integer
            Get
                Return Me.ModuleContext.ModuleId
            End Get
            Set(ByVal value As Integer)
                Me.ModuleContext.ModuleId = value
            End Set
        End Property

        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public ReadOnly Property UserInfo() As UserInfo
            Get
                Return PortalSettings.UserInfo
            End Get
        End Property

        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public ReadOnly Property UserId() As Integer
            Get
                Return PortalSettings.UserId
            End Get
        End Property

        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public ReadOnly Property PortalAlias() As PortalAliasInfo
            Get
                Return PortalSettings.PortalAlias
            End Get
        End Property

        <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
        Public ReadOnly Property Settings() As Hashtable
            Get
                Return Me.ModuleContext.Settings
            End Get
        End Property

#End Region

#Region "Protected Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Helper method that can be used to add an ActionEventHandler to the Skin for this 
        ''' Module Control
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        '''   [cnurse] 17/9/2004  Added Documentation
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Sub AddActionHandler(ByVal e As ActionEventHandler)

            'This finds a reference to the containing skin
            Dim ParentSkin As DotNetNuke.UI.Skins.Skin = DotNetNuke.UI.Skins.Skin.GetParentSkin(Me)
            'We should always have a ParentSkin, but need to make sure
            If Not ParentSkin Is Nothing Then
                'Register our EventHandler as a listener on the ParentSkin so that it may tell us 
                'when a menu has been clicked.
                ParentSkin.RegisterModuleActionEvent(Me.ModuleId, e)
            End If

        End Sub

#End Region

#Region "Public Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the Next Action ID
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        '''   [cnurse] 03/02/2006  Added Documentation
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function GetNextActionID() As Integer
            Return Me.ModuleContext.GetNextActionID()
        End Function

#End Region

#Region "Obsolete methods"

        ' CONVERSION: Remove obsoleted methods (FYI some core modules use these, such as Links)
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The CacheDirectory property is used to return the location of the "Cache"
        ''' Directory for the Module
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        '''   [cnurse] 04/28/2005  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        ''' 
        <Obsolete("This property is deprecated.  Plaese use ModuleController.CacheDirectory()")> _
        Public ReadOnly Property CacheDirectory() As String
            Get
                Return PortalController.GetCurrentPortalSettings.HomeDirectoryMapPath & "Cache"
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The CacheFileName property is used to store the FileName for this Module's
        ''' Cache
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        '''   [cnurse] 04/28/2005  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <Obsolete("This property is deprecated.  Please use ModuleController.CacheFileName(TabModuleID)")> _
        Public ReadOnly Property CacheFileName() As String
            Get
                Dim strCacheKey As String = "TabModule:"
                strCacheKey += TabModuleId.ToString & ":"
                strCacheKey += System.Threading.Thread.CurrentThread.CurrentCulture.ToString
                Return PortalController.GetCurrentPortalSettings.HomeDirectoryMapPath & "Cache" & "\" & CleanFileName(strCacheKey) & ".resources"
            End Get
        End Property

        <Obsolete("This property is deprecated.  Please use ModuleController.CacheFileName(TabModuleID)")> _
        Public ReadOnly Property CacheFileName(ByVal TabModuleID As Integer) As String
            Get
                Dim strCacheKey As String = "TabModule:"
                strCacheKey += TabModuleID.ToString & ":"
                strCacheKey += System.Threading.Thread.CurrentThread.CurrentCulture.ToString
                Return PortalController.GetCurrentPortalSettings.HomeDirectoryMapPath & "Cache" & "\" & CleanFileName(strCacheKey) & ".resources"
            End Get
        End Property

        <Obsolete("This property is deprecated.  Please use ModuleController.CacheKey(TabModuleID)")> _
        Public ReadOnly Property CacheKey() As String
            Get
                Dim strCacheKey As String = "TabModule:"
                strCacheKey += TabModuleId.ToString & ":"
                strCacheKey += System.Threading.Thread.CurrentThread.CurrentCulture.ToString
                Return strCacheKey
            End Get
        End Property

        <Obsolete("This property is deprecated.  Please use ModuleController.CacheKey(TabModuleID)")> _
        Public ReadOnly Property CacheKey(ByVal TabModuleID As Integer) As String
            Get
                Dim strCacheKey As String = "TabModule:"
                strCacheKey += TabModuleID.ToString & ":"
                strCacheKey += System.Threading.Thread.CurrentThread.CurrentCulture.ToString
                Return strCacheKey
            End Get
        End Property

        <Obsolete("Deprecated in DNN 5.0. Please use ModulePermissionController.HasModulePermission(ModuleConfiguration.ModulePermissions, PermissionKey) ")> _
        Public Function HasModulePermission(ByVal PermissionKey As String) As Boolean
            Return ModulePermissionController.HasModulePermission(Me.ModuleConfiguration.ModulePermissions, PermissionKey)
        End Function

        ' CONVERSION: Obsolete pre 5.0 => Remove in 5.0
        <ObsoleteAttribute("The HelpFile() property was deprecated in version 2.2. Help files are now stored in the /App_LocalResources folder beneath the module with the following resource key naming convention: ModuleHelp.Text")> _
        Public Property HelpFile() As String
            Get
                Return _helpfile
            End Get
            Set(ByVal Value As String)
                _helpfile = Value
            End Set
        End Property

        <Obsolete("ModulePath was renamed to ControlPath and moved to IModuleControl in version 5.0")> _
        Public ReadOnly Property ModulePath() As String
            Get
                Return Me.ControlPath
            End Get
        End Property

        <Obsolete("This method is deprecated.  Plaese use ModuleController.SynchronizeModule(ModuleId)")> _
        Public Sub SynchronizeModule()
            ModuleController.SynchronizeModule(ModuleId)
        End Sub

#End Region

    End Class

End Namespace
