' 
'' DotNetNuke® - http://www.dotnetnuke.com 
'' Copyright (c) 2002-2009 
'' by DotNetNuke Corporation 
'' 
'' Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
'' documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
'' the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
'' to permit persons to whom the Software is furnished to do so, subject to the following conditions: 
'' 
'' The above copyright notice and this permission notice shall be included in all copies or substantial portions 
'' of the Software. 
'' 
'' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
'' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
'' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
'' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
'' DEALINGS IN THE SOFTWARE. 
' 


Imports System.Collections.Generic

Namespace DotNetNuke.Modules.Dashboard.Components.Database
    Public Class DbInfo
        Private _ProductVersion As String
        Public Property ProductVersion() As String
            Get
                Return _ProductVersion
            End Get
            Set(ByVal value As String)
                _ProductVersion = value
            End Set
        End Property
        Private _ServicePack As String
        Public Property ServicePack() As String
            Get
                Return _ServicePack
            End Get
            Set(ByVal value As String)
                _ServicePack = value
            End Set
        End Property
        Private _ProductEdition As String
        Public Property ProductEdition() As String
            Get
                Return _ProductEdition
            End Get
            Set(ByVal value As String)
                _ProductEdition = value
            End Set
        End Property
        Private _SoftwarePlatform As String
        Public Property SoftwarePlatform() As String
            Get
                Return _SoftwarePlatform
            End Get
            Set(ByVal value As String)
                _SoftwarePlatform = value
            End Set
        End Property
        Public ReadOnly Property Backups() As List(Of BackupInfo)
            Get
                Return DatabaseController.GetDbBackups()
            End Get
        End Property
        Public ReadOnly Property Files() As List(Of DbFileInfo)
            Get
                Return DatabaseController.GetDbFileInfo()
            End Get
        End Property
    End Class
End Namespace