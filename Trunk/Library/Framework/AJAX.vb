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

Imports System.Web.Compilation
Imports System.Reflection
Imports System.Xml
Imports System.Xml.XPath
Imports DotNetNuke.Entities.Host

Namespace DotNetNuke.Framework

    Public Class AJAX

        Private Shared m_IsInstalled As Boolean = Null.NullBoolean
        Private Shared m_IsInstalledLoaded As Boolean = Null.NullBoolean

        Private Shared m_Initialized As Boolean = Null.NullBoolean
        Private Shared m_ScriptManagerType As Type
        Private Shared m_UpdatePanelType As Type
        Private Shared m_UpdateProgressType As Type
        Private Shared m_UpdatePanelUpdateModeType As Type


#Region "Private Methods"

        Private Shared Function ScriptManagerType() As Type
            If m_ScriptManagerType Is Nothing Then
                If Not m_Initialized Then
                    m_ScriptManagerType = Reflection.CreateType("System.Web.UI.ScriptManager", True)
                End If
                m_Initialized = True
            End If
            Return m_ScriptManagerType
        End Function

        Private Shared Function UpdatePanelType() As Type
            If m_UpdatePanelType Is Nothing Then
                m_UpdatePanelType = Reflection.CreateType("System.Web.UI.UpdatePanel")
            End If
            Return m_UpdatePanelType
        End Function

        Private Shared Function UpdateProgressType() As Type
            If m_UpdateProgressType Is Nothing Then
                m_UpdateProgressType = Reflection.CreateType("System.Web.UI.UpdateProgress")
            End If
            Return m_UpdateProgressType
        End Function

        Private Shared Function UpdatePanelUpdateModeType() As Type
            If m_UpdatePanelUpdateModeType Is Nothing Then
                m_UpdatePanelUpdateModeType = Reflection.CreateType("System.Web.UI.UpdatePanelUpdateMode")
            End If
            Return m_UpdatePanelUpdateModeType
        End Function

        Private Shared Sub SetScriptManagerProperty(ByVal objControl As Control, ByVal PropertyName As String, ByVal Args() As Object)
            If Not ScriptManagerType() Is Nothing Then
                Reflection.SetProperty(ScriptManagerType, PropertyName, objControl, Args)
            End If
        End Sub

#End Region

#Region "Public Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' IsHostEnabled is used to determine whether the Host user has enabled AJAX.
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <Obsolete("MS AJax is now required for DotNetNuke 5.0 and above - value no longer read from Host.EnableAjax")> _
        Public Shared Function IsHostEnabled() As Boolean
            Return True
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
            If Not m_IsInstalledLoaded Then
                'First check that the script module is installed
                Dim configDoc As XmlDocument = Config.Load()
                Dim moduleNavigator As XPathNavigator = configDoc.CreateNavigator.SelectSingleNode("/configuration/system.web/httpModules/add[@name='ScriptModule']")
                If moduleNavigator Is Nothing Then
                    'Check that user hasn't used a <location> node
                    moduleNavigator = configDoc.CreateNavigator.SelectSingleNode("/configuration/location/system.web/httpModules/add[@name='ScriptModule']")
                    If moduleNavigator Is Nothing Then
                        m_IsInstalled = False
                        m_IsInstalledLoaded = True
                    End If
                End If

                If Not m_IsInstalledLoaded Then
                    m_IsInstalled = Not ScriptManagerType() Is Nothing
                    m_IsInstalledLoaded = True
                End If
            End If
            Return m_IsInstalled
        End Function

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
        ''' ScriptManagerControl provides a reference to the ScriptManager control on the page
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function ScriptManagerControl(ByVal objPage As Page) As Control
            Return objPage.FindControl("ScriptManager")
        End Function

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
            If IsInstalled() AndAlso objPage.FindControl("ScriptManager") Is Nothing Then
                Dim objScriptManager As Object = Reflection.CreateInstance(ScriptManagerType)
                Dim objControl As Control = CType(objScriptManager, Control)
                objControl.ID = "ScriptManager"

                Dim args() As [Object] = {True}
                SetScriptManagerProperty(objControl, "EnableScriptGlobalization", args)

                Try
                    objPage.Form.Controls.AddAt(0, objControl)

                    If HttpContext.Current.Items("System.Web.UI.ScriptManager") Is Nothing Then
                        HttpContext.Current.Items.Add("System.Web.UI.ScriptManager", True)
                    End If
                Catch ex As Exception
                    ' ScriptManager can not be added to a page if running in Medium Trust and AJAX not installed in GAC
                    m_ScriptManagerType = Nothing
                    m_Initialized = False
                End Try
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
            If IsInstalled() = False Or IsEnabled() = False Then
                Dim objControl As Control = objPage.FindControl("ScriptManager")
                If Not objControl Is Nothing Then
                    objPage.Form.Controls.Remove(objControl)
                End If
            End If
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' SetScriptManagerProperty uses reflection to set properties on the dynamically generated ScriptManager control
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Sub SetScriptManagerProperty(ByVal objPage As Page, ByVal PropertyName As String, ByVal Args() As Object)
            If Not ScriptManagerControl(objPage) Is Nothing Then
                SetScriptManagerProperty(ScriptManagerControl(objPage), PropertyName, Args)
            End If
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' UpdatePanelControl dynamically creates an instance of an UpdatePanel control
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function CreateUpdatePanelControl() As Control
            Dim objCtl As Control = CType(Reflection.CreateInstance(UpdatePanelType), Control)
            Reflection.SetProperty(UpdatePanelType, "UpdateMode", objCtl, New Object() {System.Enum.Parse(UpdatePanelUpdateModeType, "1")})  'Conditional
            Return objCtl
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
            If Not ScriptManagerType() Is Nothing Then
                If Not ScriptManagerControl(objControl.Page) Is Nothing Then
                    Reflection.InvokeMethod(ScriptManagerType, "RegisterPostBackControl", ScriptManagerControl(objControl.Page), New Object() {objControl})
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
            Dim objPanel As Control = CreateUpdatePanelControl()
            objPanel.ID = objControl.ID & "_UP"
            Dim objContentTemplateContainer As Control = AJAX.ContentTemplateContainerControl(objPanel)

            For i As Integer = 0 To objControl.Parent.Controls.Count - 1    'find offset of original control
                If objControl.Parent.Controls(i).ID = objControl.ID Then    'if ID matches
                    objControl.Parent.Controls.AddAt(i, objPanel)       'insert update panel in that position
                    objContentTemplateContainer.Controls.Add(objControl)    'inject passed in control into update panel
                    Exit For
                End If
            Next

            If blnIncludeProgress Then
                'create image for update progress control
                Dim objImage As System.Web.UI.WebControls.Image = New System.Web.UI.WebControls.Image()
                objImage.ImageUrl = "~/images/progressbar.gif"  'hardcoded
                objImage.AlternateText = "ProgressBar"
                objContentTemplateContainer.Controls.Add(AJAX.CreateUpdateProgressControl(objPanel.ID, objImage))
            End If

            Return objPanel
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' UpdateProgressControl dynamically creates an instance of an UpdateProgress control
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function CreateUpdateProgressControl(ByVal AssociatedUpdatePanelID As String) As Control
            Dim objCtl As Control = CType(Reflection.CreateInstance(UpdateProgressType), Control)
            objCtl.ID = AssociatedUpdatePanelID + "_Prog"
            Reflection.SetProperty(UpdateProgressType, "AssociatedUpdatePanelID", objCtl, New Object() {AssociatedUpdatePanelID})
            Return objCtl
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' UpdateProgressControl dynamically creates an instance of an UpdateProgress control
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function CreateUpdateProgressControl(ByVal AssociatedUpdatePanelID As String, ByVal ProgressHTML As String) As Control
            Dim objCtl As Control = CreateUpdateProgressControl(AssociatedUpdatePanelID)
            Reflection.SetProperty(UpdateProgressType, "ProgressTemplate", objCtl, New Object() {New UI.WebControls.LiteralTemplate(ProgressHTML)})
            Return objCtl
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' UpdateProgressControl dynamically creates an instance of an UpdateProgress control
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function CreateUpdateProgressControl(ByVal AssociatedUpdatePanelID As String, ByVal ProgressControl As Control) As Control
            Dim objCtl As Control = CreateUpdateProgressControl(AssociatedUpdatePanelID)
            Reflection.SetProperty(UpdateProgressType, "ProgressTemplate", objCtl, New Object() {New UI.WebControls.LiteralTemplate(ProgressControl)})
            Return objCtl
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' ContentTemplateContainerControl gets a reference to the ContentTemplateContainer control within an UpdatePanel
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function ContentTemplateContainerControl(ByVal objUpdatePanel As Object) As Control
            Return CType(Reflection.GetProperty(UpdatePanelType, "ContentTemplateContainer", objUpdatePanel), Control)
        End Function

#End Region

    End Class
End Namespace