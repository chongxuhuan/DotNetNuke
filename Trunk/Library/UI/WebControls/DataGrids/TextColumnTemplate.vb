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
    ''' Class:      TextColumnTemplate
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The TextColumnTemplate provides a Template for the TextColumn
    ''' </summary>
    ''' <history>
    '''     [cnurse]	02/20/2006	created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class TextColumnTemplate
        Implements ITemplate

#Region "Private Members"

        Private mItemType As ListItemType = ListItemType.Item
        Private mDataField As String
        Private mDesignMode As Boolean
        Private mText As String
        Private mWidth As Unit

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
        ''' The Data Field is the field that binds to the Text Column
        ''' </summary>
        ''' <value>A String</value>
        ''' <history>
        ''' 	[cnurse]	02/20/2006	Created
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
        ''' The type of Template to Create
        ''' </summary>
        ''' <value>A String</value>
        ''' <history>
        ''' 	[cnurse]	02/20/2006	Created
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
        ''' Gets or sets the Text (for Header/Footer Templates)
        ''' </summary>
        ''' <value>A String</value>
        ''' <history>
        ''' 	[cnurse]	02/20/2006	Created
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
        ''' Gets or sets the Width of the Column
        ''' </summary>
        ''' <value>A Unit</value>
        ''' <history>
        ''' 	[cnurse]	02/20/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property Width() As Unit
            Get
                Return mWidth
            End Get
            Set(ByVal Value As Unit)
                mWidth = Value
            End Set
        End Property


#End Region

#Region "Private Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the value of the Data Field
        ''' </summary>
        '''	<param name="container">The parent container (DataGridItem)</param>
        ''' <history>
        ''' 	[cnurse]	02/20/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Function GetValue(ByVal container As DataGridItem) As String

            Dim itemValue As String = Null.NullString
            If DataField <> "" Then
                If DesignMode Then
                    itemValue = "DataBound to " & DataField
                Else
                    If Not container.DataItem Is Nothing Then
                        Dim evaluation As Object = DataBinder.Eval(container.DataItem, DataField)
                        If (Not evaluation Is Nothing) Then
                            itemValue = evaluation.ToString()
                        End If
                    End If
                End If
            End If

            Return itemValue

        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Item_DataBinding runs when an Item of type ListItemType.Item is being data-bound
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <param name="sender"> The object that triggers the event</param>
        ''' <param name="e">An EventArgs object</param>
        ''' <history>
        ''' 	[cnurse]	02/16/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub Item_DataBinding(ByVal sender As Object, ByVal e As System.EventArgs)
            Dim container As DataGridItem
            Dim keyValue As Integer = Null.NullInteger

            Select Case ItemType
                Case ListItemType.Item, ListItemType.AlternatingItem, ListItemType.SelectedItem
                    Dim lblText As Label = CType(sender, Label)
                    container = CType(lblText.NamingContainer, DataGridItem)
                    lblText.Text = GetValue(container)
                Case ListItemType.EditItem
                    Dim txtText As TextBox = CType(sender, TextBox)
                    container = CType(txtText.NamingContainer, DataGridItem)
                    txtText.Text = GetValue(container)
            End Select
        End Sub

#End Region

#Region "Public Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' InstantiateIn instantiates the template (implementation of ITemplate)
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        '''	<param name="container">The parent container (DataGridItem)</param>
        ''' <history>
        ''' 	[cnurse]	02/16/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub InstantiateIn(ByVal container As Control) Implements ITemplate.InstantiateIn

            Select Case ItemType
                Case ListItemType.Item, ListItemType.AlternatingItem, ListItemType.SelectedItem
                    'Add a Text Label
                    Dim lblText As New Label
                    lblText.Width = Width
                    AddHandler lblText.DataBinding, AddressOf Item_DataBinding
                    container.Controls.Add(lblText)
                Case ListItemType.EditItem
                    'Add a Text Box
                    Dim txtText As New TextBox
                    txtText.Width = Width
                    AddHandler txtText.DataBinding, AddressOf Item_DataBinding
                    container.Controls.Add(txtText)
                Case ListItemType.Footer, ListItemType.Header
                    container.Controls.Add(New LiteralControl(Text))
            End Select

        End Sub

#End Region

    End Class

End Namespace