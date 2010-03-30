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

Imports System.Collections.Generic
Imports System.Xml.Serialization

Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Entities.Content.Common
Imports DotNetNuke.Entities.Content.Taxonomy
Imports System.Collections.Specialized

Namespace DotNetNuke.Entities.Content

    Public Class ContentItem
        Inherits DotNetNuke.Entities.BaseEntityInfo
        Implements IHydratable

#Region "Private Members"

        Private _ContentItemId As Integer = Null.NullInteger
        Private _Content As String
        Private _ContentTypeId As Integer = Null.NullInteger
        Private _TabID As Integer = Null.NullInteger
        Private _ModuleID As Integer = Null.NullInteger
        Private _ContentKey As String
        Private _Indexed As Boolean
        Private _MetaData As NameValueCollection
        Private _Terms As List(Of Term)

#End Region

#Region "Public Properties"

        <XmlIgnore()> Public Property ContentItemId() As Integer
            Get
                Return _ContentItemId
            End Get
            Set(ByVal value As Integer)
                _ContentItemId = value
            End Set
        End Property

        <XmlIgnore()> Public Property Content() As String
            Get
                Return _Content
            End Get
            Set(ByVal value As String)
                _Content = value
            End Set
        End Property

        <XmlIgnore()> Public Property ContentKey() As String
            Get
                Return _ContentKey
            End Get
            Set(ByVal value As String)
                _ContentKey = value
            End Set
        End Property

        <XmlIgnore()> Public Property ContentTypeId() As Integer
            Get
                Return _ContentTypeId
            End Get
            Set(ByVal value As Integer)
                _ContentTypeId = value
            End Set
        End Property

        <XmlIgnore()> Public Property Indexed() As Boolean
            Get
                Return _Indexed
            End Get
            Set(ByVal value As Boolean)
                _Indexed = value
            End Set
        End Property

        <XmlIgnore()> Public ReadOnly Property MetaData() As NameValueCollection
            Get
                If _MetaData Is Nothing Then
                    _MetaData = GetMetaData(ContentItemId)
                End If
                Return _MetaData
            End Get
        End Property

        <XmlElement("moduleID")> Public Property ModuleID() As Integer
            Get
                Return _ModuleID
            End Get
            Set(ByVal value As Integer)
                _ModuleID = value
            End Set
        End Property

        <XmlElement("tabid")> Public Property TabID() As Integer
            Get
                Return _TabID
            End Get
            Set(ByVal value As Integer)
                _TabID = value
            End Set
        End Property

        Public ReadOnly Property Terms() As List(Of Term)
            Get
                If _Terms Is Nothing Then
                    _Terms = GetTerms(ContentItemId)
                End If
                Return _Terms
            End Get
        End Property

#End Region

#Region "Protected Methods"

        Protected Overrides Sub FillInternal(ByVal dr As IDataReader)
            MyBase.FillInternal(dr)

            ContentItemId = Null.SetNullInteger(dr("ContentItemID"))
            Content = Null.SetNullString(dr("Content"))
            ContentTypeId = Null.SetNullInteger(dr("ContentTypeID"))
            TabID = Null.SetNullInteger(dr("TabID"))
            ModuleID = Null.SetNullInteger(dr("ModuleID"))
            ContentKey = Null.SetNullString(dr("ContentKey"))
            Indexed = Null.SetNullBoolean(dr("Indexed"))
        End Sub

#End Region

#Region "IHydratable Implementation"

        Public Overridable Sub Fill(ByVal dr As System.Data.IDataReader) Implements IHydratable.Fill
            FillInternal(dr)
        End Sub

        Public Overridable Property KeyID() As Integer Implements IHydratable.KeyID
            Get
                Return ContentItemId
            End Get
            Set(ByVal value As Integer)
                ContentItemId = value
            End Set
        End Property

#End Region

    End Class

End Namespace