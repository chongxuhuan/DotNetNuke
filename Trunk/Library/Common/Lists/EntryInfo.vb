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

    <Serializable()> Public Class ListEntryInfo

#Region "Private Members"

        Private _EntryID As Integer
        Private _PortalID As Integer
        Private _Key As String = Null.NullString
        Private _ListName As String = Null.NullString
        Private _DisplayName As String = Null.NullString
        Private _Value As String = Null.NullString
        Private _Text As String = Null.NullString
        Private _Description As String = Null.NullString
        Private _Parent As String = Null.NullString
        Private _ParentID As Integer = 0
        Private _Level As Integer = 0
        Private _SortOrder As Integer = 0
        Private _DefinitionID As Integer = 0
        Private _HasChildren As Boolean = False
        Private _ParentKey As String = Null.NullString
        Private _systemlist As Boolean = False

#End Region

#Region "Constructors"

        Sub New()
        End Sub

#End Region

#Region "Public Properties"

        Public Property EntryID() As Integer
            Get
                Return _EntryID
            End Get
            Set(ByVal Value As Integer)
                _EntryID = Value
            End Set
        End Property

        Public Property PortalID() As Integer
            Get
                Return _PortalID
            End Get
            Set(ByVal Value As Integer)
                _PortalID = Value
            End Set
        End Property

        Public ReadOnly Property Key() As String
            Get
                Dim _Key As String = ParentKey.Replace(":", ".")
                If Not String.IsNullOrEmpty(_Key) Then
                    _Key += "."
                End If
                Return _Key + ListName + ":" + Value
            End Get
        End Property

        Public Property ListName() As String
            Get
                Return _ListName
            End Get
            Set(ByVal Value As String)
                _ListName = Value
            End Set
        End Property

        Public ReadOnly Property DisplayName() As String
            Get
                Return ListName + ":" + Text
            End Get
        End Property

        Public Property Value() As String
            Get
                Return _Value
            End Get
            Set(ByVal Value As String)
                _Value = Value
            End Set
        End Property

        Public Property Text() As String
            Get
                Return _Text
            End Get
            Set(ByVal Value As String)
                _Text = Value
            End Set
        End Property

        Public Property Description() As String
            Get
                Return _Description
            End Get
            Set(ByVal Value As String)
                _Description = Value
            End Set
        End Property

        Public Property ParentID() As Integer
            Get
                Return _ParentID
            End Get
            Set(ByVal Value As Integer)
                _ParentID = Value
            End Set
        End Property

        Public Property Parent() As String
            Get
                Return _Parent
            End Get
            Set(ByVal Value As String)
                _Parent = Value
            End Set
        End Property

        Public Property Level() As Integer
            Get
                Return _Level
            End Get
            Set(ByVal Value As Integer)
                _Level = Value
            End Set
        End Property

        Public Property SortOrder() As Integer
            Get
                Return _SortOrder
            End Get
            Set(ByVal Value As Integer)
                _SortOrder = Value
            End Set
        End Property

        Public Property DefinitionID() As Integer
            Get
                Return _DefinitionID
            End Get
            Set(ByVal Value As Integer)
                _DefinitionID = Value
            End Set
        End Property

        Public Property HasChildren() As Boolean
            Get
                Return _HasChildren
            End Get
            Set(ByVal Value As Boolean)
                _HasChildren = Value
            End Set
        End Property

        Public Property ParentKey() As String
            Get
                Return _ParentKey
            End Get
            Set(ByVal Value As String)
                _ParentKey = Value
            End Set
        End Property


        Public Property SystemList() As Boolean
            Get
                Return _systemlist
            End Get
            Set(ByVal value As Boolean)
                _systemlist = value
            End Set
        End Property


#End Region

    End Class

End Namespace

