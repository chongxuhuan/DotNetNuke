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
Imports System.IO
Imports System.Web.Caching
Imports System.Threading
Imports System.Resources
Imports System.Collections.Specialized
Imports System.Diagnostics
Imports System.Xml
Imports System.Xml.Serialization
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Entities

Namespace DotNetNuke.Services.Localization

    ''' <summary>
    ''' <para>The Locale class is a custom business object that represents a locale, which is the language and country combination.</para>
    ''' </summary>
    <Serializable()> Public Class Locale
        Inherits BaseEntityInfo
        Implements IHydratable

#Region "Private Members"

        Private _Code As String
        Private _Fallback As String
        Private _IsPublished As Boolean = Null.NullBoolean
        Private _LanguageId As Integer = Null.NullInteger
        Private _PortalId As Integer = Null.NullInteger
        Private _Text As String

#End Region

#Region "Public Properties"

        Public Property Code() As String
            Get
                Return _Code
            End Get
            Set(ByVal Value As String)
                _Code = Value
            End Set
        End Property

        Public ReadOnly Property Culture As CultureInfo
            Get
                Return CultureInfo.CreateSpecificCulture(Code)
            End Get
        End Property

        Public ReadOnly Property EnglishName As String
            Get
                Dim _Name As String = Null.NullString
                If Culture IsNot Nothing Then
                    _Name = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(Culture.EnglishName)
                End If
                Return _Name
            End Get
        End Property

        Public Property Fallback() As String
            Get
                Return _Fallback
            End Get
            Set(ByVal Value As String)
                _Fallback = Value
            End Set
        End Property

        Public ReadOnly Property FallBackLocale() As Locale
            Get
                Dim _FallbackLocale As Locale = Nothing
                If Not String.IsNullOrEmpty(Fallback) Then
                    _FallbackLocale = LocaleController.Instance().GetLocale(PortalId, Fallback)
                End If
                Return _FallbackLocale
            End Get
        End Property

        Public Property IsPublished() As Boolean
            Get
                Return _IsPublished
            End Get
            Set(ByVal value As Boolean)
                _IsPublished = value
            End Set
        End Property

        Public Property LanguageId() As Integer
            Get
                Return _LanguageId
            End Get
            Set(ByVal value As Integer)
                _LanguageId = value
            End Set
        End Property

        Public ReadOnly Property NativeName() As String
            Get
                Dim _Name As String = Null.NullString
                If Culture IsNot Nothing Then
                    _Name = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(Culture.NativeName)
                End If
                Return _Name
            End Get
        End Property

        Public Property PortalId() As Integer
            Get
                Return _PortalId
            End Get
            Set(ByVal value As Integer)
                _PortalId = value
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

#End Region

#Region "IHydratable Implementation"

        Public Sub Fill(ByVal dr As System.Data.IDataReader) Implements IHydratable.Fill
            LanguageId = Null.SetNullInteger(dr("LanguageID"))
            Code = Null.SetNullString(dr("CultureCode"))
            Text = Null.SetNullString(dr("CultureName"))
            Fallback = Null.SetNullString(dr("FallbackCulture"))

            Try
                'These fields may not be populated (for Host level locales)
                IsPublished = Null.SetNullBoolean(dr("IsPublished"))
                PortalId = Null.SetNullInteger(dr("PortalID"))
            Catch ex As IndexOutOfRangeException
            End Try

            'Call the base classes fill method to populate base class proeprties
            MyBase.FillInternal(dr)
        End Sub

        Public Property KeyID() As Integer Implements IHydratable.KeyID
            Get
                Return LanguageId
            End Get
            Set(ByVal value As Integer)
                LanguageId = value
            End Set
        End Property

#End Region

    End Class

End Namespace
