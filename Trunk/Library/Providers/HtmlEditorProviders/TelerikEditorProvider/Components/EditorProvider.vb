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
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.HtmlControls
Imports System.Web.UI.WebControls
Imports System.IO
Imports System.Xml

Imports DotNetNuke.Services.Exceptions
Imports DotNetNuke.Entities.Users
Imports DotNetNuke.UI.Utilities
Imports System.Reflection
Imports DotNetNuke.Entities.Modules
Imports Telerik.Web.UI
Imports DotNetNuke.Framework.Providers
Imports System.Collections.Generic
Imports DotNetNuke.UI

Namespace DotNetNuke.HtmlEditor.TelerikEditorProvider

    Public Class EditorProvider
        Inherits DotNetNuke.Modules.HTMLEditorProvider.HtmlEditorProvider

#Region "Private Members"

        Private WithEvents _panel As Panel = New Panel()
        Private WithEvents _editor As RadEditor = New RadEditor()

        'properties that will be skipped during ConfigFile processing
        Private _propertiesNotSupported As String = ",ContentProviderTypeName,ToolProviderID,Modules,AllowScripts,DialogHandlerUrl,RegisterWithScriptManager,ClientStateFieldID,Enabled,Visible,ControlStyleCreated,HasAttributes,ClientID,ID,EnableViewState,NamingContainer,BindingContainer,Page,TemplateControl,Parent,TemplateSourceDirectory,AppRelativeTemplateSourceDirectory,Site,UniqueID,Controls,ViewState,ViewStateIgnoreCase,"

        'default paths for the editor file browser dialogs (portal based)
        Private _portalRootPath As System.String() = New String() {FileSystemValidation.EndUserHomeDirectory}

        'config file settings
        Private _isControlInitialized As Boolean = False
        Private _languageSet As Boolean = False

        Private Const ProviderType As String = "htmlEditor"
        Private _providerConfiguration As ProviderConfiguration = ProviderConfiguration.GetProviderConfiguration(ProviderType)
        Private objProvider As DotNetNuke.Framework.Providers.Provider = CType(_providerConfiguration.Providers(_providerConfiguration.DefaultProvider), DotNetNuke.Framework.Providers.Provider)

        Private _TrackException As Exception = Nothing
#End Region

#Region "Properties"

        Private _additionalToolbars As ArrayList
        Public Overrides Property AdditionalToolbars() As ArrayList
            Get
                Return _additionalToolbars
            End Get
            Set(ByVal Value As ArrayList)
                _additionalToolbars = Value
            End Set
        End Property

        Public Overrides Property ControlID() As String
            Get
                Return _editor.ID
            End Get
            Set(ByVal Value As String)
                _editor.ID = Value
            End Set
        End Property

        Public Overrides Property Height() As Unit
            Get
                Height = _editor.Height
            End Get
            Set(ByVal Value As Unit)
                If (Not Value.IsEmpty) Then
                    _editor.Height = Value
                End If
            End Set
        End Property

        Public Overrides ReadOnly Property HtmlEditorControl() As Control
            Get
                Return CType(_panel, Control)
            End Get
        End Property

        Private _rootImageDirectory As String = String.Empty
        Public Overrides Property RootImageDirectory() As String
            Get
                If (String.IsNullOrEmpty(_rootImageDirectory)) Then
                    RootImageDirectory = PortalSettings.HomeDirectory
                Else
                    RootImageDirectory = _rootImageDirectory
                End If
            End Get
            Set(ByVal Value As String)
                _rootImageDirectory = Value
            End Set
        End Property

        Public Overrides Property Text() As String
            Get
                Return _editor.Content
            End Get
            Set(ByVal Value As String)
                _editor.Content = Value
            End Set
        End Property

        Public Overrides Property Width() As Unit
            Get
                Width = _editor.Width
            End Get
            Set(ByVal Value As Unit)
                If (Not Value.IsEmpty) Then
                    _editor.Width = Value
                End If
            End Set
        End Property

        Private _toolsFile As String = String.Empty
        Public Property ToolsFile() As String
            Get
                If (String.IsNullOrEmpty(_toolsFile) AndAlso Not objProvider.Attributes("ToolsFile") Is Nothing) Then
                    _toolsFile = objProvider.Attributes("ToolsFile")
                End If

                Return _toolsFile
            End Get
            Set(ByVal Value As String)
                _editor.ToolsFile = Value
                _toolsFile = Value
            End Set
        End Property

        Private _configFile As String = String.Empty
        Public Property ConfigFile() As String
            Get
                If (String.IsNullOrEmpty(_configFile) AndAlso Not objProvider.Attributes("ConfigFile") Is Nothing) Then
                    _configFile = objProvider.Attributes("ConfigFile")
                End If

                Return _configFile
            End Get
            Set(ByVal Value As String)
                _configFile = Value
                If (_isControlInitialized) Then
                    ProcessConfigFile()
                End If
            End Set
        End Property

        Private _filterHostExtensions As Boolean = True
        Public Property FilterHostExtensions() As Boolean
            Get
                Return _filterHostExtensions
            End Get
            Set(ByVal Value As Boolean)
                _filterHostExtensions = Value
            End Set
        End Property

        Private _providerPath As String = String.Empty
        Public ReadOnly Property ProviderPath() As String
            Get
                If (String.IsNullOrEmpty(_providerPath) AndAlso Not objProvider.Attributes("providerPath") Is Nothing) Then
                    _providerPath = objProvider.Attributes("providerPath")
                Else
                    _providerPath = "~/Providers/HtmlEditorProviders/Telerik/"
                End If

                If (Not _providerPath.EndsWith("/")) Then
                    _providerPath = _providerPath + "/"
                End If

                Return _providerPath
            End Get
        End Property

#End Region

#Region "Public Methods"

        Public Overrides Sub AddToolbar()
        End Sub

        Public Overrides Sub Initialize()
            Try
                'initialize the control
                LoadEditorProperties()
                _isControlInitialized = True
                _editor.EnableViewState = False
                _editor.ExternalDialogsPath = ProviderPath + "Dialogs/"

                If (Not String.IsNullOrEmpty(ToolsFile)) Then
                    'this will check if the file exists
                    GetFilePath(ToolsFile, "tools file")
                    _editor.ToolsFile = ToolsFile
                End If

                ProcessConfigFile()

            Catch ex As Exception
                _TrackException = New Exception("Could not load RadEditor. " & Environment.NewLine & ex.Message, ex)
                DotNetNuke.Services.Exceptions.LogException(_TrackException)
            End Try
        End Sub

        Protected Sub Panel_Init(ByVal sender As Object, ByVal e As EventArgs) Handles _panel.Init
            Try
                If (IsNothing(_TrackException)) Then
                    'Add save template js and gif
                    _panel.Controls.Add(New LiteralControl("<script type=""text/javascript"" src=""" + _panel.Page.ResolveUrl(ProviderPath + "js/RegisterDialogs.js") + """></script>"))
                    _panel.Controls.Add(New LiteralControl("<script type=""text/javascript"">var __textEditorSaveTemplateDialog = """ + _panel.Page.ResolveUrl(ProviderPath + "Dialogs/SaveTemplate.aspx") + """;</script>"))
                    _panel.Controls.Add(New LiteralControl("<style type=""text/css"">.reTool .SaveTemplate { background-image: url('" + _panel.Page.ResolveUrl(ProviderPath + "images/SaveTemplate.gif") + "'); }</style>"))

                    _panel.Controls.Add(_editor)
                End If
            Catch ex As Exception
                _TrackException = New Exception("Could not load RadEditor." & Environment.NewLine & ex.Message, ex)
                DotNetNuke.Services.Exceptions.LogException(_TrackException)
            End Try
        End Sub

        Protected Sub Panel_Load(ByVal sender As Object, ByVal e As EventArgs) Handles _panel.Load
            Try
                If (IsNothing(_TrackException)) Then
                    'register the override CSS file to take care of the DNN default skin problems
                    Dim cssOverrideUrl As String = _panel.Page.ResolveUrl(ProviderPath & "EditorOverride.css")
                    Dim pageScriptManager As ScriptManager = ScriptManager.GetCurrent(_panel.Page)
                    If (Not (pageScriptManager Is Nothing)) AndAlso (pageScriptManager.IsInAsyncPostBack) Then
                        _panel.Controls.Add(New LiteralControl(String.Format("<link title='Telerik stylesheet' type='text/css' rel='stylesheet' href='{0}'></link>", _panel.Page.Server.HtmlEncode(cssOverrideUrl))))
                    ElseIf Not (_panel.Page.Header Is Nothing) Then
                        Dim link As HtmlLink = New HtmlLink()
                        link.Href = cssOverrideUrl
                        link.Attributes.Add("type", "text/css")
                        link.Attributes.Add("rel", "stylesheet")
                        link.Attributes.Add("title", "Telerik stylesheet")
                        _panel.Page.Header.Controls.Add(link)
                    End If
                End If
            Catch ex As Exception
                _TrackException = New Exception("Could not load RadEditor." & Environment.NewLine & ex.Message, ex)
                DotNetNuke.Services.Exceptions.LogException(_TrackException)
            End Try
        End Sub

        Protected Sub Panel_PreRender(ByVal sender As Object, ByVal e As EventArgs) Handles _panel.PreRender
            Try
                'Exceptions are causing a stream of other NullReferenceExceptions
                'as a work around, track the exception and print out
                If (Not IsNothing(_TrackException)) Then
                    _panel.Controls.Clear()
                    _panel.Controls.Add(New LiteralControl(_TrackException.Message))
                End If
                Dim parentModule As PortalModuleBase = ControlUtilities.FindParentControl(Of PortalModuleBase)(HtmlEditorControl)
                Dim moduleid As Integer = CType(If(parentModule Is Nothing, -1, parentModule.ModuleId), Integer)
                Dim portalId As Integer = CType(If(parentModule Is Nothing, -1, parentModule.PortalId), Integer)
                Dim tabId As Integer = CType(If(parentModule Is Nothing, -1, parentModule.TabId), Integer)
                ClientAPI.RegisterClientVariable(HtmlEditorControl.Page, "editorModuleId", moduleid, True)
                ClientAPI.RegisterClientVariable(HtmlEditorControl.Page, "editorTabId", tabId, True)
                ClientAPI.RegisterClientVariable(HtmlEditorControl.Page, "editorPortalId", portalId, True)
                ClientAPI.RegisterClientVariable(HtmlEditorControl.Page, "editorHomeDirectory", PortalSettings.HomeDirectory, True)
                ClientAPI.RegisterClientVariable(HtmlEditorControl.Page, "editorPortalGuid", PortalSettings.GUID.ToString, True)
                ClientAPI.RegisterClientVariable(HtmlEditorControl.Page, "editorEnableUrlLanguage", PortalSettings.EnableUrlLanguage, True)
            Catch ex As Exception
                Throw ex
            End Try
        End Sub

        Protected Sub RadEditor_Load(ByVal sender As Object, ByVal e As EventArgs) Handles _editor.Load
            Try
                If (IsNothing(_TrackException)) Then
                    SetLanguage()

                    'set content paths to portal root
                    SetContentPaths(_editor.ImageManager)
                    SetContentPaths(_editor.FlashManager)
                    SetContentPaths(_editor.MediaManager)
                    SetContentPaths(_editor.DocumentManager)
                    SetContentPaths(_editor.TemplateManager)
                    SetContentPaths(_editor.SilverlightManager)

                    'set content provider type
                    Dim providerType As Type = GetType(PortalContentProvider)
                    Dim providerName As String = providerType.FullName & ", " & providerType.Assembly.FullName
                    _editor.ImageManager.ContentProviderTypeName = providerName
                    _editor.DocumentManager.ContentProviderTypeName = providerName
                    _editor.FlashManager.ContentProviderTypeName = providerName
                    _editor.MediaManager.ContentProviderTypeName = providerName
                    _editor.SilverlightManager.ContentProviderTypeName = providerName
                    _editor.TemplateManager.ContentProviderTypeName = providerName

                    'SetSearchPatterns
                    'telerik defaults + dnn extensions filter
                    Dim patterns As String = String.Empty
                    patterns = "*.gif, *.xbm, *.xpm, *.png, *.ief, *.jpg, *.jpe, *.jpeg, *.tiff, *.tif, *.rgb, *.g3f, *.xwd, *.pict, *.ppm, *.pgm, *.pbm, *.pnm, *.bmp, *.ras, *.pcd, *.cgm, *.mil, *.cal, *.fif, *.dsf, *.cmx, *.wi, *.dwg, *.dxf, *.svf".Replace(", ", ",")
                    SetFileManagerSearchPatterns(_editor.ImageManager, patterns)
                    patterns = ("*.doc, *.txt, *.docx, *.xls, *.xlsx, *.pdf" + ", *.ppt, *.pptx, *.xml, *.zip").Replace(", ", ",")
                    SetFileManagerSearchPatterns(_editor.DocumentManager, patterns)
                    patterns = "*.swf".Replace(", ", ",")
                    SetFileManagerSearchPatterns(_editor.FlashManager, patterns)
                    patterns = "*.wma, *.wmv, *.avi, *.wav, *.mpeg, *.mpg, *.mpe, *.mp3, *.m3u, *.mid, *.midi, *.snd, *.mkv".Replace(", ", ",")
                    SetFileManagerSearchPatterns(_editor.MediaManager, patterns)
                    patterns = "*.xap".Replace(", ", ",")
                    SetFileManagerSearchPatterns(_editor.SilverlightManager, patterns)
                    patterns = "*.htmtemplate, *.htm, *,html".Replace(", ", ",")
                    SetFileManagerSearchPatterns(_editor.TemplateManager, patterns)

                    'Can't set individual dialogdefinitions because they are not available, instead set globally for all dialogs
                    'this is done to fix problem with TemplateManager Window not reloading after saving a template
                    _editor.DialogOpener.Window.ReloadOnShow = True

                    'Set dialog handlers
                    Dim tempHandlerUrl As String = "Telerik.Web.UI.DialogHandler.aspx?tabid=" & PortalSettings.ActiveTab.TabID.ToString()
                    _editor.DialogHandlerUrl = _panel.Page.ResolveUrl(ProviderPath & tempHandlerUrl)
                    tempHandlerUrl = "Telerik.Web.UI.SpellCheckHandler.ashx?tabid=" & PortalSettings.ActiveTab.TabID.ToString()
                    _editor.SpellCheckSettings.AjaxUrl = _panel.Page.ResolveUrl(ProviderPath & tempHandlerUrl)
                End If
            Catch ex As Exception
                _TrackException = New Exception("Could not load RadEditor." & Environment.NewLine & ex.Message, ex)
                DotNetNuke.Services.Exceptions.LogException(_TrackException)
            End Try
        End Sub

#End Region

#Region "Config File related code"

        Private Function GetFilePath(ByVal path As String, ByVal fileDescription As String) As String
            Dim convertedPath As String = path
            If convertedPath.StartsWith("~") OrElse convertedPath.StartsWith("~") Then
                convertedPath = _panel.ResolveUrl(path)
            End If

            convertedPath = Context.Request.MapPath(convertedPath)
            If Not File.Exists(convertedPath) Then
                Throw New Exception("Invalid " + fileDescription + ". Check provider settings in the web.config: " + path)
            End If

            Return convertedPath
        End Function

        Private Function GetValidConfigFile() As XmlDocument
            Dim xmlConfigFile As New XmlDocument()
            Try
                xmlConfigFile.Load(GetFilePath(ConfigFile, "config file"))
            Catch generatedExceptionName As Exception
                Throw New Exception("Invalid Configuration File:" + ConfigFile)
            End Try
            Return xmlConfigFile
        End Function

        'don't allow property assignment of certain base control properties
        Private Sub ProcessConfigFile()
            If ConfigFile <> String.Empty Then
                Dim xmlDoc As XmlDocument = GetValidConfigFile()
                For Each node As XmlNode In xmlDoc.DocumentElement.ChildNodes
                    If node.Attributes Is Nothing OrElse node.Attributes("name") Is Nothing Then
                        Continue For
                    End If

                    Dim propertyName As String = node.Attributes("name").Value
                    Dim propValue As String = node.InnerText

                    If (propertyName.ToLower() = "cssfiles") Then
                        AddCssFiles(propValue)
                        Continue For
                    End If

                    'don't allow property assignment of certain base control properties
                    If (_propertiesNotSupported.Contains("," + propertyName + ",")) Then
                        Throw New NotSupportedException("Property assignment is not supported [" + propertyName + "]")
                    End If

                    'use reflection to set all string, integer, long, short, bool, enum properties
                    SetEditorProperty(_editor, propertyName, propValue)
                Next
            End If
        End Sub

        Private Sub SetEditorProperty(ByVal source As Object, ByVal propName As String, ByVal propValue As String)
            Dim properties As String() = propName.Split("."c)

            If (properties.Length > 0) Then
                For i As Integer = 0 To properties.Length - 2
                    If (_propertiesNotSupported.Contains("," + properties(i) + ",")) Then
                        Throw New NotSupportedException("Property assignment is not supported for this property FullPath:[" + propName + "] Property:[" + properties(i) + "]")
                    End If

                    Dim prop As PropertyInfo = source.GetType().GetProperty(properties(i))
                    If (prop Is Nothing) Then
                        Throw New NotSupportedException("Property does not exist. FullPath:[" + propName + "] Property:[" + properties(i) + "]")
                    End If

                    source = prop.GetValue(source, Nothing)
                    If (source Is Nothing) Then
                        Throw New NotSupportedException("Property does not exist or is null. FullPath:[" + propName + "] Property:[" + properties(i) + "]")
                    End If
                Next
                SetProperty(source, properties(properties.Length - 1), propValue)
            End If
        End Sub

        Private Sub SetProperty(ByVal source As Object, ByVal propName As String, ByVal propValue As String)
            Try
                If (source Is Nothing) Then
                    Exit Sub
                End If
                'Dim pi As PropertyInfo = DotNetNuke.Common.Utilities.DataCache.GetCache(Of PropertyInfo)("Telerik.EditorProvider." + propName + ".InfoCache")
                'If (pi Is Nothing) Then
                '	pi = _editor.GetType().GetProperty(propName)
                '	DotNetNuke.Common.Utilities.DataCache.SetCache("Telerik.EditorProvider." + propName + ".InfoCache", pi)
                'End If

                propValue = GetFileValueVirtualPath(propValue)

                Dim pi As PropertyInfo = source.GetType().GetProperty(propName)
                Dim propObj As Object = Nothing
                If Not (pi Is Nothing) Then
                    If pi.PropertyType.Equals(GetType(String)) Then
                        pi.SetValue(source, propValue, Nothing)
                    ElseIf pi.PropertyType.Equals(GetType(Boolean)) Then
                        pi.SetValue(source, Boolean.Parse(propValue), Nothing)
                    ElseIf pi.PropertyType.Equals(GetType(Unit)) Then
                        pi.SetValue(source, Unit.Parse(propValue), Nothing)
                    ElseIf pi.PropertyType.Equals(GetType(Integer)) Then
                        pi.SetValue(source, Integer.Parse(propValue), Nothing)
                    ElseIf pi.PropertyType.Equals(GetType(Short)) Then
                        pi.SetValue(source, Short.Parse(propValue), Nothing)
                    ElseIf pi.PropertyType.Equals(GetType(Long)) Then
                        pi.SetValue(source, Long.Parse(propValue), Nothing)
                    ElseIf pi.PropertyType.BaseType.Equals(GetType(System.Enum)) Then
                        pi.SetValue(source, System.Enum.Parse(pi.PropertyType, propValue), Nothing)
                    ElseIf pi.PropertyType.Equals(GetType(String())) Then
                        pi.SetValue(source, propValue.Split(","c), Nothing)
                    ElseIf pi.PropertyType.Equals(GetType(Telerik.Web.UI.EditorFontCollection)) Then
                        propObj = pi.GetValue(source, Nothing)
                        If (Not IsNothing(propObj)) Then
                            DirectCast(propObj, Telerik.Web.UI.EditorFontCollection).Add(propValue)
                        End If
                    ElseIf pi.PropertyType.Equals(GetType(Telerik.Web.UI.EditorFontSizeCollection)) Then
                        propObj = pi.GetValue(source, Nothing)
                        If (Not IsNothing(propObj)) Then
                            DirectCast(propObj, Telerik.Web.UI.EditorFontSizeCollection).Add(propValue)
                        End If
                    ElseIf pi.PropertyType.Equals(GetType(Telerik.Web.UI.EditorRealFontSizeCollection)) Then
                        propObj = pi.GetValue(source, Nothing)
                        If (Not IsNothing(propObj)) Then
                            DirectCast(propObj, Telerik.Web.UI.EditorRealFontSizeCollection).Add(propValue)
                        End If
                    ElseIf pi.PropertyType.Equals(GetType(Telerik.Web.UI.EditorSymbolCollection)) Then
                        propObj = pi.GetValue(source, Nothing)
                        If (Not IsNothing(propObj)) Then
                            DirectCast(propObj, Telerik.Web.UI.EditorSymbolCollection).Add(propValue)
                        End If
                    ElseIf pi.PropertyType.Equals(GetType(Telerik.Web.UI.EditorColorCollection)) Then
                        propObj = pi.GetValue(source, Nothing)
                        If (Not IsNothing(propObj)) Then
                            DirectCast(propObj, Telerik.Web.UI.EditorColorCollection).Add(propValue)
                        End If
                    ElseIf pi.PropertyType.Equals(GetType(Telerik.Web.UI.EditorParagraphCollection)) Then
                        propObj = pi.GetValue(source, Nothing)
                        If (Not IsNothing(propObj)) Then
                            DirectCast(propObj, Telerik.Web.UI.EditorParagraphCollection).Add(propValue, "." + propValue)
                        End If
                    End If
                End If
                If (propName = "Language") Then
                    _languageSet = True
                End If
            Catch ex As Exception
                Throw New Exception("Error parsing config file Type:[" + source.GetType().ToString() + "] Property:[" + propName + "] Error:[" + ex.Message + "]", ex)
            End Try
        End Sub

        Private Sub AddCssFiles(ByVal cssFiles As String)
            Dim files As String() = cssFiles.Split(","c)

            If files.Length > 0 Then
                _editor.CssFiles.Clear()
                Dim cssfile As String
                For Each cssfile In files
                    cssfile = GetFileValueVirtualPath(cssfile)
                    _editor.CssFiles.Add(cssfile)
                Next
            End If
        End Sub

        Private Function GetFileValueVirtualPath(ByVal propValue As String) As String
            If (propValue.StartsWith("[PortalRoot]") OrElse propValue.StartsWith("[ProviderPath]") OrElse propValue.StartsWith("~")) Then
                propValue = propValue.Replace("[PortalRoot]", PortalSettings.HomeDirectory)
                propValue = propValue.Replace("[ProviderPath]", ProviderPath)
                If (propValue.StartsWith("~")) Then
                    propValue = _panel.ResolveUrl(propValue)
                End If
            End If

            Return propValue
        End Function

#End Region

#Region "Private Helper Methods"

        Private Sub SetLanguage()
            _editor.LocalizationPath = ProviderPath + "App_LocalResources/"

            If (Not _languageSet) Then
                Dim localizationLang As String = String.Empty
                'first check portal settings
                localizationLang = PortalSettings.DefaultLanguage
                'then check if language cookie is present
                If Not (_panel.Page.Request.Cookies("language") Is Nothing) Then
                    Dim cookieValue As String = _panel.Page.Request.Cookies.Get("language").Value
                    localizationLang = cookieValue
                End If
                'set new value
                If (localizationLang <> String.Empty) Then
                    _editor.Language = localizationLang
                End If
            End If
        End Sub

        Private Sub SetContentPaths(ByVal manager As FileManagerDialogConfiguration)
            If (IsNothing(manager.ViewPaths) OrElse manager.ViewPaths.Length < 1) Then
                manager.ViewPaths = _portalRootPath
            End If
            If (IsNothing(manager.UploadPaths) OrElse manager.UploadPaths.Length < 1) Then
                manager.UploadPaths = _portalRootPath
            End If
            If (IsNothing(manager.DeletePaths) OrElse manager.DeletePaths.Length < 1) Then
                manager.DeletePaths = _portalRootPath
            End If
        End Sub

        Private Sub SetFileManagerSearchPatterns(ByRef dialogConfig As Telerik.Web.UI.FileManagerDialogConfiguration, ByVal patterns As String)
            If (IsNothing(dialogConfig.SearchPatterns) OrElse dialogConfig.SearchPatterns.Length < 1) Then
                dialogConfig.SearchPatterns = ApplySearchPatternFilter(patterns.Split(","))
            Else
                dialogConfig.SearchPatterns = ApplySearchPatternFilter(dialogConfig.SearchPatterns)
            End If
        End Sub

        Private Sub RemoveTool(ByVal sToolName As String)
            Dim toolbar As EditorToolGroup
            For Each toolbar In _editor.Tools
                Dim toolRef As EditorTool = toolbar.FindTool(sToolName)
                If Not (toolRef Is Nothing) Then
                    toolbar.Tools.Remove(toolRef)
                End If
            Next
        End Sub

        Private Sub LoadEditorProperties()
            Try
                If Not objProvider.Attributes("ConfigFile") Is Nothing Then _configFile = objProvider.Attributes("ConfigFile")
                If Not objProvider.Attributes("ToolsFile") Is Nothing Then _toolsFile = objProvider.Attributes("ToolsFile")
            Catch exc As Exception
                Throw New Exception("Could not load RadEditor! Error while loading provider attributes: " & exc.Message)
            End Try
        End Sub

        Private Function ApplySearchPatternFilter(ByVal patterns As String()) As String()
            If (Not FilterHostExtensions) Then
                Return patterns
            End If

            Dim returnPatterns As ArrayList = New ArrayList()
            Dim hostExtensions As String = DotNetNuke.Entities.Host.Host.FileExtensions.ToLowerInvariant().Replace(" ", "")

            If (patterns.Length = 1 AndAlso patterns(0) = "*.*") Then
                'Include all host partterns
                For Each pattern As String In hostExtensions.Split(",")
                    returnPatterns.Add("*." + pattern)
                Next
            Else
                For Each pattern As String In patterns
                    pattern = pattern.Replace("*.", "").ToLowerInvariant()
                    If (("," + hostExtensions + ",").IndexOf("," + pattern + ",") > -1) Then
                        returnPatterns.Add("*." + pattern)
                    End If
                Next
            End If

            Return DirectCast(returnPatterns.ToArray(GetType(String)), String())
        End Function

#End Region

    End Class

End Namespace
