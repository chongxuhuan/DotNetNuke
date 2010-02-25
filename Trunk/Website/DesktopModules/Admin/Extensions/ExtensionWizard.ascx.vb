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


Imports DotNetNuke.UI.Modules
Imports DotNetNuke.Services.Installer.Packages

Namespace DotNetNuke.Modules.Admin.Extensions

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The ExtensionWizard control is used to create a Extension
    ''' </summary>
    ''' <history>
    ''' 	[cnurse]	08/25/2008	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Partial Class ExtensionWizard
        Inherits ModuleUserControlBase

#Region "Private Members"

        Private _Control As Control
        Private _Package As PackageInfo

#End Region

#Region "Protected Properties"

        Protected ReadOnly Property ExtensionType() As String
            Get
                If Mode = "All" Then
                    Return cboExtensionType.Text
                Else
                    If Not String.IsNullOrEmpty(Request.QueryString("SkinType")) Then
                        Return Request.QueryString("SkinType")
                    Else
                        Return Mode
                    End If
                End If
            End Get
        End Property

        Protected ReadOnly Property Mode() As String
            Get
                Dim _Mode As String = "All"
                If Not String.IsNullOrEmpty(Request.QueryString("Type")) Then
                    _Mode = Request.QueryString("Type")
                End If
                Return _Mode
            End Get
        End Property

        Protected ReadOnly Property Package() As PackageInfo
            Get
                If _Package Is Nothing Then
                    If PackageID = Null.NullInteger Then
                        _Package = New PackageInfo()
                        _Package.PackageType = ExtensionType
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

        Protected Property PackageID() As Integer
            Get
                Dim _PackageID As Integer = Null.NullInteger
                If Not ViewState("PackageID") Is Nothing Then
                    _PackageID = Int32.Parse(ViewState("PackageID"))
                End If
                Return _PackageID
            End Get
            Set(ByVal value As Integer)
                ViewState("PackageID") = value
            End Set
        End Property

#End Region

#Region "Private Methods"

        Private Sub BindExtensionTypes()
            cboExtensionType.DataSource = PackageController.GetPackageTypes()
            cboExtensionType.DataBind()
        End Sub

        Private Sub BindPackageEditor()
            phEditor.Controls.Clear()
            phEditor.Controls.Add(PackageEditor)

            Dim moduleControl As IModuleControl = TryCast(PackageEditor, IModuleControl)
            If moduleControl IsNot Nothing Then
                moduleControl.ModuleContext.Configuration = Me.ModuleContext.Configuration
            End If
            If PackageEditor IsNot Nothing Then
                PackageEditor.PackageID = PackageID
                PackageEditor.Initialize()
                PackageEditor.IsWizard = True
            End If
        End Sub

        Private Sub DisplayLanguageHelp()
            Select Case cboExtensionType.SelectedValue
                Case "CoreLanguagePack"
                    lblLanguageHelp.Visible = True
                    lblExtensionLanguageHelp.Visible = False
                Case "ExtensionLanguagePack"
                    lblLanguageHelp.Visible = True
                    lblExtensionLanguageHelp.Visible = True
                Case Else
                    lblLanguageHelp.Visible = False
                    lblExtensionLanguageHelp.Visible = False
            End Select
        End Sub

#End Region

#Region "Protected Methods"

        Protected Function GetText(ByVal type As String) As String
            Dim text As String = Null.NullString
            Dim pageName As String = wizNewExtension.ActiveStep.Title
            If wizNewExtension.ActiveStepIndex = 1 Then
                If String.IsNullOrEmpty(Package.PackageType) Then
                    pageName += "_" + Mode
                Else
                    pageName += "_" + Package.PackageType
                End If
            End If
            If type = "Title" Then
                text = Localization.GetString(pageName + ".Title", Me.LocalResourceFile)
            ElseIf type = "Help" Then
                text = Localization.GetString(pageName + ".Help", Me.LocalResourceFile)
            End If
            Return text
        End Function

#End Region

#Region "Event Handlers"

        Protected Overrides Sub OnInit(ByVal e As System.EventArgs)
            MyBase.OnInit(e)

            Select Case Mode
                Case "All"
                    trExtensionType.Visible = True
                Case "Module"
                    trExtensionType.Visible = False
                Case "CoreLanguagePack, ExtensionLanguagePack"
                    trExtensionType.Visible = False
                Case "Skin", "Container"
                    trExtensionType.Visible = False
            End Select
        End Sub

        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
            MyBase.OnLoad(e)
            wizNewExtension.CancelButtonText = "<img src=""" + ApplicationPath + "/images/lt.gif"" border=""0"" /> " + Localization.GetString("Cancel", Me.LocalResourceFile)
            wizNewExtension.StartNextButtonText = "<img src=""" + ApplicationPath + "/images/rt.gif"" border=""0"" /> " + Localization.GetString("Next", Me.LocalResourceFile)

            If wizNewExtension.ActiveStepIndex < 2 Then
                wizNewExtension.StepNextButtonText = "<img src=""" + ApplicationPath + "/images/rt.gif"" border=""0"" /> " + Localization.GetString("Next", Me.LocalResourceFile)
            Else
                wizNewExtension.StepNextButtonText = "<img src=""" + ApplicationPath + "/images/rt.gif"" border=""0"" /> " + Localization.GetString("Finish", Me.LocalResourceFile)
            End If
            wizNewExtension.StepPreviousButtonText = "<img src=""" + ApplicationPath + "/images/lt.gif"" border=""0"" /> " + Localization.GetString("Previous", Me.LocalResourceFile)
            wizNewExtension.FinishCompleteButtonText = "<img src=""" + ApplicationPath + "/images/lt.gif"" border=""0"" /> " + Localization.GetString("Cancel", Me.LocalResourceFile)

            ctlExtension.LocalResourceFile = Me.LocalResourceFile
            ctlExtension.DataSource = Package
            ctlExtension.DataBind()

            'Bind the Owner control
            ctlOwner.LocalResourceFile = Me.LocalResourceFile
            ctlOwner.DataSource = Package
            ctlOwner.DataBind()

            If Package IsNot Nothing Then
                If PackageEditor IsNot Nothing AndAlso PackageID > Null.NullInteger Then
                    BindPackageEditor()
                End If
            End If

            If Not Page.IsPostBack Then
                BindExtensionTypes()
            End If

            DisplayLanguageHelp()
        End Sub

        Protected Sub wizInstall_ActiveStepChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles wizNewExtension.ActiveStepChanged
            Select Case wizNewExtension.ActiveStepIndex
                Case 1
                    If Package.PackageType <> "Module" AndAlso Package.PackageType <> "CoreLanguagePack" _
                                                       AndAlso Package.PackageType <> "ExtensionLanguagePack" Then
                        wizNewExtension.ActiveStepIndex = 2
                    End If
                Case 2
                    wizNewExtension.DisplayCancelButton = False
            End Select
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' wizNewExtension_CancelButtonClick runs when the Cancel Button on the Wizard is clicked.
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	08/25/2008	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Sub wizNewExtension_CancelButtonClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles wizNewExtension.CancelButtonClick
            'Redirect to Definitions page
            Response.Redirect(NavigateURL(), True)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' wizNewExtension_NextButtonClick when the next Button is clicked.  It provides
        '''	a mechanism for cancelling the page change if certain conditions aren't met.
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	08/25/2008	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Sub wizNewExtension_NextButtonClick(ByVal sender As Object, ByVal e As WizardNavigationEventArgs) Handles wizNewExtension.NextButtonClick
            Select Case e.CurrentStepIndex
                Case 0
                    If ctlExtension.IsValid And ctlExtension.IsDirty Then
                        Dim newPackage As PackageInfo = TryCast(ctlExtension.DataSource, PackageInfo)
                        Dim tmpPackage As PackageInfo = PackageController.GetPackageByName(newPackage.Name)
                        If tmpPackage Is Nothing Then
                            Select Case Mode
                                Case "All"
                                    newPackage.PackageType = cboExtensionType.SelectedValue
                                Case Else
                                    newPackage.PackageType = Mode
                            End Select
                            PackageID = PackageController.AddPackage(newPackage, True)
                        Else
                            e.Cancel = True
                            lblError.Text = String.Format(Localization.GetString("DuplicateName", Me.LocalResourceFile), newPackage.Name)
                        End If
                    End If

                    If PackageEditor IsNot Nothing AndAlso PackageID > Null.NullInteger Then
                        BindPackageEditor()
                    End If
                Case 1
                    If PackageEditor IsNot Nothing Then
                        PackageEditor.UpdatePackage()
                    End If
                Case 2
                    If ctlOwner.IsValid And ctlOwner.IsDirty Then
                        PackageController.SavePackage(TryCast(ctlOwner.DataSource, PackageInfo))
                    End If

                    Response.Redirect(NavigateURL(), True)
            End Select
        End Sub

#End Region

    End Class

End Namespace
