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

Imports DotNetNuke.Common.Lists
Imports DotNetNuke.Entities.Profile

Imports DotNetNuke.Services.Localization

Imports DotNetNuke.UI.WebControls

Namespace DotNetNuke.Modules.Admin.Users

	''' -----------------------------------------------------------------------------
	''' <summary>
    ''' The EditProfileDefinition PortalModuleBase is used to manage a Profile Property
    ''' for a portal
	''' </summary>
    ''' <remarks>
	''' </remarks>
	''' <history>
    ''' 	[cnurse]	02/22/2006  Created
    ''' </history>
	''' -----------------------------------------------------------------------------
    Partial Class EditProfileDefinition
        Inherits Entities.Modules.PortalModuleBase

#Region "Private Members"

        Private _PropertyDefinition As ProfilePropertyDefinition
        Private ResourceFile As String = "~/DesktopModules/Admin/Security/App_LocalResources/Profile.ascx"
        Private _Message As String = Null.NullString

#End Region

#Region "Protected Members"

        Protected ReadOnly Property IsAddMode() As Boolean
            Get
                Return (PropertyDefinitionID = Null.NullInteger)
            End Get
        End Property

        Protected ReadOnly Property IsList() As Boolean
            Get
                Dim _IsList As Boolean = False
                Dim objListController As New ListController
                Dim dataType As ListEntryInfo = objListController.GetListEntryInfo(PropertyDefinition.DataType)

                If (Not dataType Is Nothing) AndAlso (dataType.ListName = "DataType") AndAlso (dataType.Value = "List") Then
                    _IsList = True
                End If

                Return _IsList
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets whether we are dealing with SuperUsers
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	05/11/2006  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected ReadOnly Property IsSuperUser() As Boolean
            Get
                If PortalSettings.ActiveTab.ParentId = PortalSettings.SuperTabId Then
                    Return True
                Else
                    Return False
                End If
            End Get
        End Property

        Protected ReadOnly Property PropertyDefinition() As ProfilePropertyDefinition
            Get
                If _PropertyDefinition Is Nothing Then
                    If IsAddMode Then
                        'Create New Property Definition
                        _PropertyDefinition = New ProfilePropertyDefinition
                        _PropertyDefinition.PortalId = UsersPortalId
                    Else
                        'Get Property Definition from Data Store
                        _PropertyDefinition = ProfileController.GetPropertyDefinition(PropertyDefinitionID, UsersPortalId)
                    End If
                End If
                Return _PropertyDefinition
            End Get
        End Property

        Protected Property PropertyDefinitionID() As Integer
            Get
                Dim _DefinitionID As Integer = Null.NullInteger
                If Not ViewState("PropertyDefinitionID") Is Nothing Then
                    _DefinitionID = Int32.Parse(ViewState("PropertyDefinitionID"))
                End If
                Return _DefinitionID
            End Get
            Set(ByVal value As Integer)
                ViewState("PropertyDefinitionID") = value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the Portal Id whose Users we are managing
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	05/11/2006  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected ReadOnly Property UsersPortalId() As Integer
            Get
                Dim intPortalId As Integer = PortalId
                If IsSuperUser Then
                    intPortalId = Null.NullInteger
                End If
                Return intPortalId
            End Get
        End Property

#End Region

#Region "Private Methods"

        Private Sub UpdateResourceFileNode(ByVal xmlDoc As XmlDocument, ByVal key As String, ByVal text As String)
            Dim node, nodeData As XmlNode
            Dim attr As XmlAttribute

            node = xmlDoc.SelectSingleNode("//root/data[@name='" + key + "']/value")
            If node Is Nothing Then
                ' missing entry
                nodeData = xmlDoc.CreateElement("data")
                attr = xmlDoc.CreateAttribute("name")
                attr.Value = key
                nodeData.Attributes.Append(attr)
                xmlDoc.SelectSingleNode("//root").AppendChild(nodeData)
                node = nodeData.AppendChild(xmlDoc.CreateElement("value"))
            End If
            node.InnerXml = Server.HtmlEncode(text)

        End Sub

        Private Sub BindLanguages()
            Me.txtPropertyName.Text = Localization.GetString("ProfileProperties_" + PropertyDefinition.PropertyName, ResourceFile, cboLocales.SelectedValue)
            Me.txtPropertyHelp.Text = Localization.GetString("ProfileProperties_" + PropertyDefinition.PropertyName + ".Help", ResourceFile, cboLocales.SelectedValue)
            Me.txtPropertyRequired.Text = Localization.GetString("ProfileProperties_" + PropertyDefinition.PropertyName + ".Required", ResourceFile, cboLocales.SelectedValue)
            Me.txtPropertyValidation.Text = Localization.GetString("ProfileProperties_" + PropertyDefinition.PropertyName + ".Validation", ResourceFile, cboLocales.SelectedValue)
            Me.txtCategoryName.Text = Localization.GetString("ProfileProperties_" + PropertyDefinition.PropertyCategory + ".Header", ResourceFile, cboLocales.SelectedValue)
        End Sub

        Private Sub BindList()
            If IsList Then
                lstEntries.Mode = "ListEntries"
                lstEntries.SelectedKey = PropertyDefinition.PropertyName
                lstEntries.ListPortalID = Me.UsersPortalId
                lstEntries.ShowDelete = False
                lstEntries.DataBind()
            End If
        End Sub

        Private Function GetResourceFile(ByVal type As String, ByVal language As String) As String
            Dim resourcefilename As String = ResourceFile + ".resx"
            If language <> Localization.SystemLocale Then
                resourcefilename = resourcefilename.Substring(0, resourcefilename.Length - 5) + "." + language + ".resx"
            End If

            If type = "Portal" Then
                resourcefilename = resourcefilename.Substring(0, resourcefilename.Length - 5) + "." + "Portal-" + PortalId.ToString + ".resx"
            ElseIf type = "Host" Then
                resourcefilename = resourcefilename.Substring(0, resourcefilename.Length - 5) + "." + "Host.resx"
            End If

            Return HttpContext.Current.Server.MapPath(resourcefilename)

        End Function

        Private Function ValidateProperty(ByVal definition As ProfilePropertyDefinition) As Boolean
            Dim isValid As Boolean = True

            Dim objListController As New Common.Lists.ListController
            Dim strDataType As String = objListController.GetListEntryInfo(definition.DataType).Value

            Select Case strDataType
                Case "Text"
                    If definition.Required AndAlso definition.Length = 0 Then
                        _Message = "RequiredTextBox"
                        isValid = Null.NullBoolean
                    End If
            End Select
            Return isValid
        End Function

#End Region

#Region "Public Methods"

        Public Function GetText(ByVal type As String) As String
            Dim text As String = Null.NullString
            If IsAddMode And Wizard.ActiveStepIndex = 0 Then
                If type = "Title" Then
                    text = Localization.GetString(Wizard.ActiveStep.Title + "_Add.Title", Me.LocalResourceFile)
                ElseIf type = "Help" Then
                    text = Localization.GetString(Wizard.ActiveStep.Title + "_Add.Help", Me.LocalResourceFile)
                End If
            Else
                If type = "Title" Then
                    text = Localization.GetString(Wizard.ActiveStep.Title + ".Title", Me.LocalResourceFile)
                ElseIf type = "Help" Then
                    text = Localization.GetString(Wizard.ActiveStep.Title + ".Help", Me.LocalResourceFile)
                End If
            End If
            Return text
        End Function

#End Region

#Region "Event Handlers"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Page_Init runs when the control is initialised
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	02/22/2006  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
            'Set the List Entries Control Properties
            lstEntries.ID = "ListEntries"

            'Get Property Definition Id from Querystring
            If PropertyDefinitionID = Null.NullInteger Then
                If Not (Request.QueryString("PropertyDefinitionId") Is Nothing) Then
                    PropertyDefinitionID = Int32.Parse(Request.QueryString("PropertyDefinitionId"))
                End If
            End If

            If IsAddMode Then
                Me.ModuleConfiguration.ModuleTitle = Services.Localization.Localization.GetString("AddProperty", Me.LocalResourceFile)
            Else
                Me.ModuleConfiguration.ModuleTitle = Services.Localization.Localization.GetString("EditProperty", Me.LocalResourceFile)
            End If
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Page_Load runs when the control is loaded
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	02/22/2006  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Try

                Wizard.CancelButtonText = "<img src=""" + ApplicationPath + "/images/lt.gif"" border=""0"" /> " + Localization.GetString("cmdCancel", Me.LocalResourceFile)
                Wizard.StartNextButtonText = "<img src=""" + ApplicationPath + "/images/rt.gif"" border=""0"" /> " + Localization.GetString("Next", Me.LocalResourceFile)
                Wizard.StepNextButtonText = "<img src=""" + ApplicationPath + "/images/rt.gif"" border=""0"" /> " + Localization.GetString("Next", Me.LocalResourceFile)
                Wizard.FinishCompleteButtonText = "<img src=""" + ApplicationPath + "/images/lt.gif"" border=""0"" /> " + Localization.GetString("cmdCancel", Me.LocalResourceFile)

                If Not Page.IsPostBack Then
                    Localization.LoadCultureDropDownList(cboLocales, CultureDropDownTypes.NativeName, CType(Page, PageBase).PageCulture.Name)
                    If cboLocales.SelectedItem IsNot Nothing Then
                        lblLocales.Text = cboLocales.SelectedItem.Text
                    End If
                    cboLocales.Visible = Not (cboLocales.Items.Count = 1)
                    lblLocales.Visible = (cboLocales.Items.Count = 1)
                End If

                'Bind Property Definition to Data Store
                Properties.LocalResourceFile = Me.LocalResourceFile
                Properties.DataSource = PropertyDefinition
                Properties.DataBind()

                For Each editor As FieldEditorControl In Properties.Fields
                    If editor.DataField = "Required" Then
                        editor.Visible = Not (UsersPortalId = Null.NullInteger)
                    End If
                Next

            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        Protected Sub cboLocales_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cboLocales.SelectedIndexChanged
            BindLanguages()
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' cmdDelete_Click runs when the Delete button is clciked
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	02/22/2006  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub cmdDelete_Click(ByVal sender As Object, ByVal e As System.EventArgs)
            Try
                If PropertyDefinitionID <> Null.NullInteger Then
                    'Declare Definition and "retrieve" it from the Property Editor
                    Dim propertyDefinition As ProfilePropertyDefinition
                    propertyDefinition = DirectCast(Properties.DataSource, ProfilePropertyDefinition)

                    'Delete the Property Definition
                    ProfileController.DeletePropertyDefinition(propertyDefinition)
                End If

                'Redirect to Definitions page
                Response.Redirect(NavigateURL(TabId, "ManageProfile", "mid=" & ModuleId), True)
            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        Protected Sub cmdSaveKeys_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdSaveKeys.Click
            Dim portalResources As New XmlDocument
            Dim defaultResources As New XmlDocument
            Dim node, parent As XmlNode
            Dim filename As String

            Try
                defaultResources.Load(GetResourceFile("", Localization.SystemLocale))
                If Me.IsHostMenu Then
                    filename = GetResourceFile("Host", cboLocales.SelectedValue)
                Else
                    filename = GetResourceFile("Portal", cboLocales.SelectedValue)
                End If
                If File.Exists(filename) Then
                    portalResources.Load(filename)
                Else
                    portalResources.Load(GetResourceFile("", Localization.SystemLocale))
                End If

                UpdateResourceFileNode(portalResources, "ProfileProperties_" + PropertyDefinition.PropertyName + ".Text", txtPropertyName.Text)
                UpdateResourceFileNode(portalResources, "ProfileProperties_" + PropertyDefinition.PropertyName + ".Help", txtPropertyHelp.Text)
                UpdateResourceFileNode(portalResources, "ProfileProperties_" + PropertyDefinition.PropertyName + ".Required", txtPropertyRequired.Text)
                UpdateResourceFileNode(portalResources, "ProfileProperties_" + PropertyDefinition.PropertyName + ".Validation", txtPropertyValidation.Text)
                UpdateResourceFileNode(portalResources, "ProfileProperties_" + PropertyDefinition.PropertyCategory + ".Header", txtCategoryName.Text)

                ' remove unmodified keys
                For Each node In portalResources.SelectNodes("//root/data")
                    Dim defaultNode As XmlNode = defaultResources.SelectSingleNode("//root/data[@name='" + node.Attributes("name").Value + "']")
                    If Not defaultNode Is Nothing AndAlso defaultNode.InnerXml = node.InnerXml Then
                        parent = node.ParentNode
                        parent.RemoveChild(node)
                    End If
                Next
                ' remove duplicate keys
                For Each node In portalResources.SelectNodes("//root/data")
                    If portalResources.SelectNodes("//root/data[@name='" + node.Attributes("name").Value + "']").Count > 1 Then
                        parent = node.ParentNode
                        parent.RemoveChild(node)
                    End If
                Next

                If portalResources.SelectNodes("//root/data").Count > 0 Then
                    ' there's something to save
                    portalResources.Save(filename)
                Else
                    ' nothing to be saved, if file exists delete
                    If File.Exists(filename) Then
                        File.Delete(filename)
                    End If
                End If

            Catch exc As Exception    'Module failed to load
                UI.Skins.Skin.AddModuleMessage(Me, Localization.GetString("Save.ErrorMessage", Me.LocalResourceFile), UI.Skins.Controls.ModuleMessage.ModuleMessageType.YellowWarning)
            End Try
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Wizard_ActiveStepChanged runs when the Wizard page has been changed
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	01/30/2007	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Sub Wizard_ActiveStepChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles Wizard.ActiveStepChanged

            Select Case Wizard.ActiveStepIndex
                Case 1    'Lists
                    If Not IsList Then
                        Wizard.ActiveStepIndex = 2
                    Else
                        BindList()
                    End If
                Case 2
                    BindLanguages()
                    Wizard.DisplayCancelButton = False
            End Select
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Wizard_CancelButtonClick runs when the Cancel Button on the Wizard is clicked.
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	01/30/2007	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Sub Wizard_CancelButtonClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles Wizard.CancelButtonClick
            Try
                'Redirect to Definitions page
                Response.Redirect(NavigateURL(TabId, "ManageProfile", "mid=" & ModuleId), True)
            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Wizard_FinishButtonClick runs when the Finish Button on the Wizard is clicked.
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	01/30/2007	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Sub Wizard_FinishButtonClick(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.WizardNavigationEventArgs) Handles Wizard.FinishButtonClick
            Try
                'Redirect to Definitions page
                Response.Redirect(NavigateURL(TabId, "ManageProfile", "mid=" & ModuleId), True)
            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Wizard_NextButtonClickruns when the next Button is clicked.  It provides
        '''	a mechanism for cancelling the page change if certain conditions aren't met.
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	01/30/2007	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Sub Wizard_NextButtonClick(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.WizardNavigationEventArgs) Handles Wizard.NextButtonClick

            Select Case e.CurrentStepIndex
                Case 0    'Property Details
                    Try
                        'Check if Property Editor has been updated by user
                        If Properties.IsDirty And Properties.IsValid Then
                            'Declare Definition and "retrieve" it from the Property Editor
                            Dim propertyDefinition As ProfilePropertyDefinition
                            propertyDefinition = DirectCast(Properties.DataSource, ProfilePropertyDefinition)

                            If UsersPortalId = Null.NullInteger Then
                                propertyDefinition.Required = False
                            End If

                            If ValidateProperty(propertyDefinition) Then
                                If PropertyDefinitionID = Null.NullInteger Then
                                    'Add the Property Definition
                                    PropertyDefinitionID = ProfileController.AddPropertyDefinition(propertyDefinition)

                                    If PropertyDefinitionID < Null.NullInteger Then
                                        UI.Skins.Skin.AddModuleMessage(Me, Localization.GetString("DuplicateName", Me.LocalResourceFile), Skins.Controls.ModuleMessage.ModuleMessageType.RedError)
                                        e.Cancel = True
                                    End If
                                Else
                                    'Update the Property Definition
                                    ProfileController.UpdatePropertyDefinition(propertyDefinition)
                                End If
                            Else
                                UI.Skins.Skin.AddModuleMessage(Me, Localization.GetString(_Message, Me.LocalResourceFile), Skins.Controls.ModuleMessage.ModuleMessageType.RedError)
                                e.Cancel = True
                            End If
                        End If

                    Catch exc As Exception    'Module failed to load
                        ProcessModuleLoadException(Me, exc)
                    End Try
            End Select
        End Sub

#End Region

    End Class

End Namespace
