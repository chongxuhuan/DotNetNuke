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

Imports System
Imports System.Drawing
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.IO
Imports System.Collections
Imports System.Data


Namespace DotNetNuke.UI.WebControls

    ''' -----------------------------------------------------------------------------
    ''' Project:    DotNetNuke
    ''' Namespace:  DotNetNuke.UI.WebControls
    ''' Class:      CheckBoxColumn
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The CheckBoxColumn control provides a Check Box column for a Data Grid
    ''' </summary>
    ''' <history>
    '''     [cnurse]	02/16/2006	created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class CheckBoxColumn
        Inherits System.Web.UI.WebControls.TemplateColumn

#Region "Private Members"

        Private mAutoPostBack As Boolean = True
        Private mChecked As Boolean = False
        Private mDataField As String = Null.NullString
        Private mEnabled As Boolean = True
        Private mEnabledField As String = Null.NullString
        Private mHeaderCheckBox As Boolean = True

#End Region

#Region "Constructors"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Constructs the CheckBoxColumn
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	02/16/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub New()
            Me.New(False)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Constructs the CheckBoxColumn, with an optional AutoPostBack (where each change
        ''' of state of a check box causes a Post Back)
        ''' </summary>
        ''' <param name="autoPostBack">Optional set the checkboxes to postback</param>
        ''' <history>
        ''' 	[cnurse]	02/16/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub New(ByVal autoPostBack As Boolean)

            Me.AutoPostBack = autoPostBack

        End Sub

#End Region

#Region "Events"

        Public Event CheckedChanged As DNNDataGridCheckedColumnEventHandler

#End Region

#Region "Public Properties"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets whether the column fires a postback when any check box is 
        ''' changed
        ''' </summary>
        ''' <value>A Boolean</value>
        ''' <history>
        ''' 	[cnurse]	02/16/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property AutoPostBack() As Boolean
            Get
                Return mAutoPostBack
            End Get
            Set(ByVal Value As Boolean)
                mAutoPostBack = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets whether the checkbox is checked (unless DataBound) 
        ''' </summary>
        ''' <value>A Boolean</value>
        ''' <history>
        ''' 	[cnurse]	02/21/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property Checked() As Boolean
            Get
                Return mChecked
            End Get
            Set(ByVal Value As Boolean)
                mChecked = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The Data Field that the column should bind to
        ''' changed
        ''' </summary>
        ''' <value>A Boolean</value>
        ''' <history>
        ''' 	[cnurse]	02/16/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property DataField() As String
            Get
                Return mDataField
            End Get
            Set(ByVal Value As String)
                mDataField = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' An flag that indicates whether the checkboxes are enabled (this is overridden if
        ''' the EnabledField is set)
        ''' changed
        ''' </summary>
        ''' <value>A Boolean</value>
        ''' <history>
        ''' 	[cnurse]	02/16/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property Enabled() As Boolean
            Get
                Return mEnabled
            End Get
            Set(ByVal Value As Boolean)
                mEnabled = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The Data Field that determines whether the checkbox is Enabled
        ''' changed
        ''' </summary>
        ''' <value>A String</value>
        ''' <history>
        ''' 	[cnurse]	02/16/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property EnabledField() As String
            Get
                Return mEnabledField
            End Get
            Set(ByVal Value As String)
                mEnabledField = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' A flag that indicates whether there is a checkbox in the Header that sets all
        ''' the checkboxes
        ''' </summary>
        ''' <value>A Boolean</value>
        ''' <history>
        ''' 	[cnurse]	07/05/2007	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property HeaderCheckBox() As Boolean
            Get
                Return mHeaderCheckBox
            End Get
            Set(ByVal Value As Boolean)
                mHeaderCheckBox = Value
            End Set
        End Property

#End Region

#Region "Private Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Creates a CheckBoxColumnTemplate
        ''' </summary>
        ''' <returns>A CheckBoxColumnTemplate</returns>
        ''' <history>
        '''     [cnurse]	02/16/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Function CreateTemplate(ByVal type As ListItemType) As CheckBoxColumnTemplate

            Dim isDesignMode As Boolean = False
            If HttpContext.Current Is Nothing Then
                isDesignMode = True
            End If

            Dim template As CheckBoxColumnTemplate = New CheckBoxColumnTemplate(type)
            If type <> ListItemType.Header Then
                template.AutoPostBack = AutoPostBack
            End If
            template.Checked = Checked
            template.DataField = DataField
            template.Enabled = Enabled
            template.EnabledField = EnabledField

            AddHandler template.CheckedChanged, AddressOf OnCheckedChanged
            If type = ListItemType.Header Then
                template.Text = HeaderText
                template.AutoPostBack = True
                template.HeaderCheckBox = HeaderCheckBox
            End If

            template.DesignMode = isDesignMode

            Return template

        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Centralised Event that is raised whenever a check box is changed.
        ''' </summary>
        ''' <history>
        '''     [cnurse]	02/16/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub OnCheckedChanged(ByVal sender As Object, ByVal e As DNNDataGridCheckChangedEventArgs)

            'Add the column to the Event Args
            e.Column = Me

            RaiseEvent CheckedChanged(sender, e)

        End Sub

#End Region

#Region "Public Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Initialises the Column
        ''' </summary>
        ''' <history>
        '''     [cnurse]	02/16/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overrides Sub Initialize()

            MyClass.ItemTemplate = CreateTemplate(ListItemType.Item)
            MyClass.EditItemTemplate = CreateTemplate(ListItemType.EditItem)
            MyClass.HeaderTemplate = CreateTemplate(ListItemType.Header)

            If HttpContext.Current Is Nothing Then
                Me.HeaderStyle.Font.Names = New String() {"Tahoma, Verdana, Arial"}
                Me.HeaderStyle.Font.Size = New FontUnit("10pt")
                Me.HeaderStyle.Font.Bold = True
            End If

            MyClass.ItemStyle.HorizontalAlign = HorizontalAlign.Center
            MyClass.HeaderStyle.HorizontalAlign = HorizontalAlign.Center

        End Sub

#End Region

    End Class


End Namespace