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

Imports DotNetNuke.UI.Utilities

Namespace DotNetNuke.UI.WebControls

    ''' -----------------------------------------------------------------------------
    ''' Project:    DotNetNuke
    ''' Namespace:  DotNetNuke.UI.WebControls
    ''' Class:      ImageCommandColumnTemplate
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The ImageCommandColumnTemplate provides a Template for the ImageCommandColumn
    ''' </summary>
    ''' <history>
    '''     [cnurse]	02/16/2006	created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class ImageCommandColumnTemplate
        Implements ITemplate

#Region "Private Members"

        Private mCommandName As String
        Private mDesignMode As Boolean
        Private mEditMode As ImageCommandColumnEditMode = ImageCommandColumnEditMode.Command
        Private mImageURL As String
        Private mItemType As ListItemType = ListItemType.Item
        Private mKeyField As String
        Private mNavigateURL As String
        Private mNavigateURLFormatString As String
        Private mOnClickJS As String
        Private mShowImage As Boolean = True
        Private mText As String
        Private mVisible As Boolean = True
        Private mVisibleField As String

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
        ''' Gets or sets the CommandName for the Column
        ''' </summary>
        ''' <value>A String</value>
        ''' <history>
        ''' 	[cnurse]	02/17/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property CommandName() As String
            Get
                Return mCommandName
            End Get
            Set(ByVal Value As String)
                mCommandName = Value
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
        ''' Gets or sets the CommandName for the Column
        ''' </summary>
        ''' <value>A String</value>
        ''' <history>
        ''' 	[cnurse]	02/17/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property EditMode() As ImageCommandColumnEditMode
            Get
                Return mEditMode
            End Get
            Set(ByVal Value As ImageCommandColumnEditMode)
                mEditMode = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets or sets the URL of the Image
        ''' </summary>
        ''' <value>A String</value>
        ''' <history>
        ''' 	[cnurse]	02/17/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property ImageURL() As String
            Get
                Return mImageURL
            End Get
            Set(ByVal Value As String)
                mImageURL = Value
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
        ''' The Key Field that provides a Unique key to the data Item
        ''' </summary>
        ''' <value>A String</value>
        ''' <history>
        ''' 	[cnurse]	02/16/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property KeyField() As String
            Get
                Return mKeyField
            End Get
            Set(ByVal Value As String)
                mKeyField = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets or sets the URL of the Link (unless DataBinding through KeyField)
        ''' </summary>
        ''' <value>A String</value>
        ''' <history>
        ''' 	[cnurse]	02/17/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property NavigateURL() As String
            Get
                Return mNavigateURL
            End Get
            Set(ByVal Value As String)
                mNavigateURL = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets or sets the URL Formatting string
        ''' </summary>
        ''' <value>A String</value>
        ''' <history>
        ''' 	[cnurse]	01/06/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property NavigateURLFormatString() As String
            Get
                Return mNavigateURLFormatString
            End Get
            Set(ByVal Value As String)
                mNavigateURLFormatString = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Javascript text to attach to the OnClick Event
        ''' </summary>
        ''' <value>A String</value>
        ''' <history>
        ''' 	[cnurse]	02/16/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property OnClickJS() As String
            Get
                Return mOnClickJS
            End Get
            Set(ByVal Value As String)
                mOnClickJS = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets or sets whether an Image is displayed
        ''' </summary>
        ''' <remarks>Defaults to True</remarks>
        ''' <value>A Boolean</value>
        ''' <history>
        ''' 	[cnurse]	02/16/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property ShowImage() As Boolean
            Get
                Return mShowImage
            End Get
            Set(ByVal Value As Boolean)
                mShowImage = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets or sets the Text (for Header/Footer Templates)
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
        ''' An flag that indicates whether the buttons are visible (this is overridden if
        ''' the VisibleField is set)
        ''' changed
        ''' </summary>
        ''' <value>A Boolean</value>
        ''' <history>
        ''' 	[cnurse]	02/20/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property Visible() As Boolean
            Get
                Return mVisible
            End Get
            Set(ByVal Value As Boolean)
                mVisible = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' An flag that indicates whether the buttons are visible.
        ''' </summary>
        ''' <value>A Boolean</value>
        ''' <history>
        ''' 	[cnurse]	02/20/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property VisibleField() As String
            Get
                Return mVisibleField
            End Get
            Set(ByVal Value As String)
                mVisibleField = Value
            End Set
        End Property


#End Region

#Region "Private Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets whether theButton is visible
        ''' </summary>
        '''	<param name="container">The parent container (DataGridItem)</param>
        ''' <history>
        ''' 	[cnurse]	02/20/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Function GetIsVisible(ByVal container As DataGridItem) As Boolean

            Dim isVisible As Boolean
            If VisibleField <> "" Then
                isVisible = CType(DataBinder.Eval(container.DataItem, VisibleField), Boolean)
            Else
                isVisible = Visible
            End If

            Return isVisible

        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the value of the key
        ''' </summary>
        '''	<param name="container">The parent container (DataGridItem)</param>
        ''' <history>
        ''' 	[cnurse]	02/20/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Function GetValue(ByVal container As DataGridItem) As Integer

            Dim keyValue As Integer = Null.NullInteger
            If KeyField <> "" Then
                keyValue = CType(DataBinder.Eval(container.DataItem, KeyField), Integer)
            End If

            Return keyValue

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

            If EditMode = ImageCommandColumnEditMode.URL Then
                Dim hypLink As HyperLink = CType(sender, HyperLink)
                container = CType(hypLink.NamingContainer, DataGridItem)
                keyValue = GetValue(container)
                If NavigateURLFormatString <> "" Then
                    hypLink.NavigateUrl = String.Format(NavigateURLFormatString, keyValue)
                Else
                    hypLink.NavigateUrl = keyValue.ToString()
                End If
            Else
                'Bind Image Button
                If ImageURL <> "" And ShowImage Then
                    Dim colIcon As ImageButton = CType(sender, ImageButton)
                    container = CType(colIcon.NamingContainer, DataGridItem)
                    keyValue = GetValue(container)
                    colIcon.CommandArgument = keyValue.ToString
                    colIcon.Visible = GetIsVisible(container)
                End If

                'Bind Link Button
                If Text <> "" And Not ShowImage Then
                    Dim colLink As LinkButton = CType(sender, LinkButton)
                    container = CType(colLink.NamingContainer, DataGridItem)
                    keyValue = GetValue(container)
                    colLink.CommandArgument = keyValue.ToString
                    colLink.Visible = GetIsVisible(container)
                End If
            End If
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
                Case ListItemType.Item, ListItemType.AlternatingItem, ListItemType.SelectedItem, ListItemType.EditItem

                    If EditMode = ImageCommandColumnEditMode.URL Then
                        'Add a Hyperlink
                        Dim hypLink As New HyperLink
                        hypLink.ToolTip = Text
                        If ImageURL <> "" And ShowImage Then
                            Dim img As New System.Web.UI.WebControls.Image
                            If DesignMode Then
                                img.ImageUrl = ImageURL.Replace("~/", "../../")
                            Else
                                img.ImageUrl = ImageURL
                            End If
                            hypLink.Controls.Add(img)
                            img.ToolTip = Text
                        Else
                            hypLink.Text = Text
                        End If
                        AddHandler hypLink.DataBinding, AddressOf Item_DataBinding
                        container.Controls.Add(hypLink)
                    Else
                        'Add Image Button
                        If ImageURL <> "" And ShowImage Then
                            Dim colIcon As New ImageButton
                            If DesignMode Then
                                colIcon.ImageUrl = ImageURL.Replace("~/", "../../")
                            Else
                                colIcon.ImageUrl = ImageURL
                            End If
                            colIcon.ToolTip = Text
                            If OnClickJS <> "" Then
                                ClientAPI.AddButtonConfirm(colIcon, OnClickJS)
                            End If
                            colIcon.CommandName = CommandName
                            AddHandler colIcon.DataBinding, AddressOf Item_DataBinding
                            container.Controls.Add(colIcon)
                        End If

                        'Add Link Button
                        If Text <> "" And Not ShowImage Then
                            Dim colLink As New LinkButton
                            colLink.ToolTip = Text
                            If OnClickJS <> "" Then
                                ClientAPI.AddButtonConfirm(colLink, OnClickJS)
                            End If
                            colLink.CommandName = CommandName
                            colLink.Text = Text
                            AddHandler colLink.DataBinding, AddressOf Item_DataBinding
                            container.Controls.Add(colLink)
                        End If
                    End If

                Case ListItemType.Footer, ListItemType.Header
                    container.Controls.Add(New LiteralControl(Text))
            End Select

        End Sub

#End Region

    End Class

End Namespace