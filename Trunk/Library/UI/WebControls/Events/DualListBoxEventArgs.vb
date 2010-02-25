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
Imports System.Collections.Generic


Namespace DotNetNuke.UI.WebControls

    ''' -----------------------------------------------------------------------------
    ''' Project:    DotNetNuke
    ''' Namespace:  DotNetNuke.UI.WebControls
    ''' Class:      DualListBoxEventArgs
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The DualListBoxEventArgs class is a cusom EventArgs class for
    ''' handling Event Args in the DualListBox.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    '''     [cnurse]	02/18/2008	created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class DualListBoxEventArgs
        Inherits System.EventArgs

#Region "Private Members"

        Private _Items As List(Of String)

#End Region

#Region "Constructors"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Constructs a new DualListBoxEventArgs
        ''' </summary>
        ''' <param name="items">The items</param>
        ''' <history>
        '''     [cnurse]	02/18/2008	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub New(ByVal items As List(Of String))
            _Items = items
        End Sub

#End Region

#Region "Public Properties"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the Items
        ''' </summary>
        ''' <value>A List(Of String)</value>
        ''' <history>
        '''     [cnurse]	02/18/2008	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property Items() As List(Of String)
            Get
                Return _Items
            End Get
            Set(ByVal Value As List(Of String))
                _Items = Value
            End Set
        End Property

#End Region

    End Class

End Namespace

