'
' DotNetNuke® - http://www.dotnetnuke.com
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

Imports System

Namespace DotNetNuke.Services.Cache

    Public Class DNNCacheDependency
        Implements IDisposable

#Region "Private Members"

        Private _fileNames As String() = Nothing
        Private _cacheKeys As String() = Nothing
        Private _utcStart As DateTime = DateTime.MaxValue
        Private _cacheDependency As DNNCacheDependency = Nothing
        Private _systemCacheDependency As System.Web.Caching.CacheDependency = Nothing

#End Region

#Region "Constructors"

        Public Sub New(ByVal systemCacheDependency As System.Web.Caching.CacheDependency)
            _systemCacheDependency = systemCacheDependency
        End Sub

        Public Sub New(ByVal filename As String)
            _fileNames = New String() {filename}
        End Sub

        Public Sub New(ByVal filenames As String())
            _fileNames = filenames
        End Sub

        Public Sub New(ByVal filenames As String(), ByVal start As DateTime)
            _utcStart = start.ToUniversalTime()
            _fileNames = filenames
        End Sub

        Public Sub New(ByVal filenames As String(), ByVal cachekeys As String())
            _fileNames = filenames
            _cacheKeys = cachekeys
        End Sub

        Public Sub New(ByVal filename As String, ByVal start As DateTime)
            _utcStart = start.ToUniversalTime()
            If filename IsNot Nothing Then
                _fileNames = New String() {filename}
            End If
        End Sub

        Public Sub New(ByVal filenames As String(), ByVal cachekeys As String(), ByVal start As DateTime)
            _utcStart = start.ToUniversalTime()
            _fileNames = filenames
            _cacheKeys = cachekeys
        End Sub

        Public Sub New(ByVal filenames As String(), ByVal cachekeys As String(), ByVal dependency As DNNCacheDependency)
            _fileNames = filenames
            _cacheKeys = cachekeys
            _cacheDependency = dependency
        End Sub

        Public Sub New(ByVal filenames As String(), ByVal cachekeys As String(), ByVal dependency As DNNCacheDependency, ByVal start As DateTime)
            _utcStart = start.ToUniversalTime()
            _fileNames = filenames
            _cacheKeys = cachekeys
            _cacheDependency = dependency
        End Sub

#End Region

#Region "Public Properties"

        Public ReadOnly Property CacheKeys() As String()
            Get
                Return _cacheKeys
            End Get
        End Property

        Public ReadOnly Property FileNames() As String()
            Get
                Return _fileNames
            End Get
        End Property

        Public ReadOnly Property HasChanged() As Boolean
            Get
                Return SystemCacheDependency.HasChanged
            End Get
        End Property

        Public ReadOnly Property CacheDependency() As DNNCacheDependency
            Get
                Return _cacheDependency
            End Get
        End Property

        Public ReadOnly Property StartTime() As DateTime
            Get
                Return _utcStart
            End Get
        End Property

        Public ReadOnly Property SystemCacheDependency() As System.Web.Caching.CacheDependency
            Get
                Try
                    If _systemCacheDependency Is Nothing Then
                        If _cacheDependency Is Nothing Then
                            _systemCacheDependency = New System.Web.Caching.CacheDependency(_fileNames, _cacheKeys, _utcStart)
                        Else
                            _systemCacheDependency = New System.Web.Caching.CacheDependency(_fileNames, _cacheKeys, _cacheDependency.SystemCacheDependency, _utcStart)
                        End If
                    End If
                    Return _systemCacheDependency
                Catch ex As Exception
                    Throw ex
                End Try
            End Get
        End Property

        Public ReadOnly Property UtcLastModified() As DateTime
            Get
                Return SystemCacheDependency.UtcLastModified
            End Get
        End Property

#End Region

#Region " IDisposable Support "
        Public Overloads Sub Dispose() Implements IDisposable.Dispose
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub

        Protected Overrides Sub Finalize()
            Dispose(False)
            MyBase.Finalize()
        End Sub

        'Method that does the actual disposal of resources
        Protected Overridable Overloads Sub Dispose(ByVal disposing As Boolean)
            If (disposing) Then
                If Not _cacheDependency Is Nothing Then _cacheDependency.Dispose()
                If Not _systemCacheDependency Is Nothing Then _systemCacheDependency.Dispose()
                _fileNames = Nothing
                _cacheKeys = Nothing
                _cacheDependency = Nothing
                _systemCacheDependency = Nothing
            End If
        End Sub
#End Region

    End Class
End Namespace