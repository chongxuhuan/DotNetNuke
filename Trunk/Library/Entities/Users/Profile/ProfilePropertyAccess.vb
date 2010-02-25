'
' DotNetNuke® - http://www.dotnetnuke.com
' Copyright (c) 2002-2010 by DotNetNuke Corp. 
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
Imports DotNetNuke.Entities.Users
Imports DotNetNuke.Entities.Profile
Imports DotNetNuke.Services.Tokens

Imports System.Reflection

Namespace DotNetNuke.Entities.Users
    Public Class ProfilePropertyAccess
        Implements IPropertyAccess

        Dim objUser As UserInfo
        Dim strAdministratorRoleName As String = ""

        Public Sub New(ByVal user As UserInfo)
            Me.objUser = user
        End Sub

        Public Function GetProperty(ByVal strPropertyName As String, ByVal strFormat As String, ByVal formatProvider As System.Globalization.CultureInfo, ByVal AccessingUser As Entities.Users.UserInfo, ByVal currentScope As Scope, ByRef PropertyNotFound As Boolean) As String Implements IPropertyAccess.GetProperty
            If _
                currentScope >= Scope.DefaultSettings _
                AndAlso _
                Not (objUser Is Nothing OrElse objUser.Profile Is Nothing) _
                Then

                Dim objProfile As UserProfile = objUser.Profile
                Dim prop As ProfilePropertyDefinition

                For Each prop In objProfile.ProfileProperties
                    If prop.PropertyName.ToLower = strPropertyName.ToLower Then
                        If CheckAccessLevel(prop.Visibility, AccessingUser) Then

                            Return GetRichValue(prop, strFormat, formatProvider)
                        Else
                            PropertyNotFound = True : Return PropertyAccess.ContentLocked
                        End If
                        Exit For
                    End If
                Next prop

            End If
            PropertyNotFound = True : Return String.Empty

        End Function

        Public Shared Function GetRichValue(ByVal prop As ProfilePropertyDefinition, ByVal strFormat As String, ByVal formatProvider As System.Globalization.CultureInfo) As String
            Dim result As String = ""
            If Not prop.PropertyValue = String.Empty Then
                Select Case DisplayDataType(prop).ToLower
                    Case "truefalse"
                        result = PropertyAccess.Boolean2LocalizedYesNo(CBool(prop.PropertyValue), formatProvider)
                    Case "date", "datetime"
                        If strFormat = String.Empty Then strFormat = "g"
                        result = DateTime.Parse(prop.PropertyValue).ToString(strFormat, formatProvider)
                    Case "integer"
                        If strFormat = String.Empty Then strFormat = "g"
                        result = Integer.Parse(prop.PropertyValue).ToString(strFormat, formatProvider)
                    Case "page"
                        Dim TabCtrl As New Entities.Tabs.TabController
                        Dim tabid As Integer
                        If Integer.TryParse(prop.PropertyValue, tabid) Then
                            Dim Tab As Entities.Tabs.TabInfo = TabCtrl.GetTab(tabid, Null.NullInteger, False)
                            If Not Tab Is Nothing Then
                                result = String.Format("<a href='{0}'>{1}</a>", NavigateURL(tabid), Tab.LocalizedTabName)
                            End If
                        End If
                    Case "richtext"
                        result = PropertyAccess.FormatString(HttpUtility.HtmlDecode(prop.PropertyValue), strFormat)
                    Case Else
                        result = PropertyAccess.FormatString(prop.PropertyValue, strFormat)
                End Select
            End If
            Return result
        End Function

        Private Shared Function DisplayDataType(ByVal definition As Entities.Profile.ProfilePropertyDefinition) As String
            Dim CacheKey As String = String.Format("DisplayDataType:{0}", definition.DataType)
            Dim strDataType As String = CType(DataCache.GetCache(CacheKey), String) & ""
            If strDataType = String.Empty Then
                Dim objListController As New Common.Lists.ListController
                strDataType = objListController.GetListEntryInfo(definition.DataType).Value
                DataCache.SetCache(CacheKey, strDataType)
            End If
            Return strDataType
        End Function

        Private Function CheckAccessLevel(ByVal VisibilityMode As UserVisibilityMode, ByVal AccessingUser As Entities.Users.UserInfo) As Boolean
            If strAdministratorRoleName = "" AndAlso Not AccessingUser.IsSuperUser Then
                Dim ps As DotNetNuke.Entities.Portals.PortalInfo = New DotNetNuke.Entities.Portals.PortalController().GetPortal(objUser.PortalID)
                strAdministratorRoleName = ps.AdministratorRoleName
            End If
            Return _
                    VisibilityMode = UserVisibilityMode.AllUsers _
                    OrElse _
                    (VisibilityMode = UserVisibilityMode.MembersOnly And Not AccessingUser Is Nothing) _
                    OrElse _
                    (AccessingUser.IsSuperUser OrElse objUser.UserID = AccessingUser.UserID OrElse AccessingUser.IsInRole(strAdministratorRoleName))
        End Function

        Public ReadOnly Property Cacheability() As CacheLevel Implements Services.Tokens.IPropertyAccess.Cacheability
            Get
                Return CacheLevel.notCacheable
            End Get
        End Property

    End Class

End Namespace
