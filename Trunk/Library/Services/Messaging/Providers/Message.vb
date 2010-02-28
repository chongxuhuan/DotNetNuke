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

Namespace DotNetNuke.Services.Messaging.Providers

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
        Implements ICloneable        

#Region "Private Members"

        Private _ExecutionCycleGuid As System.Guid
        Private _FromDisplayName As String
        Private _FromEmail As String
        Private _FromUserID As Integer
        Private _IndexID As Integer
        Private _LongBody As String
        Private _MessageDate As DateTime
        Private _MessageGroup As Guid
        Private _MessageID As Integer
        Private _PendingSend As Boolean
        Private _PortalID As Integer
        Private _ReplyTo As Integer
        Private _SendDate As DateTime = DateTime.MinValue
        Private _Status As String
        Private _Subject As String
        Private _ToDisplayName As String
        Private _ToEmail As String
        Private _ToUserID As Integer
        Private _ToUserName As String

#End Region

#Region "Constructors"

        Public Sub New()
            MessageGroup = Guid.Empty
            Me.Status = "Draft"
            Me.MessageDate = DateTime.Now
        End Sub

#End Region

#Region "Public Properties"

        Public Property ExecutionCycleGuid() As System.Guid
            Get
                Return _ExecutionCycleGuid
            End Get
            Set(ByVal value As System.Guid)
                _ExecutionCycleGuid = value
            End Set
        End Property

        Public ReadOnly Property FromDisplayName() As String
            Get
                Return _FromDisplayName
            End Get
        End Property

        Public Property FromEmail() As String
            Get
                Return _FromEmail
            End Get
            Set(ByVal value As String)
                _FromEmail = value
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

        Public Property IndexID() As Integer
            Get
                Return _IndexID
            End Get
            Set(ByVal Value As Integer)
                _IndexID = Value
            End Set
        End Property

        Public Property LongBody() As String
            Get
                Return _LongBody
            End Get
            Set(ByVal Value As String)
                _LongBody = Value
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

        Public Property MessageGroup() As Guid
            Get
                Return _MessageGroup
            End Get
            Set(ByVal value As Guid)
                _MessageGroup = value
            End Set
        End Property

        Public Property MessageID() As Integer
            Get
                Return _MessageID
            End Get
            Set(ByVal Value As Integer)
                _MessageID = Value
            End Set
        End Property

        Public Property PendingSend() As Boolean
            Get
                Return _PendingSend
            End Get
            Set(ByVal value As Boolean)
                _PendingSend = value
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
            Set(ByVal value As Integer)
                _ReplyTo = value
            End Set
        End Property

        Public Property SendDate() As DateTime
            Get
                Return _SendDate
            End Get
            Set(ByVal value As DateTime)
                _SendDate = value
            End Set
        End Property

        Public Property Status() As String
            Get
                Return _Status
            End Get
            Set(ByVal value As String)
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

        Public ReadOnly Property ToDisplayName() As String
            Get
                Return _ToDisplayName
            End Get
        End Property

        Public Property ToEmail() As String
            Get
                Return _ToEmail
            End Get
            Set(ByVal value As String)
                _ToEmail = value
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

        Public ReadOnly Property ToUserName() As String
            Get
                Return _ToUserName
            End Get
        End Property

#End Region

#Region "Public Methods"

        Public Sub ConvertToNewReply()
            'Reset the ID's for a Reply
            ReplyTo = IndexID
            IndexID = Null.NullInteger
            MessageID = Null.NullInteger

            'swap the to/from
            Dim toUser As UserInfo = UserController.GetUserById(PortalID, ToUserID)
            Dim fromUser As UserInfo = UserController.GetUserById(PortalID, FromUserID)
            _ToUserID = fromUser.UserID
            _ToUserName = fromUser.Username
            _ToDisplayName = fromUser.DisplayName
            _ToEmail = fromUser.Email

            _FromUserID = toUser.UserID
            _FromDisplayName = toUser.DisplayName
            _FromEmail = toUser.Email

            'Add "Re" to subject
            If Not Subject.StartsWith("RE: ") Then
                Subject = String.Format("RE: {0}", Subject)
            End If
        End Sub

        Public Sub Init(ByVal Template As Message)
            Me.IndexID = Template.IndexID
            Me.MessageID = Template.MessageID
            Me.PortalID = Template.PortalID
            Me.FromUserID = Template.FromUserID
            Me.ToUserID = Template.ToUserID
            Me.PendingSend = Template.PendingSend
            Me.SendDate = Template.SendDate
            Me.ReplyTo = Template.ReplyTo
            Me.Status = Template.Status
            Me.LongBody = Template.LongBody
            Me.Subject = Template.Subject
            Me.MessageDate = Template.MessageDate
            Me.MessageGroup = Template.MessageGroup
            Me.ExecutionCycleGuid = Template.ExecutionCycleGuid
            Me.ToEmail = Template.ToEmail
            Me.FromEmail = Template.FromEmail
        End Sub

#End Region

#Region "IClonable Implementation"

        Public Function Clone() As Object Implements System.ICloneable.Clone
            Dim newPM As New Message
            newPM.IndexID = Me.IndexID
            newPM.MessageID = Me.MessageID
            newPM.PortalID = Me.PortalID
            newPM.FromUserID = Me.FromUserID
            newPM.ToUserID = Me.ToUserID
            newPM.PendingSend = Me.PendingSend
            newPM.SendDate = Me.SendDate
            newPM.ReplyTo = Me.ReplyTo
            newPM.Status = Me.Status
            newPM.LongBody = Me.LongBody
            newPM.Subject = Me.Subject
            newPM.MessageDate = Me.MessageDate
            newPM.MessageGroup = Me.MessageGroup
            newPM.ExecutionCycleGuid = Me.ExecutionCycleGuid
            newPM.ToEmail = Me.ToEmail
            newPM.FromEmail = Me.FromEmail
            Return newPM

        End Function

#End Region

#Region "IHydratable Implementation"

        Public Sub Fill(ByVal dr As System.Data.IDataReader) Implements Entities.Modules.IHydratable.Fill
            IndexID = Null.SetNullInteger(dr.Item("IndexID"))
            MessageID = Null.SetNullInteger(dr.Item("MessageID"))
            PortalID = Null.SetNullInteger(dr.Item("PortalID"))
            FromUserID = Null.SetNullInteger(dr.Item("FromUserID"))
            _FromDisplayName = Null.SetNullString(dr.Item("FromDisplayName"))
            ToUserID = Null.SetNullInteger(dr.Item("ToUserID"))
            _ToDisplayName = Null.SetNullString(dr.Item("ToDisplayName"))
            _ToUserName = Null.SetNullString(dr.Item("ToUserName"))
            PendingSend = Null.SetNullBoolean(dr.Item("PendingSend"))
            SendDate = Null.SetNullDateTime(dr.Item("SendDate"))
            ReplyTo = Null.SetNullInteger(dr.Item("ReplyToIndexID"))
            Status = Null.SetNullString(dr.Item("Status"))
            LongBody = Null.SetNullString(dr.Item("LongBody"))
            Subject = Null.SetNullString(dr.Item("Subject"))
            MessageDate = Null.SetNullDateTime(dr.Item("MessageDate"))
            Dim g As String = Null.SetNullString(dr.Item("MessagingGroup"))
            MessageGroup = New Guid(g)
            g = ""
            g = Null.SetNullString(dr.Item("ExecutionCycleGuid"))
            If Not (String.IsNullOrEmpty(g)) Then
                ExecutionCycleGuid = New Guid(g)
            End If
            ToEmail = Null.SetNullString(dr.Item("ToEmail"))
            FromEmail = Null.SetNullString(dr.Item("FromEmail"))

        End Sub

        Public Property KeyID() As Integer Implements Entities.Modules.IHydratable.KeyID
            Get
                Return IndexID
            End Get
            Set(ByVal value As Integer)
                IndexID = value
            End Set
        End Property

#End Region

    End Class

End Namespace
