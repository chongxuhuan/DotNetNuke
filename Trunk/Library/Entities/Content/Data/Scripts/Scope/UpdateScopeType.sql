/************************************************************/
/*****              SqlDataProvider                     *****/
/*****                                                  *****/
/*****                                                  *****/
/***** Note: To manually execute this script you must   *****/
/*****       perform a search and replace operation     *****/
/*****       for {databaseOwner} and {objectQualifier}  *****/
/*****                                                  *****/
/************************************************************/

/* Add UpdateScopeType Procedure */
/***********************************/

IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'{databaseOwner}[{objectQualifier}UpdateScopeType]') AND OBJECTPROPERTY(id, N'IsPROCEDURE') = 1)
  DROP PROCEDURE {databaseOwner}{objectQualifier}UpdateScopeType
GO

CREATE PROCEDURE {databaseOwner}[{objectQualifier}UpdateScopeType] 
	@ScopeTypeId				int,
	@ScopeType					nvarchar(250)
AS
	UPDATE {databaseOwner}{objectQualifier}Taxonomy_ScopeTypes 
		SET 
			ScopeType = @ScopeType
	WHERE ScopeTypeId = @ScopeTypeId
		
GO

/************************************************************/
/*****              SqlDataProvider                     *****/
/************************************************************/
