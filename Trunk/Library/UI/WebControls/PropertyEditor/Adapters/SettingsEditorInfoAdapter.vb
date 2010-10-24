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
    ''' Class:      SettingsEditorInfoAdapter
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The SettingsEditorInfoAdapter control provides a factory for creating the 
    ''' appropriate EditInfo object
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    '''     [cnurse]	05/08/2006	created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class SettingsEditorInfoAdapter
        Implements IEditorInfoAdapter

        Private DataMember As Object
        Private DataSource As Object
        Private FieldName As String

        Public Sub New(ByVal dataSource As Object, ByVal dataMember As Object, ByVal fieldName As String)
            Me.DataMember = dataMember
            Me.DataSource = dataSource
            Me.FieldName = fieldName
        End Sub

#Region "Private Methods"

#End Region

        Public Function CreateEditControl() As EditorInfo Implements IEditorInfoAdapter.CreateEditControl

            Dim info As SettingInfo = CType(DataMember, SettingInfo)
            Dim editInfo As New EditorInfo

            'Get the Name of the property
            editInfo.Name = info.Name

            'Get the Category
            editInfo.Category = String.Empty

            'Get Value Field
            editInfo.Value = info.Value

            'Get the type of the property
            editInfo.Type = info.Type.AssemblyQualifiedName

            'Get Editor Field
            editInfo.Editor = info.Editor

            'Get LabelMode Field
            editInfo.LabelMode = LabelMode.Left

            'Get Required Field
            editInfo.Required = False

            'Set ResourceKey Field
            editInfo.ResourceKey = editInfo.Name

            'Get Style
            editInfo.ControlStyle = New Style

            'Get Validation Expression Field
            editInfo.ValidationExpression = String.Empty

            Return editInfo

        End Function

        Public Function UpdateValue(ByVal e As PropertyEditorEventArgs) As Boolean Implements IEditorInfoAdapter.UpdateValue

            Dim key As String
            Dim name As String = e.Name
            Dim changed As Boolean = e.Changed
            Dim oldValue As Object = e.OldValue
            Dim newValue As Object = e.Value
            Dim stringValue As Object = e.StringValue
            Dim _IsDirty As Boolean = Null.NullBoolean

            Dim settings As Hashtable = CType(DataSource, Hashtable)
            Dim settingsEnumerator As IDictionaryEnumerator = settings.GetEnumerator()
            While settingsEnumerator.MoveNext()
                key = CType(settingsEnumerator.Key, String)
                'Do we have the item in the Hashtable being changed
                If key = name Then
                    'Set the Value property to the new value
                    If (Not (newValue Is oldValue)) Or changed Then
                        settings(key) = newValue
                        _IsDirty = True
                        Exit While
                    End If
                End If
            End While

            Return _IsDirty

        End Function

        Public Function UpdateVisibility(ByVal e As PropertyEditorEventArgs) As Boolean Implements IEditorInfoAdapter.UpdateVisibility
            Return Null.NullBoolean
        End Function
    End Class

End Namespace

