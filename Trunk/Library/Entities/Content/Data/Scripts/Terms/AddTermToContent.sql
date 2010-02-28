/************************************************************/
/*****              SqlDataProvider                     *****/
/*****                                                  *****/
/*****                                                  *****/
/***** Note: To manually execute this script you must   *****/
/*****       perform a search and replace operation     *****/
/*****       for {databaseOwner} and {objectQualifier}  *****/
/*****                                                  *****/
/************************************************************/

/* Add AddTermToContent Procedure */
/**********************************/

IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'{databaseOwner}[{objectQualifier}AddTermToContent]') AND OBJECTPROPERTY(id, N'IsPROCEDURE') = 1)
  DROP PROCEDURE {databaseOwner}{objectQualifier}AddTermToContent
GO

CREATE PROCEDURE {databaseOwner}[{objectQualifier}AddTermToContent] 
	@TermID			int,
	@ContentItemID	int
AS
	INSERT INTO {databaseOwner}{objectQualifier}ContentItems_Tags (
		TermID,
		ContentItemID
	)

	VALUES (
		@TermID,
		@ContentItemID
	)

	SELECT SCOPE_IDENTITY()
GO

/************************************************************/
/*****              SqlDataProvider                     *****/
/************************************************************/
