/************************************************************/
/*****              SqlDataProvider                     *****/
/*****                                                  *****/
/*****                                                  *****/
/***** Note: To manually execute this script you must   *****/
/*****       perform a search and replace operation     *****/
/*****       for {databaseOwner} and {objectQualifier}  *****/
/*****                                                  *****/
/************************************************************/

/* Add DeleteHeirarchicalTerm Procedure */
/****************************************/

IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'{databaseOwner}[{objectQualifier}DeleteHeirarchicalTerm]') AND OBJECTPROPERTY(id, N'IsPROCEDURE') = 1)
  DROP PROCEDURE {databaseOwner}{objectQualifier}DeleteHeirarchicalTerm
GO

CREATE PROCEDURE {databaseOwner}[{objectQualifier}DeleteHeirarchicalTerm] 
	@TermId			int
AS

	DECLARE @Left			int
	DECLARE @Right			int
	DECLARE @VocabularyID	int
	DECLARE @Width			int
	
	SET @Left = (SELECT TermLeft FROM {databaseOwner}{objectQualifier}Taxonomy_Terms WHERE TermID = @TermID)
	SET @Right = (SELECT TermRight FROM {databaseOwner}{objectQualifier}Taxonomy_Terms WHERE TermID = @TermID)
	SET @VocabularyID = (SELECT VocabularyID FROM {databaseOwner}{objectQualifier}Taxonomy_Terms WHERE TermID = @TermID)
	SET @Width = @Right - @Left + 1
	

	BEGIN TRANSACTION
		-- Delete term(s)
		DELETE FROM {databaseOwner}{objectQualifier}Taxonomy_Terms
		WHERE TermLeft > = @Left AND TermLeft > 0 
			AND TermRight <= @Right AND TermRight > 0
				AND VocabularyID = @VocabularyID

		IF @@ERROR = 0
			BEGIN
				-- Update Left values for all items that are after deleted term
				UPDATE {databaseOwner}{objectQualifier}Taxonomy_Terms 
					SET TermLeft = TermLeft - @Width 
					WHERE TermLeft >= @Left + @Width
						AND VocabularyID = @VocabularyID

				IF @@ERROR = 0
					BEGIN
					-- Update Right values for all items that are after deleted term
						UPDATE {databaseOwner}{objectQualifier}Taxonomy_Terms 
							SET TermRight = TermRight - @Width 
							WHERE TermRight >= @Right
								AND VocabularyID = @VocabularyID

							IF @@ERROR = 0
								BEGIN
									COMMIT TRANSACTION
								END
							ELSE
								BEGIN
									-- Rollback the transaction
									ROLLBACK TRANSACTION		
								END
						END
				ELSE
					BEGIN
						-- Rollback the transaction
						ROLLBACK TRANSACTION
					END
			END
		ELSE
			BEGIN
				-- Rollback the transaction
				ROLLBACK TRANSACTION		
			END
GO

/************************************************************/
/*****              SqlDataProvider                     *****/
/************************************************************/
