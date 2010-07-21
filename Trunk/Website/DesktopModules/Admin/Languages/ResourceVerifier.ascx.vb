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
Imports System.Collections.Generic
Imports DotNetNuke.Web.UI.WebControls

Namespace DotNetNuke.Modules.Admin.Languages

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Manages translations for Resource files
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[vmasanas]	10/04/2004  Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Partial Class ResourceVerifier
        Inherits DotNetNuke.Entities.Modules.PortalModuleBase

#Region "Private Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets all system default resource files
        ''' </summary>
        ''' <param name="fileList">List of found resource files</param>
        ''' <param name="_path">Folder to search at</param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Vicenç]	05/11/2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub GetResourceFiles(ByVal fileList As SortedList, ByVal _path As String)
            Dim folders As String() = Directory.GetDirectories(_path)
            Dim folder As String
            Dim objFile As IO.FileInfo
            Dim objFolder As DirectoryInfo

            For Each folder In folders
                objFolder = New System.IO.DirectoryInfo(folder)

                If objFolder.Name.ToLowerInvariant = Localization.LocalResourceDirectory.ToLowerInvariant Then
                    ' found local resource folder, add resources
                    For Each objFile In objFolder.GetFiles("*.ascx.resx")
                        fileList.Add(objFile.FullName, objFile)
                    Next
                    For Each objFile In objFolder.GetFiles("*.aspx.resx")
                        fileList.Add(objFile.FullName, objFile)
                    Next
                    ' add LocalSharedResources if found
                    If File.Exists(Path.Combine(folder, Localization.LocalSharedResourceFile)) Then
                        fileList.Add(Path.Combine(folder, Localization.LocalSharedResourceFile), New System.IO.FileInfo(Path.Combine(folder, Localization.LocalSharedResourceFile)))
                    End If
                Else
                    GetResourceFiles(fileList, folder)
                End If
            Next

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Returns the resource file name for a given locale
        ''' </summary>
        ''' <param name="filename">Resource file</param>
        ''' <param name="language">Locale</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Vicenç]	05/11/2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Function ResourceFile(ByVal filename As String, ByVal language As String) As String
            Dim resourcefilename As String = filename

            If language <> Localization.SystemLocale Then
                resourcefilename = resourcefilename.Replace(".resx", "." + language + ".resx")
            End If

            Return resourcefilename

        End Function

#End Region

#Region "Event Handlers"

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            Try
                Dim files As New SortedList
                Dim locales As Dictionary(Of String, Locale) = LocaleController.Instance().GetLocales(Null.NullInteger)
                Dim shc As UI.UserControls.SectionHeadControl

                GetResourceFiles(files, Server.MapPath("~\admin"))
                GetResourceFiles(files, Server.MapPath("~\controls"))
                GetResourceFiles(files, Server.MapPath("~\desktopmodules"))
                GetResourceFiles(files, Server.MapPath("~\providers"))
                GetResourceFiles(files, Server.MapPath("~\install"))
                GetResourceFiles(files, Server.MapPath("~\Portals\_Default\Skins"))
                ' Add global and shared resource files
                files.Add(Server.MapPath(Localization.GlobalResourceFile), New IO.FileInfo(Server.MapPath(Localization.GlobalResourceFile)))
                files.Add(Server.MapPath(Localization.SharedResourceFile), New IO.FileInfo(Server.MapPath(Localization.SharedResourceFile)))


                For Each locale As Locale In locales.Values
                    Dim languageLabel As New DnnLanguageLabel
                    languageLabel.Language = locale.Code

                    Dim tableTop As New HtmlTable
                    tableTop.ID = locale.Code
                    Dim rowTop As New HtmlTableRow
                    Dim cellTop As New HtmlTableCell

                    Dim tableMissing As New HtmlTable
                    tableMissing.ID = "Missing" & locale.Code
                    Dim tableEntries As New HtmlTable
                    tableEntries.ID = "Entry" & locale.Code
                    Dim tableObsolete As New HtmlTable
                    tableObsolete.ID = "Obsolete" & locale.Code
                    Dim tableOld As New HtmlTable
                    tableOld.ID = "Old" & locale.Code
                    Dim tableDuplicate As New HtmlTable
                    tableDuplicate.ID = "Duplicate" & locale.Code
                    Dim tableError As New HtmlTable
                    tableError.ID = "Error" & locale.Code


                    For Each file As DictionaryEntry In files
                        ' check for existance
                        If Not IO.File.Exists(ResourceFile(file.Key.ToString, locale.Code)) Then
                            Dim row As New HtmlTableRow
                            Dim cell As New HtmlTableCell
                            cell.InnerText = ResourceFile(file.Key.ToString, locale.Code).Replace(Server.MapPath("~"), "")
                            cell.Attributes.Item("Class") = "Normal"
                            row.Cells.Add(cell)
                            tableMissing.Rows.Add(row)
                        Else
                            Dim dsDef As New DataSet
                            Dim dsRes As New DataSet
                            Dim dtDef, dtRes As DataTable

                            Try
                                dsDef.ReadXml(file.Key.ToString)
                            Catch
                                Dim row As New HtmlTableRow
                                Dim cell As New HtmlTableCell
                                cell.InnerText = file.Key.ToString.Replace(Server.MapPath("~"), "")
                                cell.Attributes.Item("Class") = "Normal"
                                row.Cells.Add(cell)
                                tableError.Rows.Add(row)
                                dsDef = Nothing
                            End Try
                            Try
                                dsRes.ReadXml(ResourceFile(file.Key.ToString, locale.Code))
                            Catch
                                If locale.Text <> Localization.SystemLocale Then
                                    Dim row As New HtmlTableRow
                                    Dim cell As New HtmlTableCell
                                    cell.InnerText = ResourceFile(file.Key.ToString, locale.Code).Replace(Server.MapPath("~"), "")
                                    cell.Attributes.Item("Class") = "Normal"
                                    row.Cells.Add(cell)
                                    tableError.Rows.Add(row)
                                    dsRes = Nothing
                                End If
                            End Try

                            If dsRes IsNot Nothing AndAlso dsDef IsNot Nothing AndAlso dsRes.Tables("data") IsNot Nothing AndAlso dsDef.Tables("data") IsNot Nothing Then
                                dtDef = dsDef.Tables("data")
                                dtDef.TableName = "default"
                                dtRes = dsRes.Tables("data").Copy
                                dtRes.TableName = "localized"
                                dsDef.Tables.Add(dtRes)

                                ' Check for duplicate entries in localized file
                                Try
                                    ' if this fails-> file contains duplicates
                                    Dim c As New UniqueConstraint("uniqueness", dtRes.Columns("name"))
                                    dtRes.Constraints.Add(c)
                                    dtRes.Constraints.Remove("uniqueness")
                                Catch
                                    Dim row As New HtmlTableRow
                                    Dim cell As New HtmlTableCell
                                    cell.InnerText = ResourceFile(file.Key.ToString, locale.Code).Replace(Server.MapPath("~"), "")
                                    cell.Attributes.Item("Class") = "Normal"
                                    row.Cells.Add(cell)
                                    tableDuplicate.Rows.Add(row)
                                End Try

                                ' Check for missing entries in localized file
                                Try
                                    ' if this fails-> some entries in System default file are not found in Resource file
                                    dsDef.Relations.Add("missing", dtRes.Columns("name"), dtDef.Columns("name"))
                                Catch
                                    Dim row As New HtmlTableRow
                                    Dim cell As New HtmlTableCell
                                    cell.InnerText = ResourceFile(file.Key.ToString, locale.Code).Replace(Server.MapPath("~"), "")
                                    cell.Attributes.Item("Class") = "Normal"
                                    row.Cells.Add(cell)
                                    tableEntries.Rows.Add(row)
                                Finally
                                    dsDef.Relations.Remove("missing")
                                End Try

                                ' Check for obsolete entries in localized file
                                Try
                                    ' if this fails-> some entries in Resource File are not found in System default
                                    dsDef.Relations.Add("obsolete", dtDef.Columns("name"), dtRes.Columns("name"))
                                Catch
                                    Dim row As New HtmlTableRow
                                    Dim cell As New HtmlTableCell
                                    cell.InnerText = ResourceFile(file.Key.ToString, locale.Code).Replace(Server.MapPath("~"), "")
                                    cell.Attributes.Item("Class") = "Normal"
                                    row.Cells.Add(cell)
                                    tableObsolete.Rows.Add(row)
                                Finally
                                    dsDef.Relations.Remove("obsolete")
                                End Try

                                ' Check older files
                                Dim resFile As New IO.FileInfo(ResourceFile(file.Key.ToString, locale.Code))
                                If CType(file.Value, IO.FileInfo).LastWriteTime > resFile.LastWriteTime Then
                                    Dim row As New HtmlTableRow
                                    Dim cell As New HtmlTableCell
                                    cell.InnerText = ResourceFile(file.Key.ToString, locale.Code).Replace(Server.MapPath("~"), "")
                                    cell.Attributes.Item("Class") = "Normal"
                                    row.Cells.Add(cell)
                                    tableOld.Rows.Add(row)
                                End If
                            End If
                        End If
                    Next

                    If tableMissing.Rows.Count > 0 Then
                        ' ------- Missing files
                        shc = CType(LoadControl("~/controls/sectionheadcontrol.ascx"), UI.UserControls.SectionHeadControl)
                        shc.Section = "Missing" & locale.Code
                        shc.IncludeRule = False
                        shc.IsExpanded = False
                        shc.CssClass = "SubHead"
                        shc.Text = Localization.GetString("MissingFiles", Me.LocalResourceFile) & tableMissing.Rows.Count.ToString
                        cellTop.Controls.Add(shc)
                        cellTop.Controls.Add(tableMissing)
                    End If

                    If tableDuplicate.Rows.Count > 0 Then
                        ' ------- Duplicate keys
                        shc = CType(LoadControl("~/controls/sectionheadcontrol.ascx"), UI.UserControls.SectionHeadControl)
                        shc.Section = "Duplicate" & locale.Code
                        shc.IncludeRule = False
                        shc.IsExpanded = False
                        shc.CssClass = "SubHead"
                        shc.Text = Localization.GetString("DuplicateEntries", Me.LocalResourceFile) & tableDuplicate.Rows.Count.ToString
                        cellTop.Controls.Add(shc)
                        cellTop.Controls.Add(tableDuplicate)
                    End If

                    If tableEntries.Rows.Count > 0 Then
                        ' ------- Missing entries
                        shc = CType(LoadControl("~/controls/sectionheadcontrol.ascx"), UI.UserControls.SectionHeadControl)
                        shc.Section = "Entry" & locale.Code
                        shc.IncludeRule = False
                        shc.IsExpanded = False
                        shc.CssClass = "SubHead"
                        shc.Text = Localization.GetString("MissingEntries", Me.LocalResourceFile) & tableEntries.Rows.Count.ToString
                        cellTop.Controls.Add(shc)
                        cellTop.Controls.Add(tableEntries)
                    End If

                    If tableObsolete.Rows.Count > 0 Then
                        ' ------- Missing entries
                        shc = CType(LoadControl("~/controls/sectionheadcontrol.ascx"), UI.UserControls.SectionHeadControl)
                        shc.Section = "Obsolete" & locale.Code
                        shc.IncludeRule = False
                        shc.IsExpanded = False
                        shc.CssClass = "SubHead"
                        shc.Text = Localization.GetString("ObsoleteEntries", Me.LocalResourceFile) & tableObsolete.Rows.Count.ToString
                        cellTop.Controls.Add(shc)
                        cellTop.Controls.Add(tableObsolete)
                    End If

                    If tableOld.Rows.Count > 0 Then
                        ' ------- Old files
                        shc = CType(LoadControl("~/controls/sectionheadcontrol.ascx"), UI.UserControls.SectionHeadControl)
                        shc.Section = "Old" & locale.Code
                        shc.IncludeRule = False
                        shc.IsExpanded = False
                        shc.CssClass = "SubHead"
                        shc.Text = Localization.GetString("OldFiles", Me.LocalResourceFile) & tableOld.Rows.Count.ToString
                        cellTop.Controls.Add(shc)
                        cellTop.Controls.Add(tableOld)
                    End If

                    If tableError.Rows.Count > 0 Then
                        ' ------- Error files
                        shc = CType(LoadControl("~/controls/sectionheadcontrol.ascx"), UI.UserControls.SectionHeadControl)
                        shc.Section = "Error" & locale.Code
                        shc.IncludeRule = False
                        shc.IsExpanded = False
                        shc.CssClass = "SubHead"
                        shc.Text = Localization.GetString("ErrorFiles", Me.LocalResourceFile) & tableError.Rows.Count.ToString
                        cellTop.Controls.Add(shc)
                        cellTop.Controls.Add(tableError)
                    End If

                    rowTop.Cells.Add(cellTop)
                    tableTop.Rows.Add(rowTop)

                    PlaceHolder1.Controls.Add(languageLabel)
                    PlaceHolder1.Controls.Add(tableTop)
                    PlaceHolder1.Controls.Add(New LiteralControl("<br>"))

                Next
            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try

        End Sub

        Private Sub cmdCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdCancel.Click
            Try
                Response.Redirect(NavigateURL())
            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try

        End Sub

#End Region



    End Class

End Namespace
