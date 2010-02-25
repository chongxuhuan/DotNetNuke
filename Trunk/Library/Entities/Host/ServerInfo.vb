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
Imports DotNetNuke.Entities.Modules

Namespace DotNetNuke.Entities.Host
    Public Class ServerInfo
        Implements IHydratable

#Region "Private Members"

        Private _IISAppName As String
        Private _ServerID As Integer
        Private _ServerName As String
        Private _Url As String
        Private _Enabled As Boolean
        Private _CreatedDate As DateTime
        Private _LastActivityDate As DateTime

#End Region

#Region "Constructors"

        Public Sub New()
            Me.New(DateTime.Now, DateTime.Now)
        End Sub

        Public Sub New(ByVal created As DateTime, ByVal lastactivity As DateTime)
            _IISAppName = DotNetNuke.Common.Globals.IISAppName
            _ServerName = DotNetNuke.Common.Globals.ServerName
            _CreatedDate = created
            _LastActivityDate = lastactivity
        End Sub

#End Region

#Region "Public Methods"

        Public Property ServerID() As Integer
            Get
                Return _ServerID
            End Get
            Set(ByVal value As Integer)
                _ServerID = value
            End Set
        End Property

        Public Property IISAppName() As String
            Get
                Return _IISAppName
            End Get
            Set(ByVal value As String)
                _IISAppName = value
            End Set
        End Property

        Public Property ServerName() As String
            Get
                Return _ServerName
            End Get
            Set(ByVal value As String)
                _ServerName = value
            End Set
        End Property

        Public Property Url() As String
            Get
                Return _Url
            End Get
            Set(ByVal value As String)
                _Url = value
            End Set
        End Property

        Public Property Enabled() As Boolean
            Get
                Return _Enabled
            End Get
            Set(ByVal value As Boolean)
                _Enabled = value
            End Set
        End Property

        Public Property CreatedDate() As Date
            Get
                Return _CreatedDate
            End Get
            Set(ByVal value As Date)
                _CreatedDate = value
            End Set
        End Property

        Public Property LastActivityDate() As Date
            Get
                Return _LastActivityDate
            End Get
            Set(ByVal value As Date)
                _LastActivityDate = value
            End Set
        End Property

#End Region

#Region "IHydratable Implementation"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Fills a ServerInfo from a Data Reader
        ''' </summary>
        ''' <param name="dr">The Data Reader to use</param>
        ''' <history>
        ''' 	[cnurse]	03/25/2009   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub Fill(ByVal dr As System.Data.IDataReader) Implements IHydratable.Fill
            ServerID = Null.SetNullInteger(dr("ServerID"))
            IISAppName = Null.SetNullString(dr("IISAppName"))
            ServerName = Null.SetNullString(dr("ServerName"))
            Url = Null.SetNullString(dr("URL"))
            Enabled = Null.SetNullBoolean(dr("Enabled"))
            CreatedDate = Null.SetNullDateTime(dr("CreatedDate"))
            LastActivityDate = Null.SetNullDateTime(dr("LastActivityDate"))
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the Key ID
        ''' </summary>
        ''' <returns>An Integer</returns>
        ''' <history>
        ''' 	[cnurse]	03/25/2009   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property KeyID() As Integer Implements IHydratable.KeyID
            Get
                Return ServerID
            End Get
            Set(ByVal value As Integer)
                ServerID = value
            End Set
        End Property

#End Region

    End Class
End Namespace
