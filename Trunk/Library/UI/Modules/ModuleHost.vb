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

Imports System.IO
Imports System.Threading

Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Entities.Modules.Communications
Imports DotNetNuke.Entities.Portals.PortalSettings
Imports DotNetNuke.Entities.Host
Imports DotNetNuke.UI.Skins
Imports DotNetNuke.Security.Permissions

Namespace DotNetNuke.UI.Modules

    ''' -----------------------------------------------------------------------------
    ''' Project	 : DotNetNuke
    ''' Namespace: DotNetNuke.UI.Modules
    ''' Class	 : ModuleHost
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' ModuleHost hosts a Module Control (or its cached Content).
    ''' </summary>
    ''' <history>
    ''' 	[cnurse]	12/15/2007  created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class ModuleHost
        Inherits Panel

#Region "Private Members"

        Private _Control As Control
        Private _IsCached As Boolean
        Private _ModuleConfiguration As ModuleInfo
        Private _Skin As DotNetNuke.UI.Skins.Skin

        Private Shared objReaderWriterLock As New ReaderWriterLock()
        Private ReaderTimeOut As Integer = 10
        Private WriterTimeOut As Integer = 100

#End Region

#Region "Constructors"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Creates a Module Host control using the ModuleConfiguration for the Module
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	12/16/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub New(ByVal moduleConfiguration As ModuleInfo, ByVal skin As DotNetNuke.UI.Skins.Skin)
            _ModuleConfiguration = moduleConfiguration
            _Skin = skin
            ID = "ModuleContent"
        End Sub

#End Region

#Region "Public Properties"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the attached ModuleControl
        ''' </summary>
        ''' <returns>An IModuleControl</returns>
        ''' <history>
        ''' 	[cnurse]	12/15/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public ReadOnly Property ModuleControl() As IModuleControl
            Get
                'Make sure the Control tree has been created
                EnsureChildControls()
                Return TryCast(_Control, IModuleControl)
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the current POrtal Settings
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	12/15/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public ReadOnly Property PortalSettings() As PortalSettings
            Get
                Return PortalController.GetCurrentPortalSettings
            End Get
        End Property

#End Region

#Region "Private Methods"

        Private Sub InjectModuleContent(ByVal content As Control)
            If _ModuleConfiguration.IsWebSlice And Not IsAdminControl() Then
                'Assign the class - hslice to the Drag-N-Drop Panel
                Me.CssClass = "hslice"
                Dim titleLabel As New Label
                titleLabel.CssClass = "entry-title Hidden"
                If Not String.IsNullOrEmpty(_ModuleConfiguration.WebSliceTitle) Then
                    titleLabel.Text = _ModuleConfiguration.WebSliceTitle
                Else
                    titleLabel.Text = _ModuleConfiguration.ModuleTitle
                End If
                Me.Controls.Add(titleLabel)

                Dim websliceContainer As New Panel
                websliceContainer.CssClass = "entry-content"
                websliceContainer.Controls.Add(content)

                Dim expiry As New HtmlGenericControl
                expiry.TagName = "abbr"
                expiry.Attributes("class") = "endtime"
                If Not Null.IsNull(_ModuleConfiguration.WebSliceExpiryDate) Then
                    expiry.Attributes("title") = _ModuleConfiguration.WebSliceExpiryDate.ToString("o")
                    websliceContainer.Controls.Add(expiry)
                ElseIf _ModuleConfiguration.EndDate < Date.MaxValue Then
                    expiry.Attributes("title") = _ModuleConfiguration.EndDate.ToString("o")
                    websliceContainer.Controls.Add(expiry)
                End If

                Dim ttl As New HtmlGenericControl
                ttl.TagName = "abbr"
                ttl.Attributes("class") = "ttl"
                If _ModuleConfiguration.WebSliceTTL > 0 Then
                    ttl.Attributes("title") = _ModuleConfiguration.WebSliceTTL.ToString()
                    websliceContainer.Controls.Add(ttl)
                ElseIf _ModuleConfiguration.CacheTime > 0 Then
                    ttl.Attributes("title") = (_ModuleConfiguration.CacheTime \ 60).ToString()
                    websliceContainer.Controls.Add(ttl)
                End If

                Me.Controls.Add(websliceContainer)
            Else
                Me.Controls.Add(content)
            End If
        End Sub

        ''' ----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets a flag that indicates whether the Module Content should be displayed
        ''' </summary>
        ''' <returns>A Boolean</returns>
        ''' <history>
        ''' [cnurse]	12/15/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Function DisplayContent() As Boolean
            'module content visibility options
            Dim blnContent As Boolean = PortalSettings.UserMode <> Mode.Layout
            If Not Page.Request.QueryString("content") Is Nothing Then
                Select Case Page.Request.QueryString("Content").ToLower
                    Case "1", "true"
                        blnContent = True
                    Case "0", "false"
                        blnContent = False
                End Select
            End If
            If IsAdminControl() = True Then
                blnContent = True
            End If
            Return blnContent
        End Function

        Private Sub InjectMessageControl(ByVal container As Control)
            ' inject a message placeholder for common module messaging - UI.Skins.Skin.AddModuleMessage
            Dim MessagePlaceholder As New PlaceHolder
            MessagePlaceholder.ID = "MessagePlaceHolder"
            MessagePlaceholder.Visible = False
            container.Controls.Add(MessagePlaceholder)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets a flag that indicates whether the Module is in View Mode
        ''' </summary>
        ''' <returns>A Boolean</returns>
        ''' <history>
        ''' 	[cnurse]	12/15/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Function IsViewMode() As Boolean
            Return Not (ModulePermissionController.HasModuleAccess(SecurityAccessLevel.Edit, Null.NullString, _ModuleConfiguration)) _
                                OrElse PortalSettings.UserMode = PortalSettings.Mode.View
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' LoadModuleControl loads the ModuleControl (PortalModuelBase)
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	12/15/2007	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub LoadModuleControl()
            Try
                If DisplayContent() Then
                    'if the module supports caching and caching is enabled for the instance and the user does not have Edit rights or is currently in View mode
                    If SupportsCaching() AndAlso IsViewMode() Then
                        'attempt to load the cached content
                        _IsCached = TryLoadCached()
                    End If
                    If Not _IsCached Then
                        ' load the control dynamically
                        _Control = ControlUtilities.LoadControl(Of Control)(Me.Page, _ModuleConfiguration.ModuleControl.ControlSrc)

                        ' set the control ID to the resource file name ( ie. controlname.ascx = controlname )
                        ' this is necessary for the Localization in PageBase
                        _Control.ID = Path.GetFileNameWithoutExtension(_ModuleConfiguration.ModuleControl.ControlSrc)
                    End If
                Else       ' content placeholder
                    _Control = New ModuleControlBase()
                End If

                'check for IMC
                _Skin.Communicator.LoadCommunicator(_Control)

                ' add module settings
                ModuleControl.ModuleContext.Configuration = _ModuleConfiguration

            Catch exc As Threading.ThreadAbortException
                Threading.Thread.ResetAbort()
            Catch exc As Exception
                _Control = New ModuleControlBase()

                '' add module settings
                ModuleControl.ModuleContext.Configuration = _ModuleConfiguration

                If TabPermissionController.CanAdminPage() Then
                    ' only display the error to page administrators
                    ProcessModuleLoadException(_Control, exc)
                End If
            End Try

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' LoadUpdatePanel optionally loads an AJAX Update Panel
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	12/16/2007	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub LoadUpdatePanel()
            'register AJAX
            AJAX.RegisterScriptManager()

            'enable Partial Rendering
            AJAX.SetScriptManagerProperty(Me.Page, "EnablePartialRendering", New Object() {True})

            'create update panel
            Dim objUpdatePanel As Control = AJAX.CreateUpdatePanelControl
            objUpdatePanel.ID = _Control.ID & "_UP"

            'get update panel content template
            Dim objContentTemplateContainer As Control = AJAX.ContentTemplateContainerControl(objUpdatePanel)

            ' inject a message placeholder for common module messaging - UI.Skins.Skin.AddModuleMessage
            InjectMessageControl(objContentTemplateContainer)

            'inject module into update panel content template
            objContentTemplateContainer.Controls.Add(_Control)

            'inject the update panel into the panel
            InjectModuleContent(objUpdatePanel)

            'create image for update progress control
            Dim objImage As System.Web.UI.WebControls.Image = New System.Web.UI.WebControls.Image()
            objImage.ImageUrl = "~/images/progressbar.gif"  'hardcoded
            objImage.AlternateText = "ProgressBar"

            'inject updateprogress into the panel
            Me.Controls.Add(AJAX.CreateUpdateProgressControl(objUpdatePanel.ID, objImage))
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets a flag that indicates whether the Module Instance supports Caching
        ''' </summary>
        ''' <returns>A Boolean</returns>
        ''' <history>
        ''' 	[cnurse]	12/15/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Function SupportsCaching() As Boolean
            Return _ModuleConfiguration.CacheTime <> 0 _
                    AndAlso HttpContext.Current.Request.Browser.Crawler = False
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Trys to load previously cached Module Content
        ''' </summary>
        ''' <returns>A Boolean that indicates whether the cahed content was loaded</returns>
        ''' <history>
        ''' 	[cnurse]	12/15/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Function TryLoadCached() As Boolean
            Dim bSuccess As Boolean = False
            Dim _cachedContent As String = String.Empty

            Try
                Dim cache As ModuleCache.ModuleCachingProvider = ModuleCache.ModuleCachingProvider.Instance(_ModuleConfiguration.GetEffectiveCacheMethod)
                Dim varyBy As New System.Collections.Generic.SortedDictionary(Of String, String)
                varyBy.Add("locale", System.Threading.Thread.CurrentThread.CurrentCulture.ToString)
                Dim cacheKey As String = cache.GenerateCacheKey(_ModuleConfiguration.TabModuleID, varyBy)
                Dim cachedBytes As Byte() = ModuleCache.ModuleCachingProvider.Instance(_ModuleConfiguration.GetEffectiveCacheMethod).GetModule(_ModuleConfiguration.TabModuleID, cacheKey)
                If Not cachedBytes Is Nothing AndAlso cachedBytes.Length > 0 Then
                    _cachedContent = System.Text.Encoding.UTF8.GetString(cachedBytes)
                    bSuccess = True
                End If
            Catch ex As Exception
                _cachedContent = String.Empty
                bSuccess = False
            End Try

            If bSuccess Then
                _Control = New CachedModuleControl(_cachedContent)
                Me.Controls.Add(_Control)
            End If
            Return bSuccess
        End Function

#End Region

#Region "Protected Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' CreateChildControls builds the control tree
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	12/15/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overrides Sub CreateChildControls()
            'Clear existing controls
            Controls.Clear()

            'Load Module Control (or cached control)
            LoadModuleControl()

            'Optionally Inject AJAX Update Panel
            If Not ModuleControl Is Nothing Then
                'if module is dynamically loaded and AJAX is installed and the control supports partial rendering (defined in ModuleControls table )
                If Not _IsCached AndAlso _ModuleConfiguration.ModuleControl.SupportsPartialRendering AndAlso AJAX.IsInstalled Then
                    LoadUpdatePanel()
                Else
                    ' inject a message placeholder for common module messaging - UI.Skins.Skin.AddModuleMessage
                    InjectMessageControl(Me)

                    'inject the module into the panel
                    InjectModuleContent(_Control)
                End If
            End If

            'By now the control is in the Page's Controls Collection
            Dim profileModule As IProfileModule = TryCast(_Control, IProfileModule)
            If profileModule IsNot Nothing Then
                'Find Container
                Dim _Container As DotNetNuke.UI.Containers.Container = ControlUtilities.FindParentControl(Of DotNetNuke.UI.Containers.Container)(_Control)
                If _Container IsNot Nothing Then
                    _Container.Visible = profileModule.DisplayModule
                End If
            End If

        End Sub

        Protected Overrides Sub OnPreRender(ByVal e As System.EventArgs)
            MyBase.OnPreRender(e)

            Dim strModuleName As String = Me.ModuleControl.ModuleContext.Configuration.DesktopModule.ModuleName
            If Not strModuleName = Nothing Then
                strModuleName = strModuleName.Replace(".", "_")
            End If
            Me.Attributes.Add("class", strModuleName + "Content")
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' RenderContents renders the contents of the control to the output stream
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	12/15/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overrides Sub RenderContents(ByVal writer As System.Web.UI.HtmlTextWriter)
            If _IsCached Then
                'Render the cached control to the output stream
                MyBase.RenderContents(writer)
            Else
                If SupportsCaching() AndAlso IsViewMode() AndAlso Not IsAdminControl() Then
                    'Render to cache
                    Dim _cachedOutput As String = Null.NullString
                    Dim tempWriter As StringWriter = New StringWriter
                    _Control.RenderControl(New HtmlTextWriter(tempWriter))
                    _cachedOutput = tempWriter.ToString()

                    If Not String.IsNullOrEmpty(_cachedOutput) AndAlso (Not HttpContext.Current.Request.Browser.Crawler) Then
                        'Save content to cache
                        Dim moduleContent As Byte() = System.Text.Encoding.UTF8.GetBytes(_cachedOutput)
                        Dim cache As ModuleCache.ModuleCachingProvider = ModuleCache.ModuleCachingProvider.Instance(_ModuleConfiguration.GetEffectiveCacheMethod)
                        Dim varyBy As New System.Collections.Generic.SortedDictionary(Of String, String)
                        varyBy.Add("locale", System.Threading.Thread.CurrentThread.CurrentCulture.ToString)
                        Dim cacheKey As String = cache.GenerateCacheKey(_ModuleConfiguration.TabModuleID, varyBy)
                        cache.SetModule(_ModuleConfiguration.TabModuleID, cacheKey, New TimeSpan(0, 0, _ModuleConfiguration.CacheTime), moduleContent)
                    End If

                    'Render the cached content to Response
                    writer.Write(_cachedOutput)
                Else
                    'Render the control to Response
                    MyBase.RenderContents(writer)
                End If
            End If
        End Sub

#End Region

    End Class


End Namespace
