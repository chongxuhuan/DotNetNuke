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
Imports System.Data
Imports DotNetNuke

Namespace DotNetNuke.Entities.Portals
    <Serializable()> _
    Public Class PortalAliasCollection
        Inherits DictionaryBase

        Public Sub New()
            MyBase.New()
        End Sub

        ' Gets or sets the value associated with the specified key.
        Default Public Overloads Property Item(ByVal key As [String]) As PortalAliasInfo
            Get
                Return CType(Me.Dictionary(key), PortalAliasInfo)
            End Get
            Set(ByVal Value As PortalAliasInfo)
                Me.Dictionary(key) = Value
            End Set
        End Property

        Public Function Contains(ByVal key As [String]) As Boolean
            Return Dictionary.Contains(key)
        End Function 'Contains

        ' Gets a value indicating if the collection contains keys that are not null.
        Public ReadOnly Property HasKeys() As [Boolean]
            Get
                Return Me.Dictionary.Keys.Count > 0
            End Get
        End Property

        Public ReadOnly Property Keys() As ICollection
            Get
                Return Me.Dictionary.Keys
            End Get
        End Property

        Public ReadOnly Property Values() As ICollection
            Get
                Return Me.Dictionary.Values
            End Get
        End Property


        ' Adds an entry to the collection.
        Public Sub Add(ByVal key As [String], ByVal value As [PortalAliasInfo])
            Me.Dictionary.Add(key, value)
        End Sub    'Add


    End Class

End Namespace
