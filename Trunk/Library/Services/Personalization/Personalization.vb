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
Imports System.IO
Imports System.Runtime.Serialization.Formatters.Binary
Imports System.Collections

Namespace DotNetNuke.Services.Personalization

    Public Class Personalization

#Region "Private Methods"

        Private Shared Function LoadProfile() As PersonalizationInfo
            Dim context As HttpContext = HttpContext.Current

            'First try and load Personalization object from the Context
            Dim objPersonalization As PersonalizationInfo = CType(context.Items("Personalization"), PersonalizationInfo)

            'If the Personalization object is nothing load it and store it in the context for future calls
            If objPersonalization Is Nothing Then
                ' Obtain PortalSettings from Current Context
                Dim _portalSettings As PortalSettings = CType(context.Items("PortalSettings"), PortalSettings)

                ' load the user info object
                Dim UserInfo As Entities.Users.UserInfo = Entities.Users.UserController.GetCurrentUserInfo

                ' get the personalization object
                Dim objPersonalizationController As New PersonalizationController
                objPersonalization = objPersonalizationController.LoadProfile(UserInfo.UserID, _portalSettings.PortalId)

                'store it in the context
                context.Items.Add("Personalization", objPersonalization)
            End If

            Return objPersonalization
        End Function

#End Region

#Region "Public Shared Methods"

        Public Shared Function GetProfile(ByVal NamingContainer As String, ByVal Key As String) As Object
            Return GetProfile(LoadProfile(), NamingContainer, Key)
        End Function

        Public Shared Function GetProfile(ByVal objPersonalization As PersonalizationInfo, ByVal NamingContainer As String, ByVal Key As String) As Object
            If Not objPersonalization Is Nothing Then
                Return objPersonalization.Profile(NamingContainer & ":" & Key)
            Else
                Return ""
            End If
        End Function

        Public Shared Sub RemoveProfile(ByVal NamingContainer As String, ByVal Key As String)
            RemoveProfile(LoadProfile(), NamingContainer, Key)
        End Sub

        Public Shared Sub RemoveProfile(ByVal objPersonalization As PersonalizationInfo, ByVal NamingContainer As String, ByVal Key As String)
            If Not objPersonalization Is Nothing Then
                CType(objPersonalization.Profile, Hashtable).Remove(NamingContainer & ":" & Key)
                objPersonalization.IsModified = True
            End If
        End Sub

        Public Shared Sub SetProfile(ByVal NamingContainer As String, ByVal Key As String, ByVal Value As Object)
            SetProfile(LoadProfile(), NamingContainer, Key, Value)
        End Sub

        Public Shared Sub SetProfile(ByVal objPersonalization As PersonalizationInfo, ByVal NamingContainer As String, ByVal Key As String, ByVal Value As Object)
            If Not objPersonalization Is Nothing Then
                objPersonalization.Profile(NamingContainer & ":" & Key) = Value
                objPersonalization.IsModified = True
            End If
        End Sub

#End Region

    End Class

End Namespace