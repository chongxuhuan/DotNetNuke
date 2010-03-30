/************************************************************/
/*****              SqlDataProvider                     *****/
/*****                                                  *****/
/*****                                                  *****/
/***** Note: To manually execute this script you must   *****/
/*****       perform a search and replace operation     *****/
/*****       for {databaseOwner} and {objectQualifier}  *****/
/*****                                                  *****/
/************************************************************/

/* Add GetMetaData Procedure */
/*****************************/

IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'{databaseOwner}[{objectQualifier}GetMetaData]') AND OBJECTPROPERTY(id, N'IsPROCEDURE') = 1)
  DROP PROCEDURE {databaseOwner}{objectQualifier}GetMetaData
GO

CREATE PROCEDURE {databaseOwner}[{objectQualifier}GetMetaData] 
	@ContentItemId			int
AS
	SELECT *
	FROM {databaseOwner}{objectQualifier}ContentItems_MetaData
	WHERE ContentItemId = @ContentItemId
GO

/************************************************************/
/*****              SqlDataProvider                     *****/
/************************************************************/
