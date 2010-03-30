/************************************************************/
/*****              SqlDataProvider                     *****/
/*****                                                  *****/
/*****                                                  *****/
/***** Note: To manually execute this script you must   *****/
/*****       perform a search and replace operation     *****/
/*****       for {databaseOwner} and {objectQualifier}  *****/
/*****                                                  *****/
/************************************************************/

/* Add ContentItems_MetaData Table */
/***********************************/

IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'{databaseOwner}[{objectQualifier}ContentItems_MetaData]') AND OBJECTPROPERTY(id, N'IsTable') = 1)
	BEGIN
		CREATE TABLE {databaseOwner}[{objectQualifier}ContentItems_MetaData]
		(
				[ContentItemMetaDataID] [int] IDENTITY(1,1) NOT NULL,
				[ContentItemID] [int] NOT NULL,
				[MetaDataID] [int] NOT NULL,
				[MetaDataValue] [nvarchar](max) NULL,
				CONSTRAINT [PK_{objectQualifier}Content_MetaData] PRIMARY KEY CLUSTERED ( [ContentItemMetaDataID] ASC )
		)

		ALTER TABLE {databaseOwner}[{objectQualifier}ContentItems_MetaData]  WITH CHECK 
			ADD  CONSTRAINT [FK_{objectQualifier}ContentItems_MetaData_{objectQualifier}ContentItems] FOREIGN KEY([ContentItemID]) REFERENCES {databaseOwner}[{objectQualifier}ContentItems] ([ContentItemID]) ON UPDATE CASCADE ON DELETE CASCADE

		ALTER TABLE {databaseOwner}[{objectQualifier}ContentItems_MetaData]  WITH CHECK 
			ADD  CONSTRAINT [FK_{objectQualifier}ContentItems_MetaData{objectQualifier}MetaData] FOREIGN KEY([MetaDataID]) REFERENCES {databaseOwner}[{objectQualifier}MetaData] ([MetaDataID]) ON UPDATE CASCADE ON DELETE CASCADE
	END
GO

/************************************************************/
/*****              SqlDataProvider                     *****/
/************************************************************/