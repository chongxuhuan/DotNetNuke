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
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Entities.Modules.Definitions
Imports DotNetNuke.UI.Utilities

Namespace DotNetNuke.Modules.Admin.ModuleDefinitions

	''' -----------------------------------------------------------------------------
	''' <summary>
	''' The EditModuleControl PortalModuleBase is used to edit a Module Control
	''' </summary>
    ''' <remarks>
	''' </remarks>
	''' <history>
	''' 	[cnurse]	9/28/2004	Updated to reflect design changes for Help, 508 support
	'''                       and localisation
	''' </history>
	''' -----------------------------------------------------------------------------
	Partial  Class EditModuleControl
        Inherits DotNetNuke.Entities.Modules.PortalModuleBase

#Region "Private Members"

        Private _DesktopModule As DesktopModuleInfo = Nothing
        Private ModuleDefId As Integer
        Private ModuleControlId As Integer

#End Region

#Region "Protected Properties"

        Protected ReadOnly Property DesktopModule() As DesktopModuleInfo
            Get
                If _DesktopModule Is Nothing Then
                    _DesktopModule = DesktopModuleController.GetDesktopModuleByPackageID(PackageID)
                End If
                Return _DesktopModule
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

        Protected ReadOnly Property ReturnURL() As String
            Get
                Return EditUrl("PackageID", PackageID.ToString, "Edit", "ModuleDefinitionID=" + ModuleDefId.ToString())
            End Get
        End Property

#End Region

#Region "Private Methods"

		''' -----------------------------------------------------------------------------
		''' <summary>
		''' Removed from Page_Load to allow for Skin Objects to be populated without duplicating code
		''' </summary>
		''' <param name="strRoot">The Root folder to parse from</param>
		''' <param name="blnRecurse">True to iterate sub-folders</param>
		''' <remarks>
		''' Loads the cboSource control list with locations of controls.
		''' </remarks>
		''' <history>
		''' 	[pgaryga]	18/08/2004	Created
		''' </history>
		''' -----------------------------------------------------------------------------
		Private Sub BindControlList(ByVal strRoot As String, Optional ByVal blnRecurse As Boolean = True)
			Dim strFolder As String
			Dim arrFolders As String()
			Dim strFile As String
			Dim arrFiles As String()

			If Directory.Exists(Request.MapPath(Common.Globals.ApplicationPath & "/" & strRoot)) Then
				arrFolders = Directory.GetDirectories(Request.MapPath(Common.Globals.ApplicationPath & "/" & strRoot))
				If blnRecurse Then
					For Each strFolder In arrFolders
						BindControlList(strFolder.Substring(Request.MapPath(Common.Globals.ApplicationPath).Length + 1).Replace("\"c, "/"c), blnRecurse)
					Next
				End If
				arrFiles = Directory.GetFiles(Request.MapPath(Common.Globals.ApplicationPath & "/" & strRoot), "*.ascx")
				For Each strFile In arrFiles
					strFile = strRoot.Replace("\"c, "/"c) & "/" & Path.GetFileName(strFile)
					cboSource.Items.Add(New ListItem(strFile, strFile.ToLower))
				Next
			End If
		End Sub

		''' -----------------------------------------------------------------------------
		''' <summary>
		''' LoadIcons load the Icons Combo
		''' </summary>
		''' <remarks>
		''' </remarks>
		''' <history>
		''' 	[cnurse]	9/28/2004	Updated to reflect design changes for Help, 508 support
		'''                       and localisation
		''' </history>
		''' -----------------------------------------------------------------------------
		Private Sub LoadIcons()
			Dim strRoot As String
			Dim strFile As String
			Dim arrFiles As String()
			Dim strExtension As String

			cboIcon.Items.Clear()
			cboIcon.Items.Add("<" & Services.Localization.Localization.GetString("Not_Specified") & ">")

            If cboSource.SelectedItem.Value <> "" Then
                strRoot = cboSource.SelectedItem.Value
                strRoot = Request.MapPath(Common.Globals.ApplicationPath & "/" & strRoot.Substring(0, strRoot.LastIndexOf("/")))

                If Directory.Exists(strRoot) Then
                    arrFiles = Directory.GetFiles(strRoot)
                    For Each strFile In arrFiles
                        strExtension = Path.GetExtension(strFile).Replace(".", "")
                        If InStr(1, glbImageFileTypes & ",", strExtension & ",") <> 0 Then
                            cboIcon.Items.Add(New ListItem(Path.GetFileName(strFile), Path.GetFileName(strFile).ToLower))
                        End If
                    Next
                End If
            End If

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
		''' 	[cnurse]	9/28/2004	Updated to reflect design changes for Help, 508 support
		'''                       and localisation
		''' </history>
		''' -----------------------------------------------------------------------------
		Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
			Try
                If Not (Request.QueryString("moduledefid") Is Nothing) Then
                    ModuleDefId = Int32.Parse(Request.QueryString("moduledefid"))
                Else
                    ModuleDefId = Null.NullInteger
                End If

                If Not (Request.QueryString("modulecontrolid") Is Nothing) Then
                    ModuleControlId = Int32.Parse(Request.QueryString("modulecontrolid"))
                Else
                    ModuleControlId = Null.NullInteger
                End If

                If Page.IsPostBack = False Then
                    lblModule.Text = DesktopModule.FriendlyName

                    Dim objModuleDefinition As ModuleDefinitionInfo = ModuleDefinitionController.GetModuleDefinitionByID(ModuleDefId)
                    If Not objModuleDefinition Is Nothing Then
                        lblDefinition.Text = objModuleDefinition.FriendlyName
                    End If

					ClientAPI.AddButtonConfirm(cmdDelete, Services.Localization.Localization.GetString("DeleteItem"))

                    Dim objModuleControl As ModuleControlInfo

                    objModuleControl = ModuleControlController.GetModuleControl(ModuleControlId)

                    BindControlList("DesktopModules")
                    BindControlList("Admin/Skins", False)
                    cboSource.Items.Insert(0, New ListItem("<" + Services.Localization.Localization.GetString("None_Specified") + ">", ""))

                    If Not Null.IsNull(ModuleControlId) Then

                        If Not objModuleControl Is Nothing Then
                            txtKey.Text = objModuleControl.ControlKey
                            txtTitle.Text = objModuleControl.ControlTitle
                            If Not cboSource.Items.FindByValue(objModuleControl.ControlSrc.ToString.ToLower) Is Nothing Then
                                cboSource.Items.FindByValue(objModuleControl.ControlSrc.ToString.ToLower).Selected = True
                                LoadIcons()
                            Else
                                txtSource.Text = objModuleControl.ControlSrc
                            End If
                            If Not cboType.Items.FindByValue(CType(objModuleControl.ControlType, Integer).ToString) Is Nothing Then
                                cboType.Items.FindByValue(CType(objModuleControl.ControlType, Integer).ToString).Selected = True
                            End If
                            If Not Null.IsNull(objModuleControl.ViewOrder) Then
                                txtViewOrder.Text = objModuleControl.ViewOrder.ToString
                            End If
                            If Not cboIcon.Items.FindByValue(objModuleControl.IconFile.ToLower) Is Nothing Then
                                cboIcon.Items.FindByValue(objModuleControl.IconFile.ToLower).Selected = True
                            End If
                            If Not Null.IsNull(objModuleControl.HelpURL) Then
                                txtHelpURL.Text = objModuleControl.HelpURL
                            End If
                            If objModuleControl.SupportsPartialRendering Then
                                chkSupportsPartialRendering.Checked = True
                            End If
                        End If
                    Else
                        If cboType.Enabled Then
                            cboType.Items.FindByValue("0").Selected = True ' default to "View"
                        Else
                            cboType.Items.FindByValue("-2").Selected = True ' this is a skinobject
                        End If
                    End If

                End If


            Catch exc As Exception    'Module failed to load
				ProcessModuleLoadException(Me, exc)
			End Try
		End Sub

		''' -----------------------------------------------------------------------------
		''' <summary>
		''' cboSource_SelectedIndexChanged runs when the Selected Soure is changed
		''' </summary>
        ''' <remarks>
		''' </remarks>
		''' <history>
		''' 	[cnurse]	9/28/2004	Updated to reflect design changes for Help, 508 support
		'''                       and localisation
		''' </history>
		''' -----------------------------------------------------------------------------
		Private Sub cboSource_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cboSource.SelectedIndexChanged
			LoadIcons()
		End Sub

		''' -----------------------------------------------------------------------------
		''' <summary>
		''' cmdCancel_Click runs when the Cancel Button is clicked
		''' </summary>
        ''' <remarks>
		''' </remarks>
		''' <history>
		''' 	[cnurse]	9/28/2004	Updated to reflect design changes for Help, 508 support
		'''                       and localisation
		''' </history>
		''' -----------------------------------------------------------------------------
		Private Sub cmdCancel_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdCancel.Click
			Try
                Response.Redirect(ReturnURL, True)
            Catch exc As Exception           'Module failed to load
                ProcessModuleLoadException(Me, exc)
			End Try
		End Sub

		''' -----------------------------------------------------------------------------
		''' <summary>
		''' cmdDelete_Click runs when the Delete Button is clicked
		''' </summary>
        ''' <remarks>
		''' </remarks>
		''' <history>
		''' 	[cnurse]	9/28/2004	Updated to reflect design changes for Help, 508 support
		'''                       and localisation
		''' </history>
		''' -----------------------------------------------------------------------------
		Private Sub cmdDelete_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdDelete.Click
			Try
                If Not Null.IsNull(ModuleControlId) Then
                    ModuleControlController.DeleteModuleControl(ModuleControlId)
                End If

                Response.Redirect(ReturnURL, True)
            Catch exc As Exception           'Module failed to load
                ProcessModuleLoadException(Me, exc)
			End Try
		End Sub

		''' -----------------------------------------------------------------------------
		''' <summary>
		''' cmdUpdate_Click runs when the Update Button is clicked
		''' </summary>
        ''' <remarks>
		''' </remarks>
		''' <history>
		''' 	[cnurse]	9/28/2004	Updated to reflect design changes for Help, 508 support
		'''                       and localisation
		''' </history>
		''' -----------------------------------------------------------------------------
		Private Sub cmdUpdate_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdUpdate.Click
			Try

                If Page.IsValid = True Then
                    If cboSource.SelectedIndex <> 0 Or txtSource.Text <> "" Then
                        Dim objModuleControl As New ModuleControlInfo

                        objModuleControl.ModuleControlID = ModuleControlId
                        objModuleControl.ModuleDefID = ModuleDefId
                        If txtKey.Text <> "" Then
                            objModuleControl.ControlKey = txtKey.Text
                        Else
                            objModuleControl.ControlKey = Null.NullString
                        End If
                        If txtTitle.Text <> "" Then
                            objModuleControl.ControlTitle = txtTitle.Text
                        Else
                            objModuleControl.ControlTitle = Null.NullString
                        End If
                        If txtSource.Text <> "" Then
                            objModuleControl.ControlSrc = txtSource.Text
                        Else
                            objModuleControl.ControlSrc = cboSource.SelectedItem.Text
                        End If
                        objModuleControl.ControlType = CType(cboType.SelectedItem.Value, SecurityAccessLevel)
                        If txtViewOrder.Text <> "" Then
                            objModuleControl.ViewOrder = Integer.Parse(txtViewOrder.Text)
                        Else
                            objModuleControl.ViewOrder = Null.NullInteger
                        End If
                        If cboIcon.SelectedIndex > 0 Then
                            objModuleControl.IconFile = cboIcon.SelectedItem.Text
                        Else
                            objModuleControl.IconFile = Null.NullString
                        End If

                        If txtHelpURL.Text <> "" Then
                            objModuleControl.HelpURL = txtHelpURL.Text
                        Else
                            objModuleControl.HelpURL = Null.NullString
                        End If

                        objModuleControl.SupportsPartialRendering = chkSupportsPartialRendering.Checked

                        Try
                            ModuleControlController.SaveModuleControl(objModuleControl, True)
                        Catch
                            DotNetNuke.UI.Skins.Skin.AddModuleMessage(Me, Services.Localization.Localization.GetString("AddControl.ErrorMessage", Me.LocalResourceFile), DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.RedError)
                            Exit Sub
                        End Try

                        Response.Redirect(ReturnURL, True)
                    Else
                        DotNetNuke.UI.Skins.Skin.AddModuleMessage(Me, Services.Localization.Localization.GetString("MissingSource.ErrorMessage", Me.LocalResourceFile), DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.RedError)
                    End If

                End If

			Catch exc As Exception			 'Module failed to load
				ProcessModuleLoadException(Me, exc)
			End Try
		End Sub

#End Region

    End Class

End Namespace
