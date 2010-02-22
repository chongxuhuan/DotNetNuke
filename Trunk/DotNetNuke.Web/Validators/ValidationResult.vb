'
' DotNetNuke - http://www.dotnetnuke.com
' Copyright (c) 2002-2009
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

Imports DotNetNuke.Common

Namespace DotNetNuke.Web.Validators
    Public Class ValidationResult

#Region "Private Members"

        Private _Errors As IEnumerable(Of ValidationError)

#End Region

#Region "Constructors"

        Public Sub New()
            _Errors = Enumerable.Empty(Of ValidationError)()
        End Sub

        Public Sub New(ByVal errors As IEnumerable(Of ValidationError))
            Arg.NotNull("errors", errors)
            _Errors = errors
        End Sub

#End Region

#Region "Public Properties"

        Public ReadOnly Property Errors() As IEnumerable(Of ValidationError)
            Get
                Return _Errors
            End Get
        End Property

        Public ReadOnly Property IsValid() As Boolean
            Get
                Return (_Errors.Count() = 0)
            End Get
        End Property

        Public Shared ReadOnly Property Successful() As ValidationResult
            Get
                Return New ValidationResult()
            End Get
        End Property

#End Region

#Region "Public Methods"

        Public Function CombineWith(ByVal other As ValidationResult) As ValidationResult
            Arg.NotNull("other", other)

            'Just concatenate the errors collection
            Return New ValidationResult(_Errors.Concat(other.Errors))
        End Function

#End Region

    End Class
End Namespace
