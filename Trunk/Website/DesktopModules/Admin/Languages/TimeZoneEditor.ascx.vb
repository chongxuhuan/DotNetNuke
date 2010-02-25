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
Imports DotNetNuke

Namespace DotNetNuke.Modules.Admin.Languages

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Manages translations for TimeZones file
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[vmasanas]	10/04/2004  Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Partial Class TimeZoneEditor
        Inherits DotNetNuke.Entities.Modules.PortalModuleBase

#Region "Event Handlers"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Loads suported locales and shows default values
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[vmasanas]	04/10/2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Try
                If Not Page.IsPostBack Then
                    ' Localize datagrid
                    Services.Localization.Localization.LocalizeDataGrid(dgEditor, Me.LocalResourceFile)
                    BindList()
                    cboLocales.SelectedValue = Services.Localization.Localization.SystemLocale
                    BindGrid(Localization.SystemLocale)
                End If
            Catch exc As Exception           'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Loads localized file
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks>
        ''' If a localized file does not exist for the selected language it is created using default values
        ''' </remarks>
        ''' <history>
        ''' 	[vmasanas]	04/10/2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub cboLocales_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cboLocales.SelectedIndexChanged
            Try
                Try
                    If Not File.Exists(Server.MapPath(ResourceFile(Localization.TimezonesFile, cboLocales.SelectedValue))) Then
                        File.Copy(Server.MapPath(Localization.TimezonesFile), Server.MapPath(ResourceFile(Localization.TimezonesFile, cboLocales.SelectedValue)))
                    End If
                Catch
                    UI.Skins.Skin.AddModuleMessage(Me, Services.Localization.Localization.GetString("Save.ErrorMessage", Me.LocalResourceFile), UI.Skins.Controls.ModuleMessage.ModuleMessageType.YellowWarning)
                End Try
                BindGrid(cboLocales.SelectedValue)
            Catch exc As Exception           'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Updates all values from the datagrid
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[vmasanas]	04/10/2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub cmdUpdate_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdUpdate.Click
            Dim di As DataGridItem
            Dim node As XmlNode
            Dim resDoc As New XmlDocument

            Try
                resDoc.Load(Server.MapPath(ResourceFile(Localization.TimezonesFile, cboLocales.SelectedValue)))
                For Each di In dgEditor.Items
                    If (di.ItemType = ListItemType.Item Or di.ItemType = ListItemType.AlternatingItem) Then
                        Dim ctl As TextBox = CType(di.Cells(0).Controls(1), TextBox)
                        node = resDoc.SelectSingleNode("//root/timezone[@key='" + di.Cells(1).Text + "']")
                        node.Attributes("name").Value = ctl.Text
                    End If
                Next
                Try
                    File.SetAttributes(Server.MapPath(ResourceFile(Localization.TimezonesFile, cboLocales.SelectedValue)), FileAttributes.Normal)
                    resDoc.Save(Server.MapPath(ResourceFile(Localization.TimezonesFile, cboLocales.SelectedValue)))
                    BindGrid(cboLocales.SelectedValue)
                Catch
                    UI.Skins.Skin.AddModuleMessage(Me, Services.Localization.Localization.GetString("Save.ErrorMessage", Me.LocalResourceFile), UI.Skins.Controls.ModuleMessage.ModuleMessageType.YellowWarning)
                End Try
                Response.Redirect(NavigateURL())
            Catch exc As Exception           'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Returns to main control
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
                Response.Redirect(NavigateURL())
            Catch exc As Exception           'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Adds missing nodes from the System Default file to the Resource file 
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[VMasanas]	05/10/2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub cmdAddMissing_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdAddMissing.Click
            Dim node As XmlNode
            Dim resDoc As New XmlDocument
            Dim defDoc As New XmlDocument

            resDoc.Load(Server.MapPath(ResourceFile(Localization.TimezonesFile, cboLocales.SelectedValue)))
            defDoc.Load(Server.MapPath(Localization.TimezonesFile))

            For Each node In defDoc.SelectNodes("//root/timezone")
                If resDoc.SelectSingleNode("//root/timezone[@key='" + node.Attributes("key").Value + "']") Is Nothing Then
                    resDoc.SelectSingleNode("//root").AppendChild(resDoc.ImportNode(node, True))
                End If
            Next
            Try
                resDoc.Save(Server.MapPath(ResourceFile(Localization.TimezonesFile, cboLocales.SelectedValue)))
                BindGrid(cboLocales.SelectedValue)
            Catch
                UI.Skins.Skin.AddModuleMessage(Me, Services.Localization.Localization.GetString("Save.ErrorMessage", Me.LocalResourceFile), UI.Skins.Controls.ModuleMessage.ModuleMessageType.YellowWarning)
            End Try

        End Sub

#End Region

#Region "Private Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Loads suported locales
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[vmasanas]	04/10/2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub BindList()
            cboLocales.DataSource = Localization.GetLocales(Null.NullInteger).Values
            cboLocales.DataBind()
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Loads TimeZone information
        ''' </summary>
        ''' <param name="language">Language to be loaded</param>
        ''' <remarks>
        ''' If Localized file contains entries not found in the System Default they will be deleted.
        ''' If System Default contains entries not found in the Localized file user will be asked to add them.
        ''' </remarks>
        ''' <history>
        ''' 	[vmasanas]	04/10/2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub BindGrid(ByVal language As String)
            Dim ds As New DataSet
            Dim dsDef As New DataSet
            Dim dt, dtDef As DataTable

            ds.ReadXml(Server.MapPath(ResourceFile(Localization.TimezonesFile, language)))
            ds.Tables(0).TableName = "Resource"
            dt = ds.Tables("Resource")

            dsDef.ReadXml(Server.MapPath(Localization.TimezonesFile))
            dtDef = dsDef.Tables(0).Copy
            dtDef.TableName = "Default"
            dtDef.Columns("name").ColumnName = "defaultvalue"
            ds.Tables.Add(dtDef)

            ' Check for missing entries in localized file
            Try
                pnlMissing.Visible = False
                ' if this fails-> some entries in System default file are not found in Resource file
                ds.Relations.Add("missing", dt.Columns("key"), dtDef.Columns("key"))
            Catch
                pnlMissing.Visible = True
            Finally
                ds.Relations.Remove("missing")
            End Try

            ' Relate localized entries to System default
            Try
                ' if this fails-> some entries in Resource file are not found in System default
                ds.Relations.Add("defaultvalues", dtDef.Columns("key"), dt.Columns("key"))
            Catch
                ' delete orphan entries in localized file
                DeleteEntries(ResourceFile(Localization.TimezonesFile, language), Services.Localization.Localization.TimezonesFile)
                ds.Relations.Remove("defaultvalues")
                ' reload data
                ds = New DataSet
                dsDef = New DataSet
                ds.ReadXml(Server.MapPath(ResourceFile(Localization.TimezonesFile, language)))
                ds.Tables(0).TableName = "Resource"
                dt = ds.Tables("Resource")

                dsDef.ReadXml(Server.MapPath(Localization.TimezonesFile))
                dtDef = dsDef.Tables(0).Copy
                dtDef.TableName = "Default"
                dtDef.Columns("name").ColumnName = "defaultvalue"
                ds.Tables.Add(dtDef)
                ds.Relations.Add("defaultvalues", dtDef.Columns("key"), dt.Columns("key"))
            End Try

            dgEditor.DataSource = ds
            dgEditor.DataMember = "Resource"
            dgEditor.DataBind()
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Removes nodes in localized file not found in System default
        ''' </summary>
        ''' <param name="resourceFile">Resource file</param>
        ''' <param name="defaultFile">System Default resource file</param>
        ''' <remarks>
        ''' Deletes the nodes in the resource file as saves it
        ''' </remarks>
        ''' <history>
        ''' 	[VMasanas]	05/10/2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub DeleteEntries(ByVal resourceFile As String, ByVal defaultFile As String)
            Dim node, parent As XmlNode
            Dim resDoc As New XmlDocument
            Dim defDoc As New XmlDocument

            resDoc.Load(Server.MapPath(resourceFile))
            defDoc.Load(Server.MapPath(defaultFile))

            For Each node In resDoc.SelectNodes("//root/timezone")
                If defDoc.SelectSingleNode("//root/timezone[@key='" + node.Attributes("key").Value + "']") Is Nothing Then
                    parent = node.ParentNode
                    parent.RemoveChild(node)
                End If
            Next

            Try
                resDoc.Save(Server.MapPath(resourceFile))
            Catch
                UI.Skins.Skin.AddModuleMessage(Me, Services.Localization.Localization.GetString("Save.ErrorMessage", Me.LocalResourceFile), UI.Skins.Controls.ModuleMessage.ModuleMessageType.YellowWarning)
            End Try
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Returns the resource file name for a given resource and language
        ''' </summary>
        ''' <param name="filename">Resource File</param>
        ''' <param name="language">Language</param>
        ''' <returns>Localized File Name</returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[vmasanas]	04/10/2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Function ResourceFile(ByVal filename As String, ByVal language As String) As String
            If language = Services.Localization.Localization.SystemLocale Then
                Return filename
            Else
                Return filename.Substring(0, filename.Length - 4) + "." + language + ".xml"
            End If
        End Function

#End Region

    End Class

End Namespace
