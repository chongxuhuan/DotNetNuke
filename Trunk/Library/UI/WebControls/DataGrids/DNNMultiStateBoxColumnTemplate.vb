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
    ''' Class:      DNNMultiStateBoxColumnTemplate
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The DNNMultiStateBoxColumnTemplate provides a Template for the DNNMultiStateBoxColumn
    ''' </summary>
    ''' <history>
    '''     [cnurse]	02/16/2006	created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class DNNMultiStateBoxColumnTemplate
        Implements ITemplate

#Region "Private Members"

        Private mAutoPostBack As Boolean = False
        Private mSelectedStateKey As String = ""
        Private mDataField As String = Null.NullString
        Private mDesignMode As Boolean
        Private mEnabled As Boolean = True
        Private mEnabledField As String = Null.NullString
        Private mItemType As ListItemType = ListItemType.Item
        Private mText As String = ""
        Private mImagePath As String = ""
        Private mStates As DNNMultiStateCollection = Nothing

#End Region

#Region "Constructors"

        Public Sub New()
            Me.New(ListItemType.Item)
        End Sub

        Public Sub New(ByVal itemType As ListItemType)
            Me.ItemType = itemType
        End Sub

#End Region

#Region "Public Properties"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets whether the column fires a postback when the control changes
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
        ''' Gets and sets the selected state of the DNNMultiStateBox (unless DataBound) 
        ''' </summary>
        ''' <value>A Boolean</value>
        ''' <history>
        ''' 	[cnurse]	02/21/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property SelectedStateKey() As String
            Get
                Return mSelectedStateKey
            End Get
            Set(ByVal Value As String)
                mSelectedStateKey = Value
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
        ''' An flag that indicates whether the control is enabled (this is overridden if
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
        ''' The Data Field that determines whether the control is Enabled
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

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the image path of the DNNMultiStateBox 
        ''' </summary>
        ''' <value>A Boolean</value>
        ''' <history>
        ''' 	[cnurse]	02/21/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property ImagePath() As String
            Get
                Return mImagePath
            End Get
            Set(ByVal Value As String)
                mImagePath = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the state collection of the DNNMultiStateBox 
        ''' </summary>
        ''' <value>A Boolean</value>
        ''' <history>
        ''' 	[cnurse]	02/21/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property States() As DNNMultiStateCollection
            Get
                If mStates Is Nothing Then
                    mStates = New DNNMultiStateCollection(New DNNMultiStateBox)
                End If
                Return mStates
            End Get
            Set(ByVal Value As DNNMultiStateCollection)
                mStates = Value
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

        Public Sub InstantiateIn(ByVal container As Control) Implements ITemplate.InstantiateIn

            If Text <> "" Then
                container.Controls.Add(New LiteralControl(Text + "<br/>"))
            End If

            If ItemType <> ListItemType.Header Then
                Dim box As DNNMultiStateBox = New DNNMultiStateBox()
                box.AutoPostBack = AutoPostBack
                box.ImagePath = ImagePath
                For Each objState As DNNMultiState In States
                    box.States.Add(objState)
                Next

                AddHandler box.DataBinding, AddressOf Item_DataBinding

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

            Dim box As DNNMultiStateBox = CType(sender, DNNMultiStateBox)
            Dim container As DataGridItem = CType(box.NamingContainer, DataGridItem)

            If DataField <> "" And ItemType <> ListItemType.Header Then
                If DesignMode Then
                    box.SelectedStateKey = ""
                Else
                    box.SelectedStateKey = CType(DataBinder.Eval(container.DataItem, DataField), String)
                End If
            Else
                box.SelectedStateKey = Me.SelectedStateKey
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

#End Region

    End Class

End Namespace