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

Imports System.Web.Caching
Imports DotNetNuke.Services.Cache

Namespace DotNetNuke.Common.Utilities

    ''' -----------------------------------------------------------------------------
    ''' Project:    DotNetNuke
    ''' Namespace:  DotNetNuke.Common.Utilities
    ''' Class:      CacheItemArgs
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The CacheItemArgs class provides an EventArgs implementation for the
    ''' CacheItemExpiredCallback delegate
    ''' </summary>
    ''' <history>
    '''     [cnurse]	01/12/2008	created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class CacheItemArgs

#Region "Private Members"

        Private _CacheCallback As CacheItemRemovedCallback
        Private _CacheDependency As DNNCacheDependency
        Private _CacheKey As String
        Private _CachePriority As CacheItemPriority
        Private _CacheTimeOut As Integer
        Private _ParamList As ArrayList
        Private _ProcedureName As String

#End Region

#Region "Constructors"

        '''-----------------------------------------------------------------------------
        ''' <summary>
        ''' Constructs a new CacheItemArgs Object
        ''' </summary>
        ''' <param name="key"></param>
        ''' <history>
        '''     [cnurse]	01/12/2008	created
        ''' </history>
        '''-----------------------------------------------------------------------------
        Public Sub New(ByVal key As String)
            _CacheKey = key
            _CacheTimeOut = 20
            _CachePriority = CacheItemPriority.Default
            _ParamList = New ArrayList()
        End Sub

        '''-----------------------------------------------------------------------------
        ''' <summary>
        ''' Constructs a new CacheItemArgs Object
        ''' </summary>
        ''' <param name="key"></param>
        ''' <param name="timeout"></param>
        ''' <history>
        '''     [cnurse]	01/12/2008	created
        ''' </history>
        '''-----------------------------------------------------------------------------
        Public Sub New(ByVal key As String, ByVal timeout As Integer)
            Me.New(key)
            _CacheTimeOut = timeout
            _CachePriority = CacheItemPriority.Default
            _ParamList = New ArrayList()
        End Sub

        '''-----------------------------------------------------------------------------
        ''' <summary>
        ''' Constructs a new CacheItemArgs Object
        ''' </summary>
        ''' <param name="key"></param>
        ''' <param name="priority"></param>
        ''' <history>
        '''     [cnurse]	01/12/2008	created
        ''' </history>
        '''-----------------------------------------------------------------------------
        Public Sub New(ByVal key As String, ByVal priority As CacheItemPriority)
            Me.New(key)
            _CachePriority = priority
            _ParamList = New ArrayList()
        End Sub

        '''-----------------------------------------------------------------------------
        ''' <summary>
        ''' Constructs a new CacheItemArgs Object
        ''' </summary>
        ''' <param name="key"></param>
        ''' <param name="timeout"></param>
        ''' <param name="priority"></param>
        ''' <history>
        '''     [cnurse]	07/15/2008	created
        ''' </history>
        '''-----------------------------------------------------------------------------
        Public Sub New(ByVal key As String, ByVal timeout As Integer, ByVal priority As CacheItemPriority)
            Me.New(key)
            _CacheTimeOut = timeout
            _CachePriority = priority
        End Sub

        '''-----------------------------------------------------------------------------
        ''' <summary>
        ''' Constructs a new CacheItemArgs Object
        ''' </summary>
        ''' <param name="key"></param>
        ''' <param name="timeout"></param>
        ''' <param name="priority"></param>
        ''' <history>
        '''     [cnurse]	07/14/2008	created
        ''' </history>
        '''-----------------------------------------------------------------------------
        Public Sub New(ByVal key As String, ByVal timeout As Integer, ByVal priority As CacheItemPriority, ByVal ParamArray params As Object())
            Me.New(key)
            _CacheTimeOut = timeout
            _CachePriority = priority
            _ParamList = New ArrayList()
            For Each obj As Object In params
                _ParamList.Add(obj)
            Next
        End Sub

#End Region

#Region "Public Properties"

        '''-----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the Cache Item's CacheItemRemovedCallback delegate
        ''' </summary>
        ''' <history>
        '''     [cnurse]	01/13/2008	created
        ''' </history>
        '''-----------------------------------------------------------------------------
        Public Property CacheCallback() As CacheItemRemovedCallback
            Get
                Return _CacheCallback
            End Get
            Set(ByVal value As CacheItemRemovedCallback)
                _CacheCallback = value
            End Set
        End Property

        '''-----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the Cache Item's CacheDependency
        ''' </summary>
        ''' <history>
        '''     [cnurse]	01/12/2008	created
        ''' </history>
        '''-----------------------------------------------------------------------------
        Public Property CacheDependency() As DNNCacheDependency
            Get
                Return _CacheDependency
            End Get
            Set(ByVal value As DNNCacheDependency)
                _CacheDependency = value
            End Set
        End Property

        '''-----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the Cache Item's Key
        ''' </summary>
        ''' <history>
        '''     [cnurse]	01/12/2008	created
        ''' </history>
        '''-----------------------------------------------------------------------------
        Public ReadOnly Property CacheKey() As String
            Get
                Return _CacheKey
            End Get
        End Property

        '''-----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the Cache Item's priority (defaults to Default)
        ''' </summary>
        ''' <remarks>Note: DotNetNuke currently doesn't support the ASP.NET Cache's
        ''' ItemPriority, but this is included for possible future use. </remarks>
        ''' <history>
        '''     [cnurse]	01/12/2008	created
        ''' </history>
        '''-----------------------------------------------------------------------------
        Public Property CachePriority() As CacheItemPriority
            Get
                Return _CachePriority
            End Get
            Set(ByVal value As CacheItemPriority)
                _CachePriority = value
            End Set
        End Property

        '''-----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the Cache Item's Timeout
        ''' </summary>
        ''' <history>
        '''     [cnurse]	01/12/2008	created
        ''' </history>
        '''-----------------------------------------------------------------------------
        Public Property CacheTimeOut() As Integer
            Get
                Return _CacheTimeOut
            End Get
            Set(ByVal value As Integer)
                _CacheTimeOut = value
            End Set
        End Property

        '''-----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the Cache Item's Parameter List
        ''' </summary>
        ''' <history>
        '''     [cnurse]	01/12/2008	created
        ''' </history>
        '''-----------------------------------------------------------------------------
        Public ReadOnly Property ParamList() As ArrayList
            Get
                Return _ParamList
            End Get
        End Property

        '''-----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the Cache Item's Parameter Array
        ''' </summary>
        ''' <history>
        '''     [cnurse]	01/12/2008	created
        ''' </history>
        '''-----------------------------------------------------------------------------
        Public ReadOnly Property Params() As Object()
            Get
                Return ParamList.ToArray()
            End Get
        End Property

        Public Property ProcedureName() As String
            Get
                Return _ProcedureName
            End Get
            Set(ByVal value As String)
                _ProcedureName = value
            End Set
        End Property

#End Region

    End Class

End Namespace
