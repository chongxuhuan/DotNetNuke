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

Namespace DotNetNuke.UI.WebControls

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The PropertyEditorControl control provides a way to display and edit any 
    ''' properties of any Info class
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    '''     [cnurse]	02/21/2006	created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    <ToolboxData("<{0}:PropertyEditorControl runat=server></{0}:PropertyEditorControl>"), _
    Designer(GetType(DotNetNuke.UI.WebControls.Design.PropertyEditorControlDesigner)), _
    PersistChildren(True)> _
    Public Class PropertyEditorControl
        Inherits WebControl
        Implements INamingContainer

#Region "Events"

        Public Event ItemAdded As PropertyChangedEventHandler
        Public Event ItemCreated As EditorCreatedEventHandler
        Public Event ItemDeleted As PropertyChangedEventHandler

#End Region

#Region "Private Members"

        'Data
        Private _AutoGenerate As Boolean = True
        Private _DataSource As Object
        Private _UnderlyingDataSource As IEnumerable

        Private _SortMode As PropertySortType
        Private _EditMode As PropertyEditorMode
        Private _DisplayMode As EditorDisplayMode
        Private _GroupByMode As GroupByMode
        Private _HelpDisplayMode As HelpDisplayMode = HelpDisplayMode.Always
        Private _LabelMode As LabelMode = LabelMode.Left

        Private _EditControlStyle As Style = New Style
        Private _ErrorStyle As Style = New Style
        Private _GroupHeaderStyle As Style = New Style
        Private _HelpStyle As Style = New Style
        Private _LabelStyle As Style = New Style
        Private _VisibilityStyle As Style = New Style

        Private _IsDirty As Boolean
        Private _ItemChanged As Boolean = False
        Private _GroupHeaderIncludeRule As Boolean

        Private _EditControlWidth As Unit
        Private _LabelWidth As Unit

        Private _EnableClientValidation As Boolean
        Private _LocalResourceFile As String
        Private _RequiredUrl As String
        Private _ShowRequired As Boolean = True
        Private _ShowVisibility As Boolean = False

        Private _Groups As String = Null.NullString
        Private _Sections As Hashtable

        Private _Fields As New ArrayList

#End Region

#Region "Protected Members"

        Protected Overrides ReadOnly Property TagKey() As HtmlTextWriterTag
            Get
                Return HtmlTextWriterTag.Div
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the Underlying DataSource
        ''' </summary>
        ''' <value>An IEnumerable Boolean</value>
        ''' <history>
        ''' 	[cnurse]	03/09/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overridable ReadOnly Property UnderlyingDataSource() As IEnumerable
            Get
                If _UnderlyingDataSource Is Nothing Then
                    _UnderlyingDataSource = GetProperties()
                End If
                Return _UnderlyingDataSource
            End Get
        End Property

#End Region

#Region "Public Properties"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets whether the editor Autogenerates its editors
        ''' </summary>
        ''' <value>The DataSource object</value>
        ''' <history>
        ''' 	[cnurse]	02/14/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <Category("Behavior")> _
        Public Property AutoGenerate() As Boolean
            Get
                Return _AutoGenerate
            End Get
            Set(ByVal Value As Boolean)
                _AutoGenerate = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the DataSource that is bound to this control
        ''' </summary>
        ''' <value>The DataSource object</value>
        ''' <history>
        ''' 	[cnurse]	02/14/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <Browsable(False), Category("Data")> _
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
        ''' Gets and sets the Edit Mode of the Editor
        ''' </summary>
        ''' <value>The mode of the editor</value>
        ''' <history>
        '''     [cnurse]	02/27/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <Category("Appearance")> _
        Public Property EditMode() As PropertyEditorMode
            Get
                Return _EditMode
            End Get
            Set(ByVal Value As PropertyEditorMode)
                _EditMode = Value
            End Set
        End Property

        Public Property DisplayMode() As EditorDisplayMode
            Get
                Return _DisplayMode
            End Get
            Set(ByVal value As EditorDisplayMode)
                _DisplayMode = value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets a flag indicating whether the Validators should use client-side
        ''' validation
        ''' </summary>
        ''' <value>A Boolean</value>
        ''' <history>
        ''' 	[cnurse]	03/07/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <Category("Behavior")> _
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
        ''' Gets and sets the grouping mode
        ''' </summary>
        ''' <value>A GroupByMode enum</value>
        ''' <history>
        ''' 	[cnurse]	04/07/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <Category("Appearance")> _
        Public Property GroupByMode() As GroupByMode
            Get
                Return _GroupByMode
            End Get
            Set(ByVal value As GroupByMode)
                _GroupByMode = value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the grouping order
        ''' </summary>
        ''' <value>A comma-delimited list of categories/groups</value>
        ''' <history>
        ''' 	[cnurse]	04/15/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <Category("Appearance")> _
        Public Property Groups() As String
            Get
                Return _Groups
            End Get
            Set(ByVal value As String)
                _Groups = value
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
        ''' 	[cnurse]	02/14/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <Browsable(False)> _
        Public ReadOnly Property IsDirty() As Boolean
            Get
                Dim _IsDirty As Boolean
                For Each editor As FieldEditorControl In Fields
                    If editor.Visible AndAlso editor.IsDirty Then
                        _IsDirty = True
                        Exit For
                    End If
                Next
                Return _IsDirty
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets whether all of the properties are Valid
        ''' </summary>
        ''' <value>A Boolean</value>
        ''' <history>
        ''' 	[cnurse]	03/07/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <Browsable(False)> _
        Public ReadOnly Property IsValid() As Boolean
            Get
                Dim _IsValid As Boolean = True
                For Each editor As FieldEditorControl In Fields
                    If editor.Visible AndAlso Not editor.IsValid Then
                        _IsValid = False
                        Exit For
                    End If
                Next
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
        '''     [cnurse]	05/03/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <Category("Appearance")> _
        Public Property ShowVisibility() As Boolean
            Get
                Return _ShowVisibility
            End Get
            Set(ByVal Value As Boolean)
                _ShowVisibility = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets whether to sort properties. 
        ''' </summary>
        ''' <value>The Sort Mode of the editor</value>
        ''' <remarks>
        ''' By default all properties will be sorted 
        ''' </remarks>
        ''' <history>
        ''' 	[Joe]	2/25/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <Category("Appearance")> _
        Public Property SortMode() As PropertySortType
            Get
                Return _SortMode
            End Get
            Set(ByVal Value As PropertySortType)
                _SortMode = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets a collection of fields to display if AutoGenerate is false. Or the
        ''' collection of fields generated if AutoGenerate is true.
        ''' </summary>
        ''' <value>A collection of FieldEditorControl objects</value>
        ''' <history>
        '''     [cnurse]	05/08/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <Category("Behavior"), _
        PersistenceMode(PersistenceMode.InnerProperty), _
        DesignerSerializationVisibility(DesignerSerializationVisibility.Content)> _
        Public ReadOnly Property Fields() As ArrayList
            Get
                Return _Fields
            End Get
        End Property

#Region "Style Properties"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the value of the Field Style
        ''' </summary>
        ''' <value>A Style object</value>
        ''' <history>
        ''' 	[cnurse]	02/23/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <Browsable(True), Category("Styles"), _
            DesignerSerializationVisibility(DesignerSerializationVisibility.Content), _
            TypeConverter(GetType(ExpandableObjectConverter)), _
            PersistenceMode(PersistenceMode.InnerProperty), _
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
        ''' 	[cnurse]	02/23/2006	Created
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
        ''' 	[cnurse]	03/07/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <Browsable(True), Category("Styles"), _
            DesignerSerializationVisibility(DesignerSerializationVisibility.Content), _
            TypeConverter(GetType(ExpandableObjectConverter)), _
            PersistenceMode(PersistenceMode.InnerProperty), _
            Description("Set the Style for the Error Text.")> _
        Public ReadOnly Property ErrorStyle() As System.Web.UI.WebControls.Style
            Get
                Return _ErrorStyle
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the value of the Group Header Style
        ''' </summary>
        ''' <value>A Style object</value>
        ''' <history>
        ''' 	[cnurse]	04/11/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <Browsable(True), Category("Styles"), _
            DesignerSerializationVisibility(DesignerSerializationVisibility.Content), _
            TypeConverter(GetType(ExpandableObjectConverter)), _
            PersistenceMode(PersistenceMode.InnerProperty), _
            Description("Set the Style for the Group Header Control.")> _
        Public ReadOnly Property GroupHeaderStyle() As System.Web.UI.WebControls.Style
            Get
                Return _GroupHeaderStyle
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets whether to add a &lt;hr&gt; to the Group Header
        ''' </summary>
        ''' <value>A boolean</value>
        ''' <history>
        ''' 	[cnurse]	02/23/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <Browsable(True), Category("Appearance"), _
            Description("Set whether to include a rule <hr> in the Group Header.")> _
        Public Property GroupHeaderIncludeRule() As Boolean
            Get
                Return _GroupHeaderIncludeRule
            End Get
            Set(ByVal Value As Boolean)
                _GroupHeaderIncludeRule = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the value of the Label Style
        ''' </summary>
        ''' <value>A Style object</value>
        ''' <history>
        ''' 	[cnurse]	02/23/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <Browsable(True), Category("Styles"), _
            DesignerSerializationVisibility(DesignerSerializationVisibility.Content), _
            TypeConverter(GetType(ExpandableObjectConverter)), _
            PersistenceMode(PersistenceMode.InnerProperty), _
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
        ''' 	[cnurse]	02/23/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <Browsable(True), Category("Styles"), _
            DesignerSerializationVisibility(DesignerSerializationVisibility.Content), _
            TypeConverter(GetType(ExpandableObjectConverter)), _
            PersistenceMode(PersistenceMode.InnerProperty), _
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
        ''' 	[cnurse]	02/23/2006	Created
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
        ''' 	[cnurse]	05/03/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <Browsable(True), Category("Styles"), _
            DesignerSerializationVisibility(DesignerSerializationVisibility.Content), _
            TypeConverter(GetType(ExpandableObjectConverter)), _
            PersistenceMode(PersistenceMode.InnerProperty), _
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
        ''' GetProperties returns an array of <see cref="System.Reflection.PropertyInfo">PropertyInfo</see>
        ''' </summary>
        ''' <returns>An array of <see cref="System.Reflection.PropertyInfo">PropertyInfo</see> objects
        ''' for the current DataSource object.</returns>
        ''' <remarks>
        ''' GetProperties will return an array of public properties for the current DataSource
        ''' object.  The properties will be sorted according to the SortMode property.
        ''' </remarks>
        ''' <history>
        ''' 	[Joe]	2/25/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Function GetProperties() As PropertyInfo()

            If Not DataSource Is Nothing Then
                'TODO:  We need to add code to support using the cache in the future

                Dim Bindings As BindingFlags = BindingFlags.Public Or BindingFlags.Instance Or BindingFlags.Static

                Dim Properties As PropertyInfo() = DataSource.GetType().GetProperties(Bindings)

                'Apply sort method
                Select Case SortMode
                    Case PropertySortType.Alphabetical
                        Array.Sort(Properties, New PropertyNameComparer)
                    Case PropertySortType.Category
                        Array.Sort(Properties, New PropertyCategoryComparer)
                    Case PropertySortType.SortOrderAttribute
                        Array.Sort(Properties, New PropertySortOrderComparer)
                End Select

                Return Properties
            Else
                Return Nothing
            End If

        End Function

#End Region

#Region "Protected Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' AddEditorRow builds a sigle editor row and adds it to the Table, using the
        ''' specified adapter
        ''' </summary>
        ''' <param name="tbl">The Table Control to add the row to</param>
        ''' <param name="name">The name of property being added</param>
        ''' <param name="adapter">An IEditorInfoAdapter</param>
        ''' <history>
        '''     [cnurse]	05/08/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Sub AddEditorRow(ByRef tbl As Table, ByVal name As String, ByVal adapter As IEditorInfoAdapter)

            Dim row As New TableRow
            tbl.Rows.Add(row)

            Dim cell As New TableCell
            row.Cells.Add(cell)

            'Create a FieldEditor for this Row
            Dim editor As New FieldEditorControl
            editor.DataSource = DataSource
            editor.EditorInfoAdapter = adapter
            editor.DataField = name
            editor.EditorDisplayMode = DisplayMode
            editor.EnableClientValidation = EnableClientValidation
            editor.EditMode = EditMode
            editor.HelpDisplayMode = HelpDisplayMode
            editor.LabelMode = LabelMode
            editor.LabelWidth = LabelWidth
            editor.LabelStyle.CopyFrom(LabelStyle)
            editor.HelpStyle.CopyFrom(HelpStyle)
            editor.ErrorStyle.CopyFrom(ErrorStyle)
            editor.VisibilityStyle.CopyFrom(VisibilityStyle)
            editor.EditControlStyle.CopyFrom(EditControlStyle)
            editor.EditControlWidth = EditControlWidth
            editor.LocalResourceFile = LocalResourceFile
            editor.RequiredUrl = RequiredUrl
            editor.ShowRequired = ShowRequired
            editor.ShowVisibility = ShowVisibility
            editor.Width = Width

            AddHandler editor.ItemAdded, AddressOf Me.CollectionItemAdded
            AddHandler editor.ItemChanged, AddressOf Me.ListItemChanged
            AddHandler editor.ItemCreated, AddressOf Me.EditorItemCreated
            AddHandler editor.ItemDeleted, AddressOf Me.CollectionItemDeleted

            editor.DataBind()
            Fields.Add(editor)
            cell.Controls.Add(editor)

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' AddEditorRow builds a sigle editor row and adds it to the Table
        ''' </summary>
        ''' <remarks>This method is protected so that classes that inherit from
        ''' PropertyEditor can modify how the Row is displayed</remarks>
        ''' <param name="tbl">The Table Control to add the row to</param>
        ''' <history>
        '''     [cnurse]	03/08/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overridable Sub AddEditorRow(ByRef tbl As Table, ByVal obj As Object)

            Dim objProperty As PropertyInfo = CType(obj, PropertyInfo)

            AddEditorRow(tbl, objProperty.Name, New StandardEditorInfoAdapter(DataSource, objProperty.Name()))

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' AddFields adds the fields that have beend defined in design mode (Autogenerate=false)
        ''' </summary>
        ''' <param name="tbl">The Table Control to add the row to</param>
        ''' <history>
        '''     [cnurse]	05/08/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overridable Sub AddFields(ByVal tbl As Table)
            For Each editor As FieldEditorControl In Fields
                Dim row As New TableRow
                tbl.Rows.Add(row)

                Dim cell As New TableCell
                row.Cells.Add(cell)

                editor.EditorDisplayMode = DisplayMode
                editor.EditorInfoAdapter = New StandardEditorInfoAdapter(DataSource, editor.DataField)
                editor.EnableClientValidation = EnableClientValidation
                If editor.EditMode <> PropertyEditorMode.View Then
                    editor.EditMode = EditMode
                End If
                editor.HelpDisplayMode = HelpDisplayMode
                If editor.LabelMode = LabelMode.None Then
                    editor.LabelMode = LabelMode
                End If
                If editor.LabelWidth = Unit.Empty Then
                    editor.LabelWidth = LabelWidth
                End If
                editor.LabelStyle.CopyFrom(LabelStyle)
                editor.HelpStyle.CopyFrom(HelpStyle)
                editor.ErrorStyle.CopyFrom(ErrorStyle)
                editor.VisibilityStyle.CopyFrom(VisibilityStyle)
                editor.EditControlStyle.CopyFrom(EditControlStyle)
                If editor.EditControlWidth = Unit.Empty Then
                    editor.EditControlWidth = EditControlWidth
                End If
                editor.LocalResourceFile = LocalResourceFile
                editor.RequiredUrl = RequiredUrl
                editor.ShowRequired = ShowRequired
                editor.ShowVisibility = ShowVisibility
                editor.Width = Width

                AddHandler editor.ItemAdded, AddressOf Me.CollectionItemAdded
                AddHandler editor.ItemChanged, AddressOf Me.ListItemChanged
                AddHandler editor.ItemCreated, AddressOf Me.EditorItemCreated
                AddHandler editor.ItemDeleted, AddressOf Me.CollectionItemDeleted

                editor.DataSource = DataSource
                editor.DataBind()

                cell.Controls.Add(editor)
            Next
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' AddHeader builds a group header
        ''' </summary>
        ''' <remarks>This method is protected so that classes that inherit from
        ''' PropertyEditor can modify how the Header is displayed</remarks>
        ''' <param name="tbl">The Table Control that contains the group</param>
        ''' <history>
        '''     [cnurse]	04/11/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overridable Sub AddHeader(ByRef tbl As Table, ByVal header As String)

            Dim panel As panel = New panel

            Dim icon As System.Web.UI.WebControls.Image = New System.Web.UI.WebControls.Image
            icon.ID = "ico" + header
            icon.EnableViewState = False

            Dim spacer As System.Web.UI.WebControls.Literal = New System.Web.UI.WebControls.Literal
            spacer.Text = " "
            spacer.EnableViewState = False

            Dim label As label = New label
            label.ID = "lbl" + header
            label.Attributes("resourcekey") = Me.ID + "_" + header + ".Header"
            label.Text = header
            label.EnableViewState = False
            label.ControlStyle.CopyFrom(GroupHeaderStyle)

            panel.Controls.Add(icon)
            panel.Controls.Add(spacer)
            panel.Controls.Add(label)

            If GroupHeaderIncludeRule Then
                panel.Controls.Add(New LiteralControl("<hr noshade=""noshade"" size=""1""/>"))
            End If

            Controls.Add(panel)

            'Get the Hashtable
            If _Sections Is Nothing Then
                _Sections = New Hashtable
            End If
            _Sections(icon) = tbl

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' CreateEditor creates the control collection.
        ''' </summary>
        ''' <history>
        '''     [cnurse]	02/14/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overridable Sub CreateEditor()

            Dim tbl As Table
            Dim obj As Object
            Dim strGroup As String
            Dim arrGroups(-1) As String

            Controls.Clear()

            If Groups.Length > 0 Then
                arrGroups = Groups.Split(","c)
            ElseIf GroupByMode <> GroupByMode.None Then
                arrGroups = GetGroups(UnderlyingDataSource)
            End If

            If Not AutoGenerate Then
                'Create a new table
                tbl = New Table
                tbl.ID = "tbl"

                AddFields(tbl)

                'Add the Table to the Controls Collection
                Controls.Add(tbl)
            Else
                Fields.Clear()
                If arrGroups.Length > 0 Then
                    For Each strGroup In arrGroups
                        If GroupByMode = UI.WebControls.GroupByMode.Section Then
                            'Create a new table
                            tbl = New Table
                            tbl.ID = "tbl" + strGroup

                            For Each obj In UnderlyingDataSource
                                If GetCategory(obj) = strGroup.Trim() Then
                                    'Add the Editor Row to the Table
                                    If GetRowVisibility(obj) Then
                                        If tbl.Rows.Count = 0 Then
                                            'Add a Header
                                            AddHeader(tbl, strGroup)
                                        End If
                                        AddEditorRow(tbl, obj)
                                    End If
                                End If
                            Next

                            'Add the Table to the Controls Collection (if it has any rows)
                            If tbl.Rows.Count > 0 Then
                                Controls.Add(tbl)
                            End If
                        End If
                    Next
                Else
                    'Create a new table
                    tbl = New Table
                    tbl.ID = "tbl"

                    For Each obj In UnderlyingDataSource
                        'Add the Editor Row to the Table
                        If GetRowVisibility(obj) Then
                            AddEditorRow(tbl, obj)
                        End If
                    Next


                    'Add the Table to the Controls Collection
                    Controls.Add(tbl)
                End If
            End If
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' GetCategory gets the Category of an object
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	05/08/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overridable Function GetCategory(ByVal obj As Object) As String

            Dim objProperty As PropertyInfo = CType(obj, PropertyInfo)
            Dim _Category As String = Null.NullString

            'Get Category Field
            Dim categoryAttributes As Object() = objProperty.GetCustomAttributes(GetType(CategoryAttribute), True)
            If (categoryAttributes.Length > 0) Then
                Dim category As CategoryAttribute = CType(categoryAttributes(0), CategoryAttribute)
                _Category = category.Category
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
        Protected Overridable Function GetGroups(ByVal arrObjects As IEnumerable) As String()

            Dim arrGroups As New ArrayList
            Dim strGroups(-1) As String

            For Each objProperty As PropertyInfo In arrObjects

                Dim categoryAttributes As Object() = objProperty.GetCustomAttributes(GetType(CategoryAttribute), True)
                If (categoryAttributes.Length > 0) Then
                    Dim category As CategoryAttribute = CType(categoryAttributes(0), CategoryAttribute)

                    If Not arrGroups.Contains(category.Category) Then
                        arrGroups.Add(category.Category)
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
        Protected Overridable Function GetRowVisibility(ByVal obj As Object) As Boolean

            Dim objProperty As PropertyInfo = CType(obj, PropertyInfo)

            Dim isVisible As Boolean = True
            Dim browsableAttributes As Object() = objProperty.GetCustomAttributes(GetType(BrowsableAttribute), True)
            If (browsableAttributes.Length > 0) Then
                Dim browsable As BrowsableAttribute = CType(browsableAttributes(0), BrowsableAttribute)
                If Not browsable.Browsable Then
                    isVisible = False
                End If
            End If

            If Not isVisible AndAlso EditMode = PropertyEditorMode.Edit Then
                'Check if property is required - as this will need to override visibility
                Dim requiredAttributes As Object() = objProperty.GetCustomAttributes(GetType(RequiredAttribute), True)
                If (requiredAttributes.Length > 0) Then
                    Dim required As RequiredAttribute = CType(requiredAttributes(0), RequiredAttribute)
                    If required.Required Then
                        isVisible = True
                    End If
                End If
            End If

            Return isVisible

        End Function

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
        ''' Runs when an Editor is Created
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
        ''' Runs just before the control is rendered
        ''' </summary>
        ''' <history>
        '''     [cnurse]	04/11/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overrides Sub OnPreRender(ByVal e As System.EventArgs)

            If _ItemChanged Then
                'Rebind the control to the DataSource to make sure that the dependent
                'editors are updated
                DataBind()
            End If

            'Find the Min/Max buttons
            If GroupByMode = UI.WebControls.GroupByMode.Section And (Not _Sections Is Nothing) Then
                For Each key As DictionaryEntry In _Sections
                    Dim tbl As Table = DirectCast(key.Value, Table)
                    Dim icon As System.Web.UI.WebControls.Image = DirectCast(key.Key, System.Web.UI.WebControls.Image)
                    DotNetNuke.UI.Utilities.DNNClientAPI.EnableMinMax(icon, tbl, False, Page.ResolveUrl("~/images/minus.gif"), Page.ResolveUrl("~/images/plus.gif"), Utilities.DNNClientAPI.MinMaxPersistanceType.Page)
                Next
            End If
            MyBase.OnPreRender(e)
        End Sub

#End Region

#Region "Public Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Binds the controls to the DataSource
        ''' </summary>
        ''' <history>
        '''     [cnurse]	03/06/2006	created
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

            'Create the Editor
            CreateEditor()

            'Set flag so CreateChildConrols should not be invoked later in control's lifecycle
            ChildControlsCreated = True

        End Sub

#End Region

#Region "Event Handlers"

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
        ''' Runs when an Editor Is Created
        ''' </summary>
        ''' <history>
        '''     [cnurse]	02/20/2007	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overridable Sub EditorItemCreated(ByVal sender As Object, ByVal e As PropertyEditorItemEventArgs)
            OnItemCreated(e)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Runs when an Item in the List Is Changed
        ''' </summary>
        ''' <history>
        '''     [cnurse]	02/15/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overridable Sub ListItemChanged(ByVal sender As Object, ByVal e As PropertyEditorEventArgs)
            _ItemChanged = True
        End Sub

#End Region

    End Class

End Namespace

