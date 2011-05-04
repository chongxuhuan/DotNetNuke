/************************************************************/
/*****              SqlDataProvider                     *****/
/*****                                                  *****/
/*****                                                  *****/
/***** Note: To manually execute this script you must   *****/
/*****       perform a search and replace operation     *****/
/*****       for {databaseOwner} and {objectQualifier}  *****/
/*****                                                  *****/
/************************************************************/

/* Add GetContentTypes Procedure */
/********************************/

IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'{databaseOwner}[{objectQualifier}GetContentTypes]') AND OBJECTPROPERTY(id, N'IsPROCEDURE') = 1)
  DROP PROCEDURE {databaseOwner}{objectQualifier}GetContentTypes
GO

CREATE PROCEDURE {databaseOwner}[{objectQualifier}GetContentTypes] 
AS
	SELECT *
	FROM {databaseOwner}{objectQualifier}ContentTypes
GO

/************************************************************/
/*****              SqlDataProvider                     *****/
/************************************************************/
