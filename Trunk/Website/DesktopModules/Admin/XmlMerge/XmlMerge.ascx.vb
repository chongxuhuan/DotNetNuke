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

Imports DotNetNuke
Imports System
Imports System.Web.UI
Imports System.Collections.Generic
Imports System.Reflection
Imports DotNetNuke.UI
Imports DotNetNuke.UI.Skins.Controls
Imports System.Linq
Imports System.Xml
Imports DotNetNuke.Application

Namespace DotNetNuke.Modules.XmlMerge

    Partial Class XmlMerge
        Inherits Entities.Modules.PortalModuleBase

#Region "Private Functions"

        Private Function IsValidDocType(ByVal contentType As String) As Boolean
            Select Case contentType
                Case "text/plain", "application/xml"
                    Return True
                Case Else
                    Return False
            End Select
        End Function

        Private Function IsValidXmlMergDocument(ByVal mergeDocText As String) As Boolean
            If String.IsNullOrEmpty(mergeDocText.Trim()) Then
                Return False
            End If
            'TODO: Add more checks here
            Return True
        End Function

        Private Sub BindConfigList()
            Dim files As String() = System.IO.Directory.GetFiles(DotNetNuke.Common.Globals.ApplicationMapPath, "*.config")
            Dim fileList = From file In files Select System.IO.Path.GetFileName(file)
            ddlConfig.DataSource = fileList
            ddlConfig.DataBind()
            ddlConfig.SelectedValue = "web.config"
        End Sub

        Private Sub ValidateSuperUser()
        ' Verify that the current user has access to access this page
            If Not UserInfo.IsSuperUser Then
                Response.Redirect(NavigateURL("Access Denied"), True)
            End If
        End Sub
#End Region

#Region "Event Handlers"

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            ValidateSuperUser()

            If Not Page.IsPostBack Then
                BindConfigList()
            End If
            If ddlConfig.SelectedValue.ToLowerInvariant = "web.config" Then
                DotNetNuke.UI.Utilities.ClientAPI.AddButtonConfirm(cmdSave, Localization.GetString("SaveWarning", LocalResourceFile))
            End If

        End Sub

        Protected Sub cmdExecute_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdExecute.Click
            ValidateSuperUser()
            If IsValidXmlMergDocument(txtScript.Text) Then
                Try
                    Dim doc As New System.Xml.XmlDocument()
                    doc.LoadXml(txtScript.Text)
                    Dim app As DotNetNuke.Application.Application = DotNetNukeContext.Current.Application
                    Dim merge As New DotNetNuke.Services.Installer.XmlMerge(doc, FormatVersion(app.Version), app.Description)
                    merge.UpdateConfigs()
                Catch ex As Exception
                    Skins.Skin.AddModuleMessage(Me, Localization.GetString("ERROR_Merge", Me.LocalResourceFile), ModuleMessage.ModuleMessageType.RedError)
                    LogException(ex)
                End Try
            End If
        End Sub

        Protected Sub cmdUpload_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdUpload.Click
            ValidateSuperUser()
            If IsValidDocType(uplScript.PostedFile.ContentType) Then
                Dim scriptFile As New System.IO.StreamReader(uplScript.PostedFile.InputStream)
                txtScript.Text = scriptFile.ReadToEnd
            End If
        End Sub

        Protected Sub cmdLoad_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdLoad.Click
            ValidateSuperUser()
            Dim configDoc As System.Xml.XmlDocument = Config.Load(ddlConfig.SelectedValue)
            Using txtWriter As New System.IO.StringWriter()
                Using writer As New XmlTextWriter(txtWriter)
                    writer.Formatting = Formatting.Indented
                    configDoc.WriteTo(writer)
                End Using
                txtConfiguration.Text = txtWriter.ToString
            End Using

        End Sub

        Protected Sub cmdSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdSave.Click
            ValidateSuperUser()
            Dim configDoc As New System.Xml.XmlDocument
            Try
                configDoc.LoadXml(txtConfiguration.Text)
            Catch ex As Exception
                Skin.AddModuleMessage(Me, String.Format(Localization.GetString("ERROR_ConfigurationFormat", LocalResourceFile), ex.Message), ModuleMessage.ModuleMessageType.RedError)
                Exit Sub
            End Try

            Config.Save(configDoc, ddlConfig.SelectedValue)
        End Sub

#End Region

    End Class

End Namespace
