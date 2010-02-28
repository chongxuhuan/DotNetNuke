/************************************************************/
/*****              SqlDataProvider                     *****/
/*****                                                  *****/
/*****                                                  *****/
/***** Note: To manually execute this script you must   *****/
/*****       perform a search and replace operation     *****/
/*****       for {databaseOwner} and {objectQualifier}  *****/
/*****                                                  *****/
/************************************************************/

/* Add DeleteContentItem Procedure */
/***********************************/

IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'{databaseOwner}[{objectQualifier}DeleteContentItem]') AND OBJECTPROPERTY(id, N'IsPROCEDURE') = 1)
  DROP PROCEDURE {databaseOwner}{objectQualifier}DeleteContentItem
GO

CREATE PROCEDURE {databaseOwner}[{objectQualifier}DeleteContentItem] 
	@ContentItemId			int
AS
	DELETE FROM {databaseOwner}{objectQualifier}ContentItems
	WHERE ContentItemId = @ContentItemId

GO

/************************************************************/
/*****              SqlDataProvider                     *****/
/************************************************************/
