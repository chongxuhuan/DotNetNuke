/************************************************************/
/*****              SqlDataProvider                     *****/
/*****                                                  *****/
/*****                                                  *****/
/***** Note: To manually execute this script you must   *****/
/*****       perform a search and replace operation     *****/
/*****       for {databaseOwner} and {objectQualifier}  *****/
/*****                                                  *****/
/************************************************************/

/* Add GetContentItemsByTerm Procedure */
/*****************************************/

IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'{databaseOwner}[{objectQualifier}GetContentItemsByTerm]') AND OBJECTPROPERTY(id, N'IsPROCEDURE') = 1)
  DROP PROCEDURE {databaseOwner}{objectQualifier}GetContentItemsByTerm
GO

CREATE PROCEDURE {databaseOwner}[{objectQualifier}GetContentItemsByTerm] 
	@Term	nvarchar(250)
AS
	DECLARE @TermID			int
	DECLARE @TermLeft		int
	DECLARE @TermRight		int
	DECLARE @VocabularyID	int
	
	SET @TermID = (SELECT TermID FROM {databaseOwner}{objectQualifier}Taxonomy_Terms WHERE Name = @Term)
	SET @TermLeft = (SELECT TermLeft FROM {databaseOwner}{objectQualifier}Taxonomy_Terms WHERE Name = @Term)
	SET @TermRight = (SELECT TermRight FROM {databaseOwner}{objectQualifier}Taxonomy_Terms WHERE Name = @Term)
	SET @VocabularyID = (SELECT VocabularyID FROM {databaseOwner}{objectQualifier}Taxonomy_Terms WHERE Name = @Term)
	
	IF @TermLeft = 0 AND @TermRight = 0
		-- Simple Term
		BEGIN
			SELECT c.*
			FROM {databaseOwner}{objectQualifier}ContentItems As c
				INNER JOIN {databaseOwner}{objectQualifier}ContentItems_Tags ct ON ct.ContentItemID = c.ContentItemID
				INNER JOIN {databaseOwner}{objectQualifier}Taxonomy_Terms t ON t.TermID = ct.TermID
			WHERE t.TermID = @TermID
		END
	ELSE
		BEGIN
		-- Hierarchical Term
			SELECT c.*
			FROM {databaseOwner}{objectQualifier}ContentItems As c
				INNER JOIN {databaseOwner}{objectQualifier}ContentItems_Tags ct ON ct.ContentItemID = c.ContentItemID
				INNER JOIN {databaseOwner}{objectQualifier}Taxonomy_Terms t ON t.TermID = ct.TermID
			WHERE t.TermLeft >= @TermLeft
				AND t.TermRight <= @TermRight
				AND t.VocabularyID = @VocabularyID
		END
GO

/************************************************************/
/*****              SqlDataProvider                     *****/
/************************************************************/
