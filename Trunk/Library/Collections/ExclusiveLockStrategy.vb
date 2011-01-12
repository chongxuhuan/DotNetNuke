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

Imports System.Threading

Namespace DotNetNuke.Collections

    Public Class ExclusiveLockStrategy
        Implements ILockStrategy

        Private _lock As New Object
        Private _lockedThread As Thread = Nothing
        Private _isDisposed As Boolean = False

        Public Function GetReadLock() As ISharedCollectionLock Implements ILockStrategy.GetReadLock
            Return GetLock(TimeSpan.FromMilliseconds(-1))
        End Function

        Public Function GetReadLock(ByVal timeout As TimeSpan) As ISharedCollectionLock Implements ILockStrategy.GetReadLock
            Return GetLock(timeout)
        End Function

        Public Function GetWriteLock() As ISharedCollectionLock Implements ILockStrategy.GetWriteLock
            Return GetLock(TimeSpan.FromMilliseconds(-1))
        End Function

        Public Function GetWriteLock(ByVal timeout As TimeSpan) As ISharedCollectionLock Implements ILockStrategy.GetWriteLock
            Return GetLock(timeout)
        End Function

        Private Function GetLock(ByVal timeout As TimeSpan) As ISharedCollectionLock
            EnsureNotDisposed()
            If IsThreadLocked() Then
                Throw New LockRecursionException()
            End If

            If Monitor.TryEnter(_lock, timeout) Then
                _lockedThread = Thread.CurrentThread
                Return New MonitorLock(Me)
            Else
                Throw New ApplicationException("ExclusiveLockStrategy.GetLock timed out")
            End If
        End Function

        Public ReadOnly Property ThreadCanRead As Boolean Implements ILockStrategy.ThreadCanRead
            Get
                EnsureNotDisposed()
                Return IsThreadLocked()
            End Get
        End Property

        Public ReadOnly Property ThreadCanWrite As Boolean Implements ILockStrategy.ThreadCanWrite
            Get
                EnsureNotDisposed()
                Return IsThreadLocked()
            End Get
        End Property

        Public ReadOnly Property SupportsConcurrentReads() As Boolean Implements ILockStrategy.SupportsConcurrentReads
            Get
                Return False
            End Get
        End Property

        Private Function IsThreadLocked() As Boolean
            Return Thread.CurrentThread.Equals(_lockedThread)
        End Function

        Public Sub [Exit]()
            EnsureNotDisposed()
            Monitor.Exit(_lock)
            _lockedThread = Nothing
        End Sub

        Private Sub EnsureNotDisposed()
            If _isDisposed Then
                Throw New ObjectDisposedException("ExclusiveLockStrategy")
            End If
        End Sub

        Public Sub Dispose() Implements IDisposable.Dispose
            _isDisposed = True 'todo remove disposable from interface?
        End Sub
    End Class

End Namespace
