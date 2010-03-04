'
' DotNetNuke - http://www.dotnetnuke.com
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

Imports WebFormsMvp

Namespace DotNetNuke.Web.Mvp

    Public MustInherit Class ModuleView(Of TModel As {Class, New})
        Inherits ModuleView
        Implements IView(Of TModel)

        Private _Model As TModel

        Public Property Model() As TModel Implements IView(Of TModel).Model
            Get
                If (_Model Is Nothing) Then
                    Throw New InvalidOperationException("The Model property is currently null, however it should have been automatically initialized by the presenter. This most likely indicates that no presenter was bound to the control. Check your presenter bindings.")
                End If
                Return _Model
            End Get
            Set(ByVal value As TModel)
                _Model = value
            End Set
        End Property

        Protected Overrides Sub LoadViewState(ByVal savedState As Object)
            'Call the base class to load any View State
            MyBase.LoadViewState(savedState)

            AttributeBasedViewStateSerializer.DeSerialize(Model, ViewState)
        End Sub

        Protected Overrides Function SaveViewState() As Object
            AttributeBasedViewStateSerializer.Serialize(Model, ViewState)

            'Call the base class to save the View State
            Return MyBase.SaveViewState()
        End Function

    End Class

End Namespace

