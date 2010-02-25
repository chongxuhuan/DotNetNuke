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

Imports System.ComponentModel.DataAnnotations

Namespace DotNetNuke.Web.Validators

    Public Class DataAnnotationsObjectValidator
        Inherits AttributeBasedObjectValidator(Of ValidationAttribute)

        Protected Overrides Function ValidateAttribute(ByVal target As Object, ByVal targetProperty As System.Reflection.PropertyInfo, ByVal attribute As System.ComponentModel.DataAnnotations.ValidationAttribute) As ValidationResult
            Return If(Not attribute.IsValid(targetProperty.GetValue(target, New Object() {})), _
                        New ValidationResult(New ValidationError() {CreateError(targetProperty.Name, attribute)}), _
                        ValidationResult.Successful)
        End Function

        Protected Overridable Function CreateError(ByVal propertyName As String, ByVal attribute As ValidationAttribute) As ValidationError

            Return New ValidationError() With { _
                    .ErrorMessage = attribute.FormatErrorMessage(propertyName), _
                    .PropertyName = propertyName, _
                    .Validator = attribute _
                }
        End Function

    End Class

End Namespace
