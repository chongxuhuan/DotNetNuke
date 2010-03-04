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

Namespace DotNetNuke.UI.Modules

    Public MustInherit Class ProfileModuleUserControlBase
        Inherits ModuleUserControlBase
        Implements IProfileModule

        Public MustOverride ReadOnly Property DisplayModule() As Boolean Implements IProfileModule.DisplayModule

        Public ReadOnly Property ProfileUserId() As Integer Implements IProfileModule.ProfileUserId
            Get
                Dim UserId As Integer = Null.NullInteger
                If Not String.IsNullOrEmpty(Request.QueryString("userid")) Then
                    UserId = Int32.Parse(Request.QueryString("userid"))
                End If
                Return UserId
            End Get
        End Property

    End Class
End Namespace
