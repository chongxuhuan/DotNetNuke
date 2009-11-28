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
Imports DotNetNuke.Entities.Tabs

Namespace DotNetNuke.Services.Url.FriendlyUrl

    Public MustInherit Class FriendlyUrlProvider

#Region "Shared/Static Methods"

        ' return the provider
        Public Shared Function Instance() As FriendlyUrlProvider
            Return DotNetNuke.ComponentModel.ComponentFactory.GetComponent(Of FriendlyUrlProvider)()
        End Function

#End Region

#Region "Abstract Methods"

        Public MustOverride Function FriendlyUrl(ByVal tab As TabInfo, ByVal path As String) As String
        Public MustOverride Function FriendlyUrl(ByVal tab As TabInfo, ByVal path As String, ByVal pageName As String) As String
        Public MustOverride Function FriendlyUrl(ByVal tab As TabInfo, ByVal path As String, ByVal pageName As String, ByVal settings As PortalSettings) As String
        Public MustOverride Function FriendlyUrl(ByVal tab As TabInfo, ByVal path As String, ByVal pageName As String, ByVal portalAlias As String) As String

#End Region

    End Class

End Namespace
