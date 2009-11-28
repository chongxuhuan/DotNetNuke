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

Imports System.Collections.Specialized
Imports System.ComponentModel
Imports System.Reflection

Namespace DotNetNuke.UI.WebControls

    ''' -----------------------------------------------------------------------------
    ''' Project:    DotNetNuke
    ''' Namespace:  DotNetNuke.UI.WebControls
    ''' Class:      CollectionEditorInfoFactory
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The CollectionEditorInfoAdapter control provides an Adapter for Collection Onjects
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    '''     [cnurse]	05/08/2006	created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class CollectionEditorInfoAdapter
        Implements IEditorInfoAdapter

        Private DataSource As Object
        Private FieldName As String
        Private FieldNames As Hashtable
        Private Name As String

        Public Sub New(ByVal dataSource As Object, ByVal name As String, ByVal fieldName As String, ByVal fieldNames As Hashtable)
            Me.DataSource = dataSource
            Me.FieldName = fieldName
            Me.FieldNames = fieldNames
            Me.Name = name
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
        Private Function GetEditorInfo() As EditorInfo

            Dim CategoryDataField As String = CType(FieldNames("Category"), String)
            Dim EditorDataField As String = CType(FieldNames("Editor"), String)
            Dim NameDataField As String = CType(FieldNames("Name"), String)
            Dim RequiredDataField As String = CType(FieldNames("Required"), String)
            Dim TypeDataField As String = CType(FieldNames("Type"), String)
            Dim ValidationExpressionDataField As String = CType(FieldNames("ValidationExpression"), String)
            Dim ValueDataField As String = CType(FieldNames("Value"), String)
            Dim VisibilityDataField As String = CType(FieldNames("Visibility"), String)
            Dim MaxLengthDataField As String = CType(FieldNames("Length"), String)

            Dim editInfo As New EditorInfo
            Dim objProperty As PropertyInfo

            'Get the Name of the property
            editInfo.Name = String.Empty
            'Get Name Field
            If NameDataField <> "" Then
                objProperty = DataSource.GetType().GetProperty(NameDataField)
                If Not ((objProperty Is Nothing) OrElse (objProperty.GetValue(DataSource, Nothing) Is Nothing)) Then
                    editInfo.Name = CType(objProperty.GetValue(DataSource, Nothing), String)
                End If
            End If

            'Get the Category of the property
            editInfo.Category = String.Empty
            'Get Category Field
            If CategoryDataField <> "" Then
                objProperty = DataSource.GetType().GetProperty(CategoryDataField)
                If Not ((objProperty Is Nothing) OrElse (objProperty.GetValue(DataSource, Nothing) Is Nothing)) Then
                    editInfo.Category = CType(objProperty.GetValue(DataSource, Nothing), String)
                End If
            End If

            'Get Value Field
            editInfo.Value = String.Empty
            If ValueDataField <> "" Then
                objProperty = DataSource.GetType().GetProperty(ValueDataField)
                If Not ((objProperty Is Nothing) OrElse (objProperty.GetValue(DataSource, Nothing) Is Nothing)) Then
                    editInfo.Value = CType(objProperty.GetValue(DataSource, Nothing), String)
                End If
            End If

            'Get the type of the property
            editInfo.Type = "System.String"
            If TypeDataField <> "" Then
                objProperty = DataSource.GetType().GetProperty(TypeDataField)
                If Not ((objProperty Is Nothing) OrElse (objProperty.GetValue(DataSource, Nothing) Is Nothing)) Then
                    editInfo.Type = CType(objProperty.GetValue(DataSource, Nothing), String)
                End If
            End If

            'Get Editor Field
            editInfo.Editor = "DotNetNuke.UI.WebControls.TextEditControl, DotNetNuke"
            If EditorDataField <> "" Then
                objProperty = DataSource.GetType().GetProperty(EditorDataField)
                If Not ((objProperty Is Nothing) OrElse (objProperty.GetValue(DataSource, Nothing) Is Nothing)) Then
                    editInfo.Editor = EditorInfo.GetEditor(CType(objProperty.GetValue(DataSource, Nothing), Integer))
                End If
            End If

            'Get LabelMode Field
            editInfo.LabelMode = LabelMode.Left

            'Get Required Field
            editInfo.Required = False
            If RequiredDataField <> "" Then
                objProperty = DataSource.GetType().GetProperty(RequiredDataField)
                If Not ((objProperty Is Nothing) OrElse (objProperty.GetValue(DataSource, Nothing) Is Nothing)) Then
                    editInfo.Required = CType(objProperty.GetValue(DataSource, Nothing), Boolean)
                End If
            End If

            'Set ResourceKey Field
            editInfo.ResourceKey = editInfo.Name
            editInfo.ResourceKey = String.Format("{0}_{1}", Name, editInfo.Name)

            'Get Style
            editInfo.ControlStyle = New Style

            'Get Visibility Field
            editInfo.Visibility = UserVisibilityMode.AllUsers
            If VisibilityDataField <> "" Then
                objProperty = DataSource.GetType().GetProperty(VisibilityDataField)
                If Not ((objProperty Is Nothing) OrElse (objProperty.GetValue(DataSource, Nothing) Is Nothing)) Then
                    editInfo.Visibility = CType(objProperty.GetValue(DataSource, Nothing), UserVisibilityMode)
                End If
            End If

            'Get Validation Expression Field
            editInfo.ValidationExpression = String.Empty
            If ValidationExpressionDataField <> "" Then
                objProperty = DataSource.GetType().GetProperty(ValidationExpressionDataField)
                If Not ((objProperty Is Nothing) OrElse (objProperty.GetValue(DataSource, Nothing) Is Nothing)) Then
                    editInfo.ValidationExpression = CType(objProperty.GetValue(DataSource, Nothing), String)
                End If
            End If

            'Get Length Field
            If MaxLengthDataField <> "" Then
                objProperty = DataSource.GetType().GetProperty(MaxLengthDataField)
                If Not ((objProperty Is Nothing) OrElse (objProperty.GetValue(DataSource, Nothing) Is Nothing)) Then
                    Dim length As Integer = CType(objProperty.GetValue(DataSource, Nothing), Integer)

                    Dim attributes(-1) As Object
                    ReDim attributes(0)
                    attributes(0) = New MaxLengthAttribute(length)
                    editInfo.Attributes = attributes
                End If
            End If

            'Remove spaces from name
            editInfo.Name = editInfo.Name.Replace(" ", "_")

            Return editInfo

        End Function

#End Region

        Public Function CreateEditControl() As EditorInfo Implements IEditorInfoAdapter.CreateEditControl

            Return GetEditorInfo()

        End Function

        Public Function UpdateValue(ByVal e As PropertyEditorEventArgs) As Boolean Implements IEditorInfoAdapter.UpdateValue

            Dim NameDataField As String = CType(FieldNames("Name"), String)
            Dim ValueDataField As String = CType(FieldNames("Value"), String)
            Dim objProperty As PropertyInfo
            Dim PropertyName As String = ""
            Dim changed As Boolean = e.Changed
            Dim name As String = e.Name
            Dim oldValue As Object = e.OldValue
            Dim newValue As Object = e.Value
            Dim stringValue As Object = e.StringValue
            Dim _IsDirty As Boolean = Null.NullBoolean

            'Get the Name Property
            objProperty = DataSource.GetType().GetProperty(NameDataField)
            If Not objProperty Is Nothing Then
                PropertyName = CType(objProperty.GetValue(DataSource, Nothing), String)

                'Do we have the item in the IEnumerable Collection being changed
                PropertyName = PropertyName.Replace(" ", "_")
                If PropertyName = name Then
                    'Get the Value Property
                    objProperty = DataSource.GetType().GetProperty(ValueDataField)

                    'Set the Value property to the new value
                    If (Not (newValue Is oldValue)) Or changed Then
                        If objProperty.PropertyType.FullName = "System.String" Then
                            objProperty.SetValue(DataSource, stringValue, Nothing)
                        Else
                            objProperty.SetValue(DataSource, newValue, Nothing)
                        End If
                        _IsDirty = True
                    End If
                End If
            End If

            Return _IsDirty

        End Function

        Public Function UpdateVisibility(ByVal e As PropertyEditorEventArgs) As Boolean Implements IEditorInfoAdapter.UpdateVisibility

            Dim NameDataField As String = CType(FieldNames("Name"), String)
            Dim VisibilityDataField As String = CType(FieldNames("Visibility"), String)
            Dim objProperty As PropertyInfo
            Dim PropertyName As String = ""
            Dim name As String = e.Name
            Dim newValue As Object = e.Value
            Dim _IsDirty As Boolean = Null.NullBoolean

            'Get the Name Property
            objProperty = DataSource.GetType().GetProperty(NameDataField)
            If Not objProperty Is Nothing Then
                PropertyName = CType(objProperty.GetValue(DataSource, Nothing), String)

                'Do we have the item in the IEnumerable Collection being changed
                PropertyName = PropertyName.Replace(" ", "_")
                If PropertyName = name Then
                    'Get the Visibility Property
                    objProperty = DataSource.GetType().GetProperty(VisibilityDataField)

                    'Set the Visibility property to the new value
                    objProperty.SetValue(DataSource, newValue, Nothing)
                    _IsDirty = True
                End If
            End If

            Return _IsDirty

        End Function

    End Class

End Namespace

