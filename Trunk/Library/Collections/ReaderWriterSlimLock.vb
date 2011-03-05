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

Namespace DotNetNuke.Collections.Internal

    Friend Class ReaderWriterSlimLock
        Implements ISharedCollectionLock
        Private _lock As ReaderWriterLockSlim
        Private _disposed As Boolean

        Public Sub New(ByVal lock As ReaderWriterLockSlim)
            _lock = lock
        End Sub

        Private Sub EnsureNotDisposed()
            If _disposed Then
                Throw New ObjectDisposedException("ReaderWriterSlimLock")
            End If
        End Sub

        Public Sub Dispose() Implements IDisposable.Dispose
            Dispose(True)

            GC.SuppressFinalize(Me)
        End Sub

        Protected Overridable Sub Dispose(ByVal disposing As Boolean)
            If Not _disposed Then
                If disposing Then
                    'free managed resources here
                End If

                'free unmanaged resrources here
                If _lock.IsReadLockHeld Then
                    _lock.ExitReadLock()
                ElseIf _lock.IsWriteLockHeld Then
                    _lock.ExitWriteLock()
                ElseIf _lock.IsUpgradeableReadLockHeld Then
                    _lock.ExitUpgradeableReadLock()
                End If

                _lock = Nothing
                _disposed = True
            End If
        End Sub

        Protected Overrides Sub Finalize()
            Dispose(False)
        End Sub
    End Class

End Namespace


