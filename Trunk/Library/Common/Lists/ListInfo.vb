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
Imports DotNetNuke.Entities

Namespace DotNetNuke.Common.Lists

    <Serializable()> Public Class ListInfo
        Inherits BaseEntityInfo

#Region "Private Members"

        Private mName As String = Null.NullString
        Private mLevel As Integer = 0
        Private mDefinitionID As Integer = Null.NullInteger
        Private mEntryCount As Integer = 0
        Private mParentID As Integer = 0
        Private mParentKey As String = Null.NullString
        Private mParent As String = Null.NullString
        Private mParentList As String = Null.NullString
        Private mIsPopulated As Boolean = Null.NullBoolean
        Private mPortalID As Integer = Null.NullInteger
        Private mEnableSortOrder As Boolean = Null.NullBoolean
        Private mSystemList As Boolean = Null.NullBoolean

#End Region

#Region "Constructors"

        Public Sub New(ByVal Name As String)
            MyBase.New()
            mName = Name
        End Sub 'New

        Public Sub New()
            MyBase.New()
        End Sub 'New

#End Region

#Region "Public Properties"

        Public Property Name() As String
            Get
                Return mName
            End Get
            Set(ByVal Value As String)
                mName = Value
            End Set
        End Property

        Public ReadOnly Property DisplayName() As String
            Get
                Dim _DisplayName As String = Parent
                If Not String.IsNullOrEmpty(_DisplayName) Then
                    _DisplayName += ":"
                End If
                Return _DisplayName + Name
            End Get
        End Property

        Public Property Level() As Integer
            Get
                Return mLevel
            End Get
            Set(ByVal Value As Integer)
                mLevel = Value
            End Set
        End Property

        Public Property DefinitionID() As Integer
            Get
                Return mDefinitionID
            End Get
            Set(ByVal Value As Integer)
                mDefinitionID = Value
            End Set
        End Property

        Public ReadOnly Property Key() As String
            Get
                Dim _Key As String = ParentKey
                If Not String.IsNullOrEmpty(_Key) Then
                    _Key += ":"
                End If
                Return _Key + Name
            End Get
        End Property

        Public Property EntryCount() As Integer
            Get
                Return mEntryCount
            End Get
            Set(ByVal Value As Integer)
                mEntryCount = Value
            End Set
        End Property

        Public Property PortalID() As Integer
            Get
                Return mPortalID
            End Get
            Set(ByVal Value As Integer)
                mPortalID = Value
            End Set
        End Property

        Public Property ParentID() As Integer
            Get
                Return mParentID
            End Get
            Set(ByVal Value As Integer)
                mParentID = Value
            End Set
        End Property

        Public Property ParentKey() As String
            Get
                Return mParentKey
            End Get
            Set(ByVal Value As String)
                mParentKey = Value
            End Set
        End Property

        Public Property Parent() As String
            Get
                Return mParent
            End Get
            Set(ByVal Value As String)
                mParent = Value
            End Set
        End Property

        Public Property ParentList() As String
            Get
                Return mParentList
            End Get
            Set(ByVal Value As String)
                mParentList = Value
            End Set
        End Property

        Public Property IsPopulated() As Boolean
            Get
                Return mIsPopulated
            End Get
            Set(ByVal Value As Boolean)
                mIsPopulated = Value
            End Set
        End Property

        Public Property EnableSortOrder() As Boolean
            Get
                Return mEnableSortOrder
            End Get
            Set(ByVal Value As Boolean)
                mEnableSortOrder = Value
            End Set
        End Property

        Public Property SystemList() As Boolean
            Get
                Return mSystemList
            End Get
            Set(ByVal value As Boolean)
                mSystemList = value
            End Set
        End Property

#End Region

    End Class

End Namespace

