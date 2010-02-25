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

#Region "Event Handler Delegates"

    Public Delegate Sub DNNDataGridCheckedColumnEventHandler(ByVal sender As Object, ByVal e As DNNDataGridCheckChangedEventArgs)

#End Region

    ''' -----------------------------------------------------------------------------
    ''' Project:    DotNetNuke
    ''' Namespace:  DotNetNuke.UI.WebControls
    ''' Class:      DNNDataGrid
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The DNNDataGridCheckChangedEventArgs class is a cusom EventArgs class for
    ''' handling Event Args from the CheckChanged event in a CheckBox Column
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    '''     [cnurse]	02/17/2006	created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class DNNDataGridCheckChangedEventArgs
        Inherits System.Web.UI.WebControls.DataGridItemEventArgs

#Region "Private Members"

        Private mIsAll As Boolean = False
        Private mChecked As Boolean
        Private mColumn As CheckBoxColumn
        Private mField As String

#End Region

#Region "Constructors"

        Public Sub New(ByVal item As DataGridItem, ByVal isChecked As Boolean, ByVal field As String)
            Me.New(item, isChecked, field, False)
        End Sub

        Public Sub New(ByVal item As DataGridItem, ByVal isChecked As Boolean, ByVal field As String, ByVal isAll As Boolean)
            MyBase.New(item)
            mChecked = isChecked
            mIsAll = isAll
            mField = field
        End Sub

#End Region

#Region "Public Properties"

        Public Property Checked() As Boolean
            Get
                Return mChecked
            End Get
            Set(ByVal Value As Boolean)
                mChecked = Value
            End Set
        End Property

        Public Property Column() As CheckBoxColumn
            Get
                Return mColumn
            End Get
            Set(ByVal Value As CheckBoxColumn)
                mColumn = Value
            End Set
        End Property

        Public Property Field() As String
            Get
                Return mField
            End Get
            Set(ByVal Value As String)
                mField = Value
            End Set
        End Property

        Public Property IsAll() As Boolean
            Get
                Return mIsAll
            End Get
            Set(ByVal Value As Boolean)
                mIsAll = Value
            End Set
        End Property


#End Region

    End Class

End Namespace

