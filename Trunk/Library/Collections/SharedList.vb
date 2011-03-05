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

Namespace DotNetNuke.Collections.Internal

    Public Class SharedList(Of T)
        Implements IList(Of T)
        Implements IDisposable

        Dim _lockStrategy As ILockStrategy
        Dim _list As List(Of T) = New List(Of T)

        Public Sub New()
            Me.New(LockingStrategy.ReaderWriter)
        End Sub

        Public Sub New(ByVal lockStrategy As ILockStrategy)
            _lockStrategy = lockStrategy
        End Sub

        Public Sub New(ByVal strategy As LockingStrategy)
            Me.New(LockingStrategyFactory.Create(strategy))
        End Sub

        Friend ReadOnly Property BackingList As IList(Of T)
            Get
                Return _list
            End Get
        End Property

        Public Function GetReadLock() As ISharedCollectionLock
            Return GetReadLock(TimeSpan.FromMilliseconds(-1))
        End Function

        Public Function GetReadLock(ByVal timeOut As TimeSpan) As ISharedCollectionLock
            EnsureNotDisposed()
            Return _lockStrategy.GetReadLock(timeOut)
        End Function

        Public Function GetReadLock(ByVal millisecondTimeout As Integer) As ISharedCollectionLock
            Return GetReadLock(TimeSpan.FromMilliseconds(millisecondTimeout))
        End Function

        Public Function GetWriteLock() As ISharedCollectionLock
            Return GetWriteLock(TimeSpan.FromMilliseconds(-1))
        End Function

        Public Function GetWriteLock(ByVal timeOut As TimeSpan) As ISharedCollectionLock
            EnsureNotDisposed()
            Return _lockStrategy.GetWriteLock(timeOut)
        End Function

        Public Function GetWriteLock(ByVal millisecondTimeout As Integer) As ISharedCollectionLock
            Return GetWriteLock(TimeSpan.FromMilliseconds(millisecondTimeout))
        End Function

        Private Sub EnsureReadAccess()
            If Not (_lockStrategy.ThreadCanRead) Then
                Throw New ReadLockRequiredException()
            End If
        End Sub

        Private Sub EnsureWriteAccess()
            If Not _lockStrategy.ThreadCanWrite Then
                Throw New WriteLockRequiredException()
            End If
        End Sub

        Public Sub Add(ByVal item As T) Implements ICollection(Of T).Add
            EnsureNotDisposed()
            EnsureWriteAccess()
            _list.Add(item)
        End Sub

        Public Sub Clear() Implements ICollection(Of T).Clear
            EnsureNotDisposed()
            EnsureWriteAccess()
            _list.Clear()
        End Sub

        Public Function Contains(ByVal item As T) As Boolean Implements ICollection(Of T).Contains
            EnsureNotDisposed()
            EnsureReadAccess()
            Return _list.Contains(item)
        End Function

        Public Sub CopyTo(ByVal array() As T, ByVal arrayIndex As Integer) Implements ICollection(Of T).CopyTo
            EnsureNotDisposed()
            EnsureReadAccess()
            _list.CopyTo(array, arrayIndex)
        End Sub

        Public ReadOnly Property Count As Integer Implements ICollection(Of T).Count
            Get
                EnsureNotDisposed()
                EnsureReadAccess()
                Return _list.Count
            End Get
        End Property

        Public ReadOnly Property IsReadOnly As Boolean Implements ICollection(Of T).IsReadOnly
            Get
                EnsureNotDisposed()
                EnsureReadAccess()
                Return CType(_list, ICollection(Of T)).IsReadOnly
            End Get
        End Property

        Public Function Remove(ByVal item As T) As Boolean Implements ICollection(Of T).Remove
            EnsureNotDisposed()
            EnsureWriteAccess()
            Return _list.Remove(item)
        End Function

        Public Function GetEnumerator() As IEnumerator(Of T) Implements IEnumerable(Of T).GetEnumerator
            EnsureNotDisposed()
            EnsureReadAccess()
            Return _list.GetEnumerator()
        End Function

        Public Function IndexOf(ByVal item As T) As Integer Implements IList(Of T).IndexOf
            EnsureNotDisposed()
            EnsureReadAccess()
            Return _list.IndexOf(item)
        End Function

        Public Sub Insert(ByVal index As Integer, ByVal item As T) Implements IList(Of T).Insert
            EnsureNotDisposed()
            EnsureWriteAccess()
            _list.Insert(index, item)
        End Sub

        Default Public Property Item(ByVal index As Integer) As T Implements IList(Of T).Item
            Get
                EnsureNotDisposed()
                EnsureReadAccess()
                Return _list(index)
            End Get
            Set(ByVal value As T)
                EnsureNotDisposed()
                EnsureWriteAccess()
                _list(index) = value
            End Set
        End Property

        Public Sub RemoveAt(ByVal index As Integer) Implements IList(Of T).RemoveAt
            EnsureNotDisposed()
            EnsureWriteAccess()
            _list.RemoveAt(index)
        End Sub

        Public Function GetEnumerator1() As IEnumerator Implements IEnumerable.GetEnumerator
            Return GetEnumerator()
        End Function

#Region "IDisposable Support"

        Private _isDisposed As Boolean
        ' To detect redundant calls

        Public Sub EnsureNotDisposed()
            If _isDisposed Then
                Throw New ObjectDisposedException("SharedList")
            End If
        End Sub

        ' IDisposable
        Protected Overridable Sub Dispose(ByVal disposing As Boolean)
            If Not Me._isDisposed Then
                If disposing Then
                    ' dispose managed state (managed objects).
                End If

                _lockStrategy.Dispose()
                _lockStrategy = Nothing
            End If
            Me._isDisposed = True
        End Sub

        Protected Overrides Sub Finalize()
            Dispose(False)
            MyBase.Finalize()
        End Sub

        Public Sub Dispose() Implements IDisposable.Dispose
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub

#End Region
    End Class

End Namespace

