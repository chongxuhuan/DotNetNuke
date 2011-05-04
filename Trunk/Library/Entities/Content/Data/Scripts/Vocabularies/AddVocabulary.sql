/************************************************************/
/*****              SqlDataProvider                     *****/
/*****                                                  *****/
/*****                                                  *****/
/***** Note: To manually execute this script you must   *****/
/*****       perform a search and replace operation     *****/
/*****       for {databaseOwner} and {objectQualifier}  *****/
/*****                                                  *****/
/************************************************************/

/* Add AddVocabulary Procedure */
/*******************************/

IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'{databaseOwner}[{objectQualifier}AddVocabulary]') AND OBJECTPROPERTY(id, N'IsPROCEDURE') = 1)
  DROP PROCEDURE {databaseOwner}{objectQualifier}AddVocabulary
GO

CREATE PROCEDURE {databaseOwner}[{objectQualifier}AddVocabulary] 
	@VocabularyTypeID	int,
	@Name				nvarchar(250),
	@Description		nvarchar(2500),
	@Weight				int,
	@ScopeID			int,
	@ScopeTypeID		int,
	@CreatedByUserID	int
AS
	INSERT INTO {databaseOwner}{objectQualifier}Taxonomy_Vocabularies (
		VocabularyTypeID,
		[Name],
		Description,
		Weight,
		ScopeID,
		ScopeTypeID,
		CreatedByUserID,
		CreatedOnDate,
		LastModifiedByUserID,
		LastModifiedOnDate
	)

	VALUES (
		@VocabularyTypeID,
		@Name,
		@Description,
		@Weight,
		@ScopeID,
		@ScopeTypeID,
		@CreatedByUserID,
		getdate(),
		@CreatedByUserID,
		getdate()
	)

	SELECT SCOPE_IDENTITY()
GO

/************************************************************/
/*****              SqlDataProvider                     *****/
/************************************************************/
