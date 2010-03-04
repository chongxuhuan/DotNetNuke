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

Imports System.Web.UI
Imports System.Reflection

Namespace DotNetNuke.Web.Mvp

    Public Class AttributeBasedViewStateSerializer
        Private Const MemberBindingFlags As BindingFlags = BindingFlags.Instance Or BindingFlags.Public

        Public Shared Sub DeSerialize(ByVal value As Object, ByVal state As StateBag)
            Dim typ As Type = value.GetType()

            'Parse all the Public instance properties
            For Each member As PropertyInfo In typ.GetProperties(MemberBindingFlags)
                'Determine if they are attributed with a ViewState Attribute
                Dim attr As ViewStateAttribute = member.GetCustomAttributes(GetType(ViewStateAttribute), True) _
                                                        .OfType(Of ViewStateAttribute)() _
                                                        .FirstOrDefault()
                If (attr IsNot Nothing) Then
                    'Get object from ViewState bag
                    Dim viewStateKey As String = attr.ViewStateKey
                    If String.IsNullOrEmpty(viewStateKey) Then
                        'Use class member's name for Key
                        viewStateKey = member.Name
                    End If

                    member.SetValue(value, state.Item(viewStateKey), Nothing)
                End If
            Next
        End Sub

        Public Shared Sub Serialize(ByVal value As Object, ByVal state As StateBag)
            Dim typ As Type = value.GetType()

            'Parse all the Public instance properties
            For Each member As PropertyInfo In typ.GetProperties(MemberBindingFlags)
                'Determine if they are attributed with a ViewState Attribute
                Dim attr As ViewStateAttribute = member.GetCustomAttributes(GetType(ViewStateAttribute), True) _
                                                        .OfType(Of ViewStateAttribute)() _
                                                        .FirstOrDefault()
                If (attr IsNot Nothing) Then
                    'Add property to ViewState bag
                    Dim viewStateKey As String = attr.ViewStateKey
                    If String.IsNullOrEmpty(viewStateKey) Then
                        'Use class member's name for Key
                        viewStateKey = member.Name

                    End If

                    state.Item(viewStateKey) = member.GetValue(value, Nothing)
                End If
            Next
        End Sub

    End Class
End Namespace
