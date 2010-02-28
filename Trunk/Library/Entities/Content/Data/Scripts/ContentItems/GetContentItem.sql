/************************************************************/
/*****              SqlDataProvider                     *****/
/*****                                                  *****/
/*****                                                  *****/
/***** Note: To manually execute this script you must   *****/
/*****       perform a search and replace operation     *****/
/*****       for {databaseOwner} and {objectQualifier}  *****/
/*****                                                  *****/
/************************************************************/

/* Add GetContentItem Procedure */
/********************************/

IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'{databaseOwner}[{objectQualifier}GetContentItem]') AND OBJECTPROPERTY(id, N'IsPROCEDURE') = 1)
  DROP PROCEDURE {databaseOwner}{objectQualifier}GetContentItem
GO

CREATE PROCEDURE {databaseOwner}[{objectQualifier}GetContentItem] 
	@ContentItemId			int
AS
	SELECT *
	FROM {databaseOwner}{objectQualifier}ContentItems
	WHERE ContentItemId = @ContentItemId
GO

/************************************************************/
/*****              SqlDataProvider                     *****/
/************************************************************/
