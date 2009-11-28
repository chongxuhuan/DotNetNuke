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

Namespace DotNetNuke.UI.UserControls

    Public MustInherit Class ModuleAuditControl
        Inherits System.Web.UI.UserControl

        Protected lblCreatedBy As System.Web.UI.WebControls.Label
        Protected lblUpdatedBy As System.Web.UI.WebControls.Label


        Private _CreatedDate As String = ""
        Private _CreatedByUser As String = ""
        Private _LastModifiedByUser As String = String.Empty
        Private _LastModifiedDate As String = String.Empty
        'allow classes that support BaseEntityInfo to pass audit values
        Private _Entity As DotNetNuke.Entities.BaseEntityInfo
        Private _SystemUser As String = String.Empty


        Private MyFileName As String = "ModuleAuditControl.ascx"


        ' public properties
        Public WriteOnly Property CreatedDate() As String
            Set(ByVal Value As String)
                _CreatedDate = Value
            End Set
        End Property

        Public WriteOnly Property CreatedByUser() As String
            Set(ByVal Value As String)
                _CreatedByUser = Value
            End Set
        End Property


        Public WriteOnly Property LastModifiedByUser() As String
            Set(ByVal Value As String)
                _LastModifiedByUser = Value
            End Set
        End Property

        Public WriteOnly Property LastModifiedDate() As String
            Set(ByVal Value As String)
                _LastModifiedDate = Value
            End Set
        End Property

        Public WriteOnly Property Entity() As DotNetNuke.Entities.BaseEntityInfo

            Set(ByVal value As DotNetNuke.Entities.BaseEntityInfo)
                _Entity = value
            End Set
        End Property



        '*******************************************************
        '
        ' The Page_Load server event handler on this page is used
        ' to populate the role information for the page
        '
        '*******************************************************

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Try

                If Not IsNothing(_Entity) Then
                    _CreatedByUser = _Entity.CreatedByUserID.ToString
                    _CreatedDate = _Entity.CreatedOnDate.ToString
                    _LastModifiedByUser = _Entity.LastModifiedByUserID.ToString
                    _LastModifiedDate = _Entity.LastModifiedOnDate.ToString
                End If

                'check to see if updated check is redundant
                Dim isCreatorAndUpdater As Boolean = False
                If IsNumeric(_CreatedByUser) AndAlso IsNumeric(_LastModifiedByUser) AndAlso _CreatedByUser = _LastModifiedByUser Then isCreatorAndUpdater = True

                _SystemUser = Services.Localization.Localization.GetString("SystemUser", Services.Localization.Localization.GetResourceFile(Me, MyFileName))

                ShowCreatedString()
                ShowUpdatedString(isCreatorAndUpdater)

            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        Private Function ShowCreatedString() As String
            If IsNumeric(_CreatedByUser) Then
                If Integer.Parse(_CreatedByUser) = Null.NullInteger Then
                    _CreatedByUser = _SystemUser
                Else
                    ' contains a UserID
                    Dim objUsers As New UserController
                    Dim objUser As UserInfo = UserController.GetUserById(PortalController.GetCurrentPortalSettings.PortalId, Integer.Parse(_CreatedByUser))
                    If Not objUser Is Nothing Then
                        _CreatedByUser = objUser.DisplayName
                    End If
                End If
            End If

            
            Dim str As String = Services.Localization.Localization.GetString("CreatedBy", Services.Localization.Localization.GetResourceFile(Me, MyFileName))
            lblCreatedBy.Text = String.Format(str, _CreatedByUser, _CreatedDate)
            Return str
        End Function

        Private Sub ShowUpdatedString(ByVal isCreatorAndUpdater As Boolean)
            'check to see if audit contains update information
            If String.IsNullOrEmpty(_LastModifiedDate) Then Exit Sub
            Dim str As String = String.Empty

            If IsNumeric(_LastModifiedByUser) Then

                If isCreatorAndUpdater = True Then
                    _LastModifiedByUser = _CreatedByUser
                ElseIf Integer.Parse(_LastModifiedByUser) = Null.NullInteger Then
                    _LastModifiedByUser = _SystemUser
                Else
                    ' contains a UserID
                    Dim objUsers As New UserController
                    Dim objUser As UserInfo = UserController.GetUserById(PortalController.GetCurrentPortalSettings.PortalId, Integer.Parse(_LastModifiedByUser))
                    If Not objUser Is Nothing Then
                        _LastModifiedByUser = objUser.DisplayName
                    End If
                End If
            End If
            
            str = Services.Localization.Localization.GetString("UpdatedBy", Services.Localization.Localization.GetResourceFile(Me, MyFileName))
            lblUpdatedBy.Text = String.Format(str, _LastModifiedByUser, _LastModifiedDate)

        End Sub
    End Class

End Namespace
