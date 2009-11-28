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
Imports System.IO
Imports System.XML
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.UI.Skins
Imports System.Collections.Generic

Namespace DotNetNuke.Modules.Admin.Skins

    Partial Class Attributes

        Inherits DotNetNuke.Entities.Modules.PortalModuleBase

#Region "Event Handlers"

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Try
                If Not Page.IsPostBack Then
                    LoadSkins()
                    LoadContainers()
                End If
            Catch exc As Exception
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        Private Sub cboSkins_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cboSkins.SelectedIndexChanged
            ShowSkins()
        End Sub

        Private Sub cboContainers_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cboContainers.SelectedIndexChanged
            ShowContainers()
        End Sub

        Protected Sub cboFiles_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cboFiles.SelectedIndexChanged
            LoadTokens()
        End Sub

        Protected Sub cboTokens_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cboTokens.SelectedIndexChanged
            LoadSettings()
        End Sub

        Protected Sub cboSettings_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cboSettings.SelectedIndexChanged
            LoadValues()
        End Sub

        Private Sub cmdUpdate_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdUpdate.Click
            Try
                If Page.IsValid Then
                    UpdateSkin()
                End If
            Catch exc As Exception
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

#End Region

#Region "Private Methods"

        Private Sub ShowSkins()
            Dim strSkinPath As String = ApplicationMapPath.ToLower + cboSkins.SelectedItem.Value
            cboContainers.ClearSelection()

            If cboSkins.SelectedIndex > 0 Then
                LoadFiles(strSkinPath)
            End If
        End Sub

        Private Sub ShowContainers()
            Dim strContainerPath As String = ApplicationMapPath.ToLower + cboContainers.SelectedItem.Value
            cboSkins.ClearSelection()

            If cboContainers.SelectedIndex > 0 Then
                LoadFiles(strContainerPath)
            End If
        End Sub

        Private Sub LoadSkins()
            Dim strRoot As String
            Dim strFolder As String
            Dim arrFolders As String()
            Dim strName As String
            Dim strSkin As String

            cboSkins.Items.Clear()
            cboSkins.Items.Add("<" & Services.Localization.Localization.GetString("Not_Specified") & ">")

            ' load host skins
            If UserInfo.IsSuperUser Then
                strRoot = Request.MapPath(Common.Globals.HostPath & SkinController.RootSkin)
                If Directory.Exists(strRoot) Then
                    arrFolders = Directory.GetDirectories(strRoot)
                    For Each strFolder In arrFolders
                        strName = Mid(strFolder, InStrRev(strFolder, "\") + 1)
                        strSkin = strFolder.Replace(ApplicationMapPath, "")
                        If strName <> "_default" Then
                            cboSkins.Items.Add(New ListItem(strName, strSkin.ToLower))
                        End If
                    Next
                End If
            End If

            ' load portal skins
            strRoot = PortalSettings.HomeDirectoryMapPath & SkinController.RootSkin
            If Directory.Exists(strRoot) Then
                arrFolders = Directory.GetDirectories(strRoot)
                For Each strFolder In arrFolders
                    strName = Mid(strFolder, InStrRev(strFolder, "\") + 1)
                    strSkin = strFolder.Replace(ApplicationMapPath, "")
                    cboSkins.Items.Add(New ListItem(strName, strSkin.ToLower))
                Next
            End If
        End Sub

        Private Sub LoadContainers()
            Dim strRoot As String
            Dim strFolder As String
            Dim arrFolders As String()
            Dim strName As String
            Dim strSkin As String

            cboContainers.Items.Clear()
            cboContainers.Items.Add("<" & Services.Localization.Localization.GetString("Not_Specified") & ">")

            ' load host containers
            If UserInfo.IsSuperUser Then
                strRoot = Request.MapPath(Common.Globals.HostPath & SkinController.RootContainer)
                If Directory.Exists(strRoot) Then
                    arrFolders = Directory.GetDirectories(strRoot)
                    For Each strFolder In arrFolders
                        strName = Mid(strFolder, InStrRev(strFolder, "\") + 1)
                        strSkin = strFolder.Replace(ApplicationMapPath, "")
                        If strName <> "_default" Then
                            cboContainers.Items.Add(New ListItem(strName, strSkin.ToLower))
                        End If
                    Next
                End If
            End If

            ' load portal containers
            strRoot = PortalSettings.HomeDirectoryMapPath & SkinController.RootContainer
            If Directory.Exists(strRoot) Then
                arrFolders = Directory.GetDirectories(strRoot)
                For Each strFolder In arrFolders
                    strName = Mid(strFolder, InStrRev(strFolder, "\") + 1)
                    strSkin = strFolder.Replace(ApplicationMapPath, "")
                    cboContainers.Items.Add(New ListItem(strName, strSkin.ToLower))
                Next
            End If
        End Sub

        Private Sub LoadFiles(ByVal strFolderPath As String)
            cboFiles.Items.Clear()

            Dim strFile As String
            Dim arrFiles As String()

            If Directory.Exists(strFolderPath) Then
                arrFiles = Directory.GetFiles(strFolderPath, "*.ascx")
                For Each strFile In arrFiles
                    cboFiles.Items.Add(New ListItem(Path.GetFileNameWithoutExtension(strFile), strFile))
                Next
            End If

            cboFiles.Items.Insert(0, "<" & Services.Localization.Localization.GetString("Not_Specified") & ">")
        End Sub

        Private Sub LoadTokens()

            cboTokens.DataSource = SkinControlController.GetSkinControls().Values
            cboTokens.DataBind()

            cboTokens.Items.Insert(0, "<" & Services.Localization.Localization.GetString("Not_Specified") & ">")

        End Sub

        Private Sub LoadSettings()

            cboSettings.Items.Clear()

            Dim strFile As String = ApplicationMapPath & "\" & cboTokens.SelectedItem.Value.ToLower.Replace("/", "\").Replace(".ascx", ".xml")
            If File.Exists(strFile) Then
                Try
                    Dim xmlDoc As New XmlDocument
                    xmlDoc.Load(strFile)
                    Dim xmlSetting As XmlNode
                    For Each xmlSetting In xmlDoc.SelectNodes("//Settings/Setting")
                        cboSettings.Items.Add(New ListItem(xmlSetting.SelectSingleNode("Name").InnerText, xmlSetting.SelectSingleNode("Name").InnerText))
                    Next
                Catch
                    UI.Skins.Skin.AddModuleMessage(Me, "Error Loading Settings File For Object", UI.Skins.Controls.ModuleMessage.ModuleMessageType.RedError)
                End Try
            Else
                UI.Skins.Skin.AddModuleMessage(Me, "Object Selected Does Not Have Settings Defined", UI.Skins.Controls.ModuleMessage.ModuleMessageType.YellowWarning)
            End If

            cboSettings.Items.Insert(0, "<" & Services.Localization.Localization.GetString("Not_Specified") & ">")

        End Sub

        Private Sub LoadValues()

            cboValue.Items.Clear()
            txtValue.Text = ""

            Dim strFile As String = ApplicationMapPath & "\" & cboTokens.SelectedItem.Value.ToLower.Replace("/", "\").Replace(".ascx", ".xml")
            If File.Exists(strFile) Then
                Try
                    Dim xmlDoc As New XmlDocument
                    xmlDoc.Load(strFile)
                    Dim xmlSetting As XmlNode
                    For Each xmlSetting In xmlDoc.SelectNodes("//Settings/Setting")
                        If xmlSetting.SelectSingleNode("Name").InnerText = cboSettings.SelectedItem.Value Then
                            Dim strValue As String = xmlSetting.SelectSingleNode("Value").InnerText
                            Select Case strValue
                                Case ""
                                    txtValue.Visible = True
                                    cboValue.Visible = False
                                Case "[TABID]"
                                    Dim objTabs As New TabController
                                    For Each objTab As Entities.Tabs.TabInfo In objTabs.GetTabsByPortal(PortalId).AsList
                                        cboValue.Items.Add(New ListItem(objTab.TabName, objTab.TabID.ToString))
                                    Next
                                    cboValue.Items.Insert(0, "<" & Services.Localization.Localization.GetString("Not_Specified") & ">")
                                    cboValue.Visible = True
                                    txtValue.Visible = False
                                Case Else
                                    Dim arrValues() As String = (strValue & ",").Split(",")
                                    For Each strValue In arrValues
                                        If strValue <> "" Then
                                            cboValue.Items.Add(New ListItem(strValue, strValue))
                                        End If
                                    Next
                                    cboValue.Items.Insert(0, "<" & Services.Localization.Localization.GetString("Not_Specified") & ">")
                                    cboValue.Visible = True
                                    txtValue.Visible = False
                            End Select
                            lblHelp.Text = xmlSetting.SelectSingleNode("Help").InnerText
                        End If
                    Next
                Catch
                    UI.Skins.Skin.AddModuleMessage(Me, "Error Loading Settings File For Object", UI.Skins.Controls.ModuleMessage.ModuleMessageType.RedError)
                End Try
            Else
                UI.Skins.Skin.AddModuleMessage(Me, "Object Selected Does Not Have Settings Defined", UI.Skins.Controls.ModuleMessage.ModuleMessageType.YellowWarning)
            End If

        End Sub

        Private Sub UpdateSkin()

            If cboSettings.SelectedIndex > 0 Then
                If (Not cboValue.SelectedItem Is Nothing) Or txtValue.Text <> "" Then
                    Dim objStreamReader As StreamReader
                    objStreamReader = File.OpenText(cboFiles.SelectedItem.Value)
                    Dim strSkin As String = objStreamReader.ReadToEnd
                    objStreamReader.Close()

                    Dim strTag As String = "<dnn:" & cboTokens.SelectedItem.Text & " runat=""server"" id=""dnn" & cboTokens.SelectedItem.Text & """"
                    Dim intOpenTag As Integer = strSkin.IndexOf(strTag)
                    If intOpenTag <> -1 Then
                        Dim intCloseTag As Integer = strSkin.IndexOf(" />", intOpenTag)
                        Dim strAttribute As String = cboSettings.SelectedItem.Value
                        Dim intStartAttribute As Integer = strSkin.IndexOf(strAttribute, intOpenTag)
                        Dim strValue As String = ""
                        If cboValue.Visible Then
                            strValue = cboValue.SelectedItem.Value
                        Else
                            strValue = txtValue.Text
                        End If
                        If intStartAttribute <> -1 And intStartAttribute < intCloseTag Then
                            ' remove attribute
                            Dim intEndAttribute As Integer = strSkin.IndexOf(""" ", intStartAttribute)
                            strSkin = strSkin.Substring(0, intStartAttribute) & strSkin.Substring(intEndAttribute + 2)
                        End If

                        ' add attribute
                        strSkin = strSkin.Insert(intOpenTag + strTag.Length, " " & strAttribute & "=""" & strValue & """")

                        Try
                            File.SetAttributes(cboFiles.SelectedItem.Value, FileAttributes.Normal)
                            Dim objStream As StreamWriter
                            objStream = File.CreateText(cboFiles.SelectedItem.Value)
                            objStream.WriteLine(strSkin)
                            objStream.Close()

                            UpdateManifest()

                            UI.Skins.Skin.AddModuleMessage(Me, "Skin Successfully Updated", UI.Skins.Controls.ModuleMessage.ModuleMessageType.GreenSuccess)
                        Catch
                            UI.Skins.Skin.AddModuleMessage(Me, "Error Updating Skin File", UI.Skins.Controls.ModuleMessage.ModuleMessageType.RedError)
                        End Try
                    Else
                        UI.Skins.Skin.AddModuleMessage(Me, "Selected File Does Not Contain Token", UI.Skins.Controls.ModuleMessage.ModuleMessageType.YellowWarning)
                    End If
                Else
                    UI.Skins.Skin.AddModuleMessage(Me, "You Must Specify A Value For The Setting", UI.Skins.Controls.ModuleMessage.ModuleMessageType.YellowWarning)
                End If
            Else
                UI.Skins.Skin.AddModuleMessage(Me, "You Must Select A Token Setting", UI.Skins.Controls.ModuleMessage.ModuleMessageType.YellowWarning)
            End If

        End Sub

        Private Sub UpdateManifest()

            If File.Exists(cboFiles.SelectedItem.Value.Replace(".ascx", ".htm")) Then
                Dim strFile As String = cboFiles.SelectedItem.Value.Replace(".ascx", ".xml")
                If File.Exists(strFile) = False Then
                    strFile = strFile.Replace(Path.GetFileName(strFile), "skin.xml")
                End If

                Dim xmlDoc As XmlDocument = Nothing
                Try
                    xmlDoc = New XmlDocument
                    xmlDoc.Load(strFile)
                Catch
                    xmlDoc.InnerXml = "<Objects></Objects>"
                End Try

                Dim xmlToken As XmlNode = xmlDoc.DocumentElement.SelectSingleNode("descendant::Object[Token='[" & cboTokens.SelectedItem.Text & "]']")
                If xmlToken Is Nothing Then
                    ' add token
                    Dim strToken As String = "<Token>[" & cboTokens.SelectedItem.Text & "]</Token><Settings></Settings>"
                    xmlToken = xmlDoc.CreateElement("Object")
                    xmlToken.InnerXml = strToken
                    xmlDoc.SelectSingleNode("Objects").AppendChild(xmlToken)
                    xmlToken = xmlDoc.DocumentElement.SelectSingleNode("descendant::Object[Token='[" & cboTokens.SelectedItem.Text & "]']")
                End If

                Dim strValue As String = ""
                If cboValue.Visible Then
                    strValue = cboValue.SelectedItem.Value
                Else
                    strValue = txtValue.Text
                End If

                Dim blnUpdate As Boolean = False
                Dim xmlSetting As XmlNode
                For Each xmlSetting In xmlToken.SelectNodes(".//Settings/Setting")
                    If xmlSetting.SelectSingleNode("Name").InnerText = cboSettings.SelectedItem.Value Then
                        xmlSetting.SelectSingleNode("Value").InnerText = strValue
                        blnUpdate = True
                    End If
                Next

                If blnUpdate = False Then
                    Dim strSetting As String = "<Name>" & cboSettings.SelectedItem.Value & "</Name><Value>" & strValue & "</Value>"
                    xmlSetting = xmlDoc.CreateElement("Setting")
                    xmlSetting.InnerXml = strSetting
                    xmlToken.SelectSingleNode("Settings").AppendChild(xmlSetting)
                End If

                Try
                    If File.Exists(strFile) Then
                        File.SetAttributes(strFile, FileAttributes.Normal)
                    End If
                    Dim objStream As StreamWriter
                    objStream = File.CreateText(strFile)
                    Dim strXML As String = xmlDoc.InnerXml
                    strXML = strXML.Replace("><", ">" & vbCrLf & "<")
                    objStream.WriteLine(strXML)
                    objStream.Close()
                Catch
                    ' error
                End Try
            End If

        End Sub

#End Region

    End Class

End Namespace
