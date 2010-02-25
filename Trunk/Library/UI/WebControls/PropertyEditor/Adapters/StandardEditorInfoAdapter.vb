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

Imports System.Collections.Specialized
Imports System.ComponentModel
Imports System.Reflection

Namespace DotNetNuke.UI.WebControls

    ''' -----------------------------------------------------------------------------
    ''' Project:    DotNetNuke
    ''' Namespace:  DotNetNuke.UI.WebControls
    ''' Class:      StandardEditorInfoAdapter
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The StandardEditorInfoAdapter control provides an Adapter for standard datasources 
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    '''     [cnurse]	05/05/2006	created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class StandardEditorInfoAdapter
        Implements IEditorInfoAdapter

        Private DataSource As Object
        Private FieldName As String

        Public Sub New(ByVal dataSource As Object, ByVal fieldName As String)
            Me.DataSource = dataSource
            Me.FieldName = fieldName
        End Sub

#Region "Private Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' GetEditorInfo builds an EditorInfo object for a propoerty
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	05/05/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Function GetEditorInfo(ByVal dataSource As Object, ByVal objProperty As PropertyInfo) As EditorInfo

            Dim editInfo As New EditorInfo

            'Get the Name of the property
            editInfo.Name = objProperty.Name()

            'Get the value of the property
            editInfo.Value = objProperty.GetValue(dataSource, Nothing)

            'Get the type of the property
            editInfo.Type = objProperty.PropertyType().AssemblyQualifiedName

            'Get the Custom Attributes for the property
            editInfo.Attributes = objProperty.GetCustomAttributes(True)

            'Get Category Field
            editInfo.Category = String.Empty
            Dim categoryAttributes As Object() = objProperty.GetCustomAttributes(GetType(CategoryAttribute), True)
            If (categoryAttributes.Length > 0) Then
                Dim category As CategoryAttribute = CType(categoryAttributes(0), CategoryAttribute)
                editInfo.Category = category.Category
            End If

            'Get EditMode Field

            If Not objProperty.CanWrite Then
                editInfo.EditMode = PropertyEditorMode.View
            Else
                Dim readOnlyAttributes As Object() = objProperty.GetCustomAttributes(GetType(IsReadOnlyAttribute), True)
                If (readOnlyAttributes.Length > 0) Then
                    Dim readOnlyMode As IsReadOnlyAttribute = CType(readOnlyAttributes(0), IsReadOnlyAttribute)
                    If readOnlyMode.IsReadOnly Then
                        editInfo.EditMode = PropertyEditorMode.View
                    End If
                End If
            End If

            'Get Editor Field
            editInfo.Editor = "UseSystemType"
            Dim editorAttributes As Object() = objProperty.GetCustomAttributes(GetType(EditorAttribute), True)
            If (editorAttributes.Length > 0) Then
                Dim editor As EditorAttribute = Nothing
                For i As Integer = 0 To editorAttributes.Length - 1
                    If CType(editorAttributes(i), EditorAttribute).EditorBaseTypeName.IndexOf("DotNetNuke.UI.WebControls.EditControl") >= 0 Then
                        editor = CType(editorAttributes(i), EditorAttribute)
                        Exit For
                    End If
                Next
                If Not editor Is Nothing Then
                    editInfo.Editor = editor.EditorTypeName
                End If
            End If

            'Get Required Field
            editInfo.Required = False
            Dim requiredAttributes As Object() = objProperty.GetCustomAttributes(GetType(RequiredAttribute), True)
            If (requiredAttributes.Length > 0) Then
                'The property may contain multiple edit mode types, so make sure we only use DotNetNuke editors.
                Dim required As RequiredAttribute = CType(requiredAttributes(0), RequiredAttribute)
                If required.Required Then
                    editInfo.Required = True
                End If
            End If

            'Get Css Style
            editInfo.ControlStyle = New Style
            Dim StyleAttributes As Object() = objProperty.GetCustomAttributes(GetType(ControlStyleAttribute), True)
            If StyleAttributes.Length > 0 Then
                Dim attribute As ControlStyleAttribute = CType(StyleAttributes(0), ControlStyleAttribute)
                editInfo.ControlStyle.CssClass = attribute.CssClass
                editInfo.ControlStyle.Height = attribute.Height
                editInfo.ControlStyle.Width = attribute.Width
            End If

            'Get LabelMode Field
            editInfo.LabelMode = LabelMode.Left
            Dim labelModeAttributes As Object() = objProperty.GetCustomAttributes(GetType(LabelModeAttribute), True)
            If (labelModeAttributes.Length > 0) Then
                Dim mode As LabelModeAttribute = CType(labelModeAttributes(0), LabelModeAttribute)
                editInfo.LabelMode = mode.Mode
            End If

            'Set ResourceKey Field
            editInfo.ResourceKey = String.Format("{0}_{1}", dataSource.GetType.Name, objProperty.Name)

            'Get Validation Expression Field
            editInfo.ValidationExpression = String.Empty
            Dim regExAttributes As Object() = objProperty.GetCustomAttributes(GetType(RegularExpressionValidatorAttribute), True)
            If (regExAttributes.Length > 0) Then
                'The property may contain multiple edit mode types, so make sure we only use DotNetNuke editors.
                Dim regExAttribute As RegularExpressionValidatorAttribute = CType(regExAttributes(0), RegularExpressionValidatorAttribute)
                editInfo.ValidationExpression = regExAttribute.Expression
            End If

            'Set Visibility
            editInfo.Visibility = UserVisibilityMode.AllUsers

            Return editInfo

        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' GetProperty returns the property that is being "bound" to
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	05/05/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Function GetProperty(ByVal dataSource As Object, ByVal fieldName As String) As PropertyInfo
            If Not dataSource Is Nothing Then
                Dim Bindings As BindingFlags = BindingFlags.Public Or BindingFlags.Instance Or BindingFlags.Static
                Dim objProperty As PropertyInfo = dataSource.GetType().GetProperty(fieldName, Bindings)
                Return objProperty
            Else
                Return Nothing
            End If
        End Function

#End Region

        Public Function CreateEditControl() As EditorInfo Implements IEditorInfoAdapter.CreateEditControl

            Dim editInfo As EditorInfo = Nothing

            Dim objProperty As PropertyInfo = GetProperty(DataSource, FieldName)
            If Not objProperty Is Nothing Then
                editInfo = GetEditorInfo(DataSource, objProperty)
            End If

            Return editInfo

        End Function

        Public Function UpdateValue(ByVal e As PropertyEditorEventArgs) As Boolean Implements IEditorInfoAdapter.UpdateValue

            Dim changed As Boolean = e.Changed
            Dim oldValue As Object = e.OldValue
            Dim newValue As Object = e.Value
            Dim _IsDirty As Boolean = Null.NullBoolean

            'Update the DataSource
            If Not DataSource Is Nothing Then
                Dim objProperty As PropertyInfo = DataSource.GetType().GetProperty(e.Name)
                If Not objProperty Is Nothing Then
                    If (Not (newValue Is oldValue)) Or changed Then
                        objProperty.SetValue(DataSource, newValue, Nothing)
                        _IsDirty = True
                    End If
                End If
            End If

            Return _IsDirty

        End Function

        Public Function UpdateVisibility(ByVal e As PropertyEditorEventArgs) As Boolean Implements IEditorInfoAdapter.UpdateVisibility

            Dim _IsDirty As Boolean = Null.NullBoolean
            Return _IsDirty

        End Function

    End Class

End Namespace

