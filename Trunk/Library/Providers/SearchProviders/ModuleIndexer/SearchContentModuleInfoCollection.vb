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

namespace DotNetNuke.Services.Search

    ''' -----------------------------------------------------------------------------
    ''' Namespace:  DotNetNuke.Services.Search
    ''' Project:    DotNetNuke.Search.Index
    ''' Class:      SearchContentModuleInfoCollection
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Represents a collection of <see cref="SearchContentModuleInfo">SearchContentModuleInfo</see> objects.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    '''		[cnurse]	11/15/2004	documented
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class SearchContentModuleInfoCollection
        Inherits CollectionBase

#Region "Constructors"

        ''' <summary>
        ''' Initializes a new instance of the <see cref="SearchContentModuleInfoCollection">SearchContentModuleInfoCollection</see> class.
        ''' </summary>
        Public Sub New()
            MyBase.New()
        End Sub

        ''' <summary>
        ''' Initializes a new instance of the <see cref="SearchContentModuleInfoCollection">SearchContentModuleInfoCollection</see> class containing the elements of the specified source collection.
        ''' </summary>
        ''' <param name="value">A <see cref="SearchContentModuleInfoCollection">SearchContentModuleInfoCollection</see> with which to initialize the collection.</param>
        Public Sub New(ByVal value As SearchContentModuleInfoCollection)
            MyBase.New()
            AddRange(value)
        End Sub

        ''' <summary>
        ''' Initializes a new instance of the <see cref="SearchContentModuleInfoCollection">SearchContentModuleInfoCollection</see> class containing the specified array of <see cref="SearchContentModuleInfo">SearchContentModuleInfo</see> objects.
        ''' </summary>
        ''' <param name="value">An array of <see cref="SearchContentModuleInfo">SearchContentModuleInfo</see> objects with which to initialize the collection. </param>
        Public Sub New(ByVal value As SearchContentModuleInfo())
            MyBase.New()
            AddRange(value)
        End Sub

#End Region

#Region "Properties"

        ''' <summary>
        ''' Gets the <see cref="SearchContentModuleInfoCollection">SearchContentModuleInfoCollection</see> at the specified index in the collection.
        ''' <para>
        ''' In VB.Net, this property is the indexer for the <see cref="SearchContentModuleInfoCollection">SearchContentModuleInfoCollection</see> class.
        ''' </para>
        ''' </summary>
        Default Public Property Item(ByVal index As Integer) As SearchContentModuleInfo
            Get
                Return CType(List(index), SearchContentModuleInfo)
            End Get
            Set(ByVal Value As SearchContentModuleInfo)
                List(index) = Value
            End Set
        End Property

#End Region

#Region "Public Methods"

        ''' <summary>
        ''' Add an element of the specified <see cref="SearchContentModuleInfo">SearchContentModuleInfo</see> to the end of the collection.
        ''' </summary>
        ''' <param name="value">An object of type <see cref="SearchContentModuleInfo">SearchContentModuleInfo</see> to add to the collection.</param>
        Public Function Add(ByVal value As SearchContentModuleInfo) As Integer
            Return List.Add(value)
        End Function 'Add

        ''' <summary>
        ''' Gets the index in the collection of the specified <see cref="SearchContentModuleInfoCollection">SearchContentModuleInfoCollection</see>, if it exists in the collection.
        ''' </summary>
        ''' <param name="value">The <see cref="SearchContentModuleInfoCollection">SearchContentModuleInfoCollection</see> to locate in the collection.</param>
        ''' <returns>The index in the collection of the specified object, if found; otherwise, -1.</returns>
        Public Function IndexOf(ByVal value As SearchContentModuleInfo) As Integer
            Return List.IndexOf(value)
        End Function 'IndexOf

        ''' <summary>
        ''' Add an element of the specified <see cref="SearchContentModuleInfo">SearchContentModuleInfo</see> to the collection at the designated index.
        ''' </summary>
        ''' <param name="index">An <see cref="system.int32">Integer</see> to indicate the location to add the object to the collection.</param>
        ''' <param name="value">An object of type <see cref="SearchContentModuleInfo">SearchContentModuleInfo</see> to add to the collection.</param>
        Public Sub Insert(ByVal index As Integer, ByVal value As SearchContentModuleInfo)
            List.Insert(index, value)
        End Sub 'Insert

        ''' <summary>
        ''' Remove the specified object of type <see cref="SearchContentModuleInfo">SearchContentModuleInfo</see> from the collection.
        ''' </summary>
        ''' <param name="value">An object of type <see cref="SearchContentModuleInfo">SearchContentModuleInfo</see> to remove to the collection.</param>
        Public Sub Remove(ByVal value As SearchContentModuleInfo)
            List.Remove(value)
        End Sub 'Remove

        ''' <summary>
        ''' Gets a value indicating whether the collection contains the specified <see cref="SearchContentModuleInfoCollection">SearchContentModuleInfoCollection</see>.
        ''' </summary>
        ''' <param name="value">The <see cref="SearchContentModuleInfoCollection">SearchContentModuleInfoCollection</see> to search for in the collection.</param>
        ''' <returns><b>true</b> if the collection contains the specified object; otherwise, <b>false</b>.</returns>
        Public Function Contains(ByVal value As SearchContentModuleInfo) As Boolean
            ' If value is not of type SearchContentModuleInfo, this will return false.
            Return List.Contains(value)
        End Function 'Contains

        ''' <summary>
        ''' Copies the elements of the specified <see cref="SearchContentModuleInfo">SearchContentModuleInfo</see> array to the end of the collection.
        ''' </summary>
        ''' <param name="value">An array of type <see cref="SearchContentModuleInfo">SearchContentModuleInfo</see> containing the objects to add to the collection.</param>
        Public Sub AddRange(ByVal value As SearchContentModuleInfo())
            For i As Integer = 0 To value.Length - 1
                Add(value(i))
            Next i
        End Sub

        ''' <summary>
        ''' Adds the contents of another <see cref="SearchContentModuleInfoCollection">SearchContentModuleInfoCollection</see> to the end of the collection.
        ''' </summary>
        ''' <param name="value">A <see cref="SearchContentModuleInfoCollection">SearchContentModuleInfoCollection</see> containing the objects to add to the collection. </param>
        Public Sub AddRange(ByVal value As SearchContentModuleInfoCollection)
            For i As Integer = 0 To value.Count - 1
                Add(CType(value.List(i), SearchContentModuleInfo))
            Next i
        End Sub

        ''' <summary>
        ''' Copies the collection objects to a one-dimensional <see cref="T:System.Array">Array</see> instance beginning at the specified index.
        ''' </summary>
        ''' <param name="array">The one-dimensional <see cref="T:System.Array">Array</see> that is the destination of the values copied from the collection.</param>
        ''' <param name="index">The index of the array at which to begin inserting.</param>
        Public Sub CopyTo(ByVal array As SearchContentModuleInfo(), ByVal index As Integer)
            List.CopyTo(array, index)
        End Sub

        ''' <summary>
        ''' Creates a one-dimensional <see cref="T:System.Array">Array</see> instance containing the collection items.
        ''' </summary>
        ''' <returns>Array of type SearchContentModuleInfo</returns>
        Public Function ToArray() As SearchContentModuleInfo()
            Dim arr As SearchContentModuleInfo()
            ReDim Preserve arr(Count - 1)
            CopyTo(arr, 0)

            Return arr
        End Function

#End Region

    End Class
end namespace

