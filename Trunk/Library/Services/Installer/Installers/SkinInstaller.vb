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

Imports System.Collections.Generic
Imports System.Data.Common
Imports System.IO
Imports System.Text
Imports System.Xml.XPath

Imports DotNetNuke.UI.Skins

Namespace DotNetNuke.Services.Installer.Installers

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The SkinInstaller installs Skin Components to a DotNetNuke site
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[cnurse]	08/22/2007  created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class SkinInstaller
        Inherits FileInstaller

#Region "Private Members"

        Private _SkinFiles As New ArrayList()
        Private _SkinName As String = Null.NullString

        Private SkinPackage As SkinPackageInfo
        Private TempSkinPackage As SkinPackageInfo

#End Region

#Region "Protected Properties"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the name of the Collection Node ("skinFiles")
        ''' </summary>
        ''' <value>A String</value>
        ''' <history>
        ''' 	[cnurse]	08/22/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overrides ReadOnly Property CollectionNodeName() As String
            Get
                Return "skinFiles"
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the name of the Item Node ("skinFile")
        ''' </summary>
        ''' <value>A String</value>
        ''' <history>
        ''' 	[cnurse]	08/22/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overrides ReadOnly Property ItemNodeName() As String
            Get
                Return "skinFile"
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the PhysicalBasePath for the skin files
        ''' </summary>
        ''' <value>A String</value>
        ''' <history>
        ''' 	[cnurse]	07/25/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overrides ReadOnly Property PhysicalBasePath() As String
            Get
                Dim _PhysicalBasePath As String = RootPath & SkinRoot & "\" & SkinPackage.SkinName
                If Not _PhysicalBasePath.EndsWith("\") Then
                    _PhysicalBasePath += "\"
                End If
                Return _PhysicalBasePath.Replace("/", "\")
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the root folder for the Skin
        ''' </summary>
        ''' <value>A String</value>
        ''' <history>
        ''' 	[cnurse]	08/23/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected ReadOnly Property RootPath() As String
            Get
                Dim _RootPath As String = Null.NullString
                If Package.InstallerInfo.PortalID = Null.NullInteger Then
                    _RootPath = Common.Globals.HostMapPath
                Else
                    _RootPath = PortalController.GetCurrentPortalSettings.HomeDirectoryMapPath
                End If
                Return _RootPath
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the collection of Skin Files
        ''' </summary>
        ''' <value>A List(Of InstallFile)</value>
        ''' <history>
        ''' 	[cnurse]	08/23/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected ReadOnly Property SkinFiles() As ArrayList
            Get
                Return _SkinFiles
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the name of the SkinName Node ("skinName")
        ''' </summary>
        ''' <value>A String</value>
        ''' <history>
        ''' 	[cnurse]	08/23/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overridable ReadOnly Property SkinNameNodeName() As String
            Get
                Return "skinName"
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the RootName of the Skin
        ''' </summary>
        ''' <value>A String</value>
        ''' <history>
        ''' 	[cnurse]	08/22/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overridable ReadOnly Property SkinRoot() As String
            Get
                Return Skins.SkinController.RootSkin
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the Type of the Skin
        ''' </summary>
        ''' <value>A String</value>
        ''' <history>
        ''' 	[cnurse]	02/06/2008  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overridable ReadOnly Property SkinType() As String
            Get
                Return "Skin"
            End Get
        End Property

#End Region

#Region "Public Properties"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets a list of allowable file extensions (in addition to the Host's List)
        ''' </summary>
        ''' <value>A String</value>
        ''' <history>
        ''' 	[cnurse]	03/28/2008  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overrides ReadOnly Property AllowableFiles() As String
            Get
                Return "ascx, html, htm, css, xml, js, resx, jpg, jpeg, gif, png"
            End Get
        End Property

#End Region

#Region "Private Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The DeleteSkinPackage method deletes the Skin Package
        ''' from the data Store.
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	02/08/2008  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub DeleteSkinPackage()
            Try
                'Attempt to get the Authentication Service
                Dim skinPackage As SkinPackageInfo = SkinController.GetSkinByPackageID(Package.PackageID)

                If skinPackage IsNot Nothing Then
                    SkinController.DeleteSkinPackage(skinPackage)
                End If
                Log.AddInfo(String.Format(Util.SKIN_UnRegistered, skinPackage.SkinName))
            Catch ex As Exception
                Log.AddFailure(ex)
            End Try
        End Sub

#End Region

#Region "Protected Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The ProcessFile method determines what to do with parsed "file" node
        ''' </summary>
        ''' <param name="file">The file represented by the node</param>
        ''' <param name="nav">The XPathNavigator representing the node</param>
        ''' <history>
        ''' 	[cnurse]	08/07/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overrides Sub ProcessFile(ByVal file As InstallFile, ByVal nav As System.Xml.XPath.XPathNavigator)
            ' save the skin file for processing
            Select Case file.Extension
                Case "htm", "html", "ascx", "css"
                    If file.Path.ToLower().IndexOf(glbAboutPage.ToLower()) < 0 Then
                        SkinFiles.Add(PhysicalBasePath + file.FullName)
                    End If
            End Select

            'Call base method to set up for file processing
            MyBase.ProcessFile(file, nav)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The ReadCustomManifest method reads the custom manifest items
        ''' </summary>
        ''' <param name="nav">The XPathNavigator representing the node</param>
        ''' <history>
        ''' 	[cnurse]	08/22/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overrides Sub ReadCustomManifest(ByVal nav As XPathNavigator)

            SkinPackage = New SkinPackageInfo()
            SkinPackage.PortalID = Package.PortalID

            'Get the Skin name
            SkinPackage.SkinName = Util.ReadElement(nav, SkinNameNodeName)

            'Call base class
            MyBase.ReadCustomManifest(nav)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The UnInstallFile method unInstalls a single file.
        ''' </summary>
        ''' <param name="unInstallFile">The InstallFile to unInstall.</param>
        ''' <history>
        ''' 	[cnurse]	02/11/2008  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overrides Sub UnInstallFile(ByVal unInstallFile As InstallFile)
            'Uninstall file
            MyBase.UnInstallFile(unInstallFile)

            If unInstallFile.Extension = "htm" OrElse unInstallFile.Extension = "html" Then
                'Try to remove "processed file"
                Dim fileName As String = unInstallFile.FullName
                fileName = fileName.Replace(Path.GetExtension(fileName), ".ascx")
                Util.DeleteFile(fileName, PhysicalBasePath, Log)
            End If
        End Sub

#End Region

#Region "Public Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The Install method installs the skin component
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	02/06/2008  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overrides Sub Install()
            Dim bAdd As Boolean = Null.NullBoolean

            Try
                'Attempt to get the Skin Package
                TempSkinPackage = SkinController.GetSkinPackage(SkinPackage.PortalID, SkinPackage.SkinName, SkinType)

                If TempSkinPackage Is Nothing Then
                    bAdd = True
                    SkinPackage.PackageID = Package.PackageID
                Else
                    SkinPackage.SkinPackageID = TempSkinPackage.SkinPackageID
                    If TempSkinPackage.PackageID <> Package.PackageID Then
                        Completed = False
                        Log.AddFailure(Util.SKIN_Installed)
                        Exit Sub
                    Else
                        SkinPackage.PackageID = TempSkinPackage.PackageID
                    End If
                End If
                SkinPackage.SkinType = SkinType

                If bAdd Then
                    'Add new skin package
                    SkinPackage.SkinPackageID = SkinController.AddSkinPackage(SkinPackage)
                Else
                    'Update skin package
                    SkinController.UpdateSkinPackage(SkinPackage)
                End If
                Log.AddInfo(String.Format(Util.SKIN_Registered, SkinPackage.SkinName))

                'install (copy the files) by calling the base class
                MyBase.Install()

                ' process the list of skin files
                If SkinFiles.Count > 0 Then
                    Log.StartJob(Util.SKIN_BeginProcessing)

                    Dim strMessage As String = Null.NullString
                    Dim NewSkin As New UI.Skins.SkinFileProcessor(RootPath, SkinRoot, SkinPackage.SkinName)
                    For Each skinFile As String In SkinFiles
                        strMessage += NewSkin.ProcessFile(skinFile, SkinParser.Portable)

                        skinFile.Replace(DotNetNuke.Common.Globals.HostMapPath + "\", "[G]")

                        Select Case Path.GetExtension(skinFile)
                            Case ".htm"
                                SkinController.AddSkin(SkinPackage.SkinPackageID, skinFile.Replace("htm", "ascx"))
                            Case ".html"
                                SkinController.AddSkin(SkinPackage.SkinPackageID, skinFile.Replace("html", "ascx"))
                            Case ".ascx"
                                SkinController.AddSkin(SkinPackage.SkinPackageID, skinFile)
                        End Select
                    Next
                    Dim arrMessage As Array = Split(strMessage, "<br>")
                    Dim strRow As String
                    For Each strRow In arrMessage
                        Log.AddInfo(HtmlUtils.StripTags(strRow, True))
                    Next
                    Log.EndJob(Util.SKIN_EndProcessing)
                End If

                Completed = True
            Catch ex As Exception
                Log.AddFailure(ex)
            End Try

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The Rollback method undoes the installation of the component in the event 
        ''' that one of the other components fails
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	02/06/2008  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overrides Sub Rollback()
            'If Temp Skin exists then we need to update the DataStore with this 
            If TempSkinPackage Is Nothing Then
                'No Temp Skin - Delete newly added Skin
                DeleteSkinPackage()
            Else
                'Temp Skin - Rollback to Temp
                SkinController.UpdateSkinPackage(TempSkinPackage)
            End If

            'Call base class to prcoess files
            MyBase.Rollback()
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The UnInstall method uninstalls the skin component
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	02/06/2008  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overrides Sub UnInstall()
            DeleteSkinPackage()

            'Call base class to prcoess files
            MyBase.UnInstall()
        End Sub

#End Region

    End Class

End Namespace
