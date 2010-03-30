/************************************************************/
/*****              SqlDataProvider                     *****/
/*****                                                  *****/
/*****                                                  *****/
/***** Note: To manually execute this script you must   *****/
/*****       perform a search and replace operation     *****/
/*****       for {databaseOwner} and {objectQualifier}  *****/
/*****                                                  *****/
/************************************************************/

/* Add MetaData Table */
/**********************/

IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'{databaseOwner}[{objectQualifier}MetaData]') AND OBJECTPROPERTY(id, N'IsTable') = 1)
	BEGIN
		CREATE TABLE {databaseOwner}[{objectQualifier}MetaData]
		(
			[MetaDataID] [int] IDENTITY(1,1) NOT NULL,
			[MetaDataName] [nvarchar](100) NOT NULL,
			[MetaDataDescription] [nvarchar](2500) NULL,
			CONSTRAINT [PK_{objectQualifier}MetaData] PRIMARY KEY CLUSTERED ( [MetaDataID] ASC )
		)
	END
GO

/************************************************************/
/*****              SqlDataProvider                     *****/
/************************************************************/