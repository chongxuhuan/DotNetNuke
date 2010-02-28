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
Imports DotNetNuke.Entities.Tabs
Imports DotNetNuke.Entities.Content
Imports DotNetNuke.Entities.Content.Taxonomy
Imports DotNetNuke.Entities.Content.Common

Namespace DotNetNuke.UI.Skins.Controls

    Partial Class Tags
        Inherits UI.Skins.SkinObjectBase

        Private _CssClass As String
        Private _ObjectType As String = "Page"
        Private _Separator As String

        Const MyFileName As String = "Tags.ascx"

#Region "Public Members"

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

        Public Property Separator() As String
            Get
                Return _Separator
            End Get
            Set(ByVal Value As String)
                _Separator = Value
            End Set
        End Property

#End Region

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            If String.IsNullOrEmpty(CssClass) Then
                CssClass = "SkinObject"
            End If

            Dim content As ContentItem
            If ObjectType = "Page" Then
                content = PortalSettings.ActiveTab
            Else
                content = Me.ModuleControl.ModuleContext.Configuration
            End If

            lblTags.CssClass = CssClass
            lblTags.Text = content.Terms.ToDelimittedString(Separator)

        End Sub

    End Class

End Namespace
