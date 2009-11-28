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
Imports System.Web.UI

Namespace DotNetNuke.UI.WebControls

    ''' -----------------------------------------------------------------------------
    ''' Project:    DotNetNuke
    ''' Namespace:  DotNetNuke.UI.WebControls
    ''' Class:      PropertyLabelControl
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The PropertyLabelControl control provides a standard UI component for displaying
    ''' a label for a property. It contains a Label and Help Text and can be Data Bound.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    '''     [cnurse]	02/14/2006	created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    <ToolboxData("<{0}:PropertyLabelControl runat=server></{0}:PropertyLabelControl>")> _
    Public Class PropertyLabelControl
        Inherits System.Web.UI.WebControls.WebControl

#Region "Controls"

        'Label container <label>
        Protected WithEvents label As System.Web.UI.HtmlControls.HtmlGenericControl

        'Label Help icon
        Protected WithEvents cmdHelp As System.Web.UI.WebControls.LinkButton
        Protected WithEvents imgHelp As System.Web.UI.WebControls.Image

        'Label Text
        Protected WithEvents lblLabel As System.Web.UI.WebControls.Label

        'Help
        Protected WithEvents pnlHelp As System.Web.UI.WebControls.Panel
        Protected WithEvents lblHelp As System.Web.UI.WebControls.Label

#End Region

#Region "Private Members"

        'Edit Control
        Private _EditControl As Control

        'Localization
        Private _ResourceKey As String

        'Data
        Private _DataSource As Object
        Private _DataField As String

        'Styles
        Private _HelpStyle As Style = New Style
        Private _LabelStyle As Style = New Style

#End Region

#Region "Protected Members"

        Protected Overrides ReadOnly Property TagKey() As System.Web.UI.HtmlTextWriterTag
            Get
                Return HtmlTextWriterTag.Div
            End Get
        End Property

#End Region

#Region "Public Properties"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and Sets the Caption Text if no ResourceKey is provided
        ''' </summary>
        ''' <value>A string representing the Caption</value>
        ''' <history>
        ''' 	[cnurse]	02/13/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <Browsable(True), Category("Appearance"), DefaultValue("Property"), _
            Description("Enter Caption for the control.")> _
        Public Property Caption() As String
            Get
                Me.EnsureChildControls()
                Return lblLabel.Text
            End Get
            Set(ByVal Value As String)
                Me.EnsureChildControls()
                lblLabel.Text = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and Sets the related Edit Control
        ''' </summary>
        ''' <value>A Control</value>
        ''' <history>
        ''' 	[cnurse]	02/14/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <Browsable(False)> _
        Public Property EditControl() As Control
            Get
                Return _EditControl
            End Get
            Set(ByVal Value As Control)
                _EditControl = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Text is value of the Label Text if no ResourceKey is provided
        ''' </summary>
        ''' <value>A string representing the Text</value>
        ''' <history>
        ''' 	[cnurse]	02/13/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <Browsable(True), Category("Appearance"), DefaultValue(""), _
            Description("Enter Help Text for the control.")> _
        Public Property HelpText() As String
            Get
                Me.EnsureChildControls()
                Return lblHelp.Text
            End Get
            Set(ByVal Value As String)
                Me.EnsureChildControls()
                lblHelp.Text = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' ResourceKey is the root localization key for this control
        ''' </summary>
        ''' <value>A string representing the Resource Key</value>
        ''' <remarks>This control will "standardise" the resource key names, so for instance
        ''' if the resource key is "Control", Control.Text is the label text key, Control.Help
        ''' is the label help text, Control.ErrorMessage is the Validation Error Message for the
        ''' control
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	02/10/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <Browsable(True), Category("Localization"), DefaultValue(""), _
            Description("Enter the Resource key for the control.")> _
        Public Property ResourceKey() As String
            Get
                Return _ResourceKey
            End Get
            Set(ByVal Value As String)
                _ResourceKey = Value

                Me.EnsureChildControls()

                'Localize the Label and the Help text
                lblHelp.Attributes("resourcekey") = _ResourceKey + ".Help"
                imgHelp.Attributes("resourcekey") = _ResourceKey + ".Help"
                lblLabel.Attributes("resourcekey") = _ResourceKey + ".Text"

            End Set
        End Property

        <Browsable(True), Category("Behavior"), DefaultValue(False), _
            Description("Set whether the Help icon is displayed.")> _
        Public Property ShowHelp() As Boolean
            Get
                Me.EnsureChildControls()
                Return cmdHelp.Visible
            End Get
            Set(ByVal Value As Boolean)
                Me.EnsureChildControls()
                cmdHelp.Visible = Value
            End Set
        End Property


#Region "Data Properties"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the value of the Field that is bound to the Label
        ''' </summary>
        ''' <value>A string representing the Name of the Field</value>
        ''' <history>
        ''' 	[cnurse]	02/13/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <Browsable(True), Category("Data"), DefaultValue(""), _
            Description("Enter the name of the field that is data bound to the Label's Text property.")> _
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
        ''' Gets and sets the DataSource that is bound to this control
        ''' </summary>
        ''' <value>The DataSource object</value>
        ''' <history>
        ''' 	[cnurse]	02/13/2006	Created
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

#End Region

#Region "Style Properties"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the value of the Label Style
        ''' </summary>
        ''' <value>A string representing the Name of the Field</value>
        ''' <history>
        ''' 	[cnurse]	02/13/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <Browsable(True), Category("Styles"), _
            DesignerSerializationVisibility(DesignerSerializationVisibility.Content), _
            TypeConverter(GetType(ExpandableObjectConverter)), _
            Description("Set the Style for the Help Text.")> _
        Public ReadOnly Property HelpStyle() As System.Web.UI.WebControls.Style
            Get
                Me.EnsureChildControls()
                Return pnlHelp.ControlStyle
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the value of the Label Style
        ''' </summary>
        ''' <value>A string representing the Name of the Field</value>
        ''' <history>
        ''' 	[cnurse]	02/13/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <Browsable(True), Category("Styles"), _
            DesignerSerializationVisibility(DesignerSerializationVisibility.Content), _
            TypeConverter(GetType(ExpandableObjectConverter)), _
            Description("Set the Style for the Label Text")> _
        Public ReadOnly Property LabelStyle() As System.Web.UI.WebControls.Style
            Get
                Me.EnsureChildControls()
                Return lblLabel.ControlStyle
            End Get
        End Property

#End Region

#End Region

#Region "Protected Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' CreateChildControls creates the control collection.
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	02/10/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overrides Sub CreateChildControls()

            'Initialise the Label container
            label = New HtmlGenericControl
            label.TagName = "label"

            If Not DesignMode Then
                'Initialise Help LinkButton
                cmdHelp = New LinkButton
                cmdHelp.ID = Me.ID + "_cmdHelp"
                cmdHelp.CausesValidation = False
                cmdHelp.EnableViewState = False
                cmdHelp.TabIndex = -1

                'Initialise Help Image and add to Help LinkButton
                imgHelp = New System.Web.UI.WebControls.Image
                imgHelp.ID = Me.ID + "_imgHelp"
                imgHelp.EnableViewState = False
                imgHelp.ImageUrl = "~/images/help.gif"
                cmdHelp.Controls.Add(imgHelp)

                'Add Help LinkButton to Label container
                label.Controls.Add(cmdHelp)

                label.Controls.Add(New LiteralControl("&nbsp;"))
            End If

            'Initialise Label
            lblLabel = New label
            lblLabel.ID = Me.ID + "_label"
            lblLabel.EnableViewState = False
            label.Controls.Add(lblLabel)

            'Initialise Help Panel
            pnlHelp = New Panel
            pnlHelp.ID = Me.ID + "_pnlHelp"
            pnlHelp.EnableViewState = False

            'Initialise Help Label
            lblHelp = New label
            lblHelp.ID = Me.ID + "_lblHelp"
            lblHelp.EnableViewState = False
            pnlHelp.Controls.Add(lblHelp)

            Me.Controls.Add(label)
            Me.Controls.Add(pnlHelp)

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' OnDataBinding runs when the Control is being Data Bound (It is triggered by
        ''' a call to Control.DataBind()
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	02/13/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overrides Sub OnDataBinding(ByVal e As System.EventArgs)

            'If there is a DataSource bind the relevent Properties
            If Not DataSource Is Nothing Then
                'Make sure the Child Controls are created before assigning any properties
                Me.EnsureChildControls()

                If DataField <> "" Then
                    'DataBind the Label (via the Resource Key)
                    Dim dataRow As DataRowView = CType(DataSource, DataRowView)
                    If ResourceKey = String.Empty Then
                        ResourceKey = CType(dataRow(DataField), String)
                    End If
                    If DesignMode Then
                        label.InnerText = CType(dataRow(DataField), String)
                    End If
                End If
            End If

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' OnLoad runs just before the Control is rendered, and makes sure that any
        ''' properties are set properly before the control is rendered
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	02/13/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overrides Sub OnPreRender(ByVal e As System.EventArgs)

            'Make sure the Child Controls are created before assigning any properties
            Me.EnsureChildControls()

            'Set up client-side script
            DotNetNuke.UI.Utilities.DNNClientAPI.EnableMinMax(cmdHelp, pnlHelp, True, Utilities.DNNClientAPI.MinMaxPersistanceType.None)

            If Not EditControl Is Nothing Then
                label.Attributes.Add("for", EditControl.ClientID)
            End If

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Render is called by the .NET framework to render the control
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	02/10/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overrides Sub Render(ByVal writer As System.Web.UI.HtmlTextWriter)

            MyBase.Render(writer)

        End Sub

#End Region

    End Class

End Namespace

