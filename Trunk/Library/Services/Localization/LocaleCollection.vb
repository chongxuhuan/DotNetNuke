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

Imports System.Collections.Specialized

Namespace DotNetNuke.Services.Localization

	''' <summary>
	''' <para>The LocaleCollection class is a collection of Locale objects.  It stores the supported locales.</para>
	''' </summary>
    Public Class LocaleCollection
        Inherits NameObjectCollectionBase

        Private _de As New DictionaryEntry
        Public Sub New()
        End Sub    'New

        Default Public ReadOnly Property Item(ByVal index As Integer) As DictionaryEntry
            Get
                _de.Key = Me.BaseGetKey(index)
                _de.Value = Me.BaseGet(index)
                Return _de
            End Get
        End Property

        ' Gets or sets the value associated with the specified key.
        Default Public Property Item(ByVal key As [String]) As Locale
            Get
                Return CType(Me.BaseGet(key), Locale)
            End Get
            Set(ByVal Value As Locale)
                Me.BaseSet(key, Value)
            End Set
        End Property

        ' Gets a String array that contains all the keys in the collection.
        Public ReadOnly Property AllKeys() As [String]()
            Get
                Return Me.BaseGetAllKeys()
            End Get
        End Property

        ' Gets an Object array that contains all the values in the collection.
        Public ReadOnly Property AllValues() As Array
            Get
                Return Me.BaseGetAllValues()
            End Get
        End Property

        ' Gets a value indicating if the collection contains keys that are not null.
        Public ReadOnly Property HasKeys() As [Boolean]
            Get
                Return Me.BaseHasKeys()
            End Get
        End Property

        ' Adds an entry to the collection.
        Public Sub Add(ByVal key As [String], ByVal value As [Object])
            Me.BaseAdd(key, value)
        End Sub    'Add

        ' Removes an entry with the specified key from the collection.
        Public Overloads Sub Remove(ByVal key As [String])
            Me.BaseRemove(key)
        End Sub    'Remove

    End Class

End Namespace
