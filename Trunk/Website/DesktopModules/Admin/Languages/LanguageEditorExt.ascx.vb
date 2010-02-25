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
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.IO
Imports System.Xml
Imports DotNetNuke.UI.WebControls
Imports DotNetNuke

Namespace DotNetNuke.Modules.Admin.Languages

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Manages translations for Resource files
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[vmasanas]	10/04/2004  Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Partial Class LanguageEditorExt
        Inherits DotNetNuke.Entities.Modules.PortalModuleBase

#Region "Private Members"
        Private resfile As String
        Private locale As String
        Private mode As String
        Private highlight As String
#End Region

#Region "Protected Properties"

        Protected ReadOnly Property ReturnUrl() As String
            Get
                Return NavigateURL("", "Locale=" & locale, "ResourceFile=" & QueryStringEncode(resfile), "Mode=" & mode, "Highlight=" & highlight)
            End Get
        End Property

#End Region

#Region "Private Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Loads resources from file 
        ''' </summary>
        ''' <param name="mode">Active editor mode</param>
        ''' <param name="type">Resource being loaded (edit or default)</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Depending on the editor mode, resources will be overrided using default DNN schema.
        ''' "Edit" resources will only load selected file.
        ''' When loading "Default" resources (to be used on the editor as helpers) fallback resource
        ''' chain will be used in order for the editor to be able to correctly see what 
        ''' is the current default value for the any key. This process depends on the current active
        ''' editor mode:
        ''' - System: when editing system base resources on en-US needs to be loaded
        ''' - Host: base en-US, and base locale especific resource
        ''' - Portal: base en-US, host override for en-US, base locale especific resource, and host override 
        ''' for locale
        ''' </remarks>
        ''' <history>
        ''' 	[vmasanas]	25/03/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Function LoadFile(ByVal mode As String, ByVal type As String) As String
            Dim file As String = ""
            Dim t As String = ""

            Select Case type
                Case "Edit"
                    ' Only load resources from the file being edited
                    file = ResourceFile(locale, mode)
                    Dim temp As String = LoadResource(file)
                    If Not temp Is Nothing Then
                        t = temp
                    End If
                Case "Default"
                    ' Load system default
                    file = ResourceFile(Localization.SystemLocale, "System")
                    t = LoadResource(file)

                    Select Case mode
                        Case "Host"
                            If locale <> Localization.SystemLocale Then
                                ' Load base file for selected locale
                                file = ResourceFile(locale, "System")
                                Dim temp As String = LoadResource(file)
                                If Not temp Is Nothing Then
                                    t = temp
                                End If
                            End If
                        Case "Portal"
                            'Load host override for default locale
                            file = ResourceFile(Localization.SystemLocale, "Host")
                            Dim temp As String = LoadResource(file)
                            If Not temp Is Nothing Then
                                t = temp
                            End If

                            If locale <> Localization.SystemLocale Then
                                ' Load base file for locale
                                file = ResourceFile(locale, "System")
                                temp = LoadResource(file)
                                If Not temp Is Nothing Then
                                    t = temp
                                End If

                                'Load host override for selected locale
                                file = ResourceFile(locale, "Host")
                                temp = LoadResource(file)
                                If Not temp Is Nothing Then
                                    t = temp
                                End If
                            End If
                    End Select

            End Select


            Return t

        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Loads resource from file
        ''' </summary>
        ''' <param name="filepath">Resources file</param>
        ''' <returns>Resource value</returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[vmasanas]	25/03/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Function LoadResource(ByVal filepath As String) As String
            Dim d As New XmlDocument
            Dim xmlLoaded As Boolean = False
            Dim ret As String = Nothing
            Try
                d.Load(filepath)
                xmlLoaded = True
            Catch    'exc As Exception
                xmlLoaded = False
            End Try
            If xmlLoaded Then
                Dim node As XmlNode
                node = d.SelectSingleNode("//root/data[@name='" + lblName.Text + "']/value")
                If Not node Is Nothing Then
                    ret = node.InnerXml
                End If
            End If
            Return ret
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Returns the resource file name for a given resource and language
        ''' </summary>
        ''' <param name="mode">Identifies the resource being searched (System, Host, Portal)</param>
        ''' <returns>Localized File Name</returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[vmasanas]	04/10/2004	Created
        ''' 	[vmasanas]	25/03/2006	Modified to support new host resources and incremental saving
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Function ResourceFile(ByVal language As String, ByVal mode As String) As String
            Return Localization.GetResourceFileName(Server.MapPath("~\" + resfile), language, mode, PortalId)
        End Function

#End Region

#Region "Event Handlers"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Loads resource file and default data
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[vmasanas]	07/10/2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Dim resDoc As New XmlDocument
            Dim defDoc As New XmlDocument

            Try
                Dim defaultValue As String
                Dim editValue As String

                resfile = QueryStringDecode(Request.QueryString("resourcefile"))
                locale = Request.QueryString("locale")
                mode = Request.QueryString("mode")
                highlight = Request.QueryString("highlight")

                lblName.Text = Request.QueryString("name")
                lblFile.Text = ResourceFile(locale, mode).Replace(ApplicationMapPath, "").Replace("\", "/")

                If Not Page.IsPostBack Then
                    defaultValue = LoadFile(mode, "Default")
                    editValue = LoadFile(mode, "Edit")
                    If String.IsNullOrEmpty(editValue) Then
                        editValue = defaultValue
                    End If

                    teContent.Text = editValue
                    lblDefault.Text = Server.HtmlDecode(defaultValue)
                End If
            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Returns to language editor control
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[vmasanas]	04/10/2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub cmdCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdCancel.Click
            Try
                Response.Redirect(ReturnUrl, True)
            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Saves the translation to the resource file
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[vmasanas]	07/10/2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub cmdUpdate_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdUpdate.Click
            Dim node, nodeData, parent As XmlNode
            Dim attr As XmlAttribute
            Dim resDoc As New XmlDocument
            Dim filename As String
            Dim IsNewFile As Boolean = False

            Try
                If teContent.Text = "" Then
                    UI.Skins.Skin.AddModuleMessage(Me, Localization.GetString("RequiredField.ErrorMessage", Me.LocalResourceFile), UI.Skins.Controls.ModuleMessage.ModuleMessageType.YellowWarning)
                    Exit Sub
                End If

                filename = ResourceFile(locale, mode)
                If Not File.Exists(filename) Then
                    ' load system default
                    resDoc.Load(ResourceFile(Localization.SystemLocale, "System"))
                    IsNewFile = True
                Else
                    resDoc.Load(filename)
                End If

                Select Case mode
                    Case "System"
                        node = resDoc.SelectSingleNode("//root/data[@name='" + lblName.Text + "']/value")
                        If node Is Nothing Then
                            ' missing entry
                            nodeData = resDoc.CreateElement("data")
                            attr = resDoc.CreateAttribute("name")
                            attr.Value = lblName.Text
                            nodeData.Attributes.Append(attr)
                            resDoc.SelectSingleNode("//root").AppendChild(nodeData)

                            node = nodeData.AppendChild(resDoc.CreateElement("value"))
                        End If
                        node.InnerXml = teContent.Text

                        resDoc.Save(filename)
                    Case "Host", "Portal"
                        If IsNewFile Then
                            If teContent.Text <> lblDefault.Text Then
                                For Each n As XmlNode In resDoc.SelectNodes("//root/data")
                                    parent = n.ParentNode
                                    parent.RemoveChild(n)
                                Next
                                nodeData = resDoc.CreateElement("data")
                                attr = resDoc.CreateAttribute("name")
                                attr.Value = lblName.Text
                                nodeData.Attributes.Append(attr)
                                resDoc.SelectSingleNode("//root").AppendChild(nodeData)

                                node = nodeData.AppendChild(resDoc.CreateElement("value"))
                                node.InnerXml = teContent.Text

                                resDoc.Save(filename)
                            End If
                        Else
                            node = resDoc.SelectSingleNode("//root/data[@name='" + lblName.Text + "']/value")
                            If teContent.Text <> lblDefault.Text Then
                                If node Is Nothing Then
                                    ' missing entry
                                    nodeData = resDoc.CreateElement("data")
                                    attr = resDoc.CreateAttribute("name")
                                    attr.Value = lblName.Text
                                    nodeData.Attributes.Append(attr)
                                    resDoc.SelectSingleNode("//root").AppendChild(nodeData)

                                    node = nodeData.AppendChild(resDoc.CreateElement("value"))
                                End If
                                node.InnerXml = teContent.Text
                            ElseIf Not node Is Nothing Then
                                ' remove item = default
                                resDoc.SelectSingleNode("//root").RemoveChild(node.ParentNode)
                            End If
                            If resDoc.SelectNodes("//root/data").Count > 0 Then
                                ' there's something to save
                                resDoc.Save(filename)
                            Else
                                ' nothing to be saved, if file exists delete
                                If File.Exists(filename) Then
                                    File.Delete(filename)
                                End If
                            End If
                        End If
                End Select

                Response.Redirect(ReturnUrl, True)
            Catch exc As Exception    'Module failed to load
                UI.Skins.Skin.AddModuleMessage(Me, Localization.GetString("Save.ErrorMessage", Me.LocalResourceFile), UI.Skins.Controls.ModuleMessage.ModuleMessageType.YellowWarning)
            End Try
        End Sub

#End Region

    End Class

End Namespace
