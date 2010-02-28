/************************************************************/
/*****              SqlDataProvider                     *****/
/*****                                                  *****/
/*****                                                  *****/
/***** Note: To manually execute this script you must   *****/
/*****       perform a search and replace operation     *****/
/*****       for {databaseOwner} and {objectQualifier}  *****/
/*****                                                  *****/
/************************************************************/

/* Add GetScopeTypes Procedure */
/********************************/

IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'{databaseOwner}[{objectQualifier}GetScopeTypes]') AND OBJECTPROPERTY(id, N'IsPROCEDURE') = 1)
  DROP PROCEDURE {databaseOwner}{objectQualifier}GetScopeTypes
GO

CREATE PROCEDURE {databaseOwner}[{objectQualifier}GetScopeTypes] 
AS
	SELECT *
	FROM {databaseOwner}{objectQualifier}Taxonomy_ScopeTypes
GO

/************************************************************/
/*****              SqlDataProvider                     *****/
/************************************************************/
