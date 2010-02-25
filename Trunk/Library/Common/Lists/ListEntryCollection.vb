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

Imports System
Imports System.Configuration
Imports System.Data

Namespace DotNetNuke.Common.Lists
    <Serializable()> Public Class ListEntryInfoCollection
        Inherits CollectionBase

        Private mKeyIndexLookup As Hashtable = New Hashtable

        Public Sub New()
            MyBase.New()
        End Sub 'New

        Public Function GetChildren(ByVal ParentName As String) As ListEntryInfo
            Return CType(Item(ParentName), ListEntryInfo)
        End Function

        Friend Shadows Sub Clear()
            mKeyIndexLookup.Clear()
            MyBase.Clear()
        End Sub

        Public Sub Add(ByVal key As String, ByVal value As ListEntryInfo)
            Dim index As Integer
            ' <tam:note key to be lowercase for appropiated seeking>
            Try
                index = MyBase.List.Add(value)
                mKeyIndexLookup.Add(key.ToLower, index)
            Catch ex As Exception
                'Throw ex
            End Try

        End Sub

        Public Function Item(ByVal index As Integer) As ListEntryInfo
            Try
                Return CType(MyBase.List.Item(index), ListEntryInfo)
            Catch Exc As System.Exception
                Return Nothing
            End Try
        End Function

        Public Function Item(ByVal key As String) As ListEntryInfo
            Dim index As Integer

            Try ' Do validation first
                If mKeyIndexLookup.Item(key.ToLower) Is Nothing Then
                    Return Nothing
                End If
            Catch ex As Exception
                Return Nothing
            End Try

            index = CInt(mKeyIndexLookup.Item(key.ToLower))

            Return CType(MyBase.List.Item(index), ListEntryInfo)
        End Function

    End Class

End Namespace

