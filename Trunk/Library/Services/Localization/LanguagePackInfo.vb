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

    <Serializable()> Public Class LanguagePackInfo
        Inherits BaseEntityInfo
        Implements IHydratable

#Region "Private Members"

        Private _LanguagePackID As Integer = Null.NullInteger
        Private _LanguageID As Integer = Null.NullInteger
        Private _PackageID As Integer = Null.NullInteger
        Private _DependentPackageID As Integer = Null.NullInteger

#End Region

#Region "Public Properties"

        Public Property LanguagePackID() As Integer
            Get
                Return _LanguagePackID
            End Get
            Set(ByVal value As Integer)
                _LanguagePackID = value
            End Set
        End Property

        Public Property LanguageID() As Integer
            Get
                Return _LanguageID
            End Get
            Set(ByVal value As Integer)
                _LanguageID = value
            End Set
        End Property

        Public Property PackageID() As Integer
            Get
                Return _PackageID
            End Get
            Set(ByVal value As Integer)
                _PackageID = value
            End Set
        End Property

        Public Property DependentPackageID() As Integer
            Get
                Return _DependentPackageID
            End Get
            Set(ByVal value As Integer)
                _DependentPackageID = value
            End Set
        End Property

        Public ReadOnly Property PackageType() As LanguagePackType
            Get
                If DependentPackageID = -2 Then
                    Return LanguagePackType.Core
                Else
                    Return LanguagePackType.Extension
                End If
            End Get
        End Property

#End Region

#Region "IHydratable Implementation"

        Public Sub Fill(ByVal dr As System.Data.IDataReader) Implements IHydratable.Fill
            LanguagePackID = Null.SetNullInteger(dr("LanguagePackID"))
            LanguageID = Null.SetNullInteger(dr("LanguageID"))
            PackageID = Null.SetNullInteger(dr("PackageID"))
            DependentPackageID = Null.SetNullInteger(dr("DependentPackageID"))
            'Call the base classes fill method to populate base class proeprties
            MyBase.FillInternal(dr)
        End Sub

        Public Property KeyID() As Integer Implements IHydratable.KeyID
            Get
                Return LanguagePackID
            End Get
            Set(ByVal value As Integer)
                LanguagePackID = value
            End Set
        End Property

#End Region

    End Class

End Namespace
