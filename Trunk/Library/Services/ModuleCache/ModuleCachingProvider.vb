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
Imports System.Collections.Generic

Namespace DotNetNuke.Services.ModuleCache

    Public MustInherit Class ModuleCachingProvider

#Region "Shared/Static Methods"

        Public Shared Function GetProviderList() As Dictionary(Of String, ModuleCachingProvider)
            Return ComponentModel.ComponentFactory.GetComponents(Of ModuleCachingProvider)()
        End Function

        Public Shared Function Instance(ByVal FriendlyName As String) As ModuleCachingProvider
            Return DotNetNuke.ComponentModel.ComponentFactory.GetComponent(Of ModuleCachingProvider)(FriendlyName)
        End Function

        Public Shared Sub RemoveItemFromAllProviders(ByVal tabModuleId As Integer)
            For Each kvp As KeyValuePair(Of String, ModuleCachingProvider) In GetProviderList()
                kvp.Value.Remove(tabModuleId)
            Next
        End Sub

#End Region

        Protected Function ByteArrayToString(ByVal arrInput() As Byte) As String
            Dim i As Integer
            Dim sOutput As New System.Text.StringBuilder(arrInput.Length)
            For i = 0 To arrInput.Length - 1
                sOutput.Append(arrInput(i).ToString("X2"))
            Next
            Return sOutput.ToString()
        End Function


#Region "Abstract Methods"

        Public MustOverride Function GenerateCacheKey(ByVal tabModuleId As Integer, ByVal varyBy As SortedDictionary(Of String, String)) As String
        Public MustOverride Function GetItemCount(ByVal tabModuleId As Integer) As Integer
        Public MustOverride Function GetModule(ByVal tabModuleId As Integer, ByVal cacheKey As String) As Byte()
        Public MustOverride Sub PurgeCache()
        Public MustOverride Sub PurgeExpiredItems()
        Public MustOverride Sub Remove(ByVal tabModuleId As Integer)
        Public MustOverride Overloads Sub SetModule(ByVal tabModuleId As Integer, ByVal cacheKey As String, ByVal duration As TimeSpan, ByVal moduleOutput As Byte())

#End Region

    End Class

End Namespace
