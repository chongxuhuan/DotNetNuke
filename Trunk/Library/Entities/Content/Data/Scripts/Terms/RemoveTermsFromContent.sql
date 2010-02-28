/************************************************************/
/*****              SqlDataProvider                     *****/
/*****                                                  *****/
/*****                                                  *****/
/***** Note: To manually execute this script you must   *****/
/*****       perform a search and replace operation     *****/
/*****       for {databaseOwner} and {objectQualifier}  *****/
/*****                                                  *****/
/************************************************************/

/* Add RemoveTermsFromContent Procedure */
/****************************************/

IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'{databaseOwner}[{objectQualifier}RemoveTermsFromContent]') AND OBJECTPROPERTY(id, N'IsPROCEDURE') = 1)
  DROP PROCEDURE {databaseOwner}{objectQualifier}RemoveTermsFromContent
GO

CREATE PROCEDURE {databaseOwner}[{objectQualifier}RemoveTermsFromContent] 
	@ContentItemID	int
AS
	DELETE {databaseOwner}{objectQualifier}ContentItems_Tags 
	WHERE ContentItemID = @ContentItemID
GO

/************************************************************/
/*****              SqlDataProvider                     *****/
/************************************************************/
