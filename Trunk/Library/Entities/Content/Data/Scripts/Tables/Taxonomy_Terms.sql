/************************************************************/
/*****              SqlDataProvider                     *****/
/*****                                                  *****/
/*****                                                  *****/
/***** Note: To manually execute this script you must   *****/
/*****       perform a search and replace operation     *****/
/*****       for {databaseOwner} and {objectQualifier}  *****/
/*****                                                  *****/
/************************************************************/

/* Add Terms Table */
/*******************/

IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'{databaseOwner}[{objectQualifier}Taxonomy_Terms]') AND OBJECTPROPERTY(id, N'IsTable') = 1)
	BEGIN
		CREATE TABLE {databaseOwner}{objectQualifier}Taxonomy_Terms
		(
			[TermID] [int] IDENTITY(1,1) NOT NULL,
			[VocabularyID] [int] NOT NULL,
			[ParentTermID] [int] NULL,
			[Name] [nvarchar](250) NOT NULL,
			[Description] [nvarchar](2500) NULL,
			[Weight] [int] NOT NULL CONSTRAINT [DF_{objectQualifier}Taxonomy_Terms_Weight]  DEFAULT ((0)),
			[TermLeft] [int] NOT NULL CONSTRAINT [DF_{objectQualifier}Taxonomy_Terms_TermLeft]  DEFAULT ((0)),
			[TermRight] [int] NOT NULL CONSTRAINT [DF_{objectQualifier}Taxonomy_Terms_TermRight]  DEFAULT ((0)),
			[CreatedByUserID] [int] NULL,
			[CreatedOnDate] [datetime] NULL,
			[LastModifiedByUserID] [int] NULL,
			[LastModifiedOnDate] [datetime] NULL,
			CONSTRAINT [PK_{objectQualifier}Taxonomy_Terms] PRIMARY KEY CLUSTERED ( [TermID] ASC )
		)

		ALTER TABLE {databaseOwner}[{objectQualifier}Taxonomy_Terms]  WITH CHECK 
			ADD CONSTRAINT [FK_{objectQualifier}Taxonomy_Terms_{objectQualifier}Taxonomy_Vocabularies] FOREIGN KEY([VocabularyID]) REFERENCES {databaseOwner}[{objectQualifier}Taxonomy_Vocabularies] ([VocabularyID]) ON DELETE CASCADE

		ALTER TABLE {databaseOwner}[{objectQualifier}Taxonomy_Terms]  WITH CHECK 
			ADD  CONSTRAINT [FK_{objectQualifier}Taxonomy_Terms_{objectQualifier}Taxonomy_Terms] FOREIGN KEY([ParentTermID]) REFERENCES {databaseOwner}[{objectQualifier}Taxonomy_Terms] ([TermID])
	END

GO

/************************************************************/
/*****              SqlDataProvider                     *****/
/************************************************************/