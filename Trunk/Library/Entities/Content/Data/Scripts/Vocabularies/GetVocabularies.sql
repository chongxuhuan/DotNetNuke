/************************************************************/
/*****              SqlDataProvider                     *****/
/*****                                                  *****/
/*****                                                  *****/
/***** Note: To manually execute this script you must   *****/
/*****       perform a search and replace operation     *****/
/*****       for {databaseOwner} and {objectQualifier}  *****/
/*****                                                  *****/
/************************************************************/

/* Add GetVocabularies Procedure */
/*********************************/

IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'{databaseOwner}[{objectQualifier}GetVocabularies]') AND OBJECTPROPERTY(id, N'IsPROCEDURE') = 1)
  DROP PROCEDURE {databaseOwner}{objectQualifier}GetVocabularies
GO

CREATE PROCEDURE {databaseOwner}[{objectQualifier}GetVocabularies] 
AS
	SELECT *
		FROM {databaseOwner}{objectQualifier}Taxonomy_Vocabularies
GO

/************************************************************/
/*****              SqlDataProvider                     *****/
/************************************************************/
