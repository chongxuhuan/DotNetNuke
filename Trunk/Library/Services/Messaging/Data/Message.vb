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
Imports System.Configuration
Imports System.Data
Imports System.ComponentModel.DataAnnotations
Imports DotNetNuke.Services.Messaging.Data

Namespace DotNetNuke.Services.Messaging.Data

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The Info class for Messaging
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' </history>
    ''' -----------------------------------------------------------------------------
    <Serializable()> _
    Public Class Message
        Implements DotNetNuke.Entities.Modules.IHydratable

#Region "Private Members"

        Private _FromUserName As String
        Private _FromUserID As Integer
        Private _Body As String
        Private _MessageDate As DateTime
        Private _Conversation As Guid
        Private _MessageID As Integer
        Private _PortalID As Integer
        Private _ReplyTo As Integer
        Private _Status As MessageStatusType
        Private _Subject As String
        Private _ToUserID As Integer
        Private _ToUserName As String
        Private _EmailSent As Boolean
        Private _skipInbox As Boolean
        Private _ToRoleId As Integer
        Private _allowReply As Boolean

#End Region

#Region "Constructors"

        Public Sub New()
            Conversation = Guid.Empty
            Me.Status = MessageStatusType.Draft
            Me.MessageDate = DateTime.Now
        End Sub

#End Region

#Region "Public Properties"

        Public Property FromUserName() As String
            Get
                Return _FromUserName
            End Get
            Private Set(ByVal value As String)
                _FromUserName = value
            End Set
        End Property



        Public Property FromUserID() As Integer
            Get
                Return _FromUserID
            End Get
            Set(ByVal Value As Integer)
                _FromUserID = Value
            End Set
        End Property


        Public Property ToRoleID() As Integer
            Get
                Return _ToRoleId
            End Get
            Set(ByVal value As Integer)
                _ToRoleId = value
            End Set
        End Property



        Public Property AllowReply() As Boolean
            Get
                Return _allowReply
            End Get
            Set(ByVal value As Boolean)
                _allowReply = value
            End Set
        End Property



        Public Property SkipInbox() As Boolean
            Get
                Return _skipInbox
            End Get
            Set(ByVal value As Boolean)
                _skipInbox = value
            End Set
        End Property

        Public Property EmailSent() As Boolean
            Get
                Return _EmailSent
            End Get
            Set(ByVal value As Boolean)
                _EmailSent = value
            End Set
        End Property



        Public Property Body() As String
            Get
                Return _Body
            End Get
            Set(ByVal Value As String)
                _Body = Value
            End Set
        End Property

        Public Property MessageDate() As DateTime
            Get
                Return _MessageDate
            End Get
            Set(ByVal Value As DateTime)
                _MessageDate = Value
            End Set
        End Property

        Public Property Conversation() As Guid
            Get
                Return _Conversation
            End Get
            Set(ByVal value As Guid)
                _Conversation = value
            End Set
        End Property

        Public Property MessageID() As Integer
            Get
                Return _MessageID
            End Get
            Private Set(ByVal Value As Integer)
                _MessageID = Value
            End Set
        End Property



        Public Property PortalID() As Integer
            Get
                Return _PortalID
            End Get
            Set(ByVal Value As Integer)
                _PortalID = Value
            End Set
        End Property

        Public Property ReplyTo() As Integer
            Get
                Return _ReplyTo
            End Get
            Private Set(ByVal value As Integer)
                _ReplyTo = value
            End Set
        End Property

        Public Property Status() As MessageStatusType
            Get
                Return _Status
            End Get
            Set(ByVal value As MessageStatusType)
                _Status = value
            End Set
        End Property

        Public Property Subject() As String
            Get
                Return _Subject
            End Get
            Set(ByVal Value As String)
                _Subject = Value
            End Set
        End Property


        Public Property ToUserID() As Integer
            Get
                Return _ToUserID
            End Get
            Set(ByVal Value As Integer)
                _ToUserID = Value
            End Set
        End Property

        Public Property ToUserName() As String
            Get
                Return _ToUserName
            End Get
            Private Set(ByVal Value As String)
                _ToUserName = Value
            End Set
        End Property



        Private _EmailSentDate As DateTime
        Public Property EmailSentDate() As DateTime
            Get
                Return _EmailSentDate
            End Get
            Private Set(ByVal value As DateTime)
                _EmailSentDate = value
            End Set
        End Property


        Private _EmailSchedulerInstance As Guid
        Public Property EmailSchedulerInstance() As Guid
            Get
                Return _EmailSchedulerInstance
            End Get
            Private Set(ByVal value As Guid)
                _EmailSchedulerInstance = value
            End Set
        End Property




#End Region

#Region "Public Methods"


        Public Function GetReplyMessage() As Message

            Dim message As New Message()
            message.AllowReply = Me.AllowReply
            message.Body = String.Format("<br><br><br>On {0} {1} wrote ", Me.MessageDate, Me.FromUserName) + Me.Body
            message.Conversation = Me.Conversation
            message.FromUserID = Me.ToUserID
            message.ToUserID = Me.FromUserID
            message.ToUserName = Me.FromUserName
            message.PortalID = Me.PortalID
            message.ReplyTo = Me.MessageID
            message.SkipInbox = Me.SkipInbox
            message.Subject = "RE:" + Me.Subject

            Return message
        End Function

#End Region


#Region "IHydratable Implementation"

        Public Sub Fill(ByVal dr As System.Data.IDataReader) Implements Entities.Modules.IHydratable.Fill
            MessageID = Null.SetNullInteger(dr.Item("MessageID"))
            PortalID = Null.SetNullInteger(dr.Item("PortalID"))
            FromUserID = Null.SetNullInteger(dr.Item("FromUserID"))
            FromUserName = Null.SetNullString(dr.Item("FromUserName"))
            ToUserID = Null.SetNullInteger(dr.Item("ToUserID"))
            ''_ToUserName = Null.SetNullString(dr.Item("ToUserName"))
            ReplyTo = Null.SetNullInteger(dr.Item("ReplyTo"))
            Status = CType(Null.SetNullString(dr.Item("Status")), MessageStatusType)
            Body = Null.SetNullString(dr.Item("Body"))
            Subject = Null.SetNullString(dr.Item("Subject"))
            MessageDate = Null.SetNullDateTime(dr.Item("Date"))
            ToRoleID = Null.SetNullInteger(dr.Item("ToRoleID"))
            AllowReply = Null.SetNullBoolean(dr.Item("AllowReply"))
            SkipInbox = Null.SetNullBoolean(dr.Item("SkipPortal"))
            EmailSent = Null.SetNullBoolean(dr.Item("EmailSent"))
            ToUserName = Null.SetNullString(dr.Item("ToUserName"))
            Dim g As String = Null.SetNullString(dr.Item("Conversation"))
            EmailSentDate = Null.SetNullDateTime(dr.Item("EmailSentDate"))
            EmailSchedulerInstance = Null.SetNullGuid(dr.Item("EmailSchedulerInstance"))
            Conversation = Null.SetNullGuid(dr.Item("Conversation"))



            ''Conversation = New Guid(g)
        End Sub

        Public Property KeyID() As Integer Implements Entities.Modules.IHydratable.KeyID
            Get
                Return MessageID

            End Get
            Set(ByVal value As Integer)
                MessageID = value
            End Set
        End Property

#End Region

    End Class

End Namespace
