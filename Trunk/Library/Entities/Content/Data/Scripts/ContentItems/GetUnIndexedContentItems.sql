/************************************************************/
/*****              SqlDataProvider                     *****/
/*****                                                  *****/
/*****                                                  *****/
/***** Note: To manually execute this script you must   *****/
/*****       perform a search and replace operation     *****/
/*****       for {databaseOwner} and {objectQualifier}  *****/
/*****                                                  *****/
/************************************************************/

/* Add GetUnIndexedContentItems Procedure */
/**************************************/

IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'{databaseOwner}[{objectQualifier}GetUnIndexedContentItems]') AND OBJECTPROPERTY(id, N'IsPROCEDURE') = 1)
  DROP PROCEDURE {databaseOwner}{objectQualifier}GetUnIndexedContentItems
GO

CREATE PROCEDURE {databaseOwner}[{objectQualifier}GetUnIndexedContentItems] 
AS
	SELECT *
	FROM {databaseOwner}{objectQualifier}ContentItems
	WHERE Indexed = 0
GO

/************************************************************/
/*****              SqlDataProvider                     *****/
/************************************************************/
