<Query Kind="SQL">
  <Connection>
    <ID>637d1987-ae22-4668-a010-29f917077abe</ID>
    <Server>.\SQLEXPRESS</Server>
    <Persist>true</Persist>
    <SqlSecurity>true</SqlSecurity>
    <UserName>dnnuser</UserName>
    <Password>AQAAANCMnd8BFdERjHoAwE/Cl+sBAAAAgm8fv6eAt0WxDoD6Ulfb6wAAAAACAAAAAAADZgAAwAAAABAAAAAeBFC1Ds54CypJyzi1QSuXAAAAAASAAACgAAAAEAAAAJ8z7ahW/Pah0c8gwcgSSaUQAAAAIdXN8n2QipDkw9YMNVJOcBQAAAASp+tBMYgg4HKCgEjGe+xqCQ7ADQ==</Password>
    <Database>CE_DotNetNuke_5_Development</Database>
    <ShowServer>true</ShowServer>
  </Connection>
</Query>

--Region dnn_Messaging_MessageIndex

if exists (select * from dbo.sysobjects where id = object_id(N'[dnn_Messaging_MessageIndex]') and OBJECTPROPERTY(id, N'IsTable') = 1)
	BEGIN
		drop table [dnn_Messaging_MessageIndex]
	END

--if not exists (select * from dbo.sysobjects where id = object_id(N'[dnn_Messaging_MessageIndex]') and OBJECTPROPERTY(id, N'IsTable') = 1)
--	BEGIN
		CREATE TABLE [dnn_Messaging_MessageIndex]
		(
			[IndexID] int NOT NULL IDENTITY(1, 1),
			[MessageID] int NOT NULL,
			[PortalID] int NOT NULL,
			[FromUserID] int NOT NULL,
			[ToUserID] int NOT NULL,
			[PendingSend] bit NOT NULL DEFAULT 0,
			[SendDate] datetime,			
			[ReplyToIndexID] int,			
			[Status] nvarchar(100),
			[MessageDate] datetime NOT NULL DEFAULT GETDATE(),			
			[MessagingGroup] uniqueidentifier,
			[ExecutionCycleGuid] uniqueidentifier
		)

		ALTER TABLE [dnn_Messaging_MessageIndex] ADD CONSTRAINT [PK_dnn_Messaging_MessageIndex] PRIMARY KEY CLUSTERED  ([IndexID])
		CREATE NONCLUSTERED INDEX [IX_dnn_Messaging_MessageIndex] ON [dnn_Messaging_MessageIndex] ([IndexID])
--	END
GO


if exists (select * from dbo.sysobjects where id = object_id(N'[dnn_Messaging_MessageIndex_Save_MessageIndex]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure [dnn_Messaging_MessageIndex_Save_MessageIndex]
GO


/** Create Stored Procedures **/
create procedure [dnn_Messaging_MessageIndex_Save_MessageIndex]
			@MessageID int,
			@PortalID int,
			@FromUserID int,
			@ToUserID int,
			@PendingSend bit,
			@SendDate datetime,	
			@ReplyToIndexID int,
			@Status nvarchar(100),
			@MessageDate datetime,
			@MessagingGroup uniqueidentifier,
			@IndexID int OUTPUT

as

if(Exists(Select IndexID from [dnn_Messaging_MessageIndex] Where IndexID=@IndexID))
	BEGIN
		Update [dnn_Messaging_MessageIndex] set 	
			[MessageID]=@MessageID,
			[PortalID]=@PortalID,
			[FromUserID]=@FromUserID,
			[ToUserID]=@ToUserID,
			[PendingSend]=@PendingSend,
			[SendDate]=@SendDate,
			[ReplyToIndexID]=@ReplyToIndexID,
			[Status]=@Status,
			[MessageDate]=@MessageDate,
			[MessagingGroup]=@MessagingGroup
			where IndexID = @IndexID			
			select @IndexID as IndexID
	END 
ELSE
	BEGIN
		insert into [dnn_Messaging_MessageIndex] (
			[MessageID],
			[PortalID],
			[FromUserID],
			[ToUserID],
			[PendingSend],
			[SendDate],
			[ReplyToIndexID],
			[Status],
			[MessageDate],
			[MessagingGroup]
			)
			Values(
			@MessageID,
			@PortalID,
			@FromUserID,
			@ToUserID,
			@PendingSend,
			@SendDate,
			@ReplyToIndexID,
			@Status,
			@MessageDate,
			@MessagingGroup
			)
		Select @IndexID=SCOPE_IDENTITY()						
		select @IndexID as IndexID1
		
	END

GO

--End Region dnn_Messaging_MessageIndex

------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------

--Region dnn_Messaging_MessageStore

if exists (select * from dbo.sysobjects where id = object_id(N'[dnn_Messaging_MessageStore]') and OBJECTPROPERTY(id, N'IsTable') = 1)
	BEGIN
		drop table [dnn_Messaging_MessageStore]
	END
	
	
--if not exists (select * from dbo.sysobjects where id = object_id(N'[dnn_Messaging_MessageStore]') and OBJECTPROPERTY(id, N'IsTable') = 1)
--	BEGIN
		CREATE TABLE [dnn_Messaging_MessageStore]
		(
			[MessageID] int NOT NULL IDENTITY(1, 1),
			[LongBody] ntext NOT NULL,
			[Subject] nvarchar(100) NOT NULL,
		)

		ALTER TABLE [dnn_Messaging_MessageStore] ADD CONSTRAINT [PK_dnn_Messaging_MessageStore] PRIMARY KEY CLUSTERED  ([MessageID])
		CREATE NONCLUSTERED INDEX [IX_dnn_Messaging_MessageStore] ON [dnn_Messaging_MessageStore] ([MessageID])
--	END
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dnn_Messaging_Save_MessageStore]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure [dnn_Messaging_Save_MessageStore]
GO


/** Create Stored Procedures **/
create procedure [dnn_Messaging_Save_MessageStore]
			@LongBody ntext,
			@Subject nvarchar(100),
			@MessageID int OUTPUT		
as
if(Exists(Select MessageID from [dnn_Messaging_MessageStore] Where MessageID=@MessageID))
	BEGIN
		Update dnn_Messaging_MessageStore set 	
			[LongBody]=@LongBody,
			[Subject]=@Subject
			where MessageID = @MessageID
			
			select @MessageID as MessageID
	END 
ELSE
	BEGIN
		insert into [dnn_Messaging_MessageStore] (
			[LongBody],
			[Subject]
			)
			Values(
			@LongBody ,
			@Subject
			)
			
		Select @MessageID=SCOPE_IDENTITY()						
		select @MessageID as MessageID
		
	END

GO

--End Region dnn_Messaging_MessageStore

------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------

--Region Additional

if exists (select * from dbo.sysobjects where id = object_id(N'[dnn_Messaging_Save_Message]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure [dnn_Messaging_Save_Message]
GO


/** Create Stored Procedures **/
create procedure [dnn_Messaging_Save_Message]
			@LongBody ntext,
			@Subject nvarchar(100),
			@PortalID int,
			@FromUserID int,
			@ToUserID int,
			@PendingSend bit,
			@SendDate datetime,				
			@ReplyToIndexID int,
			@Status nvarchar(100),
			@MessageDate datetime,
			@MessagingGroup uniqueidentifier,
			@MessageID int OUTPUT,		
			@IndexID int OUTPUT	
as

EXEC [dnn_Messaging_Save_MessageStore] 
	@LongBody,
	@Subject,
	@MessageID OUTPUT
	

EXEC [dnn_Messaging_MessageIndex_Save_MessageIndex] 
			@MessageID,
			@PortalID,
			@FromUserID,
			@ToUserID,
			@PendingSend,
			@SendDate,				
			@ReplyToIndexID,
			@Status,
			@MessageDate,
			@MessagingGroup,
			@IndexID OUTPUT

select @MessageID as MessageID, @IndexID as IndexID

GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dnn_Messaging_Get_MessagesStatus_ByUser]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure [dnn_Messaging_Get_MessagesStatus_ByUser]
GO
create procedure [dnn_Messaging_Get_MessagesStatus_ByUser]
	@PortalID int,
	@UserID int
as

select distinct Status
from 
	[dnn_Messaging_MessageIndex] as I
Where
	I.PortalID=@PortalID and
	(I.FromUserID=@UserID or I.ToUserID=@UserID)
	
GO



if exists (select * from dbo.sysobjects where id = object_id(N'[dnn_Messaging_Get_Messages_ByUser]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure [dnn_Messaging_Get_Messages_ByUser]
GO
create procedure [dnn_Messaging_Get_Messages_ByUser]
	@PortalID int,
	@UserID int
as

select 
	i.IndexID, i.MessageID, i.PortalID, i.FromUserID, i.ToUserID, i.PendingSend, i.SendDate, i.ReplyToIndexID, i.Status, i.MessageDate, i.MessagingGroup, i.ExecutionCycleGuid,
	s.LongBody, s.Subject, uFrom.DisplayName as FromDisplayName, uTo.DisplayName as ToDisplayName, uTo.Email as ToEmail, uFrom.Email as FromEmail
FROM
	[dnn_Messaging_MessageIndex] as i left join [dnn_Messaging_MessageStore] as s on i.MessageID =  s.MessageID
	left join [dnn_Users] as uFrom on uFrom.UserID = i.FromUserID 
	join [dnn_Users] as uTo on uTo.UserID = i.ToUserID
where 
	i.PortalID = @PortalID and 
	(i.FromUserID = @UserID or i.ToUserID = @UserID) and
	(uFrom.UserID = i.FromUserID or uTo.UserID = i.ToUserID)
	
GO



if exists (select * from dbo.sysobjects where id = object_id(N'[dnn_Messaging_Get_Message_ByIndexID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	drop procedure [dnn_Messaging_Get_Message_ByIndexID]
GO
create procedure [dnn_Messaging_Get_Message_ByIndexID]
	@PortalID int,
	@UserID int,
	@IndexID int
as


select 
	i.IndexID, i.MessageID, i.PortalID, i.FromUserID, i.ToUserID, i.PendingSend, i.SendDate, i.ReplyToIndexID, i.Status, i.MessageDate, i.MessagingGroup, i.ExecutionCycleGuid,
	s.LongBody, s.Subject, uFrom.DisplayName as FromDisplayName, uTo.DisplayName as ToDisplayName, uTo.Email as ToEmail, uFrom.Email as FromEmail
FROM
	[dnn_Messaging_MessageIndex] as i left join [dnn_Messaging_MessageStore] as s on i.MessageID =  s.MessageID
	left join [dnn_Users] as uFrom on uFrom.UserID = i.FromUserID 
	join [dnn_Users] as uTo on uTo.UserID = i.ToUserID
where 
	i.PortalID = @PortalID and 
	(i.FromUserID = @UserID or i.ToUserID = @UserID) and
	(uFrom.UserID = i.FromUserID or uTo.UserID = i.ToUserID) and 
	i.IndexID = @IndexID
	
GO

--End Region Additional