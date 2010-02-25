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


Namespace DotNetNuke.UI.WebControls
    <AttributeUsage(AttributeTargets.Property)> _
    Public NotInheritable Class ControlStyleAttribute
        Inherits System.Attribute

#Region "Constructors"

        ''' <summary>
        ''' Initializes a new instance of the StyleAttribute class.
        ''' </summary>
        ''' <param name="cssClass">The css class to apply to the associated property</param>
        Public Sub New(ByVal cssClass As String)
            _CssClass = cssClass
        End Sub

        ''' <summary>
        ''' Initializes a new instance of the StyleAttribute class.
        ''' </summary>
        ''' <param name="cssClass">The css class to apply to the associated property</param>
        Public Sub New(ByVal cssClass As String, ByVal width As String)
            _CssClass = cssClass
            _Width = Unit.Parse(width)
        End Sub

        ''' <summary>
        ''' Initializes a new instance of the StyleAttribute class.
        ''' </summary>
        ''' <param name="cssClass">The css class to apply to the associated property</param>
        Public Sub New(ByVal cssClass As String, ByVal width As String, ByVal height As String)
            _CssClass = cssClass
            _Height = Unit.Parse(height)
            _Width = Unit.Parse(width)
        End Sub

#End Region

#Region "Fields"

        Private _CssClass As String
        Private _Height As Unit
        Private _Width As Unit

#End Region

#Region "Properties"

        Public ReadOnly Property CssClass() As String
            Get
                Return _CssClass
            End Get
        End Property

        Public ReadOnly Property Height() As Unit
            Get
                Return _Height
            End Get
        End Property

        Public ReadOnly Property Width() As Unit
            Get
                Return _Width
            End Get
        End Property

#End Region

    End Class

End Namespace
