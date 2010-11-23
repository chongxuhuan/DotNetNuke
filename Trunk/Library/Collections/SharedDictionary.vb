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

Imports System.Runtime.InteropServices
Imports System.Runtime.CompilerServices

Namespace DotNetNuke.Collections

    Public Class SharedDictionary(Of TKey, TValue)
        Implements IDictionary(Of TKey, TValue)
        Implements IDisposable

        Private _dict As IDictionary(Of TKey, TValue)
        Private _lockController As ILockStrategy
        Private _isDisposed As Boolean

        Public Sub New()
            Me.New(LockingStrategy.ReaderWriter)
        End Sub

        Public Sub New(ByVal lockStrategy As ILockStrategy)
            _dict = New Dictionary(Of TKey, TValue)
            _lockController = lockStrategy
        End Sub

        Public Sub New(ByVal strategy As LockingStrategy)
            Me.New(LockingStrategyFactory.Create(strategy))
        End Sub

        Friend ReadOnly Property BackingDictionary As IDictionary(Of TKey, TValue)
            Get
                Return _dict
            End Get
        End Property

        Public Function GetReadLock() As ISharedCollectionLock
            EnsureNotDisposed()
            Return _lockController.GetReadLock()
        End Function

        Public Function GetWriteLock() As ISharedCollectionLock
            EnsureNotDisposed()
            Return _lockController.GetWriteLock()
        End Function

        Private Sub EnsureReadAccess()
            If Not (_lockController.ThreadCanRead) Then
                Throw New ReadLockRequiredException()
            End If
        End Sub

        Private Sub EnsureWriteAccess()
            If Not _lockController.ThreadCanWrite Then
                Throw New WriteLockRequiredException()
            End If
        End Sub

        Public Function IEnumerable_GetEnumerator() As IEnumerator(Of KeyValuePair(Of TKey, TValue)) Implements IEnumerable _
                                                                                                      (Of KeyValuePair _
                                                                                                      (Of TKey, TValue)).GetEnumerator
            EnsureNotDisposed()
            EnsureReadAccess()

            'todo nothing ensures read lock is held for life of enumerator
            Return _dict.GetEnumerator()
        End Function

        Public Function GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
            Return IEnumerable_GetEnumerator()
        End Function

        Public Sub Add(ByVal item As KeyValuePair(Of TKey, TValue)) Implements ICollection(Of KeyValuePair(Of TKey, TValue)).Add
            EnsureNotDisposed()
            EnsureWriteAccess()
            _dict.Add(item)
        End Sub

        Public Sub Clear() Implements ICollection(Of KeyValuePair(Of TKey, TValue)).Clear
            EnsureNotDisposed()
            EnsureWriteAccess()
            _dict.Clear()
        End Sub

        Public Function Contains(ByVal item As KeyValuePair(Of TKey, TValue)) As Boolean Implements ICollection _
                                                                                          (Of KeyValuePair(Of TKey, TValue)).Contains
            EnsureNotDisposed()
            EnsureReadAccess()
            Return _dict.Contains(item)
        End Function

        Public Sub CopyTo(ByVal array As KeyValuePair(Of TKey, TValue)(), ByVal arrayIndex As Integer) Implements ICollection _
                                                                                                        (Of KeyValuePair _
                                                                                                        (Of TKey, TValue)).CopyTo
            EnsureNotDisposed()
            EnsureReadAccess()
            _dict.CopyTo(array, arrayIndex)
        End Sub

        Public Function Remove(ByVal item As KeyValuePair(Of TKey, TValue)) As Boolean Implements ICollection(Of KeyValuePair _
                                                                                        (Of TKey, TValue)).Remove
            EnsureNotDisposed()
            EnsureWriteAccess()
            Return _dict.Remove(item)
        End Function

        Public ReadOnly Property Count() As Integer Implements ICollection(Of KeyValuePair(Of TKey, TValue)).Count
            Get
                EnsureNotDisposed()
                EnsureReadAccess()
                Return _dict.Count
            End Get
        End Property

        Public ReadOnly Property IsReadOnly() As Boolean Implements ICollection(Of KeyValuePair(Of TKey, TValue)).IsReadOnly
            Get
                EnsureNotDisposed()
                EnsureReadAccess()
                Return _dict.IsReadOnly
            End Get
        End Property

        Public Function ContainsKey(ByVal key As TKey) As Boolean Implements IDictionary(Of TKey, TValue).ContainsKey
            EnsureNotDisposed()
            EnsureReadAccess()
            Return _dict.ContainsKey(key)
        End Function

        Public Sub Add(ByVal key As TKey, ByVal value As TValue) Implements IDictionary(Of TKey, TValue).Add
            EnsureNotDisposed()
            EnsureWriteAccess()
            _dict.Add(key, value)
        End Sub

        Public Function Remove(ByVal key As TKey) As Boolean Implements IDictionary(Of TKey, TValue).Remove
            EnsureNotDisposed()
            EnsureWriteAccess()
            Return _dict.Remove(key)
        End Function

        Public Function TryGetValue(ByVal key As TKey, <Out()> ByRef value As TValue) As Boolean Implements IDictionary _
                                                                                                 (Of TKey, TValue).TryGetValue
            EnsureNotDisposed()
            EnsureReadAccess()
            Return _dict.TryGetValue(key, value)
        End Function

        Default Public Property Item(ByVal key As TKey) As TValue Implements IDictionary(Of TKey, TValue).Item
            Get
                EnsureNotDisposed()
                EnsureReadAccess()
                Return _dict(key)
            End Get
            Set(ByVal value As TValue)
                EnsureNotDisposed()
                EnsureWriteAccess()
                _dict(key) = value
            End Set
        End Property

        Public ReadOnly Property Keys() As ICollection(Of TKey) Implements IDictionary(Of TKey, TValue).Keys
            Get
                EnsureNotDisposed()
                EnsureReadAccess()
                Return _dict.Keys
            End Get
        End Property

        Public ReadOnly Property Values() As ICollection(Of TValue) Implements IDictionary(Of TKey, TValue).Values
            Get
                EnsureNotDisposed()
                EnsureReadAccess()
                Return _dict.Values
            End Get
        End Property

        Private Sub EnsureNotDisposed()
            If _isDisposed Then
                Throw New ObjectDisposedException("SharedDictionary")
            End If
        End Sub

        Public Sub Dispose() Implements IDisposable.Dispose
            Dispose(True)

            GC.SuppressFinalize(Me)
        End Sub

        Protected Overridable Sub Dispose(ByVal disposing As Boolean)
            If Not _isDisposed Then
                If disposing Then
                    'dispose managed resrources here
                    _dict = Nothing
                End If

                'dispose unmanaged resrources here
                _lockController.Dispose()
                _lockController = Nothing
                _isDisposed = True
            End If
        End Sub

        Protected Overrides Sub Finalize()
            Dispose(False)
        End Sub
    End Class

End Namespace

