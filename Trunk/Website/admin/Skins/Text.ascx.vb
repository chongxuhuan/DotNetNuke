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
Imports DotNetNuke.UI.Skins
Imports DotNetNuke.Services.Localization
Imports DotNetNuke.Services.Tokens

Namespace DotNetNuke.UI.Skins.Controls

    ''' -----------------------------------------------------------------------------
    ''' <summary></summary>
    ''' <remarks></remarks>
    ''' <history>
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Partial Class Text

        Inherits UI.Skins.SkinObjectBase

        ' private members
        Private _text As String
        Private _cssClass As String
        Private _resourceKey As String
        Private _replaceTokens As Boolean = False

#Region "Public Members"

        Public Property Text() As String
            Get
                Return _text
            End Get
            Set(ByVal Value As String)
                _text = Value
            End Set
        End Property

        Public Property CssClass() As String
            Get
                Return _cssClass
            End Get
            Set(ByVal Value As String)
                _cssClass = Value
            End Set
        End Property

        Public Property ResourceKey() As String
            Get
                Return _resourceKey
            End Get
            Set(ByVal Value As String)
                _resourceKey = Value
            End Set
        End Property

        Public Property ReplaceTokens() As Boolean
            Get
                Return _replaceTokens
            End Get
            Set(ByVal Value As Boolean)
                _replaceTokens = Value
            End Set
        End Property

#End Region

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            ' public attributes
            Dim strText As String = Text

            ' load resources
            If ResourceKey <> "" Then
                ' localization
                Dim strFile As String = Path.GetFileName(Server.MapPath(PortalSettings.ActiveTab.SkinSrc))
                strFile = PortalSettings.ActiveTab.SkinPath + Localization.LocalResourceDirectory + "/" + strFile
                Dim strLocalization As String = Services.Localization.Localization.GetString(ResourceKey, strFile)
                If strLocalization <> "" Then
                    strText = strLocalization
                End If
            End If

            'If no value is found then use the value set the the Text attribute
            If String.IsNullOrEmpty(strText) Then strText = Text

            ' token replace
            If ReplaceTokens Then
                Dim tr As New TokenReplace()
                tr.AccessingUser = PortalSettings.UserInfo
                strText = tr.ReplaceEnvironmentTokens(strText)
            End If

            lblText.Text = strText

            If CssClass <> "" Then
                lblText.CssClass = CssClass
            End If

        End Sub

    End Class

End Namespace
