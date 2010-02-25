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
    ''' Class:      ImageCommandColumn
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The ImageCommandColumn control provides an Image Command (or Hyperlink) column 
    ''' for a Data Grid
    ''' </summary>
    ''' <history>
    '''     [cnurse]	02/16/2006	created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class ImageCommandColumn
        Inherits System.Web.UI.WebControls.TemplateColumn

#Region "Private Members"

        Private mCommandName As String
        Private mEditMode As ImageCommandColumnEditMode = ImageCommandColumnEditMode.Command
        Private mImageURL As String
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
        ''' Creates a ImageCommandColumnTemplate
        ''' </summary>
        ''' <returns>A ImageCommandColumnTemplate</returns>
        ''' <history>
        '''     [cnurse]	02/16/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Function CreateTemplate(ByVal type As ListItemType) As ImageCommandColumnTemplate

            Dim isDesignMode As Boolean = False
            If HttpContext.Current Is Nothing Then
                isDesignMode = True
            End If
            Dim template As ImageCommandColumnTemplate = New ImageCommandColumnTemplate(type)
            If type <> ListItemType.Header Then
                template.ImageURL = ImageURL
                If Not isDesignMode Then
                    template.CommandName = CommandName
                    template.VisibleField = VisibleField
                    template.KeyField = KeyField
                End If
            End If
            template.EditMode = EditMode
            template.NavigateURL = NavigateURL
            template.NavigateURLFormatString = NavigateURLFormatString
            template.OnClickJS = OnClickJS
            template.ShowImage = ShowImage
            template.Visible = Visible

            If type = ListItemType.Header Then
                template.Text = HeaderText
            Else
                template.Text = Text
            End If

            'Set Design Mode to True
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