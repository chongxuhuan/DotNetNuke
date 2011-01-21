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

Namespace DotNetNuke.UI.Skins

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' SkinThumbNailControl is a user control that provides that displays the skins
    '''	as a Radio ButtonList with Thumbnail Images where available
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[cnurse]	10/12/2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public MustInherit Class SkinThumbNailControl
        Inherits Framework.UserControlBase

#Region "Controls"

        Protected WithEvents optSkin As System.Web.UI.WebControls.RadioButtonList
        Protected WithEvents ControlContainer As System.Web.UI.HtmlControls.HtmlGenericControl

#End Region

#Region "Private Members"

        Private _Columns As Integer = -1

#End Region

#Region "Properties"

        Public Property Border() As String
            Get
                Return Convert.ToString(ViewState("SkinControlBorder"))
            End Get
            Set(ByVal Value As String)
                ViewState("SkinControlBorder") = Value
                If Value <> "" Then
                    ControlContainer.Style.Add("border-top", Value)
                    ControlContainer.Style.Add("border-bottom", Value)
                    ControlContainer.Style.Add("border-left", Value)
                    ControlContainer.Style.Add("border-right", Value)
                End If
            End Set
        End Property

        Public Property Columns() As Integer
            Get
                Return Convert.ToInt32(ViewState("SkinControlColumns"))
            End Get
            Set(ByVal Value As Integer)
                ViewState("SkinControlColumns") = Value
                If Value > 0 Then
                    optSkin.RepeatColumns = Value
                End If
            End Set
        End Property

        Public Property Height() As String
            Get
                Return Convert.ToString(ViewState("SkinControlHeight"))
            End Get
            Set(ByVal Value As String)
                ViewState("SkinControlHeight") = Value
                If Value <> "" Then
                    ControlContainer.Style.Add("height", Value)
                End If
            End Set
        End Property

        Public Property SkinRoot() As String
            Get
                Return Convert.ToString(ViewState("SkinRoot"))
            End Get
            Set(ByVal Value As String)
                ViewState("SkinRoot") = Value
            End Set
        End Property

        Public Property SkinSrc() As String
            Get
                If Not optSkin.SelectedItem Is Nothing Then
                    Return optSkin.SelectedItem.Value
                Else
                    Return ""
                End If
            End Get
            Set(ByVal Value As String)
                ' select current skin
                Dim intIndex As Integer
                For intIndex = 0 To optSkin.Items.Count - 1
                    If optSkin.Items(intIndex).Value = Value Then
                        optSkin.Items(intIndex).Selected = True
                        Exit For
                    End If
                Next

            End Set
        End Property

        Public Property Width() As String
            Get
                Return Convert.ToString(ViewState("SkinControlWidth"))
            End Get
            Set(ByVal Value As String)
                ViewState("SkinControlWidth") = Value
                If Value <> "" Then
                    ControlContainer.Style.Add("width", Value)
                End If
            End Set
        End Property

#End Region

#Region "Private Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' AddDefaultSkin adds the not-specified skin to the radio button list
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	12/15/2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub AddDefaultSkin()
            Dim strDefault As String = Services.Localization.Localization.GetString("Not_Specified") & "<br>"
            strDefault += "<img src=""" & Common.Globals.ApplicationPath.Replace("\", "/") & "/images/spacer.gif"" width=""140"" height=""135"" border=""0"">"
            optSkin.Items.Insert(0, New ListItem(strDefault, ""))
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' AddSkin adds the skin to the radio button list
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <param name="strFolder">The Skin Folder</param>
        ''' <param name="strFile">The Skin File</param>
        ''' <history>
        ''' 	[cnurse]	9/8/2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub AddSkin(ByVal root As String, ByVal strFolder As String, ByVal strFile As String)

            Dim strImage As String = ""

            If File.Exists(strFile.Replace(".ascx", ".jpg")) Then
                strImage += "<a href=""" & CreateThumbnail(strFile.Replace(".ascx", ".jpg")).Replace("thumbnail_", "").Replace("\", "/") & """ target=""_new""><img src=""" & CreateThumbnail(strFile.Replace(".ascx", ".jpg")).Replace("\", "/") & """ border=""1""></a>"
            Else
                strImage += "<img src=""" & Common.Globals.ApplicationPath.Replace("\", "/") & "/images/thumbnail.jpg"" border=""1"">"
            End If

            optSkin.Items.Add(New ListItem(FormatSkinName(strFolder, Path.GetFileNameWithoutExtension(strFile)) & "<br>" & strImage, root & "/" & strFolder & "/" & Path.GetFileName(strFile)))

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' format skin name
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <param name="strSkinFolder">The Folder Name</param>
        ''' <param name="strSkinFile">The File Name without extension</param>
        ''' <history>
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Function FormatSkinName(ByVal strSkinFolder As String, ByVal strSkinFile As String) As String
            If strSkinFolder.ToLower = "_default" Then
                ' host folder
                Return strSkinFile
            Else ' portal folder
                Select Case strSkinFile.ToLower
                    Case "skin", "container", "default"
                        Return strSkinFolder
                    Case Else
                        Return strSkinFolder & " - " & strSkinFile
                End Select
            End If
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' CreateThumbnail creates a thumbnail of the Preview Image
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <param name="strImage">The Image File Name</param>
        ''' <history>
        ''' 	[cnurse]	9/8/2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Function CreateThumbnail(ByVal strImage As String) As String

            Dim blnCreate As Boolean = True

            Dim strThumbnail As String = strImage.Replace(Path.GetFileName(strImage), "thumbnail_" & Path.GetFileName(strImage))

            ' check if image has changed
            If File.Exists(strThumbnail) Then
                Dim d1 As Date = File.GetLastWriteTime(strThumbnail)
                Dim d2 As Date = File.GetLastWriteTime(strImage)
                If File.GetLastWriteTime(strThumbnail) = File.GetLastWriteTime(strImage) Then
                    blnCreate = False
                End If
            End If

            If blnCreate Then

                Dim dblScale As Double
                Dim intHeight As Integer
                Dim intWidth As Integer

                Dim intSize As Integer = 140    ' size of the thumbnail 

                Dim objImage As System.Drawing.Image
                Try
                    objImage = Drawing.Image.FromFile(strImage)

                    ' scale the image to prevent distortion
                    If objImage.Height > objImage.Width Then
                        'The height was larger, so scale the width 
                        dblScale = intSize / objImage.Height
                        intHeight = intSize
                        intWidth = CInt(objImage.Width * dblScale)
                    Else
                        'The width was larger, so scale the height 
                        dblScale = intSize / objImage.Width
                        intWidth = intSize
                        intHeight = CInt(objImage.Height * dblScale)
                    End If

                    ' create the thumbnail image
                    Dim objThumbnail As System.Drawing.Image
                    objThumbnail = objImage.GetThumbnailImage(intWidth, intHeight, Nothing, IntPtr.Zero)

                    ' delete the old file ( if it exists )
                    If File.Exists(strThumbnail) Then
                        File.Delete(strThumbnail)
                    End If

                    ' save the thumbnail image 
                    objThumbnail.Save(strThumbnail, objImage.RawFormat)

                    ' set the file attributes
                    File.SetAttributes(strThumbnail, FileAttributes.Normal)
                    File.SetLastWriteTime(strThumbnail, File.GetLastWriteTime(strImage))

                    ' tidy up
                    objImage.Dispose()
                    objThumbnail.Dispose()

                Catch

                    ' problem creating thumbnail

                End Try

            End If

            strThumbnail = Common.Globals.ApplicationPath & "\" & strThumbnail.Substring(strThumbnail.ToLower.IndexOf("portals\"))

            ' return thumbnail filename
            Return strThumbnail

        End Function

#End Region

#Region "Public Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Clear clears the radio button list
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	12/15/2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub Clear()
            optSkin.Items.Clear()
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' LoadAllSkins loads all the available skins (Host and Site) to the radio button list
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <param name="includeNotSpecified">Optionally include the "Not Specified" option</param>
        ''' <history>
        ''' 	[cnurse]	12/15/2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub LoadAllSkins(ByVal includeNotSpecified As Boolean)

            ' default value
            If includeNotSpecified Then
                AddDefaultSkin()
            End If

            ' load host skins (includeNotSpecified = false as we have already added it)
            LoadHostSkins(False)

            ' load portal skins (includeNotSpecified = false as we have already added it)
            LoadPortalSkins(False)

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' LoadHostSkins loads all the available Host skins to the radio button list
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <param name="includeNotSpecified">Optionally include the "Not Specified" option</param>
        ''' <history>
        ''' 	[cnurse]	12/15/2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub LoadHostSkins(ByVal includeNotSpecified As Boolean)

            Dim strRoot As String
            Dim strFolder As String
            Dim arrFolders As String()

            ' default value
            If includeNotSpecified Then
                AddDefaultSkin()
            End If

            ' load host skins
            strRoot = Common.Globals.HostMapPath & SkinRoot
            If Directory.Exists(strRoot) Then
                arrFolders = Directory.GetDirectories(strRoot)
                For Each strFolder In arrFolders
                    If Not strFolder.EndsWith(glbHostSkinFolder) Then
                        LoadSkins(strFolder, "[G]", False)
                    End If
                Next
            End If

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' LoadHostSkins loads all the available Site/Portal skins to the radio button list
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <param name="includeNotSpecified">Optionally include the "Not Specified" option</param>
        ''' <history>
        ''' 	[cnurse]	12/15/2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub LoadPortalSkins(ByVal includeNotSpecified As Boolean)

            Dim strRoot As String
            Dim strFolder As String
            Dim arrFolders As String()

            ' default value
            If includeNotSpecified Then
                AddDefaultSkin()
            End If

            ' load portal skins
            strRoot = PortalSettings.HomeDirectoryMapPath & SkinRoot
            If Directory.Exists(strRoot) Then
                arrFolders = Directory.GetDirectories(strRoot)
                For Each strFolder In arrFolders
                    LoadSkins(strFolder, "[L]", False)
                Next
            End If

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' LoadSkins loads all the available skins in a specific folder to the radio button list
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <param name="strFolder">The folder to search for skins</param>
        ''' <param name="skinType">A string that identifies whether the skin is Host "[G]" or Site "[L]"</param>
        ''' <param name="includeNotSpecified">Optionally include the "Not Specified" option</param>
        ''' <history>
        ''' 	[cnurse]	12/15/2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub LoadSkins(ByVal strFolder As String, ByVal skinType As String, ByVal includeNotSpecified As Boolean)

            Dim strFile As String
            Dim arrFiles As String()

            ' default value
            If includeNotSpecified Then
                AddDefaultSkin()
            End If

            If Directory.Exists(strFolder) Then
                arrFiles = Directory.GetFiles(strFolder, "*.ascx")
                strFolder = Mid(strFolder, InStrRev(strFolder, "\") + 1)

                For Each strFile In arrFiles
                    AddSkin(skinType & SkinRoot, strFolder, strFile)
                Next
            End If

        End Sub

#End Region

#Region "Event Handlers"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Page_Load runs when the control is loaded
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	10/12/2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        End Sub

#End Region

    End Class

End Namespace
