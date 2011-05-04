/************************************************************/
/*****              SqlDataProvider                     *****/
/*****                                                  *****/
/*****                                                  *****/
/***** Note: To manually execute this script you must   *****/
/*****       perform a search and replace operation     *****/
/*****       for {databaseOwner} and {objectQualifier}  *****/
/*****                                                  *****/
/************************************************************/

/* Add VocabularyTypes Table */
/*****************************/

IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'{databaseOwner}[{objectQualifier}Taxonomy_VocabularyTypes]') AND OBJECTPROPERTY(id, N'IsTable') = 1)
	BEGIN
		CREATE TABLE {databaseOwner}{objectQualifier}Taxonomy_VocabularyTypes
		(
			[VocabularyTypeID] [int] IDENTITY(1,1) NOT NULL,
			[VocabularyType] [nvarchar](50) NOT NULL,
			CONSTRAINT [PK_{objectQualifier}Taxonomy_VocabularyType] PRIMARY KEY CLUSTERED ( [VocabularyTypeID] ASC )
		)

	END
GO

/************************************************************/
/*****              SqlDataProvider                     *****/
/************************************************************/