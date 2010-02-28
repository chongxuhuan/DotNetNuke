/************************************************************/
/*****              SqlDataProvider                     *****/
/*****                                                  *****/
/*****                                                  *****/
/***** Note: To manually execute this script you must   *****/
/*****       perform a search and replace operation     *****/
/*****       for {databaseOwner} and {objectQualifier}  *****/
/*****                                                  *****/
/************************************************************/

/* Add AddContentItem Procedure */
/********************************/

IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'{databaseOwner}[{objectQualifier}AddContentItem]') AND OBJECTPROPERTY(id, N'IsPROCEDURE') = 1)
  DROP PROCEDURE {databaseOwner}{objectQualifier}AddContentItem
GO

CREATE PROCEDURE {databaseOwner}[{objectQualifier}AddContentItem] 
	@Content			nvarchar(max),
	@ContentTypeID		int,
	@TabID				int,
	@ModuleID			int, 
	@ContentKey			nvarchar(250),
	@Indexed			bit,
	@CreatedByUserID	int
AS
	INSERT INTO {databaseOwner}{objectQualifier}ContentItems (
		Content,
		ContentTypeID,
		TabID,
		ModuleID,
		ContentKey,
		Indexed,
		CreatedByUserID,
		CreatedOnDate,
		LastModifiedByUserID,
		LastModifiedOnDate
	)

	VALUES (
		@Content,
		@ContentTypeID,
		@TabID,
		@ModuleID,
		@ContentKey,
		@Indexed,
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
