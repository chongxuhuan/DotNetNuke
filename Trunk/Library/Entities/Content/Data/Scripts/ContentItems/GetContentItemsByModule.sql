/************************************************************/
/*****              SqlDataProvider                     *****/
/*****                                                  *****/
/*****                                                  *****/
/***** Note: To manually execute this script you must   *****/
/*****       perform a search and replace operation     *****/
/*****       for {databaseOwner} and {objectQualifier}  *****/
/*****                                                  *****/
/************************************************************/

/* Add GetContentItemsByModule Procedure */
/*****************************************/

IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'{databaseOwner}[{objectQualifier}GetContentItemsByModule]') AND OBJECTPROPERTY(id, N'IsPROCEDURE') = 1)
  DROP PROCEDURE {databaseOwner}{objectQualifier}GetContentItemsByModule
GO

CREATE PROCEDURE {databaseOwner}[{objectQualifier}GetContentItemsByModule] 
	@ModuleID			int
AS
	SELECT *
	FROM {databaseOwner}{objectQualifier}ContentItems
	WHERE ModuleID = @ModuleID
GO

/************************************************************/
/*****              SqlDataProvider                     *****/
/************************************************************/
