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


Imports DotNetNuke.UI.Modules
Imports DotNetNuke.UI.WebControls
Imports DotNetNuke.Services.Installer
Imports DotNetNuke.Services.Installer.Packages
Imports DotNetNuke.UI.Skins

Namespace DotNetNuke.Modules.Admin.Extensions

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The EditExtension control is used to edit a Extension
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[cnurse]	01/04/2008	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Partial Class EditExtension
        Inherits ModuleUserControlBase

        Private _Control As Control
        Private _Package As PackageInfo = Nothing

#Region "Protected Properties"

        Protected ReadOnly Property IsSuperTab() As Boolean
            Get
                Return (Me.ModuleContext.PortalSettings.ActiveTab.IsSuperTab)
            End Get
        End Property

        Public ReadOnly Property Mode() As String
            Get
                Return ModuleContext.Settings("Extensions_Mode")
            End Get
        End Property

        Protected ReadOnly Property Package() As PackageInfo
            Get
                If _Package Is Nothing Then
                    If PackageID = Null.NullInteger Then
                        _Package = New PackageInfo()
                    Else
                        _Package = PackageController.GetPackage(PackageID)
                    End If
                End If
                Return _Package
            End Get
        End Property

        Protected ReadOnly Property PackageEditor() As IPackageEditor
            Get
                If _Control Is Nothing Then
                    If Package IsNot Nothing Then
                        Dim _PackageType As PackageType = PackageController.GetPackageType(Package.PackageType)
                        If (_PackageType IsNot Nothing) AndAlso (Not String.IsNullOrEmpty(_PackageType.EditorControlSrc)) Then
                            _Control = ControlUtilities.LoadControl(Of Control)(Me, _PackageType.EditorControlSrc)
                        End If
                    End If
                End If
                Return TryCast(_Control, IPackageEditor)
            End Get
        End Property

        Public ReadOnly Property PackageID() As Integer
            Get
                Dim _PackageID As Integer = Null.NullInteger
                If Not (Request.QueryString("PackageID") Is Nothing) Then
                    _PackageID = Int32.Parse(Request.QueryString("PackageID"))
                End If
                Return _PackageID
            End Get
        End Property

        Protected ReadOnly Property ViewMode() As PropertyEditorMode
            Get
                Dim _ViewMode As PropertyEditorMode = PropertyEditorMode.View
                If Request.IsLocal AndAlso IsSuperTab Then
                    _ViewMode = PropertyEditorMode.Edit
                End If
                Return _ViewMode
            End Get
        End Property

#End Region

#Region "Private Methods"

        Private Sub BindData()
            fldEmail.ValidationExpression = glbEmailRegEx

            trLanguagePackType.Visible = False
            Select Case Mode
                Case "All"
                    lblHelp.Text = Localization.GetString("EditHelp", Me.LocalResourceFile)
                    cmdUpdate.Text = Localization.GetString("cmdUpdate", Me.LocalResourceFile)
                Case "LanguagePack"
                    lblHelp.Text = Localization.GetString("EditLanguageHelp", Me.LocalResourceFile)
                    cmdUpdate.Text = Localization.GetString("cmdUpdateLanguage", Me.LocalResourceFile)
                Case "Module"
                    lblHelp.Text = Localization.GetString("EditModuleHelp", Me.LocalResourceFile)
                    cmdUpdate.Text = Localization.GetString("cmdUpdateModule", Me.LocalResourceFile)
                Case "Skin"
                    lblHelp.Text = Localization.GetString("EditSkinHelp", Me.LocalResourceFile)
                    cmdUpdate.Text = Localization.GetString("cmdUpdateSkin", Me.LocalResourceFile)
            End Select

            If ModuleContext.PortalSettings.ActiveTab.IsSuperTab Then
                lblTitle.Text = String.Format(Localization.GetString("HostHeading", LocalResourceFile), Package.FriendlyName)
            Else
                lblTitle.Text = String.Format(Localization.GetString("AdminHeading", LocalResourceFile), Package.FriendlyName)
            End If

            cmdPackage.Visible = IsSuperTab
            cmdUpdate.Visible = IsSuperTab

            If Package IsNot Nothing Then
                If PackageEditor Is Nothing OrElse PackageID = Null.NullInteger Then
                    dshExtension.Visible = False
                    dshPackage.Visible = False
                    tblExtension.Visible = False
                Else
                    phEditor.Controls.Clear()
                    phEditor.Controls.Add(PackageEditor)

                    Dim moduleControl As IModuleControl = TryCast(PackageEditor, IModuleControl)
                    If moduleControl IsNot Nothing Then
                        moduleControl.ModuleContext.Configuration = Me.ModuleContext.Configuration
                    End If
                    If PackageEditor IsNot Nothing Then
                        PackageEditor.PackageID = PackageID
                        PackageEditor.Initialize()
                    End If
                End If

                'Bind Extension Info page
                ctlExtension.EditMode = ViewMode
                ctlExtension.DataSource = Package
                ctlExtension.DataBind()

                If Mode <> "All" Then
                    fldPackageType.Visible = False
                End If

                cmdDelete.Visible = IsSuperTab AndAlso (Not Package.IsSystemPackage) AndAlso _
                                        (PackageController.CanDeletePackage(Package, ModuleContext.PortalSettings))
                'add audit details
                ctlAudit.Entity = Package
            End If
        End Sub

        Private Sub UpdatePackage(ByVal displayMessage As Boolean)
            If ctlExtension.IsValid AndAlso ctlExtension.IsDirty Then
                Dim package As PackageInfo = TryCast(ctlExtension.DataSource, PackageInfo)
                If package IsNot Nothing Then
                    PackageController.UpdatePackage(package)
                End If

                If displayMessage Then
                    Skin.AddModuleMessage(Me, Localization.GetString("PackageUpdated", Me.LocalResourceFile), Skins.Controls.ModuleMessage.ModuleMessageType.GreenSuccess)
                End If
            End If

            If PackageEditor IsNot Nothing Then
                PackageEditor.UpdatePackage()
            End If
        End Sub


#End Region

#Region "Protected Methods"

        Protected Overrides Sub OnInit(ByVal e As System.EventArgs)
            MyBase.OnInit(e)
        End Sub

#End Region

#Region "Event Handlers"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Page_Load runs when the control is loaded.
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	08/15/2007	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            BindData()
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Package_Updated runs when a Package has been updated by a custom editor.
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	08/15/2007	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub Package_Updated(ByVal sender As Object, ByVal e As EventArgs)
        End Sub

        Protected Sub cmdCancel_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdCancel.Click
            Response.Redirect(NavigateURL())
        End Sub

        Protected Sub cmdDelete_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdDelete.Click
            Response.Redirect(Util.UnInstallURL(ModuleContext.TabId, PackageID))
        End Sub

        Protected Sub cmdPackage_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdPackage.Click
            Try
                UpdatePackage(False)
                Response.Redirect(Util.PackageWriterURL(ModuleContext, PackageID))
            Catch ex As Exception
                ProcessModuleLoadException(Me, ex)
            End Try
        End Sub

        Protected Sub cmdUpdate_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdUpdate.Click
            Try
                UpdatePackage(True)
                Response.Redirect(Request.RawUrl, True)
            Catch ex As Exception
                ProcessModuleLoadException(Me, ex)
            End Try
        End Sub

#End Region

    End Class

End Namespace
