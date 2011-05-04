/************************************************************/
/*****              SqlDataProvider                     *****/
/*****                                                  *****/
/*****                                                  *****/
/***** Note: To manually execute this script you must   *****/
/*****       perform a search and replace operation     *****/
/*****       for {databaseOwner} and {objectQualifier}  *****/
/*****                                                  *****/
/************************************************************/

/* Add DeleteScopeType Procedure */
/***********************************/

IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'{databaseOwner}[{objectQualifier}DeleteScopeType]') AND OBJECTPROPERTY(id, N'IsPROCEDURE') = 1)
  DROP PROCEDURE {databaseOwner}{objectQualifier}DeleteScopeType
GO

CREATE PROCEDURE {databaseOwner}[{objectQualifier}DeleteScopeType] 
	@ScopeTypeId			int
AS
	DELETE FROM {databaseOwner}{objectQualifier}Taxonomy_ScopeTypes
	WHERE ScopeTypeId = @ScopeTypeId

GO

/************************************************************/
/*****              SqlDataProvider                     *****/
/************************************************************/
