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


Namespace DotNetNuke.UI.WebControls
    <AttributeUsage(AttributeTargets.Property)> _
    Public NotInheritable Class ListAttribute
        Inherits System.Attribute

#Region "Constructors"

        ''' <summary>
        ''' Initializes a new instance of the ListAttribute class.
        ''' </summary>
        ''' <param name="listName">The name of the List to use for this property</param>
        ''' <param name="parentKey">The key of the parent for this List</param>
        Public Sub New(ByVal listName As String, ByVal parentKey As String, ByVal valueField As ListBoundField, ByVal textField As ListBoundField)
            _ListName = listName
            _ParentKey = parentKey
            _TextField = textField
            _ValueField = valueField
        End Sub

#End Region

#Region "Fields"

        Private _ListName As String
        Private _ParentKey As String
        Private _TextField As ListBoundField
        Private _ValueField As ListBoundField

#End Region

#Region "Properties"

        Public ReadOnly Property ListName() As String
            Get
                Return _ListName
            End Get
        End Property

        Public ReadOnly Property ParentKey() As String
            Get
                Return _ParentKey
            End Get
        End Property

        Public ReadOnly Property TextField() As ListBoundField
            Get
                Return _TextField
            End Get
        End Property

        Public ReadOnly Property ValueField() As ListBoundField
            Get
                Return _ValueField
            End Get
        End Property

#End Region

    End Class

End Namespace
