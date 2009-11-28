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
'
Imports System.Xml.Serialization

Namespace DotNetNuke.Services.Log.EventLog

    <Serializable()> Public Class LogInfoArray
        Implements IEnumerable
        Implements IList

        Private arrLogs As New ArrayList

        Public ReadOnly Property Count() As Integer Implements System.Collections.ICollection.Count
            Get
                Return arrLogs.Count
            End Get
        End Property

        Public Function GetItem(ByVal Index As Integer) As LogInfo
            Return CType(arrLogs(Index), LogInfo)
        End Function


        Public Function GetEnumerator() As System.Collections.IEnumerator Implements System.Collections.IEnumerable.GetEnumerator
            Return arrLogs.GetEnumerator
        End Function

        Public Function Add(ByVal objLogInfo As Object) As Integer Implements System.Collections.IList.Add
            arrLogs.Add(objLogInfo)
        End Function

        Public Sub Remove(ByVal objLogInfo As Object) Implements System.Collections.IList.Remove
            arrLogs.Remove(objLogInfo)
        End Sub

        Public Function GetEnumerator(ByVal index As Integer, ByVal count As Integer) As System.Collections.IEnumerator
            Return arrLogs.GetEnumerator(index, count)
        End Function

        Public Sub CopyTo(ByVal array As System.Array, ByVal index As Integer) Implements System.Collections.ICollection.CopyTo
            arrLogs.CopyTo(array, index)
        End Sub

        Public ReadOnly Property IsSynchronized() As Boolean Implements System.Collections.ICollection.IsSynchronized
            Get
                Return arrLogs.IsSynchronized
            End Get
        End Property

        Public ReadOnly Property SyncRoot() As Object Implements System.Collections.ICollection.SyncRoot
            Get
                Return arrLogs.SyncRoot
            End Get
        End Property

        Public Sub Clear() Implements System.Collections.IList.Clear
            arrLogs.Clear()
        End Sub

        Public Function Contains(ByVal value As Object) As Boolean Implements System.Collections.IList.Contains
            If arrLogs.Contains(value) Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function IndexOf(ByVal value As Object) As Integer Implements System.Collections.IList.IndexOf
            Return arrLogs.IndexOf(value)
        End Function

        Public Sub Insert(ByVal index As Integer, ByVal value As Object) Implements System.Collections.IList.Insert
            arrLogs.Insert(index, value)
        End Sub

        Public ReadOnly Property IsFixedSize() As Boolean Implements System.Collections.IList.IsFixedSize
            Get
                Return arrLogs.IsFixedSize
            End Get
        End Property

        Public ReadOnly Property IsReadOnly() As Boolean Implements System.Collections.IList.IsReadOnly
            Get
                Return arrLogs.IsReadOnly
            End Get
        End Property

        Default Public Property Item(ByVal index As Integer) As Object Implements System.Collections.IList.Item
            Get
                Return arrLogs.Item(index)
            End Get
            Set(ByVal Value As Object)
                arrLogs.Item(index) = Value
            End Set
        End Property

        Public Sub RemoveAt(ByVal index As Integer) Implements System.Collections.IList.RemoveAt
            arrLogs.RemoveAt(index)
        End Sub
    End Class

End Namespace




