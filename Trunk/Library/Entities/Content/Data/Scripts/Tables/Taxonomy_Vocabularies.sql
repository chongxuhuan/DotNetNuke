/************************************************************/
/*****              SqlDataProvider                     *****/
/*****                                                  *****/
/*****                                                  *****/
/***** Note: To manually execute this script you must   *****/
/*****       perform a search and replace operation     *****/
/*****       for {databaseOwner} and {objectQualifier}  *****/
/*****                                                  *****/
/************************************************************/

/* Add Vocabularies Table */
/**************************/

IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'{databaseOwner}[{objectQualifier}Taxonomy_Vocabularies]') AND OBJECTPROPERTY(id, N'IsTable') = 1)
	BEGIN
		CREATE TABLE {databaseOwner}{objectQualifier}Taxonomy_Vocabularies
		(
			[VocabularyID] [int] IDENTITY(1,1) NOT NULL,
			[VocabularyTypeID] [int] NOT NULL,
			[Name] [nvarchar](250) NOT NULL,
			[Description] [nvarchar](2500) NULL,
			[Weight] [int] NOT NULL CONSTRAINT [DF_{objectQualifier}Taxonomy_Vocabularies_Weight]  DEFAULT ((0)),
			[ScopeID] [int] NULL,
			[ScopeTypeID] [int] NOT NULL,
			[CreatedByUserID] [int] NULL,
			[CreatedOnDate] [date] NULL,
			[LastModifiedByUserID] [int] NULL,
			[LastModifiedOnDate] [date] NULL,
			CONSTRAINT [PK_{objectQualifier}Taxonomy_Vocabulary] PRIMARY KEY CLUSTERED ( [VocabularyID] ASC )
		)

	ALTER TABLE  {databaseOwner}[{objectQualifier}Taxonomy_Vocabularies]  WITH CHECK 
		ADD CONSTRAINT [FK_{objectQualifier}Taxonomy_Vocabularies_{objectQualifier}Taxonomy_ScopeTypes] FOREIGN KEY([ScopeTypeID]) REFERENCES {databaseOwner}[{objectQualifier}Taxonomy_ScopeTypes] ([ScopeTypeID]) ON DELETE CASCADE

	ALTER TABLE {databaseOwner}[{objectQualifier}Taxonomy_Vocabularies]  WITH CHECK 
		ADD CONSTRAINT [FK_{objectQualifier}Taxonomy_Vocabularies_{objectQualifier}Taxonomy_VocabularyTypes] FOREIGN KEY([VocabularyTypeID]) REFERENCES {databaseOwner}[{objectQualifier}Taxonomy_VocabularyTypes] ([VocabularyTypeID])
	END
GO

/************************************************************/
/*****              SqlDataProvider                     *****/
/************************************************************/