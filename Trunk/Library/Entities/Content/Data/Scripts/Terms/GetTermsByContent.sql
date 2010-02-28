/************************************************************/
/*****              SqlDataProvider                     *****/
/*****                                                  *****/
/*****                                                  *****/
/***** Note: To manually execute this script you must   *****/
/*****       perform a search and replace operation     *****/
/*****       for {databaseOwner} and {objectQualifier}  *****/
/*****                                                  *****/
/************************************************************/

/* Add GetTermsByContent Procedure */
/***********************************/

IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'{databaseOwner}[{objectQualifier}GetTermsByContent]') AND OBJECTPROPERTY(id, N'IsPROCEDURE') = 1)
  DROP PROCEDURE {databaseOwner}{objectQualifier}GetTermsByContent
GO

CREATE PROCEDURE {databaseOwner}[{objectQualifier}GetTermsByContent] 
	@ContentItemID			int
AS
	SELECT T.*
	FROM {databaseOwner}{objectQualifier}ContentItems_Tags TG
		INNER JOIN {databaseOwner}{objectQualifier}Taxonomy_Terms T ON TG.TermID = T.TermID
	WHERE TG.ContentItemID = @ContentItemID
GO

/************************************************************/
/*****              SqlDataProvider                     *****/
/************************************************************/
