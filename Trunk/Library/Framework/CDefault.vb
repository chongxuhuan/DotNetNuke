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
Imports DotNetNuke.Entities.Tabs
Imports DotNetNuke.UI.Skins

Namespace DotNetNuke.Framework
    ''' -----------------------------------------------------------------------------
    ''' Project	 : DotNetNuke
    ''' Class	 : CDefault
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[sun1]	1/19/2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class CDefault
        Inherits DotNetNuke.Framework.PageBase

#Region "Public Properties"

        Public Comment As String = ""
        Public Description As String = ""
        Public KeyWords As String = ""
        Public Copyright As String = ""
        Public Generator As String = ""
        Public Author As String = ""
        Public Shadows Title As String = ""

#End Region

#Region "Protected Methods"

        Protected Overrides Sub Finalize()
            MyBase.Finalize()
        End Sub

#End Region

#Region "Public Methods"

        Public Sub AddStyleSheet(ByVal id As String, ByVal href As String, ByVal isFirst As Boolean)
            'Find the placeholder control
            Dim objCSS As Control = Me.FindControl("CSS")

            If Not objCSS Is Nothing Then
                'First see if we have already added the <LINK> control
                Dim objCtrl As Control = Page.Header.FindControl(id)

                If objCtrl Is Nothing Then
                    Dim objLink As New HtmlLink()
                    objLink.ID = id
                    objLink.Attributes("rel") = "stylesheet"
                    objLink.Attributes("type") = "text/css"
                    objLink.Href = href

                    If isFirst Then
                        'Find the first HtmlLink
                        Dim iLink As Integer
                        For iLink = 0 To objCSS.Controls.Count - 1
                            If TypeOf objCSS.Controls(iLink) Is HtmlLink Then
                                Exit For
                            End If
                        Next
                        objCSS.Controls.AddAt(iLink, objLink)
                    Else
                        objCSS.Controls.Add(objLink)
                    End If
                End If
            End If
        End Sub

        Public Sub AddStyleSheet(ByVal id As String, ByVal href As String)
            AddStyleSheet(id, href, False)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Allows the scroll position on the page to be moved to the top of the passed in control.
        ''' </summary>
        ''' <param name="objControl">Control to scroll to</param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Jon Henning]	3/30/2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub ScrollToControl(ByVal objControl As Control)
            If DotNetNuke.UI.Utilities.ClientAPI.BrowserSupportsFunctionality(DotNetNuke.UI.Utilities.ClientAPI.ClientFunctionality.Positioning) Then
                DotNetNuke.UI.Utilities.ClientAPI.RegisterClientReference(Me, DotNetNuke.UI.Utilities.ClientAPI.ClientNamespaceReferences.dnn_dom_positioning)    'client side needs to calculate the top of a particluar control (elementTop)
                DotNetNuke.UI.Utilities.ClientAPI.RegisterClientVariable(Me, "ScrollToControl", objControl.ClientID, True)
                DotNetNuke.UI.Utilities.DNNClientAPI.AddBodyOnloadEventHandler(Page, "__dnn_setScrollTop();")
            End If

        End Sub

#End Region

    End Class

End Namespace
