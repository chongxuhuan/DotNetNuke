/************************************************************/
/*****              SqlDataProvider                     *****/
/*****                                                  *****/
/*****                                                  *****/
/***** Note: To manually execute this script you must   *****/
/*****       perform a search and replace operation     *****/
/*****       for {databaseOwner} and {objectQualifier}  *****/
/*****                                                  *****/
/************************************************************/

/* Add ContentItems_Tags Table */
/*******************************/

IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'{databaseOwner}[{objectQualifier}ContentItems_Tags]') AND OBJECTPROPERTY(id, N'IsTable') = 1)
	BEGIN
		CREATE TABLE {databaseOwner}[{objectQualifier}ContentItems_Tags]
		(
			[ContentItemTagID] [int] IDENTITY(1,1) NOT NULL,
			[ContentItemID] [int] NOT NULL,
			[TermID] [int] NOT NULL,
			CONSTRAINT [PK_{objectQualifier}ContentItems_Tags] PRIMARY KEY CLUSTERED ( [ContentItemTagID] ASC )
		)

		CREATE UNIQUE NONCLUSTERED INDEX [IX_{objectQualifier}ContentItems_Tags] ON {databaseOwner}[{objectQualifier}ContentItems_Tags]
		(
			[ContentItemID] ASC,
			[TermID] ASC
		)
	END
GO

/************************************************************/
/*****              SqlDataProvider                     *****/
/************************************************************/