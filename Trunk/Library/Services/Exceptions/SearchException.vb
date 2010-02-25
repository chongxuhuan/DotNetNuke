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

Imports System.Runtime.Serialization
Imports System.Security.Permissions
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Services.Search

Namespace DotNetNuke.Services.Exceptions
    Public Class SearchException
        Inherits BasePortalException

        Private m_SearchItem As SearchItemInfo

        ' default constructor
        Public Sub New()
            MyBase.New()
        End Sub

        Public Sub New(ByVal message As String, ByVal inner As Exception, ByVal searchItem As SearchItemInfo)
            MyBase.New(message, inner)
            m_SearchItem = searchItem
        End Sub

        Public ReadOnly Property SearchItem() As SearchItemInfo
            Get
                Return m_SearchItem
            End Get
        End Property

    End Class

End Namespace