'
' DotNetNuke - http://www.dotnetnuke.com
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

Imports System.Xml
Imports System.Xml.Serialization
Imports System.Xml.Schema

Imports DotNetNuke.Common
Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Entities
Imports DotNetNuke.Entities.Content.Common

Namespace DotNetNuke.Entities.Content.Taxonomy

    <Serializable()> Public Class Vocabulary
        Inherits BaseEntityInfo
        Implements IHydratable

#Region "Private Members"

        Private _Description As String
        Private _IsSystem As Boolean
        Private _Name As String
        Private _ScopeId As Integer
        Private _ScopeType As ScopeType
        Private _ScopeTypeId As Integer
        Private _Terms As List(Of Term)
        Private _Type As VocabularyType
        Private _VocabularyId As Integer
        Private _Weight As Integer

#End Region

#Region "Constructors"

        Public Sub New()
            Me.New(Null.NullString, Null.NullString, VocabularyType.Simple)
        End Sub

        Public Sub New(ByVal name As String)
            Me.New(name, Null.NullString, VocabularyType.Simple)
        End Sub

        Public Sub New(ByVal name As String, ByVal description As String)
            Me.New(name, description, VocabularyType.Simple)
        End Sub

        Public Sub New(ByVal type As VocabularyType)
            Me.New(Null.NullString, Null.NullString, type)
        End Sub

        Public Sub New(ByVal name As String, ByVal description As String, ByVal type As VocabularyType)
            _Description = description
            _Name = name
            _Type = type

            _ScopeId = Null.NullInteger
            _ScopeTypeId = Null.NullInteger
            _VocabularyId = Null.NullInteger
            _Weight = 0
        End Sub

#End Region

#Region "Public Properties"

        Public Property Description() As String
            Get
                Return _Description
            End Get
            Set(ByVal value As String)
                _Description = value
            End Set
        End Property

        Public ReadOnly Property IsHeirarchical() As Boolean
            Get
                Return (Type = VocabularyType.Hierarchy)
            End Get
        End Property

        Public Property IsSystem() As Boolean
            Get
                Return _IsSystem
            End Get
            Set(ByVal value As Boolean)
                _IsSystem = value
            End Set
        End Property

        Public Property Name() As String
            Get
                Return _Name
            End Get
            Set(ByVal value As String)
                _Name = value
            End Set
        End Property

        Public Property ScopeId() As Integer
            Get
                Return _ScopeId
            End Get
            Set(ByVal value As Integer)
                _ScopeId = value
            End Set
        End Property

        Public ReadOnly Property ScopeType() As ScopeType
            Get
                If _ScopeType Is Nothing Then
                    _ScopeType = GetScopeType(_ScopeTypeId)
                End If

                Return _ScopeType
            End Get
        End Property

        Public Property ScopeTypeId() As Integer
            Get
                Return _ScopeTypeId
            End Get
            Set(ByVal value As Integer)
                _ScopeTypeId = value
            End Set
        End Property

        Public ReadOnly Property Terms() As List(Of Term)
            Get
                If _Terms Is Nothing Then
                    _Terms = GetTerms(_VocabularyId)
                End If
                Return _Terms
            End Get
        End Property

        Public Property Type() As VocabularyType
            Get
                Return _Type
            End Get
            Set(ByVal value As VocabularyType)
                _Type = value
            End Set
        End Property

        Public Property VocabularyId() As Integer
            Get
                Return _VocabularyId
            End Get
            Set(ByVal value As Integer)
                _VocabularyId = value
            End Set
        End Property

        Public Property Weight() As Integer
            Get
                Return _Weight
            End Get
            Set(ByVal value As Integer)
                _Weight = value
            End Set
        End Property

#End Region

#Region "IHydratable Implementation"

        Public Overridable Sub Fill(ByVal dr As System.Data.IDataReader) Implements IHydratable.Fill
            VocabularyId = Null.SetNullInteger(dr("VocabularyID"))
            Select Case Convert.ToInt16(dr("VocabularyTypeID"))
                Case 1 : Type = VocabularyType.Simple
                Case 2 : Type = VocabularyType.Hierarchy
            End Select
            IsSystem = Null.SetNullBoolean(dr("IsSystem"))
            Name = Null.SetNullString(dr("Name"))
            Description = Null.SetNullString(dr("Description"))
            ScopeId = Null.SetNullInteger(dr("ScopeID"))
            ScopeTypeId = Null.SetNullInteger(dr("ScopeTypeID"))
            Weight = Null.SetNullInteger(dr("Weight"))

            'Fill base class properties
            FillInternal(dr)
        End Sub

        Public Overridable Property KeyID() As Integer Implements IHydratable.KeyID
            Get
                Return VocabularyId
            End Get
            Set(ByVal value As Integer)
                VocabularyId = value
            End Set
        End Property

#End Region

    End Class

End Namespace

