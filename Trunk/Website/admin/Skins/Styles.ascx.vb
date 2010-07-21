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
Imports DotNetNuke.Services.Tokens

Namespace DotNetNuke.UI.Skins.Controls

    Partial Class Styles
        Inherits UI.Skins.SkinObjectBase

#Region "Fields"

        Private _condition As String
        Private _isFirst As Boolean = False
        Private _name As String
        Private _src As String
        Private _useSkinPath As Boolean = True
        Private _media As String
#End Region

#Region "Properties"
        Public Property Condition() As String
            Get
                Return _condition
            End Get
            Set(ByVal Value As String)
                _condition = Value
            End Set
        End Property

        Public Property IsFirst() As Boolean
            Get
                Return _isFirst
            End Get
            Set(ByVal Value As Boolean)
                _isFirst = Value
            End Set
        End Property

        Public Property Name() As String
            Get
                Return _name
            End Get
            Set(ByVal Value As String)
                _name = Value
            End Set
        End Property

        Public Property StyleSheet() As String
            Get
                Return _src
            End Get
            Set(ByVal Value As String)
                _src = Value
            End Set
        End Property

        Public Property UseSkinPath() As Boolean
            Get
                Return _useSkinPath
            End Get
            Set(ByVal Value As Boolean)
                _useSkinPath = Value
            End Set
        End Property

        ' NWS: add support for "media" attribute
        Public Property Media() As String
            Get
                Return _media
            End Get
            Set(ByVal Value As String)
                _media = Value
            End Set
        End Property

#End Region

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            AddStyleSheet()

        End Sub

        Protected Sub AddStyleSheet()

            'Find the placeholder control
            Dim objCSS As Control = Page.FindControl("CSS")

            If Not objCSS Is Nothing Then
                'First see if we have already added the <LINK> control
                Dim objCtrl As Control = Page.Header.FindControl(id)

                If objCtrl Is Nothing Then
                    Dim skinpath As String = String.Empty
                    If UseSkinPath Then
                        skinpath = CType(Me.Parent, Skin).SkinPath
                    End If
                    Dim objLink As New HtmlLink()
                    objLink.ID = CreateValidID(Name)
                    objLink.Attributes("rel") = "stylesheet"
                    objLink.Attributes("type") = "text/css"
                    objLink.Href = skinpath + StyleSheet
                    If Media <> "" Then objLink.Attributes("media") = Media ' NWS: add support for "media" attribute

                    If IsFirst Then
                        'Find the first HtmlLink
                        Dim iLink As Integer
                        For iLink = 0 To objCSS.Controls.Count - 1
                            If TypeOf objCSS.Controls(iLink) Is HtmlLink Then
                                Exit For
                            End If
                        Next
                        AddLink(objCSS, iLink, objLink)
                    Else
                        AddLink(objCSS, -1, objLink)
                    End If
                End If
            End If

        End Sub

        Protected Sub AddLink(ByVal cssRoot As Control, ByVal InsertAt As Integer, ByVal link As HtmlLink)
            If String.IsNullOrEmpty(Condition) Then
                If InsertAt = -1 Then
                    cssRoot.Controls.Add(link)
                Else
                    cssRoot.Controls.AddAt(InsertAt, link)
                End If
            Else
                Dim openif As New System.Web.UI.WebControls.Literal()
                openif.Text = String.Format("<!--[if {0}]>", Condition)
                Dim closeif As New System.Web.UI.WebControls.Literal()
                closeif.Text = "<![endif]-->"
                If InsertAt = -1 Then
                    cssRoot.Controls.Add(openif)
                    cssRoot.Controls.Add(link)
                    cssRoot.Controls.Add(closeif)
                Else
                    'Since we want to add at a specific location, we do this in reverse order
                    'this allows us to use the same insertion point
                    cssRoot.Controls.AddAt(InsertAt, closeif)
                    cssRoot.Controls.AddAt(InsertAt, link)
                    cssRoot.Controls.AddAt(InsertAt, openif)
                End If

            End If
        End Sub
    End Class
End Namespace
