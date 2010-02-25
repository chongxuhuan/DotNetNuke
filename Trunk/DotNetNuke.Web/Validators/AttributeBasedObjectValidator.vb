Imports System.Reflection

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

Namespace DotNetNuke.Web.Validators

    Public MustInherit Class AttributeBasedObjectValidator(Of TAttribute As Attribute)
        Inherits PropertyBasedObjectValidator

        Protected Overrides Function ValidateProperty(ByVal target As Object, ByVal targetProperty As System.Reflection.PropertyInfo) As ValidationResult
            Return targetProperty.GetCustomAttributes(True) _
                                .OfType(Of TAttribute)() _
                                .Aggregate(ValidationResult.Successful, _
                                              Function(result, attribute) _
                                                 result.CombineWith(If(ValidateAttribute(target, targetProperty, attribute), _
                                                                       ValidationResult.Successful)))
        End Function


        Protected MustOverride Function ValidateAttribute(ByVal target As Object, ByVal targetProperty As PropertyInfo, ByVal attribute As TAttribute) As ValidationResult
    End Class

End Namespace
