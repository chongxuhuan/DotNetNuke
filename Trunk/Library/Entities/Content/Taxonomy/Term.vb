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

Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Entities
Imports DotNetNuke.Entities.Content.Common

Namespace DotNetNuke.Entities.Content.Taxonomy

    <Serializable()> Public Class Term
        Inherits BaseEntityInfo
        Implements IHydratable

#Region "Private Members"

        Private _ChildTerms As List(Of Term)
        Private _Description As String
        Private _Left As Integer
        Private _Name As String
        Private _ParentTermId As Nullable(Of Integer)
        Private _Right As Integer
        Private _Synonyms As List(Of String)
        Private _TermId As Integer
        Private _Vocabulary As Vocabulary
        Private _VocabularyId As Integer
        Private _Weight As Integer

#End Region

#Region "Constructors"

        Public Sub New()
            Me.New(Null.NullString, Null.NullString, Null.NullInteger)
        End Sub

        Public Sub New(ByVal vocabularyId As Integer)
            Me.New(Null.NullString, Null.NullString, vocabularyId)
        End Sub

        Public Sub New(ByVal name As String)
            Me.New(name, Null.NullString, Null.NullInteger)
        End Sub

        Public Sub New(ByVal name As String, ByVal description As String)
            Me.New(name, description, Null.NullInteger)
        End Sub

        Public Sub New(ByVal name As String, ByVal description As String, ByVal vocabularyId As Integer)
            _Description = description
            _Name = name
            _VocabularyId = vocabularyId

            _ParentTermId = Nothing
            _TermId = Null.NullInteger
            _Left = 0
            _Right = 0
            _Weight = 0
        End Sub

#End Region

#Region "Public Properties"

        <XmlIgnore()> Public ReadOnly Property ChildTerms() As List(Of Term)
            Get
                If _ChildTerms Is Nothing Then
                    _ChildTerms = GetChildTerms(_TermId, _VocabularyId)
                End If
                Return _ChildTerms
            End Get
        End Property

        <XmlIgnore()> Public Property Description() As String
            Get
                Return _Description
            End Get
            Set(ByVal value As String)
                _Description = value
            End Set
        End Property

        <XmlIgnore()> Public ReadOnly Property IsHeirarchical() As Boolean
            Get
                Return (Vocabulary.Type = VocabularyType.Hierarchy)
            End Get
        End Property

        <XmlIgnore()> Public ReadOnly Property Left() As Integer
            Get
                Return _Left
            End Get
        End Property

        <XmlIgnore()> Public Property Name() As String
            Get
                Return _Name
            End Get
            Set(ByVal value As String)
                _Name = value
            End Set
        End Property

        <XmlIgnore()> Public Property ParentTermId() As Nullable(Of Integer)
            Get
                Return _ParentTermId
            End Get
            Set(ByVal value As Nullable(Of Integer))
                _ParentTermId = value
            End Set
        End Property

        <XmlIgnore()> Public ReadOnly Property Right() As Integer
            Get
                Return _Right
            End Get
        End Property

        <XmlIgnore()> Public ReadOnly Property Synonyms() As List(Of String)
            Get
                Return _Synonyms
            End Get
        End Property

        <XmlIgnore()> Public Property TermId() As Integer
            Get
                Return _TermId
            End Get
            Set(ByVal value As Integer)
                _TermId = value
            End Set
        End Property

        <XmlIgnore()> Public ReadOnly Property Vocabulary() As Vocabulary
            Get
                If _Vocabulary Is Nothing AndAlso _VocabularyId > Null.NullInteger Then
                    _Vocabulary = GetVocabulary(_VocabularyId)
                End If
                Return _Vocabulary
            End Get
        End Property

        <XmlIgnore()> Public ReadOnly Property VocabularyId() As Integer
            Get
                Return _VocabularyId
            End Get
        End Property

        <XmlIgnore()> Public Property Weight() As Integer
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
            TermId = Null.SetNullInteger(dr("TermID"))
            Name = Null.SetNullString(dr("Name"))
            Description = Null.SetNullString(dr("Description"))
            Weight = Null.SetNullInteger(dr("Weight"))
            _VocabularyId = Null.SetNullInteger(dr("VocabularyID"))

            If Not IsDBNull(dr("TermLeft")) Then
                _Left = Convert.ToInt32(dr("TermLeft"))
            End If
            If Not IsDBNull(dr("TermRight")) Then
                _Right = Convert.ToInt32(dr("TermRight"))
            End If
            If Not IsDBNull(dr("ParentTermID")) Then
                ParentTermId = Convert.ToInt32(dr("ParentTermID"))
            End If

            'Fill base class properties
            FillInternal(dr)
        End Sub

        Public Property KeyID() As Integer Implements IHydratable.KeyID
            Get
                Return TermId
            End Get
            Set(ByVal value As Integer)
                TermId = value
            End Set
        End Property

#End Region

        Public Function GetTermPath() As String
            Dim path As String = "\\" + Name

            If ParentTermId.HasValue Then
                Dim ctl As ITermController = Util.GetTermController()

                Dim parentTerm As Term = (From t In ctl.GetTermsByVocabulary(VocabularyId) _
                                            Where t.TermId = ParentTermId _
                                            Select t).SingleOrDefault

                If parentTerm IsNot Nothing Then
                    path = parentTerm.GetTermPath() + path
                End If
            End If
            Return path
        End Function


    End Class

End Namespace

