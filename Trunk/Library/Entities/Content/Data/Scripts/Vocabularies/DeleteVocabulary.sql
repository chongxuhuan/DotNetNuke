/************************************************************/
/*****              SqlDataProvider                     *****/
/*****                                                  *****/
/*****                                                  *****/
/***** Note: To manually execute this script you must   *****/
/*****       perform a search and replace operation     *****/
/*****       for {databaseOwner} and {objectQualifier}  *****/
/*****                                                  *****/
/************************************************************/

/* Add DeleteVocabulary Procedure */
/***********************************/

IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'{databaseOwner}[{objectQualifier}DeleteVocabulary]') AND OBJECTPROPERTY(id, N'IsPROCEDURE') = 1)
  DROP PROCEDURE {databaseOwner}{objectQualifier}DeleteVocabulary
GO

CREATE PROCEDURE {databaseOwner}[{objectQualifier}DeleteVocabulary] 
	@VocabularyID			int
AS
	DELETE FROM {databaseOwner}{objectQualifier}Taxonomy_Vocabularies
	WHERE VocabularyID = @VocabularyID

GO

/************************************************************/
/*****              SqlDataProvider                     *****/
/************************************************************/
