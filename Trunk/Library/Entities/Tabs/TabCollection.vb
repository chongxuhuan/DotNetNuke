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

Imports System.Collections.Generic
Imports System.Runtime.Serialization

Namespace DotNetNuke.Entities.Tabs

    ''' <summary>
    ''' Represents the collection of Tabs for a portal
    ''' </summary>
    ''' <remarks></remarks>
    <Serializable()> _
    Public Class TabCollection
        Inherits Dictionary(Of Integer, TabInfo)

#Region "Private Members"

        'This is used to return a sorted List
        Private list As List(Of TabInfo)

        'This is used to provide a collection of children
        Private children As Dictionary(Of Integer, List(Of TabInfo))

#End Region

#Region "Constructors"

        Public Sub New()
            list = New List(Of TabInfo)
            children = New Dictionary(Of Integer, List(Of TabInfo))
        End Sub

        'Required for Serialization
        Public Sub New(ByVal info As SerializationInfo, ByVal context As StreamingContext)
            MyBase.New(info, context)
        End Sub

        Public Sub New(ByVal tabs As List(Of TabInfo))
            Me.New()
            AddRange(tabs)
        End Sub

#End Region

        Private Function AddToChildren(ByVal tab As TabInfo) As Integer
            Dim childList As List(Of TabInfo) = Nothing
            If Not children.TryGetValue(tab.ParentId, childList) Then
                childList = New List(Of TabInfo)
                children.Add(tab.ParentId, childList)
            End If

            'Add tab to end of child list as children are returned in order
            childList.Add(tab)

            Return childList.Count
        End Function

#Region "Public Methods"

        Public Overloads Sub Add(ByVal tab As TabInfo)
            'Call base class to add to base Dictionary
            Add(tab.TabID, tab)

            If tab.ParentId = Null.NullInteger Then
                'Add tab to Children collection
                AddToChildren(tab)

                'Add to end of List as all zero-level tabs are returned in order first
                list.Add(tab)
            Else
                'Find Parent in list
                For index As Integer = 0 To list.Count() - 1
                    Dim parentTab As TabInfo = list(index)
                    If parentTab.TabID = tab.ParentId Then
                        Dim childCount As Integer = AddToChildren(tab)

                        'Insert tab in master List
                        list.Insert(index + childCount, tab)
                    End If
                Next
            End If
        End Sub

        Public Sub AddRange(ByVal tabs As List(Of TabInfo))
            For Each tab As TabInfo In tabs
                Add(tab)
            Next
        End Sub

        Public Function AsList() As List(Of TabInfo)
            Return list
        End Function

        Public Function DescendentsOf(ByVal tabId As Integer) As List(Of TabInfo)
            Dim descendantTabs As New List(Of TabInfo)
            For index As Integer = 0 To list.Count - 1
                Dim parentTab As TabInfo = list(index)
                If parentTab.TabID = tabId Then
                    'Found Parent - so add descendents
                    For descendantIndex As Integer = index + 1 To list.Count - 1
                        Dim descendantTab As TabInfo = list(descendantIndex)
                        If descendantTab.Level > parentTab.Level Then
                            'Descendant so add to collection
                            descendantTabs.Add(descendantTab)
                        Else
                            Exit For
                        End If
                    Next
                    Exit For
                End If
            Next
            Return descendantTabs
        End Function

        Public Function ToArrayList() As ArrayList
            Dim tabs As New ArrayList()
            For Each tab As TabInfo In list
                tabs.Add(tab)
            Next
            Return tabs
        End Function

        Public Function WithParentId(ByVal parentId As Integer) As List(Of TabInfo)
            Dim tabs As List(Of TabInfo) = Nothing
            If Not children.TryGetValue(parentId, tabs) Then
                tabs = New List(Of TabInfo)
            End If
            Return tabs
        End Function


#End Region
    End Class

End Namespace

