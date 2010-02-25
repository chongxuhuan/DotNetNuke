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
Imports System.Security.Permissions
Imports System.Collections

Namespace DotNetNuke.Services.Personalization

    Public Class PersonalizationController

        ' default implementation relies on HTTPContext
        Public Sub LoadProfile(ByVal objHTTPContext As HttpContext, ByVal UserId As Integer, ByVal PortalId As Integer)
            If CType(HttpContext.Current.Items("Personalization"), PersonalizationInfo) Is Nothing Then
                objHTTPContext.Items.Add("Personalization", LoadProfile(UserId, PortalId))
            End If
        End Sub

        ' override allows for manipulation of PersonalizationInfo outside of HTTPContext
        Public Function LoadProfile(ByVal UserId As Integer, ByVal PortalId As Integer) As PersonalizationInfo

            Dim objPersonalization As New PersonalizationInfo

            objPersonalization.UserId = UserId
            objPersonalization.PortalId = PortalId
            objPersonalization.IsModified = False

            Dim profileData As String = Null.NullString
            If UserId > Null.NullInteger Then
                Dim dr As IDataReader = Nothing
                Try
                    dr = DataProvider.Instance().GetProfile(UserId, PortalId)
                    If dr.Read Then
                        profileData = dr("ProfileData").ToString
                    Else ' does not exist
                        Try
                            DataProvider.Instance().AddProfile(UserId, PortalId)
                        Catch
                            ' a duplicate key error may occur due to race conditions if the Profile is not initialized early enough in the page life cycle
                        End Try
                    End If
                Catch ex As Exception
                    LogException(ex)
                Finally
                    CBO.CloseDataReader(dr, True)
                End Try
            Else
                'Anon User - so try and use cookie.
                Dim context As HttpContext = HttpContext.Current
                If context IsNot Nothing AndAlso context.Request IsNot Nothing _
                        AndAlso context.Request.Cookies("DNNPersonalization") IsNot Nothing Then
                    profileData = context.Request.Cookies("DNNPersonalization").Value
                End If
            End If

            If String.IsNullOrEmpty(profileData) Then
                objPersonalization.Profile = New Hashtable
            Else
                objPersonalization.Profile = DeserializeHashTableXml(profileData)
            End If

            Return objPersonalization

        End Function

        Public Sub SaveProfile(ByVal objPersonalization As PersonalizationInfo)
            SaveProfile(objPersonalization, objPersonalization.UserId, objPersonalization.PortalId)
        End Sub

        ' default implementation relies on HTTPContext
        Public Sub SaveProfile(ByVal objHTTPContext As HttpContext, ByVal UserId As Integer, ByVal PortalId As Integer)
            Dim objPersonalization As PersonalizationInfo = CType(objHTTPContext.Items("Personalization"), PersonalizationInfo)
            SaveProfile(objPersonalization, UserId, PortalId)
        End Sub

        ' override allows for manipulation of PersonalizationInfo outside of HTTPContext
        Public Sub SaveProfile(ByVal objPersonalization As PersonalizationInfo, ByVal UserId As Integer, ByVal PortalId As Integer)
            If Not objPersonalization Is Nothing Then
                If objPersonalization.IsModified Then
                    Dim ProfileData As String = SerializeHashTableXml(objPersonalization.Profile)
                    If UserId > Null.NullInteger Then
                        DataProvider.Instance().UpdateProfile(UserId, PortalId, ProfileData)
                    Else
                        'Anon User - so try and use cookie.
                        Dim context As HttpContext = HttpContext.Current
                        If context IsNot Nothing AndAlso context.Response IsNot Nothing Then
                            Dim personalizationCookie As HttpCookie = New HttpCookie("DNNPersonalization")
                            personalizationCookie.Value = ProfileData
                            personalizationCookie.Expires = Now.AddDays(30)
                            context.Response.Cookies.Add(personalizationCookie)
                        End If
                    End If
                End If
            End If
        End Sub

    End Class

End Namespace