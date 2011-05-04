/************************************************************/
/*****              SqlDataProvider                     *****/
/*****                                                  *****/
/*****                                                  *****/
/***** Note: To manually execute this script you must   *****/
/*****       perform a search and replace operation     *****/
/*****       for {databaseOwner} and {objectQualifier}  *****/
/*****                                                  *****/
/************************************************************/

/* Add DeleteContentType Procedure */
/***********************************/

IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'{databaseOwner}[{objectQualifier}DeleteContentType]') AND OBJECTPROPERTY(id, N'IsPROCEDURE') = 1)
  DROP PROCEDURE {databaseOwner}{objectQualifier}DeleteContentType
GO

CREATE PROCEDURE {databaseOwner}[{objectQualifier}DeleteContentType] 
	@ContentTypeId	int
AS
	DELETE FROM {databaseOwner}{objectQualifier}ContentTypes
	WHERE ContentTypeId = @ContentTypeId

GO

/************************************************************/
/*****              SqlDataProvider                     *****/
/************************************************************/
