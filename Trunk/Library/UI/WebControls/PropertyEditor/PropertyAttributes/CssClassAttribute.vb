'
' DotNetNuke® - http://www.dotnetnuke.com
' Copyright (c) 2002-2006
' by Perpetual Motion Interactive Systems Inc. ( http://www.perpetualmotion.ca )
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
    Public NotInheritable Class CssClassAttribute
        Inherits System.Attribute

#Region "Constructors"

        ''' <summary>
        ''' Initializes a new instance of the CssClassAttribute class.
        ''' </summary>
        ''' <param name="cssClass">The css class to apply to the associated property</param>
        Public Sub New(ByVal cssClass As String)
            _cssClass = cssClass
        End Sub

#End Region

#Region "Fields"

        Private _cssClass As String

#End Region

#Region "Properties"

        Public ReadOnly Property CssClass() As String
            Get
                Return _cssClass
            End Get
        End Property

#End Region

    End Class
End Namespace
