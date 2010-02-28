/************************************************************/
/*****              SqlDataProvider                     *****/
/*****                                                  *****/
/*****                                                  *****/
/***** Note: To manually execute this script you must   *****/
/*****       perform a search and replace operation     *****/
/*****       for {databaseOwner} and {objectQualifier}  *****/
/*****                                                  *****/
/************************************************************/

/* Add UpdateContentItem Procedure */
/***********************************/

IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'{databaseOwner}[{objectQualifier}UpdateContentItem]') AND OBJECTPROPERTY(id, N'IsPROCEDURE') = 1)
  DROP PROCEDURE {databaseOwner}{objectQualifier}UpdateContentItem
GO

CREATE PROCEDURE {databaseOwner}[{objectQualifier}UpdateContentItem] 
	@ContentItemID			int,
	@Content				nvarchar(max),
	@ContentTypeID			int,
	@TabID					int,
	@ModuleID				int, 
	@ContentKey				nvarchar(250),
	@Indexed				bit,
	@LastModifiedByUserID	int
AS
	UPDATE {databaseOwner}{objectQualifier}ContentItems 
		SET 
			Content = @Content,
			ContentTypeID = @ContentTypeID,
			TabID = @TabID,
			ModuleID = @ModuleID,
			ContentKey = @ContentKey,
			Indexed = @Indexed,
			LastModifiedByUserID = @LastModifiedByUserID,
			LastModifiedOnDate = getdate()
	WHERE ContentItemID = @ContentItemID
		
GO

/************************************************************/
/*****              SqlDataProvider                     *****/
/************************************************************/
