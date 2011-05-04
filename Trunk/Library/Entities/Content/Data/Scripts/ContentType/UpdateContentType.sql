/************************************************************/
/*****              SqlDataProvider                     *****/
/*****                                                  *****/
/*****                                                  *****/
/***** Note: To manually execute this script you must   *****/
/*****       perform a search and replace operation     *****/
/*****       for {databaseOwner} and {objectQualifier}  *****/
/*****                                                  *****/
/************************************************************/

/* Add UpdateContentType Procedure */
/***********************************/

IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'{databaseOwner}[{objectQualifier}UpdateContentType]') AND OBJECTPROPERTY(id, N'IsPROCEDURE') = 1)
  DROP PROCEDURE {databaseOwner}{objectQualifier}UpdateContentType
GO

CREATE PROCEDURE {databaseOwner}[{objectQualifier}UpdateContentType] 
	@ContentTypeId		int,
	@ContentType		nvarchar(250)
AS
	UPDATE {databaseOwner}{objectQualifier}ContentTypes 
		SET 
			ContentType = @ContentType
	WHERE ContentTypeId = @ContentTypeId
		
GO

/************************************************************/
/*****              SqlDataProvider                     *****/
/************************************************************/
