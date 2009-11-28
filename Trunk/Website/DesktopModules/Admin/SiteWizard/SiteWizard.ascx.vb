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

Imports System.IO
Imports System.Drawing.Imaging
Imports System.XML
Imports DotNetNuke.UI.Skins
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.UI.UserControls


Namespace DotNetNuke.Modules.Admin.Portals

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The SiteWizard Wizard is a user-friendly Wizard that leads the user through the
    '''	process of setting up a new site
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[cnurse]	10/8/2004	created
    '''     [cnurse]    12/04/2006  converted to use ASP.NET 2 Wizard classes
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Partial Public Class SiteWizard
        Inherits PortalModuleBase

        Public Enum ContainerType
            Host = 0
            Portal = 1
            Folder = 2
            All = 3
        End Enum

#Region "Private Members"


#End Region

#Region "Private Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' BindContainers manages the containers
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	12/15/2004	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub BindContainers()

            ctlPortalContainer.Clear()

            If chkIncludeAll.Checked Then
                GetContainers(ContainerType.All, "", "")
            Else
                If ctlPortalSkin.SkinSrc <> "" Then
                    Dim strFolder As String
                    Dim strContainerFolder As String = ctlPortalSkin.SkinSrc.Substring(0, ctlPortalSkin.SkinSrc.LastIndexOf("/"))
                    If strContainerFolder.StartsWith("[G]") Then
                        strContainerFolder = strContainerFolder.Replace("[G]Skins/", "Containers\")
                        strFolder = Common.Globals.HostMapPath & strContainerFolder
                        GetContainers(ContainerType.Folder, "[G]", strFolder)
                    Else
                        strContainerFolder = strContainerFolder.Replace("[L]Skins/", "Containers\")
                        strFolder = PortalSettings.HomeDirectoryMapPath & strContainerFolder
                        GetContainers(ContainerType.Folder, "[L]", strFolder)
                    End If
                Else
                    GetContainers(ContainerType.Portal, "", "")
                End If
            End If

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' GetContainers gets the containers and binds the lists to the controls
        '''	the buttons
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <param name="type">An enum indicating what type of containers to load</param>
        ''' <param name="skinType">A string that identifies whether the skin is Host "[G]" or Site "[L]"</param>
        ''' <param name="strFolder">The folder to search for skins</param>
        ''' <history>
        ''' 	[cnurse]	12/14/2004	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub GetContainers(ByVal type As ContainerType, ByVal skinType As String, ByVal strFolder As String)

            Dim objSkins As New UI.Skins.SkinController

            'Configure SkinControl
            ctlPortalContainer.Width = "500px"
            ctlPortalContainer.Height = "250px"
            ctlPortalContainer.Border = "black 1px solid"
            ctlPortalContainer.Columns = 3
            ctlPortalContainer.SkinRoot = SkinController.RootContainer
            Select Case type
                Case ContainerType.Folder
                    ctlPortalContainer.LoadSkins(strFolder, skinType, False)
                Case ContainerType.Portal
                    ctlPortalContainer.LoadPortalSkins(False)
                Case ContainerType.Host
                    ctlPortalContainer.LoadHostSkins(False)
                Case ContainerType.All
                    ctlPortalContainer.LoadAllSkins(False)
            End Select

            'Get current container and set selected skin
            'objSkin = objSkins.GetSkin(SkinInfo.RootContainer, PortalId, DotNetNuke.UI.Skins.SkinType.Portal)
            'If Not objSkin Is Nothing Then
            '    If objSkin.PortalId = PortalId Then
            '        ctlPortalContainer.SkinSrc = objSkin.SkinSrc
            '    End If
            'End If

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' GetSkins gets the skins and containers and binds the lists to the controls
        '''	the buttons
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	11/04/2004	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub GetSkins()

            Dim objSkins As New UI.Skins.SkinController

            'Configure SkinControl
            ctlPortalSkin.Width = "500px"
            ctlPortalSkin.Height = "250px"
            ctlPortalSkin.Border = "black 1px solid"
            ctlPortalSkin.Columns = 3
            ctlPortalSkin.SkinRoot = SkinController.RootSkin
            ctlPortalSkin.LoadAllSkins(False)

            'Get current skin and set selected skin
            'objSkin = objSkins.GetSkin(SkinInfo.RootSkin, PortalId, SkinType.Portal)
            'If Not objSkin Is Nothing Then
            '    If objSkin.PortalId = PortalId Then
            '        ctlPortalSkin.SkinSrc = objSkin.SkinSrc
            '    End If
            'End If

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' GetTemplates gets the skins and containers and binds the lists to the control
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	11/04/2004	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub GetTemplates()

            Dim strFolder As String
            Dim strFileName As String

            strFolder = Common.Globals.HostMapPath
            If System.IO.Directory.Exists(strFolder) Then
                ' admin.template and a portal template are required at minimum
                Dim fileEntries As String() = System.IO.Directory.GetFiles(strFolder, "*.template")
                'Me.EnableCommand(WizardCommand.NextPage, False)
                'Me.EnableCommand(WizardCommand.Finish, False)

                For Each strFileName In fileEntries
                    If Path.GetFileNameWithoutExtension(strFileName) = "admin" Then
                        'Me.EnableCommand(WizardCommand.NextPage, True)
                        'Me.EnableCommand(WizardCommand.Finish, True)
                    Else
                        lstTemplate.Items.Add(Path.GetFileNameWithoutExtension(strFileName))
                    End If
                Next

                If lstTemplate.Items.Count = 0 Then
                    'Me.EnableCommand(WizardCommand.NextPage, False)
                    'Me.EnableCommand(WizardCommand.Finish, False)
                End If
            End If

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' UseTemplate sets the page ready to select a Template
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	11/04/2004	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub UseTemplate()
            lstTemplate.Enabled = chkTemplate.Checked
            optMerge.Enabled = chkTemplate.Checked
            lblMergeTitle.Enabled = chkTemplate.Checked
            lblMergeWarning.Enabled = chkTemplate.Checked
            lblTemplateMessage.Text = ""
        End Sub

#End Region

#Region "Public Methods"

#End Region

#Region "Event Handlers"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Page_Load runs when the control is loaded
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	10/11/2004	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Try
                Wizard.StartNextButtonText = "<img src=""" + ApplicationPath + "/images/rt.gif"" border=""0"" /> " + Localization.GetString("Next", Me.LocalResourceFile)
                Wizard.StepNextButtonText = "<img src=""" + ApplicationPath + "/images/rt.gif"" border=""0"" /> " + Localization.GetString("Next", Me.LocalResourceFile)
                Wizard.StepPreviousButtonText = "<img src=""" + ApplicationPath + "/images/lt.gif"" border=""0"" /> " + Localization.GetString("Previous", Me.LocalResourceFile)
                Wizard.FinishPreviousButtonText = "<img src=""" + ApplicationPath + "/images/lt.gif"" border=""0"" /> " + Localization.GetString("Previous", Me.LocalResourceFile)
                Wizard.FinishCompleteButtonText = "<img src=""" + ApplicationPath + "/images/save.gif"" border=""0"" /> " + Localization.GetString("Finish", Me.LocalResourceFile)
                If Not Page.IsPostBack Then

                    'Get Templates for Page 1
                    GetTemplates()
                    chkTemplate.Checked = False
                    lstTemplate.Enabled = False

                    'Get Skins for Pages 2
                    GetSkins()

                    'Get Details for Page 4
                    Dim objPortalController As New PortalController
                    Dim objPortal As PortalInfo = objPortalController.GetPortal(PortalId)
                    txtPortalName.Text = objPortal.PortalName
                    txtDescription.Text = objPortal.Description
                    txtKeyWords.Text = objPortal.KeyWords

                    'Get Details for Page 5
                    urlLogo.Url = objPortal.LogoFile
                    urlLogo.FileFilter = glbImageFileTypes

                    UseTemplate()
                End If


            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' chkIncludeAll_CheckedChanged runs when include all containers checkbox status is changed
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	12/15/2004	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub chkIncludeAll_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkIncludeAll.CheckedChanged
            BindContainers()
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' chkTemplate_CheckedChanged runs when use template checkbox status is changed
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	10/13/2004	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub chkTemplate_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkTemplate.CheckedChanged

            If chkTemplate.Checked Then
                lstTemplate.SelectedIndex = -1
            End If

            UseTemplate()

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' lstTemplate_SelectedIndexChanged runs when the selected template is changed
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	11/04/2004	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub lstTemplate_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles lstTemplate.SelectedIndexChanged

            If lstTemplate.SelectedIndex > -1 Then
                Dim xmlDoc As New XmlDocument
                Dim node As XmlNode
                Dim strTemplatePath As String = Common.Globals.HostMapPath
                Dim strTemplateFile As String = lstTemplate.SelectedItem.Text & ".template"

                ' open the XML file
                Try
                    xmlDoc.Load(strTemplatePath & strTemplateFile)
                    node = xmlDoc.SelectSingleNode("//portal/description")
                    If Not node Is Nothing Then
                        lblTemplateMessage.Text = node.InnerText
                    Else
                        lblTemplateMessage.Text = ""
                    End If
                Catch    ' error
                    lblTemplateMessage.Text = "Error Loading Template description"
                End Try
            Else
                lblTemplateMessage.Text = ""
            End If
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Wizard_ActiveStepChanged runs when the Wizard page has been changed
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	12/04/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Sub Wizard_ActiveStepChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles Wizard.ActiveStepChanged

            Select Case Wizard.ActiveStepIndex
                Case 3    'Containers
                    BindContainers()
            End Select
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Wizard_FinishButtonClick runs when the Finish Button on the Wizard is clicked.
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	10/12/2004	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Sub Wizard_FinishButtonClick(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.WizardNavigationEventArgs) Handles Wizard.FinishButtonClick
            Dim objPortalController As New PortalController

            ' use Portal Template to update portal content pages
            If lstTemplate.SelectedIndex <> -1 Then
                Dim strTemplateFile As String = lstTemplate.SelectedItem.Text & ".template"

                ' process zip resource file if present
                objPortalController.ProcessResourceFile(PortalSettings.HomeDirectoryMapPath, Common.Globals.HostMapPath & strTemplateFile)

                'Process Template
                Select Case optMerge.SelectedValue
                    Case "Ignore"
                        objPortalController.ParseTemplate(PortalId, Common.Globals.HostMapPath, strTemplateFile, PortalSettings.AdministratorId, PortalTemplateModuleAction.Ignore, False)
                    Case "Replace"
                        objPortalController.ParseTemplate(PortalId, Common.Globals.HostMapPath, strTemplateFile, PortalSettings.AdministratorId, PortalTemplateModuleAction.Replace, False)
                    Case "Merge"
                        objPortalController.ParseTemplate(PortalId, Common.Globals.HostMapPath, strTemplateFile, PortalSettings.AdministratorId, PortalTemplateModuleAction.Merge, False)
                End Select
            End If

            ' update Portal info in the database
            Dim objPortal As PortalInfo = objPortalController.GetPortal(PortalId)
            objPortal.Description = txtDescription.Text
            objPortal.KeyWords = txtKeyWords.Text
            objPortal.PortalName = txtPortalName.Text
            objPortal.LogoFile = urlLogo.Url
            objPortalController.UpdatePortalInfo(objPortal)

            'Set Portal Skin
            SkinController.SetSkin(SkinController.RootSkin, PortalId, SkinType.Portal, ctlPortalSkin.SkinSrc)
            SkinController.SetSkin(SkinController.RootSkin, PortalId, SkinType.Admin, ctlPortalSkin.SkinSrc)

            'Set Portal Container
            SkinController.SetSkin(SkinController.RootContainer, PortalId, SkinType.Portal, ctlPortalContainer.SkinSrc)
            SkinController.SetSkin(SkinController.RootContainer, PortalId, SkinType.Admin, ctlPortalContainer.SkinSrc)

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Wizard_NextButtonClickruns when the next Button is clicked.  It provides
        '''	a mechanism for cancelling the page change if certain conditions aren't met.
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	12/04/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Sub Wizard_NextButtonClick(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.WizardNavigationEventArgs) Handles Wizard.NextButtonClick

            Dim strMessage As String

            Select Case e.CurrentStepIndex
                Case 1    'Templates
                    'Before we leave Page 1, the user must have selected a Portal
                    If lstTemplate.SelectedIndex = -1 Then
                        If chkTemplate.Checked Then
                            e.Cancel = True
                            lblTemplateMessage.Text = Services.Localization.Localization.GetString("TemplateRequired", Me.LocalResourceFile)
                        End If
                    Else
                        'Check Template Validity before proceeding
                        Dim schemaFilename As String = Server.MapPath("DesktopModules/Admin/Portals/portal.template.xsd")
                        Dim xmlFilename As String = Common.Globals.HostMapPath & lstTemplate.SelectedItem.Text & ".template"
                        Dim xval As New PortalTemplateValidator
                        If Not xval.Validate(xmlFilename, schemaFilename) Then
                            strMessage = Services.Localization.Localization.GetString("InvalidTemplate", Me.LocalResourceFile)
                            lblTemplateMessage.Text = String.Format(strMessage, lstTemplate.SelectedItem.Text & ".template")
                            'Cancel Page move if invalid template
                            e.Cancel = True
                        End If
                    End If
            End Select
        End Sub

#End Region

    End Class

End Namespace

