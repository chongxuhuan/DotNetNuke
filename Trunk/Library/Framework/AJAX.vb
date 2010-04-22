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

Imports System.Web.Compilation
Imports System.Reflection
Imports System.Xml
Imports System.Xml.XPath
Imports DotNetNuke.Entities.Host

Namespace DotNetNuke.Framework

    Public Class AJAX

#Region "Public Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' AddScriptManager is used internally by the framework to add a ScriptManager control to the page
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Sub AddScriptManager(ByVal objPage As Page)
            If GetScriptManager(objPage) Is Nothing Then
                Using objScriptManager As ScriptManager = New ScriptManager() With {.ID = "ScriptManager", .EnableScriptGlobalization = True}
                    If objPage.Form IsNot Nothing Then
                        objPage.Form.Controls.AddAt(0, objScriptManager)
                        If HttpContext.Current.Items("System.Web.UI.ScriptManager") Is Nothing Then
                            HttpContext.Current.Items.Add("System.Web.UI.ScriptManager", True)
                        End If
                    End If
                End Using
            End If
        End Sub

        Public Shared Function GetScriptManager(ByVal objPage As Page) As ScriptManager
            Return TryCast(objPage.FindControl("ScriptManager"), ScriptManager)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' IsEnabled can be used to determine if AJAX has been enabled already as we
        ''' only need one Script Manager per page.
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function IsEnabled() As Boolean
            If HttpContext.Current.Items("System.Web.UI.ScriptManager") Is Nothing Then
                Return False
            Else
                Return CType(HttpContext.Current.Items("System.Web.UI.ScriptManager"), Boolean)
            End If
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' IsInstalled can be used to determine if AJAX is installed on the server
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function IsInstalled() As Boolean
            Return True
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Allows a control to be excluded from UpdatePanel async callback
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Sub RegisterPostBackControl(ByVal objControl As Control)
            Dim objScriptManager As ScriptManager = GetScriptManager(objControl.Page)
            If objScriptManager IsNot Nothing Then
                objScriptManager.RegisterPostBackControl(objControl)
            End If
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' RegisterScriptManager must be used by developers to instruct the framework that AJAX is required on the page
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Sub RegisterScriptManager()
            If Not IsEnabled() Then
                HttpContext.Current.Items.Add("System.Web.UI.ScriptManager", True)
            End If
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' RemoveScriptManager will remove the ScriptManager control during Page Render if the RegisterScriptManager has not been called
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Sub RemoveScriptManager(ByVal objPage As Page)
            If IsEnabled() = False Then
                Dim objControl As Control = objPage.FindControl("ScriptManager")
                If Not objControl Is Nothing Then
                    objPage.Form.Controls.Remove(objControl)
                End If
            End If
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Wraps a control in an update panel
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function WrapUpdatePanelControl(ByVal objControl As Control, ByVal blnIncludeProgress As Boolean) As Control
            Dim updatePanel As New UpdatePanel()
            updatePanel.ID = objControl.ID & "_UP"
            updatePanel.UpdateMode = UpdatePanelUpdateMode.Conditional

            Dim objContentTemplateContainer As Control = updatePanel.ContentTemplateContainer

            For i As Integer = 0 To objControl.Parent.Controls.Count - 1    'find offset of original control
                If objControl.Parent.Controls(i).ID = objControl.ID Then    'if ID matches
                    objControl.Parent.Controls.AddAt(i, updatePanel)       'insert update panel in that position
                    objContentTemplateContainer.Controls.Add(objControl)    'inject passed in control into update panel
                    Exit For
                End If
            Next

            If blnIncludeProgress Then
                'create image for update progress control
                Dim objImage As System.Web.UI.WebControls.Image = New System.Web.UI.WebControls.Image()
                objImage.ImageUrl = "~/images/progressbar.gif"  'hardcoded
                objImage.AlternateText = "ProgressBar"

                Dim updateProgress As New UpdateProgress
                updateProgress.AssociatedUpdatePanelID = updatePanel.ID
                updateProgress.ID = updatePanel.ID + "_Prog"
                updateProgress.ProgressTemplate = New UI.WebControls.LiteralTemplate(objImage)

                objContentTemplateContainer.Controls.Add(updateProgress)
            End If

            Return updatePanel
        End Function

#End Region

#Region "Obsolete Methods"

        <Obsolete("Deprecated in DNN 5.4, Developers can work directly with the UpdatePanel")> _
        Public Shared Function ContentTemplateContainerControl(ByVal objUpdatePanel As Object) As Control
            Return TryCast(objUpdatePanel, UpdatePanel).ContentTemplateContainer
        End Function

        <Obsolete("Deprecated in DNN 5.4, MS AJax is now required for DotNetNuke 5.0.  Develoers can create the control directly")> _
        Public Shared Function CreateUpdatePanelControl() As Control
            Dim updatePanel As New UpdatePanel()
            updatePanel.UpdateMode = UpdatePanelUpdateMode.Conditional
            Return updatePanel
        End Function

        <Obsolete("Deprecated in DNN 5.4, MS AJax is now required for DotNetNuke 5.0. Developers can work directly with the UpdateProgress")> _
        Public Shared Function CreateUpdateProgressControl(ByVal AssociatedUpdatePanelID As String) As Control
            Dim updateProgress As New UpdateProgress
            updateProgress.ID = AssociatedUpdatePanelID + "_Prog"
            updateProgress.AssociatedUpdatePanelID = AssociatedUpdatePanelID
            Return updateProgress
        End Function

        <Obsolete("Deprecated in DNN 5.4, MS AJax is now required for DotNetNuke 5.0. Developers can work directly with the UpdateProgress")> _
        Public Shared Function CreateUpdateProgressControl(ByVal AssociatedUpdatePanelID As String, ByVal ProgressHTML As String) As Control
            Dim updateProgress As New UpdateProgress
            updateProgress.ID = AssociatedUpdatePanelID + "_Prog"
            updateProgress.AssociatedUpdatePanelID = AssociatedUpdatePanelID
            updateProgress.ProgressTemplate = New UI.WebControls.LiteralTemplate(ProgressHTML)
            Return updateProgress
        End Function

        <Obsolete("Deprecated in DNN 5.4, MS AJax is now required for DotNetNuke 5.0. Developers can work directly with the UpdateProgress")> _
        Public Shared Function CreateUpdateProgressControl(ByVal AssociatedUpdatePanelID As String, ByVal ProgressControl As Control) As Control
            Dim updateProgress As New UpdateProgress
            updateProgress.ID = AssociatedUpdatePanelID + "_Prog"
            updateProgress.AssociatedUpdatePanelID = AssociatedUpdatePanelID
            updateProgress.ProgressTemplate = New UI.WebControls.LiteralTemplate(ProgressControl)
            Return updateProgress
        End Function

        <Obsolete("Deprecated in DNN 5.0, MS AJax is now required for DotNetNuke 5.0 and above - value no longer read from Host.EnableAjax")> _
        Public Shared Function IsHostEnabled() As Boolean
            Return True
        End Function

        <Obsolete("Deprecated in DNN 5.4, Replaced by GetScriptManager")> _
        Public Shared Function ScriptManagerControl(ByVal objPage As Page) As Control
            Return objPage.FindControl("ScriptManager")
        End Function

        <Obsolete("Deprecated in DNN 5.4, Developers can work directly with the ScriptManager")> _
        Public Shared Sub SetScriptManagerProperty(ByVal objPage As Page, ByVal PropertyName As String, ByVal Args() As Object)
            Dim scriptManager As ScriptManager = GetScriptManager(objPage)
            If scriptManager IsNot Nothing Then
                Reflection.SetProperty(scriptManager.GetType(), PropertyName, scriptManager, Args)
            End If
        End Sub

#End Region

    End Class
End Namespace