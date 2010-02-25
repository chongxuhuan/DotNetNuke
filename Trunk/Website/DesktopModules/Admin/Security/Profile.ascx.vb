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


Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Entities.Profile
Imports DotNetNuke.Services.Localization
Imports DotNetNuke.UI.WebControls


Namespace DotNetNuke.Modules.Admin.Users

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The Profile UserModuleBase is used to register Users
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[cnurse]	03/02/2006
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Partial Class Profile
        Inherits ProfileUserControlBase

#Region "Protected Properties"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets whether to display the Visibility controls
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	08/11/2006  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected ReadOnly Property ShowVisibility() As Boolean
            Get
                Dim setting As Object = UserModuleBase.GetSetting(PortalId, "Profile_DisplayVisibility")
                Return CType(setting, Boolean) And IsUser
            End Get
        End Property

#End Region

#Region "Public Properties"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the EditorMode
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	05/02/2006  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property EditorMode() As PropertyEditorMode
            Get
                Return ProfileProperties.EditMode()
            End Get
            Set(ByVal Value As PropertyEditorMode)
                ProfileProperties.EditMode = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets whether the User is valid
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	05/18/2006  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public ReadOnly Property IsValid() As Boolean
            Get
                Dim _IsValid As Boolean = False

                If ProfileProperties.IsValid Or IsAdmin Then
                    _IsValid = True
                End If

                Return _IsValid
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets whether the Update button
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	05/18/2006  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property ShowUpdate() As Boolean
            Get
                Return cmdUpdate.Visible
            End Get
            Set(ByVal Value As Boolean)
                cmdUpdate.Visible = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the UserProfile associated with this control
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	03/02/2006  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public ReadOnly Property UserProfile() As UserProfile
            Get
                Dim _Profile As UserProfile = Nothing
                If Not User Is Nothing Then
                    _Profile = User.Profile
                End If
                Return _Profile
            End Get
        End Property

#End Region

#Region "Public Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' DataBind binds the data to the controls
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	03/01/2006  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overrides Sub DataBind()

            If IsAdmin Then
                lblTitle.Text = String.Format(Localization.GetString("ProfileTitle.Text", LocalResourceFile), User.Username, User.UserID.ToString)
            Else
                trTitle.Visible = False
            End If

            'Before we bind the Profile to the editor we need to "update" the visible data
            Dim properties As ProfilePropertyDefinitionCollection = UserProfile.ProfileProperties

            For Each profProperty As ProfilePropertyDefinition In properties
                If IsAdmin AndAlso Not IsProfile() Then
                    profProperty.Visible = True
                End If
            Next

            ProfileProperties.ShowVisibility = ShowVisibility
            ProfileProperties.DataSource = UserProfile.ProfileProperties
            ProfileProperties.DataBind()

        End Sub

#End Region

#Region "Event Handlers"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Page_Init runs when the control is initialised
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	03/01/2006  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
            'Get the base Page
            Dim basePage As PageBase = TryCast(Me.Page, PageBase)
            If basePage IsNot Nothing Then
                'Check if culture is RTL
                If basePage.PageCulture.TextInfo.IsRightToLeft Then
                    ProfileProperties.LabelMode = LabelMode.Right
                Else
                    ProfileProperties.LabelMode = LabelMode.Left
                End If
            End If
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Page_Load runs when the control is loaded
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	03/01/2006  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            ProfileProperties.LocalResourceFile = Me.LocalResourceFile

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' cmdUpdate_Click runs when the Update Button is clicked
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	03/01/2006  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub cmdUpdate_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdUpdate.Click

            If IsValid Then
                Dim properties As ProfilePropertyDefinitionCollection = CType(ProfileProperties.DataSource, ProfilePropertyDefinitionCollection)

                'Update User's profile
                User = ProfileController.UpdateUserProfile(User, properties)

                OnProfileUpdated(EventArgs.Empty)
                OnProfileUpdateCompleted(EventArgs.Empty)
            End If

        End Sub

#End Region

    End Class

End Namespace
