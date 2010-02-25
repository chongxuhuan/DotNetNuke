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

Imports System
Imports System.IO

Namespace DotNetNuke.Services.OutputCache
    ' helper class to capture the response into a file
    Public MustInherit Class OutputCacheResponseFilter
        Inherits Stream

        Private _chainedStream As Stream
        Private _cacheKey As String

        Public Property CacheKey() As String
            Get
                Return _cacheKey
            End Get
            Set(ByVal value As String)
                _cacheKey = value
            End Set
        End Property


        Public Property ChainedStream() As Stream
            Get
                Return _chainedStream
            End Get
            Set(ByVal value As Stream)
                _chainedStream = value
            End Set
        End Property

        Public Sub New(ByVal filterChain As Stream, ByVal cacheKey As String)
            MyBase.New()
            _chainedStream = filterChain
            _cacheKey = cacheKey
        End Sub

        Public MustOverride Overrides ReadOnly Property CanRead() As Boolean

        Public MustOverride Overrides ReadOnly Property CanSeek() As Boolean

        Public MustOverride Overrides ReadOnly Property CanWrite() As Boolean

        Public MustOverride Overrides ReadOnly Property Length() As Long

        Public MustOverride Overrides Property Position() As Long

        Public MustOverride Function StopFiltering(ByVal itemId As Integer, ByVal deleteData As Boolean) As Byte()

        Public MustOverride Overrides Sub Flush()

        Public MustOverride Overrides Sub Write(ByVal buffer() As Byte, ByVal offset As Integer, ByVal count As Integer)

        Public MustOverride Overrides Function Read(ByVal buffer() As Byte, ByVal offset As Integer, ByVal count As Integer) As Integer

        Public MustOverride Overrides Function Seek(ByVal offset As Long, ByVal origin As SeekOrigin) As Long

        Public MustOverride Overrides Sub SetLength(ByVal value As Long)

    End Class
End Namespace

