/************************************************************/
/*****              SqlDataProvider                     *****/
/*****                                                  *****/
/*****                                                  *****/
/***** Note: To manually execute this script you must   *****/
/*****       perform a search and replace operation     *****/
/*****       for {databaseOwner} and {objectQualifier}  *****/
/*****                                                  *****/
/************************************************************/

/* Add UpdateSimpleTerm Procedure */
/***********************************/

IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'{databaseOwner}[{objectQualifier}UpdateSimpleTerm]') AND OBJECTPROPERTY(id, N'IsPROCEDURE') = 1)
  DROP PROCEDURE {databaseOwner}{objectQualifier}UpdateSimpleTerm
GO

CREATE PROCEDURE {databaseOwner}[{objectQualifier}UpdateSimpleTerm] 
	@TermID					int,
	@VocabularyID			int,
	@Name					nvarchar(250),
	@Description			nvarchar(2500),
	@Weight					int,
	@LastModifiedByUserID	int
AS
	UPDATE {databaseOwner}{objectQualifier}Taxonomy_Terms
		SET 
			VocabularyID = @VocabularyID,
			[Name] = @Name,
			Description = @Description,
			Weight = @Weight,
			LastModifiedByUserID = @LastModifiedByUserID,
			LastModifiedOnDate = getdate()
	WHERE TermID = @TermID
		
GO

/************************************************************/
/*****              SqlDataProvider                     *****/
/************************************************************/
