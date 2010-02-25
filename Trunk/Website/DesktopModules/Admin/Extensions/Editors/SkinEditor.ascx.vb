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
Imports System.Configuration
Imports DotNetNuke.UI.Skins
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Entities.Modules.Actions
Imports DotNetNuke.Services.FileSystem
Imports DotNetNuke.Services.Localization
Imports DotNetNuke.UI.Modules
Imports DotNetNuke.Services.Installer.Packages

Namespace DotNetNuke.Modules.Admin.Features

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The SkinEditor ModuleUserControlBase is used to edit a Skin
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[cnurse]	01/23/2008  created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Partial Class SkinEditor
        Inherits PackageEditorBase

#Region "Protected Properties"

        Protected Overrides ReadOnly Property EditorID() As String
            Get
                Return "SkinEditor"
            End Get
        End Property

#End Region

#Region "Private Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' This routine Binds the Skin
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	02/04/2008  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub BindSkin()
            Dim skin As SkinPackageInfo = SkinController.GetSkinByPackageID(PackageID)

            If Not Me.ModuleContext.PortalSettings.ActiveTab.IsSuperTab Then
                ctlSkin.EditMode = UI.WebControls.PropertyEditorMode.View
                pnlHelp.Visible = False
            End If

            If skin IsNot Nothing Then
                ctlSkin.LocalResourceFile = LocalResourceFile
                ctlSkin.DataSource = skin
                ctlSkin.DataBind()
            End If
        End Sub

#End Region

#Region "Protected Methods"

        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
            MyBase.OnLoad(e)

            BindSkin()
        End Sub

#End Region

#Region "Public Methods"

        Public Overrides Sub Initialize()
            If Package.PackageType = "Skin" Then
                lblTitle.Text = Localization.GetString("SkinTitle", LocalResourceFile)
                lblHelp.Text = Localization.GetString("SkinHelp", LocalResourceFile)
            Else
                lblTitle.Text = Localization.GetString("ContainerTitle", LocalResourceFile)
                lblHelp.Text = Localization.GetString("ContainerHelp", LocalResourceFile)
            End If
        End Sub

        Public Overrides Sub UpdatePackage()
            If ctlSkin.IsValid AndAlso ctlSkin.IsDirty Then
                Dim skin As SkinPackageInfo = TryCast(ctlSkin.DataSource, SkinPackageInfo)
                If skin IsNot Nothing Then
                    SkinController.UpdateSkinPackage(skin)
                End If
            End If
        End Sub

#End Region

#Region "Event Handlers"

        Protected Sub ctlSkin_ItemAdded(ByVal sender As Object, ByVal e As UI.WebControls.PropertyEditorEventArgs) Handles ctlSkin.ItemAdded
            If Not String.IsNullOrEmpty(e.StringValue) Then
                Dim skin As SkinPackageInfo = SkinController.GetSkinByPackageID(PackageID)
                SkinController.AddSkin(skin.SkinPackageID, e.StringValue)
            End If

            BindSkin()
        End Sub

        Protected Sub ctlSkin_ItemDeleted(ByVal sender As Object, ByVal e As UI.WebControls.PropertyEditorEventArgs) Handles ctlSkin.ItemDeleted
            If e.Key IsNot Nothing Then
                SkinController.DeleteSkin(CType(e.Key, Integer))
            End If
            BindSkin()
        End Sub

#End Region

    End Class

End Namespace