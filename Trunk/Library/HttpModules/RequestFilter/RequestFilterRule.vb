'
' DotNetNuke® - http:'www.dotnetnuke.com
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
Imports System.Collections.Generic
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Web

Namespace DotNetNuke.HttpModules.RequestFilter
    <Serializable()> _
    Public Class RequestFilterRule

        Public Sub SetValues(ByVal values As String, ByVal op As RequestFilterOperatorType)
            _Values.Clear()
            If (op <> RequestFilterOperatorType.Regex) Then
                Dim vals As String() = values.Split(New Char() {" "c}, StringSplitOptions.RemoveEmptyEntries)
                For Each value As String In vals
                    _Values.Add(value.ToUpperInvariant())
                Next
            Else
                ' we do not want to split a regex string
                _Values.Add(values)
            End If
        End Sub

        ''' <summary>
        ''' Initializes a new instance of the RequestFilterRule class.
        ''' </summary>
        ''' <param name="serverVariable"></param>
        ''' <param name="values"></param>
        ''' <param name="action"></param>
        ''' <param name="location"></param>
        Public Sub New(ByVal serverVariable As String, ByVal values As String, ByVal op As RequestFilterOperatorType, ByVal action As RequestFilterRuleType, ByVal location As String)
            _ServerVariable = serverVariable
            SetValues(values, op)
            _Operator = op
            _Action = action
            _Location = location
        End Sub

        ''' <summary>
        ''' Initializes a new instance of the RequestFilterRule class.
        ''' </summary>
        Public Sub New()
        End Sub

        Private _ServerVariable As String
        Public Property ServerVariable() As String
            Get
                Return _ServerVariable
            End Get
            Set(ByVal value As String)
                _ServerVariable = value
            End Set
        End Property

        Private _Values As New List(Of String)()
        Public Property Values() As List(Of String)
            Get
                Return _Values
            End Get
            Set(ByVal value As List(Of String))
                _Values = value
            End Set
        End Property

        Public ReadOnly Property RawValue() As String
            Get
                Return String.Join(" ", _Values.ToArray())
            End Get
        End Property

        Private _Action As RequestFilterRuleType
        Public Property Action() As RequestFilterRuleType
            Get
                Return _Action
            End Get
            Set(ByVal value As RequestFilterRuleType)
                _Action = value
            End Set
        End Property

        Private _Operator As RequestFilterOperatorType
        Public Property [Operator]() As RequestFilterOperatorType
            Get
                Return _Operator
            End Get
            Set(ByVal value As RequestFilterOperatorType)
                _Operator = value
            End Set
        End Property

        Private _Location As String
        Public Property Location() As String
            Get
                Return _Location
            End Get
            Set(ByVal value As String)
                _Location = value
            End Set
        End Property

        Public Function Matches(ByVal ServerVariableValue As String) As Boolean
            Select Case [Operator]
                Case RequestFilterOperatorType.Equal
                    Return Values.Contains(ServerVariableValue.ToUpperInvariant())
                Case RequestFilterOperatorType.NotEqual
                    Return Not Values.Contains(ServerVariableValue.ToUpperInvariant())
                Case RequestFilterOperatorType.Regex
                    Return Regex.IsMatch(ServerVariableValue, Values(0), RegexOptions.IgnoreCase Or RegexOptions.CultureInvariant)
            End Select
            Return False
        End Function

        Public Sub Execute()
            Dim response As HttpResponse = HttpContext.Current.Response
            Select Case Action
                Case RequestFilterRuleType.Redirect
                    response.Redirect(Location, True)
                Case RequestFilterRuleType.PermanentRedirect
                    response.StatusCode = 301
                    response.Status = "301 Moved Permanently"
                    response.RedirectLocation = Location
                    response.End()
                Case RequestFilterRuleType.NotFound
                    response.StatusCode = 404
                    response.SuppressContent = True
                    response.End()
            End Select
        End Sub

    End Class

End Namespace
