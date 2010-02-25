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
    ''' Class:      CollectionEditorControl
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The CollectionEditorControl control provides a Control to display Collection
    ''' Properties.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    '''     [cnurse]	02/14/2006	created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    <ToolboxData("<{0}:CollectionEditorControl runat=server></{0}:CollectionEditorControl>")> _
    Public Class CollectionEditorControl
        Inherits PropertyEditorControl

#Region "Private Members"

        'Data
        Private _CategoryDataField As String
        Private _EditorDataField As String
        Private _LengthDataField As String
        Private _NameDataField As String
        Private _RequiredDataField As String
        Private _TypeDataField As String
        Private _ValidationExpressionDataField As String
        Private _ValueDataField As String
        Private _VisibleDataField As String
        Private _VisibilityDataField As String

        Private _UnderlyingDataSource As IEnumerable

#End Region

#Region "Protected Members"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the Underlying DataSource
        ''' </summary>
        ''' <value>An IEnumerable</value>
        ''' <history>
        ''' 	[cnurse]	03/09/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overrides ReadOnly Property UnderlyingDataSource() As IEnumerable
            Get
                If _UnderlyingDataSource Is Nothing Then
                    _UnderlyingDataSource = CType(DataSource, IEnumerable)
                End If
                Return _UnderlyingDataSource
            End Get
        End Property

#End Region

#Region "Public Properties"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the value of the Category
        ''' </summary>
        ''' <value>A string representing the Category of the Field</value>
        ''' <history>
        ''' 	[cnurse]	04/07/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <Browsable(True), Category("Data"), DefaultValue(""), _
            Description("Enter the name of the field that is data bound to the Category.")> _
        Public Property CategoryDataField() As String
            Get
                Return _CategoryDataField
            End Get
            Set(ByVal Value As String)
                _CategoryDataField = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the value of the Editor Type to use
        ''' </summary>
        ''' <value>A string representing the Editor Type of the Field</value>
        ''' <history>
        ''' 	[cnurse]	02/23/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <Browsable(True), Category("Data"), DefaultValue(""), _
            Description("Enter the name of the field that is data bound to the Editor Type.")> _
        Public Property EditorDataField() As String
            Get
                Return _EditorDataField
            End Get
            Set(ByVal Value As String)
                _EditorDataField = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the value of the Field that determines the length
        ''' </summary>
        ''' <value>A string representing the Name of the Field</value>
        ''' <history>
        ''' 	[cnurse]	05/08/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <Browsable(True), Category("Data"), DefaultValue(""), _
            Description("Enter the name of the field that determines the length.")> _
        Public Property LengthDataField() As String
            Get
                Return _LengthDataField
            End Get
            Set(ByVal Value As String)
                _LengthDataField = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the value of the Field that is bound to the Label
        ''' </summary>
        ''' <value>A string representing the Name of the Field</value>
        ''' <history>
        ''' 	[cnurse]	02/14/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <Browsable(True), Category("Data"), DefaultValue(""), _
            Description("Enter the name of the field that is data bound to the Label's Text property.")> _
        Public Property NameDataField() As String
            Get
                Return _NameDataField
            End Get
            Set(ByVal Value As String)
                _NameDataField = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the value of the Field that determines whether an item is required
        ''' </summary>
        ''' <value>A string representing the Name of the Field</value>
        ''' <history>
        ''' 	[cnurse]	02/24/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <Browsable(True), Category("Data"), DefaultValue(""), _
            Description("Enter the name of the field that determines whether an item is required.")> _
        Public Property RequiredDataField() As String
            Get
                Return _RequiredDataField
            End Get
            Set(ByVal Value As String)
                _RequiredDataField = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the value of the Field that is bound to the EditControl
        ''' </summary>
        ''' <value>A string representing the Name of the Field</value>
        ''' <history>
        ''' 	[cnurse]	02/23/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <Browsable(True), Category("Data"), DefaultValue(""), _
            Description("Enter the name of the field that is data bound to the EditControl's Type.")> _
        Public Property TypeDataField() As String
            Get
                Return _TypeDataField
            End Get
            Set(ByVal Value As String)
                _TypeDataField = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the value of the Field that is bound to the EditControl's 
        ''' Expression Validator
        ''' </summary>
        ''' <value>A string representing the Name of the Field</value>
        ''' <history>
        ''' 	[cnurse]	02/13/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <Browsable(True), Category("Data"), DefaultValue(""), _
            Description("Enter the name of the field that is data bound to the EditControl's Expression Validator.")> _
        Public Property ValidationExpressionDataField() As String
            Get
                Return _ValidationExpressionDataField
            End Get
            Set(ByVal Value As String)
                _ValidationExpressionDataField = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the value of the Field that is bound to the EditControl
        ''' </summary>
        ''' <value>A string representing the Name of the Field</value>
        ''' <history>
        ''' 	[cnurse]	02/13/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <Browsable(True), Category("Data"), DefaultValue(""), _
            Description("Enter the name of the field that is data bound to the EditControl's Value property.")> _
        Public Property ValueDataField() As String
            Get
                Return _ValueDataField
            End Get
            Set(ByVal Value As String)
                _ValueDataField = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the value of the Field that determines whether the control is visible
        ''' </summary>
        ''' <value>A string representing the Name of the Field</value>
        ''' <history>
        ''' 	[cnurse]	02/24/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <Browsable(True), Category("Data"), DefaultValue(""), _
            Description("Enter the name of the field that determines whether the item is visble.")> _
        Public Property VisibleDataField() As String
            Get
                Return _VisibleDataField
            End Get
            Set(ByVal Value As String)
                _VisibleDataField = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the value of the Field that determines the visibility
        ''' </summary>
        ''' <value>A string representing the Name of the Field</value>
        ''' <history>
        ''' 	[cnurse]	05/03/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <Browsable(True), Category("Data"), DefaultValue(""), _
            Description("Enter the name of the field that determines the visibility.")> _
        Public Property VisibilityDataField() As String
            Get
                Return _VisibilityDataField
            End Get
            Set(ByVal Value As String)
                _VisibilityDataField = Value
            End Set
        End Property

#End Region

#Region "Protected Methods"

        Protected Overloads Overrides Sub AddEditorRow(ByRef tbl As Table, ByVal obj As Object)

            Dim fields As Hashtable = New Hashtable
            fields.Add("Category", CategoryDataField)
            fields.Add("Editor", EditorDataField)
            fields.Add("Name", NameDataField)
            fields.Add("Required", RequiredDataField)
            fields.Add("Type", TypeDataField)
            fields.Add("ValidationExpression", ValidationExpressionDataField)
            fields.Add("Value", ValueDataField)
            fields.Add("Visibility", VisibilityDataField)
            fields.Add("Length", LengthDataField)

            AddEditorRow(tbl, NameDataField, New CollectionEditorInfoAdapter(obj, Me.ID, NameDataField, fields))

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' GetCategory gets the Category of an object
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	05/08/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overrides Function GetCategory(ByVal obj As Object) As String

            Dim objProperty As PropertyInfo
            Dim _Category As String = Null.NullString

            'Get Category Field
            If CategoryDataField <> "" Then
                objProperty = obj.GetType().GetProperty(CategoryDataField)
                If Not ((objProperty Is Nothing) OrElse (objProperty.GetValue(obj, Nothing) Is Nothing)) Then
                    _Category = CType(objProperty.GetValue(obj, Nothing), String)
                End If
            End If

            Return _Category

        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' GetGroups gets an array of Groups/Categories from the DataSource
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	04/15/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overrides Function GetGroups(ByVal arrObjects As IEnumerable) As String()

            Dim arrGroups As New ArrayList
            Dim objProperty As PropertyInfo
            Dim strGroups(-1) As String

            For Each obj As Object In arrObjects
                'Get Category Field
                If CategoryDataField <> "" Then
                    objProperty = obj.GetType().GetProperty(CategoryDataField)
                    If Not ((objProperty Is Nothing) OrElse (objProperty.GetValue(obj, Nothing) Is Nothing)) Then
                        Dim _Category As String = CType(objProperty.GetValue(obj, Nothing), String)

                        If Not arrGroups.Contains(_Category) Then
                            arrGroups.Add(_Category)
                        End If
                    End If
                End If
            Next

            ReDim strGroups(arrGroups.Count - 1)
            For i As Integer = 0 To arrGroups.Count - 1
                strGroups(i) = CStr(arrGroups(i))
            Next
            Return strGroups

        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' GetRowVisibility determines the Visibility of a row in the table
        ''' </summary>
        ''' <param name="obj">The property</param>
        ''' <history>
        '''     [cnurse]	03/08/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overrides Function GetRowVisibility(ByVal obj As Object) As Boolean

            Dim isVisible As Boolean = True
            Dim objProperty As PropertyInfo
            objProperty = obj.GetType().GetProperty(VisibleDataField)
            If Not ((objProperty Is Nothing) OrElse (objProperty.GetValue(obj, Nothing) Is Nothing)) Then
                isVisible = CType(objProperty.GetValue(obj, Nothing), Boolean)
            End If

            If Not isVisible AndAlso EditMode = PropertyEditorMode.Edit Then
                'Check if property is required - as this will need to override visibility
                objProperty = obj.GetType().GetProperty(RequiredDataField)
                If Not ((objProperty Is Nothing) OrElse (objProperty.GetValue(obj, Nothing) Is Nothing)) Then
                    isVisible = CType(objProperty.GetValue(obj, Nothing), Boolean)
                End If
            End If

            Return isVisible

        End Function

#End Region

    End Class

End Namespace

