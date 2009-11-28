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

Imports System
Imports System.Configuration
Imports System.Data

Namespace DotNetNuke.Common.Lists

    <Serializable()> Public Class ListInfoCollection
        Inherits CollectionBase

        Private mKeyIndexLookup As Hashtable = New Hashtable

        Public Sub New()
            MyBase.New()
        End Sub 'New

        Public Function GetChildren(ByVal ParentName As String) As ListInfo
            Return CType(Item(ParentName), ListInfo)
        End Function

        Friend Shadows Sub Clear()
            mKeyIndexLookup.Clear()
            MyBase.Clear()
        End Sub

        Public Sub Add(ByVal key As String, ByVal value As Object)
            Dim index As Integer
            ' <tam:note key to be lowercase for appropiated seeking>
            Try
                index = MyBase.List.Add(value)
                mKeyIndexLookup.Add(key.ToLower, index)
            Catch ex As Exception
                'Throw ex
            End Try

        End Sub

        Public Function Item(ByVal index As Integer) As Object
            Try
                Dim obj As Object
                obj = MyBase.List.Item(index)
                Return obj
            Catch Exc As System.Exception
                Return Nothing
            End Try
        End Function

        Public Function Item(ByVal key As String) As Object
            Dim index As Integer
            Dim obj As Object

            Try ' Do validation first
                If mKeyIndexLookup.Item(key.ToLower) Is Nothing Then
                    Return Nothing
                End If
            Catch ex As Exception
                Return Nothing
            End Try

            index = CInt(mKeyIndexLookup.Item(key.ToLower))
            obj = MyBase.List.Item(index)

            Return obj
        End Function

        ' Another method, get Lists on demand
        Public Function Item(ByVal key As String, ByVal Cache As Boolean) As Object
            Dim index As Integer
            Dim obj As Object = Nothing
            Dim itemExists As Boolean = False

            Try ' Do validation first
                If Not mKeyIndexLookup.Item(key.ToLower) Is Nothing Then
                    itemExists = True
                End If
            Catch ex As Exception
            End Try

            ' key will be in format Country.US:Region
            If Not itemExists Then
                Dim ctlLists As New ListController
                Dim listName As String = key.Substring(key.IndexOf(":") + 1)
                Dim parentKey As String = key.Replace(listName, "").TrimEnd(":"c)

                Dim listInfo As ListInfo = ctlLists.GetListInfo(listName, parentKey)
                ' the collection has been cache, so add this entry list into it if specified
                If Cache Then
                    Me.Add(listInfo.Key, listInfo)
                    Return listInfo
                End If
            Else
                index = CInt(mKeyIndexLookup.Item(key.ToLower))
                obj = MyBase.List.Item(index)
            End If

            Return obj
        End Function

        Public Function GetChild(ByVal ParentKey As String) As ArrayList
            Dim child As Object
            Dim childList As New ArrayList
            For Each child In List
                If CType(child, ListInfo).Key.IndexOf(ParentKey.ToLower) > -1 Then
                    childList.Add(child)
                End If
            Next
            Return childList
        End Function

    End Class

End Namespace

