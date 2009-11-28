
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
    ''' Class:      DNNDataGrid
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The DNNDataGrid control provides an Enhanced Data Grid, that supports other
    ''' column types
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    '''     [cnurse]	02/17/2006	created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class DNNDataGrid
        Inherits System.Web.UI.WebControls.DataGrid

#Region "Events"

        Public Event ItemCheckedChanged As DNNDataGridCheckedColumnEventHandler

#End Region

#Region "Private Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Centralised Event that is raised whenever a check box is changed.
        ''' </summary>
        ''' <history>
        '''     [cnurse]	02/17/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub OnItemCheckedChanged(ByVal sender As Object, ByVal e As DNNDataGridCheckChangedEventArgs)

            RaiseEvent ItemCheckedChanged(sender, e)

        End Sub

#End Region

#Region "Protected Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Called when the grid is Data Bound
        ''' </summary>
        ''' <history>
        '''     [cnurse]	02/17/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overrides Sub OnDataBinding(ByVal e As System.EventArgs)

            For Each column As DataGridColumn In Me.Columns
                If column.GetType Is GetType(CheckBoxColumn) Then
                    'Manage CheckBox column events
                    Dim cbColumn As CheckBoxColumn = CType(column, CheckBoxColumn)
                    AddHandler cbColumn.CheckedChanged, AddressOf OnItemCheckedChanged
                End If
            Next

        End Sub

#End Region

        Protected Overrides Sub CreateControlHierarchy(ByVal useDataSource As Boolean)
            MyBase.CreateControlHierarchy(useDataSource)
        End Sub

        Protected Overrides Sub PrepareControlHierarchy()
            MyBase.PrepareControlHierarchy()
        End Sub
    End Class

End Namespace

