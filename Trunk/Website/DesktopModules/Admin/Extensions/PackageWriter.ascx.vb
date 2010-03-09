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
Imports System.Xml

Imports DotNetNuke.Services.Localization
Imports DotNetNuke.Services.Installer
Imports DotNetNuke.UI.Modules
Imports DotNetNuke.Services.Installer.Packages
Imports DotNetNuke.Services.Installer.Writers
Imports System.Xml.XPath

Namespace DotNetNuke.Modules.Admin.Extensions

    ''' -----------------------------------------------------------------------------
    ''' Project	 : DotNetNuke
    ''' Class	 : PackageWriter
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Supplies the functionality for creating Extension packages
    ''' </summary>
    ''' <history>
    '''     [cnurse]   01/31/2008    Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Partial Class PackageWriter
        Inherits ModuleUserControlBase

#Region "Members"

        Private _Package As PackageInfo = Nothing
        Private _Writer As PackageWriterBase

#End Region

#Region "Public Properties"

        Public Property PackageID() As Integer
            Get
                Dim _PageNo As Integer = 0
                If Not ViewState("PackageID") Is Nothing Then
                    _PageNo = CInt(ViewState("PackageID"))
                End If
                Return _PageNo
            End Get
            Set(ByVal Value As Integer)
                ViewState("PackageID") = Value
            End Set
        End Property

        Public ReadOnly Property Package() As PackageInfo
            Get
                If _Package Is Nothing AndAlso PackageID > Null.NullInteger Then
                    _Package = PackageController.GetPackage(PackageID)
                End If
                Return _Package
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the Return Url
        ''' </summary>
        ''' <history>
        '''     [cnurse]   07/31/2007    Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public ReadOnly Property ReturnURL() As String
            Get
                Dim TabID As Integer = ModuleContext.PortalSettings.HomeTabId

                If Not Request.Params("rtab") Is Nothing Then
                    TabID = Integer.Parse(Request.Params("rtab"))
                End If
                Return NavigateURL(TabID)
            End Get
        End Property

#End Region

#Region "Private Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' This routine checks the Access Security
        ''' </summary>
        ''' <history>
        '''     [cnurse]   07/26/2007    Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub CheckSecurity()
            If Not ModuleContext.PortalSettings.UserInfo.IsSuperUser Then
                Response.Redirect(NavigateURL("Access Denied"), True)
            End If
        End Sub

        Private Sub CreateManifest()
            'Update the Writers files collections
            '_Writer.Files.Clear()
            '_Writer.AppCodeFiles.Clear()
            For Each fileName As String In txtFiles.Text.Split(vbCrLf)
                fileName = fileName.Trim()
                If Not String.IsNullOrEmpty(fileName) Then
                    Dim file As New InstallFile(fileName)
                    _Writer.AddFile(file)
                End If
            Next

            '_Writer.Assemblies.Clear()
            For Each fileName As String In txtAssemblies.Text.Split(vbCrLf)
                fileName = fileName.Trim()
                If Not String.IsNullOrEmpty(fileName) Then
                    Dim file As New InstallFile(fileName)
                    _Writer.AddFile(file)
                End If
            Next

            txtManifest.Text = _Writer.WriteManifest(False)
        End Sub

        Private Sub CreatePackage()
            Dim manifestName As String = txtManifestName.Text
            If String.IsNullOrEmpty(manifestName) Then
                manifestName = txtArchiveName.Text.ToLower().Replace("zip", "dnn")
            End If
            If chkPackage.Checked Then
                'Use the installer to parse the manifest and load the files that need to be packaged
                Dim installer As New Installer(Package, Request.MapPath("."))
                For Each file As InstallFile In installer.InstallerInfo.Files.Values
                    _Writer.AddFile(file)
                Next
                Dim basePath As String
                Select Case Package.PackageType
                    Case "Auth_System"
                        basePath = InstallMapPath + ("AuthSystem")
                    Case "Container"
                        basePath = InstallMapPath + ("Container")
                    Case "CoreLanguagePack", "ExtensionLanguagePack"
                        basePath = InstallMapPath + ("Language")
                    Case "Module"
                        basePath = InstallMapPath + ("Module")
                    Case "Provider"
                        basePath = InstallMapPath + ("Provider")
                    Case "Skin"
                        basePath = InstallMapPath + ("Skin")
                    Case Else
                        basePath = HostMapPath
                End Select

                If Not manifestName.EndsWith(".dnn") Then
                    manifestName += ".dnn"
                End If

                If Not txtArchiveName.Text.EndsWith(".zip") Then
                    txtArchiveName.Text += ".zip"
                End If

                _Writer.CreatePackage(Path.Combine(basePath, txtArchiveName.Text), manifestName, Package.Manifest, True)

                Skins.Skin.AddModuleMessage(Me, String.Format(Localization.GetString("Success", Me.LocalResourceFile), Me.ModuleContext.PortalSettings.PortalAlias.HTTPAlias & basePath.Replace(ApplicationMapPath, "").Replace("\", "/")), Skins.Controls.ModuleMessage.ModuleMessageType.GreenSuccess)
            ElseIf chkManifest.Checked Then
                _Writer.WriteManifest(manifestName, Package.Manifest)
            End If

            phInstallLogs.Controls.Add(_Writer.Log.GetLogsTable)
        End Sub

        Private Sub GetAssemblies(ByVal refreshList As Boolean)
            GetFiles(String.IsNullOrEmpty(txtFiles.Text))
            If refreshList Then
                txtAssemblies.Text = Null.NullString
                For Each file As InstallFile In _Writer.Assemblies.Values
                    txtAssemblies.Text += file.FullName + vbCrLf
                Next
            End If
        End Sub

        Private Sub GetFiles(ByVal refreshList As Boolean)
            _Writer.GetFiles(chkIncludeSource.Checked)
            If refreshList Then
                txtFiles.Text = Null.NullString
                'Display App Code files
                For Each file As InstallFile In _Writer.AppCodeFiles.Values
                    txtFiles.Text += "[app_code]" + file.FullName + vbCrLf
                Next
                'Display Script files
                For Each file As InstallFile In _Writer.Scripts.Values
                    txtFiles.Text += file.FullName + vbCrLf
                Next
                'Display regular files
                For Each file As InstallFile In _Writer.Files.Values
                    txtFiles.Text += file.FullName + vbCrLf
                Next
            End If
        End Sub

#End Region

#Region "Protected Methods"

        Protected Function GetText(ByVal type As String) As String
            Dim text As String = Null.NullString
            If type = "Title" Then
                text = Localization.GetString(wizPackage.ActiveStep.Title + ".Title", Me.LocalResourceFile)
            ElseIf type = "Help" Then
                text = Localization.GetString(wizPackage.ActiveStep.Title + ".Help", Me.LocalResourceFile)
            End If
            Return text
        End Function

#End Region

#Region "Event Handlers"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Page_Init runs when the control is initialised.
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        '''     [cnurse]   01/31/2008    Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            If Not (Request.QueryString("packageid") Is Nothing) Then
                PackageID = Int32.Parse(Request.QueryString("packageid"))
            Else
                PackageID = Null.NullInteger
            End If
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The Page_Load runs when the page loads
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        '''     [cnurse]   01/31/2008    Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Try
                CheckSecurity()

                ctlPackage.EditMode = UI.WebControls.PropertyEditorMode.View
                ctlPackage.DataSource = Package
                ctlPackage.DataBind()

                wizPackage.CancelButtonText = "<img src=""" + ApplicationPath + "/images/lt.gif"" border=""0"" /> " + Localization.GetString("Cancel", Me.LocalResourceFile)
                wizPackage.StartNextButtonText = "<img src=""" + ApplicationPath + "/images/rt.gif"" border=""0"" /> " + Localization.GetString("Next", Me.LocalResourceFile)
                wizPackage.StepNextButtonText = "<img src=""" + ApplicationPath + "/images/rt.gif"" border=""0"" /> " + Localization.GetString("Next", Me.LocalResourceFile)
                wizPackage.StepPreviousButtonText = "<img src=""" + ApplicationPath + "/images/lt.gif"" border=""0"" /> " + Localization.GetString("Previous", Me.LocalResourceFile)
                wizPackage.FinishCompleteButtonText = "<img src=""" + ApplicationPath + "/images/lt.gif"" border=""0"" /> " + Localization.GetString("Cancel", Me.LocalResourceFile)

                _Writer = PackageWriterFactory.GetWriter(Package)

                If Page.IsPostBack Then
                    _Writer.BasePath = txtBasePath.Text
                Else
                    txtBasePath.Text = _Writer.BasePath

                    'Load Manifests
                    If Not String.IsNullOrEmpty(Package.Manifest) Then
                        cboManifests.Items.Add(New ListItem("Database version", ""))
                    End If
                    Dim filePath As String = Server.MapPath(_Writer.BasePath)
                    If Not String.IsNullOrEmpty(filePath) Then
                        If Directory.Exists(filePath) Then
                            For Each file As String In Directory.GetFiles(filePath, "*.dnn")
                                Dim fileName As String = file.Replace(filePath + "\", "")
                                cboManifests.Items.Add(New ListItem(fileName, fileName))
                            Next
                            For Each file As String In Directory.GetFiles(filePath, "*.dnn.resources")
                                Dim fileName As String = file.Replace(filePath + "\", "")
                                cboManifests.Items.Add(New ListItem(fileName, fileName))
                            Next
                        End If
                    End If

                    If cboManifests.Items.Count > 0 Then
                        trUseManifest.Visible = True
                    End If
                End If

            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        Protected Sub chkUseManifest_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkUseManifest.CheckedChanged
            trManifestList.Visible = chkUseManifest.Checked
        End Sub

        Protected Sub cmdGetFiles_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdGetFiles.Click
            GetFiles(True)
        End Sub

        Protected Sub wizPackage_ActiveStepChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles wizPackage.ActiveStepChanged
            Select Case wizPackage.ActiveStepIndex
                Case 1    'Display the files
                    If chkUseManifest.Checked Then
                        wizPackage.ActiveStepIndex = 3
                    End If
                    GetFiles(String.IsNullOrEmpty(txtFiles.Text))
                    chkIncludeSource.Visible = _Writer.HasProjectFile OrElse _Writer.AppCodeFiles.Count > 0
                Case 2 'Display the assemblies
                    If _Writer.IncludeAssemblies Then
                        GetAssemblies(String.IsNullOrEmpty(txtAssemblies.Text))
                    Else
                        wizPackage.ActiveStepIndex = 3
                    End If
                Case 3 ' Display the manfest
                    If chkUseManifest.Checked Then
                        If String.IsNullOrEmpty(cboManifests.SelectedValue) Then
                            'Use Database
                            Dim sb As New StringBuilder
                            Dim settings As New XmlWriterSettings()
                            settings.ConformanceLevel = ConformanceLevel.Fragment
                            settings.OmitXmlDeclaration = True
                            settings.Indent = True

                            _Writer.WriteManifest(XmlWriter.Create(sb, settings), Package.Manifest)

                            txtManifest.Text = sb.ToString()
                        Else
                            Dim filename As String = Path.Combine(Server.MapPath(_Writer.BasePath), cboManifests.SelectedValue)
                            Dim objStreamReader As StreamReader = File.OpenText(filename)
                            txtManifest.Text = objStreamReader.ReadToEnd()
                        End If
                    Else
                        CreateManifest()
                    End If

                    If Not chkReviewManifest.Checked Then
                        wizPackage.ActiveStepIndex = 4
                    End If
                Case 4
                    txtManifestName.Text = Package.Owner + "_" + Package.Name
                    If chkUseManifest.Checked Then
                        txtArchiveName.Text = Package.Owner + "_" + Package.Name + "_" + FormatVersion(Package.Version) + "_Install.zip"
                        chkManifest.Checked = True
                        trManifest1.Visible = False
                        trManifest2.Visible = False
                    Else
                        If chkIncludeSource.Checked Then
                            txtArchiveName.Text = Package.Owner + "_" + Package.Name + "_" + FormatVersion(Package.Version) + "_Source.zip"
                        Else
                            txtArchiveName.Text = Package.Owner + "_" + Package.Name + "_" + FormatVersion(Package.Version) + "_Install.zip"
                        End If
                    End If
                    If Not txtManifestName.Text.ToLower.EndsWith(".dnn") Then txtManifestName.Text = txtManifestName.Text & ".dnn"
                    wizPackage.DisplayCancelButton = False
            End Select
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' wizPackage_CancelButtonClick runs when the Cancel Button on the Wizard is clicked.
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        '''     [cnurse]   02/01/2008    Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Sub wizPackage_CancelButtonClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles wizPackage.CancelButtonClick
            Try
                'Redirect to Definitions page
                Response.Redirect(ReturnURL(), True)
            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' wizPackage_FinishButtonClick runs when the Finish Button on the Wizard is clicked.
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        '''     [cnurse]   02/01/2008    Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Sub wizPackage_FinishButtonClick(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.WizardNavigationEventArgs) Handles wizPackage.FinishButtonClick
            Try
                'Redirect to Definitions page
                Response.Redirect(ReturnURL(), True)
            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' wizPackage_NextButtonClick runs when the next Button is clicked.  It provides
        '''	a mechanism for cancelling the page change if certain conditions aren't met.
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        '''     [cnurse]   01/31/2008    Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Sub wizPackage_NextButtonClick(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.WizardNavigationEventArgs) Handles wizPackage.NextButtonClick

            Select Case e.CurrentStepIndex
                Case 3 ' Save the Manifest
                    Dim doc As XPathDocument = New XPathDocument(New StringReader(txtManifest.Text))
                    Dim nav As XPathNavigator = doc.CreateNavigator()
                    Dim packageNav As XPathNavigator = nav.SelectSingleNode("dotnetnuke/packages")

                    Package.Manifest = packageNav.InnerXml
                    PackageController.SavePackage(Package)
                Case 4
                    If chkManifest.Checked AndAlso (Not String.IsNullOrEmpty(txtManifestName.Text)) AndAlso (Not txtManifestName.Text.ToLower.EndsWith(".dnn")) Then
                        lblMessage.Text = Localization.GetString("InvalidManifestExtension", Me.LocalResourceFile)
                        e.Cancel = True
                    ElseIf chkPackage.Checked AndAlso String.IsNullOrEmpty(txtArchiveName.Text) Then
                        lblMessage.Text = Localization.GetString("NoFileName", Me.LocalResourceFile)
                        e.Cancel = True
                    ElseIf chkPackage.Checked AndAlso Not txtArchiveName.Text.ToLower.EndsWith(".zip") Then
                        lblMessage.Text = Localization.GetString("InvalidPackageName", Me.LocalResourceFile)
                        e.Cancel = True
                    Else
                        'Create the Package
                        CreatePackage()
                    End If

            End Select
        End Sub

#End Region

    End Class

End Namespace
