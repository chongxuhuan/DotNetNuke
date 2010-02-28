/************************************************************/
/*****              SqlDataProvider                     *****/
/*****                                                  *****/
/*****                                                  *****/
/***** Note: To manually execute this script you must   *****/
/*****       perform a search and replace operation     *****/
/*****       for {databaseOwner} and {objectQualifier}  *****/
/*****                                                  *****/
/************************************************************/

/* Add DeleteSimpleTerm Procedure */
/***********************************/

IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'{databaseOwner}[{objectQualifier}DeleteSimpleTerm]') AND OBJECTPROPERTY(id, N'IsPROCEDURE') = 1)
  DROP PROCEDURE {databaseOwner}{objectQualifier}DeleteSimpleTerm
GO

CREATE PROCEDURE {databaseOwner}[{objectQualifier}DeleteSimpleTerm] 
	@TermId			int
AS
	DELETE FROM {databaseOwner}{objectQualifier}Taxonomy_Terms
	WHERE TermID = @TermID

GO

/************************************************************/
/*****              SqlDataProvider                     *****/
/************************************************************/
