/************************************************************/
/*****              SqlDataProvider                     *****/
/*****                                                  *****/
/*****                                                  *****/
/***** Note: To manually execute this script you must   *****/
/*****       perform a search and replace operation     *****/
/*****       for {databaseOwner} and {objectQualifier}  *****/
/*****                                                  *****/
/************************************************************/

/* Add AddContentType Procedure */
/********************************/

IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'{databaseOwner}[{objectQualifier}AddContentType]') AND OBJECTPROPERTY(id, N'IsPROCEDURE') = 1)
  DROP PROCEDURE {databaseOwner}{objectQualifier}AddContentType
GO

CREATE PROCEDURE {databaseOwner}[{objectQualifier}AddContentType] 
	@ContentType	nvarchar(250)
AS
	INSERT INTO {databaseOwner}{objectQualifier}ContentTypes (
		ContentType
	)

	VALUES (
		@ContentType
	)

	SELECT SCOPE_IDENTITY()
GO

/************************************************************/
/*****              SqlDataProvider                     *****/
/************************************************************/
