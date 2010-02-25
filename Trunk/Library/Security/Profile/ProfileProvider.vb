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
Imports DotNetNuke
Imports DotNetNuke.Security.Roles

Namespace DotNetNuke.Security.Profile

    Public MustInherit Class ProfileProvider

#Region "Shared/Static Methods"

        ' return the provider
        Public Shared Shadows Function Instance() As ProfileProvider
            Return DotNetNuke.ComponentModel.ComponentFactory.GetComponent(Of ProfileProvider)()
        End Function

#End Region

#Region "Abstract Properties"

        Public MustOverride ReadOnly Property CanEditProviderProperties() As Boolean

#End Region

#Region "Abstract Methods"

        Public MustOverride Sub GetUserProfile(ByRef user As UserInfo)
        Public MustOverride Sub UpdateUserProfile(ByVal user As UserInfo)

#End Region

    End Class

End Namespace

