/************************************************************/
/*****              SqlDataProvider                     *****/
/*****                                                  *****/
/*****                                                  *****/
/***** Note: To manually execute this script you must   *****/
/*****       perform a search and replace operation     *****/
/*****       for {databaseOwner} and {objectQualifier}  *****/
/*****                                                  *****/
/************************************************************/

/* Add ContentTypes Table */
/**************************/

IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'{databaseOwner}[{objectQualifier}ContentTypes]') AND OBJECTPROPERTY(id, N'IsTable') = 1)
	BEGIN
		CREATE TABLE {databaseOwner}[{objectQualifier}ContentTypes]
		(
			[ContentTypeID] [int] IDENTITY(1,1) NOT NULL,
			[ContentType] [nvarchar](250) NOT NULL,
			CONSTRAINT [PK_{objectQualifier}ContentTypes] PRIMARY KEY CLUSTERED ( [ContentTypeID] ASC )
		)
	END
GO

/************************************************************/
/*****              SqlDataProvider                     *****/
/************************************************************/