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

Imports System.ComponentModel
Imports System.Reflection

Imports DotNetNuke.Common.Lists

Namespace DotNetNuke.UI.WebControls

    ''' -----------------------------------------------------------------------------
    ''' Project:    DotNetNuke
    ''' Namespace:  DotNetNuke.UI.WebControls
    ''' Class:      ProfileEditorControl
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The ProfileEditorControl control provides a Control to display Profile
    ''' Properties.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    '''     [cnurse]	05/04/2006	created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    <ToolboxData("<{0}:ProfileEditorControl runat=server></{0}:ProfileEditorControl>")> _
    Public Class ProfileEditorControl
        Inherits CollectionEditorControl

#Region "Protected Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' CreateEditor creates the control collection.
        ''' </summary>
        ''' <history>
        '''     [cnurse]	05/08/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overrides Sub CreateEditor()

            CategoryDataField = "PropertyCategory"
            EditorDataField = "DataType"
            NameDataField = "PropertyName"
            RequiredDataField = "Required"
            ValidationExpressionDataField = "ValidationExpression"
            ValueDataField = "PropertyValue"
            VisibleDataField = "Visible"
            VisibilityDataField = "Visibility"
            LengthDataField = "Length"

            MyBase.CreateEditor()

            'We need to wire up the RegionControl to the CountryControl
            For Each editor As FieldEditorControl In Fields
                If TypeOf editor.Editor Is DNNRegionEditControl Then
                    Dim country As ListEntryInfo = Nothing

                    For Each checkEditor As FieldEditorControl In Fields
                        If TypeOf checkEditor.Editor Is DNNCountryEditControl Then
                            Dim countryEdit As DNNCountryEditControl = CType(checkEditor.Editor, DNNCountryEditControl)
                            Dim objListController As New ListController
                            Dim countries As ListEntryInfoCollection = objListController.GetListEntryInfoCollection("Country")
                            For Each checkCountry As ListEntryInfo In countries
                                If checkCountry.Text = CStr(countryEdit.Value) Then
                                    country = checkCountry
                                    Exit For
                                End If
                            Next
                        End If
                    Next
 
                   'Create a ListAttribute for the Region
                    Dim countryKey As String
                    If Not country Is Nothing Then
                        countryKey = "Country." & country.Value
                    Else
                        countryKey = "Country.Unknown"
                    End If

                    Dim attributes(-1) As Object
                    ReDim attributes(0)
                    attributes(0) = New ListAttribute("Region", countryKey, ListBoundField.Text, ListBoundField.Text)
                    editor.Editor.CustomAttributes = attributes

                End If
            Next
        End Sub

#End Region

    End Class

End Namespace

