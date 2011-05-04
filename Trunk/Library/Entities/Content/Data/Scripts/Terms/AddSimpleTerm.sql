/************************************************************/
/*****              SqlDataProvider                     *****/
/*****                                                  *****/
/*****                                                  *****/
/***** Note: To manually execute this script you must   *****/
/*****       perform a search and replace operation     *****/
/*****       for {databaseOwner} and {objectQualifier}  *****/
/*****                                                  *****/
/************************************************************/

/* Add AddSimpleTerm Procedure */
/*******************************/

IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'{databaseOwner}[{objectQualifier}AddSimpleTerm]') AND OBJECTPROPERTY(id, N'IsPROCEDURE') = 1)
  DROP PROCEDURE {databaseOwner}{objectQualifier}AddSimpleTerm
GO

CREATE PROCEDURE {databaseOwner}[{objectQualifier}AddSimpleTerm] 
	@VocabularyID		int,
	@Name				nvarchar(250),
	@Description		nvarchar(2500),
	@Weight				int,
	@CreatedByUserID	int
AS
	INSERT INTO {databaseOwner}{objectQualifier}Taxonomy_Terms (
		VocabularyID,
		[Name],
		Description,
		Weight,
		CreatedByUserID,
		CreatedOnDate,
		LastModifiedByUserID,
		LastModifiedOnDate
	)

	VALUES (
		@VocabularyID,
		@Name,
		@Description,
		@Weight,
		@CreatedByUserID,
		getdate(),
		@CreatedByUserID,
		getdate()
	)

	SELECT SCOPE_IDENTITY()
GO

/************************************************************/
/*****              SqlDataProvider                     *****/
/************************************************************/
