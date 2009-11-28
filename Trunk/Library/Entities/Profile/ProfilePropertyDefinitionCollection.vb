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

Imports DotNetNuke.UI

Namespace DotNetNuke.Entities.Profile

    ''' -----------------------------------------------------------------------------
    ''' Project:    DotNetNuke
    ''' Namespace:  DotNetNuke.Entities.Profile
    ''' Class:      ProfilePropertyDefinitionCollection
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The ProfilePropertyDefinitionCollection class provides Business Layer methods for 
    ''' a collection of property Definitions
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    '''     [cnurse]	01/31/2006	created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    <Serializable()> Public Class ProfilePropertyDefinitionCollection
        Inherits CollectionBase

#Region "Constructors"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Constructs a new default collection
        ''' </summary>
        ''' <history>
        '''     [cnurse]	01/31/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub New()
            MyBase.New()
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Constructs a new Collection from an ArrayList of ProfilePropertyDefinition objects
        ''' </summary>
        ''' <param name="definitionsList">An ArrayList of ProfilePropertyDefinition objects</param>
        ''' <history>
        '''     [cnurse]	01/31/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub New(ByVal definitionsList As ArrayList)
            AddRange(definitionsList)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Constructs a new Collection from a ProfilePropertyDefinitionCollection
        ''' </summary>
        ''' <param name="collection">A ProfilePropertyDefinitionCollection</param>
        ''' <history>
        '''     [cnurse]	01/31/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub New(ByVal collection As ProfilePropertyDefinitionCollection)
            AddRange(collection)
        End Sub

#End Region

#Region "Public Properties"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets an item in the collection.
        ''' </summary>
        ''' <remarks>This overload returns the item by its index. </remarks>
        ''' <param name="index">The index to get</param>
        ''' <returns>A ProfilePropertyDefinition object</returns>
        ''' <history>
        '''     [cnurse]	01/31/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Default Public Property Item(ByVal index As Integer) As ProfilePropertyDefinition
            Get
                Return CType(List(index), ProfilePropertyDefinition)
            End Get
            Set(ByVal Value As ProfilePropertyDefinition)
                List(index) = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets an item in the collection.
        ''' </summary>
        ''' <remarks>This overload returns the item by its name</remarks>
        ''' <param name="name">The name of the Property to get</param>
        ''' <returns>A ProfilePropertyDefinition object</returns>
        ''' <history>
        '''     [cnurse]	01/31/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Default Public ReadOnly Property Item(ByVal name As String) As ProfilePropertyDefinition
            Get
                Return GetByName(name)
            End Get
        End Property

#End Region

#Region "Public Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Adds a property Definition to the collectio.
        ''' </summary>
        ''' <param name="value">A ProfilePropertyDefinition object</param>
        ''' <returns>The index of the property Definition in the collection</returns>
        ''' <history>
        '''     [cnurse]	01/31/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function Add(ByVal value As ProfilePropertyDefinition) As Integer
            Return List.Add(value)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Add an ArrayList of ProfilePropertyDefinition objects
        ''' </summary>
        ''' <param name="definitionsList">An ArrayList of ProfilePropertyDefinition objects</param>
        ''' <history>
        '''     [cnurse]	01/31/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub AddRange(ByVal definitionsList As ArrayList)
            For Each objProfilePropertyDefinition As ProfilePropertyDefinition In definitionsList
                Add(objProfilePropertyDefinition)
            Next
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Add an existing ProfilePropertyDefinitionCollection
        ''' </summary>
        ''' <param name="collection">A ProfilePropertyDefinitionCollection</param>
        ''' <history>
        '''     [cnurse]	01/31/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub AddRange(ByVal collection As ProfilePropertyDefinitionCollection)
            For Each objProfilePropertyDefinition As ProfilePropertyDefinition In collection
                Add(objProfilePropertyDefinition)
            Next
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Determines whether the collection contains a property definition
        ''' </summary>
        ''' <param name="value">A ProfilePropertyDefinition object</param>
        ''' <returns>A Boolean True/False</returns>
        ''' <history>
        '''     [cnurse]	01/31/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function Contains(ByVal value As ProfilePropertyDefinition) As Boolean
            Return List.Contains(value)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets a sub-collection of items in the collection by category.
        ''' </summary>
        ''' <param name="category">The category to get</param>
        ''' <returns>A ProfilePropertyDefinitionCollection object</returns>
        ''' <history>
        '''     [cnurse]	01/31/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function GetByCategory(ByVal category As String) As ProfilePropertyDefinitionCollection

            Dim collection As New ProfilePropertyDefinitionCollection
            For Each profileProperty As ProfilePropertyDefinition In InnerList
                If profileProperty.PropertyCategory = category Then
                    'Found Profile property that satisfies category
                    collection.Add(profileProperty)
                End If
            Next
            Return collection

        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets an item in the collection by Id.
        ''' </summary>
        ''' <param name="id">The id of the Property to get</param>
        ''' <returns>A ProfilePropertyDefinition object</returns>
        ''' <history>
        '''     [cnurse]	01/31/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function GetById(ByVal id As Integer) As ProfilePropertyDefinition

            Dim profileItem As ProfilePropertyDefinition = Nothing
            For Each profileProperty As ProfilePropertyDefinition In InnerList
                If profileProperty.PropertyDefinitionId = id Then
                    'Found Profile property
                    profileItem = profileProperty
                End If
            Next
            Return profileItem

        End Function


        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets an item in the collection by name.
        ''' </summary>
        ''' <param name="name">The name of the Property to get</param>
        ''' <returns>A ProfilePropertyDefinition object</returns>
        ''' <history>
        '''     [cnurse]	01/31/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function GetByName(ByVal name As String) As ProfilePropertyDefinition

            Dim profileItem As ProfilePropertyDefinition = Nothing
            For Each profileProperty As ProfilePropertyDefinition In InnerList
                If profileProperty.PropertyName = name Then
                    'Found Profile property
                    profileItem = profileProperty
                End If
            Next
            Return profileItem

        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the index of a property Definition
        ''' </summary>
        ''' <param name="value">A ProfilePropertyDefinition object</param>
        ''' <returns>The index of the property Definition in the collection</returns>
        ''' <history>
        '''     [cnurse]	01/31/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function IndexOf(ByVal value As ProfilePropertyDefinition) As Integer
            Return List.IndexOf(value)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Inserts a property Definition into the collectio.
        ''' </summary>
        ''' <param name="value">A ProfilePropertyDefinition object</param>
        ''' <param name="index">The index to insert the item at</param>
        ''' <history>
        '''     [cnurse]	01/31/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub Insert(ByVal index As Integer, ByVal value As ProfilePropertyDefinition)
            List.Insert(index, value)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Removes a property definition from the collection
        ''' </summary>
        ''' <param name="value">The ProfilePropertyDefinition object to remove</param>
        ''' <history>
        '''     [cnurse]	01/31/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub Remove(ByVal value As ProfilePropertyDefinition)
            List.Remove(value)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Sorts the collection using the ProfilePropertyDefinitionComparer (ie by ViewOrder)
        ''' </summary>
        ''' <history>
        '''     [cnurse]	01/31/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub Sort()
            InnerList.Sort(New ProfilePropertyDefinitionComparer)
        End Sub

#End Region

    End Class
End Namespace

