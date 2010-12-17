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

Imports DotNetNuke.Entities.Modules
Imports System.IO
Imports DotNetNuke.Security.Permissions

Namespace DotNetNuke.UI.Modules
    Public Class WebFormModuleHost
        Inherits ModuleHostBase

        Private m_Control As Control
        Private m_Skin As DotNetNuke.UI.Skins.Skin

        Protected ReadOnly Property ModuleControl() As IModuleControl
            Get
                Return TryCast(m_Control, IModuleControl)
            End Get
        End Property

        Private Sub InjectMessageControl(ByVal container As Control)
            ' inject a message placeholder for common module messaging - UI.Skins.Skin.AddModuleMessage
            Dim MessagePlaceholder As New PlaceHolder
            MessagePlaceholder.ID = "MessagePlaceHolder"
            MessagePlaceholder.Visible = False
            container.Controls.Add(MessagePlaceholder)
        End Sub

        Private Sub InjectModuleContent(ByVal content As Control)
            If ModuleContext.Configuration.IsWebSlice And Not IsAdminControl() Then
                'Assign the class - hslice to the Drag-N-Drop Panel
                ModuleHost.CssClass = "hslice"
                Dim titleLabel As New Label
                titleLabel.CssClass = "entry-title Hidden"
                If Not String.IsNullOrEmpty(ModuleContext.Configuration.WebSliceTitle) Then
                    titleLabel.Text = ModuleContext.Configuration.WebSliceTitle
                Else
                    titleLabel.Text = ModuleContext.Configuration.ModuleTitle
                End If
                ModuleHost.Controls.Add(titleLabel)

                Dim websliceContainer As New Panel
                websliceContainer.CssClass = "entry-content"
                websliceContainer.Controls.Add(content)

                Dim expiry As New HtmlGenericControl
                expiry.TagName = "abbr"
                expiry.Attributes("class") = "endtime"
                If Not Null.IsNull(ModuleContext.Configuration.WebSliceExpiryDate) Then
                    expiry.Attributes("title") = ModuleContext.Configuration.WebSliceExpiryDate.ToString("o")
                    websliceContainer.Controls.Add(expiry)
                ElseIf ModuleContext.Configuration.EndDate < Date.MaxValue Then
                    expiry.Attributes("title") = ModuleContext.Configuration.EndDate.ToString("o")
                    websliceContainer.Controls.Add(expiry)
                End If

                Dim ttl As New HtmlGenericControl
                ttl.TagName = "abbr"
                ttl.Attributes("class") = "ttl"
                If ModuleContext.Configuration.WebSliceTTL > 0 Then
                    ttl.Attributes("title") = ModuleContext.Configuration.WebSliceTTL.ToString()
                    websliceContainer.Controls.Add(ttl)
                ElseIf ModuleContext.Configuration.CacheTime > 0 Then
                    ttl.Attributes("title") = (ModuleContext.Configuration.CacheTime \ 60).ToString()
                    websliceContainer.Controls.Add(ttl)
                End If

                ModuleHost.Controls.Add(websliceContainer)
            Else
                ModuleHost.Controls.Add(content)
            End If
        End Sub

        Private Sub LoadModuleControl()
            'Ultimately this is called by Skin.OnInit
            Try
                If DisplayContent() Then
                    If Not IsCached Then
                        ' load the control dynamically
                        m_Control = ControlUtilities.LoadControl(Of Control)(ModuleHost.Page, ModuleContext.Configuration.ModuleControl.ControlSrc)

                        ' set the control ID to the resource file name ( ie. controlname.ascx = controlname )
                        ' this is necessary for the Localization in PageBase
                        m_Control.ID = Path.GetFileNameWithoutExtension(ModuleContext.Configuration.ModuleControl.ControlSrc)
                    Else
                        m_Control = New CachedModuleControl(CachedContent)
                    End If
                Else       ' content placeholder
                    m_Control = New ModuleControlBase()
                End If

                'check for IMC
                m_Skin.Communicator.LoadCommunicator(m_Control)

                ' add module settings
                ModuleControl.ModuleContext.Configuration = ModuleContext.Configuration

            Catch exc As Threading.ThreadAbortException
                Threading.Thread.ResetAbort()
            Catch exc As Exception
                m_Control = New ModuleControlBase()

                '' add module settings
                ModuleControl.ModuleContext.Configuration = ModuleContext.Configuration

                If TabPermissionController.CanAdminPage() Then
                    ' only display the error to page administrators
                    ProcessModuleLoadException(m_Control, exc)
                End If
            End Try

        End Sub

        Private Sub LoadUpdatePanel()
            'register AJAX
            AJAX.RegisterScriptManager()

            'enable Partial Rendering
            Dim scriptManager As ScriptManager = AJAX.GetScriptManager(ModuleHost.Page)
            If scriptManager IsNot Nothing Then
                scriptManager.EnablePartialRendering = True
            End If

            'create update panel
            Dim updatePanel As New UpdatePanel()
            updatePanel.UpdateMode = UpdatePanelUpdateMode.Conditional
            updatePanel.ID = m_Control.ID & "_UP"

            'get update panel content template
            Dim objContentTemplateContainer As Control = updatePanel.ContentTemplateContainer

            ' inject a message placeholder for common module messaging - UI.Skins.Skin.AddModuleMessage
            InjectMessageControl(objContentTemplateContainer)

            'inject module into update panel content template
            objContentTemplateContainer.Controls.Add(m_Control)

            'inject the update panel into the panel
            InjectModuleContent(updatePanel)

            'create image for update progress control
            Dim objImage As System.Web.UI.WebControls.Image = New System.Web.UI.WebControls.Image()
            objImage.ImageUrl = "~/images/progressbar.gif"  'hardcoded
            objImage.AlternateText = "ProgressBar"

            'inject updateprogress into the panel
            Dim updateProgress As New UpdateProgress
            updateProgress.AssociatedUpdatePanelID = updatePanel.ID
            updateProgress.ID = updatePanel.ID + "_Prog"
            updateProgress.ProgressTemplate = New UI.WebControls.LiteralTemplate(objImage)
            ModuleHost.Controls.Add(updateProgress)
        End Sub

        Public Overrides Sub Initialize(ByVal configuration As ModuleInfo, ByVal host As ModuleHost)
            MyBase.Initialize(configuration, host)

            'Load Module Control (or cached control)
            LoadModuleControl()

            'Optionally Inject AJAX Update Panel
            If Not ModuleControl Is Nothing Then
                'if module is dynamically loaded and AJAX is installed and the control supports partial rendering (defined in ModuleControls table )
                If Not IsCached AndAlso ModuleContext.Configuration.ModuleControl.SupportsPartialRendering AndAlso AJAX.IsInstalled Then
                    LoadUpdatePanel()
                Else
                    ' inject a message placeholder for common module messaging - UI.Skins.Skin.AddModuleMessage
                    InjectMessageControl(ModuleHost)

                    'inject the module into the panel
                    InjectModuleContent(m_Control)
                End If
            End If

            'By now the control is in the Page's Controls Collection
            Dim profileModule As IProfileModule = TryCast(m_Control, IProfileModule)
            If profileModule IsNot Nothing Then
                'Find Container
                Dim _Container As DotNetNuke.UI.Containers.Container = ControlUtilities.FindParentControl(Of DotNetNuke.UI.Containers.Container)(m_Control)
                If _Container IsNot Nothing Then
                    _Container.Visible = profileModule.DisplayModule
                End If
            End If

        End Sub

        Public Overrides Sub Render(ByVal writer As HtmlTextWriter)
            Throw New NotImplementedException()
        End Sub

    End Class
End Namespace
