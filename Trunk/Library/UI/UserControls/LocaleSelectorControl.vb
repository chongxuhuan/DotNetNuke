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
Imports DotNetNuke.Services.Localization


Namespace DotNetNuke.UI.UserControls

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' LocaleSelectorControl is a user control that provides all the server code to manage
    ''' localisation selection
    ''' </summary>
    ''' <history>
    ''' 	[cathal]	30/1/2010	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public MustInherit Class LocaleSelectorControl
        Inherits Framework.UserControlBase


#Region "Controls"

        Protected WithEvents rbViewType As System.Web.UI.WebControls.RadioButtonList
        Protected WithEvents ddlPortalDefaultLanguage As System.Web.UI.WebControls.dropdownlist
        Protected WithEvents litStatus As System.Web.UI.WebControls.Literal


#End Region

#Region "Private Members"

        Private MyFileName As String = "LocaleSelectorControl.ascx"
        Private _ViewType As String = ""

        Private ReadOnly Property DisplayType() As CultureDropDownTypes
            Get
                Select Case ViewType
                    Case "NATIVE"
                        Return CultureDropDownTypes.NativeName
                    Case "ENGLISH"
                        Return CultureDropDownTypes.EnglishName
                End Select
            End Get
        End Property

        Private ReadOnly Property ViewType() As String
            Get
                If _ViewType = "" Then
                    _ViewType = Convert.ToString(Personalization.Personalization.GetProfile("LanguageEnabler", String.Format("ViewType{0}", PortalSettings.PortalId)))
                End If
                If _ViewType = "" Then _ViewType = "NATIVE"
                Return _ViewType
            End Get
        End Property

#End Region

#Region "Public Methods"

        Public Sub BindDefaultLanguageSelector()
            Localization.LoadCultureDropDownList(ddlPortalDefaultLanguage, DisplayType, PortalSettings.DefaultLanguage, True)
        End Sub

#End Region

#Region "Event Handlers"

        Private Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            If Not Page.IsPostBack Then
                Dim item As ListItem

                item = New ListItem(Localization.GetString("NativeName.Text", Services.Localization.Localization.GetResourceFile(Me, MyFileName)), "NATIVE")
                rbViewType.Items.Add(item)
                If ViewType = "NATIVE" Then
                    item.Selected = True
                End If
                item = New ListItem(Localization.GetString("EnglishName.Text", Services.Localization.Localization.GetResourceFile(Me, MyFileName)), "ENGLISH")
                rbViewType.Items.Add(item)
                If ViewType = "ENGLISH" Then
                    item.Selected = True
                End If
            End If

        End Sub

        Private Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
            If Me.Visible Then
                BindDefaultLanguageSelector()
            End If
        End Sub

        Private Sub rbViewType_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles rbViewType.SelectedIndexChanged
            _ViewType = rbViewType.SelectedValue
        End Sub

#End Region

    End Class

End Namespace
