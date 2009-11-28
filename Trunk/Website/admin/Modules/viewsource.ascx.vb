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
Imports System.Web.UI.WebControls
Imports DotNetNuke
Imports System.Reflection
Imports System.IO
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Services.FileSystem
Imports DotNetNuke.Services.Localization
Imports DotNetNuke.Security.Permissions

Namespace DotNetNuke.Modules.Admin.Modules

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Partial Class ViewSource
        Inherits Entities.Modules.PortalModuleBase

        Protected ReadOnly Property CanEditSource() As Boolean
            Get
                Return Request.IsLocal
            End Get
        End Property

        Protected ReadOnly Property ModuleControlId() As Integer
            Get
                Dim _ModuleControlId As Integer = Null.NullInteger

                If Not (Request.QueryString("ctlid") Is Nothing) Then
                    _ModuleControlId = Int32.Parse(Request.QueryString("ctlid"))
                End If
                Return _ModuleControlId
            End Get
        End Property

        Private Sub BindFiles(ByVal controlSrc As String)
            cboFile.Items.Clear()
            cboFile.Items.Add(New ListItem(Localization.GetString("None_Specified"), "None"))
            cboFile.Items.Add(New ListItem("User Control", "UserControl"))

            Dim srcPhysicalPath As String = Server.MapPath(controlSrc)
            If File.Exists(srcPhysicalPath + ".vb") OrElse File.Exists(srcPhysicalPath + ".cs") Then
                cboFile.Items.Add(New ListItem("Code File", "CodeFile"))
            End If

            Dim fileName As String = Path.GetFileName(srcPhysicalPath)
            Dim folder As String = Path.GetDirectoryName(srcPhysicalPath)
            If File.Exists(folder + "\App_LocalResources\" + fileName + ".resx") Then
                cboFile.Items.Add(New ListItem("Resource File", "ResourceFile"))
            End If
        End Sub
        Private Function GetSourceFileName(ByVal controlSrc As String) As String
            Dim srcPhysicalPath As String = Server.MapPath(controlSrc)
            Dim srcFile As String = Null.NullString
            Select Case cboFile.SelectedValue
                Case "UserControl"
                    srcFile = srcPhysicalPath
                Case "CodeFile"
                    If File.Exists(srcPhysicalPath + ".vb") Then
                        srcFile = srcPhysicalPath + ".vb"
                    ElseIf File.Exists(srcPhysicalPath + ".cs") Then
                        srcFile = srcPhysicalPath + ".cs"
                    End If
                Case "ResourceFile"
                    Dim fileName As String = Path.GetFileName(srcPhysicalPath)
                    Dim folder As String = Path.GetDirectoryName(srcPhysicalPath)
                    srcFile = folder + "\App_LocalResources\" + fileName + ".resx"
            End Select
            Return srcFile
        End Function

        Private Sub DisplayFile()
            Dim objModuleControl As ModuleControlInfo = ModuleControlController.GetModuleControl(ModuleControlId)
            If Not objModuleControl Is Nothing Then
                Dim srcVirtualPath As String = objModuleControl.ControlSrc
                Dim srcFile As String = Null.NullString
                Dim displaySource As Boolean = Not (cboFile.SelectedValue = "None")

                If displaySource Then
                    srcFile = GetSourceFileName(srcVirtualPath)
                    lblSourceFile.Text = String.Format(Localization.GetString("SourceFile", Me.LocalResourceFile), srcFile)

                    Dim objStreamReader As StreamReader
                    objStreamReader = File.OpenText(srcFile)
                    txtSource.Text = objStreamReader.ReadToEnd
                    objStreamReader.Close()
                End If
                lblSourceFile.Visible = displaySource
                trSource.Visible = displaySource
            End If
        End Sub

        Private Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
            If Page.IsPostBack = False Then
                Dim objModuleControl As ModuleControlInfo = ModuleControlController.GetModuleControl(ModuleControlId)
                If Not objModuleControl Is Nothing Then
                    BindFiles(objModuleControl.ControlSrc)
                End If

                If Not Request.UrlReferrer Is Nothing Then
                    ViewState("UrlReferrer") = Convert.ToString(Request.UrlReferrer)
                Else
                    ViewState("UrlReferrer") = ""
                End If
            End If

            cmdUpdate.Visible = CanEditSource
            txtSource.Enabled = CanEditSource

        End Sub

        Protected Sub cboFile_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cboFile.SelectedIndexChanged
            DisplayFile()
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' cmdCancel_Click runs when the cancel Button is clicked
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub cmdCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdCancel.Click
            Try
                Response.Redirect(Convert.ToString(ViewState("UrlReferrer")), True)
            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' cmdSave_Click runs when the Save Stylesheet Linkbutton is clicked.  It saves
        ''' the edited Stylesheet
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	9/9/2004	Modified
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub cmdUpdate_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdUpdate.Click
            Try
                Dim objModuleControl As ModuleControlInfo = ModuleControlController.GetModuleControl(ModuleControlId)
                If Not objModuleControl Is Nothing Then
                    Dim srcVirtualPath As String = objModuleControl.ControlSrc
                    Dim srcPhysicalPath As String = GetSourceFileName(srcVirtualPath)

                    ' reset attributes
                    If File.Exists(srcPhysicalPath) Then
                        File.SetAttributes(srcPhysicalPath, FileAttributes.Normal)
                    End If

                    ' write file
                    Dim objStream As StreamWriter
                    objStream = File.CreateText(srcPhysicalPath)
                    objStream.WriteLine(txtSource.Text)
                    objStream.Close()
                End If

                Response.Redirect(Convert.ToString(ViewState("UrlReferrer")), True)
            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

    End Class

End Namespace
