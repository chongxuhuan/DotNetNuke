'
' DotNetNuke® - http://www.dotnetnuke.com
' Copyright (c) 2002-2009 by DotNetNuke Corp. 
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

Imports DotNetNuke
Imports DotNetNuke.Services.Tokens

Imports System.Reflection

Namespace DotNetNuke.Entities.Users

    Public Class MembershipPropertyAccess
        Implements IPropertyAccess
        Dim objUser As UserInfo
        Dim isSecure As Boolean

        Public Sub New(ByVal User As UserInfo)
            objUser = User
        End Sub

        Public Function GetProperty(ByVal strPropertyName As String, ByVal strFormat As String, ByVal formatProvider As System.Globalization.CultureInfo, ByVal AccessingUser As Entities.Users.UserInfo, ByVal CurrentScope As Scope, ByRef PropertyNotFound As Boolean) As String Implements IPropertyAccess.GetProperty

            Dim objMembership As UserMembership = objUser.Membership
            Dim UserQueriesHimself As Boolean = (objUser.UserID = AccessingUser.UserID And objUser.UserID <> -1)

            If _
                CurrentScope < Scope.DefaultSettings _
                OrElse _
                (CurrentScope = Scope.DefaultSettings And Not UserQueriesHimself) _
                OrElse _
                (CurrentScope <> Scope.SystemMessages OrElse objUser.IsSuperUser) AndAlso strPropertyName.ToLower Like "password*" _
                Then

                PropertyNotFound = True : Return PropertyAccess.ContentLocked

            Else

                Dim OutputFormat As String = String.Empty
                If strFormat = String.Empty Then OutputFormat = "g"
                Select Case strPropertyName.ToLower
                    Case "approved"
                        Return (PropertyAccess.Boolean2LocalizedYesNo(objMembership.Approved, formatProvider))
                    Case "createdondate"
                        Return (objMembership.CreatedDate.ToString(OutputFormat, formatProvider))
                    Case "isonline"
                        Return (PropertyAccess.Boolean2LocalizedYesNo(objMembership.IsOnLine, formatProvider))
                    Case "lastactivitydate"
                        Return (objMembership.LastActivityDate.ToString(OutputFormat, formatProvider))
                    Case "lastlockoutdate"
                        Return (objMembership.LastLockoutDate.ToString(OutputFormat, formatProvider))
                    Case "lastlogindate"
                        Return (objMembership.LastLoginDate.ToString(OutputFormat, formatProvider))
                    Case "lastpasswordchangedate"
                        Return (objMembership.LastPasswordChangeDate.ToString(OutputFormat, formatProvider))
                    Case "lockedout"
                        Return (PropertyAccess.Boolean2LocalizedYesNo(objMembership.LockedOut, formatProvider))
                    Case "objecthydrated"
                        Return (PropertyAccess.Boolean2LocalizedYesNo(True, formatProvider))
                    Case "password"
                        Return PropertyAccess.FormatString(objMembership.Password, strFormat)
                    Case "passwordanswer"
                        Return PropertyAccess.FormatString(objMembership.PasswordAnswer, strFormat)
                    Case "passwordquestion"
                        Return PropertyAccess.FormatString(objMembership.PasswordQuestion, strFormat)
                    Case "updatepassword"
                        Return (PropertyAccess.Boolean2LocalizedYesNo(objMembership.UpdatePassword, formatProvider))
                    Case "username"
                        Return (PropertyAccess.FormatString(objUser.Username, strFormat))
                    Case "email"
                        Return (PropertyAccess.FormatString(objUser.Email, strFormat))
                End Select
            End If
            Return PropertyAccess.GetObjectProperty(objMembership, strPropertyName, strFormat, formatProvider, PropertyNotFound)

        End Function

        Public ReadOnly Property Cacheability() As CacheLevel Implements Services.Tokens.IPropertyAccess.Cacheability
            Get
                Return CacheLevel.notCacheable
            End Get
        End Property
    End Class
End Namespace
