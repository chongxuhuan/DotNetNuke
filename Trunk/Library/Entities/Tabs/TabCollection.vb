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

Imports System.Collections.Generic
Imports System.Linq
Imports System.Runtime.CompilerServices
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

        'This is used to provide a culture based set of tabs
        Private localizedTabs As Dictionary(Of String, List(Of TabInfo))

#End Region

#Region "Constructors"

        Public Sub New()
            list = New List(Of TabInfo)
            children = New Dictionary(Of Integer, List(Of TabInfo))
            localizedTabs = New Dictionary(Of String, List(Of TabInfo))
        End Sub

        'Required for Serialization
        Public Sub New(ByVal info As SerializationInfo, ByVal context As StreamingContext)
            MyBase.New(info, context)
        End Sub

        Public Sub New(ByVal tabs As IEnumerable(Of TabInfo))
            Me.New()
            AddRange(tabs)
        End Sub

#End Region

#Region "Private Methods"

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

        Private Sub AddToLocalizedTabCollection(ByVal tab As TabInfo, ByVal cultureCode As String)
            Dim localizedTabCollection As List(Of TabInfo) = Nothing

            If Not localizedTabs.TryGetValue(cultureCode.ToLowerInvariant(), localizedTabCollection) Then
                localizedTabCollection = New List(Of TabInfo)()
                localizedTabs.Add(cultureCode.ToLowerInvariant(), localizedTabCollection)
            End If

            'Add tab to end of localized tabs
            localizedTabCollection.Add(tab)
        End Sub

        Private Sub AddToLocalizedTabs(ByVal tab As TabInfo)
            If String.IsNullOrEmpty(tab.CultureCode) Then
                'Add to all cultures
                For Each locale As Locale In LocaleController.Instance().GetLocales(tab.PortalID).Values
                    AddToLocalizedTabCollection(tab, locale.Code)
                Next
            Else
                AddToLocalizedTabCollection(tab, tab.CultureCode)
            End If
        End Sub

        Private Function IsLocalizationEnabled() As Boolean
            Dim _portalSettings As PortalSettings = PortalController.GetCurrentPortalSettings()
            If _portalSettings IsNot Nothing Then
                Return PortalController.GetPortalSettingAsBoolean("ContentLocalizationEnabled", _portalSettings.PortalId, False)
            Else
                Return Null.NullBoolean
            End If
        End Function

        Private Function IsLocalizationEnabled(ByVal PortalId As Integer) As Boolean
            Return PortalController.GetPortalSettingAsBoolean("ContentLocalizationEnabled", PortalId, False)
        End Function

#End Region


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

            'Add to localized tabs
            If tab.PortalID = Null.NullInteger OrElse IsLocalizationEnabled(tab.PortalID) Then
                AddToLocalizedTabs(tab)
            End If
        End Sub

        Public Sub AddRange(ByVal tabs As IEnumerable(Of TabInfo))
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

        Public Function WithCulture(ByVal cultureCode As String, ByVal includeNeutral As Boolean) As TabCollection
            Dim tabs As List(Of TabInfo) = Nothing
            Dim collection As TabCollection = Nothing
            If IsLocalizationEnabled() Then
                If String.IsNullOrEmpty(cultureCode) Then
                    'No culture passed in - so return all tabs
                    collection = New TabCollection(list)
                ElseIf Not localizedTabs.TryGetValue(cultureCode.ToLowerInvariant(), tabs) Then
                    collection = New TabCollection(New List(Of TabInfo))
                Else
                    If Not includeNeutral Then
                        'Remove neutral culture tabs
                        collection = New TabCollection(From t As TabInfo In tabs _
                                                        Where t.CultureCode.ToLowerInvariant = cultureCode.ToLowerInvariant() _
                                                        Select t)
                    Else
                        collection = New TabCollection(tabs)
                    End If
                End If
            Else
                collection = New TabCollection(list)
            End If
            Return collection
        End Function

        Public Function WithParentId(ByVal parentId As Integer) As List(Of TabInfo)
            Dim tabs As List(Of TabInfo) = Nothing
            If Not children.TryGetValue(parentId, tabs) Then
                tabs = New List(Of TabInfo)
            End If
            Return tabs
        End Function

        Public Function WithTabId(ByVal tabId As Integer) As TabInfo
            Dim t As TabInfo = Nothing
            If ContainsKey(tabId) Then
                t = Item(tabId)
            End If
            Return t
        End Function

        Public Function WithTabNameAndParentId(ByVal tabName As String, ByVal parentId As Integer) As TabInfo
            Return (From t As TabInfo In list _
                    Where t.TabName.Equals(tabName, StringComparison.InvariantCultureIgnoreCase) _
                    AndAlso t.ParentId = parentId _
                    Select t).SingleOrDefault()
        End Function

        Public Function WithTabName(ByVal tabName As String) As TabInfo
            Return (From t As TabInfo In list _
                   Where t.TabName.Equals(tabName, StringComparison.InvariantCultureIgnoreCase) _
                   Select t).FirstOrDefault()
        End Function

#End Region


    End Class

End Namespace

