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
    ''' Class:      DNNMultiStateBoxColumn
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The DNNMultiStateBoxColumn control provides a DNNMultiState Box column for a Data Grid
    ''' </summary>
    ''' <history>
    '''     [cnurse]	02/16/2006	created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class DNNMultiStateBoxColumn
        Inherits System.Web.UI.WebControls.TemplateColumn

#Region "Private Members"

        Private mAutoPostBack As Boolean = True
        Private mSelectedStateKey As String = ""
        Private mDataField As String = Null.NullString
        Private mEnabled As Boolean = True
        Private mEnabledField As String = Null.NullString
        Private mImagePath As String = ""
        Private mStates As DNNMultiStateCollection = Nothing
#End Region

#Region "Constructors"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Constructs the DNNMultiStateBoxColumn
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
        ''' Constructs the MultiStateBoxColumn, with an optional AutoPostBack (where each change
        ''' of state of the control causes a Post Back)
        ''' </summary>
        ''' <param name="autoPostBack">Optional set the control to postback</param>
        ''' <history>
        ''' 	[cnurse]	02/16/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub New(ByVal autoPostBack As Boolean)

            Me.AutoPostBack = autoPostBack

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
        ''' An flag that indicates whether the control is enabled (this is overridden if
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

#Region "Private Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Creates a DNNMultiStateBoxColumnTemplate
        ''' </summary>
        ''' <returns>A DNNMultiStateBoxColumnTemplate</returns>
        ''' <history>
        '''     [cnurse]	02/16/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Function CreateTemplate(ByVal type As ListItemType) As DNNMultiStateBoxColumnTemplate

            Dim isDesignMode As Boolean = False
            If HttpContext.Current Is Nothing Then
                isDesignMode = True
            End If

            Dim template As DNNMultiStateBoxColumnTemplate = New DNNMultiStateBoxColumnTemplate(type)
            If type <> ListItemType.Header Then
                template.AutoPostBack = AutoPostBack
            End If
            template.DataField = DataField
            template.Enabled = Enabled
            template.EnabledField = EnabledField
            template.ImagePath = ImagePath
            For Each objState As DNNMultiState In States
                template.States.Add(objState)
            Next
            template.SelectedStateKey = SelectedStateKey

            If type = ListItemType.Header Then
                template.Text = HeaderText
                template.AutoPostBack = True
            End If

            template.DesignMode = isDesignMode

            Return template

        End Function

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