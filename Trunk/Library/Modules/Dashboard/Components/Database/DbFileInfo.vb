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


Imports System

Namespace DotNetNuke.Modules.Dashboard.Components.Database
    Public Class DbFileInfo
        Private _FileType As String
        Public Property FileType() As String
            Get
                Return _FileType
            End Get
            Set(ByVal value As String)
                _FileType = value
            End Set
        End Property
        Private _Name As String
        Public Property Name() As String
            Get
                Return _Name
            End Get
            Set(ByVal value As String)
                _Name = value
            End Set
        End Property
        Private _Size As Long
        Public Property Size() As Long
            Get
                Return _Size
            End Get
            Set(ByVal value As Long)
                _Size = value
            End Set
        End Property
        Public ReadOnly Property Megabytes() As Decimal
            Get
                Return CType((Size / 1024), Decimal)
            End Get
        End Property

        Private _FileName As String
        Public Property FileName() As String
            Get
                Return _FileName
            End Get
            Set(ByVal value As String)
                _FileName = value
            End Set
        End Property
        Public ReadOnly Property ShortFileName() As String
            Get
                Dim sname As String = String.Format("{0}...{1}", FileName.Substring(0, FileName.IndexOf("\"c) + 1), FileName.Substring(FileName.LastIndexOf("\"c, FileName.LastIndexOf("\"c) - 1)))
                Return sname
            End Get
        End Property
    End Class

End Namespace