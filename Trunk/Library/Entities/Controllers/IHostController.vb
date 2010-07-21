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


Namespace DotNetNuke.Entities.Controllers
    Public Interface IHostController

        Sub Update(ByVal config As ConfigurationSetting)

        Sub Update(ByVal config As ConfigurationSetting, ByVal clearCache As Boolean)

        Sub Update(ByVal key As String, ByVal value As String, ByVal clearCache As Boolean)

        Sub Update(ByVal key As String, ByVal value As String)

        Function GetSettings() As Dictionary(Of String, ConfigurationSetting)

        Function GetSettingsDictionary() As Dictionary(Of String, String)

        Function GetBoolean(ByVal key As String) As Boolean

        Function GetBoolean(ByVal key As String, ByVal defaultValue As Boolean) As Boolean

        Function GetDouble(ByVal key As String, ByVal defaultValue As Double) As Double

        Function GetDouble(ByVal key As String) As Double

        Function GetInteger(ByVal key As String) As Integer

        Function GetInteger(ByVal key As String, ByVal defaultValue As Integer) As Integer

        Function GetString(ByVal key As String) As String

        Function GetString(ByVal key As String, ByVal defaultValue As String) As String


    End Interface
End Namespace