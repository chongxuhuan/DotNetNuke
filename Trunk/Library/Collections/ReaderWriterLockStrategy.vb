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

    Friend Class ReaderWriterLockStrategy
        Implements IDisposable
        Implements ILockStrategy

        Private _lock As New ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion)

        Public Function GetReadLock() As ISharedCollectionLock Implements ILockStrategy.GetReadLock
            EnsureNotDisposed()
            _lock.EnterReadLock()
            Return New ReaderWriterSlimLock(_lock)
        End Function

        Public Function GetWriteLock() As ISharedCollectionLock Implements ILockStrategy.GetWriteLock
            EnsureNotDisposed()
            _lock.EnterWriteLock()
            Return New ReaderWriterSlimLock(_lock)
        End Function

        Public ReadOnly Property ThreadCanRead() As Boolean Implements ILockStrategy.ThreadCanRead
            Get
                EnsureNotDisposed()
                Return _lock.IsReadLockHeld OrElse _lock.IsWriteLockHeld 'todo uncomment if upgradelock is used OrElse _lock.IsUpgradeableReadLockHeld
            End Get
        End Property

        Public ReadOnly Property ThreadCanWrite() As Boolean Implements ILockStrategy.ThreadCanWrite
            Get
                EnsureNotDisposed()
                Return _lock.IsWriteLockHeld
            End Get
        End Property

        Public ReadOnly Property SupportsConcurrentReads() As Boolean Implements ILockStrategy.SupportsConcurrentReads
            Get
                Return True
            End Get
        End Property

#Region "IDisposable Support"

        Private Sub EnsureNotDisposed()
            If _isDisposed Then
                Throw New ObjectDisposedException("ReaderWriterLockStrategy")
            End If
        End Sub

        Private _isDisposed As Boolean

        ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(ByVal disposing As Boolean)
            If Not Me._isDisposed Then
                If disposing Then
                    'dispose managed state (managed objects).
                End If

                _lock.Dispose()
                _lock = Nothing
            End If
            Me._isDisposed = True
        End Sub

        Protected Overrides Sub Finalize()
            ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
            Dispose(False)
            MyBase.Finalize()
        End Sub

        ' This code added by Visual Basic to correctly implement the disposable pattern.
        Public Sub Dispose() Implements IDisposable.Dispose
            ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub
#End Region
    End Class

End Namespace

