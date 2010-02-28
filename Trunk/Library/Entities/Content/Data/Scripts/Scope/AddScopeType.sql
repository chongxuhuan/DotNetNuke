/************************************************************/
/*****              SqlDataProvider                     *****/
/*****                                                  *****/
/*****                                                  *****/
/***** Note: To manually execute this script you must   *****/
/*****       perform a search and replace operation     *****/
/*****       for {databaseOwner} and {objectQualifier}  *****/
/*****                                                  *****/
/************************************************************/

/* Add AddScopeType Procedure */
/******************************/

IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'{databaseOwner}[{objectQualifier}AddScopeType]') AND OBJECTPROPERTY(id, N'IsPROCEDURE') = 1)
  DROP PROCEDURE {databaseOwner}{objectQualifier}AddScopeType
GO

CREATE PROCEDURE {databaseOwner}[{objectQualifier}AddScopeType] 
	@ScopeType			nvarchar(250)
AS
	INSERT INTO {databaseOwner}{objectQualifier}Taxonomy_ScopeTypes (
		ScopeType
	)

	VALUES (
		@ScopeType
	)

	SELECT SCOPE_IDENTITY()
GO

/************************************************************/
/*****              SqlDataProvider                     *****/
/************************************************************/
