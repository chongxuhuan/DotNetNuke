/************************************************************/
/*****              SqlDataProvider                     *****/
/*****                                                  *****/
/*****                                                  *****/
/***** Note: To manually execute this script you must   *****/
/*****       perform a search and replace operation     *****/
/*****       for {databaseOwner} and {objectQualifier}  *****/
/*****                                                  *****/
/************************************************************/

/* Add ScopeTypes Table */
/**************************/

IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'{databaseOwner}[{objectQualifier}Taxonomy_ScopeTypes]') AND OBJECTPROPERTY(id, N'IsTable') = 1)
	BEGIN
		CREATE TABLE {databaseOwner}{objectQualifier}Taxonomy_ScopeTypes
		(
			[ScopeTypeID] [int] IDENTITY(1,1) NOT NULL,
			[ScopeType] [nvarchar](250) NULL
			CONSTRAINT [PK_{objectQualifier}ScopeTypes] PRIMARY KEY CLUSTERED ( [ScopeTypeID] ASC )
		)
	END
GO

/************************************************************/
/*****              SqlDataProvider                     *****/
/************************************************************/