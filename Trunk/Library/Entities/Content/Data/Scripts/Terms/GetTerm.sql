/************************************************************/
/*****              SqlDataProvider                     *****/
/*****                                                  *****/
/*****                                                  *****/
/***** Note: To manually execute this script you must   *****/
/*****       perform a search and replace operation     *****/
/*****       for {databaseOwner} and {objectQualifier}  *****/
/*****                                                  *****/
/************************************************************/

/* Add GetTerm Procedure */
/********************************/

IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'{databaseOwner}[{objectQualifier}GetTerm]') AND OBJECTPROPERTY(id, N'IsPROCEDURE') = 1)
  DROP PROCEDURE {databaseOwner}{objectQualifier}GetTerm
GO

CREATE PROCEDURE {databaseOwner}[{objectQualifier}GetTerm] 
	@TermId			int
AS
	SELECT *
	FROM {databaseOwner}{objectQualifier}Taxonomy_Terms
	WHERE TermId = @TermId
GO

/************************************************************/
/*****              SqlDataProvider                     *****/
/************************************************************/
