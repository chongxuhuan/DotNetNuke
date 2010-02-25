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
Imports System.Collections
Imports System.Configuration
Imports System.Data

Namespace DotNetNuke.Entities.Users

    ''' -----------------------------------------------------------------------------
    ''' Project:    DotNetNuke
    ''' Namespace:  DotNetNuke.Entities.Users
    ''' Class:      BaseUserInfo
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The BaseUserInfo class provides a base Entity for an online user
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    '''     [cnurse]	03/14/2006	documented
    ''' </history>
    ''' -----------------------------------------------------------------------------
    <Serializable()> Public MustInherit Class BaseUserInfo

        Private _PortalID As Integer
        Private _TabID As Integer
        Private _CreationDate As DateTime
        Private _LastActiveDate As DateTime

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the PortalId for this online user
        ''' </summary>
        ''' <history>
        '''     [cnurse]	03/14/2006	Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property PortalID() As Integer
            Get
                Return _PortalID
            End Get
            Set(ByVal Value As Integer)
                _PortalID = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the TabId for this online user
        ''' </summary>
        ''' <history>
        '''     [cnurse]	03/14/2006	Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property TabID() As Integer
            Get
                Return _TabID
            End Get
            Set(ByVal Value As Integer)
                _TabID = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the CreationDate for this online user
        ''' </summary>
        ''' <history>
        '''     [cnurse]	03/14/2006	Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property CreationDate() As DateTime
            Get
                Return _CreationDate
            End Get
            Set(ByVal Value As DateTime)
                _CreationDate = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the LastActiveDate for this online user
        ''' </summary>
        ''' <history>
        '''     [cnurse]	03/14/2006	Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property LastActiveDate() As DateTime
            Get
                Return _LastActiveDate
            End Get
            Set(ByVal Value As DateTime)
                _LastActiveDate = Value
            End Set
        End Property

    End Class

End Namespace
