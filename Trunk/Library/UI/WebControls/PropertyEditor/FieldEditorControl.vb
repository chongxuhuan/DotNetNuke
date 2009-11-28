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

Imports System.ComponentModel
Imports System.Reflection

Imports DotNetNuke.Common.Lists
Imports DotNetNuke.Services.Localization
Imports System.Collections.Generic

Namespace DotNetNuke.UI.WebControls

    Public Enum EditorDisplayMode
        Div
        Table
    End Enum

    Public Enum HelpDisplayMode
        Never
        EditOnly
        Always
    End Enum

    ''' -----------------------------------------------------------------------------
    ''' Project:    DotNetNuke
    ''' Namespace:  DotNetNuke.UI.WebControls
    ''' Class:      FieldEditorControl
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The FieldEditorControl control provides a Control to display Profile
    ''' Properties.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    '''     [cnurse]	05/05/2006	created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    <ToolboxData("<{0}:FieldEditorControl runat=server></{0}:FieldEditorControl>")> _
    Public Class FieldEditorControl
        Inherits WebControl
        Implements INamingContainer

#Region "Private Members"

        'Data
        Private _DataSource As Object
        Private _DataField As String
        Private _EditorInfoAdapter As IEditorInfoAdapter
        Private _StdAdapter As StandardEditorInfoAdapter

        Private _EditMode As PropertyEditorMode
        Private _Editor As EditControl
        Private _EditorDisplayMode As EditorDisplayMode = EditorDisplayMode.Div
        Private _EditorTypeName As String = Null.NullString

        Private _HelpDisplayMode As HelpDisplayMode = HelpDisplayMode.Always
        Private _LabelMode As LabelMode = LabelMode.None

        Private _EditControlStyle As Style = New Style
        Private _ErrorStyle As Style = New Style
        Private _HelpStyle As Style = New Style
        Private _LabelStyle As Style = New Style
        Private _VisibilityStyle As Style = New Style

        Private _IsDirty As Boolean

        Private _EditControlWidth As Unit
        Private _LabelWidth As Unit

        Private _EnableClientValidation As Boolean
        Private _LocalResourceFile As String
        Private _Required As Boolean
        Private _RequiredUrl As String
        Private _ShowRequired As Boolean = True
        Private _ShowVisibility As Boolean = False

        Private Validators As New List(Of IValidator)
        Private _ValidationExpression As String = Null.NullString
        Private _IsValid As Boolean = True
        Private _Validated As Boolean = False

#End Region

#Region "Events"

        Public Event ItemAdded As PropertyChangedEventHandler
        Public Event ItemChanged As PropertyChangedEventHandler
        Public Event ItemCreated As EditorCreatedEventHandler
        Public Event ItemDeleted As PropertyChangedEventHandler

#End Region

#Region "Protected Properties"

        Protected Overrides ReadOnly Property TagKey() As System.Web.UI.HtmlTextWriterTag
            Get
                Return HtmlTextWriterTag.Div
            End Get
        End Property

#End Region

#Region "Public Properties"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the DataSource that is bound to this control
        ''' </summary>
        ''' <value>The DataSource object</value>
        ''' <history>
        '''     [cnurse]	05/05/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <Browsable(False)> _
        Public Property DataSource() As Object
            Get
                Return _DataSource
            End Get
            Set(ByVal Value As Object)
                _DataSource = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the value of the Field/property that this control displays
        ''' </summary>
        ''' <value>A string representing the Name of the Field</value>
        ''' <history>
        '''     [cnurse]	05/05/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <Browsable(True), Category("Data"), DefaultValue(""), _
            Description("Enter the name of the field that is data bound to the Control.")> _
        Public Property DataField() As String
            Get
                Return _DataField
            End Get
            Set(ByVal Value As String)
                _DataField = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets whether the control uses Divs or Tables
        ''' </summary>
        ''' <value>An EditorDisplayMode enum</value>
        ''' <history>
        '''     [cnurse]	05/05/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property EditorDisplayMode() As EditorDisplayMode
            Get
                Return _EditorDisplayMode
            End Get
            Set(ByVal Value As EditorDisplayMode)
                _EditorDisplayMode = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the Edit Mode of the Editor
        ''' </summary>
        ''' <value>The mode of the editor</value>
        ''' <history>
        '''     [cnurse]	05/05/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property EditMode() As PropertyEditorMode
            Get
                Return _EditMode
            End Get
            Set(ByVal Value As PropertyEditorMode)
                _EditMode = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the Edit Control associated with the Editor
        ''' </summary>
        ''' <history>
        '''     [cnurse]	05/05/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public ReadOnly Property Editor() As EditControl
            Get
                Return _Editor
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the Factory used to create the Control
        ''' </summary>
        ''' <value>The mode of the editor</value>
        ''' <history>
        '''     [cnurse]	05/05/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property EditorInfoAdapter() As IEditorInfoAdapter
            Get
                If _EditorInfoAdapter Is Nothing Then
                    If _StdAdapter Is Nothing Then
                        _StdAdapter = New StandardEditorInfoAdapter(DataSource, DataField)
                    End If
                    Return _StdAdapter
                Else
                    Return _EditorInfoAdapter
                End If
            End Get
            Set(ByVal Value As IEditorInfoAdapter)
                _EditorInfoAdapter = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the Editor Type to use
        ''' </summary>
        ''' <value>The typename of the editor</value>
        ''' <history>
        '''     [cnurse]	08/29/2007	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property EditorTypeName() As String
            Get
                Return _EditorTypeName
            End Get
            Set(ByVal value As String)
                _EditorTypeName = value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets a flag indicating whether the Validators should use client-side
        ''' validation
        ''' </summary>
        ''' <value>A Boolean</value>
        ''' <history>
        '''     [cnurse]	05/05/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property EnableClientValidation() As Boolean
            Get
                Return _EnableClientValidation
            End Get
            Set(ByVal Value As Boolean)
                _EnableClientValidation = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets whether the control displays Help
        ''' </summary>
        ''' <value>A HelpDisplayMode enum</value>
        ''' <history>
        '''     [cnurse]	05/09/2007	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property HelpDisplayMode() As HelpDisplayMode
            Get
                Return _HelpDisplayMode
            End Get
            Set(ByVal Value As HelpDisplayMode)
                _HelpDisplayMode = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets whether any of the properties have been changed
        ''' </summary>
        ''' <value>A Boolean</value>
        ''' <history>
        '''     [cnurse]	05/05/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public ReadOnly Property IsDirty() As Boolean
            Get
                Return _IsDirty
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets whether all of the properties are Valid
        ''' </summary>
        ''' <value>A Boolean</value>
        ''' <history>
        '''     [cnurse]	05/05/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public ReadOnly Property IsValid() As Boolean
            Get
                If Not _Validated Then
                    Validate()
                End If
                Return _IsValid
            End Get
        End Property

        Public Property LabelMode() As LabelMode
            Get
                Return _LabelMode
            End Get
            Set(ByVal value As LabelMode)
                _LabelMode = value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the Local Resource File for the Control
        ''' </summary>
        ''' <value>A String</value>
        ''' <history>
        '''     [cnurse]	05/17/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property LocalResourceFile() As String
            Get
                Return _LocalResourceFile
            End Get
            Set(ByVal Value As String)
                _LocalResourceFile = Value
            End Set
        End Property

        Public Property Required() As Boolean
            Get
                Return _Required
            End Get
            Set(ByVal value As Boolean)
                _Required = value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the Url of the Required Image
        ''' </summary>
        ''' <value>A String</value>
        ''' <history>
        '''     [cnurse]	05/05/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property RequiredUrl() As String
            Get
                Return _RequiredUrl
            End Get
            Set(ByVal Value As String)
                _RequiredUrl = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' gets and sets whether the Required icon is used
        ''' </summary>
        ''' <history>
        '''     [cnurse]	05/08/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property ShowRequired() As Boolean
            Get
                Return _ShowRequired
            End Get
            Set(ByVal Value As Boolean)
                _ShowRequired = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' gets and sets whether the Visibility control is used
        ''' </summary>
        ''' <history>
        '''     [cnurse]	05/05/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property ShowVisibility() As Boolean
            Get
                Return _ShowVisibility
            End Get
            Set(ByVal Value As Boolean)
                _ShowVisibility = Value
            End Set
        End Property

        Public Property ValidationExpression() As String
            Get
                Return _ValidationExpression
            End Get
            Set(ByVal Value As String)
                _ValidationExpression = Value
            End Set
        End Property

#Region "Style Properties"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the value of the Field Style
        ''' </summary>
        ''' <value>A Style object</value>
        ''' <history>
        '''     [cnurse]	05/05/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <Browsable(True), Category("Styles"), _
            DesignerSerializationVisibility(DesignerSerializationVisibility.Content), _
            PersistenceMode(PersistenceMode.InnerProperty), _
            TypeConverter(GetType(ExpandableObjectConverter)), _
            Description("Set the Style for the Edit Control.")> _
        Public ReadOnly Property EditControlStyle() As System.Web.UI.WebControls.Style
            Get
                Return _EditControlStyle
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the width of the Edit Control Column
        ''' </summary>
        ''' <value>A Style object</value>
        ''' <history>
        '''     [cnurse]	05/05/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <Browsable(True), Category("Appearance"), _
            Description("Set the Width for the Edit Control.")> _
        Public Property EditControlWidth() As Unit
            Get
                Return _EditControlWidth
            End Get
            Set(ByVal Value As Unit)
                _EditControlWidth = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the value of the Error Style
        ''' </summary>
        ''' <value>A Style object</value>
        ''' <history>
        '''     [cnurse]	05/05/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <Browsable(True), Category("Styles"), _
            DesignerSerializationVisibility(DesignerSerializationVisibility.Content), _
            PersistenceMode(PersistenceMode.InnerProperty), _
            TypeConverter(GetType(ExpandableObjectConverter)), _
            Description("Set the Style for the Error Text.")> _
        Public ReadOnly Property ErrorStyle() As System.Web.UI.WebControls.Style
            Get
                Return _ErrorStyle
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the value of the Label Style
        ''' </summary>
        ''' <value>A Style object</value>
        ''' <history>
        '''     [cnurse]	05/05/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <Browsable(True), Category("Styles"), _
            DesignerSerializationVisibility(DesignerSerializationVisibility.Content), _
            PersistenceMode(PersistenceMode.InnerProperty), _
            TypeConverter(GetType(ExpandableObjectConverter)), _
            Description("Set the Style for the Help Text.")> _
        Public ReadOnly Property HelpStyle() As System.Web.UI.WebControls.Style
            Get
                Return _HelpStyle
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the value of the Label Style
        ''' </summary>
        ''' <value>A Style object</value>
        ''' <history>
        '''     [cnurse]	05/05/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <Browsable(True), Category("Styles"), _
            DesignerSerializationVisibility(DesignerSerializationVisibility.Content), _
            PersistenceMode(PersistenceMode.InnerProperty), _
            TypeConverter(GetType(ExpandableObjectConverter)), _
            Description("Set the Style for the Label Text")> _
        Public ReadOnly Property LabelStyle() As System.Web.UI.WebControls.Style
            Get
                Return _LabelStyle
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the width of the Label Column
        ''' </summary>
        ''' <value>A Style object</value>
        ''' <history>
        '''     [cnurse]	05/05/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <Browsable(True), Category("Appearance"), _
            Description("Set the Width for the Label Control.")> _
        Public Property LabelWidth() As Unit
            Get
                Return _LabelWidth
            End Get
            Set(ByVal Value As Unit)
                _LabelWidth = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the value of the Visibility Style
        ''' </summary>
        ''' <value>A Style object</value>
        ''' <history>
        '''     [cnurse]	05/05/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <Browsable(True), Category("Styles"), _
            DesignerSerializationVisibility(DesignerSerializationVisibility.Content), _
            PersistenceMode(PersistenceMode.InnerProperty), _
            TypeConverter(GetType(ExpandableObjectConverter)), _
            Description("Set the Style for the Visibility Control")> _
        Public ReadOnly Property VisibilityStyle() As System.Web.UI.WebControls.Style
            Get
                Return _VisibilityStyle
            End Get
        End Property

#End Region

#End Region

#Region "Private Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' BuildDiv creates the Control as a Div
        ''' </summary>
        ''' <param name="editInfo">The EditorInfo object for this control</param>
        ''' <history>
        '''     [cnurse]	05/08/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub BuildDiv(ByVal editInfo As EditorInfo)

            Dim divLabel As HtmlGenericControl = Nothing

            If editInfo.LabelMode <> LabelMode.None Then
                divLabel = New HtmlGenericControl("div")
                Dim style As String = "float: " + editInfo.LabelMode.ToString().ToLower()
                If editInfo.LabelMode = LabelMode.Left Or editInfo.LabelMode = LabelMode.Right Then
                    style += "; width: " + LabelWidth.ToString()
                End If
                divLabel.Attributes.Add("style", style)
                divLabel.Controls.Add(BuildLabel(editInfo))
            End If

            Dim divEdit As New HtmlGenericControl("div")
            Dim side As String = GetOppositeSide(editInfo.LabelMode)
            If side.Length > 0 Then
                Dim style As String = "float: " + side
                style += "; width: " + EditControlWidth.ToString()
                divEdit.Attributes.Add("style", style)
            End If

            Dim propEditor As EditControl = BuildEditor(editInfo)
            Dim visibility As VisibilityControl = BuildVisibility(editInfo)
            If Not visibility Is Nothing Then
                visibility.Attributes.Add("style", "float: right;")
                divEdit.Controls.Add(visibility)
            End If
            divEdit.Controls.Add(propEditor)
            Dim requiredIcon As System.Web.UI.WebControls.Image = BuildRequiredIcon(editInfo)
            If Not requiredIcon Is Nothing Then
                divEdit.Controls.Add(requiredIcon)
            End If

            If editInfo.LabelMode = LabelMode.Left Or editInfo.LabelMode = LabelMode.Top Then
                Controls.Add(divLabel)
                Controls.Add(divEdit)
            Else
                Controls.Add(divEdit)
                If (Not divLabel Is Nothing) Then
                    Controls.Add(divLabel)
                End If
            End If

            'Build the Validators
            BuildValidators(editInfo, propEditor.ID)

            If Validators.Count > 0 Then
                'Add the Validators to the editor cell
                For Each validator As BaseValidator In Validators
                    validator.Width = Me.Width
                    Controls.Add(validator)
                Next
            End If

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' BuildEditor creates the editor part of the Control
        ''' </summary>
        ''' <param name="editInfo">The EditorInfo object for this control</param>
        ''' <history>
        '''     [cnurse]	05/08/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Function BuildEditor(ByVal editInfo As EditorInfo) As EditControl

            Dim propEditor As EditControl = EditControlFactory.CreateEditControl(editInfo)
            propEditor.ControlStyle.CopyFrom(EditControlStyle)
            propEditor.LocalResourceFile = LocalResourceFile
            If Not editInfo.ControlStyle Is Nothing Then
                propEditor.ControlStyle.CopyFrom(editInfo.ControlStyle)
            End If
            AddHandler propEditor.ItemAdded, AddressOf Me.CollectionItemAdded
            AddHandler propEditor.ItemDeleted, AddressOf Me.CollectionItemDeleted
            AddHandler propEditor.ValueChanged, AddressOf Me.ValueChanged
            If TypeOf propEditor Is DNNListEditControl Then
                Dim listEditor As DNNListEditControl = CType(propEditor, DNNListEditControl)
                AddHandler listEditor.ItemChanged, AddressOf Me.ListItemChanged
            End If

            _Editor = propEditor

            Return propEditor

        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' BuildLabel creates the label part of the Control
        ''' </summary>
        ''' <param name="editInfo">The EditorInfo object for this control</param>
        ''' <history>
        '''     [cnurse]	05/08/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Function BuildLabel(ByVal editInfo As EditorInfo) As PropertyLabelControl

            Dim propLabel As PropertyLabelControl = New PropertyLabelControl
            propLabel.ID = editInfo.Name + "_Label"
            propLabel.HelpStyle.CopyFrom(HelpStyle)
            propLabel.LabelStyle.CopyFrom(LabelStyle)
            Dim strValue As String = TryCast(editInfo.Value, String)
            Select Case HelpDisplayMode
                Case HelpDisplayMode.Always
                    propLabel.ShowHelp = True
                Case HelpDisplayMode.EditOnly
                    If editInfo.EditMode = PropertyEditorMode.Edit Or (editInfo.Required And String.IsNullOrEmpty(strValue)) Then
                        propLabel.ShowHelp = True
                    Else
                        propLabel.ShowHelp = False
                    End If
                Case HelpDisplayMode.Never
                    propLabel.ShowHelp = False
            End Select
            propLabel.Caption = editInfo.Name
            propLabel.HelpText = editInfo.Name
            propLabel.ResourceKey = editInfo.ResourceKey
            If editInfo.LabelMode = LabelMode.Left Or editInfo.LabelMode = LabelMode.Right Then
                propLabel.Width = LabelWidth
            End If

            Return propLabel

        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' BuildValidators creates the validators part of the Control
        ''' </summary>
        ''' <param name="editInfo">The EditorInfo object for this control</param>
        ''' <history>
        '''     [cnurse]	05/08/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Function BuildRequiredIcon(ByVal editInfo As EditorInfo) As System.Web.UI.WebControls.Image

            Dim img As System.Web.UI.WebControls.Image = Nothing

            Dim strValue As String = TryCast(editInfo.Value, String)
            If ShowRequired AndAlso editInfo.Required AndAlso (editInfo.EditMode = PropertyEditorMode.Edit Or (editInfo.Required And String.IsNullOrEmpty(strValue))) Then
                img = New System.Web.UI.WebControls.Image
                If RequiredUrl = Null.NullString Then
                    img.ImageUrl = "~/images/required.gif"
                Else
                    img.ImageUrl = RequiredUrl
                End If
                img.Attributes.Add("resourcekey", editInfo.ResourceKey + ".Required")
            End If

            Return img

        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' BuildTable creates the Control as a Table
        ''' </summary>
        ''' <param name="editInfo">The EditorInfo object for this control</param>
        ''' <history>
        '''     [cnurse]	05/08/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub BuildTable(ByVal editInfo As EditorInfo)

            Dim tbl As New Table
            Dim labelCell As New TableCell
            Dim editorCell As New TableCell

            'Build Label Cell
            labelCell.VerticalAlign = VerticalAlign.Top
            labelCell.Controls.Add(BuildLabel(editInfo))
            If editInfo.LabelMode = LabelMode.Left Or editInfo.LabelMode = LabelMode.Right Then
                labelCell.Width = LabelWidth
            End If

            'Build Editor Cell
            editorCell.VerticalAlign = VerticalAlign.Top
            Dim propEditor As EditControl = BuildEditor(editInfo)
            Dim requiredIcon As System.Web.UI.WebControls.Image = BuildRequiredIcon(editInfo)
            editorCell.Controls.Add(propEditor)
            If Not requiredIcon Is Nothing Then
                editorCell.Controls.Add(requiredIcon)
            End If
            If editInfo.LabelMode = LabelMode.Left Or editInfo.LabelMode = LabelMode.Right Then
                editorCell.Width = EditControlWidth
            End If

            Dim visibility As VisibilityControl = BuildVisibility(editInfo)
            If Not visibility Is Nothing Then
                editorCell.Controls.Add(New LiteralControl("&nbsp;&nbsp;"))
                editorCell.Controls.Add(visibility)
            End If

            'Add cells to table
            Dim editorRow As New TableRow
            Dim labelRow As New TableRow
            If editInfo.LabelMode = LabelMode.Bottom Or editInfo.LabelMode = LabelMode.Top Or editInfo.LabelMode = LabelMode.None Then
                editorCell.ColumnSpan = 2
                editorRow.Cells.Add(editorCell)
                If editInfo.LabelMode = LabelMode.Bottom Or editInfo.LabelMode = LabelMode.Top Then
                    labelCell.ColumnSpan = 2
                    labelRow.Cells.Add(labelCell)
                End If
                If editInfo.LabelMode = LabelMode.Top Then
                    tbl.Rows.Add(labelRow)
                End If
                tbl.Rows.Add(editorRow)
                If editInfo.LabelMode = LabelMode.Bottom Then
                    tbl.Rows.Add(labelRow)
                End If
            ElseIf editInfo.LabelMode = LabelMode.Left Then
                editorRow.Cells.Add(labelCell)
                editorRow.Cells.Add(editorCell)
                tbl.Rows.Add(editorRow)
            ElseIf editInfo.LabelMode = LabelMode.Right Then
                editorRow.Cells.Add(editorCell)
                editorRow.Cells.Add(labelCell)
                tbl.Rows.Add(editorRow)
            End If

            'Build the Validators
            BuildValidators(editInfo, propEditor.ID)

            Dim validatorsRow As New TableRow
            Dim validatorsCell As New TableCell
            validatorsCell.ColumnSpan = 2
            'Add the Validators to the editor cell
            For Each validator As BaseValidator In Validators
                validatorsCell.Controls.Add(validator)
            Next
            validatorsRow.Cells.Add(validatorsCell)
            tbl.Rows.Add(validatorsRow)

            'Add the Table to the Controls Collection
            Controls.Add(tbl)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' BuildValidators creates the validators part of the Control
        ''' </summary>
        ''' <param name="editInfo">The EditorInfo object for this control</param>
        ''' <history>
        '''     [cnurse]	05/08/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub BuildValidators(ByVal editInfo As EditorInfo, ByVal targetId As String)

            Validators.Clear()

            'Add Required Validators
            If editInfo.Required Then
                Dim reqValidator As New RequiredFieldValidator
                reqValidator.ID = editInfo.Name + "_Req"
                reqValidator.ControlToValidate = targetId
                reqValidator.Display = ValidatorDisplay.Dynamic
                reqValidator.ControlStyle.CopyFrom(ErrorStyle)
                reqValidator.EnableClientScript = EnableClientValidation
                reqValidator.Attributes.Add("resourcekey", editInfo.ResourceKey + ".Required")
                reqValidator.ErrorMessage = editInfo.Name + " is Required"
                Validators.Add(reqValidator)
            End If

            'Add Regular Expression Validators
            If editInfo.ValidationExpression <> "" Then
                Dim regExValidator As New RegularExpressionValidator
                regExValidator.ID = editInfo.Name + "_RegEx"
                regExValidator.ControlToValidate = targetId
                regExValidator.ValidationExpression = editInfo.ValidationExpression
                regExValidator.Display = ValidatorDisplay.Dynamic
                regExValidator.ControlStyle.CopyFrom(ErrorStyle)
                regExValidator.EnableClientScript = EnableClientValidation
                regExValidator.Attributes.Add("resourcekey", editInfo.ResourceKey + ".Validation")
                regExValidator.ErrorMessage = editInfo.Name + " is Invalid"
                Validators.Add(regExValidator)
            End If

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' BuildVisibility creates the visibility part of the Control
        ''' </summary>
        ''' <param name="editInfo">The EditorInfo object for this control</param>
        ''' <history>
        '''     [cnurse]	05/08/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Function BuildVisibility(ByVal editInfo As EditorInfo) As VisibilityControl

            Dim visControl As VisibilityControl = Nothing

            If ShowVisibility Then
                visControl = New VisibilityControl
                visControl.ID = Me.ID + "_vis"
                visControl.Caption = Localization.GetString("Visibility")
                visControl.Name = editInfo.Name
                visControl.Value = editInfo.Visibility
                visControl.ControlStyle.CopyFrom(VisibilityStyle)
                AddHandler visControl.VisibilityChanged, AddressOf Me.VisibilityChanged
            End If

            Return visControl

        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' GetOppositeSide finds the opposite side (ie if LabelMode is left it returns right)
        ''' </summary>
        ''' <param name="labelMode">The LabelMode for this control</param>
        ''' <history>
        '''     [cnurse]	05/08/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Function GetOppositeSide(ByVal labelMode As LabelMode) As String
            Select Case labelMode
                Case labelMode.Bottom
                    Return "top"
                Case labelMode.Left
                    Return "right"
                Case labelMode.Right
                    Return "left"
                Case labelMode.Top
                    Return "bottom"
                Case Else
                    Return String.Empty
            End Select
        End Function

#End Region

#Region "Protected Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' CreateEditor creates the control collection for this control
        ''' </summary>
        ''' <history>
        '''     [cnurse]	05/05/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overridable Sub CreateEditor()

            Dim editInfo As EditorInfo = EditorInfoAdapter.CreateEditControl()
            If editInfo.EditMode = PropertyEditorMode.Edit Then
                editInfo.EditMode = EditMode
            End If

            'Get the Editor Type to use (if specified)
            If Not String.IsNullOrEmpty(EditorTypeName) Then
                editInfo.Editor = EditorTypeName
            End If

            'Get the Label Mode to use (if specified)
            If LabelMode <> UI.WebControls.LabelMode.Left Then
                editInfo.LabelMode = LabelMode
            End If

            ' if Required is specified set editors property
            If Required Then
                editInfo.Required = Required
            End If

            'Get the ValidationExpression to use (if specified)
            If Not String.IsNullOrEmpty(ValidationExpression) Then
                editInfo.ValidationExpression = ValidationExpression
            End If

            'Raise the ItemCreated Event
            OnItemCreated(New PropertyEditorItemEventArgs(editInfo))

            Me.Visible = editInfo.Visible

            If EditorDisplayMode = EditorDisplayMode.Div Then
                BuildDiv(editInfo)
            Else
                BuildTable(editInfo)
            End If

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Runs when an item is added to a collection type property
        ''' </summary>
        ''' <history>
        '''     [cnurse]	02/05/2008	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overridable Sub CollectionItemAdded(ByVal sender As Object, ByVal e As PropertyEditorEventArgs)
            OnItemAdded(e)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Runs when an item is removed from a collection type property
        ''' </summary>
        ''' <history>
        '''     [cnurse]	02/05/2008	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overridable Sub CollectionItemDeleted(ByVal sender As Object, ByVal e As PropertyEditorEventArgs)
            OnItemDeleted(e)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Runs when an item is added to a collection type property
        ''' </summary>
        ''' <history>
        '''     [cnurse]	02/05/2008	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overridable Sub OnItemAdded(ByVal e As PropertyEditorEventArgs)
            RaiseEvent ItemAdded(Me, e)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Runs when the Editor is Created
        ''' </summary>
        ''' <history>
        '''     [cnurse]	02/20/2007	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overridable Sub OnItemCreated(ByVal e As PropertyEditorItemEventArgs)
            RaiseEvent ItemCreated(Me, e)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Runs when an item is removed from a collection type property
        ''' </summary>
        ''' <history>
        '''     [cnurse]	02/05/2008	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overridable Sub OnItemDeleted(ByVal e As PropertyEditorEventArgs)
            RaiseEvent ItemDeleted(Me, e)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Runs when the Value of a Property changes
        ''' </summary>
        ''' <history>
        '''     [cnurse]	05/05/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overridable Sub ValueChanged(ByVal sender As Object, ByVal e As PropertyEditorEventArgs)
            _IsDirty = EditorInfoAdapter.UpdateValue(e)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Runs when the Visibility of a Property changes
        ''' </summary>
        ''' <history>
        '''     [cnurse]	05/05/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overridable Sub VisibilityChanged(ByVal sender As Object, ByVal e As PropertyEditorEventArgs)
            _IsDirty = EditorInfoAdapter.UpdateVisibility(e)
        End Sub

#End Region

#Region "Public Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Binds the controls to the DataSource
        ''' </summary>
        ''' <history>
        '''     [cnurse]	05/05/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overrides Sub DataBind()

            'Invoke OnDataBinding so DataBinding Event is raised
            MyBase.OnDataBinding(EventArgs.Empty)

            'Clear Existing Controls
            Controls.Clear()

            'Clear Child View State as controls will be loaded from DataSource
            ClearChildViewState()

            'Start Tracking ViewState
            TrackViewState()

            'Create the editor
            CreateEditor()

            'Set flag so CreateChildConrols should not be invoked later in control's lifecycle
            ChildControlsCreated = True

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Validates the data, and sets the IsValid Property
        ''' </summary>
        ''' <history>
        '''     [cnurse]	05/05/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overridable Sub Validate()

            _IsValid = Editor.IsValid

            If _IsValid Then
                Dim valEnumerator As IEnumerator = Validators.GetEnumerator()
                While valEnumerator.MoveNext()
                    Dim validator As IValidator = CType(valEnumerator.Current, IValidator)
                    validator.Validate()
                    If Not validator.IsValid Then
                        _IsValid = False
                        Exit While
                    End If
                End While

                _Validated = True
            End If

        End Sub

#End Region

#Region "Event Handlers"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Runs when an Item in the List Is Changed
        ''' </summary>
        ''' <remarks>Raises an ItemChanged event.</remarks>
        ''' <history>
        '''     [cnurse]	05/05/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overridable Sub ListItemChanged(ByVal sender As Object, ByVal e As PropertyEditorEventArgs)
            RaiseEvent ItemChanged(Me, e)
        End Sub


#End Region

    End Class

End Namespace

