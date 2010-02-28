/************************************************************/
/*****              SqlDataProvider                     *****/
/*****                                                  *****/
/*****                                                  *****/
/***** Note: To manually execute this script you must   *****/
/*****       perform a search and replace operation     *****/
/*****       for {databaseOwner} and {objectQualifier}  *****/
/*****                                                  *****/
/************************************************************/

/* Add UpdateVocabulary Procedure */
/***********************************/

IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'{databaseOwner}[{objectQualifier}UpdateVocabulary]') AND OBJECTPROPERTY(id, N'IsPROCEDURE') = 1)
  DROP PROCEDURE {databaseOwner}{objectQualifier}UpdateVocabulary
GO

CREATE PROCEDURE {databaseOwner}[{objectQualifier}UpdateVocabulary] 
	@VocabularyID			int,
	@VocabularyTypeID		int,
	@Name					nvarchar(250),
	@Description			nvarchar(2500),
	@Weight					int,
	@ScopeID				int,
	@ScopeTypeID			int,
	@LastModifiedByUserID	int
AS
	UPDATE {databaseOwner}{objectQualifier}Taxonomy_Vocabularies
		SET 
			VocabularyTypeID = @VocabularyTypeID,
			[Name] = @Name,
			Description = @Description,
			Weight = @Weight,
			ScopeID = @ScopeID,
			ScopeTypeID = @ScopeTypeID,
			LastModifiedByUserID = @LastModifiedByUserID,
			LastModifiedOnDate = getdate()
	WHERE VocabularyId = @VocabularyId
		
GO

/************************************************************/
/*****              SqlDataProvider                     *****/
/************************************************************/
