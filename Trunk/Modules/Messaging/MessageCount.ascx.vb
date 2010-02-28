'
' DotNetNuke - http://www.dotnetnuke.com
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

Imports DotNetNuke.Services.Messaging
Imports DotNetNuke.Services.Localization
Imports DotNetNuke.Services.Messaging.Providers

Namespace DotNetNuke.Modules.Messaging

    Partial Public Class MessageCount
        Inherits UI.Skins.SkinObjectBase

#Region "Private Members"

        Private _FormatString As String = "[COUNT] [RES:Unread] [RES:Messages]"
        Private _LocalResourceFile As String

#End Region

#Region "Public Constants "

        Public Const MyFileName As String = "MessageCount.ascx"

#End Region

        Protected ReadOnly Property LocalResourceFile() As String
            Get
                If String.IsNullOrEmpty(_LocalResourceFile) Then
                    _LocalResourceFile = Localization.GetResourceFile(Me, MyFileName)
                End If
                Return _LocalResourceFile
            End Get
        End Property

        Public Property FormatString() As String
            Get
                Return _FormatString
            End Get
            Set(ByVal value As String)
                _FormatString = value
            End Set
        End Property

        Public Function FormatFormatString(ByVal Count As Integer) As String
            Dim result As String = FormatString
            result = result.Replace("[COUNT]", Count)

            Dim pattern As String = "\[RES:[a-zA-Z]*\]"

            For Each m As Match In System.Text.RegularExpressions.Regex.Matches(FormatString, pattern)
                Dim key As String = m.Value.Replace("[RES:", "").Replace("]", "")
                Dim val As String = Localization.GetString(key, Me.LocalResourceFile)
                If String.IsNullOrEmpty(val) Then val = key
                result = result.Replace(m.Value, val)
            Next

            Return result.Trim()
        End Function

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            Dim pmc As New MessagingController

            Dim count As Integer = (From m As Message _
                                        In pmc.GetMessagesForUser(PortalSettings.PortalId, PortalSettings.UserId) _
                                    Where m.ToUserID = PortalSettings.UserId _
                                        AndAlso m.Status = "Unread" _
                                    Select m).Count

            Dim name As String = "Messaging"
            Dim nav As String = MessagingController.DefaultMessagingURL(name)
            messageCountHyperLink.NavigateUrl = nav
            messageCountHyperLink.Text = FormatFormatString(count)

            messageCountHyperLink.Visible = (Not (Me.PortalSettings.UserInfo Is Nothing) _
                                                    AndAlso Me.PortalSettings.UserId >= 0)
            If (String.IsNullOrEmpty(nav)) Then messageCountHyperLink.Visible = False
        End Sub

    End Class

End Namespace
