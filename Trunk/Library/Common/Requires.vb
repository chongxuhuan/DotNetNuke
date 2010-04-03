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

Namespace DotNetNuke.Common

    Public Module Requires

#Region "Public Methods"

        Public Sub IsTypeOf(Of T)(ByVal argName As String, ByVal argValue As Object)
            If Not (TypeOf (argValue) Is T) Then
                Throw New ArgumentException(Localization.GetExceptionMessage("ValueMustBeOfType", _
                                                                             "The argument {0} must be of type {1}.", _
                                                                             argName, _
                                                                             GetType(T).FullName))
            End If
        End Sub

        Public Sub NotNegative(ByVal argName As String, ByVal argValue As Integer)
            If argValue < 0 Then
                Throw New ArgumentOutOfRangeException(argName, Localization.GetExceptionMessage("ValueCannotBeNegative", _
                                                                                                "The argument {0} cannot be negative.", _
                                                                                                argName))
            End If
        End Sub

        Public Sub NotNull(ByVal argName As String, ByVal argValue As Object)
            If argValue Is Nothing Then
                Throw New ArgumentNullException(argName)
            End If
        End Sub

        Public Sub NotNullOrEmpty(ByVal argName As String, ByVal argValue As String)
            If String.IsNullOrEmpty(argValue) Then
                Throw New ArgumentException(argName)
            End If
        End Sub

        Public Sub PropertyNotNullOrEmpty(ByVal argName As String, ByVal argProperty As String, ByVal propertyValue As String)
            If String.IsNullOrEmpty(propertyValue) Then
                Throw New ArgumentException(argName, Localization.GetExceptionMessage("PropertyCannotBeNullOrEmpty", _
                                                                                      "The property {1} in object {0} cannot be null or empty.", _
                                                                                      argName, _
                                                                                      argProperty))
            End If
        End Sub

        Public Sub PropertyNotNegative(ByVal argName As String, ByVal argProperty As String, ByVal propertyValue As Integer)
            If propertyValue < 0 Then
                Throw New ArgumentOutOfRangeException(argName, Localization.GetExceptionMessage("PropertyCannotBeNegative", _
                                                                                                "The property {1} in object {0} cannot be negative.", _
                                                                                                argName, _
                                                                                                argProperty))
            End If
        End Sub

        Public Sub PropertyNotEqualTo(Of TValue As IEquatable(Of TValue))(ByVal argName As String, ByVal argProperty As String, ByVal propertyValue As TValue, ByVal testValue As TValue)
            If propertyValue.Equals(testValue) Then
                Throw New ArgumentException(argName, Localization.GetExceptionMessage("PropertyNotEqualTo", _
                                                                                      "The property {1} in object {0} is invalid.", _
                                                                                      argName, _
                                                                                      argProperty))
            End If
        End Sub

#End Region

    End Module

    Public Module Arg

#Region "Public Methods"

        <Obsolete("Deprecated in DNN 5.4.0.  Replaced by Requires.IsTypeOf()")> _
        Public Sub IsTypeOf(Of T)(ByVal argName As String, ByVal argValue As Object)
            Requires.IsTypeOf(Of T)(argName, argValue)
        End Sub

        <Obsolete("Deprecated in DNN 5.4.0.  Replaced by Requires.NotNegative()")> _
        Public Sub NotNegative(ByVal argName As String, ByVal argValue As Integer)
            Requires.NotNegative(argName, argValue)
        End Sub

        <Obsolete("Deprecated in DNN 5.4.0.  Replaced by Requires.NotNull()")> _
        Public Sub NotNull(ByVal argName As String, ByVal argValue As Object)
            Requires.NotNull(argName, argValue)
        End Sub

        <Obsolete("Deprecated in DNN 5.4.0.  Replaced by Requires.NotNullOrEmpty()")> _
        Public Sub NotNullOrEmpty(ByVal argName As String, ByVal argValue As String)
            Requires.NotNullOrEmpty(argName, argValue)
        End Sub

        <Obsolete("Deprecated in DNN 5.4.0.  Replaced by Requires.PropertyNotNullOrEmpty()")> _
        Public Sub PropertyNotNullOrEmpty(ByVal argName As String, ByVal argProperty As String, ByVal propertyValue As String)
            Requires.PropertyNotNullOrEmpty(argName, argProperty, propertyValue)
        End Sub

        <Obsolete("Deprecated in DNN 5.4.0.  Replaced by Requires.PropertyNotNegative()")> _
        Public Sub PropertyNotNegative(ByVal argName As String, ByVal argProperty As String, ByVal propertyValue As Integer)
            Requires.PropertyNotNegative(argName, argProperty, propertyValue)
        End Sub

        <Obsolete("Deprecated in DNN 5.4.0.  Replaced by Requires.PropertyNotEqualTo()")> _
        Public Sub PropertyNotEqualTo(Of TValue As IEquatable(Of TValue))(ByVal argName As String, ByVal argProperty As String, ByVal propertyValue As TValue, ByVal testValue As TValue)
            Requires.PropertyNotEqualTo(Of TValue)(argName, argProperty, propertyValue, testValue)
        End Sub

#End Region

    End Module

End Namespace


