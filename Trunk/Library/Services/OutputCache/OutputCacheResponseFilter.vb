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

#Region "Private Members"

        Private _cacheDuration As TimeSpan
        Private _cacheKey As String
        Private _captureStream As Stream
        Private _chainedStream As Stream
        Private _hasErrored As Boolean = False
        Private _maxVaryByCount As Integer

#End Region

#Region "Constructors"

        Public Sub New(ByVal filterChain As Stream, ByVal cacheKey As String, ByVal cacheDuration As TimeSpan, ByVal maxVaryByCount As Integer)
            MyBase.New()
            _chainedStream = filterChain
            _cacheKey = cacheKey
            _cacheDuration = cacheDuration
            _maxVaryByCount = maxVaryByCount
            _captureStream = CaptureStream
        End Sub

#End Region

#Region "Public Properties"

        Public Property CacheDuration() As TimeSpan
            Get
                Return _cacheDuration
            End Get
            Set(ByVal value As TimeSpan)
                _cacheDuration = value
            End Set
        End Property

        Public Property CacheKey() As String
            Get
                Return _cacheKey
            End Get
            Set(ByVal value As String)
                _cacheKey = value
            End Set
        End Property

        Public Property CaptureStream() As Stream
            Get
                Return _captureStream
            End Get
            Set(ByVal value As Stream)
                _captureStream = value
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

        Public Property HasErrored() As Boolean
            Get
                Return _hasErrored
            End Get
            Set(ByVal value As Boolean)
                _hasErrored = value
            End Set
        End Property

        Public Property MaxVaryByCount() As Integer
            Get
                Return _maxVaryByCount
            End Get
            Set(ByVal value As Integer)
                _maxVaryByCount = value
            End Set
        End Property

        Public Overrides ReadOnly Property CanRead() As Boolean
            Get
                Return False
            End Get
        End Property

        Public Overrides ReadOnly Property CanSeek() As Boolean
            Get
                Return False
            End Get
        End Property

        Public Overrides ReadOnly Property CanWrite() As Boolean
            Get
                Return True
            End Get
        End Property

        Public Overrides ReadOnly Property Length() As Long
            Get
                Throw New NotSupportedException
            End Get
        End Property

        Public Overrides Property Position() As Long
            Get
                Throw New NotSupportedException
            End Get
            Set(ByVal value As Long)
                Throw New NotSupportedException
            End Set
        End Property

#End Region

#Region "Public Methods"

        Public Overrides Sub Flush()
            ChainedStream.Flush()
            If HasErrored Then Return
            If (Not (_captureStream) Is Nothing) Then
                _captureStream.Flush()
            End If
        End Sub

        Public Overrides Sub Write(ByVal buffer() As Byte, ByVal offset As Integer, ByVal count As Integer)
            ChainedStream.Write(buffer, offset, count)
            If HasErrored Then Return
            If (Not (_captureStream) Is Nothing) Then
                _captureStream.Write(buffer, offset, count)
            End If
        End Sub

        Public Overrides Function Read(ByVal buffer() As Byte, ByVal offset As Integer, ByVal count As Integer) As Integer
            Throw New NotSupportedException
        End Function

        Public Overrides Function Seek(ByVal offset As Long, ByVal origin As SeekOrigin) As Long
            Throw New NotSupportedException
        End Function

        Public Overrides Sub SetLength(ByVal value As Long)
            Throw New NotSupportedException
        End Sub

#End Region

        Protected Overridable Sub AddItemToCache(ByVal itemId As Integer, ByVal output As String)
        End Sub

        Protected Overridable Sub RemoveItemFromCache(ByVal itemId As Integer)
        End Sub

        Public Overridable Sub StopFiltering(ByVal itemId As Integer, ByVal deleteData As Boolean)
            If HasErrored Then Exit Sub

            If (Not (CaptureStream) Is Nothing) Then
                CaptureStream.Position = 0
                Dim reader As New StreamReader(CaptureStream, System.Text.Encoding.Default)
                Dim output As String = reader.ReadToEnd()
                AddItemToCache(itemId, output)
                CaptureStream.Close()
                CaptureStream = Nothing
            End If
            If deleteData Then
                RemoveItemFromCache(itemId)
            End If
        End Sub

    End Class
End Namespace

