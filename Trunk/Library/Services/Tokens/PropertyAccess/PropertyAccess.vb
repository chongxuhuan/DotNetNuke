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
Imports DotNetNuke.Services.Localization
Imports System.Reflection

Namespace DotNetNuke.Services.Tokens


    ''' <summary>
    ''' Property Access to Objects using Relection
    ''' </summary>
    ''' <remarks></remarks>
    Public Class PropertyAccess
        Implements IPropertyAccess

        Dim obj As Object

        Public Sub New(ByVal TokenSource As Object)
            obj = TokenSource
        End Sub

        Public Function GetProperty(ByVal strPropertyName As String, ByVal strFormat As String, ByVal formatProvider As System.Globalization.CultureInfo, ByVal AccessingUser As Entities.Users.UserInfo, ByVal AccessLevel As Scope, ByRef PropertyNotFound As Boolean) As String Implements IPropertyAccess.GetProperty
            If obj Is Nothing Then Return String.Empty
            Return PropertyAccess.GetObjectProperty(obj, strPropertyName, strFormat, formatProvider, PropertyNotFound)
        End Function

        Public ReadOnly Property Cacheability() As CacheLevel Implements Services.Tokens.IPropertyAccess.Cacheability
            Get
                Return CacheLevel.notCacheable
            End Get
        End Property

#Region "Shared Public Helper Functions"
        Public Shared ReadOnly Property ContentLocked() As String
            Get
                Return "*******"
            End Get
        End Property

        ''' <summary>
        ''' Boolean2LocalizedYesNo returns the translated string for "yes" or "no" against a given boolean value. 
        ''' </summary>
        ''' <param name="value"></param>
        ''' <param name="formatProvider"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function Boolean2LocalizedYesNo(ByVal value As Boolean, ByVal formatProvider As Globalization.CultureInfo) As String
            Dim strValue As String = CStr(IIf(value, "Yes", "No"))
            Return Localization.Localization.GetString(strValue, Nothing, formatProvider.ToString())
        End Function

        ''' <summary>
        ''' Returns a formatted String if a format is given, otherwise it returns the unchanged value. 
        ''' </summary>
        ''' <param name="value">string to be formatted</param>
        ''' <param name="format">format specification</param>
        ''' <returns>formatted string</returns>
        ''' <remarks></remarks>
        Public Shared Function FormatString(ByVal value As String, ByVal format As String) As String
            If format.Trim = String.Empty Then
                Return value
            ElseIf value <> String.Empty Then
                Return String.Format(format, value)
            Else
                Return String.Empty
            End If
        End Function

        ''' <summary>
        '''     Returns the localized property of any object as string using reflection
        ''' </summary>
        ''' <param name="objObject">Object to access</param>
        ''' <param name="strPropertyName">Name of property</param>
        ''' <param name="strFormat">Format String</param>
        ''' <param name="formatProvider">specify formatting</param>
        ''' <param name="PropertyNotFound">out: specifies, whether property was found</param>
        ''' <returns>Localized Property</returns>
        ''' <remarks></remarks>
        Public Shared Function GetObjectProperty(ByVal objObject As Object, ByVal strPropertyName As String, ByVal strFormat As String, ByVal formatProvider As Globalization.CultureInfo, ByRef PropertyNotFound As Boolean) As String
            Dim objProperty As PropertyInfo = Nothing
            PropertyNotFound = False

            If CBO.GetProperties(objObject.GetType).TryGetValue(strPropertyName, objProperty) Then
                Dim propValue As Object = objProperty.GetValue(objObject, Nothing)
                Dim t As Type = GetType(String)
                If Not propValue Is Nothing Then
                    Select Case objProperty.PropertyType.Name
                        Case "String"
                            Return FormatString(CStr(propValue), strFormat)
                        Case "Boolean"
                            Return (PropertyAccess.Boolean2LocalizedYesNo(CBool(propValue), formatProvider))
                        Case "DateTime", "Double", "Single", "Int32", "Int64"
                            If strFormat = String.Empty Then strFormat = "g"
                            Return (CType(propValue, IFormattable).ToString(strFormat, formatProvider))
                    End Select
                Else
                    Return ""
                End If
            End If

            PropertyNotFound = True : Return String.Empty
        End Function
#End Region


    End Class

End Namespace
