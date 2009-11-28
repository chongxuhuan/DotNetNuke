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
    ''' Class:      TextColumn
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The TextColumn control provides a custom Text Column
    ''' </summary>
    ''' <history>
    '''     [cnurse]	02/20/2006	created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class TextColumn
        Inherits System.Web.UI.WebControls.TemplateColumn

#Region "Private Members"

        Private mDataField As String
        Private mText As String
        Private mWidth As Unit

#End Region

#Region "Constructors"


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
        ''' Creates a TextColumnTemplate
        ''' </summary>
        ''' <returns>A TextColumnTemplate</returns>
        ''' <history>
        '''     [cnurse]	02/16/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Function CreateTemplate(ByVal type As ListItemType) As TextColumnTemplate

            Dim isDesignMode As Boolean = False
            If HttpContext.Current Is Nothing Then
                isDesignMode = True
            End If

            Dim template As TextColumnTemplate = New TextColumnTemplate(type)
            If type <> ListItemType.Header Then
                template.DataField = DataField
            End If
            template.Width = Width

            If type = ListItemType.Header Then
                template.Text = Me.HeaderText
            Else
                template.Text = Text
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

                Me.ItemStyle.Font.Names = New String() {"Tahoma, Verdana, Arial"}
                Me.ItemStyle.Font.Size = New FontUnit("10pt")
                Me.ItemStyle.HorizontalAlign = HorizontalAlign.Left
                Me.HeaderStyle.Font.Names = New String() {"Tahoma, Verdana, Arial"}
                Me.HeaderStyle.Font.Size = New FontUnit("10pt")
                Me.HeaderStyle.Font.Bold = True
                Me.HeaderStyle.HorizontalAlign = HorizontalAlign.Left

            End If

        End Sub

#End Region

    End Class


End Namespace