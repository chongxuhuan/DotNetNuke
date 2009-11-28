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

Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Entities.Profile

Namespace DotNetNuke.Modules.Admin.Users

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The ViewProfile UserModuleBase is used to view a Users Profile
    ''' </summary>
    ''' <history>
    ''' 	[cnurse]	05/02/2006   Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Partial Class ViewProfile
        Inherits UserModuleBase

#Region "Event Handlers"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Page_Init runs when the control is initialised
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	03/01/2006
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
            ' get userid from encrypted ticket
            UserId = Null.NullInteger
            If Not Context.Request.QueryString("userticket") Is Nothing Then
                UserId = Int32.Parse(UrlUtils.DecryptParameter(Context.Request.QueryString("userticket")))
            End If

            ctlProfile.ID = "Profile"
            ctlProfile.UserId = UserId

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Page_Load runs when the control is loaded
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	03/01/2006
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Try
                'check if there is a user profile, if not give default message to stop userid identification
                If ctlProfile.UserProfile Is Nothing Then
                    lblNoProperties.Visible = True
                    Exit Try
                End If

                'Before we bind the Profile to the editor we need to "update" the visibility data
                Dim properties As ProfilePropertyDefinitionCollection = ctlProfile.UserProfile.ProfileProperties
                Dim visibleCount As Integer = 0

                For Each profProperty As ProfilePropertyDefinition In properties
                    If profProperty.Visible Then
                        'Check Visibility
                        If profProperty.Visibility = UserVisibilityMode.AdminOnly Then
                            'Only Visible if Admin (or self)
                            profProperty.Visible = (IsAdmin Or IsUser)
                        ElseIf profProperty.Visibility = UserVisibilityMode.MembersOnly Then
                            'Only Visible if Is a Member (ie Authenticated)
                            profProperty.Visible = Request.IsAuthenticated
                        End If
                    End If
                    If profProperty.Visible Then visibleCount += 1
                Next

                'Bind the profile information to the control
                ctlProfile.DataBind()

                If visibleCount = 0 Then
                    lblNoProperties.Visible = True
                End If

            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

#End Region

    End Class

End Namespace