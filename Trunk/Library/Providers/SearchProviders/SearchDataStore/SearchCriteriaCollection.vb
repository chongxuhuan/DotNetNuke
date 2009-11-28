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
Imports System.Collections

Namespace DotNetNuke.Services.Search

    ''' -----------------------------------------------------------------------------
    ''' Namespace:  DotNetNuke.Services.Search
    ''' Project:    DotNetNuke.Search.DataStore
    ''' Class:      SearchCriteria
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Represents a collection of <see cref="SearchCriteria">SearchCriteria</see> objects.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    '''		[cnurse]	11/15/2004	documented
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class SearchCriteriaCollection
        Inherits CollectionBase

#Region "Constructors"

        ''' <summary>
        ''' Initializes a new instance of the <see cref="SearchCriteriaCollection">SearchCriteriaCollection</see> class.
        ''' </summary>
        Public Sub New()
            MyBase.New()
        End Sub

        ''' <summary>
        ''' Initializes a new instance of the <see cref="SearchCriteriaCollection">SearchCriteriaCollection</see> class containing the elements of the specified source collection.
        ''' </summary>
        ''' <param name="value">A <see cref="SearchCriteriaCollection">SearchCriteriaCollection</see> with which to initialize the collection.</param>
        Public Sub New(ByVal value As SearchCriteriaCollection)
            MyBase.New()
            AddRange(value)
        End Sub

        ''' <summary>
        ''' Initializes a new instance of the <see cref="SearchCriteriaCollection">SearchCriteriaCollection</see> class containing the specified array of <see cref="SearchCriteria">SearchCriteria</see> objects.
        ''' </summary>
        ''' <param name="value">An array of <see cref="SearchCriteria">SearchCriteria</see> objects with which to initialize the collection. </param>
        Public Sub New(ByVal value As SearchCriteria())
            MyBase.New()
            AddRange(value)
        End Sub

        ''' <summary>
        ''' Initializes a new instance of the <see cref="SearchCriteriaCollection">SearchCriteriaCollection</see> class containing the elements of the specified source collection.
        ''' </summary>
        ''' <param name="value">A criteria string with which to initialize the collection</param>
        Public Sub New(ByVal value As String)
            MyBase.New()

            ' split search criteria into words
            Dim Words() As String = Split(value, " ")
            Dim word As String
            ' Add all criteria without modifiers
            For Each word In Words
                Dim criterion As New SearchCriteria

                If (Not word.StartsWith("+")) And (Not word.StartsWith("-")) Then
                    criterion.MustInclude = False
                    criterion.MustExclude = False
                    criterion.Criteria = word
                    Add(criterion)
                End If
            Next
            ' Add all mandatory criteria
            For Each word In Words
                Dim criterion As New SearchCriteria

                If word.StartsWith("+") Then
                    criterion.MustInclude = True
                    criterion.MustExclude = False
                    criterion.Criteria = word.Remove(0, 1)
                    Add(criterion)
                End If
            Next
            ' Add all excluded criteria
            For Each word In Words
                Dim criterion As New SearchCriteria

                If word.StartsWith("-") Then
                    criterion.MustInclude = False
                    Criterion.MustExclude = True
                    criterion.Criteria = word.Remove(0, 1)
                    Add(criterion)
                End If
            Next
        End Sub


#End Region

#Region "Properties"

        ''' <summary>
        ''' Gets the <see cref="SearchCriteriaCollection">SearchCriteriaCollection</see> at the specified index in the collection.
        ''' <para>
        ''' In VB.Net, this property is the indexer for the <see cref="SearchCriteriaCollection">SearchCriteriaCollection</see> class.
        ''' </para>
        ''' </summary>
        Default Public Property Item(ByVal index As Integer) As SearchCriteria
            Get
                Return CType(List(index), SearchCriteria)
            End Get
            Set(ByVal Value As SearchCriteria)
                List(index) = Value
            End Set
        End Property

#End Region

#Region "Public Methods"

        ''' <summary>
        ''' Add an element of the specified <see cref="SearchCriteria">SearchCriteria</see> to the end of the collection.
        ''' </summary>
        ''' <param name="value">An object of type <see cref="SearchCriteria">SearchCriteria</see> to add to the collection.</param>
        Public Function Add(ByVal value As SearchCriteria) As Integer
            Return List.Add(value)
        End Function    'Add

        ''' <summary>
        ''' Gets the index in the collection of the specified <see cref="SearchCriteriaCollection">SearchCriteriaCollection</see>, if it exists in the collection.
        ''' </summary>
        ''' <param name="value">The <see cref="SearchCriteriaCollection">SearchCriteriaCollection</see> to locate in the collection.</param>
        ''' <returns>The index in the collection of the specified object, if found; otherwise, -1.</returns>
        Public Function IndexOf(ByVal value As SearchCriteria) As Integer
            Return List.IndexOf(value)
        End Function    'IndexOf

        ''' <summary>
        ''' Add an element of the specified <see cref="SearchCriteria">SearchCriteria</see> to the collection at the designated index.
        ''' </summary>
        ''' <param name="index">An <see cref="system.int32">Integer</see> to indicate the location to add the object to the collection.</param>
        ''' <param name="value">An object of type <see cref="SearchCriteria">SearchCriteria</see> to add to the collection.</param>
        Public Sub Insert(ByVal index As Integer, ByVal value As SearchCriteria)
            List.Insert(index, value)
        End Sub    'Insert

        ''' <summary>
        ''' Remove the specified object of type <see cref="SearchCriteria">SearchCriteria</see> from the collection.
        ''' </summary>
        ''' <param name="value">An object of type <see cref="SearchCriteria">SearchCriteria</see> to remove to the collection.</param>
        Public Sub Remove(ByVal value As SearchCriteria)
            List.Remove(value)
        End Sub    'Remove

        ''' <summary>
        ''' Gets a value indicating whether the collection contains the specified <see cref="SearchCriteriaCollection">SearchCriteriaCollection</see>.
        ''' </summary>
        ''' <param name="value">The <see cref="SearchCriteriaCollection">SearchCriteriaCollection</see> to search for in the collection.</param>
        ''' <returns><b>true</b> if the collection contains the specified object; otherwise, <b>false</b>.</returns>
        Public Function Contains(ByVal value As SearchCriteria) As Boolean
            ' If value is not of type SearchCriteria, this will return false.
            Return List.Contains(value)
        End Function    'Contains

        ''' <summary>
        ''' Copies the elements of the specified <see cref="SearchCriteria">SearchCriteria</see> array to the end of the collection.
        ''' </summary>
        ''' <param name="value">An array of type <see cref="SearchCriteria">SearchCriteria</see> containing the objects to add to the collection.</param>
        Public Sub AddRange(ByVal value As SearchCriteria())
            For i As Integer = 0 To value.Length - 1
                Add(value(i))
            Next i
        End Sub

        ''' <summary>
        ''' Adds the contents of another <see cref="SearchCriteriaCollection">SearchCriteriaCollection</see> to the end of the collection.
        ''' </summary>
        ''' <param name="value">A <see cref="SearchCriteriaCollection">SearchCriteriaCollection</see> containing the objects to add to the collection. </param>
        Public Sub AddRange(ByVal value As SearchCriteriaCollection)
            For i As Integer = 0 To value.Count - 1
                Add(CType(value.List(i), SearchCriteria))
            Next i
        End Sub

        ''' <summary>
        ''' Copies the collection objects to a one-dimensional <see cref="T:System.Array">Array</see> instance beginning at the specified index.
        ''' </summary>
        ''' <param name="array">The one-dimensional <see cref="T:System.Array">Array</see> that is the destination of the values copied from the collection.</param>
        ''' <param name="index">The index of the array at which to begin inserting.</param>
        Public Sub CopyTo(ByVal array As SearchCriteria(), ByVal index As Integer)
            List.CopyTo(array, index)
        End Sub

        ''' <summary>
        ''' Creates a one-dimensional <see cref="T:System.Array">Array</see> instance containing the collection items.
        ''' </summary>
        ''' <returns>Array of type SearchCriteria</returns>
        Public Function ToArray() As SearchCriteria()
            Dim arr As SearchCriteria()
            ReDim Preserve arr(Count - 1)
            CopyTo(arr, 0)

            Return arr
        End Function

#End Region

    End Class

End Namespace

