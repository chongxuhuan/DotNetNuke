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
    ''' Class:      PropertyEditorEventArgs
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The PropertyEditorEventArgs class is a cusom EventArgs class for
    ''' handling Event Args from a change in value of a property.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    '''     [cnurse]	02/17/2006	created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class PropertyEditorEventArgs
        Inherits System.EventArgs

#Region "Private Members"

        Private mChanged As Boolean
        Private mIndex As Integer
        Private mKey As Object
        Private mName As String
        Private mOldValue As Object
        Private mStringValue As String
        Private mValue As Object

#End Region

#Region "Constructors"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Constructs a new PropertyEditorEventArgs
        ''' </summary>
        ''' <param name="name">The name of the property</param>
        ''' <history>
        '''     [cnurse]	02/23/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub New(ByVal name As String)
            Me.New(name, Nothing, Nothing)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Constructs a new PropertyEditorEventArgs
        ''' </summary>
        ''' <param name="name">The name of the property</param>
        ''' <param name="newValue">The new value of the property</param>
        ''' <param name="oldValue">The old value of the property</param>
        ''' <history>
        '''     [cnurse]	02/21/2006	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub New(ByVal name As String, ByVal newValue As Object, ByVal oldValue As Object)
            mName = name
            mValue = newValue
            mOldValue = oldValue
        End Sub

#End Region

#Region "Public Properties"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets whether the proeprty has changed
        ''' </summary>
        ''' <value>A String</value>
        ''' <history>
        ''' 	[cnurse]	01/05/2007	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property Changed() As Boolean
            Get
                Return mChanged
            End Get
            Set(ByVal Value As Boolean)
                mChanged = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the Index of the Item
        ''' </summary>
        ''' <value>An Integer</value>
        ''' <history>
        ''' 	[cnurse]	02/05/2008	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property Index() As Integer
            Get
                Return mIndex
            End Get
            Set(ByVal Value As Integer)
                mIndex = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the Key of the Item
        ''' </summary>
        ''' <value>An Object</value>
        ''' <history>
        ''' 	[cnurse]	02/05/2008	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property Key() As Object
            Get
                Return mKey
            End Get
            Set(ByVal Value As Object)
                mKey = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the Name of the Property being changed
        ''' </summary>
        ''' <value>A String</value>
        ''' <history>
        ''' 	[cnurse]	02/21/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property Name() As String
            Get
                Return mName
            End Get
            Set(ByVal Value As String)
                mName = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the OldValue of the Property being changed
        ''' </summary>
        ''' <value>An Object</value>
        ''' <history>
        ''' 	[cnurse]	02/21/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property OldValue() As Object
            Get
                Return mOldValue
            End Get
            Set(ByVal Value As Object)
                mOldValue = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the String Value of the Property being changed
        ''' </summary>
        ''' <value>An Object</value>
        ''' <history>
        ''' 	[cnurse]	02/21/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property StringValue() As String
            Get
                Return mStringValue
            End Get
            Set(ByVal Value As String)
                mStringValue = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the Value of the Property being changed
        ''' </summary>
        ''' <value>An Object</value>
        ''' <history>
        ''' 	[cnurse]	02/21/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property Value() As Object
            Get
                Return mValue
            End Get
            Set(ByVal Value As Object)
                mValue = Value
            End Set
        End Property

#End Region

    End Class

End Namespace

