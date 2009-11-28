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
Imports System.IO
Imports System.Text
Imports System.Xml.XPath

Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Entities.Modules.Definitions
Imports DotNetNuke.Services.EventQueue

Namespace DotNetNuke.Services.Installer.Installers

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The SkinControlInstaller installs SkinControl (SkinObject) Components to a DotNetNuke site
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[cnurse]	03/28/2008  created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class SkinControlInstaller
        Inherits ComponentInstallerBase

#Region "Private Properties"

        Private InstalledSkinControl As SkinControlInfo
        Private SkinControl As SkinControlInfo

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
                Return "ascx, vb, cs, js, resx, xml, vbproj, csproj, sln"
            End Get
        End Property

#End Region

#Region "Private Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The DeleteSkinControl method deletes the SkinControl from the data Store.
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	03/28/2008  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub DeleteSkinControl()
            Try
                'Attempt to get the SkinControl
                Dim skinControl As SkinControlInfo = SkinControlController.GetSkinControlByPackageID(Package.PackageID)

                If skinControl IsNot Nothing Then
                    SkinControlController.DeleteSkinControl(skinControl)
                End If
                Log.AddInfo(String.Format(Util.MODULE_UnRegistered, skinControl.ControlKey))
            Catch ex As Exception
                Log.AddFailure(ex)
            End Try
        End Sub

#End Region

#Region "Public Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The Commit method finalises the Install and commits any pending changes.
        ''' </summary>
        ''' <remarks>In the case of Modules this is not neccessary</remarks>
        ''' <history>
        ''' 	[cnurse]	03/28/2008  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overrides Sub Commit()
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The Install method installs the Module component
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	03/28/2008  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overrides Sub Install()
            Try
                'Attempt to get the SkinControl
                InstalledSkinControl = SkinControlController.GetSkinControlByKey(SkinControl.ControlKey)

                If InstalledSkinControl IsNot Nothing Then
                    SkinControl.SkinControlID = InstalledSkinControl.SkinControlID
                End If

                'Save SkinControl
                SkinControl.PackageID = Package.PackageID
                SkinControl.SkinControlID = SkinControlController.SaveSkinControl(SkinControl)

                Completed = True
                Log.AddInfo(String.Format(Util.MODULE_Registered, SkinControl.ControlKey))
            Catch ex As Exception
                Log.AddFailure(ex)
            End Try
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The ReadManifest method reads the manifest file for the SkinControl compoent.
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	03/28/2008  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overrides Sub ReadManifest(ByVal manifestNav As XPathNavigator)

            'Load the SkinControl from the manifest
            SkinControl = CBO.DeserializeObject(Of SkinControlInfo)(New StringReader(manifestNav.InnerXml))

            If Log.Valid Then
                Log.AddInfo(Util.MODULE_ReadSuccess)
            End If
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The Rollback method undoes the installation of the component in the event 
        ''' that one of the other components fails
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	03/28/2008  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overrides Sub Rollback()
            'If Temp SkinControl exists then we need to update the DataStore with this 
            If InstalledSkinControl Is Nothing Then
                'No Temp SkinControl - Delete newly added SkinControl
                DeleteSkinControl()
            Else
                'Temp SkinControl - Rollback to Temp
                SkinControlController.SaveSkinControl(InstalledSkinControl)
            End If
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The UnInstall method uninstalls the SkinControl component
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	03/28/2008  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overrides Sub UnInstall()
            DeleteSkinControl()
        End Sub

#End Region

    End Class

End Namespace
