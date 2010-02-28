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

Imports System
Imports System.Configuration
Imports System.Data

Namespace DotNetNuke.Services.Messaging.Providers
    Public Class Factory

        Public Shared Function CreateMessage(ByVal RecipientUserID As Integer) As IMessageProvider
            Return CreateMessage()
        End Function

        'Get the default message type
        Public Shared Function CreateMessage() As IMessageProvider
            Return CreateMessage(GetType(EmailMessage))
        End Function

        'Get the default message type, based on an existing message (clone it)
        Public Shared Function CreateMessage(ByVal PM As Message) As IMessageProvider
            Return CreateMessage(GetType(EmailMessage), PM)
        End Function

        Public Shared Function CreateMessage(ByVal Type As System.Type, ByVal PM As Message) As IMessageProvider
            Dim msg As IMessageProvider = CreateMessage(Type)
            If (Not (msg Is Nothing)) Then
                If Not (PM Is Nothing) Then msg.Init(PM)
            End If
            Return msg
        End Function

        Public Shared Function CreateMessage(ByVal Type As System.Type) As IMessageProvider
            Dim obj As IMessageProvider = TryCast(System.AppDomain.CurrentDomain.CreateInstanceAndUnwrap(Type.Assembly.FullName, Type.FullName), IMessageProvider)
            If (obj Is Nothing) Then Throw New InvalidCastException(String.Format("Could not create an instance of IMessageProvider from Type:{0}, in assembly:{1}" & Type.FullName, Type.Assembly.FullName))
            Return obj
        End Function

    End Class
End Namespace
