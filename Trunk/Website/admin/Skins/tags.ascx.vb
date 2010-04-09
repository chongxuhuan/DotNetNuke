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

Imports DotNetNuke.Entities.Content
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Entities.Host

Namespace DotNetNuke.UI.Skins.Controls

    Partial Class Tags
        Inherits UI.Skins.SkinObjectBase

        Private _AddImageUrl As String = "~/images/add.gif"
        Private _AllowTagging As Boolean = True
        Private _CancelImageUrl As String = "~/images/lt.gif"
        Private _CssClass As String = "SkinObject"
        Private _ObjectType As String = "Page"
        Private _RepeatDirection As String = "Horizontal"
        Private _SaveImageUrl As String = "~/images/save.gif"
        Private _Separator As String = ",&nbsp;"
        Private _ShowCategories As Boolean = True
        Private _ShowTags As Boolean = True

        Const MyFileName As String = "Tags.ascx"

#Region "Public Members"

        Public Property AddImageUrl() As String
            Get
                Return _AddImageUrl
            End Get
            Set(ByVal value As String)
                _AddImageUrl = value
            End Set
        End Property

        Public Property AllowTagging() As Boolean
            Get
                Return _AllowTagging
            End Get
            Set(ByVal value As Boolean)
                _AllowTagging = value
            End Set
        End Property

        Public Property CancelImageUrl() As String
            Get
                Return _CancelImageUrl
            End Get
            Set(ByVal value As String)
                _CancelImageUrl = value
            End Set
        End Property

        Public Property CssClass() As String
            Get
                Return _CssClass
            End Get
            Set(ByVal Value As String)
                _CssClass = Value
            End Set
        End Property

        Public Property ObjectType() As String
            Get
                Return _ObjectType
            End Get
            Set(ByVal Value As String)
                _ObjectType = Value
            End Set
        End Property

        Public Property RepeatDirection() As String
            Get
                Return _RepeatDirection
            End Get
            Set(ByVal value As String)
                _RepeatDirection = value
            End Set
        End Property

        Public Property SaveImageUrl() As String
            Get
                Return _SaveImageUrl
            End Get
            Set(ByVal value As String)
                _SaveImageUrl = value
            End Set
        End Property

        Public Property Separator() As String
            Get
                Return _Separator
            End Get
            Set(ByVal Value As String)
                _Separator = Value
            End Set
        End Property

        Public Property ShowCategories() As Boolean
            Get
                Return _ShowCategories
            End Get
            Set(ByVal value As Boolean)
                _ShowCategories = value
            End Set
        End Property

        Public Property ShowTags() As Boolean
            Get
                Return _ShowTags
            End Get
            Set(ByVal value As Boolean)
                _ShowTags = value
            End Set
        End Property

#End Region

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            If ObjectType = "Page" Then
                tagsControl.ContentItem = PortalSettings.ActiveTab
            Else
                tagsControl.ContentItem = Me.ModuleControl.ModuleContext.Configuration
            End If

            Dim resultsUrl As String = Null.NullString
            Dim objModules As New ModuleController
            Dim searchTabId As Integer
            Dim SearchModule As ModuleInfo = objModules.GetModuleByDefinition(PortalSettings.PortalId, "Search Results")
            If SearchModule Is Nothing Then
                Exit Sub
            Else
                searchTabId = SearchModule.TabID
            End If

            tagsControl.AddImageUrl = AddImageUrl
            tagsControl.CancelImageUrl = CancelImageUrl
            tagsControl.SaveImageUrl = SaveImageUrl

            tagsControl.CssClass = CssClass

            tagsControl.AllowTagging = AllowTagging AndAlso Request.IsAuthenticated
            tagsControl.NavigateUrlFormatString = NavigateURL(searchTabId, "", "Tag={0}")
            tagsControl.RepeatDirection = RepeatDirection
            tagsControl.Separator = Separator
            tagsControl.ShowCategories = ShowCategories
            tagsControl.ShowTags = ShowTags

        End Sub
    End Class

End Namespace
