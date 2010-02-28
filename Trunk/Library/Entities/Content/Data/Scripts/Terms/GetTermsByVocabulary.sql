/************************************************************/
/*****              SqlDataProvider                     *****/
/*****                                                  *****/
/*****                                                  *****/
/***** Note: To manually execute this script you must   *****/
/*****       perform a search and replace operation     *****/
/*****       for {databaseOwner} and {objectQualifier}  *****/
/*****                                                  *****/
/************************************************************/

/* Add GetTermsByVocabulary Procedure */
/**************************************/

IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'{databaseOwner}[{objectQualifier}GetTermsByVocabulary]') AND OBJECTPROPERTY(id, N'IsPROCEDURE') = 1)
  DROP PROCEDURE {databaseOwner}{objectQualifier}GetTermsByVocabulary
GO

CREATE PROCEDURE {databaseOwner}[{objectQualifier}GetTermsByVocabulary] 
	@VocabularyID			int
AS
	SELECT *
	FROM {databaseOwner}{objectQualifier}Taxonomy_Terms
	WHERE VocabularyID = @VocabularyID
	ORDER BY TermLeft Asc, Weight Asc, Name Asc
GO

/************************************************************/
/*****              SqlDataProvider                     *****/
/************************************************************/
