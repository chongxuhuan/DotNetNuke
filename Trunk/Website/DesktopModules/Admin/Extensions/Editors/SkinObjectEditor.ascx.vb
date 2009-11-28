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
Imports DotNetNuke.Services.Installer.Packages
Imports DotNetNuke.Entities.Modules

Namespace DotNetNuke.Modules.Admin.SkinObjects

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The SkinObjectEditor PortalModuleBase is used to edit a Skin Control
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[cnurse]	03/28/2008    Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Partial Class SkinObjectEditor
        Inherits PackageEditorBase

#Region "Protected Properties"

        Protected Overrides ReadOnly Property EditorID() As String
            Get
                Return "SkinObjectEditor"
            End Get
        End Property

#End Region

#Region "Private Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' This routine Binds the Skin Control
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	03/28/2008    Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub BindSkinObject()
            Dim skinControl As SkinControlInfo = SkinControlController.GetSkinControlByPackageID(PackageID)

            If Not Me.ModuleContext.PortalSettings.ActiveTab.IsSuperTab Then
                ctlSkinObject.EditMode = UI.WebControls.PropertyEditorMode.View
                pnlHelp.Visible = False
            End If

            If skinControl IsNot Nothing Then
                ctlSkinObject.LocalResourceFile = LocalResourceFile
                ctlSkinObject.DataSource = skinControl
                ctlSkinObject.DataBind()
            End If

        End Sub

#End Region

#Region "Protected Methods"

        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
            MyBase.OnLoad(e)

            BindSkinObject()
        End Sub

#End Region

#Region "Public Methods"

        Public Overrides Sub UpdatePackage()
            If ctlSkinObject.IsValid AndAlso ctlSkinObject.IsDirty Then
                Dim skinControl As SkinControlInfo = TryCast(ctlSkinObject.DataSource, SkinControlInfo)
                If skinControl IsNot Nothing Then
                    SkinControlController.SaveSkinControl(skinControl)
                End If
            End If
        End Sub

#End Region

    End Class

End Namespace
