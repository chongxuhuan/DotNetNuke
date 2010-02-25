'
' DotNetNuke® - http://www.dotnetnuke.com
' Copyright (c) 2002-2010
' by Perpetual Motion Interactive Systems Inc. ( http://www.perpetualmotion.ca )
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
Imports System.IO
Imports System.Collections
Imports System.Collections.Specialized
Imports System.Configuration
Imports System.Web
Imports System.Xml

Namespace DotNetNuke.Framework.Providers

    Public Class Provider

        Private _ProviderName As String
        Private _ProviderType As String
        Private _ProviderAttributes As New NameValueCollection

        Public Sub New(ByVal Attributes As XmlAttributeCollection)

            ' Set the name of the provider
            '
            _ProviderName = Attributes("name").Value

            ' Set the type of the provider
            '
            _ProviderType = Attributes("type").Value

            ' Store all the attributes in the attributes bucket
            '
            Dim Attribute As XmlAttribute
            For Each Attribute In Attributes

                If Attribute.Name <> "name" And Attribute.Name <> "type" Then
                    _ProviderAttributes.Add(Attribute.Name, Attribute.Value)
                End If
            Next Attribute
        End Sub

        Public ReadOnly Property Name() As String
            Get
                Return _ProviderName
            End Get
        End Property

        Public ReadOnly Property Type() As String
            Get
                Return _ProviderType
            End Get
        End Property

        Public ReadOnly Property Attributes() As NameValueCollection
            Get
                Return _ProviderAttributes
            End Get
        End Property
    End Class

End Namespace