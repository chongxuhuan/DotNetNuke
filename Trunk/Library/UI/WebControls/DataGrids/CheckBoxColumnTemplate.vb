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
    ''' Class:      CheckBoxColumnTemplate
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The CheckBoxColumnTemplate provides a Template for the CheckBoxColumn
    ''' </summary>
    ''' <history>
    '''     [cnurse]	02/16/2006	created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class CheckBoxColumnTemplate
        Implements ITemplate

#Region "Private Members"

        Private mAutoPostBack As Boolean = False
        Private mChecked As Boolean = False
        Private mDataField As String = Null.NullString
        Private mDesignMode As Boolean
        Private mEnabled As Boolean = True
        Private mEnabledField As String = Null.NullString
        Private mHeaderCheckBox As Boolean = True
        Private mItemType As ListItemType = ListItemType.Item
        Private mText As String = ""

#End Region

#Region "Constructors"

        Public Sub New()
            Me.New(ListItemType.Item)
        End Sub

        Public Sub New(ByVal itemType As ListItemType)
            Me.ItemType = itemType
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
        ''' </summary>
        ''' <value>A String</value>
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
        ''' Gets or sets the Design Mode of the Column
        ''' </summary>
        ''' <value>A Boolean</value>
        ''' <history>
        ''' 	[cnurse]	02/24/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property DesignMode() As Boolean
            Get
                Return mDesignMode
            End Get
            Set(ByVal Value As Boolean)
                mDesignMode = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' An flag that indicates whether the hcekboxes are enabled (this is overridden if
        ''' the EnabledField is set
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

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The type of Template to Create
        ''' </summary>
        ''' <value>A String</value>
        ''' <history>
        ''' 	[cnurse]	02/16/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property ItemType() As ListItemType
            Get
                Return mItemType
            End Get
            Set(ByVal Value As ListItemType)
                mItemType = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The Text to display in a Header Template
        ''' </summary>
        ''' <value>A String</value>
        ''' <history>
        ''' 	[cnurse]	02/16/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property Text() As String
            Get
                Return mText
            End Get
            Set(ByVal Value As String)
                mText = Value
            End Set
        End Property

#End Region

#Region "Public Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' InstantiateIn is called when the Template is instantiated by the parent control
        ''' </summary>
        ''' <param name="container">The container control</param>
        ''' <history>
        '''     [cnurse]	02/16/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub InstantiateIn(ByVal container As Control) Implements ITemplate.InstantiateIn

            If Text <> "" Then
                container.Controls.Add(New LiteralControl(Text + "<br/>"))
            End If

            If ItemType <> ListItemType.Header OrElse (ItemType = ListItemType.Header AndAlso HeaderCheckBox) Then
                Dim box As New CheckBox
                box.AutoPostBack = AutoPostBack

                AddHandler box.DataBinding, AddressOf Item_DataBinding
                AddHandler box.CheckedChanged, AddressOf OnCheckChanged

                container.Controls.Add(box)
            End If
        End Sub

#End Region

#Region "Private Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Called when the template item is Data Bound
        ''' </summary>
        ''' <history>
        '''     [cnurse]	02/14/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub Item_DataBinding(ByVal sender As Object, ByVal e As EventArgs)

            Dim box As CheckBox = CType(sender, CheckBox)
            Dim container As DataGridItem = CType(box.NamingContainer, DataGridItem)

            If DataField <> "" And ItemType <> ListItemType.Header Then
                If DesignMode Then
                    box.Checked = False
                Else
                    box.Checked = CType(DataBinder.Eval(container.DataItem, DataField), Boolean)
                End If
            Else
                box.Checked = Me.Checked
            End If

            If EnabledField <> "" Then
                If DesignMode Then
                    box.Enabled = False
                Else
                    box.Enabled = CType(DataBinder.Eval(container.DataItem, EnabledField), Boolean)
                End If
            Else
                box.Enabled = Me.Enabled
            End If

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Centralised Event that is raised whenever a check box's state is modified
        ''' </summary>
        ''' <history>
        '''     [cnurse]	02/14/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub OnCheckChanged(ByVal sender As Object, ByVal e As EventArgs)

            Dim box As CheckBox = CType(sender, CheckBox)
            Dim container As DataGridItem = CType(box.NamingContainer, DataGridItem)
            Dim evntArgs As DNNDataGridCheckChangedEventArgs

            If container.ItemIndex = Null.NullInteger Then
                evntArgs = New DNNDataGridCheckChangedEventArgs(container, box.Checked, DataField, True)
            Else
                evntArgs = New DNNDataGridCheckChangedEventArgs(container, box.Checked, DataField, False)
            End If

            RaiseEvent CheckedChanged(sender, evntArgs)

        End Sub

#End Region

    End Class

End Namespace