/************************************************************/
/*****              SqlDataProvider                     *****/
/*****                                                  *****/
/*****                                                  *****/
/***** Note: To manually execute this script you must   *****/
/*****       perform a search and replace operation     *****/
/*****       for {databaseOwner} and {objectQualifier}  *****/
/*****                                                  *****/
/************************************************************/

/* Add ContentItems Table */
/**************************/

IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'{databaseOwner}[{objectQualifier}ContentItems]') AND OBJECTPROPERTY(id, N'IsTable') = 1)
	BEGIN
		CREATE TABLE {databaseOwner}[{objectQualifier}ContentItems]
		(
			[ContentItemID] [int] IDENTITY(1,1) NOT NULL,
			[Content] [nvarchar](max) NULL,
			[ContentTypeID] [int] NULL,
			[TabID] [int] NULL,
			[ModuleID] [int] NULL,
			[ContentKey] [nvarchar](250) NULL,
			[Indexed] [bit] NOT NULL,
			[CreatedByUserID] [int] NULL,
			[CreatedOnDate] [datetime] NULL,
			[LastModifiedByUserID] [int] NULL,
			[LastModifiedOnDate] [datetime] NULL,
			CONSTRAINT [PK_{objectQualifier}ContentItems] PRIMARY KEY CLUSTERED ( [ContentItemID] ASC )
		)

		ALTER TABLE {databaseOwner}[{objectQualifier}ContentItems] 
			ADD  CONSTRAINT [DF_{objectQualifier}ContentItems_Indexed] DEFAULT ((0)) FOR [Indexed]	
	END
GO

/************************************************************/
/*****              SqlDataProvider                     *****/
/************************************************************/