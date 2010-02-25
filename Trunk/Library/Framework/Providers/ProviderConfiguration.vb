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
Imports System.IO
Imports System.Collections
Imports System.Collections.Specialized
Imports System.Configuration
Imports System.Web
Imports System.Xml

Namespace DotNetNuke.Framework.Providers

	Public Class ProviderConfiguration

		Private _Providers As New Hashtable
		Private _DefaultProvider As String

		Public Shared Function GetProviderConfiguration(ByVal strProvider As String) As ProviderConfiguration
            Return CType(Config.GetSection("dotnetnuke/" & strProvider), ProviderConfiguration)
        End Function

		Friend Sub LoadValuesFromConfigurationXml(ByVal node As XmlNode)
			Dim attributeCollection As XmlAttributeCollection = node.Attributes

			' Get the default provider
			_DefaultProvider = attributeCollection("defaultProvider").Value

			' Read child nodes
			Dim child As XmlNode
			For Each child In node.ChildNodes
				If child.Name = "providers" Then
					GetProviders(child)
				End If
			Next child
		End Sub

		Friend Sub GetProviders(ByVal node As XmlNode)

			Dim Provider As XmlNode
			For Each Provider In node.ChildNodes

				Select Case Provider.Name
					Case "add"
						Providers.Add(Provider.Attributes("name").Value, New Provider(Provider.Attributes))

					Case "remove"
						Providers.Remove(Provider.Attributes("name").Value)

					Case "clear"
						Providers.Clear()
				End Select
			Next Provider
		End Sub

		Public ReadOnly Property DefaultProvider() As String
			Get
				Return _DefaultProvider
			End Get
		End Property

		Public ReadOnly Property Providers() As Hashtable
			Get
				Return _Providers
			End Get
		End Property

    End Class

End Namespace