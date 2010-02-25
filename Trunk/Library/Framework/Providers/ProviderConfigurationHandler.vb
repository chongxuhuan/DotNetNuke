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

    <Obsolete("This class is obsolete.  It is no longer used to load provider configurations, as there are medium trust issues")> _
    Friend Class ProviderConfigurationHandler
        Implements IConfigurationSectionHandler

        Public Overridable Overloads Function Create(ByVal parent As Object, ByVal context As Object, ByVal node As System.Xml.XmlNode) As Object Implements IConfigurationSectionHandler.Create
            Dim objProviderConfiguration As New ProviderConfiguration
            objProviderConfiguration.LoadValuesFromConfigurationXml(node)
            Return objProviderConfiguration
        End Function
    End Class

End Namespace