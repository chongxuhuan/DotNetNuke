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
Imports System.Configuration
Imports System.Data
Imports System.IO
Imports System.Runtime.Serialization.Formatters.Binary
Imports System.Collections

Namespace DotNetNuke.Services.Personalization

    <Serializable()> Public Class PersonalizationInfo

        ' local property declarations
        Private _UserId As Integer
        Private _PortalId As Integer
        Private _IsModified As Boolean
        Private _Profile As Hashtable

        ' initialization
        Public Sub New()
        End Sub

        ' public properties
        Public Property UserId() As Integer
            Get
                Return _UserId
            End Get
            Set(ByVal Value As Integer)
                _UserId = Value
            End Set
        End Property

        Public Property PortalId() As Integer
            Get
                Return _PortalId
            End Get
            Set(ByVal Value As Integer)
                _PortalId = Value
            End Set
        End Property

        Public Property IsModified() As Boolean
            Get
                Return _IsModified
            End Get
            Set(ByVal Value As Boolean)
                _IsModified = Value
            End Set
        End Property

        Public Property Profile() As Hashtable
            Get
                Return _Profile
            End Get
            Set(ByVal Value As Hashtable)
                _Profile = Value
            End Set
        End Property

    End Class

End Namespace